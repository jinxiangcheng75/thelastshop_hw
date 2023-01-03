using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EquipMakingView : ViewBase<EquipMakingComp>
{
    public override string viewID => ViewPrefabName.MakingUI;

    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.all;
        isShowResPanel = true;
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.needTiliTx.text = "";
        contentPane.lingquBtn.ButtonClickTween(() =>
        {
            if (state == 1)
            {
                //领取
                AudioManager.inst.PlaySound(42);
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_MAKED, slotid);
            }
        });
        contentPane.tiliBtn.ButtonClickTween(() =>
        {
            if (state == 0)
            {
                if (Mathf.CeilToInt(getUpSpeedEnergy()) > UserDataProxy.inst.playerData.energy)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("能量不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_Faster, slotid, 0);
            }
        });

        contentPane.gemBtn.ButtonClickTween(() =>
        {
            if (state == 0)
            {
                int needGem = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(currCoolTime);
                if (needGem > UserDataProxy.inst.playerData.gem)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, needGem - UserDataProxy.inst.playerData.gem);
                    return;
                }

                if (contentPane.gemAffirmObj.activeSelf)
                    EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_Faster, slotid, 1);
                else
                    contentPane.gemAffirmObj.SetActive(true);

            }
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

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
        contentPane.gemAffirmObj.SetActive(false);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
        DOTween.Pause("DO_1");
        GameTimer.inst.RemoveTimer(coolTimerId);
        state = 0;
        equipDrawingscfg = null;

    }
    EquipDrawingsConfig equipDrawingscfg;
    int slotid = 0;
    int state = 0; //制作中 =0 ，制作完成 = 1
    private int maxtime = 0;
    private int currCoolTime = 0;
    int coolTimerId = 0;
    public void showInfo(EquipMakerSlot slot)
    {
        if (slot == null) return;
        slotid = slot.slotId;
        equipDrawingscfg = EquipConfigManager.inst.GetEquipDrawingsCfg((int)slot.equipDrawingId);
        if (equipDrawingscfg != null)
        {
            contentPane.itemNameTx.text = LanguageManager.inst.GetValueByKey(equipDrawingscfg.name);
            contentPane.itemIcon.iconImage.enabled = true;
            contentPane.itemIcon.SetSprite(equipDrawingscfg.atlas, equipDrawingscfg.icon);
        }
        else
        {
            contentPane.itemNameTx.text = "";
            contentPane.itemIcon.iconImage.enabled = false;
        }

        EquipData data = EquipDataProxy.inst.GetEquipData((int)slot.equipDrawingId);
        maxtime = (int)slot.totalTime;
        contentPane.coolTimeBar.maxValue = maxtime;
        contentPane.coolTimeBar.value = (float)(maxtime - slot.currTime);
        currCoolTime = (int)slot.currTime;
        setState(slotid, slot.makeState);
        coolTimerId = GameTimer.inst.AddTimer(1, timeupdate);
        contentPane.coolTimeText.text = TimeUtils.timeSpanStrip(currCoolTime);
        contentPane.coolTimeBar.DOValue(contentPane.coolTimeBar.maxValue, (float)slot.currTime).SetId("DO_1");
        int tiliCost = Mathf.CeilToInt(getUpSpeedEnergy());
        contentPane.needTiliTx.text = tiliCost.ToString("N0");
        contentPane.needTiliTx.color = tiliCost <= UserDataProxy.inst.playerData.energy ? Color.white : Color.red;
        contentPane.needGemTx.text = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(currCoolTime).ToString("N0");
        //contentPane.needGemTx.color = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(currCoolTime) <= UserDataProxy.inst.playerData.gem ? Color.white : Color.red;
    }

    private float getUpSpeedEnergy()
    {
        if (equipDrawingscfg == null) return 0;

        return (float)equipDrawingscfg.speed_up_energy / equipDrawingscfg.production_time * (float)currCoolTime;
    }
    private void timeupdate()
    {
        if (!isShowing) return;
        if (currCoolTime > 0)
        {
            currCoolTime--;
            contentPane.coolTimeText.text = TimeUtils.timeSpanStrip(currCoolTime);
            int tiliCost = Mathf.CeilToInt(getUpSpeedEnergy());
            //刷新体力消耗
            contentPane.needTiliTx.text = tiliCost.ToString("N0");
            contentPane.needTiliTx.color = tiliCost <= UserDataProxy.inst.playerData.energy ? Color.white : Color.red;
            //刷新钻石小号
            contentPane.needGemTx.text = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(currCoolTime).ToString("N0");
            //contentPane.needGemTx.color = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(currCoolTime) <= UserDataProxy.inst.playerData.gem ? Color.white : Color.red;
        }
        else
        {
            //完成制造

            setState(slotid, 2);
        }
    }

    public void setState(int _soltid, int _state)
    {
        if (slotid != _soltid) return;
        if (_state == 0)
        {
            contentPane.headTx.text = "";
            hide();
        }
        else if (_state == 1)
        {
            state = 0;
            contentPane.headTx.text = LanguageManager.inst.GetValueByKey("正在制作中...");
        }
        else if (_state == 2)
        {
            state = 1;
            contentPane.headTx.text = LanguageManager.inst.GetValueByKey("制作完成");

            GameTimer.inst.RemoveTimer(coolTimerId);
            contentPane.coolTimeText.text = "";
        }

        contentPane.coolTimeBar.gameObject.SetActive(state == 0);
        contentPane.needTiliTx.gameObject.SetActive(state == 0);
        contentPane.tiliBtn.gameObject.SetActive(state == 0);
        contentPane.gemBtn.gameObject.SetActive(state == 0);
        contentPane.lingquBtn.gameObject.SetActive(state == 1);
    }
}
