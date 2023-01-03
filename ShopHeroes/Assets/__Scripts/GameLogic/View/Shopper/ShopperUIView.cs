using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mosframe;
using DG.Tweening;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class ShopperUIView : ViewBase<ShopperDealUIComp>
{
    public override string viewID => ViewPrefabName.ShopperUI;
    int timeuid;
    float grayProgress = 0.66f;
    int countdownTimerId;
    TweenerCore<int, int, NoOptions> priceTxTween;
    int currparse = 0;


    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.noSetting;
        isShowResPanel = true;
        contentPane.suggestItemList.itemRenderer = this.listitemRenderer;
        contentPane.suggestItemList.scrollByItemIndex(0);
        //contentPane.suggestItemList.totalItemCount = 0;
        contentPane.bgBtn.onClick.AddListener(() =>
        {
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;
            D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            hide();
        });
        contentPane.tip_infoTip.onClick.AddListener(() => { contentPane.tip_infoTip.gameObject.SetActive(false); });

        contentPane.tip_awardTip.onClick.AddListener(() => { contentPane.tip_awardTip.gameObject.SetActive(false); });

        contentPane.headTipBtn.onClick.AddListener(() =>
        {
            //点击头顶标签 现实详情
            showTipInfo();
        });
        contentPane.tip_infoTip.gameObject.SetActive(false);

        contentPane.lastShopperBtn.ButtonClickTween(() =>
        {
            //前一个
            AudioManager.inst.PlaySound(12);
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CHANGE_LAST, currShopperUId);
        });
        contentPane.nextShopperBtn.ButtonClickTween(() =>
        {
            //下一个
            AudioManager.inst.PlaySound(12);
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CHANGE_NEXT, currShopperUId);
        });

        contentPane.discountBtn.onClick.AddListener(() =>
        {
            //打折操作
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_START_DISCOUNT, currShopperUId);
        });

        contentPane.doubleBtn.onClick.AddListener(() =>
        {
            //涨价操作
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_START_DOUBLE, currShopperUId);
        });
        contentPane.chatBtn.onClick.AddListener(() =>
        {
            //聊天
            //AudioManager.inst.PlaySound(65);
            contentPane.chatTLImage.gameObject.SetActiveFalse();
            int uid = currShopperUId;

            //int timerId = GameTimer.inst.AddTimer(0.2f, 1, () =>
            //{
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_START_CHAT, uid);
            //});

            contentPane.chatBtn.transform.DORotate(new Vector3(1800, 0, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).OnStart(() =>
            {
                //聊天动画期间 不允许 拒绝 售卖 补货 关界面 切换

                // contentPane.lastShopperBtn.interactable = false;
                // contentPane.nextShopperBtn.interactable = false;
                // contentPane.refuseBtn.interactable = false;
                // contentPane.checkoutBtn.interactable = false;
                // contentPane.stockBtn.interactable = false;
                // contentPane.bgBtn.interactable = false;
                // contentPane.cancelBtn.interactable = false;
                // contentPane.chatBtn.interactable = false;

                //FGUI.inst.showGlobalMask(0.5f);

            }).OnComplete(() =>
            {
                // contentPane.lastShopperBtn.interactable = true;
                // contentPane.nextShopperBtn.interactable = true;
                // contentPane.refuseBtn.interactable = true;
                // contentPane.checkoutBtn.interactable = true;
                // contentPane.stockBtn.interactable = true;
                // contentPane.bgBtn.interactable = true;
                // contentPane.cancelBtn.interactable = true;
                // contentPane.chatBtn.interactable = true;
                // GameTimer.inst.RemoveTimer(timerId);
            });
        });
        contentPane.refuseBtn.ButtonClickTween(() =>
        {
            //要在这里做拒绝后顾客的相关表现
            Shopper _lastShopper = IndoorRoleSystem.inst.GetShopperByUid(currShopperUId);
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_HIDEBUBBLE, currShopperUId);
            _lastShopper?.ByRefuseBehavior();

            //拒绝
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, currShopperUId, false);
        });
        contentPane.checkoutBtn.ButtonClickTween(onCheckoutBtnClick);
        contentPane.cancelBtn.ButtonClickTween(() =>
        {
            //取消
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CANCEL, currShopperUId);
        });

        contentPane.stockBtn.ButtonClickTween(() =>
        {
            ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);
            EquipQualityConfig seqcfg = EquipConfigManager.inst.GetEquipQualityConfig(sData.data.targetEquipId);
            EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_Required, seqcfg.equip_id, seqcfg.quality, sData.data.targetCount, sData.data.shopperType == (int)EShopperType.Warrior);
        });
        contentPane.closeSuggestInventoryBtn.ButtonClickTween(() =>
        {
            contentPane.suggestBgImg.enabled = false;
            contentPane.uiAnimator.Play("hide");
        });
        contentPane.SuggestBtn.ButtonClickTween(() =>
        {
            OnSuggestItemClick(currSuggestEquipItemUId);
        });
        contentPane.OtherSuggestBtn.ButtonClickTween(() =>
        {
            if (currOnShelfEquips.Count > 1)
            {
                showSuggestList();
            }
        });


        contentPane.batch_awardBtn.ButtonClickTween(onBatchAwardBtnClick);

    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(29);
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI);
        HotfixBridge.inst.TriggerLuaEvent("HideUI_GuideTask");
        timeuid = GameTimer.inst.AddTimer(30, () =>
        {
            contentPane.UI_Animation.DORestart();
        });

        FGUI.inst.showGlobalMask(0.3f);
    }

    //
    int currShopperUId;
    private List<EquipItem> currOnShelfEquips;
    private string currSuggestEquipItemUId = null;

    protected override void onHide()
    {
        base.onHide();
        isShowing = false;
        GameTimer.inst.RemoveTimer(timeuid);
        clearCountdownTimer();

        //顾客停止讨价还价
        if (currShopperUId != 0) EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_StopBargaining, currShopperUId);
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTIMERRESET, true);
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, false);
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_GuideTask");

        currShopperUId = 0;
        currparse = 0;
        currOnShelfEquips = null;
        currSuggestEquipItemUId = null;
        contentPane.tip_infoTip.gameObject.SetActive(false);
        contentPane.tip_awardTip.gameObject.SetActive(false);
        contentPane.suggestInventory.gameObject.SetActive(false);
        contentPane.suggestInventory.anchoredPosition = Vector2.down * 1800f;

        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_SHOWALLANIM);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        (contentPane.SuggestBtn.transform as RectTransform).DOAnchorPos3DX(34, 0.2f).From(-800f).SetEase(contentPane.buttonCurve);
        (contentPane.chatBtn.transform as RectTransform).DOAnchorPos3DX(34, 0.2f).From(-800f).SetEase(contentPane.buttonCurve);
        (contentPane.refuseBtn.transform as RectTransform).DOAnchorPos3DX(34, 0.2f).From(-800f).SetEase(contentPane.buttonCurve).SetDelay(0.1f);
        (contentPane.cancelBtn.transform as RectTransform).DOAnchorPos3DX(0, 0.2f).From(-800f).SetEase(contentPane.buttonCurve).SetDelay(0.1f);

        (contentPane.discountBtn.transform as RectTransform).DOAnchorPos3DX(-34, 0.2f).From(800f).SetEase(contentPane.buttonCurve);
        (contentPane.doubleBtn.transform as RectTransform).DOAnchorPos3DX(-34, 0.2f).From(800f).SetEase(contentPane.buttonCurve);
        (contentPane.checkoutBtn.transform as RectTransform).DOAnchorPos3DX(-34, 0.2f).From(800f).SetEase(contentPane.buttonCurve).SetDelay(0.1f);
        (contentPane.stockBtn.transform as RectTransform).DOAnchorPos3DX(-34, 0.2f).From(800f).SetEase(contentPane.buttonCurve).SetDelay(0.1f);

        contentPane.topAnimator.CrossFade("show", 0f);
        contentPane.topAnimator.Update(0f);
        contentPane.topAnimator.Play("show");
        //contentPane.lastShopperBtn.GetComponent<Graphic>().FadeFromTransparentTween(1f, 0.5f, 0.1f);
        //contentPane.nextShopperBtn.GetComponent<Graphic>().FadeFromTransparentTween(1f, 0.5f, 0.1f);
    }

    protected override void DoHideAnimation()
    {
        //contentPane.shopperHeadTip.Fade_a_To_0_All(1, 0.2f);
        //contentPane.middleTf.Fade_a_To_0_All(1, 0.2f);

        //GameTimer.inst.AddTimer(0.2f, 1, () =>
        //{
        //    if (contentPane == null) return;
        //    contentPane.topAnimator.CrossFade("null", 0f);
        //    contentPane.topAnimator.Update(0f);
        this.HideView();
        //});
    }

    public void UpdateInfo()
    {
        if (currShopperUId != 0)
        {
            setShopperInfo(currShopperUId);
        }
    }

    public void UpdateInfo(int shopperuid)
    {
        if (currShopperUId == shopperuid)
        {
            SetShopperData(shopperuid);
        }
    }

    public void showTipInfo()
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);
        if (sData.data.targetEquipId > 0)
        {
            contentPane.tip_infoTip.gameObject.SetActive(true);
            EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(sData.data.targetEquipId);
            if (equipConfig == null)
            {
                contentPane.tip_infoTip.gameObject.SetActive(false);
                return;
            }
            contentPane.tip_nameTx.text = LanguageManager.inst.GetValueByKey(equipConfig.quality_name);
            contentPane.tip_nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1]);
            EquipClassification classCfg = EquipConfigManager.inst.GetEquipTypeByID(equipConfig.equipDrawingsConfig.sub_type);
            contentPane.tip_typeicon.SetSprite(classCfg.Atlas, classCfg.icon);
            contentPane.tip_levelTx.text = LanguageManager.inst.GetValueByKey("{0}阶", equipConfig.equipDrawingsConfig.level.ToString());
            contentPane.tip_disTx.text = LanguageManager.inst.GetValueByKey(equipConfig.equipDrawingsConfig.desc);
            contentPane.tip_countTx.text = ItemBagProxy.inst.getEquipNumber(sData.data.targetEquipId).ToString();
        }
    }

    void setBubbleIcon(ShopperData sData)
    {
        //判断当前气泡的颜色 顾客来买-白色 顾客来买-黄色

        bool isShopperBuy = true;

        switch ((EShopperType)sData.data.shopperType)
        {
            case EShopperType.Buy:
            case EShopperType.Warrior:
            case EShopperType.HighPriceBuy:
                isShopperBuy = true;
                break;
            case EShopperType.Sell:
            case EShopperType.Worker:
            case EShopperType.SellCopyItem:
            case EShopperType.SellMultiple:
                isShopperBuy = false;
                break;
        }

        contentPane.tipBgImage.SetSprite("ShopperUI_atlas", isShopperBuy ? "goumai_duihuakuang" : "goumai_duihuakuangh");
        contentPane.tipBgArrowIcon.SetSprite("ShopperUI_atlas", isShopperBuy ? "goumai_duihuakuang1" : "goumai_duihuakuangh1");
    }

    bool checkError(int shopperuid)
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(shopperuid);

        if (sData == null)
        {
            Logger.error("shopperuid error, the shopperuid is : " + shopperuid);
            return true;
        }

        var shopper = IndoorRoleSystem.inst.GetShopperByUid(sData.data.shopperUid);

        if (shopper != null && shopper.GetCurState() == MachineShopperState.leave)
        {
            currShopperUId = 0;
            Logger.error("此顾客已经离开，仍然打开了shopperUI");
            return true;
        }

        return false;
    }

    void resetUIState(int shopperuid)
    {
        contentPane.SuggestBtnTF.gameObject.SetActive(false);
        contentPane.batch_itemNameTx.gameObject.SetActive(false);
        contentPane.equipNameTx.gameObject.SetActive(true);

        contentPane.highPriceIcon.gameObject.SetActive(false);
        contentPane.batch_awardIcon.transform.parent.gameObject.SetActive(false);
        contentPane.batch_awardPriceTx.transform.parent.gameObject.SetActive(false);

        contentPane.countdownObj.SetActive(false);
        contentPane.topNpcObj.SetActive(false);
        contentPane.highPriceIconObj.SetActive(false);
        contentPane.discountsPriceObj.SetActive(false);

        ShopperData sData = ShopperDataProxy.inst.GetShopperData(shopperuid);

        if (sData.data.shopperType != (int)EShopperType.Buy && sData.data.shopperType != (int)EShopperType.Sell)
        {
            contentPane.lastShopperBtn.gameObject.SetActiveFalse();
            contentPane.nextShopperBtn.gameObject.SetActiveFalse();
        }
        else
        {
            List<int> ids = ShopperDataProxy.inst.GetShopperIdList();

            contentPane.lastShopperBtn.gameObject.SetActive(ids.Count > 1);
            contentPane.nextShopperBtn.gameObject.SetActive(ids.Count > 1);
        }
    }

    void setRoleAction(int shopperuid)
    {
        bool isNotChanged = currShopperUId == shopperuid;

        //店主看向顾客
        Vector3 pos = IndoorRoleSystem.inst.GetShopperByUid(shopperuid).transform.position;
        //IndoorMapEditSys.inst.Shopkeeper.FaceToShopper(pos);
        HotfixBridge.inst.TriggerLuaEvent("FACETOSHOPPER", pos);

        //顾客讨价还价
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_Bargaining, shopperuid);

        if (!isNotChanged) EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_StopBargaining, currShopperUId);

        currShopperUId = shopperuid;

        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_SETREFUSEUID, currShopperUId);

        setGoldInfo(shopperuid, isNotChanged);

    }

    void setGoldInfo(int shopperuid, bool isNotChanged)
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(shopperuid);

        if (priceTxTween != null)
        {
            priceTxTween.Kill();
            priceTxTween = null;
        }
        contentPane.TipBgAnimator.SetBool("show", false);

        if (sData.data.price > 0)
        {

            if (sData.data.hasDisCount == 0 && sData.data.hasDouble == 0)
            {
                contentPane.equipPriceTx.color = sData.data.shopperType == (int)EShopperType.HighPriceBuy ? Color.white : GUIHelper.GetColorByColorHex("FFD200");
            }
            else
            {
                contentPane.equipPriceTx.color = (sData.data.hasDouble == 1 && (sData.data.shopperType == (int)EShopperType.Buy || sData.data.shopperType == (int)EShopperType.HighPriceBuy)) || (sData.data.hasDisCount == 1 && sData.data.shopperType == (int)EShopperType.Sell) ? GUIHelper.GetColorByColorHex("54F942") : GUIHelper.GetColorByColorHex("FF586C");
            }

            int price = sData.data.price;
            if (isNotChanged && currparse != price)
            {
                priceTxTween = DOTween.To(() => currparse, x => currparse = x, price, 0.6f);
                priceTxTween.SetEase(Ease.InQuad);
                priceTxTween.onUpdate = () => contentPane.equipPriceTx.text = currparse.ToString("N0");
                priceTxTween.OnStart(() =>
                {
                    contentPane.TipBgAnimator.SetBool("show", true);
                });
                priceTxTween.OnComplete(() =>
                {
                    contentPane.TipBgAnimator.SetBool("show", false);
                });
            }
            else
            {
                //if (priceTxTween != null)
                //{
                //    priceTxTween.Kill();
                //    priceTxTween = null;
                //}

                contentPane.equipPriceTx.text = price.ToString("N0");
                currparse = price;
            }
        }
        else
        {
            contentPane.equipPriceTx.text = LanguageManager.inst.GetValueByKey("免费");
            contentPane.equipPriceTx.color = Color.green;
        }

        contentPane.targetCountTx.text = sData.data.targetCount > 1 ? "x" + sData.data.targetCount : "";


        //金币状态
        contentPane.goldState[0].gameObject.SetActive(sData.data.hasDisCount == 0 && sData.data.hasDouble == 0 && sData.data.shopperType != (int)EShopperType.Warrior && sData.data.shopperType != (int)EShopperType.HighPriceBuy);
        contentPane.goldState[1].gameObject.SetActive(((sData.data.hasDouble == 1 && sData.data.shopperType == (int)EShopperType.Buy) || (sData.data.hasDisCount == 1 && sData.data.shopperType == (int)EShopperType.Sell)));
        contentPane.goldState[2].gameObject.SetActive(((sData.data.hasDisCount == 1 && sData.data.shopperType == (int)EShopperType.Buy) || (sData.data.hasDouble == 1 && sData.data.shopperType == (int)EShopperType.Sell)));

    }

    void setEnergyInfo(int shopperuid)
    {
        //交易能量
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(shopperuid);

        var counterLv = UserDataProxy.inst.GetCounter().level;
        CounterUpgradeConfig countercfg = CounterUpgradeConfigManager.inst.getConfig(counterLv);
        int energy = countercfg.energy * ((sData.data.shopperType == (int)EShopperType.Warrior || sData.data.shopperType == (int)EShopperType.SellMultiple) ? sData.data.targetCount : 1);
        var energyBuff = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.sell_energyUp);
        if (energyBuff != null)
        {
            energy += Mathf.CeilToInt(energy * (energyBuff.buffInfo.buffParam / 100f));
        }
        contentPane.checkoutTLText.text = $"+{energy}";
    }


    void setCommonTopInfo(ShopperData sData)
    {
        HeroProfessionConfigData herocfg = HeroProfessionConfigManager.inst.GetConfig(sData.data.shopperId);

        if (herocfg != null)
        {
            contentPane.tipWorkerIcon.gameObject.SetActiveFalse();
            contentPane.tipHeroBgIcon.gameObject.SetActiveTrue();
            contentPane.tipHeroBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[herocfg.type - 1]);
            contentPane.tipHeroProfessionIcon.SetSprite(herocfg.atlas, herocfg.ocp_icon);
            contentPane.tipShopperDisTx.text = LanguageManager.inst.GetValueByKey(sData.data.shopperType == (int)EShopperType.Buy ? "{0}级{1}想要" : "{0}级{1}想卖", sData.data.shopperLevel.ToString(), LanguageManager.inst.GetValueByKey(herocfg.name));
        }
        else
        {
            contentPane.tipWorkerIcon.gameObject.SetActiveTrue();
            contentPane.tipHeroBgIcon.gameObject.SetActiveFalse();
            ArtisanNPCConfigData npcCfg = ArtisanNPCConfigManager.inst.GetConfig(sData.data.shopperId);

            if (npcCfg != null)
            {
                contentPane.tipShopperDisTx.text = LanguageManager.inst.GetValueByKey("{0}有笔特殊的报价", npcCfg != null ? LanguageManager.inst.GetValueByKey(npcCfg.name) : "");
            }
            else
            {
                Logger.error("买卖顾客，既不是英雄，也不是Npc sData.data.shopperId: " + sData.data.shopperId + " sData.data.shopperGuideTaskId:" + sData.data.shopperGuideTaskId);
            }

        }

    }

    void setCommonDiscountInfo(ShopperData sData, EquipQualityConfig eqcfg)
    {

        //折扣
        contentPane.discountBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.discountBtn.name));

        if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.discountBtn.name))
        {
            var equipcount = ItemBagProxy.inst.getEquipNumber(eqcfg.equip_id, eqcfg.quality);

            contentPane.discountBtnText.text = LanguageManager.inst.GetValueByKey(sData.data.shopperType == (int)EShopperType.Buy ? "折扣" : "多付");
            contentPane.discountArrowUp.SetActive(sData.data.shopperType != (int)EShopperType.Buy);
            contentPane.discountArrowDown.SetActive(sData.data.shopperType == (int)EShopperType.Buy);
            contentPane.discountTLText.text = "+" + eqcfg.discount_energy.ToString();
            contentPane.discountTLImage.gameObject.SetActive(sData.data.hasDisCount == 0 && sData.data.hasDouble == 0);// && (sData.data.shopperType != (int)EShopperType.Buy || equipcount >= sData.data.targetCount)
            contentPane.discountBtn.enabled = sData.data.hasDisCount == 0 && sData.data.hasDouble == 0 && (sData.data.shopperType != (int)EShopperType.Buy || equipcount >= sData.data.targetCount);
            GUIHelper.SetUIGrayColor(contentPane.discountBtn.transform, !(sData.data.hasDisCount == 0 && sData.data.hasDouble == 0 && (sData.data.shopperType != (int)EShopperType.Buy || equipcount >= sData.data.targetCount)) ? grayProgress : 1);
        }
        else
        {
            contentPane.discountBtn.gameObject.SetActive(false);
        }
    }

    void setCommonDoubleInfo(ShopperData sData, EquipQualityConfig eqcfg)
    {
        //加价
        contentPane.doubleBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.doubleBtn.name));
        if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.doubleBtn.name))
        {
            var equipcount = ItemBagProxy.inst.getEquipNumber(eqcfg.equip_id, eqcfg.quality);

            contentPane.doubleBtn.gameObject.SetActive(true);
            contentPane.doubleBtnText.text = LanguageManager.inst.GetValueByKey(sData.data.shopperType == (int)EShopperType.Buy ? "消耗体力加成收益" : "少付");
            contentPane.doubleArrowUp.SetActive(sData.data.shopperType == (int)EShopperType.Buy);
            contentPane.doubleArrowDown.SetActive(sData.data.shopperType != (int)EShopperType.Buy);
            contentPane.doubleTLText.text = "-" + eqcfg.double_energy.ToString();
            contentPane.doubleTLImage.gameObject.SetActive(sData.data.hasDisCount == 0 && sData.data.hasDouble == 0);// && (sData.data.shopperType != (int)EShopperType.Buy || equipcount >= sData.data.targetCount)

            bool isGray = !(sData.data.hasDisCount == 0 && sData.data.hasDouble == 0 && eqcfg.double_energy <= UserDataProxy.inst.playerData.energy && (sData.data.shopperType != (int)EShopperType.Buy || equipcount >= sData.data.targetCount));
            contentPane.doubleBtn.enabled = !isGray;
            GUIHelper.SetUIGrayColor(contentPane.doubleBtn.transform, isGray ? grayProgress : 1);
        }
        else
        {
            contentPane.doubleBtn.gameObject.SetActive(false);
        }
    }

    void setCommonChatInfo(ShopperData sData)
    {
        //闲聊
        contentPane.chatBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.chatBtn.name));
        if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.chatBtn.name))
        {
            contentPane.chatTLImage.gameObject.SetActiveTrue();
            if (sData.data.hasChat == 0)
                contentPane.chatBtnText.text = LanguageManager.inst.GetValueByKey("闲聊");
            else
                contentPane.chatBtnText.text = LanguageManager.inst.GetValueByKey(sData.data.hasChat == 1 ? "成功" : "适得其反");
            contentPane.chatTLImage.gameObject.SetActive(sData.data.hasChat == 0);
            contentPane.chatBtn.enabled = sData.data.hasChat == 0;
            GUIHelper.SetUIGrayColor(contentPane.chatBtn.transform, sData.data.hasChat != 0 ? grayProgress : 1);
        }
        else
        {
            contentPane.chatBtn.gameObject.SetActive(false);
        }
    }

    void setCommonSuggestInfo(ShopperData sData)
    {
        //推荐
        if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.SuggestBtn.name))
        {
            if (sData.data.shopperType == (int)EShopperType.Buy)
            {
                if (sData.data.hasDisCount == 0 && sData.data.hasDouble == 0)
                {
                    SetSuggestList(ItemBagProxy.inst.GetOnShelfEquipItems());
                }
            }
            else
            {
                contentPane.SuggestBtnTF.gameObject.SetActive(false);
            }
        }
        else
        {
            contentPane.SuggestBtnTF.gameObject.SetActive(false);
        }
    }

    void setCommonCheckInfo(ShopperData sData, EquipQualityConfig eqcfg)
    {
        //结算按钮文字
        contentPane.checkoutBtnText.text = LanguageManager.inst.GetValueByKey(sData.data.shopperType == (int)EShopperType.Buy ? "售卖" : "购买");
        contentPane.checkoutIcon.SetSprite("ShopperUI_atlas", sData.data.shopperType == (int)EShopperType.Buy ? "goumai_jiaoyikuang3" : "goumai_jiaoyikuang4");

        var equipcount = ItemBagProxy.inst.getEquipNumber(eqcfg.equip_id, eqcfg.quality);

        if (sData.data.shopperType == (int)EShopperType.Buy)
        {
            int stockMinLevel = WorldParConfigManager.inst.GetConfig(146) == null ? 3 : (int)WorldParConfigManager.inst.GetConfig(146).parameters;
            if (UserDataProxy.inst.playerData.level >= stockMinLevel)
            {
                contentPane.checkoutBtn.gameObject.SetActive(equipcount >= sData.data.targetCount);
                contentPane.stockBtn.gameObject.SetActive(!contentPane.checkoutBtn.gameObject.activeSelf);
            }
            else
            {
                contentPane.checkoutBtn.gameObject.SetActive(true);
                contentPane.stockBtn.gameObject.SetActive(false);
            }
            contentPane.checkoutBtn.enabled = equipcount >= sData.data.targetCount;
            GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, !(equipcount >= sData.data.targetCount) ? grayProgress : 1);
        }
        else
        {
            contentPane.SuggestBtnTF.gameObject.SetActive(false);
            contentPane.checkoutBtn.gameObject.SetActive(true);
            contentPane.checkoutBtn.enabled = sData.data.price <= UserDataProxy.inst.playerData.gold;
            GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, sData.data.price > UserDataProxy.inst.playerData.gold ? grayProgress : 1);
            contentPane.stockBtn.gameObject.SetActive(false);

            int discounts = 100 - Mathf.FloorToInt((float)sData.data.price / (eqcfg.price_gold * sData.data.targetCount) * 100);

            contentPane.discountsPriceObj.SetActive(discounts > 0 && discounts < 100);
            contentPane.discountsPriceTx.text = "<size=32>" + LanguageManager.inst.GetValueByKey("优惠") + "</size>\n" + discounts + "%";
        }
    }

    void setCommonShopperInfo(ShopperData sData)
    {
        setCommonTopInfo(sData);

        EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(sData.data.targetEquipId);
        if (equipConfig == null)
        {
            if (currShopperUId != 0) EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_StopBargaining, currShopperUId);
            currShopperUId = 0;
            return;
        }

        //装备
        EquipDrawingsConfig cfg = equipConfig.equipDrawingsConfig;

        EquipQualityConfig eqcfg = equipConfig.equipQualityConfig;
        contentPane.obj_superEquipSign.SetActive(eqcfg.quality > StaticConstants.SuperEquipBaseQuality);
        contentPane.equipNameTx.text = equipConfig.name;
        contentPane.equipNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[eqcfg.quality - 1]);
        contentPane.equipQualityIcon.gameObject.SetActive(false);
        // contentPane.equipQualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[eqcfg.quality - 1]);
        contentPane.equipLvObj.SetActiveTrue();
        contentPane.equipLvTx.text = cfg.level.ToString();
        var subtypecfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.sub_type);
        contentPane.equipSubTypeIcon.SetSprite(subtypecfg.Atlas, subtypecfg.icon);
        contentPane.equipSubTypeIcon.iconImage.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[eqcfg.quality - 1]);
        contentPane.equipIcon.SetSprite(cfg.atlas, cfg.icon, eqcfg.quality == 1 ? "" : StaticConstants.qualityColor[eqcfg.quality - 1]);


        setCommonDiscountInfo(sData, eqcfg);
        setCommonDoubleInfo(sData, eqcfg);
        setCommonChatInfo(sData);
        setCommonSuggestInfo(sData);
        setCommonCheckInfo(sData, eqcfg);

    }

    void setSpecialBaseInfo(ShopperData sData)
    {

        contentPane.doubleBtn.gameObject.SetActive(false);
        contentPane.discountBtn.gameObject.SetActive(false);
        contentPane.chatBtn.gameObject.SetActive(false);
        contentPane.SuggestBtnTF.gameObject.SetActive(false);

        if (sData.data.targetEquipId > 0)
        {
            //装备
            EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfgByEquipId(sData.data.targetEquipId);
            if (cfg == null)
            {
                if (currShopperUId != 0) EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_StopBargaining, currShopperUId);
                currShopperUId = 0;
                return;
            }
            EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(sData.data.targetEquipId);
            contentPane.equipNameTx.text = LanguageManager.inst.GetValueByKey(cfg.name);
            contentPane.equipNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[eqcfg.quality - 1]);
            contentPane.equipQualityIcon.gameObject.SetActive(false);
            //contentPane.equipQualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[eqcfg.quality - 1]);
            contentPane.equipLvObj.SetActiveTrue();
            contentPane.equipLvTx.text = cfg.level.ToString();
            var subtypecfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.sub_type);
            contentPane.equipSubTypeIcon.SetSprite(subtypecfg.Atlas, subtypecfg.icon);
            contentPane.equipIcon.SetSprite(cfg.atlas, cfg.icon, eqcfg.quality == 1 ? "" : StaticConstants.qualityColor[eqcfg.quality - 1]);
        }
        else
        {
            //卖资源/副本材料

            itemConfig itemcfg = ItemconfigManager.inst.GetConfig(sData.data.targetItemId);

            if (itemcfg == null)
            {
                Logger.error("this shopper error, the shopperuid is : " + sData.data.shopperUid + "    targetItemId :" + sData.data.targetItemId + "      targetEquipId :" + sData.data.targetEquipId);
                return;
            }

            contentPane.equipNameTx.text = LanguageManager.inst.GetValueByKey(itemcfg.name);
            contentPane.equipNameTx.color = Color.white;
            contentPane.equipQualityIcon.gameObject.SetActiveFalse();
            contentPane.equipLvObj.SetActiveFalse();

            contentPane.equipIcon.SetSprite(itemcfg.atlas, itemcfg.icon);
            contentPane.equipSubTypeIcon.gameObject.SetActive(false);
        }

    }

    void setrefuseInfo(ShopperData sData)
    {
        //拒绝
        contentPane.refuseBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.refuseBtn.name));
        if (contentPane.refuseBtn.gameObject.activeSelf)
        {
            contentPane.refuseTLImage.gameObject.SetActive(sData.data.energy != 0);
            contentPane.refuseTLText.text = (sData.data.energy < 0 ? "+" : "-") + Mathf.Abs(sData.data.energy);
            contentPane.refuseBtn.enabled = sData.data.energy <= UserDataProxy.inst.playerData.energy;
            GUIHelper.SetUIGrayColor(contentPane.refuseBtn.transform, !(sData.data.energy <= UserDataProxy.inst.playerData.energy) ? grayProgress : 1);
        }

        contentPane.cancelBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.cancelBtn.name));
    }

    void setWarriorShopperInfo(ShopperData sData)  //批量收购
    {
        contentPane.batch_itemNameTx.text = contentPane.equipNameTx.text;
        contentPane.batch_itemNameTx.gameObject.SetActiveTrue();
        contentPane.equipNameTx.gameObject.SetActiveFalse();

        contentPane.tipWorkerIcon.gameObject.SetActiveFalse();
        contentPane.tipHeroBgIcon.gameObject.SetActiveFalse();

        if (PlayerPrefs.GetInt(AccountDataProxy.inst.account + "_shopperPop" + sData.data.shopperUid, -1) == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_ReShowPopupCheckOut, sData.data.shopperUid);
        }

        //结算按钮文字
        contentPane.checkoutBtnText.text = LanguageManager.inst.GetValueByKey("售卖");
        contentPane.batch_awardIcon.transform.parent.gameObject.SetActive(true);
        contentPane.batch_awardPriceTx.transform.parent.gameObject.SetActive(true);


        if (sData.isAutoLeave)
        {
            contentPane.countdownObj.SetActive(true);
            setCountdownTimer();
        }

        for (int i = 0; i < contentPane.goldState.Length; i++)
        {
            contentPane.goldState[i].gameObject.SetActiveFalse();
        }

        EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(sData.data.targetEquipId);
        contentPane.obj_superEquipSign.SetActive(eqcfg.quality > StaticConstants.SuperEquipBaseQuality);
        contentPane.topNpcObj.SetActive(true);
        ArtisanNPCConfigData npcCfg = ArtisanNPCConfigManager.inst.GetConfig(sData.data.shopperId);
        contentPane.tipShopperDisTx.text = string.Empty;
        contentPane.npcTalkTx.text = LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRanomTalkMsg(5, 99));
        contentPane.npcHeadIcon.SetSprite("portrait_atlas", npcCfg.icon);
        contentPane.npcNameTx.text = LanguageManager.inst.GetValueByKey(npcCfg.name);


        int inventoryCount = ItemBagProxy.inst.getEquipNumberBySuperQuip(eqcfg.id);

        contentPane.checkoutBtn.gameObject.SetActive(inventoryCount >= sData.data.targetCount);
        contentPane.checkoutBtn.enabled = contentPane.checkoutBtn.gameObject.activeSelf;
        contentPane.checkoutIcon.SetSprite("ShopperUI_atlas", "goumai_jiaoyikuang3");
        GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, !contentPane.checkoutBtn.gameObject.activeSelf ? grayProgress : 1);
        contentPane.stockBtn.gameObject.SetActive(!contentPane.checkoutBtn.gameObject.activeSelf);
        //奖励
        itemConfig rewardcfg = ItemconfigManager.inst.GetConfig(sData.data.rewardItemId);
        contentPane.batch_awardIcon.SetSpriteURL(rewardcfg.icon);
        contentPane.batch_awardNameTx.text = LanguageManager.inst.GetValueByKey(rewardcfg.name);
        contentPane.batch_awardNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[rewardcfg.property - 1]);
        contentPane.equipPriceTx.text = string.Empty;
        contentPane.batch_awardCountTx.text = sData.data.rewardItemCount <= 1 ? "" : "x" + sData.data.rewardItemCount;
        contentPane.batch_awardPriceTx.text = AbbreviationUtility.AbbreviateNumber(currparse, 2);
    }

    void setWorkerShopperInfo(ShopperData sData) // 给资源
    {
        contentPane.tipWorkerIcon.gameObject.SetActiveTrue();
        contentPane.tipHeroBgIcon.gameObject.SetActiveFalse();
        contentPane.obj_superEquipSign.SetActive(false);


        var workerCfg = WorkerConfigManager.inst.GetConfig(sData.data.shopperId);

        //结算按钮文字
        contentPane.checkoutBtnText.text = LanguageManager.inst.GetValueByKey("购买");
        contentPane.tipShopperDisTx.text = LanguageManager.inst.GetValueByKey("{0}有笔特殊的报价", LanguageManager.inst.GetValueByKey(workerCfg.name));
        contentPane.checkoutIcon.SetSprite("ShopperUI_atlas", "goumai_jiaoyikuang4");
        contentPane.checkoutBtn.gameObject.SetActive(true);
        contentPane.checkoutBtn.enabled = sData.data.price <= UserDataProxy.inst.playerData.gold;
        GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, sData.data.price > UserDataProxy.inst.playerData.gold ? grayProgress : 1);
        contentPane.stockBtn.gameObject.SetActive(false);
    }

    void setHighPriceBuyShopperInfo(ShopperData sData) // 高价收购
    {
        if (PlayerPrefs.GetInt(AccountDataProxy.inst.account + "_shopperPop" + sData.data.shopperUid, -1) == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_ReShowPopupCheckOut, sData.data.shopperUid);
        }

        contentPane.tipWorkerIcon.gameObject.SetActiveFalse();
        contentPane.tipHeroBgIcon.gameObject.SetActiveFalse();
        contentPane.checkoutBtnText.text = LanguageManager.inst.GetValueByKey("售卖");
        contentPane.checkoutIcon.SetSprite("ShopperUI_atlas", "goumai_jiaoyikuang3");

        contentPane.highPriceIcon.gameObject.SetActive(true);
        contentPane.highPriceArrowUpImg.enabled = sData.data.hasDouble == 1 && sData.data.shopperType == (int)EShopperType.HighPriceBuy;

        EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(sData.data.targetEquipId);
        contentPane.obj_superEquipSign.SetActive(eqcfg.quality > StaticConstants.SuperEquipBaseQuality);

        int basePrice = eqcfg.price_gold;
        var equipData = EquipDataProxy.inst.GetEquipData(eqcfg.equip_id);
        if (equipData != null)
        {
            var eqiupDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(eqcfg.equip_id);

            var addPrice = 0;

            //里程碑
            var val = equipData.sellAddition - 1;
            addPrice = Mathf.RoundToInt(basePrice * val);

            //家具buff

            //小类型
            val = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.sell_subTypePriceUp, eqiupDrawingCfg.sub_type);
            addPrice += Mathf.CeilToInt(basePrice * val);

            //全部类型
            val = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.sell_allPriceUp);
            addPrice += Mathf.CeilToInt(basePrice * val);

            ////
            var price = basePrice + addPrice;

            //豪华度加成
            var luxuryBuff = HotfixBridge.inst.GetLuxuryBuff(equipData.mainType);
            if (luxuryBuff != -1)
            {
                val = luxuryBuff / 100;
                price = price + Mathf.CeilToInt(price * val);
            }
            basePrice = price;

        }

        int highPriceTimes = Mathf.FloorToInt((float)sData.data.price / basePrice);
        contentPane.highPriceIconObj.SetActive(highPriceTimes > 0);
        contentPane.highPriceTimesTx.text = "<size=32>" + LanguageManager.inst.GetValueByKey("价格") + "</size>\n" + "x" + highPriceTimes;

        contentPane.topNpcObj.SetActive(true);
        ArtisanNPCConfigData npcCfg = ArtisanNPCConfigManager.inst.GetConfig(sData.data.shopperId);
        contentPane.tipShopperDisTx.text = string.Empty;
        contentPane.npcTalkTx.text = LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRanomTalkMsg(5, 100));
        contentPane.npcHeadIcon.SetSprite("portrait_atlas", npcCfg.icon);
        contentPane.npcNameTx.text = LanguageManager.inst.GetValueByKey(npcCfg.name);

        int inventoryCount = ItemBagProxy.inst.getEquipNumber(eqcfg.id);
        contentPane.checkoutBtn.gameObject.SetActive(inventoryCount >= sData.data.targetCount);
        contentPane.checkoutBtn.enabled = contentPane.checkoutBtn.gameObject.activeSelf;
        GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, !contentPane.checkoutBtn.gameObject.activeSelf ? grayProgress : 1);
        contentPane.stockBtn.gameObject.SetActive(!contentPane.checkoutBtn.gameObject.activeSelf);
    }

    void setSellCopyItemShopperInfo(ShopperData sData) //出售副本道具
    {
        contentPane.tipWorkerIcon.gameObject.SetActiveFalse();
        contentPane.tipHeroBgIcon.gameObject.SetActiveFalse();
        contentPane.obj_superEquipSign.SetActive(false);

        contentPane.checkoutBtnText.text = LanguageManager.inst.GetValueByKey("购买");
        ArtisanNPCConfigData npcCfg = ArtisanNPCConfigManager.inst.GetConfig(sData.data.shopperId);
        contentPane.tipShopperDisTx.text = LanguageManager.inst.GetValueByKey("{0}有笔特殊的报价", npcCfg != null ? LanguageManager.inst.GetValueByKey(npcCfg.name) : "");
        contentPane.checkoutBtn.gameObject.SetActive(true);
        contentPane.stockBtn.gameObject.SetActive(false);
        contentPane.checkoutBtn.enabled = sData.data.price <= UserDataProxy.inst.playerData.gold;
        GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, sData.data.price > UserDataProxy.inst.playerData.gold ? grayProgress : 1);
    }

    void setSellMultipleShopperInfo(ShopperData sData) //出售X件指定装备
    {
        contentPane.tipWorkerIcon.gameObject.SetActiveFalse();
        contentPane.tipHeroBgIcon.gameObject.SetActiveFalse();

        contentPane.checkoutBtnText.text = LanguageManager.inst.GetValueByKey("购买");
        ArtisanNPCConfigData npcCfg = ArtisanNPCConfigManager.inst.GetConfig(sData.data.shopperId);
        contentPane.tipShopperDisTx.text = LanguageManager.inst.GetValueByKey("{0}有笔特殊的报价", npcCfg != null ? LanguageManager.inst.GetValueByKey(npcCfg.name) : "");


        EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(sData.data.targetEquipId);
        contentPane.obj_superEquipSign.SetActive(eqcfg.quality > StaticConstants.SuperEquipBaseQuality);

        int discounts = 100 - Mathf.FloorToInt((float)sData.data.price / (eqcfg.price_gold * sData.data.targetCount) * 100);
        contentPane.discountsPriceObj.SetActive(discounts > 0 && discounts < 100);
        contentPane.discountsPriceTx.text = "<size=32>" + LanguageManager.inst.GetValueByKey("优惠") + "</size>\n" + discounts + "%";

        contentPane.checkoutBtn.gameObject.SetActive(true);
        contentPane.stockBtn.gameObject.SetActive(false);
        contentPane.checkoutBtn.enabled = sData.data.price <= UserDataProxy.inst.playerData.gold;
        GUIHelper.SetUIGrayColor(contentPane.checkoutBtn.transform, sData.data.price > UserDataProxy.inst.playerData.gold ? grayProgress : 1);
    }

    void setSpecialShopperInfo(ShopperData sData)
    {
        setSpecialBaseInfo(sData);

        switch ((EShopperType)sData.data.shopperType)
        {
            case EShopperType.Warrior: setWarriorShopperInfo(sData); break;
            case EShopperType.Worker: setWorkerShopperInfo(sData); break;
            case EShopperType.HighPriceBuy: setHighPriceBuyShopperInfo(sData); break;
            case EShopperType.SellCopyItem: setSellCopyItemShopperInfo(sData); break;
            case EShopperType.SellMultiple: setSellMultipleShopperInfo(sData); break;
        }


    }

    void setShopperInfo(int shopperuid)
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(shopperuid);

        if (sData.data.shopperType == (int)EShopperType.Buy || sData.data.shopperType == (int)EShopperType.Sell)
        {
            setCommonShopperInfo(sData);
        }
        else  //下面是NPC
        {
            setSpecialShopperInfo(sData);
        }

        setrefuseInfo(sData);
    }

    public void SetShopperDataWithAnim(int shopperuid, bool notSetScale = false)
    {
        if (contentPane.shopperHeadTip != null)
        {
            GameObject topObj = contentPane.shopperHeadTip.gameObject;
            if (!notSetScale)
            {
                GameObject cloneObj = GameObject.Instantiate(topObj, topObj.transform.parent);
                var cloneAnim = cloneObj.GetComponent<Animator>();
                if (cloneAnim != null)
                    cloneAnim.enabled = false;

                cloneObj.transform.DOScale(0, 0.3f).From(0.65f).OnComplete(() =>
                {
                    if (cloneObj != null)
                        GameObject.Destroy(cloneObj);
                });

                topObj.transform.Fade_0_To_a_All(1, 0.5f, 0.05f);
            }
            else
            {
                topObj.transform.Fade_0_To_a_All(1, 0.8f);
            }
        }

        SetShopperData(shopperuid);
    }

    public void SetShopperData(int shopperuid)
    {

        contentPane.suggestBgImg.enabled = false;
        contentPane.suggestInventory.gameObject.SetActive(false);
        contentPane.suggestInventory.anchoredPosition = Vector2.down * 1800f;

        clearCountdownTimer();

        if (checkError(shopperuid))
        {
            return;
        }

        resetUIState(shopperuid);
        setRoleAction(shopperuid);
        setEnergyInfo(shopperuid);
        setBubbleIcon(ShopperDataProxy.inst.GetShopperData(shopperuid));
        setShopperInfo(shopperuid);

    }

    public void SetSuggestList(List<EquipItem> equips)
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);
        if (sData == null || sData.data.shopperType == (int)EShopperType.Sell)
        {
            contentPane.SuggestBtnTF.gameObject.SetActive(false);
            return;
        }
        if (equips == null || equips.Count <= 0)
        {
            contentPane.SuggestBtnTF.gameObject.SetActive(false);
            return;
        }

        currOnShelfEquips = FilterTargetEquip(equips);
        if (currOnShelfEquips.Count > 0)
        {
            //先排个序 按照 消耗能量顺序
            currOnShelfEquips.Sort((item1, item2) => item1.equipConfig.equipQualityConfig.recommend_energy.CompareTo(item2.equipConfig.equipQualityConfig.recommend_energy));
            //筛选能量不足不能推荐的
            var last = currOnShelfEquips.FindAll(item => item.equipConfig.equipQualityConfig.recommend_energy <= UserDataProxy.inst.playerData.energy && !item.isLock);
            if (last.Count > 0)
            {
                //推荐最后一个
                contentPane.SuggestBtnTF.gameObject.SetActive(true);
                contentPane.SuggestBtn.enabled = true;
                GUIHelper.SetUIGrayColor(contentPane.SuggestBtnTF, 1);
                // contentPane.OtherSuggestBtn.enabled = last.Count > 1;
                contentPane.OtherSuggestBtn.gameObject.SetActive(last.Count > 1);
                SetCurrSuggest(last[last.Count - 1]);
            }
            else
            {
                contentPane.SuggestBtnTF.gameObject.SetActive(false);
                contentPane.SuggestBtn.enabled = false;
                GUIHelper.SetUIGrayColor(contentPane.SuggestBtnTF, grayProgress);
                //contentPane.OtherSuggestBtn.enabled = false;
                //SetCurrSuggest(last[0]);
            }
        }
        else
        {
            contentPane.SuggestBtnTF.gameObject.SetActive(false);
            return;
        }
    }

    //筛选列表
    private List<EquipItem> FilterTargetEquip(List<EquipItem> equips)
    {
        List<EquipItem> list = new List<EquipItem>();
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);
        List<int> subtypes = HeroProfessionConfigManager.inst.GetAllFieldEquipId(sData.data.shopperId);
        equips.ForEach(_equip =>
        {
            if (subtypes.IndexOf(_equip.equipConfig.equipDrawingsConfig.sub_type) >= 0)
            {
                if (_equip.equipConfig.equipQualityConfig.id != sData.data.targetEquipId && !_equip.isLock)
                {
                    list.Add(_equip);
                }
            }
        });
        return list;
    }

    //设置一个推荐
    private void SetCurrSuggest(EquipItem equip)
    {
        currSuggestEquipItemUId = equip.itemUid;
        contentPane.SuggestIcon.SetSprite(equip.equipConfig.equipDrawingsConfig.atlas, equip.equipConfig.equipDrawingsConfig.icon, StaticConstants.qualityColor[equip.quality - 1]);
        contentPane.suggest_superEquipSignObj.SetActive(equip.quality > StaticConstants.SuperEquipBaseQuality);
        contentPane.SuggestTLText.text = "-" + equip.equipConfig.equipQualityConfig.recommend_energy.ToString();
    }
    //推荐界面
    int listItemCount = 0;
    private void showSuggestList()
    {
        contentPane.suggestBgImg.enabled = true;
        contentPane.suggestInventory.gameObject.SetActive(true);
        SetListItemTotalCount(currOnShelfEquips.Count);
        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 3; ++i)
        {
            int itemIndex = index * 3 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < currOnShelfEquips.Count)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                SuggestListItem item = itemScript.buttonList[i].GetComponent<SuggestListItem>();
                item.setData(currOnShelfEquips[itemIndex], OnSuggestItemClick);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnSuggestItemClick(string equipUid)
    {
        //替换目标
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CHANGE_TAGETEQUIP, currShopperUId, equipUid);
    }
    public void refreshList()
    {
        contentPane.suggestItemList.refresh();
    }
    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (listItemCount > currOnShelfEquips.Count)
        {
            listItemCount = currOnShelfEquips.Count;
        }
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.suggestItemList.totalItemCount = count1;
    }


    private void clearCountdownTimer()
    {
        if (countdownTimerId != 0)
        {
            GameTimer.inst.RemoveTimer(countdownTimerId);
            countdownTimerId = 0;
        }
    }

    private void setCountdownTimer()
    {
        countdownMethod();
        countdownTimerId = GameTimer.inst.AddTimer(1, countdownMethod);
    }

    private void countdownMethod()
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);
        contentPane.countdownTx.text = TimeUtils.timeSpan3Str(sData.data.leaveTime);
    }

    void onCheckoutBtnClick()
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);

        if (sData.data.shopperType == (int)EShopperType.Warrior)
        {

            List<UseAdvancedOrLockEquipTipsData> datas = new List<UseAdvancedOrLockEquipTipsData>();
            int count = 0;
            var equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(sData.data.targetEquipId);
            int commonQuality = equipconfig.quality;

            var hcount = sData.data.targetCount - count;
            for (int q = commonQuality; q <= 5; q++)
            {
                var equipItem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                if (equipItem != null)
                {
                    count = (int)equipItem.count;
                    if (count > 0)
                    {
                        equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(equipconfig.equip_id, q);

                        if (equipItem.isLock)
                        {
                            datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                        }

                        if (q > commonQuality)
                        {
                            datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                        }

                    }
                    if (count >= hcount)
                    {
                        break;
                    }
                    hcount -= count;
                }
            }

            if (count < hcount)
            {
                for (int q = commonQuality > StaticConstants.SuperEquipBaseQuality ? commonQuality : StaticConstants.SuperEquipBaseQuality + commonQuality; q <= StaticConstants.SuperEquipBaseQuality + 5; q++) //超凡装备
                {
                    var equipItem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);

                    if (equipItem != null)
                    {
                        count = (int)equipItem.count;

                        if (count > 0)
                        {
                            equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(equipconfig.equip_id, q);

                            if (equipItem.isLock)
                            {
                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                            }

                            datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                        }
                        if (count >= hcount)
                        {
                            break;
                        }
                        hcount -= count;
                    }

                }
            }

            if (datas.Count > 0)
            {
                System.Action callback = () =>
                {
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CHECKOUT, currShopperUId);
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_HIDEBUBBLE, currShopperUId);
                };

                //EventController.inst.TriggerEvent(GameEventType.UseAdvancedEquip, callback, needEquips, LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？"));
                HotfixBridge.inst.TriggerLuaEvent("UseAdvancedOrLockEquipTipsEvent.UseAdvancedOrLockEquipFromCS", datas, callback);

                return;
            }
        }
        else if (sData.data.shopperType == (int)EShopperType.Buy) //普通来买的顾客
        {

            var equipItem = ItemBagProxy.inst.GetEquipItem(sData.data.targetEquipId);
            List<UseAdvancedOrLockEquipTipsData> datas = new List<UseAdvancedOrLockEquipTipsData>();

            if (equipItem != null)
            {

                if (equipItem.isLock)
                {
                    datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = sData.data.targetEquipId, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                }

                bool isBetterQuality = equipItem.equipConfig.equipQualityConfig.quality > 2;
                if (isBetterQuality)
                {
                    datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = sData.data.targetEquipId, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                }
            }


            if (datas.Count > 0)
            {
                System.Action callback = () =>
                {
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CHECKOUT, currShopperUId);
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_HIDEBUBBLE, currShopperUId);
                };

                //EventController.inst.TriggerEvent(GameEventType.UseAdvancedEquip, callback, new List<int>() { sData.data.targetEquipId }, LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？"));
                HotfixBridge.inst.TriggerLuaEvent("UseAdvancedOrLockEquipTipsEvent.UseAdvancedOrLockEquipFromCS", datas, callback);

                return;
            }
        }

        //在这里直接关闭他的气泡
        Shopper _lastShopper = IndoorRoleSystem.inst.GetShopperByUid(currShopperUId);
        _lastShopper?.HidePopupCheckout();

        //结算
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_CHECKOUT, currShopperUId);
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_HIDEBUBBLE, currShopperUId);
    }

    void onBatchAwardBtnClick()
    {
        ShopperData sData = ShopperDataProxy.inst.GetShopperData(currShopperUId);

        contentPane.tip_awardTip.gameObject.SetActiveTrue();
        itemConfig config = ItemconfigManager.inst.GetConfig(sData.data.rewardItemId);
        contentPane.awardTipTx.text = LanguageManager.inst.GetValueByKey(config.desc);
    }

}
