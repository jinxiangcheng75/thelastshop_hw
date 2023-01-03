using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleUpgradeItem : MonoBehaviour
{
    public Text contentText;
    public Text levelText;
    public Transform roleTrans;
    public Button confirmBtn;
    public List<Transform> allProperty;

    public GUIIcon qualityIcon;
    public Text nickNameTx;
    public Text newLifeText;
    public Text newAtkText;
    public Text newDefText;
    public Text fightTx;
    public Text canWearEqiupLevelTx;
    public GameObject canWearObj;
    public RectTransform horizontalRect;

    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;

    public Image lifeUpImg;
    public Image atkUpImg;
    public Image defUpImg;
}
