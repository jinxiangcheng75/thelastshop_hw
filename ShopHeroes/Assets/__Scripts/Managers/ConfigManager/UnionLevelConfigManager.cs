using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnionLevelConfig
{
    public int level;
    public int type;
    public string item_des;
    public string item_compare_des;
    public int need_level;
    public int need_point;
    public int next_level_point;
    public int add_task_num;
    public int common_material;
    public int unreal_material;
    public int precious_material;
    public int hot_shelf;
    public int define_shelf;
    public int cold_shelf;
    public int other_shelf;
    public int ware_house_add;
    public int stack_add;
    public int make_add;
    public int adv_add;
    public int material_add;
    public int exp_add;
    public int buy_add;
    public int sell_add;
    public int common_material_recover;
    public int unreal_material_recover;
    public int precious_material_recover;
    public string name;
    public string desc;
    public string atlas;
    public string icon;

    public List<int> GetShowItemTechnologyIds()
    {
        List<int> result = new List<int>();

        if (add_task_num != 0)
        {
            result.Add(add_task_num);
        }

        if (common_material != 0)
        {
            result.Add(common_material);
        }

        if (unreal_material != 0)
        {
            result.Add(unreal_material);
        }

        if (precious_material != 0)
        {
            result.Add(precious_material);
        }

        if (hot_shelf != 0)
        {
            result.Add(hot_shelf);
        }

        if (define_shelf != 0)
        {
            result.Add(define_shelf);
        }

        if (cold_shelf != 0)
        {
            result.Add(cold_shelf);
        }

        if (other_shelf != 0)
        {
            result.Add(other_shelf);
        }

        if (ware_house_add != 0)
        {
            result.Add(ware_house_add);
        }

        if (stack_add != 0)
        {
            result.Add(stack_add);
        }

        if (make_add != 0)
        {
            result.Add(make_add);
        }

        if (adv_add != 0)
        {
            result.Add(adv_add);
        }

        if (material_add != 0)
        {
            result.Add(material_add);
        }

        if (exp_add != 0)
        {
            result.Add(exp_add);
        }

        if (buy_add != 0)
        {
            result.Add(buy_add);
        }

        if (sell_add != 0)
        {
            result.Add(sell_add);
        }

        if (common_material_recover != 0)
        {
            result.Add(common_material_recover);
        }

        if (unreal_material_recover != 0)
        {
            result.Add(unreal_material_recover);
        }

        if (precious_material_recover != 0)
        {
            result.Add(precious_material_recover);
        }


        return result.FindAll(t => t != 0);
    }

}

public class UnionLevelConfigManager : TSingletonHotfix<UnionLevelConfigManager>, IConfigManager
{
    public Dictionary<int, UnionLevelConfig> unionLevelConfigDic = new Dictionary<int, UnionLevelConfig>();

    public const string CONFIG_NAME = "union_level";

    public void InitCSVConfig()
    {
        List<UnionLevelConfig> arr = CSVParser.GetConfigsFromCache<UnionLevelConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in arr)
        {
            if (sc.level <= 0) continue;
            unionLevelConfigDic.Add(sc.level, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        unionLevelConfigDic.Clear();

        InitCSVConfig();
    }
    public UnionLevelConfig[] GetAllConfig()
    {
        return unionLevelConfigDic.Values.ToArray();
    }

    public UnionLevelConfig GetConfig(int key)
    {
        if (unionLevelConfigDic.ContainsKey(key))
        {
            return unionLevelConfigDic[key];
        }
        return null;
    }
}
