using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItemDataUIMediator : BaseSystem
{
    EquipItemDataView equipItemDataView;
    ResolveEquipUI equipResolveUI;

    protected override void AddListeners()
    {
        EventController.inst.AddListener<string, int, List<EquipItem>>(GameEventType.SHOWUI_EQUIPITEMUI, openUI);
        EventController.inst.AddListener<EquipItem>(GameEventType.SHOWUI_EQUIPRESOLVEUI, openEquipResolveUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_EQUIPRESOLVEUI, closeEquipResolveUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_EQUIPITEMUI, closeUI);

        EventController.inst.AddListener(GameEventType.BagEvent.BAG_EQUIP_UPDATE, onBagEquipUpdate);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<string, int, List<EquipItem>>(GameEventType.SHOWUI_EQUIPITEMUI, openUI);
        EventController.inst.RemoveListener<EquipItem>(GameEventType.SHOWUI_EQUIPRESOLVEUI, openEquipResolveUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_EQUIPRESOLVEUI, closeEquipResolveUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_EQUIPITEMUI, closeUI);
        EventController.inst.RemoveListener(GameEventType.BagEvent.BAG_EQUIP_UPDATE, onBagEquipUpdate);
    }
    private void onBagEquipUpdate()
    {
        if (equipItemDataView != null && equipItemDataView.isShowing)
        {
            equipItemDataView.UpdateList(currequipUid);
        }
    }
    public string currequipUid = "";


    private void openUI(string equipuid, int equipid, List<EquipItem> equipitems)
    {
        equipItemDataView = GUIManager.OpenView<EquipItemDataView>((view) =>
        {
            if (string.IsNullOrEmpty(equipuid))
            {
                EquipItem item = ItemBagProxy.inst.GetEquipItem(equipid);
                if (item != null)
                {
                    equipItemDataView.setEquipItem(item, null);
                }
                else
                {
                    EquipConfig cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipid);
                    item = new EquipItem("", equipid, 0, 0, cfg);
                    equipItemDataView.setEquipItem(item, null);
                }
            }
            else
            {
                currequipUid = equipuid;
                equipItemDataView.setEquipItem(ItemBagProxy.inst.GetEquipItem(equipuid), equipitems);
            }
        });
        // if (equipItemDataView == null)
        // {
        //     equipItemDataView = new EquipItemDataView();
        // }
        // if (equipItemDataView.isShowing)
        // {
        //     if (string.IsNullOrEmpty(equipuid))
        //     {
        //         EquipItem item = ItemBagProxy.inst.GetEquipItem(equipid);
        //         if (item != null)
        //         {
        //             equipItemDataView.setEquipItem(item, null);
        //         }
        //         else
        //         {
        //             EquipConfig cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipid);
        //             item = new EquipItem("", equipid, 0, 0, cfg);
        //             equipItemDataView.setEquipItem(item, null);
        //         }
        //     }
        //     else
        //     {
        //         equipItemDataView.setEquipItem(ItemBagProxy.inst.GetEquipItem(equipuid), equipitems);
        //     }
        // }
        // else
        // {
        //     equipItemDataView.show(() =>
        //     {
        //         if (string.IsNullOrEmpty(equipuid))
        //         {
        //             EquipItem item = ItemBagProxy.inst.GetEquipItem(equipdrawingid, quality);
        //             if (item != null)
        //             {
        //                 equipItemDataView.setEquipItem(item, null);
        //             }
        //             else
        //             {
        //                 EquipConfig cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipdrawingid, quality);
        //                 item = new EquipItem("", equipid, 0, 0, cfg);
        //                 equipItemDataView.setEquipItem(item, null);
        //             }
        //         }
        //         else
        //         {
        //             currequipUid = equipuid;
        //             equipItemDataView.setEquipItem(ItemBagProxy.inst.GetEquipItem(equipuid), equipitems);
        //         }
        //     });
        // }
    }

    private void closeUI()
    {
        if (equipItemDataView.isShowing)
        {
            equipItemDataView.hide();
        }
    }

    private void openEquipResolveUI(EquipItem equipItem)
    {
        equipResolveUI = GUIManager.OpenView<ResolveEquipUI>((view) =>
        {
            equipResolveUI.SetData(equipItem);
        });
    }

    private void closeEquipResolveUI()
    {
        if (equipResolveUI.isShowing)
        {
            equipResolveUI.hide();
        }
    }
}
