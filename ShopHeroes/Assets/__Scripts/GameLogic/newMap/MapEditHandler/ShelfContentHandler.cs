using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class IndoorMapEditSys
{

    private ShelfContentUIView _contentView;
    private ShelfInventoryUIView _inventoryView;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Dictionary<int, ShelfDisplay> _shelfDisplayDic = new Dictionary<int, ShelfDisplay>();

    public Dictionary<int, Action> shelfEquipToShopperHandlers = new Dictionary<int, Action>();

    void AddListeners_Shelf()
    {
        var e = EventController.inst;
        e.AddListener(GameEventType.SHOWUI_SHELFCONTENTUI, onShowShelfContentUI);
        e.AddListener(GameEventType.HIDEUI_SHELFCONTENTUI, onHideShelfContentUI);
        e.AddListener<int[],int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SHOW_INVENTORYUI, ShowInventoryUI);
        e.AddListener(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_HIDE_INVENTORYUI, HideInventoryUI);
        e.AddListener<string, int>(GameEventType.FurnitureDisplayEvent.SHELFUPGRADE_PUTONEQUIP, PutEquipOnByShelf);
        e.AddListener<int, int, string>(GameEventType.FurnitureDisplayEvent.SHELFUPGRADE_TAKEDOWNEQUIP, TakeEquipDownByShelf);
        e.AddListener<IndoorData.ShopDesignItem, Response_Design_ShelfEquipChange>(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip, EquipChangeByShelf);
        e.AddListener<int, ShelfDisplay, ShelfUpgradeConfig>(GameEventType.FurnitureDisplayEvent.ShelfDisplay_ReBind, ReBindShlefDisplayByUid);
    }

    void RemoveListeners_Shelf()
    {
        var e = EventController.inst;
        e.RemoveListener(GameEventType.SHOWUI_SHELFCONTENTUI, onShowShelfContentUI);
        e.RemoveListener(GameEventType.HIDEUI_SHELFCONTENTUI, onHideShelfContentUI);
        e.RemoveListener<int[],int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SHOW_INVENTORYUI, ShowInventoryUI);
        e.RemoveListener(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_HIDE_INVENTORYUI, HideInventoryUI);
        e.RemoveListener<string, int>(GameEventType.FurnitureDisplayEvent.SHELFUPGRADE_PUTONEQUIP, PutEquipOnByShelf);
        e.RemoveListener<int, int, string>(GameEventType.FurnitureDisplayEvent.SHELFUPGRADE_TAKEDOWNEQUIP, TakeEquipDownByShelf);
        e.RemoveListener<IndoorData.ShopDesignItem, Response_Design_ShelfEquipChange>(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip, EquipChangeByShelf);
        e.RemoveListener<int, ShelfDisplay, ShelfUpgradeConfig>(GameEventType.FurnitureDisplayEvent.ShelfDisplay_ReBind, ReBindShlefDisplayByUid);
    }

    private void onEquipChgByShelfDisplayByUid(int key, ShelfEquip equip, int isAuto, bool onOrOff, int isFromSlotOrBox)
    {
        if (_shelfDisplayDic.ContainsKey(key))
        {
            _shelfDisplayDic[key].TakeEquipOnOrOff(equip, isAuto, onOrOff, isFromSlotOrBox);
        }
        else
        {
            Logger.log("该Uid未找到对应货柜显示    uid = " + key);
        }
    }

    private void AddShelfDisplayByUid(int shelfUid, ShelfDisplay shelfDisplay)
    {
        if (!_shelfDisplayDic.ContainsKey(shelfUid))
        {
            shelfDisplay.shelfUid = shelfUid;
            _shelfDisplayDic.Add(shelfUid, shelfDisplay);
        }
    }

    private void RemoveShelfDisplayByUid(int shelfUid)
    {
        if (_shelfDisplayDic.ContainsKey(shelfUid))
        {
            _shelfDisplayDic.Remove(shelfUid);
        }
    }

    private void ReBindShlefDisplayByUid(int shelfUid, ShelfDisplay shelfDisplay, ShelfUpgradeConfig shelfUpgradeConfig)
    {
        RemoveShelfDisplayByUid(shelfUid);
        AddShelfDisplayByUid(shelfUid, shelfDisplay);
        shelfDisplay.shelfUid = shelfUid;
        shelfDisplay.ResetShelfUpgradeCfg(shelfUpgradeConfig);
        shelfDisplay.RefreshDisPlay(UserDataProxy.inst.GetFuriture(shelfUid).equipList);
    }




    void onShowShelfContentUI()
    {
        _contentView = GUIManager.OpenView<ShelfContentUIView>((shelfContentUIView) =>
        {
            shelfContentUIView.setItem(UserDataProxy.inst.GetFuriture(currEntityUid));
        });
    }

    void onHideShelfContentUI()
    {
        GUIManager.HideView<ShelfContentUIView>();
    }


    //刷新货架内容显示
    void RefreshShelfContent(IndoorData.ShopDesignItem shelf)
    {
        if (_contentView != null && _contentView.isShowing)
        {
            _contentView.RefreshShelfGridItem(shelf);
        }

        EventController.inst.TriggerEvent(GameEventType.RefreshUI_Furniture_ShelfContent, shelf);
    }

    private void RefreshInventoryUI()
    {
        if (_inventoryView != null && _inventoryView.isShowing)
        {
            _inventoryView.GetItemLists(_inventoryView.typeIdArray, _inventoryView.shelfUid, false);
        }
    }

    //打开物品栏
    private void ShowInventoryUI(int[] ids, int shelfUid)
    {
        _inventoryView = GUIManager.OpenView<ShelfInventoryUIView>((shelfInventoryUIView) =>
        {
            shelfInventoryUIView.GetItemLists(ids, shelfUid, GameSettingManager.inst.needShowUIAnim);
        });
    }


    //关闭物品栏
    private void HideInventoryUI()
    {
        GUIManager.HideView<ShelfInventoryUIView>();
    }


    //将物品放上货架
    private void PutEquipOnByShelf(string equip_uid, int shelfUid)
    {
        var shelf = UserDataProxy.inst.GetFuriture(shelfUid);
        int fieldId = GetShelfIdleEquipItemIndex(shelf, _inventoryView.typeIdArray);

        if (fieldId == -1)
        {
            return;
        }

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_OnShelf()
            {
                designType = (int)EDesignType.Shelf,
                shelfUid = shelf.uid,
                fieldId = fieldId,
                equipUid = equip_uid,
            }
        });
    }


    //将货架上的物品取下来
    private void TakeEquipDownByShelf(int fieldId, int shelf_uid, string equip_uid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_OffShelf()
            {
                designType = (int)EDesignType.Shelf,
                shelfUid = shelf_uid,
                fieldId = fieldId,
                equipUid = equip_uid,
            }
        });
    }


    private int GetShelfIdleEquipItemIndex(IndoorData.ShopDesignItem shelf, int[] typeidArray = null)
    {
        int result = -1;

        shelf.equipList.Sort((x, y) => x.fieldId.CompareTo(y.fieldId));
        var curShelfConfig = ShelfUpgradeConfigManager.inst.getConfigByType(shelf.config.type_2, shelf.level);

        if (shelf.equipList.Count >= curShelfConfig.store)
        {
            return result;
        }

        if (shelf.type != (int)kShelfType.ColdeWeapon) //不是冷武器
        {
            for (int i = 0; i < 12; i++)
            {
                if (curShelfConfig.getFieldByLevel(i + 1)[0] != -1 && Enumerable.SequenceEqual(curShelfConfig.getFieldByLevel(i + 1), typeidArray) && shelf.equipList.Find(t => t.fieldId == i + 1) == null)
                {
                    result = i + 1;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 12; i++)
            {
                if (curShelfConfig.getFieldByLevel(i + 1)[0] == -1) continue;

                if (shelf.equipList.Find(t => t.fieldId == i + 1) == null)
                {
                    result = i + 1;
                    break;
                }
            }
        }

        return result;
    }


    //货架物品发生改变
    private void EquipChangeByShelf(IndoorData.ShopDesignItem shelf, Response_Design_ShelfEquipChange data)
    {
        if (data.onOrOff == 1)
        {
            var cfg = ShelfUpgradeConfigManager.inst.getConfigByType(shelf.config.type_2, shelf.level);

            if (shelf.equipList.Count == cfg.store || GetShelfIdleEquipItemIndex(shelf, cfg.getFieldByLevel(data.shelfEquip.fieldId)) == -1)
            {
                HideInventoryUI();
            }
        }

        //Logger.error("货架显示装备UID ： " + data.shelfEquip.equipUid);

        RefreshInventoryUI();
        RefreshShelfContent(shelf);

        if (data.isAuto == 0)
        {
            AudioManager.inst.PlaySound(data.onOrOff == 1 ? 19 : 20);
        }

        onEquipChgByShelfDisplayByUid(shelf.uid, data.shelfEquip, data.isAuto, data.onOrOff == 1, data.isFromSlotOrBox);
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.reSetContentSliderVal);
    }
}
