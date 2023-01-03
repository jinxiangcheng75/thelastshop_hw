using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
public class Platform_Android : PlatformSDKBase
{
    private string userId;
    private string userToken;
    private string channelType;
    string curradtype = "";

    private Action<bool, string, string, string, string> _asCallBack;

    public void Call(string function, params object[] args)
    {
        try
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call(function, args);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("安卓调用异常：" + ex);
        }
    }

    public void GameLogEvent(string logevent, string value)
    {
        Call("AppsFlyerEvent", logevent, value);
    }

    public string GetChannelType()
    {
        if (channelType != null)
        {
            return channelType;
        }

        return "";
    }

    public int GetUpdateUrlChannelId()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                //安卓id
                int channelId = jo.Call<int>("GameGetChannelId");
                return channelId;
            }
        }
        return 0;
    }

    public void ToAppUpdateUrl()
    {
        int channelId = GetUpdateUrlChannelId();
        if (channelId != 0)
        {
            var urlCfg = UrlUpdateConfigManager.inst.GetConfig(channelId);
            if (urlCfg != null)
            {
                Application.OpenURL(urlCfg.url_update);
            }
        }
    }

    public void getProductInfo(string product)
    {
    }

    public string GetUserID()
    {
        if (userId != null)
        {
            return userId;
        }

        return "";
    }

    public string GetUserToken()
    {
        if (userToken != null)
        {
            return userToken;
        }

        return "";
    }

    public void InitSDK()
    {
        var url_update_cfg = PlayerPrefs.GetString("url_update_android", "");
        Call("GameInit", FGUI.inst.isLandscape, url_update_cfg);
    }

    public bool IsGuest()
    {
        throw new NotImplementedException();
    }

    public bool IsRewardedVideoAvailable()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                //安卓id
                bool isAvailable = jo.Call<bool>("IsRewardedVideoAvailable");
                return isAvailable;
            }
        }
        return false;
    }

    public bool IsSDKLogined()
    {
        throw new NotImplementedException();
    }

    public void LoadVideoAvailable()
    {
        Call("GameLoadVideo", FGUI.inst.isLandscape ? 2 : 1);
    }

    public void Login()
    {
        Call("GameLogin");
    }

    public void ReLogin()
    {
        Call("GameReLogin");
    }

    public void LoginCallBack(object msg)
    {
        
    }

    public void Logout()
    {
        Call("GameExit");
    }

    public void OnLoginSuccess(string data)
    {
        //if (!string.IsNullOrEmpty(data))
        //{
        //    Call("GameSetRoleInfo", data);
        //}
        if (!string.IsNullOrEmpty(data))
        {
            var accountInfo = data;
            var infos = accountInfo.Split('|');

            if (infos.Length > 0)
                userId = infos[0];
            if (infos.Length > 1)
                userToken = infos[1];
            if (infos.Length > 2)
                channelType = infos[2];
        }
    }

    public void OnPlatformSDKInited(int state)
    {
        //初始化完成
        if (state == 0)
        {
            NetworkErrorPanel.inst.showState(3);
        }
        else
        {
            if (CSGameStart.Inst != null)
            {
                CSGameStart.Inst.StartGame();
            }
        }
    }

    public void OnScreenOrientationChange(string orientation)
    {
        if (!string.IsNullOrEmpty(orientation))
        {
            Call("SaveSceneOrientation", orientation == "L" ? 1 : 0);
        }
    }

    string currgameOrderId;
    public void pay(string product, string userid, string gameOrder, bool isInApp, System.Action<bool, string, string, string, string> _callback)
    {
        currgameOrderId = gameOrder;

        Call("GamePay", product, userid, gameOrder, isInApp);
        _asCallBack = _callback;

        FGUI.inst.ShowGolbalAwaitMask();
    }

    public void PayFinish(string data)
    {
        Debug.Log("充值回调！！！！！！" + data);

        if (data == "cancel")
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
        else if (data == "error")
        {
            FGUI.inst.HideGolbalAwaitMask();
            Debug.Log("充值失败！！！！！！");
        }
        else
        {
            if (!string.IsNullOrEmpty(data))
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
                string purchaseJson = data;
                string purchaseSign = "sss";

                if (_asCallBack != null)
                {
                    _asCallBack(true, purchaseid, purchaseJson, purchaseSign, purchaseToken);
                }

                currgameOrderId = "";
                _asCallBack = null;
            }
        }

        int ecoMode = SaveManager.inst.GetInt("EcoMode", false);
        Application.targetFrameRate = ecoMode == 1 ? 35 : 60;

    }

    public void PlayRewardedVideo(string adtype)
    {
        curradtype = adtype;
        Call("GamePlayRewardedVideo", adtype);
    }

    public void RewardedVideoAdRewarded()
    {
        EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdRewarded, curradtype);//奖励
    }

    public void RewardedVideoClose()
    {
        EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdClosed, curradtype); //关闭
    }

    public void SdkCallUnity(string[] data)
    {
    }

    public void setFloatBtnVisbale(bool visbale)
    {
        Call("GameSetFloatBtnVisbale", visbale);
    }

    public void ShowProductList(string s)
    {
    }

    public void ShowUserCenter()
    {
    }

    public void UpdateRoleInfo(string info)
    {
        if (!string.IsNullOrEmpty(info))
        {
            Call("GameSetRoleInfo", info);
        }
    }

    #region APP内五星好评

    public void show5star()
    {


    }
    #endregion

    public void acceptPrivacy()
    {
        Call("GameAcceptPrivacy");
    }

    public bool CheckUserNotificationEnable()
    {
        return false;
    }

    public void AddLocalNotice(string title, string subTitle, string body, int badge, double secs, string identifier)
    {
    }

    public void RemoveOneNotificationWithID(string noticeID)
    {
    }

    public void RemoveAllNotification()
    {
    }

    public bool CheckHaveOneNotificationWithID(string noticeID)
    {
        return false;
    }

    public void RemoveAppIconBadge()
    {
    }

}
#endif
