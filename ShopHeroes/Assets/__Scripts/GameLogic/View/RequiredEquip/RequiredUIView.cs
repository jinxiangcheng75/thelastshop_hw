using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequiredUIView : ViewBase<RequiredComp>
{
    public override string viewID => ViewPrefabName.RequiredEquipUI;
    public override string sortingLayerName => "window";
    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.marketBuyBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_REQUIREDITEM, 0, EquipConfigManager.inst.GetEquipQualityConfig(currEquipId, currEquipQuality).id, curNeedCount, _allQuality);
        });

        contentPane.makeBtn.onClick.AddListener(() =>
        {
            // EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, currEquipId);
            //先判断 是否可以制作
            var equipdata = EquipDataProxy.inst.GetEquipData(currEquipId);
            if (equipdata == null || equipdata.equipState != 2)
            {
                //
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("对应图纸还未解锁！"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            hide();
            bool typetablevisble = true;
            var cfg = WorldParConfigManager.inst.GetConfig(163);
            var value_1 = cfg == null ? 0 : (int)cfg.parameters;
            if (UserDataProxy.inst.playerData.level < value_1)
            {
                cfg = WorldParConfigManager.inst.GetConfig(162);
                var value_2 = cfg == null ? 0 : (int)cfg.parameters;
                if (EquipDataProxy.inst.GetEquipDatas().Count < value_2)
                {
                    typetablevisble = false;
                }
            }
            EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_SHOWMAKELIST, currEquipId, typetablevisble);
        });
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

    int currEquipId, currEquipQuality, curNeedCount;
    bool _allQuality;
    public void SetInfo(int equipdrawingId, int quality, int needCount, bool allQuality)
    {
        currEquipId = equipdrawingId;
        currEquipQuality = quality;
        curNeedCount = needCount;
        _allQuality = allQuality;
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingId);

        int curCount = allQuality ? ItemBagProxy.inst.getEquipAllNumber(equipdrawingId) : ItemBagProxy.inst.getEquipNumber(currEquipId, currEquipQuality);

        contentPane.equipNameTx.text = LanguageManager.inst.GetValueByKey(cfg.name);
        contentPane.numberTx.text = "<color=#ffffff><size=56>" + curCount + "</size></color>" + "/" + needCount;
        contentPane.titleTx.text = LanguageManager.inst.GetValueByKey("缺少要求") + "-" + curCount + "/" + needCount;
        contentPane.equipIcon.SetSprite(cfg.atlas, cfg.icon, StaticConstants.qualityColor[quality - 1]);

        if (curCount >= needCount) hide();
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        AudioManager.inst.PlaySound(11);
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_UPDATEBYENERGY);
    }

    public override void shiftIn()
    {
        base.shiftIn();
        SetInfo(currEquipId, currEquipQuality, curNeedCount, _allQuality);
    }

}
