using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurniturePanelComp : MonoBehaviour
{

    [Header("-文本和某些固定图片组件-")]
    public Text currentLevelTxt;
    public Text objName;
    public Text subtypeNameTx;
    public Text introTxt;

    public Text phase1LevelTxt;
    public Text phase2LevelTxt;
    public Text phase3LevelTxt;

    public Text item1NameTxt;
    public Text item1OldValueTxt;
    public Image item1ArrowImg;
    public Text item1NewValueTxt;

    public Text item2NameTxt;
    public Text item2OldValueTxt;
    public Image item2ArrowImg;
    public Text item2NewValueTxt;

    public Text item3NameTxt;
    public Text item3OldValueTxt;
    public Image item3ArrowImg;
    public Text item3NewValueTxt;

    public Text item4NameTxt;
    public Text item4TimeTxt;

    public Text coinCountTxt;
    public Text flagLevelTxt;

    public Text diamCountTx;

    public Text timerTxt;
    public Text diamCountTxt3;
    public Text diamCountTxt4;

    public Text maxLevelText;

    [Header("-按钮组件-")]
    public Button closeBtn;
    public Button upgradeBtn;
    public GameObject confirmUpgradeObj;
    public Button immediatelyUpgradeBtn;
    public Button completeUpgradeBtn;
    public Button gangongBtn;
    public Button confirmGanGongBtn;

    public GameObject notArriveLv;
    public Text arriveLv;
    //上一个架子的按钮
    public Button leftBtn;
    //下一个架子的按钮
    public Button rightBtn;

    [Header("-切换器组件-")]
    public Toggle upgradePanelTog;
    public Toggle contentPanelTog;

    [Header("动态加载的图片部分")]
    public Canvas maskCanvas;
    public RectTransform entityPos;
    public GUIIcon storageImg;

    public GUIIcon item1Img;
    public GUIIcon item2Img;
    public GUIIcon item3Img;
    public GUIIcon item4Img;
    public Image[] frameImgs;
    public GUIIcon[] phaseImgs;
    public Image[] arrowImgs;

    [Header("显隐部分")]
    public GameObject topBtnsAndLinesObj;
    public GameObject introDataObj;
    public GameObject bottomBtnsObj;
    public GameObject contentVarietyObj;

    public GameObject item1Obj;
    public GameObject item2Obj;
    public GameObject item3Obj;
    public GameObject item4Obj;

    public GameObject sceneStateObj;
    public GameObject turnStateObj;
    public GameObject turnState_UprgadingObj;

    [Header("容纳小格子的容器")]
    public GridLayoutGroup gridContent;

    [Header("小格子的预制件")]
    public GameObject oldPfb;

    [Header("-新旧块图片-")]
    public Image oldImg;
    public Image newImg;

    public Sprite whiteKuang;
    public Sprite greenKuang;
    public Sprite yellowKuang;
    public Sprite greenArrow;
    public Sprite purpleArrow;

    [Header("大内容部分")]
    public GameObject onlyIntroDataObj;
    public GridLayoutGroup only_glGroup;
    public GameObject only_item2Obj;
    public GameObject only_item4Obj;
    public Text only_IntroTxt;
    public GUIIcon only_icon1;
    public GUIIcon only_icon2;
    public GUIIcon only_icon3;
    public GUIIcon only_icon4;
    public Text only_tx1;
    public Text only_tx2;
    public Text only_tx3;
    public Text only_tx4;
    public Text only_oldVal1;
    public Text only_oldVal2;
    public Text only_oldVal3;
    public Text only_newVal1;
    public Text only_newVal2;
    public Text only_newVal3;
    public Text only_timerTxt;



    [Header("货架内容部分")]
    public ShelfContentCtrlComp ctrl;

    [Header("动画")]
    public Animator topAnimator;
    public Animator windowAnimator;

}
