using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//全局buff
public class GlobalBuffDataProxy : TSingletonHotfix<GlobalBuffDataProxy>, IDataModelProx
{

    List<GlobalBuffData> globalBuffDatas;
    //public int buffUid;

    public void Init()
    {
        globalBuffDatas = new List<GlobalBuffData>();


        Helper.AddNetworkRespListener(MsgType.Response_Activity_Buff_Info_Cmd, getGlobalBuffDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Activity_Buff_Refresh_Cmd, getGlobalBuffRefreshDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Activity_Buff_Update_Cmd, getGlobalBuffUpdateDataResp);


        ////测试
        //for (int i = 0; i < 3; i++)
        //{
        //    GlobalBuffData data = new GlobalBuffData(i + 1 + 100, new ActivityBuff() { activityTime = 3000, buffType = Random.Range(1, 14) });
        //    globalBuffDatas.Add(data);
        //}

    }


    void getGlobalBuffDataResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Activity_Buff_Info;

        foreach (var item in data.activityBuffList)
        {
            updateGlobalBuffData(item);
        }
        // serverTime = data.serverTime;
        //buffUid = data.uid;

        EventController.inst.TriggerEvent(GameEventType.GlobalBuffEvent.GLOBALBUFF_REFRESHUI_BUFFITEM);
    }

    void getGlobalBuffRefreshDataResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Activity_Buff_Refresh;

        foreach (var item in data.activityBuffList)
        {
            updateGlobalBuffData(item);
        }

        // serverTime = data.serverTime;
        //buffUid = data.uid;

        EventController.inst.TriggerEvent(GameEventType.GlobalBuffEvent.GLOBALBUFF_REFRESHUI_BUFFITEM);
    }

    void getGlobalBuffUpdateDataResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Activity_Buff_Update;

        if (data.activityBuff.buffId == 0 && data.activityBuff.buffType == 0)
        {
            Logger.error("服务器返回了错误的buff信息");
            return;
        }

        updateGlobalBuffData(data.activityBuff);
        // serverTime = data.serverTime;
        //buffUid = data.uid;

        EventController.inst.TriggerEvent(GameEventType.GlobalBuffEvent.GLOBALBUFF_REFRESHUI_BUFFITEM);
    }

    public GlobalBuffData GetGlobalBuffData(GlobalBuffType buffType)
    {
        return globalBuffDatas.Find((t) => t.buffType == buffType && t.remainTime > 0);
    }

    public List<GlobalBuffData> GetAllGlobalBuffData()
    {
        return globalBuffDatas;
    }


    public void ClearOneGlobalBuffData(int buffuId) 
    {
        GlobalBuffData data = globalBuffDatas.Find((t) => t.buffuId == buffuId);

        if (data != null)
        {
            data.ClearTimer();
            globalBuffDatas.Remove(data);
            EventController.inst.TriggerEvent(GameEventType.GlobalBuffEvent.GLOBALBUFF_DELBUFFITEM, data.buffuId);
        }
    }


    void updateGlobalBuffData(ActivityBuff buffInfo)
    {
        GlobalBuffData data = globalBuffDatas.Find((t) => t.buffuId == buffInfo.buffId);

        if (buffInfo.buffState == (int)EActivityBuffState.Done)
        {
            ClearOneGlobalBuffData(buffInfo.buffId);
        }
        else
        {
            if (data == null)
            {
                data = new GlobalBuffData(buffInfo.buffId, buffInfo);
                globalBuffDatas.Add(data);
            }
            else
            {
                data.SetInfo(buffInfo.buffId, buffInfo);
            }
        }
    }


    public void Clear()
    {
        if (globalBuffDatas != null) globalBuffDatas.Clear();
        globalBuffDatas = null;
    }

}
