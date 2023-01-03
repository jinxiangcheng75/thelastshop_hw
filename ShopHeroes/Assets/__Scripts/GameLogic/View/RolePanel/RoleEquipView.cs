using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleEquipView : ViewBase<RoleEquipComp>
{
    public override string viewID => ViewPrefabName.RoleEquipUI;
    public override string sortingLayerName => "popup";

    HeroEquip equipData;
    RoleHeroData data;
    //RoleEquipItemUI curItem;
    int fightingSum = 0;
    protected override void onInit()
    {
        base.onInit();

        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.bgBtn.onClick.AddListener(closeMethod);
        contentPane.closeBtn.onClick.AddListener(closeMethod);
        contentPane.changeBtn.onClick.AddListener(() => downButtonClickHandler(1));
        contentPane.cancleBtn.onClick.AddListener(() => downButtonClickHandler(2));
        contentPane.lockBtn.onClick.AddListener(() =>
        {
            contentPane.lockImageObj.SetActive(!contentPane.lockImageObj.activeSelf);
            contentPane.changeBtn.interactable = !contentPane.changeBtn.interactable;
            lockEquip();
        });
        contentPane.leftBtn.ButtonClickTween(() =>
        {
            changeEquip(false);
        });
        contentPane.rightBtn.ButtonClickTween(() =>
        {
            changeEquip(true);
        });
    }

    public void lockEquip()
    {

    }

    private void changeEquip(bool isAdd)
    {
        int field = equipData.equipPosId + (isAdd ? 1 : -1);
        if (isAdd)
        {
            if (field > 6)
            {
                field = 1;
            }
        }
        else
        {
            if (field < 1)
            {
                field = 6;
            }
        }
        equipData = data.GetEquipByField(field);
        if (equipData != null && equipData.equipId != 0)
        {
            setEquipData(equipData, data.uid);
        }
        else
        {
            changeEquip(isAdd);
        }
    }

    private void closeMethod()
    {
        //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.uid);
        hide();
    }

    private void downButtonClickHandler(int type)
    {
        if (type == 1)
        {
            hide();

            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEWEAREQUIP_SHOWUI, HeroProfessionConfigManager.inst.GetTargetFieldEquipList(data.id, equipData.equipPosId).ToArray(), data.uid, equipData.equipPosId, 0);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_WEAREQUIP, data.uid, equipData.equipPosId, 1, equipData.equipId.ToString());
        }
    }

    // 改成 HeroEquip + heroUid
    public void setEquipData(HeroEquip equip, int heroUid)
    {
        //curItem = itemData;
        data = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        equipData = equip;
        var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(equip.equipId);
        EquipDrawingsConfig drawingCfg = equipCfg.equipDrawingsConfig;
        contentPane.fightingText.text = "";
        contentPane.equipIcon.SetSprite(drawingCfg.atlas, drawingCfg.icon);
        var equipQualityConfig = EquipConfigManager.inst.GetEquipQualityConfig(equip.equipId);
        contentPane.qualityBgIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[equipQualityConfig.quality - 1]);
        GUIHelper.showQualiyIcon(contentPane.qualityBgIcon.GetComponent<RectTransform>(), equipQualityConfig.quality);
        contentPane.nameText.text = equipCfg.name;
        contentPane.nameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equipQualityConfig.quality - 1]);
        contentPane.qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityicon[equipQualityConfig.quality - 1]);
        contentPane.qualityText.text = LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[equipQualityConfig.quality - 1])/* + LanguageManager.inst.GetValueByKey("品质");*/;
        contentPane.qualityText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equipQualityConfig.quality - 1]);
        contentPane.classText.text = LanguageManager.inst.GetValueByKey("{0}阶", drawingCfg.level.ToString()) + LanguageManager.inst.GetValueByKey(StaticConstants.roleEquipSubType[drawingCfg.sub_type - 1]);
        float abProb = RoleDataProxy.inst.GetEquipDamagerVal(equipQualityConfig.quality) / 100.0f;
        //if()
        float damagePercent = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.equip_damageRaceDown);
        abProb = abProb - abProb * damagePercent;
        contentPane.damageText.text = abProb + "%";
        //tempItem.level + LanguageManager.inst.GetValueByKey("阶") + LanguageManager.inst.GetValueByKey(StaticConstants.roleEquipSubType[tempItem.sub_type - 1]);
        //contentPane.typeText.text = StaticConstants.roleEquipSubType[tempItem.sub_type - 1];

        contentPane.superNormalObj.SetActive(equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality);

        contentPane.ItemIconBg.SetSprite("__common_1", equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        calculateFightingNum();
        JudgeIsShowUp();
    }

    private void calculateFightingNum()
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(equipData.equipId);
        int sum = 0;
        int valCount = 0;
        for (int i = 0; i < contentPane.allProperty.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                contentPane.allProperty[index].gameObject.SetActive(true);
                contentPane.allProperty[index].valText.text = "+" + propertyList[index].ToString();
                sum += Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(200 + index).parameters * propertyList[index]);
                valCount++;
            }
            else
            {
                contentPane.allProperty[index].gameObject.SetActive(false);
            }
        }

        fightingSum = sum;
        contentPane.fightingText.text = sum.ToString();

        if (valCount <= 4)
        {
            contentPane.contentRect.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            contentPane.contentRect.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contentPane.contentRect.transform.parent.GetComponent<RectTransform>().rect.height);
        }
        else
        {
            contentPane.contentRect.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private void JudgeIsShowUp()
    {
        int maxLevel = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level).equip_lv;
        var curEquipData = EquipConfigManager.inst.GetEquipQualityConfig(equipData.equipId);
        var currOnShelfEquips = ItemBagProxy.inst.GetEquipItemsByTypeAndMaxLevel(false, HeroProfessionConfigManager.inst.GetTargetFieldEquipList(data.id, equipData.equipPosId).ToArray(), maxLevel);

        //currOnShelfEquips = currOnShelfEquips.FindAll(t => t.equipConfig.equipDrawingsConfig.level <= data.level);

        bool haveBest = false;
        for (int i = 0; i < currOnShelfEquips.Count; i++)
        {
            if(currOnShelfEquips[i].equipConfig.equipDrawingsConfig.level <= data.level)
            {
                if (curEquipData.GetEquipFightingSum(data.talentConfig) < currOnShelfEquips[i].GetEquipFightingSum(data.talentConfig))
                {
                    haveBest = true;
                    break;
                }
            }
        }

        if (haveBest)
        {
            contentPane.newObj.SetActive(true);
        }
        else
        {
            contentPane.newObj.SetActive(false);
        }
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
