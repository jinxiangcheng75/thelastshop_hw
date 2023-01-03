using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleTransferComp : MonoBehaviour
{
    public Canvas maskCanvas;
    public Button closeBtn;
    public Transform roleTrans;
    public ToggleGroupMarget group;
    public GameObject singleObj;
    public GUIIcon singleTypeIcon;
    public Text singleText;
    public Text desText;
    public Text curLifeText;
    public Text curAttText;
    public Text curArmorText;
    public Text curSpeedText;
    public Text curDodgeText;
    public Text curAccText;
    public Text transferLifeText;
    public Text transferAttText;
    public Text transferArmorText;
    public Text transferSpeedText;
    public Text transferDodgeText;
    public Text transferAccText;
    public Button transferBtn;
    public GameObject tipObj;
    public Text transferText;
    public GUIIcon skillIcon;
    public Text skillDescText;
    public List<HeroTransferNeedMatItem> heroTransferNeedMatItems;
    public List<GUIIcon> hpGrewList;
    public List<GUIIcon> atkGrewList;
    public List<GUIIcon> defGrewList;
    public List<GUIIcon> unlockNewEquipType;
}
