using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FurnitureBuffConfigData
{
    public int id;
    public int type;
    public string type_title;
    public string buff_txt;
    public int parameter_0;
    public int parameter_1;
    public int parameter_2;
    public string type_atlas;
    public string type_icon;

    public int getEffectVal() 
    {
        int result = -1;

        switch ((FurnitureBuffType)type)
        {
            case FurnitureBuffType.sell_subTypePriceUp:
                result = parameter_2;
                break;
            case FurnitureBuffType.sell_allPriceUp:
                result = parameter_1;
                break;
            case FurnitureBuffType.make_subTypeSpeedUp:
                result = parameter_2;
                break;
            case FurnitureBuffType.make_allSpeedUp:
                result = parameter_1;
                break;
            case FurnitureBuffType.make_subTypeQualityUp:
                result = parameter_2;
                break;
            case FurnitureBuffType.make_allQualityUp:
                result = parameter_1;
                break;
            case FurnitureBuffType.res_allSpeedUp:
                result = parameter_1;
                break;
            case FurnitureBuffType.res_subTypeSpeedUp:
                result = parameter_2;
                break;
            case FurnitureBuffType.hero_qualityUp:
                result = parameter_2;
                break;
            case FurnitureBuffType.equip_damageRaceDown:
                result = parameter_1;
                break;
            default:
                break;
        }


        return result;
    }

}

public class FurnitureBuffConfigManager : TSingletonHotfix<FurnitureBuffConfigManager>, IConfigManager
{
    public Dictionary<int, FurnitureBuffConfigData> cfgList = new Dictionary<int, FurnitureBuffConfigData>();
    public const string CONFIG_NAME = "furniture_buff";

    public void InitCSVConfig()
    {
        List<FurnitureBuffConfigData> scArray = CSVParser.GetConfigsFromCache<FurnitureBuffConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);
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
    public FurnitureBuffConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public FurnitureBuffConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
