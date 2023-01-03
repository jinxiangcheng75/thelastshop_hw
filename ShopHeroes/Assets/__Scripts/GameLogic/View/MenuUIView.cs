using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuUIView : ViewBase<mainUIComp>
{
    public override string viewID => ViewPrefabName.MainUI;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "window";

    private Dictionary<int, MakeSlot> equipMakeSlots = new Dictionary<int, MakeSlot>();
    private Vector3[] makeSlotSignPoses;
    private int slotId;
    private bool isOnAnim, isPosInit;

    LoopEventcomp updateTimerComp;
    int vlgEnabledTimerId = 0;
    int onlineEnabledTimerId = 0;
    int dragEndAnimTimerId = 0;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.all;
        windowAnimTime = 0.6f;
        contentPane.heroBtn.ButtonClickTween(onHeroBtnClick);
        contentPane.taskBtn.ButtonClickTween(() =>
        {
            OnTaskBtnClick();
        });
        contentPane.openBagBtn.ButtonClickTween(() =>
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                if ((K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuide && (K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuideAndTask && (K_Guide_Type)cfg.guide_type != K_Guide_Type.RestrictClick && (K_Guide_Type)cfg.guide_type != K_Guide_Type.TipsAndRestrictClick) return;
            }

            EventController.inst.TriggerEvent(GameEventType.SHOWUI_BAGUI);
        });
        contentPane.makeBtn.ButtonClickTween(() =>
        {
            //修改
            if (contentPane.addImg.gameObject.activeSelf)
            {
                if (!GuideDataProxy.inst.CurInfo.isAllOver)
                {
                    if (!GuideDataProxy.inst.CurInfo.JudgeIsFinishById(2802)) return;
                }
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_BuyMakingSlot);
            }
            else
            {
                if (!GuideDataProxy.inst.CurInfo.isAllOver)
                {
                    var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                    if ((K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuide && (K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuideAndTask && (K_Guide_Type)cfg.guide_type != K_Guide_Type.RestrictClick && (K_Guide_Type)cfg.guide_type != K_Guide_Type.TipsAndRestrictClick) return;
                }
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_PRODUCTION_SELECT, -1);
            }
        });
        contentPane.btn_design.ButtonClickTween(() =>
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                if ((K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuide && (K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuideAndTask && (K_Guide_Type)cfg.guide_type != K_Guide_Type.RestrictClick && (K_Guide_Type)cfg.guide_type != K_Guide_Type.TipsAndRestrictClick && cfg.btn_name != "设计按钮") return;
            }
            IndoorMapEditSys.inst.isClickFunriture = false;
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
        });

        contentPane.btn_Chat.ButtonClickTween(() =>
        {
            if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(137).parameters)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(137).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                return;
            }
            EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_SHOWVIEW);
        });

        contentPane.cityBtn.ButtonClickTween(() =>
        {
            //转到城市状态
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
        });

        contentPane.btn_totalRank.ButtonClickTween(() =>
        {

        });

        contentPane.btn_welfare.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_WelfareUI", 0);
        });

        contentPane.btn_mall.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_MallUI", 0);
        });

        contentPane.bindingBtn.ButtonClickTween(() =>
        {
            if (AccountDataProxy.inst.currbindingType == EBindingType.None)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_BindingUI");
            }
        });

        if (contentPane.btn_landscape != null)
        {
            contentPane.btn_landscape.onClick.AddListener(() =>
            {
                SaveManager.inst.SaveInt("zhuanzhuanpingmu", 1);
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SETTINGPANEL);
            });
        }

        contentPane.refugeBtn.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_RefugeUI");
        });

        // contentPane.AddTest.onClick.AddListener(() =>
        // {
        //     FGUI.inst.AddResolution();
        // });
        // contentPane.SubTest.onClick.AddListener(() =>
        // {
        //     FGUI.inst.subResolution();
        // });


        //动画
        contentPane.slotSR.onBeginDragHandle += onBeginDrag;
        contentPane.slotSR.onDragHandle += onDrag;
        contentPane.slotSR.onEndDragHandle += onEndDrag;

        //updateMakeSlots(ESlotAnimType.Normal);

    }
    public void setTarget()
    {
        var slotList = getCurWorkingSlots();
        if (slotList != null && slotList.Count > 0)
        {
            GameObject target = slotList[0].gameObject;
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETSLOTTARGET, target);
        }
    }

    private void OnTaskBtnClick()
    {
        if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(133).parameters)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(133).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
            return;
        }
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TASKPANEL);
    }

    private void onHeroBtnClick()
    {


        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
    }

    protected override void onShown()
    {

        //contentPane.allParentObj.SetActive(true);

        EventController.inst.AddListener(GameEventType.UpdateGameRedPoints, UpdateBtnRedDot);
        updateMakeSlots(ESlotAnimType.Normal);

        RefreshGlobalBuff();
        RefreshBtnActive();
        RefreshRedPoint();
        RefreshUIUnlock();

        //刷新礼包按钮
        RefreshPayGiftBtns();

        //刷新在线领奖按钮
        RefreshOnlineRewardBtns();

        //刷新豪华度按钮
        RefreshLuxuryItem();

        RefreshRefugeBtn();

        //推出礼包弹窗
        showDirectPurchasePanel();

        //首次登陆有系统消息弹窗
        showLookBackPanel();

        showVipOverduPanel();

        isShowin = true;
        updateTimerComp = GameTimer.inst.AddLoopTimerComp(contentPane.slotSR.gameObject, 10, springbackMethod);
    }


    protected override void beforeDispose()
    {
        base.beforeDispose();
        equipMakeSlots.Clear();
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateBtnRedDot);
    }

    protected override void onHide()
    {
        HotfixBridge.inst.TriggerLuaEvent("MainUI_ClearGiftBtns");
        HotfixBridge.inst.TriggerLuaEvent("Clear_OnlineBtn");
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateBtnRedDot);
        
        if (updateTimerComp != null)
        {
            GameTimer.inst.removeLoopTimer(updateTimerComp);
            updateTimerComp = null;
        }

        if (vlgEnabledTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(vlgEnabledTimerId);
            vlgEnabledTimerId = 0;
        }

        if (onlineEnabledTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(onlineEnabledTimerId);
            onlineEnabledTimerId = 0;
        }

        if (dragEndAnimTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(dragEndAnimTimerId);
            dragEndAnimTimerId = 0;
        }

        needReset = true;

    }
    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        showSystemBtn(true);
    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();

        hideSystemBtn();
    }

    public void RefreshRedPoint()
    {
        contentPane.heroIdleCount.text = RoleDataProxy.inst.GetIdleStateHeroCount.ToString();
        if (RoleDataProxy.inst.workerRedPointShow)
        {
            contentPane.workerRedPoint.SetActiveTrue();
            //contentPane.heroIdleBg.SetActiveFalse();
        }
        else
        {
            bool hasRedPoint = RoleDataProxy.inst.heroRedPointShow;
            contentPane.workerRedPoint.SetActive(hasRedPoint);
            //contentPane.heroIdleBg.SetActive(!hasRedPoint);
            //contentPane.heroIdleCount.text = RoleDataProxy.inst.GetIdleStateHeroCount.ToString();
        }

        contentPane.city_redPoint.SetActive(ExploreDataProxy.inst.HasFinishExplore || MarketDataProxy.inst.redPointShow || TreasureBoxDataProxy.inst.newBoxGroupId != 0);

        if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(133).parameters)
        {
            contentPane.task_redPoint.SetActive(false);
        }
        else
        {
            contentPane.task_redPoint.SetActive(UserDataProxy.inst.task_needShowRedPoint);
        }

        HotfixBridge.inst.TriggerLuaEvent("mainUI_refreshRedPoints");

    }

    void setBindingBtnActive()
    {

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            contentPane.bindingBtn.gameObject.SetActive(false);
            if (contentPane.btn_landscape != null)
                contentPane.btn_landscape.gameObject.SetActive(false);
            contentPane.btn_Chat.gameObject.SetActive(false);
            return;
        }
        contentPane.btn_Chat.gameObject.SetActive(true);

        //台服外服  需要绑定账号的逻辑
        //if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(147)?.parameters)
        //{
        //    contentPane.bindingBtn.gameObject.SetActive(false);
        //    if (contentPane.btn_landscape != null)
        //        contentPane.btn_landscape.gameObject.SetActive(false);

        //    return;
        //}



        //contentPane.bindingBtn.gameObject.SetActive(AccountDataProxy.inst.currbindingType == EBindingType.None);
        //contentPane.bindingText.text = "+" + WorldParConfigManager.inst.GetConfig(8100).parameters.ToString();
        //if (!contentPane.bindingBtn.gameObject.activeSelf)
        //{
        //    if (contentPane.btn_landscape != null)
        //    {
        //        if (SaveManager.inst.GetInt("zhuanzhuanpingmu") == 0)
        //            contentPane.btn_landscape.gameObject.SetActive(true);
        //        else
        //            contentPane.btn_landscape.gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //    if (contentPane.btn_landscape != null)
        //    {
        //        contentPane.btn_landscape.gameObject.SetActive(false);
        //    }
        //}


        //国服逻辑 仅判断是否为ipad分辨率
        contentPane.bindingBtn.gameObject.SetActive(false);
        if (contentPane.btn_landscape != null)
        {
            if (GameSettingManager.inst.IsIpad() && SaveManager.inst.GetInt("zhuanzhuanpingmu") == 0)
                contentPane.btn_landscape.gameObject.SetActive(true);
            else
                contentPane.btn_landscape.gameObject.SetActive(false);
        }

    }

    void setWelfareBtnActive()
    {

        if (UIUnLockConfigMrg.inst.GetBtnInteractable("btn_welfare"))
        {
            contentPane.btn_welfare.gameObject.SetActive(true);
        }
        else
        {
            WorldParConfig worldParConfig = WorldParConfigManager.inst.GetConfig(343);
            if (worldParConfig != null)
            {
                contentPane.btn_welfare.gameObject.SetActive(UserDataProxy.inst.playerData.level >= worldParConfig.parameters);
            }
            else
            {
                contentPane.btn_welfare.gameObject.SetActive(false);
            }
        }

    }

    private void RefreshBtnActive()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
        {
            //contentPane.btn_sevenday.gameObject.SetActive(!SevenDayGoalDataProxy.inst.isAllOver);
            //contentPane.taskBtn.gameObject.SetActive(true);
            //contentPane.btn_attendance.gameObject.SetActive(true);
            if (SevenDayGoalDataProxy.inst.waitSingleData != null)
            {
                if (UserDataProxy.inst.playerData.level >= WorldParConfigManager.inst.GetConfig(135).parameters && UserDataProxy.inst.currMainTaskGroup >= 9999)
                {
                    EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.SEVENDAY_CONTENTCHANGE, SevenDayGoalDataProxy.inst.waitSingleData);
                    SevenDayGoalDataProxy.inst.waitSingleData = null;
                }
            }
        }

        setBindingBtnActive();
        setWelfareBtnActive();

    }

    private void RefreshUIUnlock()
    {
        GUIHelper.SetUIGray(contentPane.taskBtn.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(133).parameters);
        GUIHelper.SetUIGray(contentPane.btn_Chat.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(137).parameters);
    }

    public void RefreshChatRedPoint()
    {
        var wdpCfg = WorldParConfigManager.inst.GetConfig(137);
        int isShowRedLv = 0;
        if (wdpCfg != null)
            isShowRedLv = (int)wdpCfg.parameters;
        contentPane.chatBtnRedPoint.SetActive(ChatDataProxy.inst.hasRedTip && UserDataProxy.inst.playerData.level >= isShowRedLv);
    }

    public void RefreshPayGiftBtns()
    {

        contentPane.rightBtnsVLG_row1.enabled = false;

        for (int i = 0; i < contentPane.commonGiftParent_luaBehaviour.transform.childCount; i++)
        {
            GameObject.Destroy(contentPane.commonGiftParent_luaBehaviour.transform.GetChild(i).gameObject);
        }

        HotfixBridge.inst.TriggerLuaEvent("MainUI_RefreshGiftBtn", contentPane.commonGiftParent_luaBehaviour);

        vlgEnabledTimerId = GameTimer.inst.AddTimerFrame(2, 1, () =>
        {
            if (this == null)
            {
                return;
            }

            contentPane.rightBtnsVLG_row1.enabled = true;
        });

    }

    public void RefreshOnlineRewardBtns()
    {
        if (GuideDataProxy.inst == null || GuideDataProxy.inst.CurInfo == null || !GuideDataProxy.inst.CurInfo.isAllOver) return;
        if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(126)?.parameters) return;
        contentPane.rightBtnsVLG_row1.enabled = false;

        for (int i = 0; i < contentPane.onlineReward_luaBehaviour.transform.childCount; i++)
        {
            GameObject.Destroy(contentPane.onlineReward_luaBehaviour.transform.GetChild(i).gameObject);
        }

        HotfixBridge.inst.TriggerLuaEvent("Refresh_OnlineBtn", contentPane.onlineReward_luaBehaviour);

        onlineEnabledTimerId = GameTimer.inst.AddTimerFrame(2, 1, () =>
        {
            if (this == null)
            {
                return;
            }

            contentPane.rightBtnsVLG_row1.enabled = true;
        });
    }

    public void RefreshRefugeBtn()
    {
        if (GuideDataProxy.inst == null || (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo == null) || (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)) { contentPane.refugeBtn.gameObject.SetActive(false); return; }
        if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(330)?.parameters)
        {
            contentPane.refugeBtn.gameObject.SetActive(false);
            return;
        }
        contentPane.rightBtnsVLG_row1.enabled = false;

        HotfixBridge.inst.TriggerLuaEvent("Set_MainUIRefugeBtn", contentPane.refugeTimeText, contentPane.refugeBtn);

        //SetRefugeBtnState(true);

        onlineEnabledTimerId = GameTimer.inst.AddTimerFrame(2, 1, () =>
        {
            if (this == null)
            {
                return;
            }

            contentPane.rightBtnsVLG_row1.enabled = true;
        });

        if (ExploreDataProxy.inst.needShowRefugePanel)
        {
            ExploreDataProxy.inst.needShowRefugePanel = false;
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_RefugePanelUI");
        }
    }

    public void SetRefugeBtnState(bool state)
    {
        contentPane.refugeBtn.gameObject.SetActive(state);
    }

    public void RefreshLuxuryItem()
    {
        if (GuideDataProxy.inst == null) return;
        if (GuideDataProxy.inst.CurInfo == null) return;
        if (!GuideDataProxy.inst.CurInfo.isAllOver) return;
        if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(180)?.parameters) return;

        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_Luxury", contentPane.luxuryItem);
    }

    public void showLookBackPanel()
    {
        if (GuideDataProxy.inst == null) return;
        if (GuideDataProxy.inst.CurInfo == null) return;
        if (!GuideDataProxy.inst.CurInfo.isAllOver) return;
        HotfixBridge.inst.TriggerLuaEvent("Check_LookBackUI");
    }

    public void showDirectPurchasePanel()
    {
        HotfixBridge.inst.TriggerLuaEvent("showDirectPurchasePanel"); //切回主界面推出礼包弹窗
    }

    public void showVipOverduPanel()
    {
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_VipOverDueUI");
    }

    public void RefreshGlobalBuff()
    {
        if (WorldParConfigManager.inst.GetConfig(151).parameters > UserDataProxy.inst.playerData.level)
        {
            return;
        }

        var globalBuffs = GlobalBuffDataProxy.inst.GetAllGlobalBuffData();

        for (int i = 0; i < contentPane.globalBuffItems.Length; i++)
        {
            var item = contentPane.globalBuffItems[i];

            if (i < globalBuffs.Count)
            {
                item.SetData(globalBuffs[i]);
            }
            else
            {
                item.Clear();
            }
        }

        if (GuideDataProxy.inst.CurInfo.isAllOver) showGlobalBuffPanel(); //展示主题UI
    }

    void showGlobalBuffPanel()
    {
        var globalBuffs = GlobalBuffDataProxy.inst.GetAllGlobalBuffData();

        foreach (GlobalBuffData item in globalBuffs)
        {
            if (!string.IsNullOrEmpty(AccountDataProxy.inst.account))
            {
                if (PlayerPrefs.GetInt(AccountDataProxy.inst.account + "_globalBuff" + item.buffuId, -1) != 1)
                {
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutGlobalBuff { type = ReceiveInfoUIType.GlobalBuff, buffType = item.buffType });
                    PlayerPrefs.SetInt(AccountDataProxy.inst.account + "_globalBuff" + item.buffuId, 1);
                }
            }
        }

    }

    //显示顶侧，左侧，底部 功能按钮
    public void showSystemBtn(bool slotRefresh)
    {
        if (isOnAnim) return;
        //updateSlotsPos(ESlotAnimType.Refresh);
        isOnAnim = GameSettingManager.inst.needShowUIAnim;

        //if (!isOnAnim) contentPane.btn_Chat.GetComponent<Graphic>().FadeFromTransparentTween(1, 0.3f);

        if (!GameSettingManager.inst.needShowUIAnim)
        {
            contentPane.bottomAniTf.anchoredPosition = new Vector2(contentPane.bottomAniTf.anchoredPosition.x, 0);
        }
        else
        {
            DOTween.Kill(contentPane.bottomAniTf);
            contentPane.bottomAniTf.DOAnchorPos3DY(0, 0.3f).From(-500f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (!GameSettingManager.inst.needShowUIAnim) return;

                contentPane.leftAnimTf.anchoredPosition = new Vector2(0, contentPane.leftAnimTf.anchoredPosition.y);
                contentPane.rightAnimTf.anchoredPosition = new Vector2(0, contentPane.rightAnimTf.anchoredPosition.y);

                contentPane.btn_Chat.interactable = true;
                //contentPane.btn_Chat.GetComponent<Graphic>().FadeFromTransparentTween(1, 0.2f);
                if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.makeBtnTf.name))
                {
                    if (slotRefresh)
                    {
                        contentPane.makeBtnTf.DOAnchorPos3DX(-115f, 0.15f).From(150f).SetEase(Ease.OutBack).OnComplete(() =>
                        {

                            var curWorkingSlots = getCurWorkingSlots();

                            foreach (var item in curWorkingSlots)
                            {
                                item.transform.rotation = Quaternion.Euler(Vector3.forward * 30);
                                item.transform.DOLocalMoveX(calSingleSlotPos(item.slotIndex, item.slotGroup, getWorkingSlotsByGroup(0).Count).x, 0.3f).From(contentPane.makeBtnTf.position.x).OnComplete(() =>
                                {
                                    item.transform.DOLocalRotate(Vector3.zero, 0.15f).OnComplete(() =>
                                    {
                                        if (isOnAnim) isOnAnim = false;
                                        contentPane.slotSR.enabled = curWorkingSlots.Count > StaticConstants.ShowSlotNum;
                                    });
                                });
                            }

                            contentPane.makeSlotListContent.gameObject.SetActiveTrue();
                            contentPane.makeBtnTf.DOLocalRotate(Vector3.zero, 0.15f).OnComplete(() =>
                            {
                                if (curWorkingSlots.Count == 0 && isOnAnim) isOnAnim = false;
                            });
                        }).OnStart(() =>
                        {
                            contentPane.makeBtnTf.rotation = Quaternion.Euler(Vector3.forward * 30);
                            contentPane.makeBtnTf.gameObject.SetActiveTrue();
                        });
                    }
                    else
                    {
                        contentPane.slotSR.enabled = getCurWorkingSlots().Count > StaticConstants.ShowSlotNum;
                    }

                }
                else
                {
                    contentPane.makeBtnTf.gameObject.SetActiveFalse();
                }

            }).OnStart(() =>
            {
                if (!GameSettingManager.inst.needShowUIAnim) return;
                contentPane.makeBtnTf.gameObject.SetActive(!slotRefresh);
                contentPane.makeSlotListContent.gameObject.SetActive(!slotRefresh);
            });
        }


    }
    //隐藏顶侧，左侧，底部 功能按钮
    public void hideSystemBtn()
    {
        if (contentPane == null) return;

        isOnAnim = false;

        if (GameSettingManager.inst.needShowUIAnim)
        {
            DOTween.Kill(contentPane.bottomAniTf);
            //DOTween.Kill(contentPane.btn_Chat.GetComponent<Graphic>(), true);

            contentPane.bottomAniTf.DOAnchorPos3DY(-500f, 0.1f).From(0f).OnComplete(() =>
            {
                //if (GuideDataProxy.inst == null || (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver))
                //{
                //    contentPane.allParentObj.SetActive(false);
                //}
            });
            //contentPane.btn_Chat.GetComponent<Graphic>().FadeTransparentTween(1, 0.1f, false);
            //DoTweenUtil.Fade_a_To_0_All(contentPane.leftAnimTf, 1, 0.1f, false);
            //DoTweenUtil.Fade_a_To_0_All(contentPane.rightAnimTf, 1, 0.1f, false);

        }
        else
        {
            if (GuideDataProxy.inst == null || (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver))
            {
                //contentPane.allParentObj.SetActive(false);
            }
            contentPane.bottomAniTf.anchoredPosition = new Vector2(contentPane.bottomAniTf.anchoredPosition.x, -500f);
        }

        contentPane.leftAnimTf.anchoredPosition = new Vector2(-3000, contentPane.leftAnimTf.anchoredPosition.y);
        contentPane.rightAnimTf.anchoredPosition = new Vector2(3000, contentPane.rightAnimTf.anchoredPosition.y);
        contentPane.makeBtnTf.gameObject.SetActiveFalse();
        contentPane.btn_Chat.interactable = false;

    }

    bool isShowin = true;
    public override void shiftIn()
    {
        HotfixBridge.inst.TriggerLuaEvent("View_OnShow", viewID);
        EventController.inst.TriggerEvent(GameEventType.UIUnlock.VIEW_ONSHOW, viewID);
        if (isShowin) return;
        //contentPane.allParentObj.SetActive(true);
        DOTween.Kill(contentPane.bottomAniTf);
        isShowin = true;
        RefreshRedPoint();
        RefreshBtnActive();
        RefreshUIUnlock();
        if (GameSettingManager.inst.needShowUIAnim)
        {
            showSystemBtn(false);
        }
        else
        {
            contentPane.bottomAniTf.DOAnchorPos3DY(0, 0).From(-500f);
            //DoTweenUtil.Fade_0_To_a_All(contentPane.leftAnimTf, 1, 0f);
            //DoTweenUtil.Fade_0_To_a_All(contentPane.rightAnimTf, 1, 0f);
            contentPane.leftAnimTf.anchoredPosition = new Vector2(0, contentPane.leftAnimTf.anchoredPosition.y);
            contentPane.rightAnimTf.anchoredPosition = new Vector2(0, contentPane.leftAnimTf.anchoredPosition.y);
            var makeBtnRect = contentPane.makeBtn.GetComponent<RectTransform>();
            makeBtnRect.anchoredPosition = new Vector2(-115, makeBtnRect.anchoredPosition.y);
            contentPane.btn_Chat.interactable = true;
            contentPane.makeBtnTf.gameObject.SetActiveTrue();
        }

        //刷新红点
        UpdateBtnRedDot();

    }

    public override void shiftOut()
    {
        if (!isShowin) return;
        isShowin = false;
        hideSystemBtn();
    }

    private void UpdateBtnRedDot()
    {
        //制作
        var reddot = contentPane.makeBtnTf.Find("RedDot");
        if (reddot != null)
        {
            reddot.gameObject.SetActive(EquipDataProxy.inst.hasNewEquipDrawing());
            contentPane.idleSlotNumTx.transform.parent.gameObject.SetActive(!reddot.gameObject.activeSelf);
        }

        RefreshChatRedPoint();

        //if (HotfixBridge.inst != null)
        //{
        //    HotfixBridge.inst.TriggerLuaEvent("CS_RefreshRedPoint_Activity_WorkerGame_ScoreCanReward");
        //}
    }
    public void PlayEquipFly(ShelfChange_Equip_SFX_Data shelfChange_Equip_SFX_Data)
    {
        SpriteRenderer spriteRender = shelfChange_Equip_SFX_Data.spriteRenderer;
        bool onOrOff = shelfChange_Equip_SFX_Data.onOrOff;
        string equipUid = shelfChange_Equip_SFX_Data.equipUid;
        int isFromSlotOrBox = shelfChange_Equip_SFX_Data.isFromSlotOrBox;

        Vector3 startPos = Vector3.zero;
        Vector3 endVector = spriteRender.transform.localPosition;
        Vector3 endScale = spriteRender.transform.localScale;

        if (onOrOff)
        {
            bool fromSlot = isFromSlotOrBox == 0;

            //Vector3 endRotate = spriteRender.transform.rotation.eulerAngles;
            if (fromSlot)
            {
                startPos = GetStartPos(slotId);
                EquipItem equip = ItemBagProxy.inst.GetEquipItem(equipUid);
                if (equip != null)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您存放了") + equip.equipConfig.name, GUIHelper.GetColorByColorHex("FFD907"));
                }


                PlayerEquipFlyAnimBySlot(spriteRender, startPos, endVector, endScale, false, () =>
                {
                    if (IndoorMap.inst.GetFurnituresByUid(shelfChange_Equip_SFX_Data.shelfUid, out Furniture shelfEntity))
                    {
                        shelfEntity.EquipFlyOnAnim();
                    }
                });

            }
            else
            {
                startPos = UserDataProxy.inst.GetOneTrunkPosition(out int trunkUid);

                PlayEquipFlyAnim(spriteRender, startPos, endVector, endScale, false, () =>
                {
                    //DoTweenUtil.ClickTween(spriteRender.transform);
                    if (IndoorMap.inst.GetFurnituresByUid(shelfChange_Equip_SFX_Data.shelfUid, out Furniture shelfEntity))
                    {
                        shelfEntity.EquipFlyOnAnim();
                    }
                });
            }
        }
        else
        {
            //var shopper = IndoorMapEditSys.inst.GetShopperActor(isFromSlotOrBox);
            var shopper = IndoorRoleSystem.inst.GetShopperByUid(isFromSlotOrBox);
            if (shopper != null && !IndoorMapEditSys.inst.shelfEquipToShopperHandlers.ContainsKey(shopper.shopperUid))
            {
                IndoorMapEditSys.inst.shelfEquipToShopperHandlers.Add(shopper.shopperUid, null);
                spriteRender.enabled = false;

                startPos = spriteRender.transform.position;
                endVector = spriteRender.transform.position + (shopper.transform.position + Vector3.up * 0.5f) - spriteRender.transform.position;

                endScale = new Vector3(spriteRender.transform.localScale.x, spriteRender.transform.localScale.y, spriteRender.transform.localScale.z);

                SpriteRenderer tmp_spriteRender = GameObject.Instantiate(spriteRender);

                EquipItem equip = ItemBagProxy.inst.GetEquipItem(equipUid);

                // ManagerBinder.inst.Asset.getSpriteAsync(equip.equipConfig.equipDrawingsConfig.atlas, equip.equipConfig.equipDrawingsConfig.icon, (sprite) =>
                // {
                //     tmp_spriteRender.sprite = sprite;
                // });
                AtlasAssetHandler.inst.GetAtlasSprite(equip.equipConfig.equipDrawingsConfig.atlas, equip.equipConfig.equipDrawingsConfig.icon, (gsprite) =>
                {
                    var sex = tmp_spriteRender.gameObject.AddComponent<SpriteEX>();
                    sex.mGSprite = gsprite;
                });

                PlayEquipFlyAnim(tmp_spriteRender, startPos, endVector, endScale, false, () =>
                {
                    GameObject.Destroy(tmp_spriteRender.gameObject);
                    IndoorMapEditSys.inst.shelfEquipToShopperHandlers[shopper.shopperUid]?.Invoke();
                    IndoorMapEditSys.inst.shelfEquipToShopperHandlers.Remove(shopper.shopperUid);
                });

                //Logger.error("货架装备  芜湖起飞 1");
            }
            else
            {
                spriteRender.enabled = false;
                Logger.log("没有uid为" + isFromSlotOrBox + "的顾客  或者货架装备已经开始起飞");
            }
        }
    }

    #region 拖拽制造槽效果

    bool beginDragDirection = true;//从左往右
    bool isChanged;
    bool needReset = true;
    bool onDragDirection = true;//True 从左往右 False 从右往左
    bool canDrag;
    bool isOnDrag;//是否正在拖拽
    MakeSlot leftSlot;
    Vector3 leftOriPos;
    MakeSlot rightSlot;
    Vector3 rightOriPos;

    void springbackMethod()
    {
        if (isOnDrag || getWorkingSlotsByGroup(0).Count <= 1 || !isShowing || getCurWorkingSlots().Count <= StaticConstants.ShowSlotNum) return;

        contentPane.slotSR.SetSpringback(false, getWorkingSlotsByGroup(0).Count - 1);
    }

    public void RefreshMakeBtnState()
    {
        int idleSlotNum = EquipDataProxy.inst.GetIdleEquipMakeSlotNum();//空闲槽位数量
        contentPane.addImg.gameObject.SetActive(idleSlotNum == 0);
        contentPane.idleSlotNumTx.gameObject.SetActive(idleSlotNum != 0);
        contentPane.idleSlotNumTx.text = idleSlotNum.ToString();
    }

    //刷新装备制造槽位
    public void updateMakeSlots(ESlotAnimType animType)
    {
        // EquipDataProxy.inst.equipSlotList;
        int notWorkedNum = 0;


        if (animType == ESlotAnimType.Normal)
        {
            needReset = true;
            EquipDataProxy.inst.MaskeSlotsSort();
        }
        else if (animType == ESlotAnimType.MakeEnd && isOnDrag)  //如果拖拽中制作完成 仅刷新 
        {
            animType = ESlotAnimType.Refresh;
        }

        for (int i = 0; i < EquipDataProxy.inst.mskeSlotCount; i++)
        {
            var _slot = EquipDataProxy.inst.GetMakeSlotByIndex(i);
            bool isWorkering = _slot.makeState != 0;
            if (!isWorkering) notWorkedNum++;

            if (equipMakeSlots.ContainsKey(_slot.slotId))
            {
                equipMakeSlots[_slot.slotId].gameObject.SetActive(isWorkering);
                equipMakeSlots[_slot.slotId].UpdateState(_slot.makeState, _slot.equipDrawingId, _slot.currTime, _slot.totalTime);
            }
            else
            {
                var newGo = GameObject.Instantiate(contentPane.makeSlotItemGO, contentPane.makeSlotListContent);
                newGo.SetActive(true);
                newGo.name = _slot.slotId.ToString();
                MakeSlot makeslot = newGo.GetComponent<MakeSlot>();
                makeslot.checkCanClickHandler = () =>
                {
                    if (getCurWorkingSlots().Count <= StaticConstants.ShowSlotNum)
                    {
                        return true;
                    }

                    return !isOnDrag;
                };
                makeslot.slotId = _slot.slotId;
                makeslot.UpdateState(_slot.makeState, _slot.equipDrawingId, _slot.currTime, _slot.totalTime);
                equipMakeSlots.Add(_slot.slotId, makeslot);
                makeslot.gameObject.SetActive(isWorkering);
            }

            if (isWorkering) equipMakeSlots[_slot.slotId].slotIndex = i - notWorkedNum;
        }

        if (animType != ESlotAnimType.Refresh)
        {

            if (animType == ESlotAnimType.MakeEnd && getCurWorkingSlots().Count >= StaticConstants.ShowSlotNum)
            {
                ///根据收起装备的位置决定方向
                var startGroup = getWorkingSlotsByGroup(0);
                var endGroup = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1);
                bool direction = endGroup.Count > 1;

                if (startGroup.Count != 0) rightSlot = !direction ? startGroup[startGroup.Count > 2 ? startGroup.Count - 2 : 0] : null;//右侧的那位哥们
                if (endGroup.Count != 0) leftSlot = direction ? endGroup[endGroup.Count > 1 ? 1 : 0] : null;//左侧的那位哥们

                if (dragEndAnimTimerId != 0)
                {
                    GameTimer.inst.RemoveTimer(dragEndAnimTimerId);
                    dragEndAnimTimerId = 0;
                }

                contentPane.slotSR.enabled = false;
                dragEndAnimTimerId = GameTimer.inst.AddTimer(0.5f, 1, () =>
                {
                    if (contentPane.slotSR != null)
                    {
                        contentPane.slotSR.enabled = getCurWorkingSlots().Count > StaticConstants.ShowSlotNum;
                    }
                });

            }

            updateSlotsPos(animType);
        }
        needReset = false;

        RefreshMakeBtnState();
    }

    bool needSetSiblingIndexBySlotAnimType(ESlotAnimType animType)
    {
        if (animType == ESlotAnimType.Normal || animType == ESlotAnimType.MakeEnd || animType == ESlotAnimType.Draging)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool needDoAniBySlotAnimType(ESlotAnimType animType)
    {
        if (animType == ESlotAnimType.Refresh || animType == ESlotAnimType.Normal || animType == ESlotAnimType.Draging)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void updateSlotsPos(ESlotAnimType animType)
    {
        if (needReset) contentPane.slotSR.enabled = getCurWorkingSlots().Count > StaticConstants.ShowSlotNum;
        //设置槽位组编号
        updateSlotGroup(animType);
        //设置槽位渲染顺序
        updateSlotSetSiblingIndex(animType);
        //设置槽位位置、progress动画及是否可以点击
        updateSlotAnimAndPos(animType);
    }

    void updateSlotGroup(ESlotAnimType animType)
    {
        if (animType != ESlotAnimType.MakeEnd && animType != ESlotAnimType.Normal) return;

        List<MakeSlot> curWorkingSlots = getCurWorkingSlots();

        if (curWorkingSlots.Count <= StaticConstants.ShowSlotNum)
        {
            for (int i = 0; i < curWorkingSlots.Count; i++)
            {
                var slot = curWorkingSlots[i];
                slot.slotGroup = slot.slotIndex;
            }
        }
        else //大于展示槽位数
        {
            if (animType == ESlotAnimType.Normal)
            {
                for (int i = 0; i < curWorkingSlots.Count; i++)
                {
                    var slot = curWorkingSlots[i];
                    slot.slotGroup = Mathf.Min(StaticConstants.ShowSlotNum - 1, slot.slotIndex);
                }
            }
            else
            {
                int lackGroup = -1;
                for (int i = 0; i < StaticConstants.ShowSlotNum; i++)
                {
                    int groupNum = getWorkingSlotsByGroup(i).Count;

                    if (groupNum == 0)
                    {
                        lackGroup = i;
                        break;
                    }
                }

                if (lackGroup != -1)
                {
                    if (lackGroup - 0 >= StaticConstants.ShowSlotNum - 1 - lackGroup) //缺少的组靠右 先从右边借 不足从左边
                    {
                        int num = 0;
                        int index = lackGroup + 1;
                        while (index < StaticConstants.ShowSlotNum)
                        {
                            num += getWorkingSlotsByGroup(index).Count;
                            index++;
                        }

                        int needNum = StaticConstants.ShowSlotNum - lackGroup; //至少需要的数量

                        if (num < needNum) //不足 从左边借 此时左边一定是足够的情况
                        {
                            for (int i = lackGroup - 1; i >= 0; i--)
                            {
                                var list = getWorkingSlotsByGroup(i);
                                var slot = list[list.Count - 1];
                                slot.slotGroup = slot.slotGroup + 1;
                            }
                        }
                        else //右边足够的情况 从最右侧分出来一个给前面顶过去 只有最右侧会堆叠
                        {
                            for (int i = lackGroup + 1; i <= StaticConstants.ShowSlotNum - 1; i++)
                            {
                                var slot = getWorkingSlotsByGroup(i)[0];
                                slot.slotGroup = slot.slotGroup - 1;
                            }
                        }

                    }
                    else //缺少的组靠左 先从左边借 不足从右边
                    {
                        int num = 0;
                        int index = lackGroup - 1;
                        while (index >= 0)
                        {
                            num += getWorkingSlotsByGroup(index).Count;
                            index--;
                        }

                        int needNum = lackGroup + 1; //至少需要的数量

                        if (num < needNum) //不足 从右边借 此时右边一定是足够的情况
                        {
                            for (int i = lackGroup + 1; i <= StaticConstants.ShowSlotNum - 1; i++)
                            {
                                var slot = getWorkingSlotsByGroup(i)[0];
                                slot.slotGroup = slot.slotGroup - 1;
                            }
                        }
                        else //左边足够的情况
                        {
                            for (int i = lackGroup - 1; i >= 0; i--)
                            {
                                var list = getWorkingSlotsByGroup(i);
                                var slot = list[list.Count - 1];
                                slot.slotGroup = slot.slotGroup + 1;
                            }
                        }

                    }
                }
            }
        }
    }

    void updateSlotSetSiblingIndex(ESlotAnimType animType)
    {
        List<MakeSlot> curWorkingSlots = getCurWorkingSlots();
        bool needSetSiblingIndex = needSetSiblingIndexBySlotAnimType(animType);

        if (needSetSiblingIndex)
        {
            int startGroupNum = needReset ? 1 : getWorkingSlotsByGroup(0).Count;

            for (int i = 0; i < curWorkingSlots.Count; i++)
            {
                var slot = curWorkingSlots[i];
                if (slot.slotIndex < StaticConstants.ShowSlotNum + startGroupNum - 1)
                {
                    slot.transform.SetSiblingIndex(slot.slotIndex);
                }
                else
                {
                    slot.transform.SetSiblingIndex(StaticConstants.ShowSlotNum - 1);
                }
            }
        }

        var list = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1 - 1);
        list.ForEach((t) =>
        {
            t.transform.SetAsLastSibling();
        });
    }

    void updateSlotAnimAndPos(ESlotAnimType animType)
    {

        List<MakeSlot> curWorkingSlots = getCurWorkingSlots();

        int allSlotNum = curWorkingSlots.Count;
        var startGroup = getWorkingSlotsByGroup(0);
        int startGroupNum = startGroup.Count;

        bool doAnim = needDoAniBySlotAnimType(animType);

        if (allSlotNum <= StaticConstants.ShowSlotNum)
        {
            for (int i = 0; i < curWorkingSlots.Count; i++)
            {
                MakeSlot slot = curWorkingSlots[i];
                slot.SetSelfBtnInteractable(true);
                if (animType != ESlotAnimType.JustAnim) slot.SetAnimProgress(1);

                //更新位置
                updateSingleSlotPos(slot, doAnim);
            }
        }
        else
        {
            for (int i = 0; i < curWorkingSlots.Count; i++)
            {
                MakeSlot slot = curWorkingSlots[i];

                //设置其是否可以点击    6   
                if (slot.slotIndex < allSlotNum - getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1).Count + 1)
                {
                    if (slot.slotIndex < (startGroupNum - 1))
                    {
                        slot.SetSelfBtnInteractable(false);
                        if (animType != ESlotAnimType.JustAnim) slot.SetAnimProgress(0);
                    }
                    else
                    {
                        slot.SetSelfBtnInteractable(true);
                        if (animType != ESlotAnimType.JustAnim) slot.SetAnimProgress(1);
                    }
                }
                else
                {
                    slot.SetSelfBtnInteractable(false);
                    if (animType != ESlotAnimType.JustAnim) slot.SetAnimProgress(0);
                }

                //更新位置
                updateSingleSlotPos(slot, doAnim);
            }
        }

    }

    //更新装备制造槽位位置
    private void updateSingleSlotPos(MakeSlot slot, bool doAnim)
    {
        var startGroup = getWorkingSlotsByGroup(0);

        int startGroupNum = needReset ? 1 : startGroup.Count;

        Vector3 targetPos = calSingleSlotPos(slot.slotIndex, slot.slotGroup, startGroupNum);
        RectTransform rect = slot.transform as RectTransform;

        ////TEST 需要注释
        //doAnim = false;
        if (!doAnim)
        {
            rect.localPosition = targetPos;
        }
        else
        {
            //播放动画
            //if (DOTween.IsTweening(rect))
            //{
            //    DOTween.Kill(rect);
            //}
            Tween tween = rect.DOLocalMove(targetPos, 0.2f);

            if (getCurWorkingSlots().Count <= StaticConstants.ShowSlotNum) //不进行渐变效果
            {
                slot.SetAnimProgress(1);
                return;
            }

            bool aniLeftToRight = targetPos.x > rect.localPosition.x;

            if (aniLeftToRight) //从左往右时
            {
                if (slot == rightSlot)
                {
                    tween.OnUpdate(() =>
                    {
                        float aniScale = aniLeftToRight == beginDragDirection ? ((aniLeftToRight ? targetPos.x - rect.localPosition.x : -(targetPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset)
                             : ((aniLeftToRight ? rightOriPos.x - rect.localPosition.x : -(rightOriPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset);

                        aniScale = Mathf.Clamp01(aniScale);
                        aniScale = aniLeftToRight ? aniScale : 1 - aniScale;
                        slot.SetAnimProgress(aniScale);
                    });
                }

                if (slot == leftSlot)
                {
                    tween.OnUpdate(() =>
                    {
                        float aniScale = aniLeftToRight == beginDragDirection ? ((aniLeftToRight ? targetPos.x - rect.localPosition.x : -(targetPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset)
                            : ((aniLeftToRight ? leftOriPos.x - rect.localPosition.x : -(leftOriPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset);

                        aniScale = Mathf.Clamp01(aniScale);
                        aniScale = aniLeftToRight ? 1 - aniScale : aniScale;
                        slot.SetAnimProgress(aniScale);
                    });
                }
            }
            else //从右往左时
            {
                if (slot == rightSlot)
                {
                    tween.OnUpdate(() =>
                    {
                        float aniScale = aniLeftToRight == beginDragDirection ? ((aniLeftToRight ? targetPos.x - rect.localPosition.x : -(targetPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset)
                             : ((aniLeftToRight ? rightOriPos.x - rect.localPosition.x : -(rightOriPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset);

                        aniScale = Mathf.Clamp01(aniScale);
                        aniScale = aniLeftToRight ? aniScale : 1 - aniScale;
                        slot.SetAnimProgress(aniScale);
                    });
                }

                if (slot == leftSlot)
                {
                    tween.OnUpdate(() =>
                    {
                        float aniScale = aniLeftToRight == beginDragDirection ? ((aniLeftToRight ? targetPos.x - rect.localPosition.x : -(targetPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset)
                             : ((aniLeftToRight ? leftOriPos.x - rect.localPosition.x : -(leftOriPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset);

                        aniScale = Mathf.Clamp01(aniScale);
                        aniScale = aniLeftToRight ? 1 - aniScale : aniScale;

                        slot.SetAnimProgress(aniScale);
                    });
                }
            }

        }

    }

    //计算单个槽位的位置
    /// <summary>
    /// 
    /// </summary>
    /// <param name="startGroupNum">最右侧组内数量</param>
    /// <returns></returns>
    private Vector3 calSingleSlotPos(int slotIndex, int slotGroup, int startGroupNum)
    {
        if (!isPosInit)
        {
            makeSlotSignPoses = new Vector3[contentPane.makeSlotSigns.Length];
            for (int i = 0; i < contentPane.makeSlotSigns.Length; i++)
            {
                makeSlotSignPoses[i] = contentPane.makeSlotSigns[i].localPosition;
            }
            isPosInit = true;
        }

        Vector3 result = Vector3.zero;

        if (slotIndex < StaticConstants.ShowSlotNum + startGroupNum - 1)
        {
            result = makeSlotSignPoses[slotGroup];
            if (slotIndex < startGroupNum)
            {
                result += Vector3.right * StaticConstants.SlotOffset * (startGroupNum - (slotIndex + 1));
            }
        }
        else
        {
            result = makeSlotSignPoses[StaticConstants.ShowSlotNum - 1];
            result -= Vector3.right * StaticConstants.SlotOffset * (slotIndex + 1 - StaticConstants.ShowSlotNum - startGroupNum + 1);
        }


        return result;
    }

    MakeSlot[] _moveSlots = new MakeSlot[StaticConstants.ShowSlotNum - 1];

    //获得当前工作的槽位列表
    List<MakeSlot> getCurWorkingSlots()
    {
        List<int> workingSlotIds = EquipDataProxy.inst.GetAllWorkEquipMakeSlot();//工作槽位数量

        List<MakeSlot> workingSlots = new List<MakeSlot>();
        workingSlotIds.ForEach((t) =>
        {
            workingSlots.Add(equipMakeSlots[t]);
        });

        workingSlots.Sort((a, b) => a.slotIndex.CompareTo(b.slotIndex));

        return workingSlots;
    }

    //获得当前工作的槽位列表中该组的列表
    List<MakeSlot> getWorkingSlotsByGroup(int group)
    {
        return getCurWorkingSlots().FindAll(t => t.slotGroup == group);
    }


    void getCurMoveSlots()
    {
        isChanged = false;

        if (beginDragDirection)
        {
            for (int i = 0; i < _moveSlots.Length; i++)
            {
                _moveSlots[i] = getWorkingSlotsByGroup(i + 1)[0];
            }
        }
        else
        {
            var firstGroup = getWorkingSlotsByGroup(0);
            _moveSlots[0] = firstGroup[firstGroup.Count - 1];
            for (int i = 1; i < _moveSlots.Length; i++)
            {
                _moveSlots[i] = getWorkingSlotsByGroup(i)[0];
            }

        }

    }

    public void onBeginDrag(Vector2 delta)
    {
        if (!contentPane.slotSR.enabled) return;

        float deltaX = delta.x;
        if (deltaX == 0)
        {
            canDrag = false;
            return;
        }

        beginDragDirection = deltaX > 0;
        canDrag = true;

        if (beginDragDirection)
        {
            if (getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1).Count == 1) //最左侧只有一个，无法拖拽
            {
                //Logger.error("最左侧的兄弟已经被看光了，别拉了");
                canDrag = false;
            }
        }
        else
        {
            if (getWorkingSlotsByGroup(0).Count == 1) //最右侧只有一个，无法拖拽
            {
                //Logger.error("最右侧的兄弟已经被看光了，别拉了");
                canDrag = false;
            }

        }

        for (int i = 0; i < StaticConstants.ShowSlotNum; i++)
        {
            if (getWorkingSlotsByGroup(i).Count <= 0)
            {
                canDrag = false;
            }
        }

        if (!canDrag) return;

        isOnDrag = true;
        getCurMoveSlots();
    }

    public void onDrag(Vector2 delta)
    {
        if (!contentPane.slotSR.enabled) return;

        if (!canDrag)
        {
            onBeginDrag(delta);
            return;
        }

        float deltaX = delta.x;

        if (deltaX == 0) return;

        if (deltaX > 0)//向右拖拽 
        {
            onDragDirection = true;
        }
        else //向左拖拽
        {
            onDragDirection = false;

            //更新层级
            var list = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1 - 1);
            list.ForEach((t) =>
            {
                t.transform.SetAsLastSibling();
            });
        }

        isChanged = false;

        var startGroup = getWorkingSlotsByGroup(0);
        var endGroup = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1);

        if (startGroup.Count == 0 || endGroup.Count == 0)
        {
            canDrag = false;
            return;
        }

        rightSlot = onDragDirection == beginDragDirection ? (onDragDirection ? startGroup[startGroup.Count > 1 ? startGroup.Count - 1 : 0] : startGroup[startGroup.Count > 2 ? startGroup.Count - 2 : 0])
            : rightSlot;//右侧的那位哥们
        leftSlot = onDragDirection == beginDragDirection ? onDragDirection ? endGroup[endGroup.Count > 1 ? 1 : 0] : endGroup[0]
            : leftSlot; //左侧的那位哥们

        leftOriPos = calSingleSlotPos(leftSlot.slotIndex, leftSlot.slotGroup, getWorkingSlotsByGroup(0).Count);
        rightOriPos = calSingleSlotPos(rightSlot.slotIndex, rightSlot.slotGroup, getWorkingSlotsByGroup(0).Count);


        List<MakeSlot> curWorkingSlots = getCurWorkingSlots();
        for (int i = 0; i < curWorkingSlots.Count; i++)
        {
            MakeSlot slot = curWorkingSlots[i];
            var rect = slot.transform as RectTransform;
            int newSlotGroup = getSlotNextGroup(slot);

            Vector3 originPos = calSingleSlotPos(slot.slotIndex, slot.slotGroup, getWorkingSlotsByGroup(0).Count);//beginDragDirection == onDragDirection ? calSingleSlotPos(slot.slotIndex, slot.slotGroup, getWorkingSlotsByGroup(0).Count) : rect.localPosition;
            Vector3 targetPos = calSingleSlotPos(slot.slotIndex, _moveSlots.Contains(slot) ? newSlotGroup : slot.slotGroup, getWorkingSlotsByGroup(0).Count + (beginDragDirection ? +1 : -1));
            //Logger.error("rect.localPosition : " + rect.localPosition.ToString() + "   targetPos : " + targetPos.ToString());

            if (_moveSlots.Contains(slot))
            {
                //float way_long = onDragDirection ? targetPos.x - originPos.x : originPos.x - targetPos.x;
                //Logger.error("long way_long : " + way_long);

                Vector3 expectVal = rect.localPosition + Vector3.right * deltaX;

                if (onDragDirection ? targetPos.x - expectVal.x > 0 : expectVal.x - targetPos.x > 0)
                {
                    rect.localPosition = expectVal;
                }
                else
                {
                    rect.localPosition = targetPos;
                    isChanged = true;
                    break;
                }

            }
            else
            {
                ///以中间三个为主
                float way_long = contentPane.makeSlotSigns[1].localPosition.x - contentPane.makeSlotSigns[2].localPosition.x;//中间三个之间的间距
                float moveDistance = deltaX / way_long * StaticConstants.SlotOffset;

                //Logger.error("实际移动的距离moveDistance : " + moveDistance);

                Vector3 expectVal = rect.localPosition + Vector3.right * moveDistance;
                rect.localPosition = expectVal;

                if (slot == rightSlot)
                {
                    float aniScale = onDragDirection == beginDragDirection ? ((onDragDirection ? targetPos.x - rect.localPosition.x : -(targetPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset)
                            : ((onDragDirection ? originPos.x - rect.localPosition.x : -(originPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset);

                    aniScale = Mathf.Clamp01(aniScale);
                    aniScale = onDragDirection ? aniScale : 1 - aniScale;
                    slot.SetAnimProgress(aniScale);
                }

                if (slot == leftSlot)
                {
                    float aniScale = onDragDirection == beginDragDirection ? ((onDragDirection ? targetPos.x - rect.localPosition.x : -(targetPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset)
                            : ((onDragDirection ? originPos.x - rect.localPosition.x : -(originPos.x - rect.localPosition.x)) / StaticConstants.SlotOffset);

                    aniScale = Mathf.Clamp01(aniScale);
                    aniScale = onDragDirection ? 1 - aniScale : aniScale;
                    slot.SetAnimProgress(aniScale);
                }


            }
        }

        if (isChanged)
        {
            for (int i = 0; i < _moveSlots.Length; i++)
            {
                var slot = _moveSlots[i];
                int newSlotGroup = getSlotNextGroup(slot);
                slot.slotGroup = newSlotGroup;
            }

            updateSlotsPos(ESlotAnimType.Draging);
            onBeginDrag(delta);
        }
    }


    int getSlotNextGroup(MakeSlot slot)
    {
        int theRealSlotGroup = slot.slotGroup + (beginDragDirection ? -1 : +1);
        int newSlotGroup = slot.slotGroup + (onDragDirection ? -1 : +1);

        if (newSlotGroup != theRealSlotGroup) newSlotGroup = slot.slotGroup;//等于自家的

        return newSlotGroup;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slot">自身Item</param>
    /// <returns></returns>


    private void checkSlotIsIn(MakeSlot slot)
    {
        var rect1 = slot.transform as RectTransform;

        var newSlotGroup = getSlotNextGroup(slot);

        if (newSlotGroup >= StaticConstants.ShowSlotNum || newSlotGroup < 0) return;

        RectTransform rect2 = contentPane.makeSlotSigns[newSlotGroup];//参照物

        float scale;
        if (DoTweenUtil.RectTransformOverlap(rect1, rect2, out scale))
        {
            if (scale >= 0.5f)
            {
                slot.slotGroup = newSlotGroup;
            }
        }
    }

    public void onEndDrag(Vector2 delta)
    {
        isOnDrag = false;
        if (!canDrag || !contentPane.slotSR.enabled) return;

        //判断过半 更新组
        for (int i = 0; i < _moveSlots.Length; i++)
            checkSlotIsIn(_moveSlots[i]);


        updateSlotsPos(ESlotAnimType.JustAnim);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="slotId"></param>
    /// <param name="equipUid"></param>
    /// <param name="toStoreBasket">1 - box else - shelf</param>
    public void SetEquipFly(int slotId, string equipUid, int toStoreBasket)
    {
        if (toStoreBasket == 1)
        {
            Vector3 startPos = GetStartPos(slotId);
            Vector3 endPos = UserDataProxy.inst.GetOneTrunkPosition(out int trunkUid);

            Vector3 endScale = new Vector3(0.2f, 0.2f, 0.2f);
            GameObject spriteObj = new GameObject("spriteRender");
            SpriteRenderer spriteRender = spriteObj.AddComponent<SpriteRenderer>();
            EquipItem equip = ItemBagProxy.inst.GetEquipItem(equipUid);
            string qcolor = equip.quality > 1 ? StaticConstants.qualityColor[equip.quality - 1] : "";
            //Material mat = new Material(GUIHelper.GetSceneOutlineMat());
            //mat.SetColor("_OutlineColor", string.IsNullOrEmpty(qcolor) ? GUIHelper.GetColorByColorHex("000000") : GUIHelper.GetColorByColorHex(qcolor));
            //mat.SetFloat("_Width", 0.002f);
            //spriteRender.material = mat;
            spriteRender.material = GUIHelper.GetSceneOutlineMat();
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_OutlineColor", string.IsNullOrEmpty(qcolor) ? GUIHelper.GetColorByColorHex("000000") : GUIHelper.GetColorByColorHex(qcolor));
            materialPropertyBlock.SetFloat("_Width", 0.002f);
            spriteRender.SetPropertyBlock(materialPropertyBlock);

            AtlasAssetHandler.inst.GetAtlasSprite(equip.equipConfig.equipDrawingsConfig.atlas, equip.equipConfig.equipDrawingsConfig.icon, (gsprite) =>
            {
                spriteRender.enabled = false;
                var sex = spriteRender.gameObject.AddComponent<SpriteEX>();
                sex.mGSprite = gsprite;
            });


            spriteRender.transform.localScale = Vector3.one * 0.44f;
            PlayerEquipFlyAnimBySlot(spriteRender, startPos, endPos, endScale, true, () =>
            {
                if (IndoorMap.inst.GetFurnituresByUid(trunkUid, out Furniture trunkEntity))
                {
                    trunkEntity.EquipFlyOnAnim();
                }
                GameObject.Destroy(spriteRender.gameObject);

            });

            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("没有{0}可用的家具，已存放进物品栏", LanguageManager.inst.GetValueByKey(equip.equipConfig.equipDrawingsConfig.name)), GUIHelper.GetColorByColorHex("FFD907"));
        }
        else
        {
            this.slotId = slotId;

            //SpriteRenderer spriteRenderer = null;

            //if (EquipDataProxy.inst.spriteRender != null)
            //{
            //    Logger.error("!!!!!!!!!!!!!");

            //    spriteRenderer = EquipDataProxy.inst.spriteRender;
            //    Vector3 startPos = GetStartPos(slotId);
            //    Vector3 endPos = spriteRenderer.transform.localPosition;
            //    Vector3 endScale = spriteRenderer.transform.localScale;

            //    PlayEquipFlyAnim(spriteRenderer, startPos, endPos, endScale, false);

            //    EquipDataProxy.inst.spriteRender = null;
            //}
        }
    }

    private Vector3 GetStartPos(int slotId)
    {
        Vector3 startVector3;

        var pos = FGUI.inst.uiCamera.WorldToScreenPoint(equipMakeSlots[slotId].transform.position);
        pos.y = Mathf.Max(contentPane.bottomAniTf.rect.height + 10, pos.y);

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), pos, Camera.main, out startVector3))
        {
            return startVector3;
        }

        return startVector3;
    }

    //从制作栏位上出来往 货架/储物箱 飞
    private void PlayerEquipFlyAnimBySlot(SpriteRenderer spriteRender, Vector3 startPos, Vector3 endPos, Vector3 endScale, bool flyToBox, Action callback = null)
    {
        if (spriteRender == null) return;

        spriteRender.enabled = true;
        spriteRender.transform.position = new Vector3(startPos.x, startPos.y, 0);
        string sortingLayerName = spriteRender.sortingLayerName;
        int sortingOrder = spriteRender.sortingOrder;
        GUIHelper.setRandererSortinglayer(spriteRender.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder);

        spriteRender.transform.DOLocalMoveY(spriteRender.transform.localPosition.y + (1.6f * (D2DragCamera.inst.camera.orthographicSize / 6.5f)), 0.3f).OnStart(() =>
        {
            EffectManager.inst.Spawn(3025, Vector3.zero, (gamevfx) =>
            {
                if (this == null || gamevfx == null) return;
                gamevfx.transform.SetParent(spriteRender.transform, true);
                gamevfx.transform.localScale = Vector3.one;
                gamevfx.transform.localPosition = Vector3.zero;
                gamevfx.gameObject.SetActive(true);
                GUIHelper.setRandererSortinglayer(gamevfx.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder);
            });
        });

        spriteRender.transform.DOScale(0.9f * (D2DragCamera.inst.camera.orthographicSize / 6.5f), 0.5f).SetEase(Ease.OutCubic).SetDelay(0.1f).OnComplete(() =>
        {

            ManagerBinder.inst.Asset.loadPrefabAsync("Trail_tuowei", spriteRender.transform, (vfx) =>
            {
                if (spriteRender == null || vfx == null) return;

                vfx.transform.localPosition = Vector3.zero;


                float animTime = 0.6f;
                float timeCount = 0;
                DOTween.To(() => timeCount, a => timeCount = a, 0, animTime / 2).OnComplete(() =>
                {
                    if (!flyToBox)
                    {
                        GUIHelper.setRandererSortinglayer(spriteRender.transform, sortingLayerName, sortingOrder);
                    }
                });

                DOTween.To(() => spriteRender.transform.localPosition, x => spriteRender.transform.localPosition = x, endPos, animTime).SetEase(Ease.OutSine).SetDelay(0.1f);
                DOTween.To(() => spriteRender.transform.localScale, x => spriteRender.transform.localScale = x, endScale, animTime).SetEase(Ease.OutSine).SetDelay(0.1f).OnComplete(() =>
                {
                    GameObject.Destroy(vfx.gameObject);
                    callback?.Invoke();
                });

                if (flyToBox)
                {
                    spriteRender.transform.DORotate(new Vector3(0, 0, 0), animTime);
                }
            });
        });
    }

    private void PlayEquipFlyAnim(SpriteRenderer spriteRender, Vector3 startPos, Vector3 endPos, Vector3 endScale, bool flyToBox, Action callback = null)
    {
        spriteRender.enabled = true;
        spriteRender.transform.position = new Vector3(startPos.x, startPos.y, 0);
        string sortingLayerName = spriteRender.sortingLayerName;
        int sortingOrder = spriteRender.sortingOrder;
        GUIHelper.setRandererSortinglayer(spriteRender.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder);
        spriteRender.transform.localScale = endScale * 0.5f;

        ManagerBinder.inst.Asset.loadPrefabAsync("Trail_tuowei", spriteRender.transform, (vfx) =>
        {
            vfx.transform.localPosition = Vector3.zero;


            float animTime = 0.6f;
            float timeCount = 0;
            DOTween.To(() => timeCount, a => timeCount = a, 0, animTime / 2).OnComplete(() =>
            {
                if (!flyToBox)
                {
                    GUIHelper.setRandererSortinglayer(spriteRender.transform, sortingLayerName, sortingOrder);
                }
            });

            DOTween.To(() => spriteRender.transform.localPosition, x => spriteRender.transform.localPosition = x, endPos, animTime).SetEase(Ease.OutSine);
            DOTween.To(() => spriteRender.transform.localScale, x => spriteRender.transform.localScale = x, endScale, animTime).SetEase(Ease.OutSine).OnComplete(() =>
            {
                GameObject.Destroy(vfx.gameObject);
                callback?.Invoke();
            });

            if (flyToBox)
            {
                spriteRender.transform.DORotate(new Vector3(0, 0, 0), animTime);
            }
        });

    }
}
