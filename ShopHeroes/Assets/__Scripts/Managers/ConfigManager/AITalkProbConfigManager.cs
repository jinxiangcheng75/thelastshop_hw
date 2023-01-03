using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITalkProbConfig
{
    public int id;
    public int[] weight_1_lv;
    public int weight_1;
    public int[] weight_2_lv;
    public int weight_2;
    public int aitalk_times;
    public int furniture1_probability;
    public int furniture2_probability;
    public int pets_probability;
    public int end_probability;
    public int talk_type;
}
public class AITalkProbConfigManager : TSingletonHotfix<AITalkProbConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "aistroll";
    public Dictionary<int, AITalkProbConfig> dic = new Dictionary<int, AITalkProbConfig>();
    public void InitCSVConfig()
    {
        List<AITalkProbConfig> buildingArr = CSVParser.GetConfigsFromCache<AITalkProbConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingArr)
        {
            if (sc.id <= 0) continue;
            dic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        dic.Clear();
        InitCSVConfig();
    }

    public AITalkProbConfig GetConfig(int id)
    {
        if (dic.ContainsKey(id))
        {
            return dic[id];
        }
        return null;
    }

    public int[] GetWeights(int id_min, int id_max)
    {
        List<int> result = new List<int>();

        var allConfigs = dic.Values.ToList();

        var list = allConfigs.FindAll(t => t.id >= id_min && t.id < id_max);

        foreach (var item in list)
        {

            if (UserDataProxy.inst.playerData.level >= item.weight_1_lv[0] && UserDataProxy.inst.playerData.level <= item.weight_1_lv[1]) 
            {
                result.Add(item.weight_1);
            }
            else if(UserDataProxy.inst.playerData.level >= item.weight_2_lv[0] && UserDataProxy.inst.playerData.level <= item.weight_2_lv[1])
            {
                result.Add(item.weight_2);
            }
            else
            {
                result.Add(0);
            }

        }

        return result.ToArray();
    }

}
