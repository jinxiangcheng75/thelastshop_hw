using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TalkConditionType
{
    none,
    shelfIsNull = 1,//货架空
    optGood = 2, //选中商品
    notOptGood = 3, //没有选中
    queuingNumIsFull = 4, //排队人数达到上限
    lookSomething = 5, //观赏
    hitInToOther = 6, //碰到人
    beforeIndoor = 7,//进门前
    touchTigger = 8, //点击触发
    shopkeeper_ramble = 9, //店主-闲逛
    shopkeeper_customerCome = 10, //店主-来顾客
    cantMoveToInDoor = 11, //无法进门
    lookPet = 12, //观赏宠物
    shopkeeper_shelfIsNull = 13,//店主货架空
    shopkeeper_rambleOver = 14, //店主闲逛完说话
    shopper_cantMoveToTargetShelf = 15,//到达不了目标货架
    shopper_cantMoveToCounter = 16,//到达不了柜台（柜台前位置站满了）
}

public class AITalkConfig
{
    public int id;
    public int hero_type;
    public int level_min;
    public int level_max;
    public int conditions;  //条件
    public string talk;
    public int wight;
}

public class AITalkConfigManager : TSingletonHotfix<AITalkConfigManager>, IConfigManager
{
    public Dictionary<int, List<AITalkConfig>> cfgList = new Dictionary<int, List<AITalkConfig>>();
    List<AITalkConfig> scArray;
    public const string CONFIG_NAME = "aiTalk";

    public void InitCSVConfig()
    {
        scArray = CSVParser.GetConfigsFromCache<AITalkConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            List<AITalkConfig> _talklist;
            if (cfgList.TryGetValue(item.hero_type, out _talklist))
            {
                _talklist.Add(item);
            }
            else
            {
                _talklist = new List<AITalkConfig>();
                _talklist.Add(item);
            }
            cfgList[item.hero_type] = _talklist;
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();
        InitCSVConfig();
    }


    //lua侧重载 不支持静态转换enum-int
    public string GetRandomTalk(int hero_id, int level, TalkConditionType eventtype) 
    {
        return GetRandomTalk(hero_id, level,(int)eventtype);
    }

    public string GetRandomTalk(int hero_id, int level, int eventtype)
    {
        string str;
        int type = 5;

        var cfg = HeroProfessionConfigManager.inst.GetConfig(hero_id);
        if (cfg != null)
        {
            type = cfg.type;
        }

        if (cfgList.ContainsKey(type))
        {
            var talklist = cfgList[type];
            List<AITalkConfig> targetList = talklist.FindAll(item => item.conditions == eventtype && (level >= item.level_min) && (level <= item.level_max));
            str = GetCfgByWight(targetList);
        }
        else
        {
            str = "...";
        }
        return str;
    }

    //lua侧重载 enum与int无法静态转换
    public string GetSpkeeperRandomTalk(TalkConditionType conditionType) 
    {
        return GetSpkeeperRandomTalk((int)conditionType);
    }

    public string GetSpkeeperRandomTalk(int conditionType)
    {
        string str;

        if (cfgList.ContainsKey(4))
        {
            var talklist = cfgList[4];
            List<AITalkConfig> targetList = talklist.FindAll(item => item.conditions == conditionType);
            str = GetCfgByWight(targetList);
        }
        else
        {
            str = "...";
        }

        return str;
    }

    public string GetRanomTalkMsg(int roleType, int eventtype) 
    {
        string str;
        int type = roleType;

        if (cfgList.ContainsKey(type))
        {
            var talklist = cfgList[type];
            List<AITalkConfig> targetList = talklist.FindAll(item => item.conditions == eventtype);
            str = GetCfgByWight(targetList);
        }
        else
        {
            str = "...";
        }
        return str;
    }

    private string GetCfgByWight(List<AITalkConfig> items)
    {
        int allnum = 0;
        foreach (AITalkConfig n in items)
        {
            allnum += n.wight;
        }
        int r = UnityEngine.Random.Range(0, allnum);

        allnum = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].wight <= 0) continue;
            if (r >= allnum && r < allnum + items[i].wight)
            {
                return items[i].talk;
            }
            allnum += items[i].wight;
        }
        return null;
    }
}
