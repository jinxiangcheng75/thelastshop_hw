using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.poptiger.events;

[XLua.LuaCallCSharp]
public class NetworkManager
{
    public static NetworkManager inst;
    NetworkPipeline mNetworkPipeline;
    public IHandlerFactory<IPackageHandler, kRequestHandlerType> mRequestHandlerFactory;
    IHandlerFactory<IPackageHandler, kResponseHandlerType> mResponseHandlerFactory;
    INetworkErrorHandler mErrorHandler;
    IHandlerFactory<IStreamHandler, kStreamType> mStreamHandlerFactory;
    IEncryptHandler mEncryptHandler;
    public IMessageEncodeHandler mEncodeHandler;
    IHandlerFactory<IResponseDispatchHandler, kResponseDispatchType> mDispatchHandlerFactory;

    public Dictionary<int, bool> mHotfixResponseDict = new Dictionary<int, bool>();
    public NetworkManager()
    {
        inst = this;
        //NetworkConfig.Host = "https://town.kfc.poptiger.cn/town/api/";
        NetworkCommand.Init();

        Logger.log("Host : " + NetworkConfig.Host);

        initStreamHandlers();
        initRequestHandlers();
        initResponseHandlers();
        initErrorHandler();
        initEvent();

        mNetworkPipeline = new NetworkPipeline(new NetworkPipelineParams()
        {
            requestHandlerFactory = mRequestHandlerFactory,
            responseHandlerFactory = mResponseHandlerFactory,
            errorHandler = mErrorHandler,
            dispatchHandlerFactory = mDispatchHandlerFactory,
        });

        appendGameObject();
        //   test();
    }
    public void LuaCallTest()
    {
        Logger.log("LuaCallTest : " + NetworkConfig.Host);
    }
    void initStreamHandlers()
    {
        StreamHandlerFactory factory = new StreamHandlerFactory();
        factory.add(kStreamType.String, new StringStreamHandler());
        factory.add(kStreamType.Binary, new BinaryStreamHandler());
        mStreamHandlerFactory = factory;

        mEncryptHandler = new EncryptHandler();
        mEncodeHandler = new DefaultMessageEncodeHandler();

        var dispatchFactory = new ResponseDispatchHandlerFactory();
        dispatchFactory.add(kResponseDispatchType.Seperate, new SeperateResponseDispatchHandler());
        mDispatchHandlerFactory = dispatchFactory;
    }

    void initRequestHandlers()
    {
        RequestHandlerFactory factory = new RequestHandlerFactory();
        HttpRequestHandler httpHandler = new HttpRequestHandler(new RequestHandlerParams()
        {
            host = NetworkConfig.Host,
            useCookie = false,
            streamHandlerFactory = mStreamHandlerFactory,
            encryptHandler = mEncryptHandler,
            encodeHandler = mEncodeHandler,
            markHeaders = getHeaderList(),
            headerOverwriteCount = 1
        });
        factory.add(kRequestHandlerType.Http, httpHandler);
        mRequestHandlerFactory = factory;
        Logger.log("initRequestHandlers：");
    }

    List<string> getHeaderList()
    {
        return new List<string>() {
            "x-wh-token"
        };
    }

    void initResponseHandlers()
    {
        ResponseHandlerFactory factory = new ResponseHandlerFactory();
        factory.add(kResponseHandlerType.Json, new JsonListResponseHandler(mEncryptHandler, mEncodeHandler));
        mResponseHandlerFactory = factory;
    }

    void initErrorHandler()
    {
        mErrorHandler = new NetworkErrorHandler();
    }

    void initEvent()
    {
        NetworkEvent.inst.onSendRequest += NetworkEvent_onSendRequest;
        NetworkEvent.inst.onSendPackage += NetworkEvent_onSendPackage;
    }
    public void Send(int cmd, string jsonData)
    {
        var cPkg = new HotfixNetworkPackage();
        cPkg.init(cmd, new HotfixNetworkRequest(cmd, jsonData));
        mNetworkPipeline.handleSend(cPkg);
    }
    void appendGameObject()
    {
        GameObject go = new GameObject("NetworkMono");
        var mono = go.AddComponent<MonoUpdater>();
        mono.setUpdater(mNetworkPipeline);
    }

    void NetworkEvent_onSendRequest(NetworkRequestWrapper wrapper)
    {
        //send(wrapper);
        var pkg = NetworkPackage.POOL.Get();

        pkg.init(wrapper.req.GetCMD(), wrapper.req);
        pkg.setPipelineConfig(PackagePipelineConfig.GetPipeline(kPipelineType.Default));
        if (mHotfixResponseDict.TryGetValue(pkg.getCmd(), out bool isHotfix))
        {
            pkg.setHotfix(isHotfix);
        }
        //Logger.log("C# send hotfix:" + mHotfixResponseDict[pkg.getCmd()] + " is:" + pkg.isHotfix());
        //NetworkEvent.SetCallback(wrapper.responseCmd, wrapper.success, wrapper.failed);
        mNetworkPipeline.handleSend(pkg);
    }

    void NetworkEvent_onSendPackage(INetworkPackage pkg)
    {
        mNetworkPipeline.handleSend(pkg);
    }

    // public void send(NetworkRequestWrapper wrapper)
    // {
    //     //INetworkPackage pkg = NetworkPackage
    //     var pkg = NetworkPackage.POOL.Get();

    //     pkg.init(wrapper.req.GetCMD(), wrapper.req);
    //     pkg.setPipelineConfig(PackagePipelineConfig.GetPipeline(kPipelineType.Default));
    //     if (mHotfixResponseDict.TryGetValue(pkg.getCmd(), out bool isHotfix))
    //     {
    //         pkg.setHotfix(isHotfix);
    //     }
    //     //Logger.log("C# send hotfix:" + mHotfixResponseDict[pkg.getCmd()] + " is:" + pkg.isHotfix());
    //     //NetworkEvent.SetCallback(wrapper.responseCmd, wrapper.success, wrapper.failed);
    //     mNetworkPipeline.handleSend(pkg);
    // }

    public void sendLua(HotfixNetworkPackage pkg)
    {
        mNetworkPipeline.handleSend(pkg);
    }

    public void clear()
    {
        mNetworkPipeline.clear();
        NetworkEvent.inst.onSendRequest -= NetworkEvent_onSendRequest;
        NetworkEvent.inst.onSendPackage -= NetworkEvent_onSendPackage;
    }

    public void test()
    {

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Login()
            {
                // userId = "123",
            }
        });
        bool retry = true;
        NetworkEvent.SetCallback(MsgType.Response_User_Login_Cmd,
        (successResp) =>
        {
            var res = successResp as Response_User_Login;
            Logger.log("test login response success : " + res);
            if (retry)
            {
                retry = false;
                test();
            }
        },
        (failedResp) =>
        {
            //
        });
    }

    #region 网络
    //手机上网方式
    public static NetworkReachability GetNetWorkType()
    {
        return Application.internetReachability;
    }
    //当前手机是否连接wifi
    public bool isWifi
    {
        get { return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork; }
    }
    //是否有网络
    public bool isNotReachable
    {
        get { return Application.internetReachability == NetworkReachability.NotReachable; }
    }


    public bool isOnline = true; //是否在线
    private bool isAliveResponse = true;
    ///数据
    private float lastRspdTime = 0;
    private float keepAliveRuntimeTime = 0;

    private bool lastKeepAliveEnd = true;
    private int _heartbeat = 0;
    private int _lastDay = 0;
    private int _onlineTime = 0;
    private int _mailTime = 0;
    private int _chatIndex = 0;
    public int _raceLampIndex;
    private int _activityBuffVersion = 0;
    private int _ver1 = 0;
    private int _ver2 = 0;
    private int _ver3 = 0;
    private int _ver4 = 0;
    private int _ver5 = 0;
    private int _ver6 = 0;
    private int _ver7 = 0;
    private int _ver8 = 0;
    private int _ver9 = 0;
    private int _ver10 = 0;
    private string _verStr = "";
    private int newSystemMsgEvent = 0;

    private int keepaliveErrorCount = 0;
    public bool KeepAlivePause = false;

    public void HeartbeatDataInit() 
    {
        _heartbeat = 0;
        _lastDay = 0;
        _onlineTime = 0;
        _mailTime = 0;
        _chatIndex = 0;
        _raceLampIndex = 0;
        _activityBuffVersion = 0;
        _ver1 = 0;
        _ver2 = 0;
        _ver3 = 0;
        _ver4 = 0;
        _ver5 = 0;
        _ver6 = 0;
        _ver7 = 0;
        _ver8 = 0;
        _ver9 = 0;
        _ver10 = 0;
        _verStr = "";
        newSystemMsgEvent = 0;

        isOnline = true;
        lastKeepAliveEnd = true;
        keepaliveErrorCount = 0;
        KeepAlivePause = false;
    }

    public void startKeepAlive()
    {
        keepAliveRuntimeTime = Time.time;
        lastRspdTime = Time.time;
        Helper.AddNetworkRespListener(MsgType.Response_Heartbeat_Cmd, OnResponseHeartbetSuccess);
        //开始
        PauseKeepAlive(false);
    }
    public void PauseKeepAlive(bool pause)
    {
        if (pause)
        {
            GameTimer.inst.StopCoroutine(UpdateKeepAlive());
        }
        else
        {
            if (!KeepAlivePause)
            {
                keepAliveRuntimeTime = Time.time;
                lastRspdTime = Time.time;
                keepaliveErrorCount = 0;

                GameTimer.inst.StartCoroutine(UpdateKeepAlive());
            }
        }
        KeepAlivePause = pause;
    }
    private void OnResponseHeartbetSuccess(HttpMsgRspdBase msg)
    {
        if (Time.time - lastRspdTime > 10)
        {
            //两次收到消息超过10秒则短线提示
            keepaliveErrorCount = 5;
            NetworkErrorPanel.inst.showState(1);
            isOnline = false;
            return;
        }

        lastKeepAliveEnd = true;
        NetworkErrorPanel.inst.showState(0);
        keepaliveErrorCount = 0;
        if (isOnline == false)
        {
            EventController.inst.TriggerEvent(GameEventType.NETWORK_RELINK);
        }
        isOnline = true;
        var data = (Response_Heartbeat)msg;
        if (GameTimer.inst != null)
        {
            Logger.log("服务器时间:" + TimeUtils.timeSpan2Str(msg.serverTime), "#ff00ff");
            GameTimer.inst.setServerTime(msg.serverTime);
        }
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //收到到心跳
            _heartbeat = data.heartbeat;
            _lastDay = data.lastDay;
            _onlineTime = data.onlineTime;
            _mailTime = data.mailTime;
            _chatIndex = data.chatIndex;
            _raceLampIndex = data.raceLampIndex;
            _activityBuffVersion = data.activityBuffVersion;
            _ver1 = data.ver1;
            _ver2 = data.ver2;
            _ver3 = data.ver3;
            _ver4 = data.ver4;
            _ver5 = data.ver5;
            _ver6 = data.ver6;
            _ver7 = data.ver7;
            _ver8 = data.ver8;
            _ver9 = data.ver9;
            _ver10 = data.ver10;
            _verStr = data.verStr;
            newSystemMsgEvent = data.newSystemMsgEvent;

            EventController.inst.globalEventHandler.GameSystemDataChange(data);
        }
        else
        {
            PauseKeepAlive(true);
            if (data.errorCode == (int)EErrorCode.EEC_AppVersionUpdated)
            {
                FGUI.inst.msgtipUI.gameObject.SetActive(true);
                FGUI.inst.msgtipUI.setShowInfo(0);
            }
            else if (data.errorCode == (int)EErrorCode.EEC_ResVersionUpdated)
            {
                FGUI.inst.msgtipUI.gameObject.SetActive(true);
                FGUI.inst.msgtipUI.setShowInfo(1);
            }
            else if (data.errorCode == (int)EErrorCode.EEC_DuplicateLogin)
            {
                FGUI.inst.msgtipUI.gameObject.SetActive(true);
                FGUI.inst.msgtipUI.setShowInfo(2);
            }
            else if (data.errorCode == 104)  //已经被封号
            {
                EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, LanguageManager.inst.GetValueByKey("您的账号因为异常，已被封禁，请联系官方运营团队！"), () =>
                {

                    PlatformManager.inst.ExitApp(); // 退出游戏
                });
            }
            else if(data.errorCode == (int)EErrorCode.EEC_ServerMaintain) //服务器维护
            {
                EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, LanguageManager.inst.GetValueByKey("服务器处于维护中，请重启游戏查看最新公告！"), () =>
                {
                    PlatformManager.inst.ExitApp(); // 退出游戏
                });
            }
            else
            {
                //異常
                FGUI.inst.msgtipUI.gameObject.SetActive(true);
                FGUI.inst.msgtipUI.setShowInfo(999);
            }
        }
    }


    // private float
    IEnumerator UpdateKeepAlive()
    {
        while (AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            keepAliveRuntimeTime = Time.time;
            if (isNotReachable)  //没有网络
            {
                isOnline = false;
                yield return new WaitForSeconds(0.2f);
                NetworkErrorPanel.inst.showState(1);
                break;
            }
            if (keepAliveRuntimeTime - lastRspdTime > 5)
            {
                //转菊花
                lastKeepAliveEnd = true;
                keepaliveErrorCount++;
                yield return new WaitForSeconds(0.2f);
                NetworkErrorPanel.inst.showState(2);
                isOnline = false;
            }
            if (keepaliveErrorCount > 3)
            {
                //掉线提示
                yield return new WaitForSeconds(0.2f);
                NetworkErrorPanel.inst.showState(1);
                isOnline = false;
                break;
            }

            if (lastKeepAliveEnd)
            {
                yield return new WaitForSeconds(3); //这三秒内切换账号 会无条件发一次心跳 从而导致问题
                if (AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
                {
                    lastRspdTime = Time.time;
                    keepAliveRuntimeTime = Time.time;
                    //发送下一个心跳
                    lastKeepAliveEnd = false;
                    NetworkEvent.SendRequest(new NetworkRequestWrapper()
                    {
                        req = new Request_Heartbeat()
                        {
                            heartbeat = _heartbeat,
                            unionId = UserDataProxy.inst.playerData.unionId,
                            lastDay = _lastDay,
                            onlineTime = _onlineTime,
                            mailTime = _mailTime,
                            chatIndex = _chatIndex,
                            raceLampIndex = _raceLampIndex,
                            activityBuffVersion = _activityBuffVersion,
                            ver1 = _ver1,
                            ver2 = _ver2,
                            ver3 = _ver3,
                            ver4 = _ver4,
                            ver5 = _ver5,
                            ver6 = _ver6,
                            ver7 = _ver7,
                            ver8 = _ver8,
                            ver9 = _ver9,
                            ver10 = _ver10,
                            verStr = _verStr,
                            appVer = GameSettingManager.appVersion,
                            resVer = GameSettingManager.resVersion,
                        }
                    });
                }
            }
            else
            {

            }
            //刷新
            yield return new WaitForSeconds(1);
        }
    }


    #endregion
}
