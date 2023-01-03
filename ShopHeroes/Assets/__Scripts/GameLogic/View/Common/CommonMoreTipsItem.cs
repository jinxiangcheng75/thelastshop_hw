using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonMoreTipsItem : MonoBehaviour
{
    public GUIIcon icon;
    public Text nameText;
    public Text numText;
    public Button selfBtn;

    CommonRewardData data;

    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, data, selfBtn.transform);
        });
    }

    public void setData(CommonRewardData _data)
    {
        if (_data.rewardId == -1)
        {
            gameObject.SetActive(false);
            return;
        }
        data = _data;
        var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
        if (tempItemCfg == null)
        {
            EquipConfig eqcfg = EquipConfigManager.inst.GetEquipInfoConfig(data.rewardId);
            if (eqcfg != null)
            {
                icon.SetSprite(eqcfg.equipDrawingsConfig.atlas, eqcfg.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[eqcfg.equipQualityConfig.quality - 1]);
                nameText.text = eqcfg.name;
            }
        }
        else
        {
            nameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
            if ((ItemType)tempItemCfg.type == ItemType.EquipmentDrawing)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
                if (equipCfg != null)
                    icon.SetSprite(equipCfg.atlas, equipCfg.icon);
            }
            else
            {
                icon.SetSprite(tempItemCfg.atlas, tempItemCfg.icon);
            }
        }
        numText.text = data.count.ToString();
        gameObject.SetActive(true);
    }

    public void clearData()
    {
        gameObject.SetActive(false);
    }
}
