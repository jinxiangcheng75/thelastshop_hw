using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum kIndoorRoleActionType
{
    none = 0,
    normal_standby = 1,
    special_standby = 2,
    normal_walking = 3,
    rejected = 4,
    successful_transaction = 5,
    ornamental_furniture = 6,
    ornamental_shelves = 7,
    ornamental_pets = 8,
    haggle = 9,

}

public class IndoorRoleActionConfig
{
    public int id;
    public int gender;
    public string normal_standby;
    public string special_standby;
    public string normal_walking;
    public string rejected;
    public string successful_transaction;
    public string ornamental_furniture;
    public string ornamental_shelves;
    public string ornamental_pets;
    public string haggle;

    public bool HasThisAction(int actionType)
    {

        switch ((kIndoorRoleActionType)actionType)
        {
            case kIndoorRoleActionType.none: return false;
            case kIndoorRoleActionType.normal_standby: return !string.IsNullOrEmpty(normal_standby);
            case kIndoorRoleActionType.special_standby: return !string.IsNullOrEmpty(special_standby);
            case kIndoorRoleActionType.normal_walking: return !string.IsNullOrEmpty(normal_walking);
            case kIndoorRoleActionType.rejected: return !string.IsNullOrEmpty(rejected);
            case kIndoorRoleActionType.successful_transaction: return !string.IsNullOrEmpty(successful_transaction);
            case kIndoorRoleActionType.ornamental_furniture: return !string.IsNullOrEmpty(ornamental_furniture);
            case kIndoorRoleActionType.ornamental_shelves: return !string.IsNullOrEmpty(ornamental_shelves);
            case kIndoorRoleActionType.ornamental_pets: return !string.IsNullOrEmpty(ornamental_pets);
            case kIndoorRoleActionType.haggle: return !string.IsNullOrEmpty(haggle);
            default: return false;
        }
    }

    public string GetAction(int actionType)
    {

        string str = "";

        switch ((kIndoorRoleActionType)actionType)
        {
            case kIndoorRoleActionType.none: break;
            case kIndoorRoleActionType.normal_standby: str = normal_standby; break;
            case kIndoorRoleActionType.special_standby: str = special_standby; break;
            case kIndoorRoleActionType.normal_walking: str = normal_walking; break;
            case kIndoorRoleActionType.rejected: str = rejected; break;
            case kIndoorRoleActionType.successful_transaction: str = successful_transaction; break;
            case kIndoorRoleActionType.ornamental_furniture: str = ornamental_furniture; break;
            case kIndoorRoleActionType.ornamental_shelves: str = ornamental_shelves; break;
            case kIndoorRoleActionType.ornamental_pets: str = ornamental_pets; break;
            case kIndoorRoleActionType.haggle: str = haggle; break;
            default: break;
        }

        return str;
    }

}

public class IndoorRoleActionConfigManager : TSingletonHotfix<IndoorRoleActionConfigManager>, IConfigManager
{

    public Dictionary<int, List<IndoorRoleActionConfig>> actoinDic = new Dictionary<int, List<IndoorRoleActionConfig>>();
    public const string CONFIG_NAME = "action_shop";

    public void InitCSVConfig()
    {
        List<IndoorRoleActionConfig> scArray = CSVParser.GetConfigsFromCache<IndoorRoleActionConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var item in scArray)
        {
            if (item.id <= 0) continue;
            List<IndoorRoleActionConfig> _actionlist;
            if (actoinDic.TryGetValue(item.gender, out _actionlist))
            {
                _actionlist.Add(item);
            }
            else
            {
                _actionlist = new List<IndoorRoleActionConfig>();
                _actionlist.Add(item);
            }
            actoinDic[item.gender] = _actionlist;
        }
    }
    public void ReLoadCSVConfig()
    {
        actoinDic.Clear();

        InitCSVConfig();
    }

    //lua侧枚举不支持静默转换int/string lua侧重载使用
    public string GetRandomAction(EGender gender, kIndoorRoleActionType actionType) 
    {
        return GetRandomAction((int)gender, (int)actionType);
    }


    public string GetRandomAction(int gender, int actionType)
    {
        string str = "";

        if (actoinDic.ContainsKey(gender))
        {
            var cfgs = actoinDic[gender];
            var actions = cfgs.FindAll(t => t.HasThisAction(actionType));

            if (actions.Count > 0)
            {
                str = actions.GetRandomElement().GetAction(actionType);
            }
            else
            {
                str = "GetActionError...";
            }
        }
        else
        {
            str = "GetActionError...";
        }

        return str;

    }

}
