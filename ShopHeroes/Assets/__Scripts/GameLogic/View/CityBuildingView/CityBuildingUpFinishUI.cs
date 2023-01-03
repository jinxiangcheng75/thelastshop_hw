using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildingUpFinishUI : ViewBase<CityBuildingUpFinishUIComp>
{

    public override string viewID => ViewPrefabName.CityBuildingUpFinishUI;

    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;
        contentPane.continueBtn.ButtonClickTween(hide);
    }


    string getOldValueText(CityBuildingData data)
    {
        string result = string.Empty;
        var oldUpgradeConfig = BuildingUpgradeConfigManager.inst.GetConfig(data.buildingId, data.level - 1);

        if (data.buildingId == 3700)
        {
            result = UserDataProxy.inst.GetExploreGroupRestTimeSpeedUp(data.upgradeConfig.effect_id, true) * 100 + "%";
        }
        else if (data.buildingId == 3800)
        {
            result = UserDataProxy.inst.GetExploreDropMaterialOutputUp(data.upgradeConfig.effect_id, true) * 100 + "%";
        }
        else
        {
            result = oldUpgradeConfig.GetEffectDec();
        }

        return result;
    }

    public void SetData(CityBuildingData data)
    {
        contentPane.upgradeTips.text = LanguageManager.inst.GetValueByKey("{0}升级到了等级{1}！", LanguageManager.inst.GetValueByKey(data.config.name), data.level.ToString());

        bool needSetUrl = data.config.architecture_type == 1 || data.config.architecture_type == 4;

        contentPane.buildingIcon.SetSpriteURL(needSetUrl ? data.config.big_icon : "jianzhu_kexueyuan", needSetNativeSize: true);

        string icon = data.GetIconAndAtlas(out string atlas, data.level - 1);

        contentPane.upgradeItem_icon.SetSprite(atlas, icon);

        contentPane.upgradeItem_title.text = data.GetValueTipStr(false);

        contentPane.upgradeItem_oldValue.text = getOldValueText(data);
        contentPane.upgradeItem_newValue.text = data.upgradeConfig.GetEffectDec();
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(18);
    }

    protected override void onHide()
    {
        base.onHide();

    }
}
