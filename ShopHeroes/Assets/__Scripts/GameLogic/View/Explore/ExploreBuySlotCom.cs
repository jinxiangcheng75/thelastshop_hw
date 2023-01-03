using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreBuySlotCom : MonoBehaviour
{
    public Button closeBtn;
    public Text curSlotNumText;
    public Text nextSlotNumText;
    public Text levelText;
    public Text goldText;
    public Text gemText;
    public Button goldBtn;
    public Button gemBtn;
    public Button toExploreBtn;
    public GameObject sureAgainObj;

    public GameObject notArriveLv;
    public Text arriveLv;

    [Header("动画")]
    public Animator uiAnimator;

}
