using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionCreateUIComp : MonoBehaviour
{
    public Button closeBtn;
    public InputField nameField;
    public Button goldBtn;
    public Button gemBtn;
    public Text goldTx;
    public Text gemTx;
    public GameObject gemAffirmObj;
    public Text gemTip;
    public Button enterLeftBtn;
    public Text enterTx;
    public Button enterRightBtn;
    public Button lvLeftBtn;
    public InputField lowestLvField;
    public Button lvRightBtn;
    public InputField investField;



    private void Start()
    {
        nameField.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
        investField.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
    }

}
