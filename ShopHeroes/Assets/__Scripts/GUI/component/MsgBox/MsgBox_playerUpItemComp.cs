using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox_playerUpItemComp : MonoBehaviour
{

    public Text top_tip_text;
    public Text bottom_tip_text;

    public Button closeBtn;

    public Transform content;
    public Transform longItem;

    public GUIIcon longItemIcon;

    public LevelUpShowItem resItem;

    public RectTransform bottomTf;
    public GameObject bottom_moneyIcon;
    public Text bottom_moneyTips;
    public Text bottom_unlockTips;

    [Header("动画")]
    public Animator uiAnimator;

}
