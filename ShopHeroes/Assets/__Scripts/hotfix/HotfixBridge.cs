using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

public class DirectPurchaseData
{
    public string uid;
    public int id;
    public string bgIconAtlas;
    public string bgIcon;
    public string iconAtlas;
    public string icon;
}

public class TriggerData
{
    public int triggerType;
    public int triggerCondition;
    public int triggerVal;
    public string position;
}


[LuaCallCSharp]
[CSharpCallLua]
public class HotfixBridge : TSingleton<HotfixBridge>
{

    System.Action<IStateTransition> OnChangeState;
    //System.Action test;
    //System.Action<System.Action> testAction;
    System.Func<System.Type, bool, uiWindow> GetWindowFunc;
    System.Func<System.Type, System.Action<System.Object>, uiWindow> OpenViewFunc;
    System.Action<System.Object, System.Action<System.Object>> ShowViewFunc;
    System.Action<System.Type> CloseViewFunc;
    System.Action<System.Type> HideViewFunc;
    System.Func<System.Type, bool> ViewShowingFunc;
    System.Func<System.String, uiWindow> GetWindowByViewIdFunc;
    System.Func<List<uiWindow>> AllShowingViewFunc;
    System.Action BackToMainUI;
    System.Action BackAndChangeMainUI;
    System.Action ClearAllView;
    System.Func<System.Int32, System.Boolean> HasDirectPurchaseDataFunc;
    System.Func<System.Int32, DirectPurchaseData> GetDirectPurchaseDataFunc;
    System.Func<System.Boolean> HasBuyLevelGrowthFunc;


    System.Func<System.Int32> GetVipRemainTimeFunc;
    System.Func<System.Int32, System.Boolean> GetTriggerIsTrigFunc;
    System.Func<TriggerData> GetTriggerDataFunc;

    System.Func<bool> GetActivity_WorkerGameFlagFunc;
    System.Func<int, bool> GetActivity_WorkerGameEquipCanAddRateByDrawingIdFunc;
    System.Func<int, int> GetActivity_WorkerGameEquipMakeIntegralByDrawingIdFunc;
    System.Func<long> GetActivity_WorkerGameCoinCountFunc;
    System.Func<int, string> GetActivity_WorkerGameStringFunc;

    System.Func<bool> GetActivity_GoldenCityFlagFunc;
    System.Func<System.Int32> GetActivity_GoldenCityCanRewardCountFunc;
    System.Func<System.Int32> GetActivity_GoldenCityCurScoreLvFunc;

    System.Func<bool> HaveTimeLimitActivitySelfScoreFunc;

    System.Func<System.Int32, System.Int32> GetLuxuryBuffFunc;

    System.Func<DirectPurchaseData> GetRefugeDataFunc;

    System.Func<TriggerData> GetRuinsBattleFunc;

    System.Func<uiWindow> CurrWindowFunc;
    System.Func<string> CurrWindowViewID;
    System.Func<string, bool> GetViewIsShowingByViewIDFunc;
    System.Action hbFunc;

    System.Func<bool> GetShopkeeperCanMoveToCounterFunc;
    System.Func<bool> GetShopkeeperExistFunc;
    System.Func<bool> GetShopkeeperIsMovingFunc;

    System.Action<string> callLuaGlobalHeartbeatEvent;
    System.Action<string> OnNetworkSuccessFunc;
    System.Action<System.Object, int> OnNetworkFailedFunc;

    public delegate void LuaBridgeEvent(string eventtype, params object[] param);

    System.Action<string> luaEventFunctionCall;

    System.Action<string, System.Object> luaEventFunctionCallP1;

    System.Action<string, System.Object, System.Object> luaEventFunctionCallP2;

    System.Action<string, System.Object, System.Object, System.Object> luaEventFunctionCallP3;

    System.Action<string, System.Object, System.Object, System.Object, System.Object> luaEventFunctionCallP4;
    LuaTable hotfixScriptEnv;
    public void _init()
    {
        var luaStr = VersionManager.inst.GetLuaText("HotfixBridge");
        var chunk = "HotfixBridge";
        hotfixScriptEnv = XLuaManager.inst.getScriptEnv(chunk, this);
        hotfixScriptEnv.Set("self", this);
        XLuaManager.inst.DoString(luaStr, chunk, hotfixScriptEnv);
        //scriptEnv.Get("test", out test);
        //scriptEnv.Get("testAction", out testAction);
        hotfixScriptEnv.Get("OnChangeState", out OnChangeState);
        hotfixScriptEnv.Get("OnGetWindow", out GetWindowFunc);
        hotfixScriptEnv.Get("OpenView", out OpenViewFunc);
        hotfixScriptEnv.Get("HideView", out HideViewFunc);
        hotfixScriptEnv.Get("ShowView", out ShowViewFunc);
        hotfixScriptEnv.Get("closeView", out CloseViewFunc);
        hotfixScriptEnv.Get("GetWindowByViewId", out GetWindowByViewIdFunc);
        hotfixScriptEnv.Get("Showing", out ViewShowingFunc);
        hotfixScriptEnv.Get("CurrentWindow", out CurrWindowFunc);
        hotfixScriptEnv.Get("allShowingView", out AllShowingViewFunc);
        hotfixScriptEnv.Get("HasDirectPurchaseDataFunc", out HasDirectPurchaseDataFunc);
        hotfixScriptEnv.Get("GetDirectPurchaseDataFunc", out GetDirectPurchaseDataFunc);
        hotfixScriptEnv.Get("HasBuyLevelGrowthFunc", out HasBuyLevelGrowthFunc);
        hotfixScriptEnv.Get("GetRefugeDataFunc", out GetRefugeDataFunc);
        hotfixScriptEnv.Get("GetVipRemainTimeFunc", out GetVipRemainTimeFunc);
        hotfixScriptEnv.Get("GetTriggerIsTrig", out GetTriggerIsTrigFunc);
        hotfixScriptEnv.Get("GetActivity_WorkerGameFlag", out GetActivity_WorkerGameFlagFunc);
        hotfixScriptEnv.Get("GetActivity_WorkerGameEquipCanAddRateByDrawingId", out GetActivity_WorkerGameEquipCanAddRateByDrawingIdFunc);
        hotfixScriptEnv.Get("GetActivity_WorkerGameEquipMakeIntegralByDrawingId", out GetActivity_WorkerGameEquipMakeIntegralByDrawingIdFunc);
        hotfixScriptEnv.Get("GetActivity_WorkerGameCoinCount", out GetActivity_WorkerGameCoinCountFunc);
        hotfixScriptEnv.Get("GetActivity_WorkerGameString", out GetActivity_WorkerGameStringFunc);
        hotfixScriptEnv.Get("GetActivity_GoldenCityFlag", out GetActivity_GoldenCityFlagFunc);
        hotfixScriptEnv.Get("GetActivity_GoldenCityCanRewardCount", out GetActivity_GoldenCityCanRewardCountFunc);
        hotfixScriptEnv.Get("GetActivity_GoldenCityCurScoreLv", out GetActivity_GoldenCityCurScoreLvFunc);
        hotfixScriptEnv.Get("HaveTimeLimitActivitySelfScore", out HaveTimeLimitActivitySelfScoreFunc);
        hotfixScriptEnv.Get("GetLuxuryBuff", out GetLuxuryBuffFunc);
        hotfixScriptEnv.Get("GetCurTriggerData", out GetTriggerDataFunc);
        hotfixScriptEnv.Get("GetRuinsBattleData", out GetRuinsBattleFunc);
        hotfixScriptEnv.Get("OnNetworkSuccess", out OnNetworkSuccessFunc);
        hotfixScriptEnv.Get("GlobalHeartbeatEvent", out callLuaGlobalHeartbeatEvent);
        hotfixScriptEnv.Get("OnNetworkFailed", out OnNetworkFailedFunc);
        hotfixScriptEnv.Get("CurrentWindowViewID", out CurrWindowViewID);
        hotfixScriptEnv.Get("onBackToMainUI", out BackToMainUI);
        hotfixScriptEnv.Get("onBackAndChangeMainView", out BackAndChangeMainUI);
        hotfixScriptEnv.Get("GetViewIsShowingByViewID", out GetViewIsShowingByViewIDFunc);
        hotfixScriptEnv.Get("GetShopkeeperCanMoveToCounter", out GetShopkeeperCanMoveToCounterFunc);
        hotfixScriptEnv.Get("GetShopkeeperExist", out GetShopkeeperExistFunc);
        hotfixScriptEnv.Get("GetShopkeeperIsMoving", out GetShopkeeperIsMovingFunc);
        hotfixScriptEnv.Get("onClearAllView", out ClearAllView);
        hotfixScriptEnv.Get("Bridge_onBridgeEvent", out luaEventFunctionCall);
        hotfixScriptEnv.Get("Bridge_onBridgeEventP1", out luaEventFunctionCallP1);
        hotfixScriptEnv.Get("Bridge_onBridgeEventP2", out luaEventFunctionCallP2);
        hotfixScriptEnv.Get("Bridge_onBridgeEventP3", out luaEventFunctionCallP3);
        hotfixScriptEnv.Get("Bridge_onBridgeEventP4", out luaEventFunctionCallP4);
    }

    public override void Release()
    {
        OnChangeState = null;
        GetWindowFunc = null;
        HideViewFunc = null;
        OpenViewFunc = null;
        ShowViewFunc = null;
        CloseViewFunc = null;
        GetWindowByViewIdFunc = null;
        ViewShowingFunc = null;
        CurrWindowFunc = null;
        CurrWindowViewID = null;
        AllShowingViewFunc = null;
        HasDirectPurchaseDataFunc = null;
        GetDirectPurchaseDataFunc = null;
        HasBuyLevelGrowthFunc = null;
        GetRefugeDataFunc = null;
        GetVipRemainTimeFunc = null;
        OnNetworkSuccessFunc = null;
        callLuaGlobalHeartbeatEvent = null;
        GetTriggerIsTrigFunc = null;
        GetTriggerDataFunc = null;
        GetActivity_GoldenCityFlagFunc = null;
        GetActivity_GoldenCityCanRewardCountFunc = null;
        GetActivity_GoldenCityCurScoreLvFunc = null;
        HaveTimeLimitActivitySelfScoreFunc = null;
        GetLuxuryBuffFunc = null;
        GetRuinsBattleFunc = null;
        ClearAllView = null;
        OnNetworkFailedFunc = null;
        BackToMainUI = null;
        BackAndChangeMainUI = null;
        GetViewIsShowingByViewIDFunc = null;
        luaEventFunctionCall = null;
        luaEventFunctionCallP1 = null;
        luaEventFunctionCallP2 = null;
        luaEventFunctionCallP3 = null;
        luaEventFunctionCallP4 = null;

        hotfixScriptEnv.Dispose();
        base.Release();
    }

    public void TriggerLuaEvent(string eventtype, params object[] param)
    {
        if (luaEventFunctionCall == null) return;
        switch (param.Length)
        {
            case 0:
                luaEventFunctionCall(eventtype);
                break;
            case 1:
                luaEventFunctionCallP1(eventtype, param[0]);
                break;
            case 2:
                luaEventFunctionCallP2(eventtype, param[0], param[1]);
                break;
            case 3:
                luaEventFunctionCallP3(eventtype, param[0], param[1], param[2]);
                break;
            case 4:
                luaEventFunctionCallP4(eventtype, param[0], param[1], param[2], param[3]);
                break;
        }
    }
    public void ChangeState(IStateTransition transition)
    {
        Logger.log("HotfixBridge ChangeState called");
        if (OnChangeState != null)
            OnChangeState(transition);
    }

    public uiWindow GetWindow(System.Type csType, bool needNew)
    {

        return GetWindowFunc == null ? null : GetWindowFunc(csType, needNew);
    }

    public uiWindow GetWindowByViewId(System.String viewid)
    {
        return GetWindowByViewIdFunc == null ? null : GetWindowByViewIdFunc(viewid);
    }

    //
    public void ShowView(System.Object obj, System.Action<System.Object> callback)
    {
        if (ShowViewFunc != null)
            ShowViewFunc(obj, callback);
    }
    public uiWindow OpenView(System.Type csType, System.Action<System.Object> callback)
    {
        if (callback == null)
        {
            Logger.log("GUI 打开没有回调  " + csType.ToString());
        }
        return OpenViewFunc == null ? null : OpenViewFunc(csType, callback);
    }

    public void HideView(System.Type csType)
    {
        if (HideViewFunc != null)
            HideViewFunc(csType);
    }
    public void CloseView(System.Type viewid)
    {
        if (CloseViewFunc != null)
            CloseViewFunc(viewid);
    }
    public List<uiWindow> AllShowingView()
    {
        return AllShowingViewFunc == null ? null : AllShowingViewFunc();
    }
    public uiWindow GetCurrWindow()
    {

        return CurrWindowFunc == null ? null : CurrWindowFunc();
    }
    public void CallBackMainView()
    {
        if (BackToMainUI != null)
            BackToMainUI();
    }

    //只允许主场景和城市场景中调用
    public void CallBackAndChangeMainView()
    {
        if (BackAndChangeMainUI != null)
            BackAndChangeMainUI();
    }
    public void CallClearAllView()
    {
        if (BackToMainUI != null)
            ClearAllView();
    }
    public string GetCurrWindowViewID()
    {
        return CurrWindowViewID == null ? "" : CurrWindowViewID();
    }

    public bool GetViewIsShowingByViewID(string viewID)
    {
        return GetViewIsShowingByViewIDFunc == null ? false : GetViewIsShowingByViewIDFunc(viewID);
    }

    public bool Showing(System.Type csType)
    {
        return ViewShowingFunc == null ? false : ViewShowingFunc(csType);
    }


    #region 礼包相关
    public bool GetDirectPurchaseDataById(int id, out DirectPurchaseData data)  //根据id获取直购礼包信息
    {

        if (HasDirectPurchaseDataFunc(id))
        {
            data = GetDirectPurchaseDataFunc(id);
            return true;
        }
        else
        {
            data = null;
            return false;
        }

    }

    public bool HasBuyLevelGrowth() 
    {

        if (HasBuyLevelGrowthFunc != null)
        {
            return HasBuyLevelGrowthFunc();
        }

        return false;
    }

    #endregion

    #region 特权卡相关
    public int GetVipRemainTime()
    {
        return GetVipRemainTimeFunc();
    }
    #endregion

    #region 触发引导相关

    public bool GetTriggerIsTrig(int furnId)
    {
        return GetTriggerIsTrigFunc(furnId);
    }

    public TriggerData GetTriggerData()
    {
        return GetTriggerDataFunc();
    }
    #endregion

    #region 巧匠大赛相关
    public bool GetActivity_WorkerGameFlag() 
    {
        if (GetActivity_WorkerGameFlagFunc != null)
        {
            return GetActivity_WorkerGameFlagFunc();
        }

        return false;
    }

    public bool GetActivity_WorkerGameEquipCanAddRateByDrawingId(int equipDrawingId) 
    {
        if (GetActivity_WorkerGameEquipCanAddRateByDrawingIdFunc != null)
        {
            return GetActivity_WorkerGameEquipCanAddRateByDrawingIdFunc(equipDrawingId);
        }

        return false;
    }

    public int GetActivity_WorkerGameEquipMakeIntegralByDrawingId(int equipDrawingId) 
    {
        if (GetActivity_WorkerGameEquipMakeIntegralByDrawingIdFunc != null)
        {
            return GetActivity_WorkerGameEquipMakeIntegralByDrawingIdFunc(equipDrawingId);
        }

        return 0;
    }

    public long GetActivity_WorkerGameCoinCount()
    {
        if (GetActivity_WorkerGameCoinCountFunc != null)
        {
            return GetActivity_WorkerGameCoinCountFunc();
        }

        return 0;
    }

    public string GetActivity_WorkerGameStr(EOperatingActivityStringType strType)
    {
        if (GetActivity_WorkerGameStringFunc != null)
        {
            return GetActivity_WorkerGameStringFunc((int)strType);
        }

        return string.Empty;
    }

    #endregion

    #region 黄金城
    public bool GetActivity_GoldenCityFlag()
    {
        if (GetActivity_GoldenCityFlagFunc != null)
        {
            return GetActivity_GoldenCityFlagFunc();
        }

        return false;
    }

    public int GetActivity_GoldenCityCanRewardCount()
    {
        if (GetActivity_GoldenCityCanRewardCountFunc != null)
        {
            return GetActivity_GoldenCityCanRewardCountFunc();
        }

        return 0;
    }

    public int GetActivity_GoldenCityCurScoreLv()
    {
        if (GetActivity_GoldenCityCurScoreLvFunc != null)
        {
            return GetActivity_GoldenCityCurScoreLvFunc();
        }

        return 0;
    }

    #endregion

    #region 限时活动相关

    public bool HaveTimeLimitActivitySelfScore() 
    {
        if (HaveTimeLimitActivitySelfScoreFunc != null)
        {
            return HaveTimeLimitActivitySelfScoreFunc();
        }

        return false;
    }

    #endregion

    #region 豪华度buff
    public int GetLuxuryBuff(int subType)
    {
        if (GetLuxuryBuffFunc != null)
        {
            return GetLuxuryBuffFunc(subType);
        }

        return -1;
    }
    #endregion

    #region 获取活动副本等级数据
    public DirectPurchaseData GetRefugeData()
    {
        if (GetRefugeDataFunc != null)
        {
            return GetRefugeDataFunc();
        }

        return null;
    }
    #endregion

    #region 店主相关
    public bool GetShopkeeperCanMoveToCounter()
    {
        if (GetShopkeeperCanMoveToCounterFunc != null)
        {
            return GetShopkeeperCanMoveToCounterFunc();
        }

        return false;
    }

    public bool GetShopkeeperExist()
    {
        if (GetShopkeeperExistFunc != null)
        {
            return GetShopkeeperExistFunc();
        }

        return false;
    }

    public bool GetShopkeeperIsMoving()
    {
        if (GetShopkeeperIsMovingFunc != null)
        {
            return GetShopkeeperIsMovingFunc();
        }

        return false;
    }
    #endregion

    #region 废墟相关
    public TriggerData GetRuinsBattleData()
    {
        if (GetRuinsBattleFunc != null)
        {
            return GetRuinsBattleFunc();
        }

        return null;
    }
    #endregion
    //
    public void OnMessageSuccess(string msg)
    {
        if (OnNetworkSuccessFunc != null)
            OnNetworkSuccessFunc(msg);
    }

    public void OnMessageFailed(System.Object obj, int code)
    {
        if (OnNetworkFailedFunc != null)
            OnNetworkFailedFunc(obj, code);
    }

    public void OncallLuaGlobalHeartbeatEvent(string msg)
    {
        if (callLuaGlobalHeartbeatEvent != null)
            callLuaGlobalHeartbeatEvent(msg);
    }
}
