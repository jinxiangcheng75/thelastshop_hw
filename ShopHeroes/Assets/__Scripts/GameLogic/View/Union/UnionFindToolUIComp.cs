using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionFindToolUIComp : MonoBehaviour
{

    public Button closeBtn;
    public ToggleGroupMarget toggleGroup;
    public Button createBtn;
    public Button exitBtn;

    public GameObject[] toggleLinkObjs;

    [Header("Hot")]
    public GameObject hot_nothingTipObj;
    public UnionHotItem[] unionHotItems;
    public Text hot_loadingTips;

    [Header("Union")]
    public InputField union_IF;
    public DynamicScrollView union_superList;
    public Text union_nothingTips;

    [Header("Player")]
    public InputField player_IF;
    public DynamicScrollView player_superList;
    public Text player_nothingTips;


}
