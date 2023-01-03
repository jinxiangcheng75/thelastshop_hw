using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FloorEditer : MonoBehaviour
{
    public LineRenderer outLine;
    public Transform floorPanle;
    public PolygonCollider2D floorCollider;
    public GameObject floorGo;
    public Transform vertex_L;
    public Transform vertex_T;
    public Transform vertex_R;
    public Transform vertex_D;
    public Transform sp_skin;
    private RectInt defaultSize;
    //x 0--40 y 0--40
    private List<GameObject> floorList = new List<GameObject>();
    void Start()
    {
        floorGo.SetActive(false);
    }
    public void setSize(ref RectInt size)
    {
        var mapsize = UserDataProxy.inst.GetIndoorSize();
        if (size.xMin < mapsize.xMin + StaticConstants.IndoorOffsetX || size.yMin < mapsize.yMin || size.xMax >= mapsize.xMax + StaticConstants.IndoorOffsetX + 2 || size.yMax >= mapsize.yMax + 2)
        {
            size = defaultSize;
            return;
        }
        if (size.xMin >= size.xMax || size.yMin >= size.yMax)
        {
            size = defaultSize;
            return;
        }

        defaultSize = size;
        floorList.ForEach(go =>
        {
            go.SetActive(false);
        });
        int index = 0;
        for (int x = size.xMin; x < size.xMax; x += 2)
        {
            for (int y = size.yMin; y < size.yMax; y += 2)
            {
                GameObject floor;
                if (index < floorList.Count)
                {
                    floor = floorList[index];
                    floor.SetActive(true);
                }
                else
                {
                    floor = GameObject.Instantiate(floorGo, floorPanle);
                    floor.SetActive(true);
                    floorList.Add(floor);
                }
                setCellPos(floor.transform, x, y);
                index++;
            }
        }
        List<Vector2> points = new List<Vector2>();
        Vector3 pos1 = MapUtils.CellPosToWorldPos(new Vector3Int(size.xMin, size.yMin, 0));
        outLine.SetPosition(0, pos1);
        vertex_D.localPosition = pos1;
        points.Add(pos1);
        Vector3 pos2 = MapUtils.CellPosToWorldPos(new Vector3Int(size.xMin, size.yMax, 0));
        outLine.SetPosition(1, pos2);
        vertex_L.localPosition = pos2;
        points.Add(pos2);
        Vector3 pos3 = MapUtils.CellPosToWorldPos(new Vector3Int(size.xMax, size.yMax, 0));
        outLine.SetPosition(2, pos3);
        vertex_T.localPosition = pos3;
        points.Add(pos3);
        Vector3 pos4 = MapUtils.CellPosToWorldPos(new Vector3Int(size.xMax, size.yMin, 0));
        outLine.SetPosition(3, pos4);
        vertex_R.localPosition = pos4;
        points.Add(pos4);
        floorCollider.SetPath(0, points);

        Vector3 centerPos = MapUtils.CellPosToWorldPos(new Vector3Int((int)size.center.x, (int)size.center.y, 0));
        sp_skin.localPosition = centerPos;

        // setFloorSprite();
    }
    void setCellPos(Transform floortf, int x, int y)
    {
        Vector3 pos = MapUtils.CellPosToWorldPos(new Vector3Int(x, y, 0));
        floortf.localPosition = pos;
    }
    [HideInInspector]
    public int floorId = 0;
    public void setFloorSprite(int id)
    {
        floorId = id;
        if (floorId <= 0) return;
        FurnitureConfig currFurniture = FurnitureConfigManager.inst.getConfig(floorId);
        if (currFurniture != null)
            Addressables.LoadAssetAsync<Sprite>(currFurniture.sprites).Completed += OnLoaded;
    }

    private void OnLoaded(AsyncOperationHandle<Sprite> obj)
    {
        Sprite floorSprite = obj.Result;
        floorGo.GetComponent<SpriteRenderer>().sprite = floorSprite;
        floorList.ForEach(floorobj =>
        {
            floorobj.GetComponent<SpriteRenderer>().sprite = floorSprite;
        });
    }
}
