using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoothCreateListUI : ViewBase<BoothCreateListUIComp>
{

    public override string viewID => ViewPrefabName.BoothCreateListUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;


        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.sellBtn.ButtonClickTween(onSellBtnClick);
        contentPane.buyBtn.ButtonClickTween(onBuyBtnClick);
        contentPane.unionBtn.ButtonClickTween(onUniconBtnClick);

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

    protected override void onHide()
    {
        base.onHide();
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETUI);
    }

    private void onSellBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_TOMARKETBYBAGUI, kMarketItemType.selfSell);
    }

    private void onBuyBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_TOMARKETBYBAGUI, kMarketItemType.selfBuy);
    }

    private void onUniconBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_TOMARKETBYBAGUI, kMarketItemType.UnionBuy);
    }


}
