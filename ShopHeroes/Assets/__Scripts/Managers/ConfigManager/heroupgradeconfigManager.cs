using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class heroupgradeconfig
{
    public int level;
    public int exp;
    public int exp_r;
    public int exp_sr;
    public int exp_ssr;
    public int equip_lv;
    public int dismissal_cost;

    public int getExp(int rarity)
    {
        switch (rarity)
        {
            case 1: return exp;
            case 2: return exp_r;
            case 3: return exp_sr;
            case 4: return exp_ssr;
            default:return exp;
        }
    }

}

public class heroupgradeconfigManager : TSingletonHotfix<heroupgradeconfigManager>, IConfigManager
{
    public Dictionary<int, heroupgradeconfig> heroupgradeDic = new Dictionary<int, heroupgradeconfig>();

    public const string CONFIG_NAME = "hero_upgrade";

    private static heroupgradeconfigManager m_instance;

    public void InitCSVConfig()
    {
        List<heroupgradeconfig> heroupgradeArr = CSVParser.GetConfigsFromCache<heroupgradeconfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in heroupgradeArr)
        {
            if (sc.level <= 0) continue;
            heroupgradeDic.Add(sc.level, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        heroupgradeDic.Clear();

        InitCSVConfig();
    }
    public heroupgradeconfig[] GetAllHeroUpgradeConfig()
    {
        return heroupgradeDic.Values.ToArray();
    }

    public heroupgradeconfig GetHeroUpgradeConfig(int level)
    {
        if (heroupgradeDic.ContainsKey(level))
        {
            return heroupgradeDic[level];
        }
        return null;
    }

    public long GetSumExp(int level,int rarity)
    {
        long sumExp = 0;
        foreach (var item in heroupgradeDic.Values)
        {
            if (item.level <= level)
            {
                sumExp += item.getExp(rarity);
            }
        }

        return sumExp;
    }

    //通过装备等阶获取当前可穿戴该等阶的最低英雄等级
    public int GetCanWearFloorLvByEquipLv(int equipLv) 
    {
        int floorLv = 50;

        var arr = GetAllHeroUpgradeConfig();

        //正序从小到大遍历
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].equip_lv >= equipLv)
            {
                floorLv = arr[i].level;
                break;
            }
        }

        return floorLv;
    }

}
