
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtensionFinishComp : MonoBehaviour
{
    public Button confirmBtn;

    public Text oldValue;
    public Text newValue;
}

public class ExtensionFinishView : ViewBase<ExtensionFinishComp>
{
    public override string viewID => ViewPrefabName.ExtensionFinishView;
    public override string sortingLayerName => "window";

    private ExtensionConfig oldCfg;
    private ExtensionConfig newCfg;

    protected override void onInit()
    {
        var c = contentPane;
        c.confirmBtn.onClick.AddListener(hide);
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(68);
        var c = contentPane;
        oldCfg = ExtensionConfigManager.inst.GetExtensionConfig(UserDataProxy.inst.shopData.shopLevel);
        newCfg = ExtensionConfigManager.inst.GetExtensionConfig(UserDataProxy.inst.shopData.shopLevel + 1);

        c.oldValue.text = oldCfg.furniture.ToString();
        c.newValue.text = newCfg == null ? LanguageManager.inst.GetValueByKey("已满级") : $"{newCfg.furniture}(+{newCfg.furniture - oldCfg.furniture})";
    }

    protected override void onHide()
    {
    }
}