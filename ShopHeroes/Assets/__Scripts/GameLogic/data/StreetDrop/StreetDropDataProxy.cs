using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum StreetDropState   // 0 未掉落  1 待捡起 2 已领取
{
    notDrop = 0,
    waitPick = 1,
    alreadyGet = 2,
}

//街道掉落物
public class StreetDropData
{
    public int uid;
    public Vector3Int dropPos;//掉落位置
    public StreetDropState state;
    public AccessoryData accessoryData;

    public StreetDropData(OneRubbish oneRubbish)
    {
        uid = oneRubbish.rubbishId;
        accessoryData = new AccessoryData(oneRubbish.item);
        dropPos = StreetDropDataProxy.inst.GetStreetDropRandomPos();//登录就收到了 indoorMapEditSys还未初始化 垃圾掉落位置缓存先放在dataProxy中
    }
}


public class StreetDropDataProxy : TSingletonHotfix<StreetDropDataProxy>, IDataModelProx
{

    int refreshTimer;
    Dictionary<int, StreetDropData> streetDropDic;

    List<Vector3Int> streetDropPosCache;//掉落物的位置缓存

    public void Init()
    {
        streetDropDic = new Dictionary<int, StreetDropData>();
        streetDropPosCache = new List<Vector3Int>();


        Helper.AddNetworkRespListener(MsgType.Response_Rubbish_List_Cmd, getRubbishListResp);
        Helper.AddNetworkRespListener(MsgType.Response_Rubbish_Refresh_Cmd, getRubbishRefreshResp);
        Helper.AddNetworkRespListener(MsgType.Response_Rubbish_Claimed_Cmd, getRubbishClaimedResp);
    }


    public Vector3Int GetStreetDropRandomPos()
    {
        Vector3Int result = Vector3Int.zero;

        for (int i = 0; i < 100; i++)
        {
            var cfg = StreetDropPosConfigManager.inst.GetRandomConfig();
            result = new Vector3Int(cfg.pos_x, cfg.pos_y, 0);
            if (!streetDropPosCache.Contains(result))
            {
                streetDropPosCache.Add(result);
                return result;
            }
        }

        return Vector3Int.zero;
    }

    public void DelStreetDropPosCache(Vector3Int key)
    {
        if (streetDropPosCache.Contains(key))
        {
            streetDropPosCache.Remove(key);
        }
    }

    void updataeStreetDropData(OneRubbish oneRubbish, StreetDropState state)
    {
        if (oneRubbish.item.itemId == 0) //说明要删除
        {

            if (state == StreetDropState.alreadyGet)
            {
                EventController.inst.TriggerEvent(GameEventType.StreetDropEvent.STREETDROP_DEAL, oneRubbish.rubbishId);
            }

            streetDropDic.Remove(oneRubbish.rubbishId);
        }
        else
        {
            StreetDropData data = new StreetDropData(oneRubbish);
            data.state = state;

            if (streetDropDic.ContainsKey(oneRubbish.rubbishId))
            {
            }
            else
            {
                streetDropDic.Add(data.uid, data);
            }

            EventController.inst.TriggerEvent(GameEventType.StreetDropEvent.STREETDROP_DATAREFRESH, data.uid);
        }

    }

    //残留的掉落物列表
    private void getRubbishListResp(HttpMsgRspdBase msg)
    {
        Response_Rubbish_List data = msg as Response_Rubbish_List;
        reSetRefreshTimer(data.nextRefreshTime);


        Dictionary<int, bool> map = new Dictionary<int, bool>();
        bool isHas = false;

        foreach (var item in data.rubbishList.rubbishList)
        {
            if (item.item.itemId == 0) continue;

            if (map.ContainsKey(item.rubbishId))
            {
                isHas = true;
                break;
            }
            else
            {
                map.Add(item.rubbishId, true);
            }
        }

        if (isHas)
        {
            string str = "服务器数据(去除要删除的id):";

            foreach (var item in data.rubbishList.rubbishList)
            {
                if (item.item.itemId == 0) continue;
                str += "  垃圾id  " + item.rubbishId;
            }

            str += "     ******** 客户端缓存数据(暂未同步)：";
            foreach (var item in streetDropDic.Keys)
            {
                str += "  垃圾id  " + item;
            }

            Logger.error("重复垃圾id！！ " + str);
        }


        foreach (var item in data.rubbishList.rubbishList)
        {
            updataeStreetDropData(item, StreetDropState.waitPick);
        }



    }

    //新的掉落物
    private void getRubbishRefreshResp(HttpMsgRspdBase msg)
    {
        Response_Rubbish_Refresh data = msg as Response_Rubbish_Refresh;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        reSetRefreshTimer(data.nextRefreshTime);

        updataeStreetDropData(data.oneRubbish, StreetDropState.notDrop);
    }

    //领取后的掉落物
    private void getRubbishClaimedResp(HttpMsgRspdBase msg)
    {
        Response_Rubbish_Claimed data = msg as Response_Rubbish_Claimed;
        reSetRefreshTimer(data.nextRefreshTime);

        updataeStreetDropData(data.oneRubbish, StreetDropState.alreadyGet);
    }


    void reSetRefreshTimer(int nextRefreshTime)
    {
        if (refreshTimer != 0)
        {
            GameTimer.inst.RemoveTimer(refreshTimer);
            refreshTimer = 0;
        }

        refreshTimer = GameTimer.inst.AddTimer(nextRefreshTime, 1, () =>
        {
            EventController.inst.TriggerEvent(GameEventType.StreetDropEvent.STREETDROP_REQUEST_REFRESH);
        });
    }

    public List<StreetDropData> GetAllStreetDropData()
    {
        return streetDropDic.Values.ToList();
    }

    public StreetDropData GetStreetDropData(int uid)
    {
        if (streetDropDic.ContainsKey(uid))
        {
            return streetDropDic[uid];
        }

        return null;
    }

    public void Clear()
    {
        refreshTimer = 0;
        if (streetDropDic != null)
            streetDropDic.Clear();
        if (streetDropPosCache != null)
            streetDropPosCache.Clear();
    }
}
