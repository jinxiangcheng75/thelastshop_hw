using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnLockDrawingComp : MonoBehaviour
{
    public Button closeBtn;
    public GUIIcon equipIcon;
    public Text equipNameTx;
    public GUIIcon equipSubTypeIcon;
    public Text currDrawingCountTx;
    public Text needDrawingCountTx;
    public Text remainDrawindCountTx;
    public Text needDrawingCountBtnTx;
    public GameObject lackOfDrawCountObj;
    public Button unLockBtn;

    [Header("动画")]
    public Animator uiAnimator;
}
