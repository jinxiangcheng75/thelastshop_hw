using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CityUIComp : MonoBehaviour
{
    public Button shopBtn;
    public Button exploreBtn;
    public Text residueText;
    public Transform exploreSlotContent;
    public List<ExploreSlotItem> allSlots;
    public RectTransform[] makeSlotSigns;
    public HorizontalLayoutGroup layoutGroup;
    public ExploreSlotItem makeSlotItemGO;
    public Transform makeSlotListContent;
    public OverrideScrollRect slotSR;
    public RectTransform bottomAniTf;
    [Header("英雄")]
    public Button heroBtn;
    public GameObject heroIdleBg;
    public GameObject workerRedPoint;
    public Text heroIdleCountText;
    public Button marketBtn;
    public GameObject obj_marketRedPoint;
    public Button unionBtn;
    public Button chatBtn;
    public Button tBoxBtn;
    public RectTransform shopBtnImg;
    public RectTransform heroBtnImg;
    public RectTransform marketBtnImg;
    public RectTransform unionBtnImg;
    public RectTransform exploreBtnImg;
    public RectTransform tBoxBtnImg;
    public Text shopBtnTx;
    public Text heroBtnTx;
    public Text marketBtnTx;
    public Text unionBtnTx;
    public Text tBoxBtnTx;
    public DOTweenAnimation leftPlaneAnim;
    public GameObject tBoxRedPoint;
    public GameObject chatRedPoint;
    public RectTransform leftAnimTf;
    public RectTransform rightAnimTf;
    public Button ruinsBtn;
}
