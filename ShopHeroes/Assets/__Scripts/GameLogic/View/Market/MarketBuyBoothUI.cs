using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//市场 扩建摊位
public class MarketBuyBoothUI : ViewBase<MarketBuyBoothUIComp>
{

    public override string viewID => ViewPrefabName.MarketBuyBoothUI;
    public override string sortingLayerName => "window";

    private BoothDataConfig cfg;

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;


        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.goldBtn.ButtonClickTween(() => buyBoothField(1));
        contentPane.gemBtn.ButtonClickTween(() =>
        {
            if (contentPane.gemAffirmObj.activeSelf)
                buyBoothField(2);
            else
                contentPane.gemAffirmObj.SetActive(true);
        }
       );

    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
        contentPane.gemAffirmObj.SetActive(false);
    }

    protected override void onHide()
    {
        base.onHide();
        AudioManager.inst.PlaySound(11);
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

    public void Init(int boothNum)
    {
        cfg = MarketBoothConfigManger.inst.GetConfig(boothNum + 1);

        contentPane.goldTx.text = cfg.coin_demand.ToString("N0");
        contentPane.gemTx.text = cfg.diamond_demand.ToString("N0");
        contentPane.lvTx.text = cfg.level_demand.ToString();

        contentPane.goldTx.color = cfg.coin_demand > UserDataProxy.inst.playerData.gold ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white;
        //contentPane.gemTx.color = cfg.diamond_demand > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white;
        contentPane.lvTx.color = cfg.level_demand > UserDataProxy.inst.playerData.level ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white;

        contentPane.notArriveLv.SetActive(cfg.level_demand > UserDataProxy.inst.playerData.level);
        contentPane.arriveLv.enabled = cfg.level_demand <= UserDataProxy.inst.playerData.level;
    }

    private void buyBoothField(int costType)
    {

        if (costType == 1)
        {
            if (cfg.level_demand > UserDataProxy.inst.playerData.level)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            else if (cfg.coin_demand > UserDataProxy.inst.playerData.gold)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不够"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else if (costType == 2 && cfg.diamond_demand > UserDataProxy.inst.playerData.gem)
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("FF2828"));
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, cfg.diamond_demand - UserDataProxy.inst.playerData.gem);
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETBOOTH_BUYFIELD, costType);
    }

}
