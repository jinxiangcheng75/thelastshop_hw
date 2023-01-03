using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HeroFireReturnData
{
    public itemConfig itemConfig;
    public int count;
}

public class RoleHeroInfoView : ViewBase<RoleHeroInfoComp>
{
    public override string viewID => ViewPrefabName.RoleHeroInfoPanel;
    public override string sortingLayerName => "window";

    RoleHeroData data;
    ERoleState roleState;
    int timerId;
    DressUpSystem heroDress;
    public int heroUid;
    bool isFighting = false;
    GameObject lastEffectObj;
    int curHpNum = 0, curHpGeneNum = 0;
    int curAtkNum = 0, curAtkGeneNum = 0;
    int curDefNum = 0, curDefGeneNum = 0;
    int curFighting = 0;
    int curHeroUid = 0;
    List<HeroFireReturnData> heroFireReturnDatas = new List<HeroFireReturnData>();



    float[] arrowPos = new float[] { -147, 12, 173 };

    GraphicDressUpSystem graphicDressUp;
    GraphicDressUpSystem fireGraphicDressUp;
    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.noSettingAndEnergy;
        isShowResPanel = true;
        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.leftBtn.ButtonClickTween(() => setHeroInfoData(RoleDataProxy.inst.GetNearHeroData(data, true)));
        contentPane.rightBtn.ButtonClickTween(() => setHeroInfoData(RoleDataProxy.inst.GetNearHeroData(data, false)));
        contentPane.useLevelItemBtn.ButtonClickTween(() =>
        {
            if (data.currentState == 2)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("英雄探索期间不能使用经验卡"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            //heroDress.SetAnimationSpeed(0);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUSEEXPITEM_SHOWUI, data.uid);
        });
        contentPane.settingBtn.ButtonClickTween(() =>
        {
            if (data.currentState == 2)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("英雄探索期间不能设置英雄信息"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            if (contentPane.secondaryInterfaceObj.activeSelf)
                contentPane.secondaryInterfaceObj.SetActive(false);
            else
                contentPane.secondaryInterfaceObj.SetActive(true);
        });
        contentPane.settingBgBtn.onClick.AddListener(() =>
        {
            contentPane.secondaryInterfaceObj.SetActive(false);
        });
        contentPane.transferBtn.ButtonClickTween(() =>
        {
            if (data.currentState == 2)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("英雄探索期间不能转职"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            //heroDress.SetAnimationSpeed(0);
            //contentPane.toTransferObj.SetActiveFalse();
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLETRANSFER_SHOWUI, data.uid);
        });
        contentPane.allWearBtn.ButtonClickTween(equipAllWear);
        contentPane.restBtn.ButtonClickTween(() =>
        {
            //heroDress.SetAnimationSpeed(0);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.SINGLEROLERESTING_SHOWUI, data, 1);
        });
        contentPane.exploreBtn.ButtonClickTween(() =>
        {
            int slotId = ExploreDataProxy.inst.GetSlotIdByHeroUid(heroUid);
            var tempSlotData = ExploreDataProxy.inst.GetSlotDataById(slotId);
            if (slotId != -1)
            {
                if (tempSlotData.slotType == 0 || tempSlotData.slotType == 1)
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEADVENTUREBYSLOT_SHOWUI, slotId);
                else if (tempSlotData.slotType == 2)
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_RefugeAdventure", slotId);
                else if (tempSlotData.slotType == 3)
                    HotfixBridge.inst.TriggerLuaEvent("OpenUI_GoldenCityAdventure", slotId);
            }
            else
            {
                Logger.error("没有英雄uid为" + heroUid + "的副本槽位");
            }
        });
        contentPane.renameBtn.ButtonClickTween(() =>
        {
            //heroDress.SetAnimationSpeed(0);
            contentPane.secondaryInterfaceObj.SetActive(false);
            contentPane.renameObj.SetActive(true);
            contentPane.renameInput.text = LanguageManager.inst.GetValueByKey(data.nickName);
        });
        contentPane.dismissalBtn.ButtonClickTween(() =>
        {
            setFireData();
        });
        contentPane.renameCancleBtn.ButtonClickTween(closeRenamePanel);
        contentPane.dismissalCancleBtn.ButtonClickTween(() =>
        {
            closeDismissalPanel();
        });
        contentPane.renameConfirmBtn.ButtonClickTween(() =>
        {
            string curNameStr = contentPane.renameInput.text;
            if (string.IsNullOrEmpty(curNameStr))
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名字不能为空"), GUIHelper.GetColorByColorHex("FF2828"));
            else
            {
                if (WordFilter.inst.filter(contentPane.renameInput.text, out curNameStr, check_only: true))
                {
                    contentPane.renameInput.text = "";
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称中包含敏感词汇！"), GUIHelper.GetColorByColorHex("FF2828"));
                }
                else
                {
                    closeRenamePanel();
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RENAME, curNameStr, data.uid);
                }
            }
        });
        contentPane.dismissalConfirmBtn.ButtonClickTween(() =>
        {
            if (!contentPane.dismissalAgainObj.activeSelf)
            {
                contentPane.dismissalAgainObj.SetActive(true);
                //contentPane.dismissalNumText.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                int gemCost = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level).dismissal_cost;
                if (gemCost > UserDataProxy.inst.playerData.gem)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, gemCost - UserDataProxy.inst.playerData.gem);

                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_DISMISSAL, data.uid);
                }
            }
        });
        contentPane.renameCloseBtn.ButtonClickTween(() =>
        {
            closeRenamePanel();
        });
        contentPane.dismissalCloseBtn.ButtonClickTween(() =>
        {
            closeDismissalPanel();
        });
        contentPane.professionBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.SHOWUI_TRANSFERPREVIEW, data.id);
        });
        contentPane.attBtn.ButtonClickTween(() =>
        {
            if (contentPane.showCAttPanel)
            {
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.SHOWUI_ROLEHEROATTVIEW, data.attributeConfig);
            }
            else
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_RoleAtt", data.uid);
            }

        });

        contentPane.btn_transferNumLimitDes.ButtonClickTween(() =>
        {
            contentPane.rtf_transferNumLimitDesViewBg.position = contentPane.btn_transferNumLimitDes.transform.position;
            contentPane.rtf_transferNumLimitDesViewContent.position = contentPane.btn_transferNumLimitDes.transform.position;

            contentPane.rtf_transferNumLimitDesViewBg.anchoredPosition = new Vector2(contentPane.rtf_transferNumLimitDesViewBg.anchoredPosition.x + 24, contentPane.rtf_transferNumLimitDesViewBg.anchoredPosition.y + 65);
            contentPane.rtf_transferNumLimitDesViewContent.anchoredPosition = new Vector2(contentPane.rtf_transferNumLimitDesViewContent.anchoredPosition.x + 48, contentPane.rtf_transferNumLimitDesViewContent.anchoredPosition.y + 65);

            contentPane.btn_transferNumLimitDesView.gameObject.SetActive(true);
        });

        contentPane.btn_transferNumLimitDesView.onClick.AddListener(() =>
        {
            contentPane.btn_transferNumLimitDesView.gameObject.SetActive(false);
        });

        contentPane.group.OnSelectedIndexValueChange = typeSelectedChange;
        contentPane.skillGroup.OnSelectedIndexValueChange = typeSelectedChangeSkill;

        contentPane.hpAddBtn.ButtonClickTween(() => onAddPropertyBtnClick(kRolePropertyType.hp));
        contentPane.defAddBtn.ButtonClickTween(() => onAddPropertyBtnClick(kRolePropertyType.def));
        contentPane.atkAddBtn.ButtonClickTween(() => onAddPropertyBtnClick(kRolePropertyType.atk));

    }

    private void onAddPropertyBtnClick(kRolePropertyType propertyType)
    {
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_UseGeneItem", heroUid, (int)propertyType);
    }

    public void typeSelectedChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        contentPane.infoObj.SetActive(index == 0);
        contentPane.skillObj.SetActive(index == 1);
        if (index == 1)
        {
            contentPane.skillGroup.OnEnableMethod();
        }
    }

    public void typeSelectedChangeSkill(int index)
    {
        AudioManager.inst.PlaySound(8);
        contentPane.skillArrowRect.anchoredPosition = new Vector2(arrowPos[index], contentPane.skillArrowRect.anchoredPosition.y);
        HeroSkillShowConfig skillShowData = new HeroSkillShowConfig();
        if (index == 0)
        {
            skillShowData = data.skill1;
        }
        else if (index == 1)
        {
            skillShowData = data.skill2;
        }
        else if (index == 2)
        {
            skillShowData = data.skill3;
        }

        if (skillShowData == null)
        {
            contentPane.skillGroup.selectedIndex = 0;
            return;
        }

        contentPane.skillNameText.text = LanguageManager.inst.GetValueByKey(skillShowData.skill_name);
        contentPane.skillDescText.text = LanguageManager.inst.GetValueByKey(skillShowData.skill_dec);
    }

    public override void shiftIn()
    {
        base.shiftIn();
        //if (data != null)
        //{
        //    if (RoleDataProxy.inst.GetHeroDataByUid(data.uid) != null)
        //    {
        //        setHeroInfoData(data.uid);
        //    }
        //}
    }

    public override void shiftOut()
    {

    }

    private void setFireData()
    {
        //heroDress.SetAnimationSpeed(0);
        contentPane.secondaryInterfaceObj.SetActive(false);
        contentPane.dismissalObj.SetActive(true);
        contentPane.dismissalNameText.text = LanguageManager.inst.GetValueByKey("确认要解雇{0}吗？", LanguageManager.inst.GetValueByKey(data.nickName));
        //LanguageManager.inst.GetValueByKey("确认要解雇") + data.nickName + LanguageManager.inst.GetValueByKey("吗？");
        contentPane.dismissalNumText.text = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level).dismissal_cost.ToString();
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        contentPane.fireLvText.text = data.level.ToString();

        contentPane.fireBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        contentPane.fireIntelligenceIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);
        contentPane.fireIntelligenceText.text = data.intelligence.ToString();

        contentPane.fireTypeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        contentPane.fireTypeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);

        setFireHeroHeadIcon();
        setFireAllStarsData();
        setFireReturnData();
    }

    void setFireExpCardData()
    {
        long sumExp = heroupgradeconfigManager.inst.GetSumExp(data.level, RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence));
        sumExp += data.exp;
        List<Item> itemCfgs = ItemBagProxy.inst.GetItemsByType(ItemType.HeroExp);
        heroFireReturnDatas = new List<HeroFireReturnData>();

        for (int i = itemCfgs.Count - 1; i >= 0; i--)
        {
            int index = i;
            var curItemCfg = itemCfgs[index].itemConfig;
            int returnCardNum = 0;
            if (sumExp >= curItemCfg.effect)
            {
                returnCardNum = Mathf.FloorToInt(sumExp / curItemCfg.effect);
                sumExp -= returnCardNum * curItemCfg.effect;
            }

            if (returnCardNum > 0)
            {
                heroFireReturnDatas.Add(new HeroFireReturnData() { itemConfig = curItemCfg, count = returnCardNum });
            }
        }
        //var item1 = ItemconfigManager.inst.GetConfig(150003);
        //var item2 = ItemconfigManager.inst.GetConfig(150004);
        //var item3 = ItemconfigManager.inst.GetConfig(150005);

        //int card1 = 0;
        //int card2 = 0;
        //int card3 = 0;

        //if (sumExp >= item3.effect)
        //{
        //    card3 = Mathf.FloorToInt(sumExp / item3.effect);
        //    sumExp -= card3 * item3.effect;
        //}

        //if (sumExp >= item2.effect)
        //{
        //    card2 = Mathf.FloorToInt(sumExp / item2.effect);
        //    sumExp -= card2 * item2.effect;
        //}

        //if (sumExp >= item1.effect)
        //{
        //    card1 = Mathf.FloorToInt(sumExp / item1.effect);
        //}

        //heroFireReturnDatas.Add(new HeroFireReturnData() { itemConfig = item1, count = card1 });
        //heroFireReturnDatas.Add(new HeroFireReturnData() { itemConfig = item2, count = card2 });
        //heroFireReturnDatas.Add(new HeroFireReturnData() { itemConfig = item3, count = card3 });

    }

    void setFireCertificateAndTransferMatsData()
    {

        if (data.config.hero_grade == 1 || data.config.pre_profession == 0) //无前置职业id或职业星数为1 无需返回的职业材料
        {
            return;
        }
        else
        {

            List<HeroProfessionConfigData> heroProfessionConfigDatas = new List<HeroProfessionConfigData>();

            HeroProfessionConfigData professiconCfg = data.config;

            while (professiconCfg.hero_grade > 1 && professiconCfg.pre_profession != 0)
            {
                heroProfessionConfigDatas.Add(professiconCfg);
                professiconCfg = HeroProfessionConfigManager.inst.GetConfig(professiconCfg.pre_profession);
            }

            for (int i = heroProfessionConfigDatas.Count - 1; i >= 0; i--)
            {
                professiconCfg = heroProfessionConfigDatas[i];

                var list = professiconCfg.GetHeroProfessionNeedMatDatas();

                for (int k = 0; k < list.Count; k++)
                {
                    HeroProfessionNeedMatData heroProfessionNeedMatData = list[k];
                    HeroFireReturnData returnData = heroFireReturnDatas.Find(t => t.itemConfig.id == heroProfessionNeedMatData.itemId);
                    if (returnData == null)
                    {
                        heroFireReturnDatas.Add(new HeroFireReturnData() { itemConfig = ItemconfigManager.inst.GetConfig(heroProfessionNeedMatData.itemId), count = heroProfessionNeedMatData.needItemCount });
                    }
                    else
                    {
                        returnData.count += heroProfessionNeedMatData.needItemCount;
                    }
                }
            }

        }
    }

    private void setPropertyAddData()
    {
        List<Item> itemCfgs = ItemBagProxy.inst.GetItemsByType(ItemType.HeroPropertyUp);
        foreach (var item in itemCfgs)
        {
            var geneItemNum = data.GetGeneItemNum(item.itemConfig.effect);
            if (geneItemNum > 0)
            {
                heroFireReturnDatas.Add(new HeroFireReturnData() { itemConfig = item.itemConfig, count = geneItemNum });
            }
        }
    }

    private void setFireReturnData()
    {
        heroFireReturnDatas.Clear();
        setFireExpCardData();//经验卡
        setFireCertificateAndTransferMatsData();//凭证 和 转职证书
        setPropertyAddData();//属性基因药水

        if (heroFireReturnDatas.Count <= 0)
        {
            contentPane.fireReturnObj.SetActive(false);
            contentPane.firePromptText.SetActive(false);
        }
        else
        {
            for (int i = 0; i < contentPane.fireAllReturnIcons.Count; i++)
            {
                GUIIcon icon = contentPane.fireAllReturnIcons[i];
                var tx_count = contentPane.fireAllReturnText[i];

                if (i < heroFireReturnDatas.Count)
                {
                    HeroFireReturnData returnData = heroFireReturnDatas[i];
                    if (returnData.count <= 0)
                    {
                        icon.gameObject.SetActive(false);
                        continue;
                    }
                    icon.gameObject.SetActive(true);
                    icon.SetSprite(returnData.itemConfig.atlas, returnData.itemConfig.icon);
                    tx_count.text = returnData.count.ToString();
                }
                else
                {
                    icon.gameObject.SetActive(false);
                }
            }
            contentPane.fireLineSecondObj.SetActive(heroFireReturnDatas.Count > 6);

            contentPane.fireReturnObj.SetActive(true);
            contentPane.firePromptText.SetActive(true);
        }

    }

    private void setFireAllStarsData()
    {
        for (int i = 0; i < contentPane.fireAllStars.Count; i++)
        {
            int index = i;
            if (index < data.config.hero_grade)
            {
                contentPane.fireAllStars[index].gameObject.SetActive(true);
                contentPane.fireAllStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
            }
            else
            {
                if (index < data.transferNumLimit)
                {
                    contentPane.fireAllStars[index].gameObject.SetActive(true);
                    contentPane.fireAllStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
                }
                else
                {
                    contentPane.fireAllStars[index].gameObject.SetActive(false);
                }
            }
        }
    }

    private void setFireHeroHeadIcon()
    {
        if (fireGraphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender, callback: system =>
            {
                fireGraphicDressUp = system;
                system.transform.SetParent(contentPane.fireRoleDressTrans);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(fireGraphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender);
        }
    }

    //一键换装
    private void equipAllWear()
    {

        if (data == null) return;
        if (data.currentState == 2)  //在战斗呢~
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("探索中不可切换装备"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }


        List<HeroChangeEquipData> list = RoleDataProxy.inst.GetHeroBestEquips(data.uid);

        List<HeroEquipAuto> eqiupList = new List<HeroEquipAuto>();

        foreach (var item in list)
        {
            eqiupList.Add(new HeroEquipAuto() { equipPosId = item.equipField, equipId = item.equipUid });
        }

        if (list.Count > 0)
        {
            AudioManager.inst.PlaySound(129);
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HEROWEARALLEQUIP, data.uid, eqiupList);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("没有可以替换的更高级装备"), GUIHelper.GetColorByColorHex("FFD907"));
        }

        //foreach (HeroChangeEquipData item in list)
        //{
        //    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_WEAREQUIP, item.heroUid, item.equipField, 0, item.equipUid);
        //}
    }

    private void closeRenamePanel()
    {
        //heroDress.SetAnimationSpeed(1);
        contentPane.renameInput.text = "";
        contentPane.renameObj.SetActive(false);
    }

    private void closeDismissalPanel()
    {
        //heroDress.SetAnimationSpeed(1);
        contentPane.dismissalObj.SetActive(false);
        contentPane.dismissalAgainObj.SetActive(false);
        //contentPane.dismissalNumText.gameObject.SetActive(true);
    }

    public void setHeroInfoData(int heroUid)
    {
        this.heroUid = heroUid;

        contentPane.toTransferObj.SetActiveTrue();

        data = RoleDataProxy.inst.GetHeroDataByUid(heroUid);

        contentPane.group.OnEnableMethod(contentPane.group.selectedIndex);

        if (data.hasRedPoint == 1)
        {
            data.hasRedPoint = 2;
        }

        contentPane.skillGroup.togglesBtn[0].interactable = data.skill1 != null;
        contentPane.skillGroup.togglesBtn[1].interactable = data.skill2 != null;
        contentPane.skillGroup.togglesBtn[2].interactable = data.skill3 != null;
        //contentPane.settingBtn.interactable = data.currentState != 2;
        //contentPane.transferBtn.interactable = data.currentState != 2;
        GUIHelper.SetUIGray(contentPane.settingBtn.transform, data.currentState == 2);
        //contentPane.useLevelItemBtn.interactable = data.currentState != 2;
        GUIHelper.SetUIGray(contentPane.useLevelItemBtn.transform, data.currentState == 2);
        var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level + 1);
        if (upgradeCfg != null)
        {
            contentPane.useLevelItemBtn.gameObject.SetActive(true);
        }
        else
        {
            contentPane.useLevelItemBtn.gameObject.SetActive(false);
        }
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        contentPane.intelligenceBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        roleState = (ERoleState)data.currentState;
        string qualityColor = StaticConstants.roleIntelligenceColor[rarity];
        contentPane.typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        contentPane.typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        contentPane.intelligenceText.text = data.intelligence.ToString();
        contentPane.intelligenceText.color = GUIHelper.GetColorByColorHex(qualityColor);
        contentPane.newintelligenceText.text = StaticConstants.roleIntelligenceStr[rarity - 1];
        contentPane.newintelligenceText.color = GUIHelper.GetColorByColorHex(qualityColor);

        contentPane.professionNameTx.text = LanguageManager.inst.GetValueByKey(data.config.name);

        //string talentColor = StaticConstants.roleIntelligenceColor[data.talentConfig.quality - 1];
        //contentPane.talentText.color = GUIHelper.GetColorByColorHex(talentColor);
        contentPane.talentNameText.text = LanguageManager.inst.GetValueByKey(data.talentConfig.name);
        contentPane.talentNameText.color = GUIHelper.GetColorByColorHex(StaticConstants.roleIntelligenceColor[data.talentConfig.quality - 1]);

        contentPane.talentQualityStr.text = LanguageManager.inst.GetValueByKey(StaticConstants.roleTalentQualityStr[data.talentConfig.quality - 2]) + LanguageManager.inst.GetValueByKey("天赋");

        var talentBySkillCfg = HeroSkillShowConfigManager.inst.GetConfig(data.talentConfig.skill_id);
        contentPane.talentXDescText.text = LanguageManager.inst.GetValueByKey(talentBySkillCfg.skill_dec);

        contentPane.talentDescText.gameObject.SetActive(!string.IsNullOrEmpty(data.talentConfig.easy_txt_1));
        contentPane.talentDescText2.gameObject.SetActive(!string.IsNullOrEmpty(data.talentConfig.easy_txt_2));
        contentPane.talentDescText3.gameObject.SetActive(!string.IsNullOrEmpty(data.talentConfig.easy_txt_3));
        contentPane.talentDescText4.gameObject.SetActive(!string.IsNullOrEmpty(data.talentConfig.easy_txt_4));

        if (!string.IsNullOrEmpty(data.talentConfig.easy_txt_1))
            contentPane.talentDescText.text = LanguageManager.inst.GetValueByKey(data.talentConfig.easy_txt_1);
        if (!string.IsNullOrEmpty(data.talentConfig.easy_txt_2))
            contentPane.talentDescText2.text = LanguageManager.inst.GetValueByKey(data.talentConfig.easy_txt_2);
        if (!string.IsNullOrEmpty(data.talentConfig.easy_txt_3))
            contentPane.talentDescText3.text = LanguageManager.inst.GetValueByKey(data.talentConfig.easy_txt_3);
        if (!string.IsNullOrEmpty(data.talentConfig.easy_txt_4))
            contentPane.talentDescText4.text = LanguageManager.inst.GetValueByKey(data.talentConfig.easy_txt_4);

        contentPane.talentQualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleTalentBgIconName[data.talentConfig.quality - 2]);
        var cfg = HeroSkillShowConfigManager.inst.GetConfig(data.talentConfig.skill_id);
        contentPane.talentIcon.SetSprite(cfg.skill_atlas, cfg.skill_icon);
        contentPane.bgCanvas.sortingOrder = _uiCanvas.sortingOrder - 2;

        contentPane.qualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);

        setHeroHeadIcon();
        setAttributeData();
        setEffectInfo();
        setHeroExpInfo();
        JudgeCanTransfer();
        SetHeroPrefabData();
        setStarNum();
        setEquipData();
        setSkillData();
        setHeroState();
    }

    public void RoleEquipAutoEnd(int heroUid)
    {
        setHeroInfoData(heroUid);
        //todo 音效
        heroDress?.Play("happy", completeDele: (t) =>
          {
              if (this != null)
              {
                  string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                  heroDress.Play(idleAnimationName, true);
              }
          });
    }


    private void setAttributeData()
    {
        if (curHeroUid != data.uid || (curHpNum == 0 && curAtkNum == 0 && curDefNum == 0 && curFighting == 0))
        {
            curHpNum = data.attributeConfig.hp_basic;
            curHpGeneNum = data.GetGeneProperty((int)kRolePropertyType.hp);
            curAtkNum = data.attributeConfig.atk_basic;
            curAtkGeneNum = data.GetGeneProperty((int)kRolePropertyType.atk);
            curDefNum = data.attributeConfig.def_basic;
            curDefGeneNum = data.GetGeneProperty((int)kRolePropertyType.def);
            curFighting = data.fightingNum;
            curHeroUid = data.uid;

            contentPane.lifeTx.text = data.attributeConfig.hp_basic + (data.hpAdd > 0 ? "<color=#58ff68>(+" + curHpGeneNum + ")</color>" : "");
            contentPane.atkTx.text = data.attributeConfig.atk_basic + (data.atkAdd > 0 ? "<color=#58ff68>(+" + curAtkGeneNum + ")</color>" : "");
            contentPane.armorTx.text = data.attributeConfig.def_basic + (data.defAdd > 0 ? "<color=#58ff68>(+" + curDefGeneNum + ")</color>" : "");

            contentPane.fightingText.text = data.fightingNum.ToString();
        }
        else
        {
            //isNext = true;
            if (curHpNum != data.attributeConfig.hp_basic || curHpGeneNum != data.GetGeneProperty((int)kRolePropertyType.hp))
            {
                setNumChangeAnim(contentPane.hpChangeIcon, contentPane.hpChangeText, contentPane.lifeTx, curHpNum, data.attributeConfig.hp_basic, curHpGeneNum, data.GetGeneProperty((int)kRolePropertyType.hp));
                curHpNum = data.attributeConfig.hp_basic;
                curHpGeneNum = data.GetGeneProperty((int)kRolePropertyType.hp);
            }
            else
            {
                contentPane.lifeTx.text = data.attributeConfig.hp_basic + (data.hpAdd > 0 ? "<color=#58ff68>(+" + curHpGeneNum + ")</color>" : "");
            }
            if (curAtkNum != data.attributeConfig.atk_basic || curAtkGeneNum != data.GetGeneProperty((int)kRolePropertyType.atk))
            {
                setNumChangeAnim(contentPane.atkChangeIcon, contentPane.atkChangeText, contentPane.atkTx, curAtkNum, data.attributeConfig.atk_basic, curAtkGeneNum, data.GetGeneProperty((int)kRolePropertyType.atk));
                curAtkNum = data.attributeConfig.atk_basic;
                curAtkGeneNum = data.GetGeneProperty((int)kRolePropertyType.atk);
            }
            else
            {
                contentPane.atkTx.text = data.attributeConfig.atk_basic + (data.atkAdd > 0 ? "<color=#58ff68>(+" + curAtkGeneNum + ")</color>" : "");
            }
            if (curDefNum != data.attributeConfig.def_basic || curDefGeneNum != data.GetGeneProperty((int)kRolePropertyType.def))
            {
                setNumChangeAnim(contentPane.defChangeIcon, contentPane.defChangeText, contentPane.armorTx, curDefNum, data.attributeConfig.def_basic, curDefGeneNum, data.GetGeneProperty((int)kRolePropertyType.def));
                curDefNum = data.attributeConfig.def_basic;
                curDefGeneNum = data.GetGeneProperty((int)kRolePropertyType.def);
            }
            else
            {
                contentPane.armorTx.text = data.attributeConfig.def_basic + (data.defAdd > 0 ? "<color=#58ff68>(+" + curDefGeneNum + ")</color>" : "");
            }
            if (curFighting != data.fightingNum)
            {
                setNumChangeAnim(contentPane.fightingChangeIcon, contentPane.fightingChangeText, contentPane.fightingText, curFighting, data.fightingNum);
                curFighting = data.fightingNum;
            }
            else
            {
                contentPane.fightingText.text = data.fightingNum.ToString();
            }
            isNext = true;
        }
    }

    Tween moveAnchorYTween;
    Tween changeTextTween;
    Tween iconFadeToZeroTween;
    Tween textFadeToZeroTween;
    bool isNext;
    // lastNum - 变化前的值 curNum - 变化后的值（从lastNum -> curNum）
    private void setNumChangeAnim(GUIIcon targetIcon, UnityEngine.UI.Text targetText, UnityEngine.UI.Text uiText, int lastNum, int curNum, int geneLastNum = 0, int geneCurNum = 0)
    {
        var targetRect = targetIcon.GetComponent<RectTransform>();
        DOTween.Kill(targetRect);
        DOTween.Kill(targetIcon.iconImage);
        DOTween.Kill(targetText);
        //if (isNext)
        //{
        //    //moveAnchorYTween.Kill();
        //    //changeTextTween.Kill();
        //    //iconFadeToZeroTween.Kill();
        //    //textFadeToZeroTween.Kill();

        //    isNext = false;
        //}

        bool isUp = lastNum == curNum ? geneLastNum < geneCurNum : lastNum < curNum;

        bool geneChanged = geneLastNum != geneCurNum;

        string iconName = isUp ? "yingxiong_iconxiaojtl" : "yingxiong_iconxiaojth";
        targetIcon.SetSprite(StaticConstants.roleAtlasName, iconName);
        float fromPos = isUp ? -80 : 80;
        float toPos = 10;


        targetText.text = Mathf.Abs(curNum + geneCurNum - lastNum - geneLastNum).ToString();
        moveAnchorYTween = targetRect.DOAnchorPos3DY(toPos, 0.6f).From(fromPos).OnComplete(() =>
         {
             if (geneChanged) DOTween.To(() => geneLastNum, x => geneLastNum = x, geneCurNum, 0.5f);

             changeTextTween = DOTween.To(() => lastNum, x => lastNum = x, curNum, 0.5f).OnUpdate(() =>
             {
                 uiText.text = lastNum.ToString() + (geneChanged ? "<color=#58ff68>(+" + geneLastNum + ")</color>" : geneCurNum == 0 ? "" : "<color=#58ff68>(+" + geneCurNum + ")</color>");
             }).OnComplete(() =>
             {
                 //DoTweenUtil.Fade_a_To_0_All(targetIcon.transform, 1, 0.5f, false);
                 iconFadeToZeroTween = targetIcon.iconImage.DOFade(0, 0.5f);
                 textFadeToZeroTween = targetText.DOFade(0, 0.5f);
             });
         }).OnStart(() =>
         {
             targetIcon.iconImage.color = new Color(1, 1, 1, 1);
             targetText.color = new Color(1, 1, 1, 1);
         });
    }

    private void setHeroHeadIcon()
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender, callback: system =>
            {
                graphicDressUp = system;
                system.transform.SetParent(contentPane.headParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender);
        }
    }

    private void setHeroExpInfo()
    {
        if (data == null) return;
        contentPane.nickNameTx.text = LanguageManager.inst.GetValueByKey(data.nickName);
        contentPane.levelText.text = data.level.ToString();
        heroupgradeconfig upgradeConfig = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level + 1);
        heroupgradeconfig curLevelConfig = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level);
        if (upgradeConfig != null)
        {
            var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);
            if (cityData != null && data.level >= cityData.effectVal)
            {
                contentPane.limitImg.enabled = true;
                var cityCfg = BuildingUpgradeConfigManager.inst.GetConfig(StaticConstants.heroLvLimitHouseID, cityData.upgradeConfig.architecture_lv + 1);
                if (cityCfg != null)
                    contentPane.scheduleText.text = LanguageManager.inst.GetValueByKey("需要{0}级{1}", (cityData.upgradeConfig.architecture_lv + 1).ToString(), LanguageManager.inst.GetValueByKey(cityData.upgradeConfig.name));
                else
                    contentPane.scheduleText.text = LanguageManager.inst.GetValueByKey("已达等级上限");
                contentPane.levelSlider.maxValue = 1;
                contentPane.levelSlider.value = 1;
            }
            else
            {
                contentPane.limitImg.enabled = false;
                contentPane.scheduleText.text = data.exp + "/" + upgradeConfig.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence));
                contentPane.levelSlider.maxValue = upgradeConfig.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence));
                contentPane.levelSlider.value = Mathf.Max(contentPane.levelSlider.maxValue * 0.05f, data.exp);
            }
        }
        else
        {
            contentPane.limitImg.enabled = false;
            contentPane.levelSlider.maxValue = 1;
            contentPane.levelSlider.value = 1;
            contentPane.scheduleText.text = "max";
        }
        contentPane.canWearEquipClassText.text = LanguageManager.inst.GetValueByKey("可穿戴装备等阶：");
        if (curLevelConfig != null)
            contentPane.equipNumText.text = curLevelConfig.equip_lv.ToString();
    }

    private void setEffectInfo()
    {
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        if (lastEffectObj != null)
        {
            lastEffectObj.SetActive(false);
        }
        lastEffectObj = contentPane.QualityFxList[rarity - 1];
        lastEffectObj.SetActive(true);

        //var fxList = lastEffectObj.GetComponentsInChildren<ParticleSystem>(true);
        //foreach (var item in fxList)
        //{
        //    item.Play();
        //    item.GetComponent<Renderer>().sortingOrder = _uiCanvas.sortingOrder - 2;
        //}
    }

    private void JudgeCanTransfer()
    {
        List<HeroProfessionConfigData> canTransferList = HeroProfessionConfigManager.inst.GetTransferData(data.id, data.intelligence);
        contentPane.transferBtn.gameObject.SetActive(canTransferList.Count > 0);
        contentPane.topObj.SetActive(canTransferList.Count <= 0);
        if (canTransferList.Count > 0)
        {
            bool isArrive = false;
            for (int i = 0; i < canTransferList.Count; i++)
            {
                int index = i;
                if (data.level >= canTransferList[index].level_need)
                {
                    isArrive = true;
                    GUIHelper.SetUIGray(contentPane.transferBtn.transform, false);
                    //contentPane.transferBtn.interactable = true;
                    //contentPane.hintInfoText.gameObject.SetActive(false);

                    var needMatList = canTransferList[index].GetHeroProfessionNeedMatDatas();
                    for (int k = 0; k < needMatList.Count; k++)
                    {
                        Item voucherItem = ItemBagProxy.inst.GetItem(needMatList[k].itemId);
                        if (voucherItem != null)
                        {
                            if (voucherItem.count < needMatList[k].needItemCount)
                            {
                                isArrive = false;
                                break;
                            }
                        }
                        else
                        {
                            isArrive = false;
                            break;
                        }
                    }

                    if (isArrive)
                    {
                        contentPane.transferRedPoint.enabled = true;
                        break;
                    }
                    else
                        contentPane.transferRedPoint.enabled = false;
                    //Item voucherItem = ItemBagProxy.inst.GetItem(canTransferList[index].cost_item1_id);
                    //Item certificateItem = ItemBagProxy.inst.GetItem(canTransferList[index].cost_item2_id);
                    //if (voucherItem.count >= canTransferList[index].cost_item1_num && certificateItem.count >= canTransferList[index].cost_item2_num)
                    //{
                    //    contentPane.transferRedPoint.enabled = true;
                    //    break;
                    //}
                    //else
                    //{
                    //    contentPane.transferRedPoint.enabled = false;
                    //}
                }
            }

            if (!isArrive)
            {
                contentPane.transferRedPoint.enabled = false;
                //contentPane.transferBtn.interactable = false;
                //GUIHelper.SetUIGray(contentPane.transferBtn.transform, true);
                //contentPane.hintInfoText.enabled = true;
                //contentPane.hintInfoText.text = LanguageManager.inst.GetValueByKey("需要{0}级可转职", canTransferList[0].level_need.ToString());
            }

            contentPane.transferText.text = LanguageManager.inst.GetValueByKey(data.config.hero_grade == 1 ? "转职" : "进阶");

        }
        else
        {
            contentPane.transferRedPoint.enabled = false;
            //GUIHelper.SetUIGray(contentPane.transferBtn.transform, true);
            //contentPane.transferBtn.interactable = false;
            //contentPane.hintInfoText.gameObject.SetActive(true);
            //contentPane.hintInfoText.text = LanguageManager.inst.GetValueByKey("没有可转职的职业了");
        }
    }

    void heroShowAnim()
    {
        if (data.equip1.equipId != 0)
        {
            EquipConfig equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(data.equip1.equipId);

            if (equipCfg != null)
            {
                var action = EquipActionConfigManager.inst.GetCfg(equipCfg.equipDrawingId);

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
        else
        {
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

    private void SetHeroPrefabData()
    {
        if (heroDress == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress), callback: (dress) =>
            {
                heroDress = dress;
                heroDress.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1, 0.875f);
                heroShowAnim();
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByHero(heroDress, (EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress));
            heroDress.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1, 0.875f);
            heroShowAnim();
        }
    }

    private void setStarNum()
    {
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

    private void setEquipData()
    {
        Dictionary<int, List<int>> equipDic = new Dictionary<int, List<int>>();
        equipDic = HeroProfessionConfigManager.inst.GetEquipDic(data.id);
        int canClick = data.currentState;
        List<HeroChangeEquipData> list = RoleDataProxy.inst.GetHeroBestEquips(data.uid);
        for (int i = 0; i < contentPane.allEquips.Count; i++)
        {
            int index = i;
            if (index < equipDic.Count)
            {
                contentPane.allEquips[index].gameObject.SetActive(true);
                HeroEquip tempEquip = data.GetEquipByField(index + 1);

                if (tempEquip.equipId == 0)
                {
                    int maxLevel = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level).equip_lv;
                    bool hasEquip = ItemBagProxy.inst.GetEquipItemsByHero(equipDic[index].ToArray(), maxLevel).Count > 0;
                    contentPane.allEquips[index].setNotWearEquipData(equipDic[index], data.uid, index + 1, canClick, hasEquip);
                }
                else
                {
                    var hasBest = list.Find(t => t.equipField == i + 1) != null;
                    contentPane.allEquips[index].setWearEquipData(tempEquip, data.uid, index + 1, equipDic[index], canClick, hasBest);
                }

                //contentPane.allEquips[index].clickHandler = (() =>
                //{
                //    heroDress.SetAnimationSpeed(0);
                //});
            }
            else
            {
                contentPane.allEquips[index].setLockData();
            }
        }
    }

    private void setSkillData()
    {
        List<HeroSkillShowConfig> tempSkillList = data.GetAllSkillId();
        //contentPane.skillTipObj.SetActiveTrue();
        //if (tempSkillList.Count <= 1)
        //    contentPane.skillTipObj.transform.SetParent(contentPane.tipTrans1, false);
        //else if (tempSkillList.Count <= 2)
        //    contentPane.skillTipObj.transform.SetParent(contentPane.tipTrans2, false);
        //else
        //    contentPane.skillTipObj.SetActiveFalse();
        for (int i = 0; i < contentPane.allSkills.Count; i++)
        {
            int index = i;
            if (index < tempSkillList.Count)
            {
                contentPane.allSkills[index].setData(tempSkillList[index]);
            }
            else
                contentPane.allSkills[index].clearData();
        }
    }

    private void setHeroState()
    {
        contentPane.restObj.SetActive(roleState == ERoleState.Resting);
        contentPane.exploreObj.SetActive(roleState == ERoleState.Fighting);
        contentPane.idleObj.SetActive(roleState == ERoleState.Idle);
        isFighting = false;
        switch (roleState)
        {
            case ERoleState.Idle:
                break;
            case ERoleState.Resting:
                contentPane.tempText = contentPane.restingTimeText;
                break;
            case ERoleState.Fighting:
                isFighting = true;
                contentPane.tempText = contentPane.exploreTimeText;
                break;
            default:
                break;
        }

        if (timerId == 0 && roleState != ERoleState.Idle)
        {
            if (data.remainTime > 0)
                contentPane.tempText.text = TimeUtils.timeSpanStrip(data.remainTime);
            else
            {
                contentPane.tempText.text = isFighting ? LanguageManager.inst.GetValueByKey("完成") : "1" + LanguageManager.inst.GetValueByKey("秒");
            }
            timerId = GameTimer.inst.AddTimer(1, RefreshContent);
        }
    }

    private void RefreshContent()
    {
        if (data.remainTime <= 0)
        {
            contentPane.tempText.text = isFighting ? LanguageManager.inst.GetValueByKey("完成") : "1" + LanguageManager.inst.GetValueByKey("秒");
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        else
        {
            contentPane.tempText.text = TimeUtils.timeSpanStrip(data.remainTime);
        }
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
        contentPane.renameObj.SetActive(false);
        contentPane.dismissalObj.SetActive(false);
        contentPane.secondaryInterfaceObj.SetActive(false);
        contentPane.dismissalNumText.transform.parent.gameObject.SetActive(true);
        contentPane.dismissalAgainObj.SetActive(false);
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
