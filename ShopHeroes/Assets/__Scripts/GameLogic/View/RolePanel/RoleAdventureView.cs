using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAdventureView : ViewBase<RoleAdventureComp>
{
    public override string viewID => ViewPrefabName.RoleAdventureUI;
    public override string sortingLayerName => "popup";
    ExploreSlotData data;
    int timerId;
    bool canQuicken = false;
    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noRoleAndSettingAndEnergy;
        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.onClick.AddListener(() =>
        {
            hide();
        });
        //contentPane.finishBtn.onClick.AddListener(() =>
        //{
        //    hide();
        //    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREEND, data.slotId);
        //});
        contentPane.gemBtn.onClick.AddListener(() =>
        {
            if (data.exploreState == 2)
            {
                if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
                {
                    GuideManager.inst.secondIsNotWait = false;
                }
                hide();
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREEND, data.slotId);
            }
            else if (data.exploreState == 1)
            {
                if (!canQuicken && UserDataProxy.inst.playerData.exploreImmediatelyFreeCount <= 0)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(data.exploringRemainTime) - UserDataProxy.inst.playerData.gem);
                    return;
                }
                if (!GuideDataProxy.inst.CurInfo.isAllOver)
                {
                    var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                    GuideManager.inst.secondIsNotWait = true;
                    if (((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)cfg.guide_type == K_Guide_Type.TipsAndRestrictClick) && cfg.btn_name == contentPane.gemBtn.name)
                    {
                        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREIMMEDIATELY, data.slotId);
                        return;
                    }
                }
                if (contentPane.sureAgainObj.activeSelf)
                {
                    contentPane.sureAgainObj.SetActiveFalse();
                    RoleDataProxy.inst.roleHeroState = kRoleHeroChange.ExploreImmediately;
                    // 缺少发送网络数据逻辑
                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREIMMEDIATELY, data.slotId);
                }
                else
                    contentPane.sureAgainObj.SetActiveTrue();
            }
        });
    }

    public void setAdventureDataBySlot(int slotId)
    {
        data = ExploreDataProxy.inst.GetSlotDataById(slotId);
        var cfg = ExploreInstanceConfigManager.inst.GetConfig(data.exploreId);
        var exploreGroupData = ExploreDataProxy.inst.GetGroupDataByGroupId(cfg.instance_group);
        contentPane.difficultyText.text = LanguageManager.inst.GetValueByKey(StaticConstants.diffType[cfg.difficulty - 1]);
        contentPane.difficultyText.color = GUIHelper.GetColorByColorHex(StaticConstants.diffColor[cfg.difficulty - 1]);
        contentPane.difficultyIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.diffIconName[cfg.difficulty - 1]);
        contentPane.adventureIcon.SetSprite(StaticConstants.exploreAtlas, cfg.instance_icon);
        if (cfg.instance_type == 1)
        {
            contentPane.notBossObj.SetActive(true);
            itemConfig itemCfg = ItemconfigManager.inst.GetConfig(exploreGroupData.explores[data.exploreType - 1].id);
            itemConfig selectCfg = ItemconfigManager.inst.GetConfig(data.useItemId);
            float itemAddPercent = 1;
            if (selectCfg != null && (ItemType)selectCfg.type == ItemType.ExploreAddYieldItem)
            {
                itemAddPercent = (100 + (selectCfg != null ? selectCfg.effect : 0)) / 100.0f;
            }

            contentPane.awardIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
            int minNum = 0;
            int maxNum = 0;
            int addPercent = 0;
            if (data.exploreType == 1)
            {
                minNum = cfg.drop1_num_min;
                maxNum = cfg.drop1_num_max;
                addPercent = exploreGroupData.dropCount[0];
            }
            else if (data.exploreType == 2)
            {
                minNum = cfg.drop2_num_min;
                maxNum = cfg.drop2_num_max;
                addPercent = exploreGroupData.dropCount[1];
            }
            else if (data.exploreType == 3)
            {
                minNum = cfg.drop3_num_min;
                maxNum = cfg.drop3_num_max;
                addPercent = exploreGroupData.dropCount[2];
            }
            minNum += addPercent;
            maxNum += addPercent;
            minNum = Mathf.CeilToInt(minNum * itemAddPercent);
            maxNum = Mathf.CeilToInt(maxNum * itemAddPercent);
            var buildPercent = UserDataProxy.inst.GetExploreDropMaterialOutputUp(itemCfg.id);
            minNum = Mathf.CeilToInt(minNum * (1 + buildPercent));
            maxNum = Mathf.CeilToInt(maxNum * (1 + buildPercent));
            var buffCfg = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.explore_dropUp);
            if (buffCfg != null)
            {
                minNum = Mathf.CeilToInt(minNum * (1 + buffCfg.buffInfo.buffParam / 100.0f));
                maxNum = Mathf.CeilToInt(maxNum * (1 + buffCfg.buffInfo.buffParam / 100.0f));
            }
            contentPane.dungeonText.text = minNum + " - " + maxNum;
        }
        else
        {
            contentPane.notBossObj.SetActive(false);
        }

        setState();
        setHeroData();

        contentPane.dungeonSlider.maxValue = data.exploreTotalTime;

        if (timerId == 0)
        {
            if (data.exploreState == 1 && data.exploringRemainTime > 0)
            {
                contentPane.dungeonSlider.value = data.exploreTotalTime - data.exploringRemainTime;
                contentPane.residueText.text = TimeUtils.timeSpanStrip(data.exploringRemainTime);
                int result = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(data.exploringRemainTime);
                contentPane.gemNumText.text = result.ToString();
                //contentPane.gemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
                canQuicken = UserDataProxy.inst.playerData.gem >= result;

                timerId = GameTimer.inst.AddTimer(0.3f, () =>
                {
                    if (data.exploringRemainTime <= 0)
                    {
                        contentPane.residueText.text = "0" + LanguageManager.inst.GetValueByKey("秒");
                        GameTimer.inst.RemoveTimer(timerId);
                        timerId = 0;
                    }
                    else
                    {
                        result = DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(data.exploringRemainTime);
                        contentPane.gemNumText.text = result.ToString();
                        //contentPane.gemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
                        canQuicken = UserDataProxy.inst.playerData.gem >= result;
                        contentPane.dungeonSlider.value = data.exploreTotalTime - data.exploringRemainTime;
                        contentPane.residueText.text = TimeUtils.timeSpanStrip(data.exploringRemainTime);
                    }
                });
            }
            else if (data.exploreState == 2)
            {
                contentPane.dungeonSlider.maxValue = 1;
                contentPane.dungeonSlider.value = 1;
            }
        }

        JudgeIsFree();
    }

    private void setState()
    {
        if (data.exploreState == 2)
        {
            contentPane.residueText.gameObject.SetActive(false);
            contentPane.finishTextObj.SetActive(true);
            contentPane.finishBtn.gameObject.SetActive(true);
        }
        else if (data.exploreState == 1)
        {
            contentPane.residueText.gameObject.SetActive(true);
            contentPane.finishTextObj.SetActive(false);
            contentPane.finishBtn.gameObject.SetActive(false);
        }
    }

    private void setHeroData()
    {
        for (int i = 0; i < contentPane.heroes.Count; i++)
        {
            int index = i;

            if (index < data.heroInfoUIds.Count)
            {
                contentPane.heroes[index].gameObject.SetActive(true);
                var tempData = RoleDataProxy.inst.GetHeroDataByUid(data.heroInfoUIds[index]);
                var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(tempData.level + 1);
                contentPane.heroes[index].setData(tempData, upgradeCfg);
            }
            else
            {
                contentPane.heroes[index].gameObject.SetActive(false);
            }
        }
    }

    private void JudgeIsFree()
    {
        contentPane.costObj.SetActive(UserDataProxy.inst.playerData.exploreImmediatelyFreeCount <= 0);
        contentPane.freeObj.SetActive(UserDataProxy.inst.playerData.exploreImmediatelyFreeCount > 0);
    }

    protected override void onShown()
    {
        contentPane.sureAgainObj.SetActiveFalse();
    }

    public void setAdventureDataByHero()
    {

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animTime = contentPane.uiAnimator.GetClipLength("common_popUpUI_show");
        GameTimer.inst.AddTimer(animTime, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }


    protected override void onHide()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
