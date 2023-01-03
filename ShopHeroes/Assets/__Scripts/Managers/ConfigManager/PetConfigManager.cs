using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetConfig //注释字段均在lua侧对应Config中
{
    public int id;
    //public int type;
    //public string name;
    //public string icon;
    public int model;
    //public int furniture_id;
    //public int show_lv;
    //public int unlock_level;
    //public int unlock_type;
    //public int unlock_sale_id;
    public int pet_music_id;
    //public int unlock_money;
    //public int unlock_diamond;
    public float moveSpeed;
}

public class PetConfigManager : TSingletonHotfix<PetConfigManager>, IConfigManager
{
    public Dictionary<int, PetConfig> configDic = new Dictionary<int, PetConfig>();

    public const string CONFIG_NAME = "pet";

    public void InitCSVConfig()
    {
        List<PetConfig> arr = CSVParser.GetConfigsFromCache<PetConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in arr)
        {
            if (sc.id <= 0) continue;
            configDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        configDic.Clear();

        InitCSVConfig();
    }

    public PetConfig[] GetAllConfig()
    {
        return configDic.Values.ToArray();
    }

    public PetConfig GetConfig(int key)
    {
        if (configDic.ContainsKey(key))
        {
            return configDic[key];
        }
        return null;
    }

}
