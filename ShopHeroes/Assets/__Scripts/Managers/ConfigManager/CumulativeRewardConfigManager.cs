using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CumulativeRewardConfig
{
    public int id;
    public int reward_group;
    public int reward_sequence;
    public int cumulative_times;
    public int reward_type1;
    public string reward_name1;
    public int reward_item_id1;
    public int reward_item_num1;
    public int reward_type2;
    public string reward_name2;
    public int reward_item_id2;
    public int reward_item_num2;
    public int reward_type3;
    public string reward_name3;
    public int reward_item_id3;
    public int reward_item_num3;
}

public class CumulativeRewardConfigManager : TSingletonHotfix<CumulativeRewardConfigManager>, IConfigManager
{
    public Dictionary<int, CumulativeRewardConfig> cfgList = new Dictionary<int, CumulativeRewardConfig>();
    public const string CONFIG_NAME = "lucky_cumulative_reward";

    public void InitCSVConfig()
    {
        List<CumulativeRewardConfig> scArray = CSVParser.GetConfigsFromCache<CumulativeRewardConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public CumulativeRewardConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public CumulativeRewardConfig GetConfig(int field)
    {
        if (cfgList.ContainsKey(field))
        {
            return cfgList[field];
        }

        return null;
    }

    public int GetLastGroupData(int group)
    {
        List<CumulativeRewardConfig> listData = cfgList.Values.ToList();
        List<CumulativeRewardConfig> tempData = new List<CumulativeRewardConfig>();
        if (group == 1)
        {
            return 0;
        }
        else
        {
            int sum = 0;
            for (int i = group; i > 1; i--)
            {
                int index = i;
                tempData = listData.FindAll(t => t.reward_group == index - 1);
                sum += tempData[tempData.Count - 1].cumulative_times;
            }

            return sum;
        }
    }
}
