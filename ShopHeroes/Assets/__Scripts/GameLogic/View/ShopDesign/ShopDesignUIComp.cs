using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopDesignUIComp : MonoBehaviour
{
    [Header("EditLayer 1")]
    public RectTransform trans_edit;
    public Button btn_done;
    public Button btn_edit_pets;
    public Button btn_customize;
    public Button btn_furniture;
    public Button btn_basement;
    public Button btn_expand;

    [Header("EditLayer 2")]
    public RectTransform trans_op;
    public HorizontalLayoutGroup hlayout_op;
    public Button btn_cancel;
    public Button btn_skin;
    public Button btn_upgrade;
    public Button btn_storage;//for container
    public Button btn_resource;//for resource
    public GUIIcon img_resIcon;
    public Text txt_resName;
    public Text txt_resNum;
    public Button btn_content;//for shelf
    public Slider slider_content;//for shelf
    public Button btn_edit;
    public Button btn_op_pets;//for pet
    public Text txt_op_pets;


    public Button btn_rotate;
    public Button btn_store;
    public Button btn_confirm;
    public GUIIcon img_confirmType;
    public Text txt_confirm;
    public Text txt_affirm;

    public GameObject go_itemTitle;
    public VerticalLayoutGroup vlGroup_itemTitle;
    public Image img_itemTitle;
    public Text txt_itemLevel;
    public Text txt_itemTitle;
    public Text[] txt_itemBuffDeses;


    [Header("Create")]
    public RectTransform create_op;
    public Button btn_apply;
    public Button btn_applyAll;
    public Button btn_back;
    public Button create_btn_rotate;
}