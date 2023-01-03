using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoothCreateListUIComp : MonoBehaviour
{
    public Button closeBtn;
    public Button sellBtn;
    public Button buyBtn;
    public Button unionBtn;
    public Text countDownTx;
    public Image unionImg;

    [Header("动画")]
    public Animator uiAnimator;
}
