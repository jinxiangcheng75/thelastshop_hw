using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterConfigData
{
    public int id;
    public string monster_name;
    public int action_id;
    public int equip_type;
    public int monster_lv;
    public int monster_type;
    public int monster_ethnicity;
    public string monster_story;
    public string monster_atlas;
    public string monster_icon;
    public int monster_model;
    // public int hp_basic;
    // public int atk_basic;
    // public int def_basic;
    // public int spd_basic;
    // public int acc_basic;
    // public int dodge_basic;
    // public int cri_basic;
    // public int tough_basic;
    // public int piercing_dmg;
    // public int burn_dmg;
    // public int ment_dmg;
    // public int electricity_dmg;
    // public int radiation_dmg;
    // public int piercing_res;
    // public int burn_res;
    // public int ment_res;
    // public int electricity_res;
    // public int radiation_res;
    // public int id_skill1;
    // public int id_skill2;
    // public int id_skill3;
    // public int[] opening_dialog_id;
    // public int[] skill_dialog_id;
    // public int[] fail_dialog_id;
}

public class MonsterConfigManager : TSingletonHotfix<MonsterConfigManager>, IConfigManager
{
    public Dictionary<int, MonsterConfigData> cfgList = new Dictionary<int, MonsterConfigData>();
    public const string CONFIG_NAME = "monster";

    public void InitCSVConfig()
    {
        List<MonsterConfigData> scArray = CSVParser.GetConfigsFromCache<MonsterConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public MonsterConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public MonsterConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
