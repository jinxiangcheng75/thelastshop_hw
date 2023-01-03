using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeNameComp : MonoBehaviour
{

    [Header("-更改玩家名字-")]
    public Button closeBtn;
    public InputField nameInput;
    public Button changeBtn;
    public Text applyText;
    public Text timeText;
    public Image greenImg;
    public Image gemObj;
    public GameObject changePanel;
    public Text titleText;

    [Header("-打开面板动画-")]
    public Animator uiAnimator;
}
