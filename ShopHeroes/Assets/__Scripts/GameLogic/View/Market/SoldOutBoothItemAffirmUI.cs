using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldOutBoothItemAffirmUI : ViewBase<SoldOutBoothItemAffirmUIComp>
{

    public override string viewID => ViewPrefabName.SoldOutBoothItemAffirmUI;

    public override string sortingLayerName => "popup";

    BoothItem _item;

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;


        contentPane.cancelBtn.ButtonClickTween(onCancelBtnClick);
        contentPane.affirmBtn.ButtonClickTween(onAffirmBtnClick);
        contentPane.closeBtn.ButtonClickTween(hide);
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

    public void SetInfo(BoothItem boothItem)
    {
        _item = boothItem;

        contentPane.msgTip.text = LanguageManager.inst.GetValueByKey("您确定要取消交易吗？") + LanguageManager.inst.GetValueByKey((boothItem.marketType == 0 ? "（您的物品会被退回）" : "（您的新币或金条会被退回）"));
    }

    private void onCancelBtnClick()
    {
        hide();
    }

    private void onAffirmBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_BOOTHITEMSOLDOUT, _item.boothField);
    }

    public void NeedHide(BoothItem item)
    {
        if (item.boothField == _item.boothField) hide();
    }

}
