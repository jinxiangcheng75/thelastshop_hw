using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageConfigData
{
    public string languages_game;   // key
    public string languages_CN;     // 简体中文
    public string languages_TC;     // 繁体中文
    public string languages_EN;     // 英文
}

public class LanguageConfigManager : TSingletonHotfix<LanguageConfigManager>, IConfigManager
{
    public Dictionary<string, LanguageConfigData> cfgDic = new Dictionary<string, LanguageConfigData>();
    public const string CONFIGSTART_NAME = "translate_start";
    public const string CONFIG_NAME = "translate";

    public void initstartcsvfile()
    {
        cfgDic.Clear();
        addLanguageConfigEx(CONFIGSTART_NAME);
    }
    public void InitCSVConfig()
    {
        cfgDic.Clear();

        addLanguageConfigEx(CONFIG_NAME);
        addIncrementalLanguageConfig();

    }

    private void addLanguageConfigEx(string cfgname)
    {
        var scArray = CSVParser.GetConfigsFromCache<LanguageConfigData>(cfgname, CSVParser.STRING_SPLIT);
        if (scArray != null)
        {
            foreach (var sc in scArray)
            {
                if (string.IsNullOrEmpty(sc.languages_game)) continue;
                if (cfgDic.ContainsKey(sc.languages_game))
                {
                    cfgDic[sc.languages_game] = sc;
                    continue;
                }
                cfgDic.Add(sc.languages_game, sc);
            }
        }
    }

    private void addIncrementalLanguageConfig()
    {
        int incrementIndex = 1;

        while (CsvCfgCatalogMgr.inst.IsContainsCsvByName(CONFIG_NAME + "_" + incrementIndex.ToString("D2")))
        {
            addLanguageConfigEx(CONFIG_NAME + "_" + incrementIndex.ToString("D2"));
            incrementIndex++;
        }

    }

    public void ReLoadCSVConfig()
    {
        cfgDic.Clear();

        InitCSVConfig();
    }
    public string CheckContentLanguage(string key)
    {
        if (cfgDic.ContainsKey(key))
        {
            return key;
        }
        foreach (var item in cfgDic.Values)
        {
            if (item.languages_CN == key)
            {
                return item.languages_game;
            }
            else if (item.languages_TC == key)
            {
                return item.languages_game;
            }
            else if (item.languages_EN == key)
            {
                return item.languages_game;
            }
        }

        return "-1";
    }

    public string GetCNLanguageConfig(string key)
    {
        if (cfgDic.ContainsKey(key))
        {
            string s = cfgDic[key].languages_CN;
            return s;
        }

        return key;
    }

    public string GetTRCNLanguageConfig(string key)
    {
        if (cfgDic.ContainsKey(key))
        {
            string s = cfgDic[key].languages_TC;
            return s;
        }

        return key;
    }

    public string GetENGLanguageConfig(string key)
    {
        if (cfgDic.ContainsKey(key))
        {
            string s = cfgDic[key].languages_EN;
            return s;
        }

        return key;
    }
}

