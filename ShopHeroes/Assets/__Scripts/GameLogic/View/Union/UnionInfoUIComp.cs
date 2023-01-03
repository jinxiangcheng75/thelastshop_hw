using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class UnionInfoUIComp : MonoBehaviour
{

    public Button closeBtn;
    public Text unionNameTx;
    public Text unionUidTx;
    public Text unionLvTx;
    public Text memberNumTx;


    public ToggleGroupMarget toggleGroup;
    public GameObject[] toggleLinkObjs;


    [Header("公会摘要")]
    public Text visitOpennessTips;
    public Text unionPresidentTips;
    public Text lowestLvTips;
    public Text enterInvestTips;
    public Text netWorthTips;
    public Button joinUnionBtn; //非自己公会
    public Button setUnionBtn;  //自己公会 设置按钮
    public Button findUnionBtn;
    public Button exitUnionBtn;


    [Header("成员列表")]
    public DynamicScrollView member_superList;

}
