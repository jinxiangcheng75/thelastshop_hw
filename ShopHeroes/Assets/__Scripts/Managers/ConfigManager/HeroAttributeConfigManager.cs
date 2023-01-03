using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroAttributeConfigeData
{
    public int id;
    public int lv;
    public int profession_id;
    public string name;
    public int hp_basic;
    public int atk_basic;
    public int def_basic;
    public int spd_basic;
    public int acc_basic;
    public int dodge_basic;
    public int cri_basic;
    public int tough_basic;
    public int piercing_dmg;
    public int burn_dmg;
    public int ment_dmg;
    public int electricity_dmg;
    public int radiation_dmg;
    public int piercing_res;
    public int burn_res;
    public int ment_res;
    public int electricity_res;
    public int radiation_res;
}

public class HeroAttributeConfigManager : TSingletonHotfix<HeroAttributeConfigManager>, IConfigManager
{
    public Dictionary<int, HeroAttributeConfigeData> cfgList = new Dictionary<int, HeroAttributeConfigeData>();
    public const string CONFIG_NAME = "hero_attributes";

    public void InitCSVConfig()
    {
        List<HeroAttributeConfigeData> scArray = CSVParser.GetConfigsFromCache<HeroAttributeConfigeData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();
        InitCSVConfig();
    }
    public HeroAttributeConfigeData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public HeroAttributeConfigeData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public HeroAttributeConfigeData GetDataByLevelAndProfession(int level, int profession)
    {
        HeroAttributeConfigeData tempData = new HeroAttributeConfigeData();
        foreach (var item in cfgList.Values)
        {
            if (item.lv == level && item.profession_id == profession)
            {
                return item;
            }
        }

        Logger.error("没有找到职业id是" + profession + "并且等级是" + level + "的数据");
        return tempData;
    }
}
