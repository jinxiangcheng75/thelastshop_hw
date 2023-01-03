using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerRecruitUI : ViewBase<WorkerRecruitUIComp>
{
    public override string sortingLayerName => "window";
    public override string viewID => ViewPrefabName.WorkerRecruitUI;

    GraphicDressUpSystem graphicDressUpSystem;

    protected override void onInit()
    {

        isShowResPanel = true;

        contentPane.leftPageBtn.onClick.AddListener(() => turnPage(true));
        contentPane.rightPageBtn.onClick.AddListener(() => turnPage(false));

        contentPane.goldBtn.ButtonClickTween(() => recruitWorker(EItemType.Gold));
        contentPane.gemBtn.ButtonClickTween(() =>
        {
            if (contentPane.gemConfirmObj.activeSelf)
            {
                recruitWorker(EItemType.Gem);
            }
            else
            {
                contentPane.gemConfirmObj.SetActiveTrue();
            }
        });

        contentPane.otherBtn.ButtonClickTween(onOtherBtnClick);
        contentPane.closeBtn.ButtonClickTween(hide);
    }

    WorkerData _data;
    System.Action _callback;
    public void SetData(WorkerData data, bool showNear, System.Action callback)
    {
        this._data = data;
        WorkerConfig _workerCfg = _data.config;
        _callback = callback;

        if (graphicDressUpSystem == null)
        {
            CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(_workerCfg.model, callback: (sys) =>
            {
                graphicDressUpSystem = sys;
                graphicDressUpSystem.transform.SetParent(contentPane.workerTf);
                graphicDressUpSystem.transform.localScale = Vector3.one;
                graphicDressUpSystem.transform.localPosition = Vector3.zero;
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
                graphicDressUpSystem.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(graphicDressUpSystem, _workerCfg.model);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
            graphicDressUpSystem.Play(idleAnimationName, true);
        }


        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(_workerCfg.name);
        contentPane.typeText.text = LanguageManager.inst.GetValueByKey(_workerCfg.profession);
        contentPane.contentText.text = LanguageManager.inst.GetValueByKey(_workerCfg.desc);

        int[] itemDatas = _workerCfg.equipment_id;

        for (int i = 0; i < contentPane.unlockItems.Count; i++)
        {
            if (i >= itemDatas.Length)
            {
                contentPane.unlockItems[i].SetActiveFalse();
            }
            else
            {
                contentPane.unlockItems[i].SetActiveTrue();
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemDatas[i]);
                contentPane.unlockItemIcons[i].SetSprite(equipCfg.atlas, equipCfg.icon);

                contentPane.unlockItemBtns[i].onClick.RemoveAllListeners();
                contentPane.unlockItemBtns[i].onClick.AddListener(() =>
                {
                    //EquipConfig cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipCfg.id, 1);
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, "", cfg.equipQualityConfig.id, new List<EquipItem>());
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUIBYDRAWINGID, equipCfg.id);
                });
            }
        }

        contentPane.needLvTx.text = _workerCfg.level.ToString();
        contentPane.needLvTx.color = _workerCfg.level > UserDataProxy.inst.playerData.level ? GUIHelper.GetColorByColorHex("FF0000") : Color.white;
        contentPane.goldCostTx.text = _workerCfg.cost_money.ToString("N0");
        contentPane.goldCostTx.color = _workerCfg.cost_money > UserDataProxy.inst.playerData.gold ? GUIHelper.GetColorByColorHex("FF0000") : Color.white;
        contentPane.gemCostTx.text = _workerCfg.cost_diamond.ToString("N0");
        //contentPane.gemCostTx.color = _workerCfg.cost_diamond > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FF0000") : Color.white;

        contentPane.notArriveLv.SetActive(_workerCfg.level > UserDataProxy.inst.playerData.level);
        contentPane.arriveLv.enabled = _workerCfg.level <= UserDataProxy.inst.playerData.level;

        bool active = showNear ? _data.state == EWorkerState.CanUnlock && RoleDataProxy.inst.WorkerList.FindAll(t => t.state == EWorkerState.CanUnlock).Count > 1 : false;
        contentPane.leftPageBtn.gameObject.SetActive(active);
        contentPane.rightPageBtn.gameObject.SetActive(active);


        setBottomInfo();
    }


    void setBottomInfo()
    {

        if (_data.state == EWorkerState.Locked)
        {
            contentPane.goldBtn.gameObject.SetActive(false);
            contentPane.gemBtn.gameObject.SetActive(false);
            contentPane.otherBtn.gameObject.SetActive(false);
            contentPane.orTx.enabled = false;
            contentPane.lockedTx.enabled = true;

            if (UserDataProxy.inst.playerData.level < _data.config.level)
            {
                contentPane.lockedTx.text = LanguageManager.inst.GetValueByKey("需要店主达到{0}级", _data.config.level.ToString());
            }
            else
            {
                if (_data.config.get_type == (int)kRoleWorkerGetType.buildingLink)
                {
                    var buildingCfg = BuildingConfigManager.inst.GetConfig(_data.config.build_id);
                    contentPane.lockedTx.text = LanguageManager.inst.GetValueByKey("需要{0}达到{1}级", LanguageManager.inst.GetValueByKey(buildingCfg.name), _data.config.build_level_id.ToString());
                }
                else if (_data.config.get_type == (int)kRoleWorkerGetType.giftLink)
                {
                    contentPane.lockedTx.text = LanguageManager.inst.GetValueByKey("需要等待礼包活动开启");
                }
            }

        }
        else if (_data.state == EWorkerState.CanUnlock)
        {
            bool btnActive = (kRoleWorkerGetType)_data.config.get_type == kRoleWorkerGetType.buy || (kRoleWorkerGetType)_data.config.get_type == kRoleWorkerGetType.buildingLink;

            contentPane.lockedTx.enabled = false;
            contentPane.goldBtn.gameObject.SetActive(btnActive);
            contentPane.gemBtn.gameObject.SetActive(btnActive);
            contentPane.orTx.enabled = btnActive;
            contentPane.otherBtn.gameObject.SetActive(!btnActive);

            if (!btnActive) contentPane.otherBtnTx.text = LanguageManager.inst.GetValueByKey(_data.config.lock_des);
        }

    }


    protected override void onShown()
    {
        base.onShown();

    }

    protected override void onHide()
    {
        contentPane.gemConfirmObj.SetActiveFalse();
        _callback?.Invoke();
        _callback = null;
    }

    void recruitWorker(EItemType costType)
    {
        if (costType == EItemType.Gold)
        {
            if (UserDataProxy.inst.playerData.level < _data.config.level)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                return;
            }

            if (UserDataProxy.inst.playerData.gold < _data.config.cost_money)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                return;
            }
        }
        else if (costType == EItemType.Gem)
        {
            if (UserDataProxy.inst.playerData.gem < _data.config.cost_diamond)
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, _data.config.cost_diamond - UserDataProxy.inst.playerData.gem);
                contentPane.gemConfirmObj.SetActiveFalse();
                return;
            }
        }

        EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.REQUEST_Worker_Recruit, _data.id, costType);
    }

    void turnPage(bool isLeft)
    {
        SetData(RoleDataProxy.inst.GetNearWorkerData(_data, isLeft), true, _callback);
    }

    void onOtherBtnClick()
    {

        switch ((kRoleWorkerGetType)_data.config.get_type)
        {
            case kRoleWorkerGetType.turntable: //转盘
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(100).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(100).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_SHOWVIEW);
                break;

            case kRoleWorkerGetType.guide://引导
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey(_data.config.lock_des), Color.white);
                break;

            case kRoleWorkerGetType.buy://雇佣
                break;

            case kRoleWorkerGetType.sevenDay://七日
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(135).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(135).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
                EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.SHOWUI_SEVENDAYUI);
                break;

            case kRoleWorkerGetType.buildingLink: //建筑
                break;

            case kRoleWorkerGetType.giftLink: //礼包

                HotfixBridge.inst.TriggerLuaEvent("CSCallLua_ShowGiftDeatilUI", _data.config.sale_id);

                break;

            default:
                break;
        }

    }

}
