using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleEquipDamagedInfoView : ViewBase<RoleEquipDamagedInfoComp>
{
    public override string viewID => ViewPrefabName.RoleEquipDamagedInfoUI;
    public override string sortingLayerName => "window";
    List<HeroInfo> data;
    int curIndex = 0;
    EquipConfig curCfg;
    float goldPrice;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noRoleAndSettingAndEnergy;
        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.leftBtn.ButtonClickTween(() =>
        {
            if (curIndex <= 0) return;
            contentPane.allObjList[curIndex].iconImage.color = Color.white;
            curIndex -= 1;
            contentPane.allObjList[curIndex].iconImage.color = Color.green;
            setEquipInfoData();
        });

        contentPane.rightBtn.ButtonClickTween(() =>
        {
            if (curIndex >= data.Count - 1) return;
            contentPane.allObjList[curIndex].iconImage.color = Color.white;
            curIndex += 1;
            contentPane.allObjList[curIndex].iconImage.color = Color.green;
            setEquipInfoData();
        });

        contentPane.infoBtn.ButtonClickTween(() =>
        {
            setIntroduceData();
        });

        contentPane.discardBtn.ButtonClickTween(() =>
        {
            data.RemoveAt(curIndex);
            if (data.Count <= 0)
            {
                hide();
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
                return;
            }
            if (curIndex >= data.Count) curIndex = data.Count - 1;
            setEquipInfoData();
            setAllDamagedIcon();
        });

        contentPane.repairByMoney.ButtonClickTween(() =>
        {
            if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_BuyVipUI", 1);
            }
            if (goldPrice > UserDataProxy.inst.playerData.gold)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_REPAIREQUIP, data[curIndex].heroUid, (int)EItemType.Gold);
        });

        contentPane.repairByItem.ButtonClickTween(() =>
        {
            if (ItemBagProxy.inst.GetItem(130400).count < 1)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("修复道具不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_REPAIREQUIP, data[curIndex].heroUid, (int)EItemType.FixEquipItem);
        });

        contentPane.repairByGem.ButtonClickTween(() =>
        {
            if (DiamondCountUtils.GetExploreOrMakeEquipUpgradeDiamonds(curCfg.equipDrawingsConfig.production_time) > UserDataProxy.inst.playerData.gem)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_REPAIREQUIP, data[curIndex].heroUid, (int)EItemType.Gem);
        });
    }

    public void setData()
    {
        if (ExploreDataProxy.inst.currExploreData == null) return;

        data = ExploreDataProxy.inst.currExploreData.heroInfo.FindAll(t => t.brokenEquip.equipId != 0);
        contentPane.leftBtn.gameObject.SetActive(data.Count > 1);
        contentPane.rightBtn.gameObject.SetActive(data.Count > 1);

        setEquipInfoData();
        setAllDamagedIcon();
    }

    public void refreshBuyVip()
    {
        GUIHelper.SetUIGray(contentPane.huangguanTrans, (K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip);
    }

    private void setEquipInfoData()
    {
        int equipId = data[curIndex].brokenEquip.equipId;
        var cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipId);
        curCfg = cfg;
        var classCfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.equipDrawingsConfig.sub_type);
        contentPane.typeIcon.SetSprite(classCfg.Atlas, classCfg.icon);
        contentPane.nameText.text = cfg.name;
        contentPane.typeIcon.iconImage.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1]);
        contentPane.nameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1]);
        contentPane.icon.SetSpriteURL(cfg.equipDrawingsConfig.big_icon);
        contentPane.qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[cfg.equipQualityConfig.quality - 1]);
        GUIHelper.showQualiyIcon(contentPane.qualityIcon.GetComponent<RectTransform>(), cfg.equipQualityConfig.quality);
        goldPrice = cfg.equipQualityConfig.price_gold;

        var playerData = UserDataProxy.inst.playerData;
        if ((K_Vip_State)playerData.vipState == K_Vip_State.Vip)
        {
            int percentState = VipLevelConfigManager.inst.GetValByLevelAndType(playerData.vipLevel, K_Vip_Type.RepairEquipReduce);
            if (percentState != 0)
            {
                goldPrice = Mathf.CeilToInt(goldPrice * (1 - percentState / 100.0f));
            }
        }

        contentPane.moneyText.text = goldPrice.ToString();
        contentPane.gemText.text = cfg.equipQualityConfig.fixGem.ToString();
        contentPane.itemText.text = ItemBagProxy.inst.GetItem(130400).count.ToString();
        contentPane.moneyText.color = cfg.equipQualityConfig.price_gold > UserDataProxy.inst.playerData.gold ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");
        contentPane.gemText.color = cfg.equipQualityConfig.fixGem > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");
        contentPane.itemText.color = ItemBagProxy.inst.GetItem(130400).count < 1 ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");

        var qualityColor = StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1];
        var fxList = contentPane.qualityFx.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var item in fxList)
        {
            item.startColor = GUIHelper.GetColorByColorHex(qualityColor);
            item.Play();
        }

        contentPane.superNormalObj.SetActive(cfg.equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality);

        GUIHelper.SetUIGray(contentPane.huangguanTrans, (K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip);
    }

    private void setAllDamagedIcon()
    {
        for (int i = 0; i < contentPane.allObjList.Count; i++)
        {
            int index = i;
            if (index < data.Count)
            {
                contentPane.allObjList[index].gameObject.SetActive(true);
            }
            else
            {
                contentPane.allObjList[index].gameObject.SetActive(false);
            }
            contentPane.allObjList[index].iconImage.color = Color.white;
            contentPane.allObjList[index].iconImage.enabled = true;
        }
        contentPane.allObjList[curIndex].iconImage.color = Color.green;
    }

    private void setIntroduceData()
    {
        contentPane.introduce.gameObject.SetActive(true);
        contentPane.introduce.setInfoData(data[curIndex]);
    }

    public void ResponseRepairData()
    {
        if (data == null || data.Count <= 0) return;
        data.RemoveAt(curIndex);
        if (data.Count <= 0)
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
            return;
        }
        if (curIndex >= data.Count) curIndex = data.Count - 1;
        setEquipInfoData();
        setAllDamagedIcon();
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        AudioManager.inst.PlaySound(11);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();
    }
}
