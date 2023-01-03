using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class AcheivementComp : MonoBehaviour
{
    public Button closeBtn;
    public GUIIcon roadIcon;
    public Text roadNameText;
    public Slider roadSlider;
    public Slider realSlider;
    public Text roadScheduleText;
    public GameObject leftObj;
    public GameObject rightObj;
    public DynamicScrollView scrollView;
    public RectTransform contentRect;
    public ScrollRect scrollRect;
    public AcheivementAwardItem awardItemPrefab;
    public List<GameObject> effects;
}
