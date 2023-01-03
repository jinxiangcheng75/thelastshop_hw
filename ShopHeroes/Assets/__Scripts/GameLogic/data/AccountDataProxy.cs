using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[XLua.LuaCallCSharp]
public class AccountData
{
    public string accountName;
    public string userID;
    public string deviceUniqueIdentifier = "";

    public void clear()
    {
        accountName = string.Empty;
        userID = string.Empty;
    }
}

public class AccountDataProxy : TSingletonHotfix<AccountDataProxy>, IDataModelProx
{
    private AccountData accountData;
    public bool isLogined = false;
    public EBindingType currbindingType = EBindingType.None;
    public bool bindingClaimState = false;
    public bool needCreatRole = false;
    public int awakeNeedCreatRole;
    //公告
    public List<Notice> mNotice = null;

    public string sdkUserID = "";
    public string sdkUserToken = "";

    public bool NeedCreatRole
    {
        get
        {
            return needCreatRole && WorldParConfigManager.inst.GetConfig(127).parameters == 1;
        }
    }

    public string userId
    {
        get
        {
            if (isLogined)
            {
                return accountData.userID;
            }
            return null;
        }
    }

    public string account
    {
        get
        {
            if (isLogined)
            {
                return accountData.accountName;
            }
            return string.Empty;
        }
    }
    public string deviceId
    {
        get
        {
            Debug.Log("getMacAddress:  c# GET accountData.deviceUniqueIdentifier = " + accountData.deviceUniqueIdentifier);
            return accountData.deviceUniqueIdentifier;
        }
    }
    public void Init()
    {
        if (accountData == null)
            accountData = new AccountData();

        accountData.deviceUniqueIdentifier = PlatformManager.inst.GetDeviceUniqueIdentifier();
        Debug.Log("getMacAddress:  c# accountData.deviceUniqueIdentifier = " + accountData.deviceUniqueIdentifier);
#if UNITY_EDITOR
        accountData.accountName = PlayerPrefs.GetString("account");
#endif
        // Helper.AddNetworkRespListener(MsgType.Response_User_Login_Cmd, GetUserLoginData);

        // Helper.AddNetworkRespListener(MsgType.Response_Gate_Cmd, GetGateData);
    }

    private void GetGateData(HttpMsgRspdBase msg)
    {
        var data = (Response_Gate)msg;

        if (data.noticeList.Count > 0)
        {
            var noticeData = data.noticeList.Find(t => t.lang == (int)LanguageManager.inst.curType);
            EventController.inst.TriggerEvent(GameEventType.NoticeEvent.SHOWUI_NOTICE, noticeData);
        }
    }


    public void Clear()
    {
        //清理数据
        OnLogOut();
    }
    //登陆成功
    public void OnLogin(Response_User_Login data)
    {
        isLogined = true;
        accountData.accountName = data.account;
        accountData.userID = data.userId;
        currbindingType = (EBindingType)data.bindingType;
        bindingClaimState = data.bindingClaimState == 1;

        var worldParCfg = WorldParConfigManager.inst.GetConfig(128);
        bool skipCreatRole = worldParCfg != null && worldParCfg.parameters == 1;
        awakeNeedCreatRole = data.isNewUser;

        needCreatRole = skipCreatRole && data.isNewUser == 1;

        PlayerPrefs.SetString("account", account);
        PlayerPrefs.SetString("accountToken", data.accountToken);
    }

    public string GetAccount()
    {
        return accountData.accountName;
    }
    public string GetToken()
    {
        if (PlayerPrefs.HasKey("accountToken"))
        {
            var accountToken = PlayerPrefs.GetString("accountToken");
            Logger.log("c# requestLogin" + accountToken);
            return PlayerPrefs.GetString("accountToken");
        }
        return "";
    }

    public void setNewToken(string newtoken)
    {
        PlayerPrefs.SetString("accountToken", newtoken);
        PlayerPrefs.Save();
    }
    //登出
    public void OnLogOut()
    {
        isLogined = false;
        if (accountData != null)
        {
            accountData.accountName = "";
            accountData.userID = "";
        }
    }

    #region  账号绑定
    public string platformUID = "";
    public string platformToken = "";

    public string platformUseName = "";
    #endregion
}