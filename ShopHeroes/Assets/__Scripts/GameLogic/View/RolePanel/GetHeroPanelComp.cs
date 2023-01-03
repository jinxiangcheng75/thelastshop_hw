using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetHeroPanelComp : MonoBehaviour
{
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public Transform roleTrans;
    public GUIIcon qualityIcon;
    public Text nickNameTx;
    public List<GUIIcon> allStars;
    public Text intelligenceText;
    public Text fightValTx;
    public Text lifeText;
    public Text attackText;
    public Text armorText;
    public Text speedText;
    public Button talentBtn;
    public GUIIcon talentIcon;
    public Transform allEquips;
    public List<RoleSkillItemUI> allSkills;
    public Button okBtn;
    public GameObject qualityFx;
    public Canvas qualityCanvas;
    public Animator anim;
}
