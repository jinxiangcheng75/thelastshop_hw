using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ChatMsgUICom : MonoBehaviour
{
    public Button closeBtn;
    public InputField inputField;
    public Button sendBtn;
    public Mosframe.DynamicVScrollView scrollView;
    public ToggleGroup tableGroup;
    public Toggle worldToggle;
    public Toggle SysTogle;
    public Toggle UnionTogle;
    public DOTweenAnimation showAnimation;
    public Transform noneMsgText;
    public GameObject raceLampObj;
    public RectTransform raceLampTextRect;
    public Text raceLampText;
    public GameObject worldRedPoint;
    public GameObject unionRedPoint;
    public GameObject sysRedPoint;
}
