using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mosframe;
using DG.Tweening;

public class ExploreSelectHeroItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GameObject heroObj;
    public GameObject restObj;
    public GameObject heroIdleObj;
    public GameObject heroRestObj;
    public Image grayObj;
    public Text allRestItemCountText;
    public Text allRestGemCountText;
    public Text restTimeText;
    public Text levelText;
    public GUIIcon headBgIcon;
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public Text fightingText;
    public GameObject upObj;
    public Button selfBtn;
    public Button infoBtn;
    public Slider levelSlider;
    public RectTransform tweenTrans;
    public RectTransform maskTrans;
    public Text nameTx;
    public Text limitTx;
    public List<GUIIcon> allStars;
    RoleHeroData data;
    kRoleItemType itemType = kRoleItemType.max;
    [HideInInspector]
    public int index = 0;
    public Action<int> addComHandler;
    Image tweenImage;
    int timerId = 0;

    //头像
    public Transform headParent;
    GraphicDressUpSystem graphicDressUp;

    private void Awake()
    {
        infoBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.uid);
        });
        selfBtn.onClick.AddListener(() =>
        {
            if (itemType == kRoleItemType.Hero && data.currentState == 0)
                addComHandler?.Invoke(data.uid);
            else if (itemType == kRoleItemType.Hero && data.currentState == 1)
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.SINGLEROLERESTING_SHOWUI, data, 0);
            else if (itemType == kRoleItemType.RecoverHero)
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ALLROLERESTING_SHOWUI);
        });

        tweenImage = tweenTrans.GetComponent<Image>();
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public void setData(RoleHeroData _data, Action<int> addAction, float canAddExp)
    {
        itemType = kRoleItemType.Hero;
        heroObj.SetActive(true);
        restObj.SetActive(false);
        infoBtn.gameObject.SetActive(true);
        data = _data;
        addComHandler = addAction;
        levelText.text = data.level.ToString();
        fightingText.text = data.fightingNum.ToString();
        nameTx.text = LanguageManager.inst.GetValueByKey(data.nickName);
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        headBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);

        setHeroHeadIcon();
        setStarShow();
        setStateData(canAddExp);
    }

    private void setStarShow()
    {
        for (int i = 0; i < allStars.Count; i++)
        {
            int index = i;
            if (index < data.config.hero_grade)
            {
                allStars[index].gameObject.SetActive(true);
                allStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
            }
            else
            {
                if (index < data.transferNumLimit)
                {
                    allStars[index].gameObject.SetActive(true);
                    allStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
                }
                else
                {
                    allStars[index].gameObject.SetActive(false);
                }
            }
        }
    }

    private void setStateData(float canAddExp)
    {
        heroIdleObj.SetActive(data.currentState == 0);
        heroRestObj.SetActive(data.currentState == 1);
        grayObj.enabled = data.currentState == 1;

        if (data.currentState == 0)
        {
            heroupgradeconfig nextCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level + 1);
            if (nextCfg == null)
            {
                levelSlider.maxValue = 1;
                levelSlider.value = 1;
                tweenTrans.gameObject.SetActive(false);
                maskTrans.gameObject.SetActive(false);
                upObj.SetActive(false);
            }
            else
            {
                var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);
                if (cityData != null)
                {
                    if (data.level >= cityData.effectVal)
                    {
                        limitTx.enabled = true;
                        levelSlider.maxValue = 1;
                        levelSlider.value = 1;
                        tweenTrans.gameObject.SetActive(false);
                        maskTrans.gameObject.SetActive(false);
                        upObj.SetActive(false);
                    }
                    else
                    {
                        limitTx.enabled = false;
                        levelSlider.maxValue = nextCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence));
                        levelSlider.value = data.exp;
                        tweenTrans.gameObject.SetActive(true);
                        maskTrans.gameObject.SetActive(true);
                        maskTrans.anchorMax = new Vector2(levelSlider.value / levelSlider.maxValue, 1);
                        setTweenData(canAddExp, (int)levelSlider.maxValue);
                    }
                }
            }
        }
        else if (data.currentState == 1)
        {
            if (timerId == 0)
            {
                restTimeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                timerId = GameTimer.inst.AddTimer(1, () =>
                 {
                     if (data.remainTime <= 0)
                     {
                         GameTimer.inst.RemoveTimer(timerId);
                         timerId = 0;
                     }
                     else
                     {
                         restTimeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                     }
                 });
            }
        }
    }

    private void setTweenData(float canAddExp, int maxExp)
    {
        Color color = tweenImage.color;
        color.a = 0;
        tweenImage.color = color;
        tweenImage.DOFade(0.8f, 1.2f).SetLoops(-1);

        levelSlider.value += canAddExp;
        if (levelSlider.value >= maxExp)
        {
            levelSlider.value = maxExp;
            upObj.SetActive(true);
        }
        else
        {
            upObj.SetActive(false);
        }

        tweenTrans.anchorMax = new Vector2(levelSlider.value / levelSlider.maxValue, 1);
    }

    public void setAllTreatData()
    {
        itemType = kRoleItemType.RecoverHero;
        infoBtn.gameObject.SetActive(false);
        heroObj.SetActive(false);
        restObj.SetActive(true);
        grayObj.enabled = false;
        Item itemData = ItemBagProxy.inst.GetItem(150001);
        allRestItemCountText.text = itemData.count.ToString();
        allRestItemCountText.color = itemData.count >= 1 ? Color.white : Color.red;

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        var allRestingHero = RoleDataProxy.inst.GetRestingStateHeroCount();
        int result = 0;
        //int result = 0;
        for (int i = 0; i < allRestingHero.Count; i++)
        {
            int index = i;
            result += DiamondCountUtils.GetHeroRestingFastCost(allRestingHero[index].remainTime);
        }

        //result = DiamondCountUtils.GetHeroRestingFastCost(allRemainTime);
        allRestGemCountText.text = result.ToString("N0");
        allRestGemCountText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;


        timerId = GameTimer.inst.AddTimer(1, () =>
        {
            result = 0;
            for (int i = 0; i < allRestingHero.Count; i++)
            {
                result += DiamondCountUtils.GetHeroRestingFastCost(allRestingHero[i].remainTime);
            }
            //result = DiamondCountUtils.GetHeroRestingFastCost(allRemainTime);
            allRestGemCountText.text = result.ToString("N0");
            allRestGemCountText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
        });
    }

    private void setHeroHeadIcon()
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender, callback: system =>
            {
                graphicDressUp = system;
                system.transform.SetParent(headParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender);
        }
    }

    public void clearData()
    {

    }

    private void OnDisable()
    {
        DOTween.Kill(tweenImage);
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
