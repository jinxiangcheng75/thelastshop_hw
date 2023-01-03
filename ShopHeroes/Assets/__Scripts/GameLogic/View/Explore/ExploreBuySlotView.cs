using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreBuySlotView : ViewBase<ExploreBuySlotCom>
{
    public override string viewID => ViewPrefabName.BuyExploreSlotUI;
    public override string sortingLayerName => "window";
    private FieldCostConfig cfg;
    private int exploreSlotType = 4;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;
        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.goldBtn.onClick.AddListener(() => buyRoleSlotField(0));
        contentPane.gemBtn.onClick.AddListener(() =>
        {
            if (contentPane.sureAgainObj.activeSelf)
                buyRoleSlotField(1);
            else
                contentPane.sureAgainObj.SetActive(true);
        });
        contentPane.toExploreBtn.ButtonClickTween(() =>
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_SHOWUI);
        });
    }

    public void Init(int slotNum)
    {
        cfg = FieldConfigManager.inst.GetFieldConfig(exploreSlotType, slotNum + 1);

        contentPane.curSlotNumText.text = slotNum.ToString();
        contentPane.nextSlotNumText.text = (slotNum + 1).ToString();
        contentPane.goldText.text = cfg.money.ToString();
        contentPane.gemText.text = cfg.diamond.ToString();
        contentPane.levelText.text = cfg.level.ToString();

        contentPane.goldText.color = cfg.money > UserDataProxy.inst.playerData.gold ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");
        //contentPane.gemText.color = cfg.diamond > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");
        contentPane.levelText.color = cfg.level > UserDataProxy.inst.playerData.level ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");

        contentPane.notArriveLv.SetActive(UserDataProxy.inst.playerData.level < cfg.level);
        contentPane.arriveLv.enabled = UserDataProxy.inst.playerData.level >= cfg.level;
    }

    private void buyRoleSlotField(int costType)
    {
        if (costType == 0)
        {
            if (cfg.level > UserDataProxy.inst.playerData.level)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            else if (cfg.money > UserDataProxy.inst.playerData.gold)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不够"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else if (costType == 1 && cfg.diamond > UserDataProxy.inst.playerData.gem)
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("FF2828"));
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, cfg.diamond - UserDataProxy.inst.playerData.gem);
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_BUYEXPLORESLOT, costType);
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

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
        contentPane.sureAgainObj.SetActive(false);
    }
}
