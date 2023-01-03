using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AcheivementConfigData
{
    public int id;
    public int if_special;
    public int type;
    public int unlock_num;
    public int group;
    public int index;
    public int next_id;
    public string name;
    public string desc;
    public string condition_des;
    public int if_explore;
    public int condition_type;
    public long condition_num;
    public int item_type;
    public int reward_type;
    public int reward_num;
    public int reward_points;
    public string atlas;
    public string icon;
    public string frame;
}

public class AcheivementConfigManager : TSingletonHotfix<AcheivementConfigManager>, IConfigManager
{
    public Dictionary<int, AcheivementConfigData> cfgList = new Dictionary<int, AcheivementConfigData>();
    public const string CONFIG_NAME = "achievement";

    public void InitCSVConfig()
    {
        List<AcheivementConfigData> scArray = CSVParser.GetConfigsFromCache<AcheivementConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
    }

    public AcheivementConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public AcheivementConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public void ReLoadCSVConfig()
    {
        cfgList.Clear();
        InitCSVConfig();
    }
}
