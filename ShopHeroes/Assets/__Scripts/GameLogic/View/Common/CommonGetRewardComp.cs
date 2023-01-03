using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonGetRewardComp : MonoBehaviour
{
    public Button confirmBtn;
    public List<CommonRewardItem> allItems;
    public GridLayoutGroup gridLayout;

    [Header("动画")]
    public Transform tipsBgTf;
    public Animator uiAnimator;

    public CommonRewardItem flyToBagRewardItem;
    public GameObject flyToBagAnimObj;

}
