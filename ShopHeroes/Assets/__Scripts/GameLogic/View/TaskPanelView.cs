using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 任务系统  #陆泓屹
/// </summary>

public class TaskPanelView : ViewBase<TaskPanel>
{
    public override string viewID => ViewPrefabName.TaskPanel;
    public override string sortingLayerName => "window";

    int taskNumMax = 3;
    int _index = 0;
    List<FlyVfxCtrl> vfxCtrls = new List<FlyVfxCtrl>();
    int taskVfxTimer;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;

        contentPane.toggleGroup.OnSelectedIndexValueChange = onSelectedIndexValueChange;

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.bgBtn.onClick.AddListener(() =>
        {
            contentPane.bgBtn.interactable = false;
            hide();
        });

        contentPane.activeInfoBtn.ButtonClickTween(onLivenessInfoBtnClick);
        contentPane.livenessTipsBgBtn.onClick.AddListener(onLivenessTipsBgBtnClick);
        contentPane.vipBtn.ButtonClickTween(onVipBtnClick);
        contentPane.vipTipsBgBtn.onClick.AddListener(onVipBgBtnClick);

        //悬赏
        contentPane.renownBtn.ButtonClickTween(onRenownBtnClick);
        contentPane.toRenownBtn.ButtonClickTween(onRenownBtnClick);
        contentPane.unionTaskCancelBtn.ButtonClickTween(onUnionTaskCancelBtnClick);
        contentPane.unionTaskAwardBtn.ButtonClickTween(onUnionTaskAwardBtnClick);
        contentPane.unionTaskGemBtn.ButtonClickTween(onUnionTaskGemBtnClick);

    }


    bool posInit = false;
    List<Vector3> posList;

    protected override void onShown()
    {

        if (!posInit)
        {

            posList = new List<Vector3>();

            for (int i = 0; i < contentPane.taskItems.Count; i++)
            {
                posList.Add((contentPane.taskItems[i].transform as RectTransform).anchoredPosition3D);
            }

            posInit = true;
        }


        RefreshRedPoint();
        if (!GameSettingManager.inst.needShowUIAnim)
            contentPane.bgBtn.interactable = true;
        _curActivePoint = UserDataProxy.inst.task_activePoint;

        contentPane.tipsTx.text = contentPane.tipsTx.text.Replace("\\n", "\n");

        contentPane.toggleGroup.OnEnableMethod(_index);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_show");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            if (contentPane != null)
            {
                contentPane.bgBtn.interactable = true;
            }
        });
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }


    public void RefreshRedPoint()
    {
        contentPane.daliyRedPoint.SetActive(UserDataProxy.inst.task_dailyNeedShowRedPoint);
        contentPane.unionTaskRedPoint.SetActive(UserDataProxy.inst.task_unionTaskNeedShowRedPoint);

        //每日任务的红点 点开关掉
        var dateTime = TimeUtils.getDateTimeBySecs(GameTimer.inst.serverNow);
        PlayerPrefs.SetInt(AccountDataProxy.inst.account + "_DailyTaskRedPoint_" + dateTime.Year.ToString() + dateTime.Month.ToString() + dateTime.Day.ToString(), 1);
    }

    private void setState(int index)
    {
        switch (index)
        {
            case 0: dailyTaskState(); break;
            case 1: unionTaskState(); break;
        }
    }

    void dailyTaskState()
    {

    }


    #region 日常任务
    private void onLivenessInfoBtnClick()
    {
        contentPane.livenessTipsObj.gameObject.SetActiveTrue();
    }

    private void onLivenessTipsBgBtnClick()
    {
        contentPane.livenessTipsObj.gameObject.SetActiveFalse();
    }

    private void onVipBtnClick()
    {
        contentPane.vipTipsObj.gameObject.SetActiveTrue();
    }

    private void onVipBgBtnClick()
    {
        contentPane.vipTipsObj.gameObject.SetActiveFalse();
    }

    public void ShowAllTaskItemsByAnim(int taskId)
    {
        var animItem = contentPane.taskItems.Find(t => t.data.taskId == taskId);

        if (animItem == null)
        {
            Logger.error("界面缓存没有这个任务 taskId :" + taskId);
            return;
        }

        int index = contentPane.taskItems.IndexOf(animItem);

        Vector3 oriPos = animItem.rewardImg.iconImage.transform.position;
        int changeNum = UserDataProxy.inst.task_activePoint - _curActivePoint;


        if (changeNum <= 0)
        {
            //Logger.error("UserDataProxy.inst.task_activePoint :" + UserDataProxy.inst.task_activePoint + "     当前界面缓存活跃度:" + _curActivePoint);
            ShowAllTaskItems();
            return;
        }

        UserDataProxy.inst.task_isUiAnimShow = true;

        taskVfxTimer = GameTimer.inst.AddTimer(0.05f, changeNum, () =>
          {
              if (animItem == null || this == null || contentPane == null) return;
              var activeVfx = GameObject.Instantiate<FlyVfxCtrl>(contentPane.activeVfxPfb, contentPane.dailyTaskVfxParent);
              vfxCtrls.Add(activeVfx);

              activeVfx.SetSprite(animItem.rewardImg.iconImage.sprite, scale: 0.7f);
              activeVfx.SetTarget(oriPos, contentPane.activeTargetTf.position, 1f, refreshActiveSliderAnim);
          });

        animItem.Exit().SetDelay(0.2f).OnComplete(() =>
        {
            if (animItem == null) 
            {
                UserDataProxy.inst.task_isUiAnimShow = false;
                return;
            }
            for (int i = index + 1; i < contentPane.taskItems.Count; i++)
            {
                contentPane.taskItems[i].transform.DOLocalMove(getTaskItemEndPos(i - 1), 0.2f);
            }

            contentPane.taskItems.Remove(animItem);
            contentPane.taskItems.Add(animItem);
            animItem.transform.localPosition = getTaskItemEndPos(taskNumMax);

            refreshTaskDatas();

            UserDataProxy.inst.task_isUiAnimShow = false;
        });
    }

    public void ShowAllTaskItems()
    {
        if ((K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip)
        {
            contentPane.vipBtn.gameObject.SetActive(false);
        }
        else
        {
            contentPane.vipBtn.gameObject.SetActive(true);
        }
        refreshTaskDatas();

        refreshActiveSlider();
        refreshActiveTaskDatas();
    }

    //任务
    void refreshTaskDatas()
    {
        RefreshRedPoint();

        List<TaskData> datas = UserDataProxy.inst.taskList;

        for (int i = 0; i < contentPane.taskItems.Count; i++)
        {
            if (i < datas.Count)
            {
                contentPane.taskItems[i].ShowItemContent(datas[i]);
            }
            else
            {
                contentPane.taskItems[i].ClearData();
            }
        }

        if (datas.Count < taskNumMax)
        {
            contentPane.taskItems[datas.Count].ShowTaskCooling();
        }
    }

    int _curActivePoint;
    void refreshActiveSlider()
    {
        _curActivePoint = UserDataProxy.inst.task_activePoint;
        contentPane.curActiveNumTx.text = UserDataProxy.inst.task_activePoint.ToString();
        contentPane.activeSlider.value = (float)UserDataProxy.inst.task_activePoint / UserDataProxy.inst.task_activePointEnd;
    }

    void refreshActiveSliderAnim()
    {
        _curActivePoint++;
        _curActivePoint = Mathf.Min(_curActivePoint, UserDataProxy.inst.task_activePoint);
        contentPane.curActiveNumTx.text = _curActivePoint.ToString();
        contentPane.activeSlider.DOValue((float)_curActivePoint / UserDataProxy.inst.task_activePointEnd, 0.1f);

        if (_curActivePoint == UserDataProxy.inst.task_activePoint)
        {
            refreshActiveTaskDatas();
        }
    }

    //活跃宝箱
    void refreshActiveTaskDatas()
    {
        float sliderWidth = (contentPane.activeSlider.transform as RectTransform).rect.width - 60f;

        List<ActiveRewardBoxData> datas = UserDataProxy.inst.activeBoxList;

        foreach (var item in contentPane.activeTaskItems)
        {
            item.Clear();
        }

        for (int i = 0; i < datas.Count; i++)
        {
            if (i < contentPane.activeTaskItems.Count)
            {
                contentPane.activeTaskItems[i].SetData(datas[i], sliderWidth);
            }
            else
            {
                GameObject obj = GameObject.Instantiate(contentPane.activeBoxItemPfb.gameObject, contentPane.activeSlider.transform);
                DailyTaskLivenessItem item = obj.GetComponent<DailyTaskLivenessItem>();
                item.SetData(datas[i], sliderWidth);
                contentPane.activeTaskItems.Add(item);
            }
        }
    }

    Vector3 getTaskItemEndPos(int index)
    {
        return posList[index];//new Vector3((contentPane.taskItems[0].transform as RectTransform).anchoredPosition.x, -94 + (-176 * index), 0);
    }


    private void onSelectedIndexValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        _index = index;

        foreach (var item in contentPane.toggleLinkObjs)
        {
            item.SetActive(false);
        }

        contentPane.toggleLinkObjs[index].SetActive(true);

        setState(index);

    }

    #endregion

    #region 联盟悬赏

    public void RefreshUnionTaskMess()
    {
        RefreshRedPoint();
        contentPane.toggleGroup.OnEnableMethod(1);
    }

    int unionTaskCountDownTimer;

    void unionTaskState()
    {

        if (unionTaskCountDownTimer != 0)
        {
            GameTimer.inst.RemoveTimer(unionTaskCountDownTimer);
            unionTaskCountDownTimer = 0;
        }

        var selfUnionTask = UserDataProxy.inst.selfUnionTask;
        if (selfUnionTask != null)
        {
            contentPane.haveObj.SetActive(true);
            contentPane.notHaveObj.SetActive(false);

            contentPane.headIcon.SetSprite("portrait_atlas", selfUnionTask.npcConfig.icon);
            contentPane.npcNameTx.text = LanguageManager.inst.GetValueByKey(selfUnionTask.npcConfig.name);
            contentPane.talkTx.text = LanguageManager.inst.GetValueByKey(selfUnionTask.npcConfig.union_task_desc);

            for (int i = 0; i < contentPane.stars.Count; i++)
            {
                if (i < selfUnionTask.data.point)
                {
                    contentPane.stars[i].SetSprite("__common_1", "lianmeng_shengwang");
                }
                else
                {
                    contentPane.stars[i].SetSprite("union_atlas", "lianmeng_shengwang2");
                }
            }

            contentPane.unionTaskTargetIcon.SetSprite(selfUnionTask.atlas, selfUnionTask.icon);

            contentPane.unionAwardTaskNameTx.text = contentPane.unionTaskNameTx.text = LanguageManager.inst.GetValueByKey(selfUnionTask.config.desc, selfUnionTask.data.limit.ToString(), selfUnionTask.StrParam_2);

            contentPane.unionTaskSlider.value = (float)selfUnionTask.data.process / selfUnionTask.data.limit;
            contentPane.unionTaskSliderTx.text = selfUnionTask.data.process + "/" + selfUnionTask.data.limit;

            contentPane.selfAwardIcon.SetSprite(selfUnionTask.selfAwardConfig.atlas, selfUnionTask.selfAwardConfig.icon);
            contentPane.selfAwardNumTx.text = "X" + selfUnionTask.config.reward_number;

            contentPane.unionTaskCountDownTx.text = TimeUtils.timeSpanStrip(selfUnionTask.remainTime);
            unionTaskCountDownTimer = GameTimer.inst.AddTimer(1, selfUnionTask.data.endTime, () =>
            {
                contentPane.unionTaskCountDownTx.text = TimeUtils.timeSpanStrip(selfUnionTask.remainTime);
            });

            int costGem = (int)WorldParConfigManager.inst.GetConfig(selfUnionTask.config.difficulty + 2024 - 1).parameters;
            contentPane.unoinTaskGemTx.text = costGem.ToString();
            //contentPane.unoinTaskGemTx.color = costGem > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FF2828") : GUIHelper.GetColorByColorHex("FFFFFF");

            contentPane.unionDoingStateObj.SetActive(selfUnionTask.data.state != (int)EUnionTaskState.CanReward);
            contentPane.unionTaskAwardBtn.gameObject.SetActive(selfUnionTask.data.state == (int)EUnionTaskState.CanReward);
            contentPane.unionTaskGemBtn.gameObject.SetActive(selfUnionTask.data.state != (int)EUnionTaskState.CanReward);

        }
        else
        {
            contentPane.haveObj.SetActive(false);
            contentPane.notHaveObj.SetActive(true);
        }
    }

    private void onRenownBtnClick()
    {
        if (UserDataProxy.inst.playerData.hasUnion)
        {
            int unionTaskUnlockLv = (int)WorldParConfigManager.inst.GetConfig(2030).parameters;
            if (UserDataProxy.inst.playerData.level < unionTaskUnlockLv)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", unionTaskUnlockLv.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                return;
            }

            HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionTaskUIView");
        }
        else
        {
            if (UserDataProxy.inst.playerData.level >= WorldParConfigManager.inst.GetConfig(132).parameters)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("请先加入一个联盟"), GUIHelper.GetColorByColorHex("FFFFFF"));
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主升到{0}级加入联盟解锁悬赏任务", WorldParConfigManager.inst.GetConfig(132).parameters.ToString()), GUIHelper.GetColorByColorHex("FFFFFF"));
            }

        }
    }

    private void onUnionTaskCancelBtnClick()
    {
        if (UserDataProxy.inst.unionTaskCancelCoolTime > 0)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您可以在{0}后再次放弃。", TimeUtils.timeSpanStrip(UserDataProxy.inst.unionTaskCancelCoolTime)), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else
        {
            AudioManager.inst.PlaySound(127);
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_CANCELUNIONTASK, UserDataProxy.inst.selfUnionTask.data.taskUid);
        }
    }


    private void onUnionTaskAwardBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_REWARDUNIONTASK, UserDataProxy.inst.selfUnionTask.data.taskUid);
    }

    private void onUnionTaskGemBtnClick()
    {
        int costGem = (int)WorldParConfigManager.inst.GetConfig(UserDataProxy.inst.selfUnionTask.config.difficulty + 2022).parameters;

        if (costGem > UserDataProxy.inst.playerData.gem)
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, costGem - UserDataProxy.inst.playerData.gem);
        }
        else
        {
            if (contentPane.unionConfirmImg.enabled)
            {
                contentPane.unionConfirmImg.enabled = false;
                EventController.inst.TriggerEvent<int>(GameEventType.UnionEvent.UNION_REQUEST_GEMREWARDUNIONTASK, UserDataProxy.inst.selfUnionTask.data.taskUid);
            }
            else
            {
                contentPane.unionConfirmImg.enabled = true;
            }
        }
    }

    #endregion


    protected override void onHide()
    {

        if (taskVfxTimer != 0)
        {
            GameTimer.inst.RemoveTimer(taskVfxTimer);
            taskVfxTimer = 0;
        }

        if (vfxCtrls.Count > 0)
        {
            for (int i = 0; i < vfxCtrls.Count; i++)
            {
                var vfxCtrl = vfxCtrls[i];
                if (vfxCtrl != null)
                {
                    vfxCtrl.ClearEndHandler();
                }
            }
        }
        vfxCtrls.Clear();

        if (unionTaskCountDownTimer != 0)
        {
            GameTimer.inst.RemoveTimer(unionTaskCountDownTimer);
            unionTaskCountDownTimer = 0;
        }

        for (int i = 0; i < contentPane.taskItems.Count; i++)
        {
            contentPane.taskItems[i].ClearData();
        }

        posInit = false;
        UserDataProxy.inst.task_isUiAnimShow = false;
    }

}