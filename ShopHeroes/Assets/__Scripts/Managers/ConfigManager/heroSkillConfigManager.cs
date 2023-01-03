using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroSkillConfig
{
    public int id;
    public int priority;
    public int if_miss;
    public int if_crit;
    public int classification;
    public int equip;
    public int quality;
    public int trigger_percent;
    public int skill_object;
    public int attack_percent;
    public int[] my_buff_id;
    public int[] my_buff_trigger_type;
    public int[] my_buff_target;
    public int[] my_buff_value_type;
    public int[] my_buff_value;
    public int[] my_buff_max;
    public int[] my_buff_times;
    public int[] enemy_buff_id;
    public int[] enemy_buff_trigger_type;
    public int[] enemy_buff_target;
    public int[] enemy_buff_value_type;
    public int[] enemy_buff_value;
    public int[] enemy_buff_max;
    public int[] enemy_buff_times;

    public int cd;
}


public class heroSkillConfigManager : TSingletonHotfix<heroSkillConfigManager>, IConfigManager
{
    public Dictionary<int, HeroSkillConfig> cfgList = new Dictionary<int, HeroSkillConfig>();
    // //show
    // public Dictionary<int, HeroSkillShowCfg> skillShowList = new Dictionary<int, HeroSkillShowCfg>();

    public const string CONFIG_NAME = "hero_skills";
    //public const string CONFIG_NAME2 = "hero_skills_show";
    public void InitCSVConfig()
    {
        List<HeroSkillConfig> scArray = CSVParser.GetConfigsFromCache<HeroSkillConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
        // List<HeroSkillShowCfg> scArray2 = CSVParser.GetConfigsFromCache<HeroSkillShowCfg>(CONFIG_NAME2, CSVParser.STRING_SPLIT);
        // foreach (var c in scArray2)
        // {
        //     if (c.id <= 0) continue;
        //     skillShowList.Add(c.id, c);
        // }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();
        InitCSVConfig();
    }
    public HeroSkillConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public HeroSkillConfig GetConfig(int field)
    {
        if (cfgList.ContainsKey(field))
        {
            return cfgList[field];
        }
        return null;
    }

    // public HeroSkillShowCfg GetSkillShowConfig(int skillid)
    // {
    //     if (skillShowList.ContainsKey(skillid))
    //     {
    //         return skillShowList[skillid];
    //     }
    //     return null;
    // }
}
