using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtisanNPCConfigData
{
    public int id;
    public string name;
    public int type;
    public string icon;
    public string desc;
    public string pic;
    public int model;
    public string union_task_desc;
}

public class ArtisanNPCConfigManager : TSingletonHotfix<ArtisanNPCConfigManager>, IConfigManager
{
    public Dictionary<int, ArtisanNPCConfigData> cfgDic = new Dictionary<int, ArtisanNPCConfigData>();

    public const string CONFIG_NAME = "artisan_npc";

    public void InitCSVConfig()
    {
        List<ArtisanNPCConfigData> worriorArr = CSVParser.GetConfigsFromCache<ArtisanNPCConfigData>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in worriorArr)
        {
            if (sc.id <= 0) continue;
            cfgDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgDic.Clear();
        InitCSVConfig();
    }
    public ArtisanNPCConfigData[] GetAllConfig()
    {
        return cfgDic.Values.ToArray();
    }

    public ArtisanNPCConfigData GetConfig(int key)
    {
        if (cfgDic.ContainsKey(key))
        {
            return cfgDic[key];
        }
        return null;
    }
}
