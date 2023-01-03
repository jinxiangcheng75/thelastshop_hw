using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GuideTriggerConditionConfigData
{
    public int id;
    public int unlock_type;
    public int unlock_value;
    public int end_level;
    public int type;
    public int child_type;
    public int index;
    public string type_des;
    public int condition_type;
    public int condition_value;
    public string condition_des;
    public int result_id;
    public int connect_id;
}

public class GuideTriggerConditionConfigManagaer : TSingletonHotfix<GuideTriggerConditionConfigManagaer>, IConfigManager
{
    public Dictionary<int, GuideTriggerConditionConfigData> cfgList = new Dictionary<int, GuideTriggerConditionConfigData>();
    public const string CONFIG_NAME = "guide_trigger_conditions";

    public void InitCSVConfig()
    {
        List<GuideTriggerConditionConfigData> scArray = CSVParser.GetConfigsFromCache<GuideTriggerConditionConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public GuideTriggerConditionConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public GuideTriggerConditionConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
