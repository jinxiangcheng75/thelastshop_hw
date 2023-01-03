using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideTriggerOperationConfigData
{
    public int id;
    public int type;
    public int if_strong;
    public string type_des;
    public int group;
    public string group_des;
    public int index;
    public string scene;
    public string main_interface;
    public string child_interface;
    public int if_btn;
    public string btn;
}

public class GuideTriggerOperationConfigManager : TSingletonHotfix<GuideTriggerOperationConfigManager>, IConfigManager
{
    public Dictionary<int, GuideTriggerOperationConfigData> cfgList = new Dictionary<int, GuideTriggerOperationConfigData>();
    public const string CONFIG_NAME = "seven_day_operations";

    public void InitCSVConfig()
    {
        List<GuideTriggerOperationConfigData> scArray = CSVParser.GetConfigsFromCache<GuideTriggerOperationConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public GuideTriggerOperationConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public GuideTriggerOperationConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
