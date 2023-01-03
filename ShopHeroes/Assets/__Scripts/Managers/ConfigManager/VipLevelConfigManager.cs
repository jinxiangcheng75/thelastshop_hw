using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VipLevelConfigData
{
    public int id;
    public int level;
    public int type_1;
    public int type_2;
    public int type_3;
    public int type_4;
    public int type_5;
    public int type_6;
    public int type_7;
    public int type_8;
    public int type_9;
    public int type_10;
    public int type_11;
}

public class VipLevelConfigManager : TSingletonHotfix<VipLevelConfigManager>, IConfigManager
{
    public Dictionary<int, VipLevelConfigData> cfgList = new Dictionary<int, VipLevelConfigData>();
    public const string CONFIG_NAME = "vip_level";

    public void InitCSVConfig()
    {
        List<VipLevelConfigData> scArray = CSVParser.GetConfigsFromCache<VipLevelConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
    }

    public VipLevelConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public VipLevelConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public int GetValByLevelAndType(int level, K_Vip_Type type)
    {
        var cfg = GetConfig(level);

        switch (type)
        {
            case K_Vip_Type.UnlockRepairEquipPower:
                return cfg.type_1;
            case K_Vip_Type.RepairEquipReduce:
                return cfg.type_2;
            case K_Vip_Type.UnlockActivityReward:
                return cfg.type_3;
            case K_Vip_Type.ExploreDropAdd:
                return cfg.type_4;
            case K_Vip_Type.OpenTBoxAdd:
                return cfg.type_5;
            case K_Vip_Type.PiggyRewardAdd:
                return cfg.type_6;
            case K_Vip_Type.UnlockTaskFourthReward:
                return cfg.type_7;
            case K_Vip_Type.CanReceiveTaskFourthReward:
                return cfg.type_8;
            case K_Vip_Type.LotteryTenthPriceReduce:
                return cfg.type_9;
            case K_Vip_Type.MallBuyGoodsPriceReduce:
                return cfg.type_10;
            case K_Vip_Type.RecruitHeroPriceReduce:
                return cfg.type_11;
            case K_Vip_Type.ReceiveTargetDecoration:
                return -1;
        }

        return -1;
    }

    public void ReLoadCSVConfig()
    {
        cfgList.Clear();
        InitCSVConfig();
    }
}
