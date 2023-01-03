using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//全服buff
public class GlobalBuffConfig
{
    public int id;
    public int type;
    //public string type_des;//不使用
    public string type_atlas;
    public string type_icon;
    //public string title_0;
    //public string title_1;
    //public string title_2;
    //public string title_3;
    //public int add_times;
    //public string con;
    //public string herald;
    //public int add_start;
    //public int add_end;

}

public class GlobalBuffConfigManager : TSingletonHotfix<GlobalBuffConfigManager>, IConfigManager
{
    public Dictionary<int, GlobalBuffConfig> cfgList = new Dictionary<int, GlobalBuffConfig>();
    public const string CONFIG_NAME = "overall_buff";

    public void InitCSVConfig()
    {
        List<GlobalBuffConfig> scArray = CSVParser.GetConfigsFromCache<GlobalBuffConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public GlobalBuffConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public GlobalBuffConfig GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

}
