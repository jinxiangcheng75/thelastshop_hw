using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureBoxCompleteCom : MonoBehaviour
{
    public GUIIcon boxIcon;
    public List<BoxRewardItem> allReward;
    public Button collectBtn;
    public GameObject completeObj;
    public GUIIcon popupObj;
    public Image tipsObj;
    public Text tipsText;
    public Text popupNameText;
    public RectTransform popupRect;
    public Canvas effectObj;
}
