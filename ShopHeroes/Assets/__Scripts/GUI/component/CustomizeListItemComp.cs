using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CustomizeListItemComp : MonoBehaviour, Mosframe.IDynamicScrollViewItem
{
    public GUIIcon img_bg;
    public GUIIcon img_titleBg;
    public Image img_palette;
    public Image img_bottomBg;

    public GUIIcon img_iconBg;
    public GUIIcon img_icon;
    public Image img_level;
    public Text txt_level;
    public Image img_num;
    public Text txt_num;
    public Text txt_title;
    public GUIIcon img_subTypeIcon;
    public GUIIcon img_mark;
    public Text txt_mark;
    public GUIIcon img_cost;
    public Text txt_cost;
    public Text txt_owned;
    public Text txt_storeNum;
    public Text txt_sellOut;
    public Text txt_type;
    public GUIIcon[] img_contentTypeList;
    public Transform VarietyIconsTf;
    public Transform unlock;
    public Slider slider_unlock;
    public Text text_unlock;
    public Text txt_unlockSlider;
    public Text txt_lockVipText;
    public Image vipImg;
    //礼包
    public GUIIcon giftBgIcon;
    public GUIIcon giftIcon;

    public GameObject buffObj;
    public List<Text> buffTextList;

    [Header("动画")]
    public Animator btnAnimator;

    public int index;
    public void onUpdateItem(int _index)
    {
        index = _index;
    }

    bool isShowAnimed;

    public void reSetData()
    {
        isShowAnimed = false;
    }

    public void showAnim(int index)
    {
        if (isShowAnimed) return;
        isShowAnimed = true;

        gameObject.SetActive(false);

        GameTimer.inst.AddTimer(0.28f + 0.02f * index, 1, () =>
        {
            gameObject.SetActive(true);
            btnAnimator.CrossFade("show", 0f);
            btnAnimator.Update(0f);
            btnAnimator.Play("show");
        });

    }

}
