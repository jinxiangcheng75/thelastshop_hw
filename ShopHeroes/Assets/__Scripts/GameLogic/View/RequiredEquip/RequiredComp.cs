using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RequiredComp : MonoBehaviour
{
    public Button closeBtn;
    public Button makeBtn;
    public Button marketBuyBtn;
    public GUIIcon equipIcon;
    public Text equipNameTx;
    public Text numberTx;
    public Text titleTx;

    [Header("动画")]
    public Animator uiAnimator;

}
