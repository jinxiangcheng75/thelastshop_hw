using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceLabBuildingUnLockTip : InputEventListener
{

    private ScienceLabBuildingUnLockHud hud;

    private void Awake()
    {
        EventController.inst.AddListener(GameEventType.CityBuildingEvent.HUD_SCIENCEBUILDINGREFRESH, updateHudInfo);
    }

    public override bool TOnClick(Vector3 mousepos)
    {
        if (hud != null && hud.gameObject.activeSelf)
        {
            EventController.inst.TriggerEvent<int>(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, 2300);
        }
        return true;
    }

    void Start()
    {
        updateHudInfo();
    }

    void updateHudInfo()
    {
        if (null == this) return;

        if (hud == null)
            hud = FGUI.inst.CreateBuildingUnLockHUD(this.transform);
        if (hud != null)
        {
            hud.FindUnLockBuilding();
        }
    }


    private void OnDestroy()
    {
        EventController.inst.RemoveListener(GameEventType.CityBuildingEvent.HUD_SCIENCEBUILDINGREFRESH, updateHudInfo);
    }


}
