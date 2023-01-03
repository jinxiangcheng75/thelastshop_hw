using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExploreInstanceLvConfigData
{
    public int id;
    public int instance_id;
    public string instance_name;
    public int unlock_gold;
    public int unlock_diamond;
    public int instance_lv;
    public int need_instance_exp;
    public int effect_type;
    public int[] effect_id;
    public int effect_num;
    public string effect_atlas;
    public string effect_icon;
    public string effect_dec1;
    public string effect_dec2;
}

public class ExploreInstanceLvConfigManager : TSingletonHotfix<ExploreInstanceLvConfigManager>, IConfigManager
{
    public Dictionary<int, ExploreInstanceLvConfigData> cfgList = new Dictionary<int, ExploreInstanceLvConfigData>();
    public const string CONFIG_NAME = "instance_lv";

    public void InitCSVConfig()
    {
        List<ExploreInstanceLvConfigData> scArray = CSVParser.GetConfigsFromCache<ExploreInstanceLvConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);
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
    public ExploreInstanceLvConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public ExploreInstanceLvConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public List<ExploreInstanceLvConfigData> GetSameGroupData(int group, int level)
    {
        return cfgList.Values.ToList().FindAll(t => t.instance_id == group && t.instance_lv <= level);
    }

    public ExploreInstanceLvConfigData GetConfigDataByGroupAndLevel(int group, int level)
    {
        return cfgList.Values.ToList().Find(t => t.instance_id == group && t.instance_lv == level);
    }



    public List<ExploreInstanceLvConfigData> GetConfigDataByGroup(int group)
    {
        return cfgList.Values.ToList().FindAll(t => t.instance_id == group);
    }

    public int GetConfigDataByTypeAndIndex(int type, int index = 0, bool isNext = false, int groupId = 1)
    {
        if (type == 4)
            return cfgList.Values.ToList().FindAll(t => t.effect_type == type)[index].instance_lv;
        else if (type == 5)
        {
            groupId = Mathf.Max(groupId - 1, 1);
            var temp = cfgList.Values.ToList().FindAll(t => t.effect_type == type && t.instance_id >= groupId);
            int count = 0;
            for (int i = 0; i < temp.Count; i++)
            {
                var instanceCfg = ExploreInstanceConfigManager.inst.GetConfig(temp[i].effect_id[0]);
                if (instanceCfg.instance_group != temp[i].instance_id && isNext)
                {
                    return temp[i].instance_lv;
                }
                else if (instanceCfg.instance_group == temp[i].instance_id && !isNext)
                {
                    if (count == index)
                    {
                        return temp[i].instance_lv;
                    }
                    count++;
                }
            }
            return 0;
        }

        Logger.error("没有找到类型是" + type + "下表为" + index + "的数据");
        return -1;
    }

    public long GetExpByCurLevel(int level, int groupId)
    {
        long sumExp = 0;
        var tempList = GetConfigDataByGroup(groupId);
        if (tempList != null)
        {
            for (int i = 0; i < level; i++)
            {
                if (i > tempList.Count - 1) break;
                sumExp += tempList[i].need_instance_exp;
            }
        }

        return sumExp;
    }
}
