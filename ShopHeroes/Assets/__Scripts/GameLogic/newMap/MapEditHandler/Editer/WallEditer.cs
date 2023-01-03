using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WallEditer : MonoBehaviour
{
    public LineRenderer outLine;
    public Transform wallPanle;
    public PolygonCollider2D wallCollider;
    public GameObject wallGo;
    public Transform vertex_L;
    public Transform vertex_R;
    private List<GameObject> wallList = new List<GameObject>();
    [HideInInspector]
    public int wallPaperId;
    private int direction = 0;    //默认方向 0   左墙面.
    private RectInt defaultSize;
    void Start()
    {
    }

    private List<GameObject> lWallList = new List<GameObject>();
    private List<GameObject> rWallList = new List<GameObject>();
    public void initWallList(int wallid)
    {
        wallPaperId = wallid;
        defaultSize = UserDataProxy.inst.GetIndoorSize();
        wallGo.SetActive(false);

        defaultSize.x += StaticConstants.IndoorOffsetX;
        defaultSize.y += StaticConstants.IndoorOffsetY;
        //左侧
        for (int x = defaultSize.xMin; x < defaultSize.xMax; x += 2)
        {
            GameObject wall = GameObject.Instantiate(wallGo, wallPanle);
            setCellPos(wall.transform, x, defaultSize.yMax + 1, 0);
            wall.SetActive(false);
            lWallList.Add(wall);
        }
        //右侧
        for (int y = defaultSize.yMax - 1; y >= defaultSize.yMin; y -= 2)
        {
            GameObject wall = GameObject.Instantiate(wallGo, wallPanle);
            setCellPos(wall.transform, defaultSize.xMax + 1, y, 1);
            wall.SetActive(false);
            rWallList.Add(wall);
        }
        loadspriteindex = 1;
        setWallSprite();
    }

    int cStart = -1;
    int cCount = -1;
    int cDir = 0;
    public void setEditorWallSize(ref int dir, ref int start, ref int count)
    {
        if (cStart == -1) cStart = start;
        if (cCount == -1) cCount = count;
        if (cDir != dir)
        {
            cStart = start = 0;
            if (dir == 0)
            {
                if (count > lWallList.Count)
                {
                    cCount = count = lWallList.Count;
                }
            }
            else
            {
                if (count > rWallList.Count)
                {
                    cCount = count = lWallList.Count;
                }
            }
            cDir = dir;
        }
        if (dir == 0)
        {
            var num = start + count;
            if (num > lWallList.Count)
            {
                start = cStart;
                count = cCount;
                return;
            }
        }
        else
        {
            var num = start + count;
            if (num > rWallList.Count)
            {
                start = cStart;
                count = cCount;
                return;
            }
        }
        cStart = start;
        cCount = count;
        lWallList.ForEach(go =>
        {
            go.SetActive(false);
        });
        rWallList.ForEach(go =>
        {
            go.SetActive(false);
        });
        direction = dir;
        RectInt wallSize;
        List<Vector2> points = new List<Vector2>();
        if (dir == 0)
        {
            if ((start + count) > lWallList.Count)
            {
                count = lWallList.Count - start;
            }
            for (int i = 0; i < count; i++)
            {
                if (i < lWallList.Count)
                {
                    lWallList[start + i].SetActive(true);
                }
            }
            wallSize = new RectInt(defaultSize.xMin + start * 2, defaultSize.yMax + 1, count * 2, 0);
        }
        else
        {
            if ((start + count) > rWallList.Count)
            {
                count = rWallList.Count - start;
            }
            for (int i = 0; i < count; i++)
            {
                if (rWallList.Count - start - i - 1 >= 0)
                {
                    rWallList[rWallList.Count - start - i - 1].SetActive(true);
                }
            }
            wallSize = new RectInt(defaultSize.xMax + 1, defaultSize.yMin + start * 2, 0, count * 2);
        }
        Vector3 pos1 = MapUtils.CellPosToWorldPos(new Vector3Int(wallSize.x, wallSize.y, 0));
        Vector3 pos4 = MapUtils.CellPosToWorldPos(new Vector3Int(wallSize.xMax, wallSize.yMax, 0));
        outLine.SetPosition(0, pos1);
        points.Add(pos1);
        Vector3 pos2 = pos1 + Vector3.up * 1.8f;
        outLine.SetPosition(1, pos2);
        points.Add(pos2);
        Vector3 pos3 = pos4 + Vector3.up * 1.8f;
        outLine.SetPosition(2, pos3);
        points.Add(pos3);
        outLine.SetPosition(3, pos4);
        points.Add(pos4);
        wallCollider.SetPath(0, points);

        vertex_L.localPosition = (cDir == 0 ? pos1 : pos4) + Vector3.up * .9f;
        vertex_R.localPosition = (cDir == 0 ? pos4 : pos1) + Vector3.up * .9f;
    }
    void setCellPos(Transform walltf, int x, int y, int dir)
    {
        Vector3 pos = MapUtils.CellPosToWorldPos(new Vector3Int(x, y, 0));
        walltf.localPosition = pos;
        walltf.localScale = new Vector3(dir == 0 ? 1 : -1, 1, 1);
    }
    int loadspriteindex = 1;
    Sprite[] wallsprites = new Sprite[3];
    public void setWallSprite()
    {
        if (wallPaperId <= 0) return;
        if (loadspriteindex <= 3)
        {
            FurnitureConfig currFurniture = FurnitureConfigManager.inst.getConfig(wallPaperId);
            if (currFurniture != null)
            {
                string url = currFurniture.sprites + "_" + loadspriteindex.ToString();
                Addressables.LoadAssetAsync<Sprite>(url).Completed += OnLoaded;
            }
        }
        else
        {
            int index = 0;
            lWallList.ForEach(wall =>
            {
                lWallList[index].GetComponent<SpriteRenderer>().sprite = wallsprites[index % 3];
                index++;
            });
            index = 0;
            rWallList.ForEach(wall =>
            {
                rWallList[index].GetComponent<SpriteRenderer>().sprite = wallsprites[index % 3];
                index++;
            });
        }
    }

    private void OnLoaded(AsyncOperationHandle<Sprite> obj)
    {
        wallsprites[loadspriteindex - 1] = obj.Result;
        loadspriteindex++;
        setWallSprite();
    }
}
