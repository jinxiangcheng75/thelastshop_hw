using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class CreatRoleComp : MonoBehaviour
{
    [Header("-窗口UI-")]
    public Button exteriorButton;
    public GUIIcon sexIcon;
    public Button randomButton;
    public Button startGameButton;
    public Button maleButton;
    public Button femaleButton;
    public GameObject maleSelect;
    public GameObject femaleSelect;
    public SkeletonDataAsset shopkeeperManAss;
    public SkeletonDataAsset shopkeeperWomanAss;
    public Transform roleTrans;
    public Transform moveToTrans;
    public ToggleGroupMarget subTypeGroup;
    public DynamicVScrollView scrollView;


    [Header("-外观-")]
    public GameObject pupopObj;
    public Text titleText;
    public Button closeButton;
    public RectTransform typeContent;
    public GridLayoutGroup typeGroup;
    public RoleClothingItemComp item;
    public RoleSubItemComp subItem;

    [Header("-类型二-")]
    public Transform itemContent;

    [Header("-输入姓名-")]
    public GameObject creatNameObj;
    public InputField nameInput;
    public Text promptText;
    public Button nameCloseBtn;
    public Button nameCloseButton;
    public Button nameEnterButton;
    public Button nameRandomBtn;

    [Header("动画")]
    public Animator windowAnimator;
    public Animator proAnimator;
    public Animator creatNameAnimator;
}
