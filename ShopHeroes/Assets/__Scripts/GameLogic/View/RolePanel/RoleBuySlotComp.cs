using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleBuySlotComp : MonoBehaviour
{
    public Button closeBtn;
    public Text levelText;
    public Text goldText;
    public Text gemText;
    public Button goldBtn;
    public Button gemBtn;
    public GameObject sureAgainObj;
    public Text tipsText;

    public GameObject notArriveLv;
    public Text arriveLv;

    [Header("动画")]
    public Animator uiAnimator;

}
