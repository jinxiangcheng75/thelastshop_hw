using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMoveConfig
{
    public int id;
    public int type;
    public int level;
    public float zoom1;//镜头上限
    public float zoom2;//镜头拉过去的高度

    public float x_revise;
    public float x_revise_landscape;
    public float y_revise;
    public float y_revise_landscape;

    public float X_revise
    {
        get 
        {
            if (FGUI.inst != null)
            {
                return FGUI.inst.isLandscape ? x_revise_landscape : x_revise;
            }

            return 0;
        }
    }

    public float Y_revise 
    {
        get
        {
            if (FGUI.inst != null)
            {
                return FGUI.inst.isLandscape ? y_revise_landscape : y_revise;
            }

            return 0;
        }
    }


}

public class CameraMoveConfigManager : TSingletonHotfix<CameraMoveConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "camera_height_change";
    public Dictionary<int, CameraMoveConfig> dic = new Dictionary<int, CameraMoveConfig>();
    CameraMoveConfig furnitureUpCfg = null;
    CameraMoveConfig shopperDealCfg = null;
    CameraMoveConfig citySceneCfg = null;

    public void InitCSVConfig()
    {
        List<CameraMoveConfig> arr = CSVParser.GetConfigsFromCache<CameraMoveConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in arr)
        {
            if (sc.id <= 0) continue;
            dic.Add(sc.id, sc);

            if (sc.type == (int)kCameraMoveType.furnitureUp)
            {
                furnitureUpCfg = sc;
            }

            if (sc.type == (int)kCameraMoveType.shopperDeal)
            {
                shopperDealCfg = sc;
            }

            if (sc.type == (int)kCameraMoveType.citySecene)
            {
                citySceneCfg = sc;
            }

        }

    }

    public void ReLoadCSVConfig()
    {
        dic.Clear();
        InitCSVConfig();
    }

    public CameraMoveConfig GetConfig(int id)
    {
        if (dic.ContainsKey(id))
        {
            return dic[id];
        }
        return null;
    }

    public CameraMoveConfig GetConfg(kCameraMoveType moveType, int level = 0)
    {
        CameraMoveConfig result = null;


        if (moveType == kCameraMoveType.shopExtend)
        {
            foreach (var item in dic.Values.ToArray())
            {
                if (item.type == (int)moveType && item.level == level)
                {
                    result = item;
                    break;
                }
            }
        }
        else
        {
            if (moveType == kCameraMoveType.furnitureUp)
            {
                if (furnitureUpCfg != null)
                {
                    result = furnitureUpCfg;
                }
            }
            else if (moveType == kCameraMoveType.shopperDeal)
            {
                if (shopperDealCfg != null)
                {
                    result = shopperDealCfg;
                }
            }
            else if (moveType == kCameraMoveType.citySecene)
            {
                if (citySceneCfg != null)
                {
                    result = citySceneCfg;
                }
            }

        }

        if (result == null)
        {
            Logger.error("未找到对应相机高度配置  moveType : " + moveType.ToString() + "    level : " + level);
            result = new CameraMoveConfig();
            result.zoom1 = WorldParConfigManager.inst.GetConfig(8101).parameters;
            result.zoom2 = WorldParConfigManager.inst.GetConfig(8102).parameters;
        }

        return result;
    }

}