using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureSkinUIComp : MonoBehaviour
{
    [Header("top")]
    public Button closeBtn;
    public GUIIcon titleImg;
    public Text titleTx;

    [Header("middle")]
    public DynamicScrollView superList;

    [Header("bottom")]
    public Button leftBtn;
    public Button rightBtn;
    public Button buyBtn;
    public GUIIcon buyMoneyIcon;
    public Text buyMoneyTx;
    public GameObject ownObj;

}
