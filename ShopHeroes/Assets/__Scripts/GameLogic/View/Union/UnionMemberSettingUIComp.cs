using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionMemberSettingUIComp : MonoBehaviour
{
    public Button closeBtn;
    public Button leftBtn;
    public Button rightBtn;
    public Text jobTx;
    public GUIIcon jobIcon;
    public Button kickoutBtn;
    public Button confirmBtn;

    [Header("动画")]
    public Animator uiAnimator;
}
