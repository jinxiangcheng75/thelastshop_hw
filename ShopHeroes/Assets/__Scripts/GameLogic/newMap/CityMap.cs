using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMap : SingletonMono<CityMap>
{
    public Transform cameraStartpos;
    public CameraBounds cameraBounds;
    public List<CityHouse> houseList;

    [Header("城市prefab")]
    public GameObject AI_actor;
    public GameObject AI_man;
    public GameObject AI_woman;


    void Start()
    {
        D2DragCamera.inst.setCameraPositionAndSaveLastPos(new Vector3(1, -1, 0));
    }

}
