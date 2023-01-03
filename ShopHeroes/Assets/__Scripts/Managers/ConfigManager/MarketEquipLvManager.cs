using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MarketItemLevelConfig
{
    public int id;
    public int shopper_level;
    public int equipment_level;
}

public class MarketEquipLvManager : TSingletonHotfix<MarketEquipLvManager>, IConfigManager
{
    public Dictionary<int, MarketItemLevelConfig> marketlevelDic = new Dictionary<int, MarketItemLevelConfig>();

    public const string CONFIG_NAME = "auction_equiplevel";

    public void InitCSVConfig()
    {
        List<MarketItemLevelConfig> worriorArr = CSVParser.GetConfigsFromCache<MarketItemLevelConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in worriorArr)
        {
            if (sc.equipment_level <= 0) continue;
            marketlevelDic.Add(sc.equipment_level, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        marketlevelDic.Clear();

        InitCSVConfig();
    }
    public int GetCurMarketLevel()
    {
        uint shopperLv = UserDataProxy.inst.playerData.level;

        MarketItemLevelConfig cfg = null;

        foreach (var item in marketlevelDic.Values)
        {
            if (shopperLv >= item.shopper_level)
            {
                cfg = item;
            }
            else
            {
                break;
            }
        }

        return cfg == null ? 1 : cfg.equipment_level;
    }

}
