using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLockDrawingByWorkerUIView : ViewBase<UnLockDrawingByWorkerUIComp>
{
    public override string viewID => ViewPrefabName.UnLockDrawingByWorkerUI;

    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.workerInfoBtn.onClick.AddListener(() =>
        {
            hide();
            EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, workers[_curWorkerIndex].id, false, null);
        });

        contentPane.equipDesBtn.onClick.AddListener(() =>
        {
            //EquipQualityConfig cfg = EquipConfigManager.inst.GetEquipQualityConfig(_equipDrawingId, 1);
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, "", cfg.id, new List<EquipItem>());
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUIBYDRAWINGID, _equipDrawingId);
        });

        contentPane.leftBtn.onClick.AddListener(() => turnPage(-1));
        contentPane.rightBtn.onClick.AddListener(() => turnPage(1));
    }

    int _equipDrawingId;
    int _curWorkerIndex;
    List<WorkerConfig> workers = new List<WorkerConfig>();
    public void showInfo(EquipDrawingsConfig cfg)
    {
        _equipDrawingId = cfg.id;
        workers.Clear();

        contentPane.equipIcon.SetSprite(cfg.atlas, cfg.icon);

        for (int i = 0; i < cfg.artisan_id.Length; i++)
        {
            workers.Add(WorkerConfigManager.inst.GetConfig(cfg.artisan_id[i]));
        }

        contentPane.leftBtn.gameObject.SetActive(workers.Count > 1);
        contentPane.rightBtn.gameObject.SetActive(workers.Count > 1);

        _curWorkerIndex = 0;

        setWorkerInfo();
    }

    private void turnPage(int param)
    {
        _curWorkerIndex += param;

        if (_curWorkerIndex == 0) _curWorkerIndex = workers.Count - 1;
        if (_curWorkerIndex == workers.Count) _curWorkerIndex = 0;

        setWorkerInfo();
    }

    private void setWorkerInfo()
    {
        var workerCfg = workers[_curWorkerIndex];

        contentPane.workerInfoTip.text = LanguageManager.inst.GetValueByKey("你必须招募{0}，方可解锁此图纸", LanguageManager.inst.GetValueByKey(workerCfg.name));
        contentPane.workerIcon.SetSprite("portrait_atlas", workerCfg.icon);
    }


}
