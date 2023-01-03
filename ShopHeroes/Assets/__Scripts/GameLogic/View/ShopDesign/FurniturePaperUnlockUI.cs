using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePaperUnlockUI : ViewBase<FurniturePaperUnlockUIComp>
{

    public override string viewID => ViewPrefabName.FurniturePaperUnlockUI;
    public override string sortingLayerName => "popup";


    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.goldBtn.onClick.AddListener(() =>
        {
            if (_meet)
            {
                if (_okcallback != null)
                {
                    _okcallback();
                }
                hide();
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("#FF2828"));
            }
        });
        contentPane.gemBtn.onClick.AddListener(() =>
        {
            if (_meet)
            {
                contentPane.gemAffirmObj.gameObject.SetActive(true);
            }
            else
            {
                FurnitureConfig fcfg = FurnitureConfigManager.inst.getConfig(_currId);
                if (fcfg != null)
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, fcfg.cost_num);
                else
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("#FF2828"));

            }
        });
        contentPane.gemAffirmObj.onClick.AddListener(() =>
        {
            if (_okcallback != null)
            {
                _okcallback();
            }
            hide();
        });
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        contentPane.gemAffirmObj.gameObject.SetActive(false);
        _okcallback = null;
        AudioManager.inst.PlaySound(11);
    }
    int _currId = 0;
    bool _meet = false;
    System.Action _okcallback;
    public void setbuyInfo(int currEditId, bool meet, System.Action okcallback)
    {
        _currId = currEditId;
        _meet = meet;
        _okcallback = okcallback;
        FurnitureConfig fcfg = FurnitureConfigManager.inst.getConfig(currEditId);
        if (fcfg != null)
        {
            contentPane.msgTx.text = LanguageManager.inst.GetValueByKey(fcfg.name);
            contentPane.titleText.text = LanguageManager.inst.GetValueByKey(fcfg.type_1 == 1 ? "墙纸解锁" : "地板解锁");
            contentPane.icon.SetSprite(fcfg.atlas, fcfg.icon);
            if (fcfg.cost_type == 1)
            {
                contentPane.goldBtn.gameObject.SetActive(true);
                contentPane.gemBtn.gameObject.SetActive(false);
                contentPane.goldCostTx.text = fcfg.cost_num.ToString();
                contentPane.goldCostTx.color = meet ? Color.white : Color.red;
            }
            else
            {
                contentPane.goldBtn.gameObject.SetActive(false);
                contentPane.gemBtn.gameObject.SetActive(true);
                contentPane.gemCostTx.text = fcfg.cost_num.ToString();
                //contentPane.gemCostTx.color = meet ? Color.white : Color.red;
            }
        }
        else
        {
            hide();
        }
    }
}
