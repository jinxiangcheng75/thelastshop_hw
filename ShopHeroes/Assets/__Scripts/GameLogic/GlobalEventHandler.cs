using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件 处理
/// </summary>
public class GlobalEventHandler
{
    public GlobalEventHandler()
    {
        AddListeners();
    }
    public void AddListeners()
    {

    }

    public void GameSystemDataChange(Response_Heartbeat data)
    {
        //聊天
        if (data.newChatEvent == 1)
        {
            //聊天数据变化，刷新数据或者红点显示
            EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_UPDATE_NewChatData);
        }
        //新邮件数据
        if (data.newMailEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_DATA);
        }

        //建筑信息变化
        if (data.newUnionBuildEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.CITYBUILDING_GET_DATA);
            EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYCHECK);
        }

        //公会基础信息变化
        if (data.newUnionEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_DATA, "");
        }

        //公会科技信息变化
        if (data.newUnionScienceEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_HAVENEWSCIENCEDATA);
        }

        //公会科技技能buff变化
        if (data.newUnionSkillEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_SKILLLIST);
        }

        //公会援助变化
        if (data.newUnionHelpEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_HAVENEWHELPDATA);
        }

        //公会悬赏任务变化
        if (data.newUnionTaskEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_HAVENEWTASKDATA);
        }

        //设计家具解锁数据变化
        if (data.newDesignEvent == 1)
        {

        }

        //市场摊位数据发生变化
        if (data.newMarketEvent == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_REDPOINT_HAVENEWBOOTHDATA);
        }

        // 有新的系统消息
        if(data.newSystemMsgEvent == 1)
        {
            //聊天数据变化，刷新数据或者红点显示
            EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_UPDATE_NewChatData);
        }


        CallLuaGlobalHeartbeatEvent(data);
        EventController.inst.TriggerEvent(GameEventType.UpdateGameRedPoints);
    }

    public void CallLuaGlobalHeartbeatEvent(Response_Heartbeat data)
    {
        HotfixBridge.inst.OncallLuaGlobalHeartbeatEvent(JsonUtility.ToJson(data));
    }
}
