using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShelfContentUIComp : MonoBehaviour {
    public Image img_mask;
    public GUIIcon img_title;
    public Button btn_close;
    public Button btn_left;
    public Button btn_right;
    public RectTransform signTf;
    public ShelfContentCtrlComp ctrl;
    public GameObject leftRightBtnObj;

    public RectTransform shelfTf;
}
