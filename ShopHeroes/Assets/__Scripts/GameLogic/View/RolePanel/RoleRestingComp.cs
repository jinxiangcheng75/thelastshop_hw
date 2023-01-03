using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleRestingComp : MonoBehaviour
{
    public Button closeBtn;
    public GameObject allObj;
    public GameObject singleObj;
    public Button bgBtn;

    [Header("all component")]
    public Transform restingRoleContent;
    public Text itemNumText;
    public Text gemNumText;
    public GameObject sureAgainObj;
    public Button allItemBtn;
    public Button allGemBtn;
    public Text restingCountText;

    [Header("single component")]
    public Text topText;
    public Text singleRestingText;
    public GameObject singleSureAgainObj;
    public Button singleItemBtn;
    public Button singleGemBtn;
    public Text singleItemNumText;
    public Text singleGemNumText;
    public GameObject notFinishObj;
    public GameObject finishObj;
    public Button singleFinishBtn;

    [Header("动画")]
    public Animator uiAnimator;
}
