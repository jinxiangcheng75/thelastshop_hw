using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
//

//本地存储地图数据
[Serializable]
public class IndoorData
{
    //商店内物品状态
    [Serializable]
    public class ShopEntityState
    {
        public double stateStartTime = 0;
        public double stateEndTime = 0;
        public double stateRemainTime = 0;
        public double LocalTime = 0;

        public double stateTime
        {
            get { return stateEndTime - stateStartTime; }
        }
        // public int remainTime
        // {
        //     get { return stateRemainTime + (int)(GameTimer.inst. - LocalTime); }
        // }

        public double leftTime
        {
            get
            {
                Logger.log("家具升级时间：GameTimer.inst.serverNow = " + GameTimer.inst.serverNow.ToString() + " , LocalTime=" + LocalTime.ToString() + " , stateRemainTime =" + stateRemainTime.ToString(), "#ffff00");
                return stateRemainTime - (int)(GameTimer.inst.serverNow - LocalTime);
            }
        }
    }

    [Serializable]
    public class ShopDesignItem
    {
        public int index;
        public int uid;
        public int id;
        public int type;    //1-墙壁 2-地板 3-地毯 4-墙壁装饰 5-室内装饰 6-柜台 7-货架 8-仓库箱子 9-资源篮子 10-室外装饰 11-宠物小窝
        public int level;
        public int x;
        public int y;
        public int sizeX;
        public int sizeY;
        public int dir; //朝向 两个 默认0 朝向 右下。 1 朝向 左下
        public int state;  //EDesignState  {  Idle = 0,Upgrading = 1,Finished = 2,InStore = 3,}
        public int param_1;     //预留变量
        public string param_2;
        public FurnitureConfig config;
        public List<ShelfEquip> equipList = new List<ShelfEquip>();
        public ShopEntityState entityState; //本地时间 

        public int resItemId
        {
            get
            {
                ResourceBinUpgradeConfig subCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(config.type_2, 1);
                return subCfg == null ? -1 : subCfg.item_id;
            }
        }
        public ShopDesignItem()
        {

        }
        public ShopDesignItem(int _id, int _type, int _level, int _x, int _y, int _sizeX, int _sizeY, int _dir, int _state, int _stateStartTime, int _stateEndTime, int _stateRemainTime)
        {
            config = FurnitureConfigManager.inst.getConfig(_id);
            SetData(_id, _type, _level, _x, _y, config.width, config.height, _dir, _state, _stateStartTime, _stateEndTime, _stateRemainTime);
        }

        public void SetData(int _id, int _type, int _level, int _x, int _y, int _sizeX, int _sizeY, int _dir, int _state, int _stateStartTime, int _stateEndTime, int _stateRemainTime)
        {
            id = _id;
            type = _type;
            level = Mathf.Max(_level, 1);
            x = _x;
            y = _y;

            dir = _dir;
            state = _state;
            if (entityState == null) entityState = new ShopEntityState();
            entityState.stateStartTime = _stateStartTime;
            entityState.stateRemainTime = _stateRemainTime;
            entityState.stateEndTime = _stateEndTime;
            entityState.LocalTime = GameTimer.inst.serverNow;
            if (state == 1)
            {
                if (_stateRemainTime > 0)
                {
                    if (refreshTimeId > 0) GameTimer.inst.RemoveTimer(refreshTimeId);
                    refreshTimeId = GameTimer.inst.AddTimer(_stateRemainTime, 1, Refresh);
                }
                else
                {
                    Refresh();
                }
            }
            config = FurnitureConfigManager.inst.getConfig(_id);
            sizeX = config.width;
            sizeY = config.height;
        }
        int refreshTimeId = 0;
        public void Refresh()
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading, uid);
        }
    }

    public int FurnitureCount = 0;
    public int stateStartTime = 0;
    public int stateEndTime = 0;
    public int stateRemainTime = 0;
    public float LocalTime = 0;
    public int stateTime
    {
        get { return stateEndTime - stateStartTime; }
    }
    public int leftTime
    {
        get { return stateRemainTime - (int)(GameTimer.inst.serverNow - LocalTime); }
    }
    public int shopLevel;
    public int currentState;
    public RectInt size;
    public int defaultFloorid = 10001;
    public List<ShopDesignItem> floorList;        //地板2
    //public List<ShopDesignItem> allFloorList; //商店地块最大等级全部地砖
    public int defaultWallPaperId = 11001;
    public List<ShopDesignItem> wallPaperList;    //墙纸1
    public List<ShopDesignItem> floorClothList;   //地毯3
    public List<ShopDesignItem> wallDecorList;    //墙面装饰4
    public List<ShopDesignItem> decorList;        //装饰品（可加能量）5
    public List<ShopDesignItem> shelfList;        //货架7
    public List<ShopDesignItem> resBasketList;    //资源篮子9
    public List<ShopDesignItem> storageBoxList;   //储物箱子8
    public List<ShopDesignItem> outdoorDecor;     //室外装饰10
    public List<ShopDesignItem> petHouseList;     //宠物小家11

    public Dictionary<int, ShopDesignItem> allIndoorEntity;  //商店内所有物件实体列表  Key  (地板墙纸除外)
    public ShopDesignItem counter;    //  柜台 6

    List<int> ownedFloorList;
    //已经拥有地板id列表
    public List<int> OwnedFloorList
    {
        get { return ownedFloorList; }
        private set { ownedFloorList = value; }
    }
    public bool hasFloor(int fid)
    {
        return ownedFloorList != null && ownedFloorList.IndexOf(fid) >= 0;
    }
    //判断场景中是否有对应家具
    public bool hasFurnitureInShop(int furnitureid)
    {
        foreach (var item in allIndoorEntity.Values)
        {
            if (item.state != 2 && item.state != 3)
            {
                if (item.id == furnitureid)
                {
                    return true;
                }
            }
        }
        return false;
    }

    List<int> ownedWallList;
    //已经拥有墙面id列表
    public List<int> OwnedWallList
    {
        get { return ownedWallList; }
        private set { ownedWallList = value; }
    }
    public bool hasWall(int wid)
    {
        return ownedWallList != null && ownedWallList.IndexOf(wid) >= 0;
    }
    public IndoorData()
    {
        //allFloorList = new List<ShopDesignItem>();
        allIndoorEntity = new Dictionary<int, ShopDesignItem>();
        floorList = new List<ShopDesignItem>();
        wallPaperList = new List<ShopDesignItem>();
        floorClothList = new List<ShopDesignItem>();
        wallDecorList = new List<ShopDesignItem>();
        decorList = new List<ShopDesignItem>();
        shelfList = new List<ShopDesignItem>();
        storageBoxList = new List<ShopDesignItem>();
        resBasketList = new List<ShopDesignItem>();
        outdoorDecor = new List<ShopDesignItem>();
        petHouseList = new List<ShopDesignItem>();
        ownedFloorList = new List<int>();
        ownedWallList = new List<int>();
    }

    public void InitIndoorData(ShopData shopData, FloorData floorData, WallData wallData, List<OneFurniture> furnitureList)
    {
        setShopData(shopData);
        //地板
        bool resultFloor = InitIndoorFloorData(floorData);
        //墙
        bool resultWall = InitIndoorWallPaperData(wallData);
        //家具
        InitIndoorFurnitures(furnitureList);

        if (!resultFloor || !resultWall)
        {
            EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_CALLTAO_SHOPUPGRADE);
        }

        if (AccountDataProxy.inst.NeedCreatRole) return;
        D2DragCamera.inst.updateCameMaxZoom(shopData.shopLevel, kCameraMoveType.shopExtend);

    }
    public void setShopData(ShopData shopData)
    {
        if (IndoorGridMapClr.inst.GetCurrIndoorSize(shopData.shopLevel, ref size))
        {
            shopLevel = shopData.shopLevel;
            currentState = shopData.state;
            stateStartTime = shopData.stateStartTime;
            stateRemainTime = shopData.stateRemainTime;
            stateEndTime = shopData.stateEndTime;
            LocalTime = (float)GameTimer.inst.serverNow;
        }
        else
        {
            Logger.error("商店数据异常！！！UserDataProxy.setShopData()");
        }
    }
    //初始地板数据
    public bool InitIndoorFloorData(FloorData data)
    {
        OwnedFloorList = data.ownedFloorList;

        // floorList.Clear();
        // for (int sizex = size.xMin; sizex < size.xMax; sizex += 2)
        // {
        //     for (int xizey = size.yMin; xizey < size.yMax; xizey++)
        //     {
        //         IndoorData.ShopDesignItem flooritem = new IndoorData.ShopDesignItem(data.floorList[0].furnitureId, (int)kTileGroupType.Floor, 0, sizex, xizey, 2, 2, 0, 0, 0, 0, 0);
        //         var config = FurnitureConfigManager.inst.getConfig(data.floorList[0].furnitureId);
        //         flooritem.type = config.type_1;
        //         flooritem.config = config;
        //         flooritem.index = sizex * StaticConstants.IndoorMaxX + sizex * StaticConstants.IndoorMaxY;
        //         floorList.Add(flooritem);
        //     }
        // }

        updateBaseMapFloor();

        data.floorList.ForEach(floor =>
        {
            IndoorData.ShopDesignItem flooritem = floorList.Find(f => f.x == floor.x && f.y == floor.y);
            if (flooritem == null)
            {
                flooritem = new IndoorData.ShopDesignItem(floor.furnitureId, (int)kTileGroupType.Floor, 0, floor.x, floor.y, 2, 2, 0, 0, 0, 0, 0);
                var config = FurnitureConfigManager.inst.getConfig(floor.furnitureId);
                flooritem.type = config.type_1;
                flooritem.config = config;
                flooritem.index = floor.x * StaticConstants.IndoorMaxX + floor.y * StaticConstants.IndoorMaxY;
                floorList.Add(flooritem);
            }
            else
            {
                flooritem.id = floor.furnitureId;
                var config = FurnitureConfigManager.inst.getConfig(floor.furnitureId);
                flooritem.type = config.type_1;
                flooritem.config = config;
            }
        });

        if (data.floorList.Count != floorList.Count)
        {
            return false;
        }
        return true;
    }

    public ShopDesignItem FindFloor(int Cellx, int Celly)
    {
        return floorList.Find(_floor => _floor.x == Cellx && _floor.y == Celly);
    }

    //初始墙面数据
    public bool InitIndoorWallPaperData(WallData data)
    {
        OwnedWallList = data.ownedWallList;

        updateBaseMapWall();

        //刷新墙体数据
        data.wallList.ForEach(wall =>
        {
            IndoorData.ShopDesignItem wallitem = wallPaperList.Find(item => item.index == wall.index);
            if (wallitem != null)
            {
                var config = FurnitureConfigManager.inst.getConfig(wall.furnitureId);
                wallitem.type = config.type_1;
                wallitem.config = config;
                wallitem.id = wall.furnitureId;
                // wallPaperList.Add(wallitem);
            }
            else
            {
                int dir = wall.x == (size.xMax + 1) ? 1 : 0;
                wallitem = new IndoorData.ShopDesignItem(wall.furnitureId, (int)kTileGroupType.Floor, 0, wall.x, wall.y, 2, 2, dir, 0, 0, 0, 0);
                var config = FurnitureConfigManager.inst.getConfig(wallitem.id);
                wallitem.type = config.type_1;
                wallitem.config = config;
                wallitem.index = wall.index;
                wallPaperList.Add(wallitem);
            }
        });
        if (wallPaperList.Count != data.wallList.Count) return false;
        return true;
    }

    public void updateBaseMapFloor()
    {
        List<CellPosition> floorlist = IndoorGridMapClr.inst.GetfloorPosList(shopLevel);
        floorlist.ForEach(pos =>
        {
            IndoorData.ShopDesignItem flooritem = floorList.Find(f => f.x == pos.x && f.y == pos.y);
            if (flooritem == null)
            {
                flooritem = new IndoorData.ShopDesignItem(StaticConstants.floorDefaultId, (int)kTileGroupType.Floor, 0, pos.x, pos.y, 2, 2, 0, 0, 0, 0, 0);
                var config = FurnitureConfigManager.inst.getConfig(flooritem.id);
                flooritem.type = config.type_1;
                flooritem.config = config;
                flooritem.index = pos.index;
                floorList.Add(flooritem);
            }
        });
    }
    public void updateBaseMapWall()
    {
        //获取基本墙体
        List<CellPosition> walllist = IndoorGridMapClr.inst.GetWallPosList(shopLevel);
        walllist.ForEach(pos =>
        {
            IndoorData.ShopDesignItem wallitem = wallPaperList.Find(item => item.index == pos.index);
            if (wallitem == null)
            {
                wallitem = new IndoorData.ShopDesignItem(StaticConstants.wallDefaultId, (int)kTileGroupType.Wall, 0, pos.x, pos.y, 1, 1, pos.dir, 0, 0, 0, 0);
                var config = FurnitureConfigManager.inst.getConfig(StaticConstants.wallDefaultId);
                wallitem.type = config.type_1;
                wallitem.config = config;
                wallitem.index = pos.index;
                wallPaperList.Add(wallitem);
            }
            else
            {
                wallitem.x = pos.x;
                wallitem.y = pos.y;
                wallitem.dir = pos.dir;
            }
        });
    }
    public ShopDesignItem FindWallPaper(int Cellx, int Celly)
    {
        return wallPaperList.Find(_floor => _floor.x == Cellx && _floor.y == Celly);
    }

    public int furnitureLimit()
    {
        var shopmapcfg = ExtensionConfigManager.inst.GetExtensionConfig(shopLevel);
        return shopmapcfg.furniture;
    }

    public int indoorMapFurniture()
    {
        return indoorFurnitureCount;
    }
    //初始化家具
    private int indoorFurnitureCount = 0;
    public void InitIndoorFurnitures(List<OneFurniture> datas)
    {
        Logger.log("初始化家具数据");
        if (!UserDataProxy.inst.hasIndoorData)
            indoorFurnitureCount = 0;
        datas.ForEach(furniture =>
        {
            SetFurnitureData(furniture);
        });
    }

    public void SetFurnitureData(OneFurniture furniture)
    {
        if (!allIndoorEntity.ContainsKey(furniture.furnitureUid))
        {
            var config = FurnitureConfigManager.inst.getConfig(furniture.furnitureId);
            if (config != null)
            {
                var indoorFurniture = new IndoorData.ShopDesignItem(furniture.furnitureId, config.type_1, furniture.level,
                    furniture.x, furniture.y, 0, 0, furniture.rotate, furniture.state, furniture.stateStartTime, furniture.stateEndTime, furniture.stateRemainTime);
                indoorFurniture.uid = furniture.furnitureUid;
                indoorFurniture.config = config;
                indoorFurniture.equipList = furniture.equipList;
                allIndoorEntity.Add(indoorFurniture.uid, indoorFurniture);
                switch (config.type_1)
                {
                    case (int)kTileGroupType.Counter:
                        counter = indoorFurniture;
                        break;
                    case (int)kTileGroupType.ResourceBin:
                        if (indoorFurniture.state != 3)
                            indoorFurnitureCount++;
                        resBasketList.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.OutdoorFurniture:
                        outdoorDecor.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.Carpet:
                        floorClothList.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.WallFurniture:
                        wallDecorList.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.Shelf:
                        if (indoorFurniture.state != 3)
                            indoorFurnitureCount++;
                        shelfList.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.Trunk:
                        if (indoorFurniture.state != 3)
                            indoorFurnitureCount++;
                        storageBoxList.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.Furniture:
                        decorList.Add(indoorFurniture);
                        break;
                    case (int)kTileGroupType.PetHouse:
                        petHouseList.Add(indoorFurniture);
                        break;

                }

            }
            else
            {
                Debug.LogError("初始家具异常 furnitureId = 0");
            }
        }
        else
        {
            var _furniture = allIndoorEntity[furniture.furnitureUid];
            if ((int)kTileGroupType.ResourceBin == _furniture.type || (int)kTileGroupType.Shelf == _furniture.type || (int)kTileGroupType.Trunk == _furniture.type)
            {
                if (_furniture.state != furniture.state)
                {
                    if (_furniture.state == 3 && furniture.state == 0 && _furniture.type != (int)kTileGroupType.PetHouse)
                        indoorFurnitureCount++;
                    else if (_furniture.state == 0 && furniture.state == 3 && _furniture.type != (int)kTileGroupType.PetHouse)
                        indoorFurnitureCount--;

                    //if (_furniture.state == 0 && furniture.state == 3) //收起来 
                    //{
                    //    furniture.x = 200;
                    //    furniture.y = 200;
                    //}
                }
            }
            //FurnitureConfig fcfg = FurnitureConfigManager.inst.getConfig
            _furniture.SetData(furniture.furnitureId, _furniture.type, furniture.level,
                furniture.x, furniture.y, _furniture.config.width, _furniture.config.height,
                furniture.rotate, furniture.state, furniture.stateStartTime, furniture.stateEndTime, furniture.stateRemainTime);
            _furniture.equipList = furniture.equipList;
        }
    }
}


