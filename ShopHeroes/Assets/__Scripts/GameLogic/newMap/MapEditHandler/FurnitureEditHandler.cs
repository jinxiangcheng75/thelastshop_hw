using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店场景内  柜台货架、装饰操作交互
/// </summary>
public partial class IndoorMapEditSys
{
    private int _currSelectFunritureUid = -1;
    public int currEntityUid
    {
        get { return _currSelectFunritureUid; } // 选中对象
    }
    public bool isClickFunriture;

    public bool storePetHouseByPetSetting = false;

    private void AddListeners_Furniture()
    {
        EventController.inst.AddListener<int>(GameEventType.ShopDesignEvent.PICK_ITEM, OnFurnituresSelect);
        EventController.inst.AddListener(GameEventType.ShopDesignEvent.ROTATE_ITEM, OnFurnituresChangeDir);
        EventController.inst.AddListener<int, int>(GameEventType.ShopDesignEvent.Store_Result, onStoreItem);
        EventController.inst.AddListener<int, int, int, int>(GameEventType.ShopDesignEvent.Furniture_Move_Rotate, FurnitureMove);
        //创建
        EventController.inst.AddListener<FurnitureDisplayData>(GameEventType.ShopDesignEvent.Create_Furniture, onCreateFurniture);
        EventController.inst.AddListener<int, bool>(GameEventType.ShopDesignEvent.Furniture_Data_Update, FurnitureDataUpdate);
        EventController.inst.AddListener<bool>(GameEventType.ShopDesignEvent.Apply, OnDesignApply);
        //资源篮显示
        EventController.inst.AddListener(GameEventType.FurnitureDisplayEvent.ResBoxDisPlay_ReShow, OnResItemChange);
    }
    private void RemoveListeners_Furniture()
    {
        EventController.inst.RemoveListener<int>(GameEventType.ShopDesignEvent.PICK_ITEM, OnFurnituresSelect);
        EventController.inst.RemoveListener(GameEventType.ShopDesignEvent.ROTATE_ITEM, OnFurnituresChangeDir);
        EventController.inst.RemoveListener<int, int>(GameEventType.ShopDesignEvent.Store_Result, onStoreItem);
        EventController.inst.RemoveListener<int, int, int, int>(GameEventType.ShopDesignEvent.Furniture_Move_Rotate, FurnitureMove);

        EventController.inst.RemoveListener<FurnitureDisplayData>(GameEventType.ShopDesignEvent.Create_Furniture, onCreateFurniture);
        EventController.inst.RemoveListener<int, bool>(GameEventType.ShopDesignEvent.Furniture_Data_Update, FurnitureDataUpdate);
        EventController.inst.RemoveListener<bool>(GameEventType.ShopDesignEvent.Apply, OnDesignApply);

        //资源篮显示
        EventController.inst.RemoveListener(GameEventType.FurnitureDisplayEvent.ResBoxDisPlay_ReShow, OnResItemChange);


    }

    void OnDesignApply(bool all)
    {
        switch (shopDesignMode)
        {
            case DesignMode.FurnitureEdit:
                {
                    if (currEntityUid == IndoorMap.tempItemUid)
                    {
                        //购买
                        var cfg = FurnitureConfigManager.inst.getConfig(IndoorMap.inst.currSelectEntity.id);
                        UserDataProxy.inst.buyItem(cfg.id, cfg.type_1, IndoorMap.inst.currSelectEntity.cellpos.x, IndoorMap.inst.currSelectEntity.cellpos.y, IndoorMap.inst.currSelectEntity.dir);
                    }
                    else
                    {
                        IndoorMap.inst.saveCurrentFurniture();
                    }
                }
                break;
            case DesignMode.FloorEdit:
                {
                    if (currEditFloorId <= 0) return;
                    var changeSize = editFloorSize;
                    changeSize.x -= StaticConstants.IndoorOffsetX;
                    if (all)
                    {
                        changeSize = UserDataProxy.inst.GetIndoorSize();
                    }

                    //检查是否拥有墙面
                    int index = UserDataProxy.inst.shopData.OwnedFloorList.IndexOf(currEditFloorId);
                    if (index < 0)
                    {
                        var cfg = FurnitureConfigManager.inst.getConfig(currEditFloorId);
                        if (UserDataProxy.inst.playerData.level < cfg.unlock_lv)
                        {
                            //等级不足
                            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                            return;
                        }
                        GUIManager.OpenView<FurniturePaperUnlockUI>((view) =>
                        {
                            view.setbuyInfo(currEditFloorId, buyCheck(cfg.cost_type, cfg.cost_num, cfg.cost_num, cfg.unlock_lv), () =>
                            {
                                UserDataProxy.inst.setFloor(changeSize, currEditFloorId, true);
                                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
                            });
                        });
                        return;
                    }
                    UserDataProxy.inst.setFloor(changeSize, currEditFloorId, false);
                }
                break;
            case DesignMode.WallEdit:
                {
                    if (currEditWallId <= 0) return;
                    int _index = 0;
                    if (all)
                    {
                        _index = 0;
                        currEditorCount = UserDataProxy.inst.shopData.wallPaperList.Count;
                    }
                    else
                    {
                        _index = currStartWallIndex;
                        if (currEditorWallDir == 1)
                        {
                            var mapsize = UserDataProxy.inst.GetIndoorSize();
                            _index += (mapsize.width + 1) / 2;
                        }
                    }
                    int index = UserDataProxy.inst.shopData.OwnedWallList.IndexOf(currEditWallId);
                    if (index < 0)
                    {
                        //提示购买
                        var cfg = FurnitureConfigManager.inst.getConfig(currEditWallId);
                        if (UserDataProxy.inst.playerData.level < cfg.unlock_lv)
                        {
                            //等级不足
                            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                            return;
                        }

                        GUIManager.OpenView<FurniturePaperUnlockUI>((view) =>
                        {
                            view.setbuyInfo(currEditWallId, buyCheck(cfg.cost_type, cfg.cost_num, cfg.cost_num, cfg.unlock_lv), () =>
                            {
                                UserDataProxy.inst.SetWall(_index, currEditorCount, currEditWallId, true);
                                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
                            });
                        });
                        return;
                    }

                    UserDataProxy.inst.SetWall(_index, currEditorCount, currEditWallId, false);
                }
                break;
        }
    }


    bool buyCheck(int costType, int needGold, int needGem, int needLevel)
    {
        if (GuideManager.inst.isInTriggerGuide)
        {
            return true;
        }
        if (costType == 1)
        {
            if (UserDataProxy.inst.playerData.gold < needGold && UserDataProxy.inst.playerData.designFreeCount <= 0)
            {
                //金币不足
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                return false;
            }
        }
        else
        {
            if (UserDataProxy.inst.playerData.gem < needGem)
            {
                //钻石不足
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("#FF2828"));
                return false;
            }
        }
        return true;
    }

    void FurnitureDataUpdate(int uid, bool change)
    {
        //刷新场景中的家具
        IndoorMap.inst.UpdateFurniture(uid, change);
    }
    void OnFurnituresSelect(int uid)
    {
        IndoorMap.inst.OnFurnituresSelect(uid);
        isClickFunriture = true;
        _currSelectFunritureUid = IndoorMap.inst == null ? IndoorMap.tempItemUid : IndoorMap.inst.currSelectEntity == null ? IndoorMap.tempItemUid : IndoorMap.inst.currSelectEntity.uid;
        //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.normal, -1);
        //显示设计界面
        DesignViewRefresh();
    }

    void OnFurnituresRelease()
    {
        IndoorMap.inst.OnFurnituresUnSelect();
        _currSelectFunritureUid = -1;
    }

    void OnFurnituresChangeDir()
    {
        if (shopDesignMode == DesignMode.WallEdit)
        {
            wallEditerRotate();
        }
        else if (shopDesignMode == DesignMode.FurnitureEdit)
        {
            IndoorMap.inst.ChangeFurnitureDir();
        }
    }

    public void storeItem(int uid, int type_1)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_InStore()
            {
                uid = uid,
                designType = type_1
            }
        });
    }

    void onStoreItem(int uid, int type)
    {

        IndoorMap.inst.RemoveFurnituresOnMap(uid);

        if (currEntityUid == uid)
        {
            OnFurnituresRelease();
            isClickFunriture = false;

            DesignModeChange(DesignMode.modeSelection, IndoorMap.tempItemUid);
        }

        if (type == 11 && storePetHouseByPetSetting)
        {
            storePetHouseByPetSetting = false;
            return;
        }
    }

    //移动到
    void FurnitureMove(int uid, int intx, int inty, int dir)
    {
        UserDataProxy.inst.setFurniturePos(uid, intx, inty, dir);
    }

    //添加到场景
    void onCreateFurniture(FurnitureDisplayData uiDisplayerData)
    {
        IndoorData.ShopDesignItem data = UserDataProxy.inst.GetFuriture(uiDisplayerData.uid);
        if (data != null)
        {
            IndoorMap.inst.AddFurnituresToMap(data, true);
            _currSelectFunritureUid = uiDisplayerData.uid;
        }
        else
        {
            //新建
            IndoorMap.inst.AddNewFurnituresToMap(uiDisplayerData.id, true);
            _currSelectFunritureUid = IndoorMap.tempItemUid;
        }
        if (shopDesignMode != DesignMode.normal)
        {
            shopDesignMode = DesignMode.FurnitureEdit;
        }
    }

    void OnResItemChange()
    {
        var list = UserDataProxy.inst.GetEntitys(kTileGroupType.ResourceBin);
        if (list == null) return;

        foreach (IndoorData.ShopDesignItem item in list)
        {
            Furniture furniture;
            if (IndoorMap.inst.GetFurnituresByUid(item.uid, out furniture))
            {
                furniture.OnResItemCountChange();
            }
        }
    }
}
