using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingConfig
{
    public int id;
    public string name;
    public int architecture_type; //1-资源建筑，2-功能建筑，3-科研项目, 4-特殊建筑
    public int cost_grade; //投资等级 1 - A 2 - B 3 - C
    public string icon;
    public string big_icon;
    public string atlas;
    public int unlock_type;
    public int unlock_id;
    public int unlock_val;
    public string introduction_dec;
    public string functional_dec;
    public string shared_dec;
}


public class BuildingConfigManager : TSingletonHotfix<BuildingConfigManager>, IConfigManager
{
    public Dictionary<int, BuildingConfig> buildingDic = new Dictionary<int, BuildingConfig>();

    public const string CONFIG_NAME = "architecture";

    public readonly List<int> NotNarmalScienceIds = new List<int>() { 3400, 3500, 3600 };

    public void InitCSVConfig()
    {
        List<BuildingConfig> buildingArr = CSVParser.GetConfigsFromCache<BuildingConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingArr)
        {
            if (sc.id <= 0) continue;
            buildingDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        buildingDic.Clear();
        InitCSVConfig();
    }
    public BuildingConfig[] GetAllConfig()
    {
        return buildingDic.Values.ToArray();
    }

    public BuildingConfig GetConfig(int key)
    {
        if (buildingDic.ContainsKey(key))
        {
            return buildingDic[key];
        }
        return null;
    }


}

