using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class HeroExchangeInfoData
{
    public int id;
    public int unlockLevel;
    public long unlockCost;
    public EHeroExchangeState state;
    public int heroId;
    public HeroProfessionConfigData cfg;
    public int intelligence;
    public int talentId;
    public int gender;
    public int rarity;
    public RoleDress roleDress;

    public void setData(ExchangeHeroInfo data)
    {
        id = data.id;
        unlockLevel = data.unlockLevel;
        unlockCost = data.unlockCost;
        state = (EHeroExchangeState)data.exchangeState;
        heroId = data.heroInfo.heroId;
        cfg = HeroProfessionConfigManager.inst.GetConfig(heroId);
        intelligence = data.heroInfo.aptitude;
        talentId = data.heroInfo.talentId;
        gender = data.heroInfo.gender;
        rarity = RoleDataProxy.inst.ReturnRarityByAptitude(intelligence);
        roleDress = data.heroInfo.roleDress;
    }
}

public class HeroChangeEquipData
{
    public int heroUid;
    public int equipField;
    public string equipUid;

    public HeroChangeEquipData(int _heroUid, int _equipField, string _equipUid)
    {
        heroUid = _heroUid;
        equipField = _equipField;
        equipUid = _equipUid;
    }

}

public class RoleHeroData : IComparable<RoleHeroData>
{
    public int id;
    public int uid;
    public int talentId;
    public int intelligence;//资质
    public int currentState; // 0 - idle 1 - resting 2 - fighting
    public int level;
    public int exp;
    public int startTime;
    public int endTime;
    public int remainTime;
    public int creatTime;
    public bool isSelect;
    public string nickName = "";
    public int fightingNum;
    public int transferNumLimit;//英雄转职上限

    public int remainHp;

    public int hpAdd
    {
        get;
        private set;
    }
    public int atkAdd
    {
        get;
        private set;
    }
    public int defAdd
    {
        get;
        private set;
    }
    public HeroEquip equip1;
    public HeroEquip equip2;
    public HeroEquip equip3;
    public HeroEquip equip4;
    public HeroEquip equip5;
    public HeroEquip equip6;
    public HeroSkillShowConfig skill1;
    public HeroSkillShowConfig skill2;
    public HeroSkillShowConfig skill3;
    public HeroProfessionConfigData config;
    public HeroPropertyData attributeConfig = new HeroPropertyData();
    public HeroTalentDataBase talentConfig;

    int roleEndTime = 0;

    //换装外观相关
    public int gender;
    public RoleDress roleDress;

    public int hasRedPoint = 0;

    public void setData(HeroInfo heroData)
    {
        this.id = heroData.heroId;
        this.uid = heroData.heroUid;
        this.talentId = heroData.talentId;
        this.intelligence = heroData.aptitude;
        this.currentState = heroData.currentState;
        this.level = heroData.level;
        this.exp = heroData.exp;
        this.startTime = heroData.stateStartTime;
        this.endTime = heroData.stateEndTime;
        this.remainTime = heroData.stateRemainTime;
        roleEndTime = heroData.stateRemainTime + (int)GameTimer.inst.serverNow;
        this.nickName = heroData.nickName;
        this.creatTime = heroData.createTime;
        this.equip1 = heroData.equip1;
        this.equip2 = heroData.equip2;
        this.equip3 = heroData.equip3;
        this.equip4 = heroData.equip4;
        this.equip5 = heroData.equip5;
        this.equip6 = heroData.equip6;
        this.remainHp = heroData.remainHp;

        hpAdd = heroData.hpAdd;
        atkAdd = heroData.atkAdd;
        defAdd = heroData.defAdd;


        //换装外观相关
        gender = heroData.gender; //enum EGender
        roleDress = heroData.roleDress;

        config = HeroProfessionConfigManager.inst.GetConfig(id);
        var data = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(level, id);
        talentConfig = HeroTalentDBConfigManager.inst.GetConfig(talentId);
        attributeConfig.setData(data, intelligence, GetAllWearEquipId(), talentConfig);
        attributeConfig.SetGenePropertyNum(GetGeneProperty((int)kRolePropertyType.hp), GetGeneProperty((int)kRolePropertyType.atk), GetGeneProperty((int)kRolePropertyType.def), intelligence);
        skill1 = HeroSkillShowConfigManager.inst.GetConfig(config.id_skill1);
        if (config.id_skill2 != 0)
            skill2 = HeroSkillShowConfigManager.inst.GetConfig(config.id_skill2);
        else
            skill2 = null;
        if (config.id_skill3 != 0)
            skill3 = HeroSkillShowConfigManager.inst.GetConfig(config.id_skill3);
        else
            skill3 = null;

        calculateTime();
        GetAllPropertyAddResult();
        calTransferNumLimit();
    }

    public int GetGeneItemNum(int type)
    {
        int val = 0;

        switch ((kRolePropertyType)type)
        {
            case kRolePropertyType.hp: val = hpAdd; break;
            case kRolePropertyType.def: val = defAdd; break;
            case kRolePropertyType.atk: val = atkAdd; break;
        }

        return val;
    }

    public int GetGeneProperty(int type)
    {
        int val = 0;

        int effect = 0;
        var item = ItemBagProxy.inst.GetItemsByType(ItemType.HeroPropertyUp, type);
        if (item != null)
        {
            effect = item.itemConfig.effect_val;
        }

        switch ((kRolePropertyType)type)
        {
            case kRolePropertyType.hp: val = hpAdd * effect; break;
            case kRolePropertyType.def: val = defAdd * effect; break;
            case kRolePropertyType.atk: val = atkAdd * effect; break;
        }

        return val;
    }

    public int GetProperty(int type, bool hasGene) //type  kRolePropertyType   hasGene 是否需要包含基因药水后的结果
    {
        int val = 0;

        switch ((kRolePropertyType)type)
        {
            case kRolePropertyType.hp: val = attributeConfig.hp_basic; break;
            case kRolePropertyType.def: val = attributeConfig.def_basic; break;
            case kRolePropertyType.atk: val = attributeConfig.atk_basic; break;
            default:
                //error
                break;
        }

        return val + (hasGene ? GetGeneProperty(type) : 0);
    }

    int timerId;
    private void calculateTime()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (currentState != 0)
        {
            if (remainTime > 0)
            {
                timerId = GameTimer.inst.AddTimer(1, () =>
                {
                    remainTime = roleEndTime - (int)GameTimer.inst.serverNow;
                    remainTime = Mathf.Clamp(remainTime, 0, remainTime);
                    if (remainTime <= 0)
                    {
                        EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HEROREFRESH, uid);
                        GameTimer.inst.RemoveTimer(timerId);
                        timerId = 0;
                    }
                });
            }
        }
        else
        {
            if (timerId > 0)
            {
                GameTimer.inst.RemoveTimer(timerId);
                timerId = 0;
            }
        }
    }

    public void clearTime()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    public void GetAllPropertyAddResult()
    {
        int sum = 0;
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(200).parameters * (attributeConfig.hp_basic + attributeConfig.hp_gene));
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(201).parameters * (attributeConfig.atk_basic + attributeConfig.atk_gene));
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(202).parameters * (attributeConfig.def_basic + attributeConfig.def_gene));
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(203).parameters * attributeConfig.spd_basic);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(204).parameters * attributeConfig.acc_basic);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(205).parameters * attributeConfig.dodge_basic);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(206).parameters * attributeConfig.cri_basic);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(207).parameters * attributeConfig.tough_basic);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(208).parameters * attributeConfig.piercing_dmg);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(209).parameters * attributeConfig.burn_dmg);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(210).parameters * attributeConfig.ment_dmg);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(211).parameters * attributeConfig.electricity_dmg);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(212).parameters * attributeConfig.radiation_dmg);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(213).parameters * attributeConfig.piercing_res);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(214).parameters * attributeConfig.burn_res);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(215).parameters * attributeConfig.ment_res);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(216).parameters * attributeConfig.electricity_res);
        sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(217).parameters * attributeConfig.radiation_res);
        fightingNum = sum;
    }

    public HeroEquip GetEquipByField(int field)
    {
        switch (field)
        {
            case 1: return equip1;
            case 2: return equip2;
            case 3: return equip3;
            case 4: return equip4;
            case 5: return equip5;
            case 6: return equip6;
        }

        Logger.error("没有这个槽位的装备数据");
        return null;
    }

    public List<int> GetAllWearEquipId()
    {
        List<int> equipIds = new List<int>();
        if (equip1.equipId != 0)
            equipIds.Add(equip1.equipId);
        if (equip2.equipId != 0)
            equipIds.Add(equip2.equipId);
        if (equip3.equipId != 0)
            equipIds.Add(equip3.equipId);
        if (equip4.equipId != 0)
            equipIds.Add(equip4.equipId);
        if (equip5.equipId != 0)
            equipIds.Add(equip5.equipId);
        if (equip6.equipId != 0)
            equipIds.Add(equip6.equipId);

        return equipIds;
    }

    public List<int> GetHeadDressIds()
    {
        List<int> result = new List<int>();

        List<int> equipIdList = new List<int>();//上衣和帽子
        if (equip2.equipId != 0) equipIdList.Add(equip2.equipId);
        if (equip3.equipId != 0) equipIdList.Add(equip3.equipId);

        result.AddRange(SpineUtils.RoleDressToHeadDressIdList(roleDress));
        result.AddRange(CharacterManager.inst.EquipIdsToDressIds(equipIdList));

        return result;
    }

    public void WearEquip(int field, HeroEquip newEquip)
    {
        switch (field)
        {
            case 1: equip1 = newEquip; break;
            case 2: equip2 = newEquip; break;
            case 3: equip3 = newEquip; break;
            case 4: equip4 = newEquip; break;
            case 5: equip5 = newEquip; break;
            case 6: equip6 = newEquip; break;
        }

        var data = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(level, id);
        attributeConfig.setData(data, intelligence, GetAllWearEquipId(), talentConfig);
        GetAllPropertyAddResult();
    }

    public List<HeroSkillShowConfig> GetAllSkillId()
    {
        List<HeroSkillShowConfig> tempList = new List<HeroSkillShowConfig>();
        if (skill1 != null)
            tempList.Add(skill1);
        if (skill2 != null)
            tempList.Add(skill2);
        if (skill3 != null)
            tempList.Add(skill3);

        return tempList;
    }

    private void calTransferNumLimit()
    {
        List<HeroProfessionConfigData> result = HeroProfessionConfigManager.inst.GetTransferData(id, intelligence);

        int curTransferNumLimit = 0;
        while (result.Count > 0)
        {
            curTransferNumLimit++;
            result = HeroProfessionConfigManager.inst.GetTransferData(result[0].id, intelligence);
        }

        transferNumLimit = config.hero_grade + curTransferNumLimit;
    }

    public int CompareTo(RoleHeroData other)
    {
        if (this.currentState.CompareTo(other.currentState) == 0)
        {
            if (this.currentState == 0)
            {
                if (this.fightingNum.CompareTo(other.fightingNum) == 0)
                {
                    if (this.level.CompareTo(other.level) == 0)
                    {
                        if (this.config.hero_grade.CompareTo(other.config.hero_grade) == 0)
                        {
                            return this.config.id.CompareTo(other.config.id);
                        }
                        else
                            return -this.config.hero_grade.CompareTo(other.config.hero_grade);
                    }
                    else
                        return -this.level.CompareTo(other.level);
                }
                else
                    return -this.fightingNum.CompareTo(other.fightingNum);
            }
            else if (this.currentState == 1)
            {
                if (this.remainTime.CompareTo(other.remainTime) == 0)
                {
                    if (this.fightingNum.CompareTo(other.fightingNum) == 0)
                    {
                        if (this.level.CompareTo(other.level) == 0)
                        {
                            if (this.config.hero_grade.CompareTo(other.config.hero_grade) == 0)
                            {
                                return this.config.id.CompareTo(other.config.id);
                            }
                            else
                                return -this.config.hero_grade.CompareTo(other.config.hero_grade);
                        }
                        else
                            return -this.level.CompareTo(other.level);
                    }
                    else
                        return -this.fightingNum.CompareTo(other.fightingNum);
                }
                else
                    return this.remainTime.CompareTo(other.remainTime);
            }
            else
            {
                if (this.remainTime.CompareTo(other.remainTime) == 0)
                    return this.creatTime.CompareTo(other.creatTime);
                else
                    return this.remainTime.CompareTo(other.remainTime);
            }
        }
        else
        {
            return this.currentState.CompareTo(other.currentState);
        }
    }
}

//工匠数据类
public class WorkerExpData
{
    public int id;
    public int changeExp;
}

public class WorkerData : IComparable<WorkerData>
{
    public int id;
    public int level;
    public int exp;
    public int maxExp;
    public EWorkerState state;//0 未解锁 1 可解锁 2 已解锁
    public WorkerConfig config;
    public float addSpeed; //制造速度加成百分比

    //前端临时红点
    public bool isNew;

    public bool canShow
    {
        get
        {
            if (state == EWorkerState.Locked)
            {
                if (config.show_level <= UserDataProxy.inst.playerData.level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    public float lastSpeed
    {
        get
        {
            return WorkerUpConfigManager.inst.GetConfig(level > 1 ? level - 1 : 1).crafting_speed_bonus / 100;
        }
    }

    public float nextSpeed
    {
        get
        {
            return WorkerUpConfigManager.inst.GetConfig(level < 40 ? level + 1 : 40).crafting_speed_bonus / 100;
        }
    }

    public WorkerData(WorkerInfo info)
    {
        SetInfo(info);
    }

    public void SetInfo(WorkerInfo info)
    {
        this.config = WorkerConfigManager.inst.GetConfig(info.workerId);

        if (config == null)
        {
            Logger.error("没找到工匠id所对应的config  workerId :" + info.workerId);
        }

        this.id = info.workerId;
        this.level = info.workerLevel;
        this.exp = info.exp;
        this.maxExp = WorkerUpConfigManager.inst.GetConfig(level < 40 ? level + 1 : 40).artisan_exp;
        this.addSpeed = level > 0 ? WorkerUpConfigManager.inst.GetConfig(level).crafting_speed_bonus / 100 : 0;
        this.state = (EWorkerState)info.workerState;
    }

    public void SetExp(int exp)
    {
        if (this.exp != exp)
        {
            this.exp = exp;
        }
    }

    public void SetLevel(int level)
    {
        if (this.level != level)
        {
            this.level = level;
            this.maxExp = WorkerUpConfigManager.inst.GetConfig(level < 40 ? level + 1 : 40).artisan_exp;
            this.addSpeed = WorkerUpConfigManager.inst.GetConfig(level).crafting_speed_bonus / 100;
        }
    }

    public int CompareTo(WorkerData other)
    {
        if (this.state == EWorkerState.Unlock && other.state == EWorkerState.Unlock)
        {
            return this.id.CompareTo(other.id);
        }
        else
        {
            return other.state.CompareTo(this.state);
        }

        //if (this.state == EWorkerState.CanUnlock && other.state == EWorkerState.CanUnlock)
        //{
        //    return this.id.CompareTo(other.id);
        //}
        //else if (this.state == EWorkerState.CanUnlock && other.state != EWorkerState.CanUnlock)
        //{
        //    return -1;
        //}
        //else if (this.state != EWorkerState.CanUnlock && other.state == EWorkerState.CanUnlock)
        //{
        //    return 1;
        //}


        //if (this.state == EWorkerState.Unlock && other.state == EWorkerState.Unlock)
        //{
        //    return -this.level.CompareTo(other.level);
        //}

        //if (this.state == EWorkerState.Locked && other.state == EWorkerState.Locked)
        //{
        //    return this.id.CompareTo(other.id);
        //}
        //else if (this.state == EWorkerState.Unlock)
        //{
        //    return -1;
        //}
        //else if (other.state == EWorkerState.Unlock)
        //{
        //    return 1;
        //}


        return this.id.CompareTo(other.id);
    }
}

public class HeroPropertyData
{
    public int hp_basic;
    public int atk_basic;
    public int def_basic;
    public int spd_basic;
    public int acc_basic;
    public int dodge_basic;
    public int cri_basic;
    public int tough_basic;
    public int piercing_dmg;
    public int burn_dmg;
    public int ment_dmg;
    public int electricity_dmg;
    public int radiation_dmg;
    public int piercing_res;
    public int burn_res;
    public int ment_res;
    public int electricity_res;
    public int radiation_res;

    public int hp_gene;
    public int atk_gene;
    public int def_gene;

    public void setData(HeroAttributeConfigeData data, int aptitude)
    {
        hp_basic = data.hp_basic;
        atk_basic = data.atk_basic;
        def_basic = data.def_basic;
        spd_basic = data.spd_basic;
        acc_basic = data.acc_basic;
        dodge_basic = data.dodge_basic;
        cri_basic = data.cri_basic;
        tough_basic = data.tough_basic;
        piercing_dmg = data.piercing_dmg;
        burn_dmg = data.burn_dmg;
        ment_dmg = data.ment_dmg;
        electricity_dmg = data.electricity_dmg;
        radiation_dmg = data.radiation_dmg;
        piercing_res = data.piercing_res;
        burn_res = data.burn_res;
        ment_res = data.ment_res;
        electricity_res = data.electricity_res;
        radiation_res = data.radiation_res;

        calculatePropertyNum(data.profession_id, aptitude);
    }

    public void setData(HeroAttributeConfigeData data, int aptitude, List<int> equipIds, HeroTalentDataBase talentData)
    {
        setData(data, aptitude);
        //calculateEquipFighting(equipIds);
        calculateTalent(talentData, equipIds);
    }

    public void SetGenePropertyNum(int hp_geneNum, int atk_geneNum, int def_geneNum, int aptitude)
    {
        hp_gene = hp_geneNum;
        atk_gene = atk_geneNum;
        def_gene = def_geneNum;

        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(aptitude);
        int worldParIndex = StaticConstants.roleWorldParId[rarity - 1];
        float coefficient = WorldParConfigManager.inst.GetConfig(worldParIndex).parameters;
        float n = WorldParConfigManager.inst.GetConfig(StaticConstants.roleWorldParNId[rarity - 1]).parameters;
        float m = WorldParConfigManager.inst.GetConfig(StaticConstants.roleWorldParMId[rarity - 1]).parameters;
        float w = WorldParConfigManager.inst.GetConfig(242).parameters;

        hp_gene = Mathf.CeilToInt(hp_gene * (n + (aptitude - m) * w * coefficient));
        atk_gene = Mathf.CeilToInt(atk_gene * (n + (aptitude - m) * w * coefficient));
        def_gene = Mathf.CeilToInt(def_gene * (n + (aptitude - m) * w * coefficient));
    }

    private void calculateTalent(HeroTalentDataBase talentData, List<int> equipIds)
    {
        float val1 = 0, val2 = 0, val3 = 0;
        if (talentData.entry1_type != 0)
        {
            val1 = switchTalentEntryType(talentData.entry1_type, talentData.entry1_value_type, talentData.entry1_value);
        }
        if (talentData.entry2_type != 0)
        {
            val2 = switchTalentEntryType(talentData.entry2_type, talentData.entry2_value_type, talentData.entry2_value);
        }
        if (talentData.entry3_type != 0)
        {
            val3 = switchTalentEntryType(talentData.entry3_type, talentData.entry3_value_type, talentData.entry3_value);
        }

        calculateEquipByTalent(equipIds, talentData.entry1_type, val1, talentData.entry2_type, val2, talentData.entry3_type, val3);
    }

    private float switchTalentEntryType(int entryType, int entryTypeVal, int val)
    {
        float returnVal = 0;
        if (entryTypeVal == 0)
        {
            returnVal = val;
            calculateTalentPropertyNum(entryType, val);
        }
        else if (entryTypeVal == 1)
        {
            returnVal = val / 100.0f;
            calculateTalentPropertyNum(entryType, val / 100.0f);
        }
        else if (entryTypeVal == 2)
        {
            returnVal = val / 1000.0f;
            calculateTalentPropertyNum(entryType, val / 1000.0f);
        }

        return returnVal;
    }

    private void calculateTalentPropertyNum(int talentType, float talentValue)
    {
        if (talentType == 1)
        {
            atk_basic += Mathf.CeilToInt(atk_basic * talentValue);
        }
        else if (talentType == 2)
        {
            def_basic += Mathf.CeilToInt(def_basic * talentValue);
        }
        else if (talentType == 3)
        {
            hp_basic += Mathf.CeilToInt(hp_basic * talentValue);
        }
        else if (talentType == 4)
        {
            tough_basic += Mathf.CeilToInt(tough_basic * talentValue);
        }
        else if (talentType == 5)
        {
            cri_basic += Mathf.CeilToInt(cri_basic * talentValue);
        }
        else if (talentType == 10)
        {
            acc_basic += Mathf.CeilToInt(acc_basic * talentValue);
        }
        else if (talentType == 12)
        {
            dodge_basic += Mathf.CeilToInt(dodge_basic * talentValue);
        }
    }

    public void calculatePropertyNum(int profession, int aptitude)
    {
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(aptitude);
        int worldParIndex = StaticConstants.roleWorldParId[rarity - 1];
        float coefficient = WorldParConfigManager.inst.GetConfig(worldParIndex).parameters;
        float n = WorldParConfigManager.inst.GetConfig(StaticConstants.roleWorldParNId[rarity - 1]).parameters;
        float m = WorldParConfigManager.inst.GetConfig(StaticConstants.roleWorldParMId[rarity - 1]).parameters;
        float w = WorldParConfigManager.inst.GetConfig(242).parameters;

        hp_basic = Mathf.CeilToInt(hp_basic * (n + (aptitude - m) * w * coefficient));
        atk_basic = Mathf.CeilToInt(atk_basic * (n + (aptitude - m) * w * coefficient));
        def_basic = Mathf.CeilToInt(def_basic * (n + (aptitude - m) * w * coefficient));
        spd_basic = Mathf.CeilToInt(spd_basic * (n + (aptitude - m) * w * coefficient));
        acc_basic = Mathf.CeilToInt(acc_basic * (n + (aptitude - m) * w * coefficient));
        dodge_basic = Mathf.CeilToInt(dodge_basic * (n + (aptitude - m) * w * coefficient));
        cri_basic = Mathf.CeilToInt(cri_basic * (n + (aptitude - m) * w * coefficient));
        tough_basic = Mathf.CeilToInt(tough_basic * (n + (aptitude - m) * w * coefficient));
    }

    private void calculateEquipByTalent(List<int> equipIds, int talentType, float talentVal, int talentType2, float talentVal2, int talentType3, float talentVal3)
    {
        List<float> percents = new List<float>(equipIds.Count);
        for (int i = 0; i < equipIds.Count; i++)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equipIds[i]);
            if (equipCfg == null) continue;
            var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipCfg.equip_id);
            if (equipDrawingCfg == null) continue;
            float curPercent = 0;
            if (talentType >= 26)
            {
                if (talentType <= 48)
                {
                    if (StaticConstants.talentDataBaseEntry[talentType] == equipDrawingCfg.sub_type)
                    {
                        curPercent += talentVal;
                    }
                }
                else
                {
                    if (StaticConstants.talentDataBaseEntry[talentType] == equipDrawingCfg.type)
                    {
                        curPercent += talentVal;
                    }
                }

            }
            if (talentType2 >= 26)
            {
                if (talentType2 <= 48)
                {
                    if (StaticConstants.talentDataBaseEntry[talentType2] == equipDrawingCfg.sub_type)
                    {
                        curPercent += talentVal2;
                    }
                }
                else
                {
                    if (StaticConstants.talentDataBaseEntry[talentType2] == equipDrawingCfg.type)
                    {
                        curPercent += talentVal2;
                    }
                }
            }
            if (talentType3 >= 26)
            {
                if (talentType3 <= 48)
                {
                    if (StaticConstants.talentDataBaseEntry[talentType3] == equipDrawingCfg.sub_type)
                    {
                        curPercent += talentVal3;
                    }
                }
                else
                {
                    if (StaticConstants.talentDataBaseEntry[talentType3] == equipDrawingCfg.type)
                    {
                        curPercent += talentVal3;
                    }
                }
            }

            percents.Add(curPercent);
        }

        calculateEquipFighting(equipIds, percents);
    }

    //public void calculateEquipFighting(List<int> equipIds)
    //{
    //    for (int i = 0; i < equipIds.Count; i++)
    //    {
    //        var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equipIds[i]);
    //        if (equipCfg == null) continue;
    //        hp_basic += equipCfg.hp_basic;
    //        atk_basic += equipCfg.atk_basic;
    //        def_basic += equipCfg.def_basic;
    //        spd_basic += equipCfg.spd_basic;
    //        acc_basic += equipCfg.acc_basic;
    //        dodge_basic += equipCfg.dodge_basic;
    //        cri_basic += equipCfg.cri_basic;
    //        tough_basic += equipCfg.tough_basic;
    //        piercing_dmg += equipCfg.piercing_dmg;
    //        burn_dmg += equipCfg.burn_dmg;
    //        ment_dmg += equipCfg.ment_dmg;
    //        electricity_dmg += equipCfg.electricity_dmg;
    //        radiation_dmg += equipCfg.radiation_dmg;
    //        piercing_res += equipCfg.piercing_res;
    //        burn_res += equipCfg.burn_res;
    //        ment_res += equipCfg.ment_res;
    //        electricity_res += equipCfg.electricity_res;
    //        radiation_res += equipCfg.radiation_res;
    //    }
    //}

    public void calculateEquipFighting(List<int> equipIds, List<float> percents)
    {
        for (int i = 0; i < equipIds.Count; i++)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equipIds[i]);
            if (equipCfg == null) continue;
            var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipCfg.equip_id);
            if (equipDrawingCfg == null) continue;
            hp_basic += Mathf.CeilToInt(equipCfg.hp_basic * (1 + percents[i]));
            atk_basic += Mathf.CeilToInt(equipCfg.atk_basic * (1 + percents[i]));
            def_basic += Mathf.CeilToInt(equipCfg.def_basic * (1 + percents[i]));
            spd_basic += Mathf.CeilToInt(equipCfg.spd_basic * (1 + percents[i]));
            acc_basic += Mathf.CeilToInt(equipCfg.acc_basic * (1 + percents[i]));
            dodge_basic += Mathf.CeilToInt(equipCfg.dodge_basic * (1 + percents[i]));
            cri_basic += Mathf.CeilToInt(equipCfg.cri_basic * (1 + percents[i]));
            tough_basic += Mathf.CeilToInt(equipCfg.tough_basic * (1 + percents[i]));
            piercing_dmg += Mathf.CeilToInt(equipCfg.piercing_dmg * (1 + percents[i]));
            burn_dmg += Mathf.CeilToInt(equipCfg.burn_dmg * (1 + percents[i]));
            ment_dmg += Mathf.CeilToInt(equipCfg.ment_dmg * (1 + percents[i]));
            electricity_dmg += Mathf.CeilToInt(equipCfg.electricity_dmg * (1 + percents[i]));
            radiation_dmg += Mathf.CeilToInt(equipCfg.radiation_dmg * (1 + percents[i]));
            piercing_res += Mathf.CeilToInt(equipCfg.piercing_res * (1 + percents[i]));
            burn_res += Mathf.CeilToInt(equipCfg.burn_res * (1 + percents[i]));
            ment_res += Mathf.CeilToInt(equipCfg.ment_res * (1 + percents[i]));
            electricity_res += Mathf.CeilToInt(equipCfg.electricity_res * (1 + percents[i]));
            radiation_res += Mathf.CeilToInt(equipCfg.radiation_res * (1 + percents[i]));
        }
    }
}

public class RoleRecruitData
{
    public int heroId;
    public int heroIndex;
    public int intelligence;//资质
    public int recruitCostGold;
    public int recruitCostGem;
    public int recruitState = 0; //enum ERecruitState

    public HeroPropertyData attributeConfig = new HeroPropertyData();
    public HeroProfessionConfigData professionCfg;
    public HeroSkillShowConfig skillCfg;

    //换装外观相关
    public int gender;
    public RoleDress roleDress;
    private HeroEquip equip1, equip2, equip3, equip4, equip5, equip6;

    public void setRecruitData(EnlistHeroInfo heroInfo)
    {
        heroId = heroInfo.heroId;
        heroIndex = heroInfo.heroIndex;
        intelligence = heroInfo.aptitude;
        recruitCostGold = heroInfo.recruitCostGold;
        recruitCostGem = heroInfo.recruitCostGem;
        recruitState = heroInfo.recruitState;

        //换装外观相关
        gender = heroInfo.gender;
        roleDress = heroInfo.roleDress;
        equip1 = heroInfo.equip1;
        equip2 = heroInfo.equip2;
        equip3 = heroInfo.equip3;
        equip4 = heroInfo.equip4;
        equip5 = heroInfo.equip5;
        equip6 = heroInfo.equip6;

        var temp = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(1, heroId);
        attributeConfig.setData(temp, intelligence);
        professionCfg = HeroProfessionConfigManager.inst.GetConfig(heroId);
        skillCfg = HeroSkillShowConfigManager.inst.GetConfig(professionCfg.id_skill1);
    }

    public List<int> GetAllWearEquipId()
    {
        List<int> equipIds = new List<int>();
        if (equip1.equipId != 0)
            equipIds.Add(equip1.equipId);
        if (equip2.equipId != 0)
            equipIds.Add(equip2.equipId);
        if (equip3.equipId != 0)
            equipIds.Add(equip3.equipId);
        if (equip4.equipId != 0)
            equipIds.Add(equip4.equipId);
        if (equip5.equipId != 0)
            equipIds.Add(equip5.equipId);
        if (equip6.equipId != 0)
            equipIds.Add(equip6.equipId);

        return equipIds;
    }

    public List<int> GetHeadDressIds()
    {
        List<int> result = new List<int>();

        List<int> equipIdList = new List<int>();//上衣和帽子
        if (equip2.equipId != 0) equipIdList.Add(equip2.equipId);
        if (equip3.equipId != 0) equipIdList.Add(equip3.equipId);

        result.AddRange(SpineUtils.RoleDressToHeadDressIdList(roleDress));
        result.AddRange(CharacterManager.inst.EquipIdsToDressIds(equipIdList));

        return result;
    }

}

public class RoleDataProxy : TSingletonHotfix<RoleDataProxy>, IDataModelProx
{
    private Dictionary<int, RoleHeroData> heroDic;
    private Dictionary<int, WorkerData> _workerDic;
    private Dictionary<int, HeroExchangeInfoData> heroExchangeDic;
    public int heroFieldCount;
    public int nextRefreshTime;
    public int refreshCount;
    public EGoldOrGem costType;
    public int costValue;
    public List<RoleRecruitData> recruitList;
    public kRoleHeroChange roleHeroState = kRoleHeroChange.max;
    public HeroInfo tempInfo;
    public int enterType;
    public int slotComType;
    public int buyHeroIndex;
    public int buyHeroCostType;
    public int exchangeHeroId;
    //private bool isRefreshing;
    public int curExploreSelectHeroIndex;
    public int curRefugeSelectHeroIndex;
    public int curSelectItemId = -1;
    public bool recruitIsRefreshing = false;

    public bool recruitIsNew;
    public bool exchangeIsNew;

    int refreshEndTime = 0;

    //加个英雄头像缓存池（暂时作为 无限滑动+换装刷新 卡顿 问题的解决方法）
    public Dictionary<int, GraphicDressUpSystem> heroGraphicDressDic;

    public List<RoleHeroData> HeroList;

    public List<RoleHeroData> HeroListSort
    {
        get
        {
            var heroList = HeroList;
            heroList.Sort((x, y) => x.CompareTo(y));
            return heroList;
        }
    }

    public List<WorkerData> WorkerList
    {
        get
        {
            return _workerDic.Values.ToList();
        }
    }

    public List<HeroExchangeInfoData> heroExchangeList;
    public List<HeroExchangeInfoData> heroFrontExchangeList
    {
        get
        {
            return heroExchangeDic.Values.ToList();
        }
    }

    public bool workerRedPointShow
    {
        get
        {
            if (WorkerList == null || WorkerList.Count == 0)
            {
                return false;
            }
            else
            {
                return WorkerList.FindAll(t => t.state == EWorkerState.CanUnlock && t.isNew).Count > 0;
            }
        }
    }

    public bool heroRedPointShow
    {
        get
        {
            if ((RoleDataProxy.inst.costValue == 0 && RoleDataProxy.inst.recruitIsNew) || (RoleDataProxy.inst.hasCanExchangeHero && RoleDataProxy.inst.exchangeIsNew) || (RoleDataProxy.inst.hasCanBuyField && RoleDataProxy.inst.FieldNumAbtractHeroNum <= 0))
            {
                return true;
            }
            else
            {
                for (int i = 0; i < HeroList.Count; i++)
                {
                    var curData = HeroList[i];
                    var curBestList = GetHeroBestEquips(curData.uid);
                    if (curBestList.Count > 0 && curData.currentState != 2 && curData.hasRedPoint != 2)
                    {
                        curData.hasRedPoint = 1;
                        return true;
                    }
                }
                return false;
            }
        }
    }

    public bool JudgeHeroFieldIsMax
    {
        get
        {
            var cfg = FieldConfigManager.inst.GetFieldConfig(1, heroFieldCount + 1);
            return cfg == null ? true : false;
        }
    }

    public int FieldNumAbtractHeroNum
    {
        get
        {
            int num = heroFieldCount - HeroList.Count; // 减去英雄数量
            return num;
        }
    }

    public int GetIdleStateHeroCount
    {
        get
        {
            return HeroList.FindAll(t => t.currentState == 0).Count;
        }
    }

    public bool hasBestEquip
    {
        get
        {
            for (int i = 0; i < HeroList.Count; i++)
            {
                var list = GetHeroBestEquips(HeroList[i].uid);
                if (list.Count > 0)
                    return true;
            }
            return false;
        }
    }

    public bool hasCanExchangeHero
    {
        get
        {
            var playerData = UserDataProxy.inst.playerData;
            foreach (var item in heroExchangeDic.Values)
            {
                if (item.state == EHeroExchangeState.Idle && playerData.gold >= item.unlockCost && playerData.level >= item.unlockLevel)
                {
                    return true;
                }
            }

            return false;
        }
    }

    bool _hasCanBuyField = false;
    public bool hasCanBuyField
    {
        get { return _hasCanBuyField; }
        set { _hasCanBuyField = value; }
    }

    public void Clear()
    {
        heroDic.Clear();
        _workerDic.Clear();
        recruitList.Clear();
        HeroList.Clear();
        WorkerList.Clear();
        heroExchangeDic.Clear();
        tempInfo = null;
    }

    public void Init()
    {
        heroDic = new Dictionary<int, RoleHeroData>();
        _workerDic = new Dictionary<int, WorkerData>();
        heroExchangeDic = new Dictionary<int, HeroExchangeInfoData>();
        recruitList = new List<RoleRecruitData>();
        heroGraphicDressDic = new Dictionary<int, GraphicDressUpSystem>();
        HeroList = new List<RoleHeroData>();

        Helper.AddNetworkRespListener(MsgType.Response_Hero_Data_Cmd, GetRoleData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_FieldUnlock_Cmd, CreatNewFieldCom);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_BuyList_Cmd, GetRecruitListData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_BuyListRefresh_Cmd, GetRecruitListRefreshData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Buy_Cmd, GetBuyNewHero);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Equip_Cmd, GetChangeEquip);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_EquipAuto_Cmd, getOneHeroAllEquips);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Setting_Cmd, GetHeroRenameData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Fire_Cmd, GetFireHeroData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_UseItem_Cmd, GetHeroUseItemData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Recover_Cmd, GetHeroRecover);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Transfer_Cmd, GetHeroTransferData);
        Helper.AddNetworkRespListener(MsgType.Response_Worker_MakeExp_Cmd, GetWorkerMakeExpData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_WorkerChange_Cmd, GetWorkerChangeData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_DataRefresh_Cmd, GetHeroRefreshData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Change_Cmd, GetHeroChangeData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_FixBrokenEquip_Cmd, GetRepairEquipData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_ExchangeList_Cmd, GetHeroExchangeListData);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_Exchange_Cmd, GetHeroExchangeData);
    }

    private void GetHeroExchangeData(HttpMsgRspdBase msg)
    {
        var data = (Response_Hero_Exchange)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;

        for (int i = 0; i < data.exchangeHeroList.Count; i++)
        {
            addHeroExchangeData(data.exchangeHeroList[i], false);
        }

        var list = heroExchangeDic.Values.ToList();
        list.Reverse();
        heroExchangeList = list;



        //HotfixBridge.inst.TriggerLuaEvent("RefreshUI_RoleExchange");
    }

    private void GetHeroExchangeListData(HttpMsgRspdBase msg)
    {
        var data = (Response_Hero_ExchangeList)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;

        for (int i = 0; i < data.exchangeHeroList.Count; i++)
        {
            addHeroExchangeData(data.exchangeHeroList[i], true);
        }

        var list = heroExchangeDic.Values.ToList();
        list.Reverse();
        heroExchangeList = list;
    }

    private void addHeroExchangeData(ExchangeHeroInfo data, bool isInit)
    {

        if (heroExchangeDic.ContainsKey(data.id))
        {
            var dicData = heroExchangeDic[data.id];
            EHeroExchangeState lastState = dicData.state;
            dicData.setData(data);
            var playerData = UserDataProxy.inst.playerData;

            if (lastState == EHeroExchangeState.Lock && dicData.state == EHeroExchangeState.Idle && playerData.gold >= dicData.unlockCost && playerData.level >= dicData.unlockLevel)
            {
                exchangeIsNew = true;
            }
        }
        else
        {
            HeroExchangeInfoData tempData = new HeroExchangeInfoData();
            tempData.setData(data);
            heroExchangeDic.Add(tempData.id, tempData);

            //var playerData = UserDataProxy.inst.playerData;
            //if (tempData.state == EHeroExchangeState.Idle && playerData.gold >= tempData.unlockCost && playerData.level >= tempData.unlockLevel)
            //{
            //    exchangeIsNew = true;
            //}
        }

    }

    public void CheckHasNewExhcangeHero(int lastLv, int curLv)
    {
        if (heroExchangeDic == null) return;
        foreach (var item in heroExchangeDic.Values)
        {
            if (item.state == EHeroExchangeState.Idle && item.unlockLevel > lastLv && item.unlockLevel <= curLv)
                exchangeIsNew = true;
        }
    }

    private void GetRecruitListRefreshData(HttpMsgRspdBase msg)
    {
        var data = (Response_Hero_BuyListRefresh)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        //isRefreshing = true;
    }


    public void ClearHeroGraphicDressCache()
    {
        foreach (var item in heroGraphicDressDic)
        {
            if (item.Value != null)
            {
                GameObject.Destroy(item.Value.gameObject);
            }
        }

        heroGraphicDressDic.Clear();
    }


    public void ResetHeroGraphicDressCache()
    {
        foreach (var item in HeroList)
        {
            resetHeroGraphicDress(item);
        }
    }

    public void resetHeroGraphicDress(int heroUid, Action<GraphicDressUpSystem> comAction)
    {
        if (heroGraphicDressDic.ContainsKey(heroUid))
        {
            //GameObject.Destroy(heroGraphicDressDic[heroData.uid].gameObject);
            if (comAction != null)
            {
                comAction?.Invoke(heroGraphicDressDic[heroUid]);
            }
        }
        else
        {
            var heroData = GetHeroDataByUid(heroUid);
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)heroData.gender), heroData.GetHeadDressIds(), (EGender)heroData.gender, callback: system =>
            {
                if (heroGraphicDressDic.ContainsKey(heroData.uid))
                {
                    GameObject.Destroy(heroGraphicDressDic[heroData.uid].gameObject);
                }
                heroGraphicDressDic[heroData.uid] = system;
                //system.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
                if (comAction != null)
                {
                    comAction?.Invoke(system);
                }
            });
        }
    }

    public void ResetHeroExchangeGraphicCache()
    {
        foreach (var item in heroExchangeList)
        {
            resetHeroGraphicDress(item);
        }
    }

    public void ReSetHeroRedPoints()
    {
        foreach (var item in HeroList)
        {
            item.hasRedPoint = 0;
        }
    }

    void resetHeroGraphicDress(RoleHeroData heroData)
    {
        CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)heroData.gender), heroData.GetHeadDressIds(), (EGender)heroData.gender, callback: system =>
        {
            if (heroGraphicDressDic.ContainsKey(heroData.uid))
            {
                GameObject.Destroy(heroGraphicDressDic[heroData.uid].gameObject);
            }
            heroGraphicDressDic[heroData.uid] = system;
            system.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
            system.transform.localScale = Vector3.one;
            system.transform.localPosition = Vector3.zero;
        });
    }

    void resetHeroGraphicDress(HeroExchangeInfoData heroData)
    {
        CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)heroData.gender), SpineUtils.RoleDressToHeadDressIdList(heroData.roleDress), (EGender)heroData.gender, callback: system =>
        {
            if (heroGraphicDressDic.ContainsKey(heroData.id))
            {
                GameObject.Destroy(heroGraphicDressDic[heroData.id].gameObject);
            }
            system.gameObject.name = heroData.id.ToString();
            heroGraphicDressDic[heroData.id] = system;
            system.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
            system.transform.localScale = Vector3.one;
            system.transform.localPosition = Vector3.zero;
        });
    }

    void removeHeroGraphicDress(int heroUid)
    {
        if (heroGraphicDressDic.ContainsKey(heroUid))
        {
            var data = heroGraphicDressDic[heroUid];
            heroGraphicDressDic.Remove(heroUid);
            GameObject.Destroy(data.gameObject);
        }
    }

    private void GetRepairEquipData(HttpMsgRspdBase msg)
    {
        var data = (Response_Hero_FixBrokenEquip)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        AddHeroData(data.heroInfo);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_REPAIREQUIPDATA);
    }

    private void GetHeroChangeData(HttpMsgRspdBase msg)
    {
        Response_Hero_Change data = (Response_Hero_Change)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        if (roleHeroState == kRoleHeroChange.UseExpItem)
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUSEITEMANIM_SHOW, data.heroList[0]);
        }
        else if (roleHeroState == kRoleHeroChange.UseRestItem)
        {
            tempInfo = data.heroList[0];
            for (int i = 0; i < data.heroList.Count; i++)
            {
                AddHeroData(data.heroList[i]);
            }

            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERESTING_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORESELECTHERO_HIDEUI);
            HotfixBridge.inst.TriggerLuaEvent("HideUI_RefugeSelectHero");
            HotfixBridge.inst.TriggerLuaEvent("RefreshUI_RefugeSelectHero");
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_SETHEROINFODATA, tempInfo.heroUid);
            if (data.heroList.Count > 1)
            {
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.RESPONSE_PREPAREREFRESHDATA);
                HotfixBridge.inst.TriggerLuaEvent("RefreshUI_RefugePrepareUI");
            }
            else if (data.heroList.Count == 1)
            {
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREPREPAREADD_COM, tempInfo.heroUid, curExploreSelectHeroIndex);
                curExploreSelectHeroIndex = 0;
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_RefugePrepareUIAddHero", tempInfo.heroUid, curRefugeSelectHeroIndex);
                curRefugeSelectHeroIndex = 0;
            }

            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 0);
            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_EXPLORESHIFTIN);
        }
        else if (roleHeroState == kRoleHeroChange.ExploreImmediately)
        {
            for (int i = 0; i < data.heroList.Count; i++)
            {
                AddHeroData(data.heroList[i]);
            }
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROSHIFTIN);
        }
        roleHeroState = kRoleHeroChange.max;

        HotfixBridge.inst.TriggerLuaEvent("Activity_DeadRisingEvent.RefreshUI_Activity_DeadRising_SelectHeroUI");

    }

    private void GetHeroRefreshData(HttpMsgRspdBase msg)
    {
        Response_Hero_DataRefresh data = (Response_Hero_DataRefresh)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        AddHeroData(data.hero);

        var heroData = GetHeroDataByUid(data.hero.heroUid);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_SETHEROINFODATA, data.hero.heroUid);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_RESTINGSETDATA, heroData, 0);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_EXPLORESHIFTIN);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEISSHOWING_SHOWUI);

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver && GuideDataProxy.inst.CurInfo.m_curCfg != null)
        {
            if (data.hero.heroUid == 10101 && (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.RestrictAndJudgeHeroTime)
            {
                GuideManager.inst.SetWaitHeroTimeNext(data.hero.stateRemainTime <= 0);
            }
        }

        HotfixBridge.inst.TriggerLuaEvent("Activity_DeadRisingEvent.RefreshUI_Activity_DeadRising_SelectHeroUI");

    }

    private void GetHeroUseItemData(HttpMsgRspdBase msg)
    {
        Response_Hero_UseItem data = (Response_Hero_UseItem)msg;

        if ((EItemUse)data.itemUse == EItemUse.Success)
        {
            roleHeroState = kRoleHeroChange.UseExpItem;
        }
    }

    private void GetHeroRecover(HttpMsgRspdBase msg)
    {
        Response_Hero_Recover data = (Response_Hero_Recover)msg;

        if ((EErrorCode)data.errorCode == EErrorCode.EEC_Success)
            roleHeroState = kRoleHeroChange.UseRestItem;
    }

    private void GetWorkerMakeExpData(HttpMsgRspdBase msg)
    {
        Response_Worker_MakeExp data = (Response_Worker_MakeExp)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        var WorkerExpDataList = new List<WorkerExpData>();

        for (int i = 0; i < data.workerMakeExpList.Count; i++)
        {
            var workerMakeExpData = data.workerMakeExpList[i];
            WorkerData wkData = GetWorker(workerMakeExpData.makeWorkerId);
            if (wkData == null) continue;
            if (wkData.level == workerMakeExpData.makeLevelWorker)
            {
                int changeExp = workerMakeExpData.makeExpCount - wkData.exp;
                wkData.SetExp(workerMakeExpData.makeExpCount);

                //做一层经验已满的筛选
                if (changeExp != 0)
                {
                    WorkerExpDataList.Add(new WorkerExpData() { id = workerMakeExpData.makeWorkerId, changeExp = changeExp });
                }
                else if (changeExp == 0 && wkData.exp >= wkData.maxExp)
                {
                    WorkerExpDataList.Add(new WorkerExpData() { id = workerMakeExpData.makeWorkerId, changeExp = changeExp });
                }
            }
            else
            {
                wkData.SetExp(workerMakeExpData.makeExpCount);
                wkData.SetLevel(workerMakeExpData.makeLevelWorker);
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutWorker { type = ReceiveInfoUIType.WorkerUp, workerId = wkData.id });
            }
        }
        //工匠经验增加 改为一个列表
        if (WorkerExpDataList.Count > 0)
        {
            EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.Worker_ExpChange, WorkerExpDataList);

            var list = WorkerExpDataList.FindAll(t => t.changeExp == 0);

            if (list.Count > 0)
            {
                HotfixBridge.inst.TriggerLuaEvent("TextTip_FullWorkerExpDataList", list);
            }

        }


    }

    private void GetWorkerChangeData(HttpMsgRspdBase msg)
    {
        Response_Hero_WorkerChange data = (Response_Hero_WorkerChange)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;

        SetWorkerInfo(data.worker, false, data.reason);
    }

    private void GetHeroTransferData(HttpMsgRspdBase msg)
    {
        Response_Hero_Transfer data = (Response_Hero_Transfer)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        AddHeroData(data.hero);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLETRANSFER_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLETRANSFERCOM_SHOWUI, data.hero.heroUid);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.hero.heroUid);
    }

    private void GetFireHeroData(HttpMsgRspdBase msg)
    {
        Response_Hero_Fire data = (Response_Hero_Fire)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        AudioManager.inst.PlaySound(14);
        RemoveHeroData(data.hero.heroUid);
        removeHeroGraphicDress(data.hero.heroUid);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
    }

    private void GetHeroRenameData(HttpMsgRspdBase msg)
    {
        Response_Hero_Setting data = (Response_Hero_Setting)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        AudioManager.inst.PlaySound(13);
        AddHeroData(data.hero);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.hero.heroUid);
    }

    private void GetChangeEquip(HttpMsgRspdBase msg)
    {
        Response_Hero_Equip data = (Response_Hero_Equip)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        var roleData = GetHeroDataByUid(data.heroUid);
        if (roleData != null)
        {
            roleData.WearEquip(data.equipField, data.equip);

            resetHeroGraphicDress(roleData);
        }

        if (data.onOrOff == 0)
        {
            AudioManager.inst.PlaySound(19);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEWEAREQUIP_HIDEUI);
            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.heroUid);
        }
        else
        {
            AudioManager.inst.PlaySound(20);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEEQUIP_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.heroUid);
        }
    }

    void getOneHeroAllEquips(HttpMsgRspdBase msg)
    {
        Response_Hero_EquipAuto data = (Response_Hero_EquipAuto)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;

        var roleData = GetHeroDataByUid(data.heroUid);

        if (roleData != null)
        {
            foreach (var changeEquip in data.equip)
            {
                roleData.WearEquip(changeEquip.equipPosId, changeEquip);
            }

            resetHeroGraphicDress(roleData);
        }

        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROEQUIPAUTO, data.heroUid);
    }

    private void GetRecruitListData(HttpMsgRspdBase msg)
    {
        Response_Hero_BuyList data = (Response_Hero_BuyList)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        nextRefreshTime = data.nextRefreshTime;
        refreshEndTime = data.nextRefreshTime + (int)GameTimer.inst.serverNow;
        costType = (EGoldOrGem)data.refreshCostType;
        costValue = data.refreshCostValue;
        refreshCount = data.recruitRefreshCount;
        setRecruitListData(data.enlistHeroList);

        calculateRefreshTime();

        int isSkip = SaveManager.inst.GetInt("RoleRecruitAnimSkipFlag");
        if(isSkip == 1)
        {
            recruitIsRefreshing = false;
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_RECRUITLISTBAR);
        }

        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_RECRUITLIST);
    }

    int refreshTimerId = 0;
    private void calculateRefreshTime()
    {

        WorldParConfig worldParConfig = WorldParConfigManager.inst.GetConfig(305);

        bool timerOnOff = true;

        if (worldParConfig != null && worldParConfig.parameters == 0) timerOnOff = false;

        if (!timerOnOff)
        {
            recruitIsNew = false;
            nextRefreshTime = -1;
            return;
        }


        if (nextRefreshTime > 0)
        {
            recruitIsNew = false;
            if (refreshTimerId > 0)
            {
                GameTimer.inst.RemoveTimer(refreshTimerId);
                refreshTimerId = 0;
            }

            refreshTimerId = GameTimer.inst.AddTimer(1, () =>
             {
                 if (nextRefreshTime <= 0)
                 {
                     EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITLIST);
                     GameTimer.inst.RemoveTimer(refreshTimerId);
                     refreshTimerId = 0;
                 }
                 else
                 {
                     nextRefreshTime = refreshEndTime - (int)GameTimer.inst.serverNow;
                 }
             });
        }
        else
        {
            recruitIsNew = true;
        }
    }

    private void setRecruitListData(List<EnlistHeroInfo> dataList)
    {
        recruitList.Clear();
        for (int i = 0; i < dataList.Count; i++)
        {
            int index = i;
            RoleRecruitData tempData = new RoleRecruitData();
            tempData.setRecruitData(dataList[index]);
            recruitList.Add(tempData);
        }
    }

    private void GetBuyNewHero(HttpMsgRspdBase msg)
    {
        Response_Hero_Buy data = (Response_Hero_Buy)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        if (data.errorCode != 113)
        {
            PlatformManager.inst.GameHandleEventLog("Recruit_Hero", "");

            var roleData = AddHeroData(data.newHero);
            resetHeroGraphicDress(roleData);

            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUITSUB_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUIT_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.GETHERO_SHOWUI, data.newHero.heroUid);
            //EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutWorker { type = ReceiveInfoUIType.GetNewHero, workerId = data.newHero.heroUid });
        }
    }

    private void GetRoleData(HttpMsgRspdBase msg)
    {
        Response_Hero_Data data = (Response_Hero_Data)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        heroFieldCount = data.heroFieldLimit;


        if (JudgeHeroFieldIsMax)
        {
            hasCanBuyField = false;
        }
        else
        {
            var fieldCfg = FieldConfigManager.inst.GetFieldConfig(1, heroFieldCount + 1);
            if (fieldCfg != null)
            {
                hasCanBuyField = fieldCfg.money <= UserDataProxy.inst.playerData.gold && fieldCfg.level <= UserDataProxy.inst.playerData.level;
            }
            else
            {
                hasCanBuyField = false;
            }
        }



        for (int i = 0; i < data.heroList.Count; i++)
        {
            int index = i;
            var heroData = data.heroList[index];
            var roleData = AddHeroData(heroData);
        }

        for (int i = 0; i < data.workerList.Count; i++)
        {
            SetWorkerInfo(data.workerList[i], true, -1);
        }

        ///测试
        //var woD = new WorkerData(new WorkerInfo() { workerId = 4});
        //woD.state = 1;
        //var woD2 = new WorkerData(new WorkerInfo() { workerId = 5});
        //woD2.state = 1;
        //_workerDic.Add(4, woD);
        //_workerDic.Add(5, woD2);
    }

    public RoleHeroData AddHeroData(HeroInfo info)
    {
        if (heroDic.ContainsKey(info.heroUid))
        {
            heroDic[info.heroUid].setData(info);
        }
        else
        {
            RoleHeroData data = new RoleHeroData();
            data.setData(info);
            heroDic.Add(info.heroUid, data);
        }
        HeroList = heroDic.Values.ToList();

        return heroDic[info.heroUid];
    }

    public void RecordHeroData(HeroInfo info)
    {
        tempInfo = info;
        AddHeroData(info);
    }

    private void RemoveHeroData(int heroUid)
    {
        if (heroDic.ContainsKey(heroUid))
        {
            var tempHeroData = heroDic[heroUid];
            tempHeroData.clearTime();
            heroDic.Remove(heroUid);
            //tempHeroData = null;
        }
        else
            Logger.error("没有Uid为" + heroUid + "的英雄数据");

        HeroList = heroDic.Values.ToList();
    }

    public List<HeroChangeEquipData> GetHeroBestEquips(int heroUid)
    {
        List<HeroChangeEquipData> heroChangeEquips = new List<HeroChangeEquipData>();

        RoleHeroData data = GetHeroDataByUid(heroUid);

        if (data == null) return heroChangeEquips;


        Dictionary<int, List<int>> equipDic = HeroProfessionConfigManager.inst.GetEquipDic(data.id);

        for (int i = 0; i < equipDic.Count; i++)
        {
            List<EquipItem> equips = ItemBagProxy.inst.GetEquipItemsByHero(equipDic[i].ToArray(), heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level).equip_lv);

            if (equips.Count > 0)//仓库数量至少有1个
            {

                if (equips.Count > 1) equips.Sort((a, b) => b.GetEquipFightingSum(data.talentConfig).CompareTo(a.GetEquipFightingSum(data.talentConfig)));
                EquipItem fightingMaxEquip = equips[0];//找到战力最高的那个

                HeroEquip heroEquip = data.GetEquipByField(i + 1);
                if (heroEquip != null)
                {
                    if (heroEquip.equipId > 0)//和当前穿的装备了进行比较
                    {
                        EquipConfig cfg = EquipConfigManager.inst.GetEquipInfoConfig(heroEquip.equipId);

                        if (cfg != null && cfg.GetFightingSum(data.talentConfig) < fightingMaxEquip.GetEquipFightingSum(data.talentConfig))
                        {
                            heroChangeEquips.Add(new HeroChangeEquipData(data.uid, i + 1, fightingMaxEquip.itemUid));
                        }
                    }
                    else //当前部位没穿装备
                    {
                        heroChangeEquips.Add(new HeroChangeEquipData(data.uid, i + 1, fightingMaxEquip.itemUid));
                    }
                }
            }


        }

        return heroChangeEquips;
    }

    private void CreatNewFieldCom(HttpMsgRspdBase msg)
    {
        Response_Hero_FieldUnlock data = (Response_Hero_FieldUnlock)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        heroFieldCount = data.heroFieldLimit;

        if (JudgeHeroFieldIsMax)
        {
            hasCanBuyField = false;
        }
        else
        {
            var fieldCfg = FieldConfigManager.inst.GetFieldConfig(1, heroFieldCount + 1);
            if (fieldCfg != null)
            {
                hasCanBuyField = fieldCfg.money <= UserDataProxy.inst.playerData.gold && fieldCfg.level <= UserDataProxy.inst.playerData.level;
            }
            else
            {
                hasCanBuyField = false;
            }
        }


        if (slotComType == 0)
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.BUYSLOT_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_BUYHERO, buyHeroIndex, buyHeroCostType);
        }
        else if (slotComType == 1)
        {
            //  发送消息刷新列表
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.BUYSLOT_HIDEUI);
            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, 0);
        }
        else if (slotComType == 2)
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.BUYSLOT_HIDEUI);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HEROEXCHANGE, exchangeHeroId);
        }
    }

    private void SetWorkerInfo(WorkerInfo workerInfo, bool isRoleDataInit, int reson)
    {
        if (!_workerDic.ContainsKey(workerInfo.workerId))
        {
            var data = new WorkerData(workerInfo);
            _workerDic.Add(data.id, data);

            //解锁新的工匠
            if (!isRoleDataInit && (workerInfo.workerState == (int)EWorkerState.Unlock) && reson != (int)EItemLogReason.Prize)
            {
                PlatformManager.inst.GameHandleEventLog("Unlock_" + workerInfo.workerId, "");

                GUIManager.HideView<WorkerRecruitUI>();

                if (GuideDataProxy.inst.CurInfo.isAllOver)
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutWorker { type = ReceiveInfoUIType.UnlockWorker, workerId = data.id });
            }
        }
        else
        {
            EWorkerState lastState = _workerDic[workerInfo.workerId].state;
            _workerDic[workerInfo.workerId].SetInfo(workerInfo);
            var data = _workerDic[workerInfo.workerId];

            //前端红点
            if (!isRoleDataInit && (lastState == EWorkerState.Locked && workerInfo.workerState == (int)EWorkerState.CanUnlock))
            {
                data.isNew = true;
                EventController.inst.TriggerEvent(GameEventType.REFRESHMAINUIREDPOINT);
            }


            //解锁新的工匠
            if (!isRoleDataInit && (lastState == EWorkerState.CanUnlock && workerInfo.workerState == (int)EWorkerState.Unlock) && reson != (int)EItemLogReason.Prize)
            {
                PlatformManager.inst.GameHandleEventLog("Unlock_" + workerInfo.workerId, "");

                GUIManager.HideView<WorkerRecruitUI>();

                if (GuideDataProxy.inst.CurInfo.isAllOver)
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutWorker { type = ReceiveInfoUIType.UnlockWorker, workerId = data.id });
            }
        }

        EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.Worker_DataChg);
    }

    public WorkerData GetWorker(int id)
    {
        if (_workerDic.ContainsKey(id))
        {
            return _workerDic[id];
        }

        return null;
    }

    public WorkerData GetNearWorkerData(WorkerData curData, bool isLeft)
    {
        var list = WorkerList.FindAll(t => t.state == curData.state);

        int index = list.IndexOf(curData);

        index += isLeft ? -1 : 1;
        if (index == -1) index = list.Count - 1;
        if (index == list.Count) index = 0;

        return list[index];
    }

    public List<RoleHeroData> GetRestingStateHeroCount()
    {
        List<RoleHeroData> tempList = new List<RoleHeroData>();
        foreach (var hero in heroDic.Values)
        {
            if (hero.currentState == 1)
                tempList.Add(hero);
        }

        tempList.Sort(CompareByTime);

        return tempList;
    }

    public static int CompareByTime(RoleHeroData c1, RoleHeroData c2)
    {
        if (c1.remainTime == c2.remainTime)
        {
            if (c1.fightingNum == c2.fightingNum)
            {
                return c1.level.CompareTo(c2.level) * -1;
            }
            return c1.fightingNum < c2.fightingNum ? 1 : -1;
        }

        return c1.remainTime < c2.remainTime ? -1 : 1;
    }

    public RoleRecruitData GetCurIndexRecruitData(int index)
    {
        for (int i = 0; i < recruitList.Count; i++)
        {
            if (recruitList[i].heroIndex == index)
                return recruitList[i];
        }

        //Logger.error("没有下标是" + index + "的招募英雄数据");
        return null;
    }

    public RoleHeroData GetHeroDataByUid(int uid)
    {
        if (heroDic.ContainsKey(uid))
            return heroDic[uid];

        Logger.error("没有uid是" + uid + "的英雄数据");
        return null;
    }

    public int GetNearHeroData(RoleHeroData curData, bool isLeft)
    {
        int index = HeroList.IndexOf(curData);

        index += isLeft ? -1 : 1;
        if (index == -1) index = HeroList.Count - 1;
        if (index == HeroList.Count) index = 0;

        return HeroList[index].uid;
    }

    public List<RoleHeroData> GetNotFightingStateHeroList()
    {
        return HeroListSort.FindAll(t => t.currentState != 2 && !t.isSelect);
    }

    public int ReturnRarityByAptitude(int aptitude)
    {
        if (aptitude <= WorldParConfigManager.inst.GetConfig(266).parameters)
            return 1;
        else if (aptitude >= WorldParConfigManager.inst.GetConfig(267).parameters && aptitude <= WorldParConfigManager.inst.GetConfig(268).parameters)
            return 2;
        else if (aptitude >= WorldParConfigManager.inst.GetConfig(269).parameters && aptitude <= WorldParConfigManager.inst.GetConfig(270).parameters)
            return 3;
        else
            return 4;
    }

    public string ReturnStrByResult(int curProperty, int lastProperty)
    {
        int result = curProperty - lastProperty;
        return (result >= 0 ? "+" : "-") + result;
    }

    public int GetHeroUidByExplore(int suggestFighting)
    {
        RoleHeroData tempData = GetNotFightingStateHeroList().Find(t => t.currentState == 0);
        if (tempData != null)
        {
            return tempData.uid;
        }
        return -1;
    }

    int localUid = -1;
    int curSum = 0;
    int localId = 0;
    int localType = 0;
    int local_hero_grade = 0;
    int localCostId = 0;
    public int GetHeroUidByExploreIndex(int posIndex)
    {
        localUid = -1;
        var tempList = GetNotFightingStateHeroList().FindAll(t => t.currentState == 0);

        if (posIndex == 0 || posIndex == 1)
        {
            for (int i = 0; i < tempList.Count; i++)
            {
                var curData = tempList[i];
                if (i == 0)
                {
                    setBigData(curData);
                    continue;
                }

                if (localType == curData.config.type)
                {
                    if (local_hero_grade == curData.config.hero_grade)
                    {
                        if (local_hero_grade == 1)
                        {
                            if (curData.fightingNum > curSum)
                            {
                                setBigData(curData);
                            }
                        }
                        else
                        {
                            if (localCostId == curData.config.cost_item1_id)
                            {
                                if (curData.fightingNum > curSum)
                                {
                                    setBigData(curData);
                                }
                            }
                            else
                            {
                                if (curData.config.id < localId)
                                {
                                    setBigData(curData);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (local_hero_grade == 1 || curData.config.hero_grade == 1)
                        {
                            if (curData.config.hero_grade > local_hero_grade)
                            {
                                setBigData(curData);
                            }
                        }
                        else
                        {
                            if (localCostId == curData.config.cost_item1_id)
                            {
                                if (curData.config.hero_grade > local_hero_grade)
                                {
                                    setBigData(curData);
                                }
                            }
                            else
                            {
                                if (curData.id < localId)
                                {
                                    setBigData(curData);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (curData.config.type < localType)
                    {
                        setBigData(curData);
                    }
                }
            }
            return localUid;
        }
        else
        {
            for (int i = 0; i < tempList.Count; i++)
            {
                var curData = tempList[i];
                if (i == 0)
                {
                    setBigData(curData);
                    continue;
                }
                if (curData.fightingNum > curSum)
                {
                    setBigData(curData);
                }
                else if (curData.fightingNum == curSum)
                {
                    if (localType != curData.config.type)
                    {
                        if (curData.config.type > localType)
                        {
                            setBigData(curData);
                        }
                    }
                    else
                    {
                        if (local_hero_grade == curData.config.hero_grade)
                        {
                            if (localCostId != curData.config.cost_item1_id)
                            {
                                if (curData.config.id < localId)
                                {
                                    setBigData(curData);
                                }
                            }
                        }
                        else
                        {
                            if (local_hero_grade == 1 || curData.config.hero_grade == 1)
                            {
                                if (curData.config.hero_grade > local_hero_grade)
                                {
                                    setBigData(curData);
                                }
                            }
                            else
                            {
                                if (localCostId == curData.config.cost_item1_id)
                                {
                                    if (curData.config.hero_grade > local_hero_grade)
                                    {
                                        setBigData(curData);
                                    }
                                }
                                else
                                {
                                    if (curData.id < localId)
                                    {
                                        setBigData(curData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return localUid;
        }
        return -1;
    }

    private void setBigData(RoleHeroData curData)
    {
        curSum = curData.fightingNum;
        localUid = curData.uid;
        localId = curData.id;
        localType = curData.config.type;
        local_hero_grade = curData.config.hero_grade;
        localCostId = curData.config.cost_item1_id;
    }

    public int GetTargetIndexByRarity(int rarity)
    {
        int start = 0, end = 0;
        if (rarity == 1)
        {
            start = (int)WorldParConfigManager.inst.GetConfig(265).parameters;
            end = (int)WorldParConfigManager.inst.GetConfig(266).parameters;
        }
        else if (rarity == 2)
        {
            start = (int)WorldParConfigManager.inst.GetConfig(267).parameters;
            end = (int)WorldParConfigManager.inst.GetConfig(268).parameters;
        }
        else if (rarity == 3)
        {
            start = (int)WorldParConfigManager.inst.GetConfig(269).parameters;
            end = (int)WorldParConfigManager.inst.GetConfig(270).parameters;
        }
        else if (rarity == 4)
        {
            start = (int)WorldParConfigManager.inst.GetConfig(271).parameters;
            end = (int)WorldParConfigManager.inst.GetConfig(272).parameters;
        }

        int firstIndex = -1;
        int canExchangeIndex = -1;
        var tempList = FGUI.inst.isLandscape ? heroFrontExchangeList : heroExchangeList;
        for (int i = 0; i < tempList.Count; i++)
        {
            int index = i;
            if (tempList[index].intelligence >= start && tempList[index].intelligence <= end)
            {
                if (firstIndex == -1 && (index + 1 == tempList.Count || (index + 1 < tempList.Count && (tempList[index + 1].intelligence < start || tempList[index + 1].intelligence > end))))
                {
                    firstIndex = index;
                }
                if (tempList[index].state == EHeroExchangeState.Idle && tempList[index].unlockCost <= UserDataProxy.inst.playerData.gold && tempList[index].unlockLevel <= UserDataProxy.inst.playerData.level)
                {
                    canExchangeIndex = index;
                }
            }
        }

        return canExchangeIndex != -1 ? canExchangeIndex : firstIndex;
    }

    public HeroExchangeInfoData GetExchangeDataById(int id)
    {
        HeroExchangeInfoData data = new HeroExchangeInfoData();
        if (heroExchangeDic.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    public float GetEquipDamagerVal(int quality)
    {
        quality -= 1;
        if (quality >= 100)
        {
            quality -= 85;
        }

        var cfg = WorldParConfigManager.inst.GetConfig(140 + quality);
        if (cfg != null)
        {
            return cfg.parameters;
        }

        return 0;
    }
}
