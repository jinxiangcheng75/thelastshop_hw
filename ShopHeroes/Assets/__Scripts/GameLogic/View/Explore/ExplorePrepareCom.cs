using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorePrepareCom : MonoBehaviour
{
    public GUIIcon icon;
    public Button closeBtn;
    public Text nameText;
    public Text levelText;
    public Slider scheduleSlider;
    //public Text scheduleText;
    public GUIIcon nextIcon;
    public Button nextBtn;
    //public Text teamFightingText;
    public Text suggestFightingText;
    //public ToggleGroupMarget group;
    public Text exploreTimeText;
    public Text numText;
    public Button leftBtn;
    public Button rightBtn;
    public GUIIcon diffIcon;
    public Text diffText;
    public List<Image> points;
    public Button useItemBtn;
    public Button infoBtn;
    public GUIIcon useItemIcon;
    public Button exploreBtn;
    public Text exploreBtnText;
    public GameObject nextObj;
    public GameObject nextUpObj;
    public GameObject bgObj;
    //public GameObject talkObj;
    public List<ExplorePrepareAwardItem> allItem;
    public List<ExploreHeroItemView> allHeroes;
    public GUIIcon rewardIcon;
    public Text rewardText;
    public GridLayoutGroup gridLayoutGroup;
    public GameObject useItemNumObj;
    public Text useItemNumText;
    public GameObject useObj;
    public GameObject notUseObj;
    public GameObject bossObj;
    public GUIIcon bossIcon;
    public Text bossText;
    public Text limitText;
    public Canvas heroCanvas;

    public Button showTalentObj;
    public GUIIcon showTalentIcon;
    public Button showItemObj;
    public GUIIcon showItemIcon;
    public Button talentTipObj;
    public Text talentNameText;
    public Text talentDescText;

    [Header("动画")]
    public Animator windowAnim;
}
