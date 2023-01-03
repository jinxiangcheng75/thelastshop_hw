using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuyMakingSlotComp : MonoBehaviour
{
    public Button closeBtn;
    public Text currSlotCountTx;
    public Text nextSlotCountTx;
    public Text needBuyLvTx;
    public Text needGlodNumberTx;
    public Text needGemNumberTx;
    public Button glod_BuyBtn;
    public Button gem_BuyBtn;
    public GameObject gemAffirmObj;
    public Button openMakeListBtn;

    public GameObject obj_countNotMax;
    public GameObject obj_countMax;
    public Text tx_countMax;
    public GameObject obj_buttonNotMax;
    public GameObject obj_TipsMax;


    [Header("动画")]
    public Animator uiAnimator;

}
