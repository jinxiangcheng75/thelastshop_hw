using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveTaskConfig
{
    public int id;
    public int need_point;
    public string atlas_point;
    public string icon_point;
    public int vip_on;
    public int reward1_type;
    public int reward1_id;
    public int reward1_num;
    public int reward2_type;
    public int reward2_id;
    public int reward2_num;
}

public class ActiveTaskConfigManager : TSingletonHotfix<ActiveTaskConfigManager>, IConfigManager
{

    public Dictionary<int, ActiveTaskConfig> activeTaskDic = new Dictionary<int, ActiveTaskConfig>();

    public const string CONFIG_NAME = "active_task";

    public void InitCSVConfig()
    {
        List<ActiveTaskConfig> buildingArr = CSVParser.GetConfigsFromCache<ActiveTaskConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingArr)
        {
            if (sc.id <= 0) continue;
            activeTaskDic.Add(sc.id, sc);
        }

    }
    public void ReLoadCSVConfig()
    {
        activeTaskDic.Clear();
        InitCSVConfig();
    }
    public ActiveTaskConfig GetConfig(int key)
    {
        if (activeTaskDic.ContainsKey(key))
        {
            return activeTaskDic[key];
        }
        return null;
    }

}
