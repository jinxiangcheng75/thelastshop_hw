using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUnlockFurniture : MonoBehaviour
{
    public GUIIcon icon;
    public Text upText;
    public Text downText;
    public Button confirmBtn;

    private void Awake()
    {
        confirmBtn.onClick.AddListener(() =>
        {
            AudioManager.inst.PlaySound(62);
            GuideDataProxy.inst.CurInfo.isClickTarget = true;
            GuideManager.inst.GuideManager_OnNextGuide();
        });
    }

    public void showGUnlockFurniture()
    {
        AudioManager.inst.PlaySound(25);
        gameObject.SetActiveTrue();

        var cfg = GuideDataProxy.inst.CurInfo;
        //var furniture = FurnitureConfigManager.inst.getConfig(int.Parse(cfg.m_curCfg.conditon_param_1));
        icon.SetSpriteURL(cfg.m_curCfg.conditon_param_1, needSetNativeSize: true);
        upText.text = LanguageManager.inst.GetValueByKey(cfg.m_curCfg.conditon_param_2);
        downText.text = LanguageManager.inst.GetValueByKey(cfg.m_curCfg.conditon_param_3);
    }

    public void hideGUnlockFurniture()
    {
        gameObject.SetActiveFalse();
    }
}
