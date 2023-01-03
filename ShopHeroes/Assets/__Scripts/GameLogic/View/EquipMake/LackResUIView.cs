using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LackResUIView : ViewBase<LackResUIComp>
{
    public override string viewID => ViewPrefabName.LackResUI;
    public override string sortingLayerName => "popup";
    private ItemType currrestype = ItemType.Material; //普通资源-Material  特殊资源-TaskMaterial 装备配件-Equip
    private int itemId = 0; //当前资源id或者当前装备+品质id
    private int lackCount = 0;
    private int hideItemId = 0;//足够自动关闭的timerId

    private List<needMaterialsInfo> _lackResItems;
    private needMaterialsInfo _curInfo;

    protected override void onInit()
    {
        base.onInit();


        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;

        contentPane.closeButton.onClick.AddListener(hide);

        contentPane.leftBtn.ButtonClickTween(() => turnPageBtnClick(true));
        contentPane.rightBtn.ButtonClickTween(() => turnPageBtnClick(false));

        contentPane.gemBuyButton.onClick.AddListener(onGemBuyBtnClick);
        contentPane.makeButton.onClick.AddListener(onMakeBtnClick);
        contentPane.marketButton.onClick.AddListener(onMarketBtnClick);
        contentPane.ectypalButton.onClick.AddListener(() =>
        {
            hide();
            if (_curInfo.needId == 30051)
            {
                if (HotfixBridge.inst.GetActivity_GoldenCityFlag())
                {
                    HotfixBridge.inst.TriggerLuaEvent("OpenUI_GoldenCityMain", 0);
                }
                else if (!HotfixBridge.inst.GetActivity_GoldenCityFlag() && HotfixBridge.inst.GetActivity_GoldenCityCanRewardCount() > 0)
                {
                    HotfixBridge.inst.TriggerLuaEvent("OpenUI_GoldenCityRewardList", HotfixBridge.inst.GetActivity_GoldenCityCurScoreLv());
                }
            }
            else
            {
                //打开副本界面
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_SHOWUI);
            }
        });

        contentPane.unionBuyButton.onClick.AddListener(onUnionBuyButtonClick);
        contentPane.putResBoxButton.onClick.AddListener(onPutResBoxButtonClick);

    }

    private void onGemBuyBtnClick()
    {

        if (!contentPane.gemConfirmObj.activeSelf)
            contentPane.gemConfirmObj.SetActiveTrue();
        else
        {
            if (lackCount <= 0)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("资源已达上限，无法继续补充"), GUIHelper.GetColorByColorHex("ff2828"));
                return;
            }
            int needGem = DiamondCountUtils.GetEquipMakeMaterialsReFullCost(lackCount);

            if (UserDataProxy.inst.playerData.gem < needGem)
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("ff2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, needGem - UserDataProxy.inst.playerData.gem);
                contentPane.gemConfirmObj.SetActiveFalse();
            }
            else
            {

                if (_curInfo.type == 0 && itemId == _curInfo.needId && ItemBagProxy.inst.resItemCount(itemId) >= _curInfo.needCount)
                {
                    return;
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.BagEvent.Bag_BuyProduction, itemId);
                    contentPane.gemConfirmObj.SetActiveFalse();
                }

            }
        }
    }

    private void onUnionBuyButtonClick()
    {
        if (contentPane.unionBuyCoinTx.color == GUIHelper.GetColorByColorHex("fd4f4f"))
        {
            string tip = UserDataProxy.inst.playerData.hasUnion ? LanguageManager.inst.GetValueByKey("联盟币不足") : LanguageManager.inst.GetValueByKey("请先加入联盟");
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, tip, GUIHelper.GetColorByColorHex("ff2828"));
        }
        else
        {
            if (lackCount <= 0)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("资源已达上限，无法继续补充"), GUIHelper.GetColorByColorHex("ff2828"));
                return;
            }
            if (_curInfo.type == 0 && itemId == _curInfo.needId && ItemBagProxy.inst.resItemCount(itemId) >= _curInfo.needCount)
            {
                return;
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.BagEvent.Bag_BuyProductionByUnoinCoin, itemId);
            }
        }
    }

    private void onPutResBoxButtonClick()
    {
        helpConfig hcfg = GameHelpNavigationConfigManager.inst.GetHelpConfigBytyp(2, itemId);
        if (hcfg != null)
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", hcfg.id);
        }
    }

    private void onMakeBtnClick()
    {
        //制作装备
        var equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(itemId);
        EquipData data = EquipDataProxy.inst.GetEquipData(equipconfig.equip_id);
        if (data == null)
        {
            //未解锁
            //Debug.LogError("未解锁");
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("图纸暂未解锁"), GUIHelper.GetColorByColorHex("ff2828"));
            return;
        }
        else
        {
            //制作
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, equipconfig.equip_id);
        }
    }

    private void onMarketBtnClick()
    {
        EquipConfig equipCfg = null;
        itemConfig itemCfg = null;
        int itemType = 0;
        if (currrestype == ItemType.Equip)
        {
            equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(itemId);
            itemType = 0;

        }
        else if (currrestype == ItemType.TaskMaterial)
        {
            itemCfg = ItemconfigManager.inst.GetConfig(itemId);
            itemType = 1;
        }

        hide();
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_REQUIREDITEM, itemType, itemType == 0 ? equipCfg.equipQualityConfig.id : itemCfg.id, lackCount, false);
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
    }
    protected override void onHide()
    {
        base.onHide();
        contentPane.gemConfirmObj.SetActiveFalse();
        clearUnionCountdownTimer();
        EquipDataProxy.inst.lackResIsShowing = false;
        AudioManager.inst.PlaySound(11);
    }


    private ItemType getItemType(int type)
    {
        if (type == 0)
            return ItemType.Material;
        else if (type == 1)
            return ItemType.Equip;
        else if (type == 2)
            return ItemType.TaskMaterial;

        return ItemType.Material;
    }

    private int getInventoryNum()
    {
        int count = 0;
        if (_curInfo.type == 0)
        {
            count = (int)ItemBagProxy.inst.resItemCount(_curInfo.needId);

            ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
            if (rp != null) lackCount = (int)rp.countLimit - count;
            if (lackCount < 0) lackCount = 0;
        }
        else if (_curInfo.type == 1)
        {
            // EquipQualityConfig eqCfg = EquipConfigManager.inst.GetEquipQualityConfig(_curInfo.needId);
            count = ItemBagProxy.inst.getEquipNumberBySuperQuip(_curInfo.needId);
            lackCount = _curInfo.needCount - count;
        }
        else if (_curInfo.type == 2)
        {
            count = (int)ItemBagProxy.inst.resItemCount(_curInfo.needId);
            lackCount = _curInfo.needCount - count;
        }

        return count;
    }


    public void setData(List<needMaterialsInfo> lackResItems)
    {
        if (lackResItems == null || lackResItems.Count == 0)
        {
            hide();
            return;
        }

        contentPane.leftBtn.gameObject.SetActive(lackResItems.Count > 1);
        contentPane.rightBtn.gameObject.SetActive(lackResItems.Count > 1);

        _lackResItems = lackResItems;
        _curInfo = lackResItems[0];

        setItem(getItemType(_curInfo.type), _curInfo.needId, _curInfo.needCount);

    }



    private void setItem(ItemType type, int id, int needcount)
    {
        contentPane.gemBuyButton.gameObject.SetActive(type == ItemType.Material);
        contentPane.makeButton.gameObject.SetActive(type == ItemType.Equip);
        contentPane.marketButton.gameObject.SetActive(type != ItemType.Material);
        contentPane.ectypalButton.gameObject.SetActive(type == ItemType.TaskMaterial);
        contentPane.unionBuyCountdownObj.gameObject.SetActive(type != ItemType.Equip && type != ItemType.TaskMaterial);
        contentPane.unionBuyButton.gameObject.SetActive(false);
        contentPane.putResBoxButton.gameObject.SetActive(false);
        if (currrestype != type || itemId != id)
        {
            contentPane.gemConfirmObj.SetActiveFalse();
        }
        contentPane.lackResTips_1.enabled = false;
        contentPane.remainCountTx.text = string.Empty;
        currrestype = type;
        itemId = id;
        contentPane.currInventoryTx.text = "<size=56>" + getInventoryNum().ToString() + "</size><color=#bb887c>" + "/" + needcount.ToString() + "</color>";
        contentPane.titleNumTx.text = _lackResItems.IndexOf(_curInfo) + 1 + "/" + _lackResItems.Count;

        RectTransform marketRect = contentPane.marketButton.GetComponent<RectTransform>();
        marketRect.anchoredPosition = new Vector2(-224, marketRect.anchoredPosition.y);

        if (currrestype == ItemType.Equip)
        {
            var equipconfig = EquipConfigManager.inst.GetEquipInfoConfig(itemId);
            contentPane.icon.SetSprite(equipconfig.equipDrawingsConfig.atlas, equipconfig.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[equipconfig.equipQualityConfig.quality - 1]);
            contentPane.slider.gameObject.SetActive(false);
        }
        else
        {
            int needGem = DiamondCountUtils.GetEquipMakeMaterialsReFullCost(lackCount);
            //contentPane.needGemTx.color = UserDataProxy.inst.playerData.gem >= needGem ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("fd4f4f");
            contentPane.needGemTx.text = needGem.ToString("N0");
            itemConfig item = ItemconfigManager.inst.GetConfig(itemId);
            contentPane.icon.SetSprite(item.atlas, item.icon);
            if (currrestype == ItemType.Material)
            {
                ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);

                contentPane.lackResTips_1.enabled = true;

                if (rp != null && rp.isActivate)
                {
                    GUIHelper.SetUIGray(contentPane.gemBuyButton.transform, false);
                    contentPane.gemBuyButton.interactable = true;

                    bool isEnough = ItemBagProxy.inst.resItemCount(itemId) >= needcount;
                    if (isEnough)
                    {
                        if (hideItemId != 0)
                        {
                            GameTimer.inst.RemoveTimer(hideItemId);
                            hideItemId = 0;
                        }

                        setBtnRefreshAni(true);

                        hideItemId = GameTimer.inst.AddTimer(0.1f, 1, () =>
                        {
                            setBtnRefreshAni(false);
                            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIRefresh_CheckMakeEquipRes);
                        });
                    }
                    else
                    {
                        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.SyncUpdateProductionData);//不足直接同步数据
                        setBtnRefreshAni(false);
                        UnionResourceConfig union_resCfg = UnionResourceConfigManager.inst.GetConfig(itemId);

                        bool haveStore = rp.unionCanBuyTimes > 0;
                        contentPane.unionBuyButton.gameObject.SetActive(haveStore);
                        contentPane.unionBuyCountdownObj.gameObject.SetActive(!haveStore);

                        if (lackCount > 0)
                        {
                            contentPane.putResBoxButton.gameObject.SetActive(false);
                            if (haveStore)
                            {
                                contentPane.remainCountTx.text = haveStore ? LanguageManager.inst.GetValueByKey("剩余{0}次", rp.unionCanBuyTimes.ToString()) : "";
                                contentPane.unionReplenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "x" + (lackCount <= 0 ? 0 : Mathf.CeilToInt((float)(rp.countLimit * union_resCfg.item_num / 10000.0)));
                                contentPane.unionBuyCoinTx.text = union_resCfg.price.ToString("N0");
                                contentPane.unionBuyCoinTx.color = UserDataProxy.inst.playerData.unionCoin >= union_resCfg.price ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("fd4f4f");
                            }
                            else
                            {
                                contentPane.remainCountTx.text = LanguageManager.inst.GetValueByKey("下次倒计时");
                                setUnionCountdownTimer();
                            }

                            contentPane.slider.gameObject.SetActive(true);
                            contentPane.replenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "x" + lackCount;
                            contentPane.sliderCtrl.RefreshMakeBar((float)rp.duration, (float)rp.time);
                        }
                        else
                        {
                            contentPane.putResBoxButton.gameObject.SetActive(true);
                            contentPane.slider.gameObject.SetActive(true);
                            contentPane.gemBuyButton.gameObject.SetActive(false);
                            contentPane.unionBuyButton.gameObject.SetActive(false);

                        }


                    }
                }
                else
                {
                    contentPane.slider.value = 0;
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("请先摆放对应的资源篮"), Color.white);

                    GUIHelper.SetUIGray(contentPane.gemBuyButton.transform, true);
                    contentPane.gemBuyButton.interactable = false;
                }
            }
            else if (currrestype == ItemType.TaskMaterial)
            {
                if (_curInfo.needId != 30051)
                {
                    contentPane.slider.gameObject.SetActive(false);
                    var icfg = ExploreInstanceConfigManager.inst.GetConfigByDropid(itemId);
                    contentPane.ectypalBtnIcon.SetSprite(StaticConstants.exploreAtlas, icfg.instance_icon);
                    contentPane.ectypalBtnText.text = LanguageManager.inst.GetValueByKey("前往{0}", LanguageManager.inst.GetValueByKey(icfg.instance_name));
                }
                else
                {
                    contentPane.slider.gameObject.SetActive(false);

                    marketRect.anchoredPosition = new Vector2(-224, marketRect.anchoredPosition.y);

                    if ((HotfixBridge.inst.GetActivity_GoldenCityFlag() || (!HotfixBridge.inst.GetActivity_GoldenCityFlag() && HotfixBridge.inst.GetActivity_GoldenCityCanRewardCount() > 0)) && UserDataProxy.inst.playerData.level >= WorldParConfigManager.inst.GetConfig(8405).parameters)
                    {
                        contentPane.ectypalButton.gameObject.SetActive(true);

                        var goldenCityWdpCfg = WorldParConfigManager.inst.GetConfig(8403);
                        if (goldenCityWdpCfg != null)
                        {
                            var goldenCityItemCfg = ItemconfigManager.inst.GetConfig((int)goldenCityWdpCfg.parameters);

                            if (goldenCityItemCfg != null)
                            {
                                contentPane.ectypalBtnIcon.iconImage.enabled = true;
                                contentPane.ectypalBtnIcon.SetSprite(goldenCityItemCfg.atlas, goldenCityItemCfg.icon);
                            }
                            else
                            {
                                contentPane.ectypalBtnIcon.iconImage.enabled = false;
                            }
                        }
                        else
                        {
                            contentPane.ectypalBtnIcon.iconImage.enabled = false;
                        }

                        contentPane.ectypalBtnText.text = LanguageManager.inst.GetValueByKey("前往活动");
                    }
                    else
                    {
                        contentPane.ectypalButton.gameObject.SetActive(HotfixBridge.inst.GetActivity_GoldenCityCanRewardCount() > 0);
                        marketRect.anchoredPosition = new Vector2(0, marketRect.anchoredPosition.y);
                    }
                }

            }
        }
    }

    public void RefreshResProduction(int itemId)
    {
        var info = _lackResItems.Find(t => t.type == 0 && t.needId == itemId);
        if (info == null) return;

        bool isSameOne = this.itemId == itemId;

        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        if (rp != null && rp.isActivate)
        {
            GUIHelper.SetUIGray(contentPane.gemBuyButton.transform, false);
            contentPane.gemBuyButton.interactable = true;

            bool isEnough = ItemBagProxy.inst.resItemCount(itemId) >= info.needCount;
            if (isEnough)
            {
                if (isSameOne)
                {
                    if (hideItemId != 0)
                    {
                        GameTimer.inst.RemoveTimer(hideItemId);
                        hideItemId = 0;
                    }

                    setBtnRefreshAni(true);

                    hideItemId = GameTimer.inst.AddTimer(0.1f, 1, () =>
                    {
                        setBtnRefreshAni(false);
                        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIRefresh_CheckMakeEquipRes);
                    });
                }
                else
                {
                    setBtnRefreshAni(false);
                    _lackResItems.Remove(info);

                    //int index = _lackResItems.IndexOf(info);
                    //if (index != 0 && index != -1)
                    //{
                    //    needMaterialsInfo temp = _lackResItems[0];
                    //    _lackResItems[0] = info;
                    //    _lackResItems[index] = temp;
                    //}
                    setData(_lackResItems);
                }
            }
            else if (isSameOne)
            {
                setData(_lackResItems);

                //getInventoryNum();
                //contentPane.currInventoryTx.text = getInventoryNum().ToString() + "<color=#f41100>/" + info.needCount.ToString() + "</color>";
                //contentPane.replenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "x" + lackCount;
                //contentPane.needGemTx.text = DiamondCountUtils.GetEquipMakeMaterialsReFullCost(lackCount).ToString("N0");
                //contentPane.sliderCtrl.RefreshMakeBar((float)rp.duration, (float)rp.time);
            }
        }
    }


    private void turnPageBtnClick(bool isLeft)
    {
        int index = _lackResItems.IndexOf(_curInfo);
        if (isLeft)
        {
            index -= 1;
            if (index == -1) index = _lackResItems.Count - 1;
        }
        else
        {
            index += 1;
            if (index == _lackResItems.Count) index = 0;
        }

        _curInfo = _lackResItems[index];
        setItem(getItemType(_curInfo.type), _curInfo.needId, _curInfo.needCount);
    }


    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    int unionCountDownTimer;

    void clearUnionCountdownTimer()
    {
        if (unionCountDownTimer != 0)
        {
            GameTimer.inst.RemoveTimer(unionCountDownTimer);
            unionCountDownTimer = 0;
        }
    }

    void setUnionCountdownTimer()
    {
        clearUnionCountdownTimer();

        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        unionCountdownTimerMethod();
        unionCountDownTimer = GameTimer.inst.AddTimer(1, rp.unionBuyCountdownTime, unionCountdownTimerMethod);
    }

    void unionCountdownTimerMethod()
    {
        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        if (rp != null)
        {
            contentPane.unionBuyCountdownTx.text = TimeUtils.timeSpan3Str(rp.unionBuyCountdownTime);
        }
    }

    //按钮置灰并无法点击
    private void setBtnRefreshAni(bool isInDealing) //是否为交易中
    {
        
        contentPane.unionProcessingObj.SetActive(isInDealing);
        contentPane.unionCanClickObj.SetActive(!isInDealing);
        contentPane.unionBuyButton.enabled = !isInDealing;

        contentPane.gemProcessingObj.SetActive(isInDealing);
        contentPane.gemCanClickObj.SetActive(!isInDealing);
        contentPane.gemBuyButton.enabled = !isInDealing;
    }


}
