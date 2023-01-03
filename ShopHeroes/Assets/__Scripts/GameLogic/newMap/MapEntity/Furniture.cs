using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;
using UnityEngine.AddressableAssets;
//家具
public class Furniture : Entity
{
    private GameObject furnitureGO;
    public Transform PopUIRoot;
    public Transform furnitureGOParent;
    public bool Results = true; //操作是否成功
    private List<SpriteRenderer> renderObjs = new List<SpriteRenderer>();
    private List<SpriteRenderer> subRenderObjs = new List<SpriteRenderer>();
    public bool isIndoor = true;
    public bool isCreatFurniture = false;
    public AnimationCurve animCurve;
    public GameObject UpgradeVfx;
    Vector2 toUIOffset;
    ShelfDisplay shelfDisplay;
    Animator furnitureAnim;
    bool isWallDeco = false;
    //资源篮状态 
    int resBoxStage;

    bool isvalid = true;
    void OnEnable()
    {
        isvalid = true;
    }
    void OnDestroy()
    {
        isvalid = false;
    }
    private void OnLoaded(GameObject obj)
    {
        if (!isvalid)
        {
            Destroy(obj);
            return;
        }
        renderObjs.Clear();
        subRenderObjs.Clear();
        furnitureGO = obj;

        if (type == kTileGroupType.Carpet)
        {
            furnitureGO.transform.localPosition = new Vector3(0, 0, 0.5f);
            //D2DragCamera.inst.LookToPosition(transform.position, true, D2DragCamera.inst.minZoom);
        }
        else
            furnitureGO.transform.localPosition = Vector3.zero;
        setDir(dir);
        var eventTriggerListener = furnitureGO.GetComponent<InputEventListener>();
        if (eventTriggerListener != null)
        {
            eventTriggerListener.MouseUp = MouseUp;
            eventTriggerListener.MouseDown = MouseDown;
            eventTriggerListener.MouseDrag = Drag;
            eventTriggerListener.OnClick = MouseClick;
            eventTriggerListener.MousePointBlank = MousePointBlank;
        }
        this.PopUIRoot.localPosition = GetMiddlePos();
        ResetShelfData();
        UpdatePethouseFeedTips();

        foreach (Transform item in furnitureGO.transform)
        {
            var render = item.GetComponent<SpriteRenderer>();
            if (render != null) renderObjs.Add(render);
        }

        //蜘蛛网
        var spiderWeb = furnitureGO.GetComponent<SpiderWeb>();
        if (spiderWeb != null)
        {
            for (int i = 0; i < spiderWeb.spiders.Count; i++)
            {
                var subParent = spiderWeb.spiders[i];
                foreach (Transform item in subParent.transform)
                {
                    var subRender = item.GetComponent<SpriteRenderer>();
                    if (subRender != null) subRenderObjs.Add(subRender);
                }
            }
        }
        UpdatePickUpSortingLayer();
        updateSortingOrder();
        GameTimer.inst.AddTimerFrame(5, 1, needSelected);
        OnResItemCountChange();
        resBoxStage = -1;//第一次创建初设置sprite不确定阶段
        if (changemodel)
        {
            //要先把自己设为不占格子，，
            IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 0);
            IndoorMap.inst.ChangeFurniturePosint(uid, cellpos, dir);
            if (!Results)
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, uid);
            else
                IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 1);
        }
        else
        {
            IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 1);
        }
        //ToUI 家具升级面板 和 货架内容面板 记录偏移
        var signTf = obj.transform.Find("sign");
        if (signTf) toUIOffset = new Vector2(-signTf.localPosition.x, -signTf.localPosition.y);

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            var guideCfg = GuideDataProxy.inst.CurInfo.m_curCfg;
            if (guideCfg != null)
            {
                SpiderWeb temp;
                if (guideCfg.sort <= 1 && uid == 60001)
                {
                    temp = furnitureGO.GetComponent<SpiderWeb>();
                    if (temp != null)
                    {
                        temp.setAllActiveTrue();
                        GuideDataProxy.inst.spiders.Add(temp);
                    }
                }
                else if (guideCfg.sort <= 2 && uid == 70001)
                {
                    temp = furnitureGO.GetComponent<SpiderWeb>();
                    if (temp != null)
                    {
                        temp.setAllActiveTrue();
                        GuideDataProxy.inst.spiders.Add(temp);
                    }
                }
                else if (guideCfg.sort <= 10 && uid == 90001)
                {
                    temp = furnitureGO.GetComponent<SpiderWeb>();
                    if (temp != null)
                    {
                        temp.setAllActiveTrue();
                        GuideDataProxy.inst.spiders.Add(temp);
                    }
                }
            }
        }

        if (changemodel && needReSetPos)
        {
            ReSetPos();
        }
    }

    public void ResetShelfData()
    {
        if (type == kTileGroupType.Shelf)
        {
            shelfDisplay = furnitureGO.GetComponent<ShelfDisplay>();
            if (uid != IndoorMap.tempItemUid)
            {
                var ucfg = ShelfUpgradeConfigManager.inst.getConfigByType(currFurniture.type_2, UserDataProxy.inst.GetFuriture(uid).level);
                EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.ShelfDisplay_ReBind, uid, shelfDisplay, ucfg);
            }
        }
    }

    void needSelected()
    {
        //判断是否以选择
        if (IndoorMapEditSys.inst.currEntityUid == uid)
        {
            //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, uid);
            isSelected = true;
            ShowGrid();
            if (IndoorMap.inst != null)
                IndoorMap.inst.ChangeFurniturePosint(uid, cellpos, dir);
        }
        if (isCreatFurniture)
        {
            if (IndoorMap.inst.furnitureHandleResults)
            {
                int symbol = furnitureGO.transform.localScale.x > 0 ? 1 : -1;
                furnitureGO.transform.DOScale(new Vector3(symbol, 1, 1), 0.45f).From(0).SetEase(animCurve);
            }
            isCreatFurniture = false;
        }
    }

    public void EquipFlyOnAnim()
    {
        if (furnitureAnim != null)
        {
            furnitureAnim.CrossFade("equipFlyEnd", 0f);
            furnitureAnim.Update(0f);
            furnitureAnim.Play("equipFlyEnd");
        }
    }



    public override void ShowGrid()
    {
        base.ShowGrid();
        IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 0);
        if (selectedGrid != null)
        {
            selectedGrid.ShowGrid(currSize.x, currSize.y);
            //selectedGrid.ChangeSize(currFurniture.width, currFurniture.height, dir);
        }
        UpdatePickUpSortingLayer();
        updateSortingOrder();
    }
    public override void HideGrid()
    {
        base.HideGrid();
        if (selectedGrid != null)
        {
            selectedGrid.HideGrid();
        }

        if (uid != IndoorMap.tempItemUid)
        {
            IndoorData.ShopDesignItem grid = UserDataProxy.inst.GetFuriture(uid);
            if (grid != null && grid.state == (int)EDesignState.InStore) //仍然处于存放状态 取消选中删除TA
            {
                IndoorMap.inst.RemoveFurnituresOnMap(uid);
                return;
            }
        }

        // //取消选择 位置不合法还原位置
        if (!Results)
        {
            ResetState();
            //还原位置
            PickUP(false);
        }
        IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 1);
        //if (type == kTileGroupType.Counter)
        //{
        //    ///新
        //    IndoorMapEditSys.inst.Shopkeeper.OnDesignChanged();
        //}
    }

    public override void Drag(Vector3 lastpos, Vector3 newpos)
    {
        if (ManagerBinder.inst != null && ManagerBinder.inst.mGameState == kGameState.VisitShop) return;

        if (!isPickUp) return;
        var world = IndoorMap.inst.mainCamera.ScreenToWorldPoint(newpos);
        Vector3Int gridpos = IndoorMap.inst.gameMapGrid.WorldToCell(world);
        var topos = gridpos + offsetMousePos;
        cellpos.x = topos.x - 5;
        cellpos.y = topos.y + 1;
        SetCellPosInt(cellpos);
        IndoorMap.inst.ChangeFurniturePosint(uid, cellpos, dir);//变化
    }

    public override void MouseUp(Vector3 mousepos)
    {
        if (ManagerBinder.inst != null && ManagerBinder.inst.mGameState == kGameState.VisitShop) return;
        if (isSelected && isPickUp)
        {
            OccupyDrag = false;
            if (Results)
            {
                //放到合法位置
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Move_Rotate, uid, cellpos.x, cellpos.y, dir);
                PickUP(false);
            }
        }
    }
    public override void MouseDown(Vector3 mousepos)
    {
        if (ManagerBinder.inst != null && ManagerBinder.inst.mGameState == kGameState.VisitShop) return;
        if (IndoorMap.inst != null)
        {
            if (isSelected)
            {
                //已经选中
                if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.isDesigning)
                {
                    OccupyDrag = true;
                    if (!isPickUp)
                        PickUP(true);
                }
            }

            var world = IndoorMap.inst.mainCamera.ScreenToWorldPoint(mousepos);
            offsetMousePos = MapUtils.IndoorCellposToMapCellPos(cellpos) - IndoorMap.inst.gameMapGrid.WorldToCell(world);
        }
    }

    private void PickUP(bool v)
    {
        isPickUp = v;
        if (furnitureGO != null)
        {
            if (isPickUp)
            {
                furnitureGO.transform.DOLocalMoveY(0.2f, .3f);
                Vector3 tpos = this.transform.localPosition;
                tpos.z = -0.2f;
                this.transform.localPosition = tpos;
            }
            else
            {
                Vector3 tpos = this.transform.localPosition;
                tpos.z = 0;
                this.transform.localPosition = tpos;
                furnitureGO.transform.DOLocalMoveY(0f, .3f).onComplete = () =>
                {
                    updateSortingOrder();
                };
            }
        }
        UpdatePickUpSortingLayer();
    }
    private void updateSortingOrder()
    {

        if (shelfDisplay != null)// 货架逻辑多一层处理
        {
            //var order = MapUtils.GetTileMapOrder(transform.position.y, transform.position.x, currSize.x, currSize.y);
            shelfDisplay.SetSortingOrder(currSize.x, currSize.y);

            for (int i = 0; i < subRenderObjs.Count; i++)
            {
                var obj = subRenderObjs[i].gameObject;
                subRenderObjs[i].sortingOrder = renderObjs[0].sortingOrder + 1;
            }
        }
        else
        {
            //  var order = MapUtils.GetTileMapOrder(transform.position.y, transform.position.x, currSize.x, currSize.y);
            for (int i = 0; i < renderObjs.Count; i++)
            {
                if (type == kTileGroupType.Carpet)
                {
                    renderObjs[i].sortingOrder = isPickUp ? 10005 : 10001;
                    return;
                }
                var obj = renderObjs[i].transform;
                renderObjs[i].sortingOrder = MapUtils.GetTileMapOrder(obj.position.y, obj.position.x, currSize.x, currSize.y);
            }
            for (int i = 0; i < subRenderObjs.Count; i++)
            {
                subRenderObjs[i].sortingOrder = renderObjs[0].sortingOrder + 1;
            }
        }
        updateUpgradePop(UserDataProxy.inst.GetFuriture(uid));
    }

    private void UpdatePickUpSortingLayer()
    {
        // if (type == kTileGroupType.WallFurniture) return;
        int order = 0;
        foreach (var item in renderObjs)
        {
            order = item.sortingOrder;
            if (type == kTileGroupType.Carpet || type == kTileGroupType.WallFurniture)
            {
                item.sortingLayerName = "map_floor";
                if (type == kTileGroupType.Carpet)
                    item.sortingOrder = isPickUp ? 10005 : 10001;
            }
            else
            {
                if (type == kTileGroupType.OutdoorFurniture) //outdoorActor
                {
                    item.sortingLayerName = isPickUp ? "Actors_PickUp" : "outdoorActor";
                }
                else
                {
                    item.sortingLayerName = isPickUp ? "Actors_PickUp" : "map_Actor";
                }
            }
        }
        if (type == kTileGroupType.Carpet)
        {
            selectedGrid.Setorder("map_floor", 10002);
            return;
        }
        else
        {
            selectedGrid.Setorder(type == kTileGroupType.OutdoorFurniture ? "outdoorActor" : "map_Actor", isPickUp ? order + 10 : 1);
        }
        if (shelfDisplay != null) // 货架逻辑多一层处理
        {
            shelfDisplay.SetSortingLayer(isPickUp ? "Actors_PickUp" : "map_Actor");
        }
    }

    public override int Rotate()
    {
        setDir(dir == 0 ? 1 : 0);
        this.PopUIRoot.localPosition = GetMiddlePos();
        return dir;
    }
    public void changeState(bool _results)
    {
        Results = _results;
        selectedGrid.GridIsRed(!Results);
        if (/*!_results && */!isPickUp)
        {
            PickUP(!_results);
        }
    }
    public void ResetState()
    {
        IndoorData.ShopDesignItem grid = UserDataProxy.inst.GetFuriture(uid);
        UpdatePickUpSortingLayer();
        SetCellPosInt(new Vector3Int(grid.x, grid.y, 0));
        setDir(grid.dir);
    }


    bool changemodel;
    public void updateData(IndoorData.ShopDesignItem data, bool changemodel)
    {
        this.changemodel = changemodel;
        if (changemodel)
        {
            if (furnitureGO != null)
            {
                Destroy(furnitureGO);
                renderObjs.Clear();
                subRenderObjs.Clear();
            }
            needReSetPos = true;
            id = data.id;
            create();
        }
        else
        {
            ResetState();
            //升级状态更新
            updateUpgradePop(data);
        }
    }

    public UpgradeAttacher upgradeAttacher;
    //Tweener _coloTweener;
    Tweener[] _coloTweeners;
    public void UpdateUpgradeAnim(bool runing)
    {
        if (type == kTileGroupType.Carpet || type == kTileGroupType.WallFurniture) return;
        // if (_coloTweeners == null)
        // {
        //     _coloTweeners = new Tweener[renderObjs.Count];
        // }
        if (UpgradeVfx != null && renderObjs.Count > 0)
        {
            //UpgradeVfx.SetActive(false);
            UpgradeVfx.SetActive(runing);
            if (runing)
            {
                UpgradeVfx.transform.localPosition = PopUIRoot.localPosition;
                var vfsrenderers = UpgradeVfx.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in vfsrenderers)
                {
                    renderer.sortingLayerName = "map_Actor";
                    renderer.sortingOrder = renderObjs[0].sortingOrder + 1;
                }
            }
        }
        for (int i = 0; i < renderObjs.Count; i++)
        {
            // if (_coloTweeners[i] != null)
            //  {
            DOTween.Kill(renderObjs[i]);
            //_coloTweeners[i] = null;
            // }
            if (runing)
            {
                renderObjs[i].color = Color.white;
                renderObjs[i].DOColor(GUIHelper.GetColorByColorHex("9BFB9E"), 1.5f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                renderObjs[i].color = Color.white;
            }
        }

        // if (runing)
        // {
        //     for (int i = 0; i < renderObjs.Count; i++)
        //     {

        //         _coloTweeners[i] = renderObjs[i].DOColor(GUIHelper.GetColorByColorHex("9BFB9E"), 1.5f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
        //     }
        // }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);

        if (visible)
        {
            ShowPethouseFeedTips();
        }
        else
        {
            HidePethouseFeedTips();
        }

    }

    public void ShowPethouseFeedTips()
    {
        UpdatePethouseFeedTips();
    }

    public void HidePethouseFeedTips()
    {
        if (pethouseFeedTips != null)
        {
            pethouseFeedTips.gameObject.SetActiveFalse();
        }
    }

    PethouseFeedTips pethouseFeedTips;
    public void UpdatePethouseFeedTips()
    {
        if (type != kTileGroupType.PetHouse || IndoorMapEditSys.inst.shopDesignMode != DesignMode.normal) return;

        var petData = PetDataProxy.inst.GetPetDataByFurnitureUid(uid);

        if (petData == null) return;

        if (petData.petInfo.petNextFeedTime <= 0 && PetDataProxy.inst.GetPethouseNeedFeedTips(petData.petUid)) //可以喂食
        {
            if (pethouseFeedTips == null)
            {
                var gameObj = GameObject.Instantiate<GameObject>(IndoorMap.inst.PethouseFeedTipsPfb, this.PopUIRoot);
                gameObj.transform.localPosition = Vector3.zero;
                pethouseFeedTips = gameObj.GetComponent<PethouseFeedTips>();
                this.PopUIRoot.localPosition = GetMiddlePos();
            }

            pethouseFeedTips.FUid = uid;
            pethouseFeedTips.gameObject.SetActiveTrue();
            updateSortingOrder();
        }
        else
        {
            if (pethouseFeedTips != null)
            {
                pethouseFeedTips.gameObject.SetActiveFalse();
            }
        }

    }



    private void updateUpgradePop(IndoorData.ShopDesignItem data)
    {
        if (data == null) return;

        if (pethouseFeedTips != null)
        {
            pethouseFeedTips.setSortingLayer("map_Actor");
            pethouseFeedTips.setSortingOrder(MapUtils.GetTileMapOrder(transform.position.y, transform.position.x, currSize.x, currSize.y) + 5);
        }

        if (data.state == 0 || data.state == 3)
        {
            if (upgradeAttacher != null)
            {
                upgradeAttacher.setFinished();
                upgradeAttacher.gameObject.SetActive(false);
            }
            UpdateUpgradeAnim(false);
            return;
        }


        UpdateUpgradeAnim(true);
        if (upgradeAttacher == null)
        {
            var gameObj = GameObject.Instantiate<GameObject>(IndoorMap.inst.UpGradeAttacher, this.PopUIRoot);
            gameObj.transform.localPosition = Vector3.zero;
            upgradeAttacher = gameObj.GetComponent<UpgradeAttacher>();
            this.PopUIRoot.localPosition = GetMiddlePos();
        }
        upgradeAttacher.gameObject.SetActive(true);
        upgradeAttacher.FUid = uid;

        if (type == kTileGroupType.Carpet || type == kTileGroupType.WallFurniture)
        {
            upgradeAttacher.setSortingLayer("map_floor");
            if (type == kTileGroupType.Carpet)
                upgradeAttacher.setSortingOrder(isPickUp ? 10005 : 10001, false);
        }
        else
        {
            upgradeAttacher.setSortingOrder(MapUtils.GetTileMapOrder(transform.position.y, transform.position.x, currSize.x, currSize.y) + 5, false);
            upgradeAttacher.setSortingLayer(isPickUp ? "Actors_PickUp" : (type == kTileGroupType.OutdoorFurniture ? "outdoorActor" : "map_Actor"));
        }

        if (data.entityState.leftTime <= 0 || data.state == 2)
        {
            upgradeAttacher.setFinished();
        }
        else
        {
            upgradeAttacher.setTime(data.entityState.stateTime, data.entityState.leftTime);
        }
    }
    ///////////////////////////////////////////////////////////////////////////////////
    //2d 旋转 ， xy对调
    private void PosRotate(ref Vector3Int pos)
    {
        var v = pos.x;
        pos.x = pos.y;
        pos.y = v;
    }
    public List<Vector3Int> GetShopKeeperPos()
    {
        List<Vector3Int> poslist = new List<Vector3Int>();
        if (type == kTileGroupType.Counter) //柜台有固定位置
        {

            List<Vector3Int> offsets = new List<Vector3Int>() { StaticConstants.shopKeeperPoint };

            if (UserDataProxy.inst.GetFuriture(uid).level < 6)//柜台第一阶段（多一个位置）
            {
                offsets.Add(new Vector3Int(0, 1, 0));
            }

            for (int i = 0; i < offsets.Count; i++)
            {
                if (dir == 1)
                {
                    var offset = offsets[i];
                    PosRotate(ref offset);
                    offsets[i] = offset;
                }
            }

            for (int i = 0; i < offsets.Count; i++)
            {
                var offset = offsets[i];
                RectInt size = UserDataProxy.inst.GetIndoorSize();
                if (cellpos.x + offset.x < size.xMin || cellpos.x + offset.x > size.xMax || cellpos.y + offset.y < size.yMin || cellpos.y + offset.y > size.yMax)
                {
                    //越界 不加
                }
                else
                {
                    var pos = MapUtils.IndoorCellposToMapCellPos(cellpos + offset);

                    int gridFlag = IndoorMap.inst.GetIndoorGridFlags(cellpos.x + offset.x, cellpos.y + offset.y);
                    if (gridFlag == 0 || gridFlag == 1415926 || gridFlag == (int)kTileGroupType.Counter)
                    {
                        // 店主应站位置的那个格格 设为非障碍物
                        if (gridFlag == 0 || gridFlag == (int)kTileGroupType.Counter) IndoorMap.inst.SetGridFlags(true, cellpos.x + offset.x, cellpos.y + offset.y, 1415926, 1415926);

                        poslist.Add(pos);
                    }
                }
            }
        }
        else
        {
            //其他装饰没有店主位置
        }
        return poslist;
    }

    //获取观看位置
    public List<Vector3Int> GetFreePos()
    {
        List<Vector3Int> poslist = new List<Vector3Int>();
        if (type == kTileGroupType.Counter)
        {
            //查找第一圈
            for (int x = -1; x < currSize.x; x++)
            {
                for (int y = -1; y < currSize.y; y++)
                {
                    if (x >= 0 && y >= 0 && x < currSize.x && y < currSize.y)
                        continue;
                    var rotpos = new Vector3Int(x, y, 0);
                    //  if (dir == 1) PosRotate(ref rotpos);
                    if (!IndoorMap.inst.IsObstacleGrid((rotpos + cellpos).x, (rotpos + cellpos).y))
                        poslist.Add(MapUtils.IndoorCellposToMapCellPos(rotpos + cellpos));
                }
            }
            if (poslist.Count > 0)
            {
                //第一圈有位置则返回
                return poslist;
            }
            else
            {
                //查找第二圈
                for (int x = -2; x < currSize.x; x++)
                {
                    for (int y = -2; y < currSize.y; y++)
                    {
                        if (y > -2 && x > -2)
                            continue;
                        var rotpos = new Vector3Int(x, y, 0);
                        // if (dir == 1) PosRotate(ref rotpos);
                        if (!IndoorMap.inst.IsObstacleGrid((rotpos + cellpos).x, (rotpos + cellpos).y))
                            poslist.Add(MapUtils.IndoorCellposToMapCellPos(rotpos + cellpos));
                    }
                }
            }
        }
        else
        {

            if (type == kTileGroupType.PetHouse) //宠物小家 无视障碍 除了地图外（越界出地图）
            {
                RectInt size = UserDataProxy.inst.GetIndoorSize();

                //int[] directionArr = { 0, 1, 0, -1, 0 }; //  0,1左 1,0上 0,-1 右 -1,0下

                Vector2Int[] directionArr = { new Vector2Int(0, currSize.y), new Vector2Int(currSize.x, 0), new Vector2Int(0, -currSize.y), new Vector2Int(-currSize.x, 0) };

                for (int i = 0; i < directionArr.Length; i++)
                {
                    int left = directionArr[i].x;
                    int right = directionArr[i].y;

                    var pos = cellpos + new Vector3Int(left, right, 0);

                    if (pos.x < size.xMin || pos.x > size.xMax || pos.y < size.yMin || pos.y > size.yMax)
                    {
                        //越界 不加
                    }
                    else
                    {
                        poslist.Add(MapUtils.IndoorCellposToMapCellPos(pos));
                    }

                }

            }
            else
            {
                //Logger.error("我是" + currFurniture.name + "    我的大小 x:" + currSize.x + " y:" + currSize.y + "  旋转：" + dir + "  位置:" + MapUtils.IndoorCellposToMapCellPos(cellpos) + "   开始遍历我的周围");
                //string str = "";

                for (int x = -1; x < currSize.x + 1; x++)
                {
                    for (int y = -1; y < currSize.y + 1; y++)
                    {
                        if (x >= 0 && y >= 0 && x < currSize.x && y < currSize.y)
                            continue;
                        var rotpos = new Vector3Int(x, y, 0);
                        //if (dir == 1) PosRotate(ref rotpos);
                        //str += MapUtils.IndoorCellposToMapCellPos(rotpos + cellpos).ToString();
                        if (!IndoorMap.inst.IsObstacleGrid((rotpos + cellpos).x, (rotpos + cellpos).y))
                            poslist.Add(MapUtils.IndoorCellposToMapCellPos(rotpos + cellpos));
                    }
                }

                //Logger.error(currFurniture.name + "遍历结束 遍历的位置有 " + str);
            }
        }
        return poslist;
    }

    public Vector3 GetMiddlePos()
    {
        Vector3 pos = Vector3.zero;
        int maxx;
        int maxy;
        if (dir == 0)
        {
            maxx = currFurniture.width;
            maxy = currFurniture.height;
        }
        else
        {
            maxx = currFurniture.height;
            maxy = currFurniture.width;
        }
        pos = MapUtils.CellPosToWorldPos(new Vector3Int(maxx, maxy, 0)) * .5f;
        return pos;
    }

    //资源篮相关显示刷新
    public void OnResItemCountChange()
    {
        if (currFurniture.type_1 != 9) return;//不是资源篮
        if (currFurniture.type_2 == 4 || currFurniture.type_2 == 5) return; //储油箱，珠宝箱没有阶段转换

        var upCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(currFurniture.type_2, 1);
        Item item = ItemBagProxy.inst.GetItem(upCfg.item_id);
        if (item == null)
        {
            Debug.LogError("itembagProxy 找不到item ：" + upCfg.item_id);
            return;
        }
        int count = (int)item.count;
        int limit = UserDataProxy.inst.GetResCountLimit(item.ID);
        int stage = count == 0 ? 0 : count <= (limit / 2) ? 1 : count == limit ? 3 : 2; //四个阶段  无 0   0-50% 1   50%-99% 2   100% 3

        if (resBoxStage == stage) return; //同阶段退出
        resBoxStage = stage;

        string iconNameBase = currFurniture.prefabnew.Replace(".prefab", "");

        if (renderObjs.Count == 1)
        {
            SpriteRenderer spriteRenderer = renderObjs[0];
            string iconName = iconNameBase + "_" + string.Format("{0:00}", resBoxStage);

            ManagerBinder.inst.Asset.loadMiscAsset<Sprite>(iconName, (sprite) =>
            {
                if (spriteRenderer) spriteRenderer.sprite = sprite;
            });
        }
        else
        {
            for (int i = 0; i < renderObjs.Count; i++)
            {
                SpriteRenderer spriteRenderer = renderObjs[i];
                string iconName = iconNameBase + "_" + string.Format("{0:00}", resBoxStage) + spriteRenderer.gameObject.name;
                ManagerBinder.inst.Asset.loadMiscAsset<Sprite>(iconName, (sprite) =>
                {
                    if (spriteRenderer) spriteRenderer.sprite = sprite;
                });
            }
        }
    }
    bool needReSetPos = false;
    //家具升级和货架内容  
    public void ReSetPos() //把它送回来
    {
        if (changemodel && !needReSetPos)
        {
            return;
        }

        changemodel = false;
        needReSetPos = false;

        transform.SetParent(IndoorMap.inst.indoorFlootNode.transform);
        transform.localScale = Vector3.one;
        if (furnitureGO != null)
            furnitureGO.transform.localPosition = Vector3.zero;

        ResetState();
        updateSortingOrder();
        SetColliderVisible(true);

        if (isSelected)
            ShowGrid();
        else
            HideGrid();
    }
    //================================================================================================
    //界面上的操作
    public void SetUIPosition(Transform parent, string sortingLayer, int sortingOrder, int height)
    {
        HideGrid();
        SetColliderVisible(false);

        if (upgradeAttacher != null)
        {
            upgradeAttacher.gameObject.SetActiveFalse();
        }

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * (400 - (height - 1) * 20);

        Vector2 offset = new Vector2(toUIOffset.x * (dir == 0 ? 1 : -1), toUIOffset.y);
        furnitureGO.transform.localPosition = offset;

        //设置层级
        foreach (var item in renderObjs)
        {
            item.sortingLayerName = sortingLayer;
            item.sortingOrder = sortingOrder;
        }

        if (shelfDisplay != null)
        {
            shelfDisplay.SetSortingLayer(sortingLayer);
            shelfDisplay.SetSortingOrder(currSize.x, currSize.y, sortingOrder);
        }
    }

    // void updateAnimi
    //***************************************************************************************************

    //位置控制器
    ActorPosCrl actorPosCrl;

    //家具占地格子显示控制
    public FurnitureSelectedGrid selectedGrid;

    void Awake()
    {
        if (actorPosCrl == null)
            actorPosCrl = GetComponent<ActorPosCrl>();
        if (selectedGrid == null)
            selectedGrid = GetComponentInChildren<FurnitureSelectedGrid>();
    }

    public void SetFurnitureId(int fid)
    {
        id = fid;
        //
        currFurniture = FurnitureConfigManager.inst.getConfig(id);
        if (currFurniture == null)
        {
            Logger.error($"无法创建家具，找不到对应{id}家具配置！");
            Destroy(this.gameObject, 1);
            return;
        }
        isWallDeco = currFurniture.type_1 == 4;
        if (string.IsNullOrEmpty(currFurniture.prefabnew))
        {
            return;
        }

        if (changemodel)
        {
            IndoorMap.inst.ChangeFurniturePosint(uid, cellpos, dir);
            if (!Results)
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, uid);
            else
                IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 1);
        }
        else
        {
            IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 1);
        }
    }
    //创建对象
    public override void create()
    {
        base.create();
        SetFurnitureId(id);
        furnitureAnim = furnitureGOParent.GetComponent<Animator>();
        if (furnitureAnim == null)
        {
            furnitureAnim = furnitureGOParent.gameObject.AddComponent<Animator>();
        }

        ManagerBinder.inst.Asset.loadMiscAsset<RuntimeAnimatorController>("Assets/GUI2D/Animation/Furniture/furnitureAnim.controller", (animCtrl) =>
        {
            if (this == null)
            {
                return;
            }
            furnitureAnim.runtimeAnimatorController = animCtrl;
        });


        ManagerBinder.inst.Asset.loadPrefabAsync(currFurniture.prefabnew, furnitureGOParent, OnLoaded);

        if (uid != IndoorMap.tempItemUid)
        {
            IndoorData.ShopDesignItem grid = UserDataProxy.inst.GetFuriture(uid);

            if (grid != null && grid.state == (int)EDesignState.Idle)
            {

            }

        }

    }


    //操作
    //改变位置
    public override void SetCellPosInt(Vector3Int cellposint)
    {
        if (type != kTileGroupType.OutdoorFurniture)
        {
            RectInt size = UserDataProxy.inst.GetIndoorSize();
            if (type == kTileGroupType.WallFurniture)
            {
                if (dir == 0)
                {
                    cellposint.y = size.yMax;
                }
                else
                {
                    cellposint.x = size.xMax;
                }
            }
            if (cellposint.x < size.xMin) cellposint.x = size.xMin;
            if (cellposint.y < size.yMin) cellposint.y = size.yMin;
            if ((cellposint.x + currSize.x - 1) >= size.xMax) cellposint.x = size.xMax - currSize.x + 1;
            if ((cellposint.y + currSize.y - 1) >= size.yMax) cellposint.y = size.yMax - currSize.y + 1;
            cellpos = cellposint;
            actorPosCrl.SetPosition(MapUtils.IndoorCellposToMapCellPos(cellposint));
        }
        else
        {
            cellpos = cellposint;
            actorPosCrl.SetPosition(cellposint);
        }
    }

    private void setDir(int _dir)
    {
        dir = _dir;

        if (dir == 0)
        {
            currSize.x = currFurniture.width;
            currSize.y = currFurniture.height;
        }
        else
        {
            currSize.x = currFurniture.height;
            currSize.y = currFurniture.width;
        }


        if (selectedGrid != null)
            selectedGrid.ChangeSize(currSize.x, currSize.y);
        if (furnitureGO != null)
            furnitureGO.transform.localScale = new Vector3(dir == 0 ? 1 : -1, 1, 1);
        SetCellPosInt(cellpos);
    }

    public void RemoveSelf()
    {
        if (!isPickUp)
            IndoorMap.inst.SetIndoorGridFlags(uid, cellpos.x, cellpos.y, dir, 0);
        Destroy(gameObject, 0.1f);
    }

    public override void MouseClick(Vector3 mousepos)
    {
        if (ManagerBinder.inst != null && ManagerBinder.inst.mGameState == kGameState.VisitShop) return;

        IndoorData.ShopDesignItem grid = UserDataProxy.inst.GetFuriture(uid);
        if (grid != null && grid.state == 2)
        {

            if (IndoorMapEditSys.inst.currEntityUid != uid && IndoorMapEditSys.inst.currEntityUid != -1)
            {
                if (IndoorMap.inst.GetFurnituresByUid(IndoorMapEditSys.inst.currEntityUid, out Furniture furnitureEntity))
                {
                    furnitureEntity.UnSelected();
                    EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, uid);
                    OnSelected();
                }
            }

            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, uid);
            return;
        }

        if (type == kTileGroupType.PetHouse && IndoorMapEditSys.inst.shopDesignMode == DesignMode.LookPetHouse) //是宠物窝 并且在观看宠物窝
        {
            return;
        }

        if (type != kTileGroupType.Carpet && type != kTileGroupType.WallFurniture)
        {
            if (IndoorMap.inst != null && IndoorMap.inst.CanChangeSelectEntity())
            {
                if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null)
                {
                    if (!GuideDataProxy.inst.CurInfo.isAllOver)
                    {
                        return;
                    }
                }
                HotfixBridge.inst.TriggerLuaEvent("HideUI_GuideTask");
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI);
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, uid);
                OnSelected();
            }
        }
        else
        {
            //墙饰和地毯只能在 设计模式下选中(非创建模式)
            if (IndoorMapEditSys.inst.isDesigning && IndoorMapEditSys.inst.currEntityUid != IndoorMap.tempItemUid)
            {
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, uid);
                OnSelected();
            }
        }
    }


}