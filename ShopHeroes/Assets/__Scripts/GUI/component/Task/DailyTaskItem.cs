using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务系统  #陆泓屹
/// </summary>

public class DailyTaskItem : MonoBehaviour
{
    //当前Item的数据
    public TaskData data;
    //当前的下个冷却任务的冷却时间
    private LoopEventcomp timerComp;

    [Header("-按钮组件-")]
    public Button getAwardBtn;
    public Button refreshBtn;

    [Header("-文本组件-")]
    public Text diamondCountTxt;
    public Text unfinished_progressTxt;
    public Text finished_progressTxt;
    public Text unFinishedTaskNameTxt;
    public Text finishedTaskNameTxt;
    public Text remainingCoolingTxt;

    [Header("-动态替换的图片组件-")]
    public GUIIcon goodsImg;
    public GUIIcon rewardImg;
    public GUIIcon finishGoodsIcon;

    [Header("-任务进度条-")]
    public Slider unfinished_progressBar;
    public Slider finished_progressBar;

    [Header("-Item状态组件-")]
    public GameObject unfinishedComp;
    public GameObject finishedComp;
    public GameObject countDownComp;

    [Header("-完成状态动画-")]
    public RectTransform tipsRt;


    private void Start()
    {
        getAwardBtn.ButtonClickTween(OnGetAwardBtnClick);
        refreshBtn.ButtonClickTween(OnRefreshBtnClick);
    }

    //刷新按钮回调
    private void OnRefreshBtnClick()
    {


        EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOXCOM_TASK, data.taskId);
    }

    //请求此次完成获得宝石之后的任务列表
    private void OnGetAwardBtnClick()
    {
        if (UserDataProxy.inst.task_isUiAnimShow)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您点的太快了，稍微等一下"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else
        {

            EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REWARD_DATALIST, data.taskId);
        }
    }


    //给Item的组件赋值
    public void ShowItemContent(TaskData data)
    {
        gameObject.SetActive(true);

        this.data = data;

        rewardImg.SetSprite(ItemconfigManager.inst.GetConfig(data.rewardId).atlas, ItemconfigManager.inst.GetConfig(data.rewardId).icon);
        unFinishedTaskNameTxt.text = finishedTaskNameTxt.text = UserDataProxy.inst.GetTaskName(data);
        diamondCountTxt.text = "+" + data.reward_number.ToString("N0");

        GUIHelper.SetUIGray(refreshBtn.transform, UserDataProxy.inst.task_refreshNumber == 0);

        switch (data.taskState)
        {
            case (int)EDailyTaskState.Doing:
                {
                    finishedComp.gameObject.SetActive(false);
                    countDownComp.gameObject.SetActive(false);

                    unfinishedComp.gameObject.SetActive(true);
                    //DOTween.Kill(tipsRt);

                    goodsImg.SetSprite(data.atlas, data.icon);
                    unfinished_progressBar.minValue = 0;
                    unfinished_progressBar.maxValue = data.parameter_2;
                    unfinished_progressBar.value = data.parameter_1;
                    unfinished_progressTxt.text = $"{data.parameter_1}" + "/" + $"{data.parameter_2}";

                    break;
                }

            case (int)EDailyTaskState.Reached:
                {
                    unfinishedComp.gameObject.SetActive(false);
                    countDownComp.gameObject.SetActive(false);

                    finishedComp.gameObject.SetActive(true);

                    finished_progressBar.minValue = 0;
                    finished_progressBar.maxValue = data.parameter_2;
                    finished_progressBar.value = data.parameter_1;
                    finished_progressTxt.text = $"{data.parameter_1}" + "/" + $"{data.parameter_2}";

                    //tipsRt.DOScale(1, 0.5f).From(1.1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);

                    finishGoodsIcon.SetSprite(data.atlas, data.icon, GUIHelper.NOTNEEDCLEARMAT);

                    break;
                }
        }
    }


    void clearTimer()
    {
        if (timerComp != null)
        {
            GameTimer.inst.removeLoopTimer(timerComp);
            timerComp = null;
        }
    }

    public void ClearData()
    {
        data = null;
        clearTimer();
        this.gameObject.SetActive(false);
    }

    public void ShowTaskCooling()
    {
        ClearData();
        gameObject.SetActive(true);

        finishedComp.gameObject.SetActive(false);
        unfinishedComp.gameObject.SetActive(false);

        countDownComp.gameObject.SetActive(true);

        remainingCoolingTxt.text = TimeUtils.timeSpanStrip(UserDataProxy.inst.task_nextTime);

        timerComp = GameTimer.inst.AddLoopTimerComp(remainingCoolingTxt.gameObject, 1, () =>
        {
            remainingCoolingTxt.text = TimeUtils.timeSpanStrip(UserDataProxy.inst.task_nextTime);
        });

    }

    public TweenerCore<Vector3, Vector3, VectorOptions> Exit()
    {
        return transform.DOLocalMoveX(1600, 0.2f);
    }

}