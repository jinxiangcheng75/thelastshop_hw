using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleTransferItem : MonoBehaviour
{
    public Text contentText;
    public Transform roleTrans;
    public Button confirmBtn;
    public Text lifeText;
    public Text atkText;
    public Text defText;
    public Text spdText;
    public Text accText;
    public Text dodgeText;
    public Text criText;
    public Text toughText;
    public List<Transform> allProperty;
    public Text titleText;
    public Text titleText2;
    public List<GUIIcon> allStar;

    public GUIIcon qualityIcon;
    public Text nickNameTx;
    public Text fightTx;
    public Text newLifeTx;
    public Text newAtkTx;
    public Text newDefTx;

    public RectTransform horizontalRect;
    //左上角星星 新技能 装备解锁
    public GUIIcon newUnlockSkillIcon;
    public List<GUIIcon> allEquipTypeIcon;
    public GameObject hasUnlockEquipType;
    public GameObject notHasUnlockEquipType;

    public Image lifeUpImg;
    public Image atkUpImg;
    public Image defUpImg;
}
