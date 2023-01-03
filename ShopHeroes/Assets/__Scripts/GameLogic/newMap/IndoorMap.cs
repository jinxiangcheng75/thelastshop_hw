using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;
using XLua;
//
//室内地图管理
//
public class IndoorMap : SingletonMono<IndoorMap>
{
    public string currUserId = "";
    public static int tempItemUid = -9999;
    public Tilemap gameMapGrid;
    public CameraBounds cameraBounds;
    public MapType currentMapType = MapType.INDOOR; // 室内
    //地图
    public TileBase tileGridItem_W;
    public TileBase tileGridItem_R;
    public Tilemap pathGrid_Tile;       //室内地图室外导航网格
    public Tilemap indoorFlootNode;
    public Tilemap indoorGridLine;

    public Transform cameraStartPos;
    [HideInInspector]
    public bool isInit = false;

    [Header("室内人物创建父级")]
    public Transform actorRoot; //顾客、NPC
    public Transform streetDropRoot; //街道掉落物
    public Transform petRoot; //宠物
    public Transform workerCanLockRoot; //可招募工匠的位置
    public Transform storyRoleRoot;//剧情NPC

    [Header("室内prefab")]
    public GameObject EntityPrefab;
    public GameObject UpGradeAttacher;
    public GameObject PethouseFeedTipsPfb;
    public GameObject shopkeeperPfb;//店主预制体
    public GameObject shopperPfb; //顾客预制体
    public GameObject npcPfb; //npc预制体
    public GameObject passerbyPfb; //街道行人预制体
    public GameObject streetDropPfb;//掉落物预制体
    public GameObject petPfb;//宠物预制体
    public GameObject canLockWorkerPfb; //可招募工匠预制体
    public GameObject storyRolePfb;//礼包、活动 剧情NPC预制体

    public List<Vector3Int> shopperStartPosList;
    public List<Vector3Int> shopperEndPosList;
    public List<Vector3Int> doorNodePosList;

    public RoomFrameProfabs roomFrameProfabs;

    [HideInInspector]
    public Camera mainCamera;

    private int[,] indoorGridFlags; //室内网格标记
    private int[,] indoorfloorClothFlags; //室内地毯层标记
    private int[,] outdoorGridFlags; //室外网格标记
    private int[] wallFurnitureFlage;  //墙面空间标记
    private List<GameObject> indoorPillars = new List<GameObject>();
    //////
    [HideInInspector]
    public Furniture currSelectEntity = null;

    private Furniture creatTempEntity = null;

    public GameObject indoorMask;

    public void SetFurnituresVisible(bool visible)
    {
        if (indoorFurnitures == null) return;

        foreach (var furniture in indoorFurnitures.Values)
        {
            furniture.SetVisible(visible);
        }
    }

    public void HidePethouseFeedTips()
    {
        if (indoorFurnitures == null) return;

        foreach (var furniture in indoorFurnitures.Values)
        {
            furniture.HidePethouseFeedTips();
        }
    }

    public void ShowPethouseFeedTips()
    {
        if (indoorFurnitures == null) return;

        foreach (var furniture in indoorFurnitures.Values)
        {
            furniture.ShowPethouseFeedTips();
        }
    }

    void OnDestroy()
    {
        isInit = false;
    }
    void OnDisable()
    {
        Logger.log("indoormap 被 disable");

    }
    //////
    public void Start()
    {
        mainCamera = Camera.main;
    }
    #region  新的场景加载
    [HideInInspector]
    public Transform carport; //车库
    [HideInInspector]
    public Transform extension; // 扩建区域
    [HideInInspector]
    public Transform door; //门口
    [HideInInspector]
    public List<Transform> zhuziList = new List<Transform>();
    [HideInInspector]
    public bool isSelfe = true;
    [HideInInspector]
    public List<DecorateEntity> decorateList = new List<DecorateEntity>();  //装饰
    private Dictionary<int, Floor> floorGOList;                             //地板 
    private Dictionary<int, WallBase> wallGOList;                           //墙体
    private Dictionary<int, Furniture> indoorFurnitures;                    //室内家具

    private IndoorData mCurrIndoorMapData;
    public List<Furniture> IndoorFunituresList
    {
        get
        {
            if (indoorFurnitures != null)
                return indoorFurnitures.Values.ToList();
            else
                return null;
        }
    }
    public Furniture GetFunitures(int uid)
    {
        if (indoorFurnitures.ContainsKey(uid))
            return indoorFurnitures[uid];
        return null;
    }
    //升级更新地板
    public void ShopUpdataRefreshFloor()
    {
        initIndoorGridLine();   //网格
        updateGridFlags();      //标记
        updateUpgradePop();
    }
    public void RefreshFloor(IndoorData data)
    {
        //初始地板
        CreateFloor(data.size, data.floorList);
    }
    public void RefreshWall(IndoorData data)
    {
        //初始墙面
        CreateWall(data.size, data.wallPaperList);
        //刷新墙面装饰
        foreach (var furniture in indoorFurnitures.Values)
        {
            if (furniture.type == kTileGroupType.WallFurniture)
            {
                furniture.ResetState();
            }
        }
    }
    public void CreateIndoorRoom(string userid, IndoorData currindoordata, bool isvisit)
    {
        if (currindoordata == null || AccountDataProxy.inst == null || GuideDataProxy.inst == null)
        {

            PlatformManager.inst.Restart();
            return;
        }
        mCurrIndoorMapData = currindoordata;
        currUserId = userid;
        isSelfe = AccountDataProxy.inst.userId == userid;
        //尺寸
        RectInt size = currindoordata.size;
        //路径网格
        InitPathGrid();
        if (!isvisit)
        {
            //初始室内网格标记
            initGridFlags();
            //操作线
            initIndoorGridLine();
            if (indoorGridLine != null)
                indoorGridLine.gameObject.SetActive(false);
        }

        //获取柜台的位置
        var guitai = UserDataProxy.inst.GetCounter();
        if (guitai != null)
        {
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.m_curCfg != null && (GuideDataProxy.inst.CurInfo.m_curCfg.id == 0 || GuideDataProxy.inst.CurInfo.m_curCfg.id == 101))
            {
                if (AccountDataProxy.inst.NeedCreatRole)
                {
                    D2DragCamera.inst.maxZoom = FGUI.inst.isLandscape ? 8 : 15;
                    Camera.main.orthographicSize = FGUI.inst.isLandscape ? 8 : 15;
                    float tempX = FGUI.inst.isLandscape ? 3.5f : 8;
                    float tempY = FGUI.inst.isLandscape ? 7.5f : 9.8f;
                    Camera.main.transform.position = new Vector3(tempX, tempY, 0);
                }
                else
                {
                    Camera.main.transform.position = new Vector3(-10, 6.75f, 0);
                }
            }
            else
            {
                if (D2DragCamera.inst != null)
                    D2DragCamera.inst.setCameraPositionAndSaveLastPos(MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(guitai.x, guitai.y, 0))));
            }
        }

        StartCoroutine(AsyncCreate(size, currindoordata));
    }
    bool isFirst = true;
    IEnumerator AsyncCreate(RectInt size, IndoorData currindoordata)
    {
        //预加载
        List<string> reloadlist = new List<string>();
        foreach (var floor in currindoordata.floorList)
        {
            if (reloadlist.IndexOf(floor.config.sprites) < 0)
            {
                reloadlist.Add(floor.config.sprites);
            }
        }
        foreach (var wall in currindoordata.wallPaperList)
        {
            string str1 = wall.config.sprites + "_1";
            string str2 = wall.config.sprites + "_2";
            string str3 = wall.config.sprites + "_3";
            if (reloadlist.IndexOf(str1) < 0)
            {
                reloadlist.Add(str1);
                reloadlist.Add(str2);
                reloadlist.Add(str3);
            }
        }
        AsyncOperationHandle handle = ManagerBinder.inst.Asset.LoadMiscAssetsAsync(reloadlist);
        while (!handle.IsDone)
        {
            yield return null;
        }
        //初始地板
        CreateFloor(size, currindoordata.floorList);
        //
        //初始墙面
        CreateWall(size, currindoordata.wallPaperList);

        //家具
        indoorFurnitures = new Dictionary<int, Furniture>();
        var allFurnitures = UserDataProxy.inst.shopData.allIndoorEntity.Values.ToList();
        for (int i = 0; i < allFurnitures.Count; i++)
        {
            var item = allFurnitures[i];
            if (item.state != 3)
            {
                AddFurnituresToMap(item, false, false, true);
            }
            yield return null;
        }
        if (IndoorEnvironmentObjVisibleClr.inst != null)
        {
            IndoorEnvironmentObjVisibleClr.inst.UpdateObjVisible();
        }
        isInit = true;
        yield return new WaitForSeconds(0.2f);
        isFirst = false;
        FGUI.inst.StartExcessAnimation(false, false);
        FGUI.inst.showGlobalMask(2f);
        yield return new WaitForSeconds(0.5f);
        updateUpgradePop();
        EventController.inst.TriggerEvent(GameEventType.Map2dEvent.IndoorInitEnd, currUserId);
        yield return new WaitForSeconds(0.6f);
        foreach (var f in indoorFurnitures.Values)
        {
            yield return null;
            f.create();
            yield return null;
        }

    }

    void CreateWall(RectInt mapsize, List<IndoorData.ShopDesignItem> walllist)
    {
        //创建墙体
        if (walllist != null)
        {
            if (wallGOList == null)
                wallGOList = new Dictionary<int, WallBase>();
            foreach (var wall in wallGOList.Values)
            {
                if (wall)
                {
                    Destroy(wall.gameObject);
                }
            }
            wallGOList.Clear();
            walllist.ForEach(item =>
            {
                //创建墙体实例
                var newGo = GameObject.Instantiate<GameObject>(item.dir == 0 ? roomFrameProfabs.GetGameProfab("左墙") : roomFrameProfabs.GetGameProfab("右墙"), indoorFlootNode.transform);
                newGo.SetActive(true);
                WallBase f = newGo.GetComponent<WallBase>();
                f.uid = item.uid = UserDataProxy.inst.GetFurnitureUid;
                f.id = item.id;
                f.dir = item.dir;
                f.index = item.index;
                f.type = (kTileGroupType)item.type;
                f.SetCellPosInt(new Vector3Int(item.x, item.y, 0));
                f.create();
                wallGOList.Add(f.uid, f);
            });
        }
        else
        {
            Logger.error("地图数据获取失败");
        }
        //门口
        if (door == null)
        {
            door = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("门口"), indoorFlootNode.transform).transform;
        }
        door.localPosition = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(StaticConstants.indoorGatePoint));
        //柱子
        foreach (var zhuzi in zhuziList)
        {
            if (zhuzi)
            {
                Destroy(zhuzi.gameObject);
            }
        }
        zhuziList.Clear();

        int zjxcount = (mapsize.xMax - mapsize.xMin) / 6;
        int xjycount = (mapsize.yMax - mapsize.yMin) / 6;
        if ((mapsize.xMax - mapsize.xMin) % 6 == 0) zjxcount--;
        if ((mapsize.yMax - mapsize.yMin) % 6 == 0) xjycount--;
        //for (int x = 0; x < zjxcount; x++)
        //{
        // var zhuziGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("右矮柱"), indoorFlootNode.transform);
        // zhuziGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int((x + 1) * 6 + mapsize.xMin, mapsize.yMin, 0)));
        // zhuziList.Add(zhuziGo.transform);
        // //右侧
        // zhuziGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("左中柱"), indoorFlootNode.transform);
        // zhuziGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int((x + 1) * 6 + mapsize.xMin, mapsize.yMax + 1, 0)));
        // zhuziList.Add(zhuziGo.transform);
        //}
        // for (int y = 0; y < xjycount; y++)
        // {
        //     var zhuziGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("右中柱"), indoorFlootNode.transform);
        //     zhuziGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMax + 1, (y + 1) * 6 + mapsize.yMin, 0)));
        //     zhuziList.Add(zhuziGo.transform);
        // }
        //左下
        var NewGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("左柱子"), indoorFlootNode.transform);
        NewGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMin, mapsize.yMax + 1, 0)));
        zhuziList.Add(NewGo.transform);
        //左上
        NewGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("中间柱"), indoorFlootNode.transform);
        NewGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMax + 1, mapsize.yMax + 1, 0)));
        zhuziList.Add(NewGo.transform);
        //右下
        NewGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("前柱"), indoorFlootNode.transform);
        NewGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMin, mapsize.yMin, 0)));
        zhuziList.Add(NewGo.transform);
        SpriteRenderer _renderer = NewGo.GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.sortingOrder = MapUtils.GetTileMapOrder(NewGo.transform.position.y - 0.5f, NewGo.transform.position.x - 0.5f, 1, 1);
        }
        //右上
        NewGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("右柱子"), indoorFlootNode.transform);
        NewGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMax + 1, mapsize.yMin, 0)));
        zhuziList.Add(NewGo.transform);
        //右墙基
        int index = 0;
        for (int x = mapsize.xMin; x < mapsize.xMax; x += 2)
        {
            NewGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("右墙基"), indoorFlootNode.transform);
            NewGo.transform.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(x, mapsize.yMin, 0)));
            SpriteEX renderEx = NewGo.GetComponent<SpriteEX>();
            if (renderEx != null)
            {
                string spriteurl = "tjx_0" + (index % 3 + 1);
                ManagerBinder.inst.Asset.getSpriteAsync(spriteurl, (gsprite) =>
                {
                    renderEx.mGSprite = gsprite;
                });
                //renderEx.spriteUrl = spriteurl;
            }
            SpriteRenderer renderer = NewGo.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = MapUtils.GetTileMapOrder(NewGo.transform.position.y - 0.125f, NewGo.transform.position.x - 0.5f, 1, 2);
            }
            zhuziList.Add(NewGo.transform);
            index++;
        }
        //左墙基
        index = 0;
        for (int y = mapsize.yMin; y < mapsize.yMax; y += 2)
        {
            if (y < 20 || y > 24)
            {
                var pos = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(0, y, 0)));

                NewGo = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("左墙基"), indoorFlootNode.transform);
                NewGo.transform.position = pos;
                SpriteEX renderEx = NewGo.GetComponent<SpriteEX>();
                if (renderEx != null)
                {
                    string spriteurl = "tjx_0" + (index % 3 + 1);
                    ManagerBinder.inst.Asset.getSpriteAsync(spriteurl, (gsprite) =>
                    {
                        renderEx.mGSprite = gsprite;
                    });
                    //renderEx.spriteUrl = spriteurl;
                }
                SpriteRenderer renderer = NewGo.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = MapUtils.GetTileMapOrder(pos.y - 0.15f, pos.x - 0.25f, 1, 2);
                }
                zhuziList.Add(NewGo.transform);
                index++;
            }
        }
    }
    //创建地板砖
    void CreateFloor(RectInt mapsize, List<IndoorData.ShopDesignItem> floorlist)
    {
        //创建车库
        if (carport == null)
        {
            carport = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("车库"), indoorFlootNode.transform).transform;
        }
        carport.localPosition = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMin, mapsize.yMax + 1, 0)));

        if (floorGOList != null)
        {
            foreach (Floor floor in floorGOList.Values)
            {
                GameObject.Destroy(floor.gameObject);
            }
            floorGOList.Clear();
        }
        //创建扩建区
        if (UserDataProxy.inst.shopData.shopLevel < StaticConstants.shopMap_MaxLevel)
        {
            if (extension == null)
            {
                extension = GameObject.Instantiate(roomFrameProfabs.GetGameProfab("建造"), indoorFlootNode.transform).transform;
            }
            extension.position = MapUtils.CellPosToWorldPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(mapsize.xMin, mapsize.yMin, 0)));
            BuildingSite aa = extension.gameObject.GetComponent<BuildingSite>();
            if (aa != null)
            {
                aa.setSize(UserDataProxy.inst.shopData.size.xMax);
            }
        }
        //初始化地板
        if (floorlist != null)
        {
            if (floorGOList == null)
                floorGOList = new Dictionary<int, Floor>();
            floorlist.ForEach(item =>
            {
                //创建地板实例
                GameObject newGo;
                var index = item.index;
                if (floorGOList.ContainsKey(index))
                {
                    Floor f = floorGOList[index];
                    f.SetUpState(item.state);
                }
                else
                {
                    newGo = GameObject.Instantiate<GameObject>(roomFrameProfabs.GetGameProfab("地板"), indoorFlootNode.transform);
                    newGo.SetActive(true);
                    Floor f = newGo.GetComponent<Floor>();
                    f.uid = item.uid;
                    f.id = item.id;
                    f.dir = item.dir;
                    f.type = (kTileGroupType)item.type;
                    f.SetCellPosInt(new Vector3Int(item.x, item.y, 0));
                    f.create();
                    f.SetUpState(item.state);
                    floorGOList.Add(index, f);
                }
            });
        }
        else
        {
            Logger.error("地图数据获取失败");
        }
    }
    #endregion 新的场景加载
    public void ClearNewFurniture()
    {
        if (creatTempEntity != null)
        {
            creatTempEntity.RemoveSelf();
            creatTempEntity = null;
        }
    }
    //初始化家具
    private void initFurniture(List<IndoorData.ShopDesignItem> allFurnitures)
    {
        indoorFurnitures = new Dictionary<int, Furniture>();

        allFurnitures.ForEach(item =>
        {
            if (item.state != 3)
            {
                AddFurnituresToMap(item, false, false, true);
            }
        });
    }

    //添加已有家具到场景
    public void AddFurnituresToMap(IndoorData.ShopDesignItem item, bool select = false, bool isCreatFurniture = true, bool isInit = false)
    {
        bool indoor = item.type != (int)kTileGroupType.OutdoorFurniture;
        Vector3Int position = Vector3Int.zero;
        if (select)
        {
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
            {
                string[] pos = GuideDataProxy.inst.CurInfo.m_curCfg.conditon_param_4.Split('|');
                position = new Vector3Int(int.Parse(pos[0]) - StaticConstants.IndoorOffsetX, int.Parse(pos[1]) - StaticConstants.IndoorOffsetY, 0);
            }
            else
            {
                position = FindFreeCell(item.sizeX, item.sizeY, indoor);
            }
        }
        else
        {
            position.x = item.x;
            position.y = item.y;
        }
        if (!indoorFurnitures.ContainsKey(item.uid))
        {
            if (isInit && !indoor)
            {
                if (position.x < 0 || position.x > StaticConstants.OutDoorMaxX || position.y < 0 || position.y > StaticConstants.OutDoorMaxY)
                {
                    IndoorMapEditSys.inst.storeItem(item.uid,item.type);
                    return;
                }
            }

            var furnitureGo = NewFurnitureGO(item.uid, item.id, item.dir, item.type, position.x, position.y, isCreatFurniture);
            if (isInit)
            {
                furnitureGo.SetFurnitureId(item.id);
            }
            else
            {
                furnitureGo.create();
            }
            indoorFurnitures.Add(item.uid, furnitureGo);

        }
        if (select)
        {
            IndoorMap.inst.OnFurnituresSelect(item.uid);
            if (D2DragCamera.inst != null)
            {
                D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(indoor ? MapUtils.IndoorCellposToMapCellPos(new Vector3Int(position.x, position.y, 0)) : new Vector3Int(position.x, position.y, 0)), true, D2DragCamera.inst.camera.orthographicSize);
            }
        }
    }

    //添加新的家具到场景
    public void AddNewFurnituresToMap(int id, bool select = false)
    {

        FurnitureConfig cfg = FurnitureConfigManager.inst.getConfig(id);
        if (cfg != null)
        {
            // var pos = gameMapGrid.WorldToCell(Camera.main.transform.position);
            // if (cfg.type_1 != 10)
            // {
            //     pos.x -= StaticConstants.IndoorOffsetX;
            //     pos.y -= StaticConstants.IndoorOffsetY;
            // }
            bool indoor = cfg.type_1 != (int)kTileGroupType.OutdoorFurniture;
            Vector3Int position = Vector3Int.zero;
            int dir = 0;
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
            {
                if (GuideDataProxy.inst.CurInfo.m_curCfg.conditon_param_4 != null)
                {
                    string[] pos = GuideDataProxy.inst.CurInfo.m_curCfg.conditon_param_4.Split('|');
                    if (pos != null)
                    {
                        if (pos.Length >= 2)
                            position = new Vector3Int(int.Parse(pos[0]) - StaticConstants.IndoorOffsetX, int.Parse(pos[1]) - StaticConstants.IndoorOffsetY, 0);
                        if (pos.Length >= 3)
                            dir = int.Parse(pos[2]);
                    }
                }
            }
            else
            {
                string offPos = GuideManager.inst.triggerFurnOffset;
                if (GuideManager.inst.isInTriggerGuide && offPos != null)
                {
                    string[] pos = offPos.Split('|');
                    if (pos != null)
                    {
                        if (pos.Length >= 2)
                            position = new Vector3Int(int.Parse(pos[0]) - StaticConstants.IndoorOffsetX, int.Parse(pos[1]) - StaticConstants.IndoorOffsetY, 0);
                        if (pos.Length >= 3)
                            dir = int.Parse(pos[2]);

                        if (!IndoorMap.inst.CheckFurnituresOccGrid(cfg, position.x, position.y, dir))
                        {
                            position = FindFreeCell(cfg.width, cfg.height, indoor);
                            dir = 0;
                        }
                    }
                    GuideManager.inst.triggerFurnOffset = null;
                }
                else
                {
                    position = FindFreeCell(cfg.width, cfg.height, indoor);
                }
            }
            var furnitureGo = NewFurnitureGO(tempItemUid, id, dir, cfg.type_1, position.x, position.y);
            furnitureGo.create();
            indoorFurnitures.Add(tempItemUid, furnitureGo);
            if (select)
            {
                IndoorMap.inst.OnFurnituresSelect(tempItemUid);
            }
            D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(indoor ? MapUtils.IndoorCellposToMapCellPos(position) : position), false, D2DragCamera.inst.camera.orthographicSize);
        }
    }
    public Vector3Int _FindFreeCell(Vector3Int targetPos, int sizeX, int sizeY, bool indoor, bool ignoreRole = false)
    {
        var pos = targetPos;
        var size = UserDataProxy.inst.GetIndoorSize();
        if (!indoor)
        {
            size = new RectInt(0, 0, StaticConstants.IndoorOffsetX - 1, StaticConstants.IndoorMaxY);
        }
        Vector3Int newpos = pos;
        newpos.x = Mathf.Min(size.xMax, Mathf.Max(size.xMin, pos.x));
        newpos.y = Mathf.Min(size.yMax, Mathf.Max(size.yMin, pos.y));

        ///先排除最下和最右一行一列

        int index = 0;
        while (index <= 40)
        {

            //右->左
            for (int y = -index; y <= index; y++)
            {
                if (newpos.y + y > size.yMax || newpos.y + y <= size.yMin)
                {
                    continue;
                }

                pos.y = newpos.y + y;

                //下->上
                for (int x = -index; x <= index; x++)
                {
                    if (newpos.x + x > size.xMax || newpos.x + x <= size.xMin)
                    {
                        continue;
                    }

                    pos.x = newpos.x + x;


                    if (x == -index || x == index)
                    {
                        if (check(pos.x, pos.y, sizeX, sizeY, indoor, ignoreRole))
                        {
                            return pos;
                        }
                    }
                    else if (y == -index || y == index)
                    {
                        if (check(pos.x, pos.y, sizeX, sizeY, indoor, ignoreRole))
                        {
                            return pos;
                        }
                    }

                }

            }

            index++;
        }



        int yIndex = size.yMax - size.yMin;
        int xIndex = size.xMax - size.xMin;

        ///从最下和最右一行一列筛选
        //右->左
        for (int y = -yIndex; y <= yIndex; y++)
        {
            if (newpos.y + y != size.yMin)
            {
                continue;
            }

            pos.y = newpos.y + y;

            //下->上
            for (int x = -xIndex; x <= xIndex; x++)
            {
                if (newpos.x + x != size.xMin)
                {
                    continue;
                }

                pos.x = newpos.x + x;

                if (check(pos.x, pos.y, sizeX, sizeY, indoor, ignoreRole))
                {
                    return pos;
                }
            }

        }


        //int index = 0;
        //while (index <= 40)
        //{
        //    pos = newpos;

        //    //右->左
        //    for (int y = -index; y <= index; y++)
        //    {
        //        if (newpos.y + y >= size.yMax || newpos.y + y < size.yMin)
        //        {
        //            continue;
        //        }

        //        pos.y = newpos.y + y;

        //        //下->上
        //        for (int x = -index; x <= index; x++)
        //        {
        //            if (newpos.x + x >= size.xMax || newpos.x + x < size.xMin)
        //            {
        //                continue;
        //            }
        //            pos.x = newpos.x + x;
        //            if (x == -index || x == index)
        //            {
        //                if (check(pos.x, pos.y, sizeX, sizeY, indoor))
        //                {
        //                    return pos;
        //                }
        //            }
        //            else if (y == -index || y == index)
        //            {
        //                if (check(pos.x, pos.y, sizeX, sizeY, indoor))
        //                {
        //                    return pos;
        //                }
        //            }
        //        }
        //    }
        //    index++;
        //}
        return newpos;
    }
    public Vector3Int FindFreeCell(int sizeX, int sizeY, bool indoor)
    {
        Vector3Int pos = gameMapGrid.WorldToCell(Camera.main.transform.position);
        pos.x -= StaticConstants.IndoorOffsetX;
        pos.y -= StaticConstants.IndoorOffsetY;
        return _FindFreeCell(pos, sizeX, sizeY, indoor, true);
    }

    public int GetIndoorGridFlags(int x, int y)
    {
        if (indoorGridFlags == null) return -1;
        var size = UserDataProxy.inst.GetIndoorSize();
        if (x < size.xMin || x > size.xMax || y < size.yMin || y > size.yMax)
            return -1;
        return indoorGridFlags[x, y];
    }
    private bool check(int minx, int miny, int sizeX, int sizeY, bool indoor, bool ignoreRole = true)
    {
        if (indoor)
        {
            var size = UserDataProxy.inst.GetIndoorSize();
            for (int x = minx; x < minx + sizeX; x++)
                for (int y = miny; y < miny + sizeY; y++)
                {
                    if (x < size.xMin || x > size.xMax || y < size.yMin || y > size.yMax)
                        return false;
                    if (ignoreRole)
                    {
                        if (indoorGridFlags[x, y] != 0 && indoorGridFlags[x, y] != 99999999 && indoorGridFlags[x, y] != 1415926)/*顾客 和 店主*/
                            return false;
                    }
                    else if (indoorGridFlags[x, y] != 0)//&& indoorGridFlags[x, y] != 99999999
                    {
                        return false;
                    }
                }
        }
        else
        {
            for (int x = minx; x < minx + sizeX; x++)
                for (int y = miny; y < miny + sizeY; y++)
                {
                    if (x < 0 || x >= outdoorGridFlags.GetLength(0) || y < 0 || y >= outdoorGridFlags.GetLength(1)) return false;
                    if (outdoorGridFlags[x, y] != 0)
                    {
                        return false;
                    }
                }
        }
        return true;
    }
    //删除家具从场景
    public void RemoveFurnituresOnMap(int uid)
    {
        if (indoorFurnitures.ContainsKey(uid))
        {
            if (currSelectEntity != null && currSelectEntity.uid == uid)
            {
                currSelectEntity = null;
            }

            indoorFurnitures[uid].RemoveSelf();
            indoorFurnitures.Remove(uid);
        }
    }

    public void RemoveFurnitureDataNotPrefab(int uid)
    {
        if (indoorFurnitures.ContainsKey(uid))
        {
            //    indoorFurnitures[uid].RemoveSelf();
            indoorFurnitures.Remove(uid);
        }
    }

    public Furniture NewFurnitureGO(int uid, int id, int dir, int type, int x, int y, bool isCreatFurniture = true)
    {
        var counterGo = GameObject.Instantiate<GameObject>(EntityPrefab, indoorFlootNode.transform);
        counterGo.SetActive(true);
        Furniture f = counterGo.GetComponent<Furniture>();
        f.uid = uid;
        f.id = id;
        f.dir = dir;
        f.type = (kTileGroupType)type;
        f.isIndoor = type != (int)kTileGroupType.OutdoorFurniture;
        f.SetCellPosInt(new Vector3Int(x, y, 0));
        f.isCreatFurniture = isCreatFurniture;
        // f.create();
        return f;
    }
    //初始化室内网格
    private void initIndoorGridLine()
    {
        //室内尺寸
        var size = UserDataProxy.inst.GetIndoorSize();
        if (indoorGridLine != null)
        {
            for (int x = size.xMin; x <= size.xMax; x++)
            {
                for (int y = size.yMin; y <= size.yMax; y++)
                {
                    indoorGridLine.SetTile(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(x, y, 0)), tileGridItem_W);
                }
            }

            for (int x = 0; x < StaticConstants.IndoorOffsetX - 1; x++)
            {
                for (int y = 0; y < StaticConstants.IndoorMaxY; y++)
                {
                    indoorGridLine.SetTile(new Vector3Int(x, y, 0), outdoorGridFlags[x, y] == 0 ? tileGridItem_W : null);
                }
            }
        }
    }
    // 设置障碍
    public void setOccGrid(bool indoor, int x, int y, int type)
    {
        if (type == 99999999)
            type = 0;
        if (indoor)
        {
            indoorGridLine.SetTile(new Vector3Int(x + StaticConstants.IndoorOffsetX, y + StaticConstants.IndoorOffsetY, 0), type > 0 ? tileGridItem_R : tileGridItem_W);
        }
        else
        {
            indoorGridLine.SetTile(new Vector3Int(x, y, 0), type > 0 ? tileGridItem_R : tileGridItem_W);
        }
    }

    private void ClearfloorObj()
    {
        foreach (var obj in floorGOList.Values)
        {
            GameObject.Destroy(obj);
        }
        floorGOList.Clear();
        foreach (var obj in wallGOList.Values)
        {
            GameObject.Destroy(obj);
        }
        wallGOList.Clear();
    }
    private void updateGridFlags()
    {
        RectInt size = UserDataProxy.inst.GetIndoorSize();
        for (int x = 0; x < StaticConstants.IndoorMaxX; x++)
        {
            for (int y = 0; y < StaticConstants.IndoorMaxY; y++)
            {
                if (x < size.xMin || x > size.xMax || y < size.yMin || y > size.yMax)
                {
                    indoorGridFlags[x, y] = -1;
                    indoorfloorClothFlags[x, y] = -1;
                    continue;
                }
                if (indoorGridFlags[x, y] == -1)
                {
                    indoorGridFlags[x, y] = 0;
                    indoorfloorClothFlags[x, y] = 0;
                }
                else
                {
                    setOccGrid(true, x, y, indoorGridFlags[x, y]);
                }
            }
        }
        //wallFurnitureFlage
        for (int x = 0; x < StaticConstants.IndoorMaxX; x++)
        {
            if (x < size.xMin || x > size.xMax)
            {
                wallFurnitureFlage[x] = -1;
            }
            else
            {
                if (wallFurnitureFlage[x] == -1)
                    wallFurnitureFlage[x] = 0;
            }
        }
        for (int y = 0; y < StaticConstants.IndoorMaxY; y++)
        {
            if (y < size.yMin || y > size.yMax)
            {
                wallFurnitureFlage[StaticConstants.IndoorMaxX + y] = -1;
            }
            else
            {
                if (wallFurnitureFlage[StaticConstants.IndoorMaxX + y] == -1)
                    wallFurnitureFlage[StaticConstants.IndoorMaxX + y] = 0;
            }
        }
    }

    #region 交互操作
    public bool CanChangeSelectEntity()
    {
        if (currSelectEntity != null)
        {
            //if (currSelectEntity.uid == tempItemUid) return false;
            var F = UserDataProxy.inst.GetFuriture(currSelectEntity.uid);
            if (F == null || F.state == 3) return false;
        }
        return true;
    }
    public void MapGridLineVisible(bool visible)
    {
        indoorGridLine.gameObject.SetActive(visible);
    }

    //操作
    //除了墙体地砖其他物品 选中事件
    public bool GetFurnituresByUid(int uid, out Furniture furniture)
    {
        if (indoorFurnitures == null)
        {
            furniture = null;
            return false;
        }
        return indoorFurnitures.TryGetValue(uid, out furniture);
    }

    public void OnFurnituresSelect(int uid)
    {
        if (currSelectEntity != null)
        {
            if (currSelectEntity.uid == uid) return;
            currSelectEntity.UnSelected();
        }
        GetFurnituresByUid(uid, out currSelectEntity);
    }

    public void OnFurnituresUnSelect()
    {
        if (currSelectEntity != null)
        {
            var F = UserDataProxy.inst.GetFuriture(currSelectEntity.uid);
            if (F == null || F.state == 3)
            {
                //RemoveFurnituresOnMap(currSelectEntity.uid);
                if (IndoorMapEditSys.inst.shopDesignMode != DesignMode.modeSelection)
                {
                    RemoveFurnituresOnMap(currSelectEntity.uid);
                }
                else
                {
                    RemoveFurnitureDataNotPrefab(currSelectEntity.uid);
                    currSelectEntity.UnSelected();
                    creatTempEntity = currSelectEntity;
                }
            }
            else
            {
                currSelectEntity.UnSelected();
            }
            currSelectEntity = null;
        }
    }

    public void UpdateFurniture(int furitureuid, bool change)
    {
        IndoorData.ShopDesignItem item = UserDataProxy.inst.GetFuriture(furitureuid);
        Furniture f;

        if (indoorFurnitures == null) return;

        indoorFurnitures.TryGetValue(furitureuid, out f);
        if (f == null && item.state != (int)EDesignState.InStore)
        {
            //没有则新建
            if (creatTempEntity != null)
            {
                Furniture tempFurn = creatTempEntity;
                tempFurn.uid = item.uid;
                tempFurn.id = item.id;
                tempFurn.dir = item.dir;
                tempFurn.type = (kTileGroupType)item.type;
                tempFurn.isIndoor = item.type != (int)kTileGroupType.OutdoorFurniture;
                tempFurn.SetCellPosInt(new Vector3Int(item.x, item.y, 0));
                tempFurn.ResetShelfData();
                tempFurn.ReSetPos();
                indoorFurnitures.Add(item.uid, tempFurn);
                creatTempEntity = null;
            }
            else
                AddFurnituresToMap(item, false, false);
        }
        else
        {
            if (item.state != (int)EDesignState.InStore)
                f.updateData(item, change);
            else
                RemoveFurnituresOnMap(furitureuid);
        }
    }
    //拾起物品(拾起物品为当前选中物品)OnSelected()
    public bool isFurnitureOccupyDrag
    {
        get
        {
            if (currSelectEntity != null)
            {
                return currSelectEntity.OccupyDrag;
            }
            return false;
        }
    }
    #endregion
    //
    public bool furnitureHandleResults = true;   //操作结果是否有效
                                                 //改变家具位置
    public void ChangeFurniturePosint(int uid, Vector3Int cellposint, int dir)
    {

        if (currSelectEntity == null || currSelectEntity.uid != uid) return;
        //判断位置是否合法
        var f = indoorFurnitures[uid];
        FurnitureConfig fcfg = FurnitureConfigManager.inst.getConfig(f.id);
        furnitureHandleResults = CheckFurnituresOccGrid(fcfg, cellposint.x, cellposint.y, dir);
        currSelectEntity.changeState(furnitureHandleResults);
        //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_CANAPPLY, furnitureHandleResults);
        HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Furniture_CANAPPLY", furnitureHandleResults);
    }
    //改变当前家具朝向
    public void ChangeFurnitureDir()
    {
        if (currSelectEntity != null)
        {
            var dir = currSelectEntity.Rotate();
            var f = indoorFurnitures[currSelectEntity.uid];
            FurnitureConfig fcfg = FurnitureConfigManager.inst.getConfig(f.id);
            furnitureHandleResults = CheckFurnituresOccGrid(fcfg, currSelectEntity.cellpos.x, currSelectEntity.cellpos.y, currSelectEntity.dir);
            currSelectEntity.isPickUp = !furnitureHandleResults ? true : false;
            currSelectEntity.changeState(furnitureHandleResults);
            if (currSelectEntity.uid == tempItemUid)
            {
                //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_CANAPPLY, furnitureHandleResults);
                HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Furniture_CANAPPLY", furnitureHandleResults);
            }
            else
            {
                //currSelectEntity.isPickUp = false;
                if (furnitureHandleResults)
                {
                    var F = UserDataProxy.inst.GetFuriture(currSelectEntity.uid);
                    if (F != null && F.state != 3)
                        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Move_Rotate, currSelectEntity.uid, currSelectEntity.cellpos.x, currSelectEntity.cellpos.y, currSelectEntity.dir);
                }
                //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_CANAPPLY, furnitureHandleResults);
                HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Furniture_CANAPPLY", furnitureHandleResults);
            }

        }
    }
    public void saveCurrentFurniture()
    {
        if (currSelectEntity != null)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Move_Rotate, currSelectEntity.uid, currSelectEntity.cellpos.x, currSelectEntity.cellpos.y, currSelectEntity.dir);

        }
    }
    //初始化网格标记网格
    private void initGridFlags()
    {
        indoorGridFlags = new int[StaticConstants.IndoorMaxX, StaticConstants.IndoorMaxY];
        indoorfloorClothFlags = new int[StaticConstants.IndoorMaxX, StaticConstants.IndoorMaxY];
        wallFurnitureFlage = new int[StaticConstants.IndoorMaxX + StaticConstants.IndoorMaxY];
        // RectInt size = UserDataProxy.inst.GetIndoorSize();
        // for (int x = 0; x < StaticConstants.IndoorMaxX; x++)
        // {
        //     for (int y = 0; y < StaticConstants.IndoorMaxY; y++)
        //     {
        //         indoorGridFlags[x, y] = -1;
        //         indoorfloorClothFlags[x, y] = -1;
        //         if (x < size.xMin || x > size.xMax || y < size.yMin || y > size.yMax) continue;
        //         indoorGridFlags[x, y] = 0;
        //         indoorfloorClothFlags[x, y] = 0;
        //     }
        // }
        updateGridFlags();

        outdoorGridFlags = new int[StaticConstants.IndoorOffsetX, StaticConstants.IndoorMaxY];
        for (int x = 0; x < StaticConstants.IndoorOffsetX; x++)
        {
            for (int y = 0; y < StaticConstants.IndoorMaxY; y++)
            {
                outdoorGridFlags[x, y] = IndoorGridMapClr.inst.outdoorGrid[x, y];
            }
        }
        UpdateAllPathObstacle();
    }

    public bool IsObstacleGrid(int indoorx, int indoory)
    {
        RectInt size = UserDataProxy.inst.GetIndoorSize();
        if (indoorx < 0 || indoorx >= size.xMax || indoory < 0 || indoory >= size.yMax)
            return true;
        return indoorGridFlags[indoorx, indoory] != 0;
    }

    public Vector3Int GetFreeGridPos()
    {
        RectInt size = UserDataProxy.inst.GetIndoorSize();
        int randomX = 0, randomY = 0;
        while (true)
        {
            randomX = Random.Range(size.xMin, size.xMax);
            randomY = Random.Range(size.yMin, size.yMax);
            if (indoorGridFlags[randomX, randomY] == 0)
                break;
        }
        return new Vector3Int(randomX, randomY, 0);
    }

    //检测是否被占用
    public bool CheckFurnituresOccGrid(FurnitureConfig fcfg, int cellx, int celly, int dir)
    {
        if (fcfg == null) return true;
        //if (fcfg.type_1 == (int)kTileGroupType.WallFurniture) return true;
        int maxx;
        int maxy;
        if (dir == 0)
        {
            maxx = fcfg.width;
            maxy = fcfg.height;
        }
        else
        {
            maxx = fcfg.height;
            maxy = fcfg.width;
        }
        RectInt size = UserDataProxy.inst.GetIndoorSize();

        if (fcfg.type_1 == (int)kTileGroupType.WallFurniture)
        {
            //墙饰
            if (dir == 0)
            {
                for (int x = cellx; x < cellx + maxx; x++)
                {
                    if (x < size.xMin || x > size.xMax) return false;
                    // wallFurnitureFlage[x] = flags;
                    if (wallFurnitureFlage[x] != 0)
                    {
                        return false;
                    }
                }
            }
            else
            {
                int index = StaticConstants.IndoorMaxX;
                for (int y = celly; y < celly + maxy; y++)
                {
                    if (y < size.yMin || y > size.yMax) return false;
                    if (wallFurnitureFlage[index + y] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        if (fcfg.type_1 != (int)kTileGroupType.OutdoorFurniture)
        {

            for (int x = cellx; x < cellx + maxx; x++)
                for (int y = celly; y < celly + maxy; y++)
                {
                    if (x < size.xMin || x > size.xMax || y < size.yMin || y > size.yMax) return false;

                    if (fcfg.type_1 == (int)kTileGroupType.Carpet)
                    {
                        if (indoorfloorClothFlags[x, y] != 0) return false;
                    }
                    else
                    {
                        if (indoorGridFlags[x, y] != 0 && indoorGridFlags[x, y] != 99999999 && indoorGridFlags[x, y] != 1415926)
                        {
                            return false;
                        }
                    }
                }
        }
        else
        {
            //室外
            for (int x = cellx; x < cellx + maxx; x++)
                for (int y = celly; y < celly + maxy; y++)
                {
                    if (x < 0 || x >= StaticConstants.OutDoorMaxX || y < 0 || y >= StaticConstants.OutDoorMaxY) return false;
                    if (outdoorGridFlags[x, y] != 0) return false;
                }
        }
        return true;
    }
    public void SetIndoorGridFlags(int uid, int cellx, int celly, int dir, int flags)
    {
        IndoorData.ShopDesignItem grid = UserDataProxy.inst.GetFuriture(uid);
        if (grid == null) return;

        int maxx;
        int maxy;
        if (dir == 0)
        {
            maxx = grid.sizeX;
            maxy = grid.sizeY;
        }
        else
        {
            maxx = grid.sizeY;
            maxy = grid.sizeX;
        }
        if (grid.type == (int)kTileGroupType.WallFurniture)
        {
            //墙饰
            if (dir == 0)
            {
                for (int x = grid.x; x < grid.x + maxx; x++)
                {
                    if (x >= wallFurnitureFlage.Length) continue;
                    wallFurnitureFlage[x] = flags;
                }
            }
            else
            {
                int index = StaticConstants.IndoorMaxX;
                for (int y = grid.y; y < grid.y + maxy; y++)
                {
                    if (index + y >= wallFurnitureFlage.Length) continue;
                    wallFurnitureFlage[index + y] = flags;
                }
            }
            return;
        }
        bool isoutdoor = grid.type == (int)kTileGroupType.OutdoorFurniture;
        for (int x = grid.x; x < grid.x + maxx; x++)
        {
            for (int y = grid.y; y < grid.y + maxy; y++)
            {
                if (flags == 1)
                {
                    SetGridFlags(!isoutdoor, x, y, grid.type, grid.type);
                    setOccGrid(!isoutdoor, x, y, grid.type);
                }
                else
                {
                    IndoorMap.inst.setOccGrid(!isoutdoor, x, y, 0);
                    SetGridFlags(!isoutdoor, x, y, grid.type, 0);
                }
            }
        }
        UpdateAllPathObstacle();
    }
    public void SetGridFlags(bool indoor, int x, int y, int type, int flags)
    {
        if (indoor)
        {
            RectInt size = UserDataProxy.inst.GetIndoorSize();
            if (x < size.xMin || x > size.xMax || y < size.yMin || y > size.yMax) return;
            if (type != 3)
            {
                indoorGridFlags[x, y] = flags;
            }
            else
            {
                indoorfloorClothFlags[x, y] = flags;
            }
        }
        else
        {
            if (x < 0 || x >= StaticConstants.OutDoorMaxX || y < 0 || y >= StaticConstants.OutDoorMaxY) return;
            outdoorGridFlags[x, y] = flags;
        }
        UpdateAllPathObstacle();
    }
    #region 寻路
    private PathNode[,] indoorPathGrid;   //场景寻路地图
    private PathNode[,] indoorPathGrid_barrierFree;//场景寻路地图（完全无障碍）

    private RectInt pathMapSize = new RectInt(-5, -10, 40, 50);  //寻路网格size
    private Vector3Int pathOffset = new Vector3Int(-5, -10, 0);
    //初始化寻路网格
    void InitPathGrid()
    {
        indoorPathGrid = new PathNode[pathMapSize.width - pathOffset.x, pathMapSize.height - pathOffset.y];
        indoorPathGrid_barrierFree = new PathNode[pathMapSize.width - pathOffset.x, pathMapSize.height - pathOffset.y];
        if (pathGrid_Tile == null) return;
        Vector3Int currCellPos;
        for (int x = 0; x < pathMapSize.width - pathOffset.x; x++)
            for (int y = 0; y < pathMapSize.height - pathOffset.y; y++)
            {
                currCellPos = new Vector3Int(x, y, 0) + pathOffset;
                bool obstacle = !pathGrid_Tile.HasTile(currCellPos);
                indoorPathGrid[x, y] = new PathNode(x, y, x + pathOffset.x, y + pathOffset.y, obstacle);
                indoorPathGrid_barrierFree[x, y] = new PathNode(x, y, x + pathOffset.x, y + pathOffset.y, obstacle);
            }
    }
    void UpdateAllPathObstacle()
    {
        int _posx, _posy;
        for (int x = 0; x < pathMapSize.width - pathMapSize.x; x++)
        {
            for (int y = 0; y < pathMapSize.height - pathMapSize.y; y++)
            {
                _posx = x + pathMapSize.x - StaticConstants.IndoorOffsetX;
                _posy = y + pathMapSize.y - StaticConstants.IndoorOffsetY;
                if (_posx >= 0 && _posy >= 0 && _posx < StaticConstants.IndoorMaxX && _posy < StaticConstants.IndoorMaxY)
                {
                    if (indoorGridFlags[_posx, _posy] == 0 || indoorGridFlags[_posx, _posy] == (int)kTileGroupType.Carpet || indoorGridFlags[_posx, _posy] == 99999999 || indoorGridFlags[_posx, _posy] == 1415926/*店主站位*/)
                    {
                        indoorPathGrid[x, y].setState(false);
                        // pathGrid_Tile.SetTile(new Vector3Int(_posx + StaticConstants.IndoorOffsetX, _posy, 0), tileGridItem_R);
                    }
                    else
                    {
                        indoorPathGrid[x, y].setState(true);
                        //pathGrid_Tile.SetTile(new Vector3Int(_posx + StaticConstants.IndoorOffsetX, _posy, 0), tileGridItem_W);
                    }
                }
            }
        }
    }

    public Stack<PathNode> FindPath(Vector3Int start, Vector3Int end)
    {
        return SimpleAStart.inst.FindPath(indoorPathGrid, start.x - pathOffset.x, start.y - pathOffset.y, end.x - pathOffset.x, end.y - pathOffset.y);
    }

    //无障碍物的寻路
    public Stack<PathNode> FindPathBarrierFree(Vector3Int start, Vector3Int end)
    {
        return SimpleAStart.inst.FindPath(indoorPathGrid_barrierFree, start.x - pathOffset.x, start.y - pathOffset.y, end.x - pathOffset.x, end.y - pathOffset.y);
    }



    #endregion
    #region IndoorRoleActor

    //获取柜台店主位置
    public Vector3Int GetCounterOperationPos()
    {
        //if(indoorFurnitures)
        var counterdata = UserDataProxy.inst.GetCounter();
        var counter = indoorFurnitures[counterdata.uid];
        var posList = counter.GetShopKeeperPos();
        if (posList.Count > 0)
        {
            return posList[0];
        }
        else
        {
            return new Vector3Int(99999, 99999, 99999);
        }
    }

    //获得柜台周边可以站的位置
    public Vector3Int GetCounterFrontPos()
    {
        var counterdata = UserDataProxy.inst.GetCounter();
        var counter = indoorFurnitures[counterdata.uid];
        List<Vector3Int> pos = counter.GetFreePos();
        var count = pos.Count;
        Vector3Int target = Vector3Int.zero;
        while (count > 0)
        {
            target = pos.GetRandomElement<Vector3Int>();
            if (target != null) break;
        }
        return target;
    }

    public Vector3Int GetFurnitureFrontPos(int uid)
    {
        if (!indoorFurnitures.ContainsKey(uid))
        {
            Logger.log("GetFurnitureFrontPos 没有家具实例 error:uid=" + uid);
            return Vector3Int.zero;
        }
        var counter = indoorFurnitures[uid];
        List<Vector3Int> pos = counter.GetFreePos();
        var count = pos.Count;
        Vector3Int target = Vector3Int.zero;
        while (count > 0)
        {
            target = pos.GetRandomElement<Vector3Int>();
            if (target != null) break;
        }
        return target;
    }
    //获取出生点
    int startposindex = 0;
    public Vector3Int GetStartPoint()
    {
        startposindex++;
        startposindex = startposindex >= shopperStartPosList.Count ? 0 : startposindex;
        return shopperStartPosList[startposindex];
    }

    //获取进门点 
    public Vector3Int GetIndoorPoint()
    {
        if (this == null) return Vector3Int.zero;

        var priorityPosList = doorNodePosList.GetRange(1, 2);
        var canMovePosList = priorityPosList.FindAll(t => indoorGridFlags[t.x - StaticConstants.IndoorOffsetX, t.y - StaticConstants.IndoorOffsetY] == 0);

        //优先门口中间2个点
        if (canMovePosList.Count > 0)
        {
            return canMovePosList[Random.Range(0, canMovePosList.Count)];
        }

        //没有再从剩下的里面筛
        canMovePosList = doorNodePosList.FindAll(t => indoorGridFlags[t.x - StaticConstants.IndoorOffsetX, t.y - StaticConstants.IndoorOffsetY] == 0);

        if (canMovePosList.Count > 0)
        {
            return canMovePosList[Random.Range(0, canMovePosList.Count)];
        }

        return Vector3Int.zero;
    }

    //创建店主

    public GameObject CreateShopkeeper()
    {
        GameObject instance = GameObject.Instantiate(shopkeeperPfb);
        //Shopkeeper shopkeeper = instance.AddComponent<Shopkeeper>();
        //return shopkeeper;
        return instance;
    }

    //创建顾客
    public Shopper CreateShopper(ShopperData data, bool isNew)
    {
        if (shopperPfb == null) return null;
        GameObject instance = GameObject.Instantiate(shopperPfb, actorRoot);

        Shopper shopper = instance.GetComponent<Shopper>();

        if (!isNew)
        {
            var startpos = Vector3Int.zero;

            // 改成读取本地缓存的位置
            if (SaveManager.inst.GetGameValue<TempData_ShopperPos>(data.data.shopperUid + "_shopperPos", out TempData_ShopperPos shopperPosData))
            {
                startpos = shopperPosData.cellPos;
                shopper.SetCellPos(startpos);
            }
            else
            {
                startpos = GetCounterFrontPos();

                if (startpos == Vector3Int.zero) //如果柜台周围没找到合适位置 随机一个空余位置站立
                {
                    startpos = MapUtils.IndoorCellposToMapCellPos(GetFreeGridPos());
                }

                shopper.SetCellPos(startpos);
            }

            //占领位置
            SetGridFlags(true, startpos.x - StaticConstants.IndoorOffsetX, startpos.y - StaticConstants.IndoorOffsetY, 99999999, 99999999);
        }
        else
        {
            if (data.data.isGuide == 1 || data.data.shopperComeType == (int)EShopperComeType.GuideTask)
            {
                shopper.SetCellPos(StaticConstants.specialRoleInitPos);
            }
            else if (SaveManager.inst.GetGameValue<TempData_ShopperPos>(data.data.shopperUid + "_shopperPos", out TempData_ShopperPos shopperPosData))
            {
                shopper.SetCellPos(shopperPosData.cellPos);
            }
            else
            {
                shopper.SetCellPos(GetStartPoint());
            }
        }

        shopper.SetData(data, isNew);

        return shopper;
    }

    private int passerbyUid;

    //创建行人
    public Passerby CreatePasserby()
    {
        GameObject obj = GameObject.Instantiate(passerbyPfb);
        Passerby passerby = obj.GetComponent<Passerby>();

        return passerby;
    }


    //创建宠物
    public Pet CreatePet(PetData data)
    {
        GameObject obj = GameObject.Instantiate(petPfb, petRoot);
        Pet pet = obj.GetComponent<Pet>();

        pet.SetData(data);

        return pet;
    }

    //创建可解锁工匠
    public IndoorCanLockWorker CreateIndoorCanLockWorker(int workerId)
    {
        GameObject obj = GameObject.Instantiate(canLockWorkerPfb, workerCanLockRoot);
        obj.transform.position = Vector3.zero;
        IndoorCanLockWorker canLockWorker = obj.GetComponent<IndoorCanLockWorker>();

        canLockWorker.SetData(workerId);

        return canLockWorker;
    }

    //创建剧情NPC --活动\礼包
    public GameObject CreateIndoorStoryRole()
    {
        GameObject obj = GameObject.Instantiate(storyRolePfb, storyRoleRoot);
        obj.transform.position = Vector3.zero;

        return obj;
    }

    #endregion

    #region 扩建显示
    //扩展升级
    UpgradeAttacher upgradeAttacher;
    public void updateUpgradePop()
    {
        if (extension == null) return;
        BuildingSite aa = extension.gameObject.GetComponent<BuildingSite>();
        if (aa != null)
        {
            EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;
            aa.tipTF.gameObject.SetActive(state == EDesignState.Idle && UserDataProxy.inst.shopData.shopLevel < StaticConstants.shopMap_MaxLevel && WorldParConfigManager.inst.GetConfig(117).parameters <= UserDataProxy.inst.playerData.level);
            aa.gameObject.SetActive(UserDataProxy.inst.shopData.shopLevel < StaticConstants.shopMap_MaxLevel);
        }

        if (UserDataProxy.inst.shopData.currentState == 0 || UserDataProxy.inst.shopData.currentState == 3)
        {
            if (upgradeAttacher != null)
            {
                upgradeAttacher.setFinished();
                upgradeAttacher.gameObject.SetActive(false);
            }
            return;
        }
        if (upgradeAttacher == null)
        {
            var gameObj = GameObject.Instantiate<GameObject>(UpGradeAttacher, extension.transform.Find("gdzw_zawu"));
            upgradeAttacher = gameObj.GetComponent<UpgradeAttacher>();
            gameObj.transform.localPosition = new Vector3(2.55f, 0.17f, 0);
        }
        upgradeAttacher.gameObject.SetActive(true);
        upgradeAttacher.type = 1;
        upgradeAttacher.setSortingOrder(MapUtils.GetTileMapOrder(transform.position.y, transform.position.x, 0, 0), true);
        if (UserDataProxy.inst.shopData.leftTime <= 0 || UserDataProxy.inst.shopData.currentState == 2)
        {
            upgradeAttacher.setFinished();
        }
        else
        {
            upgradeAttacher.setTime(UserDataProxy.inst.shopData.stateTime, UserDataProxy.inst.shopData.leftTime);
        }
    }
    #endregion

    #region 街道掉落物

    public StreetDrop CreateStreetDropItem(StreetDropData data)
    {
        data.state = StreetDropState.waitPick;

        GameObject obj = GameObject.Instantiate(streetDropPfb, streetDropRoot);
        StreetDrop streetDrop = obj.GetComponent<StreetDrop>();
        streetDrop.SetData(data);

        return streetDrop;
    }
    #endregion
}
