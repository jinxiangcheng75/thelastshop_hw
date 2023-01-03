using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomizeSelectionUIComp : MonoBehaviour {
    public Image img_mask;
    public Text txt_title;
    public Button btn_close;
    public Toggle[] tg_group;
    public ScrollRect sr_itemList;
    public Transform trans_scrollItem;

    [Header("动画")]
    public Animator uiAnimator;
    public GameObject maskObj;
}
