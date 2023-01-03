using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WorkerConfig
{
    public int id;
    public string name;
    public string profession;
    public int type; // 1-工匠 2-Npc
    public string icon;
    public string profession_icon;
    public string desc;
    public int show_level;//展示等级
    public int level;//店主前提等级   
    public int cost_money;
    public int cost_diamond;
    public int build_id;
    public int build_level_id;
    public int[] equipment_id;//解锁蓝图
    public int locked;//礼包解锁 1是 2否
    public int get_type;
    public string lock_des;
    public int connect_build_id;
    public string pic;
    public int model;
    public int sale_id;
}


public class WorkerConfigManager : TSingletonHotfix<WorkerConfigManager>, IConfigManager
{
    public Dictionary<int, WorkerConfig> workerDic = new Dictionary<int, WorkerConfig>();

    public const string CONFIG_NAME = "artisan_item";

    public void InitCSVConfig()
    {
        List<WorkerConfig> worriorArr = CSVParser.GetConfigsFromCache<WorkerConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in worriorArr)
        {
            if (sc.id <= 0) continue;
            workerDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        workerDic.Clear();

        InitCSVConfig();
    }
    public WorkerConfig[] GetAllConfig()
    {
        return workerDic.Values.ToArray();
    }

    public WorkerConfig GetConfig(int key)
    {
        if (workerDic.ContainsKey(key))
        {
            return workerDic[key];
        }
        return null;
    }


}
