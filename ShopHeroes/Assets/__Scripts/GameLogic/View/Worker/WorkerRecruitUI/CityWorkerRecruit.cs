using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityWorkerRecruit : InputEventListener
{
    private WorkerRecruitHud hud;

    public override bool TOnClick(Vector3 mousepos)
    {
        if (hud != null && hud.gameObject.activeSelf)
        {
            EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, hud.Data.id, false, null);
        }
        return true;
    }

    void Awake()
    {
        LanguageManager.inst.ChangeLangeuageEvent += updateHudInfo;
        EventController.inst.AddListener(GameEventType.WorkerCompEvent.Worker_DataChg, updateHudInfo);
    }

    void OnDestroy()
    {
        LanguageManager.inst.ChangeLangeuageEvent -= updateHudInfo;
        EventController.inst.RemoveListener(GameEventType.WorkerCompEvent.Worker_DataChg, updateHudInfo);
    }
    void Start()
    {
        updateHudInfo();
    }

    void updateHudInfo()
    {
        if (null == this) return;

        if (hud == null)
            hud = FGUI.inst.CreateWorkerRecruitHUD(this.transform);
        if (hud != null)
        {
            hud.FindCanLockWorker();
            hud.SetFont();
        }
    }

}
