using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class equipDisplayConfig
{
    public uint id;
    public int equipment_id;
    public string equipment_name;
    public int type;
    public int level;
    public float[] slot1_data;
    public float[] slot2_data;
    public float[] slot3_data;
    public float[] slot4_data;
    public float[] slot5_data;
    public float[] slot6_data;
    public float[] slot7_data;
    public float[] slot8_data;
    public float[] slot9_data;
    public float[] slot10_data;
    public float[] slot11_data;
    public float[] slot12_data;


    public float[] getSlotByIndex(int index)
    {
        switch (index)
        {
            case 1: return slot1_data;
            case 2: return slot2_data;
            case 3: return slot3_data;
            case 4: return slot4_data;
            case 5: return slot5_data;
            case 6: return slot6_data;
            case 7: return slot7_data;
            case 8: return slot8_data;
            case 9: return slot9_data;
            case 10: return slot10_data;
            case 11: return slot11_data;
            case 12: return slot12_data;
        }
        return null;
    }
}

public class ShelfDisplayConfigManager : TSingletonHotfix<ShelfDisplayConfigManager>, IConfigManager
{
    public Dictionary<uint, equipDisplayConfig> cfgList = new Dictionary<uint, equipDisplayConfig>();
    public const string CONFIG_NAME = "shelf_dress";

    public void InitCSVConfig()
    {
        List<equipDisplayConfig> scArray = CSVParser.GetConfigsFromCache<equipDisplayConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public equipDisplayConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public equipDisplayConfig GetConfig(int equipId, int shelfType, int shelfLevel)
    {
        foreach (var item in cfgList.Values)
        {
            if (item.equipment_id == equipId && item.type == shelfType && item.level == shelfLevel)
            {
                return item;
            }
        }

        return null;
    }

}
