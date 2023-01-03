using UnityEngine;
using UnityEngine.UI;
using Mosframe;
using System.Collections.Generic;

public class equipBagComp : MonoBehaviour
{
    public Button closeBtn;

    public Button slotUpdateBtn;
    //装备分类
    public ToggleGroupMarget mainTypeTF;
    public RectTransform subTypeTF;
    public RectTransform subTypeTF1;
    public EquipSubTypeitem subTypeItem; // 小分类

    public Toggle allTypeToggle;    //总览时间类
    public Toggle collectToggle;    //收藏分类
    public Toggle activity_workerGameToggle;    //巧匠大赛分类
    public Text activity_workerGameNameTx_1;
    public Text activity_workerGameNameTx_2;

    public GameObject collectCheckObj;

    public DynamicScrollView loopListView;
    public ScrollRect scrollRect;

    //UnionBuff
    public List<UnionBuffItem> unionBuffItems;

    //RES
    public Transform resListParent;
    public ResComp[] resList;

    //没有图纸解锁
    public GameObject nullItem;

    [Header("动画")]
    public Animator uiAnimator;
    public GameObject maskObj;
    public GridLayoutGroup gridLayoutGroup;
    public ContentSizeFitter contentSizeFitter;
}
