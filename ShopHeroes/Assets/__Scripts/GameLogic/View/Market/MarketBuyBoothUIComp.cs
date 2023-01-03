using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketBuyBoothUIComp : MonoBehaviour
{
    public Button closeBtn;
    public Button goldBtn;
    public Button gemBtn;
    public GameObject gemAffirmObj;
    public Text lvTx;
    public Text goldTx;
    public Text gemTx;

    public GameObject notArriveLv;
    public Text arriveLv;

    [Header("动画")]
    public Animator uiAnimator;
}
