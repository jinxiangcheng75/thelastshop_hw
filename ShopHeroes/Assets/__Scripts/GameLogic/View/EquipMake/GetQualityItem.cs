using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetQualityItem : MonoBehaviour
{
    public Transform qualityFx;
    public GUIIcon itemicon;
    public GUIIcon qualityIcon;
    public Text priceText;
    public Text itemNameText;
    public Button okBtn;
    public Button upBtn;
    public Text nextQualitytext;
    public Text needGemText;
    public GameObject costObj;
    public GameObject freeObj;

    public Text levelText;
    public ObjList canWearHeroProfessionIcons;
    public Text tx_canWearFloorLv;

    public Button canWearHeroBtn;

    public List<RoleEquipItem> allProperty;
    public GridLayoutGroup layoutGroup;

    public Animator uiAnimator;
    public RectTransform okBtnAnimTf;

}
