using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine;

//游戏平台管理
//
//
//
//PlatformType  平台类型 根据不同平台做不同处理
//
public enum PlatformType
{
    unityendtor = 0,
    googleplay = 1,
    IOS = 2,
    qkgameandroid = 3,
    qkgameios = 4
}


[DisallowMultipleComponent]
public class PlatformManager : SingletonMono<PlatformManager>
{
    //===========================================================================================================================
    // 获取平台类型 PlatformType;
#if UNITY_EDITOR
    public int GetPlatformType()
    {
        return 0;
    }
#elif UNITY_IOS
    [DllImport("__Internal")]
    private static extern int PlatformType();//当前平台类型

    public int GetPlatformType()
    {
        return PlatformType();
    }
#elif UNITY_ANDROID
    public int GetPlatformType()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                //安卓id
                int type = jo.Call<int>("PlatformType");
                return type;
            }
        }
        return 4;
    }
#endif
    //===========================================================================================================================
    public static PlatformType platform;

    private PlatformSDKBase platformprxy;
    public override void init()
    {
        base.init();
        //获取平台
        platform = (PlatformType)GetPlatformType();

        //初始化平台代理
        switch ((int)platform)
        {
            case 0:
                Debug.Log("平台:" + platform.ToString());
#if UNITY_EDITOR
                platformprxy = new Platform_EDITOR();
#endif
                break;
            case 4:
                Debug.Log("平台:" + platform.ToString());
#if UNITY_IOS
                platformprxy = new Platform_IOS();
#endif
                break;
            case 3:
                Debug.Log("平台:" + platform.ToString());
#if UNITY_ANDROID
                platformprxy = new Platform_Android();
#endif
                break;
            default:
                break;
        }
        if (platformprxy == null)
        {
            Debug.LogError("平台代理初始化失败！请定义平台类型！");
            // Application.Quit();
        }

        //验证so
        try { GetCustomData(""); } catch (System.Exception e) { Logger.logException(e); }
        //验证signatures
        try { GetCustomData2(""); } catch (System.Exception e) { Logger.logException(e); }
        //验证PackageManager
        try { GetCustomData3(""); } catch (System.Exception e) { Logger.logException(e); }
        //验证是否为模拟器
        try { GetCustomData4(""); } catch (System.Exception e) { Logger.logException(e); }
    }
    //===========================================================================================================================
    //出特定方法回调外，通用数据回调 回调参数包含事件名，和逻辑参数（统一字符串 参数之间 '\t' 分割） 根据不同平台解析
    public void PlatformCallUnity(string msg)
    {
        if (string.IsNullOrEmpty(msg)) return;
        string[] data = msg.Split('\t');
        if (platformprxy != null)
        {
            platformprxy.SdkCallUnity(data);
        }
    }
    //===========================================================================================================================
    //初始化SDK
    //游戏启动优先调用
    public void InitPlatformSDK()
    {
        if (platformprxy != null)
        {
            GameHandleEventLog("Before_InitSDK", "");
            platformprxy.InitSDK();
        }
    }
    //sdk 初始化完成可进入游戏
    /// state = 0 初始化失败  state=1 初始化成功
    public void OnPlatformSDKInited(string state)
    {
        if (platformprxy != null)
        {
            GameHandleEventLog("InitSDK_Success", "");
            platformprxy.OnPlatformSDKInited(int.Parse(state));
        }
    }

    //如果需要初始化完成前调用
    public void updateGameServerURl(string serverhost)
    {
        CSGameStart.Inst.setGameServerUrl(serverhost, serverhost);
    }

    //====================================================获取设备唯一标识========================================================
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern string GetUUIDByKeyChain();
#endif

    public string GetDeviceUniqueIdentifier()
    {

#if UNITY_EDITOR
        return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(SystemInfo.deviceUniqueIdentifier));  //
#elif UNITY_ANDROID
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                //安卓id 两个方法
                string android_id = jo.Call<string>("getAndroid_ID");
                //mac 两个方法
                string mac = jo.Call<string>("getMacAddress"); 
                mac = mac.Replace(':', '0');
                string id = android_id + mac;
                Debug.Log("getMacAddress:  c# android_id = " + id);
                return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(id));
            }
        }
#elif UNITY_IOS
        return "";
#endif
    }
    //=================================================登录相关======================================================
    //登陆sdk
    public void LoginSdk()
    {
        FGUI.inst.sdkLoginBtn.gameObject.SetActive(true);
        if (platformprxy != null)
        {
            platformprxy.Login();
        }
    }

    //登录验证失败 需重新登陆
    public void ReLoginSdk()
    {
        FGUI.inst.sdkLoginBtn.gameObject.SetActive(true);
        if (platformprxy != null)
        {
            platformprxy.ReLogin();
        }
    }

    public void OnLoginSuccess(string data)
    {
        Debug.Log("登陆返回！！" + data);
        FGUI.inst.sdkLoginBtn.gameObject.SetActive(false);
        if (platformprxy != null)
        {
            GameHandleEventLog("Login_Success", "");
            platformprxy.OnLoginSuccess(data);
        }

        GameHandleEventLog("Open", "");
        GUIManager.OpenView<GameLoginView>(view =>
        {
            view.setLastAccountInput(AccountDataProxy.inst.GetAccount());
        });
    }

    public void setRoleInfo(string info)
    {
        if (platformprxy != null)
        {
            platformprxy.UpdateRoleInfo(info);
        }
    }
    //登出sdk
    public void LogoutSdk()
    {
        if (platformprxy != null)
        {
            platformprxy.Logout();
        }
    }

    //sdk登陆回调 结构 "{json字符串}"
    public void SDKLoginCallBack(string msg)
    {
        if (platformprxy != null)
        {
            GameHandleEventLog("Login_Success", "");
            platformprxy.LoginCallBack(msg);
        }
    }

    public string GetUserToken()
    {
        if (platformprxy != null)
        {
            return platformprxy.GetUserToken();
        }
        return "";
    }

    public string GetUserID()
    {
        if (platformprxy != null)
        {
            return platformprxy.GetUserID();
        }
        return "";
    }
    public string GetChannelType()
    {
        if (platformprxy != null)
        {
            return platformprxy.GetChannelType();
        }
        return "";
    }
    //===============================================充值相关====================================================================
    //获取商品信息
    /*
    *productids = @"product1\tproduct2\tproduct3"
    */
    public void GetProductPriceList(string productids)
    {
        if (!string.IsNullOrEmpty(productids) && platformprxy != null)
        {
            platformprxy.getProductInfo(productids);
        }
    }

    //返回商品信息
    public void ShowProductList(string infos)
    {
        if (platformprxy != null)
        {
            platformprxy.ShowProductList(infos);
        }
    }

    //购买商品
    public void PayProduct(string product, string userid, string gameOrder, bool isInApp, System.Action<bool, string, string, string, string> _callback)
    {
        if (platformprxy != null)
        {
            platformprxy.pay(product, userid, gameOrder, isInApp, _callback);
        }
    }

    //充值完成回调
    public void OnPayFinish(string msg)
    {
        if (platformprxy != null)
        {
            platformprxy.PayFinish(msg);
        }

    }

    //===============================================广告相关====================================================================
    //加载一个广告
    public void LoadVideoAvailable()
    {
        if (platformprxy != null)
        {
            platformprxy.LoadVideoAvailable();
        }
    }
    //播放广告
    public void PlayRewardedVideo(string adtype)
    {
        if (platformprxy != null)
        {
            platformprxy.PlayRewardedVideo(adtype);
        }
    }


    public void OnRewardedVideoOpen(string msg)
    {
        //广告界面打开
        AudioManager.inst.PauseAll(true);
    }

    //激励广告达成奖励
    public void RewardedVideoAdRewarded(string msg)
    {
        if (platformprxy != null)
        {
            platformprxy.RewardedVideoAdRewarded();
        }
    }

    public void OnRewardedVideoPlayError(string msg)
    {
        AudioManager.inst.PauseAll(false);
    }
    //关闭广告
    public void RewardedVideoClose(string msg)
    {
        AudioManager.inst.PauseAll(false);
        if (platformprxy != null)
        {
            platformprxy.RewardedVideoClose();
        }
    }
    //广告是否准备好
    public bool IsRewardedVideoAvailable()
    {
        if (platformprxy != null)
        {
            return platformprxy.IsRewardedVideoAvailable();
        }
        return false;
    }
    //===============================================游戏打点====================================================================
    public void GameHandleEventLog(string handlename, string value)
    {
        if (platformprxy != null)
        {
            platformprxy.GameLogEvent(handlename, value);
        }
    }
    //===============================================APP 内评价===============================================================
    public void OnSDKLoginout()
    {
        //当用户退出或销毁

        if (ManagerBinder.inst.mGameState == kGameState.Login)
        {
            //登出
            if (platformprxy != null)
            {
                platformprxy.Logout();

#if UNITY_ANDROID
                platformprxy.Login();
#endif
            }
        }
        else
        {
            EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, "您的账号已经在用户中心退出，请重新进入并登陆游戏！", () =>
            {
                //
                ReLoginGame();
                platformprxy.Logout();
            });
        }
    }
    //==================================================其他=================================================================

    //==================================================跳转应用页进行更新===================================================
    public void ToAppUpdateUrl()
    {
        if (platformprxy != null)
        {
            platformprxy.ToAppUpdateUrl();
        }
    }


    //==============================================屏幕旋转=================================================================
    public void ScreenOrientationChange(int type)
    {
        if (platformprxy != null)
        {
            platformprxy.OnScreenOrientationChange(type == 1 ? "L" : "H");
        }
    }
    //=============================================返回登陆界面 and 退出游戏 and 重启（安卓退出重启，ios 无效）================================================================
    public void ReLoginGame() //返回登陆界面
    {
        Time.timeScale = 1;
        if (HotfixBridge.inst != null && ManagerBinder.inst != null)
        {
            if ((int)ManagerBinder.inst.mGameState > (int)kGameState.Login)  //进入游戏之后才可以有改操作操作
            {
                HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Login, true));
            }
        }
    }

    //退出游戏
    public void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        return;
#endif
        Application.Quit();
    }

    public void Restart()
    {
        ExitApp();
    }
    //===========================================================================================================================
    //type  1准备 2弹出

    public void show5star()
    {
        if (platformprxy != null)
        {
            platformprxy.show5star();
        }
    }

    public void setFloatBtnVisbale(bool visbale)
    {
        if (platformprxy != null)
        {
            platformprxy.setFloatBtnVisbale(visbale);
        }
    }

    //===================================================== 移动设备(Android)点击返回 打开游戏中退出框界面 ==================================================
    public void OnClickBackBtn(string msg)
    {
        EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OKCANCLE_MSGBOX, "是否退出游戏?", () =>
        {
            Application.Quit();
        });
    }

    //================================================ 用户接受隐私协议 ==================================================
    public void ShowPrivacyPanel(string msg)
    {
        if (FGUI.inst != null)
        {
            FGUI.inst.privacyPanel.showPrivacyPanel();
        }
    }

    public void PrivacyCallBack()
    {
        if (platformprxy != null)
        {
            platformprxy.acceptPrivacy();
        }
    }
    //===========================================================================================================================
    //============
    //.so verification
    [NonSerialized] public string s1 = "";
    [NonSerialized] public string s2 = "";
    [NonSerialized] public string s3 = "";
    [NonSerialized] public string s4 = "";
    public void GetCustomData(string content)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string h = "";
       {
            //Debug.Log("psi start");
            var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
            var dp = activity.Call<string>("getPackageCodePath");//dex path
            var ai = activity.Call<AndroidJavaObject>("getApplicationInfo");
            var nd = ai.Get<string>("nativeLibraryDir");//nativeLibraryDir;
            var clz = new AndroidJavaClass("java.lang.ClassLoader");
            var cl = clz.CallStatic<AndroidJavaObject>("getSystemClassLoader");
            //var pcl = new AndroidJavaClass("dalvik.system.PathClassLoader");
            var pcl = new AndroidJavaObject("dalvik.system.PathClassLoader", dp, nd, cl);
            var r = pcl.Call<string>("findLibrary", "il2cpp");
            //Debug.Log("psi r:" + r);

            var fl = new AndroidJavaObject("java.io.File", r);
            long len = fl.Call<long>("length");
            //Debug.Log("psi fl len:" + len);
            //byte[] bbb = new byte[(int)len];
            System.IntPtr btsPtr = AndroidJNI.NewByteArray((int)len);//AndroidJNI.ToByteArray(bbb);
            try {
                var fis = new AndroidJavaObject("java.io.FileInputStream", fl);
                int readed = 0;
                //int state = 0;
                //readed = fis.Call<int>("read", btsPtr);
                System.IntPtr mid = AndroidJNIHelper.GetMethodID(fis.GetRawClass(), "read", "([BII)I");// "(byte[];int;int;)int;");
                jvalue[] jvs = new jvalue[3];
                jvs[0].l = btsPtr;
                jvs[1].i = 0;
                jvs[2].i = (int)len;
                readed = AndroidJNI.CallIntMethod(fis.GetRawObject(), mid, jvs);
                //Debug.Log("psi readed:" + readed);
                fis.Call("close");
                //AndroidJNI.ExceptionClear();
            } catch (System.Exception e) {

                //Debug.LogError(e);
            }
            byte[] bts = AndroidJNIHelper.ConvertFromJNIArray<byte[]>(btsPtr);
            //Debug.Log("psi bts len:" + bts.Length);
            if (bts != null) {
                string b = FileUtils.GetBytesMD5(bts);
                //Debug.Log("psi b:" + b);
                h += b + bts.Length;
                //Debug.Log("HHHH so hash h:" + h);
                var upload_device_id = FileUtils.GetBytesMD5(System.Text.Encoding.UTF8.GetBytes(h));
                //Debug.Log("HHHH id:" + upload_device_id);
                upload_device_id = upload_device_id.Substring(UnityEngine.Random.Range(0, 10));
                //Debug.Log("HHHH fid:" + upload_device_id);
                s1 = upload_device_id;
            }
            //Debug.Log("psi end");
        }
#endif
    }
    //signatures verification
    public void GetCustomData2(string content)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        //var ai = activity.Call<AndroidJavaObject>("getApplicationInfo");
        var pm = activity.Call<AndroidJavaObject>("getPackageManager");
        var pmclz = new AndroidJavaClass("android.content.pm.PackageManager");
        var pkgName = activity.Call<string>("getPackageName");
        var version_clz = new AndroidJavaClass("android.os.Build$VERSION");
        var version_codes_claz = new AndroidJavaClass("android.os.Build$VERSION_CODES");
        int sdk_int = version_clz.GetStatic<int>("SDK_INT");
        int version_code = 28;//version_codes_claz.GetStatic<int>("P");
        // get sig
        var sig_sha1 = "";
        AndroidJavaObject sig_obj = null;
        if(sdk_int >= version_code) {
            var sig_id = pmclz.GetStatic<int>("GET_SIGNING_CERTIFICATES");
            var pkg_info = pm.Call<AndroidJavaObject>("getPackageInfo", pkgName, sig_id);
            var sig_info = pkg_info.Get<AndroidJavaObject>("signingInfo");
            sig_obj = sig_info.Call<AndroidJavaObject>("getApkContentsSigners");
            
        } else {
            var sig_id = pmclz.GetStatic<int>("GET_SIGNATURES");
            var pkg_info = pm.Call<AndroidJavaObject>("getPackageInfo", pkgName, sig_id);
            sig_obj = pkg_info.Get<AndroidJavaObject>("signatures");
        }

        if(sig_obj.GetRawObject().ToInt32() != 0) {
            var sigs = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(sig_obj.GetRawObject());
            var all_sig = "";
            for(int i = 0; i < sigs.Length; i++) {
                var sig = sigs[i];
                all_sig += sig.Call<string>("toCharsString");
            }
            sig_sha1 = EncryptoUtils.EncryptSHA1(all_sig, System.Text.Encoding.UTF8);
        }

        sig_sha1 = sig_sha1.Replace("-", "");
        if(!string.IsNullOrEmpty(sig_sha1)) {
            var random_sig_sha1 = sig_sha1.Substring(UnityEngine.Random.Range(0, 10));
            
        }
        s2 = sig_sha1;

#if UNITY_EDITOR
        Debug.Log("sig_sha1:"+sig_sha1);
#endif
#endif
    }
    //packagemanager verification
    public void GetCustomData3(string content)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string pmName = "android.content.pm.IPackageManager$Stub$Proxy";
        string curName = "";

        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        var pm = activity.Call<AndroidJavaObject>("getPackageManager");
        var pm_clz = pm.Call<AndroidJavaObject>("getClass");
        var pm_field = pm_clz.Call<AndroidJavaObject>("getDeclaredField", "mPM");
        pm_field.Call("setAccessible", true);
        var mpm = pm_field.Call<AndroidJavaObject>("get", pm);
        var mpm_clz = mpm.Call<AndroidJavaObject>("getClass");
        curName = mpm_clz.Call<string>("getName");
        
        s3 = curName;
#if UNITY_EDITOR
        Debug.Log("pm name equals:" + (pmName==curName)+ " curName:" + curName);
#endif
#endif
    }

    public void GetCustomData4(string content)
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        
        var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
        var detect_class = new AndroidJavaClass("com.snail.antifake.jni.EmulatorDetectUtil");
        var aggressive = detect_class.CallStatic<bool>("isEmulatorFromAll", activity);
        var imeiutil_class = new AndroidJavaClass("com.snail.antifake.deviceid.AndroidDeviceIMEIUtil");
        var conservative = imeiutil_class.CallStatic<bool>("isRunOnEmulator", activity);

        s4 = "&agg=" + aggressive + "&con=" + conservative;
#endif
    }

    public void UpdateCustomData(string content)
    {
#if UNITY_IOS
        s4 = content;
#endif
    }

    public void SetUserAgent(string UA)
    {
        GameManager.userAgent = UA.IfNullThenEmpty();
    }

    //================================================ 本地推送 ==================================================
    public bool CheckUserNotificationEnable() //用户是否允许本地推送
    {
        Debug.Log("localPushLog----   查看用户是否同意本地推送");

        if (platformprxy != null)
        {
            return platformprxy.CheckUserNotificationEnable();
        }
        return false;
    }

    /// <summary>
    /// 添加本地推送
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="subTitle">副标题</param>
    /// <param name="body">文本内容</param>
    /// <param name="badge">角标数字</param>
    /// <param name="secs">标准时间戳</param>
    /// <param name="identifier">通知标识 可用于更新或删除此推送内容</param>
    public void AddLocalNotice(string title, string subTitle, string body, int badge, double secs, string identifier)
    {
        Debug.Log("localPushLog----   添加本地推送 title:" + title + "  subTitle:" + subTitle + "  body:" + body + "  badge:" + badge + "  secs:" + secs + "  identifier:" + identifier);
        if (platformprxy != null)
        {
            platformprxy.AddLocalNotice(title, subTitle, body, badge, secs, identifier);
        }
    }

    public void RemoveOneNotificationWithID(string noticeID)
    {
        Debug.Log("localPushLog----   移除本地推送 noticeID：" + noticeID);
        if (platformprxy != null)
        {
            platformprxy.RemoveOneNotificationWithID(noticeID);
        }
    }

    public void RemoveAllNotification()
    {
        Debug.Log("localPushLog----   移除所有本地推送");
        if (platformprxy != null)
        {
            platformprxy.RemoveAllNotification();
        }
    }

    public bool CheckHaveOneNotificationWithID(string noticeID)
    {
        Debug.Log("localPushLog----   查看该推送是否存在 noticeID：" + noticeID);
        if (platformprxy != null)
        {
            platformprxy.CheckHaveOneNotificationWithID(noticeID);
        }
        return false;
    }

    public void RemoveAppIconBadge() 
    {
        Debug.Log("localPushLog----   清除角标");
        if (platformprxy != null)
        {
            platformprxy.RemoveAppIconBadge();
        }
    }

}
