using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class ExploreInfoCom : MonoBehaviour
{
    public Button closeBtn;
    public GUIIcon icon;
    public GameObject nextObj;
    public GameObject nextUpObj;
    public Text nameText;
    public Text levelText;
    public Text scheduleText;
    public Slider levelSlider;
    public GUIIcon itemIcon;
    //public GameObject nextObj;
    public DynamicVScrollView scrollView;
    [Header("动画")]
    public Animator windowAnim;
}
