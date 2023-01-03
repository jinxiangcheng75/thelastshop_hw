using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务系统    #陆泓屹
/// </summary>

public class TaskPanel : MonoBehaviour
{
    public Button closeBtn;
    public Button bgBtn;
    public Button vipBtn;

    public ToggleGroupMarget toggleGroup;
    public GameObject[] toggleLinkObjs;

    [Header("日常任务")]

    public GameObject taskContent;

    public Button activeInfoBtn;
    public Slider activeSlider;

    public Text curActiveNumTx;
    public DailyTaskLivenessItem activeBoxItemPfb;

    public List<DailyTaskItem> taskItems;
    public List<DailyTaskLivenessItem> activeTaskItems;

    [Header("任务提示小面板")]
    public GameObject livenessTipsObj;
    public Button livenessTipsBgBtn;
    public GameObject vipTipsObj;
    public Button vipTipsBgBtn;

    [Header("悬赏任务")]
    public GameObject notHaveObj;
    public Button renownBtn;
    public GameObject haveObj;
    public GUIIcon headIcon;
    public Text npcNameTx;
    public Text talkTx;
    public Button toRenownBtn;
    public List<GUIIcon> stars;
    public GUIIcon unionTaskTargetIcon;
    public Text unionTaskNameTx;
    public Slider unionTaskSlider;
    public Text unionTaskSliderTx;
    public Text unionTaskCountDownTx;
    public GameObject unionTaskCountDownObj;
    public Button unionTaskCancelBtn;
    public GUIIcon selfAwardIcon;
    public Text selfAwardNumTx;
    public Button unionTaskGemBtn;
    public Text unoinTaskGemTx;
    public Text unionTaskAwardTx;
    public GameObject unionDoingStateObj;
    public Button unionTaskAwardBtn;
    public Text unionAwardTaskNameTx;

    public Image unionConfirmImg;


    [Header("红点")]
    public GameObject daliyRedPoint;
    public GameObject unionTaskRedPoint;


    [Header("动画")]
    public Animator uiAnimator;
    public Transform activeTargetTf;
    public FlyVfxCtrl activeVfxPfb;
    public Transform dailyTaskVfxParent;

    [Header("翻译")]
    public Text tipsTx;

}
