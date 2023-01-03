using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 扩建系统    #陆泓屹
/// </summary>

public class ExtensionPanelComp : MonoBehaviour
{
    [Header("-店铺空间Item文本组件-")]
    public Text area_OldCount_Txt;
    public Text area_NewCount_Txt;

    [Header("-家具空间Item文本组件-")]
    public Text space_OldCount_Txt;
    public Text space_NewCount_Txt;

    [Header("-升级所需时间Item文本组件-")]
    public Text upgrade_NeedTime_Txt;

    [Header("-金币扩建按钮的文本组件-")]
    public Text btn_NeedLevel_Txt;
    public Text btn_NeedCoin_Txt;

    public GameObject notArriveLv;
    public Text arriveLv;

    [Header("-按钮上需要多少钻石的文本组件-")]
    public Text btn_NeedDiam_Txt1;

    [Header("-按钮组件-")]
    public Button upgradeBtn;
    public Button upgradeImmediatelyBtn;
    public GameObject confirmUpgradeObj;
    public Button closeBtn;

    [Header("-新旧块图片-")]
    public Image oldImg;
    public Image newImg;

    [Header("-旧的图片块预制件-")]
    public GameObject oldPfb;

    [Header("-占地空间显示组件-")]
    public GameObject content;

    [Header("-扩建按钮显影组件-")]
    public GameObject canUpgradeObj;

    [Header("动画")]
    public Animator uiAnimator;

}
