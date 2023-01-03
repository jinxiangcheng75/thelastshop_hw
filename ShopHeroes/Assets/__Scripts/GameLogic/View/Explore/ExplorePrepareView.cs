using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplorePrepareView : ViewBase<ExplorePrepareCom>
{
    public override string viewID => ViewPrefabName.ExplorePrepareUI;
    public override string sortingLayerName => "window";

    ExploreGroup data;
    int curIndex;
    int curDiff;
    //int fightingSum = 0;
    int suggestFight = 0;
    bool isAuto;
    float itemAddPercent = 1;
    float itemAddExpPercent = 1;
    float itemAttPercent = -1;
    float itemTimePercent = 1;
    bool isFirst = true;
    int sumNum = 0;
    //List<int> heroUids;
    itemConfig selectItem;
    ExploreInstanceConfigData curExploreCfg;
    int teamFightSum = 0;
    OneTalentDataBase selectTalent;
    //Tween tween;
    protected override void onInit()
    {
        base.onInit();

        AddUIEvent();
        InitComponent();
        setHeroDatas();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });
        contentPane.leftBtn.ButtonClickTween(() =>
        {
            curDiff--;
            if (curDiff <= 0)
                curDiff = 1;
            setDifficultyData(curDiff);
        });
        contentPane.rightBtn.ButtonClickTween(() =>
        {
            curDiff++;
            if (curDiff > 3)
                curDiff = 3;
            setDifficultyData(curDiff);
        });
        contentPane.useItemBtn.ButtonClickTween(() =>
        {
            if (contentPane.useObj.activeSelf)
            {
                SwitchItemTypeAdd(false);
                contentPane.useObj.SetActive(false);
                contentPane.notUseObj.SetActive(true);
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("{0}已移除", LanguageManager.inst.GetValueByKey(selectItem.name)), GUIHelper.GetColorByColorHex("FFEA00"));
                contentPane.useItemNumObj.SetActive(true);
                selectItem = null;
                contentPane.showItemObj.gameObject.SetActive(false);
            }
            else
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUSEITEM_SHOWUI, 6);
        });
        contentPane.exploreBtn.ButtonClickTween(() =>
        {
            clickExploreStartBtn();
        });
        contentPane.nextBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREINFO_SHOWUI, data.groupData.groupId);
        });
        contentPane.infoBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREINFO_SHOWUI, data.groupData.groupId);
        });
        contentPane.showItemObj.ButtonClickTween(() =>
        {
            if (selectItem == null) return;
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, new CommonRewardData(selectItem.id, 1, 1, selectItem.type), contentPane.showItemObj.transform);
        });
        contentPane.showTalentObj.onClick.AddListener(() =>
        {
            if (selectTalent == null) return;
            contentPane.talentTipObj.gameObject.SetActive(true);
            contentPane.talentDescText.text = LanguageManager.inst.GetValueByKey(HeroBuffConfigManager.inst.GetConfig(selectTalent.type).name, selectTalent.value.ToString());
            contentPane.talentNameText.text = LanguageManager.inst.GetValueByKey(selectTalent.talentDesc);
        });
        contentPane.talentTipObj.onClick.AddListener(() =>
        {
            contentPane.talentTipObj.gameObject.SetActive(false);
        });
    }

    private void clickExploreStartBtn()
    {
        var slotData = ExploreDataProxy.inst.GetFreeSlotData();
        if (slotData == null)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("无可用探索栏位"), Color.white);
            return;
        }
        else
        {
            ExploreDataProxy.inst.curSlotId = slotData.slotId;
        }
        List<int> combatHeroList = new List<int>();
        int sum = 0;
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            if (contentPane.allHeroes[i].Data != null)
            {
                combatHeroList.Add(contentPane.allHeroes[i].Data.uid);
                sum++;
            }
            else
            {
                combatHeroList.Add(contentPane.allHeroes[i].indexState);
            }
        }

        if (sum <= 0)
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("请先选择英雄参战"), GUIHelper.GetColorByColorHex("FFFFFF"));
        else
        {
            contentPane.leftBtn.interactable = false;
            contentPane.rightBtn.interactable = false;
            for (int i = 0; i < contentPane.allHeroes.Count; i++)
            {
                int index = i;
                if (contentPane.allHeroes[index].Data != null)
                {
                    if (contentPane.allHeroes[index].Data.equip1.equipId != 0)
                    {
                        EquipConfig equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(contentPane.allHeroes[index].Data.equip1.equipId);

                        if (equipCfg != null)
                        {
                            var action = EquipActionConfigManager.inst.GetCfg(equipCfg.equipDrawingId);

                            contentPane.allHeroes[index].heroDress.Play(action.act_skill, completeDele: (t) =>
                            {
                                if (this != null)
                                {
                                    contentPane.allHeroes[index].heroDress.Play(action.act_combat_standby, true);
                                }
                            });
                        }
                    }
                    else
                    {
                        var action = EquipActionConfigManager.inst.GetCfg(999999);
                        contentPane.allHeroes[index].heroDress.Play(action.act_skill, completeDele: (t) =>
                        {
                            if (this != null)
                            {
                                contentPane.allHeroes[index].heroDress.Play(action.act_combat_standby, true);
                            }
                        });
                    }
                    // contentPane.allHeroes[i].heroDress.Play("happy");
                }
            }

            AudioManager.inst.PlaySound(149);
            //contentPane.maskBgCanvas.gameObject.SetActive(true);
            FGUI.inst.showGlobalMask(1.5f);
            GameTimer.inst.AddTimer(1.5f, 1, () =>
            {
                //contentPane.maskBgCanvas.gameObject.SetActive(false);
                ExploreSlot temp = new ExploreSlot();
                temp.slotId = ExploreDataProxy.inst.curSlotId;
                temp.exploreId = curExploreCfg.id;
                temp.exploreType = curIndex + 1;
                temp.heroInfoUIds = combatHeroList;
                int itemId = selectItem == null ? 0 : selectItem.id;
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLORESTART, temp, itemId);
            });
        }
    }

    private void InitComponent()
    {
        //heroUids = new List<int>();
        //contentPane.group.SetToggleSize(new Vector2(232, 120), new Vector2(232, 100));
        //contentPane.group.OnSelectedIndexValueChange = typeSelectedChange;
        //contentPane.heroCanvas.sortingLayerName = "window";
    }

    public void SortAllHeroList()
    {
        contentPane.allHeroes.Sort((hero1, hero2) => hero1.index.CompareTo(hero2.index));
        contentPane.talentTipObj.gameObject.SetActive(false);

        RefreshShowTalent();
    }

    private void typeSelectedChange(int index)
    {
        curIndex = index;
        curDiff = 1;
        setDifficultyData(curDiff);
    }

    public void setData(int group, int clickIndex)
    {
        //contentPane.maskObj.SetActive(false);
        contentPane.leftBtn.interactable = true;
        contentPane.rightBtn.interactable = true;
        selectItem = null;
        data = ExploreDataProxy.inst.GetGroupDataByGroupId(group);

        setNextLevelData(group);

        typeSelectedChange(clickIndex);

        //ToggleItemData();
        //if (contentPane.group.selectedIndex == clickIndex)
        //{
        //    typeSelectedChange(clickIndex);
        //}
        //else
        //    contentPane.group.selectedIndex = clickIndex;
        //typeSelectedChange(curIndex);

        int allItemCount = ItemBagProxy.inst.GetItemsByTypes(new ItemType[] { ItemType.ExploreTimeItem, ItemType.ExploreAddYieldItem, ItemType.ExploreAttBonus, ItemType.ExploreExpBonusItem }, false).Count;
        contentPane.useItemNumText.text = allItemCount.ToString();
    }

    private void setNextLevelData(int group)
    {
        ExploreInstanceLvConfigData nextLvCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(group, data.groupData.level + 1);
        ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(group);
        contentPane.levelText.text = /*"Lv." + */LanguageManager.inst.GetValueByKey("{0}级", data.groupData.level.ToString());
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name);
        contentPane.icon.SetSprite(StaticConstants.exploreAtlas, cfg.instance_icon);
        if (nextLvCfg == null)
        {
            //contentPane.scheduleText.text = "max";
            contentPane.scheduleSlider.maxValue = 1;
            contentPane.scheduleSlider.value = 1;
            contentPane.nextObj.SetActive(false);
        }
        else
        {
            contentPane.nextObj.SetActive(true);
            //contentPane.scheduleText.text = data.groupData.exp + "/" + nextLvCfg.need_instance_exp;
            contentPane.scheduleSlider.maxValue = nextLvCfg.need_instance_exp;
            contentPane.scheduleSlider.value = Mathf.Max(nextLvCfg.need_instance_exp * 0.05f, data.groupData.exp);
            //contentPane.nextIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon);
            contentPane.nextUpObj.SetActive(nextLvCfg.effect_type == 1 || nextLvCfg.effect_type == 6);
            if (nextLvCfg.effect_type == 5)
            {
                var exploreCfg = ExploreInstanceConfigManager.inst.GetConfig(nextLvCfg.effect_id[0]);
                if (cfg.instance_group != exploreCfg.instance_group)
                {
                    contentPane.nextIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon, needSetNativeSize: true);
                }
                else
                {
                    contentPane.nextIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                    contentPane.nextIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon);
                }
            }
            else
            {
                contentPane.nextIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);
                contentPane.nextIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon);
            }
        }
    }

    public void refreshData()
    {
        isAuto = false;
        if (data != null)
            setData(data.groupData.groupId, curIndex);
    }

    private void setHeroDatas()
    {
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            contentPane.allHeroes[index].setData(i, HeroItemAddClickHandler, RemoveHeroCom);
        }

        //contentPane.teamFightingText.text = fightingSum.ToString();
    }

    private void HeroItemAddClickHandler(int heroIndex)
    {
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREHERO_SHOWUI, heroIndex, curExploreCfg.id, itemAddExpPercent);
    }

    public void AddHeroCom(int heroUid, int index)
    {
        var heroData = RoleDataProxy.inst.GetHeroDataByUid(heroUid);

        heroData.isSelect = true;
        float canAddExp = curExploreCfg.hero_exp * data.AddExpPercent * itemAddExpPercent;
        contentPane.allHeroes[index].setHeroData(heroData, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder, canAddExp);
        exploreHeroNumChnage(true);
        teamFightSum += contentPane.allHeroes[index].fightingCount;
        setFightChange();

        if (isAuto)
        {
            contentPane.allHeroes[index].CheckPosAndProfessionType();
        }

        RefreshShowTalent();
    }

    public void RefreshShowTalent()
    {
        int[] heroTypes = new int[3];
        selectTalent = null;

        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int heroIndex = i;
            if (contentPane.allHeroes[heroIndex].Data != null)
            {
                heroTypes[contentPane.allHeroes[heroIndex].Data.config.type - 1]++;
            }
        }

        for (int i = contentPane.allHeroes.Count - 1; i >= 0; i--)
        {
            int heroIndex = i;
            if (contentPane.allHeroes[heroIndex].Data != null)
            {
                var curTalentData = contentPane.allHeroes[heroIndex].Data.talentConfig.GetRingVal();
                if (curTalentData != null)
                {
                    if (heroTypes[curTalentData.GetRingHeroType() - 1] >= 3)
                    {
                        selectTalent = curTalentData;
                        selectTalent.isOpen = true;
                    }
                }
            }
        }

        if (selectTalent != null)
        {
            contentPane.showTalentObj.gameObject.SetActive(selectTalent.isOpen);
            if (selectTalent.isOpen)
            {
                var skillShowCfg = HeroSkillShowConfigManager.inst.GetConfig(selectTalent.talentId);
                if (skillShowCfg != null)
                {
                    contentPane.showTalentIcon.SetSprite(skillShowCfg.skill_atlas, skillShowCfg.skill_icon);
                }
            }
        }
        else
        {
            contentPane.showTalentObj.gameObject.SetActive(false);
        }
    }

    public void RemoveHeroCom(int heroUid, int index)
    {
        var heroData = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        heroData.isSelect = false;
        contentPane.allHeroes[index].clearHeroData();
        exploreHeroNumChnage(false);
        teamFightSum -= contentPane.allHeroes[index].fightingCount;
        setFightChange();

        RefreshShowTalent();
    }

    private void setFightChange()
    {
        teamFightSum = 0;
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            if (contentPane.allHeroes[i].Data != null)
            {
                contentPane.allHeroes[i].setHeroFightCount(itemAttPercent);
            }
        }
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            if (contentPane.allHeroes[index].Data != null)
            {
                teamFightSum += contentPane.allHeroes[index].fightingCount;
            }
        }
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            if (contentPane.allHeroes[i].Data != null)
            {
                contentPane.allHeroes[i].setHeroFighit(itemAttPercent, curExploreCfg.recommend_power, teamFightSum, curExploreCfg.team_strength);
            }
        }
    }

    //private void setTalkObj(bool isKill)
    //{
    //    if (isKill)
    //    {
    //        tween.Kill(true);
    //        tween = null;
    //    }
    //    else
    //    {
    //        if (tween == null)
    //        {
    //            float timerId = 0;
    //            tween = DOTween.To(() => timerId, x => timerId = x, 3, 2).SetLoops(-1).OnStepComplete(() =>
    //            {
    //                if (contentPane.talkObj.activeSelf)
    //                {
    //                    contentPane.talkObj.transform.DOScale(0, 0.5f).From(1).SetEase(Ease.InBack).OnComplete(() =>
    //                    {
    //                        contentPane.talkObj.SetActive(false);
    //                    });
    //                }
    //                else
    //                {
    //                    contentPane.talkObj.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack).OnStart(() =>
    //                    {
    //                        contentPane.talkObj.SetActive(true);
    //                    });
    //                }
    //            });
    //        }
    //    }
    //}

    //private void ToggleItemData()
    //{
    //    for (int i = 0; i < contentPane.group.togglesBtn.Count; i++)
    //    {
    //        int index = i;
    //        ExplorePrepareItemView itemData = contentPane.group.togglesBtn[index].GetComponent<ExplorePrepareItemView>();
    //        itemData.setData(data.explores[index], data.groupData.groupId);
    //    }
    //}

    private void setDifficultyData(int difficulty)
    {
        int type = data.explores[curIndex].type != 4 ? 1 : 2;
        curExploreCfg = ExploreInstanceConfigManager.inst.GetDataByTypeAndDifficultyGroup(data.groupData.groupId, type, difficulty);
        int tempVal = suggestFight;
        suggestFight = curExploreCfg.recommend_power;
        if (isFirst)
        {
            contentPane.suggestFightingText.text = suggestFight.ToString();
            isFirst = false;
        }
        else
        {
            DOTween.To(() => tempVal, x => tempVal = x, suggestFight, 0.8f).OnUpdate(() =>
            {
                contentPane.suggestFightingText.text = tempVal.ToString();
            }).SetEase(Ease.OutExpo);
        }

        contentPane.diffText.text = LanguageManager.inst.GetValueByKey(StaticConstants.diffType[difficulty - 1]);
        contentPane.diffText.color = GUIHelper.GetColorByColorHex(StaticConstants.diffColor[difficulty - 1]);
        contentPane.diffIcon.SetSprite(StaticConstants.exploreAtlas, StaticConstants.diffIconName[difficulty - 1]);

        //int curTime = Mathf.CeilToInt(curExploreCfg.explore_time * data.ExploreTimePercent);
        //contentPane.exploreTimeText.text = TimeUtils.timeSpanStrip(curTime);

        if (difficulty > data.difficult)
        {
            contentPane.exploreBtnText.text = LanguageManager.inst.GetValueByKey("{0}级解锁", data.dfficLevel[difficulty - 1].ToString());
            //data.dfficLevel[difficulty - 1] + LanguageManager.inst.GetValueByKey("级") + LanguageManager.inst.GetValueByKey("解锁");
            contentPane.exploreBtn.interactable = false;
            GUIHelper.SetUIGray(contentPane.exploreBtn.transform, true);
        }
        else
        {
            contentPane.exploreBtnText.text = LanguageManager.inst.GetValueByKey("探索");
            contentPane.exploreBtn.interactable = true;
            GUIHelper.SetUIGray(contentPane.exploreBtn.transform, false);
        }

        if (curExploreCfg.instance_type == 1) // 普通副本
        {
            contentPane.bossObj.SetActive(false);
            contentPane.limitText.text = ItemBagProxy.inst.GetItem(data.explores[curIndex].id).count + "/" + UserDataProxy.inst.playerData.pileLimit;
        }
        else if (curExploreCfg.instance_type == 2) // boss副本
        {
            contentPane.bossObj.SetActive(true);
            var monsterCfg = MonsterConfigManager.inst.GetConfig(curExploreCfg.boss_id);
            contentPane.bossIcon.SetSprite(monsterCfg.monster_atlas, monsterCfg.monster_icon);
            contentPane.bossText.text = LanguageManager.inst.GetValueByKey(monsterCfg.monster_name);
        }

        setHeroPositionData();
        setPointData(difficulty);
        setAwardItemData(curExploreCfg);
        setAutomaticHeroData(curExploreCfg);
        calculateTime();

        contentPane.numText.text = sumNum.ToString();

        if (selectItem != null)
            SwitchItemTypeAdd();
    }

    private void exploreHeroNumChnage(bool isAdd)
    {
        sumNum = sumNum + (isAdd ? 1 : -1);
        contentPane.numText.text = sumNum.ToString();

        calculateTime();
    }

    private void calculateTime()
    {
        int exploreTime = curExploreCfg.explore_time;
        int numPercent = Mathf.CeilToInt(exploreTime * curExploreCfg.decreas_formula / 100.0f);
        exploreTime -= sumNum * numPercent;
        exploreTime = Mathf.RoundToInt(exploreTime * data.exploreTimePercent);
        exploreTime = Mathf.CeilToInt(exploreTime * itemTimePercent);
        var buff = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.explore_speedUp);
        if (buff != null)
            exploreTime = exploreTime - Mathf.CeilToInt(exploreTime * buff.buffInfo.buffParam / 100.0f);
        var unionBuffData = UserDataProxy.inst.GetUnionBuffData(EUnionScienceType.ExploreSkill);
        if (unionBuffData != null)
            exploreTime = exploreTime - Mathf.CeilToInt(exploreTime * unionBuffData.config.add2_num / 100.0f);

        //int curTime = Mathf.CeilToInt(curExploreCfg.explore_time * data.ExploreTimePercent);
        //curTime -= Mathf.CeilToInt(curTime * curExploreCfg.decreas_formula / 100.0f * sumNum);
        //curTime = Mathf.CeilToInt(curTime * itemTimePercent);
        contentPane.exploreTimeText.text = TimeUtils.timeSpanStrip(exploreTime);
    }

    private void setHeroPositionData()
    {
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            if (curExploreCfg.people_number == 5)
            {
                contentPane.allHeroes[index].setIndexState(0);
                contentPane.allHeroes[index].gameObject.SetActive(true);
            }
            else if (curExploreCfg.people_number == 4)
            {
                if (index + 1 == curExploreCfg.position_open[0] || index + 1 == curExploreCfg.position_open[1] || index + 1 == curExploreCfg.position_open[2] || index + 1 == curExploreCfg.position_open[3])
                {
                    contentPane.allHeroes[index].setIndexState(0);
                    contentPane.allHeroes[index].gameObject.SetActive(true);
                }
                else
                {
                    contentPane.allHeroes[index].setIndexState(-1);
                    contentPane.allHeroes[index].clearHeroData();
                    contentPane.allHeroes[index].gameObject.SetActive(false);
                }
            }
            else if (curExploreCfg.people_number == 3)
            {
                if (index + 1 == curExploreCfg.position_open[0] || index + 1 == curExploreCfg.position_open[1] || index + 1 == curExploreCfg.position_open[2])
                {
                    contentPane.allHeroes[index].setIndexState(0);
                    contentPane.allHeroes[index].gameObject.SetActive(true);
                }
                else
                {
                    contentPane.allHeroes[index].setIndexState(-1);
                    contentPane.allHeroes[index].clearHeroData();
                    contentPane.allHeroes[index].gameObject.SetActive(false);
                }
            }
        }
    }

    private void setPointData(int difficulty)
    {
        for (int i = 0; i < contentPane.points.Count; i++)
        {
            if (difficulty - 1 == i)
            {
                contentPane.points[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                contentPane.points[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void setAutomaticHeroData(ExploreInstanceConfigData cfg)
    {
        if (isAuto)
        {
            float canAddExp = curExploreCfg.hero_exp * data.AddExpPercent * itemAddExpPercent;

            teamFightSum = 0;
            for (int i = 0; i < contentPane.allHeroes.Count; i++)
            {
                if (contentPane.allHeroes[i].Data != null)
                {
                    contentPane.allHeroes[i].setHeroFightCount(itemAttPercent);
                }
            }
            for (int i = 0; i < contentPane.allHeroes.Count; i++)
            {
                if (contentPane.allHeroes[i].Data != null)
                {
                    teamFightSum += contentPane.allHeroes[i].fightingCount;
                }
            }

            for (int i = 0; i < contentPane.allHeroes.Count; i++)
            {
                if (contentPane.allHeroes[i].Data != null)
                {
                    contentPane.allHeroes[i].setHeroFighit(itemAttPercent, curExploreCfg.recommend_power, teamFightSum, curExploreCfg.team_strength);
                    contentPane.allHeroes[i].setHeroTween(canAddExp);
                }
            }
            return;
        }
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            if (contentPane.allHeroes[index].Data == null && contentPane.allHeroes[index].gameObject.activeSelf)
            {
                int heroUid = RoleDataProxy.inst.GetHeroUidByExploreIndex(index);
                if (heroUid != -1)
                {
                    AddHeroCom(heroUid, contentPane.allHeroes[index].index);
                }
            }
        }
        isAuto = true;
    }

    private void setAwardItemData(ExploreInstanceConfigData cfg)
    {
        List<int> minList = new List<int>();
        List<int> maxList = new List<int>();
        minList.Add(cfg.drop1_num_min);
        minList.Add(cfg.drop2_num_min);
        minList.Add(cfg.drop3_num_min);
        maxList.Add(cfg.drop1_num_max);
        maxList.Add(cfg.drop2_num_max);
        maxList.Add(cfg.drop3_num_max);
        for (int i = 0; i < contentPane.allItem.Count; i++)
        {
            int index = i;

            if (data.explores[curIndex].type != 4)
            {
                contentPane.gridLayoutGroup.cellSize = new Vector2(210, 210);
                if (index < 1)
                {
                    contentPane.allItem[index].gameObject.SetActive(true);
                    contentPane.allItem[index].setData(data.explores[curIndex].id, minList[curIndex], maxList[curIndex], data.dropCount[curIndex], itemAddPercent, true);
                    var itemCfg = ItemconfigManager.inst.GetConfig(data.explores[curIndex].id);
                    contentPane.rewardIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
                    int minNum = minList[curIndex];
                    int maxNum = maxList[curIndex];
                    minNum += data.dropCount[curIndex];
                    maxNum += data.dropCount[curIndex];
                    minNum = Mathf.CeilToInt(minNum * itemAddPercent);
                    maxNum = Mathf.CeilToInt(maxNum * itemAddPercent);

                    var buildPercent = UserDataProxy.inst.GetExploreDropMaterialOutputUp(data.explores[curIndex].id);
                    minNum = Mathf.CeilToInt(minNum * (1 + buildPercent));
                    maxNum = Mathf.CeilToInt(maxNum * (1 + buildPercent));

                    var buffCfg = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.explore_dropUp);
                    if (buffCfg != null)
                    {
                        minNum = Mathf.CeilToInt(minNum * (1 + buffCfg.buffInfo.buffParam / 100.0f));
                        maxNum = Mathf.CeilToInt(maxNum * (1 + buffCfg.buffInfo.buffParam / 100.0f));
                    }

                    contentPane.rewardText.text = minNum + " - " + maxNum;
                }
                else
                    contentPane.allItem[index].gameObject.SetActive(false);
            }
            else
            {
                contentPane.gridLayoutGroup.cellSize = new Vector2(120, 120);
                contentPane.allItem[index].gameObject.SetActive(true);
                if (index + 1 != contentPane.allItem.Count)
                    contentPane.allItem[index].setData(data.explores[curIndex].awards[index], minList[index], maxList[index], data.dropCount[index], itemAddPercent, false);
                else
                {
                    contentPane.rewardIcon.SetSprite("item_atlas", (60000 + cfg.instance_group).ToString());
                    contentPane.rewardText.text = StaticConstants.getKeyProbability[cfg.difficulty - 1];
                    contentPane.allItem[index].setRandomData(cfg.difficulty, cfg.instance_group);
                }
            }
        }
    }

    public void SelectItemComplete(int itemId)
    {
        selectItem = ItemconfigManager.inst.GetConfig(itemId);
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("{0}添加为强化品", LanguageManager.inst.GetValueByKey(selectItem.name)), GUIHelper.GetColorByColorHex("FFEA00"));
        contentPane.useItemNumObj.SetActive(false);
        //contentPane.useItemIcon.gameObject.SetActive(true);
        contentPane.useObj.SetActive(true);
        contentPane.notUseObj.SetActive(false);
        //contentPane.bgObj.SetActive(false);
        contentPane.useItemIcon.SetSprite(selectItem.atlas, selectItem.icon);

        contentPane.showItemObj.gameObject.SetActive(true);
        contentPane.showItemIcon.SetSprite(selectItem.atlas, selectItem.icon);

        SwitchItemTypeAdd();
    }

    private void SwitchItemTypeAdd(bool isAdd = true)
    {
        switch (selectItem.type)
        {
            // 减少时间
            case 20:
                itemTimePercent = isAdd ? (1 - (float)selectItem.effect / 100) : 1;
                //int curTime = Mathf.CeilToInt(curExploreCfg.explore_time * data.ExploreTimePercent);
                //curTime -= Mathf.CeilToInt(curTime * curExploreCfg.decreas_formula / 100.0f * sumNum);
                //curTime = Mathf.CeilToInt(curTime * itemTimePercent);
                //contentPane.exploreTimeText.text = TimeUtils.timeSpanStrip(curTime);
                calculateTime();
                break;
            // 增加经验
            case 24:
                itemAddExpPercent = isAdd ? (100 + selectItem.effect) / 100.0f : 1;
                float canAddExp = curExploreCfg.hero_exp * data.AddExpPercent * itemAddExpPercent;
                for (int i = 0; i < contentPane.allHeroes.Count; i++)
                {
                    int index = i;
                    if (contentPane.allHeroes[index].Data != null)
                    {
                        contentPane.allHeroes[index].setHeroTween(canAddExp);
                    }
                }
                break;
            // 增加产量
            case 21:
                itemAddPercent = isAdd ? (100 + selectItem.effect) / 100.0f : 1;
                setAwardItemData(curExploreCfg);
                break;
            // 增加战力
            case 22:
                //int tempFight = fightingSum;
                //fightingSum = 0;
                itemAttPercent = isAdd ? selectItem.effect / 100.0f : -1;
                teamFightSum = 0;
                for (int i = 0; i < contentPane.allHeroes.Count; i++)
                {
                    int index = i;
                    if (contentPane.allHeroes[index].Data != null)
                    {
                        contentPane.allHeroes[index].setHeroFightCount(itemAttPercent);
                    }
                }
                for (int i = 0; i < contentPane.allHeroes.Count; i++)
                {
                    int index = i;
                    if (contentPane.allHeroes[index].Data != null)
                    {
                        teamFightSum += contentPane.allHeroes[index].fightingCount;
                    }
                }
                for (int i = 0; i < contentPane.allHeroes.Count; i++)
                {
                    int index = i;
                    if (contentPane.allHeroes[index].Data != null)
                    {
                        contentPane.allHeroes[index].setHeroFighit(itemAttPercent, curExploreCfg.recommend_power, teamFightSum, curExploreCfg.team_strength);
                        //fightingSum += contentPane.allHeroes[index].fightingCount;
                    }
                }
                //DOTween.To(() => tempFight, x => tempFight = x, fightingSum, 0.8f).OnUpdate(() =>
                //   {
                //       contentPane.teamFightingText.text = tempFight.ToString();
                //   });
                //contentPane.talkObj.transform.GetChild(0).gameObject.SetActive(fightingSum >= curExploreCfg.recommend_power);
                //contentPane.talkObj.transform.GetChild(1).gameObject.SetActive(fightingSum < curExploreCfg.recommend_power);
                break;
        }
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
        //contentPane.talkObj.GetComponent<Canvas>().sortingOrder = _uiCanvas.sortingOrder + 2;
        contentPane.showItemObj.gameObject.SetActive(false);
        contentPane.talentTipObj.gameObject.SetActive(false);
        contentPane.showTalentObj.gameObject.SetActive(false);
        contentPane.heroCanvas.sortingOrder = _uiCanvas.sortingOrder + 2;
        //contentPane.maskBgCanvas.sortingOrder = _uiCanvas.sortingOrder + 3;
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
        contentPane.useObj.SetActive(false);
        contentPane.notUseObj.SetActive(true);
        contentPane.bgObj.SetActive(true);
        contentPane.talentTipObj.gameObject.SetActive(false);
        contentPane.showItemObj.gameObject.SetActive(false);

        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            if (contentPane.allHeroes[index].Data != null)
            {
                RemoveHeroCom(contentPane.allHeroes[index].Data.uid, index);
            }
            else
            {
                contentPane.allHeroes[index].clearHeroData();
            }
        }
        //heroUids.Clear();
        selectItem = null;
        selectTalent = null;
        //fightingSum = 0;
        //contentPane.teamFightingText.text = fightingSum.ToString();
        curIndex = 0;
        isAuto = false;
        isFirst = true;
        sumNum = 0;
        itemAddPercent = 1;
        itemAddExpPercent = 1;
        itemAttPercent = -1;
        itemTimePercent = 1;
        //contentPane.talkObj.SetActive(false);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        //contentPane.windowAnim.CrossFade("show", 0f);
        //contentPane.windowAnim.Update(0f);
        //contentPane.windowAnim.Play("show");
    }

    //protected override void DoHideAnimation()
    //{
    //    //float animTime = contentPane.windowAnim.GetClipLength("explorePrepare_hideui");
    //    //GameTimer.inst.AddTimer()
    //}
}
