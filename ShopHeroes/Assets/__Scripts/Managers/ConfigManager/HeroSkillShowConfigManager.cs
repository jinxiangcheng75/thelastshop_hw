using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatSkillsConfig
{
    public int index;
    public int id;
    public int equip_type;
    public string skill_name;
    public int display_type;    //0- 近战 1- 远程
    public int anger_skill_prepare_effect_id; //蓄力特效id
    public int anger_skill_prepare_life_time;//蓄力特效生命时间
    public int anger_skill_prepare_audio_id;    //蓄力音效ID
    public int anger_skill_prepare_next_time; //蓄力时间


    public int skill_attack_effect_id;  //攻击特效
    public int effect_trigger_time; //攻击特效时长。
    public int skill_attack_audio; //攻击音效

    //子弹
    public int skill_attack_bullet;//子弹id
    public int skill_attack_bullet_if_through; // 是否穿透
    public int skill_attack_bullet_next_time; // 飞行特效保持时长

    public int skill_attack_trigger_effect_id;  //飞行特效终点特效id
    public int skill_attack_trigger_audio;  //定点特效音效
    public int skill_attack_trigger_next_time; //定点特效保持时长

    public int skill_attack_hit_effect_id; //受击特效

    public int skill_attack_hit_audio; //受击音效
}


public class HeroSkillShowConfig
{
    public int id;
    public string skill_atlas;
    public string skill_icon;
    public string skill_name;
    public string skill_dec;
}

public class HeroSkillShowConfigManager : TSingletonHotfix<HeroSkillShowConfigManager>, IConfigManager
{
    public Dictionary<int, HeroSkillShowConfig> cfgList = new Dictionary<int, HeroSkillShowConfig>();
    public const string CONFIG_NAME = "hero_skills_show";

    public void InitCSVConfig()
    {
        List<HeroSkillShowConfig> scArray = CSVParser.GetConfigsFromCache<HeroSkillShowConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
        combatShowSkills = CSVParser.GetConfigsFromCache<CombatSkillsConfig>(CONFIG_NAME2, CSVParser.STRING_SPLIT);
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();
        combatShowSkills.Clear();
        InitCSVConfig();
    }
    public HeroSkillShowConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public HeroSkillShowConfig GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }
        return null;
    }


    #region 技能表现
    string CONFIG_NAME2 = "hero_equip_skills_show";
    List<CombatSkillsConfig> combatShowSkills;

    public CombatSkillsConfig GetCombatSkillsConfig(int equipType, int skillId)
    {
        if (combatShowSkills != null)
        {
            CombatSkillsConfig cfg = combatShowSkills.Find(c => c.id == skillId && c.equip_type == equipType);
            return cfg;
        }
        return null;
    }
    #endregion
}
