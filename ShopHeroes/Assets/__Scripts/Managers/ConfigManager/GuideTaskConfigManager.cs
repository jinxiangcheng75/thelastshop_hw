using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideTaskConfigData
{
    public int id;
    public int type;
    public int unlock_num;
    public string desc;
    public int guide_id;
    public int reward_type;
    public int reward_value;
}

public class GuideTaskConfigManager : TSingletonHotfix<GuideTaskConfigManager>, IConfigManager
{
    public Dictionary<int, GuideTaskConfigData> cfgList = new Dictionary<int, GuideTaskConfigData>();
    public const string CONFIG_NAME = "guide_task";

    public void InitCSVConfig()
    {
        List<GuideTaskConfigData> scArray = CSVParser.GetConfigsFromCache<GuideTaskConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public GuideTaskConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public GuideTaskConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
