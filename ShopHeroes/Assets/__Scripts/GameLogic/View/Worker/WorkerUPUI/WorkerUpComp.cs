using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUpComp : MonoBehaviour
{

    public Text lvTipText;
    public Text professionTx;
    public Text lvText;

    public Button continueBtn;

    public Transform workerTf;
    public GUIIcon professionIcon;
    public Text lastSpeedTx;
    public Text curSpeedTx;


    [Header("UI动画")]
    public Animator uiAnimator;
    public RectTransform topBg;


}
