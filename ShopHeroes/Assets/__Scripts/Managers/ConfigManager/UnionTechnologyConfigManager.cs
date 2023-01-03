using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnionTechnologyConfig
{
    public int id;
    public int type_group;
    public int big_type;
    public int technology_level;
    public int need_level;
    public int need_point;
    public int type;
    public int skill_time;
    public string skill_desc;
    public int behind_skill_time;
    public int now_level;
    public int behind_level;
    public int add_num;
    public int add2_num;
    public string name;
    public string desc;
    public int next_id;
    public string next_desc;
    public string next_union_level_desc;
    public string atlas;
    public string icon;
    public string item_des;
    public string item_compare_des;

}

public class UnionTechnologyConfigManager : TSingletonHotfix<UnionTechnologyConfigManager>, IConfigManager
{
    public Dictionary<int, UnionTechnologyConfig> unionTechnologyConfigDic = new Dictionary<int, UnionTechnologyConfig>();

    public const string CONFIG_NAME = "union_technology";

    public void InitCSVConfig()
    {
        List<UnionTechnologyConfig> arr = CSVParser.GetConfigsFromCache<UnionTechnologyConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in arr)
        {
            if (sc.id <= 0) continue;
            unionTechnologyConfigDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        unionTechnologyConfigDic.Clear();

        InitCSVConfig();
    }
    public UnionTechnologyConfig[] GetAllConfig()
    {
        return unionTechnologyConfigDic.Values.ToArray();
    }

    public UnionTechnologyConfig GetConfig(int id)
    {
        if (unionTechnologyConfigDic.ContainsKey(id))
        {
            return unionTechnologyConfigDic[id];
        }

        return null;
    }

    public UnionTechnologyConfig GetConfig(int big_Type, int type, int level)
    {
        return unionTechnologyConfigDic.Values.ToList().Find(t => t.big_type == big_Type && t.type == type && t.technology_level == level);
    }

    public UnionTechnologyConfig GetConfig(int big_Type, int level)
    {
        return unionTechnologyConfigDic.Values.ToList().Find(t => t.big_type == big_Type && t.technology_level == level);
    }

}
