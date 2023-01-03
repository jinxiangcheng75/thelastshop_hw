using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionSetSettingUIComp : MonoBehaviour
{
    public Button closeBtn;
    public Text nameTx;
    public InputField nameField;
    public Button enterLeftBtn;
    public Text enterTx;
    public Button enterRightBtn;
    public Button lvLeftBtn;
    public InputField lowestLvField;
    public Button lvRightBtn;
    public InputField investField;
    public Button cancelBtn;
    public Button confirmBtn;

    [Header("动画")]
    public Animator uiAnimator;
}
