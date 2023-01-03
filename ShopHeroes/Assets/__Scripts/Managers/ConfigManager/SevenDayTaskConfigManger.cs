using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SevenDayTaskConfigData
{
    public int id;
    public int day;
    public int type;
    public string type_des;
    public string type_atlas;
    public string type_icon;
    public int[] parameter_1;
    public int[] parameter_2;
    public int[] parameter_3;
    public int type_reward1;
    public int reward1;
    public int reward1_number;
    public int type_reward2;
    public int reward2;
    public int reward2_number;
    public int type_reward3;
    public int reward3;
    public int reward3_number;
}

public class SevenDayTaskConfigManger : TSingletonHotfix<SevenDayTaskConfigManger>, IConfigManager
{
    public Dictionary<int, SevenDayTaskConfigData> cfgList = new Dictionary<int, SevenDayTaskConfigData>();
    public const string CONFIG_NAME = "seven_day_task";

    public void InitCSVConfig()
    {
        List<SevenDayTaskConfigData> scArray = CSVParser.GetConfigsFromCache<SevenDayTaskConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public SevenDayTaskConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public SevenDayTaskConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
