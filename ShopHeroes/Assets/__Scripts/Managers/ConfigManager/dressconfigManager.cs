using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class dressconfig
{
    public int id;
    public string name;
    public string icon;
    public string atlas;
    public int is_show;//是否在UI上显示 1显示 2不显示
    public int player_role; //角色类型 1 shopkeeper  2 hero
    public string slot_man;
    public string slot_woman;
    public int type_1;
    public int type_2;
    public int gender;
    public string val;
    public int get_type;
    public int price_money;
    public int price_diamond;
    public int sale_id;
    public int guide;
    public int consume;
    public int group;
}

public class dressconfigManager : TSingletonHotfix<dressconfigManager>, IConfigManager
{
    public Dictionary<int, dressconfig> cfgList = new Dictionary<int, dressconfig>();
    public const string CONFIG_NAME = "dress";

    public void InitCSVConfig()
    {
        List<dressconfig> scArray = CSVParser.GetConfigsFromCache<dressconfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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

    public dressconfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public dressconfig GetConfig(int key)
    {
        if (cfgList.ContainsKey(key))
        {
            return cfgList[key];
        }

        return null;
    }


    public void GetRandomDressList(out EGender gender, out List<int> facadeList, out List<int> equipList)
    {

        gender = UnityEngine.Random.Range(0, 2) == 0 ? EGender.Male : EGender.Female;
        facadeList = new List<int>();
        int genderInt = (int)gender;


        var facadeCfgs = cfgList.Values.ToList().FindAll(t => t.type_1 == 1);

        foreach (FacadeType item in Enum.GetValues(typeof(FacadeType)))
        {
            if (item != FacadeType.max)
            {
                var cfgs = facadeCfgs.FindAll(t => t.type_2 == (int)item && t.gender == genderInt);
                if (cfgs.Count > 0)
                {
                    var cfg = cfgs[UnityEngine.Random.Range(0, cfgs.Count)];
                    facadeList.Add(cfg.id);
                }
            }
        }


        int EquipMaxlevel = MarketEquipLvManager.inst.GetCurMarketLevel();


        var heroProfessionCfg = HeroProfessionConfigManager.inst.GetRandomCfg();

        var dic = HeroProfessionConfigManager.inst.GetEquipDic(heroProfessionCfg.id);
        var list = EquipConfigManager.inst.GetAllEquipConfigs();


        equipList = new List<int>();

        foreach (int key in dic.Keys)
        {
            var subList = list.FindAll(t => t.sub_type != 23 && t.level <= EquipMaxlevel && dic[key].Contains(t.sub_type));
            if (subList.Count > 0)
            {
                var equipCfg = subList[UnityEngine.Random.Range(0, subList.Count)];
                equipList.Add(equipCfg.id + 1);
            }
        }

    }

}
