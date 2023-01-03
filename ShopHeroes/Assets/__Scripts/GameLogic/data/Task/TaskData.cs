using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务系统  #陆泓屹
/// </summary>

public class TaskData
{
    //任务ID
    public int taskId;
    //任务类型
    public int taskType; //enum EDailyTaskType
    //当前的任务状态
    public int taskState; //enum EDailyTaskState
    //任务完成的目标物品ID
    public int taskTargetId;
    //当前完成的任务进度
    public int parameter_1;
    //任务需要完成的总进度
    public int parameter_2;
    //奖励的物品ID
    public int rewardId;
    //奖励的物品数量
    public int reward_number;


    public TaskItemConfig config;


    //感叹号提示是否显示
    public bool isShowNotice;
    //任务名称
    public string name;
    //任务图标
    public string icon;
    //图标所在的图集名
    public string atlas;

    public TaskData(OneDailyTask data)
    {

        this.taskId = data.taskId;
        this.taskType = data.taskType;
        this.taskState = data.taskState;
        this.taskTargetId = data.taskTarget;
        this.parameter_1 = data.taskProgress;
        this.parameter_2 = data.taskCondition;
        this.rewardId = data.rewardId;
        this.reward_number = data.rewardNumber;

        this.config = TaskItemConfigManager.inst.GetTaskConfig(data.taskId);
        this.name = config.name;

        switch ((EDailyTaskType)taskType)
        {
            case EDailyTaskType.SellItem:
            case EDailyTaskType.MakeItem:
                EquipDrawingsConfig equipConfig = EquipConfigManager.inst.GetEquipDrawingsCfg(this.taskTargetId);
                this.icon = equipConfig.icon;
                this.atlas = equipConfig.atlas;
                break;
            case EDailyTaskType.Double:
            case EDailyTaskType.Discount:
            case EDailyTaskType.Chat:
            case EDailyTaskType.Promote:
                this.icon = config.icon;
                this.atlas = config.atlas;
                break;
            case EDailyTaskType.SellAmount:
                EquipClassification equipTypeCfg = EquipConfigManager.inst.GetEquipTypeByID(this.taskTargetId);
                this.icon = equipTypeCfg.icon;
                this.atlas = equipTypeCfg.Atlas;
                break;

            case EDailyTaskType.ExploreItem:
                itemConfig cfg = ItemconfigManager.inst.GetConfig(this.taskTargetId);
                this.icon = cfg.icon;
                this.atlas = cfg.atlas;
                break;

            case EDailyTaskType.MarketSellGold:
            case EDailyTaskType.MarketSellEquip:
            case EDailyTaskType.ExploreHero:
            case EDailyTaskType.RefreshBar:
            case EDailyTaskType.BuildCost:
            case EDailyTaskType.ScienceCost:
            case EDailyTaskType.Gacha:
                this.icon = config.icon;
                this.atlas = config.atlas;
                break;

            default:
                break;
        }
    }
}

public enum ActiveRewardBoxState
{
    dontGet,
    canGet,
    alreadyGet
}

//活跃宝箱数据
public class ActiveRewardBoxData
{
    public int active_task_id;
    public int state;

    public ActiveTaskConfig config;
    public ActiveRewardData fixedAward;
    public List<ActiveRewardData> randomAwards;



    public ActiveRewardBoxData(OneActiveReward info)
    {
        SetInfo(info);
    }

    public void SetInfo(OneActiveReward info)
    {
        active_task_id = info.activeRewardId;
        state = info.activeRewardState;

        fixedAward = new ActiveRewardData(info.activeRewardItem.id, info.activeRewardItem.count, 0);

        randomAwards = new List<ActiveRewardData>();

        foreach (var item in info.activeRewardItemList)
        {
            randomAwards.Add(new ActiveRewardData(item.id, item.count, (float)item.chance / info.activeRewardItemLimit));
        }

        config = ActiveTaskConfigManager.inst.GetConfig(active_task_id);
    }
}

//活跃奖励数据
public class ActiveRewardData
{
    public int itemId;
    public int num;
    public float probability;
    public itemConfig itemConfig;

    public ActiveRewardData(int _itemId, int _num, float _probability)
    {
        itemId = _itemId;
        num = _num;
        probability = _probability;

        itemConfig = ItemconfigManager.inst.GetConfig(itemId);
    }

}
