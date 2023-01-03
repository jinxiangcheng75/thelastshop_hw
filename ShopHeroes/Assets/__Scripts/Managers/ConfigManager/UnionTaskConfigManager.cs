using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnoinTaskConfig
{
    public int id;
    public string name;
    public string desc;
    public int if_special;
    public int difficulty;
    public int big_type;
    public int Equip_type;
    public int minus_level;
    public int type;
    public string type_desc;
    public string desc_help;
    public int parameter_2;
    public int reward_type;
    public int reward_number;
    public int union_reward_type;
    public int union_reward_num;
    public int level;
    public string atlas;
    public string icon;
    public int npc_id;
}

public class UnionTaskConfigManager : TSingletonHotfix<UnionTaskConfigManager>, IConfigManager
{
    public Dictionary<int, UnoinTaskConfig> unionTaskPopularityDic = new Dictionary<int, UnoinTaskConfig>();

    public const string CONFIG_NAME = "union_task";

    public void InitCSVConfig()
    {
        List<UnoinTaskConfig> unionTaskArr = CSVParser.GetConfigsFromCache<UnoinTaskConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in unionTaskArr)
        {
            if (sc.id <= 0) continue;
            unionTaskPopularityDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        unionTaskPopularityDic.Clear();

        InitCSVConfig();
    }
    public UnoinTaskConfig[] GetAllConfig()
    {
        return unionTaskPopularityDic.Values.ToArray();
    }

    public UnoinTaskConfig GetConfig(int key)
    {
        if (unionTaskPopularityDic.ContainsKey(key))
        {
            return unionTaskPopularityDic[key];
        }
        return null;
    }
}
