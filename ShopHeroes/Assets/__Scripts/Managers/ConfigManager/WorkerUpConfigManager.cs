using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkerUpConfig
{
    public int artisan_lv;
    public int artisan_exp;
    public float crafting_speed_bonus;
}


public class WorkerUpConfigManager : TSingletonHotfix<WorkerUpConfigManager>, IConfigManager
{
    public Dictionary<int, WorkerUpConfig> workerUpDic = new Dictionary<int, WorkerUpConfig>();

    public const string CONFIG_NAME = "artisan_update";

    public void InitCSVConfig()
    {
        List<WorkerUpConfig> worriorArr = CSVParser.GetConfigsFromCache<WorkerUpConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in worriorArr)
        {
            if (sc.artisan_lv <= 0) continue;
            workerUpDic.Add(sc.artisan_lv, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        workerUpDic.Clear();

        InitCSVConfig();
    }
    public WorkerUpConfig[] GetAllConfig()
    {
        return workerUpDic.Values.ToArray();
    }

    public WorkerUpConfig GetConfig(int key)
    {
        if (workerUpDic.ContainsKey(key))
        {
            return workerUpDic[key];
        }
        return null;
    }

}
