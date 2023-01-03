using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionMsgUpdateUIComp : MonoBehaviour
{
    public Button closeBtn;

    public Text lvTx;
    public RectTransform headGraphicTf;
    public Text nickNameTx;
    public GUIIcon memberJobIcon;
    public Text unionMsgTx;

    public Button ignoreBtn;
    public Button nextBtn;

    [Header("动画")]
    public Animator uiAnimator;

}
