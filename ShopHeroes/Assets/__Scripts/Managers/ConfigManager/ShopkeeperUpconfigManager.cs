using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ShopkeeperUpconfig
{
    public uint level;
    public long experience;
    public int market_level;
    public int hero_slot;
    public int trade;
    public int maker_slot;
    public int shopSize_slot;
    public int adventur_slot;
    public int warrior_id;
    public int worker_id;
    public int[] hero_occupation;
    public int[] furniture;
    public int[] decoration;
}

public class ShopkeeperUpconfigManager : TSingletonHotfix<ShopkeeperUpconfigManager>, IConfigManager
{

    public Dictionary<uint, ShopkeeperUpconfig> cfgList = new Dictionary<uint, ShopkeeperUpconfig>();
    public const string CONFIG_NAME = "shopkeeper_update";

    public void InitCSVConfig()
    {
        List<ShopkeeperUpconfig> scArray = CSVParser.GetConfigsFromCache<ShopkeeperUpconfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.level <= 0) continue;
            cfgList.Add(item.level, item);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public ShopkeeperUpconfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public ShopkeeperUpconfig GetConfig(uint key)
    {
        if (cfgList.ContainsKey(key))
        {
            return cfgList[key];
        }

        return null;
    }

    public long GetExpVal(int curLevel)
    {
        long sumExp = 0;
        var tempList = cfgList.Values.ToList();
        for (int i = 0; i < curLevel; i++)
        {
            if (i > tempList.Count - 1) break;
            sumExp += tempList[i].experience;
        }

        return sumExp;
    }
}
