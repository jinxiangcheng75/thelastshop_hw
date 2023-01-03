using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideTriggerResultConfigData
{
    public int id;
    public int group;
    public string help_desc;
    public int index;
    public string desc;
    public int type;
    public string view_name;
    public string guide_btn_dev;
    public int operation_id;
    public int dialogue_id;
    public int next_id;
}

public class GuideTriggerResultConfigManager : TSingletonHotfix<GuideTriggerResultConfigManager>, IConfigManager
{
    public Dictionary<int, GuideTriggerResultConfigData> cfgList = new Dictionary<int, GuideTriggerResultConfigData>();
    public const string CONFIG_NAME = "guide_trigger_result";

    public void InitCSVConfig()
    {
        List<GuideTriggerResultConfigData> scArray = CSVParser.GetConfigsFromCache<GuideTriggerResultConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public GuideTriggerResultConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public GuideTriggerResultConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
