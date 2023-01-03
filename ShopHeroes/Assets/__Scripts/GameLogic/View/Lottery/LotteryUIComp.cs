using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryUIComp : MonoBehaviour
{
    public Text timeText;
    public Text payText;
    public Button refreshBtn;
    public List<AwardItemUI> awards;
    public Button singleBtn;
    public Button tenEvenBtn;
    public Button closeBtn;

    public GameObject singleFree;
    public GameObject singleItem;
    public GameObject singleGem;
    public GameObject tenthItem;
    public GameObject tenthGem;
    public Text singleItemNumText;
    public Text tenthItemNumText;
    public Text singleGemNumText;
    public Text tenthGemNumText;

    public Slider cumulativeSlider;
    public CumulativeItemUI cumulative;
    public Text firstStageText;
    public Button helpBtn;
    public Button recordBtn;
    public GameObject freeRefreshObj;
    public Transform panTrans;
    public Graphic backBg;
    public Graphic refreshText;
    public Animator panAnimator;

    public GUIIcon singleItemIcon;
    public GUIIcon tenthItemIcon;

    public GameObject vipObj;

    public GameObject singleRedPoint;
    public GameObject refreshRedPoint;

    public Image singleConfigImg;
    public Image tenConfigImg;
    public Image refreshConfigImg;
}
