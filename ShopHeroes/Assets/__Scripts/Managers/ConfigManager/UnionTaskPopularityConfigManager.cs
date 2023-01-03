using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnoinTaskPopularityConfig
{
    public int id;
    public int need_point_num;
    public int reward_point_num;
}

public class UnionTaskPopularityConfigManager : TSingletonHotfix<UnionTaskPopularityConfigManager>, IConfigManager
{
    public Dictionary<int, UnoinTaskPopularityConfig> unionTaskPopularityDic = new Dictionary<int, UnoinTaskPopularityConfig>();

    public const string CONFIG_NAME = "union_task_popularity";

    public void InitCSVConfig()
    {
        List<UnoinTaskPopularityConfig> unionTaskPopularityArr = CSVParser.GetConfigsFromCache<UnoinTaskPopularityConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in unionTaskPopularityArr)
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
    public UnoinTaskPopularityConfig[] GetAllConfig()
    {
        return unionTaskPopularityDic.Values.ToArray();
    }

    public UnoinTaskPopularityConfig GetConfig(int key)
    {
        if (unionTaskPopularityDic.ContainsKey(key))
        {
            return unionTaskPopularityDic[key];
        }
        return null;
    }
}
