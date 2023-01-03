using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OneTalentDataBase
{
    public int talentId;
    public string talentDesc;
    public int type;
    public int value_type;
    public float value;
    public int index;
    public bool isOpen = false;

    public OneTalentDataBase(string _talentDesc, int _type, int _value_type, float _value, int _talentId)
    {
        talentDesc = _talentDesc;
        type = _type;
        value_type = _value_type;
        value = _value;
        talentId = _talentId;
    }

    public OneTalentDataBase()
    {

    }

    public int GetRingHeroType()
    {
        if (type == 19)
        {
            return 1;
        }
        else if (type == 20)
        {
            return 2;
        }
        else if (type == 21)
        {
            return 3;
        }

        return -1;
    }
}

public class HeroTalentDataBase
{
    public int id;
    public int sort;
    public string name;
    public int skill_id;
    public int type;
    public int quality;
    public int entry1_type;
    public int entry1_value_type;
    public int entry1_value;
    public int entry2_type;
    public int entry2_value_type;
    public int entry2_value;
    public int entry3_type;
    public int entry3_value_type;
    public int entry3_value;
    public int entry4_type;
    public int entry4_value_type;
    public int entry4_value;
    public string talent_skill_atlas;
    public string talent_skill_icon;
    public string easy_txt_1;
    public string easy_txt_2;
    public string easy_txt_3;
    public string easy_txt_4;

    public OneTalentDataBase GetRingVal()
    {
        if (entry1_type >= 19 && entry1_type <= 21)
        {
            OneTalentDataBase data = new OneTalentDataBase(easy_txt_1, entry1_type, entry1_value_type, entry1_value, skill_id);
            return data;
        }
        if (entry2_type >= 19 && entry2_type <= 21)
        {
            OneTalentDataBase data = new OneTalentDataBase(easy_txt_2, entry2_type, entry2_value_type, entry2_value, skill_id);
            return data;
        }
        if (entry3_type >= 19 && entry3_type <= 21)
        {
            OneTalentDataBase data = new OneTalentDataBase(easy_txt_3, entry3_type, entry3_value_type, entry3_value, skill_id);
            return data;
        }
        if (entry4_type >= 19 && entry4_type <= 21)
        {
            OneTalentDataBase data = new OneTalentDataBase(easy_txt_4, entry4_type, entry4_value_type, entry4_value, skill_id);
            return data;
        }

        return null;
    }

    public float GetValsByEquipType(int equip_type, int equip_sub_type)
    {
        float val = 0;

        if (entry1_type >= 26 && entry1_type <= 56)
        {
            if (entry1_type <= 48)
            {
                if (StaticConstants.talentDataBaseEntry[entry1_type] == equip_sub_type)
                {
                    val += GetValByValType(entry1_value, entry1_value_type);
                }
            }
            else
            {
                if (StaticConstants.talentDataBaseEntry[entry1_type] == equip_type)
                {
                    val += GetValByValType(entry1_value, entry1_value_type);
                }
            }
        }

        if (entry2_type >= 26 && entry2_type <= 56)
        {
            if (entry2_type <= 48)
            {
                if (StaticConstants.talentDataBaseEntry[entry2_type] == equip_sub_type)
                {
                    val += GetValByValType(entry2_value, entry2_value_type);
                }
            }
            else
            {
                if (StaticConstants.talentDataBaseEntry[entry2_type] == equip_type)
                {
                    val += GetValByValType(entry2_value, entry2_value_type);
                }
            }
        }

        if (entry3_type >= 26 && entry3_type <= 56)
        {
            if (entry3_type <= 48)
            {
                if (StaticConstants.talentDataBaseEntry[entry3_type] == equip_sub_type)
                {
                    val += GetValByValType(entry3_value, entry3_value_type);
                }
            }
            else
            {
                if (StaticConstants.talentDataBaseEntry[entry3_type] == equip_type)
                {
                    val += GetValByValType(entry3_value, entry3_value_type);
                }
            }
        }

        return (1 + val);
    }

    public float GetValByValType(int val, int val_type)
    {
        float endVal = 0;
        if (val_type == 0)
        {
            endVal = val;
        }
        else if (val_type == 1)
        {
            endVal = val / 100.0f;
        }
        else if (val_type == 2)
        {
            endVal = val / 1000.0f;
        }

        return endVal;
    }
}

public class HeroTalentDBConfigManager : TSingletonHotfix<HeroTalentDBConfigManager>, IConfigManager
{
    public Dictionary<int, HeroTalentDataBase> cfgList = new Dictionary<int, HeroTalentDataBase>();
    public const string CONFIG_NAME = "hero_talent_database";

    public void InitCSVConfig()
    {
        List<HeroTalentDataBase> scArray = CSVParser.GetConfigsFromCache<HeroTalentDataBase>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public HeroTalentDataBase[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public HeroTalentDataBase GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }
        return null;
    }

    public string GetStrByTalentId(int talentId)
    {
        string tempStr = string.Empty;
        if (cfgList.ContainsKey(talentId))
        {
            var cfg = cfgList[talentId];
            if (cfg.entry1_type != 0)
            {
                tempStr = tempStr + LanguageManager.inst.GetValueByKey(HeroBuffConfigManager.inst.GetConfig(cfg.entry1_type).name, cfg.entry1_value.ToString());
            }
            if (cfg.entry2_type != 0)
            {
                tempStr = tempStr + "\n" + LanguageManager.inst.GetValueByKey(HeroBuffConfigManager.inst.GetConfig(cfg.entry2_type).name, cfg.entry2_value.ToString());
            }
            if (cfg.entry3_type != 0)
            {
                tempStr = tempStr + "\n" + LanguageManager.inst.GetValueByKey(HeroBuffConfigManager.inst.GetConfig(cfg.entry3_type).name, cfg.entry3_value.ToString());
            }
            if (cfg.entry4_type != 0)
            {
                tempStr = tempStr + "\n" + LanguageManager.inst.GetValueByKey(HeroBuffConfigManager.inst.GetConfig(cfg.entry4_type).name, cfg.entry4_value.ToString());
            }
        }

        return tempStr;
    }

    private string GetSymbolStr(int symbolType)
    {
        if (symbolType == 0)
        {
            return "";
        }
        else if (symbolType == 1)
        {
            return "%";
        }
        else
        {
            return "‰";
        }
    }
}
