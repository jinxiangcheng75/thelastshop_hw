using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleBuySlotView : ViewBase<RoleBuySlotComp>
{
    public override string viewID => ViewPrefabName.RoleBuySlotUI;
    public override int showType => (int)ViewShowType.popup;
    public override string sortingLayerName => "popup";

    private FieldCostConfig cfg;
    public int heroSlotType = 1;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.goldBtn.ButtonClickTween(() => buyRoleSlotField(0));
        contentPane.gemBtn.ButtonClickTween(() =>
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                if (((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)cfg.guide_type == K_Guide_Type.TipsAndRestrictClick) && cfg.btn_name == contentPane.gemBtn.name)
                {
                    buyRoleSlotField(1);
                    return;
                }
            }
            if (contentPane.sureAgainObj.activeSelf)
                buyRoleSlotField(1);
            else
                contentPane.sureAgainObj.SetActive(true);
        });
    }

    public void Init(int slotNum)
    {
        cfg = FieldConfigManager.inst.GetFieldConfig(heroSlotType, slotNum + 1);

        contentPane.tipsText.text = LanguageManager.inst.GetValueByKey(GameTipsConfigManager.inst.GetRandomTipsByType(TipsType.HeroBuySlot));

        contentPane.goldText.text = cfg.money.ToString("N0");
        contentPane.gemText.text = cfg.diamond.ToString("N0");
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
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else if (costType == 1 && cfg.diamond > UserDataProxy.inst.playerData.gem)
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, cfg.diamond - UserDataProxy.inst.playerData.gem);
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_BUYNEWSLOT, costType);
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
        contentPane.sureAgainObj.SetActive(false);
    }
}
