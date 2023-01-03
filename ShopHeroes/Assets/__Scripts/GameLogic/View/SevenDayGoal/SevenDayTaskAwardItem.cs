using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SevenDayTaskAwardItem : MonoBehaviour
{
    public GUIIcon icon;
    public GUIIcon icon_kuang;
    public Text numText;
    public Image gouImg;
    public Image grayImg;
    public Button selfBtn;

    CommonRewardData commonData;

    private void Awake()
    {
        selfBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, selfBtn.transform);
        });
    }

    public void setData(int itemType, int itemId, int itemCount, bool isGet, bool isSevenPass)
    {
        commonData = new CommonRewardData(itemId, itemCount, 1, itemType);
        commonData.specialType = isSevenPass ? 2 : 0;
        gouImg.enabled = isGet;
        grayImg.enabled = isGet;

        if (isSevenPass)
        {
            icon_kuang.SetSprite("sevenday_atlas", isGet ? "qiri_jianglidi2" : "fuli_haohuakuang");
        }

        numText.text = itemCount.ToString();
        var itemCfg = ItemconfigManager.inst.GetConfig(itemId);
        if (itemCfg == null)
        {
            EquipConfig eqcfg = EquipConfigManager.inst.GetEquipInfoConfig(itemId);
            icon.SetSprite(eqcfg.equipDrawingsConfig.atlas, eqcfg.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[eqcfg.equipQualityConfig.quality - 1]);
        }
        else
        {
            if (itemCfg != null)
            {
                if ((ItemType)itemCfg.type == ItemType.EquipmentDrawing)
                {
                    var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemCfg.effect);
                    icon.SetSprite(equipCfg.atlas, equipCfg.icon);
                }
                else if ((ItemType)itemType == ItemType.Equip)
                {
                    EquipConfig eqcfg = EquipConfigManager.inst.GetEquipInfoConfig(itemId);
                    icon.SetSprite(eqcfg.equipDrawingsConfig.atlas, eqcfg.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[eqcfg.equipQualityConfig.quality - 1]);
                }
                else
                {
                    icon.SetSprite(itemCfg.atlas, itemCfg.icon);
                }
            }
        }
    }
}
