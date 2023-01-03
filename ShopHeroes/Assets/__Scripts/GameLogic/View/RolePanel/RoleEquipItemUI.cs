using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleEquipItemUI : MonoBehaviour
{
    public GameObject wearObj;
    public GameObject noWearObj;
    public GameObject lockObj;
    public Button equipBtn;

    [Header("穿装备的条件下组件")]
    public GUIIcon qualityIcon;
    public GUIIcon equipIcon;
    public Text classText;
    public GUIIcon abrasionIcon;
    public Text abrasionText;
    public GameObject redPointObj;
    public RectTransform qualityBgIcon;
    public GameObject superNormalObj;

    [Header("未穿装备的条件下组件")]
    public List<GUIIcon> allIcons;

    public int _heroUId;
    public int _fieldId;
    public List<int> _curDicList;
    HeroEquip equip;
    RoleHeroData roleData;
    public int _equipId;
    bool isWearEvent;
    int canClick; // 3 - 未解锁 0 1 2 - heroState

    public System.Action clickHandler;

    private void Awake()
    {
        equipBtn.ButtonClickTween(setEquipBtnEvent);
    }

    public void setWearEquipData(HeroEquip equip, int heroId, int fieldId, List<int> equipIds, int canClick, bool hasBest)
    {
        roleData = RoleDataProxy.inst.GetHeroDataByUid(heroId);
        this.canClick = canClick;
        _heroUId = heroId;
        _fieldId = fieldId;
        _equipId = equip.equipId;
        this.equip = equip;
        _curDicList = equipIds;
        isWearEvent = true;
        wearObj.SetActive(true);
        noWearObj.SetActive(false);
        lockObj.SetActive(false);
        EquipDrawingsConfig tempItem = EquipConfigManager.inst.GetEquipDrawingsCfgByEquipId(equip.equipId);
        classText.text = tempItem.level.ToString();
        var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equip.equipId);
        qualityIcon.SetSprite(StaticConstants.roleAtlasName, /*StaticConstants.roleEquipQualityIconName[equipCfg.quality - 1]*/"yingxiong_xiangqzbk");
        GUIHelper.showQualiyIcon(qualityBgIcon.GetComponent<RectTransform>(), equipCfg.quality);
        string qcolor = equipCfg.quality > 1 ? StaticConstants.qualityColor[equipCfg.quality - 1] : "";
        equipIcon.SetSprite(tempItem.atlas, tempItem.icon, qcolor);
        redPointObj.SetActive(hasBest);
        float abProb = RoleDataProxy.inst.GetEquipDamagerVal(equipCfg.quality) / 100.0f;
        //if()
        float damagePercent = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.equip_damageRaceDown);
        abProb = abProb - abProb * damagePercent;
        abrasionText.text = abProb + "%";
        superNormalObj.SetActive(equipCfg.quality > StaticConstants.SuperEquipBaseQuality);
    }

    public void setNotWearEquipData(List<int> equipIds, int heroId, int fieldId, int canClick, bool hasEquip)
    {
        roleData = RoleDataProxy.inst.GetHeroDataByUid(heroId);
        this.canClick = canClick;
        _curDicList = equipIds;
        _heroUId = heroId;
        _fieldId = fieldId;
        isWearEvent = false;
        wearObj.SetActive(false);
        noWearObj.SetActive(true);
        lockObj.SetActive(false);
        qualityIcon.SetSprite(StaticConstants.roleAtlasName, /*StaticConstants.roleEquipQualityIconName[StaticConstants.roleEquipQualityIconName.Length - 1]*/"yingxiong_xiangqzbk1");
        redPointObj.SetActive(hasEquip);
        for (int i = 0; i < allIcons.Count; i++)
        {
            int index = i;
            if (index < equipIds.Count)
            {
                EquipClassification tempData = EquipConfigManager.inst.GetEquipTypeByID(equipIds[index]);
                allIcons[index].gameObject.SetActive(true);
                allIcons[index].SetSprite(tempData.Atlas, tempData.icon);
            }
            else
            {
                allIcons[index].gameObject.SetActive(false);
            }
        }
    }

    public void setLockData()
    {
        canClick = 3;
        wearObj.SetActive(false);
        noWearObj.SetActive(false);
        lockObj.SetActive(true);
        qualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleEquipQualityIconName[5]);
        redPointObj.SetActive(false);
    }

    private void setEquipBtnEvent()
    {
        if (canClick == 3)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("未解锁该装备槽"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }
        if (canClick == 2)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("探索中不可切换装备"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }
        if (isWearEvent)
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEEQUIP_SHOWUI, equip, _heroUId);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEWEAREQUIP_SHOWUI, _curDicList.ToArray(), _heroUId, _fieldId, 0);
        }

        clickHandler?.Invoke();
    }
}
