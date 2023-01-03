using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class StreetDropPosConfig
{
    public int id;
    public int pos_x;
    public int pos_y;
    public int probability;
}

public class StreetDropPosConfigManager : TSingletonHotfix<StreetDropPosConfigManager>, IConfigManager
{
    public Dictionary<int, StreetDropPosConfig> cfgList = new Dictionary<int, StreetDropPosConfig>();
    public const string CONFIG_NAME = "collect_rubbish_pos";

    public void InitCSVConfig()
    {
        List<StreetDropPosConfig> scArray = CSVParser.GetConfigsFromCache<StreetDropPosConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public StreetDropPosConfig GetRandomConfig()
    {
        var list = cfgList.Values.ToList();

        int[] weights = new int[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            weights[i] = list[i].probability;
        }


        int index = Helper.getRandomValuefromweights(weights);

        return list[index];
    }

}
