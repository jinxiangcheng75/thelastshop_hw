using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreasureBoxConfigData
{
    public int id;
    public int box_group;
    public string box_name;
    public int box_item_id;
    public int key_item_id;
    public int cost_num;
    public int rarity;
    public int reward_type1;
    public string reward_name1;
    public int reward_item_id1;
    public int reward_item_num1;
    public int probability;
}

public class TreasureBoxConfigManager : TSingletonHotfix<TreasureBoxConfigManager>, IConfigManager
{
    public Dictionary<int, TreasureBoxConfigData> cfgList = new Dictionary<int, TreasureBoxConfigData>();
    public const string CONFIG_NAME = "box";

    public void InitCSVConfig()
    {
        List<TreasureBoxConfigData> scArray = CSVParser.GetConfigsFromCache<TreasureBoxConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public TreasureBoxConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public TreasureBoxConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public TreasureBoxConfigData GetConfigByBoxItemId(int boxItemId)
    {
        foreach (var item in cfgList.Values)
        {
            if (item.box_item_id == boxItemId)
                return item;
        }

        Logger.error("没有boxItemId为" + boxItemId + "的数据");
        return null;
    }

    public TreasureBoxConfigData GetConfigByGroup(int group)
    {
        foreach (var item in cfgList.Values)
        {
            if (item.box_group == group)
                return item;
        }
        return null;
    }
}
