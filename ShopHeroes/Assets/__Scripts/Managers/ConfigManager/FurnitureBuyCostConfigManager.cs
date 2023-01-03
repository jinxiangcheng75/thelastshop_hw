using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class FurnitureBuyCostConfig
{
    public int id;
    public int furniture_id;
    public string furniture_name;
    public int type_1;
    public int type_2;
    public int purchase_times;
    public int cost_type;
    public int cost_num;
}


public class FurnitureBuyCostConfigManager : TSingletonHotfix<FurnitureBuyCostConfigManager>, IConfigManager
{
    public Dictionary<int, FurnitureBuyCostConfig> dic = new Dictionary<int, FurnitureBuyCostConfig>();

    public const string CONFIG_NAME = "furniture_unlock_cost";

    public void InitCSVConfig()
    {
        List<FurnitureBuyCostConfig> arr = CSVParser.GetConfigsFromCache<FurnitureBuyCostConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in arr)
        {
            if (sc.id <= 0) continue;
            dic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        dic.Clear();

        InitCSVConfig();
    }

    public FurnitureBuyCostConfig GetConfig(int key)
    {
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return null;
    }

    public FurnitureBuyCostConfig GetConfig(int furniture_id, int purchase_times)
    {

        foreach (var cfg in dic.Values)
        {
            if (cfg.furniture_id == furniture_id && cfg.purchase_times == purchase_times)
            {
                return cfg;
            }
        }

        return null;
    }


}
