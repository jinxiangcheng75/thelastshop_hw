using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBuffConfigData
{
    public int type;
    public string name;
    public string desc;
}

public class HeroBuffConfigManager : TSingletonHotfix<HeroBuffConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "hero_talent_buff";
    public Dictionary<int, HeroBuffConfigData> buffDic = new Dictionary<int, HeroBuffConfigData>();
    public void InitCSVConfig()
    {
        List<HeroBuffConfigData> buildingArr = CSVParser.GetConfigsFromCache<HeroBuffConfigData>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingArr)
        {
            if (sc.type <= 0) continue;
            buffDic.Add(sc.type, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        buffDic.Clear();
        InitCSVConfig();
    }
    public HeroBuffConfigData GetConfig(int id)
    {
        if (buffDic.ContainsKey(id))
        {
            return buffDic[id];
        }
        return null;
    }
}
