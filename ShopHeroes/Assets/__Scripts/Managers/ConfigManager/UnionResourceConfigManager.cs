using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UnionResourceConfig
{
    public int item_id;
    public int item_type;
    public int item_num;
    public int next_time;
    public int buy_times;
    public int price;
}


public class UnionResourceConfigManager : TSingletonHotfix<UnionResourceConfigManager>, IConfigManager
{
    public Dictionary<int, UnionResourceConfig> dic = new Dictionary<int, UnionResourceConfig>();

    public const string CONFIG_NAME = "union_resource";

    public void InitCSVConfig()
    {
        List<UnionResourceConfig> arr = CSVParser.GetConfigsFromCache<UnionResourceConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in arr)
        {
            if (sc.item_id <= 0) continue;
            dic.Add(sc.item_id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        dic.Clear();

        InitCSVConfig();
    }

    public UnionResourceConfig[] GetAllConfig()
    {
        return dic.Values.ToArray();
    }

    public UnionResourceConfig GetConfig(int key)
    {
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return null;
    }
}
