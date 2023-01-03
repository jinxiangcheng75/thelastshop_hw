using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyMakingSlotView : ViewBase<BuyMakingSlotComp>
{
    public override string viewID => ViewPrefabName.BuyMakingSlotUI;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });

        contentPane.glod_BuyBtn.ButtonClickTween(() =>
        {
            //金币购买槽位
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIHandle_BuyMakeSlot, 0);
        });

        contentPane.gem_BuyBtn.ButtonClickTween(() =>
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                if (((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)cfg.guide_type == K_Guide_Type.TipsAndRestrictClick) && cfg.btn_name == contentPane.gem_BuyBtn.name)
                {
                    EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIHandle_BuyMakeSlot, 1);
                    return;
                }
            }
            if (contentPane.gemAffirmObj.activeSelf)
            {
                //钻石购买槽位
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIHandle_BuyMakeSlot, 1);
            }
            else
            {
                contentPane.gemAffirmObj.SetActive(true);
            }
        });

        contentPane.openMakeListBtn.onClick.AddListener(() =>
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPLIST);
        });
    }

    protected override void onShown()
    {
        base.onShown();
        contentPane.gemAffirmObj.SetActive(false);
        updateUI();
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


    public void updateUI()
    {
        var slotNumber = EquipDataProxy.inst.mskeSlotCount;
        contentPane.currSlotCountTx.text = slotNumber.ToString();
        contentPane.nextSlotCountTx.text = (slotNumber + 1).ToString();
        contentPane.nextSlotCountTx.color = Color.green;
        var cfg = FieldConfigManager.inst.GetFieldConfig(3, slotNumber + 1);
        if (cfg != null)
        {
            contentPane.obj_countMax.SetActive(false);
            contentPane.obj_TipsMax.SetActive(false);
            contentPane.obj_countNotMax.SetActive(true);
            contentPane.obj_buttonNotMax.SetActive(true);

            contentPane.needGlodNumberTx.text = cfg.money == 0 ? LanguageManager.inst.GetValueByKey("免费") : cfg.money.ToString("N0");
            contentPane.needGlodNumberTx.color = cfg.money > UserDataProxy.inst.playerData.gold ? GUIHelper.GetColorByColorHex("FD4F4F") : GUIHelper.GetColorByColorHex("FFFFFF");

            contentPane.needGemNumberTx.text = cfg.diamond == 0 ? LanguageManager.inst.GetValueByKey("免费") : cfg.diamond.ToString("N0");
            //contentPane.needGemNumberTx.color = cfg.diamond > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FD4F4F") : GUIHelper.GetColorByColorHex("FFFFFF");

            contentPane.needBuyLvTx.text = cfg.level.ToString();
            contentPane.needBuyLvTx.color = cfg.level <= UserDataProxy.inst.playerData.level ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("FD4F4F");
        }
        else
        {
            contentPane.obj_countMax.SetActive(true);
            contentPane.obj_TipsMax.SetActive(true);
            contentPane.obj_countNotMax.SetActive(false);
            contentPane.obj_buttonNotMax.SetActive(false);

            contentPane.tx_countMax.text = (slotNumber + 1).ToString();

        }
    }
}
