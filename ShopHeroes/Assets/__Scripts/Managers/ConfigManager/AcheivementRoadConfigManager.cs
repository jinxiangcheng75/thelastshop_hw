using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AcheivementRoadConfigData
{
    public int id;
    public int need_point;
    public int index;
    public int item1_type;
    public int reward1_type;
    public int reward1_num;
    public int item2_type;
    public int reward2_type;
    public int reward2_num;
    public int next_id;
    public string name;
    public string altas;
    public string frame;
    public int level_color;
}

public class AcheivementRoadConfigManager : TSingletonHotfix<AcheivementRoadConfigManager>, IConfigManager
{
    public Dictionary<int, AcheivementRoadConfigData> cfgList = new Dictionary<int, AcheivementRoadConfigData>();
    public const string CONFIG_NAME = "achievement_road";

    public void InitCSVConfig()
    {
        List<AcheivementRoadConfigData> scArray = CSVParser.GetConfigsFromCache<AcheivementRoadConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public AcheivementRoadConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public AcheivementRoadConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public AcheivementRoadConfigData GetLastConfig()
    {
        return cfgList.Values.ToList()[cfgList.Count - 1];
    }

    public AcheivementRoadConfigData GetConfigByIndex(int index)
    {
        return cfgList.Values.ToList().Find(t => t.index == index);
    }
}
