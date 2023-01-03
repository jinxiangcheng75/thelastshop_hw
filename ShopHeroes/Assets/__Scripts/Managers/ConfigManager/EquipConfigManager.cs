using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//装备图纸 配置  没有品质区别。 对应 equip_property_mobile表
public class EquipDrawingsConfig
{
    public int id;
    public string name;
    public string atlas;
    public string big_icon;
    public string icon; //图标
    public string icon_shelf;
    public int type;
    public int sub_type;
    public string desc;
    public int sort;
    public int level;
    public int unlock_type; //解锁类型：1-获得指定员工解锁，2-制作指定装备达到X数量解锁，3-通过指定宝箱获得解锁
    public int unlock_val_01;   //解锁条件
    public int unlock_val_02;   //解锁数值

    public int unlock_show_type;
    public int unlock_show_val;

    public string unlock_dec;   //解锁说明
    public int activate_drawing; //解锁后激活图纸数量
    public int[] artisan_id; //所需工匠id
    public int[] artisan_lv; //所需工匠等级
    public int speed_up_energy; //加速能量与时间兑换比例1能量=X秒
    public int speed_up_diamond; //加速消耗钻石
    public int production_time;
    public int[] material_id;
    public int[] material_num;

    public int component1_type;
    public int component1_id;
    public int component1_num;

    public int component2_type;
    public int component2_id;
    public int component2_num;

    public int[] delete_material_id;
    public int[] delete_material_num;

    public int progress1_reward_type;
    public int progress1_reward_id;
    public float progress1_reward_val;
    public int progress1_exp;
    public string progress1_dec;

    public int progress2_reward_type;
    public int progress2_reward_id;
    public float progress2_reward_val;
    public int progress2_exp;
    public string progress2_dec;

    public int progress3_reward_type;
    public int progress3_reward_id;
    public float progress3_reward_val;
    public int progress3_exp;
    public string progress3_dec;

    public int progress4_reward_type;
    public int progress4_reward_id;
    public float progress4_reward_val;
    public int progress4_exp;
    public string progress4_dec;

    public int progress5_reward_type;
    public int progress5_reward_id;
    public float progress5_reward_val;
    public int progress5_exp;
    public string progress5_dec;

    public int star1_reward_type;
    public int star1_reward_id;
    public float star1_reward_val;
    public int star1_item_num;
    public string star1__dec;

    public int star2_reward_type;
    public int star2_reward_id;
    public float star2_reward_val;
    public int star2_item_num;
    public string star2__dec;

    public int star3_reward_type;
    public int star3_reward_id;
    public float star3_reward_val;
    public int star3_item_num;
    public string star3__dec;

    public needMaterialsInfo[] GetNeedMaterialsInfos()
    {
        needMaterialsInfo[] arr = new needMaterialsInfo[material_id.Length];

        for (int i = 0; i < material_id.Length; i++)
        {
            arr[i] = new needMaterialsInfo();
            arr[i].type = 0;
            arr[i].needId = material_id[i];
            arr[i].needCount = material_num[i];
        }

        return arr;
    }

    public progressItemInfo[] GetProgressItemInfos()
    {
        progressItemInfo[] arr = new progressItemInfo[5];
        arr[0] = new progressItemInfo(progress1_reward_type, progress1_reward_id, progress1_reward_val, progress1_dec, progress1_exp);
        arr[1] = new progressItemInfo(progress2_reward_type, progress2_reward_id, progress2_reward_val, progress2_dec, progress2_exp);
        arr[2] = new progressItemInfo(progress3_reward_type, progress3_reward_id, progress3_reward_val, progress3_dec, progress3_exp);
        arr[3] = new progressItemInfo(progress4_reward_type, progress4_reward_id, progress4_reward_val, progress4_dec, progress4_exp);
        arr[4] = new progressItemInfo(progress5_reward_type, progress5_reward_id, progress5_reward_val, progress5_dec, progress5_exp);

        return arr;
    }

    public starUpProgressItemInfo[] GetStarUpProgressItemInfos()
    {
        starUpProgressItemInfo[] arr = new starUpProgressItemInfo[3];
        arr[0] = new starUpProgressItemInfo(star1_reward_type, star1_reward_val, star1_item_num, star1__dec);
        arr[1] = new starUpProgressItemInfo(star2_reward_type, star2_reward_val, star2_item_num, star2__dec);
        arr[2] = new starUpProgressItemInfo(star3_reward_type, star3_reward_val, star3_item_num, star3__dec);

        return arr;
    }

}
//装备品质配置  对应 equip_mobile表
public class EquipQualityConfig
{
    public int id;
    public string name;
    public int quality;
    public int equip_id;
    public int dressId;
    public int extraordinary_equip_id;

    public int exp;
    public int price_gold;
    public int purchase_price;
    public int auction_price;
    public int price_diamond;
    public int atk_basic;
    public int def_basic;
    public int hp_basic;
    public int spd_basic;
    public int acc_basic;

    public int cri_basic;
    public int dodge_basic;
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
    public int artisan_exp;
    public int discount_energy;//打折能量

    public int double_energy;//加倍能量

    public int recommend_energy;//建议能量

    public int probability_make;//制作获得概率

    public int improve_quality_diamond;//升级品质消耗钻石

    public int talk_success_energy;//闲聊成功奖励能量百分比

    public int talk_failure_energy;//闲聊失败扣除能量百分比

    public int fixGem;


    //获取装备战力
    public float GetEquipFightingSum(HeroTalentDataBase talentCfg)
    {
        float fightingSum = 0;
        List<int> properties = new List<int>();
        float percent = 1;

        if (talentCfg != null)
        {
            var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equip_id);
            if (equipDrawingCfg != null)
            {
                percent = talentCfg.GetValsByEquipType(equipDrawingCfg.type, equipDrawingCfg.sub_type);
            }
        }

        var cfg = this;
        properties.Add(cfg.hp_basic);
        properties.Add(cfg.atk_basic);
        properties.Add(cfg.def_basic);
        properties.Add(cfg.spd_basic);
        properties.Add(cfg.acc_basic);
        properties.Add(cfg.dodge_basic);
        properties.Add(cfg.cri_basic);
        properties.Add(cfg.tough_basic);
        properties.Add(cfg.piercing_dmg);
        properties.Add(cfg.burn_dmg);
        properties.Add(cfg.ment_dmg);
        properties.Add(cfg.electricity_dmg);
        properties.Add(cfg.radiation_dmg);
        properties.Add(cfg.piercing_res);
        properties.Add(cfg.burn_res);
        properties.Add(cfg.ment_res);
        properties.Add(cfg.electricity_res);
        properties.Add(cfg.radiation_res);

        for (int i = 0; i < 18; i++)
        {
            int index = i;
            if (properties[index] > 0)
            {
                fightingSum += (properties[index] * percent) * WorldParConfigManager.inst.GetConfig(200 + index).parameters;
            }
        }

        return fightingSum;
    }

}

public class EquipClassification
{
    public int id;
    public string name;
    public string Atlas;
    public string icon;
    public int type;

    public float m_rotation_angle;//旋转角度
    public float[] m_coordinate;//位置
    public float[] m_size;//大小
    public float g_rotation_angle;//旋转角度
    public float[] g_coordinate;//位置
    public float[] g_size;//大小

    public Vector2 GetSlotPos(EGender gender)
    {
        float[] poses = null;

        switch (gender)
        {
            case EGender.Male:
                poses = m_coordinate;
                break;
            case EGender.Female:
                poses = g_coordinate;
                break;
        }

        Vector2 result = new Vector2();
        if (poses != null && poses.Length > 1)
        {
            result.x = poses[0];
            result.y = poses[1];
        }

        return result;
    }

    public Vector2 GetSlotScale(EGender gender)
    {
        float[] scales = null;

        switch (gender)
        {
            case EGender.Male:
                scales = m_size;
                break;
            case EGender.Female:
                scales = g_size;
                break;
        }

        Vector2 result = new Vector2();
        if (scales != null && scales.Length > 1)
        {
            result.x = scales[0];
            result.y = scales[1];
        }

        return result;
    }

}

public class EquipConfig
{
    public int equipDrawingId;
    public EquipQualityConfig equipQualityConfig;
    public EquipDrawingsConfig equipDrawingsConfig;

    public string name
    {
        get
        {
            if (equipQualityConfig != null && equipDrawingsConfig != null)
            {
                string str_prefix = "";
                if (equipQualityConfig.extraordinary_equip_id <= 0) //是超凡装备
                {
                    str_prefix = LanguageManager.inst.GetValueByKey("<color=#fb1470>超凡</color>") + "<color=#ffffff>·</color>";
                }

                return str_prefix + LanguageManager.inst.GetValueByKey(equipDrawingsConfig.name);
            }

            return string.Empty;
        }
    }

    public string quality_name
    {
        get
        {
            if (equipQualityConfig != null)
            {
                string str_prefix = "";
                if (equipQualityConfig.extraordinary_equip_id <= 0) //是超凡装备
                {
                    str_prefix = LanguageManager.inst.GetValueByKey("<color=#fb1470>超凡</color>") + "<color=#ffffff>·</color>";
                }

                return str_prefix + LanguageManager.inst.GetValueByKey(equipQualityConfig.name);
            }

            return string.Empty;
        }
    }


    public EquipConfig(int _equipDrawingId, EquipQualityConfig cfg_1, EquipDrawingsConfig cfg_2)
    {
        equipDrawingId = _equipDrawingId;
        equipQualityConfig = cfg_1;
        equipDrawingsConfig = cfg_2;
    }

    //public int fightingSum { get { return equipQualityConfig == null ? 0 : equipQualityConfig.GetEquipFightingSum(); } }

    public float GetFightingSum(HeroTalentDataBase talentCfg)
    {
        float fightSum = 0;

        if (equipQualityConfig != null)
        {
            fightSum = equipQualityConfig.GetEquipFightingSum(talentCfg);
        }

        return fightSum;
    }
}
public class EquipConfigManager : TSingletonHotfix<EquipConfigManager>, IConfigManager
{
    public const string CONFIG_FILENAME1 = "equip";
    public const string CONFIG_FILENAME2 = "equip_property";
    public const string CONFIG_FILENAME3 = "equipment_classification";

    public List<EquipClassification> equipClassifications;
    public Dictionary<int, EquipClassification> equipTypeDic = new Dictionary<int, EquipClassification>();

    List<EquipDrawingsConfig> equipDrawingsList;
    public Dictionary<int, EquipDrawingsConfig> equipDrawingsCfgMap = new Dictionary<int, EquipDrawingsConfig>();

    public List<EquipQualityConfig> equipQualityConfigs;
    public Dictionary<int, List<EquipQualityConfig>> equipQualityCfgMap = new Dictionary<int, List<EquipQualityConfig>>();

    public Dictionary<int, EquipConfig> equipInfoConfigList = new Dictionary<int, EquipConfig>();

    public void ReLoadCSVConfig()
    {
        equipDrawingsCfgMap.Clear();
        equipTypeDic.Clear();
        equipQualityCfgMap.Clear();
        equipInfoConfigList.Clear();
        InitCSVConfig();
    }

    public void InitCSVConfig()
    {
        equipDrawingsList = CSVParser.GetConfigsFromCache<EquipDrawingsConfig>(CONFIG_FILENAME2, CSVParser.STRING_SPLIT);
        foreach (var drawings in equipDrawingsList)
        {
            equipDrawingsCfgMap.Add(drawings.id, drawings);
        }

        equipClassifications = CSVParser.GetConfigsFromCache<EquipClassification>(CONFIG_FILENAME3, CSVParser.STRING_SPLIT);
        foreach (var equipType in equipClassifications)
        {
            equipTypeDic.Add(equipType.id, equipType);
        }

        equipQualityConfigs = CSVParser.GetConfigsFromCache<EquipQualityConfig>(CONFIG_FILENAME1, CSVParser.STRING_SPLIT);

        foreach (var cfg in equipQualityConfigs)
        {
            if (equipDrawingsCfgMap.ContainsKey(cfg.equip_id))
            {
                List<EquipQualityConfig> eqcfgs;
                if (equipQualityCfgMap.TryGetValue(cfg.equip_id, out eqcfgs))
                {
                    eqcfgs.Add(cfg);
                    equipQualityCfgMap[cfg.equip_id] = eqcfgs;
                }
                else
                {
                    eqcfgs = new List<EquipQualityConfig>();
                    eqcfgs.Add(cfg);
                    equipQualityCfgMap.Add(cfg.equip_id, eqcfgs);
                }

                equipInfoConfigList.Add(cfg.id, new EquipConfig(cfg.equip_id, cfg, equipDrawingsCfgMap[cfg.equip_id]));
            }
        }

    }

    public EquipDrawingsConfig GetEquipDrawingsCfg(int equipDrawingId)
    {
        EquipDrawingsConfig cfg;
        return equipDrawingsCfgMap.TryGetValue(equipDrawingId, out cfg) ? cfg : null;
    }
    public EquipDrawingsConfig GetEquipDrawingsCfgByEquipId(int equipId)
    {
        var equipcfg = GetEquipQualityConfig(equipId);
        if (equipcfg == null) return null;
        EquipDrawingsConfig cfg;
        return equipDrawingsCfgMap.TryGetValue(equipcfg.equip_id, out cfg) ? cfg : null;
    }
    public List<EquipQualityConfig> GetEquipSubCfgsListByDrawingId(int drawingid)
    {
        List<EquipQualityConfig> cfgList;
        return equipQualityCfgMap.TryGetValue(drawingid, out cfgList) ? cfgList : null;
    }
    public EquipQualityConfig GetEquipQualityConfig(int _equipdrawingid, int quality)
    {
        List<EquipQualityConfig> cfgList;
        if (equipQualityCfgMap.TryGetValue(_equipdrawingid, out cfgList))
        {
            return cfgList.Find(item => item.quality == quality);
        }
        else
        {
            return null;
        }
    }
    public EquipQualityConfig GetEquipQualityConfig(int equipId)
    {
        EquipConfig equipcfg;
        return equipInfoConfigList.TryGetValue(equipId, out equipcfg) ? equipInfoConfigList[equipId].equipQualityConfig : null;
    }

    public List<int> GetEquipAllPropertyList(int equipId)
    {
        List<int> properties = new List<int>();
        var cfg = GetEquipQualityConfig(equipId);
        properties.Add(cfg.hp_basic);
        properties.Add(cfg.atk_basic);
        properties.Add(cfg.def_basic);
        properties.Add(cfg.spd_basic);
        properties.Add(cfg.acc_basic);
        properties.Add(cfg.dodge_basic);
        properties.Add(cfg.cri_basic);
        properties.Add(cfg.tough_basic);
        properties.Add(cfg.piercing_dmg);
        properties.Add(cfg.burn_dmg);
        properties.Add(cfg.ment_dmg);
        properties.Add(cfg.electricity_dmg);
        properties.Add(cfg.radiation_dmg);
        properties.Add(cfg.piercing_res);
        properties.Add(cfg.burn_res);
        properties.Add(cfg.ment_res);
        properties.Add(cfg.electricity_res);
        properties.Add(cfg.radiation_res);

        return properties;
    }

    public EquipConfig GetEquipInfoConfig(int equipid)
    {
        if (equipInfoConfigList.ContainsKey(equipid))
        {
            return equipInfoConfigList[equipid];
        }
        return null;
    }

    public EquipConfig GetEquipInfoConfig(int equipdrawingid, int quality)
    {
        var equalitycfg = GetEquipQualityConfig(equipdrawingid, quality);
        return GetEquipInfoConfig(equalitycfg.id);
    }

    //lua侧枚举不支持静默转换int/string lua侧重载使用
    public EquipClassification GetEquipTypeByID(EquipSubType subType)
    {
        return GetEquipTypeByID((int)subType);
    }

    public EquipClassification GetEquipTypeByID(int targetID)
    {
        if (equipTypeDic.ContainsKey(targetID))
        {
            return equipTypeDic[targetID];
        }
        return null;
    }

    //根据等级获取图纸列表
    public List<EquipDrawingsConfig> GetEquipDrawingsConfigs(int lv, int subtype)
    {
        var list = equipDrawingsCfgMap.Values.ToList().FindAll(t => t.level == lv && t.sub_type == subtype);
        return list;
    }
    public List<EquipDrawingsConfig> GetEquipsByWorkerId(int workerId)
    {
        int workerLevel = RoleDataProxy.inst.GetWorker(workerId).level;

        var list = equipDrawingsCfgMap.Values.ToList().FindAll(t =>
        {
            if (t.artisan_id == null || t.artisan_id.Length == 0) return false;

            int index = Array.IndexOf(t.artisan_id, workerId);

            EquipData equipData = EquipDataProxy.inst.GetEquipData(t.id);

            if (equipData == null || equipData.equipState != 2) return false;

            if (index != -1)
            {
                return t.artisan_lv[index] <= workerLevel;
            }

            return false;
        });

        list.Sort((a, b) => -a.level.CompareTo(b.level));
        return list;
    }


    public List<EquipDrawingsConfig> GetAllEquipConfigs()
    {
        return equipDrawingsCfgMap.Values.ToList();
    }

    public List<EquipDrawingsConfig> GetEquipConfigsByType(EquipSubType type)
    {
        int curCanUseLevelMax = MarketEquipLvManager.inst.GetCurMarketLevel();
        return GetAllEquipConfigs().FindAll(item => item.sub_type == (int)type && item.level <= curCanUseLevelMax);
    }

    public List<EquipDrawingsConfig> GetEquipConfigsByTypes(int[] subtypes)
    {
        List<EquipDrawingsConfig> result = new List<EquipDrawingsConfig>();
        List<EquipDrawingsConfig> allList = GetAllEquipConfigs();
        for (int i = 0; i < allList.Count; i++)
        {
            for (int k = 0; k < subtypes.Length; k++)
            {
                if (allList[i].sub_type == subtypes[k])
                {
                    result.Add(allList[i]);
                }
            }
        }

        return result;
    }

    //获取装备最大上架钻石数
    public int GetEquipSubmitGemMax(int equipId, int quality)
    {
        var equalitycfg = GetEquipQualityConfig(equipId, 1); //获取基础品质金币价值
        int basicGoldPrice = equalitycfg.auction_price;

        float result = 0;

        if (basicGoldPrice <= 200)
        {
            result = 10;
        }
        else if (basicGoldPrice <= 2000)
        {
            result = 15;
        }
        else
        {
            result = 10 + 5 * (int)Math.Log(Mathf.CeilToInt(basicGoldPrice / 1000f), 2);
        }

        result *= StaticConstants.qualityGemCoefficient[quality - 1] * 2;


        return Mathf.FloorToInt(result);
    }

}
