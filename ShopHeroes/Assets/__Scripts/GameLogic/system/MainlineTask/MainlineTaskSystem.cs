using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainlineTaskSystem : BaseSystem
{
    MainLineTaskView mainlineTaskView;
    MainLineTaskInfoView mainlineTaskInfoView;
    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener<bool>(GameEventType.MainlineTaskEvent.SETTIMERRESET, setTimerReset);
        EventController.inst.AddListener<bool>(GameEventType.MainlineTaskEvent.SETFINGTERACTIVE, setFingerActive);
        EventController.inst.AddListener<Transform, bool, K_Operation_Finger>(GameEventType.MainlineTaskEvent.SETTARGETTRANSFORM, setTargetTrans);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.SHOWMAINLINEUI, showMainlineTaskUI);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI, hideMainlineTaskUI);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.REALHIDEPANEL, realHideUI);

        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.FINDTARGETTRANSFORM, waitTargetPanelOpen);
        EventController.inst.AddListener<string>(GameEventType.MainlineTaskEvent.SHOWMAINLINEDIALOG, showMainLineDialog);

        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.REQUESTMAINLINEDATA, requestMainlineTask);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.REQUESTMAINLINEREWARD, requestMainlineReward);

        EventController.inst.AddListener<bool>(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, refreshTaskData);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.NEWTASKPLAYANIM, newTaskPlayAnim);

        EventController.inst.AddListener<MainlineData>(GameEventType.MainlineTaskEvent.SHOWMAINLINEINFOUI, showMainlineInfoUI);
        EventController.inst.AddListener<int, long>(GameEventType.MainlineTaskEvent.CREATFLOATPREFAB, creatFloatPrefab);

        //行为
        EventController.inst.AddListener<int>(GameEventType.MainlineTaskEvent.SELECTSCENEFUR, selectSceneFurn);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTUIFURN, selectUIFurn);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.RECRUITHERO, recruitHero);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.ADDHEROSLOT, addHeroSlot);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.CLICKHEROINFO, clickHeroInfo);
        EventController.inst.AddListener<int>(GameEventType.MainlineTaskEvent.CLICKSHOPPERPOP, clickShopperPop);
        EventController.inst.AddListener<int, MainLineData>(GameEventType.MainlineTaskEvent.OPENTARGETTBOX, openTargetTbox);
        EventController.inst.AddListener<int>(GameEventType.MainlineTaskEvent.BUILDINVEST, buildInvest);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.SCIENCEBUILDINVEST, scienceBuildInvest);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETEQUIP, selectTargetEquip);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETWORKER, selectTargetWorker);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETTRANSFERHERO, selectTargetTransferHero);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETEXPLORE, selectTargetExplore);
        EventController.inst.AddListener<int, MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETTRANSFERTOGGLE, selectTargetTransferToggle);
        EventController.inst.AddListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTCANRECRUITHERO, selectCanRecruitHero);
        EventController.inst.AddListener<int>(GameEventType.MainlineTaskEvent.SELECTTARGETEQUIPPAGE, selectTargetEquipPage);

        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.Reset_TargetExplore, resetTargetExplore);
        EventController.inst.AddListener(GameEventType.MainlineTaskEvent.Reset_TargetEquipType, resetSelectTargetEquipType);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener<bool>(GameEventType.MainlineTaskEvent.SETTIMERRESET, setTimerReset);
        EventController.inst.RemoveListener<bool>(GameEventType.MainlineTaskEvent.SETFINGTERACTIVE, setFingerActive);
        EventController.inst.RemoveListener<Transform, bool, K_Operation_Finger>(GameEventType.MainlineTaskEvent.SETTARGETTRANSFORM, setTargetTrans);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.SHOWMAINLINEUI, showMainlineTaskUI);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI, hideMainlineTaskUI);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.REALHIDEPANEL, realHideUI);

        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.FINDTARGETTRANSFORM, waitTargetPanelOpen);
        EventController.inst.RemoveListener<string>(GameEventType.MainlineTaskEvent.SHOWMAINLINEDIALOG, showMainLineDialog);

        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.REQUESTMAINLINEDATA, requestMainlineTask);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.REQUESTMAINLINEREWARD, requestMainlineReward);

        EventController.inst.RemoveListener<bool>(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, refreshTaskData);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.NEWTASKPLAYANIM, newTaskPlayAnim);

        EventController.inst.RemoveListener<MainlineData>(GameEventType.MainlineTaskEvent.SHOWMAINLINEINFOUI, showMainlineInfoUI);
        EventController.inst.RemoveListener<int, long>(GameEventType.MainlineTaskEvent.CREATFLOATPREFAB, creatFloatPrefab);

        //行为
        EventController.inst.RemoveListener<int>(GameEventType.MainlineTaskEvent.SELECTSCENEFUR, selectSceneFurn);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTUIFURN, selectUIFurn);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.RECRUITHERO, recruitHero);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.ADDHEROSLOT, addHeroSlot);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.CLICKHEROINFO, clickHeroInfo);
        EventController.inst.RemoveListener<int>(GameEventType.MainlineTaskEvent.CLICKSHOPPERPOP, clickShopperPop);
        EventController.inst.RemoveListener<int, MainLineData>(GameEventType.MainlineTaskEvent.OPENTARGETTBOX, openTargetTbox);
        EventController.inst.RemoveListener<int>(GameEventType.MainlineTaskEvent.BUILDINVEST, buildInvest);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.SCIENCEBUILDINVEST, scienceBuildInvest);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETEQUIP, selectTargetEquip);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETWORKER, selectTargetWorker);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETTRANSFERHERO, selectTargetTransferHero);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETEXPLORE, selectTargetExplore);
        EventController.inst.RemoveListener<int, MainLineData>(GameEventType.MainlineTaskEvent.SELECTTARGETTRANSFERTOGGLE, selectTargetTransferToggle);
        EventController.inst.RemoveListener<MainLineData>(GameEventType.MainlineTaskEvent.SELECTCANRECRUITHERO, selectCanRecruitHero);
        EventController.inst.RemoveListener<int>(GameEventType.MainlineTaskEvent.SELECTTARGETEQUIPPAGE, selectTargetEquipPage);

        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.Reset_TargetExplore, resetTargetExplore);
        EventController.inst.RemoveListener(GameEventType.MainlineTaskEvent.Reset_TargetEquipType, resetSelectTargetEquipType);
    }

    void newTaskPlayAnim()
    {
        if (MainLineDataProxy.inst.Data == null || MainLineDataProxy.inst.Data.cfg == null || MainLineDataProxy.inst.MainTaskIsAllOver) return;
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.newTaskPlayAnim();
        }
    }

    void creatFloatPrefab(int itemId, long count)
    {
        if (MainLineDataProxy.inst.Data == null || MainLineDataProxy.inst.Data.cfg == null || MainLineDataProxy.inst.MainTaskIsAllOver) return;
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setFloatPrefabPlay(itemId, count);
        }
    }

    void realHideUI()
    {
        GUIManager.HideView<MainLineTaskView>();
    }

    void showMainlineInfoUI(MainlineData data)
    {
        if (MainLineDataProxy.inst.Data == null || MainLineDataProxy.inst.Data.cfg == null || MainLineDataProxy.inst.MainTaskIsAllOver) return;
        GUIManager.OpenView<MainLineTaskInfoView>((view) =>
        {
            mainlineTaskInfoView = view;
            view.setData(data);
        });
    }

    void refreshTaskData(bool needPlayAnim)
    {
        if (GuideManager.inst.isInTriggerGuide) return;
        if (MainLineDataProxy.inst.MainTaskIsAllOver) return;
        if (ManagerBinder.inst.mGameState != kGameState.Shop && ManagerBinder.inst.mGameState != kGameState.Town) return;
        var shopperUI = GUIManager.GetWindow<ShopperUIView>();
        if (shopperUI != null && shopperUI.isShowing) return;
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setTaskData(needPlayAnim);
        }
    }

    void requestMainlineTask()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_MainTask()
        });
    }

    void requestMainlineReward()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_MainTaskReward()
        });
    }

    void showMainLineDialog(string talkStr)
    {
        if (MainLineDataProxy.inst.Data == null || MainLineDataProxy.inst.Data.cfg == null) return;
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setDialogData(talkStr);
        }
    }

    void hideMainlineTaskUI()
    {
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.hideTaskContent();
        }
    }

    void showMainlineTaskUI()
    {
        if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
        {
            GUIManager.OpenView<MainLineTaskView>((view) =>
            {
                mainlineTaskView = view;
            });
        }
    }

    void setTimerReset(bool state)
    {
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setTimerReset(state);
        }
    }

    void setFingerActive(bool fingerActive)
    {
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setFingerActive(fingerActive);
        }
    }

    void setTargetTrans(Transform targetTrans, bool needSetPosImmediately, K_Operation_Finger type)
    {
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setTargetTransform(targetTrans, needSetPosImmediately, type);
        }
    }

    void setTargetTrans(Vector3 targetTrans, bool needSetPosImmediately, K_Operation_Finger type)
    {
        if (mainlineTaskView != null && mainlineTaskView.isShowing)
        {
            mainlineTaskView.setTargetTransform(targetTrans, needSetPosImmediately, type);
        }
    }

    Transform target = null;
    int timerId = 0;
    #region 正常点击按钮操作
    void waitTargetPanelOpen(MainLineData curOperationData)
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (ManagerBinder.inst.stateIsChanging)
        {
            timerId = GameTimer.inst.AddTimer(0.1f, () =>
             {
                 if (!ManagerBinder.inst.stateIsChanging)
                 {
                     GameTimer.inst.RemoveTimer(timerId);
                     timerId = 0;
                     waitTargetPanelOpen(curOperationData);
                 }
             });
        }
        else
        {
            GameTimer.inst.AddTimer(0.5f, 1, () =>
        {
            if ((GUIManager.GetCurrWindowViewID() == curOperationData.panelName || curOperationData.panelName == "TopPlayerInfoPanel"))
            {
                findTargetTrans(curOperationData);
                //GameTimer.inst.RemoveTimer(timerId);
                //timerId = 0;
            }
            else
            {
                GoOperationManager.inst.DissatisfactionDialog();
                setTimerReset(false);
            }
        });
        }
    }

    void findTargetTrans(MainLineData curOperationData)
    {
        if (curOperationData.btnName.Contains("-"))
        {
            var btn_names = curOperationData.btnName.Split('-');
            GameObject panel = FGUI.inst.uiRootTF.Find(curOperationData.panelName) != null ? FGUI.inst.uiRootTF.Find(curOperationData.panelName).gameObject : null;
            if (panel != null)
            {
                for (int i = 0; i < btn_names.Length; i++)
                {
                    panel = panel.FindHideChildGameObject(btn_names[i]);
                    if (panel == null) return;
                }

                if (panel != null)
                {
                    target = panel.transform;
                    setComponentEvent();
                }
                else
                {
                    GoOperationManager.inst.DissatisfactionDialog();
                    setTimerReset(false);
                    return;
                }
            }
        }
        else
        {
            GameObject panel = FGUI.inst.uiRootTF.Find(curOperationData.panelName) != null ? FGUI.inst.uiRootTF.Find(curOperationData.panelName).gameObject : null;
            if (panel != null)
            {
                GameObject targetGo = panel.FindHideChildGameObject(curOperationData.btnName);
                if (targetGo != null)
                {
                    target = targetGo.transform;
                    setComponentEvent();
                }
                else
                {
                    GoOperationManager.inst.DissatisfactionDialog();
                    setTimerReset(false);
                    return;
                }
            }
        }

        if (target == null)
        {
            Logger.error("没有找到当前操作的按钮" + curOperationData.panelName + "   " + curOperationData.btnName);
            GoOperationManager.inst.DissatisfactionDialog();
            setTimerReset(false);
            return;
        }

        setTargetTrans(target, true, K_Operation_Finger.Normal);
    }
    #endregion

    int sceneFurnTimerId = 0;
    int furnCount = 0;
    Furniture tempFurn;
    #region 点击场景中指定家具
    void selectSceneFurn(int furnId)
    {
        if (timerId > 0)
        {
            furnCount = 0;
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (ManagerBinder.inst.stateIsChanging)
        {
            timerId = GameTimer.inst.AddTimer(0.1f, () =>
            {
                furnCount++;
                if (!ManagerBinder.inst.stateIsChanging && IndoorMap.inst != null && IndoorMap.inst.IndoorFunituresList != null && IndoorMap.inst.isInit)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                    selectSceneFurn(furnId);
                }
                if (furnCount >= 50)
                {
                    furnCount = 0;
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                }
            });
        }
        else
        {
            if (IndoorMap.inst == null)
            {
                Logger.error("IndoorMap is null in mainLineSystem");
                return;
            }

            var allFurn = IndoorMap.inst.IndoorFunituresList;
            for (int i = 0; i < allFurn.Count; i++)
            {
                int index = i;
                var curFurn = allFurn[index];
                if (curFurn.id == furnId)
                {
                    if (sceneFurnTimerId > 0)
                    {
                        GameTimer.inst.RemoveTimer(sceneFurnTimerId);
                        sceneFurnTimerId = 0;
                    }

                    sceneFurnTimerId = GameTimer.inst.AddTimer(0.1f, () =>
                     {
                         var eventListen = curFurn.GetComponentInChildren<InputEventListener>();
                         if (eventListen != null)
                         {
                             //Logger.error("找到家具了");
                             target = eventListen.transform;
                             tempFurn = curFurn;
                             D2DragCamera.inst.LookToPosition(tempFurn.PopUIRoot.position, cameraMoveEndToSetFurnFinger);
                             //setComponentEvent();
                             //setTargetTrans(target, true, K_Operation_Finger.SceneFurn);
                             GameTimer.inst.RemoveTimer(sceneFurnTimerId);
                             sceneFurnTimerId = 0;
                         }
                     });

                    return;
                }
            }

            int[] furnIdArr = FurnitureConfigManager.inst.getSameTypeFurnitureId(furnId);
            if (furnIdArr != null)
            {
                for (int i = 0; i < allFurn.Count; i++)
                {
                    int index = i;
                    var curFurn = allFurn[index];
                    if (curFurn.id == furnIdArr[0] || curFurn.id == furnIdArr[1])
                    {
                        var eventListen = curFurn.GetComponentInChildren<InputEventListener>();
                        if (eventListen != null)
                        {
                            tempFurn = curFurn;
                            target = eventListen.transform;
                            D2DragCamera.inst.LookToPosition(tempFurn.PopUIRoot.position, cameraMoveEndToSetFurnFinger);
                            //setComponentEvent();
                            //setTargetTrans(target, true, K_Operation_Finger.SceneFurn);
                        }
                        return;
                    }
                }
            }

            GoOperationManager.inst.DissatisfactionDialog();
            setTimerReset(false);
        }
    }

    void cameraMoveEndToSetFurnFinger()
    {
        setComponentEvent();

        setTargetTrans(tempFurn.PopUIRoot, true, K_Operation_Finger.SceneFurn);
    }
    #endregion

    #region 点击界面上指定家具(包含跳转指定类型操作)
    void selectUIFurn(MainLineData data)
    {
        int furnId = int.Parse(data.btnName);
        var cfg = FurnitureConfigManager.inst.getConfig(furnId);
        if (cfg == null)
        {
            Logger.error("not find id is" + furnId + " cfgData");
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TARGETFURN, (Int16)getDisplayType(cfg.type_1), furnId);
        waitTargetPanelOpen(data);
    }

    kFurnitureDisplayType getDisplayType(int firstType)
    {
        kTileGroupType type = (kTileGroupType)firstType;
        switch (type)
        {
            case kTileGroupType.Carpet:
                return kFurnitureDisplayType.Carpet;
            case kTileGroupType.WallFurniture:
                return kFurnitureDisplayType.Furniture;
            case kTileGroupType.Furniture:
                return kFurnitureDisplayType.Furniture;
            case kTileGroupType.Shelf:
                return kFurnitureDisplayType.ShelfAndTrunk;
            case kTileGroupType.Trunk:
                return kFurnitureDisplayType.ShelfAndTrunk;
            case kTileGroupType.ResourceBin:
                return kFurnitureDisplayType.ResourceBin;
            case kTileGroupType.OutdoorFurniture:
                return kFurnitureDisplayType.OutdoorFurniture;
        }
        return kFurnitureDisplayType.None;
    }
    #endregion

    #region 英雄招募
    void recruitHero(MainLineData data)
    {
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 0);

        int canRecruitCount = RoleDataProxy.inst.FieldNumAbtractHeroNum;
        if (canRecruitCount > 0)
        {
            waitTargetPanelOpen(data);
        }
        else
        {
            GoOperationManager.inst.DissatisfactionDialog();
        }
    }
    #endregion

    #region 增加英雄栏位
    void addHeroSlot(MainLineData data)
    {
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 0);

        bool isMax = RoleDataProxy.inst.JudgeHeroFieldIsMax;

        if (isMax)
        {
            GoOperationManager.inst.DissatisfactionDialog();
        }
        else
        {
            waitTargetPanelOpen(data);
        }
    }
    #endregion

    #region 查看英雄详情
    void clickHeroInfo(MainLineData data)
    {
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 0);

        var heroList = RoleDataProxy.inst.HeroList;

        if (heroList.Count > 0)
        {
            data.btnName = heroList[0].uid.ToString();
            waitTargetPanelOpen(data);
        }
        else
        {
            GoOperationManager.inst.DissatisfactionDialog();
        }
    }
    #endregion

    int shopperTimerId = 0;
    #region 点击顾客气泡
    void clickShopperPop(int targetEquipId)
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (ManagerBinder.inst.stateIsChanging)
        {
            timerId = GameTimer.inst.AddTimer(0.1f, () =>
            {
                if (!ManagerBinder.inst.stateIsChanging && /*IndoorRoleSystem.inst.GetAllShopperList().Count > 0*/GoOperationManager.inst.isInitShopperData)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                    clickShopperPop(targetEquipId);
                }
            });
        }
        else
        {
            bool notHave = true;
            var shopperDatas = ShopperDataProxy.inst.GetShopperList();
            // if (shopperDatas == null || shopperDatas.Count <= 0) return;
            int specialId = targetEquipId;
            targetEquipId = EquipConfigManager.inst.GetEquipDrawingsCfg(targetEquipId) == null ? targetEquipId > 0 ? -2 : -1 : targetEquipId;
            if (targetEquipId == -1)
            {
                var allShopper = IndoorRoleSystem.inst.GetAllShopperList();
                //if (allShopper == null || allShopper.Count <= 0) return;
                for (int i = 0; i < allShopper.Count; i++)
                {
                    int index = i;
                    if ((EShopperType)allShopper[index].shopperData.data.shopperType == EShopperType.Buy && allShopper[index].shopperData.data.shopperState == 99 && !allShopper[index].isMoving)
                    {
                        notHave = false;
                        this.target = allShopper[index].Attacher.spRoot.transform;
                        D2DragCamera.inst.LookToPosition(target.transform.position, cameraMoveEndToSetShopperFinger);
                        //setComponentEvent();
                        //setTargetTrans(this.target, true, K_Operation_Finger.ShopperPop);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < shopperDatas.Count; i++)
                {
                    int index = i;

                    if ((EShopperType)shopperDatas[index].data.shopperType == EShopperType.Buy && shopperDatas[index].data.shopperState == 99)
                    {
                        var equipQualityCfg = EquipConfigManager.inst.GetEquipQualityConfig(shopperDatas[index].data.targetEquipId);
                        var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipQualityCfg.equip_id);
                        if ((targetEquipId != -2 && equipQualityCfg.equip_id == targetEquipId) || (targetEquipId == -2 && equipDrawingCfg.sub_type == specialId))
                        {
                            notHave = false;
                            if (shopperTimerId > 0)
                            {
                                GameTimer.inst.RemoveTimer(shopperTimerId);
                                shopperTimerId = 0;
                            }

                            shopperTimerId = GameTimer.inst.AddTimer(0.1f, () =>
                            {
                                var target = IndoorRoleSystem.inst.GetShopperByUid(shopperDatas[index].data.shopperUid);
                                if (target != null && target.Attacher != null && target.Attacher.spRoot != null && !target.isMoving)
                                {
                                    this.target = target.Attacher.spRoot.transform;
                                    D2DragCamera.inst.LookToPosition(target.transform.position, cameraMoveEndToSetShopperFinger);
                                    //setComponentEvent();
                                    //setTargetTrans(this.target, true, K_Operation_Finger.ShopperPop);
                                    GameTimer.inst.RemoveTimer(shopperTimerId);
                                    shopperTimerId = 0;
                                }
                            });
                            break;
                        }
                    }
                }
            }

            if (notHave)
            {
                GoOperationManager.inst.DissatisfactionDialog();
                setTimerReset(false);
            }
        }
    }

    void cameraMoveEndToSetShopperFinger()
    {
        setComponentEvent();
        setTargetTrans(this.target, true, K_Operation_Finger.ShopperPop);
    }
    #endregion

    #region 打开指定宝箱
    void openTargetTbox(int boxId, MainLineData data)
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (ManagerBinder.inst.stateIsChanging)
        {
            timerId = GameTimer.inst.AddTimer(0.1f, () =>
            {
                if (!ManagerBinder.inst.stateIsChanging && TBoxManager.inst != null)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                    openTargetTbox(boxId, data);
                }
            });
        }
        else
        {
            var allTbox = TreasureBoxDataProxy.inst.boxList;
            if (allTbox == null || allTbox.Count <= 0) return;

            bool notBox = true;
            for (int i = 0; i < allTbox.Count; i++)
            {
                int index = i;
                if (allTbox[index].boxItemId == boxId)
                {
                    notBox = false;
                    TreasureBoxDataProxy.inst.newBoxGroupId = boxId;
                    EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.OPENTBOXUINOTPARA);
                }
            }

            if (notBox)
            {
                GoOperationManager.inst.DissatisfactionDialog();
                setTimerReset(false);
            }
            else
            {
                waitTargetPanelOpen(data);
            }
        }
    }
    #endregion

    #region 指定建筑投资
    void buildInvest(int buildId)
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (ManagerBinder.inst.stateIsChanging)
        {
            timerId = GameTimer.inst.AddTimer(0.1f, () =>
            {
                if (!ManagerBinder.inst.stateIsChanging && CityMap.inst != null && CityMap.inst.houseList.Count > 0)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                    buildInvest(buildId);
                }
            });
        }
        else
        {
            var allBuild = CityMap.inst.houseList;
            if (allBuild == null || allBuild.Count <= 0) return;

            buildId = buildId == -1 ? 1200 : buildId;

            for (int i = 0; i < allBuild.Count; i++)
            {
                int index = i;
                if (allBuild[index].houseID == buildId)
                {
                    target = allBuild[index].transform;
                    setComponentEvent();
                    setTargetTrans(target, true, K_Operation_Finger.BuildInvest);
                    return;
                }
            }

            GoOperationManager.inst.DissatisfactionDialog();
            setTimerReset(false);
        }
    }
    #endregion

    #region 科学院指定建筑投资
    void scienceBuildInvest()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (ManagerBinder.inst.stateIsChanging)
        {
            timerId = GameTimer.inst.AddTimer(0.1f, () =>
            {
                if (!ManagerBinder.inst.stateIsChanging && CityMap.inst != null && CityMap.inst.houseList.Count > 0)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                    scienceBuildInvest();
                }
            });
        }
        else
        {
            var allBuild = CityMap.inst.houseList;
            if (allBuild == null || allBuild.Count <= 0) return;

            for (int i = 0; i < allBuild.Count; i++)
            {
                int index = i;
                if (allBuild[index].houseID == 2300)
                {
                    if (null == allBuild) return;
                    target = allBuild[index].transform;
                    setComponentEvent();
                    setTargetTrans(target, true, K_Operation_Finger.BuildInvest);
                    return;
                }
            }

            GoOperationManager.inst.DissatisfactionDialog();
            setTimerReset(false);
        }
    }
    #endregion

    int targetEquipTimerId = 0;
    #region 选中指定的装备
    void selectTargetEquip(MainLineData data)
    {
        waitTargetPanelOpen(data);
    }
    #endregion

    #region 选中指定装备分页
    int resetEquipId = -1;
    void selectTargetEquipPage(int equipId)
    {
        if (EquipDataProxy.inst.GetEquipData(equipId) != null)
        {
            if(GUIManager.GetWindow<EquipListUIView>() != null && GUIManager.GetWindow<EquipListUIView>().isShowing)
            {
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_SHOWTARGETTYPE, equipId);
                GoOperationManager.inst.nextOperation(false);
            }
            else
            {
                resetEquipId = equipId;
            }
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("该图纸尚未解锁"), GUIHelper.GetColorByColorHex("FF2828"));
            setTimerReset(false);
        }
    }

    void resetSelectTargetEquipType()
    {
        if (GoOperationManager.inst.isDoing && resetEquipId != -1)
        {
            selectTargetEquipPage(resetEquipId);
            resetEquipId = -1;
        }
    }
    #endregion

    #region 选中指定工匠
    void selectTargetWorker(MainLineData data)
    {
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 1);
        waitTargetPanelOpen(data);
    }
    #endregion

    #region 选中指定职业英雄
    void selectTargetTransferHero(MainLineData data)
    {
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 0);
        int transferId = int.Parse(data.btnName);
        var cfg = HeroProfessionConfigManager.inst.GetConfig(transferId);
        if (cfg == null)
        {

            return;
        }
        var allHero = RoleDataProxy.inst.HeroList;
        if (allHero == null || allHero.Count <= 0) return;

        for (int i = 0; i < allHero.Count; i++)
        {
            int index = i;
            if (allHero[index].id == cfg.pre_profession)
            {
                data.btnName = allHero[index].uid.ToString();
                waitTargetPanelOpen(data);
                return;
            }
        }

        GoOperationManager.inst.DissatisfactionDialog();
        setTimerReset(false);
    }
    #endregion

    #region 选中指定副本
    MainLineData exploreData = null;
    void selectTargetExplore(MainLineData data)
    {
        if (GUIManager.GetWindow<ExplorePanelView>() != null && GUIManager.GetWindow<ExplorePanelView>().isShowing)
        {
            exploreData = null;
            int exploreId = int.Parse(data.btnName);
            var cfg = ExploreInstanceConfigManager.inst.GetConfigByDropid(exploreId);
            if (cfg == null)
            {
                Logger.error("输出 副本是空");
                GoOperationManager.inst.DissatisfactionDialog();
                return;
            };

            if(cfg.instance_type == 1)
            {
                if(exploreId == cfg.drop1_id)
                {
                    data.btnName = cfg.drop1_id.ToString();
                }
                else if(exploreId == cfg.drop2_id)
                {
                    data.btnName = cfg.drop2_id.ToString();
                }
                else if(exploreId == cfg.drop3_id)
                {
                    data.btnName = cfg.drop3_id.ToString();
                }
            }
            else
            {
                data.btnName = cfg.boss_id.ToString();
            }
            //data.btnName = (cfg.instance_type == 1 ? cfg.drop1_id : cfg.boss_id).ToString();
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.Explore_JumpToTargetExplore, cfg.instance_group);
            waitTargetPanelOpen(data);
        }
        else
        {
            exploreData = new MainLineData(data.panelName, data.btnName);
        }
    }

    void resetTargetExplore()
    {
        if (GoOperationManager.inst.isDoing && exploreData != null)
        {
            selectTargetExplore(exploreData);
            exploreData = null;
        }
    }
    #endregion

    int transferId;
    #region 选中指定转职分页
    void selectTargetTransferToggle(int heroId, MainLineData data)
    {
        if (transferId > 0)
        {
            GameTimer.inst.RemoveTimer(transferId);
            transferId = 0;
        }

        RoleTransferView view = GUIManager.GetWindow<RoleTransferView>();
        transferId = GameTimer.inst.AddTimer(0.1f, () =>
         {
             if (view != null && view.isShowing)
             {
                 EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLETRANSFERJUMPTOTOGGLE, heroId);
                 waitTargetPanelOpen(data);
                 GameTimer.inst.RemoveTimer(transferId);
                 transferId = 0;
             }

         });
    }
    #endregion

    #region 选择能招募的英雄
    void selectCanRecruitHero(MainLineData data)
    {
        var recruitHeroList = RoleDataProxy.inst.recruitList;
        if (recruitHeroList == null || recruitHeroList.Count <= 0) return;

        for (int i = 0; i < recruitHeroList.Count; i++)
        {
            int index = i;
            if ((ERecruitState)recruitHeroList[index].recruitState == ERecruitState.NotRecruited)
            {
                data.btnName = "hero" + (index + 1) + "-" + "recruitBtn";
                waitTargetPanelOpen(data);
                return;
            }
        }
        //data.btnName = "refreshBtn";
        //waitTargetPanelOpen(data);
        //GoOperationManager.inst.DissatisfactionDialog();
        //setTimerReset(false);
    }
    #endregion

    void setComponentEvent()
    {
        Button targetBtn = target.GetComponent<Button>();
        if (targetBtn != null)
        {
            targetBtn.onClick.RemoveListener(buttonClickToNext);
            targetBtn.onClick.AddListener(buttonClickToNext);
        }
        Toggle targetToggle = target.GetComponent<Toggle>();
        if (targetToggle != null)
        {
            targetToggle.onValueChanged.RemoveListener(toggleClickToNext);
            targetToggle.onValueChanged.AddListener(toggleClickToNext);
        }
        InputEventListener targetListener = target.GetComponent<InputEventListener>();
        if (targetListener != null)
        {
            targetListener.OnClick -= listenerClickToNext;
            targetListener.OnClick += listenerClickToNext;
        }
        EventTriggerListener triggerListener = target.GetComponent<EventTriggerListener>();
        if (triggerListener != null)
        {
            triggerListener.onClick -= triggerClickToNext;
            triggerListener.onClick += triggerClickToNext;
        }
    }

    void buttonClickToNext()
    {
        if (target == null) return;

        setTimerReset(false);
        var button = target.GetComponent<Button>();
        if (button != null)
            button.onClick.RemoveListener(buttonClickToNext);

        if (target.name == "城市按钮" || target.name == "店铺按钮")
        {
            GoOperationManager.inst.operationIsStart = 2;
        }
        GoOperationManager.inst.nextOperation();
    }

    void toggleClickToNext(bool isOn)
    {
        if (target == null) return;

        setTimerReset(false);
        var toggle = target.GetComponent<Toggle>();
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(toggleClickToNext);

        GoOperationManager.inst.nextOperation();
    }

    void listenerClickToNext(Vector3 v3)
    {
        if (target == null) return;

        setTimerReset(false);
        var listener = target.GetComponent<InputEventListener>();
        if (listener != null)
            listener.OnClick -= listenerClickToNext;

        GoOperationManager.inst.nextOperation();
    }

    void triggerClickToNext(GameObject go)
    {
        if (target == null) return;

        setTimerReset(false);
        var listener = target.GetComponent<EventTriggerListener>();
        if (listener != null)
            listener.onClick -= triggerClickToNext;

        GoOperationManager.inst.nextOperation();
    }
}
