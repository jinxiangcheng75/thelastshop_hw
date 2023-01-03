using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldCostConfig
{
    public int id;
    public int type;
    public int sequence;
    public int level;
    public long money;
    public int diamond;
}
public class FieldConfigManager : TSingletonHotfix<FieldConfigManager>, IConfigManager
{
    public const string CONFIG_FILENAME = "field_cost";
    public List<FieldCostConfig> data = new List<FieldCostConfig>();

    public void InitCSVConfig()
    {
        data = CSVParser.GetConfigsFromCache<FieldCostConfig>(CONFIG_FILENAME, CSVParser.STRING_SPLIT);
    }
    public void ReLoadCSVConfig()
    {
        data.Clear();
        InitCSVConfig();
    }
    public List<FieldCostConfig> GetFieldCostCfgs(int type)
    {
        var list = data.FindAll(item => item.type == type);
        //list.Sort((FieldCostConfig x, FieldCostConfig y)=>
        //{
        //    if (x.sequence == 0 && y.sequence == 0) return 0;
        //    else
        //    {
        //        return x.sequence.CompareTo(y.sequence);
        //    }
        //});
        return list;
    }

    public FieldCostConfig GetFieldConfig(int type, int sequence)
    {
        var cfg = data.Find(item => item.type == type && item.sequence == sequence);
        if (cfg != null)
        {
            return cfg;
        }
        Logger.log("查找FieldCostConfig配置失败: type = " + type.ToString());
        return null;
    }
}
