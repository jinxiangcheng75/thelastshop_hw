using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//账号系统
public class AccountSystem : BaseSystem
{
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.HIDEUI_LOGIN, HideUI);
        EventController.inst.AddListener(GameEventType.SHOWUI_LOGIN, ShowUI);
        EventController.inst.AddListener<string, string>(GameEventType.AccountEvent.UI_LoginServer, LoginServer);
        // EventController.inst.AddListener<System.Object>(SDKEvent.SDKEvent_LoginEnd, onSdkLoginEnd);
        EventController.inst.AddListener<Notice>(GameEventType.NoticeEvent.SHOWUI_NOTICE, ShowNoticeUI);
        //  Helper.AddNetworkRespListener(MsgType.Response_Gate_Cmd, GetNoticeData);
        Helper.AddNetworkRespListener(MsgType.Response_User_BindingQuery_Cmd, onResponseBindingQuery);
        Helper.AddNetworkRespListener(MsgType.Response_User_Binding_Cmd, onResponseBinding);
        Helper.AddNetworkRespListener(MsgType.Response_User_BindingClaim_Cmd, onResponseBindingClaim);

        EventController.inst.AddListener(GameEventType.LOGIN_SDK, login_sdk);
        EventController.inst.AddListener(GameEventType.BindingEvent.BINDINGACCOUNT, RequestAccountBinding);
        EventController.inst.AddListener(GameEventType.BindingEvent.CHANGEACCOUNTRLOGIN, changeAccountToken);
        EventController.inst.AddListener(GameEventType.BindingEvent.GETBINDINGAWARD, RequestBindingAward);
        addLoginResponse();
    }
    void addLoginResponse()
    {
        //登陆返回消息
        NetworkEvent.SetCallback(MsgType.Response_User_Login_Cmd,
        (successResp) =>
        {
            Logger.log("C# 收到登陆消息");
            Logined((Response_User_Login)successResp);
        },
        (failedResp) =>
        {
            //登录失败
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("登录失败!"));
        });
    }
    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.HIDEUI_LOGIN, HideUI);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_LOGIN, ShowUI);
        EventController.inst.RemoveListener<string, string>(GameEventType.AccountEvent.UI_LoginServer, LoginServer);
        // EventController.inst.RemoveListener<System.Object>(SDKEvent.SDKEvent_LoginEnd, onSdkLoginEnd);
        EventController.inst.RemoveListener<Notice>(GameEventType.NoticeEvent.SHOWUI_NOTICE, ShowNoticeUI);
        EventController.inst.RemoveListener(GameEventType.LOGIN_SDK, login_sdk);
        EventController.inst.RemoveListener(GameEventType.BindingEvent.BINDINGACCOUNT, RequestAccountBinding);
        EventController.inst.RemoveListener(GameEventType.BindingEvent.CHANGEACCOUNTRLOGIN, changeAccountToken);
        EventController.inst.RemoveListener(GameEventType.BindingEvent.GETBINDINGAWARD, RequestBindingAward);
    }
    NoticeUIView noticeView;
    private void HideUI()
    {
        GUIManager.HideView<GameLoginView>();
    }
    private void ShowUI()
    {
        //调用sdk 登陆.
        PlatformManager.inst.LoginSdk();

        FGUI.inst.RefreshLoginBG();

#if UNITY_EDITOR
        FGUI.inst.sdkLoginBtn.gameObject.SetActive(false);
        PlatformManager.inst.GameHandleEventLog("Open", "");
        GUIManager.OpenView<GameLoginView>(view =>
        {
            view.setLastAccountInput(AccountDataProxy.inst.GetAccount());
        });

#endif

    }

    private void ShowNoticeUI(Notice notice)
    {
        GUIManager.OpenView<NoticeUIView>((view) =>
        {
            noticeView = view;
            noticeView.setNoticeData(notice);
        });
    }

    private void LoginServer(string account, string password)
    {
        requestLogin(account);
    }

    void requestLogin(string account)
    {
        Logger.log("c# requestLogin");
        //登录使用初始key,防止重新登录时key不对
        FileUtils.OverrideEncByDkk();
        //
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Login()
            {
#if UNITY_EDITOR //|| true  /////////////////////////测试打开
                account = account,
#else
                account = "",
#endif
                platform = (int)PlatformManager.platform,
                deviceId = AccountDataProxy.inst.deviceId,
                accountToken = AccountDataProxy.inst.GetToken(),
                platformUid = PlatformManager.inst.GetUserID(),
                platformToken = PlatformManager.inst.GetUserToken(),
                platformChannel = PlatformManager.inst.GetChannelType(),
                s1 = PlatformManager.inst.s1,
                s2 = PlatformManager.inst.s2,
                s3 = PlatformManager.inst.s3,
                s4 = PlatformManager.inst.s4,
#if UNITY_EDITOR
                osType = 0,
#elif UNITY_ANDROID
                osType = 1,
#elif UNITY_IOS
                osType = 2,
#endif
            }
        });
    }

    //登录成功
    private void Logined(Response_User_Login msg)
    {

        GameTimer.inst.setServerTime(msg.serverTime);

        if (msg.errorCode == 0)
        {
            //登录后重新标记enc key
            FileUtils.SetEnc(msg.k1);

            PlatformManager.inst.setRoleInfo(msg.userId);

            AccountDataProxy.inst.OnLogin(msg);

            //登陆uuid 设置打点
            //SDKManager.inst.onLoginSucceed(msg.account);
            PlatformManager.inst.GameHandleEventLog("GameLogin", msg.account);
            UserDataProxy.inst.playerData.userUid = msg.userId;

            //检查并加载一次视频广告
            //SDKManager.inst.loadVideoAvailable();

            AccountDataProxy.inst.currbindingType = (EBindingType)msg.bindingType;
            AccountDataProxy.inst.bindingClaimState = msg.bindingClaimState == 0;

            //获取玩家数据
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_User_Data()
            });

            if (NetworkManager.inst != null)
            {
                NetworkManager.inst.startKeepAlive();
            }
            if (GameSettingManager.HandleEventState == 1)
                ManagerBinder.inst.startSeneEvent(5);
            // }
            GamePay.inst.LoadOrderData();

        }
        else if (msg.errorCode == 104)  //已经被封号
        {
            EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, LanguageManager.inst.GetValueByKey("您的账号因为异常，已被封禁，请联系官方运营团队！"), () =>
            {
                //SDKManager.inst.ExitApp(); // 退出游戏
            });
        }
        else if (msg.errorCode == (int)EErrorCode.EEC_ServerMaintain)
        {
            if (AccountDataProxy.inst.mNotice != null)
            {
                var curLanguageData = AccountDataProxy.inst.mNotice.Find(t => t.lang == (int)LanguageManager.inst.curType);
                EventController.inst.TriggerEvent(GameEventType.NoticeEvent.SHOWUI_NOTICE, curLanguageData);
            }
            else
            {
                EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, LanguageManager.inst.GetValueByKey("服务器正在维护中，请耐心等待最新通知！"), null);
            }
        }
        else if (msg.errorCode == (int)EErrorCode.EEC_NeedReLogin)
        {
            //需要重新登陆
            PlatformManager.inst.ReLoginSdk();
        }
        else
        {
            Debug.LogError("登录失败" + msg.message);
        }
    }

    private void login_sdk()
    {
        //SDKManager.inst.LoginSDk((PlatformType)1);
    }

    //sdk 登陆返回
    private void onSdkLoginEnd(System.Object _Object)
    {
        if (_Object == null) return;
        Dictionary<string, System.Object> data = _Object as Dictionary<string, System.Object>;
        string token = data["Token"].ToString();
        string pfUid = data["UID"].ToString();
        string platformUseName = data["useName"].ToString();
        //获取到当前sdk token
        AccountDataProxy.inst.platformToken = token;
        AccountDataProxy.inst.platformUID = pfUid;
        AccountDataProxy.inst.platformUseName = platformUseName;
        //获取进度
        RequestBindingQuery(pfUid, token);
    }
    private void RequestBindingQuery(string sdkUid, string sdktoken)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_BindingQuery()
            {
                platform = (int)PlatformManager.platform,
                platformUid = sdkUid,
                platformToken = sdktoken
            }
        });
    }
    //账号绑定
    private void RequestAccountBinding()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Binding()
            {
                platform = (int)PlatformManager.platform,
                platformUid = AccountDataProxy.inst.platformUID,
                platformToken = AccountDataProxy.inst.platformToken,
                platformUserName = AccountDataProxy.inst.platformUseName
            }
        });
    }
    private void RequestBindingAward()
    {
        //请求绑定奖励
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_BindingClaim()
        });
    }

    private void changeAccountToken()
    {
        if (!string.IsNullOrEmpty(alreadyBindingAccountToken) && alreadyBindingAccountToken != AccountDataProxy.inst.GetToken())
        {
            AccountDataProxy.inst.setNewToken(alreadyBindingAccountToken);
            alreadyBindingAccountToken = "";
            //AccountDataProxy.inst.Clear();
            //重新登录游戏
            GameTimer.inst.AddTimer(0.5f, 1, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.GAME_RESTART);
            });
        }
    }

    #region Response
    //查询绑定状态
    string alreadyBindingAccountToken = "";
    private void onResponseBindingQuery(HttpMsgRspdBase msg)
    {
        var data = (Response_User_BindingQuery)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            if (data.isBinding == 1)
            {
                alreadyBindingAccountToken = data.alreadyBindingAccountToken;
                //已经绑定
                EventController.inst.TriggerEvent(GameEventType.BindingEvent.UPDATEPLATFORMQUERY, data.alreadyBindingUser);
            }
            else
            {
                //未绑定
                EventController.inst.TriggerEvent<UserData>(GameEventType.BindingEvent.UPDATEPLATFORMQUERY, null);
            }
        }
    }
    //账号绑定返回
    private void onResponseBinding(HttpMsgRspdBase msg)
    {
        var data = (Response_User_Binding)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            AccountDataProxy.inst.currbindingType = (EBindingType)data.bindingType;
            AccountDataProxy.inst.bindingClaimState = data.bindingClaimState == 0;

            //EventController.inst.TriggerEvent(GameEventType.BindingEvent.UPDATEBINGSTATE);
        }
        var settingview = GUIManager.GetWindow<SettingPanelView>();
        if (settingview != null && settingview.isShowing)
        {
            settingview.updateBindingState();
        }
    }
    //获取奖励返回
    private void onResponseBindingClaim(HttpMsgRspdBase msg)
    {
        var data = (Response_User_BindingClaim)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            AccountDataProxy.inst.currbindingType = (EBindingType)data.bindingType;
            AccountDataProxy.inst.bindingClaimState = data.bindingClaimState == 0;
            //   EventController.inst.TriggerEvent(GameEventType.BindingEvent.UPDATEBINGSTATE);
        }
        var settingview = GUIManager.GetWindow<SettingPanelView>();
        if (settingview != null && settingview.isShowing)
        {
            settingview.updateBindingState();
        }
    }
    #endregion
}
