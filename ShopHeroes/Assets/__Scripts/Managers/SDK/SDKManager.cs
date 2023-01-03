//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Runtime.InteropServices;
//using System;
//using System.Security.Cryptography;
//using System.Text;

//public class SDKAndroid
//{
//    public void Call(string function, params object[] args)
//    {
//        Logger.log("调用接口:" + function);
//#if !UNITY_EDITOR && UNITY_ANDROID
//                try
//                {
//                    using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
//                        using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity")) {
//                            jo.Call(function, args);
//                        }
//                    }
//                }
//                catch (System.Exception ex)
//                {
//                    Logger.log("安卓调用异常："+ex);
//                }
//                finally
//                {
//                }   
//#endif
//    }

//#if !UNITY_EDITOR && UNITY_ANDROID
//    public void Login(PlatformType platform)
//    {
//        switch (platform)
//        {
//            case PlatformType.googleplay:
//                {
//                    try
//                    {
//                        // //初始化 googleplaygames
//                        // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//                        //     //请求生成ID令牌， 将玩家表示为其他服务。
//                        //     .RequestIdToken()
//                        //     .Build();
//                        // //
//                        // PlayGamesPlatform.InitializeInstance(config);
//                        // //开启调试log(正式包关闭), 查看google服务输出信息
//                        // PlayGamesPlatform.DebugLogEnabled = true;
//                        // //激活谷歌服务
//                        // PlayGamesPlatform.Activate();
//                    }
//                    catch (Exception e)
//                    {
//                        Logger.error(e.ToString());
//                        return;
//                    }

//                    // //googleplaygame 登陆
//                    // PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (resulr) =>
//                    // {
//                    //     if (resulr == SignInStatus.Success)
//                    //     {
//                    //         //登录成功
//                    //     }
//                    // });
//                }
//                break;
//            case PlatformType.taptap:
//                break;
//        }
//    }

//    public string GetPlayerToken()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        if (Social.localUser != null)
//        {
//          //  return ((PlayGamesLocalUser)Social.localUser).GetIdToken();
//        }
      
//#endif
//        return "";
//    }
//    public void UnLogin(PlatformType platform)
//    {
//        switch (platform)
//        {
//            case PlatformType.googleplay:
//                break;
//        }
//    }
//#endif
//}


//public class PlatformInfo
//{
//    public PlatformType type;
//    public string token;
//    public string uid;
//    public string usename;
//}


//[DisallowMultipleComponent]
//public class SDKManager : SingletonMono<SDKManager>
//{
//    //     public static PlatformType platformType
//    //     {
//    //         get
//    //         {
//    // #if  UNITY_ANDROID
//    //             return PlatformType.googleplay;
//    // #elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)
//    //             return PlatformType.app_store;
//    // #else
//    //             return PlatformType.unityendtor;
//    // #endif
//    //         }
//    //     }

//#if UNITY_ANDROID
//    private SDKAndroid sDKAndroid;
//#elif (UNITY_IPHONE || UNITY_IOS)
//    private Platform_IOS sDKIos;
//#endif
//    public override void init()
//    {
//        base.init();

//        if (Application.internetReachability == NetworkReachability.NotReachable)
//        {
//            //无网络

//            return;
//        }

//#if !UNITY_EDITOR && UNITY_ANDROID
//        //sDKAndroid = new SDKAndroid();
//        ////
//        //switch(SDKManager.platformType)
//        //{
//        //    case PlatformType.googleplay:
//        //        {

//        //        }   
//        //        break;
//        //}
//#elif !UNITY_EDITOR && UNITY_IPHONE
//        sDKIos = new Platform_IOS();
//        sDKIos.InitSDK();
//#endif

//    }

//    /// <summary>
//    /// AppFlyer
//    /// </summary>
//    public void AppsFlyerEvent(string Event, string Value)
//    {
//#if UNITY_EDITOR
//        Logger.log("发送事件：" + Event + "__" + Value);
//#elif !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("AppsFlyerEvent", Event, Value);
//#elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)
        
//#endif
//    }
//    //
//    public void DFGameEvent(string e, string v)
//    {
//#if UNITY_EDITOR
//        Logger.log("发送事件：" + e + "__" + v);
//#elif !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("DFGameFinderEvent", e, v);
//#elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)
       
//#endif
//    }

//    //登录成功调用
//    public void onLoginSucceed(string uuid)
//    {
//#if UNITY_EDITOR
//        Logger.log("登录成功：" + uuid);
//#elif !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("GameLoginSucceed", uuid);
//#elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)
//       Logger.log("登录成功：" + uuid);
//#endif
//    }


//    //#if !UNITY_EDITOR && UNITY_ANDROID

//    // 在每个要被调用的方法添加委托
//    public void AndroidCallUnity(string str)
//    {
//        Logger.log("移动平台回调参数：" + str);
//        //参数类型是字符串 例： "eventtype&json}" 参数解析到各自功能方法内解析
//        var arges = str.Split('&');
//        if (arges.Length >= 1)
//        {
//            string eventtype = arges[0];
//            EventController.inst.TriggerEvent(eventtype, arges.Length >= 2 ? MiniJSON.Json.Deserialize(arges[1]) : null);
//        }
//    }
//    //#endif


//    private void As_SetLanguage(string langusge)
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        Logger.log("unity：当前系统语言 = "+langusge);
//        if(langusge.StartsWith("zh-TW"))
//        {
//            LanguageManager.inst.LanguageState(LanguageType.TRADITIONAL_CHINESE);
//        }
//        else if (langusge.StartsWith("zh"))
//        {
//            LanguageManager.inst.LanguageState(LanguageType.SIMPLIFIED_CHINESE);
//        }
//        else
//        {
//            LanguageManager.inst.LanguageState(LanguageType.ENGLISH);
//            //LanguageManager.inst.LanguageState(LanguageType.ENGLISH);
//        }
//#elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)
//        LanguageManager.inst.LanguageState(LanguageType.SIMPLIFIED_CHINESE);
//#elif UNITY_EDITOR
//        if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
//        {
//            LanguageManager.inst.LanguageState(LanguageType.SIMPLIFIED_CHINESE);
//        }
//        else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
//        {
//            LanguageManager.inst.LanguageState(LanguageType.TRADITIONAL_CHINESE);
//        }
//        else
//        {
//            LanguageManager.inst.LanguageState(LanguageType.ENGLISH);
//        }
//#endif
//        PlayerPrefs.SetString("languageType", LanguageManager.inst.curType.ToString());
//    }
//    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    //获得语言
//    public void GetLanguage()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        Logger.log("unity：获取当前系统语言 ");
//        sDKAndroid.Call("GetLanguage");
//#elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)

//#else
//        As_SetLanguage("");
//#endif
//    }

//    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    #region 充值
//    //获取充值商品价格列表
//    public void FindProductPriceList(string products)
//    {

//#if !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("GetProductList", products);
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//        //暂时无充值
//#elif UNITY_EDITOR
//        ProductPriceList("{\"com.tbmrsd.lastshop.099\":\"$99\",\"com.tbmrsd.lastshop.1299\": \"$1299\",\"com.tbmrsd.lastshop.1499\":\"$1499\",\"com.tbmrsd.lastshop.199\":\"$199\",\"com.tbmrsd.lastshop.1999\":\"$1999\",\"com.tbmrsd.lastshop.299\":\"$299\",\"com.tbmrsd.lastshop.2999\":\"$2999\",\"com.tbmrsd.lastshop.399\":\"$399\",\"com.tbmrsd.lastshop.499\":\"$499\",\"com.tbmrsd.lastshop.999\":\"$999\",\"com.tbmrsd.lastshop.9999\":\"$9999\"}");
//#endif
//    }
//    public void ProductPriceList(string prices)
//    {
//        Dictionary<string, System.Object> data = MiniJSON.Json.Deserialize(prices) as Dictionary<string, System.Object>;
//        Debug.Log("商品列表");
//        foreach (var kv in data)
//        {
//            Debug.Log($"{kv.Key}:{kv.Value}");
//            GamePay.inst.addProductPriceList(kv.Key, kv.Value.ToString());
//        }
//    }
//    //初始化
//    public void InitGamePaySdk()
//    {
//#if !UNITY_EDITOR &&UNITY_ANDROID
//        sDKAndroid.Call("GamePayInit");
//#elif !UNITY_EDITOR && (UNITY_IPHONE||UNITY_IOS)

//#endif
//    }
//    System.Action<bool, string, string, string, string> callback;
//    public void Pay(string payid, string userid, string gameOrderId, bool isInApp, System.Action<bool, string, string, string, string> _callback)
//    {
//        FGUI.inst.showGlobalMask(2); //遮罩2秒
//        callback = _callback;
//#if UNITY_EDITOR
//        Debug.Log("调用充值, 1秒钟后模拟回调");
//        //EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, "调用充值, 1秒钟后模拟回调");
//        GameTimer.inst.AddTimer(1, 1, () =>
//        {
//            callback?.Invoke(true, "", "", "", "1223423f32fds"); //模拟成功
//        });
//#elif UNITY_ANDROID
//        if(isInApp)
//            sDKAndroid.Call("InAppPay", payid, userid, gameOrderId, "1");
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)

//#endif


//    }
//    //获取商品回执
//    public void ProvideContent(string s)
//    {
//        Debug.Log("商品购买返回 : " + s);
//#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//         OnPayFinish($"1&{s}&"+"eeeee"+$"&{sDKIos.currgameOrderId}&" + "0000");
//         sDKIos.currgameOrderId = "";
//#endif
//    }

//    public void ShowProductList(string s)
//    {
//#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//        sDKIos.ShowProductList(s);
//#endif
//    }

//    public void OnPayFinish(string msg)
//    {
//        if (string.IsNullOrEmpty(msg)) return;
//        var datas = msg.Split('&');
//        string purchaseJson = "";
//        string purchaseSign = "";
//        string purchaseid = "";
//        string purchaseToken = "";
//        if (datas[0] == "1")
//        {
//#if UNITY_ANDROID
//            Dictionary<string, System.Object> data = MiniJSON.Json.Deserialize(datas[1]) as Dictionary<string, System.Object>;
//            purchaseid = data["productId"].ToString();
//#elif UNITY_IOS
//            purchaseid = datas[3];
//#endif
//            purchaseToken = datas[4];
//            purchaseJson = datas[1];
//            purchaseSign = datas[2];

//            if (callback != null)
//            {
//                Debug.Log("购买回调 gamepay : " + msg);
//                callback(datas[0] == "1", purchaseid, purchaseJson, purchaseSign, purchaseToken);
//            }
//            else
//            {

//            }
//        }
//        callback = null;
//    }

//    //检查订单
//    public void ReQueryPurchaess()
//    {

//#if !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("ReQueryPurchaess");
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)

//#else

//#endif
//    }
//    #endregion
//    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//    //退出
//    public void ExitApp()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("exit");
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//       Application.Quit();
//#elif UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
//    }
//    public static void Restart(int delay)
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        try
//        {
//            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//            {
//                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
//                {
//                    jo.Call("doRestart", delay);
//                }
//            }
//        }
//        catch (System.Exception ex)
//        {
//            Logger.log("安卓调用异常：" + ex);
//        }
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//    //重启app
//#endif
//    }
//    public void ToGooglePlay()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        sDKAndroid.Call("transferToGooglePlay");  //谷歌平台
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//        //appstore

//#endif
//        //SDKManager.inst.ExitApp();
//    }
//    #region 获取设备唯一标识
//    // Hash an input string and return the hash as
//    // a 32 character hexadecimal string.
//    static string getMd5Hash(string input)
//    {
//        if (input == "")
//            return "";
//        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
//        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
//        StringBuilder sBuilder = new StringBuilder();
//        for (int i = 0; i < data.Length; i++)
//            sBuilder.Append(data[i].ToString("x2"));
//        return sBuilder.ToString();
//    }

//    public static string GetDeviceUniqueIdentifier()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//        {
//            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
//            {
//                //安卓id
//                string android_id = jo.Call<string>("getAndroid_ID");
//                //mac
//                string mac = jo.Call<string>("getMacAddress");
//                mac = mac.Replace(':', '0');
//                string id = android_id + mac;
//                Debug.Log("getMacAddress:  c# android_id = " + id);
//                return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(id));
//            }
//        }
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//        return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(SystemInfo.deviceUniqueIdentifier));    //ios先使用
//#else
//        return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(SystemInfo.deviceUniqueIdentifier));  //
//#endif
//    }
//    #endregion
//    //
//    #region 登录sdk
//    System.Action<string> loginCallBack;
//    PlatformType currPlatForm;
//    System.Action<PlatformInfo> OnGetPlatFormInfo;
//    public void LoginSDk(PlatformType type)
//    {
//        // OnGetPlatFormInfo = callback;
//#if !UNITY_EDITOR && UNITY_ANDROID
//        switch (type)
//        {
//            case PlatformType.googleplay:

//                break;
//            case PlatformType.facebook:
//                break;
//        }
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//        //暂时没有sdk登陆
//#elif UNITY_EDITOR
//        AndroidCallUnity(SDKEvent.SDKEvent_LoginEnd + "&{\"UID\":\"abc13326\",\"Token\": \"iwe2422rf0s90df98dddg9ds9g98sef89es\",\"useName\":\"testname\"}");
//#endif
//    }
//    #endregion


//    public void onSceneOrientation(int type)
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//    sDKAndroid.Call("SaveSceneOrientation", type);
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
    
//#endif
//    }

//    #region 广告
//#if !UNITY_EDITOR && UNITY_ANDROID
//    //public IronSourceAdMgr ironSourceAdMgr;
//#endif

//    //初始化
//    public void InterstitialAdsSDK()
//    {

//#if !UNITY_EDITOR && UNITY_ANDROID
////  if (ironSourceAdMgr == null)
////             ironSourceAdMgr = new IronSourceAdMgr();
////         sDKAndroid.Call("InterstitialAds");
////         ironSourceAdMgr.init("13eff3ff1");
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)

//#else
//        //
//        Debug.Log("初始化广告sdk");
//#endif
//    }

//    //判断广告是否准备好
//    public bool IsRewardedVideoAvailable()
//    {

//#if !UNITY_EDITOR && UNITY_ANDROID
//        return false;//ironSourceAdMgr.IsRewardedVideoAvailable();
//#elif !UNITY_EDITOR && Unity_IOS
//        return false;
//#else
//        return true;
//#endif

//    }

//    public void loadVideoAvailable()
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//        // if(!ironSourceAdMgr.IsRewardedVideoAvailable())
//        //  ironSourceAdMgr.loadRewardedVideo();
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//#endif
//    }
//    void OnApplicationPause(bool isPaused)
//    {
//#if !UNITY_EDITOR && UNITY_ANDROID
//       // IronSource.Agent.onApplicationPause(isPaused);
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)
//#endif
//    }
//    //播放广告
//    public void PlayRewardedVideo(string adtype)
//    {
//        if (SaveManager.inst != null)
//        {
//            SaveManager.inst.SaveInt("lastADPlayTime", TimeUtils.GetNowSeconds());
//        }
//#if !UNITY_EDITOR && UNITY_ANDROID
//        //sDKAndroid.Call("PlayRewardedVideo", adtype);
//       // ironSourceAdMgr.ShowRewardedVideo(adtype);
//#elif !UNITY_EDITOR && (UNITY_IPHONE || UNITY_IOS)

//#else
//        //测试
//        GameTimer.inst.AddTimer(2, 1, () =>
//        {
//            //  EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdRewarded, adtype);//奖励
//            //  EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdClosed, adtype); //关闭
//        });
//#endif
//    }
//    #endregion
//}

//public class SDKEvent
//{
//    public readonly static string SDKEvent_Login = "SDKEvent_Login";
//    public readonly static string SDKEvent_LoginEnd = "SDKEvent_LoginEnd";


//}



