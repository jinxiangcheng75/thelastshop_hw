using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExploreFinishHeroItemView : MonoBehaviour
{
    public Image icon;
    public GUIIcon typeIcon;
    public Text timeText;
    public Text levelText;
    public Slider levelSlider;
    public GameObject idleObj;
    public GameObject restingObj;
    public GameObject stateObj;
    public GameObject levelObj;
    public Text stateText;
    public GUIIcon typeBgIcon;
    public GUIIcon heroTypeIcon;

    public RectTransform fillMaskRect;

    RoleHeroData data;
    RoleHeroData roleData;

    //头像
    public Transform headParent;
    GraphicDressUpSystem graphicDressUp;
    public void setData(RoleHeroData _data, int index)
    {
        this.data = _data;

        levelObj.SetActive(true);
        stateObj.SetActive(false);
        stateText.gameObject.SetActive(false);

        roleData = RoleDataProxy.inst.GetHeroDataByUid(data.uid);
        //icon.sprite = RoleDataProxy.inst.GetSprite(StaticConstants.rolePhotoOffset + data.heroUid);
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence);
        typeIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        heroTypeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);
        bool isArriveLimit = roleData.level >= cityData.effectVal;
        //RoleHeroData lastHeroData = new RoleHeroData();
        //var tempData = RoleDataProxy.inst.GetHeroDataByUid(data.heroUid);
        //lastHeroData.level = tempData.level;
        //lastHeroData.exp = tempData.exp;

        //var lastHeroData = RoleDataProxy.inst.GetHeroDataByUid(data.heroUid);
        //roleData = RoleDataProxy.inst.AddHeroData(data);

        setHeroHeadIcon();

        int exp = 0;
        if (data.level < roleData.level)
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADESTART);
            var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(roleData.level);
            var nextUpgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(roleData.level + 1);
            levelText.text = data.level.ToString();
            levelSlider.maxValue = upgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence));
            exp = data.exp;
            levelSlider.value = data.exp;

            GameTimer.inst.AddTimer(0.4f, 1, () =>
              {
                  fillMaskRect.gameObject.SetActive(true);
                  fillMaskRect.anchorMax = new Vector2(1, fillMaskRect.anchorMax.y);

                  DOTween.To(() => exp, t => exp = t, (int)levelSlider.maxValue, 0.4f).OnUpdate(() =>
                  {
                      levelSlider.value = exp;
                  }).SetDelay(0.4f).OnComplete(() =>
                  {
                      fillMaskRect.gameObject.SetActive(false);
                      if (nextUpgradeCfg == null)
                      {
                          stateText.gameObject.SetActive(true);
                          stateText.text = "max";
                          levelSlider.maxValue = 1;
                          levelSlider.value = 1;

                          GameTimer.inst.AddTimer(0.8f, 1, () =>
                          {
                              upgradeItem upgradeItem = new upgradeItem();
                              upgradeItem.heroUid = roleData.uid;
                              upgradeItem.intoType = 3;
                              upgradeItem.type = kExploreItemUpgradeType.HeroUpgrade;
                              EventController.inst.TriggerEvent(GameEventType.ExploreEvent.UPGRADEADD, upgradeItem);
                              levelObj.SetActive(false);
                              stateObj.SetActive(true);
                              setState();
                              EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADEEND);
                          });

                      }
                      else
                      {
                          levelText.text = roleData.level.ToString();
                          exp = 0;
                          levelSlider.value = 0;
                          levelSlider.maxValue = nextUpgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence));

                          GameTimer.inst.AddTimer(0.4f, 1, () =>
                            {
                                fillMaskRect.gameObject.SetActive(true);
                                fillMaskRect.anchorMax = new Vector2((float)roleData.exp / levelSlider.maxValue, fillMaskRect.anchorMax.y);

                                DOTween.To(() => exp, t => exp = t, roleData.exp, 0.8f).OnUpdate(() =>
                                {
                                    levelSlider.value = exp;
                                }).SetDelay(0.4f).OnComplete(() =>
                                {
                                    fillMaskRect.gameObject.SetActive(false);
                                    upgradeItem upgradeItem = new upgradeItem();
                                    upgradeItem.heroUid = roleData.uid;
                                    upgradeItem.intoType = 3;
                                    upgradeItem.type = kExploreItemUpgradeType.HeroUpgrade;
                                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.UPGRADEADD, upgradeItem);
                                    levelObj.SetActive(false);
                                    stateObj.SetActive(true);
                                    setState();
                                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADEEND);
                                });
                            });


                      }
                  });
              });


        }
        else
        {
            var nextUpgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(roleData.level + 1);
            if (nextUpgradeCfg != null)
            {
                levelText.text = roleData.level.ToString();

                if (isArriveLimit)
                {
                    stateText.gameObject.SetActive(true);
                    stateText.text = LanguageManager.inst.GetValueByKey("已达上限");
                    levelSlider.maxValue = 1;
                    levelSlider.value = 1;
                    fillMaskRect.gameObject.SetActive(false);
                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADEEND);
                }
                else
                {
                    exp = data.exp;
                    levelSlider.maxValue = nextUpgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence));
                    levelSlider.value = exp;

                    GameTimer.inst.AddTimer(0.4f, 1, () =>
                      {
                          fillMaskRect.gameObject.SetActive(true);
                          fillMaskRect.anchorMax = new Vector2((float)roleData.exp / nextUpgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence)), fillMaskRect.anchorMax.y);

                          DOTween.To(() => exp, t => exp = t, roleData.exp, 1.2f).SetDelay(0.4f).OnUpdate(() =>
                          {
                              levelSlider.value = exp;
                          }).OnComplete(() =>
                          {
                              fillMaskRect.gameObject.SetActive(false);
                              levelObj.SetActive(false);
                              stateObj.SetActive(true);
                              setState();
                              EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADEEND);
                          });
                      });

                }
            }
            else
            {
                fillMaskRect.gameObject.SetActive(false);
                stateText.gameObject.SetActive(true);
                levelText.text = roleData.level.ToString();
                stateText.text = "max";
                levelSlider.maxValue = 1;
                levelSlider.value = 1;
                levelObj.SetActive(false);
                stateObj.SetActive(true);
                setState();
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.HEROUPGRADEEND);
            }
        }
    }

    public void setLoseData(RoleHeroData _data, int index)
    {
        data = _data;

        levelObj.SetActive(false);
        stateObj.SetActive(true);
        roleData = RoleDataProxy.inst.GetHeroDataByUid(data.uid);
        //icon.sprite = RoleDataProxy.inst.GetSprite(StaticConstants.rolePhotoOffset + data.heroUid);
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence);
        typeIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        heroTypeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);

        //roleData = RoleDataProxy.inst.AddHeroData(data);

        setHeroHeadIcon();
        setState();
    }

    private void setHeroHeadIcon()
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)roleData.gender), roleData.GetHeadDressIds(), (EGender)roleData.gender, callback: system =>
            {
                graphicDressUp = system;
                system.transform.SetParent(headParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)roleData.gender), roleData.GetHeadDressIds(), (EGender)roleData.gender);
        }
    }

    private void setState()
    {
        if (roleData.currentState == 0)
        {
            idleObj.SetActive(true);
            restingObj.SetActive(false);
        }
        else if (roleData.currentState == 1)
        {
            idleObj.SetActive(false);
            restingObj.SetActive(true);
            timeText.text = TimeUtils.timeSpanStrip(roleData.endTime - roleData.startTime);
        }
    }

    public void clearData()
    {

    }
}
