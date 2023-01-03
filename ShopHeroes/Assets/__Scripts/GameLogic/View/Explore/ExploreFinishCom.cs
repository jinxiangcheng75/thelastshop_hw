using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreFinishCom : MonoBehaviour
{
    public Text exploreNameAndDiffText;
    public Text finishResultText;
    public Image separatorImage;
    public List<ExploreFinishHeroItemView> heroes;
    public GUIIcon finishBgIcon;

    [Header("Explore Success")]
    public GameObject successObj;
    public GameObject exploreObj;
    public GameObject awardsObj;
    public GUIIcon exploreIcon;
    public Text exploreLevelText;
    public Text exploreNameText;
    public Slider exploreExpSlider;
    public GUIIcon nextIcon;
    public Image nextUpImg;
    public List<ExploreAwardItemView> awards;
    public Button collectBtn;
    public Button collectAllBtn;

    [Header("Explore Lose")]
    public GameObject loseObj;
    public Button continueBtn;

    [Header("Introduce")]
    public GameObject introduceObj;
    public Text introduceDesText;
    public Text introduecNumText;

    public GameObject allBtn;
    public GameObject exploreMaxObj;

    public RectTransform fillMaskRect;
}
