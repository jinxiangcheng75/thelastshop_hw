using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePayPricecConfig
{
    public int id;
    public int type;
    public string monetary_unit;
    public string money;
    public string order_info;
    public string ios_order_info;
}
public class GamePayPricecConfigManager : TSingletonHotfix<GamePayPricecConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "price_list";
    public Dictionary<string, GamePayPricecConfig> guideDic = new Dictionary<string, GamePayPricecConfig>();
    public void InitCSVConfig()
    {
        List<GamePayPricecConfig> guideArr = CSVParser.GetConfigsFromCache<GamePayPricecConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        for (int i = 0; i < guideArr.Count; i++)
        {
            int index = i;
#if !UNITY_IOS
            if (!guideDic.ContainsKey(guideArr[index].order_info))
                guideDic.Add(guideArr[index].order_info, guideArr[index]);
#else
            if (guideArr[index].ios_order_info == null) continue;
            Logger.log(guideArr[index].ios_order_info, "#fff554");
            if (!guideDic.ContainsKey(guideArr[index].ios_order_info))
                guideDic.Add(guideArr[index].ios_order_info, guideArr[index]);
#endif
        }
    }

    //获取所有商品id
    public string GetAllProductIds()
    {
        string products = "";
        foreach(string key in guideDic.Keys)
        {
            products = products + (key + "\t");
        }
        return products;
    }
    public void ReLoadCSVConfig()
    {
        guideDic.Clear();
        InitCSVConfig();
    }

    public string[] GetAllKey()
    {
        string[] keylist = new string[guideDic.Count];
        guideDic.Keys.CopyTo(keylist, 0);
        return keylist;
    }
    public string GetMoneystr(string pid)
    {
        if (guideDic.ContainsKey(pid))
        {
            return guideDic[pid].monetary_unit + guideDic[pid].money;
        }
        return "--";
    }
}
