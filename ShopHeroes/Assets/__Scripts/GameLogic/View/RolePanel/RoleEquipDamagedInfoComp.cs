using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleEquipDamagedInfoComp : MonoBehaviour
{
    public List<GUIIcon> allObjList;
    public Button infoBtn;
    public Button leftBtn;
    public Button rightBtn;
    public GUIIcon typeIcon;
    public Text nameText;
    public GUIIcon icon;
    public GUIIcon qualityIcon;
    public Button discardBtn;
    public Button repairByGem;
    public Button repairByItem;
    public Button repairByMoney;
    public Text gemText;
    public Text itemText;
    public Text moneyText;
    public RoleEquipDamagedIntroduce introduce;
    public GameObject qualityFx;
    public Transform huangguanTrans;
    public GameObject superNormalObj;
}
