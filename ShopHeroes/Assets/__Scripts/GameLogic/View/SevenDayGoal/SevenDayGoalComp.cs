using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class SevenDayGoalComp : MonoBehaviour
{
    public Text nameText;
    public Text listDescText;
    public Text descText;
    public GUIIcon rewardIcon;
    public Image tuzhiImg;
    public Image lockImg;
    public Image gouImg;
    public Text rewardNumText;
    public Text promptText;
    //public Text canReward;
    public GameObject effectObj;
    public Button rewardTrans;
    public Button rewardBtn;
    public Text rewardBtnText;
    public GUIIcon btnIcon;
    public Transform tipsTrans;
    public ToggleGroupMarget group;
    public DynamicScrollView scrollView;
    public Button closeBtn;
    public ScrollRect toggleRect;
    public List<SevenDayToggleItem> toggleItems;
    public List<SevenDayAwardItem> awardItemList;
}
