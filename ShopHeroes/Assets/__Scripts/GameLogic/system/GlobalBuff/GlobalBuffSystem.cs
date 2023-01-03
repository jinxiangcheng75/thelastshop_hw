using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//全局buff逻辑处理
public class GlobalBuffSystem : BaseSystem
{

    MenuUIView _mainUI;
    GlobalBuffDetailUI _globalBuffDetailUI;

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.GlobalBuffEvent.GLOBALBUFF_REFRESHUI_BUFFITEM, refreshMainUIBuffItems);
        EventController.inst.AddListener<GlobalBuffType>(GameEventType.GlobalBuffEvent.GLOBALBUFF_SHOWUI_DETAIL, showGlobalBuffDetail);
        EventController.inst.AddListener<int>(GameEventType.GlobalBuffEvent.GLOBALBUFF_DELBUFFITEM, delOneBuff);
        EventController.inst.AddListener<int>(GameEventType.GlobalBuffEvent.REQEST_GLOBALBUFF_UPDATE, reqest_globalBuffUpdate);
        //EventController.inst.AddListener(GameEventType.GlobalBuffEvent.REQEST_GLOBALBUFF_REFRESH, requst_globalBuffRefresh);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.GlobalBuffEvent.GLOBALBUFF_REFRESHUI_BUFFITEM, refreshMainUIBuffItems);
        EventController.inst.RemoveListener<GlobalBuffType>(GameEventType.GlobalBuffEvent.GLOBALBUFF_SHOWUI_DETAIL, showGlobalBuffDetail);
        EventController.inst.RemoveListener<int>(GameEventType.GlobalBuffEvent.GLOBALBUFF_DELBUFFITEM, delOneBuff);
        EventController.inst.RemoveListener<int>(GameEventType.GlobalBuffEvent.REQEST_GLOBALBUFF_UPDATE, reqest_globalBuffUpdate);
        //EventController.inst.RemoveListener(GameEventType.GlobalBuffEvent.REQEST_GLOBALBUFF_REFRESH, requst_globalBuffRefresh);
    }

    //int requestTimingTimer;
    //float timingTime;
    //float request_timingTime = 30f; //30秒定期请求buff数据

    protected override void OnInit()
    {
        //timingTime = 0;
        //requestTimingTimer = GameTimer.inst.AddTimer(1, timingRequest);
    }


    //void timingRequest() //定时请求
    //{
    //    timingTime += 1;
    //    if (timingTime >= request_timingTime)
    //    {
    //        timingTime = 0;
    //        requst_globalBuffRefresh();
    //    }
    //}


    void refreshMainUIBuffItems()
    {
        var mainUI = GUIManager.GetWindow<MenuUIView>();

        if (mainUI != null && mainUI.isShowing)
        {
            mainUI.RefreshGlobalBuff();
        }

    }

    void showGlobalBuffDetail(GlobalBuffType type)
    {
        GUIManager.OpenView<GlobalBuffDetailUI>(view =>
        {
            view.SetData(type);
        });
    }

    void delOneBuff(int buffUId)
    {
        var buffDetailUI = GUIManager.GetWindow<GlobalBuffDetailUI>();
        if (buffDetailUI != null && buffDetailUI.isShowing)
        {
            buffDetailUI.DelOneBuff(buffUId);
        }
    }

    void reqest_globalBuffUpdate(int buffId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Activity_Buff_Update()
            {
                buffId = buffId,
            }
        });
    }

    //void requst_globalBuffRefresh()
    //{
    //    if (AccountDataProxy.inst.isLogined)
    //    {

    //        if (UserDataProxy.inst.playerData.level < 5) return; //5级前不刷新

    //        NetworkEvent.SendRequest(new NetworkRequestWrapper()
    //        {
    //            req = new Request_Activity_Buff_Refresh()
    //            {
    //                uid = GlobalBuffDataProxy.inst.buffUid,
    //            }
    //        });
    //    }

    //}

    public override void OnExit()
    {
        base.OnExit();
        //GameTimer.inst.RemoveTimer(requestTimingTimer);
    }

}
