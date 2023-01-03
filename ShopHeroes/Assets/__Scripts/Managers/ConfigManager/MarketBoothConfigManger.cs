using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BoothDataConfig
{
    public int field_number;
    public long coin_demand;
    public int diamond_demand;
    public int level_demand;
}

public class MarketBoothConfigManger : TSingletonHotfix<MarketBoothConfigManger>, IConfigManager
{


    public Dictionary<int, BoothDataConfig> cfgList = new Dictionary<int, BoothDataConfig>();
    public const string CONFIG_NAME = "auction_fieldnum";

    public void InitCSVConfig()
    {
        List<BoothDataConfig> scArray = CSVParser.GetConfigsFromCache<BoothDataConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.field_number <= 0) continue;
            cfgList.Add(item.field_number, item);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public BoothDataConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public BoothDataConfig GetConfig(int field)
    {
        if (cfgList.ContainsKey(field))
        {
            return cfgList[field];
        }

        return null;
    }

}
