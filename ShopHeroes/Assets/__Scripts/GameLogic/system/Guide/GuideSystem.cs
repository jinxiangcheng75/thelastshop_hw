using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSystem : BaseSystem
{
    GuideView _guideView;
    protected override void AddListeners()
    {
        base.AddListeners();
        EventController.inst.AddListener<K_Guide_UI>(GameEventType.GuideEvent.SHOWGUIDEUI, showGuideByType);
        EventController.inst.AddListener<K_Guide_UI>(GameEventType.GuideEvent.HIDEGUIDEUI, hideGuideByType);
        EventController.inst.AddListener(GameEventType.GuideEvent.REALHIDEGUIDEUI, realHideGuide);
        EventController.inst.AddListener<Transform, int, bool>(GameEventType.GuideEvent.SETFINGERPOS, setFingerPos);
        EventController.inst.AddListener<Transform, bool>(GameEventType.GuideEvent.SETPROMPTPOS, setPromptPos);
        EventController.inst.AddListener(GameEventType.GuideEvent.NPCCREAT, npcCreat);
        EventController.inst.AddListener(GameEventType.GuideEvent.FINGERACTIVEFALSE, fingerActiveFalse);
        EventController.inst.AddListener<GameObject>(GameEventType.GuideEvent.SETTARGET, setTarget);
        EventController.inst.AddListener(GameEventType.GuideEvent.SETNPCMOVE, setNpcMove);
        EventController.inst.AddListener(GameEventType.GuideEvent.NPCLEAVE, npcLeave);
        EventController.inst.AddListener<Transform>(GameEventType.GuideEvent.SETTRIGGERMASKTARGET, setTriggerMaskTarget);
        EventController.inst.AddListener<bool>(GameEventType.GuideEvent.WAITPREMASK, showPreMask);
        EventController.inst.AddListener<bool>(GameEventType.GuideEvent.SETNETWORKMASK, setNetworkMask);
        EventController.inst.AddListener(GameEventType.GuideEvent.REFRESHTASK, refreshTask);
        EventController.inst.AddListener<bool>(GameEventType.GuideEvent.SETGNEWTASK, setGNewTask);
        EventController.inst.AddListener<GameObject>(GameEventType.GuideEvent.SETSLOTTARGET, setSlotTarget);
        EventController.inst.AddListener<string, string, bool, bool>(GameEventType.GuideEvent.SETTRIGGERMASK, setTriggerMask);
        EventController.inst.AddListener(GameEventType.GuideEvent.HIDEALLSUBPANEL, hideAllSubPanel);

        EventController.inst.AddListener<int>(GameEventType.GuideEvent.REQUEST_SETGUIDE, requestSetGuide);
        EventController.inst.AddListener(GameEventType.GuideEvent.REQUEST_SKIPGUIDE, requestSkipGuide);

        EventController.inst.AddListener(GameEventType.GuideEvent.HIDESKIPBTN, hideSkipBtn);

        EventController.inst.AddListener(GameEventType.NETWORK_RELINK, reSendGuideId);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener<K_Guide_UI>(GameEventType.GuideEvent.SHOWGUIDEUI, showGuideByType);
        EventController.inst.RemoveListener<K_Guide_UI>(GameEventType.GuideEvent.HIDEGUIDEUI, hideGuideByType);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.REALHIDEGUIDEUI, realHideGuide);
        EventController.inst.RemoveListener<Transform, int, bool>(GameEventType.GuideEvent.SETFINGERPOS, setFingerPos);
        EventController.inst.RemoveListener<Transform, bool>(GameEventType.GuideEvent.SETPROMPTPOS, setPromptPos);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.NPCCREAT, npcCreat);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.FINGERACTIVEFALSE, fingerActiveFalse);
        EventController.inst.RemoveListener<GameObject>(GameEventType.GuideEvent.SETTARGET, setTarget);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.SETNPCMOVE, setNpcMove);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.NPCLEAVE, npcLeave);
        EventController.inst.RemoveListener<Transform>(GameEventType.GuideEvent.SETTRIGGERMASKTARGET, setTriggerMaskTarget);
        EventController.inst.RemoveListener<bool>(GameEventType.GuideEvent.WAITPREMASK, showPreMask);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.REFRESHTASK, refreshTask);
        EventController.inst.RemoveListener<bool>(GameEventType.GuideEvent.SETGNEWTASK, setGNewTask);
        EventController.inst.RemoveListener<bool>(GameEventType.GuideEvent.SETNETWORKMASK, setNetworkMask);
        EventController.inst.RemoveListener<GameObject>(GameEventType.GuideEvent.SETSLOTTARGET, setSlotTarget);
        EventController.inst.RemoveListener<string, string, bool, bool>(GameEventType.GuideEvent.SETTRIGGERMASK, setTriggerMask);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.HIDEALLSUBPANEL, hideAllSubPanel);

        EventController.inst.RemoveListener<int>(GameEventType.GuideEvent.REQUEST_SETGUIDE, requestSetGuide);
        EventController.inst.RemoveListener(GameEventType.GuideEvent.REQUEST_SKIPGUIDE, requestSkipGuide);

        EventController.inst.RemoveListener(GameEventType.GuideEvent.HIDESKIPBTN, hideSkipBtn);

        EventController.inst.RemoveListener(GameEventType.NETWORK_RELINK, reSendGuideId);
    }

    void reSendGuideId()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.m_curCfg != null)
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, GuideDataProxy.inst.CurInfo.m_curCfg.id);
    }

    void hideSkipBtn()
    {
        if (_guideView != null)
        {
            _guideView.setSkipBtnNotActive();
        }
    }

    void hideAllSubPanel()
    {
        if (_guideView != null)
        {
            _guideView.hideAllSubPanel();
        }
    }

    #region 触发式

    void setTriggerMask(string panelName, string btnName, bool needWait, bool isStrong)
    {
        GUIManager.OpenView<GuideView>((view) =>
        {
            _guideView = view;
            view.setTriggerMask(panelName, btnName, needWait, isStrong);
        });
    }
    #endregion

    void setSlotTarget(GameObject target)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setSlotTarget(target);
        }
    }

    void showGuideByType(K_Guide_UI type)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.Guide_OnUIOpen(type);
        }
        else
        {
            GUIManager.OpenView<GuideView>((view) =>
            {
                _guideView = view;
                view.Guide_OnUIOpen(type);
            });
        }
    }

    void hideGuideByType(K_Guide_UI type)
    {
        if (_guideView != null)
            _guideView.Guide_OnUIHide(type);
    }

    void realHideGuide()
    {
        GUIManager.HideView<GuideView>();
    }

    void refreshTask()
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.refreshTaskData();
        }
        else
        {
            GUIManager.OpenView<GuideView>((view) =>
            {
                _guideView = view;
                view.refreshTaskData();
            });
        }
    }

    void setGNewTask(bool isOk)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setNewTask(isOk);
        }
    }

    void showPreMask(bool activeState)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setPreMask(activeState);
        }
        else
        {
            GUIManager.OpenView<GuideView>((view) =>
            {
                _guideView = view;
                view.setPreMask(activeState);
            });
        }
    }

    void setNetworkMask(bool isActive)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            // _guideView.setNetworkMask(isActive);
        }
    }
    void setFingerPos(Transform btnTrans, int size, bool needSetParent = false)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setFingerPos(btnTrans, size, needSetParent);
        }
        else
        {
            GUIManager.OpenView<GuideView>((view) =>
            {
                _guideView = view;
                view.setFingerPos(btnTrans, size, needSetParent);
            });
        }
    }

    void setPromptPos(Transform btnTrans, bool active)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setPromptPos(btnTrans, active);
        }
        else
        {
            GUIManager.OpenView<GuideView>((view) =>
            {
                _guideView = view;
                view.setPromptPos(btnTrans, active);
            });
        }
    }

    int creatTimerId = 0;
    void npcCreat()
    {
        if (GuideDataProxy.inst == null) return;
        if (GuideDataProxy.inst.CurInfo == null) return;
        if (GuideDataProxy.inst.CurInfo.m_curCfg == null) return;
        if (GuideManager.inst.isInit)
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
            GuideManager.inst.isInit = false;
        }
        var curInfo = GuideDataProxy.inst.CurInfo;
        //if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.Shopkeeper != null)
        //    IndoorMapEditSys.inst.Shopkeeper.gameObject.SetActive(true);
        HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperObjActive", true);

        if (GuideDataProxy.inst.GetNpcById(curInfo.m_curCfg.character_id) == null)
        {
            bool needShowPrompt = curInfo.m_curCfg.conditon_param_5 != "0";
            var curWorker = ArtisanNPCConfigManager.inst.GetConfig(curInfo.m_curCfg.character_id);
            string[] pos = curInfo.m_curCfg.conditon_param_1.Split('|');
            RoleDirectionType dir = RoleDirectionType.Left;
            if (curInfo.m_curCfg.conditon_param_2 != null)
            {
                dir = int.Parse(curInfo.m_curCfg.conditon_param_2) == 0 ? RoleDirectionType.Left : RoleDirectionType.Right;
            }
            int npcX = 0, npcY = 0;
            if (pos.Length > 0)
                npcX = int.Parse(pos[0]);
            if (pos.Length > 1)
                npcY = int.Parse(pos[1]);
            Vector3Int creatPos = new Vector3Int(npcX, npcY, 0);
            if (IndoorMap.inst == null)
            {
                creatTimerId = GameTimer.inst.AddTimer(1.2f, () =>
                {
                    if (IndoorMap.inst != null)
                    {
                        if (curInfo.m_curCfg.conditon_param_4 != null)
                        {
                            //if (IndoorMapEditSys.inst.Shopkeeper != null)
                            //{
                            //    var spkeeperPos = curInfo.m_curCfg.conditon_param_4.Split('|');
                            //    RoleDirectionType spDir = RoleDirectionType.Left;
                            //    if (curInfo.m_curCfg.conditon_param_3 != null)
                            //        spDir = int.Parse(curInfo.m_curCfg.conditon_param_3) == 0 ? RoleDirectionType.Left : RoleDirectionType.Right;
                            //    IndoorMapEditSys.inst.Shopkeeper.SetState((int)MachineShopkeeperState.inGuide);
                            //    int spX = 0, spY = 0;
                            //    if (spkeeperPos.Length > 0)
                            //        spX = int.Parse(spkeeperPos[0]);
                            //    if (spkeeperPos.Length > 1)
                            //        spY = int.Parse(spkeeperPos[1]);
                            //    IndoorMapEditSys.inst.Shopkeeper.SetCellPos(new Vector3Int(spX, spY, 0));
                            //    IndoorMapEditSys.inst.Shopkeeper.UpdateSortingOrder();
                            //    if (IndoorMapEditSys.inst.Shopkeeper.Character != null)
                            //        IndoorMapEditSys.inst.Shopkeeper.Character.SetDirection(spDir);
                            //}
                            var spkeeperPos = curInfo.m_curCfg.conditon_param_4.Split('|');
                            RoleDirectionType spDir = RoleDirectionType.Left;
                            if (curInfo.m_curCfg.conditon_param_3 != null)
                                spDir = int.Parse(curInfo.m_curCfg.conditon_param_3) == 0 ? RoleDirectionType.Left : RoleDirectionType.Right;
                            int spX = 0, spY = 0;
                            if (spkeeperPos.Length > 0)
                                spX = int.Parse(spkeeperPos[0]);
                            if (spkeeperPos.Length > 1)
                                spY = int.Parse(spkeeperPos[1]);

                            HotfixBridge.inst.TriggerLuaEvent("GuideSystem_CreateShopkeeper", spX,spY,spDir);
                        }
                        creat(curWorker, creatPos, needShowPrompt, dir);
                        GameTimer.inst.RemoveTimer(creatTimerId);
                    }
                });
            }
            else
            {
                if (curInfo.m_curCfg.conditon_param_4 != null)
                {
                    //if (IndoorMapEditSys.inst.Shopkeeper != null)
                    //{
                    //    var spkeeperPos = curInfo.m_curCfg.conditon_param_4.Split('|');
                    //    RoleDirectionType spDir = curInfo.m_curCfg.conditon_param_3 == null ? RoleDirectionType.Left : int.Parse(curInfo.m_curCfg.conditon_param_3) == 0 ? RoleDirectionType.Left : RoleDirectionType.Right;
                    //    IndoorMapEditSys.inst.Shopkeeper.SetState((int)MachineShopkeeperState.inGuide);
                    //    int spX = 0, spY = 0;
                    //    if (spkeeperPos.Length > 0)
                    //        spX = int.Parse(spkeeperPos[0]);
                    //    if (spkeeperPos.Length > 1)
                    //        spY = int.Parse(spkeeperPos[1]);
                    //    IndoorMapEditSys.inst.Shopkeeper.SetCellPos(new Vector3Int(spX, spY, 0));
                    //    IndoorMapEditSys.inst.Shopkeeper.UpdateSortingOrder();
                    //    if (IndoorMapEditSys.inst.Shopkeeper.Character != null)
                    //        IndoorMapEditSys.inst.Shopkeeper.Character.SetDirection(spDir);
                    //}

                    var spkeeperPos = curInfo.m_curCfg.conditon_param_4.Split('|');
                    RoleDirectionType spDir = RoleDirectionType.Left;
                    if (curInfo.m_curCfg.conditon_param_3 != null)
                        spDir = int.Parse(curInfo.m_curCfg.conditon_param_3) == 0 ? RoleDirectionType.Left : RoleDirectionType.Right;
                    int spX = 0, spY = 0;
                    if (spkeeperPos.Length > 0)
                        spX = int.Parse(spkeeperPos[0]);
                    if (spkeeperPos.Length > 1)
                        spY = int.Parse(spkeeperPos[1]);

                    HotfixBridge.inst.TriggerLuaEvent("GuideSystem_CreateShopkeeper", spX, spY, spDir);

                }
                creat(curWorker, creatPos, needShowPrompt, dir);
            }
        }
        else
        {
            GuideDataProxy.inst.CurInfo.isCreatFinish = true;
            GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.CreatNpcFinish, 0);
        }
    }

    void creat(ArtisanNPCConfigData curWorker, Vector3Int creatPos, bool needShowPrompt, RoleDirectionType dir)
    {
        GameObject npcPbj = GameObject.Instantiate(IndoorMap.inst.npcPfb);
        var npc = npcPbj.AddComponent<NpcController>();
        npc.npcId = curWorker.id;
        GuideDataProxy.inst.AddNpc(npc);
        if (GuideManager.inst.curNpc != null)
        {
            GuideManager.inst.lastNpc = GuideManager.inst.curNpc;
        }
        GuideManager.inst.curNpc = npcPbj;

        npc.setData(curWorker.model, 0.14f, creatPos, needShowPrompt, dir);
    }

    int leaveTimer = 0;
    Stack<PathNode> leavePath = new Stack<PathNode>();
    void npcLeave()
    {
        if (GuideManager.inst.lastNpc != null)
        {
            if (leaveTimer == 0)
            {
                leavePath.Clear();
                var npc = GuideManager.inst.lastNpc.GetComponent<NpcController>();
                GuideDataProxy.inst.RemoveNpc(npc.npcId);
                leavePath = IndoorMap.inst.FindPath(npc.currCellPos, new Vector3Int(-3, -9, 0));
                npc.move(leavePath);
                leaveTimer = GameTimer.inst.AddTimer(1, () =>
                 {
                     if (!npc.isMoving)
                     {
                         GameObject.DestroyImmediate(GuideManager.inst.lastNpc);
                         GuideManager.inst.lastNpc = null;
                         GameTimer.inst.RemoveTimer(leaveTimer);
                         leaveTimer = 0;
                     }
                 });
            }
        }
    }

    void setTriggerMaskTarget(Transform trans)
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setTriggerMaskTarget(trans);
        }
        else
        {
            GUIManager.OpenView<GuideView>((view) =>
            {
                _guideView = view;
                view.setTriggerMaskTarget(trans);
            });
        }
    }

    void fingerActiveFalse()
    {
        if (_guideView != null && _guideView.isShowing)
        {
            _guideView.setFingerFalse();
        }
    }

    void setTarget(GameObject target)
    {
        //GuideManager.inst.setTarget(target);
    }

    int timerId = 0;
    Stack<PathNode> movepath = new Stack<PathNode>();
    Stack<PathNode> spmovepath = new Stack<PathNode>();
    Stack<PathNode> spiecalpath = new Stack<PathNode>();
    void setNpcMove()
    {
        movepath.Clear();
        spmovepath.Clear();
        spiecalpath.Clear();

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (GuideManager.inst.curNpc != null)
        {
            var g_Info = GuideDataProxy.inst.CurInfo;
            bool isNeedSpFollow = int.Parse(g_Info.m_curCfg.conditon_param_2) == 1;

            //var spkeeper = IndoorMapEditSys.inst.Shopkeeper;
            NpcController npc = GuideManager.inst.curNpc.GetComponent<NpcController>();
            NpcController lastNpc = new NpcController();

            string[] pos = g_Info.m_curCfg.conditon_param_3.Split('|');
            Vector3Int npcEndPos = new Vector3Int(int.Parse(pos[0]), int.Parse(pos[1]), 0);
            movepath = IndoorMap.inst.FindPath(npc.currCellPos, npcEndPos);
            npc.move(movepath);
            if (isNeedSpFollow)
            {
                Vector3Int spkeeperEndPos = Vector3Int.zero;
                pos = g_Info.m_curCfg.conditon_param_4.Split('|');
                spkeeperEndPos = new Vector3Int(int.Parse(pos[0]), int.Parse(pos[1]), 0);
                //spmovepath = IndoorMap.inst.FindPath(spkeeper.currCellPos, spkeeperEndPos);

                //spkeeper.move(spmovepath);
                HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperMove", spkeeperEndPos);
            }

            cameraZoomInNpc(npc.gameObject);

            if (g_Info.m_curCfg.id == 403)
            {
                lastNpc = GuideManager.inst.lastNpc.GetComponent<NpcController>();
                //spmovepath = IndoorMap.inst.FindPath(spkeeper.currCellPos, IndoorMap.inst.GetCounterOperationPos());
                //spkeeper.move(spmovepath);
                HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperMove", IndoorMap.inst.GetCounterOperationPos());

                spiecalpath = IndoorMap.inst.FindPath(lastNpc.currCellPos, new Vector3Int(11, 23, 0));
                lastNpc.move(spiecalpath);
            }

            timerId = GameTimer.inst.AddTimer(0.5f, () =>
            {
                if (npc == null)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    return;
                }

                if (g_Info.m_curCfg.id == 403)
                {
                    if (!lastNpc.isMoving && !HotfixBridge.inst.GetShopkeeperIsMoving() /*!spkeeper.isMoving*/)
                    {
                        HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperDirection", RoleDirectionType.Left);
                        //spkeeper.Character.SetDirection(RoleDirectionType.Left);
                        lastNpc.ChangeRoleDirection(RoleDirectionType.Left);
                    }
                }

                if (!npc.isMoving && !HotfixBridge.inst.GetShopkeeperIsMoving() /*!spkeeper.isMoving*/ )
                {
                    D2DragCamera.inst.npcIsMoving = false;
                    //var attacher = npc.GetComponent<ActorAttacher>();
                    if (null != npc.Attacher && !npc.promptIsShow)
                    {
                        AtlasAssetHandler.inst.GetAtlasSprite("guide_atlas", "xinshou_tanhao", (sp) =>
                         {
                             Color qcol = Color.white;
                             npc.ShowSpPop(sp.sprite, 1, false, false, in qcol, false, 1.25f);
                         });

                        //attacher.showPromptPopup();
                        npc.promptIsShow = true;
                        //GameTimer.inst.AddTimer(0.4f, 1, () =>
                        //  {
                        EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, npc.Attacher.spRoot.transform, 150, false);
                        //});
                    }
                    if (g_Info.m_curCfg.id == 902 || g_Info.m_curCfg.id == 105)
                    {
                        //spkeeper.Character.SetDirection(RoleDirectionType.Left);
                        HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperDirection", RoleDirectionType.Left);
                        npc.ChangeRoleDirection(RoleDirectionType.Right);
                    }
                    g_Info.isArriveTarget = true;
                    GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.MoveToTarget, 0);
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                }
            });
        }
    }

    private void cameraZoomInNpc(GameObject npcObj)
    {
        GameTimer.inst.AddTimer(0.3f, 1, () =>
        {
            // 镜头切近
            D2DragCamera.inst.SetNpc(npcObj);
        });
    }
    int globalGuideId;
    int loopTimerId = 0;
    int calculateTime = 0;
    void requestSetGuide(int guideId)
    {
        globalGuideId = guideId;
        GuideManager.inst.isLoop = true;
        requestGuideData(globalGuideId);
        calculateTime = 0;
        if (loopTimerId == 0)
        {
            loopTimerId = GameTimer.inst.AddTimer(1, () =>
             {
                 if (GuideManager.inst.isLoop)
                 {
                     calculateTime += 1;
                     if (calculateTime >= 3)
                         requestGuideData(globalGuideId);
                 }
                 else
                 {
                     calculateTime = 0;
                 }
                 if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
                 {
                     GameTimer.inst.RemoveTimer(loopTimerId);
                     loopTimerId = 0;
                 }
             });
        }
    }

    void requestGuideData(int guideId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_SetGuide()
            {
                guideId = guideId
            }
        });
    }

    void requestSkipGuide()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_SkipGuide()
        });
    }
}
