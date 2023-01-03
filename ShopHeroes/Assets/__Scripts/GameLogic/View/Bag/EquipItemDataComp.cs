using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipItemDataComp : MonoBehaviour
{
    public Button closeBtn;
    public Button leftBtn;
    public Button rightBtn;
    public Image img_drawing;
    public Button drawingBtn;
    public GUIIcon equipIcon;
    public GUIIcon equipIconBg;
    public Text equipLevelTx;
    public GUIIcon equipSubTypeIcon;
    public Text equipNameTx;
    public Text equipSubTypeTx;
    public Text priceText;      //价格
    public Text desTx;


    public GUIIcon qualityIcon;
    public Text qualityText;
    public Button removeBtn;
    public Toggle lockBtn;
    public Text inBoxNumberTx;
    public Text inShelfNumberTx;

    public ObjList canWearHeroProfessionIcons;
    public Text tx_canWearFloorLv;
    public Button canWearHeroBtn;

    public List<RoleEquipItem> allProperty;
    public ContentSizeFitter contentSize;

    [Header("面板")]
    public RectTransform data_plane;

    [Header("动画")]
    public Animator topAnimator;
    public Animator windowAnimator;
    public Graphic[] btns;
    public Graphic mask;



}
