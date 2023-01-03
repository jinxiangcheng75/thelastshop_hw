using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationData
{
    public K_Operation_DataType type;
    public List<int> operations = new List<int>();
    public string task_scenes;
    public int condition_id;
    public string dialog;
    public string offset;

    public void setData(K_Operation_DataType type, int cfgId)
    {
        this.type = type;
        operations.Clear();
        switch (type)
        {
            case K_Operation_DataType.MainLine:
                var cfg = TaskMainConfigManager.inst.GetConfig(cfgId);
                task_scenes = cfg.task_scenes;
                condition_id = cfg.condition_id;
                dialog = cfg.dialog;
                if (cfg.task_guide_1 != 0)
                {
                    operations.Add(cfg.task_guide_1);
                }
                if (cfg.task_guide_2 != 0)
                {
                    operations.Add(cfg.task_guide_2);
                }
                if (cfg.task_guide_3 != 0)
                {
                    operations.Add(cfg.task_guide_3);
                }
                if (cfg.task_guide_4 != 0)
                {
                    operations.Add(cfg.task_guide_4);
                }
                if (cfg.task_guide_5 != 0)
                {
                    operations.Add(cfg.task_guide_5);
                }
                break;
            case K_Operation_DataType.HyperLink:
                var helpLinkCfg = GameHelpNavigationConfigManager.inst.GetHelpLinkConfig(cfgId);
                task_scenes = helpLinkCfg.task_scenes;
                condition_id = helpLinkCfg.jump_val;
                dialog = "";
                if (helpLinkCfg.jump_id_1 != 0)
                {
                    operations.Add(helpLinkCfg.jump_id_1);
                }
                if (helpLinkCfg.jump_id_2 != 0)
                {
                    operations.Add(helpLinkCfg.jump_id_2);
                }
                if (helpLinkCfg.jump_id_3 != 0)
                {
                    operations.Add(helpLinkCfg.jump_id_3);
                }
                if (helpLinkCfg.jump_id_4 != 0)
                {
                    operations.Add(helpLinkCfg.jump_id_4);
                }
                if (helpLinkCfg.jump_id_5 != 0)
                {
                    operations.Add(helpLinkCfg.jump_id_5);
                }
                break;
            case K_Operation_DataType.SevenDay:
                var sevenDayCfg = SevenDayTaskConfigManger.inst.GetConfig(cfgId);
                var sevenDayTaskListCfg = SevenDayTaskListConfigManager.inst.GetConfig(sevenDayCfg.type);
                task_scenes = sevenDayTaskListCfg.task_scenes;
                condition_id = sevenDayCfg.parameter_1[0];
                dialog = sevenDayTaskListCfg.dialog;
                if (sevenDayTaskListCfg.task_guide_1 != 0)
                {
                    operations.Add(sevenDayTaskListCfg.task_guide_1);
                }
                if (sevenDayTaskListCfg.task_guide_2 != 0)
                {
                    operations.Add(sevenDayTaskListCfg.task_guide_2);
                }
                if (sevenDayTaskListCfg.task_guide_3 != 0)
                {
                    operations.Add(sevenDayTaskListCfg.task_guide_3);
                }
                if (sevenDayTaskListCfg.task_guide_4 != 0)
                {
                    operations.Add(sevenDayTaskListCfg.task_guide_4);
                }
                if (sevenDayTaskListCfg.task_guide_5 != 0)
                {
                    operations.Add(sevenDayTaskListCfg.task_guide_5);
                }
                break;
            case K_Operation_DataType.NewFunction:

                break;
        }
    }
}

public class MainlineData
{
    public TaskMainConfigData cfg;
    public int limit;
    public int param;
    public EMainTaskState state;
    public List<OneRewardItem> rewards = new List<OneRewardItem>();

    public void setData(OneMainTask data, List<OneRewardItem> list)
    {
        cfg = TaskMainConfigManager.inst.GetConfig(data.taskId);
        limit = data.taskLimit;
        param = data.taskParam;
        state = (EMainTaskState)data.taskState;
        rewards = list;
    }
}

public class MainLineDataProxy : TSingletonHotfix<MainLineDataProxy>, IDataModelProx
{
    private MainlineData data = null;

    public bool isReceiveNet = true;

    public MainlineData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
        }
    }

    private int mainTaskFlag;

    public bool MainTaskIsAllOver
    {
        get
        {
            return mainTaskFlag == 1;
        }

        private set { }
    }
    public void Clear()
    {
        data = null;
    }

    public void Init()
    {
        data = new MainlineData();
        Helper.AddNetworkRespListener(MsgType.Response_User_MainTask_Cmd, GetMainlineTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_User_MainTaskChange_Cmd, GetMainlineTaskChange);
        Helper.AddNetworkRespListener(MsgType.Response_User_MainTaskReward_Cmd, GetMainlineTaskReward);
    }

    private void GetMainlineTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_User_MainTask)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        this.data.setData(data.task, data.rewardList);
        mainTaskFlag = data.mainTaskFlag;

        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SHOWMAINLINEUI);
    }

    private void GetMainlineTaskChange(HttpMsgRspdBase msg)
    {
        var data = (Response_User_MainTaskChange)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        this.data.setData(data.task, data.rewardList);
        if (data.task.taskState == 2)
        {
            AudioManager.inst.PlaySound(111);
        }
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, true);
    }

    private void GetMainlineTaskReward(HttpMsgRspdBase msg)
    {
        var data = (Response_User_MainTaskReward)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        if (data.nowTask.taskId == 0 && data.mainTaskFlag != 1) return;
        //isReceiveNet = true;
        this.data.setData(data.nowTask, data.nowRewardList);
        mainTaskFlag = data.mainTaskFlag;

        if (data.rewardList.Count > 1)
        {
            List<CommonRewardData> tempList = new List<CommonRewardData>();
            for (int i = 0; i < data.rewardList.Count; i++)
            {
                if ((ItemType)data.rewardList[i].itemType != ItemType.Glod && (ItemType)data.rewardList[i].itemType != ItemType.Gem)
                {
                    CommonRewardData tempData = new CommonRewardData(data.rewardList[i].itemId, data.rewardList[i].count, data.rewardList[i].quality, data.rewardList[i].itemType);
                    tempList.Add(tempData);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.CREATFLOATPREFAB, data.rewardList[i].itemId, data.rewardList[i].count);
                }
            }

            if (tempList.Count > 0)
                EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONREWARD_SETINFO, tempList);
        }
        else
        {
            if (data.rewardList.Count > 0)
            {
                if ((ItemType)data.rewardList[0].itemType != ItemType.Glod && (ItemType)data.rewardList[0].itemType != ItemType.Gem)
                {
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, data.rewardList[0].itemId, data.rewardList[0].count));
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.CREATFLOATPREFAB, data.rewardList[0].itemId, data.rewardList[0].count);
                }
            }
            else
            {
                Logger.error("没有给到主线任务奖励数据");
            }
        }

        if (MainTaskIsAllOver)
        {
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI);
            return;
        }
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.NEWTASKPLAYANIM);
    }
}
