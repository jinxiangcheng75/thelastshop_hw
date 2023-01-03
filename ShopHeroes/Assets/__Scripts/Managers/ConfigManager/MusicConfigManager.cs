using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicConfig
{
    public int ID;
    public string name;
    public string desc;
}

public class MusicConfigManager : TSingletonHotfix<MusicConfigManager>, IConfigManager
{
    public Dictionary<int, MusicConfig> cfgList = new Dictionary<int, MusicConfig>();
    public const string CONFIG_NAME = "music_req";

    public void InitCSVConfig()
    {
        List<MusicConfig> scArray = CSVParser.GetConfigsFromCache<MusicConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var item in scArray)
        {
            if (item.ID <= 0) continue;
            cfgList.Add(item.ID, item);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public string GetMusicName(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id].name;
        }
        return "";
    }
}
