using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UrlUpdateConfigData
{
    public int id;
    public string url_update;
    public string appsflyer_dev_key;
}

public class UrlUpdateConfigManager : TSingletonHotfix<UrlUpdateConfigManager>, IConfigManager
{
    public Dictionary<int, UrlUpdateConfigData> cfgList = new Dictionary<int, UrlUpdateConfigData>();
    public const string CONFIG_NAME = "url_update_android";

    public void InitCSVConfig()
    {
        cfgList.Clear();
        List<UrlUpdateConfigData> scArray = CSVParser.GetConfigsFromCache<UrlUpdateConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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

    public void PreloadCsvConfig(string content)
    {
        try
        {
            List<UrlUpdateConfigData> scArray = CSVParser.ParseConfigs<UrlUpdateConfigData>(content, CSVParser.STRING_SPLIT);

            foreach (var item in scArray)
            {
                if (item.id <= 0) continue;
                cfgList.Add(item.id, item);
            }
        }
        catch (System.Exception ex)
        {
            Logger.logException(ex);
        }
    }

    public UrlUpdateConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public UrlUpdateConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }
}
