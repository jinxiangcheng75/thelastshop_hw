using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipInfoUIComp : MonoBehaviour
{
    public Button closeBtn;
    public Button leftBtn;
    public Button rightBtn;
    public GUIIcon equipIcon;
    public GUIIcon equipTypeIcon;
    public Text equipSubTypeText;
    public Text equiptyName;
    public Text equiptyLevel;
    //public Text priceText;      //价格
    public Text shuomingTx;

    public ToggleGroupMarget toggleGroup;
    public List<GameObject> toggleLinkObjs;
    public Toggle favoriteBtn;

    [Header("信息面板")]
    public RectTransform infoPlane;
    public List<needRes> needResList;
    public List<needWorker> needWorkerList;
    public GUIIcon gjHeadIcon;

    // ---- 信息小面板
    public Button briefInfoBtn;
    public GameObject briefInfoObj;
    public Text goldTx;
    public List<RoleEquipItem> equipPropertyItems;
    public Button briefInfoBgBtn;
    public Text briefStoreTx;

    public ObjList canWearHeroProfessionIcons;
    public Text tx_canWearFloorLv;

    public Button canWearHeroBtn;


    [Header("熟练度面板")]
    public RectTransform progressPlane;
    public EquipProgress[] equipInfoPlaneList; //按顺序
    public Toggle[] jiantou;  //按顺序

    public Text infoCoolTimeTx;
    public Text progressCoolTimeTx;
    public Button infoMakeButton;
    public Button infoUnlockButton;
    public Text infoUnlockCountText;
    public Button progressMakeButton;
    public Text tip_needLearn;

    [Header("升星面板")]
    public EquipStarUpProgress[] equipStarUpProgresses;
    public Toggle[] starUpToggles;
    public GUIIcon[] starIcons;
    public GUIIcon useItemIcon;
    public Button btn_starUp;
    public Text tx_needStarUpItemNum;
    public Text tip_starUpOver;

    [Header("anim")]
    public Animator topAnimator;
    public Animator windowAnimator;

    public Graphic mask;

}
