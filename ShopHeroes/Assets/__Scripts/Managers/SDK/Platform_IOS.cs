using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
#if UNITY_IOS
public class Platform_IOS : PlatformSDKBase
{
    [DllImport("__Internal")]
    private static extern void login(); //登陆

    [DllImport("__Internal")]
    private static extern void reLogin(); //重新登陆

    [DllImport("__Internal")]
    private static extern void logout(); //登出

    [DllImport("__Internal")]
    private static extern bool isSDKLogined();

    [DllImport("__Internal")]
    private static extern void InitIAPManager();   //  初始化Sdk

    [DllImport("__Internal")]
    private static extern bool IsProductAvailable();//判断是否可以充值

    [DllImport("__Internal")]
    private static extern void RequestProductInfo(string s); // 获取商品信息

    [DllImport("__Internal")]
    private static extern void BuyProduct(string s);//购买商品

    [DllImport("__Internal")]
    private static extern void SaveSceneQrientation(string type);

    [DllImport("__Internal")]
    private static extern void updateRoleInfo(string info);

    [DllImport("__Internal")]
    private static extern void sendGameEvent(string eventname, string eventvalue); /// 游戏事件打点

    //广告
    [DllImport("__Internal")]
    private static extern bool isRewardedVideoAvailable(); //
    [DllImport("__Internal")]
    private static extern void playRewardedVideo();

    [DllImport("__Internal")]
    private static extern void showFloatBtn();

    [DllImport("__Internal")]
    private static extern void hideFloatBtn();

    [DllImport("__Internal")]
    private static extern void showUserCenter();

    [DllImport("__Internal")]
    private static extern bool isGuest();

    [DllImport("__Internal")]
    private static extern bool isLogined();

    [DllImport("__Internal")]
    private static extern bool ShowAppReview();

    [DllImport("__Internal")]
    private static extern bool toAppUpdateUrl();

    //推送
    [DllImport("__Internal")]
    private static extern bool checkUserNotificationEnable();
    [DllImport("__Internal")]
    private static extern void addLocalNotice(string title, string subTitle, string body, int badge, double secs, string identifier);
    [DllImport("__Internal")]
    private static extern void removeOneNotificationWithID(string noticeID);
    [DllImport("__Internal")]
    private static extern void removeAllNotification();
    [DllImport("__Internal")]
    private static extern bool checkHaveOneNotificationWithID(string noticeID);
    [DllImport("__Internal")]
    private static extern void removeAppIconBadge();


    public List<string> productInfo = new List<string>();
    public void InitSDK()
    {

        //if (PlayerPrefs.GetInt("privacy_state", -1) == -1)
        //{
        //    FGUI.inst.privacyPanel.showPrivacyPanel();
        //}
        //else
        //{
        if (CSGameStart.Inst != null)
        {
            Debug.Log("初始化sdk");
            InitIAPManager(); //初始化充值

            if (IsProductAvailable())
            {
                //如果支持内购 则请求商品列表
                string[] products = GamePayPricecConfigManager.inst.GetAllKey();
                string ids = "";
                foreach (string key in products)
                {
                    ids += key + "/t";
                }
                getProductInfo(ids);
            }
        }
        //}
    }

    //悬浮球显隐
    public void setFloatBtnVisbale(bool visbale)
    {
        if (visbale)
        {
            showFloatBtn();
        }
        else
        {
            hideFloatBtn();
        }
    }

    //显示用户中心
    public void ShowUserCenter()
    {
        showUserCenter();
    }

    //是否是游客
    public bool IsGuest()
    {
        return isGuest();
    }

    //是否sdk已经登陆
    public bool IsSDKLogined()
    {
        return isLogined();
    }


    public void Login()
    {
        Debug.Log("登录账号");
        login();
    }

    public void ReLogin()
    {
        Debug.Log("登陆失败重新调起登录!");
        reLogin();
    }
    public void Logout()
    {
        Debug.Log("退出登录");
        logout();
    }

    public void LoginCallBack(System.Object msg)
    {
        Debug.Log("登陆返回" + msg as string);
    }

    public void getProductInfo(string products)
    {
        Debug.Log("请求商品数据" + products);
        RequestProductInfo(products);
    }


    //获取product列表
    public void ShowProductList(string s)
    {
        productInfo.Add(s);
    }

    public string currgameOrderId = "";
    System.Action<bool, string, string, string, string> callback;

    public void pay(string product, string userid, string gameOrder, bool isInApp, System.Action<bool, string, string, string, string> _callback)
    {
        callback = _callback;
        string productid = product.Split('&')[0];
        Debug.Log("充值pay！！！！！！" + productid);
        // if (IsProductAvailable())
        //{
        FGUI.inst.ShowGolbalAwaitMask();

        currgameOrderId = gameOrder;
        BuyProduct(productid);
        //}
        //else
        //{
        //    Debug.Log("充值pay！！！！！！ 不支持");
        //}
        setFloatBtnVisbale(false);

    }
    public void PayFinish(string msg)
    {
        Debug.Log("充值回调！！！！！！" + msg);
        if (msg == "cancel")
        {
            FGUI.inst.HideGolbalAwaitMask();
            Debug.Log("充值取消！！！！！！");
            //发送订单取消的协议
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Pay_OrderCancel()
                {
                    gameOrderId = currgameOrderId
                }
            });

        }
        else if (msg == "error")
        {
            FGUI.inst.HideGolbalAwaitMask();
            Debug.Log("充值失败！！！！！！");
        }
        else
        {

            float waitTime = 15f;//默认15s

            var worldParCfg = WorldParConfigManager.inst.GetConfig(8602);
            if (worldParCfg != null)
            {
                waitTime = worldParCfg.parameters;
            }

            FGUI.inst.TimerHideGolbalAwaitMask(waitTime, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("充值订单好像丢了，请联系客服！"), GUIHelper.GetColorByColorHex("FF2828"));
            });

            string purchaseid = currgameOrderId;
            string purchaseToken = "000";
            string purchaseJson = msg;
            string purchaseSign = "sss";

            if (callback != null)
            {
                callback(true, purchaseid, purchaseJson, purchaseSign, purchaseToken);
            }
        }
        currgameOrderId = "";
        callback = null;
        setFloatBtnVisbale(true);

        int ecoMode = SaveManager.inst.GetInt("EcoMode", false);
        Application.targetFrameRate = ecoMode == 1 ? 35 : 60;
    }

    public void SdkCallUnity(string[] data)
    {

    }

    //数据打点
    public void GameLogEvent(string logevent, string value)
    {
        sendGameEvent(logevent, value);
    }

    public void OnScreenOrientationChange(string orientation)
    {
        SaveSceneQrientation(orientation);
    }

    public bool IsRewardedVideoAvailable()
    {
        return isRewardedVideoAvailable();
    }

    public void LoadVideoAvailable()
    {

    }
    string curradtype = "";
    public void PlayRewardedVideo(string adtype)
    {
        if (isRewardedVideoAvailable())
        {
            if (SaveManager.inst != null)
            {
                SaveManager.inst.SaveInt("lastADPlayTime", TimeUtils.GetNowSeconds());
            }
            curradtype = adtype;
            playRewardedVideo();
        }
        else
        {

        }
    }

    //达到奖励条件
    public void RewardedVideoAdRewarded()
    {
        EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdRewarded, curradtype);//奖励

    }
    //视频关闭
    public void RewardedVideoClose()
    {
        EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdClosed, curradtype);//关闭
    }

    public string GetUserToken()
    {
        string utoken = AccountDataProxy.inst.sdkUserToken;
        Debug.Log("返回uutoken" + utoken);
        return utoken;
    }

    public string GetUserID()
    {
        string uid = AccountDataProxy.inst.sdkUserID;
        Debug.Log("返回userid" + uid);
        return uid;
    }

    public string GetChannelType()
    {
        return "";
    }

    public void ToAppUpdateUrl()
    {
        toAppUpdateUrl();
    }

    //只穿userid
    public void UpdateRoleInfo(string info)
    {
        updateRoleInfo(info);

        //获取内购商品价格
        string str = GamePayPricecConfigManager.inst.GetAllProductIds();
        getProductInfo(str);
    }

    public void OnLoginSuccess(string data)
    {
        string[] info = data.Split('\t');
        Debug.Log("userID = " + info[0]);
        Debug.Log("userToken = " + info[1]);
        AccountDataProxy.inst.sdkUserID = info[0];
        AccountDataProxy.inst.sdkUserToken = info[1];
    }

    public void OnPlatformSDKInited(int state)
    {
        //初始化完成
        if (state == 0)
        {
            NetworkErrorPanel.inst.showState(3);
            //     EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, "初始化异常，请检查网络或重新安装重试！", () =>
            //    {
            //        //退出
            //        Application.Quit();
            //    });
        }
        else
        {
            if (CSGameStart.Inst != null)
            {
                CSGameStart.Inst.StartGame();
            }
        }
    }
    #region APP内五星好评

    public void show5star()
    {
        ShowAppReview();
    }
    #endregion

    public void acceptPrivacy()
    {
        PlayerPrefs.SetInt("privacy_state", 1);
        if (CSGameStart.Inst != null)
        {
            CSGameStart.Inst.StartGame();
        }
    }

    public bool CheckUserNotificationEnable()
    {
        return checkUserNotificationEnable();
    }

    public void AddLocalNotice(string title, string subTitle, string body, int badge, double secs, string identifier)
    {
        addLocalNotice(title, subTitle, body, badge, secs, identifier);
    }

    public void RemoveOneNotificationWithID(string noticeID)
    {
        removeOneNotificationWithID(noticeID);
    }

    public void RemoveAllNotification()
    {
        removeAllNotification();
    }

    public bool CheckHaveOneNotificationWithID(string noticeID)
    {
        return checkHaveOneNotificationWithID(noticeID);
    }

    public void RemoveAppIconBadge() 
    {
        removeAppIconBadge();
    }

}
#endif