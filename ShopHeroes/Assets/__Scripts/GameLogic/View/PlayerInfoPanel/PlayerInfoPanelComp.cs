using DG.Tweening;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanelComp : MonoBehaviour
{
    [Header("-角色位置-")]
    public Transform roleTrans;
    public Transform petTrans;

    [Header("-头像-")]
    public Text levelText;
    public Transform headParent;
    public Button headBtn;
    public Button headCircleBtn;
    public Button closeBtn;

    [Header("-名字账号在线状态-")]
    //public RectTransform nameTxBg;
    public Text nameText;
    public Text uidText;
    public GameObject playerStateObj;
    public Text playerStateText;
    public Button changeNameBtn;

    [Header("-公会信息-")]
    public GameObject unionInfoObj;
    public Text unionLvTx;
    public Text unionNameTx;
    public Text unionJobTx;

    [Header("-个人信息-")]
    public Text worthTx;//净价值
    public Text investTx;//投资
    public Text helpTx;//已帮助
    public Text rewardTx;//悬赏
    public Text monsterTx;//已精通

    [Header("-等级信息-")]
    public Slider levelSlider;
    public Text levelSliderText;

    [Header("-人物信息界面动画-")]
    public Animator uiAnimator;

    [Header("-拜访-")]
    public Transform expTf;
    public Transform visitTF;
    public Button VisitBtn;

    [Header("-下方按钮-")]
    public GameObject unionMemberObj;
    public Button setMemberJobBtn;

    [Header("-弹劾会长-")]
    public Button impeachPresidentBtn;
    public GameObject impeachPresidentGemAffirmObj;
    public Text impeachGemCostTx;

    public GameObject selfObj;
    public Button customBtn;

    [Header("上方按钮")]
    public Button vipBtn;
    public Text vipText;
}
