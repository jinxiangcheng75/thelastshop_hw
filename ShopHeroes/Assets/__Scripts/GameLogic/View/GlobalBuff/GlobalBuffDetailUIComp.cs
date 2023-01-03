using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalBuffDetailUIComp : MonoBehaviour
{
    public Text gameDateTimeTx;
    public Text titleTx;
    public Text effectTx;
    public Text desTx;
    public GUIIcon buffIcon;
    public Text topLeftTx;
    public Text remainTip;
    public Text remainTimeTx;
    public RectTransform clockHand;
    public RectTransform advanceNoticeTf;
    public Text advanceNoticeTx;
    public Button okBtn;
    public Text okBtnTx;


    [Header("动画")]
    public Animator uiAnimator;

    void AA() 
    {
    }
}
