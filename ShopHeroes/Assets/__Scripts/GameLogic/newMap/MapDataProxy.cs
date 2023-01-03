using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;


//public class UserDataProxy : TSingletonHotfix<UserDataProxy>, IDataModelProx
//{
//    private int LocalIncreaseUid = 100000000;
//    public int GetFurnitureUid
//    {
//        get { return LocalIncreaseUid++; }
//    }
//    IndoorData indoorData;


//    public IndoorData shopData
//    {
//        get { return indoorData; }
//    }
//    public void Clear() { }
//    public void Init()
//    {
//        AddServerResponseEvent();
//        indoorData = new IndoorData();
//    }

//    public void save()
//    {
//        SaveManager.Save<IndoorData>(indoorData, StaticConstants.MapDataPath);
//    }

//    public void Load()
//    {
//        //加载本地数据
//        if (!SaveManager.Load(StaticConstants.MapDataPath, ref indoorData))
//        {
//            //加载失败
//            //save();
//        }
//        else
//        {
//            //加载成功
//        }
//    }

//    public int getFurnitureNum()
//    {
//        if (indoorData != null)
//            return indoorData.allIndoorEntity.Count();
//        return 0;
//    }
//    /// <summary>
//    /// 初始化等级场景地格
//    /// </summary>
//    /// <returns></returns>

//    //初始化地图数据
//    bool hasIndoorData = false;
//    public void InitIndoorData(Response_Design_Data data)
//    {
//        indoorData.InitIndoorData(data.shopData, data.floorData, data.wallData, data.furnitureList);
//        hasIndoorData = true;
//    }

//    public Vector3 GetOneTrunkPosition()
//    {
//        IndoorData.ShopDesignItem trunk = indoorData.storageBoxList.GetRandomElement();
//        if (trunk != null && IndoorMap.inst.gameMapGrid)
//        {
//            return MapUtils.CellPosToCenterPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(trunk.x, trunk.y, 0)));
//        }
//        return Vector3.zero;
//    }
//    public IndoorData.ShopDesignItem GetFuriture(int uid)
//    {
//        if (indoorData.allIndoorEntity.ContainsKey(uid))
//            return indoorData.allIndoorEntity[uid];
//        return null;
//    }

//    //随即获取一个货架
//    public int GetOneShelfUid()
//    {
//        var count = indoorData.shelfList.Count;
//        if (count > 0)
//        {
//            return indoorData.shelfList[Random.Range(0, count)].uid;
//        }
//        return 0;
//    }
//    //随即获取一个货架
//    public int GetOneDecorUid()
//    {
//        var count = indoorData.decorList.Count;
//        if (count > 0)
//        {
//            return indoorData.decorList[Random.Range(0, count)].uid;
//        }
//        return 0;
//    }

//    public IndoorData.ShopDesignItem GetCounter()
//    {
//        if (hasIndoorData)
//        {
//            return indoorData.counter;
//        }
//        return null;
//    }
//    public int GetResourceBinCount(int resitemid)
//    {
//        var binList = GetEntitys(kTileGroupType.ResourceBin);
//        if (binList == null || binList.Count <= 0) return 0;
//        var list = binList.FindAll(item => item.resItemId == resitemid);
//        return list == null ? 0 : list.Count;
//    }
//    public List<IndoorData.ShopDesignItem> GetEntitys(kTileGroupType type)
//    {
//        if (!hasIndoorData) return null;
//        switch (type)
//        {
//            case kTileGroupType.Floor:
//                return indoorData.floorList;
//            case kTileGroupType.Wall:   //墙纸
//                return indoorData.wallPaperList;
//            case kTileGroupType.Carpet:     //地毯
//                return indoorData.floorClothList;
//            case kTileGroupType.Shelf:      //货架
//                return indoorData.shelfList;
//            case kTileGroupType.Trunk:      //仓库
//                return indoorData.storageBoxList;
//            case kTileGroupType.ResourceBin: //资源箱子
//                return indoorData.resBasketList;
//            case kTileGroupType.Furniture:  //室内装饰
//                return indoorData.decorList;
//            case kTileGroupType.OutdoorFurniture: //室外装饰
//                return indoorData.outdoorDecor;
//            case kTileGroupType.WallFurniture:  //墙上挂件
//                return indoorData.wallDecorList;
//        }
//        return null;
//    }

//    public RectInt GetIndoorSize()
//    {
//        return indoorData.size;
//    }

//    public int GetCurrentUpgradefurniture()
//    {
//        foreach (var f in indoorData.allIndoorEntity.Values)
//        {
//            if (f.type == 2) continue;
//            if (f.state == 1 || f.state == 2)
//            {
//                return f.uid;
//            }
//        }
//        return 0;
//    }

//    public IndoorData.ShopDesignItem getNearFurniture(int uid, int type, bool isLeft)
//    {
//        var list = GetEntitys((kTileGroupType)type).FindAll((t) => t.state != (int)EDesignState.InStore);
//        int index = list.FindIndex(t => t.uid == uid);
//        index += (isLeft ? -1 : 1);

//        if (index == list.Count) index = 0;
//        if (index == -1) index = list.Count - 1;

//        return list[index];
//    }
//    #region 数据刷新

//    public void updateShopData(ShopData shopData)
//    {
//        indoorData.setShopData(shopData);
//        EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_CALLTAO_SHOPUPGRADE);
//    }


//    public void updateFurnitureData(OneFurniture data)
//    {
//        bool changeModel = false;
//        var furniture = GetFuriture(data.furnitureUid);
//        if (furniture != null)
//        {
//            if (data.furnitureId != furniture.id)
//                changeModel = true;
//        }
//        indoorData.SetFurnitureData(data);
//        //刷新家具数据事件
//        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Data_Update, data.furnitureUid, changeModel);
//    }

//    public void setFurniturePos(int uid, int x, int y, int dir)
//    {
//        if (indoorData.allIndoorEntity.ContainsKey(uid))
//        {
//            var data = indoorData.allIndoorEntity[uid];
//            data.x = x;
//            data.y = y;
//            data.dir = dir;
//            //服务器验证
//            moveItem(data.type, uid, x, y, dir);
//        }
//    }
//    //移动
//    public void moveItem(int type_1, int uid, int _x, int _y, int rot)
//    {
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_Move()
//            {
//                designType = type_1,
//                uid = uid,
//                x = _x,
//                y = _y,
//                rotate = rot,
//            }
//        });
//    }

//    //购买
//    public void buyItem(int id, int type, int _x, int _y, int rot)
//    {
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_Buy()
//            {
//                designType = type,
//                x = _x,
//                y = _y,
//                furnitureId = id,
//                rotate = rot
//            }2
//        });
//    }

//    public void requestLayout()
//    {
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_Data()
//        });
//        if (mapdataIsReady)
//        {
//            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ServerData_Ready);
//        }
//    }
//    //刷新地板
//    public void setFloor(RectInt size, int floorid, bool buy)
//    {
//        if (floorid <= 0) return;
//        List<OneFloor> list = new List<OneFloor>();
//        shopData.floorList.ForEach(floor =>
//        {
//            if (floor.x >= size.xMin && floor.x < size.xMax && floor.y >= size.yMin && floor.y < size.yMax)
//            {
//                floor.id = floorid;
//            }
//            OneFloor f = new OneFloor();
//            f.x = floor.x;
//            f.y = floor.y;
//            f.furnitureId = floor.id;
//            list.Add(f);
//        });
//        var buyfloorid = buy ? floorid : 0;
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_SetFloor()
//            {
//                buyFloorId = buyfloorid,
//                floorList = list
//            }
//        });
//    }
//    public void requestFloor()
//    {
//        shopData.updateBaseMapFloor();
//        List<OneFloor> list = new List<OneFloor>();
//        shopData.floorList.ForEach(floor =>
//        {
//            OneFloor f = new OneFloor();
//            f.x = floor.x;
//            f.y = floor.y;
//            f.furnitureId = floor.id;
//            list.Add(f);
//        });
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_SetFloor()
//            {
//                floorList = list
//            }
//        }
//        );
//    }
//    //修改墙纸
//    public void SetWall(int start, int count, int wallid, bool buy)
//    {
//        if (wallid <= 0) return;
//        List<OneWall> list = new List<OneWall>();
//        shopData.wallPaperList.ForEach(wall =>
//        {
//            OneWall w = new OneWall();
//            w.index = wall.index;
//            w.x = wall.x;
//            w.y = wall.y;
//            if (wall.index < start || wall.index >= (start + count))
//            {
//                w.furnitureId = wall.id;
//            }
//            else
//            {
//                w.furnitureId = wallid;
//            }
//            list.Add(w);
//        });
//        var buywallid = buy ? wallid : 0;
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_SetWall()
//            {
//                buyWallId = buywallid,
//                wallList = list
//            }
//        }
//        );
//    }
//    //刷新墙纸
//    public void requestWall()
//    {
//        shopData.updateBaseMapWall();
//        List<OneWall> list = new List<OneWall>();
//        shopData.wallPaperList.ForEach((wall) =>
//        {
//            OneWall w = new OneWall();
//            w.x = wall.x;
//            w.y = wall.y;
//            w.index = wall.index;
//            w.furnitureId = wall.id;
//            list.Add(w);
//        });
//        NetworkEvent.SendRequest(new NetworkRequestWrapper()
//        {
//            req = new Request_Design_SetWall()
//            {
//                wallList = list
//            }
//        });
//    }
//    #endregion
//    #region  服务器数据返回响应事件
//    private void AddServerResponseEvent()
//    {
//        Helper.AddNetworkRespListener(MsgType.Response_Design_Data_Cmd, OnResponseDesignData);
//        Helper.AddNetworkRespListener(MsgType.Response_Design_FurnitureChange_Cmd, OnResponseDesignFurnitureChange);
//        Helper.AddNetworkRespListener(MsgType.Response_Design_ShelfEquipChange_Cmd, ChangeShelfEquipList);
//        Helper.AddNetworkRespListener(MsgType.Response_Design_InStore_Cmd, onDesignInStoreResp);

//        Helper.AddNetworkRespListener(MsgType.Response_Design_SetFloor_Cmd, OnDesignSetFloorResp);
//        Helper.AddNetworkRespListener(MsgType.Response_Design_SetWall_Cmd, OnDesignSetWallResp);
//        Helper.AddNetworkRespListener(MsgType.Response_Design_Buy_Cmd, onDesignBuy);
//    }
//    void OnDesignSetFloorResp(HttpMsgRspdBase msg)
//    {
//        var data = (Response_Design_SetFloor)msg;
//        if (data.errorCode == (int)EErrorCode.EEC_Success)
//        {
//            shopData.InitIndoorFloorData(data.floorData);
//        }
//        //刷新
//        IndoorMap.inst.RefreshFloor(shopData);
//    }
//    void OnDesignSetWallResp(HttpMsgRspdBase msg)
//    {
//        var data = (Response_Design_SetWall)msg;
//        if (data.errorCode == (int)EErrorCode.EEC_Success)
//        {
//            shopData.InitIndoorWallPaperData(data.wallData);
//        }
//        //刷新
//        IndoorMap.inst.RefreshWall(shopData);
//    }
//    //HttpMsgRspdBase msg
//    void onDesignInStoreResp(HttpMsgRspdBase msg)
//    {
//        var data = (Response_Design_InStore)msg;
//        if (data.errorCode == (int)EErrorCode.EEC_Success)
//        {
//            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Store_Result, data.uid, data.designType);
//            // EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Show_EditMenus);

//        }
//    }
//    void onDesignBuy(HttpMsgRspdBase msg)
//    {
//        var data = (Response_Design_Buy)msg;
//        if (data.errorCode == (int)EErrorCode.EEC_Success)
//        {
//            var furnitureCfg = FurnitureConfigManager.inst.getConfig(data.furnitureId);
//            if (furnitureCfg != null)
//            {
//                if (furnitureCfg.id == 18001)
//                {
//                     PlatformManager.inst.GameHandleEventLog("Res_Woodbin", "");
//                }
//                else if (furnitureCfg.id == 20001)
//                {
//                     PlatformManager.inst.GameHandleEventLog("Res_Chemical", "");
//                }
//                else if ((kTileGroupType)furnitureCfg.type_1 == kTileGroupType.Furniture)
//                {
//                     PlatformManager.inst.GameHandleEventLog("Decorate", "");
//                }
//            }
//        }
//    }

//    bool mapdataIsReady = false;
//    private void OnResponseDesignData(HttpMsgRspdBase msg)
//    {
//        var data = msg as Response_Design_Data;
//        data.furnitureList.Sort((a, b) => { return sortByLevel(a.furnitureId, b.furnitureId, a.level, b.level); });  //排个序
//        InitIndoorData(data);
//        if (!mapdataIsReady)
//        {
//            mapdataIsReady = true;
//            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ServerData_Ready);
//        }

//    }

//    private void OnResponseDesignFurnitureChange(HttpMsgRspdBase msg)
//    {
//        var data = msg as Response_Design_FurnitureChange;
//        if (data.errorCode == (int)EErrorCode.EEC_Success)
//        {
//            updateFurnitureData(data.furniture);
//        }
//    }

//    private void ChangeShelfEquipList(HttpMsgRspdBase msg)
//    {
//        Response_Design_ShelfEquipChange data = (Response_Design_ShelfEquipChange)msg;
//        IndoorData.ShopDesignItem shelf = GetFuriture(data.shelfUid);

//        if (data.onOrOff == 1)
//        {
//            shelf.equipList.Add(data.shelfEquip);
//        }
//        else
//        {
//            shelf.equipList.Remove(shelf.equipList.Find(t => t.fieldId == data.shelfEquip.fieldId));
//        }

//        EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip, shelf, data);
//    }
//    #endregion
//    int sortByLevel(int id1, int id2, int level1, int level2)
//    {
//        if (id1 < id2)
//        {
//            return -1;
//        }
//        else if (id1 > id2)
//        {
//            return 1;
//        }
//        else
//        {
//            return level1.CompareTo(level2);
//        }
//    }


//}
