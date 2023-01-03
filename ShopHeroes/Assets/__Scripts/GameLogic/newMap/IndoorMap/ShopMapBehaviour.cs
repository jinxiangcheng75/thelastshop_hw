using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShopMapBehaviour : MonoBehaviour
{
    //地图网格，用来计算网格和坐标
    [SerializeField]
    private Tilemap gameMapGrid;
    [SerializeField]
    private TileBase tileGridItem_W;
    [SerializeField]
    private TileBase tileGridItem_R;

    //寻路地图(-40,-40)==>(30,46)
    [SerializeField]
    private Tilemap mapPathGrid;

    //场景
    public Transform mapRootNode;

}
