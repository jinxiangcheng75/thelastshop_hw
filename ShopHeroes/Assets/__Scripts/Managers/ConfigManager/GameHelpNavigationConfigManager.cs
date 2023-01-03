using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp]
public class helpConfig
{
    public int id;
    public int help_type;
    public int[] triggered_val_1;
    public int[] triggered_val_2;
    public string help_tips;
    public int link_type;
    public int link_group_id;
}

[XLua.LuaCallCSharp]
public class helpLinkConfig
{
    public int id;
    public int link_group;
    public string help_dec;
    public string atlas;
    public string icon;
    public string btn_txt;
    public string task_scenes;
    public int jump_val;
    public int jump_id_1;
    public int jump_id_2;
    public int jump_id_3;
    public int jump_id_4;
    public int jump_id_5;
    public string unlock_txt;
    public int unlock_lv;
    // public string interface;

}


public class GameHelpNavigationConfigManager : TSingletonHotfix<GameHelpNavigationConfigManager>, IConfigManager
{
    private const string CONFIG_NAME1 = "help";
    private const string CONFIG_NAME2 = "help_link";
    private Dictionary<int, helpConfig> helpconfgMap = new Dictionary<int, helpConfig>();
    private Dictionary<int, helpLinkConfig> helpLinkCfgMap = new Dictionary<int, helpLinkConfig>();
    public void InitCSVConfig()
    {
        helpconfgMap.Clear();
        helpLinkCfgMap.Clear();
        var scArray = CSVParser.GetConfigsFromCache<helpConfig>(CONFIG_NAME1, CSVParser.STRING_SPLIT);
        foreach (var sc in scArray)
        {
            //itemconfig cfg = new itemconfig(sc);
            if (sc.id <= 0) continue;
            helpconfgMap.Add(sc.id, sc);
        }
        scArray.Clear();
        var scArray2 = CSVParser.GetConfigsFromCache<helpLinkConfig>(CONFIG_NAME2, CSVParser.STRING_SPLIT);
        foreach (var sc in scArray2)
        {
            //itemconfig cfg = new itemconfig(sc);
            if (sc.id <= 0) continue;
            helpLinkCfgMap.Add(sc.id, sc);
        }
        scArray2.Clear();
    }
    public void ReLoadCSVConfig()
    {
        helpconfgMap.Clear();
        helpLinkCfgMap.Clear();
        InitCSVConfig();
    }
    public helpConfig GetHelpConfigBytyp(int type, int val1)
    {
        foreach (var cfg in helpconfgMap.Values)
        {
            if (cfg.help_type == type)
            {
                foreach (var val in cfg.triggered_val_1)
                {
                    if (val1 == val)
                    {
                        return cfg;
                    }
                }
            }
        }
        return null;
    }
    public helpConfig GetHelpConfig(int id)
    {
        if (helpconfgMap.ContainsKey(id))
        {
            return helpconfgMap[id];
        }
        return null;
    }


    public helpLinkConfig GetHelpLinkConfig(int id)
    {
        if (helpLinkCfgMap.ContainsKey(id))
        {
            return helpLinkCfgMap[id];
        }
        return null;
    }

    public List<helpLinkConfig> GetHelpLinkByGroup(int groupid)
    {
        List<helpLinkConfig> list = new List<helpLinkConfig>();
        foreach (var cfg in helpLinkCfgMap.Values)
        {
            if (groupid == cfg.link_group)
            {
                list.Add(cfg);
            }
        }
        return list;
    }
}
