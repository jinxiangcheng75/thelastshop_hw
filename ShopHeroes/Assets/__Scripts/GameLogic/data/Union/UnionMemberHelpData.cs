using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionMemberHelpData
{

    public OneHelpData severData;
    private int cooltime = 0;
    private int timerId;

    public int remainTime
    {
        get { return cooltime - (int)GameTimer.inst.serverNow; }
    }

    public UnionMemberHelpData(OneHelpData _severData)
    {
        this.severData = _severData;
        cooltime = (int)GameTimer.inst.serverNow + _severData.stateEndTime;
        setTimer();
    }
    public void ClearTimer()
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    void setTimer()
    {
        ClearTimer();
        timerId = GameTimer.inst.AddTimer(remainTime, 1, timerMethod, GameTimerType.byServerTime);
    }

    void timerMethod()
    {
        if (remainTime <= 0)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_MEMBERHELPLIST);
            ClearTimer();
        }
        else
        {
            timerId = GameTimer.inst.AddTimer(remainTime, 1, timerMethod, GameTimerType.byServerTime);
        }
    }

}


public class UnionTaskData
{
    public OneUnionTaskData data;
    private int timerId;
    private int endTime;
    public int remainTime
    {
        get { return endTime - (int)GameTimer.inst.serverNow; }
    }
    public UnoinTaskConfig config;
    public ArtisanNPCConfigData npcConfig;
    public itemConfig selfAwardConfig;
    public itemConfig unionAwardConfig;
    public string atlas;
    public string icon;

    string strParam_2;
    public string StrParam_2
    {
        get
        {
            string str = string.Empty;

            if (config.big_type == (int)EUnionTaskType.EquipSoldUp)
            {
                str = LanguageManager.inst.GetValueByKey("{0}", data.taskTargetId.ToString());
            }
            else if (config.big_type == (int)EUnionTaskType.EquipSoldDown)
            {
                str = LanguageManager.inst.GetValueByKey("{0}", data.taskTargetId.ToString());
            }
            else
            {
                str = string.IsNullOrEmpty(strParam_2) ? "" : LanguageManager.inst.GetValueByKey(strParam_2);
            }

            return str;
        }
    }

    public UnionTaskData(OneUnionTaskData _severData)
    {
        SetInfo(_severData);
    }

    public void SetInfo(OneUnionTaskData _severData)
    {
        data = _severData;
        endTime = _severData.endTime + (int)GameTimer.inst.serverNow;
        setTimer();

        config = UnionTaskConfigManager.inst.GetConfig(data.taskId);

        if (config != null)
        {
            npcConfig = ArtisanNPCConfigManager.inst.GetConfig(config.npc_id);
            selfAwardConfig = ItemconfigManager.inst.GetConfig(config.reward_type);
            unionAwardConfig = ItemconfigManager.inst.GetConfig(config.union_reward_type);

            setAtalsAndIcon();
        }
        else
        {
            Logger.error("联盟悬赏任务配置表 未读取到该id ： " + data.taskId);
        }

    }

    void setAtalsAndIcon()
    {
        if (config.big_type == (int)EUnionTaskType.UnionGemHelp || config.big_type == (int)EUnionTaskType.EquipSoldUp || config.big_type == (int)EUnionTaskType.EquipSoldDown)
        {
            atlas = config.atlas;
            icon = config.icon;
        }
        else
        {
            if (data.taskTargetId != 0)
            {
                if (config.big_type == (int)EUnionTaskType.Equip) //制作
                {
                    EquipDrawingsConfig equipConfig = EquipConfigManager.inst.GetEquipDrawingsCfg(data.taskTargetId);
                    this.atlas = equipConfig.atlas;
                    this.icon = equipConfig.icon;
                    this.strParam_2 = equipConfig.name;
                }
                else if (config.big_type == (int)EUnionTaskType.OpenBox) //宝箱 
                {
                    itemConfig itemConfig = ItemconfigManager.inst.GetConfig(data.taskTargetId);
                    this.atlas = itemConfig.atlas;
                    this.icon = itemConfig.icon;
                    strParam_2 = itemConfig.name;
                }
                else if (config.big_type == 4) // 收集材料 
                {
                    itemConfig cfg = ItemconfigManager.inst.GetConfig(data.taskTargetId);
                    this.atlas = cfg.atlas;
                    this.icon = cfg.icon;
                    strParam_2 = cfg.name;
                }
                else if (config.big_type == 7) //探索副本 
                {
                    ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfig(data.taskTargetId);
                    atlas = StaticConstants.exploreAtlas;
                    icon = cfg.instance_icon;
                    strParam_2 = cfg.instance_name;
                }
            }

            strParam_2 = LanguageManager.inst.GetValueByKey(strParam_2);
        }
    }

    public void ClearTimer()
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        config = null;
    }

    void setTimer()
    {
        ClearTimer();

        if (data.state == (int)EUnionTaskState.Idle)
        {
            return;
        }

        timerId = GameTimer.inst.AddTimer(remainTime, 1, timerMethod, GameTimerType.byServerTime);
    }

    void timerMethod()
    {
        if (remainTime > 0)
        {
            timerId = GameTimer.inst.AddTimer(remainTime, 1, timerMethod, GameTimerType.byServerTime);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_TASKLIST);
        }
    }

}


public class UnionScienceData
{
    public OneUnionScienceData serverData;
    public UnionTechnologyConfig config;
    public UnionTechnologyConfig nextLvCfg;
    public UnionLevelConfig unionLvCfg;
    public UnionLevelConfig unionNextLvCfg;
    public int state;// 0未解锁 1可解锁 2不可升级 3可升级


    public UnionScienceData(OneUnionScienceData data)
    {
        SetInfo(data);
    }

    public void SetInfo(OneUnionScienceData data)
    {
        serverData = data;
        config = UnionTechnologyConfigManager.inst.GetConfig(data.type, Mathf.Max(1, data.level));
        nextLvCfg = UnionTechnologyConfigManager.inst.GetConfig(data.type, data.level + 1);


        if (serverData.type == (int)EUnionScienceType.UnionLevelUp) //本身就是联盟等级科技
        {
            state = 3;
            unionLvCfg = UnionLevelConfigManager.inst.GetConfig(Mathf.Max(1, data.level));
            unionNextLvCfg = UnionLevelConfigManager.inst.GetConfig(data.level + 1);
        }
        else
        {
            if (config == null)
            {
                state = 2;
                return;
            }

            int unionLevel = UserDataProxy.inst.UnionLevel;


            if (serverData.level == 0)
            {
                if (config.need_level > unionLevel)
                {
                    state = 0;
                }
                else
                {
                    state = 1;
                }
            }
            else
            {
                if (nextLvCfg != null)
                {
                    if (nextLvCfg.need_level <= unionLevel)
                    {
                        state = 3;
                    }
                    else
                    {
                        state = 2;
                    }
                }
                else
                {
                    state = 2;
                }

            }
        }

    }
}

public class UnionBuffData
{
    public OneUnionScienceSkillData serverData;
    int timerId;
    int endTime;
    public int remainTime
    {
        get { return endTime - (int)GameTimer.inst.serverNow; }
    }
    public UnionTechnologyConfig config
    {
        get
        {
            return UserDataProxy.inst.getUnionScienceDataByType(serverData.type).config;
        }

    }


    public UnionBuffData(OneUnionScienceSkillData data)
    {
        SetInfo(data);
    }

    public void SetInfo(OneUnionScienceSkillData data)
    {
        serverData = data;
        endTime = data.time + (int)GameTimer.inst.serverNow;
        setTimer();
    }

    public void ClearTimer()
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    void setTimer()
    {
        ClearTimer();

        if (remainTime <= 0) return;

        timerId = GameTimer.inst.AddTimer(remainTime, 1, timerMethod, GameTimerType.byServerTime);
    }

    void timerMethod()
    {

        if (remainTime > 0)
        {
            timerId = GameTimer.inst.AddTimer(remainTime, 1, timerMethod, GameTimerType.byServerTime);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_SKILLREFRESH, serverData.type);
        }


    }

}
