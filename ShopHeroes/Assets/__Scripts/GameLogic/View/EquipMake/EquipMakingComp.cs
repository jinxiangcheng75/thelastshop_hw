using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipMakingComp : MonoBehaviour
{
    public Button closeBtn;
    public Button tiliBtn;
    public Button gemBtn;
    public GameObject gemAffirmObj;
    public GUIIcon itemIcon;
    public Text headTx;
    public Text itemNameTx;
    public Slider coolTimeBar;
    public Text coolTimeText;
    public Button lingquBtn;
    public Text needTiliTx;
    public Text needGemTx;
    public Animator uiAnimator;

    private int currSlotId = 0;

}
