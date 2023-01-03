using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class ExclusiveItem : MonoBehaviour, IDynamicScrollViewItem
{
    public Image bg;
    public GUIIcon icon;
    public GameObject selectObj;
    public Button selfBtn;
    ExclusiveItemData data;
    CommonRewardData commonData;
    public int index = 0;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            var cfg = ItemconfigManager.inst.GetConfig(data.itemId);
            if (cfg.type == 16)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUIBYDRAWINGID, cfg.effect);
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, selfBtn.transform);
            }
        });
    }

    public void setData(ExclusiveItemData _data)
    {
        data = _data;
        commonData = new CommonRewardData(data.itemId, 1, 1, (int)ItemType.EquipmentDrawing);
        //var cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(data.equipId);
        var cfg = ItemconfigManager.inst.GetConfig(data.itemId);
        if (cfg != null)
        {
            if (cfg.type == 16)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(cfg.effect);
                icon.SetSprite(equipCfg.atlas, equipCfg.icon);
            }
            else
            {
                icon.SetSprite(cfg.atlas, cfg.icon);
            }
        }
        if (data.isUnlock == 1)
        {
            GUIHelper.SetUIGray(bg.transform, false);
            selectObj.SetActive(true);
        }
        else
        {
            GUIHelper.SetUIGray(bg.transform, true);
            selectObj.SetActive(false);
        }
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
    }
}
