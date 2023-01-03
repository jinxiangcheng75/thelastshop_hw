using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBuyProductionUIComp : MonoBehaviour
{
    public Button closeButton;
    public GUIIcon icon;
    public Text currInventoryTx;
    public Slider slider;
    public LackResSlider sliderCtrl;

    public Button gemBuyButton;
    public GameObject gemConfimObj;
    public Text replenishCountTx;
    public Text needGemTx;

    public Text remainTx;
    public Button unionCoinButton;
    public Text unoin_replenishCountTx;
    public Text needUnionCoinTx;

    public GameObject unionBuyCountdownObj;
    public Text unionBuyCountdownTx;

    [Header("动画")]
    public Animator uiAnimator;
}
