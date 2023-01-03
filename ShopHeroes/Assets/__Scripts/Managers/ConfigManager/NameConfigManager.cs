using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameConfig
{
    public int id;
    public string female_name;
    public string male_name;
}

public class NameConfigManager : TSingletonHotfix<NameConfigManager>, IConfigManager
{
    public Dictionary<int, NameConfig> worriorDic = new Dictionary<int, NameConfig>();
    public const string CONFIG_NAME = "name";

    public void InitCSVConfig()
    {
        List<NameConfig> worriorArr = CSVParser.GetConfigsFromCache<NameConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in worriorArr)
        {
            if (sc.id <= 0) continue;
            worriorDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        worriorDic.Clear();

        InitCSVConfig();
    }
    public string GetRandomName(EGender gender)
    {
        string name = "";

        while (string.IsNullOrEmpty(name))
        {
            var cfg = worriorDic[Random.Range(0, worriorDic.Count)];
            name = gender == EGender.Male ? cfg.male_name : cfg.female_name;
        }

        return name;
    }
}
