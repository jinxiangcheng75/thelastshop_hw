using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLockDrawingUIView : ViewBase<UnLockDrawingComp>
{
    public override string viewID => ViewPrefabName.UnLockDrawingUI;

    protected override void onInit()
    {
        isShowResPanel = false;
        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.unLockBtn.onClick.AddListener(UnLockEquip);
    }

    private void UnLockEquip()
    {
        if (currEquipId > 0)
        {
            EquipDrawingsConfig _cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(currEquipId);
            if (_cfg.activate_drawing <= UserDataProxy.inst.playerData.drawing)
            {
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_UNLOCKEQUIP, currEquipId);
                hide();
            }
            else
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 26);
            }
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

    private int currEquipId = 0;
    public void showInfo(int equipdrawingid)
    {
        currEquipId = equipdrawingid;
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingid);
        if (cfg != null)
        {
            contentPane.equipIcon.SetSprite(cfg.atlas, cfg.icon);
            contentPane.equipNameTx.text = LanguageManager.inst.GetValueByKey(cfg.name);

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.sub_type);
            contentPane.equipSubTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

            contentPane.currDrawingCountTx.text = UserDataProxy.inst.playerData.drawing.ToString();
            contentPane.needDrawingCountTx.text = "-" + cfg.activate_drawing.ToString();
            contentPane.needDrawingCountBtnTx.text = cfg.activate_drawing.ToString();
            contentPane.remainDrawindCountTx.text = (UserDataProxy.inst.playerData.drawing - cfg.activate_drawing).ToString();

            if (UserDataProxy.inst.playerData.drawing >= cfg.activate_drawing)
            {
                GUIHelper.SetUIGrayColor(contentPane.unLockBtn.transform,1);
                //contentPane.unLockBtn.interactable = true;
                contentPane.lackOfDrawCountObj.SetActive(false);
            }
            else
            {
                GUIHelper.SetUIGrayColor(contentPane.unLockBtn.transform, 0.6f);
                //contentPane.unLockBtn.interactable = false;
                contentPane.lackOfDrawCountObj.SetActive(true);
            }

        }
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
    }
}
