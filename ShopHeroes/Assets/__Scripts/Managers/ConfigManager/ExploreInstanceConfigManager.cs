using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExploreInstanceConfigData
{
    public int id;
    public string instance_name;
    public int instance_group;
    public int instance_type;
    public int difficulty;
    public string scenes;
    public string instance_icon;
    public int explore_time;
    public int rest_time;
    public int treatment_time;
    public int refresh_interval;
    public int[] monster1_id;
    public int[] monster2_id;
    public int[] monster3_id;
    public int boss_id;
    public int drop1_id;
    public int drop1_num_min;
    public int drop1_num_max;
    public int drop2_id;
    public int drop2_num_min;
    public int drop2_num_max;
    public int drop3_id;
    public int drop3_num_min;
    public int drop3_num_max;
    public int random_drop_id;
    public int random_drop_times;
    public int subscription_drop_id;
    public int subscription_drop_times;
    public int team_strength;
    public int recommend_power;
    public int hero_exp;
    public int instance_exp;
    public int people_number;
    public int[] position_open;
    public int decreas_formula;
}

public class ExploreInstanceConfigManager : TSingletonHotfix<ExploreInstanceConfigManager>, IConfigManager
{
    public Dictionary<int, ExploreInstanceConfigData> cfgList = new Dictionary<int, ExploreInstanceConfigData>();
    public const string CONFIG_NAME = "instance";

    public void InitCSVConfig()
    {
        List<ExploreInstanceConfigData> scArray = CSVParser.GetConfigsFromCache<ExploreInstanceConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public ExploreInstanceConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public ExploreInstanceConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
    public ExploreInstanceConfigData GetConfigByDropid(int id)
    {
        foreach (var item in cfgList.Values)
        {
            if (item.drop1_id == id || item.drop2_id == id || item.drop3_id == id || item.boss_id == id)
                return item;
        }
        return null;
    }
    public ExploreInstanceConfigData GetConfigByGroupId(int groupId)
    {
        foreach (var item in cfgList.Values)
        {
            if (item.instance_group == groupId && item.instance_type == 2)
                return item;
        }

        //Logger.error("没有id为" + groupId + "的数据");
        return null;
    }

    public bool JudgeHasTargetGroupData(int group)
    {
        foreach (var item in cfgList.Values)
        {
            if (item.instance_group == group)
                return true;
        }

        //Logger.error("没有组id是" + group + "的数据");
        return false;
    }

    public ExploreInstanceConfigData GetDataByTypeAndDifficultyGroup(int group, int type, int difficulty)
    {
        foreach (var item in cfgList.Values)
        {
            if ((item.instance_group == group && item.instance_type == type) && item.difficulty == difficulty)
            {
                return item;
            }
        }

        //Logger.error("没有组为" + group + "类型为" + type + "难度为" + difficulty + "的数据");
        return null;
    }
}
