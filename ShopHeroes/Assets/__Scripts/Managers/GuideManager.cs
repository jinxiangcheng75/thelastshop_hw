using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideManager : TSingletonHotfix<GuideManager>
{
    private GuideInfo m_gInfo;
    private GuideConfigData subCfg = new GuideConfigData();
    public bool waitStart;
    private int timerId = 0;
    private int waitPanelId = 0;
    private int waitShopper = 0;
    private GameObject subTarget;
    public GameObject curNpc;
    public GameObject lastNpc;
    private bool firstIsNotWait = false;
    public bool secondIsNotWait = false;
    public bool isInit = false;
    private bool isWaitHeroRestingTime = false;
    public bool isLoop = false;
    public bool isInTriggerGuide;
    public string triggerFurnOffset;
    public int CurrEquipId;
    public GameObject curTargetBtn;
    public UnityEngine.Events.UnityAction curFunc;

    protected override void init()
    {
        m_gInfo = GuideDataProxy.inst.CurInfo;
    }

    private bool levelCheck(int targetLv)
    {
        return UserDataProxy.inst.playerData.level >= targetLv;
    }

    private bool equipNumCheck(int targetNum)
    {
        return m_gInfo.val >= targetNum;
    }

    public void GuideManager_OnNextGuide()
    {
        //Logger.error("nextguide" + m_gInfo.m_curCfg.id);
        // if (!GuideDataProxy.inst.isGetNetworkd) return;
        if (m_gInfo == null) return;

        if (m_gInfo.isAllOver) return;

        if ((K_Guide_End)m_gInfo.m_curCfg.end_type == K_Guide_End.ClickTarget)
        {
            if (!m_gInfo.isClickTarget) return;
            m_gInfo.isClickTarget = false;
        }

        if ((K_Guide_End)m_gInfo.m_curCfg.end_type == K_Guide_End.MoveToTarget)
        {
            if (!m_gInfo.isArriveTarget) return;
            m_gInfo.isArriveTarget = false;
        }

        if ((K_Guide_End)m_gInfo.m_curCfg.end_type == K_Guide_End.CreatNpcFinish)
        {
            if (!m_gInfo.isCreatFinish) return;
            m_gInfo.isCreatFinish = false;
        }

        if ((K_Guide_End)m_gInfo.m_curCfg.end_type == K_Guide_End.DialogFinish)
        {
            if (!m_gInfo.isDialogFinish) return;
            m_gInfo.isDialogFinish = false;
        }

        if ((K_Guide_End)m_gInfo.m_curCfg.end_type == K_Guide_End.ArriveLevel)
        {
            if (!levelCheck(m_gInfo.m_curCfg.end_param))
            {
                return;
            }
        }

        if ((K_Guide_End)m_gInfo.m_curCfg.end_param == K_Guide_End.ClickCountArrive)
        {
            if (m_gInfo.val < m_gInfo.m_curCfg.end_param) return;
            m_gInfo.val = 0;
        }
        var nextCfg = GuideConfigManager.inst.GetNextConfiData(m_gInfo.m_curGroup, m_gInfo.m_curIndex);
        if (null != nextCfg)
        {
            //Logger.error("关掉预遮罩界面");
            //EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
            if ((K_Guide_Type)nextCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)nextCfg.guide_type == K_Guide_Type.TipsAndRestrictClick || (K_Guide_Type)nextCfg.guide_type == K_Guide_Type.RestrictAndJudgeHeroTime || (K_Guide_Type)nextCfg.guide_type == K_Guide_Type.NPCCreat)
            {
                if (nextCfg.id != 4201 && nextCfg.id != 4902)
                {
                    //Logger.error("打开预遮罩界面" + nextCfg.id);
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
                }
            }
            if ((K_Guide_Type)nextCfg.guide_type == K_Guide_Type.FullScreenDialog || (K_Guide_Type)nextCfg.guide_type == K_Guide_Type.GetItemPanel)
            {
                //Logger.error("打开预遮罩界面" + nextCfg.id);
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
            }
        }
        if ((null != nextCfg && (m_gInfo.m_curCfg.guide_type != nextCfg.guide_type || (K_Guide_Type)m_gInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)m_gInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick || (K_Guide_Type)m_gInfo.m_curCfg.guide_type == K_Guide_Type.RestrictAndJudgeHeroTime)) || null == nextCfg)
        {
            switch ((K_Guide_Type)m_gInfo.m_curCfg.guide_type)
            {
                case K_Guide_Type.FullScreenDialog:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GDialog);
                    break;
                case K_Guide_Type.Tip:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GTips);
                    break;
                case K_Guide_Type.RestrictClick:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GMask);
                    break;
                case K_Guide_Type.UnlockFurniture:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GUnlockNewFurniture);
                    break;
                case K_Guide_Type.UnlockWorker:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GUnlockWorker);
                    break;
                case K_Guide_Type.ClickUnlockFurn:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GTips);
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GMask);
                    break;
                case K_Guide_Type.WeakGuideAndTask:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GTask);
                    RemoveWeakGuidance();
                    break;
                case K_Guide_Type.WeakGuide:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GTask);
                    RemoveWeakGuidance();
                    break;
                case K_Guide_Type.TipsAndRestrictClick:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GMaskTips);
                    //EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GMask);
                    break;
                case K_Guide_Type.RestrictShopper:
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GTips);
                    break;
                case K_Guide_Type.RestrictAndJudgeHeroTime:
                    isWaitHeroRestingTime = false;
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GMask);
                    break;
            }
        }
        m_gInfo.completeCheck();
        //Logger.error("manager里面的" + m_gInfo.m_curCfg.id);
        if (m_gInfo == null) return;
        if (m_gInfo.isAllOver) return;
        // if (m_gInfo.m_curCfg.id != 4501)
        //     EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETNETWORKMASK, true);
        // GuideDataProxy.inst.isGetNetworkd = false;

        EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, m_gInfo.m_curCfg.id);
    }

    public void waitNetworkBack()
    {
        if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type == K_Guide_Trigger.ArriveLevel)
        {
            if (!levelCheck(int.Parse(m_gInfo.m_curCfg.trigger_param)))
            {
                return;
            }
        }
        if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type == K_Guide_Trigger.ArriveEquipNum)
        {
            if (!equipNumCheck(int.Parse(m_gInfo.m_curCfg.trigger_param))) return;
        }
        if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type == K_Guide_Trigger.WaitTargetShopper)
        {
            if (waitShopper > 0)
            {
                GameTimer.inst.RemoveTimer(waitShopper);
                waitShopper = 0;
            }

            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
            var shopperDatas = ShopperDataProxy.inst.GetShopperList();
            ShopkeeperMoveToCounter();
            waitShopper = GameTimer.inst.AddTimer(0.5f, () =>
            {
                for (int i = 0; i < shopperDatas.Count; i++)
                {
                    if (shopperDatas[i].data.targetEquipId == int.Parse(GuideDataProxy.inst.CurInfo.m_curCfg.conditon_param_1) && shopperDatas[i].data.shopperState == 99)
                    {
                        var targetShopper = IndoorRoleSystem.inst.GetShopperByUid(shopperDatas[i].data.shopperUid);

                        if (targetShopper.GetCurState() == MachineShopperState.queuing && !targetShopper.isMoving)
                        {
                            //EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, m_gInfo.m_curCfg.id);
                            proceedGuide(false);
                            GameTimer.inst.RemoveTimer(waitShopper);
                            waitShopper = 0;
                            break;
                        }
                    }
                }
            });
            return;
        }
        if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type == K_Guide_Trigger.TargetPanelOpen)
        {
            if (waitPanelId > 0)
            {
                GameTimer.inst.RemoveTimer(waitPanelId);
                waitPanelId = 0;
            }
            waitPanelId = GameTimer.inst.AddTimer(0.2f, () =>
            {
                if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type != K_Guide_Trigger.TargetPanelOpen)
                {
                    GameTimer.inst.RemoveTimer(waitPanelId);
                    waitPanelId = 0;
                }
                if (m_gInfo.m_curCfg != null && m_gInfo.m_curCfg.trigger_param != null/* && GUIManager.CurrWindow != null*/)
                {
                    if (GUIManager.GetCurrWindowViewID() == m_gInfo.m_curCfg.trigger_param || (FGUI.inst.uiRootTF.gameObject.FindHideChildGameObject(m_gInfo.m_curCfg.btn_view) != null && FGUI.inst.uiRootTF.gameObject.FindHideChildGameObject(m_gInfo.m_curCfg.btn_view).activeInHierarchy))
                    {
                        //EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, m_gInfo.m_curCfg.id);
                        proceedGuide(false);
                        GameTimer.inst.RemoveTimer(waitPanelId);
                        waitPanelId = 0;
                    }
                }
            });
            return;
        }

        if (m_gInfo.m_curCfg.last_id != 0)
        {
            var lastCfg = GuideConfigManager.inst.GetConfig(m_gInfo.m_curCfg.last_id);
            if (lastCfg != null && (K_Guide_End)lastCfg.end_type == K_Guide_End.ArriveLevel)
            {
                //EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, m_gInfo.m_curCfg.id);
                if (checkTimeId > 0)
                {
                    GameTimer.inst.RemoveTimer(checkTimeId);
                    checkTimeId = 0;
                }

                checkTimeId = GameTimer.inst.AddTimer(0.5f, () =>
                {
                    if (GUIManager.GetCurrWindowViewID() != ViewPrefabName.PlayerLevelUpUI && GUIManager.GetCurrWindowViewID() != ViewPrefabName.MsgBoxPlayerUpItemUI)
                    {
                        proceedGuide(false);
                        GameTimer.inst.RemoveTimer(checkTimeId);
                        checkTimeId = 0;
                    }
                });
                return;
            }
        }

        proceedGuide(false);
    }

    //Stack<PathNode> spmovepath = new Stack<PathNode>();
    //int sptimerId = 0;
    private void ShopkeeperMoveToCounter()
    {
        HotfixBridge.inst.TriggerLuaEvent("GuideManager_ShopkeeperMoveToCounter");

        //if (sptimerId > 0)
        //{
        //    spmovepath.Clear();
        //    GameTimer.inst.RemoveTimer(sptimerId);
        //    sptimerId = 0;
        //}

        //var spkeeper = IndoorMapEditSys.inst.Shopkeeper;
        //spmovepath = IndoorMap.inst.FindPath(spkeeper.currCellPos, IndoorMap.inst.GetCounterOperationPos());
        //spkeeper.move(spmovepath);

        //sptimerId = GameTimer.inst.AddTimer(1, () =>
        // {
        //     if (!spkeeper.isMoving)
        //     {
        //         spkeeper.Character.SetDirection(RoleDirectionType.Left);
        //         //IndoorMapEditSys.inst.Shopkeeper.SetState((int)MachineShopkeeperState.onCounterRound);
        //         GameTimer.inst.RemoveTimer(sptimerId);
        //         sptimerId = 0;
        //     }
        // });
    }

    int checkTimeId = 0;
    public void GuideManager_OnCheckGuide(K_Guide_End type, int val)
    {
        if (m_gInfo.m_curCfg == null) return;

        if (m_gInfo.isAllOver) return;

        if ((K_Guide_End)m_gInfo.m_curCfg.end_type != type) return;

        bool isMatch = false;

        switch (type)
        {
            case K_Guide_End.None:
                isMatch = true;
                break;
            case K_Guide_End.ArriveLevel:
                isMatch = UserDataProxy.inst.playerData.level >= m_gInfo.m_curCfg.end_param;
                break;
            case K_Guide_End.ArriveEquipNum:
                m_gInfo.val = val;
                isMatch = m_gInfo.val >= m_gInfo.m_curCfg.end_param;
                break;
            case K_Guide_End.ClickTarget:
                isMatch = m_gInfo.isClickTarget;
                break;
            case K_Guide_End.DialogFinish:
                isMatch = m_gInfo.isDialogFinish;
                break;
            case K_Guide_End.MoveToTarget:
                isMatch = m_gInfo.isArriveTarget;
                break;
            case K_Guide_End.ClickCountArrive:
                isMatch = m_gInfo.val >= m_gInfo.m_curCfg.end_param;
                break;
            case K_Guide_End.CreatNpcFinish:
                isMatch = m_gInfo.isCreatFinish;
                break;
        }

        if (m_gInfo == null) return;

        if (m_gInfo.isAllOver) return;

        if (isMatch)
        {
            if (subCfg != null) subCfg = null;
            GuideManager_OnNextGuide();
        }
    }

    int dialogTimerId;
    public void proceedGuide(bool isMatch)
    {
        //Logger.error("proceedGuide = " + m_gInfo.m_curCfg.id);
        if (m_gInfo == null) return;
        if (m_gInfo.isAllOver) return;
        GuideConfigData cfg = m_gInfo.m_curCfg;
        if (cfg.task_id != 0)
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETGNEWTASK, false);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEGUIDEUI, K_Guide_UI.GNewTask);
        }

        isWaitHeroRestingTime = false;
        //Logger.log("当前引导" + cfg.desc);
        if (IndoorMap.inst != null && null != IndoorMap.inst.indoorMask)
            IndoorMap.inst.indoorMask.SetActiveTrue();


        if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.FullScreenDialog)
        {
            if (dialogTimerId > 0)
            {
                GameTimer.inst.RemoveTimer(dialogTimerId);
                dialogTimerId = 0;
            }

            dialogTimerId = GameTimer.inst.AddTimer(0.1f, () =>
             {
                 if (!ManagerBinder.inst.stateIsChanging)
                 {
                     if (cfg.last_id != 0)
                     {
                         GuideDataProxy.inst.needWaitTime = 0.3f;
                         EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GDialog);
                     }
                     else
                     {
                         GuideDataProxy.inst.needWaitTime = -1;
                         EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GDialog);
                     }

                     GameTimer.inst.RemoveTimer(dialogTimerId);
                     dialogTimerId = 0;
                 }
             });

        }
        else if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictClick)
        {
            if (cfg.btn_name == "Sure")
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETGNEWTASK, true);
            }
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GMask);
        }
        else if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.ResearchEquip)
        {
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.ActivateDrawing, "", int.Parse(cfg.conditon_param_1), 0, 1));
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
        }
        else if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
        {
            if (cfg.btn_name == "Sure")
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETGNEWTASK, true);
            }
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GMaskTips);
            //EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
        }
        else if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictAndJudgeHeroTime)
        {
            if (cfg.btn_name == "Sure")
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETGNEWTASK, true);
            }
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GMask);
            isWaitHeroRestingTime = true;
        }
        else if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.GetItemPanel)
        {
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, int.Parse(cfg.conditon_param_1), int.Parse(cfg.conditon_param_2)));
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
        }

        GameTimer.inst.AddTimer(0.2f, 1, () =>
          {
              switch ((K_Guide_Type)cfg.guide_type)
              {
                  case K_Guide_Type.Tip:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
                      break;
                  case K_Guide_Type.UnlockFurniture:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GUnlockNewFurniture);
                      break;
                  case K_Guide_Type.UnlockWorker:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GUnlockWorker);
                      break;
                  case K_Guide_Type.Task:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTask);
                      break;
                  case K_Guide_Type.NPCCreat:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.NPCCREAT);
                      break;
                  case K_Guide_Type.NPCState:
                      if (int.Parse(m_gInfo.m_curCfg.conditon_param_1) == 1)
                      {
                          if (curNpc != null && HotfixBridge.inst.GetShopkeeperExist() /*&& IndoorMapEditSys.inst.Shopkeeper != null*/)
                          {
                              EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
                              EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETNPCMOVE);
                          }
                      }
                      break;
                  case K_Guide_Type.ClickUnlockFurn:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GWhiteMask);
                      break;
                  case K_Guide_Type.WeakGuide:
                      IndoorMap.inst.indoorMask.SetActiveFalse();
                      WeakGuidance();
                      break;
                  case K_Guide_Type.RestrictShopper:
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GShopperMask);
                      break;
                  case K_Guide_Type.WeakGuideAndTask:
                      IndoorMap.inst.indoorMask.SetActiveFalse();
                      EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTask);
                      WeakGuidance();
                      break;
                  case K_Guide_Type.GiveEquip:
                      GuideManager_OnNextGuide();
                      break;
                  case K_Guide_Type.End:
                      if (IndoorMap.inst != null)
                          IndoorMap.inst.indoorMask.SetActiveFalse();
                      m_gInfo.isAllOver = true;
                      break;
                  case K_Guide_Type.JudgeExploreTime:
                      if (ExploreDataProxy.inst.GetSlotDataById(1).exploringRemainTime <= int.Parse(m_gInfo.m_curCfg.conditon_param_1))
                      {

                          if (m_gInfo.m_curCfg.wait_id != 0)
                          {
                              if (!firstIsNotWait)
                              {
                                  m_gInfo.setGuideData(m_gInfo.m_curCfg.wait_id);
                                  WaitTargetPanel();
                              }
                              else
                              {
                                  if (!secondIsNotWait)
                                  {
                                      m_gInfo.setGuideData(m_gInfo.m_curCfg.wait_id);
                                      WaitTargetPanel();
                                  }
                                  else
                                  {
                                      m_gInfo.setGuideData(m_gInfo.m_curCfg.next_id);
                                      WaitTargetPanel();
                                  }
                              }
                          }
                      }
                      else
                      {
                          if (m_gInfo.m_curCfg.next_id != 0)
                          {
                              firstIsNotWait = true;
                              m_gInfo.setGuideData(m_gInfo.m_curCfg.next_id);
                              WaitTargetPanel();
                          }
                      }
                      break;
                  case K_Guide_Type.JudgeMakeEquipSlot:
                      var slot = EquipDataProxy.inst.equipSlotList;
                      if (slot != null)
                      {
                          bool haveIdle = false;
                          for (int i = 0; i < slot.Count; i++)
                          {
                              if (slot[i].makeState == 0)
                              {
                                  haveIdle = true;
                                  break;
                              }
                          }

                          if (haveIdle)
                          {
                              m_gInfo.setGuideData(m_gInfo.m_curCfg.next_id);
                              WaitTargetPanel();
                          }
                          else
                          {
                              m_gInfo.setGuideData(m_gInfo.m_curCfg.wait_id);
                              WaitTargetPanel();
                          }
                      }
                      break;
                  case K_Guide_Type.EmptyOperation:
                      GuideManager_OnNextGuide();
                      break;

              }
          });
    }

    public void SetWaitHeroTimeNext(bool isArrive)
    {
        if (!isWaitHeroRestingTime) return;

        if (isArrive)
        {
            isWaitHeroRestingTime = false;
            m_gInfo.setGuideData(m_gInfo.m_curCfg.wait_id);
            WaitTargetPanel();
        }
    }

    private void WaitTargetPanel()
    {
        if ((K_Guide_Type)m_gInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)m_gInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick /*|| (K_Guide_Type)m_gInfo.m_curCfg.guide_type == K_Guide_Type.RestrictAndJudgeHeroTime*/)
        {
            if (m_gInfo.m_curCfg.id != 4201)
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
            }
        }
        if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type == K_Guide_Trigger.TargetPanelOpen)
        {
            if (waitPanelId > 0)
            {
                GameTimer.inst.RemoveTimer(waitPanelId);
                waitPanelId = 0;
            }
            waitPanelId = GameTimer.inst.AddTimer(0.15f, () =>
            {
                if ((K_Guide_Trigger)m_gInfo.m_curCfg.trigger_type != K_Guide_Trigger.TargetPanelOpen)
                {
                    GameTimer.inst.RemoveTimer(waitPanelId);
                    waitPanelId = 0;
                }

                //Logger.error("输出33  " + (GUIManager.CurrWindow != null).ToString());
                //if (GUIManager.CurrWindow != null)
                //{

                //}
                if (GUIManager.GetCurrWindowViewID() == m_gInfo.m_curCfg.trigger_param)
                {
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, m_gInfo.m_curCfg.id);
                    proceedGuide(false);
                    GameTimer.inst.RemoveTimer(waitPanelId);
                    waitPanelId = 0;
                }
            });
            return;
        }
    }

    uiWindow tempWindow;
    int count = 0;
    public void WeakGuidance()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        timerId = GameTimer.inst.AddTimer(0.2f, () =>
        {
            count++;
            if (tempWindow != null)
            {
                if (tempWindow != GUIManager.CurrWindow)
                {
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
                    count = 0;
                }
            }

            tempWindow = GUIManager.CurrWindow;

            if (count >= 50)
            {
                count = 0;
                var curView = GUIManager.CurrWindow;

                if (curView.viewID != ViewPrefabName.MainUI || curView.viewID != ViewPrefabName.EquipMakeUI)
                {
                    if (curView.viewID == ViewPrefabName.MainUI)
                    {
                        var slot = EquipDataProxy.inst.equipSlotList;
                        bool haveIdle = false;
                        if (slot != null)
                        {
                            for (int i = 0; i < slot.Count; i++)
                            {
                                if (slot[i].makeState == 0)
                                {
                                    haveIdle = true;
                                    break;
                                }
                            }
                        }

                        if (haveIdle)
                        {
                            var mainUI = FGUI.inst.uiRootTF.Find(curView.viewID).gameObject;
                            subTarget = mainUI.FindHideChildGameObject("makeButton");
                            if (GUIManager.CurrWindow.viewID == ViewPrefabName.MainUI)
                                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, subTarget.transform, 140, false);
                        }
                    }
                    else
                    {
                        var closeBtn = FGUI.inst.uiRootTF.Find(curView.viewID).gameObject.FindHideChildGameObject("closeBtn");
                        if (closeBtn == null) return;
                        EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, closeBtn.transform, 140, false);
                    }
                }
            }

        });
    }

    private void RemoveWeakGuidance()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    public void RemoveAllTime()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        if (waitPanelId > 0)
        {
            GameTimer.inst.RemoveTimer(waitPanelId);
            waitPanelId = 0;
        }
        if (waitShopper > 0)
        {
            GameTimer.inst.RemoveTimer(waitShopper);
            waitShopper = 0;
        }
    }
}
