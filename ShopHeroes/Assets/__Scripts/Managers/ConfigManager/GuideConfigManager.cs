using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideConfigData
{
    public int id;
    public int sort;
    public int index;
    public int task_id;
    public string desc;
    public int guide_type;
    public int mask_color;
    public int trigger_type;
    public string trigger_param;
    public int if_service_help;
    public int[] service_value;
    public int end_type;
    public int end_param;
    public string conditon_param_1;
    public string conditon_param_2;
    public string conditon_param_3;
    public string conditon_param_4;
    public string conditon_param_5;
    public string guide_btn_dev;
    public string conditon_param_6;
    public string btn_view;
    public string btn_name;
    public int character_id;
    public int arrow_direction;
    public int wait_id;
    public int next_id;
    public int last_id;
}

public class GuideConfigManager : TSingletonHotfix<GuideConfigManager>, IConfigManager
{
    public const string CONFIG_NAME = "guide";
    public Dictionary<int, GuideConfigData> guideDic = new Dictionary<int, GuideConfigData>();
    public void InitCSVConfig()
    {
        List<GuideConfigData> guideArr = CSVParser.GetConfigsFromCache<GuideConfigData>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        for (int i = 0; i < guideArr.Count; i++)
        {
            int index = i;
            guideDic.Add(guideArr[index].id, guideArr[index]);
        }
    }
    public void ReLoadCSVConfig()
    {
        guideDic.Clear();
        InitCSVConfig();
    }
    public GuideConfigData GetConfig(int id)
    {
        if (guideDic.ContainsKey(id))
        {
            return guideDic[id];
        }
        return null;
    }

    public GuideConfigData GetConfigByGroupAndIndex(int group, int index)
    {
        foreach (var item in guideDic.Values)
        {
            if (item.sort == group && item.index == index)
                return item;
        }

        Logger.error("没有找到组为 = " + group + "  索引为 = " + index + "的数据");
        return null;
    }

    public GuideConfigData GetNextConfiData(int group, int index)
    {
        GuideConfigData tempCfg = GetConfigByGroupAndIndex(group, index);

        if (tempCfg != null)
        {
            var nextCfg = GetConfig(tempCfg.next_id);
            if (nextCfg != null)
            {
                return nextCfg;
            }
            else
            {
                Logger.error("group为" + group + " index为" + index + "的引导项没有下一步引导");
                return null;
            }
        }

        return null;
    }

    public GuideConfigData GetLastConfigData(int group,int index)
    {
        GuideConfigData tempCfg = GetConfigByGroupAndIndex(group, index);

        if (tempCfg != null)
        {
            var lastCfg = GetConfig(tempCfg.last_id);
            if (lastCfg != null)
            {
                return lastCfg;
            }
            else
            {
                Logger.error("group为" + group + " index为" + index + "的引导项没有上一步引导");
                return null;
            }
        }

        return null;
    }
}
