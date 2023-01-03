using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//城市建筑物数据类
public class CityBuildingData
{
    public int buildingId;
    public int level;//建筑物等级
    //public int unionShareLevel;//公会共享等级 废弃
    public int oneSelfCostCount;//该建筑物自己投资次数
    public int unionAllShareCostCount;//公会共享总次数
    public int state; // EBuildState
    public BuildingConfig config;
    public BuildingUpgradeConfig upgradeConfig;
    //public BuildingUpgradeConfig shareUpgradeCfg;

    //前端临时红点
    public bool isNew;

    //该建筑的加成享有
    public float effectVal
    {
        get
        {
            float selfVal = upgradeConfig.effect_val;
            //float unionVal = shareUpgradeCfg.guild_effect_val;

            return selfVal /*+ (UserDataProxy.inst.playerData.unionId == "" ? 0 : unionVal)*/;
        }
    }

    public int costCount //当前次数
    {
        get
        {
            var oldCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, level - 1);
            return /*oneSelfCostCount*/unionAllShareCostCount - (oldCfg == null ? 0 : oldCfg.click_num);
        }
    }

    public int needClickCount //需要点击次数
    {
        get
        {
            var oldCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, level - 1);
            return upgradeConfig.click_num - (oldCfg == null ? 0 : oldCfg.click_num);
        }
    }

    //public int unionShareCostCount //公会共享次数
    //{
    //    get
    //    {
    //        var oldCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, unionShareLevel - 1);
    //        return unionAllShareCostCount - (oldCfg == null ? 0 : oldCfg.guild_click_num);
    //    }
    //}

    //public int unionShareNeedCount //公会当前等级需要点击次数
    //{
    //    get
    //    {
    //        if (config.architecture_type == 2) return 0;

    //        var oldCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, unionShareLevel - 1);
    //        return shareUpgradeCfg.guild_click_num - (oldCfg == null ? 0 : oldCfg.guild_click_num);
    //    }
    //}

    //public string GetShareEffectDec()
    //{
    //    string des = "";

    //    des = shareUpgradeCfg.guild_effect_val_type == 1 ? shareUpgradeCfg.guild_effect_val + "" : shareUpgradeCfg.guild_effect_val * 100 + "%";

    //    return des;
    //}


    public void SetInfo(int _id, int _level, int _state, int _oneSelfCostCount, int _unionCostCount)
    {
        buildingId = _id;
        level = _level;
        state = _state;
        oneSelfCostCount = _oneSelfCostCount;

        config = BuildingConfigManager.inst.GetConfig(_id);
        upgradeConfig = BuildingUpgradeConfigManager.inst.GetConfig(_id, level);

        if (config != null && config.architecture_type == 2) return; //功能性建筑

        unionAllShareCostCount = _unionCostCount;
        //shareUpgradeCfg = BuildingUpgradeConfigManager.inst.GetUnionShareConfig(_id, _unionCostCount);
        //unionShareLevel = shareUpgradeCfg.architecture_lv;
    }


    public string GetIconAndAtlas(out string atlas, int buildingLevel)
    {
        atlas = config.atlas;
        string icon = config.icon;

        if (config.architecture_type == 1)
        {
            atlas = config.atlas;
            icon = config.icon;
        }
        else if (config.unlock_type == (int)kCityBuildingUnlockType.NeedOneWorker)
        {
            WorkerData workerData = RoleDataProxy.inst.GetWorker(config.unlock_id);
            if (workerData != null)
            {
                atlas = StaticConstants.roleHeadIconAtlasName;
                icon = workerData.config.icon;
            }
            else
            {
                Logger.error("未获取到对应工匠 工匠ID ： " + config.unlock_id);
            }
        }
        else
        {
            if (buildingId == 3700)//康复研究 为具体的副本图片
            {
                var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, buildingLevel + 1);

                if (nextUpCfg == null) //满级了
                {
                    nextUpCfg = upgradeConfig;
                }

                ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(nextUpCfg.effect_id);
                atlas = StaticConstants.exploreAtlas;
                icon = cfg.instance_icon;
            }
            else if (buildingId == 3800)//探险 为具体的副本材料图标
            {
                var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, buildingLevel + 1);

                if (nextUpCfg == null) //满级了
                {
                    nextUpCfg = upgradeConfig;
                }

                Item item = ItemBagProxy.inst.GetItem(nextUpCfg.effect_id);
                if (item != null)
                {
                    atlas = item.itemConfig.atlas;
                    icon = item.itemConfig.icon;
                }
            }
        }

        return icon;
    }

    public string GetValueTipStr(bool nextLevel)
    {
        string result = "";

        if (buildingId == 3700)
        {
            var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, nextLevel ? level + 1 : level);

            if (nextUpCfg == null) //满级了
            {
                nextUpCfg = upgradeConfig;
            }

            ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(nextUpCfg.effect_id);
            result = LanguageManager.inst.GetValueByKey(nextUpCfg.effect_dec, LanguageManager.inst.GetValueByKey(cfg.instance_name));
        }
        else if (buildingId == 3800)
        {
            var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, nextLevel ? level + 1 : level);

            if (nextUpCfg == null) //满级了
            {
                nextUpCfg = upgradeConfig;
            }

            Item item = ItemBagProxy.inst.GetItem(nextUpCfg.effect_id);
            result = LanguageManager.inst.GetValueByKey(nextUpCfg.effect_dec, LanguageManager.inst.GetValueByKey(item.itemConfig.name));
        }
        else
        {
            var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(buildingId, nextLevel ? level + 1 : level);

            if (nextUpCfg == null) //满级了
            {
                nextUpCfg = upgradeConfig;
            }

            result = LanguageManager.inst.GetValueByKey(nextUpCfg.effect_dec);
        }

        return result;
    }

}
