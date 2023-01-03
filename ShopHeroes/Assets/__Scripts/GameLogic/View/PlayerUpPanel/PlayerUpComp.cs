using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpComp : MonoBehaviour
{

    public Text lvTipText;
    public Text lvText;
    public Button continueBtn;

    public Transform shopKeeperTf;
    public Transform content;

    public PlayerUpItem resItem;


    [Header("动画")]
    public Animator uiAnimator;
    public RectTransform topBg;

    [Header("特效")]
    public Canvas vfxCanvns;
}
