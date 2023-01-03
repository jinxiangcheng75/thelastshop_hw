using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerInfoComp : MonoBehaviour
{

    public Button leftBtn;
    public Button rightBtn;
    public Transform workerTf;


    public GUIIcon headIcon;
    public Text nameText;
    public Text lvText;
    public Slider expSlider;
    public Button closeBtn;
    public Button expInfoBtn;

    public ToggleGroupMarget toggleGroup;
    public GameObject infoUI;
    public GameObject drawingUI;
   

    [Header("--infoUI--")]
    public GUIIcon typeIcon;
    public Text professionText;
    public Text descText;
    public Text curSpeedTx;
    public Text nextSpeedTx;
    public Button investBtn;
    public Text investBtnTx;
    public Text expTx;

    [Header("--drawingUI--")]
    public DynamicScrollView superList;

    [Header("--expTips--")]
    public GameObject expTipsObj;
    public Button expTipsCloseBtn;
    public Text expTips_expTx;

}
