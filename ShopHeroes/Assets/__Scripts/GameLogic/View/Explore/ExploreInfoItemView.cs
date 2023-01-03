using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class ExploreInfoItemView : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon icon;
    public Text upText;
    public Text downText;
    public Text lockNeedLevelText;
    public GameObject unlockObj;
    public GameObject lockObj;
    public Image lockBgObj;
    public Image upObj;
    public Image selectObj;
    public Image lastObj;
    public Image iconBg;
    public Image iconMask;
    public Image iconBgObj;

    ExploreInstanceLvConfigData data;
    public int index = 0;

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public void setData(ExploreInstanceLvConfigData configData, int curLevel)
    {
        data = configData;

        upText.text = LanguageManager.inst.GetValueByKey(data.effect_dec1);
        downText.text = LanguageManager.inst.GetValueByKey(data.effect_dec2);

        if (configData.effect_type == 1 || configData.effect_type == 6)
        {
            upObj.enabled = true;
        }
        else
        {
            upObj.enabled = false;
        }

        if (configData.effect_type == 5)
        {
            var cfg = ExploreInstanceConfigManager.inst.GetConfig(configData.effect_id[0]);
            if (cfg.instance_group != configData.instance_id)
            {
                iconBg.enabled = true;
                iconMask.enabled = true;
                iconBgObj.enabled = false;
                icon.SetSprite(data.effect_atlas, data.effect_icon, needSetNativeSize: true);
            }
            else
            {
                iconBg.enabled = false;
                iconMask.enabled = false;
                iconBgObj.enabled = true;
                icon.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
                icon.SetSprite(data.effect_atlas, data.effect_icon);
            }
        }
        else
        {
            iconBg.enabled = false;
            iconMask.enabled = false;
            iconBgObj.enabled = true;
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 140);
            icon.SetSprite(data.effect_atlas, data.effect_icon);
        }

        selectObj.enabled = false;
        lockBgObj.enabled = false;
        lastObj.enabled = false;
        if (configData.instance_lv <= curLevel)
        {
            unlockObj.SetActive(true);
            lockObj.SetActive(false);
            lockBgObj.enabled = true;
        }
        else
        {
            if (configData.instance_lv == curLevel + 1)
            {
                selectObj.enabled = true;
            }
            else
            {
                lastObj.enabled = true;
            }
            unlockObj.SetActive(false);
            lockObj.SetActive(true);
            lockNeedLevelText.text = LanguageManager.inst.GetValueByKey("{0}级", configData.instance_lv.ToString());
            //configData.instance_lv + LanguageManager.inst.GetValueByKey("级");
        }
    }
}
