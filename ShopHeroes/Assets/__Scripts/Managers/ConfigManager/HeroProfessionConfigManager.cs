using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeroProfessionNeedMatData
{
    public int itemId;
    public int needItemCount;
}

public class HeroProfessionConfigData
{
    public int id;
    public string name;
    public int position;
    public int initSex;
    public int[] dress_b;
    public int[] dress_g;
    public string ocp_story;
    public string atlas;
    public string ocp_icon;
    public int type;
    public int hero_grade;
    public int pre_profession;
    public int aptitude_need;
    public int level_need;
    public int cost_item1_id;
    public int cost_item1_num;
    public int cost_item2_id;
    public int cost_item2_num;
    public int cost_item3_id;
    public int cost_item3_num;
    public int cost_item4_id;
    public int cost_item4_num;
    public int cost_item5_id;
    public int cost_item5_num;
    public int[] equip1;
    public int[] equip2;
    public int[] equip3;
    public int[] equip4;
    public int[] equip5;
    public int[] equip6;
    public int id_skill1;
    public int id_skill2;
    public int id_skill3;
    public float grew_hp;
    public float grew_atk;
    public float grew_def;
    public int gene_num;

    //是否包含此装备小类型
    public bool ContainsEquipSubType(/*EquipSubType*/int equipSubType)
    {
        bool result = false;

        result = equip1.Contains(equipSubType);
        if (result) return result;
        result = equip2.Contains(equipSubType);
        if (result) return result;
        result = equip3.Contains(equipSubType);
        if (result) return result;
        result = equip4.Contains(equipSubType);
        if (result) return result;
        result = equip5.Contains(equipSubType);
        if (result) return result;
        if (equip6 != null && equip6.Length > 0)
        {
            result = equip6.Contains(equipSubType);
            if (result) return result;
        }

        return result;
    }

    //获取该英雄转职or进阶所需的所有材料
    public List<HeroProfessionNeedMatData> GetHeroProfessionNeedMatDatas()
    {
        List<HeroProfessionNeedMatData> needDatas = new List<HeroProfessionNeedMatData>();

        if (cost_item1_id > 0 && cost_item1_num > 0)
        {
            needDatas.Add(new HeroProfessionNeedMatData() { itemId = cost_item1_id, needItemCount = cost_item1_num });
        }

        if (cost_item2_id > 0 && cost_item2_num > 0)
        {
            needDatas.Add(new HeroProfessionNeedMatData() { itemId = cost_item2_id, needItemCount = cost_item2_num });
        }

        if (cost_item3_id > 0 && cost_item3_num > 0)
        {
            needDatas.Add(new HeroProfessionNeedMatData() { itemId = cost_item3_id, needItemCount = cost_item3_num });
        }

        if (cost_item4_id > 0 && cost_item4_num > 0)
        {
            needDatas.Add(new HeroProfessionNeedMatData() { itemId = cost_item4_id, needItemCount = cost_item4_num });
        }

        if (cost_item5_id > 0 && cost_item5_num > 0)
        {
            needDatas.Add(new HeroProfessionNeedMatData() { itemId = cost_item5_id, needItemCount = cost_item5_num });
        }

        return needDatas;
    }

}

public class HeroProfessionConfigManager : TSingletonHotfix<HeroProfessionConfigManager>, IConfigManager
{
    public Dictionary<int, HeroProfessionConfigData> cfgList = new Dictionary<int, HeroProfessionConfigData>();
    public const string CONFIG_NAME = "hero_profession";

    public void InitCSVConfig()
    {
        List<HeroProfessionConfigData> scArray = CSVParser.GetConfigsFromCache<HeroProfessionConfigData>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public HeroProfessionConfigData[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public HeroProfessionConfigData GetRandomCfg()
    {
        return GetAllConfig()[Random.Range(0, GetAllConfig().Length)];
    }

    public HeroProfessionConfigData GetConfig(int id)
    {
        if (cfgList.ContainsKey(id))
        {
            return cfgList[id];
        }

        return null;
    }

    public List<HeroProfessionConfigData> GetTransferData(int profession, int intelligence)
    {
        List<HeroProfessionConfigData> cfgs = new List<HeroProfessionConfigData>();

        cfgs = cfgList.Values.ToList().FindAll(t => t.pre_profession == profession && intelligence >= t.aptitude_need);
        return cfgs;
    }

    public Dictionary<int, List<int>> GetEquipDic(int heroId)
    {
        HeroProfessionConfigData temp = GetConfig(heroId);
        Dictionary<int, List<int>> tempDic = new Dictionary<int, List<int>>();
        tempDic.Add(0, GetEquipList(temp.equip1));
        tempDic.Add(1, GetEquipList(temp.equip2));
        tempDic.Add(2, GetEquipList(temp.equip3));
        tempDic.Add(3, GetEquipList(temp.equip4));
        tempDic.Add(4, GetEquipList(temp.equip5));
        if (temp.equip6 != null && temp.equip6.Length > 0)
            tempDic.Add(5, GetEquipList(temp.equip6));
        return tempDic;
    }

    public List<int> GetTargetFieldEquipList(int heroId, int field)
    {
        HeroProfessionConfigData temp = GetConfig(heroId);
        List<int> tempList = new List<int>();
        switch (field)
        {
            case 1:
                tempList = GetEquipList(temp.equip1);
                break;
            case 2:
                tempList = GetEquipList(temp.equip2);
                break;
            case 3:
                tempList = GetEquipList(temp.equip3);
                break;
            case 4:
                tempList = GetEquipList(temp.equip4);
                break;
            case 5:
                tempList = GetEquipList(temp.equip5);
                break;
            case 6:
                tempList = GetEquipList(temp.equip6);
                break;
        }
        return tempList;
    }

    public List<int> GetAllFieldEquipId(int heroId)
    {
        List<int> ids = new List<int>();
        HeroProfessionConfigData tempData = GetConfig(heroId);
        ids.AddRange(tempData.equip1);
        ids.AddRange(tempData.equip2);
        ids.AddRange(tempData.equip3);
        ids.AddRange(tempData.equip4);
        ids.AddRange(tempData.equip5);
        if (tempData.equip6 != null && tempData.equip6.Length > 0)
            ids.AddRange(tempData.equip6);

        return ids;
    }

    private List<int> GetEquipList(int[] equipArr)
    {
        List<int> tempList = new List<int>();
        tempList.AddRange(equipArr);
        return tempList;
    }

    //所有相关英雄职业
    public List<HeroProfessionConfigData> GetCanWearHeroProfessionsByEquipSubType(int subType)
    {

        List<HeroProfessionConfigData> result = new List<HeroProfessionConfigData>();

        foreach (var item in cfgList.Values)
        {
            //筛选可穿戴该装备小类型的英雄职业
            if (item.ContainsEquipSubType(subType))
            {
                result.Add(item);
            }
        }

        return result;
    }

}
