using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
public struct MapUtils
{
    //zfs
    //获得地块中心坐标
    public static Vector3 CellPosToCenterPos(Vector3Int cell)
    {
        var pos = CellPosToWorldPos(cell);
        pos.y += 0.25f;
        return pos;
    }

    //zfs
    public static Vector3Int IndoorCellposToMapCellPos(Vector3Int local)
    {
        return new Vector3Int(local.x + StaticConstants.IndoorOffsetX, local.y + StaticConstants.IndoorOffsetY, local.z);
    }
    public static Vector3Int MapCellPosToIndoorCellpos(Vector3Int local)
    {
        return new Vector3Int(local.x - StaticConstants.IndoorOffsetX, local.y - StaticConstants.IndoorOffsetY, local.z);
    }
    //前提 CellLayout == Isometric Z AS Y 
    public static Vector3 CellPosToWorldPos(Vector3Int cell)
    {
        Vector3 size = StaticConstants.CellSize;
        Vector3 pos = Vector3.zero;
        pos.x = (cell.x - cell.y) * size.x * .5f;
        pos.y = (cell.x + cell.y) * size.y * .5f;
        return pos;
    }
    public static Vector3Int WorldPosToCellPos(Vector3 pos)
    {
        Vector3 size = StaticConstants.CellSize;
        Vector3Int cell = Vector3Int.zero;
        var _x = pos.x * 2f / size.x;
        var _y = pos.y * 2f / size.y;
        cell.x = Mathf.FloorToInt((_x + _y) * .5f);
        cell.y = Mathf.FloorToInt((_y - _x) * .5f);
        return cell;
    }

    //根据Y值取出新的order
    public static int GetTileMapOrder(float y, float x, int sizeX, int sizeY)
    {
        // float offsetx = 0.5f;
        // float offsety = 0.25f;
        // float _cx = 0.5f * sizeX;
        // float _cy = 0.5f * sizeY;
        // {
        //     offsetx *= sizeY >= sizeX ? -sizeY : sizeX;
        //     offsety *= sizeY >= sizeX ? sizeY : sizeX;
        // }
        var pos = CellPosToWorldPos(new Vector3Int(sizeX > 2 ? 2 : sizeX, sizeY > 2 ? 2 : sizeY, 0)) * 0.5f;
        y += pos.y;
        x += pos.x;
        return 10000 - (int)(y * 250f) - (int)(x * 24);
    }
}

