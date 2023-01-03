using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlatformSDKBase
{
    //初始化 sdk
    void InitSDK();

    #region  
    //登陆
    void Login();
    void ReLogin();
    //退出登录
    void Logout();
    //登陆回调
    void LoginCallBack(System.Object msg);

    void UpdateRoleInfo(string info);
    #endregion


    #region 充值
    //获取商品信息
    void getProductInfo(string product);
    //购买商品
    void pay(string product, string userid, string gameOrder, bool isInApp, System.Action<bool, string, string, string, string> _callback);

    void PayFinish(string msg);

    void ShowProductList(string s);
    #endregion

    #region 广告

    //广告是否准备好
    bool IsRewardedVideoAvailable();

    //加载一个广告
    void LoadVideoAvailable();

    //播放广告
    //adtype == 当前广告类型
    void PlayRewardedVideo(string adtype);

    //广告达成奖励回调
    void RewardedVideoAdRewarded();
    //广告关闭回调
    void RewardedVideoClose();
    #endregion

    //SDK回调
    void SdkCallUnity(string[] data);

    //游戏事件打点
    void GameLogEvent(string logevent, string value);

    //当屏幕旋转
    void OnScreenOrientationChange(string orientation);

    string GetUserToken();

    string GetUserID();

    string GetChannelType();

    void OnLoginSuccess(string data);

    void OnPlatformSDKInited(int state);

    void show5star();

    //跳转到对应url进行app更新
    void ToAppUpdateUrl();

    //打开用户中心
    void ShowUserCenter();

    //悬浮球按钮显隐
    void setFloatBtnVisbale(bool visbale);

    //是否使游客
    bool IsGuest();

    //是否已经登陆sdk
    bool IsSDKLogined();

    //接受隐私协议
    void acceptPrivacy();

    // ---本地推送---
    bool CheckUserNotificationEnable();

    void AddLocalNotice(string title, string subTitle, string body, int badge, double secs, string identifier);

    void RemoveOneNotificationWithID(string noticeID);

    void RemoveAllNotification();

    bool CheckHaveOneNotificationWithID(string noticeID);

    void RemoveAppIconBadge();

}
