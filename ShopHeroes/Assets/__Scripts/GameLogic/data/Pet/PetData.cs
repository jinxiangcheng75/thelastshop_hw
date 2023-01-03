using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetData
{
    public int petUid;
    public OnePetInfo petInfo;
    public PetConfig petCfg;

    public int petNextFeedTime 
    {
        get { return cooltime - (int)GameTimer.inst.serverNow; }
    }
    private int cooltime;
    private int timeId;

    public PetData(OnePetInfo petInfo)
    {
        SetData(petInfo);
    }

    public void SetData(OnePetInfo petInfo)
    {
        petUid = petInfo.petUid;
        this.petInfo = petInfo;
        petCfg = PetConfigManager.inst.GetConfig(petInfo.petId);
        cooltime = petInfo.petNextFeedTime + (int)GameTimer.inst.serverNow;
        setTimer();
    }

    void clearTimer()
    {
        if (timeId != 0)
        {
            GameTimer.inst.RemoveTimer(timeId);
            timeId = 0;
        }
    }

    void setTimer()
    {
        clearTimer();

        if (petNextFeedTime > 0) //说明喂食在CD
        {
            timeId = GameTimer.inst.AddTimer(petNextFeedTime, 1, timerMethod, GameTimerType.byServerTime);
        }
    }

    void timerMethod() 
    {
        if (petNextFeedTime <= 0)
        {
            clearTimer();
            petInfo.petNextFeedTime = 1;
            EventController.inst.TriggerEvent(GameEventType.PetCompEvent.REQUEST_PET_UPDATEINFO, petUid);
        }
        else
        {
            timeId = GameTimer.inst.AddTimer(petNextFeedTime, 1, timerMethod, GameTimerType.byServerTime);
        }
    }

}
