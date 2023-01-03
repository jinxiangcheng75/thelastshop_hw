using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox_TaskComp : MonoBehaviour
{
    public Text infoNameTxt;
    public Text msgTxt;

    public Button closeBtn;
    public Button refresh_OK_Btn;
    public Button refresh_CANCEL_Btn;
    public Button cd_OK_Btn;

    public GameObject refreshStateObj;
    public GameObject cdStateObj;

    public Animator uiAnimator;
}
