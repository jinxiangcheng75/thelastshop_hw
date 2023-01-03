using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopkeeperPanelComp : MonoBehaviour
{
    [Header("-店主位置-")]
    public Transform roleTrans;
    public Canvas modelCanvas;

    [Header("-初始界面-")]
    public Button exteriorBtn;
    public Button fashionBtn;
    public Button finishBtn;
    public Button cancleBtn;
    public Button applyBtn;
    public GameObject changeBtnList;

    [Header("-批量购买-")]
    public GameObject batchPanel;
    public GameObject batchSelectObj;
    public Button batchCloseBtn;
    public Transform batchContent;
    public CustomBuyItemComp batchItem;
    public Button batchAllBtn;
    public Button batchApplyBtn;
    public Text batchPriceText;
    public GameObject batchGemIcon;
    public GameObject batchGlodIcon;

    [Header("动画")]
    public Animator windowAnimator;
    public Animator batchBuyAnimator;
}
