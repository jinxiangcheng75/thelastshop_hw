using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class IndoorScene : MonoBehaviour
{
    //室内物品节点
    public Transform entityRootTF;
    //寻路层
    public Tilemap pathGrid;
    //室内网格
    public Tilemap indoorGridLine;
    public CameraBounds cameraBounds;
    [Header("室内prefab")]
    public GameObject floor;
    public GameObject wall_L;
    public GameObject wall_R;
    public GameObject IndoorPillar_01;
    public GameObject IndoorPillar_02;
    public GameObject IndoorPillar_03;
    public GameObject EntityPrefab;
    public GameObject UpGradeAttacher;
    public GameObject roleAttacher;
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    private Dictionary<int, Floor> floorGOList;       //地板 
    private Dictionary<int, WallBase> wallGOList;  //墙体
    private Dictionary<int, Furniture> indoorFurnitures;    //室内家具装饰
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public List<Transform> roleStartPos;
    public List<Transform> roleTargetPos;
}
