using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ExploreHeroItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Button selfBtn;
    public Button addBtn;
    public GameObject heroObj;
    public Transform heroTrans;
    public Transform talkTrans;
    public Text fightingText;
    public GUIIcon faceIcon;
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public Text levelText;
    public Slider levelSlider;
    public RectTransform tweenTrans;
    Image tweenImage;
    Tween tween;
    public RectTransform maskTrans;
    public GameObject upObj;
    public int fightingCount;
    RoleHeroData data;
    public DressUpSystem heroDress;

    public Transform talkBg;
    public Text frontText;
    public Text backText;

    public int indexState;

    Tween talkTween = null;
    int talkTimer = 0;

    Vector3 startPos;
    Vector3 startOffset;

    public RoleHeroData Data
    {
        get { return data; }
    }
    public int index;
    System.Action<int> itemAddClickAction;
    System.Action<int, int> selfClickAction;

    public void OnBeginDrag(PointerEventData eventData)
    {
        selfBtn.interactable = false;
        startPos = heroObj.transform.localPosition;
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(heroObj.transform.position);
        startOffset = heroObj.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (data == null || !heroObj.activeSelf) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(heroObj.transform.position);
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
        heroObj.transform.position = Camera.main.ScreenToWorldPoint(mouseScreenPos) + startOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        selfBtn.interactable = true;
        if (eventData == null) return;
        if (eventData.pointerEnter == null)
        {
            heroObj.transform.localPosition = startPos;
            return;
        }
        if (eventData.pointerEnter.name == "addHeroBtn")
        {
            if (eventData.pointerEnter.transform.parent != null)
            {
                var tempItem = eventData.pointerEnter.transform.parent.gameObject.GetComponent<ExploreHeroItemView>();
                if (tempItem == null)
                {
                    heroObj.transform.localPosition = startPos;
                }
                else
                {
                    changePos(tempItem);
                }
            }
            else
            {
                heroObj.transform.localPosition = startPos;
            }
            return;
        }

        var heroItem = eventData.pointerEnter.GetComponent<ExploreHeroItemView>();
        if (heroItem == null)
        {
            heroObj.transform.localPosition = startPos;
            return;
        }
        if (heroItem.data == null)
        {
            //heroObj.transform.localPosition = startPos;
            changePos(heroItem);
            return;
        }
        if (heroItem.index != index)
        {
            changePos(heroItem);
        }
        else
        {
            heroObj.transform.localPosition = startPos;
        }
    }

    private void changePos(ExploreHeroItemView heroItem)
    {
        var tempPos = transform.localPosition;
        var tempIndex = index;
        transform.localPosition = heroItem.transform.localPosition;
        heroObj.transform.localPosition = new Vector3(0, -69);
        heroItem.transform.localPosition = tempPos;
        index = heroItem.index;
        heroItem.index = tempIndex;

        CheckPosAndProfessionType();
        heroItem.CheckPosAndProfessionType();

        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REFRESH_SORTHEROLIST);
        HotfixBridge.inst.TriggerLuaEvent("Refresh_SortHeroList");
    }

    public void CheckPosAndProfessionType()
    {
        if (data == null) return;
        if (index >= 2 && data.config.type == 1)
        {
            setFaultPosTalkPopup(1);
            return;
        }
        else if (index <= 1 && data.config.type != 1)
        {
            setFaultPosTalkPopup(2);
            return;
        }

        if (talkBg.gameObject.activeSelf)
        {
            talkTween.Kill(true);
            if (talkTimer > 0)
            {
                GameTimer.inst.RemoveTimer(talkTimer);
                talkTimer = 0;
            }

            talkBg.gameObject.SetActive(false);
        }
    }

    public void setFaultPosTalkPopup(int type)// 1-武士在后排 2-学者猎人在前排
    {
        frontText.enabled = type == 1;
        backText.enabled = type == 2;

        talkTween.Kill(true);
        if (talkTimer > 0)
        {
            GameTimer.inst.RemoveTimer(talkTimer);
            talkTimer = 0;
        }

        talkTween = talkBg.DOScale(new Vector3(1, -1, 1), 0.5f).From(new Vector3(0, 0, 0)).SetEase(Ease.OutBack).OnComplete(() =>
             {
                 talkTimer = GameTimer.inst.AddTimer(2, 1, () =>
                   {
                       talkBg.DOScale(new Vector3(0, 0, 0), 0.5f).From(new Vector3(1, -1, 1)).SetEase(Ease.InBack).OnComplete(() =>
                       {
                           talkBg.gameObject.SetActive(false);
                       });
                   });

             }).OnStart(() =>
             {
                 talkBg.gameObject.SetActive(true);
             });
    }

    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            //Logger.error("click Self");
            //EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREREMOVEHERO_COM, data.uid, index);
            if (data == null) return;
            selfClickAction?.Invoke(data.uid, index);
        });

        addBtn.onClick.AddListener(() =>
        {
            itemAddClickAction?.Invoke(index);
            //EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREHERO_SHOWUI, index);
        });

        tweenImage = tweenTrans.GetComponent<Image>();
    }

    public void setData(int index, System.Action<int> clickHandler, System.Action<int, int> selfClickHander)
    {
        this.index = index;
        heroObj.SetActive(false);
        //selfBtn.interactable = false;
        addBtn.gameObject.SetActive(true);
        itemAddClickAction = clickHandler;
        selfClickAction = selfClickHander;
    }

    private void setHeroPrefabData(string canvasSortingLayerName, int canvasSortingOrder)
    {
        if (heroDress == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress), callback: (dress) =>
            {
                heroDress = dress;
                heroDress.SetUIPosition(heroTrans, canvasSortingLayerName, canvasSortingOrder + 1, 0.3f);
                heroShowAnim();
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByHero(heroDress, (EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress));
            heroDress.SetUIPosition(heroTrans, canvasSortingLayerName, canvasSortingOrder + 1, 0.3f);
            heroShowAnim();
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
                        heroDress.Play(action.act_combat_standby, true);
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
                    heroDress.Play(action.act_combat_standby, true);
                }
            });
        }
    }

    public void setHeroData(RoleHeroData _data, string canvasSortingLayerName, int canvasSortingOrder, float canAddExp)
    {
        data = _data;
        heroObj.SetActive(true);
        addBtn.gameObject.SetActive(false);
        //selfBtn.interactable = true;
        fightingCount = data.fightingNum;
        fightingText.text = data.fightingNum.ToString();
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        levelText.text = data.level.ToString();
        typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);

        setHeroPrefabData(canvasSortingLayerName, canvasSortingOrder);
        //setHeroFighit(addition, suggestFight);
        setHeroTween(canAddExp);
        setHeroEffect(data.talentConfig);
    }

    public void setHeroEffect(HeroTalentDataBase talentCfg)
    {

    }

    public void setHeroTween(float canAddExp)
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
                    levelSlider.maxValue = 1;
                    levelSlider.value = 1;
                    tweenTrans.gameObject.SetActive(false);
                    maskTrans.gameObject.SetActive(false);
                    upObj.SetActive(false);
                }
                else
                {
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

    private void setTweenData(float canAddExp, int maxExp)
    {
        DOTween.Kill(tweenTrans);
        tween.Kill();
        //DOTween.Kill(tweenImage);
        //Color color = tweenImage.color;
        //color.a = 0;
        //tweenImage.color = color;
        //tweenImage.DOFade(0.8f, 1.2f).SetLoops(-1);

        float result = levelSlider.value + canAddExp;
        //levelSlider.value += canAddExp;
        if (/*levelSlider.value*/result >= maxExp)
        {
            //levelSlider.value = maxExp;
            result = maxExp;
            upObj.SetActive(true);
        }
        else
        {
            upObj.SetActive(false);
        }

        tweenTrans.DOAnchorMax(new Vector2(result / levelSlider.maxValue, 1), 0.8f).OnComplete(() =>
        {
            tweenTrans.DOAnchorMax(new Vector2(levelSlider.value / levelSlider.maxValue, 1), 0).SetDelay(0.3f);
        });

        float temp = 0;
        tween = DOTween.To(() => temp, x => temp = x, 1, 1.6f).OnStepComplete(() =>
           {
               tweenTrans.DOAnchorMax(new Vector2(result / levelSlider.maxValue, 1), 0.8f).OnComplete(() =>
               {
                   tweenTrans.DOAnchorMax(new Vector2(levelSlider.value / levelSlider.maxValue, 1), 0).SetDelay(0.3f);
               });
           }).SetLoops(-1);

        //tweenTrans.anchorMax = new Vector2(/*levelSlider.value*/result / levelSlider.maxValue, 1);
    }

    public void setHeroFightCount(float addition)
    {
        int addNum = addition != -1 ? Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(201).parameters * data.attributeConfig.atk_basic * addition) : 0;
        fightingCount = data.fightingNum + addNum;
    }

    public void setHeroFighit(float addition, int suggestFight, int sumFight, int suggestTeamFight)
    {
        int tempFight = fightingCount;
        
        float faceResult = (float)fightingCount / suggestFight * 1000;
        float teamFaceResult = (float)sumFight / suggestTeamFight * 1000;
        int faceIndex = 0;

        if (teamFaceResult <= WorldParConfigManager.inst.GetConfig(408).parameters)
        {
            faceIndex = 2;
        }
        else if (teamFaceResult > WorldParConfigManager.inst.GetConfig(408).parameters && teamFaceResult <= WorldParConfigManager.inst.GetConfig(409).parameters)
        {
            faceIndex = 1;
        }
        else if (teamFaceResult > WorldParConfigManager.inst.GetConfig(409).parameters && teamFaceResult <= WorldParConfigManager.inst.GetConfig(410).parameters)
        {
            faceIndex = 0;
        }

        if (faceIndex < 2)
        {
            if (faceResult <= WorldParConfigManager.inst.GetConfig(405).parameters) // 红脸
            {
                faceIndex = 2;
            }
            else if (faceResult > WorldParConfigManager.inst.GetConfig(405).parameters && faceResult <= WorldParConfigManager.inst.GetConfig(406).parameters) // 黄脸
            {
                faceIndex = 1;
            }
            else if (faceResult >= WorldParConfigManager.inst.GetConfig(407).parameters) // 绿脸
            {
                faceIndex = faceIndex == 1 ? 1 : 0;
            }
        }

        fightingText.color = fightingCount >= suggestFight ? GUIHelper.GetColorByColorHex("#51e0ff") : GUIHelper.GetColorByColorHex("#ff4a4a");

        faceIcon.SetSprite(StaticConstants.exploreAtlas, StaticConstants.heroFaceIconName[faceIndex]);

        DOTween.To(() => tempFight, x => tempFight = x, fightingCount, 0.8f).OnUpdate(() =>
        {
            fightingText.text = tempFight.ToString();
        });
    }

    public void setIndexState(int state) // 0 - 槽位开了 -1 - 槽位没开
    {
        indexState = state;
    }

    public void clearHeroData()
    {
        heroObj.SetActive(false);
        //selfBtn.interactable = false;
        addBtn.gameObject.SetActive(true);
        data = null;
        DOTween.Kill(tweenImage);
        talkTween.Kill(true);
        if (talkTimer > 0)
        {
            GameTimer.inst.RemoveTimer(talkTimer);
            talkTimer = 0;
        }

        talkBg.gameObject.SetActive(false);
    }
}
