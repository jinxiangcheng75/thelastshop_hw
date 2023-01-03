using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FurnitureUpgradeView : ViewBase<FurniturePanelComp>
{
    public override string viewID => ViewPrefabName.FurnitureUpgradePanel;
    public override string sortingLayerName => "window";

    private Stack<GameObject> items;          //预制件栈
    private GameObject[] partOldImgs;         //隔离每行独立的需要替换的新的块部分

    public int maxLevel = 15;

    public int remainTime;

    public int remainTimerId;

    public int state;

    private int immediatelyDiamCount;
    private int nextLevel;
    private int nextGold;

    public IndoorData.ShopDesignItem currUpgradeItem;
    private Furniture _curFurniture;

    private void ClearDatas()
    {
        items.Clear();
    }

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });

        contentPane.upgradeBtn.ButtonClickTween(() =>
        {


            // switch (UserDataProxy.inst.shopData.currentState)
            // {
            //     case 1:
            //     case 2:
            //         {
            //             EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_SHOWMSGBOX);
            //             break;
            //         }
            //     default:
            //         {
            //             break;
            //         }
            // }

            //contentPane.upgradeBtn.interactable = UserDataProxy.inst.playerData.level >= nextConfig.shopkeeper_level && UserDataProxy.inst.playerData.gold >= nextConfig.money;
            //GUIHelper.SetUIGray(contentPane.upgradeBtn.transform, !(UserDataProxy.inst.playerData.level >= nextConfig.shopkeeper_level && UserDataProxy.inst.playerData.gold >= nextConfig.money));
            if (UserDataProxy.inst.playerData.level < nextLevel)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            if (UserDataProxy.inst.playerData.gold < nextGold)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            int curUpgradeFurnitureUid = UserDataProxy.inst.GetCurrentUpgradefurniture();

            if (curUpgradeFurnitureUid == 0)
            {
                Upgrade((int)currUpgradeItem.type, currUpgradeItem.uid, 0);
            }
            else
            {
                IndoorData.ShopDesignItem item = UserDataProxy.inst.GetFuriture(curUpgradeFurnitureUid);

                EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOWMSGBOX, item);
            }
        });

        contentPane.immediatelyUpgradeBtn.ButtonClickTween(() =>
        {
            if (!contentPane.confirmUpgradeObj.activeSelf)
            {
                contentPane.confirmUpgradeObj.SetActive(true);
            }
            else
            {
                Upgrade(currUpgradeItem.type, currUpgradeItem.uid, 1);
            }
        });



        items = new Stack<GameObject>();

        contentPane.upgradePanelTog.onValueChanged.AddListener((value) =>
        {
            AudioManager.inst.PlaySound(11);

            for (int i = 0; i < contentPane.upgradePanelTog.graphic.transform.childCount; i++)
            {
                contentPane.upgradePanelTog.graphic.transform.GetChild(i).gameObject.SetActive(value);
            }

            if (value)
                OnUpgradePanelTogDown();
        });

        contentPane.contentPanelTog.onValueChanged.AddListener((value) =>
        {
            AudioManager.inst.PlaySound(11);

            for (int i = 0; i < contentPane.contentPanelTog.graphic.transform.childCount; i++)
            {
                contentPane.contentPanelTog.graphic.transform.GetChild(i).gameObject.SetActive(value);
            }

            if (value)
                OnContentPanelTogDown();
        });

        contentPane.leftBtn.ButtonClickTween(() => PageChange(true));

        contentPane.rightBtn.ButtonClickTween(() => PageChange(false));

        contentPane.completeUpgradeBtn.ButtonClickTween(() =>
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, item.uid)
        );

        contentPane.gangongBtn.ButtonClickTween(() => contentPane.confirmGanGongBtn.gameObject.SetActiveTrue());

        contentPane.confirmGanGongBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE_Immediately, currUpgradeItem.type, item.uid);
        });
    }

    //点击左右按钮所选择的对象
    private void PageChange(bool isLeft)
    {
        AudioManager.inst.PlaySound(21);
        if (item.type == (int)kTileGroupType.Counter) return;

        Furniture furniture;
        if (currUpgradeItem != null)
        {
            if (IndoorMap.inst.GetFurnituresByUid(currUpgradeItem.uid, out furniture))
            {
                furniture.ReSetPos();
            }
        }
        currUpgradeItem = UserDataProxy.inst.getNearFurniture(item.uid, item.type, isLeft);
        EventController.inst.TriggerEvent<int>(GameEventType.ShopDesignEvent.PICK_ITEM, currUpgradeItem.uid);
        IndoorMap.inst.OnFurnituresSelect(currUpgradeItem.uid);
        GameTimer.inst.RemoveTimer(remainTimerId);

        //MgrShowView(null);
        setData(currUpgradeItem);
    }

    //倘若还有剩余时间的话，就刷新当前的页面的方法
    private void RemainTimeUpdate()
    {
        if (!isShowing) return;

        if (remainTime > 0)
        {
            SetState(1);
            remainTime--;
            contentPane.timerTxt.text = TimeUtils.timeSpanStrip(remainTime);
            //刷新钻石消耗（最少十颗钻石）
            immediatelyDiamCount = DiamondCountUtils.GetFurnitureUpgradeDiamonds(remainTime);
            contentPane.diamCountTxt3.text = immediatelyDiamCount.ToString("N0");
            contentPane.diamCountTxt4.text = immediatelyDiamCount.ToString("N0");
        }
        else
        {
            //完成升级
            SetState(2);
        }
    }

    //设置当前家具的升级状态
    public void SetState(int state)
    {
        if (state == 1)
        {
            this.state = 1;
        }
        else if (state == 2)
        {
            this.state = 2;
            contentPane.timerTxt.text = LanguageManager.inst.GetValueByKey("就绪");
            contentPane.turnState_UprgadingObj.SetActiveFalse();
            GameTimer.inst.RemoveTimer(remainTimerId);
        }

        contentPane.gangongBtn.gameObject.SetActive(this.state == 1);
        contentPane.completeUpgradeBtn.gameObject.SetActive(this.state == 2);
    }

    IndoorData.ShopDesignItem item = null;
    int[] iconItems = new int[4];

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);

    }

    public void setData(IndoorData.ShopDesignItem _data)
    {
        var c = contentPane;
        var l = LanguageManager.inst;
        currUpgradeItem = _data;
        item = currUpgradeItem;
        iconItems = item.config.iconitem_id;

        // contentPane.leftRightBtnObj.gameObject.SetActive(ShopDesignDataProxy.inst.getFurnitureNum(item.uid) != 1);
        contentPane.introTxt.text = LanguageManager.inst.GetValueByKey(item.config.des);

        OnUpgradePanelTogDown();

        contentPane.bottomBtnsObj.SetActive(item.level <= maxLevel);

        contentPane.currentLevelTxt.text = item.level.ToString();

        //contentPane.upgradePanelTog.onValueChanged.Equals(true);

        c.gangongBtn.gameObject.SetActiveTrue();
        c.confirmGanGongBtn.gameObject.SetActiveFalse();

        switch ((EDesignState)currUpgradeItem.state)
        {
            case EDesignState.Upgrading:
                {
                    c.sceneStateObj.SetActiveFalse();
                    c.turnStateObj.SetActiveTrue();
                    c.turnState_UprgadingObj.SetActiveTrue();
                    c.completeUpgradeBtn.gameObject.SetActiveFalse();

                    remainTime = (int)currUpgradeItem.entityState.leftTime;

                    c.timerTxt.text = remainTime.ToString();

                    RemainTimeUpdate();

                    remainTimerId = GameTimer.inst.AddTimer(1, RemainTimeUpdate);

                    break;
                }
            case EDesignState.Finished:
                {
                    c.sceneStateObj.SetActiveFalse();
                    c.turnStateObj.SetActiveTrue();
                    c.turnState_UprgadingObj.SetActiveFalse();
                    c.completeUpgradeBtn.gameObject.SetActiveTrue();

                    c.maxLevelText.gameObject.SetActive(false);
                    c.timerTxt.text = l.GetValueByKey("就绪");
                    c.completeUpgradeBtn.onClick.AddListener(() =>
                    EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, item.uid)
                    );

                    break;
                }
            default:
                {

                    if (item.level < maxLevel)
                    {

                        bool flag = false;

                        if (item.config.type_1 == (int)kTileGroupType.ResourceBin)
                        {
                            var resUpCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(item.config.type_2, item.level + 1);

                            if (resUpCfg != null)
                            {
                                var buildData = UserDataProxy.inst.GetBuildingData(resUpCfg.build_id);

                                if (buildData != null && buildData.state != (int)EBuildState.EB_Lock)
                                {
                                    if (buildData.level < resUpCfg.build_level)
                                    {
                                        c.maxLevelText.text = LanguageManager.inst.GetValueByKey("需要{0}达到{1}级", LanguageManager.inst.GetValueByKey(buildData.config.name), resUpCfg.build_level.ToString());
                                        c.sceneStateObj.SetActiveFalse();
                                        c.maxLevelText.gameObject.SetActive(true);
                                        flag = true;
                                    }
                                }
                            }
                        }

                        if (!flag)
                        {
                            c.sceneStateObj.SetActiveTrue();
                            c.maxLevelText.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        c.sceneStateObj.SetActiveFalse();
                        c.maxLevelText.text = LanguageManager.inst.GetValueByKey("已升至最高级");
                        c.maxLevelText.gameObject.SetActive(true);

                    }
                    c.turnStateObj.SetActiveFalse();

                    break;
                }
        }

        switch (item.config.type_1)
        {
            //柜台
            case (int)kTileGroupType.Counter:
                {
                    ShowCounterUpgradePanel();
                    break;
                }

            //资源篮
            case (int)kTileGroupType.ResourceBin:
                {
                    ShowResourceUpgradePanel();
                    break;
                }

            //货架
            case (int)kTileGroupType.Shelf:
                {
                    ShowShelfUpgradePanel();
                    break;
                }

            //储物箱
            case (int)kTileGroupType.Trunk:
                {
                    ShowStorageUpgradePanel();
                    break;
                }

            default:
                {
                    Debug.LogError("未能找到对应类别");
                    break;
                }
        }

        SetPhaseImgsFrameColor();
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
        contentPane.confirmUpgradeObj.SetActive(false);
        if (_curFurniture != null)
        {
            _curFurniture.ReSetPos();
            _curFurniture.OnSelected();
        }
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.topAnimator.CrossFade("show", 0f);
        contentPane.topAnimator.Update(0f);
        contentPane.topAnimator.Play("show");

        contentPane.windowAnimator.CrossFade("show", 0f);
        contentPane.windowAnimator.Update(0f);
        contentPane.windowAnimator.Play("show");

        //contentPane.leftBtn.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);
        //contentPane.rightBtn.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);

        //GameTimer.inst.AddTimer(0.2f, 1, () =>
        //{
        //    contentPane.leftBtn.GetComponent<Graphic>().FadeFromTransparentTween(1f, 0.5f);
        //    contentPane.rightBtn.GetComponent<Graphic>().FadeFromTransparentTween(1f, 0.5f);
        //});
    }

    protected override void DoHideAnimation()
    {
        contentPane.topAnimator.Play("hide");
        contentPane.windowAnimator.Play("hide");

        float animLength = contentPane.windowAnimator.GetClipLength("window_hide");

        //Graphic[] topGraphics = contentPane.topAnimator.transform.parent.GetComponentsInChildren<Graphic>();
        //foreach (var item in topGraphics) item.FadeTransparentTween(item.color.a, animLength);

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.topAnimator.CrossFade("null", 0f);
            contentPane.topAnimator.Update(0f);
            contentPane.windowAnimator.CrossFade("null", 0f);
            contentPane.windowAnimator.Update(0f);
            this.HideView();
        });
    }


    //按下升级面板切换器时
    private void OnUpgradePanelTogDown()
    {
        contentPane.upgradePanelTog.isOn = true;
        contentPane.contentPanelTog.isOn = false;

        contentPane.contentVarietyObj.SetActive(false);

        contentPane.introDataObj.SetActive(true);

        contentPane.bottomBtnsObj.SetActive(item.level <= maxLevel);
    }

    //按下内容面板切换器时
    private void OnContentPanelTogDown()
    {
        contentPane.contentPanelTog.isOn = true;
        contentPane.upgradePanelTog.isOn = false;

        contentPane.introDataObj.SetActive(false);
        contentPane.bottomBtnsObj.SetActive(false);

        contentPane.contentVarietyObj.SetActive(true);
    }

    //升级
    private void Upgrade(int designType, int designUid, int kind)    //0为金币，1为钻石
    {
        //if (designType == 9)
        //{
        //    var furnCfg = UserDataProxy.inst.GetFuriture(designUid);
        //    var upCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(furnCfg.config.type_2, 1);
        //    NetworkEvent.SendRequest(new NetworkRequestWrapper()
        //    {
        //        req = new Request_Resource_ProductionRefresh()
        //        {
        //            itemId = (int)upCfg.item_id
        //        }
        //    });
        //}

        if (kind == 1 && !UserDataProxy.inst.FurnitureCanUpgradeFinish(designUid)) //为钻石 先前端检测空间是否足够升级
        {
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SAVEDATA, designUid, kind);
        EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE, designType, designUid, kind);
    }

    //展示储存箱面板
    public void ShowStorageUpgradePanel()
    {
        var l = LanguageManager.inst;
        bool isArriveMaxLevel = currUpgradeItem.level >= maxLevel;
        TrunkUpgradeConfig lastConfig = FurnitureUpgradeConfigManager.inst.GetTrunkUpgradeConfig(currUpgradeItem.level);
        TrunkUpgradeConfig nextConfig = isArriveMaxLevel ? null : FurnitureUpgradeConfigManager.inst.GetTrunkUpgradeConfig(currUpgradeItem.level + 1);
        FurnitureConfig newConfig = isArriveMaxLevel ? null : FurnitureConfigManager.inst.getConfig(nextConfig.furniture_id);
        FurnitureConfig oldConfig = FurnitureConfigManager.inst.getConfig(lastConfig.furniture_id);

        if (!isArriveMaxLevel)
        {
            ShowBlocks(oldConfig.width, oldConfig.height, newConfig.width, newConfig.height);
            onShowUpgradePanel(nextConfig.time);
        }

        ClearDatas();


        SetTopContentValue(false, true);

        var c = contentPane;
        for (int i = 0; i < 3; i++)
        {
            var sucfg = TrunkUpgradeConfigManager.inst.getConfig(i * 5 + 1);
            var ftcfg = FurnitureConfigManager.inst.getConfig(sucfg.furniture_id);
            c.phaseImgs[i].SetSprite(ftcfg.atlas, ftcfg.icon);
        }

        var ssucfg = TrunkUpgradeConfigManager.inst.getConfig(item.level);
        var fftcfg = FurnitureConfigManager.inst.getConfig(ssucfg.furniture_id);
        c.storageImg.gameObject.SetActiveTrue();
        c.storageImg.SetSpriteURL(fftcfg.icon_big);
        c.maskCanvas.sortingLayerName = _uiCanvas.sortingLayerName;
        c.maskCanvas.sortingOrder = _uiCanvas.sortingOrder - 10;
        c.entityPos.gameObject.SetActiveFalse();
        _curFurniture = null;

        //Item1
        //contentPane.item1Img.SetSprite($"{GetItemIconAtlas(item.config, 0)}", $"{GetItemIconSprite(item.config, 0)}");
        contentPane.item1Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[0]]);
        contentPane.item1NameTxt.text = l.GetValueByKey("仓库容量");
        contentPane.item1OldValueTxt.text = $"{lastConfig.space}";
        if (!isArriveMaxLevel) contentPane.item1NewValueTxt.text = $"{nextConfig.space}";
        contentPane.item1NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && lastConfig.space < nextConfig.space);
        contentPane.item1ArrowImg.gameObject.SetActive(!isArriveMaxLevel && lastConfig.space < nextConfig.space);

        //Item2
        //contentPane.item2Img.SetSprite($"{GetItemIconAtlas(item.config, 1)}", $"{GetItemIconSprite(item.config, 1)}");
        contentPane.item2Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[1]]);
        contentPane.item2NameTxt.text = l.GetValueByKey("堆叠上限");
        contentPane.item2OldValueTxt.text = $"{lastConfig.pile_space}";
        if (!isArriveMaxLevel) contentPane.item2NewValueTxt.text = $"{nextConfig.pile_space}";
        contentPane.item2NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && lastConfig.pile_space < nextConfig.pile_space);
        contentPane.item2ArrowImg.gameObject.SetActive(!isArriveMaxLevel && lastConfig.pile_space < nextConfig.pile_space);

        //Item3
        //contentPane.item3Img.SetSprite($"{GetItemIconAtlas(item.config, 2)}", $"{GetItemIconSprite(item.config, 2)}");
        contentPane.item3Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[2]]);
        contentPane.item3NameTxt.text = l.GetValueByKey("大小");
        contentPane.item3OldValueTxt.text = $"{item.config.height}x{item.config.width}";
        if (!isArriveMaxLevel) contentPane.item3NewValueTxt.text = $"{newConfig.height}x{newConfig.width}";

        contentPane.item3NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && newConfig.width * newConfig.height > oldConfig.width * oldConfig.height);
        contentPane.item3ArrowImg.gameObject.SetActive(!isArriveMaxLevel && newConfig.width * newConfig.height > oldConfig.width * oldConfig.height);

        //底部按钮组件
        SetBottomBtns(nextConfig, isArriveMaxLevel);
    }

    //展示柜台面板
    public void ShowCounterUpgradePanel()
    {
        var l = LanguageManager.inst;
        bool isArriveMaxLevel = currUpgradeItem.level >= maxLevel;
        CounterUpgradeConfig lastConfig = FurnitureUpgradeConfigManager.inst.GetCounterUpgradeConfig(currUpgradeItem.level);
        CounterUpgradeConfig nextConfig = isArriveMaxLevel ? null : FurnitureUpgradeConfigManager.inst.GetCounterUpgradeConfig(currUpgradeItem.level + 1);
        FurnitureConfig newConfig = isArriveMaxLevel ? null : FurnitureConfigManager.inst.getConfig(nextConfig.furniture_id);
        FurnitureConfig oldConfig = FurnitureConfigManager.inst.getConfig(lastConfig.furniture_id);

        if (!isArriveMaxLevel)
        {
            ShowBlocks(oldConfig.width, oldConfig.height, newConfig.width, newConfig.height);
            onShowUpgradePanel(nextConfig.time);
        }

        ClearDatas();

        SetTopContentValue(false, false);

        var c = contentPane;
        for (int i = 0; i < 3; i++)
        {
            var sucfg = CounterUpgradeConfigManager.inst.getConfig(i * 5 + 1);
            var ftcfg = FurnitureConfigManager.inst.getConfig(sucfg.furniture_id);
            c.phaseImgs[i].SetSprite(ftcfg.atlas, ftcfg.icon);
        }

        var ssucfg = CounterUpgradeConfigManager.inst.getConfig(item.level);
        var fftcfg = FurnitureConfigManager.inst.getConfig(ssucfg.furniture_id);
        c.storageImg.gameObject.SetActiveTrue();
        c.storageImg.SetSpriteURL(fftcfg.icon_big);
        c.maskCanvas.sortingLayerName = _uiCanvas.sortingLayerName;
        c.maskCanvas.sortingOrder = _uiCanvas.sortingOrder - 10;
        c.entityPos.gameObject.SetActiveFalse();
        _curFurniture = null;

        //Item1
        //contentPane.item1Img.SetSprite($"{GetItemIconAtlas(item.config, 0)}", $"{GetItemIconSprite(item.config, 0)}");
        contentPane.item1Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[0]]);
        contentPane.item1NameTxt.text = l.GetValueByKey("售卖获得的能量");
        contentPane.item1OldValueTxt.text = $"{lastConfig.energy}";
        if (!isArriveMaxLevel) contentPane.item1NewValueTxt.text = $"{nextConfig.energy}";
        contentPane.item1NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && lastConfig.energy < nextConfig.energy);
        contentPane.item1ArrowImg.gameObject.SetActive(!isArriveMaxLevel && lastConfig.energy < nextConfig.energy);

        //Item3
        //contentPane.item3Img.SetSprite($"{GetItemIconAtlas(item.config, 2)}", $"{GetItemIconSprite(item.config, 2)}");
        contentPane.item3Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[2]]);
        contentPane.item3NameTxt.text = l.GetValueByKey("大小");
        contentPane.item3OldValueTxt.text = $"{item.config.height}x{item.config.width}";
        if (!isArriveMaxLevel) contentPane.item3NewValueTxt.text = $"{newConfig.height}x{newConfig.width}";
        contentPane.item3NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && newConfig.height * newConfig.width > oldConfig.height * oldConfig.width);
        contentPane.item3ArrowImg.gameObject.SetActive(!isArriveMaxLevel && newConfig.height * newConfig.width > oldConfig.height * oldConfig.width);

        //底部按钮组件
        SetBottomBtns(nextConfig, isArriveMaxLevel);
    }

    //展示资源篮面板
    public void ShowResourceUpgradePanel()
    {
        var l = LanguageManager.inst;
        bool isArriveMaxLevel = currUpgradeItem.level >= maxLevel;
        ResourceBinUpgradeConfig lastConfig = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(currUpgradeItem.config.type_2, currUpgradeItem.level);
        ResourceBinUpgradeConfig nextConfig = isArriveMaxLevel ? null : FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(currUpgradeItem.config.type_2, currUpgradeItem.level + 1);
        FurnitureConfig newConfig = isArriveMaxLevel ? null : FurnitureConfigManager.inst.getConfig(nextConfig.furniture_id);
        FurnitureConfig oldConfig = FurnitureConfigManager.inst.getConfig(lastConfig.furniture_id);

        if (!isArriveMaxLevel)
        {
            ShowBlocks(oldConfig.width, oldConfig.height, newConfig.width, newConfig.height);
            onShowUpgradePanel(nextConfig.time);
        }

        ClearDatas();


        SetTopContentValue(false, false);

        var c = contentPane;
        for (int i = 0; i < 3; i++)
        {
            var sucfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(lastConfig.type, i * 5 + 1);
            var ftcfg = FurnitureConfigManager.inst.getConfig(sucfg.furniture_id);
            c.phaseImgs[i].SetSprite(ftcfg.atlas, ftcfg.icon);
            //c.PhImgs[i].sprite = getSprite(ftcfg.atlas, ftcfg.icon);
        }

        var ssucfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(lastConfig.type, item.level);
        var fftcfg = FurnitureConfigManager.inst.getConfig(ssucfg.furniture_id);
        c.storageImg.SetSpriteURL(fftcfg.icon_big);

        if (!(fftcfg.type_2 == 4 || fftcfg.type_2 == 5)) //储油箱，珠宝箱没有阶段转换 无须拿来实例
        {
            c.maskCanvas.sortingLayerName = _uiCanvas.sortingLayerName;
            c.maskCanvas.sortingOrder = _uiCanvas.sortingOrder - 30;
            c.entityPos.gameObject.SetActiveTrue();
            if (IndoorMap.inst.GetFurnituresByUid(currUpgradeItem.uid, out _curFurniture))
            {
                _curFurniture.SetUIPosition(contentPane.entityPos, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 30 + 1, currUpgradeItem.config.height);
            }
            c.storageImg.gameObject.SetActiveFalse();
        }
        else
        {
            c.storageImg.gameObject.SetActiveTrue();
        }


        //Item1
        //contentPane.item1Img.SetSprite($"{GetItemIconAtlas(item.config, 0)}", $"{GetItemIconSprite(item.config, 0)}");
        contentPane.item1Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[0]]);
        contentPane.item1NameTxt.text = l.GetValueByKey("仓库容量");
        contentPane.item1OldValueTxt.text = $"{lastConfig.store}";
        if (!isArriveMaxLevel) contentPane.item1NewValueTxt.text = $"{nextConfig.store}";
        contentPane.item1NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && lastConfig.store < nextConfig.store);
        contentPane.item1ArrowImg.gameObject.SetActive(!isArriveMaxLevel && lastConfig.store < nextConfig.store);

        //Item3
        //contentPane.item3Img.SetSprite($"{GetItemIconAtlas(item.config, 2)}", $"{GetItemIconSprite(item.config, 2)}");
        contentPane.item3Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[2]]);
        contentPane.item3NameTxt.text = l.GetValueByKey("大小");
        contentPane.item3OldValueTxt.text = $"{item.config.height}x{item.config.width}";
        if (!isArriveMaxLevel) contentPane.item3NewValueTxt.text = $"{newConfig.height}x{newConfig.width}";
        contentPane.item3NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && newConfig.height * newConfig.height > oldConfig.height * oldConfig.height);
        contentPane.item3ArrowImg.gameObject.SetActive(!isArriveMaxLevel && newConfig.height * newConfig.height > oldConfig.height * oldConfig.height);

        //底部按钮组件
        SetBottomBtns(nextConfig, isArriveMaxLevel);
    }

    //展示货架面板
    public void ShowShelfUpgradePanel()
    {
        var l = LanguageManager.inst;
        bool isArriveMaxLevel = currUpgradeItem.level >= maxLevel;
        ShelfUpgradeConfig lastConfig = FurnitureUpgradeConfigManager.inst.GetShelfUpgradeConfig(currUpgradeItem.config.type_2, currUpgradeItem.level);
        ShelfUpgradeConfig nextConfig = isArriveMaxLevel ? null : FurnitureUpgradeConfigManager.inst.GetShelfUpgradeConfig(currUpgradeItem.config.type_2, currUpgradeItem.level + 1);
        FurnitureConfig newConfig = isArriveMaxLevel ? null : FurnitureConfigManager.inst.getConfig(nextConfig.furniture_id);
        FurnitureConfig oldConfig = FurnitureConfigManager.inst.getConfig(lastConfig.furniture_id);

        if (!isArriveMaxLevel)
        {
            ShowBlocks(oldConfig.width, oldConfig.height, newConfig.width, newConfig.height);
            onShowUpgradePanel(nextConfig.time);
        }

        ClearDatas();


        SetTopContentValue(true, true);

        var c = contentPane;
        for (int i = 0; i < 3; i++)
        {
            var sucfg = ShelfUpgradeConfigManager.inst.getConfigByType(lastConfig.type, i * 5 + 1);
            var ftcfg = FurnitureConfigManager.inst.getConfig(sucfg.furniture_id);
            c.phaseImgs[i].SetSprite(ftcfg.atlas, ftcfg.icon);
            GUIIcon img = c.ctrl.img_levelShelfList[i];
            img.SetSprite(ftcfg.atlas, ftcfg.icon);

            if (i * 5 + 1 > lastConfig.level)
            {
                GUIHelper.SetUIGray(img.transform, true);
            }
            else
            {
                GUIHelper.SetUIGray(img.transform, false);
            }
        }

        var ssucfg = ShelfUpgradeConfigManager.inst.getConfigByType(lastConfig.type, item.level);
        var fftcfg = FurnitureConfigManager.inst.getConfig(ssucfg.furniture_id);
        c.storageImg.gameObject.SetActiveFalse();
        //c.storageImg.SetSpriteURL(fftcfg.icon_big);

        c.maskCanvas.sortingLayerName = _uiCanvas.sortingLayerName;
        c.maskCanvas.sortingOrder = _uiCanvas.sortingOrder - 30;
        c.entityPos.gameObject.SetActiveTrue();
        if (IndoorMap.inst.GetFurnituresByUid(currUpgradeItem.uid, out _curFurniture))
        {
            _curFurniture.SetUIPosition(contentPane.entityPos, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 30 + 1, currUpgradeItem.config.height);
        }


        //Item1
        //contentPane.item1Img.SetSprite($"{GetItemIconAtlas(item.config, 0)}", $"{GetItemIconSprite(item.config, 0)}");
        contentPane.item1Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[0]]);
        contentPane.item1NameTxt.text = l.GetValueByKey("能量上限");
        contentPane.item1OldValueTxt.text = $"{lastConfig.energy}";
        if (!isArriveMaxLevel) contentPane.item1NewValueTxt.text = $"{nextConfig.energy}";
        contentPane.item1NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && lastConfig.energy < nextConfig.energy);
        contentPane.item1ArrowImg.gameObject.SetActive(!isArriveMaxLevel && lastConfig.energy < nextConfig.energy);

        //Item2
        //contentPane.item2Img.SetSprite($"{GetItemIconAtlas(item.config, 1)}", $"{GetItemIconSprite(item.config, 1)}");
        contentPane.item2Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[1]]);
        contentPane.item2NameTxt.text = l.GetValueByKey("容量");
        contentPane.item2OldValueTxt.text = $"{lastConfig.store}";
        if (!isArriveMaxLevel) contentPane.item2NewValueTxt.text = $"{nextConfig.store}";
        contentPane.item2NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && lastConfig.store < nextConfig.store);
        contentPane.item2ArrowImg.gameObject.SetActive(!isArriveMaxLevel && lastConfig.store < nextConfig.store);

        //Item3
        //contentPane.item3Img.SetSprite($"{GetItemIconAtlas(item.config, 2)}", $"{GetItemIconSprite(item.config, 2)}");
        contentPane.item3Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[2]]);
        contentPane.item3NameTxt.text = l.GetValueByKey("大小");
        contentPane.item3OldValueTxt.text = $"{item.config.height}x{item.config.width}";
        if (!isArriveMaxLevel) contentPane.item3NewValueTxt.text = $"{newConfig.height}x{newConfig.width}";
        contentPane.item3NewValueTxt.gameObject.SetActive(!isArriveMaxLevel && newConfig.width * newConfig.height > oldConfig.width * oldConfig.height);
        contentPane.item3ArrowImg.gameObject.SetActive(!isArriveMaxLevel && newConfig.width * newConfig.height > oldConfig.width * oldConfig.height);

        //底部按钮组件
        SetBottomBtns(nextConfig, isArriveMaxLevel);

        #region ContentVariety部分

        switch (lastConfig.type)
        {
            case (int)kShelfType.ColdeWeapon:
                c.ctrl.SetNodes(false, item);
                break;
            //热武器 防具
            case (int)kShelfType.ThermalWeapon:
            case (int)kShelfType.Armor:
            case (int)kShelfType.Misc:
                c.ctrl.SetNodes(true, item);
                break;
            default:
                break;
        }
        #endregion
    }

    //底部按钮状态
    private void SetBottomBtns(BaseUpgradeConfig nextConfig, bool isMax)
    {
        syncOnlyIntroDataDes();
        if (isMax)
        {
            contentPane.item4Obj.SetActiveFalse();
            contentPane.only_item4Obj.SetActiveFalse();
            return;
        }
        else
        {
            contentPane.item4Obj.SetActiveTrue();
            contentPane.only_item4Obj.SetActiveTrue();
        }

        if (UserDataProxy.inst.playerData.gold < nextConfig.money)
        {
            contentPane.coinCountTxt.text = $"<color=#ff2828>{nextConfig.money}</color>";
        }
        else
        {
            contentPane.coinCountTxt.text = $"<color=#FFFFFF>{nextConfig.money}</color>";
        }

        if (UserDataProxy.inst.playerData.gem < nextConfig.diamond)
        {
            contentPane.immediatelyUpgradeBtn.interactable = false;
            GUIHelper.SetUIGray(contentPane.immediatelyUpgradeBtn.transform, true);
            contentPane.diamCountTx.text = $"<color=#ff2828>{nextConfig.diamond}</color>";
        }
        else
        {
            contentPane.immediatelyUpgradeBtn.interactable = true;
            GUIHelper.SetUIGray(contentPane.immediatelyUpgradeBtn.transform, false);
            contentPane.diamCountTx.text = $"<color=#FFFFFF>{nextConfig.diamond}</color>";
        }

        if (UserDataProxy.inst.playerData.level < nextConfig.shopkeeper_level)
        {
            contentPane.notArriveLv.SetActive(true);
            contentPane.arriveLv.enabled = false;
            contentPane.flagLevelTxt.text = $"<color=#ff2828>{nextConfig.shopkeeper_level}</color>";
        }
        else
        {
            contentPane.notArriveLv.SetActive(false);
            contentPane.arriveLv.enabled = true;
            contentPane.flagLevelTxt.text = $"<color=#FFFFFF>{nextConfig.shopkeeper_level}</color>";
        }

        nextLevel = nextConfig.shopkeeper_level;
        nextGold = nextConfig.money;
        //contentPane.upgradeBtn.interactable = UserDataProxy.inst.playerData.level >= nextConfig.shopkeeper_level && UserDataProxy.inst.playerData.gold >= nextConfig.money;
        //GUIHelper.SetUIGray(contentPane.upgradeBtn.transform, !(UserDataProxy.inst.playerData.level >= nextConfig.shopkeeper_level && UserDataProxy.inst.playerData.gold >= nextConfig.money));

        if (item.level >= maxLevel)
        {
            contentPane.bottomBtnsObj.SetActiveFalse();
            contentPane.item4Obj.SetActiveFalse();
        }

    }

    private void syncOnlyIntroDataDes()
    {
        contentPane.only_IntroTxt.text = contentPane.introTxt.text;

        //contentPane.only_icon1.sprite = contentPane.item1Img.GetComponent<Image>().sprite;
        //contentPane.only_icon2.sprite = contentPane.item2Img.GetComponent<Image>().sprite;
        //contentPane.only_icon3.sprite = contentPane.item3Img.GetComponent<Image>().sprite;
        //contentPane.only_icon4.sprite = contentPane.item4Img.GetComponent<Image>().sprite;
        if (iconItems[0] != 0)
            //contentPane.only_icon1.SetSprite($"{GetItemIconAtlas(item.config, 0)}", $"{GetItemIconSprite(item.config, 0)}");
            contentPane.only_icon1.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[0]]);
        if (iconItems[1] != 0)
            //contentPane.only_icon2.SetSprite($"{GetItemIconAtlas(item.config, 1)}", $"{GetItemIconSprite(item.config, 1)}");
            contentPane.only_icon2.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[1]]);
        if (iconItems[2] != 0)
            //contentPane.only_icon3.SetSprite($"{GetItemIconAtlas(item.config, 2)}", $"{GetItemIconSprite(item.config, 2)}");
            contentPane.only_icon3.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[2]]);
        if (iconItems[3] != 0)
            //contentPane.only_icon4.SetSprite($"{GetItemIconAtlas(item.config, 3)}", $"{GetItemIconSprite(item.config, 3)}");
            contentPane.only_icon4.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[3]]);

        contentPane.only_tx1.text = contentPane.item1NameTxt.text;
        contentPane.only_tx2.text = contentPane.item2NameTxt.text;
        contentPane.only_tx3.text = contentPane.item3NameTxt.text;
        contentPane.only_tx4.text = contentPane.item4NameTxt.text;

        contentPane.only_oldVal1.text = contentPane.item1OldValueTxt.text;
        contentPane.only_oldVal2.text = contentPane.item2OldValueTxt.text;
        contentPane.only_oldVal3.text = contentPane.item3OldValueTxt.text;

        contentPane.only_newVal1.text = contentPane.item1NewValueTxt.text;
        contentPane.only_newVal2.text = contentPane.item2NewValueTxt.text;
        contentPane.only_newVal3.text = contentPane.item3NewValueTxt.text;

        contentPane.only_timerTxt.text = contentPane.item4TimeTxt.text;


    }

    public void RefreshShelfGridItem(IndoorData.ShopDesignItem shelf)
    {
        if (_curFurniture.uid != shelf.uid) return;
        contentPane.ctrl.RefreshShelfGridItem(shelf);
    }

    private float scaleFactor;      //缩放系数
    private float xFactor = 0.054f;
    private float yFactor = 0.027f;

    //在货架升级的面板中生成小格子
    public void ShowBlocks(int oldLength, int oldWidth, int newLength, int newWidth/*,bool isArriveLevel*/)
    {

        contentPane.gridContent.constraintCount = newWidth;
        contentPane.only_glGroup.constraintCount = newWidth;

        for (int i = 0; i < contentPane.gridContent.transform.childCount; i++)
        {
            GameObject.Destroy(contentPane.gridContent.transform.GetChild(i).gameObject);
            GameObject.Destroy(contentPane.only_glGroup.transform.GetChild(i).gameObject);
        }

        //设置缩放比例
        scaleFactor =/*x4 * y4 <= 12 ? 1 :*/ 1 - (newWidth * yFactor + newLength * xFactor);

        contentPane.gridContent.GetComponent<RectTransform>().localScale =
            new Vector3(scaleFactor, scaleFactor, 1);

        contentPane.only_glGroup.GetComponent<RectTransform>().localScale =
            new Vector3(scaleFactor, scaleFactor, 1);

        Stack<GameObject> onlyItems = new Stack<GameObject>();

        for (int i = newLength * newWidth; i > 0; i--)
        {
            GameObject item = GameObject.Instantiate(contentPane.oldPfb, contentPane.gridContent.transform);
            GameObject onlyItem = GameObject.Instantiate(contentPane.oldPfb, contentPane.only_glGroup.transform);

            items.Push(item);
            onlyItems.Push(onlyItem);
        }

        for (int i = (newLength - oldLength) * newWidth; i > 0; i--)
        {
            GameObject item = items.Pop();
            GameObject onlyItem = onlyItems.Pop();
            onlyItem.GetComponent<Image>().sprite = item.GetComponent<Image>().sprite = contentPane.newImg.sprite;
        }

        partOldImgs = items.ToArray();
        Array.Reverse(partOldImgs);

        for (int i = 0; i < partOldImgs.Length; i += newWidth)
        {
            for (int j = oldWidth; j < newWidth; j++)
            {
                partOldImgs[i + j].gameObject.GetComponent<Image>().sprite = contentPane.newImg.sprite;
            }
        }

        partOldImgs = onlyItems.ToArray();
        Array.Reverse(partOldImgs);

        for (int i = 0; i < partOldImgs.Length; i += newWidth)
        {
            for (int j = oldWidth; j < newWidth; j++)
            {
                partOldImgs[i + j].gameObject.GetComponent<Image>().sprite = contentPane.newImg.sprite;
            }
        }

    }

    public void onShowUpgradePanel(int sec)
    {
        //Item4
        //contentPane.item4Img.SetSprite($"{GetItemIconAtlas(item.config, 3)}", $"{GetItemIconSprite(item.config, 3)}");
        contentPane.item4Img.SetSprite(StaticConstants.funitureItemAtlasName, StaticConstants.funitureItemIcons[iconItems[3]]);
        contentPane.item4NameTxt.text = LanguageManager.inst.GetValueByKey("升级时间");
        contentPane.item4TimeTxt.text = TimeUtils.timeSpanStrip(sec, false);
    }

    /// <summary>
    /// -Index    0-是否为货架（否则不需要toggles）      1-该面板是否需要item2组件
    /// </summary>
    /// <param name="boolArray"></param>
    public void SetTopContentValue(bool topBtnsAndLinesObjActive, bool item2ObjActive)
    {
        //该面板需要顶部按钮组件以及item2组件
        contentPane.topBtnsAndLinesObj.SetActive(topBtnsAndLinesObjActive);
        contentPane.introDataObj.SetActive(topBtnsAndLinesObjActive);
        contentPane.onlyIntroDataObj.SetActive(!topBtnsAndLinesObjActive);
        contentPane.item2Obj.SetActive(item2ObjActive);
        contentPane.only_item2Obj.SetActive(item2ObjActive);

        //文本组件部分
        contentPane.objName.text = LanguageManager.inst.GetValueByKey(item.config.name);
        contentPane.subtypeNameTx.text = LanguageManager.inst.GetValueByKey(StaticConstants.furnitureSubTypeNames[item.config.type_1]);
    }

    //设置不同阶段的框框背景颜色（倘若不知道框框是啥的可以看一下策划文档）
    private void SetPhaseImgsFrameColor()
    {
        var c = contentPane;

        for (int i = 0; i < 3; i++)
        {
            if (item.level >= i * 5 + 1 && item.level <= (i + 1) * 5 + 1)
            {
                c.frameImgs[i].sprite = c.yellowKuang;

            }
            else
            {
                c.frameImgs[i].sprite = c.whiteKuang;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            contentPane.arrowImgs[i].sprite = contentPane.purpleArrow;
        }

        if (item.level < 6)
        {
            GUIHelper.SetUIGray(c.phaseImgs[2].transform, true);
        }
        else
        {
            GUIHelper.SetUIGray(c.phaseImgs[2].transform, false);

        }

        SetArrowImgs();
    }

    //设置箭头的图片（绿色或者紫色）
    private void SetArrowImgs()
    {
        if (item.level == 5)
        {
            contentPane.frameImgs[1].sprite = contentPane.yellowKuang;
            contentPane.arrowImgs[0].sprite = contentPane.greenArrow;
            contentPane.frameImgs[1].sprite = contentPane.greenKuang;
        }

        if (item.level == 10)
        {
            contentPane.frameImgs[2].sprite = contentPane.yellowKuang;
            contentPane.arrowImgs[1].sprite = contentPane.greenArrow;
            contentPane.frameImgs[2].sprite = contentPane.greenKuang;
        }
    }
}