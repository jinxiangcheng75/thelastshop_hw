//配置文件读取类管理 
//注释 “#？？#” 为批处理标记 不要随意删除


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class itemConfig
{
    public int id;
    public string name;
    public string atlas;
    public string icon;
    public string model;
    public int type;
    public int property;
    public string desc;
    public int price;
    public int npc_price;
    public int low_price_m;
    public int low_price_d;
    public int amount_cap = 999999;
    public int transaction;
    //public int consume;
    public int effect;
    public int effect_val;

    /*public itemconfig(SingleContent sc)
    {
        if (!string.IsNullOrEmpty(sc["id(k:u)"])) id = uint.Parse(sc["id(k:u)"]);
        if (!string.IsNullOrEmpty(sc["name(v:s)"])) name = sc["name(v:s)"];
        if (!string.IsNullOrEmpty(sc["icon(v:s)"])) icon = sc["icon(v:s)"];
        if (!string.IsNullOrEmpty(sc["model(v:s)"])) model = sc["model(v:s)"];
        if (!string.IsNullOrEmpty(sc["type(v:u)"])) type = uint.Parse(sc["type(v:u)"]);
        if (!string.IsNullOrEmpty(sc["desc(v:s)"])) desc = sc["desc(v:s)"];
        if (!string.IsNullOrEmpty(sc["lowPrice_m(v:u)"])) lowPrice_m = uint.Parse(sc["lowPrice_m(v:u)"]);
        if (!string.IsNullOrEmpty(sc["lowPrice_d(v:u)"])) lowPrice_d = uint.Parse(sc["lowPrice_d(v:u)"]);
        if (!string.IsNullOrEmpty(sc["consume(v:u)"])) consume = uint.Parse(sc["consume(v:u)"]);
    }*/
}

public class ItemconfigManager : TSingletonHotfix<ItemconfigManager>, IConfigManager
{
    public Dictionary<int, itemConfig> cfgList = new Dictionary<int, itemConfig>();
    public const string CONFIG_NAME = "item";

    public void InitCSVConfig()
    {
        //SingleContent[] scArray = CSVHelper.ReadConfigFromCSV(CONFIG_NAME);
        var scArray = CSVParser.GetConfigsFromCache<itemConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var sc in scArray)
        {
            //itemconfig cfg = new itemconfig(sc);
            if (sc.id <= 0) continue;
            cfgList.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public itemConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public itemConfig GetConfig(int key)
    {
        if (cfgList.ContainsKey(key))
        {
            return cfgList[key];
        }
        return null;
    }

    public itemConfig GetConfigByEffectId(int effectId)
    {
        foreach (var cfg in cfgList.Values)
        {
            if(cfg.effect == effectId)
            {
                return cfg;
            }
        }

        return null;
    }

}