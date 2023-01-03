using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildingLockDetailUI : ViewBase<CityBuildingLockDetailUIComp>
{

    public override string viewID => ViewPrefabName.CityBuildingLockDetailUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

    }

    public void SetData(CityBuildingData data)
    {
        if (data == null)
        {
            hide();
            Logger.error("没有该建筑数据");
            return;
        }


        contentPane.buidlingNameTx.text = LanguageManager.inst.GetValueByKey(data.config.name);
        contentPane.buidlingIcon.SetSpriteURL(data.config.big_icon);
        contentPane.buidlingDexTx.text = LanguageManager.inst.GetValueByKey(data.config.introduction_dec);

        contentPane.jumpBtn.onClick.RemoveAllListeners();

        if (data.config.unlock_type == (int)kCityBuildingUnlockType.BuildingLv)
        {
            var linkBuildingCfg = BuildingConfigManager.inst.GetConfig(data.config.unlock_id);

            contentPane.jumpDexTx.text = LanguageManager.inst.GetValueByKey("需要{0}等级达到{1}级", LanguageManager.inst.GetValueByKey(linkBuildingCfg.name), data.config.unlock_val.ToString());
            contentPane.jumpBtn.onClick.AddListener(() => clickLinkCityBuilding(data.config.unlock_id));
        }
        else if (data.config.unlock_type == (int)kCityBuildingUnlockType.ShopLv)
        {
            contentPane.jumpDexTx.text = LanguageManager.inst.GetValueByKey("店主等级达到{0}级", data.config.unlock_val.ToString());
            contentPane.jumpBtn.onClick.AddListener(gotoShop);
        }
        else if (data.config.unlock_type == (int)kCityBuildingUnlockType.NeedOneWorker)
        {
            var linkWorkerCfg = WorkerConfigManager.inst.GetConfig(data.config.unlock_id);

            contentPane.jumpDexTx.text = LanguageManager.inst.GetValueByKey("需要{0}等级达到{1}级", LanguageManager.inst.GetValueByKey(linkWorkerCfg.name), data.config.unlock_val.ToString());
            contentPane.jumpBtn.onClick.AddListener(() => clickLinkWorker(data.config.unlock_id));
        }

    }

    void clickLinkCityBuilding(int buildingId)
    {
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, buildingId);
    }

    void clickLinkWorker(int workerId)
    {

        var workerData = RoleDataProxy.inst.GetWorker(workerId);

        if (workerData == null) return;

        if (workerData.state == EWorkerState.Unlock)
        {
            EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.SHOWUI_WORKERINFOUI, workerData);
        }
        else if (workerData.state == EWorkerState.CanUnlock)
        {
            EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, workerData.id, false, null);
        }
        else if (workerData.state == EWorkerState.Locked)
        {

        }

    }

    void gotoShop()
    {
        hide();
        HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Shop, true));
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {

    }
}
