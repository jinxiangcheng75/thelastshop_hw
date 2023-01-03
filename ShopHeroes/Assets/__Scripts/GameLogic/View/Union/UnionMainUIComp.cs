using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionMainUIComp : MonoBehaviour
{
    public Text unionLvTx;
    public Text unionNameTx;
    public Text unionMemberNumTx;
    public Text worthTx;
    public GameObject aidRedPointObj;
    public Text aidRedPointNumTx;

    public List<UnionBuffItem> unionBuffItems;

    [Header("Tips详情")]
    public Button tipMaskBtn;
    public GameObject selfUnionCoinTipObj;
    public GameObject unionCoinTipObj;
    [HideInInspector]
    public GameObject curOpenTipObj;

    [Header("联盟buff详情")]
    public Button unionBuffDetailBgBtn;
    public Text unionBuffNameTx;
    public Text unionBuffDesTx;
    public Text unionBuffCountdownTx;
    public Slider unoinBuffSlider;
    public RectTransform unionBuffFillBgTf;
    public RectTransform unionBuffDetailTf;
    public RectTransform unionBuffDetailBgTf;


    [Header("联盟币")]
    public Button selfUnionTokenBtn;
    public Text selfUnionTokenTx;
    public Button unionCoinBtn;
    public Text unionCoinTx;

    [Header("下方按钮")]
    public Button cityBtn;
    public Button findBtn;
    public Button aidBtn;
    public Button taskBtn;
    public Button detailBtn;
    public Button wealBtn;

}
