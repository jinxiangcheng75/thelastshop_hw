using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuideTriggerDialogConfigData
{
    public int id;
    public int artisan_item_id;
    public int pos;
    public string desc;
}

public class GuideTriggerDialogConfigManager : TSingletonHotfix<GuideTriggerDialogConfigManager>, IConfigManager
{
    public Dictionary<int, GuideTriggerDialogConfigData> cfgList = new Dictionary<int, GuideTriggerDialogConfigData>();
    public const string CONFIG_NAME = "guide_trigger_dialogue";

    public void InitCSVConfig()
    {
        List<GuideTriggerDialogConfigData> scArray = CSVParser.GetConfigsFromCache<GuideTriggerDialogConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public GuideTriggerDialogConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public GuideTriggerDialogConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
