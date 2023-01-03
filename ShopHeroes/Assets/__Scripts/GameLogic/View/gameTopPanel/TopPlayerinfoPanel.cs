using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TopPlayerinfoPanel : MonoBehaviour
{
    [Header("-资源-")]
    public Text playerLv;
    public Slider lvbar;

    public Text addExpText;
    public Button gameSettingBtn;
    public GameObject settingRedPoint;
    public Text energyText;
    public Text selfUnionTokenTx;
    public Text unionCoinTx;
    public Text activity_workerGame_coinTx;
    public Text glodText;
    public Text lotteryItemText;
    public Text gemText;
    public Text luxuryText;
    public Button addGemBtn;
    public Slider energySlider;

    [Header("-店主换装-")]
    public Button shopkeeperBtn;
    public Image huangguanImg;
    public Image vipLvBg;

    public Transform flyActivity_workerGameCoinTf;
    public Transform flyUnionCoinTF;
    public Transform flyEnergyTF;
    public Transform flyGoldTF;
    public Transform flyGemTF;
    public RectTransform activity_workerGameCoinTf;
    public RectTransform unionCoinTargetTf;
    public RectTransform energyTargetTf;
    public RectTransform goldTargetTf;

    [Header("储蓄罐提示")]
    public MoneyBoxTipcom moneyBoxTip;

    [Header("小贴士按钮")]
    public Button energyTipBtn;
    public Button selfUnionTokenBtn;//自己公会币
    public Button unionCoinBtn;//公会积分
    public Button activity_workerGame_coinBtn;
    public Image img_activity_workerGame_coinAdd;
    public Button goldTipBtn;
    public Button tipMaskBtn;
    public GameObject lotteryObj;
    public GameObject luxuryObj;


    public Text goldTipTx;

    [Header("家具数量限制")]
    public Slider mapCountLimit;
    public Text mapCountLimitText;

    public GameObject selfUnionCoinTipObj;
    public GameObject unionCoinTipObj;
    public GameObject energyTipObj;
    public GameObject goldTipObj;
    [HideInInspector]
    public GameObject curOpenTipObj;

    [Header("动画")]
    public RectTransform animTf;

    //物品栏数量
    public TopPlayerAniItem inventoryAni;


    //工匠经验
    public RectTransform expAniTf;
    public TopPlayerAniItem expAniItemPfb;

    public TopPlayerAniItem taskAni;
    public TopSevenDayAnim sevenAnim;
    public TopSevenDayAnim unionTaskAnim;
    public TopSevenDayAnim dailyTaskAnim;
}
