using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItemInfoComp : MonoBehaviour
{

    public Text titleText;
    public Text levelText;
    public Text goldLowPriceText;
    public Text inventoryNumText;
    public Text marketNumText;
    public Text goldTip;
    public Text goldPriceText;
    public Text gemTip;
    public Text gemPriceText;
    public Text itemNameText;
    public Text exchangeTipOrRequestNameText;
    public Image marketIcon;
    public GUIIcon itemBgIcon;
    public GUIIcon itemIcon;
    public GUIIcon subTypeIcon;
    public GUIIcon equipQuality;
    public Button infoBtn;
    public Button offerBtn;
    public Button closeBtn;
    public Button maskBtn;
    public GameObject goldAndGemObj;
    public GameObject obj_superEquipSign;

    [Header("金币按钮")]
    public Button goldBtn;
    public GameObject goldCanClickObj;
    public GameObject goldProcessingObj;
    public Text goldProcessingTip;


    [Header("钻石按钮")]
    public Button gemBtn;
    public GameObject gemCanClickObj;
    public GameObject gemAffirmObj;
    public GameObject gemProcessingObj;
    public Text gemProcessingTip;


}
