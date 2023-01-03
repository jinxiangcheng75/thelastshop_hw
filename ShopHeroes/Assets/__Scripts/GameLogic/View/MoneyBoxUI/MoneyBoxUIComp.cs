using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MoneyBoxUIComp : MonoBehaviour
{
    public Button closeBtn;
    public List<ProgressStateTip> stateTips;
    public Text goldText;
    public Text progressText;
    public Text coolTimeText;
    public Image progressMask;
    public Button okBtn;
    public Slider slider;
    public Transform windowTF;
    public Transform inProgressNode;
    public Transform nextStateTimeNode;
    public GUIIcon bgPigIcon;
    public GUIIcon maskPigIcon;
    public GUIIcon grayPigIcon;
    public GUIIcon rewardIcon;
    public GameObject effectObj;
    public Animator rewardAnim;
}
