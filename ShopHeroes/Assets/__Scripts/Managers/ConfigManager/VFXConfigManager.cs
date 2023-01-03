using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXConfig
{
    public int id;
    public string vfxname;
    public bool isLoop;
    public float speed;
    public int time;    //生命周期， 毫秒
}
public class VFXConfigManager : TSingletonHotfix<VFXConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "vfx_mobile";
    private Dictionary<int, VFXConfig> cfgList = new Dictionary<int, VFXConfig>();
    public Dictionary<int, VFXConfig> VFXCfgs
    {
        get { return cfgList; }
    }

    public void InitCSVConfig()
    {
        List<VFXConfig> resList = CSVParser.GetConfigsFromCache<VFXConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var sc in resList)
        {
            if (sc.id <= 0) continue;
            cfgList.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public VFXConfig GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }
        return null;
    }

    public bool CheckForExistingTemplate(int vfxid)
    {
        if (cfgList.ContainsKey(vfxid))
        {
            return true;
        }
        return false;
    }
}
