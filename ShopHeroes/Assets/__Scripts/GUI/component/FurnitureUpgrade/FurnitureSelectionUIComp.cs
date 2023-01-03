using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FurnitureSelectionUIComp : MonoBehaviour {
    public Image img_mask;
    public Button btn_close;
    public Text txt_title;
    public Toggle[] tg_catagoryList;
    public ScrollRect sr_itemlist;

    public Transform trans_scrollItem;

    public Animator uiAnimator;
    public GameObject maskObj;
}
