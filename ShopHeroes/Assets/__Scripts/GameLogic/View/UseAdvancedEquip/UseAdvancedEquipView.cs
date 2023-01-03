using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAdvancedEquipView : ViewBase<UseAdvancedEquipTipsCom>
{
    public override string viewID => ViewPrefabName.UseAdvancedEquipTips;
    public System.Action<bool, string> backEvent;
    string curItemIntroduceTx;
    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.onClick.AddListener(() =>
        {
            if (backEvent != null)
            {
                backEvent.Invoke(false, curItemIntroduceTx);
            }
        });
        contentPane.cancleBtn.onClick.AddListener(() =>
        {
            if (backEvent != null)
            {
                backEvent.Invoke(false, curItemIntroduceTx);
            }
        });
        contentPane.okBtn.onClick.AddListener(() =>
        {
            if (backEvent != null)
            {
                backEvent.Invoke(true, curItemIntroduceTx);
            }
        });
    }

    public void setCurrEquipInfo(int equipid, string itemIntroduceTx)
    {
        curItemIntroduceTx = itemIntroduceTx;
        EquipConfig ecfg = EquipConfigManager.inst.GetEquipInfoConfig(equipid);
        if (ecfg != null)
        {
            contentPane.itemName.text = ecfg.quality_name;
            var quality = ecfg.equipQualityConfig.quality;
            contentPane.qualiyText.text = LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[quality - 1]);
            contentPane.qualiyText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[quality - 1]);
            contentPane.itemBg.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[quality - 1]);
            GUIHelper.showQualiyIcon(contentPane.itemBg.GetComponent<RectTransform>(), quality);
            contentPane.itemIcon.SetSprite(ecfg.equipDrawingsConfig.atlas, ecfg.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[quality - 1]);
            contentPane.itemIntroduce.text = itemIntroduceTx;
        }
        else
        {

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
