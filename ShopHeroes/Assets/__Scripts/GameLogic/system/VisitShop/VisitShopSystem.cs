using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VisitShopSystem : BaseSystem
{
    //玩家信息界面
    PlayerInfoPanelView playerInfoPanelView;
    IndoorScene indoorScene;
    protected override void AddListeners()
    {
        EventController.inst.AddListener<string>(GameEventType.VisitShopEvent.VISIT_ENTER_SHOP, EnterShop);
    }
    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<string>(GameEventType.VisitShopEvent.VISIT_ENTER_SHOP, EnterShop);
    }

    //进入 玩家店铺 
    private void EnterShop(string PlayerUId)
    {
        if (FGUI.inst != null)
        {
            FGUI.inst.showGlobalMask(2);
        }
        RequestUserData(PlayerUId);
    }

    //获取用户数据
    public void RequestUserData(string userid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Detail()
            {
                userId = userid
            }
        });
    }
    //用户数据获取回调
    void OnRequestUserDataEnd()
    {
        //进入拜访状态
    }
}
