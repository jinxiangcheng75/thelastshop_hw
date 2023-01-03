using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgBoxPlayerUpItemUIView : ViewBase<MsgBox_playerUpItemComp>
{
    public override string viewID => ViewPrefabName.MsgBoxPlayerUpItemUI;
    public override string sortingLayerName => "popup";

    private List<LevelUpShowItem> _items;

    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(CloseBtnOnClick);
        _items = new List<LevelUpShowItem>();

        //生成小Item 最多4只
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = GameObject.Instantiate(contentPane.resItem.gameObject, contentPane.content, false);

            LevelUpShowItem item = obj.GetComponent<LevelUpShowItem>();

            _items.Add(item);
            item.Clear();
        }

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });

    }

    public void setMsgText(PlayerUpItemData data)
    {
        contentPane.top_tip_text.text = LanguageManager.inst.GetValueByKey(StaticConstants.PlayerUpMsgBoxTip_Top[data.mainType]);
        contentPane.bottom_tip_text.text = LanguageManager.inst.GetValueByKey(StaticConstants.PlayerUpMsgBoxTip_Bottom[data.mainType]);
        RefreshItems(data);

        switch (data.mainType)
        {
            //金币解锁
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 8:
                contentPane.bottom_moneyIcon.SetActiveTrue();
                contentPane.bottom_moneyTips.gameObject.SetActiveTrue();
                contentPane.bottom_unlockTips.gameObject.SetActiveTrue();
                break;
            default:
                contentPane.bottom_moneyIcon.SetActiveFalse();
                contentPane.bottom_moneyTips.gameObject.SetActiveFalse();
                contentPane.bottom_unlockTips.gameObject.SetActiveFalse();
                break;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.bottomTf);
    }

    private void CloseBtnOnClick()
    {
        hide();
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_PLAYERUPUI);
    }

    void RefreshItems(PlayerUpItemData data)
    {

        //勇士   大长格子
        if (data.mainType == 6)
        {
            contentPane.longItem.gameObject.SetActive(true);
            contentPane.content.gameObject.SetActive(false);
            //contentPane.longItemIcon.SetSprite();
        }
        else
        {
            contentPane.longItem.gameObject.SetActive(false);
            contentPane.content.gameObject.SetActive(true);


            if (data.mainType != 9 && data.mainType != 10)
            {
                data.subtypes = new int[] { data.mainValue };
            }

            for (int i = 0; i < _items.Count; i++)
            {

                if (i < data.subtypes.Length)
                {
                    _items[i].SetData(data.mainType, data.subtypes[i], data.mainValue);
                }
                else
                {
                    _items[i].Clear();
                }

            }

        }

    }


}
