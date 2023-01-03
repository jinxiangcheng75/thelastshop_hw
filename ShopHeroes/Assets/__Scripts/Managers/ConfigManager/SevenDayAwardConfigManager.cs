using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SevenDayAwardConfigData
{
    public int id;
    public int day;
    public string title;
    public string slogan;
    public string pic;
    public int type;
    public int reward;
    public int reward_number;
}

public class SevenDayAwardConfigManager : TSingletonHotfix<SevenDayAwardConfigManager>, IConfigManager
{
    public Dictionary<int, SevenDayAwardConfigData> cfgList = new Dictionary<int, SevenDayAwardConfigData>();
    public const string CONFIG_NAME = "seven_day_award";

    public void InitCSVConfig()
    {
        List<SevenDayAwardConfigData> scArray = CSVParser.GetConfigsFromCache<SevenDayAwardConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public SevenDayAwardConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public SevenDayAwardConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
