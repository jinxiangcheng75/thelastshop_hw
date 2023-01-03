using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class RecruitItemUI : MonoBehaviour
{
    public Button selfBtn;

    // 正面
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public GUIIcon bgIcon;
    public Text nameText;
    public Transform roleTrans;
    public Text intelligenceText;
    public Text lifeText;
    public Text attText;
    public Text armorText;
    public Text speedText;
    public GameObject freeObj;
    public Transform freeFrontTrans;
    public GameObject recruitDoneObj;
    public GameObject canNotRecruitObj;
    public Button recruitBtn;
    private DressUpSystem heroDress;
    public Text recruitText;
    public GUIIcon intelligenceIcon;
    public List<GUIIcon> equipType;

    // 背面
    public Button skillBtn;
    public GUIIcon backBgIcon;
    public Text curDescText;
    public GUIIcon curSkillIcon;
    public Text curSkillNameText;
    public Text curHp;
    public Text curAtk;
    public Text curDef;
    public Text highRarityObj;

    public GUIIcon randomIcon;
    public GameObject bottomObj;

    public GUIIcon heroPosIcon;
    RectTransform heroPosRect;

    private RoleRecruitData data;
    private int index;
    public Action<RoleRecruitData, int> recruitClickHandler;

    kRoleRecruitDirec cardState = kRoleRecruitDirec.Front;
    bool isRotating;

    private void Awake()
    {
        if (heroPosIcon != null)
        {
            heroPosRect = heroPosIcon.GetComponent<RectTransform>();
        }
        selfBtn.onClick.AddListener(() =>
        {
            if (RoleDataProxy.inst.costValue <= 0) return;
            if (isRotating) return;
            isRotating = true;
            if (cardState == kRoleRecruitDirec.Front)
            {
                bgIcon.transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
                {
                    backBgIcon.transform.DORotate(new Vector3(0, 180, 0), 0.25f).OnComplete(() =>
                       {
                           cardState = kRoleRecruitDirec.Back;
                           isRotating = false;
                       });
                });
            }
            else
            {
                backBgIcon.transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
                {
                    bgIcon.transform.DORotate(new Vector3(0, 0, 0), 0.25f).OnComplete(() =>
                    {
                        cardState = kRoleRecruitDirec.Front;
                        isRotating = false;
                    });
                });
            }
        });

        skillBtn.onClick.AddListener(() => EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINTRODUCE_SHOWUI, skillBtn.transform, data.skillCfg));
        recruitBtn.onClick.AddListener(() => recruitClickHandler?.Invoke(data, index));
    }

    public void setData(RoleRecruitData heroData, int index, string canvasSortingLayer, int canvasOrder)
    {
        if (heroData == null)
        {
            Logger.error("招募英雄数据是空");
            return;
        }
        if (cardState == kRoleRecruitDirec.Back)
        {
            cardState = kRoleRecruitDirec.Front;
            bgIcon.transform.localEulerAngles = new Vector3(0, 0, 0);
            backBgIcon.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
        data = heroData;
        this.index = index;

        if (!isRefreshing)
        {
            if (!bottomObj.activeSelf)
                bottomObj.SetActive(true);
        }

        freeObj.SetActive(RoleDataProxy.inst.costValue <= 0);
        selfBtn.gameObject.SetActive(RoleDataProxy.inst.costValue > 0);
        canNotRecruitObj.SetActive(RoleDataProxy.inst.costValue <= 0);

        if ((ERecruitState)data.recruitState == ERecruitState.NotRecruited)
        {
            if (RoleDataProxy.inst.costValue <= 0)
            {
                recruitDoneObj.SetActive(false);
                recruitBtn.gameObject.SetActive(false);
            }
            else
            {
                recruitDoneObj.SetActive(false);
                recruitBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            recruitBtn.gameObject.SetActive(false);
            recruitDoneObj.SetActive(true);
        }

        setCardFrontData();
        setCardBackData();
        //SetHeroPrefabData(canvasSortingLayer, canvasOrder);
        setEquipTypeIcon();
        JudgeIsFree();
    }

    private void setCardFrontData()
    {
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        string qualityColor = StaticConstants.roleIntelligenceColor[rarity];
        bgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleRecruitBgIconName[rarity - 1]);
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.professionCfg.type - 1]);
        typeIcon.SetSprite(data.professionCfg.atlas, data.professionCfg.ocp_icon);
        nameText.text = LanguageManager.inst.GetValueByKey(data.professionCfg.name);
        intelligenceText.text = data.intelligence.ToString();
        intelligenceText.color = GUIHelper.GetColorByColorHex(qualityColor);
        lifeText.text = data.attributeConfig.hp_basic.ToString();
        attText.text = data.attributeConfig.atk_basic.ToString();
        armorText.text = data.attributeConfig.def_basic.ToString();
        speedText.text = data.attributeConfig.spd_basic.ToString();
        intelligenceIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);
        highRarityObj.enabled = (rarity == 3 || rarity == 4);
        if (rarity == 3)
            highRarityObj.text = LanguageManager.inst.GetValueByKey("招募必出<Color=#c617ff>史诗天赋</Color>");
        else if(rarity == 4)
            highRarityObj.text = LanguageManager.inst.GetValueByKey("招募必出<Color=#ff911c>传奇天赋</Color>");
        string posIconName = data.gender == 1 ? "m_" + data.professionCfg.ocp_icon : "w_" + data.professionCfg.ocp_icon;
        heroPosIcon.SetSpriteURL(posIconName, needSetNativeSize: true);
        if (heroPosRect != null)
        {
            if (data.professionCfg.type != 1)
            {
                heroPosRect.anchoredPosition = new Vector2(0, heroPosRect.anchoredPosition.y);
            }
            else
            {
                heroPosRect.anchoredPosition = new Vector2(-20, heroPosRect.anchoredPosition.y);
            }
        }
    }

    private void setCardBackData()
    {
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        backBgIcon.SetSprite(StaticConstants.roleAtlasName, "yingxiong_zhaomudi5");
        curDescText.text = LanguageManager.inst.GetValueByKey(data.professionCfg.ocp_story);
        var skillCfg = HeroSkillShowConfigManager.inst.GetConfig(data.professionCfg.id_skill1);
        curSkillIcon.SetSprite(skillCfg.skill_atlas, skillCfg.skill_icon);
        curSkillNameText.text = LanguageManager.inst.GetValueByKey(skillCfg.skill_name);
        curHp.text = data.attributeConfig.hp_basic.ToString();
        curAtk.text = data.attributeConfig.atk_basic.ToString();
        curDef.text = data.attributeConfig.def_basic.ToString();
    }

    private void setEquipTypeIcon()
    {
        var cfg = HeroProfessionConfigManager.inst.GetConfig(data.heroId);
        List<int> ids = new List<int>();
        foreach (var item in cfg.equip1)
        {
            ids.Add(item);
        }
        foreach (var item in cfg.equip2)
        {
            ids.Add(item);
        }

        for (int i = 0; i < equipType.Count; i++)
        {
            int index = i;
            if (index < ids.Count)
            {
                EquipClassification tempClass = EquipConfigManager.inst.GetEquipTypeByID(ids[index]);
                equipType[index].gameObject.SetActive(true);
                equipType[index].SetSprite(tempClass.Atlas, tempClass.icon);
            }
            else
            {
                equipType[index].gameObject.SetActive(false);
            }
        }
    }

    private void SetHeroPrefabData(string canvasSortingLayer, int canvasOrder)
    {
        if (heroDress == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress), callback: (dress) =>
            {
                heroDress = dress;
                heroDress.SetUIPosition(roleTrans, canvasSortingLayer, canvasOrder, 0.625f);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
                heroDress.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByHero(heroDress, (EGender)data.gender, data.GetAllWearEquipId(), SpineUtils.RoleDressToUintList(data.roleDress));

            heroDress.SetUIPosition(roleTrans, canvasSortingLayer, canvasOrder, 0.625f);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)heroDress.gender, (int)kIndoorRoleActionType.normal_standby);
            heroDress.Play(idleAnimationName, true);
        }
    }

    private void JudgeIsFree()
    {
        recruitText.text = LanguageManager.inst.GetValueByKey(UserDataProxy.inst.playerData.heroBuyFreeCount > 0 ? "免费招募" : "招募");
        //freeText.SetActive(UserDataProxy.inst.playerData.heroBuyFreeCount > 0);
    }

    public void setFreeItem()
    {
        freeObj.SetActive(true);
        canNotRecruitObj.SetActive(true);
        recruitDoneObj.SetActive(false);
        recruitBtn.gameObject.SetActive(false);
    }

    int rotateCount = 0;
    float maxDurationTime = 1.0f;
    bool isRefreshing = false;

    public void setRefreshAnim(Action animComp = null)
    {
        isRefreshing = true;
        selfBtn.gameObject.SetActive(false);
        bottomObj.SetActive(false);
        freeObj.SetActive(true);
        freeFrontTrans.eulerAngles = new Vector3(0, 0, 0);
        randomIcon.transform.eulerAngles = new Vector3(0, 90, 0);
        setAnimParam(0.22f, 5, animComp);
    }

    Tween tween_a, tween_b, tween_c, tween_d;

    private void setAnimParam(float duration, int loopCount, Action animComp = null)
    {
        rotateCount += 1;
        tween_a = freeFrontTrans.DORotate(new Vector3(0, 90, 0), duration / 4.0f).OnComplete(() =>
        {
            int randomNumber = UnityEngine.Random.Range(0, 4);
            randomIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleRecruitBgIconName[randomNumber]);
            if (duration >= maxDurationTime || loopCount <= 1)
            {
                animComp?.Invoke();
            }
            tween_b = randomIcon.transform.DORotate(new Vector3(0, 180, 0), duration / 4.0f).OnComplete(() =>
            {
                if (duration >= maxDurationTime || loopCount <= 1)
                {
                    rotateCount = 0;
                    selfBtn.gameObject.SetActive(true);
                    bottomObj.SetActive(true);
                    freeObj.SetActive(false);
                    isRefreshing = false;
                }
                else
                {
                    tween_c = randomIcon.transform.DORotate(new Vector3(0, 90, 0), duration / 4.0f).OnComplete(() =>
                    {
                        tween_d = freeFrontTrans.DORotate(new Vector3(0, 0, 0), duration / 4.0f).OnComplete(() =>
                        {
                            setAnimParam(duration + 0.07f * rotateCount, loopCount - 1, animComp);
                        });
                    });
                }
            });
        });
    }

    public void ClearAnim()
    {

        selfBtn.gameObject.SetActive(true);
        bottomObj.SetActive(true);
        freeObj.SetActive(false);
        isRefreshing = false;

        if (tween_a != null)
        {
            tween_a.Kill();
            tween_a = null;
        }
        if (tween_b != null)
        {
            tween_b.Kill();
            tween_b = null;
        }
        if (tween_c != null)
        {
            tween_c.Kill();
            tween_c = null;
        }
        if (tween_d != null)
        {
            tween_d.Kill();
            tween_d = null;
        }
    }

    private void OnDestroy()
    {
        tween_a = null;
        tween_b = null;
        tween_c = null;
        tween_d = null;
    }

}
