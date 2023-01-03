using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;

public partial class IndoorMapEditSys
{
    private string wallPlanePath = "Wall_Edite";
    private int currEditWallId = -1;

    private WallEditer wallEditer;
    private int currStartWallIndex;
    private int currEditorCount;
    private int currEditorWallDir;

    public void EnterWallEditMode(int wallId)
    {
        currStartWallIndex = 0;
        currEditorCount = 1;
        currEditorWallDir = 0;
        currEditWallId = wallId;
        //显示网格
        IndoorMap.inst.MapGridLineVisible(true);
        //隐藏宠物小家头顶信息
        HidePethouseFeedTips();
        //隐藏所有Role
        HideRole();
        //取消当前选择
        OnFurnituresRelease();
        HideFurniture();
        //显示墙体
        InitWallPlane();
    }

    public void ExitWallEditMode()
    {
        if (UserDataProxy.inst.shopData != null)
        {
            var cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopExtend, UserDataProxy.inst.shopData.shopLevel);
            // D2DragCamera.inst.maxZoom = cfg.zoom1 / 100f;
        }
        D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
        currEditWallId = -1;
        if (wallEditer != null)
        {
            GameObject.Destroy(wallEditer.gameObject);
        }
        ShowFurniture();
    }

    void InitWallPlane()
    {
        ManagerBinder.inst.Asset.loadPrefabAsync(wallPlanePath, IndoorMap.inst.indoorFlootNode.transform, (obj) =>
        {
            var newGo = obj;
            newGo.SetActive(true);
            newGo.transform.localPosition = Vector3.zero;
            CreateWalled(newGo);
        });
    }

    void CreateWalled(GameObject wallGo)
    {
        wallEditer = wallGo.GetComponent<WallEditer>();
        if (wallEditer != null)
        {
            //注册操作事件
            var eventTriggerListener = wallEditer.vertex_L.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += wallEditVertexLDrag;
            }
            eventTriggerListener = wallEditer.vertex_R.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += wallEditVertexRDrag;
            }


            eventTriggerListener = wallEditer.wallPanle.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseDrag += EditWallDrag;
                eventTriggerListener.MouseDown += EditWallMouseDown;
            }

            wallEditer.initWallList(currEditWallId);
            wallEditer.setEditorWallSize(ref currEditorWallDir, ref currStartWallIndex, ref currEditorCount);
            var indoorsize = UserDataProxy.inst.GetIndoorSize();
            //D2DragCamera.inst.maxZoom = 10f;
            //D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(new Vector3Int((indoorsize.xMin + StaticConstants.IndoorOffsetX), (indoorsize.yMin + indoorsize.yMax) / 2, 0)), false, D2DragCamera.inst.maxZoom);
            D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(new Vector3Int((int)((indoorsize.xMax + indoorsize.xMin) * .5f) + StaticConstants.IndoorOffsetX, indoorsize.yMax, 0)), false, D2DragCamera.inst.maxZoom);
        }
    }

    Vector3Int wallMouseOffset = Vector3Int.zero;
    void EditWallMouseDown(Vector3 mousepos)
    {
        var world = Camera.main.ScreenToWorldPoint(mousepos);
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(wallEditer.wallPanle.position);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        wallMouseOffset = newmouse - vertexpos;
    }
    void EditWallDrag(Vector3 lastpos, Vector3 newpos)
    {
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(wallEditer.wallPanle.position);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        int xdis = newmouse.x - vertexpos.x - wallMouseOffset.x;
        int ydis = newmouse.y - vertexpos.y - wallMouseOffset.y;
        if (currEditorWallDir == 0)
        {
            if (Mathf.Abs(xdis) > 1)
            {
                if (xdis > 0)
                {
                    currStartWallIndex += 1;
                }
                else
                {
                    currStartWallIndex -= 1;
                }
                if (currStartWallIndex < 0)
                {
                    currStartWallIndex = 0;
                    return;
                }
                wallEditer.setEditorWallSize(ref currEditorWallDir, ref currStartWallIndex, ref currEditorCount);
                wallMouseOffset = newmouse - vertexpos;
            }
        }
        else
        {
            if (Mathf.Abs(ydis) > 1)
            {
                if (ydis > 0)
                {
                    currStartWallIndex += 1;
                }
                else
                {
                    currStartWallIndex -= 1;
                }
                if (currStartWallIndex < 0)
                {
                    currStartWallIndex = 0;
                    return;
                }
                wallEditer.setEditorWallSize(ref currEditorWallDir, ref currStartWallIndex, ref currEditorCount);
                wallMouseOffset = newmouse - vertexpos;
            }
        }
    }

    void wallEditerRotate()
    {
        currEditorWallDir = currEditorWallDir == 0 ? 1 : 0;

        currStartWallIndex = 0;

        wallEditer.setEditorWallSize(ref currEditorWallDir, ref currStartWallIndex, ref currEditorCount);
        var indoorsize = UserDataProxy.inst.GetIndoorSize();
        if (currEditorWallDir == 0)
        {
            D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(new Vector3Int((int)((indoorsize.xMax + indoorsize.xMin) * .5f) + StaticConstants.IndoorOffsetX, indoorsize.yMax, 0)), false, D2DragCamera.inst.maxZoom);
        }
        else
        {
            D2DragCamera.inst.LookToPosition(MapUtils.CellPosToWorldPos(new Vector3Int(indoorsize.xMax + StaticConstants.IndoorOffsetX, (int)((indoorsize.yMin + indoorsize.yMax) * .5f), 0)), false, D2DragCamera.inst.maxZoom);
        }
    }
    void wallEditVertexRDrag(Vector3 lastpos, Vector3 newpos)
    {
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(wallEditer.vertex_R.transform.position);
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);

        float xdis = currEditorWallDir == 0 ? (newmouse.x - vertexpos.x) : (newmouse.y - vertexpos.y);

        if (Mathf.Abs(xdis) > 1)
        {
            if (currEditorWallDir == 0)
            {
                if (xdis > 0)
                {
                    currEditorCount += 1;
                }
                else
                {
                    currEditorCount -= 1;
                    if (currEditorCount < 1)
                    {
                        currEditorCount = 1;
                        return;
                    }
                }
            }
            else
            {
                if (xdis > 0)
                {
                    currStartWallIndex += 1;
                    currEditorCount -= 1;
                    if (currEditorCount < 1)
                    {
                        currEditorCount = 1;
                        currStartWallIndex -= 1;
                        return;
                    }
                }
                else
                {
                    currStartWallIndex -= 1;
                    currEditorCount += 1;
                    if (currStartWallIndex < 0)
                    {
                        currStartWallIndex = 0;
                        currEditorCount -= 1;
                        return;
                    }
                }
            }
            wallEditer.setEditorWallSize(ref currEditorWallDir, ref currStartWallIndex, ref currEditorCount);
        }
    }
    void wallEditVertexLDrag(Vector3 lastpos, Vector3 newpos)
    {
        Vector3Int vertexpos = MapUtils.WorldPosToCellPos(wallEditer.vertex_L.transform.position);
        var world = Camera.main.ScreenToWorldPoint(newpos);
        Vector3Int newmouse = MapUtils.WorldPosToCellPos(world);
        float xdis = currEditorWallDir == 0 ? (newmouse.x - vertexpos.x) : (newmouse.y - vertexpos.y);
        if (Mathf.Abs(xdis) > 1)
        {
            if (currEditorWallDir == 0)
            {
                if (xdis > 0)
                {
                    currStartWallIndex += 1;
                    currEditorCount -= 1;
                    if (currEditorCount < 1)
                    {
                        currEditorCount = 1;
                        currStartWallIndex -= 1;
                        return;
                    }
                }
                else
                {
                    currStartWallIndex -= 1;
                    currEditorCount += 1;
                    if (currStartWallIndex < 0)
                    {
                        currStartWallIndex = 0;
                        currEditorCount -= 1;
                        return;
                    }
                }
            }
            else
            {
                if (xdis > 0)
                {
                    currEditorCount += 1;
                }
                else
                {
                    currEditorCount -= 1;
                    if (currEditorCount < 1)
                    {
                        currEditorCount = 1;
                        return;
                    }
                }
            }
            wallEditer.setEditorWallSize(ref currEditorWallDir, ref currStartWallIndex, ref currEditorCount);
        }
    }
}
