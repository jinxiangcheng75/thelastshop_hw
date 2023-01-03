using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 任务系统  #陆泓屹
/// </summary>

public class TaskItemConfig
{
    //任务ID
    public int id;
    //任务名称
    public string name;
    //任务图标
    public string icon;
    //图标所在的图集名
    public string atlas;
    //任务类型
    public int type;
    //当前完成的任务进度
    public int parameter_1;
    //任务需要完成的总进度
    public int parameter_2;
    //奖励的物品ID
    public int reward;
    //奖励的物品数量
    public int reward_number;
    
    public string task_scenes;
    public int task_guide_1;
    public int task_guide_2;
    public int task_guide_3;
    public int task_guide_4;
    public int task_guide_5;
    public string dialog;
}

public class TaskItemConfigManager : TSingletonHotfix<TaskItemConfigManager>, IConfigManager
{
    public Dictionary<int, TaskItemConfig> cfgList = new Dictionary<int, TaskItemConfig>();
    public const string CONFIG_NAME = "task_item";

    public void InitCSVConfig()
    {
        List<TaskItemConfig> scArray = CSVParser.GetConfigsFromCache<TaskItemConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in scArray)
        {
            if (sc.id <= 0) continue;
            cfgList.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public TaskItemConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public TaskItemConfig GetTaskConfig(int key)
    {
        if (cfgList.ContainsKey(key))
        {
            return cfgList[key];
        }
        return null;
    }
}
