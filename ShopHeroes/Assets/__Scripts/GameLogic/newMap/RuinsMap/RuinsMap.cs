using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinsMap : SingletonMono<RuinsMap>
{
    public float maxX
    {
        get
        {
            if(FGUI.inst != null)
            {
                if (FGUI.inst.isLandscape)
                {
                    return 3.29f;
                }
                else
                {
                    return 5.7f;
                }
            }
            else
            {
                return 5.7f;
            }
        }
    }
    public float minX
    {
        get
        {
            if (FGUI.inst != null)
            {
                if (FGUI.inst.isLandscape)
                {
                    return -6.18f;
                }
                else
                {
                    return -8.58f;
                }
            }
            else
            {
                return -8.58f;
            }
        }
    }
    public float maxY
    {
        get
        {
            if (FGUI.inst != null)
            {
                if (FGUI.inst.isLandscape)
                {
                    return 6.37f;
                }
                else
                {
                    return 3.97f;
                }
            }
            else
            {
                return 3.97f;
            }
        }
    }
    public float minY
    {
        get
        {
            if (FGUI.inst != null)
            {
                if (FGUI.inst.isLandscape)
                {
                    return -7.91f;
                }
                else
                {
                    return -5.5f;
                }
            }
            else
            {
                return -5.5f;
            }
        }
    }
    public CameraBounds cameraBounds;
    public List<RuinsHouse> houseList = new List<RuinsHouse>();
    private Dictionary<int, LuaListItem> houseDic = new Dictionary<int, LuaListItem>();
    private bool updateState = true;
    private bool isInMain = false;
    private Transform curTargetHouse;
    float addTime = 0;

    private void Start()
    {

    }

    public void setHouseDic(int houseId, LuaListItem hud)
    {
        if (houseDic != null)
        {
            if (houseDic.ContainsKey(houseId))
            {
                houseDic[houseId] = hud;
            }
            else
            {
                houseDic.Add(houseId, hud);
            }
        }
    }

    public void setCurHouse(int houseId)
    {
        for (int i = 0; i < houseList.Count; i++)
        {
            if (houseList[i].houseId == houseId)
            {
                curTargetHouse = houseList[i].transform;
                break;
            }
        }
    }

    public void setIsInMain(bool state)
    {
        isInMain = state;
    }

    public Vector3 GetClampPos(Vector3 tempPos)
    {
        if (tempPos.x > RuinsMap.inst.maxX)
        {
            tempPos.x = RuinsMap.inst.maxX;
        }
        else if (tempPos.x < RuinsMap.inst.minX)
        {
            tempPos.x = RuinsMap.inst.minX;
        }
        else if (tempPos.y > RuinsMap.inst.maxY)
        {
            tempPos.y = RuinsMap.inst.maxY;
        }
        else if (tempPos.y < RuinsMap.inst.minY)
        {
            tempPos.y = RuinsMap.inst.minY;
        }

        return tempPos;
    }

    public void setUpdateState(bool state)
    {
        if (!state)
            addTime = 0;
        updateState = state;
    }

    private void Update()
    {
        if (isInMain && updateState)
        {
            addTime += Time.deltaTime;

            if (addTime >= 10)
            {
                addTime = 0;
                if (curTargetHouse != null)
                {
                    var curPos = GetClampPos(curTargetHouse.position);
                    D2DragCamera.inst.LookToPosition(curPos);
                    updateState = false;
                }
            }
        }
    }
}
