using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OperationConfigData
{
    public int id;
    public int group_type;
    public int type;
    public string type_des;
    public string group_des;
    public string main_interface;
    public int if_special;
    public string if_special_des;
    public string btn;
    public string guide_btn_dev;
}

public class OperationConfigManager : TSingletonHotfix<OperationConfigManager>, IConfigManager
{
    public Dictionary<int, OperationConfigData> cfgList = new Dictionary<int, OperationConfigData>();
    public const string CONFIG_NAME = "operations_database";

    public void InitCSVConfig()
    {
        List<OperationConfigData> scArray = CSVParser.GetConfigsFromCache<OperationConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public OperationConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public OperationConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
