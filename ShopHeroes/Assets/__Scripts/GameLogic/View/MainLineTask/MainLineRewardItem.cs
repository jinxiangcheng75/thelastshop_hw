using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLineRewardItem : MonoBehaviour
{
    public GUIIcon icon;
    public Text nameText;
    public Text numText;
    public Button selfBtn;

    //int itemId;
    //long count;
    CommonRewardData data;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, data, selfBtn.transform);
        });
    }

    public void setData(OneRewardItem _data)
    {
        data = new CommonRewardData(_data.itemId, _data.count, _data.quality, _data.itemType);
        //this.itemId = data.itemId;
        //this.count = data.count;

        var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
        if (tempItemCfg == null)
        {
            var equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(data.rewardId);
            icon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1]);
            nameText.text = equipConfig.name;
        }
        else
        {
            nameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
            if (tempItemCfg.type == 16)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
                icon.SetSprite(equipCfg.atlas, equipCfg.icon);
            }
            else
            {
                icon.SetSprite(tempItemCfg.atlas, tempItemCfg.icon);
            }
        }

        numText.text = data.count.ToString();
    }
}
