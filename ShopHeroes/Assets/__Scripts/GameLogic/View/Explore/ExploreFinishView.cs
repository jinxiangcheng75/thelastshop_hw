using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExploreFinishView : ViewBase<ExploreFinishCom>
{
    public override string viewID => ViewPrefabName.ExploreCompleteUI;
    public override string sortingLayerName => "window";

    int exploreId;
    int slotId;
    int levelUpHeroCount = 0;
    bool isSuccess;
    ExploreGroupData lastGroup;
    List<RoleHeroData> lastHeroInfos = new List<RoleHeroData>();
    List<OneRewardItem> getItemList = new List<OneRewardItem>();
    protected override void onInit()
    {
        base.onInit();
        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.collectBtn.onClick.AddListener(() =>
        {
            ExitExplore();
        });
        contentPane.collectAllBtn.onClick.AddListener(() =>
        {
            //GameStateEvent.inst.changeState(new StateTransition(kGameState.Town, false));
            //HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
            //GuideManager.inst.GuideTrigger();
            //hide();
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
            {
                if ((K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip)
                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREREWARDVIP, slotId);
                else
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_BuyVipUI", 1);
            }
            else
            {
                ExitExplore();
            }
        });
        contentPane.continueBtn.onClick.AddListener(() =>
        {
            ExitExplore();
        });
    }

    public void GetBeforeChangeData(/*ExploreGroupData exploreGroupData, List<HeroInfo> heroInfos*/)
    {
        lastGroup = ExploreDataProxy.inst.exploreGroupData;
        lastHeroInfos = ExploreDataProxy.inst.heroInfos;
    }

    public void setExploreSuccessFinishData(List<OneRewardItem> bagResources, int slotId, int exploreId)
    {
        isSuccess = true;
        GetBeforeChangeData();
        contentPane.finishBgIcon.SetSprite("__common_1", "shoudao_piaodai");
        contentPane.allBtn.SetActive(false);
        //contentPane.collectBtn.gameObject.SetActive(false);
        //contentPane.collectAllBtn.gameObject.SetActive(false);
        if ((K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip)
            contentPane.collectBtn.gameObject.SetActive(false);
        else
            contentPane.collectBtn.gameObject.SetActive(true);
        this.exploreId = exploreId;
        this.slotId = slotId;
        getItemList = bagResources;
        contentPane.successObj.SetActive(true);
        contentPane.loseObj.SetActive(false);
        contentPane.finishResultText.text = LanguageManager.inst.GetValueByKey("冒险完成!");
        //contentPane.finishResultText.color = Color.white;
        //contentPane.separatorImage.color = Color.white;
        var cfg = ExploreInstanceConfigManager.inst.GetConfig(exploreId);
        if (ExploreDataProxy.inst.slotType != 2)
            contentPane.exploreNameAndDiffText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name) + " <Color=" + StaticConstants.diffColor[cfg.difficulty - 1] + ">" + LanguageManager.inst.GetValueByKey(StaticConstants.diffType[cfg.difficulty - 1]) + "</Color>";
        else
            contentPane.exploreNameAndDiffText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name);
        setHeroData();
    }

    public void setExploreLoseFinishData(int exploreId)
    {
        isSuccess = false;
        contentPane.finishBgIcon.SetSprite("__common_1", "shoudao_shibai");
        GetBeforeChangeData();
        contentPane.successObj.SetActive(false);
        contentPane.loseObj.SetActive(true);
        this.exploreId = exploreId;
        contentPane.finishResultText.text = LanguageManager.inst.GetValueByKey("冒险失败");
        //contentPane.finishResultText.color = Color.red;
        //contentPane.separatorImage.color = Color.red;
        var cfg = ExploreInstanceConfigManager.inst.GetConfig(exploreId);
        if (ExploreDataProxy.inst.slotType != 2)
            contentPane.exploreNameAndDiffText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name) + " " + LanguageManager.inst.GetValueByKey(StaticConstants.diffType[cfg.difficulty - 1]);
        else
            contentPane.exploreNameAndDiffText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name);

        setHeroData();
    }

    int timerId = 0;
    private void setHeroData()
    {
        contentPane.exploreObj.SetActive(false);
        contentPane.awardsObj.SetActive(false);
        if (isSuccess)
        {
            levelUpHeroCount += lastHeroInfos.Count;
        }
        for (int i = 0; i < contentPane.heroes.Count; i++)
        {
            int index = i;

            if (index < lastHeroInfos.Count)
            {
                contentPane.heroes[index].gameObject.SetActive(true);
                if (isSuccess)
                {
                    contentPane.heroes[index].setData(lastHeroInfos[index], index);
                }
                else
                    contentPane.heroes[index].setLoseData(lastHeroInfos[index], index);
            }
            else
            {
                contentPane.heroes[index].gameObject.SetActive(false);
            }
        }

        if (isSuccess)
        {
            //timerId = GameTimer.inst.AddTimer(1.8f, 1, JudgeAllHeroesIsComplete);

        }
        else
            JudgeAllHeroesIsComplete();
    }

    public void JudgeAllHeroesIsComplete()
    {
        if (levelUpHeroCount <= 0)
        {
            contentPane.gameObject.SetActive(true);
            if (isSuccess)
            {
                if (ExploreDataProxy.inst.slotType != 2)
                    setExploreData();
                else
                    ExploreLevelUpComplete();
            }

        }
        else
        {
            contentPane.gameObject.SetActive(false);
        }
        //GameTimer.inst.RemoveTimer(timerId);
        timerId = 0;
    }

    public void AddHeroCount()
    {
        levelUpHeroCount++;
    }

    public void RemoveHeroCount()
    {
        levelUpHeroCount--;
        if (levelUpHeroCount <= 0)
        {
            contentPane.gameObject.SetActive(true);
            if (ExploreDataProxy.inst.slotType != 2)
                setExploreData();
            else
                ExploreLevelUpComplete();
        }
    }

    public void ExploreLevelUpComplete()
    {
        contentPane.gameObject.SetActive(true);
        setAwardData();
    }

    private void setAwardData()
    {
        contentPane.awardsObj.SetActive(true);
        contentPane.exploreObj.SetActive(false);
        for (int i = 0; i < contentPane.awards.Count; i++)
        {
            int index = i;

            if (index < getItemList.Count)
            {
                contentPane.awards[index].gameObject.SetActive(true);
                contentPane.awards[index].transform.localScale = new Vector3(0f, 0f, 0f);
                bool isvip = index == getItemList.Count - 1;
                contentPane.awards[index].setData(getItemList[index], isvip, index);
            }
            else
            {
                contentPane.awards[index].gameObject.SetActive(false);
            }
        }

        GameTimer.inst.AddTimer(0.5f + 0.1f * getItemList.Count, 1, () =>
          {
              for (int i = 0; i < getItemList.Count; i++)
              {
                  int index = i;
                  contentPane.awards[index].RecoveryData();
              }
          });

        GameTimer.inst.AddTimer(0.2f + 0.1f * getItemList.Count + 0.4f, 1, () =>
          {
              contentPane.allBtn.SetActive(true);
              //contentPane.collectBtn.gameObject.SetActive(true);
              //contentPane.collectAllBtn.gameObject.SetActive(true);
          });
    }

    public void vipBuyRefresh()
    {
        if (getItemList == null) return;
        if (getItemList.Count <= 0) return;
        if ((K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip)
            contentPane.collectBtn.gameObject.SetActive(false);
        else
            contentPane.collectBtn.gameObject.SetActive(true);
        contentPane.awards[getItemList.Count - 1].RecoveryData();
    }

    private void setExploreData()
    {
        GameTimer.inst.AddTimer(0.2f, 1, () =>
          {
              contentPane.exploreObj.SetActive(true);

              int exp = 0;
              int level = 0;
              //var lastGroupData = ExploreDataProxy.inst.GetGroupDataByGroupId(lastGroup.groupId);
              exp = lastGroup.exp;
              level = lastGroup.level;
              var nextData = ExploreDataProxy.inst.GetGroupDataByGroupId(lastGroup.groupId);//ExploreDataProxy.inst.AddExploreData(lastGroup);
              var instanceCfg = ExploreInstanceConfigManager.inst.GetConfig(exploreId);
              var instanceLvCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(nextData.groupData.groupId, nextData.groupData.level);
              var nextInstanceLvCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(nextData.groupData.groupId, nextData.groupData.level + 1);

              contentPane.exploreIcon.SetSprite(StaticConstants.exploreAtlas, instanceCfg.instance_icon);
              contentPane.exploreNameText.text = LanguageManager.inst.GetValueByKey(instanceCfg.instance_name);

              if (level < nextData.groupData.level)
              {
                  contentPane.exploreMaxObj.SetActive(true);
                  contentPane.nextIcon.SetSprite(instanceLvCfg.effect_atlas, instanceLvCfg.effect_icon);
                  contentPane.nextUpImg.enabled = instanceLvCfg.effect_type == 1 || instanceLvCfg.effect_type == 6 ? true : false;
                  contentPane.exploreLevelText.text = "Lv." + level;
                  contentPane.exploreExpSlider.maxValue = instanceLvCfg.need_instance_exp;
                  contentPane.exploreExpSlider.value = exp;

                  GameTimer.inst.AddTimer(0.4f, 1, () =>
                    {
                        contentPane.fillMaskRect.gameObject.SetActive(true);
                        contentPane.fillMaskRect.anchorMax = new Vector2(1, contentPane.fillMaskRect.anchorMax.y);

                        DOTween.To(() => exp, t => exp = t, instanceLvCfg.need_instance_exp, 0.6f).SetDelay(0.4f).OnUpdate(() =>
                        {
                            contentPane.exploreExpSlider.value = exp;
                        }).OnComplete(() =>
                        {
                            contentPane.fillMaskRect.gameObject.SetActive(false);
                            contentPane.exploreLevelText.text = "Lv." + nextData.groupData.level/* + nextData.groupData.level*/;
                            if (nextInstanceLvCfg == null)
                            {
                                contentPane.exploreMaxObj.SetActive(false);
                                //contentPane.nextIcon.gameObject.SetActiveFalse();
                                //contentPane.nextUpImg.enabled = false;
                                contentPane.exploreExpSlider.maxValue = 1;
                                contentPane.exploreExpSlider.value = 1;

                                GameTimer.inst.AddTimer(1.2f, 1, () =>
                                {
                                    contentPane.gameObject.SetActive(false);
                                    upgradeItem upgradeItem = new upgradeItem();
                                    upgradeItem.type = kExploreItemUpgradeType.ExploreUpgrade;
                                    upgradeItem.exploreGroupId = nextData.groupData.groupId;
                                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.UPGRADEADD, upgradeItem);
                                });
                            }
                            else
                            {
                                exp = 0;
                                contentPane.exploreMaxObj.SetActive(true);
                                //contentPane.nextIcon.gameObject.SetActiveTrue();
                                contentPane.nextIcon.SetSprite(nextInstanceLvCfg.effect_atlas, nextInstanceLvCfg.effect_icon);
                                contentPane.nextUpImg.enabled = nextInstanceLvCfg.effect_type == 1 || nextInstanceLvCfg.effect_type == 6 ? true : false;
                                contentPane.exploreExpSlider.maxValue = nextInstanceLvCfg.need_instance_exp;
                                contentPane.exploreExpSlider.value = exp;

                                GameTimer.inst.AddTimer(0.4f, 1, () =>
                                  {
                                      contentPane.fillMaskRect.gameObject.SetActive(true);
                                      contentPane.fillMaskRect.anchorMax = new Vector2((float)nextData.groupData.exp / nextInstanceLvCfg.need_instance_exp, contentPane.fillMaskRect.anchorMax.y);

                                      DOTween.To(() => exp, t => exp = t, nextData.groupData.exp, 1.2f).SetDelay(0.4f).OnUpdate(() =>
                                      {
                                          contentPane.exploreExpSlider.value = exp;
                                      }).OnComplete(() =>
                                      {
                                          contentPane.fillMaskRect.gameObject.SetActive(false);
                                          contentPane.gameObject.SetActive(false);
                                          upgradeItem upgradeItem = new upgradeItem();
                                          upgradeItem.type = kExploreItemUpgradeType.ExploreUpgrade;
                                          upgradeItem.exploreGroupId = nextData.groupData.groupId;
                                          EventController.inst.TriggerEvent(GameEventType.ExploreEvent.UPGRADEADD, upgradeItem);
                                      });
                                  });


                            }
                        });
                    });


              }
              else
              {
                  contentPane.exploreLevelText.text = "Lv." + nextData.groupData.level/* + nextData.groupData.level*/;
                  if (nextInstanceLvCfg == null)
                  {
                      contentPane.exploreExpSlider.maxValue = 1;
                      contentPane.exploreExpSlider.value = 1;
                      //contentPane.nextIcon.gameObject.SetActiveFalse();
                      //contentPane.nextUpImg.enabled = false;
                      contentPane.exploreMaxObj.SetActive(false);

                      GameTimer.inst.AddTimer(1.2f, 1, () =>
                        {
                            ExploreLevelUpComplete();
                        });
                  }
                  else
                  {
                      contentPane.exploreMaxObj.SetActive(true);
                      //contentPane.nextIcon.gameObject.SetActiveTrue();
                      contentPane.nextIcon.SetSprite(nextInstanceLvCfg.effect_atlas, nextInstanceLvCfg.effect_icon);
                      contentPane.nextUpImg.enabled = nextInstanceLvCfg.effect_type == 1 || nextInstanceLvCfg.effect_type == 6 ? true : false;
                      contentPane.exploreExpSlider.maxValue = nextInstanceLvCfg.need_instance_exp;
                      contentPane.exploreExpSlider.value = exp;

                      GameTimer.inst.AddTimer(0.4f, 1, () =>
                        {
                            contentPane.fillMaskRect.gameObject.SetActive(true);
                            contentPane.fillMaskRect.anchorMax = new Vector2((float)nextData.groupData.exp / contentPane.exploreExpSlider.maxValue, contentPane.fillMaskRect.anchorMax.y);

                            DOTween.To(() => exp, t => exp = t, nextData.groupData.exp, 1.2f).SetDelay(0.4f).OnUpdate(() =>
                            {
                                contentPane.exploreExpSlider.value = exp;
                            }).OnComplete(() =>
                            {
                                contentPane.fillMaskRect.gameObject.SetActive(false);
                                ExploreLevelUpComplete();
                            });
                        });


                  }
              }
          });

    }

    public void ExitExplore()
    {
        var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);

        bool hasMaxLevelHero = false;

        for (int i = 0; i < lastHeroInfos.Count; i++)
        {
            var heroInfo = lastHeroInfos[i];
            if (cityData != null && heroInfo.level >= cityData.effectVal && heroupgradeconfigManager.inst.GetHeroUpgradeConfig(heroInfo.level + 1) != null) //说明等级到了建筑物上限
            {
                hasMaxLevelHero = true;
                break;
            }
        }

        if (hasMaxLevelHero) HotfixBridge.inst.TriggerLuaEvent("HasMaxLevelHero");

        if (ExploreDataProxy.inst.slotType != 2)
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
        else
        {
            ExploreDataProxy.inst.needShowRefugePanel = true;
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Shop, true));
            //HotfixBridge.inst.TriggerLuaEvent("ShowUI_RefugePrepareUI");
        }

        hide();
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {

        levelUpHeroCount = 0;
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }
}
