using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityBuildingInvestUIComp : MonoBehaviour
{

    public Canvas maskCanvas;
    public RectTransform BottomTf;

    [Header("Top")]
    public Button leftBtn;
    public Button rightBtn;
    public GUIIcon buildingIcon;

    public GameObject scienceTop;
    public Button science_leftBtn;
    public Button science_rightBtn;
    public Text science_roleNameTx;
    public Text science_roleTalkTx;
    public RectTransform science_roleTf;

    public GameObject investTop;


    [Header("Middle - Top")]
    public Text lvTx;
    public Text buildingDesTx;
    public Text buildingNameTip;
    public Text buildingTypeTx;
    public Button closeBtn;
    public ToggleGroupMarget toggleGroup;
    public Text investToggleTip;
    public Text investToggleBgTip;


    [Header("Middle - Bottom")]
    public GameObject[] toggleLinks;

    [Header("LinkObj - Invest")]
    public Text investDes;
    public GUIIcon itemIcon;
    public GameObject workerObj;
    public GUIIcon workerIcon;
    public GameObject workerLvBgObj;
    public Text workerLvTx;

    public Text oldValueTip;
    public Text oldValueTx;
    public Text newValueTx;
    public Text onlyNewValueTx;

    public Text invest_levelUpDes;
    public Button delBtn;
    public RectTransform curSliderFill;
    public RectTransform fillTween;
    public Slider invest_slider;
    public Text invest_sliderTip;
    public Text invest_sliderChgTx;
    public Button addBtn;
    public Text addSelfUnionTokenTx;
    public Button goldBtn;
    public Text goldTip;
    public Text goldNumTx;
    public Button gemBtn;
    public Text gemTip;
    public Text gemNumTx;
    public GameObject gemAffirmObj;
  

    [Header("LinkObj - Rank")]
    public GameObject hasUnionObj;
    public DynamicScrollView superList;
    public Text rank_levelUpDes;

    public GameObject notHasUnionObj;


}
