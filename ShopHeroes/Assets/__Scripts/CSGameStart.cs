using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[DisallowMultipleComponent]
public class CSGameStart : MonoBehaviour
{

    public string gateServerHost = "http://192.168.1.218:12223/";//"http://game.shop.clashofpuzzle.com";//"http://192.168.1.241:2223/" //"http://shop-hero.poptiger.cn:2223"
    public string gameServerUrl = "http://192.168.1.218:12223/";
    public string AssetHost = "";
    [Header("协议加密开关")]
    public bool mProtocolEncryption = true;
    [HideInInspector]
    public bool onApplicationFocus { get; private set; }

    public static CSGameStart Inst { get; private set; }
    void Awake()
    {
        if (Inst != null)
        {
            Debug.LogError("GameStart.Awake called twice !!!");
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        AddressableConfig.addressableRuntimePath = AssetHost;
        Inst = this;
        onApplicationFocus = true;
        EventController.inst.AddListener(GameEventType.GAME_RESTART, PlayRestart);
    }

    void Start()
    {
        GameSettingManager.appVersion = Application.version;
        GameSettingManager.ProtocolEncryption = mProtocolEncryption;
        CsvCfgCatalogMgr.inst.init();
        StartCoroutine(loadUrlUpdateCfg());
        //initManagers();
    }
    IEnumerator loadUrlUpdateCfg()
    {
        string filefullpath = ResPathUtility.getpersistentDataPath(false) + "cfgs/CsvCatalog.txt";
        LocalCsvCatalog localCsvCatalog = null;
        if (File.Exists(filefullpath) != false)
        {
            SaveManager.Load<LocalCsvCatalog>(filefullpath, ref localCsvCatalog);
        }
        else
        {
            filefullpath = ResPathUtility.getstreamingAssetsPath(true) + "/cfgs/CsvCatalog.txt";
            UnityWebRequest request = UnityWebRequest.Get(filefullpath);
            yield return request.SendWebRequest();
            if (!request.isHttpError && !request.isNetworkError)
            {
                Stream s = new MemoryStream(request.downloadHandler.data);
                BinaryFormatter bf = new BinaryFormatter();
                object o = bf.Deserialize(s);
                s.Close();
                localCsvCatalog = (LocalCsvCatalog)o;
            }
        }

        string fileFullName = "";
        if (localCsvCatalog.csvFileList.ContainsKey("url_update_android"))
        {
            fileFullName = localCsvCatalog.csvFileList["url_update_android"].fileFullName;
        }

       // AddressablesManager.inst.CheckUpdate();// 提前判断是否更新
        string configpath = ResPathUtility.getpersistentDataPath(false) + "cfgs/" + fileFullName + ".csv";

        if (File.Exists(configpath) != false)
        {
            StreamReader sr = new StreamReader(configpath);
            string csv = sr.ReadToEnd();
            sr.Close();
            PlayerPrefs.SetString("url_update_android", csv);
            UrlUpdateConfigManager.inst.PreloadCsvConfig(csv);
            GameTimer.inst.AddTimer(0.5f, 1, InitSDK);
        }
        else
        {
            configpath = ResPathUtility.getstreamingAssetsPath(true) + "/cfgs/" + fileFullName + ".csv";
            UnityWebRequest request = UnityWebRequest.Get(configpath);
            yield return request.SendWebRequest();
            if (!request.isHttpError && !request.isNetworkError)
            {
                UrlUpdateConfigManager.inst.PreloadCsvConfig(request.downloadHandler.text);
                PlayerPrefs.SetString("url_update_android", request.downloadHandler.text);
            }
            GameTimer.inst.AddTimer(0.5f, 1, InitSDK);
        }
    }
    public void InitSDK()
    {
      //  FGUI.inst.SetLoginBGVisible(true);
        //检查更新 显示闪屏视频
        if (FGUI.inst.videoMusic != null)
            FGUI.inst.videoMusic.Play();
        NetworkConfig.SetHost(gateServerHost + "/");
        //初始化sdk
        PlatformManager.inst.InitPlatformSDK();
    }
    public void setGameServerUrl(string serverhost, string server)
    {
        gateServerHost = serverhost;
        gameServerUrl = server;
        NetworkConfig.SetHost(gateServerHost + "/");
    }
    public void StartGame()
    {
        //初始化网络
        ManagerBinder.inst.mNetworkMgr = new NetworkManager();
        ManagerBinder.inst.Init(this);

        LanguageConfigManager.inst.initstartcsvfile();
        //
        FGUI.inst.showLoading();

        FGUI.inst.updateProgressText(LanguageManager.inst.GetValueByKey("连接服务器..."));//(LanguageManager.inst.curType == LanguageType.TRADITIONAL_CHINESE ? "連接伺服器..." : "连接服务器...");

        Helper.AddNetworkRespListener(MsgType.Response_Gate_Cmd, GetNoticeData);
        //资源更新的版本
        if (PlayerPrefs.HasKey("updateResV"))
        {
            updateResV = PlayerPrefs.GetString("updateResV");
        }
        //获取本地资源版本
        if (PlayerPrefs.HasKey("resVersion"))
        {
            resVersion = PlayerPrefs.GetString("resVersion");
            GameTimer.inst.StartCoroutine(startLinkGateServer());
        }
        else
        {
            var fileFullPath = ResPathUtility.getstreamingAssetsPath(true) + "/ResVersion.txt";
            StartCoroutine(ReadTextFile(fileFullPath, (text) =>
            {
                resVersion = text;
                //LoginGate();
                GameTimer.inst.StartCoroutine(startLinkGateServer());
            }));
        }
    }

    //登陆网关获取游戏服务器地址，上传版本号
    //资源版本
    string resVersion = "";
    string updateResV = "";
    bool isGateLink = false;
    IEnumerator startLinkGateServer()
    {
        yield return null;
        while (!isGateLink)
        {
            LoginGate();
            yield return new WaitForSeconds(3);
        }

    }
    public void LoginGate()
    {
        GameSettingManager.resVersion = resVersion;
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Gate()
            {
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
                osType = (int)EOsType.Ios,
#elif !UNITY_EDITOR && UNITY_ANDROID
                osType = (int)EOsType.Android,
#endif

                appVer = GameSettingManager.appVersion,
                resVer = resVersion,
                gameUrl = gameServerUrl
            }
        });
    }

    void GetNoticeData(HttpMsgRspdBase msg)
    {
        if (isGateLink) return;
        Response_Gate data = (Response_Gate)msg;
        
        //生成并记录约定的enc key
        FileUtils.SetDkkIndex(data.lei);

        GameTimer.inst.setServerTime(data.serverTime);
        gameServerUrl = data.gameUrl;
        AssetHost = data.resUrl;
        isGateLink = true;
        if (data.noticeList.Count > 0)
        {
            //var noticeData = data.noticeList.Find(t => t.lang == (int)LanguageManager.inst.curType);
            AccountDataProxy.inst.mNotice = data.noticeList;
        }

        string vs = resVersion.Replace('.', '0');
        int localVer = int.Parse(vs);
        vs = data.resVer.Replace('.', '0');
        int remotever = int.Parse(vs);

        if (AssetHost.Length > 0)
        {
            if (localVer > remotever)
            {
                AssetHost = AssetHost.Replace(data.resVer, resVersion);
            }
            AddressableConfig.addressableRuntimePath = AssetHost;
        }

        GameSettingManager.HandleEventState = data.userBehaviorReportFlag;
        //AddressableConfig.addressableRuntimePath = "http://192.168.1.200/mu/shopheroes/shoptest";
        SetGameServerUrl(gameServerUrl + "/");

        AudioManager.inst.PlayMusic("login_bgm");
        string localappv = GameSettingManager.appVersion.Replace('.', '0');
        string remoteappv = data.appVer.Replace('.', '0');
        if (data.appVer != GameSettingManager.appVersion && int.Parse(remoteappv) > int.Parse(localappv))
        {
            //版本不对提示下载新的客户端版本
            FGUI.inst.msgtipUI.gameObject.SetActive(true);
            FGUI.inst.msgtipUI.setShowInfo(0);
            return;
        }

        if (AssetHost.Length > 0 && (data.resVer != resVersion || PlayerPrefs.HasKey("needUpdateAssets") || string.IsNullOrEmpty(updateResV) || updateResV != GameSettingManager.appVersion))
        {
            if (!string.IsNullOrEmpty(AddressableConfig.addressableRuntimePath))
            {
                //进入更新
                ManagerBinder.inst.mGameState = kGameState.Update;
                AddressablesManager.inst.startUpdate((hasupdate, hasDownload) =>
                {
                    Logger.log("更新：更新结束");
                    PlatformManager.inst.GameHandleEventLog("Update_ResEnd", "");
                    if (hasupdate)
                    {
                        GameSettingManager.resVersion = data.resVer;
                        PlayerPrefs.SetString("resVersion", data.resVer);
                    }
                    PlayerPrefs.SetString("updateResV", GameSettingManager.appVersion);
                    CsvCfgCatalogMgr.inst.InitCsvCatalog(startCheckGameConfigCallBack);
                    
                });
                return;
            }
            else
            {
                Debug.LogError("资源更新地址为空!!!!");
            }
        }

        //
        // SetGameServerUrl(gameServerUrl + "/");
        FGUI.inst.SetLoginBGVisible(true);
        PlayerPrefs.SetString("resVersion", data.resVer);
        CsvCfgCatalogMgr.inst.InitCsvCatalog(startCheckGameConfigCallBack);
    }

    void SetGameServerUrl(string url)
    {
        NetworkConfig.SetHost(url);
        var requestFactory = ManagerBinder.inst.mNetworkMgr.mRequestHandlerFactory;
        HttpRequestHandler requestHandler = (HttpRequestHandler)requestFactory.getHandler(kRequestHandlerType.Http);
        requestHandler.setHost(url);
    }
    void EnterVersionManager()
    {
        Logger.log("更新：更新配置结束");
        CsvCfgCatalogMgr.inst.ReloadCSVFile(() =>
        {
            Logger.log("loadLocalCatalog: 12");

            VersionManager.inst.CheckUpdate(this, onUpdateComplete);
        });
    }

    private void startCheckGameConfigCallBack(bool isSucced)
    {
        if (isSucced)
        {
            GameTimer.inst.AddTimer(0.5f, 1, () =>
            {
                Logger.log("loadLocalCatalog: 11");
                FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("进入壁垒..."), 0f, 0f);
                EnterVersionManager();
            });
        }
        else
        {
            Debug.LogError("热更新配置文件失败!! 3秒钟后重试");
            //失败  3秒后重新更新
            GameTimer.inst.AddTimer(3, 1, () => CsvCfgCatalogMgr.inst.InitCsvCatalog(startCheckGameConfigCallBack));
        }
    }


    void onUpdateComplete()
    {
        Logger.log("loadLocalCatalog: 14");
        //start lua env
        Logger.log("start lua env");

        //XLuaManager.inst.mUniLuaEnv.DoString("util =  require('xlua/util')");
        XLuaManager.inst.DoString("require \"GameStart\"");
        HotfixBridge.inst._init();
    }


    IEnumerator ReadTextFile(string path, System.Action<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (!request.isHttpError && !request.isNetworkError)
        {
            string data = request.downloadHandler.text;
            callback?.Invoke(data);
        }
        else
        {
            callback?.Invoke("");
        }
    }

    bool _hasReCheckUpdate;
    Action _hasUpdateCallback;
    Action _notHasUpdateCallback;
    //重新检查更新 有更新更新完毕退出
    public void ReCheckUpdate(Action hasUpdateCallback, Action notHasUpdateCallback)
    {
        FGUI.inst.showLoading();

        ManagerBinder.inst.mGameState = kGameState.Update;
        AddressablesManager.inst.startUpdate((hasupdate, hasDownload) =>
        {
            Logger.log("reLogin 更新：更新结束");
            _hasReCheckUpdate = hasupdate && hasDownload;
            _hasUpdateCallback = hasUpdateCallback;
            _notHasUpdateCallback = notHasUpdateCallback;
            CsvCfgCatalogMgr.inst.InitCsvCatalog(reCheckConfigsUpdateComplete);
        });
        return;
    }

    void reCheckConfigsUpdateComplete(bool isSucced)
    {
        if (isSucced)
        {
            GameTimer.inst.AddTimer(0.5f, 1, () =>
            {
                if (_hasReCheckUpdate)
                {
                    _hasUpdateCallback?.Invoke();
                }
                else
                {
                    _notHasUpdateCallback?.Invoke();
                }

                _hasReCheckUpdate = false;
                _hasUpdateCallback = null;
                _notHasUpdateCallback = null;
            });
        }
        else
        {
            Debug.LogError("热更新配置文件失败!! 3秒钟后重试");
            //失败  3秒后重新更新
            GameTimer.inst.AddTimer(3, 1, () => CsvCfgCatalogMgr.inst.InitCsvCatalog(reCheckConfigsUpdateComplete));
        }
    }

    void OnDestroy()
    {

        NetworkEvent.clear();
        NetworkManager.inst.clear();
    }

    private void OnApplicationFocus(bool focus)
    {
        onApplicationFocus = focus;
        if (focus)
        {
            Debug.Log("进入前台");
            AudioManager.inst.PausedAll(false);
            PlatformManager.inst.RemoveAppIconBadge();
        }
        else
        {
            Debug.Log("进入后台");
            AudioManager.inst.PausedAll(true);
        }
    }

    //返回游戏登陆游戏
    public void PlayRestart()
    {
        Time.timeScale = 1;
        if (HotfixBridge.inst != null && ManagerBinder.inst != null)
        {
            if ((int)ManagerBinder.inst.mGameState > (int)kGameState.CreatRole)  //进入游戏之后才可以有改操作操作
            {
                HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Login, true));
            }
        }

        ////
        ////清理游戏循环
        //GameTimer.inst.clearAll();
        ////清理游戏内事件
        //EventController.inst.Cleanup();
        ////清理网络事件
        //NetworkEvent.clear();
        //NetworkManager.inst.PauseKeepAlive(true);
        ////
        //ManagerBinder.inst.clear();

        ////清理GUI管理器
        //GUIManager.ClearAll();

        ////删除GUI隐藏的对象
        //foreach (Transform ctf in FGUI.inst.uiHideRootTF)
        //{
        //    GameObject.DestroyImmediate(ctf.gameObject);
        //}

        //VersionManager.inst.Clear();

        //HotfixBridge.inst.Release();

        //// Debug.LogError(@"解决方案
        //// 释放这些delegate即可，所谓释放，在C#中，就是没有引用：
        //// 你是在C#通过LuaTable.Get获取并保存到对象成员，赋值该成员为null；
        //// 你是在lua那把lua函数注册到一些事件事件回调，反注册这些回调；
        //// 如果你是通过xlua.hotfix(class, method, func)注入到C#，则通过xlua.hotfix(class,                                     
        //// method, nil)删除；
        //// 要注意以上操作在Dispose之前完成。
        //// ");
        //// GameTimer.inst.AddTimer(1, 1, () =>
        //// {
        ////     XLuaManager.inst.mUniLuaEnv.DoString("util.print_func_ref_by_csharp()");
        //// });

        ////返回登陆场景
        //ManagerBinder.inst.mSceneMgr.ToStartScene();
        //ManagerBinder.inst.isReStart = true;
        //GameTimer.inst.AddTimer(0.5f, 1, () =>
        //{
        //    //最后释放lua环境
        //    XLuaManager.inst.Dispose();

        //    XLuaManager.inst.InitLuaEnv();

        //    //  GameObject.DestroyImmediate(FGUI.inst.gameObject);
        //});
        ////重新开始游戏(jiancha)
        //GameSettingManager.appVersion = Application.version;
        //FGUI.inst.SetLoginBGVisible(true);
        //GameTimer.inst.AddTimer(1, 1, StartGame);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, "您的账号已经在用户中心退出，请重新进入并登陆游戏！", () =>
            {
                PlayRestart();
            });
        }
    }
#endif


}