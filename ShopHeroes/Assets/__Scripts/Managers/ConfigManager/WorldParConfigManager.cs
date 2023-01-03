using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldParConfig
{
    public int id;
    public float parameters;
    public string name;
}

public class WorldParConfigManager : TSingletonHotfix<WorldParConfigManager>, IConfigManager
{
    public Dictionary<int, WorldParConfig> cfgList = new Dictionary<int, WorldParConfig>();
    public const string CONFIG_NAME = "world_par";

    public void InitCSVConfig()
    {
        List<WorldParConfig> scArray = CSVParser.GetConfigsFromCache<WorldParConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            cfgList.Add(item.id, item);
        }
        SetStaticView();
        setCameraBloom();
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public WorldParConfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public WorldParConfig GetConfig(int field)
    {
        if (cfgList.ContainsKey(field))
        {
            return cfgList[field];
        }
        return null;
    }

    private void SetStaticView()
    {
        StaticConstants.moveSpeed = cfgList[116].parameters * .1f;
    }

    private void setCameraBloom()
    {
        float bloomThreshold = 1.5f;
        float bloomIntensity = 10;
        float bloomScatter = 0;

        if (cfgList.TryGetValue(8205, out WorldParConfig bloomCfg))
        {
            bool active = bloomCfg.parameters == 1;
            D2DragCamera.inst.SetVolumeBloomActive(active);

            if (active)
            {
                if (cfgList.TryGetValue(8206, out bloomCfg))
                {
                    bloomThreshold = bloomCfg.parameters;
                }
                if (cfgList.TryGetValue(8207, out bloomCfg))
                {
                    bloomIntensity = bloomCfg.parameters;
                }
                if (cfgList.TryGetValue(8208, out bloomCfg))
                {
                    bloomScatter = bloomCfg.parameters;
                }

                D2DragCamera.inst.SetVolumeBloomFloat(bloomThreshold, bloomIntensity, bloomScatter);
            }

        }

    }

}
