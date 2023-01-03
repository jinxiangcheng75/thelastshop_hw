using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskMainConfigData
{
    public int id;
    public int pre_id;
    public int task_type;
    public string atlas;
    public string icon;
    public string des;
    public string des_2;
    public int condition_id;
    public int parameter_num;
    public int award_1_type;
    public int award_1_id;
    public int award_1_num;
    public int award_2_type;
    public int award_2_id;
    public int award_2_num;
    public string task_scenes;
    public int task_guide_1;
    public int task_guide_2;
    public int task_guide_3;
    public int task_guide_4;
    public int task_guide_5;
    public string dialog;
}

public class TaskMainConfigManager : TSingletonHotfix<TaskMainConfigManager>, IConfigManager
{
    public Dictionary<int, TaskMainConfigData> cfgList = new Dictionary<int, TaskMainConfigData>();
    public const string CONFIG_NAME = "task_main";

    public void InitCSVConfig()
    {
        List<TaskMainConfigData> scArray = CSVParser.GetConfigsFromCache<TaskMainConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public TaskMainConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public TaskMainConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
