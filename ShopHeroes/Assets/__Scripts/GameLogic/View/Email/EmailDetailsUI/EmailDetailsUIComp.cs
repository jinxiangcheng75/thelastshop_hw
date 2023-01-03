using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailDetailsUIComp : MonoBehaviour
{
    public Button closeBtn;

    public Text titleTx;
    public Text recipientTx;
    public ScrollRect scrollViewRect;
    public Text contentTx;
    public Text addresserTx;

    [Header("附件")]
    public GameObject AccessoryObj;
    public GameObject fourObj;
    public List<AccessorieItem> fourAccessorieItems;
    public DynamicScrollView superList;

    public Button readBtn;
    public Text readBtnTx;
    public Button delBtn;


}
