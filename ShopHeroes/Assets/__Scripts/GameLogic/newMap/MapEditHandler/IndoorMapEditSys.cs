using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//商店室内地图系统
public partial class IndoorMapEditSys
{
    public static IndoorMapEditSys inst;

    ShopDesignUIView mDesignView;   //设计界面

    public IndoorMapEditSys()
    {
        inst = this;
        //XLuaManager.inst.HotfixSystem(GetType().Name, this);
        XLuaManager.inst.HotfixRaw(GetType().Name, this);
    }
    public void EnterSystem()
    {
        //添加消息监听
        AddListeners();
        AddListeners_EditMode();
        AddListeners_Furniture();
        AddListeners_Shelf();
        AddListeners_Shopkeeper();

        indoorLoaded = false;
        //场景家在完成 获取商店数据
        UserDataProxy.inst.requestLayout();
    }

    public void ExitSystem()
    {
        //删除消息监听
        RemoveListeners();
        RemoveListeners_EditMode();
        RemoveListeners_Furniture();
        RemoveListeners_Shelf();
        RemoveListeners_Shopkeeper();
    }

    private void AddListeners()
    {
        var eventclr = EventController.inst;
        eventclr.AddListener(GameEventType.ShopDesignEvent.ServerData_Ready, serverdataReady);
        eventclr.AddListener<string>(GameEventType.Map2dEvent.IndoorInitEnd, IndoorMapInitEnd);
        eventclr.AddListener(GameEventType.TOUCHEVENT_OnPointBlank, onTouchPointBlank);
        eventclr.AddListener<int>(GameEventType.TOUCHEVENT_OnPointClick, onpointSceneObject);
        eventclr.AddListener(GameEventType.ShopDesignEvent.CameraMove_CheckExtendUpOrFurnitureUpEnd, CameraMove_CheckExtendUpOrFurnitureUpEnd);
    }
    private void RemoveListeners()
    {
        var eventclr = EventController.inst;
        eventclr.RemoveListener(GameEventType.ShopDesignEvent.ServerData_Ready, serverdataReady);
        eventclr.RemoveListener<string>(GameEventType.Map2dEvent.IndoorInitEnd, IndoorMapInitEnd);
        eventclr.RemoveListener(GameEventType.TOUCHEVENT_OnPointBlank, onTouchPointBlank);
        eventclr.RemoveListener<int>(GameEventType.TOUCHEVENT_OnPointClick, onpointSceneObject);
        eventclr.RemoveListener(GameEventType.ShopDesignEvent.CameraMove_CheckExtendUpOrFurnitureUpEnd, CameraMove_CheckExtendUpOrFurnitureUpEnd);
    }
    bool indoorLoaded = false;
    private void serverdataReady()
    {
        if (indoorLoaded) return;
        indoorLoaded = true;
        if (IndoorMap.inst != null)
            IndoorMap.inst.CreateIndoorRoom(AccountDataProxy.inst.userId, UserDataProxy.inst.shopData, false);
    }

    private void onpointSceneObject(int objlayer)
    {
        if (currEntityUid > 0 && shopDesignMode == DesignMode.normal)
        {
            if (objlayer != LayerMask.NameToLayer("furniture"))
            {
                onTouchPointBlank();
            }
        }
    }
    void onTouchPointBlank()
    {
        if (shopDesignMode == DesignMode.normal && currEntityUid > 0)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.normal, IndoorMap.tempItemUid);
        }
        else if (shopDesignMode == DesignMode.FurnitureEdit)
        {
            if (currEntityUid == IndoorMap.tempItemUid)//新创建的
            {
            }
            else if (IndoorMap.inst.currSelectEntity != null && UserDataProxy.inst.GetFuriture(IndoorMap.inst.currSelectEntity.uid).state == 3) //库存里拿出来的新创建的
            {
            }
            else if (IndoorMap.inst.currSelectEntity != null && IndoorMap.inst.currSelectEntity.isPickUp) //目标家具被抬起
            {
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
            }
        }
    }
    //是否在自己商店内
    public bool isSelfShop
    {
        get
        {
            if (IndoorMap.inst == null) return false;
            return AccountDataProxy.inst.userId == IndoorMap.inst.currUserId;
        }
    }

    //室内场景创建完成
    void IndoorMapInitEnd(string userid)
    {
        if (!isSelfShop)
        {
            //进入访问逻辑

            return;
        }

        //获取顾客数据
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_GETSHOPPERLIST);
        ////获取宠物数据
        //EventController.inst.TriggerEvent(GameEventType.PetCompEvent.REQUEST_PET_INFO);
        //创建map店主

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.m_curCfg != null)
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver && ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.NPCCreat || (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick))
            {
                if ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.NPCCreat)
                {
                    GuideManager.inst.isInit = true;
                }
                if (GuideManager.inst.waitStart)
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
            }
        }

        //创建宠物
        EventController.inst.TriggerEvent(GameEventType.PetCompEvent.PETDATA_GETEND);

        //创建游客
        EventController.inst.TriggerEvent(GameEventType.StreetDropEvent.PASSBY_INITCREATE);

        //创建可招募的工匠
        EventController.inst.TriggerEvent(GameEventType.IndoorRole_CanLockWorkerCompEvent.CHECK_CANLOCKWORKER);

        //创建店主
        //SpawnShopkeeper();
        HotfixBridge.inst.TriggerLuaEvent("SPAWNSHOPKEEPER");

        GameTimer.inst.AddTimer(0.3f, 1, () =>
        {
            if (IndoorEnvironmentObjVisibleClr.inst != null)
            {
                IndoorEnvironmentObjVisibleClr.inst.setVisible(true);
            }
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOWTOPINFOUI);
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPSCENE);

            HotfixBridge.inst.TriggerLuaEvent("RefreshShowUI_GuideTask");

            GameTimer.inst.AddTimer(1, 1, () =>
            {
                if (GuideManager.inst.waitStart)
                {
                    GuideManager.inst.waitStart = false;
                    if (!GuideDataProxy.inst.CurInfo.isAllOver)
                    {
                        if (AccountDataProxy.inst.needCreatRole)
                        {
                            if (WorldParConfigManager.inst.GetConfig(127).parameters == 1)
                            {

                                FGUI.inst.BlackMaskAnimation(() => FGUI.inst.SetLoopJumpLoadingAnim(false), () =>
                                {
                                    float tempStartX = FGUI.inst.isLandscape ? -8.9f : -14;
                                    float tempStartY = FGUI.inst.isLandscape ? 7.5f : 9.8f;
                                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, GuideDataProxy.inst.CurInfo.m_curCfg.id);
                                    Camera.main.transform.DOMove(new Vector3(tempStartX, tempStartY, 0), 4).SetUpdate(UpdateType.Normal).OnComplete(() =>
                                    {
                                        Camera.main.transform.position = new Vector3(-10, 6.75f, 0);
                                        Camera.main.orthographicSize = 4.5f;
                                        D2DragCamera.inst.updateCameMaxZoom(UserDataProxy.inst.shopData.shopLevel, kCameraMoveType.shopExtend);
                                        IndoorMap.inst.indoorMask.SetActiveTrue();
                                        AccountDataProxy.inst.needCreatRole = false;
                                        GuideManager.inst.waitNetworkBack();

                                        //Camera.main.transform.DOMove(new Vector3(-7.8f, 9.8f, 0), 2).SetUpdate(UpdateType.Normal).SetEase(Ease.InOutSine).OnComplete(() =>
                                        //{
                                        //    var cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopExtend, UserDataProxy.inst.shopData.shopLevel);
                                        //    D2DragCamera.inst.maxZoom = cfg.zoom1 / 100f;
                                        //    IndoorMap.inst.indoorMask.SetActiveTrue();
                                        //    Camera.main.transform.DOMove(new Vector3(-12.3f, 5.6f, 0), 1.6f).SetUpdate(UpdateType.Normal).SetEase(Ease.OutSine).OnComplete(() =>
                                        //    {
                                        //        AccountDataProxy.inst.needCreatRole = false;
                                        //        GuideManager.inst.waitNetworkBack();
                                        //    });
                                        //    Camera.main.DOOrthoSize(4.5f, 1.6f).SetUpdate(UpdateType.Normal).SetEase(Ease.OutSine);
                                        //});
                                    });
                                }, 0, 1.6f);
                            }
                            else
                            {
                                IndoorMap.inst.indoorMask.SetActiveTrue();
                                EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, GuideDataProxy.inst.CurInfo.m_curCfg.id);
                                FGUI.inst.BlackMaskAnimation(() => FGUI.inst.SetLoopJumpLoadingAnim(false), () =>
                                 {
                                     FGUI.inst.SetLoopJumpLoadingAnim(false);
                                     AccountDataProxy.inst.needCreatRole = false;
                                     GuideManager.inst.waitNetworkBack();
                                 }, 0, 1.6f);
                            }
                        }
                        else
                        {
                            IndoorMap.inst.indoorMask.SetActiveTrue();
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, GuideDataProxy.inst.CurInfo.m_curCfg.id);
                        }
                    }
                }
            });
        });

        //镜头移动

        GameTimer.inst.AddTimer(1f, 1, () =>
        {
            HotfixBridge.inst.TriggerLuaEvent("EventSystem_AddEvent", "Design_Levelup_Event", "", 2, (int)kGameState.Shop);
        });
    }

    //显示界面
    void ShowDesignView()
    {
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_ShopDesignUI");

        //mDesignView = GUIManager.OpenView<ShopDesignUIView>();
    }
    void HideDesignView()
    {
        HotfixBridge.inst.TriggerLuaEvent("HideUI_ShopDesignUI");
        //GUIManager.HideView<ShopDesignUIView>();
    }

    void DesignViewRefresh()
    {
        //if (mDesignView == null || !mDesignView.isShowing)
        //{
        //    ShowDesignView();
        //    return;
        //}
        //mDesignView.Refresh();

        HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Refresh");

    }


    void cameraMoveCheckExtendUpOrFurnitureUpEnd()
    {
        if (!GuideDataProxy.inst.CurInfo.isAllOver) return;

        //先判断扩建
        EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;

        if (state == EDesignState.Finished)
        {
            if (IndoorMap.inst.extension == null) return;
            BuildingSite aa = IndoorMap.inst.extension.gameObject.GetComponent<BuildingSite>();

            CameraMoveConfig cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopExtend, UserDataProxy.inst.shopData.shopLevel);

            Vector3 pos = new Vector3(aa.transform.position.x + cfg.X_revise, aa.transform.position.y + cfg.Y_revise, 0);
            D2DragCamera.inst.LookToPosition(pos, false, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1), canBreak: true);
            return;
        }


        //再家具
        int curUpgradeFurnitureUid = UserDataProxy.inst.GetCurrentUpgradefurniture();

        if (curUpgradeFurnitureUid == 0)
        {
            return;
        }
        else
        {
            if (UserDataProxy.inst.GetFuriture(curUpgradeFurnitureUid).state == (int)EDesignState.Finished)//为升级完成
            {
                if (IndoorMap.inst.GetFurnituresByUid(curUpgradeFurnitureUid, out Furniture furniture))
                {
                    CameraMoveConfig cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.furnitureUp);
                    Vector3 pos = new Vector3(furniture.transform.position.x + cfg.X_revise, furniture.transform.position.y + cfg.Y_revise);
                    D2DragCamera.inst.LookToPosition(pos, false, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1), canBreak: true);
                }
            }
        }
    }

    void CameraMove_CheckExtendUpOrFurnitureUpEnd()
    {

        if (GUIManager.GetCurrWindowViewID() == ViewPrefabName.MainUI && GUIManager.CurrWindow != null && GUIManager.CurrWindow.viewID == ViewPrefabName.MainUI)
        {
            cameraMoveCheckExtendUpOrFurnitureUpEnd();
        }
    }

}
