using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingUpgradeConfig
{
    public int id;
    public int architecture_id; //建筑id
    public string name;
    public int architecture_lv;
    public int architecture_type;
    public int click_num;
    public int effect_type;//升级作用类型：1-指定资源产量（X个/小时），2-指定工匠等级上限，3-指定副本休息时间减少X%,4-指定资源产量增加百分比，5-佣兵等级上限，6-商会联盟人数上限
    public int effect_id;
    public float effect_val;//升级作用参数（0.2代表20%，整数代表常数）
    public string effect_dec;
    //public int guild_click_num;
    //public int guild_effect_type;
    //public int guild_effect_val_type;
    //public float guild_effect_val;

    public string GetEffectDec()
    {
        string des = "";

        if (effect_type == 1) des = effect_val + "/" + LanguageManager.inst.GetValueByKey("小时");
        else if (effect_type == 2 || effect_type == 5) des = LanguageManager.inst.GetValueByKey("{0}级", effect_val.ToString());
        else if (effect_type == 6) des = effect_val + LanguageManager.inst.GetValueByKey("人");
        else des = (effect_val * 100) + "%";

        return des;
    }

}


public class BuildingUpgradeConfigManager : TSingletonHotfix<BuildingUpgradeConfigManager>, IConfigManager
{
    public Dictionary<int, BuildingUpgradeConfig> buildingUpgradeDic = new Dictionary<int, BuildingUpgradeConfig>();

    public const string CONFIG_NAME = "architecture_update";

    public void InitCSVConfig()
    {
        List<BuildingUpgradeConfig> buildingUpgradeArr = CSVParser.GetConfigsFromCache<BuildingUpgradeConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingUpgradeArr)
        {
            if (sc.id <= 0) continue;
            buildingUpgradeDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        buildingUpgradeDic.Clear();
        InitCSVConfig();
    }
    public BuildingUpgradeConfig[] GetAllConfig()
    {
        return buildingUpgradeDic.Values.ToArray();
    }

    //public BuildingUpgradeConfig GetConfig(int key)
    //{
    //    if (buildingUpgradeDic.ContainsKey(key))
    //    {
    //        return buildingUpgradeDic[key];
    //    }
    //    return null;
    //}

    public BuildingUpgradeConfig GetConfig(int id, int buildingSelfLevel)
    {
        foreach (var item in buildingUpgradeDic.Values)
            if (item.architecture_id == id && item.architecture_lv == buildingSelfLevel)
                return item;

        return null;
    }


    //public BuildingUpgradeConfig GetUnionShareConfig(int id, int buildingUnionCostCount)
    //{
    //    var list = buildingUpgradeDic.Values.ToList().FindAll(t => t.architecture_id == id);
    //    list.Sort((a, b) => a.architecture_lv.CompareTo(b.architecture_lv));

    //    BuildingUpgradeConfig unionShareCfg = list[0];

    //    foreach (var item in list)
    //    {
    //        if (item.guild_click_num > buildingUnionCostCount)
    //        {
    //            unionShareCfg = item;
    //            break;
    //        }
    //        else
    //        {
    //            unionShareCfg = item;//为最大等级时
    //        }
    //    }

    //    return unionShareCfg;
    //}

}