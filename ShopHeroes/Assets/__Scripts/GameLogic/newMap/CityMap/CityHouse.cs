using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHouse : InputEventListener
{
    public int houseID;
    private HouseComp hud;
    public GameObject lockObj;
    public GameObject unLockObj;
    public List<GameObject> cfxs;
    public override bool TOnClick(Vector3 mousepos)
    {
        base.TOnClick(mousepos);
        AudioManager.inst.PlaySound(143);
        if (GuideDataProxy.inst == null || GuideDataProxy.inst.CurInfo == null || !GuideDataProxy.inst.CurInfo.isAllOver) return false;
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, houseID);
        return true;
    }

    void Awake()
    {
        LanguageManager.inst.ChangeLangeuageEvent += updatehudinfo;
    }

    void OnDestroy()
    {
        LanguageManager.inst.ChangeLangeuageEvent -= updatehudinfo;
    }
    void Start()
    {
        updatehudinfo();
    }

    void updatehudinfo()
    {
        if (null == this) return;

        if (hud == null)
        {
            hud = FGUI.inst.CreaterHousePlane(this.transform);
            hud.unLockHandler = () => updateImg(false);
            hud.lockHandler = () => updateImg(true);
            hud.eventListener.OnClick = (v3) => TOnClick(v3);
        }
        if (hud != null)
        {
            hud.setFont();
            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_BUILDINGUPADD, houseID, hud);
            hud.setHouseInfo(houseID);
        }

        if (cfxs.Count > 0)
        {
            CityBuildingData buildingData = UserDataProxy.inst.GetBuildingData(houseID);
            bool showVfx = false;
            if (buildingData != null)
            {
                if (buildingData.state != 0)
                {
                    showVfx = true;
                }
            }

            showVFX(showVfx);
        }
    }

    private void showVFX(bool show)
    {
        foreach (var cfx in cfxs)
        {
            cfx.SetActive(show);
        }
    }

    void updateImg(bool isLock)
    {
        showVFX(!isLock);
        //if (lockObj != null)
        //{
        //    lockObj.SetActive(isLock);
        //}
        //if (unLockObj != null)
        //{
        //    unLockObj.SetActive(!isLock);
        //}
    }

}
