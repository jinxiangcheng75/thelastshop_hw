using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class RolePanelComp : MonoBehaviour
{
    public ToggleGroupMarget topToggleGroup;
    public DynamicScrollView scrollView;
    public ScrollRect scrollRect;
    public Button closeBtn;
    public Text heroNumText;
    public Text selectHeroNumText;
    public Text workerNumText;
    public Text selectWorkerNumText;

    [Header("RedPoint")]
    public GameObject worker_redPointObj;
    public GameObject hero_redPointObj;

    [Header("动画")]
    public Animator uiAnimator;

}
