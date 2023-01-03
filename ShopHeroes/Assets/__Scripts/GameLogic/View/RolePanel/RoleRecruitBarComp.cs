using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleRecruitBarComp : MonoBehaviour
{
    public Button closeBtn;
    public Button btn_probabilityPublic;//概率公示
    public List<RecruitItemUI> allHeroes;
    public Text timeText;
    public Text diamondText;
    public Button refreshBtn;
    public Button freeRefreshBtn;
    public Image refreshGemObj;
    public Image refreshGoldObj;
    public GameObject windowObj;
    public GameObject sureAgainObj;
    public Text probebilityText;
    public Slider probebilitySlider;
    public Text probebilitySliderTx;
    public GameObject redPoint;
    public RectTransform bgRect;

    public Toggle Toggle_skipAnim;
}