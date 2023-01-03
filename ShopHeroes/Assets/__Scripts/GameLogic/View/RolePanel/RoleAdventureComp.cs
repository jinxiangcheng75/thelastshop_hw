using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleAdventureComp : MonoBehaviour
{
    public Button closeBtn;
    public GUIIcon adventureIcon;
    public GUIIcon difficultyIcon;
    public List<RoleAdventureItemUI> heroes;
    public Text difficultyText;
    public GUIIcon awardIcon;
    public Text dungeonText;
    public Slider dungeonSlider;
    public Text residueText;
    public GameObject finishTextObj;
    public GameObject sureAgainObj;
    public Button gemBtn;
    public Text gemNumText;
    public Button finishBtn;
    public GameObject notBossObj;
    public GameObject costObj;
    public GameObject freeObj;

    [Header("动画")]
    public Animator uiAnimator;

}
