using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FurnitureSelectedGrid : MonoBehaviour
{
    public Transform grid;
    public Transform jiantou_UR;
    public Transform jiantou_DR;
    public Transform jiantou_UL;
    public Transform jiantou_DL;
    [HideInInspector]
    public int sizeX = 0;
    [HideInInspector]
    public int sizeY = 0;
    private List<Transform> gridList = new List<Transform>();

    int maxGridSize = 5;
    private bool isShow = false;
    void Awake()
    {
        grid.gameObject.SetActive(false);
        InitGridSize();
    }
    public void ChangeSize(int x, int y)
    {
        if (isShow)
            ShowGrid(x, y);
    }
    public void InitGridSize()
    {
        for (int x = 0; x < maxGridSize; x++)
        {
            for (int y = 0; y < maxGridSize; y++)
            {
                var tf = (x * maxGridSize + y) >= gridList.Count ? null : gridList[x * maxGridSize + y];
                if (tf == null)
                {
                    tf = GameObject.Instantiate(grid, transform);
                }
                Vector3 pos = MapUtils.CellPosToWorldPos(new Vector3Int(x, y, 0));
                tf.localPosition = pos;
                gridList.Add(tf);
            }
        }
        GridIsRed(false);
        HideGrid();
    }
    void OnEnable()
    {
        // ShowGrid();
    }
    void OnDisable()
    {
        // HideGrid();
    }

    public void ShowGrid(int sx, int sy)
    {
        sizeX = sx;
        sizeY = sy;
        HideGrid();
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                var tf = (x * maxGridSize + y) >= gridList.Count ? null : gridList[x * maxGridSize + y];
                if (tf)
                    tf.gameObject.SetActive(true);
            }
        }
        var p_l = IndoorMap.inst.gameMapGrid.CellToLocal(new Vector3Int(0, sizeY, 0));
        var p_u = IndoorMap.inst.gameMapGrid.CellToLocal(new Vector3Int(sizeX, sizeY, 0));
        var p_r = IndoorMap.inst.gameMapGrid.CellToLocal(new Vector3Int(sizeX, 0, 0));
        jiantou_DL.localPosition = p_l * .5f;
        jiantou_UL.localPosition = (p_l + p_u) * .5f;
        jiantou_DR.localPosition = p_r * .5f;
        jiantou_UR.localPosition = (p_r + p_u) * .5f;
        jiantou_UR.gameObject.SetActive(true);
        jiantou_DR.gameObject.SetActive(true);
        jiantou_UL.gameObject.SetActive(true);
        jiantou_DL.gameObject.SetActive(true);
        // grid.gameObject.SetActive(true);
        isShow = true;
    }
    public void HideGrid()
    {
        grid.gameObject.SetActive(false);
        gridList.ForEach(g =>
        {
            g.gameObject.SetActive(false);
        });
        jiantou_UR.gameObject.SetActive(false);
        jiantou_DR.gameObject.SetActive(false);
        jiantou_UL.gameObject.SetActive(false);
        jiantou_DL.gameObject.SetActive(false);
        isShow = false;
    }

    public void GridIsRed(bool isRed)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                var tf = (x * 6 + y) >= gridList.Count ? null : gridList[x * maxGridSize + y];
                if (tf != null)
                    tf.GetComponent<SpriteRenderer>().color = isRed ? Color.red : Color.green;
            }
        }
    }

    public void Setorder(string LayerName, int order)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                var tf = (x * maxGridSize + y) >= gridList.Count ? null : gridList[x * maxGridSize + y];
                if (tf != null)
                {
                    var render = tf.GetComponent<SpriteRenderer>();
                    render.sortingOrder = order;
                    render.sortingLayerName = LayerName;
                }
            }
        }
    }
}
