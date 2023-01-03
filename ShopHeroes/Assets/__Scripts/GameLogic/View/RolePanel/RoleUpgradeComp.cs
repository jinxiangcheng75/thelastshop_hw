using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoleUpgradeComp : MonoBehaviour
{
    public RoleUpgradeItem roleUpgradeItem;
    public RoleTransferItem roleTransferItem;
    public ExploreUpgradeItem exploreUpgradeItem;
}

public class RoleUpgradeView : ViewBase<RoleUpgradeComp>
{
    public override string viewID => ViewPrefabName.HeroUpgradeComUI;
    public override string sortingLayerName => "popup";

    RoleHeroData data;
    DressUpSystem heroDress;
    Animator anim;
    int contentType = 0; // 1 - heroUpgrade 2 - heroTransfer 3 - explore
    Sequence se;
    protected override void onInit()
    {
        base.onInit();
    }

    public void CloseAllPanel()
    {
        contentPane.roleUpgradeItem.gameObject.SetActive(false);
        contentPane.roleTransferItem.gameObject.SetActive(false);
        contentPane.exploreUpgradeItem.gameObject.SetActive(false);
    }

    public void ShowUpgradePanel(upgradeItem item)
    {
        if (item.type == kExploreItemUpgradeType.HeroUpgrade)
        {
            setData(item.heroUid, item.intoType);
        }
        else if (item.type == kExploreItemUpgradeType.ExploreUpgrade)
        {
            setExploreData(item.exploreGroupId);
        }
    }

    #region 英雄升级
    public void setData(int heroUid, int type)
    {
        AudioManager.inst.PlaySound(18);
        CloseAllPanel();
        contentPane.roleUpgradeItem.gameObject.SetActive(true);
        data = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        contentType = type;
        contentPane.roleUpgradeItem.contentText.text = LanguageManager.inst.GetValueByKey("恭喜！{0}已到达等级{1}", LanguageManager.inst.GetValueByKey(data.nickName), data.level.ToString());
        contentPane.roleUpgradeItem.levelText.text = data.level.ToString();

        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        contentPane.roleUpgradeItem.typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        contentPane.roleUpgradeItem.typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        contentPane.roleUpgradeItem.qualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);
        contentPane.roleUpgradeItem.nickNameTx.text = LanguageManager.inst.GetValueByKey(data.nickName);
        contentPane.roleUpgradeItem.fightTx.text = data.fightingNum.ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.roleUpgradeItem.horizontalRect);

        heroupgradeconfig curLevelConfig = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level);
        heroupgradeconfig lastLevelConfig = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level - 1);
        if (lastLevelConfig != null)
        {
            if (lastLevelConfig.equip_lv != curLevelConfig.equip_lv)
            {
                contentPane.roleUpgradeItem.canWearObj.SetActive(true);
                contentPane.roleUpgradeItem.canWearEqiupLevelTx.text = LanguageManager.inst.GetValueByKey("可穿戴{0}阶装备", curLevelConfig.equip_lv.ToString());
            }
            else
            {
                contentPane.roleUpgradeItem.canWearObj.SetActive(false);
            }
        }

        setUpgradeAttribute();
        setUpgradePrefab(contentPane.roleUpgradeItem.roleTrans);
        setUpgradeAnim();

        GUIHelper.setRandererSortinglayer(_uiCanvas.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2);
        setUpgradeUIEvent();
    }

    private void setUpgradeUIEvent()
    {
        contentPane.roleUpgradeItem.confirmBtn.onClick.RemoveAllListeners();
        contentPane.roleUpgradeItem.confirmBtn.onClick.AddListener(() =>
        {
            hide();
            if (contentType == 1)
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUSEEXPITEM_SHOWUI, data.uid);
            else if (contentType == 3)
            {
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADEEND);
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.UPGRADENEXT);
            }
        });
    }

    private void setUpgradeAttribute()
    {
        HeroPropertyData last = new HeroPropertyData();
        var lastProfession = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level - 1, data.id);
        var lastTalentConfig = HeroTalentDBConfigManager.inst.GetConfig(data.talentId);
        last.setData(lastProfession, data.intelligence, data.GetAllWearEquipId(), lastTalentConfig);

        HeroPropertyData cur = new HeroPropertyData();
        var curProfession = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level, data.id);
        var curTalentConfig = HeroTalentDBConfigManager.inst.GetConfig(data.talentId);
        cur.setData(curProfession, data.intelligence, data.GetAllWearEquipId(), curTalentConfig);

        //HeroAttributeConfigeData attConfig = new HeroAttributeConfigeData();
        //attConfig = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level - 1, data.id);
        //HeroPropertyData lastPropertyData = new HeroPropertyData();
        //lastPropertyData.setData(attConfig, data.intelligence);

        //HeroAttributeConfigeData curattConfig = new HeroAttributeConfigeData();
        //curattConfig = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level, data.id);
        //HeroPropertyData curPropertyData = new HeroPropertyData();
        //curPropertyData.setData(curattConfig, data.intelligence);

        float hpVal = cur.hp_basic - last.hp_basic;
        float atkVal = cur.atk_basic - last.atk_basic;
        float defVal = cur.def_basic - last.def_basic;

        contentPane.roleUpgradeItem.lifeUpImg.enabled = hpVal > 0;
        contentPane.roleUpgradeItem.atkUpImg.enabled = atkVal > 0;
        contentPane.roleUpgradeItem.defUpImg.enabled = defVal > 0;

        contentPane.roleUpgradeItem.newLifeText.text = "" + hpVal;
        contentPane.roleUpgradeItem.newAtkText.text = "" + atkVal;
        contentPane.roleUpgradeItem.newDefText.text = "" + defVal;
    }

    private void setUpgradePrefab(Transform posTrans)
    {
        if (heroDress == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress), callback: (dress) =>
            {
                heroDress = dress;
                heroDress.SetUIPosition(posTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2, 0.875f);
                GUIHelper.setRandererSortinglayer(_uiCanvas.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2);
                heroShowAnim();
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByHero(heroDress, (EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress));
            heroDress.SetUIPosition(posTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2, 0.875f);
            GUIHelper.setRandererSortinglayer(_uiCanvas.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2);
            heroShowAnim();
        }
    }

    private void setUpgradeAnim()
    {
        anim = contentPane.roleUpgradeItem.GetComponent<Animator>();
        if (anim == null) return;
        if (GameSettingManager.inst.needShowUIAnim)
        {
            if (heroDress != null)
                heroDress.Fade(1, 0.3f).From(0);
            anim.enabled = true;

            anim.CrossFade("show", 0f);
            anim.Update(0f);
            anim.Play("show");

            float animTime = Mathf.Ceil(anim.GetClipLength("hero_upgradeShow"));
            GameTimer.inst.AddTimer(animTime * 1.03f, 1, () =>
            {
                if (contentPane == null) return;
                if (contentPane.roleUpgradeItem.confirmBtn == null) return;
                anim.enabled = false;
                contentPane.roleUpgradeItem.confirmBtn.gameObject.SetActiveTrue();
            });
        }
        else
        {
            if (contentPane.roleUpgradeItem == null) return;
            anim.enabled = false;
            if (contentPane.roleUpgradeItem.confirmBtn == null) return;
            contentPane.roleUpgradeItem.confirmBtn.gameObject.SetActiveTrue();
        }
    }
    #endregion

    #region 英雄进阶
    public void setTransferData(int heroUid)
    {
        AudioManager.inst.PlaySound(18);
        CloseAllPanel();
        contentPane.roleTransferItem.gameObject.SetActive(true);
        data = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        contentType = 2;
        contentPane.roleTransferItem.titleText.text = HeroProfessionConfigManager.inst.GetConfig(data.config.pre_profession).level_need == 0 ? LanguageManager.inst.GetValueByKey("转职成功!") : LanguageManager.inst.GetValueByKey("进阶成功!");
        contentPane.roleTransferItem.titleText2.text = HeroProfessionConfigManager.inst.GetConfig(data.config.pre_profession).level_need == 0 ? LanguageManager.inst.GetValueByKey("转职成功!") : LanguageManager.inst.GetValueByKey("进阶成功!");
        string str = HeroProfessionConfigManager.inst.GetConfig(data.config.pre_profession).level_need == 0 ? LanguageManager.inst.GetValueByKey("恭喜!{0}转职成为{1}!", LanguageManager.inst.GetValueByKey(data.nickName), LanguageManager.inst.GetValueByKey(data.config.name)) : LanguageManager.inst.GetValueByKey("恭喜!{0}进阶成为{1}!", LanguageManager.inst.GetValueByKey(data.nickName), LanguageManager.inst.GetValueByKey(data.config.name));
        contentPane.roleTransferItem.contentText.text = str;

        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        contentPane.roleTransferItem.qualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);
        contentPane.roleTransferItem.nickNameTx.text = LanguageManager.inst.GetValueByKey(data.nickName);
        contentPane.roleTransferItem.fightTx.text = data.fightingNum.ToString("N0");
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.roleTransferItem.horizontalRect);

        JudgeNewUnlock();
        setTransferAttribute();
        setUpgradePrefab(contentPane.roleTransferItem.roleTrans);
        setTransferAnim();
        setTransferUIEvent();
        setTrasnferAllStarData();
    }

    private void JudgeNewUnlock()
    {
        if (data.config.pre_profession != 0)
        {
            var lastCfg = HeroProfessionConfigManager.inst.GetConfig(data.config.pre_profession);
            HeroSkillShowConfig skillCfg = new HeroSkillShowConfig();
            if (data.config.id_skill1 != lastCfg.id_skill1)
            {
                skillCfg = HeroSkillShowConfigManager.inst.GetConfig(data.config.id_skill1);
            }
            else if (data.config.id_skill2 != lastCfg.id_skill2)
            {
                skillCfg = HeroSkillShowConfigManager.inst.GetConfig(data.config.id_skill2);
            }
            else if (data.config.id_skill3 != lastCfg.id_skill3)
            {
                skillCfg = HeroSkillShowConfigManager.inst.GetConfig(data.config.id_skill3);
            }
            contentPane.roleTransferItem.newUnlockSkillIcon.SetSprite(skillCfg.skill_atlas, skillCfg.skill_icon);
            if (data.config.equip1.Length != lastCfg.equip1.Length)
            {
                contentPane.roleTransferItem.hasUnlockEquipType.SetActive(true);
                contentPane.roleTransferItem.notHasUnlockEquipType.SetActive(false);
                List<int> ids = new List<int>();
                for (int i = lastCfg.equip1.Length; i < data.config.equip1.Length; i++)
                {
                    ids.Add(data.config.equip1[i]);
                }
                for (int i = 0; i < contentPane.roleTransferItem.allEquipTypeIcon.Count; i++)
                {
                    int index = i;
                    if (index < ids.Count)
                    {
                        contentPane.roleTransferItem.allEquipTypeIcon[index].gameObject.SetActive(true);
                        EquipClassification tempClass = EquipConfigManager.inst.GetEquipTypeByID(ids[index]);
                        contentPane.roleTransferItem.allEquipTypeIcon[index].SetSprite(tempClass.Atlas, tempClass.icon);
                    }
                    else
                    {
                        contentPane.roleTransferItem.allEquipTypeIcon[index].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                contentPane.roleTransferItem.hasUnlockEquipType.SetActive(false);
                contentPane.roleTransferItem.notHasUnlockEquipType.SetActive(true);
            }
        }
    }

    private void setTrasnferAllStarData()
    {
        for (int i = 0; i < contentPane.roleTransferItem.allStar.Count; i++)
        {
            int index = i;
            if (index < data.config.hero_grade)
            {
                contentPane.roleTransferItem.allStar[index].gameObject.SetActive(true);
                contentPane.roleTransferItem.allStar[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
            }
            else
            {
                if (index < data.transferNumLimit)
                {
                    contentPane.roleTransferItem.allStar[index].gameObject.SetActive(true);
                    contentPane.roleTransferItem.allStar[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
                }
                else
                {
                    contentPane.roleTransferItem.allStar[index].gameObject.SetActive(false);
                }

            }
        }
    }

    private void setTransferAttribute()
    {
        HeroPropertyData last = new HeroPropertyData();
        var lastProfession = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level, data.config.pre_profession);
        var lastTalentConfig = HeroTalentDBConfigManager.inst.GetConfig(data.talentId);
        last.setData(lastProfession, data.intelligence, data.GetAllWearEquipId(), lastTalentConfig);

        HeroPropertyData cur = new HeroPropertyData();
        var curProfession = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level, data.id);
        var curTalentConfig = HeroTalentDBConfigManager.inst.GetConfig(data.talentId);
        cur.setData(curProfession, data.intelligence, data.GetAllWearEquipId(), curTalentConfig);

        //HeroAttributeConfigeData attConfig = new HeroAttributeConfigeData();
        //attConfig = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(data.level, data.config.pre_profession);
        //HeroPropertyData lastPropertyData = new HeroPropertyData();
        //lastPropertyData.setData(attConfig, data.intelligence);

        //contentPane.roleTransferItem.lifeText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.hp_basic, lastPropertyData.hp_basic);
        //contentPane.roleTransferItem.atkText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.atk_basic, lastPropertyData.atk_basic);
        //contentPane.roleTransferItem.defText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.def_basic, lastPropertyData.def_basic);
        //contentPane.roleTransferItem.spdText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.spd_basic, lastPropertyData.spd_basic);
        //contentPane.roleTransferItem.dodgeText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.dodge_basic, lastPropertyData.dodge_basic);
        //contentPane.roleTransferItem.criText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.cri_basic, lastPropertyData.cri_basic);
        //contentPane.roleTransferItem.toughText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.tough_basic, lastPropertyData.tough_basic);
        //contentPane.roleTransferItem.accText.text = RoleDataProxy.inst.ReturnStrByResult(data.attributeConfig.acc_basic, lastPropertyData.acc_basic);

        var lifeVal = cur.hp_basic - last.hp_basic;
        var atkVal = cur.atk_basic - last.atk_basic;
        var defVal = cur.def_basic - last.def_basic;

        contentPane.roleTransferItem.lifeUpImg.enabled = lifeVal > 0;
        contentPane.roleTransferItem.atkUpImg.enabled = atkVal > 0;
        contentPane.roleTransferItem.defUpImg.enabled = defVal > 0;

        contentPane.roleTransferItem.newLifeTx.text = "" + lifeVal;
        contentPane.roleTransferItem.newAtkTx.text = "" + atkVal;
        contentPane.roleTransferItem.newDefTx.text = "" + defVal;
    }

    private void setTransferUIEvent()
    {
        contentPane.roleTransferItem.confirmBtn.onClick.RemoveAllListeners();
        contentPane.roleTransferItem.confirmBtn.onClick.AddListener(() =>
        {
            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, heroUid);
            hide();
        });
    }

    private void setTransferAnim()
    {
        anim = contentPane.roleTransferItem.GetComponent<Animator>();
        if (anim == null) return;
        if (GameSettingManager.inst.needShowUIAnim)
        {
            if (heroDress != null)
                heroDress.Fade(1, 0.3f).From(0);
            anim.enabled = true;
            anim.CrossFade("show", 0f);
            anim.Update(0f);
            anim.Play("show");
            float animTime = Mathf.Ceil(anim.GetClipLength("hero_upgradeShow"));
            GameTimer.inst.AddTimer(animTime * 1.03f, 1, () =>
            {
                if (contentPane == null) return;
                if (contentPane.roleTransferItem.confirmBtn == null) return;
                anim.enabled = false;
                contentPane.roleTransferItem.confirmBtn.gameObject.SetActiveTrue();
            });
        }
        else
        {
            anim.enabled = false;
            if (contentPane.roleTransferItem.confirmBtn == null) return;
            contentPane.roleTransferItem.confirmBtn.gameObject.SetActiveTrue();
            var allList = contentPane.roleTransferItem.allProperty;
            if (allList == null) return;
            for (int i = 0; i < allList.Count; i++)
            {
                int index = i;
                allList[index].gameObject.SetActiveTrue();
            }
        }
    }
    #endregion

    #region 副本升级
    private void setExploreData(int groupId)
    {
        AudioManager.inst.PlaySound(18);
        CloseAllPanel();
        contentPane.exploreUpgradeItem.gameObject.SetActive(true);

        var exploreGroupData = ExploreDataProxy.inst.GetGroupDataByGroupId(groupId);
        var instanceCfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(exploreGroupData.groupData.groupId);
        var instanceLvCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(exploreGroupData.groupData.groupId, exploreGroupData.groupData.level);
        contentPane.exploreUpgradeItem.contentText.text = LanguageManager.inst.GetValueByKey("{0}到达等级{1}", LanguageManager.inst.GetValueByKey(instanceCfg.instance_name), exploreGroupData.groupData.level.ToString());
        //LanguageManager.inst.GetValueByKey(instanceCfg.instance_name) + LanguageManager.inst.GetValueByKey("到达等级") + exploreGroupData.groupData.level;
        contentPane.exploreUpgradeItem.headIcon.SetSprite(StaticConstants.exploreAtlas, instanceCfg.instance_icon);
        contentPane.exploreUpgradeItem.getIcon.SetSprite(instanceLvCfg.effect_atlas, instanceLvCfg.effect_icon);
        contentPane.exploreUpgradeItem.titleText.text = LanguageManager.inst.GetValueByKey(instanceLvCfg.effect_dec1);
        contentPane.exploreUpgradeItem.getNew.text = LanguageManager.inst.GetValueByKey(instanceLvCfg.effect_dec2);

        if (instanceLvCfg.effect_type == 1 || instanceLvCfg.effect_type == 6)
        {
            contentPane.exploreUpgradeItem.upImg.enabled = true;
        }
        else
        {
            contentPane.exploreUpgradeItem.upImg.enabled = false;
        }

        contentPane.exploreUpgradeItem.okBtn.onClick.RemoveAllListeners();
        contentPane.exploreUpgradeItem.okBtn.onClick.AddListener(() =>
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUPGRADEEND);
            //EventController.inst.TriggerEvent(GameEventType.ExploreEvent.UPGRADENEXT);
        });
    }
    #endregion

    void heroShowAnim()
    {
        heroDress?.Play("happy", completeDele: (t) =>
        {
            if (this != null)
            {
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                heroDress.Play(idleAnimationName, true);
            }
        });
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        CloseAllPanel();
        if (anim != null)
        {
            anim.enabled = true;
            anim.CrossFade("null", 0f);
            anim.Update(0f);
        }
        se.Kill();
    }
}
