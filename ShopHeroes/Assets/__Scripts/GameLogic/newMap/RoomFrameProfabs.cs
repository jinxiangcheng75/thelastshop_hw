using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class RoomFrameObj
{
    public string name;
    public GameObject gameobj;
}
public class RoomFrameProfabs : MonoBehaviour
{
    [SerializeField]
    List<RoomFrameObj> objList;
    public GameObject GetGameProfab(string name)
    {
        RoomFrameObj obj = objList.Find(item => item.name == name);
        if (obj != null)
        {
            return obj.gameobj;
        }
        return null;
    }
}
