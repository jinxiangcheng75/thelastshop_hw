using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTipsConfig
{
    public int id;
    public string tips;
    public int type;
}

public enum TipsType
{
    min,
    ChangeScene,
    HeroBuySlot,
}


public class GameTipsConfigManager : TSingletonHotfix<GameTipsConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "tips";
    public List<GameTipsConfig> cfgsList;
    public void InitCSVConfig()
    {
        if (cfgsList != null)
            cfgsList.Clear();
        cfgsList = CSVParser.GetConfigsFromCache<GameTipsConfig>
                    (CONFIG_NAME, CSVParser.STRING_SPLIT);
    }
    public void ReLoadCSVConfig()
    {
        InitCSVConfig();
    }
    public string GetRandemTipsStr()
    {
        if (cfgsList == null || cfgsList.Count <= 0) return "";
        var cfg = cfgsList.GetRandomElement();
        return cfg.tips;
    }

    public string GetRandomTipsByType(TipsType tipsType)
    {
        var tempList = cfgsList.FindAll(t => t.type == (int)tipsType);
        if (tempList == null || tempList.Count <= 0) return "";
        var cfg = tempList.GetRandomElement();
        return cfg.tips;
    }
}