using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class IndoorMapEditSys
{
    private string floorPlanePath = "floor_Edite";
    private int currEditFloorId = -1;
    private FloorEditer floorEditer;    //当前操作的地板
    private RectInt editFloorSize = new RectInt(0, 0, 2, 2);

    //进入地板编辑模式
    //
    public void EnterFloorEditMode(int floorId, RectInt mapSize)
    {
        currEditFloorId = floorId;
        editFloorSize = mapSize;
        //显示网格
        IndoorMap.inst.MapGridLineVisible(true);
        //隐藏宠物小家头顶信息
        HidePethouseFeedTips();
        //隐藏所有Role
        HideRole();
        //取消当前选择
        OnFurnituresRelease();
        //
        HideFurniture();
        InitFloorPlane();
    }

    public void clearFloorEditMode()
    {
        if (UserDataProxy.inst.shopData != null)
        {

            D2DragCamera.inst.updateCameMaxZoom(UserDataProxy.inst.shopData.shopLevel, kCameraMoveType.shopExtend);
        }
        D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
        currEditFloorId = -1;
        if (floorEditer != null)
        {
            GameObject.Destroy(floorEditer.gameObject);
        }
        editFloorSize = new RectInt(0, 0, 2, 2);
        ShowFurniture();
    }
    void InitFloorPlane()
    {
        ManagerBinder.inst.Asset.loadPrefabAsync(floorPlanePath, IndoorMap.inst.indoorFlootNode.transform, (obj) =>
         {
             var newGo = obj;
             newGo.SetActive(true);
             newGo.transform.localPosition = Vector3.zero;
             CreateFloorPlane(newGo);
         });
    }
    void CreateFloorPlane(GameObject newGo)
    {
        floorEditer = newGo.GetComponent<FloorEditer>();
        if (floorEditer != null)
        {
            //设置地砖
            floorEditer.setFloorSprite(currEditFloorId);
            //注册操作事件
            var eventTriggerListener = floorEditer.vertex_L.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += vertexLDrag;
            }

            eventTriggerListener = floorEditer.vertex_T.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += vertexTDrag;
            }

            eventTriggerListener = floorEditer.vertex_R.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += vertexRDrag;
            }

            eventTriggerListener = floorEditer.vertex_D.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += vertexDDrag;
            }

            //地板拖拽
            eventTriggerListener = floorEditer.floorPanle.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += floorDrag;
                eventTriggerListener.MouseDown += floorMouseDown;
            }

            //地板皮肤
            eventTriggerListener = floorEditer.sp_skin.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.OnClick += (v3) =>
                {
                    //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ShowUI_FurnitureSkinUI);
                };
            }

            floorEditer.setSize(ref editFloorSize);
            //  D2DragCamera.inst.maxZoom = 10f;
            D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(new Vector3Int(editFloorSize.xMin, editFloorSize.yMin, 0)), false, D2DragCamera.inst.maxZoom);
        }
    }

    //地板拖拽
    Vector3Int floorMouseOffset = Vector3Int.zero;
    void floorMouseDown(Vector3 mousepos)
    {
        var world = Camera.main.ScreenToWorldPoint(mousepos);
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(floorEditer.floorPanle.position);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        floorMouseOffset = newmouse - vertexpos;
    }
    void floorDrag(Vector3 lastpos, Vector3 newpos)
    {
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(floorEditer.floorPanle.position);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        int xdis = newmouse.x - vertexpos.x - floorMouseOffset.x;
        int ydis = newmouse.y - vertexpos.y - floorMouseOffset.y;
        if (Mathf.Abs(xdis) > 2)
        {
            if (xdis > 0)
            {
                editFloorSize.x += 2;
            }
            else
            {
                editFloorSize.x -= 2;
            }
            floorEditer.setSize(ref editFloorSize);
            floorMouseOffset = newmouse - vertexpos;
        }
        if (Mathf.Abs(ydis) > 2)
        {
            if (ydis > 0)
            {
                editFloorSize.y += 2;
            }
            else
            {
                editFloorSize.y -= 2;
            }
            floorEditer.setSize(ref editFloorSize);
            floorMouseOffset = newmouse - vertexpos;
        }

    }
    //下侧顶点拖拽事件
    void vertexDDrag(Vector3 lastpos, Vector3 newpos)
    {
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(floorEditer.vertex_D.transform.position);
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        float xdis = newmouse.x - vertexpos.x;
        float ydis = newmouse.y - vertexpos.y;
        if (Mathf.Abs(xdis) > 1)
        {
            if (xdis > 0)
            {
                subSize_L();
            }
            else
            {
                addSize_L();
            }
        }
        if (Mathf.Abs(ydis) > 1)
        {
            if (ydis > 0)
            {

                subSize_D();
            }
            else
            {
                addSize_D();
            }
        }
    }
    //上侧顶点拖拽事件
    void vertexTDrag(Vector3 lastpos, Vector3 newpos)
    {
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(floorEditer.vertex_T.transform.position);
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        float xdis = newmouse.x - vertexpos.x;
        float ydis = newmouse.y - vertexpos.y;
        if (Mathf.Abs(xdis) > 1)
        {
            if (xdis > 0)
            {
                addSize_R();
            }
            else
            {
                subSize_R();
            }
        }
        if (Mathf.Abs(ydis) > 1)
        {
            if (ydis > 0)
            {
                addSize_T();
            }
            else
            {
                subSize_T();
            }
        }
    }
    //右侧顶点拖拽事件
    void vertexRDrag(Vector3 lastpos, Vector3 newpos)
    {
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(floorEditer.vertex_R.transform.position);
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        float xdis = newmouse.x - vertexpos.x;
        float ydis = newmouse.y - vertexpos.y;
        if (Mathf.Abs(xdis) > 1)
        {
            if (xdis > 0)
            {
                addSize_R();
            }
            else
            {
                subSize_R();
            }
        }
        if (Mathf.Abs(ydis) > 1)
        {
            if (ydis > 0)
            {
                subSize_D();
            }
            else
            {
                addSize_D();
            }
        }
    }
    //左边顶点拖拽事件
    void vertexLDrag(Vector3 lastpos, Vector3 newpos)
    {
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(floorEditer.vertex_L.transform.position);
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        float xdis = newmouse.x - vertexpos.x;
        float ydis = newmouse.y - vertexpos.y;
        if (Mathf.Abs(xdis) > 1)
        {
            if (xdis > 0)
            {
                subSize_L();
            }
            else
            {
                addSize_L();
            }
        }
        if (Mathf.Abs(ydis) > 1)
        {
            if (ydis > 0)
            {
                addSize_T();
            }
            else
            {
                subSize_T();
            }
        }
    }
    void addSize_L()
    {
        editFloorSize.x -= 2;
        editFloorSize.width += 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
    void addSize_R()
    {
        editFloorSize.width += 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
    void addSize_T()
    {
        editFloorSize.height += 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
    void addSize_D()
    {
        editFloorSize.y -= 2;
        editFloorSize.height += 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
    void subSize_L()
    {
        editFloorSize.x += 2;
        editFloorSize.width -= 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
    void subSize_R()
    {
        editFloorSize.width -= 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
    void subSize_T()
    {
        editFloorSize.height -= 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);

    }
    void subSize_D()
    {
        editFloorSize.y += 2;
        editFloorSize.height -= 2;
        floorEditer.setSize(ref editFloorSize);
        Logger.log("拖拽 鼠标坐标：" + editFloorSize);
    }
}
