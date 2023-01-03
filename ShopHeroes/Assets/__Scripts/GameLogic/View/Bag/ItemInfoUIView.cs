using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoUIView : ViewBase<ItemInfoUIComp>
{
    public override string viewID => ViewPrefabName.ItemInfoUI;
    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;
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

    public void ShowInfo(int itemid)
    {
        contentPane.itemInfoObj.SetActive(true);
        itemConfig cfg = ItemconfigManager.inst.GetConfig(itemid);
        if (cfg.type == 16)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(cfg.effect);
            contentPane.icon.SetSprite(equipCfg.atlas, equipCfg.icon);
        }
        else
        {
            contentPane.icon.SetSprite(cfg.atlas, cfg.icon);
        }
        contentPane.nameTx.text = LanguageManager.inst.GetValueByKey(cfg.name);
        contentPane.typeTx.text = ItemBagProxy.inst.GetItemTypeStr((ItemType)cfg.type);
        contentPane.desTx.text = LanguageManager.inst.GetValueByKey(cfg.desc);
        contentPane.inventoryTx.text = ItemBagProxy.inst.resItemCount(itemid).ToString();
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
    }
}
