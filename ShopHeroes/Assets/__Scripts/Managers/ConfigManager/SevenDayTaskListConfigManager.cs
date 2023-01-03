using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SevenDayTaskListConfigData
{
    public int id;
    public string task_scenes;
    public int task_guide_1;
    public int task_guide_2;
    public int task_guide_3;
    public int task_guide_4;
    public int task_guide_5;
    public string dialog;
}

public class SevenDayTaskListConfigManager : TSingletonHotfix<SevenDayTaskListConfigManager>, IConfigManager
{
    public Dictionary<int, SevenDayTaskListConfigData> cfgList = new Dictionary<int, SevenDayTaskListConfigData>();
    public const string CONFIG_NAME = "seven_task_list";

    public void InitCSVConfig()
    {
        List<SevenDayTaskListConfigData> scArray = CSVParser.GetConfigsFromCache<SevenDayTaskListConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public SevenDayTaskListConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public SevenDayTaskListConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
