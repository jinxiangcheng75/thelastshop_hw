using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolveEquipUIComp : MonoBehaviour
{

    public Button closeBtn;
    public Button addBtn;
    public Button delBtn;
    public Button cancelBtn;
    public Button resolveBtn;
    public GUIIcon equipIcon;
    public Text titleTx;
    public Text resolveNumTx;
    public Text tipsTx;
    public List<ResolveEquipItem> items;


    [Header("动画")]
    public Animator uiAnimator;

}
