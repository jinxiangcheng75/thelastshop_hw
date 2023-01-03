using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffConfig
{
    public int id;
    public string name;
    public int type;
    public int num;
    public int priority;
    public string icon_atlas;
    public string icon;
    public int effect;
    public string flow_atlas;
    public int flow_text;
    public string flow_name;
}
public class BuffConfigManager : TSingletonHotfix<BuffConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "buff";
    public Dictionary<int, BuffConfig> buffDic = new Dictionary<int, BuffConfig>();
    public void InitCSVConfig()
    {
        List<BuffConfig> buildingArr = CSVParser.GetConfigsFromCache<BuffConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingArr)
        {
            if (sc.id <= 0) continue;
            buffDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        buffDic.Clear();
        InitCSVConfig();
    }
    public BuffConfig GetConfig(int id)
    {
        if (buffDic.ContainsKey(id))
        {
            return buffDic[id];
        }
        return null;
    }
}
