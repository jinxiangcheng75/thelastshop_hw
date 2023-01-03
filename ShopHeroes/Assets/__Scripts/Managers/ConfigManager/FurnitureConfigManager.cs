using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
 * 
 * 
 * tier0
 * 1. 墙壁
 * 2. 地板
 * 3. 地毯
 * 4. 墙壁装饰
 * 5. 室内地面装饰 
 * 6. 柜台
 * 7. 货架
 * 8. 储物箱
 * 9. 资源篮
 * 10. 室外装饰
 * 
 * tier1
 * 货架-
 *      1. 热武器 
 *      2. 冷兵器
 *      3. 防具
 *      4. 散件
 * 资源篮-
 *      1. 铁
 *      2. 木头
 *      3. 皮革
 *      4. 草药
 *      5. 钢
 *      6. 硬木
 *      7. 布料
 *      8. 储油
 *      9. 珠宝
 *      10. 以太
 *  
 * 储物箱-
 *      1. 储物箱
 *      2. 墙壁储物箱
 *  
 * 
 * 
 * */
public class FurnitureConfig
{
    public int id;
    public string name;
    public string icon;
    public string atlas;
    public string icon_big;
    public string prefabnew;
    public string sprites;
    public string furniture_type_icon;
    public string prefab;
    public string des;
    public int[] iconitem_id;
    public int type_1;
    public int type_2;
    public int placement_num;
    public int show_lv;
    public int unlock_lv;
    public int is_show;
    public int cost_type;
    public int cost_num;
    public int cost_times;
    public int length;
    public int width;
    public int height;
    public int luxury;
    public int energy;
    public int[] buff_ids;
}

public sealed class FurnitureConfigManager : BaseConfigManager<int, FurnitureConfig, FurnitureConfigManager>
{

    protected override string Csv_name => "furniture";

    protected override void processConfigs(List<FurnitureConfig> list)
    {
        foreach (var sc in list)
        {
            if (sc.id > 0)
                mDict.Add(sc.id, sc);
        }
    }

    public string getPrefab(int id)
    {
        var cfg = getConfig(id);
        return cfg?.prefab;
    }

    public string getPrefabByType(int type_1, int type_2)
    {
        foreach (var cfg in getList())
        {
            if (cfg.type_1 == type_1 && cfg.type_2 == type_2)
            {
                return cfg.prefab;
            }
        }
        return null;
    }

    public int[] getSameTypeFurnitureId(int furnId)
    {
        var cfg = getConfig(furnId);
        if (cfg == null) return null;
        int type_1 = cfg.type_1;
        int type_2 = cfg.type_2;
        int[] furnIdArr = new int[2];
        int index = 0;

        foreach (var item in getList())
        {
            if (item.id != furnId && item.type_1 == type_1 && item.type_2 == type_2)
            {
                furnIdArr[index] = item.id;
                index++;
            }
        }

        return furnIdArr;
    }
}

public class BaseUpgradeConfig
{
    public int id;
    public int furniture_id;
    public int time;//升级时间
    public int level;
    public int shopkeeper_level;
    public int money;
    public int diamond;  //立即升级需要的钻石
    public int start_diamond;       //开始加速钻石数
    public int rate;            //加速时间  每钻石/S
}

public class ShelfUpgradeConfig : BaseUpgradeConfig
{
    public int type;
    public int energy;
    public int store;//存储空间
    public int[] field_1;
    public int[] field_2;
    public int[] field_3;
    public int[] field_4;
    public int[] field_5;
    public int[] field_6;
    public int[] field_7;
    public int[] field_8;
    public int[] field_9;
    public int[] field_10;
    public int[] field_11;
    public int[] field_12;

    public int[] getFieldByLevel(int level)
    {
        switch (level)
        {
            case 1: return field_1;
            case 2: return field_2;
            case 3: return field_3;
            case 4: return field_4;
            case 5: return field_5;
            case 6: return field_6;
            case 7: return field_7;
            case 8: return field_8;
            case 9: return field_9;
            case 10: return field_10;
            case 11: return field_11;
            case 12: return field_12;
        }
        return null;
    }
}

public sealed class ShelfUpgradeConfigManager : BaseConfigManager<int, ShelfUpgradeConfig, ShelfUpgradeConfigManager>, IConfigManager
{
    protected override string Csv_name { get { return "furniture_shelf_update"; } }
    Dictionary<int, ShelfUpgradeConfig>[] mTypedDict;
    protected override void processConfigs(List<ShelfUpgradeConfig> list)
    {
        mTypedDict = new Dictionary<int, ShelfUpgradeConfig>[(int)kShelfType.Num];
        foreach (var sc in list)
        {
            mDict.Add(sc.id, sc);

            var dict = mTypedDict[sc.type];
            if (dict == null)
            {
                dict = new Dictionary<int, ShelfUpgradeConfig>();
                mTypedDict[sc.type] = dict;
            }
            dict.Add(sc.level, sc);
        }
    }


    public ShelfUpgradeConfig getConfigByType(int type2, int level)
    {
        if (type2 < 0 || type2 >= mTypedDict.Length)
        {
            return null;
        }
        var dict = mTypedDict[type2];
        if (dict != null)
        {
            ShelfUpgradeConfig cfg = null;
            dict.TryGetValue(level, out cfg);
            return cfg;
        }
        return null;
    }

    public List<int> GetShelfImgTypes(int shelfType, bool isDecollator = true)
    {
        var cfg = getConfigByType(shelfType, 12);

        List<int> types = new List<int>();

        if (shelfType == (int)EShelfType.Weapons)
        {
            types = new List<int>(cfg.getFieldByLevel(1));
        }
        else
        {

            //if (shelfType == (int)EShelfType.Firearm) isDecollator = false;

            for (int i = 1; i <= 4; i++)
            {
                int[] field = cfg.getFieldByLevel(i);

                for (int k = 0; k < field.Length; k++)
                {
                    if (!types.Contains(field[k]))
                    {
                        types.Add(field[k]);
                    }
                }

                //if (i < 3 && isDecollator)
                //{
                //    //表示切割符号
                //    types.Add(0);
                //}
            }
        }

        return types;
    }

}

public class ResourceBinUpgradeConfig : BaseUpgradeConfig
{
    public int type;
    public int store;//储量
    public int build_id;//需求建筑id
    public int build_level;//需求建筑等级
    public int item_id;
}

public class ResourceBinUpgradeConfigManager : BaseConfigManager<int, ResourceBinUpgradeConfig, ResourceBinUpgradeConfigManager>
{
    protected override string Csv_name => "furniture_res_upgrade";
    Dictionary<int, ResourceBinUpgradeConfig>[] mTypedDict;
    protected override void processConfigs(List<ResourceBinUpgradeConfig> list)
    {
        mTypedDict = new Dictionary<int, ResourceBinUpgradeConfig>[(int)kResourceBinType.Num];
        foreach (var sc in list)
        {
            mDict.Add(sc.id, sc);

            var dict = mTypedDict[sc.type];
            if (dict == null)
            {
                dict = new Dictionary<int, ResourceBinUpgradeConfig>();
                mTypedDict[sc.type] = dict;
            }
            dict.Add(sc.level, sc);
        }
    }

    public ResourceBinUpgradeConfig getConfigByType(int type2, int level)
    {
        if (mTypedDict == null) return null;
        if (type2 < 0 || type2 >= mTypedDict.Length)
        {
            return null;
            //throw new System.ArgumentException("type2 not valid : " + type2);
        }
        var dict = mTypedDict[type2];
        if (dict != null)
        {
            ResourceBinUpgradeConfig cfg = null;
            dict.TryGetValue(level, out cfg);
            return cfg;
        }
        return null;
    }
}

public class TrunkUpgradeConfig : BaseUpgradeConfig
{
    public int space;
    public int pile_space;
}
public class TrunkUpgradeConfigManager : BaseConfigManager<int, TrunkUpgradeConfig, TrunkUpgradeConfigManager>
{
    protected override string Csv_name => "furniture_trunk_upgrade";
    protected override void processConfigs(List<TrunkUpgradeConfig> list)
    {
        foreach (var sc in list)
        {
            mDict.Add(sc.level, sc);
        }
    }
}

public class CounterUpgradeConfig : BaseUpgradeConfig
{
    public int energy;
}

public class CounterUpgradeConfigManager : BaseConfigManager<int, CounterUpgradeConfig, CounterUpgradeConfigManager>
{
    protected override string Csv_name => "furniture_counter_upgrade";
    protected override void processConfigs(List<CounterUpgradeConfig> list)
    {
        foreach (var sc in list)
        {
            mDict.Add(sc.level, sc);
        }
    }
}
public class FurnitureUpgradeConfigManager : TSingletonHotfix<FurnitureUpgradeConfigManager>
{
    public CounterUpgradeConfig GetCounterUpgradeConfig(int counterlevel)
    {
        return CounterUpgradeConfigManager.inst.getConfig(counterlevel);
    }
    public ShelfUpgradeConfig GetShelfUpgradeConfig(int type2, int shelflevel)
    {
        return ShelfUpgradeConfigManager.inst.getConfigByType(type2, shelflevel);
    }
    public TrunkUpgradeConfig GetTrunkUpgradeConfig(int trunklevel)
    {
        return TrunkUpgradeConfigManager.inst.getConfig(trunklevel);
    }
    public ResourceBinUpgradeConfig GetResourceBinUpgradeConfig(int type2, int level)
    {
        return ResourceBinUpgradeConfigManager.inst.getConfigByType(type2, level);
    }
}
public class FurnitureClassificationConfig
{
    public int id;
    public string name;
    public string atlas;
    public string icon;
}

public class FurnitureClassificationConfigManager : BaseConfigManager<int, FurnitureClassificationConfig, FurnitureClassificationConfigManager>
{
    protected override string Csv_name => "funiture_classification";
    protected override void processConfigs(List<FurnitureClassificationConfig> list)
    {
        foreach (var sc in list)
        {
            mDict.Add(sc.id, sc);
        }
    }

}
public class FurnitureItemiconConfig
{
    public int id;
    public string name;
    public string atlas;
    public string icon;
}

public class FurnitureItemiconConfigManager : BaseConfigManager<int, FurnitureItemiconConfig, FurnitureItemiconConfigManager>
{
    protected override string Csv_name => "funiture_itemicon";
    protected override void processConfigs(List<FurnitureItemiconConfig> list)
    {
        foreach (var sc in list)
        {
            mDict.Add(sc.id, sc);
        }
    }
}