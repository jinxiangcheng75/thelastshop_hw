using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketTradingHallUIComp : MonoBehaviour
{
    public ToggleGroupMarget bigToggleGroup;
    public ToggleGroupMarget subToggleGroup;
    public DynamicScrollView superList;
    public Button closeBtn;
    public Toggle allTypeToggle;
    public GUIIcon titleTypeIcon;
    public GUIIcon topBgIcon;
    public Text title_1;
    public Text title_2;
    public Text tx_nothingTip;

    [Header("筛选 -- 等阶")]
    public Button levelScreenBtn;
    public Button levelScreenObjBtn;
    public GameObject levelScreenObj;
    public Toggle[] levelScreenToggles;
    public Button levelScreenApplyBtn;
    public Button levelScreenCancelBtn;
    public Toggle allLevelToggle;

    [Header("筛选 -- 品质")]
    public Button qualityScreenBtn;
    public Button qualityScreenObjBtn;
    public GameObject qualityScreenObj;
    public Toggle[] qualityScreenToggles;
    public Button qualityScreenApplyBtn;
    public Button qualityScreenCancelBtn;
    public Toggle allQualityToggle;

    [Header("排序")]
    public Button sortBtn;
    public Text sortText;

    [Header("刷新")]
    public GameObject refreshMask;
    public GameObject superListContent;

    [Header("动画")]
    public Animator uiAnimator;
}
