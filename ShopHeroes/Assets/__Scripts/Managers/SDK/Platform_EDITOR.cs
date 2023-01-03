using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
public class Platform_EDITOR : PlatformSDKBase
{
    bool isInit = false;
    public void GameLogEvent(string logevent, string value)
    {
        Logger.log($"游戏打点！！！     {logevent} : {value}");
    }

    public string GetChannelType()
    {
        return "";
    }

    public void ToAppUpdateUrl() 
    {

    }

    public void getProductInfo(string product)
    {

    }

    public string GetUserID()
    {
        return "";
    }

    public string GetUserToken()
    {
        return "";
    }

    public void InitSDK()
    {
        //初始化sdk
        Logger.log("初始化sdk", GUIHelper.GetHexColorByColor(Color.yellow));
        isInit = true;
        // CSGameStart.Inst.setGameServerUrl("https://gate-shop-cn.poptiger.cn", "https://gate-shop-cn.poptiger.cn");
        OnPlatformSDKInited(1);
    }

    public bool IsRewardedVideoAvailable()
    {
        return true;
    }

    public void LoadVideoAvailable()
    {

    }

    public void UpdateRoleInfo(string info)
    {

    }

    public void Login()
    {
        //遮罩
        FGUI.inst.setGlobalMaskActice(true);
        Logger.log("登录平台", GUIHelper.GetHexColorByColor(Color.yellow));
        string Token = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(SystemInfo.deviceUniqueIdentifier));
        PlatformManager.inst.SDKLoginCallBack("{\"UID\":\"abc13326\",\"Token\": \"Token\",\"useName\":\"testname\"}");
    }

    public void ReLogin()
    {
        Logger.log("登陆失败重新调起登录!");
    }

    public void LoginCallBack(System.Object msg)
    {
        //登陆返回
        Logger.log("登录成功" + msg as string);

        FGUI.inst.setGlobalMaskActice(false);
    }

    public void Logout()
    {
        Logger.log("退出平台登陆!");
    }

    public void OnScreenOrientationChange(string orientation)
    {
        throw new NotImplementedException();
    }

    System.Action<bool, string, string, string, string> callback;
    //
    public void pay(string productid, string userid, string gameOrder, bool isInApp, System.Action<bool, string, string, string, string> _callback)
    {
        Debug.Log("调用充值, 1秒钟后模拟回调");

        GameTimer.inst.AddTimer(1, 1, () =>
        {
            callback?.Invoke(true, "", "", "", "1223423f32fds"); //模拟成功
        });
    }

    public void PayFinish(string data)
    {

    }
    string curradtype = "";
    public void PlayRewardedVideo(string adtype)
    {
        curradtype = adtype;
        Logger.log("播放广告");
        if (SaveManager.inst != null)
        {
            SaveManager.inst.SaveInt("lastADPlayTime", TimeUtils.GetNowSeconds());
        }
        GameTimer.inst.AddTimer(2, 1, () =>
        {

            RewardedVideoAdRewarded();
            RewardedVideoClose();
        });
    }

    public void RewardedVideoAdRewarded()
    {
        EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdRewarded, curradtype);//奖励
    }

    public void SdkCallUnity(string[] data)
    {

    }

    public void RewardedVideoClose()
    {
        EventController.inst.TriggerEvent(AdEvent.RewardedVideoAdClosed, curradtype); //关闭
    }

    public void OnLoginSuccess(string data)
    {
        //   string[] info = data.Split('\t');
        //Debug.Log("userID = " + info[0]);
        //Debug.Log("userToken = " + info[1]);
        //AccountDataProxy.inst.sdkUserID = info[0];
        //AccountDataProxy.inst.sdkUserToken = info[1];
    }

    public void ShowProductList(string s)
    {

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

    public void show5star()
    {
        
    }

    public void ShowUserCenter()
    {

    }

    public void setFloatBtnVisbale(bool visbale)
    {

    }

    public bool IsGuest()
    {
        throw new NotImplementedException();
    }

    public bool IsSDKLogined()
    {
        throw new NotImplementedException();
    }

    public void acceptPrivacy()
    {

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