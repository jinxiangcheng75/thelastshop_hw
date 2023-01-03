using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatRoleSystem : BaseSystem
{
    //private CreatRoleView _creatRoleView;

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_CREATROLEPANEL, ShowCreatRolePanelView);
        EventController.inst.AddListener(GameEventType.HIDEUI_CREATROLEPANEL, HideCreatRolePanelView);

        NetworkEvent.SetCallback(MsgType.Response_User_Create_Cmd,
        (successResp) =>
        {
            var data = (Response_User_Create)successResp;
            if (data.errorCode == (int)EErrorCode.EEC_Success)
            {
                //登录使用初始key,防止重新登录时key不对
                FileUtils.OverrideEncByDkk();
                //获取玩家数
                NetworkEvent.SendRequest(new NetworkRequestWrapper()
                {
                    req = new Request_User_Login()
                    {
                        account = AccountDataProxy.inst.account,
                        s1 = PlatformManager.inst.s1,
                        s2 = PlatformManager.inst.s2,
                        s3 = PlatformManager.inst.s3,
                        s4 = PlatformManager.inst.s4,
                    }
                });
            }
        },
        (failedResp) =>
        {
            //创建角色失败
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("登录失败!"));
        });
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_CREATROLEPANEL, ShowCreatRolePanelView);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_CREATROLEPANEL, HideCreatRolePanelView);
    }

    private void ShowCreatRolePanelView()
    {
        GUIManager.OpenView<CreatRoleView>();
    }

    private void HideCreatRolePanelView()
    {
        GUIManager.HideView<CreatRoleView>();
    }
}
