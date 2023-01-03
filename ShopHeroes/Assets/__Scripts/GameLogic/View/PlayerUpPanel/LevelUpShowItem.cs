using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpShowItem : MonoBehaviour
{
    public GUIIcon contentGuiIcon;
    public Image GoldIcon;
    public GameObject type_bg;
    public Text orderTx;
    public Image workerBg;

    public void SetData(int mainType, int itemId, int mainValue)
    {
        gameObject.SetActive(true);

        switch (mainType)
        {
            //金币解锁
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 8:
                GoldIcon.gameObject.SetActive(true);
                break;
            default:
                GoldIcon.gameObject.SetActive(false);
                break;
        }

        if (mainType == 0) //市场
        {
            orderTx.text = mainValue.ToString();
            type_bg.SetActive(true);
        }
        else
        {
            orderTx.text = "";
            type_bg.SetActive(false);
        }

        workerBg.enabled = mainType == 7; //工匠红色底图

        if (mainType <= 5)
        {
            contentGuiIcon.SetSprite("PlayerUp_atlas", StaticConstants.PlayerUpTypeSprites[mainType]/*, needSetNativeSize: true*/);
        }
        else
        {
            string _atlasName = string.Empty;
            string _spriteName = string.Empty;
            bool needSetNativeSize = true;

            switch (mainType)
            {
                case 6:
                    break;
                case 7: //工匠id
                    var workerCfg = WorkerConfigManager.inst.GetConfig(itemId);
                    if (workerCfg == null)
                    {
                        Logger.error("工匠表中无此ID：" + itemId);
                        break;
                    }
                    _atlasName = StaticConstants.roleHeadIconAtlasName;
                    _spriteName = workerCfg.icon;
                    needSetNativeSize = false;
                    break;
                case 8:
                    break;
                case 9://装饰
                case 10://家具
                    var cfg = FurnitureConfigManager.inst.getConfig(itemId);
                    if (cfg == null)
                    {
                        Logger.error("家具表中无此ID：" + itemId);
                        break;
                    }
                    _atlasName = cfg.atlas;
                    _spriteName = cfg.icon;
                    break;
                default:
                    break;
            }

            contentGuiIcon.SetSprite(_atlasName, _spriteName, needSetNativeSize: needSetNativeSize);
            if (!needSetNativeSize)
                contentGuiIcon.iconImage.rectTransform.sizeDelta = Vector2.one * 190f;
        }
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }

}
