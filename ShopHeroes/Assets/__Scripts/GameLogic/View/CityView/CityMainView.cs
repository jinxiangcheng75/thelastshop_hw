using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CityMainView : ViewBase<CityUIComp>
{
    public override string viewID => ViewPrefabName.CityUI;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "window";
    private Dictionary<int, ExploreSlotItem> slotDic = new Dictionary<int, ExploreSlotItem>();
    int slotState = 0; // 0 - 满了 1 - 没满
    int curSlotIndex = 0;
    private int springBackTimer;
    private bool isOnAnim;
    bool isPosInit;
    Vector3[] makeSlotSignPoses;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;
        contentPane.shopBtn.ButtonClickTween(() =>
        {
            //GameStateEvent.inst.changeState(new StateTransition(kGameState.Shop, false));
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Shop, true));
        });

        contentPane.ruinsBtn.ButtonClickTween(() =>
        {
            if (WorldParConfigManager.inst.GetConfig(182) != null)
            {
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(182).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(182).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
            }
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Ruins, true));
        });

        contentPane.exploreBtn.ButtonClickTween(() =>
        {
            if (contentPane == null) return;
            if (slotState == 0)
            {
                var cfg = FieldConfigManager.inst.GetFieldConfig(4, ExploreDataProxy.inst.slotCount + 1);
                if (cfg == null)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("槽位已满"), GUIHelper.GetColorByColorHex("FFD907"));
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREBUYSLOT_SHOWUI, ExploreDataProxy.inst.slotCount);
                }
            }
            else
            {
                var slotData = ExploreDataProxy.inst.GetFreeSlotData();

                if (slotData != null)
                {
                    ExploreDataProxy.inst.isOpenExplorePanel = true;
                    ExploreDataProxy.inst.curSlotId = slotData.slotId;
                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREGROUPDATA);
                }
            }
        });

        contentPane.heroBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
        });

        contentPane.marketBtn.ButtonClickTween(() =>
        {
            if (WorldParConfigManager.inst.GetConfig(101) != null)
            {
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(101).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(101).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
            }

            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, 2100);
        });

        contentPane.unionBtn.ButtonClickTween(() =>
        {
            if (WorldParConfigManager.inst.GetConfig(132) != null)
            {
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(132).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(132).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
            }

            EventController.inst.TriggerEvent(GameEventType.UnionEvent.ENTER_UNIONSCENE);

            //EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, 2200);
        });

        contentPane.chatBtn.ButtonClickTween(() =>
        {
            if (WorldParConfigManager.inst.GetConfig(137) != null)
            {
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(137).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(137).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
            }

            EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_SHOWVIEW);
        });

        contentPane.tBoxBtn.ButtonClickTween(() =>
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                if ((K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuide && (K_Guide_Type)cfg.guide_type != K_Guide_Type.WeakGuideAndTask && (K_Guide_Type)cfg.guide_type != K_Guide_Type.RestrictClick && (K_Guide_Type)cfg.guide_type != K_Guide_Type.TipsAndRestrictClick && cfg.btn_name != "宝箱按钮") return;
            }
            if (WorldParConfigManager.inst.GetConfig(403) != null)
            {
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(403).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(403).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
            }

            //EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.OPENTBOXUINOTPARA);
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.TBox, true));
        });

        //动画
        contentPane.slotSR.onBeginDragHandle += onBeginDrag;
        contentPane.slotSR.onDragHandle += onDrag;
        contentPane.slotSR.onEndDragHandle += onEndDrag;

        slotDic.Clear();
        updateMakeSlots(ESlotAnimType.Normal);
    }
    int viewUpdateTimerId = 0;
    private void AllSlotHide()
    {
        for (int i = 0; i < contentPane.allSlots.Count; i++)
        {
            contentPane.allSlots[i].gameObject.SetActive(false);
        }
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        showSystemBtn();
    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();
        hideSystemBtn();
    }

    public void showSystemBtn()
    {
        if (isOnAnim) return;
        updateSlotsPos(ESlotAnimType.Normal);
        isOnAnim = GameSettingManager.inst.needShowUIAnim;
        contentPane.slotSR.enabled = isOnAnim ? false : true;

        //if (!isOnAnim) contentPane.chatBtn.GetComponent<Graphic>().FadeFromTransparentTween(1, 0.3f);

        //DOTween.Kill(contentPane.bottomAniTf, true);

        contentPane.bottomAniTf.DOAnchorPos3DY(0, 0.3f).From(-500f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (!GameSettingManager.inst.needShowUIAnim) return;
            contentPane.leftAnimTf.anchoredPosition = new Vector2(0, contentPane.leftAnimTf.anchoredPosition.y);
            contentPane.rightAnimTf.anchoredPosition = new Vector2(0, contentPane.rightAnimTf.anchoredPosition.y);
            //DoTweenUtil.MainUIBtnCommonTween_Front(contentPane.shopBtnImg, contentPane.shopBtnTx);
            //DoTweenUtil.MainUIBtnCommonTween_Front(contentPane.heroBtnImg, contentPane.heroBtnTx);
            //DoTweenUtil.MainUIBtnCommonTween_Front(contentPane.marketBtnImg, contentPane.marketBtnTx);
            //DoTweenUtil.MainUIBtnCommonTween_Front(contentPane.unionBtnImg, contentPane.unionBtnTx);
            //DoTweenUtil.MainUIBtnCommonTween_Front(contentPane.tBoxBtnImg, contentPane.tBoxBtnTx);

            //contentPane.chatBtn.GetComponent<Graphic>().FadeFromTransparentTween(1, 0.2f);

            contentPane.exploreBtnImg.DOAnchorPos3DX(-115, 0.15f).From(150f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                var curWorkingSlots = getCurWorkingSlots();

                foreach (var item in curWorkingSlots)
                {
                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 30);
                    item.transform.DOLocalMoveX(calSingleSlotPos(item.slotIndex, item.slotGroup, getWorkingSlotsByGroup(0).Count).x, 0.3f).From(contentPane.exploreBtnImg.position.x).OnComplete(() =>
                    {
                        item.transform.DOLocalRotate(Vector3.zero, 0.15f).OnComplete(() =>
                        {
                            if (isOnAnim) isOnAnim = false;
                            contentPane.slotSR.enabled = curWorkingSlots.Count > StaticConstants.ShowSlotNum;
                        });
                    });
                }

                contentPane.makeSlotListContent.gameObject.SetActiveTrue();
                contentPane.exploreBtnImg.DOLocalRotate(Vector3.zero, 0.15f).OnComplete(() =>
                {
                    if (curWorkingSlots.Count == 0 && isOnAnim) isOnAnim = false;
                });
            }).OnStart(() =>
            {
                contentPane.exploreBtnImg.rotation = Quaternion.Euler(Vector3.forward * 30);
                contentPane.exploreBtnImg.gameObject.SetActiveTrue();
            });

        }).OnStart(() =>
        {
            if (!GameSettingManager.inst.needShowUIAnim) return;
            //contentPane.shopBtnImg.gameObject.SetActiveFalse();
            //contentPane.heroBtnImg.gameObject.SetActiveFalse();
            //contentPane.marketBtnImg.gameObject.SetActiveFalse();
            //contentPane.unionBtnImg.gameObject.SetActiveFalse();
            //contentPane.tBoxBtnImg.gameObject.SetActiveFalse();
            //contentPane.shopBtnTx.gameObject.SetActiveFalse();
            //contentPane.heroBtnTx.gameObject.SetActiveFalse();
            //contentPane.marketBtnTx.gameObject.SetActiveFalse();
            //contentPane.unionBtnTx.gameObject.SetActiveFalse();
            //contentPane.tBoxBtnTx.gameObject.SetActiveFalse();

            contentPane.exploreBtnImg.gameObject.SetActiveFalse();
            contentPane.makeSlotListContent.gameObject.SetActiveFalse();

            //contentPane.chatBtn.GetComponent<Graphic>().DOFade(0, 0);
        });

        contentPane.leftPlaneAnim.DOPlayBackwards();
    }

    public void hideSystemBtn()
    {
        //contentPane.chatBtn.GetComponent<Graphic>().FadeTransparentTween(1, 0.1f, false);
        DOTween.Kill(contentPane.bottomAniTf, true);
        //DOTween.Kill(contentPane.chatBtn.GetComponent<Graphic>(), true);
        isOnAnim = false;
        contentPane.bottomAniTf.DOAnchorPos3DY(-500f, 0.1f).From(0f).From(0f);
        contentPane.leftAnimTf.anchoredPosition = new Vector2(-3000, contentPane.leftAnimTf.anchoredPosition.y);
        contentPane.rightAnimTf.anchoredPosition = new Vector2(3000, contentPane.rightAnimTf.anchoredPosition.y);
    }

    bool isShowin = true;
    public override void shiftIn()
    {
        if (getCurWorkingSlots().Count != 0) updateSlotsPos(ESlotAnimType.Normal);
        if (isShowin) return;
        isShowin = true;
        RefreshHeroIdleCount();
        RefreshRedPoints();
        RefreshUnlockBtn();
        RefreshBtnActive();
        if (GameSettingManager.inst.needShowUIAnim)
            showSystemBtn();
        else
        {
            contentPane.bottomAniTf.DOAnchorPos3DY(0, 0).From(-500f);
            contentPane.leftAnimTf.anchoredPosition = new Vector2(0, contentPane.leftAnimTf.anchoredPosition.y);
        }

        UpdateBtnRedDot();
    }

    public void RefreshHeroIdleCount()
    {
        contentPane.heroIdleCountText.text = RoleDataProxy.inst.GetIdleStateHeroCount.ToString();
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
            //contentPane.heroIdleCountText.text = RoleDataProxy.inst.GetIdleStateHeroCount.ToString();
        }
    }

    private void RefreshBtnActive()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            contentPane.chatBtn.gameObject.SetActive(false);
            return;
        }
        contentPane.chatBtn.gameObject.SetActive(true);
    }

    private void RefreshUnlockBtn()
    {
        if (WorldParConfigManager.inst.GetConfig(101) != null)
            GUIHelper.SetUIGray(contentPane.marketBtn.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(101).parameters);
        if (WorldParConfigManager.inst.GetConfig(132) != null)
            GUIHelper.SetUIGray(contentPane.unionBtn.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(132).parameters);
        if (WorldParConfigManager.inst.GetConfig(403) != null)
            GUIHelper.SetUIGray(contentPane.tBoxBtn.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(403).parameters);
        if (WorldParConfigManager.inst.GetConfig(137) != null)
            GUIHelper.SetUIGray(contentPane.chatBtn.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(137).parameters);
        if (WorldParConfigManager.inst.GetConfig(182) != null)
            GUIHelper.SetUIGray(contentPane.ruinsBtn.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(182).parameters);
    }

    public void UpdateBtnRedDot()
    {
        var wdpCfg = WorldParConfigManager.inst.GetConfig(137);
        int isShowRedLv = 0;
        if (wdpCfg != null)
            isShowRedLv = (int)wdpCfg.parameters;
        contentPane.chatRedPoint.SetActive(ChatDataProxy.inst.hasRedTip && UserDataProxy.inst.playerData.level >= isShowRedLv);
    }

    public void RefreshRedPoints()
    {
        refreshTBoxRedPoint();
        refreshMarketRedPoint();
        HotfixBridge.inst.TriggerLuaEvent("cityUI_refreshRedPoints");
    }

    private void refreshMarketRedPoint()
    {
        contentPane.obj_marketRedPoint.SetActive(MarketDataProxy.inst.redPointShow);
    }

    private void refreshTBoxRedPoint()
    {
        contentPane.tBoxRedPoint.SetActive(TreasureBoxDataProxy.inst.newBoxGroupId != 0);
    }

    public override void shiftOut()
    {
        if (!isShowin) return;
        isShowin = false;
        hideSystemBtn();
    }

    #region 拖拽制造槽效果

    bool beginDragDirection = true;//从左往右
    bool isChanged;
    bool needReset = true;
    bool onDragDirection = true;//True 从左往右 False 从右往左
    bool canDrag;
    bool isOnDrag;//是否正在拖拽
    ExploreSlotItem leftSlot;
    Vector3 leftOriPos;
    ExploreSlotItem rightSlot;
    Vector3 rightOriPos;

    void springbackMethod()
    {
        if (isOnDrag || getWorkingSlotsByGroup(0).Count <= 1 || !isShowing) return;

        contentPane.slotSR.SetSpringback(false, getWorkingSlotsByGroup(0).Count - 1);
    }

    //刷新装备制造槽位
    public void updateMakeSlots(ESlotAnimType animType)
    {
        int notWorkedNum = 0;

        if (animType == ESlotAnimType.Normal) needReset = true;
        else if (animType == ESlotAnimType.MakeEnd && isOnDrag)  //如果拖拽中制作完成 仅刷新 
        {
            animType = ESlotAnimType.Refresh;
        }

        for (int i = 0; i < ExploreDataProxy.inst.slotCount; i++)
        {
            var _slot = ExploreDataProxy.inst.GetMakeSlotByIndex(i);
            bool isWorkering = _slot.exploreState != 0;
            if (!isWorkering) notWorkedNum++;

            if (slotDic.ContainsKey(_slot.slotId))
            {
                slotDic[_slot.slotId].gameObject.SetActive(isWorkering);
                slotDic[_slot.slotId].UpdateState(_slot);
            }
            else
            {
                var newGo = GameObject.Instantiate(contentPane.makeSlotItemGO.gameObject, contentPane.makeSlotListContent);
                newGo.SetActive(true);
                newGo.name = _slot.slotId.ToString();
                ExploreSlotItem makeslot = newGo.GetComponent<ExploreSlotItem>();
                //LanguageManager.inst.AddChangeText(makeslot.timeText);
                makeslot.UpdateState(_slot);
                slotDic.Add(_slot.slotId, makeslot);
                makeslot.gameObject.SetActive(isWorkering);
            }

            if (isWorkering && animType != ESlotAnimType.Refresh) slotDic[_slot.slotId].slotIndex = i - notWorkedNum;
        }

        if (animType != ESlotAnimType.Refresh)
        {
            if (animType == ESlotAnimType.MakeEnd && getCurWorkingSlots().Count >= 4)
            {
                ///根据收起装备的位置决定方向
                var startGroup = getWorkingSlotsByGroup(0);
                var endGroup = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1);
                bool direction = endGroup.Count > 1;

                if (startGroup.Count != 0) rightSlot = !direction ? startGroup[startGroup.Count > 2 ? startGroup.Count - 2 : 0] : null;//右侧的那位哥们
                if (endGroup.Count != 0) leftSlot = direction ? endGroup[endGroup.Count > 1 ? 1 : 0] : null;//左侧的那位哥们
            }


            updateSlotsPos(animType);
        }
        needReset = false;

        contentPane.slotSR.enabled = getCurWorkingSlots().Count > StaticConstants.ShowSlotNum;
        contentPane.residueText.text = ExploreDataProxy.inst.SlotIdleCount == 0 ? "+" : ExploreDataProxy.inst.SlotIdleCount.ToString();
        slotState = ExploreDataProxy.inst.SlotIdleCount == 0 ? 0 : 1;
    }

    bool needSetSiblingIndexBySlotAnimType(ESlotAnimType animType)
    {
        if (animType == ESlotAnimType.Normal || animType == ESlotAnimType.MakeEnd)
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
        if (animType == ESlotAnimType.Refresh || animType == ESlotAnimType.Normal)
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

        bool needSetSiblingIndex = needSetSiblingIndexBySlotAnimType(animType);
        bool doAnim = needDoAniBySlotAnimType(animType);

        List<ExploreSlotItem> curWorkingSlots = getCurWorkingSlots();
        //设置槽位组及渲染顺序
        for (int i = 0; i < curWorkingSlots.Count; i++)
        {
            updateSingleSlotGroup(curWorkingSlots[i], needSetSiblingIndex, animType == ESlotAnimType.MakeEnd);
        }

        var list = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1 - 1);
        list.ForEach((t) =>
        {
            t.transform.SetAsLastSibling();
        });

        //设置间距
        //int startGroupNum = getWorkingSlotsByGroup(0).Count;
        //contentPane.layoutGroup.spacing = 12 - (needReset ? 2 : getWorkingSlotsByGroup(0).Count * 2);
        //contentPane.layoutGroup.padding.right = (startGroupNum - 1) * 7;

        int allGroupNum = curWorkingSlots.Count;
        var startGroup = getWorkingSlotsByGroup(0);
        int startGroupNum = startGroup.Count;

        for (int i = 0; i < curWorkingSlots.Count; i++)
        {
            ExploreSlotItem slot = curWorkingSlots[i];

            //设置其是否可以点击
            if (slot.slotIndex < (StaticConstants.ShowSlotNum + startGroupNum - 1))
            {
                if (slot.slotIndex < (startGroupNum - 1))
                {
                    slot.SetSelfBtnInteractable(false);
                    if (animType != ESlotAnimType.JustAnim) slot.SetAnimProgress(allGroupNum < StaticConstants.ShowSlotNum ? 1 : 0);
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
                if (animType != ESlotAnimType.JustAnim) slot.SetAnimProgress(allGroupNum < StaticConstants.ShowSlotNum ? 1 : 0);
            }

            //更新位置
            updateSingleSlotPos(slot, doAnim);
        }


    }

    //更新装备制造槽位的组及渲染顺序
    void updateSingleSlotGroup(ExploreSlotItem slot, bool needSetSiblingIndex, bool isMakeEnd)
    {
        int startGroupNum = needReset ? 1 : getWorkingSlotsByGroup(0).Count;
        int endGroupNum = getWorkingSlotsByGroup(StaticConstants.ShowSlotNum - 1).Count;

        if ((startGroupNum == 0 || endGroupNum <= 1) && isMakeEnd)
        {
            if (getCurWorkingSlots().Count < StaticConstants.ShowSlotNum)
                slot.slotGroup = slot.slotIndex;
            else
                slot.slotGroup = System.Math.Max(slot.slotIndex - (getCurWorkingSlots().Count - StaticConstants.ShowSlotNum), 0);
        }
        else
        {
            if (slot.slotIndex < StaticConstants.ShowSlotNum + startGroupNum - 1)
            {
                if (needSetSiblingIndex) slot.transform.SetSiblingIndex(slot.slotIndex);

                if (slot.slotIndex < startGroupNum)
                    slot.slotGroup = 0;
                else
                    slot.slotGroup = slot.slotIndex - startGroupNum + 1;
            }
            else
            {
                if (needSetSiblingIndex) slot.transform.SetSiblingIndex(StaticConstants.ShowSlotNum - 1);
                slot.slotGroup = StaticConstants.ShowSlotNum - 1;
            }
        }

    }

    //更新装备制造槽位位置
    private void updateSingleSlotPos(ExploreSlotItem slot, bool doAnim = false)
    {
        var startGroup = getWorkingSlotsByGroup(0);

        int startGroupNum = needReset ? 1 : startGroup.Count;

        Vector3 targetPos = calSingleSlotPos(slot.slotIndex, slot.slotGroup, startGroupNum);
        RectTransform rect = slot.transform as RectTransform;

        if (!doAnim)
        {
            rect.localPosition = targetPos;
        }
        else
        {
            //播放动画
            Tween tween = rect.DOLocalMove(targetPos, 0.2f);

            if (getCurWorkingSlots().Count < StaticConstants.ShowSlotNum) return;//不进行渐变效果

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
            result = makeSlotSignPoses[slotGroup]; //contentPane.makeSlotSigns[slotGroup].localPosition /*+ contentPane.layoutGroup.transform.localPosition*/;
            if (slotIndex < startGroupNum)
            {
                result += Vector3.right * StaticConstants.SlotOffset * (startGroupNum - (slotIndex + 1)/* slot.slotIndex*/);
            }
        }
        else
        {
            result = makeSlotSignPoses[StaticConstants.ShowSlotNum - 1];//contentPane.makeSlotSigns[StaticConstants.ShowSlotNum - 1].localPosition /*+ contentPane.layoutGroup.transform.localPosition*/;
            result -= Vector3.right * StaticConstants.SlotOffset * (slotIndex + 1 - StaticConstants.ShowSlotNum - startGroupNum + 1);
        }


        return result;
    }

    ExploreSlotItem[] _moveSlots = new ExploreSlotItem[StaticConstants.ShowSlotNum - 1];

    //获得当前工作的槽位列表
    List<ExploreSlotItem> getCurWorkingSlots()
    {
        List<int> workingSlotIds = ExploreDataProxy.inst.GetAllWorkEquipMakeSlot();//工作槽位数量

        List<ExploreSlotItem> workingSlots = new List<ExploreSlotItem>();
        workingSlotIds.ForEach((t) =>
        {
            workingSlots.Add(slotDic[t]);
        });

        workingSlots.Sort((a, b) => a.slotIndex.CompareTo(b.slotIndex));

        return workingSlots;
    }

    //获得当前工作的槽位列表中该组的列表
    List<ExploreSlotItem> getWorkingSlotsByGroup(int group)
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

        if (!canDrag) return;

        isOnDrag = true;
        springBackTimer = 0;

        getCurMoveSlots();
    }

    public void onDrag(Vector2 delta)
    {
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

        rightSlot = onDragDirection == beginDragDirection ? (onDragDirection ? startGroup[startGroup.Count > 1 ? startGroup.Count - 1 : 0] : startGroup[startGroup.Count > 2 ? startGroup.Count - 2 : 0])
            : rightSlot;//右侧的那位哥们
        leftSlot = onDragDirection == beginDragDirection ? onDragDirection ? endGroup[endGroup.Count > 1 ? 1 : 0] : endGroup[0]
            : leftSlot; //左侧的那位哥们

        leftOriPos = calSingleSlotPos(leftSlot.slotIndex, leftSlot.slotGroup, getWorkingSlotsByGroup(0).Count);
        rightOriPos = calSingleSlotPos(rightSlot.slotIndex, rightSlot.slotGroup, getWorkingSlotsByGroup(0).Count);


        List<ExploreSlotItem> curWorkingSlots = getCurWorkingSlots();
        for (int i = 0; i < curWorkingSlots.Count; i++)
        {
            ExploreSlotItem slot = curWorkingSlots[i];
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
                    slot.slotGroup = newSlotGroup;
                    isChanged = true;
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
            updateSlotsPos(ESlotAnimType.Normal);
            onBeginDrag(delta);
        }
    }

    int getSlotNextGroup(ExploreSlotItem slot)
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
    private void checkSlotIsIn(ExploreSlotItem slot)
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
        if (!canDrag) return;

        //判断过半 更新组
        for (int i = 0; i < _moveSlots.Length; i++)
            checkSlotIsIn(_moveSlots[i]);

        //播放动画位移到正确位置
        updateSlotsPos(ESlotAnimType.JustAnim);
    }
    #endregion

    protected override void onShown()
    {
        isShowin = true;
        EventController.inst.AddListener(GameEventType.UpdateGameRedPoints, UpdateBtnRedDot);
        RefreshHeroIdleCount();
        RefreshRedPoints();
        RefreshUnlockBtn();
        RefreshBtnActive();

        viewUpdateTimerId = GameTimer.inst.AddTimer(1f, () =>
{
    springBackTimer++;
    if (springBackTimer >= 5)
    {
        springbackMethod();
        springBackTimer = 0;
    }
});

    }

    protected override void beforeDispose()
    {
        base.beforeDispose();
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateBtnRedDot);
    }

    protected override void onHide()
    {
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateBtnRedDot);
        foreach (var item in slotDic.Values)
        {
            item.ClearTime();
        }
        if (viewUpdateTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(viewUpdateTimerId);
        }
    }
}
