using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleEquipComp : MonoBehaviour
{
    public Button bgBtn;
    public Button closeBtn;
    public Text fightingText;
    public GUIIcon equipIcon;
    public GUIIcon qualityBgIcon;
    public Text nameText;
    public Text classText;
    public GUIIcon qualityIcon;
    public Text qualityText;
    public Text damageText;
    public Transform propertyContent;
    public Button lockBtn;
    public Button cancleBtn;
    public Button changeBtn;
    public GameObject newObj;
    public GameObject propertyItem;
    public GameObject lockImageObj;
    public List<RoleEquipItem> allProperty;
    public ContentSizeFitter contentRect;
    public Button leftBtn;
    public Button rightBtn;
    public GameObject superNormalObj;
    public GUIIcon ItemIconBg;
}
