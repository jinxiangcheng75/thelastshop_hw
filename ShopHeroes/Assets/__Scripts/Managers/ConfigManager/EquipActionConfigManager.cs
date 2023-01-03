using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipAction
{
    public int id;
    public string name;
    public int sub_type;
    public string act_combat_standby;   // 战斗待机
    public string act_combat_standby_show; //战斗特殊待机
    public string act_run;
    public int if_close_combat;
    public string act_attack;           //普通攻击动作
    public int common_attack_time;
    public string act_hit;              //受击动作
    public string act_victory;          //胜利动作
    public string act_failed;           //死亡动作
    public string act_failed_standby;   //死亡待机
    public string act_resurrection;     //复活动作
    public string act_skill;            //释放技能动作
    public int skill_attack_time;

}
public class EquipActionConfigManager : TSingletonHotfix<EquipActionConfigManager>, IConfigManager
{
    public Dictionary<int, EquipAction> cfgDic = new Dictionary<int, EquipAction>();
    public const string CONFIG_NAME = "equip_action";

    public void InitCSVConfig()
    {
        List<EquipAction> scArray = CSVParser.GetConfigsFromCache<EquipAction>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in scArray)
        {
            if (sc.id < 0) continue;
            cfgDic.Add(sc.id, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgDic.Clear();
        InitCSVConfig();
    }
    public EquipAction GetCfg(int key)
    {
        if (cfgDic.ContainsKey(key))
        {
            return cfgDic[key];
        }
        return null;
    }
}
