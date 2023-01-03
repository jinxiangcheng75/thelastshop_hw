using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum EModel_Type //1男店主 2女店主 3男英雄（不配装备） 4女英雄（不配装备）5男性人形怪 6女性人形怪 7僵尸 8巨型怪
{
    none,
    shopkeeper_man,
    shopkeeper_woman,
    hero_man,
    hero_woman,
    peopleShape_monster_man,
    peopleShape_monster_woman,
    monster_zombie,
    monster_huge,
    pets,
}

//public class RoleDress
//{
//    public int model_color; //裸模颜色
//    public int face;//五官
//    public int face_color;//五官颜色
//    public int hair;//发型
//    public int hair_color;//头发颜色
//    public int eyes_color;//眼睛颜色
//    public int weapon;//手持
//    public int upper;//上衣
//    public int lower;//下衣
//    public int shoes;//鞋子
//    public int head_hat;//帽子
//}

public class modelconfig //模型配置
{
    public int id;
    public int type;//1男店主 2女店主 3男英雄（不配装备） 4女英雄（不配装备）5男性人形怪 6女性人形怪 7僵尸 8巨型怪 9宠物
    public string name;
    public int model;//0 使用换装系统   1 不使用换装系统
    public int model_color;
    public string model_path;
    public int face;
    public int face_color;
    public int hair;
    public int hair_color;
    public int eyes_color;
    public int weapon;
    public int upper;
    public int lower;
    public int shoes;
    public int head_hat;
    public int shield;
    public int monster_action;

    //仅外观
    public List<int> ToFacadeDressIds()
    {
        List<int> dressIds = new List<int>();

        if (model == 1) return dressIds;

        dressIds.Add(model_color);
        dressIds.Add(hair);
        dressIds.Add(hair_color);
        dressIds.Add(face);
        dressIds.Add(face_color);
        dressIds.Add(eyes_color);

        dressIds.RemoveAll(t => t == 0);//去除空白的

        return dressIds;
    }

    //仅时装or装备
    public List<int> ToFashionDressIds()
    {
        List<int> dressIds = new List<int>();

        if (model == 1) return dressIds;

        dressIds.Add(weapon);
        dressIds.Add(upper);
        dressIds.Add(lower);
        dressIds.Add(shoes);
        dressIds.Add(head_hat);
        dressIds.Add(shield);

        dressIds.RemoveAll(t => t == 0);//去除空白的

        return dressIds;
    }


    //全
    public List<int> ToDressIds()
    {
        List<int> dressIds = new List<int>();

        if (model == 1) return dressIds;

        dressIds.AddRange(ToFacadeDressIds());
        dressIds.AddRange(ToFashionDressIds());

        return dressIds;
    }

    public RoleDress ToRoleDress()
    {
        RoleDress roleDress = new RoleDress();

        roleDress.modelColor = model_color;
        roleDress.face = face;
        roleDress.faceColor = face_color;
        roleDress.hair = hair;
        roleDress.hairColor = hair_color;
        roleDress.eyesColor = eyes_color;

        roleDress.weapon = weapon;
        roleDress.upper = upper;
        roleDress.lower = lower;
        roleDress.shoes = shoes;
        roleDress.headHat = head_hat;

        return roleDress;
    }

    public EGender GetGender()
    {
        EGender result = EGender.Male;
        switch ((EModel_Type)type)
        {
            case EModel_Type.shopkeeper_man:
            case EModel_Type.hero_man:
            case EModel_Type.peopleShape_monster_man:
                result = EGender.Male;
                break;

            case EModel_Type.shopkeeper_woman:
            case EModel_Type.hero_woman:
            case EModel_Type.peopleShape_monster_woman:
                result = EGender.Female;
                break;

            case EModel_Type.monster_zombie:
            case EModel_Type.monster_huge:
            case EModel_Type.pets:

                break;
        }

        return result;
    }

}

public class CharacterModelConfigManager : TSingletonHotfix<CharacterModelConfigManager>, IConfigManager
{
    public Dictionary<int, modelconfig> cfgList = new Dictionary<int, modelconfig>();
    public const string CONFIG_NAME = "outward_database";

    public void InitCSVConfig()
    {
        List<modelconfig> scArray = CSVParser.GetConfigsFromCache<modelconfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);

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
    public modelconfig[] GetAllConfig()
    {
        return cfgList.Values.ToArray();
    }

    public modelconfig GetConfig(int key)
    {
        if (cfgList.ContainsKey(key))
        {
            return cfgList[key];
        }

        return null;
    }

}
