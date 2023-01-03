using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LackResUIComp : MonoBehaviour
{
    public GUIIcon icon;
    public Text titleNumTx;
    public Text currInventoryTx;
    public Slider slider;
    public LackResSlider sliderCtrl;

    public Text lackResTips_1;

    public Button leftBtn;
    public Button rightBtn;
    public Button makeButton;
    public Button marketButton;
    public Button putResBoxButton;
    public Button closeButton;
    public Button ectypalButton;
    public GUIIcon ectypalBtnIcon;
    public Text ectypalBtnText;


    [Header("公会币购买资源包")]
    public Text remainCountTx;
    public Button unionBuyButton;
    public Text unionReplenishCountTx;
    public Text unionBuyCoinTx;
    public Button gemBuyButton;
    public Text needGemTx;
    public Text replenishCountTx;
    public GameObject gemConfirmObj;
    public GameObject gemCanClickObj;
    public GameObject gemProcessingObj;
    public GameObject unionBuyCountdownObj;
    public GameObject unionCanClickObj;
    public GameObject unionProcessingObj;
    public Text unionBuyCountdownTx;


    [Header("动画")]
    public Animator uiAnimator;
}
