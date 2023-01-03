using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleTransferView : ViewBase<RoleTransferComp>
{
    public override string viewID => ViewPrefabName.RoleTransferPanel;
    public override string sortingLayerName => "window";

    RoleHeroData curData;
    int targetHeroId;
    List<HeroProfessionConfigData> professionCfg;
    DressUpSystem heroDress;
    int needLevel;
    bool heroProfessNeedMatEnough;
    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = false;

        AddUIEvent();
        InitComponent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.onClick.AddListener(() =>
        {
            hide();
            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, curData.uid);
        });
        contentPane.transferBtn.onClick.AddListener(() =>
        {
            if (curData.level < needLevel)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("要达到{0}级可转职", needLevel.ToString()), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            if (!heroProfessNeedMatEnough)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 27);
                return;
            }
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HEROTRANSFER, curData.uid, targetHeroId);
        });

    }

    private void InitComponent()
    {
        professionCfg = new List<HeroProfessionConfigData>();
        contentPane.group.SetToggleSize(new Vector2(276, 124), new Vector2(254, 100));
        contentPane.group.OnSelectedIndexValueChange = typeSelectedChange;
    }

    public void jumpToTargetToggle(int heroId)
    {
        if (professionCfg == null || professionCfg.Count <= 0) return;

        for (int i = 0; i < professionCfg.Count; i++)
        {
            int index = i;
            if (professionCfg[index].id == heroId)
            {
                contentPane.group.selectedIndex = index;
                //typeSelectedChange(index);
                return;
            }
        }
    }

    private void typeSelectedChange(int index)
    {
        if (professionCfg.Count > 0)
        {
            AudioManager.inst.PlaySound(11);
            HeroProfessionConfigData occuCfg = professionCfg[index];
            setPropertyData(occuCfg);
        }
    }

    public void setData(int heroUid)
    {
        curData = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        professionCfg = HeroProfessionConfigManager.inst.GetTransferData(curData.id, curData.intelligence);

        contentPane.transferText.text = LanguageManager.inst.GetValueByKey(curData.config.hero_grade == 1 ? "转职" : "进阶");

        if (professionCfg.Count > 1)
        {
            contentPane.group.gameObject.SetActive(true);
            contentPane.singleObj.SetActiveFalse();
            for (int i = 0; i < contentPane.group.togglesBtn.Count; i++)
            {
                int index = i;

                if (i < professionCfg.Count)
                {
                    contentPane.group.togglesBtn[index].gameObject.SetActive(true);
                    contentPane.group.togglesBtn[index].GetComponent<RoleTransferItemUI>().setData(professionCfg[index]);
                }
                else
                {
                    contentPane.group.togglesBtn[index].gameObject.SetActive(false);
                }
            }
            contentPane.group.OnEnableMethod(0);
            //typeSelectedChange(0);
        }
        else if (professionCfg.Count == 1)
        {
            HeroProfessionConfigData occuCfg = professionCfg[0];
            contentPane.singleObj.SetActiveTrue();
            contentPane.singleTypeIcon.SetSprite(occuCfg.atlas, occuCfg.ocp_icon);
            contentPane.singleText.text = LanguageManager.inst.GetValueByKey(occuCfg.name);
            contentPane.group.gameObject.SetActive(false);
            setPropertyData(occuCfg);
            //setNewUnlockEquipType(occuCfg);
            //setGrewData(occuCfg);
            //setPayData(occuCfg);
        }

        SetHeroPrefabData();
    }

    private void SetHeroPrefabData()
    {
        if (heroDress == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)curData.gender, curData.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(curData.roleDress), callback: (dress) =>
            {
                heroDress = dress;
                heroDress.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, 0.9375f);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                heroDress.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByHero(heroDress, (EGender)curData.gender, curData.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(curData.roleDress));
            heroDress.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, 0.9375f);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
            heroDress.Play(idleAnimationName, true);
        }

        contentPane.maskCanvas.sortingLayerName = _uiCanvas.sortingLayerName;
        contentPane.maskCanvas.sortingOrder = _uiCanvas.sortingOrder;
        _uiCanvas.sortingOrder += 2;
    }

    private void setPayData(HeroProfessionConfigData occuCfg)
    {

        var needMats = occuCfg.GetHeroProfessionNeedMatDatas();

        heroProfessNeedMatEnough = true;

        for (int i = 0; i < contentPane.heroTransferNeedMatItems.Count; i++)
        {
            var item = contentPane.heroTransferNeedMatItems[i];

            if (i < needMats.Count)
            {
                if (heroProfessNeedMatEnough && ItemBagProxy.inst.GetItem(needMats[i].itemId).count < needMats[i].needItemCount)
                {
                    heroProfessNeedMatEnough = false;
                }

                item.gameObject.SetActive(true);
                item.SetData(needMats[i]);
            }
            else
            {
                item.gameObject.SetActive(false);
            }

        }
        
        needLevel = occuCfg.level_need;
        if (heroProfessNeedMatEnough && needLevel <= curData.level)
        {
            //contentPane.transferBtn.interactable = true;
            contentPane.transferText.text = LanguageManager.inst.GetValueByKey(curData.config.hero_grade == 1 ? "转职" : "进阶");
            contentPane.tipObj.SetActive(true);
            contentPane.transferBtn.GetComponent<GUIIcon>().SetSprite("__common_1", "icon_lvse");
            //GUIHelper.SetUIGray(contentPane.transferBtn.transform, false);
        }
        else
        {
            if (needLevel > curData.level)
            {
                contentPane.transferText.text = LanguageManager.inst.GetValueByKey("{0}级后可转职", needLevel.ToString());
            }
            //contentPane.transferBtn.interactable = false;
            contentPane.tipObj.SetActive(false);
            contentPane.transferBtn.GetComponent<GUIIcon>().SetSprite("__common_1", "icon_huang");
            //GUIHelper.SetUIGray(contentPane.transferBtn.transform, true);
        }
    }

    private void setPropertyData(HeroProfessionConfigData occuCfg)
    {
        if (occuCfg == null) return;
        targetHeroId = occuCfg.id;
        contentPane.desText.text = LanguageManager.inst.GetValueByKey(occuCfg.ocp_story);
        contentPane.curLifeText.text = curData.attributeConfig.hp_basic.ToString();
        contentPane.curAttText.text = curData.attributeConfig.atk_basic.ToString();
        contentPane.curArmorText.text = curData.attributeConfig.def_basic.ToString();
        contentPane.curSpeedText.text = curData.attributeConfig.spd_basic.ToString();
        contentPane.curDodgeText.text = curData.attributeConfig.dodge_basic.ToString();
        contentPane.curAccText.text = curData.attributeConfig.acc_basic.ToString();

        var Cfg = HeroAttributeConfigManager.inst.GetDataByLevelAndProfession(curData.level, occuCfg.id);
        HeroPropertyData transferAttributeCfg = new HeroPropertyData();
        transferAttributeCfg.setData(Cfg, curData.intelligence, curData.GetAllWearEquipId(), curData.talentConfig);
        contentPane.transferLifeText.text = transferAttributeCfg.hp_basic.ToString();
        contentPane.transferAttText.text = transferAttributeCfg.atk_basic.ToString();
        contentPane.transferArmorText.text = transferAttributeCfg.def_basic.ToString();
        contentPane.transferSpeedText.text = transferAttributeCfg.spd_basic.ToString();
        contentPane.transferDodgeText.text = transferAttributeCfg.dodge_basic.ToString();
        contentPane.transferAccText.text = transferAttributeCfg.acc_basic.ToString();

        var lastHeroCfg = HeroProfessionConfigManager.inst.GetConfig(occuCfg.pre_profession);
        HeroSkillShowConfig skillCfg = new HeroSkillShowConfig();
        if (occuCfg.id_skill1 != lastHeroCfg.id_skill1)
        {
            skillCfg = HeroSkillShowConfigManager.inst.GetConfig(occuCfg.id_skill1);
        }
        else if (occuCfg.id_skill2 != lastHeroCfg.id_skill2)
        {
            skillCfg = HeroSkillShowConfigManager.inst.GetConfig(occuCfg.id_skill2);
        }
        else if (occuCfg.id_skill3 != lastHeroCfg.id_skill3)
        {
            skillCfg = HeroSkillShowConfigManager.inst.GetConfig(occuCfg.id_skill3);
        }
        contentPane.skillIcon.SetSprite(skillCfg.skill_atlas, skillCfg.skill_icon);
        contentPane.skillDescText.text = LanguageManager.inst.GetValueByKey(skillCfg.skill_dec);

        setPayData(occuCfg);
        setGrewData(occuCfg);
        setNewUnlockEquipType(occuCfg);
    }

    private void setGrewData(HeroProfessionConfigData data)
    {
        float hp = data.grew_hp;
        float atk = data.grew_atk;
        float def = data.grew_def;
        for (int i = 0; i < contentPane.hpGrewList.Count; i++)
        {
            int index = i;
            if (hp > 0)
            {
                if (hp < 1 && hp % 1 != 0)
                {
                    contentPane.hpGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng2");
                }
                else
                {
                    contentPane.hpGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
                }
            }
            else
            {
                contentPane.hpGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
            }
            hp -= 1;
        }
        for (int i = 0; i < contentPane.atkGrewList.Count; i++)
        {
            int index = i;
            if (atk > 0)
            {
                if (atk < 1 && atk % 1 != 0)
                {
                    contentPane.atkGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng2");
                }
                else
                {
                    contentPane.atkGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
                }
            }
            else
            {
                contentPane.atkGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
            }
            atk -= 1;
        }
        for (int i = 0; i < contentPane.defGrewList.Count; i++)
        {
            int index = i;
            if (def > 0)
            {
                if (def < 1 && def % 1 != 0)
                {
                    contentPane.defGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng2");
                }
                else
                {
                    contentPane.defGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
                }
            }
            else
            {
                contentPane.defGrewList[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
            }
            def -= 1;
        }
    }

    private void setNewUnlockEquipType(HeroProfessionConfigData data)
    {
        var lastCfg = HeroProfessionConfigManager.inst.GetConfig(data.pre_profession);
        List<int> ids = new List<int>();
        if (data.equip1.Length != lastCfg.equip1.Length)
        {
            //contentPane.roleTransferItem.hasUnlockEquipType.SetActive(true);
            //contentPane.roleTransferItem.notHasUnlockEquipType.SetActive(false);
            for (int i = lastCfg.equip1.Length; i < data.equip1.Length; i++)
            {
                ids.Add(data.equip1[i]);
            }
            for (int i = 0; i < contentPane.unlockNewEquipType.Count; i++)
            {
                int index = i;
                if (index < ids.Count)
                {
                    contentPane.unlockNewEquipType[index].gameObject.SetActive(true);
                    EquipClassification tempClass = EquipConfigManager.inst.GetEquipTypeByID(ids[index]);
                    contentPane.unlockNewEquipType[index].SetSprite(tempClass.Atlas, tempClass.icon);
                }
                else
                {
                    contentPane.unlockNewEquipType[index].gameObject.SetActive(false);
                }
            }
        }
        else if (data.equip6.Length > lastCfg.equip6.Length)
        {
            for (int i = lastCfg.equip6.Length; i < data.equip6.Length; i++)
            {
                ids.Add(data.equip6[i]);
            }
            for (int i = 0; i < contentPane.unlockNewEquipType.Count; i++)
            {
                int index = i;
                if (index < ids.Count)
                {
                    contentPane.unlockNewEquipType[index].gameObject.SetActive(true);
                    EquipClassification tempClass = EquipConfigManager.inst.GetEquipTypeByID(ids[index]);
                    contentPane.unlockNewEquipType[index].SetSprite(tempClass.Atlas, tempClass.icon);
                }
                else
                {
                    contentPane.unlockNewEquipType[index].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < contentPane.unlockNewEquipType.Count; i++)
            {
                int index = i;
                contentPane.unlockNewEquipType[index].gameObject.SetActive(false);
            }
        }
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
