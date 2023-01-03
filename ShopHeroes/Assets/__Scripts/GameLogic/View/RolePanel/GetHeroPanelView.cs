using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHeroPanelView : ViewBase<GetHeroPanelComp>
{
    public override string viewID => ViewPrefabName.GetHeroPanel;
    public override string sortingLayerName => "window";

    private RoleHeroData data;
    DressUpSystem heroDress;
    protected override void onInit()
    {
        base.onInit();

        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.talentBtn.onClick.AddListener(() => EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLETALENTINTRODUCE_SHOWUI, contentPane.talentBtn.transform, data.talentConfig));
        contentPane.okBtn.onClick.AddListener(() =>
        {
            hide();
            //EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });
    }

    public void InitData(int heroUid)
    {
        contentPane.anim.CrossFade("show", 0f);
        contentPane.anim.Update(0f);
        contentPane.anim.Play("show");
        data = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        string qualityColor = StaticConstants.roleIntelligenceColor[rarity];
        contentPane.typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        contentPane.typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        contentPane.intelligenceText.text = data.intelligence.ToString();
        contentPane.intelligenceText.color = GUIHelper.GetColorByColorHex(qualityColor);
        var cfg = HeroSkillShowConfigManager.inst.GetConfig(data.talentConfig.skill_id);
        contentPane.talentIcon.SetSprite(cfg.skill_atlas, cfg.skill_icon);

        var fxList = contentPane.qualityFx.GetComponentsInChildren<ParticleSystem>(true);
        contentPane.qualityCanvas.sortingOrder = _uiCanvas.sortingOrder + 1;
        foreach (var item in fxList)
        {
            item.startColor = GUIHelper.GetColorByColorHex(qualityColor);
            item.Play();
            item.GetComponent<Renderer>().sortingOrder = _uiCanvas.sortingOrder;
        }

        setHeroPrefabData();
        setPropertyData();
        setStarNum(rarity);
        setEquipContent();
        setSkillData();
    }

    private void setHeroPrefabData()
    {
        if (heroDress == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress), callback: (dress) =>
            {
                heroDress = dress;
                heroDress.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2, 0.75f);
                //string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                //heroDress.Play(idleAnimationName, true);

                var action = EquipActionConfigManager.inst.GetCfg(999999);
                heroDress.Play(action.act_combat_standby_show, completeDele: (t) =>
                {
                    if (this != null)
                    {
                        string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                        heroDress.Play(idleAnimationName, true);
                    }
                });
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByHero(heroDress, (EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress));
            heroDress.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2, 0.75f);
            //string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
            //heroDress.Play(idleAnimationName, true);
            var action = EquipActionConfigManager.inst.GetCfg(999999);
            heroDress.Play(action.act_combat_standby_show, completeDele: (t) =>
            {
                if (this != null)
                {
                    string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                    heroDress.Play(idleAnimationName, true);
                }
            });
        }
    }

    private void setStarNum(int rarity)
    {
        contentPane.nickNameTx.text = LanguageManager.inst.GetValueByKey(data.nickName);
        contentPane.qualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);

        for (int i = 0; i < contentPane.allStars.Count; i++)
        {
            int index = i;
            if (index < data.config.hero_grade)
            {
                contentPane.allStars[index].gameObject.SetActive(true);
                contentPane.allStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
            }
            else
            {
                if (index < data.transferNumLimit)
                {
                    contentPane.allStars[index].gameObject.SetActive(true);
                    contentPane.allStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
                }
                else
                {
                    contentPane.allStars[index].gameObject.SetActive(false);
                }
            }
        }
    }

    private void setPropertyData()
    {
        HeroPropertyData tempAttribute = data.attributeConfig;
        contentPane.fightValTx.text = data.fightingNum.ToString("N0");
        contentPane.lifeText.text = tempAttribute.hp_basic.ToString();
        contentPane.attackText.text = tempAttribute.atk_basic.ToString();
        contentPane.armorText.text = tempAttribute.def_basic.ToString();
        contentPane.speedText.text = tempAttribute.spd_basic.ToString();
    }

    private void setEquipContent()
    {
        List<int> ids = HeroProfessionConfigManager.inst.GetAllFieldEquipId(data.id);

        int allCount = contentPane.allEquips.childCount;
        for (int i = 0; i < allCount; i++)
        {
            int index = i;
            GUIIcon curIcon = contentPane.allEquips.GetChild(index).GetComponent<GUIIcon>();
            if (index < ids.Count)
            {
                EquipClassification tempClass = EquipConfigManager.inst.GetEquipTypeByID(ids[index]);
                curIcon.gameObject.SetActive(true);
                curIcon.SetSprite(tempClass.Atlas, tempClass.icon);
            }
            else
            {
                curIcon.gameObject.SetActive(false);
            }
        }
    }

    private void setSkillData()
    {
        List<HeroSkillShowConfig> skills = data.GetAllSkillId();
        for (int i = 0; i < contentPane.allSkills.Count; i++)
        {
            int index = i;
            if (index < skills.Count)
            {
                contentPane.allSkills[index].setData(skills[index]);
            }
            else
            {
                contentPane.allSkills[index].clearData();
            }
        }
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(25);
    }

    protected override void onHide()
    {

    }
}
