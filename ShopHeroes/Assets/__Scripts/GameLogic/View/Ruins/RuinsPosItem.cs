using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuinsPosItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text fightText;
    public Transform heroTrans;
    public GUIIcon faceIcon;
    public Slider hpSlider;
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public Text levelText;
    public Button selfBtn;
    public GameObject addHeroObj;
    public GameObject heroObj;

    Tween tween;
    public Transform talkBg;
    public Text frontText;
    public Text backText;
    Tween talkTween = null;
    int talkTimer = 0;

    public int index;
    public RoleHeroData data;
    private DressUpSystem heroDress;

    System.Action<int, int> selfClickAction;

    Vector3 startPos;
    Vector3 startOffset;

    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            if (data == null) return;
            HotfixBridge.inst.TriggerLuaEvent("Ruins_RemoveHeroCom", data.uid, index);
        });
    }

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
                var tempItem = eventData.pointerEnter.transform.parent.gameObject.GetComponent<RuinsPosItem>();
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

        var heroItem = eventData.pointerEnter.GetComponent<RuinsPosItem>();
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

    private void changePos(RuinsPosItem heroItem)
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

        HotfixBridge.inst.TriggerLuaEvent("Ruins_SortAllHero");
    }

    public void CheckPosAndProfessionType()
    {
        if (data == null) return;
        if (index > 2 && data.config.type == 1)
        {
            setFaultPosTalkPopup(1);
            return;
        }
        else if (index <= 2 && data.config.type != 1)
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

    public void InitItem(int index)
    {
        this.index = index;
        addHeroObj.SetActive(true);
        heroObj.SetActive(false);
    }

    public void setData(RoleHeroData _data, string canvasSortingLayerName, int canvasSortingOrder)
    {
        if (_data == null) return;
        addHeroObj.SetActive(false);
        heroObj.SetActive(true);

        data = _data;
        fightText.text = data.fightingNum.ToString();
        int remainHp = data.remainHp;
        if(remainHp <= 0 && data.currentState == 0)
        {
            remainHp = data.attributeConfig.hp_basic;
        }
        if(data.currentState == 3)
        {
            remainHp = 0;
        }
        hpSlider.maxValue = data.attributeConfig.hp_basic;
        hpSlider.value = remainHp;
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        levelText.text = data.level.ToString();

        setHeroPrefabData(canvasSortingLayerName, canvasSortingOrder);
    }

    public void setFaceState(int suggestFight, int sumFight, int suggestTeamFight)
    {
        float faceResult = (float)data.fightingNum / suggestFight * 1000;
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

        fightText.color = data.fightingNum >= suggestFight ? GUIHelper.GetColorByColorHex("#51e0ff") : GUIHelper.GetColorByColorHex("#ff4a4a");

        faceIcon.SetSprite(StaticConstants.exploreAtlas, StaticConstants.heroFaceIconName[faceIndex]);
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

    public void clearHeroData()
    {
        heroObj.SetActive(false);
        //selfBtn.interactable = false;
        addHeroObj.SetActive(true);
        data = null;
        talkTween.Kill(true);
        if (talkTimer > 0)
        {
            GameTimer.inst.RemoveTimer(talkTimer);
            talkTimer = 0;
        }

        talkBg.gameObject.SetActive(false);
    }
}
