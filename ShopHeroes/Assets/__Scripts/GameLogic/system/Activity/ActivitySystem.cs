using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivitySystem : BaseSystem
{
    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener(GameEventType.ActivityEvent.REQUEST_DAILYGIFTLIST, requestDailyGiftList);
        EventController.inst.AddListener(GameEventType.ActivityEvent.REQUEST_DAILYGIFTREWARD, requestDailyGiftAward);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener(GameEventType.ActivityEvent.REQUEST_DAILYGIFTLIST, requestDailyGiftList);
        EventController.inst.RemoveListener(GameEventType.ActivityEvent.REQUEST_DAILYGIFTREWARD, requestDailyGiftAward);
    }

    void requestDailyGiftList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Activity_List()
        });
    }

    void requestDailyGiftAward()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Activity_DailyGiftReward()
        });
    }
}
