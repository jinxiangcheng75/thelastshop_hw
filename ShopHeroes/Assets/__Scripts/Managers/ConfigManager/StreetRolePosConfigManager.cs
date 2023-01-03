using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreetRoleOriPosConfig
{
    public int id;
    public int direction; //方向 0左；1右
    public int pos_x;
    public int pos_y;
    public int probablity;
    public int if_rubbish;//是否是垃圾人的生成点 0不是；1是
}

public class StreetRolePosConfigManager : TSingletonHotfix<StreetRolePosConfigManager>, IConfigManager
{
    public Dictionary<int, StreetRoleOriPosConfig> cfgList = new Dictionary<int, StreetRoleOriPosConfig>();
    public const string CONFIG_NAME = "ai_pos";

    List<StreetRoleOriPosConfig> passbyCfgList = new List<StreetRoleOriPosConfig>();//路人
    List<StreetRoleOriPosConfig> dropCfgList = new List<StreetRoleOriPosConfig>();//会掉落的路人

    public void InitCSVConfig()
    {
        List<StreetRoleOriPosConfig> scArray = CSVParser.GetConfigsFromCache<StreetRoleOriPosConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);

            if (item.if_rubbish == 0) passbyCfgList.Add(item);
            else if (item.if_rubbish == 1) dropCfgList.Add(item);

        }
    }
    public void ReLoadCSVConfig()
    {
        passbyCfgList.Clear();
        dropCfgList.Clear();
        cfgList.Clear();
        InitCSVConfig();
    }
    //获取路人初始的全部位置
    public List<StreetRoleOriPosConfig> GetGetPassbyConfigs()
    {
        return passbyCfgList;
    }

    //获取路人的初始位置配置
    public StreetRoleOriPosConfig GetPassbyConfig()
    {
        int[] weights = new int[passbyCfgList.Count];

        for (int i = 0; i < passbyCfgList.Count; i++)
        {
            weights[i] = passbyCfgList[i].probablity;
        }


        int index = Helper.getRandomValuefromweights(weights);

        return passbyCfgList[index];
    }


    //获取会掉落垃圾的路人初始位置配置
    public StreetRoleOriPosConfig GetDropConfig()
    {
        int[] weights = new int[dropCfgList.Count];

        for (int i = 0; i < dropCfgList.Count; i++)
        {
            weights[i] = dropCfgList[i].probablity;
        }


        int index = Helper.getRandomValuefromweights(weights);

        return dropCfgList[index];
    }

}
