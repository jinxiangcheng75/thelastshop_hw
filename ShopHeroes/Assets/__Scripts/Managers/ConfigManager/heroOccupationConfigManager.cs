using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroOccupationDataConfig
{
    public int id;
    public string name;
    public int order;
    public int ocp_style;
    public string ocp_story;
    public string atlas;
    public string ocp_icon;
    public int level_skill1;
    public int level_skill2;
    public int level_skill3;
    public int eqiup6_unlock;
    public int[] up_list;
    public int level_need;
    public int hp_basic;
    public float hp_grow;
    public int atk_basic;
    public float atk_grow;
    public int arm_basic;
    public float arm_grow;
    public int spd_basic;
    public float spd_grow;
    public int evd_basic;
    public float evd_grow;
    public int cri_basic;
    public float cri_grow;
    public int res_basic;
    public float res_gorw;
    public int cost_item1_id;
    public int cost_item1_num;
    public int cost_item2_id;
    public int cost_item2_num;
    public int[] equip1;
    public int[] equip2;
    public int[] equip3;
    public int[] equip4;
    public int[] equip5;
    public int[] equip6;
    public int id_skill1;
    public int id_skill2;
    public int id_skill3;
}

public class heroOccupationConfigManager : TSingletonHotfix<heroOccupationConfigManager>
{
    public Dictionary<int, HeroOccupationDataConfig> cfgList = new Dictionary<int, HeroOccupationDataConfig>();
    public const string CONFIG_NAME = "hero_occupation";

    public void InitCSVConfig()
    {
        List<HeroOccupationDataConfig> scArray = CSVParser.GetConfigsFromCache<HeroOccupationDataConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public HeroOccupationDataConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public HeroOccupationDataConfig GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
