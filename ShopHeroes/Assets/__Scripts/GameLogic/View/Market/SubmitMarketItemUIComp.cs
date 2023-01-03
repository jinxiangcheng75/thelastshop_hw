using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitMarketItemUIComp : MonoBehaviour
{
    public Button closeBtn;
    [Header("上")]
    public GUIIcon Top_iconBg;
    public GUIIcon Top_itemIcon;
    public GUIIcon Top_qualityIcon;
    public GUIIcon Top_arrowIcon;
    public GUIIcon Top_subTypeIcon;
    public GUIIcon topBgIcon;
    public Text Top_lvTx;
    public Text Top_nameTx;
    public Text Top_typeTx;
    public GameObject Top_superEquipSignObj;

    [Header("中")]
    public ToggleGroupMarget toggleGroup;
    public Toggle[] toggles;
    public GameObject[] toggleLinks;
    public GameObject toggleLinkObj;

    [Header("中 --- 数量 ---")]
    public Text Num_toggleTx;
    public Text Num_tip1;
    public Text Num_content1;
    public Text Num_marketNumTx;
    public InputField Num_inputField;
    public Button Num_superDelBtn;
    public Button Num_delBtn;
    public Button Num_addBtn;
    public Button Num_superAddBtn;


    [Header("中 --- 稀有度 ---")]
    public Text Rarity_toggleTx;
    public ToggleGroupMarget Rarity_ToggleGroup;
    public Toggle Rarity_toggle_isSuper;


    [Header("中 --- 价格 ---")]
    public Text Price_toggleTx;
    public GUIIcon Price_toggleIcon;
    public GUIIcon Price_basicIcon;
    public GUIIcon Price_lowestIcon;
    public Text Price_basicsPriceTx;
    public Text Price_marketLowestPriceTx;
    public InputField Price_inputField;
    public Button Price_superDelBtn;
    public Button Price_delBtn;
    public Button Price_addBtn;
    public Button Price_superAddBtn;
    public Button Price_changeMoneyBtn;
    public GUIIcon Price_toggleCheckmask;


    [Header("中 --- 上架时间 ---")]
    public Text Time_taxRateTx;
    public Text Time_tip2;
    public Text Time_content2Tx;
    public InputField Time_inputField;
    public Button Time_delBtn;
    public Button Time_addBtn;
    public GUIIcon Time_moneyIcon;



    [Header("中 -- confirmPanel")]
    public GameObject confirmUIObj;
    public GameObject qualityObj;
    public Text numTx;
    public Text qualityTx;
    public GUIIcon unitMoneyIcon;
    public Text unitPriceTx;
    public Text continueTimeTx;

    [Header("中 -- confirmPanel -- tips")]
    public Button tipsBtn;
    public GameObject tipsObj;
    public Button tipsMaskBtn;
    public Text tips_tip2;
    public Text tips_content1;
    public Text tips_content2;


    [Header("下")]
    public Button backBtn;
    public Button nextBtn;
    public Text rightBtnTx;

}
