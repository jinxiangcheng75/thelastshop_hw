using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

[LuaCallCSharp]

public class EventController : TSingletonHotfix<EventController>
{
    public GlobalEventHandler globalEventHandler;
    public Dictionary<string, Delegate> TheRouter;
    public EventController()
    {
        TheRouter = new Dictionary<string, Delegate>();
        globalEventHandler = new GlobalEventHandler();
        //

    }

    private Delegate GetEventDelegate(string eventType)
    {
        if (ContainsEvent(eventType))
        {
            return TheRouter[eventType];
        }
        else
            return null;
    }

    public void Cleanup()
    {
        TheRouter.Clear();
    }

    public bool ContainsEvent(string eventType)
    {
        return TheRouter.ContainsKey(eventType);
    }
    private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            TheRouter[eventType] = Delegate.Combine(TheRouter[eventType], listenerBeingAdded);
        }
        else
        {
            TheRouter.Add(eventType, listenerBeingAdded);
        }
    }

    //public void AddListener(string eventType, Action<object[]> handler)
    //{
    //    OnListenerAdding(eventType, handler);
    //}

    public void AddListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
    {
        OnListenerAdding(eventType, handler);
    }

    public void AddListener<T, U, V>(string eventType, Action<T, U, V> handler)
    {
        OnListenerAdding(eventType, handler);
    }

    public void AddListener<T, U>(string eventType, Action<T, U> handler)
    {
        OnListenerAdding(eventType, handler);
    }

    public void AddListener<T>(string eventType, Action<T> handler)
    {
        OnListenerAdding(eventType, handler);
    }
    public void AddListener(string eventType, Action handler)
    {
        OnListenerAdding(eventType, handler);
    }

    //public void RemoveListener(string eventType, Action<object[]> handler)
    //{
    //    if (TheRouter.ContainsKey(eventType))
    //    {
    //        TheRouter[eventType] = Delegate.Remove(TheRouter[eventType], handler);
    //        if (TheRouter[eventType] == null)
    //        {
    //            TheRouter.Remove(eventType);
    //        }
    //    }
    //}

    public void RemoveListener<T>(string eventType, Action<T> handler)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            TheRouter[eventType] = Delegate.Remove(TheRouter[eventType], handler);
            if (TheRouter[eventType] == null)
            {
                TheRouter.Remove(eventType);
            }
        }
    }

    public void RemoveListener<T, U>(string eventType, Action<T, U> handler)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            TheRouter[eventType] = Delegate.Remove(TheRouter[eventType], handler);
            if (TheRouter[eventType] == null)
            {
                TheRouter.Remove(eventType);
            }
        }
    }

    public void RemoveListener<T, U, V>(string eventType, Action<T, U, V> handler)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            TheRouter[eventType] = Delegate.Remove(TheRouter[eventType], handler);
            if (TheRouter[eventType] == null)
            {
                TheRouter.Remove(eventType);
            }
        }
    }

    public void RemoveListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            TheRouter[eventType] = Delegate.Remove(TheRouter[eventType], handler);
            if (TheRouter[eventType] == null)
            {
                TheRouter.Remove(eventType);
            }
        }
    }

    public void RemoveListener(string eventType, Action handler)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            TheRouter[eventType] = Delegate.Remove(TheRouter[eventType], handler);
            if (TheRouter[eventType] == null)
            {
                TheRouter.Remove(eventType);
            }
        }
    }

    #region TriggerEvent
    /// <summary>
    /// 发送事件
    /// </summary>

    /// <param name="eventType">事件类型  字符串</param>
    //public void TriggerEvent(string eventType, params object[] paramList)
    //{
    //    if (TheRouter.ContainsKey(eventType))
    //    {
    //        if(TheRouter[eventType] == null)
    //        {
    //            return;
    //        }
    //        Delegate[] eventlist = TheRouter[eventType].GetInvocationList();
    //        for (int i = 0; i < eventlist.Length; i++)
    //        {
    //            Action<object[]> handler = eventlist[i] as Action<object[]>;
    //            handler(paramList);
    //        }
    //    }
    //}

    public void TriggerEvent(string eventType)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            if (TheRouter[eventType] == null)
            {
                return;
            }
            Delegate[] eventlist = TheRouter[eventType].GetInvocationList();
            for (int i = 0; i < eventlist.Length; i++)
            {
                Action handler = eventlist[i] as Action;
                handler();
            }
        }
    }
    public void TriggerEvent<T>(string eventType, T arg1)
    {
        if (TheRouter.ContainsKey(eventType))
        {

            if (TheRouter[eventType] == null)
            {
                return;
            }
            Delegate[] eventlist = TheRouter[eventType].GetInvocationList();
            for (int i = 0; i < eventlist.Length; i++)
            {
                Action<T> handler = eventlist[i] as Action<T>;
                handler(arg1);
            }
        }
    }

    public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            if (TheRouter[eventType] == null)
            {
                return;
            }
            Delegate[] eventlist = TheRouter[eventType].GetInvocationList();
            for (int i = 0; i < eventlist.Length; i++)
            {
                Action<T, U> handler = eventlist[i] as Action<T, U>;
                handler(arg1, arg2);
            }
        }
    }

    public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            if (TheRouter[eventType] == null)
            {
                return;
            }
            Delegate[] eventlist = TheRouter[eventType].GetInvocationList();
            for (int i = 0; i < eventlist.Length; i++)
            {
                Action<T, U, V> handler = eventlist[i] as Action<T, U, V>;
                if (handler != null)
                    handler(arg1, arg2, arg3);
            }
        }
    }

    public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
    {
        if (TheRouter.ContainsKey(eventType))
        {
            if (TheRouter[eventType] == null)
            {
                return;
            }
            Delegate[] eventlist = TheRouter[eventType].GetInvocationList();
            for (int i = 0; i < eventlist.Length; i++)
            {
                Action<T, U, V, W> handler = eventlist[i] as Action<T, U, V, W>;
                handler(arg1, arg2, arg3, arg4);
            }
        }
    }


    //lua call C#
    public void TriggerEvent_Lua0(string eventType)
    {
        TriggerEvent(eventType);
    }

    [LuaCallCSharp]
    public void TriggerEvent_Lua1<T>(string eventType, T arg1)
    {
        TriggerEvent(eventType, arg1);
    }
    [LuaCallCSharp]
    public void TriggerEvent_Lua2<T, U>(string eventType, T arg1, U arg2)
    {
        TriggerEvent(eventType, arg1, arg2);
    }
    [LuaCallCSharp]
    public void TriggerEvent_Lua3<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
        TriggerEvent(eventType, arg1, arg2, arg3);
    }

    public void TriggerEvent_Lua4<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
    {
        TriggerEvent(eventType, arg1, arg2, arg3, arg4);
    }

    //-------------------------------------------------------------------------------------------------

    #endregion
}

public static class GameEventType
{
    public static readonly string GameEvent_Test = "GameEvent_Test";  //测试事件

    public static readonly string NETWORK_RELINK = "NETWORK_RELINK";

    public static readonly string GAME_RESTART = "GAME_RESTART"; //重新运行游戏
    public static readonly string UpdateGameRedPoints = "UpdateGameRedPoints"; // 刷新红点
    public static readonly string Activity_WorkerGameCoin_Fly = "Activity_WorkerGameCoin_Fly";
    public static readonly string UnionCoin_FLY = "UnionCoin_FLY";
    public static readonly string ENERGY_FLY = "ENERGY_FLY";
    public static readonly string GOLD_FLY = "GOLD_FLY";
    public static readonly string GEM_FLY = "GEM_FLY";
    public static readonly string UI_TopRes_Update = "UI_TopRes_Update";

    public static readonly string UI_TOPTESPANEL_ShiftOut = "UI_TOPTESPANEL_ShiftOut";
    public static readonly string MSG_SERVER_TEST = "MSG_SERVER_TEST";

    public static readonly string ChangeState = "ChangeState";

    public static readonly string NETWORK_ERROR = "NETWORK_ERROR";
    public static readonly string NETWORK_Success = "NETWORK_Success";
    //UI
    public static readonly string SHOWUI_LOGIN = "SHOWUI_LOGIN";
    public static readonly string HIDEUI_LOGIN = "HIDEUI_LOGIN";

    public static readonly string SHOWUI_SHOPSCENE = "SHOWUI_SHOPSCENE";
    public static readonly string HIDEUI_SHOPSCENE = "HIDEUI_SHOPSCENE";

    public static readonly string MAINUI_SHIFTOUT = "MAINUI_SHIFTOUT";
    public static readonly string MAINUI_SHIFTIN = "MAINUI_SHIFTIN";

    public static readonly string SHOWUI_SHOWTOPINFOUI = "SHOWUI_SHOWTOPINFOUI";
    public static readonly string REFRESHMAINUIREDPOINT = "REFRESHMAINUIREDPOINT";

    public static readonly string REFRESHDAILYSIGN = "REFRESHDAILYSIGN";

    // subtop
    public static readonly string SHOWUI_SUBTOP = "SHOWUI_SUBTOP";

    public static readonly string SHOWUI_SHOPKEEPERPANEL = "SHOWUI_SHOPKEEPERPANEL";
    public static readonly string HIDEUI_SHOPKEEPERPANEL = "HIDEUI_SHOPKEEPERPANEL";
    public static readonly string SHOWUI_SHOPKEEPERSUBPANEL = "SHOWUI_SHOPKEEPERSUBPANEL";
    public static readonly string JUDGESHOPKEEPERDRESS = "JUDGESHOPKEEPERDRESS";
    public static readonly string SHOWUI_SINGLEBUY = "SHOWUI_SINGLEBUY";
    public static readonly string HIDEUI_SINGLEBUY = "HIDEUI_SINGLEBUY";
    public static readonly string SHOWUI_PROMPT = "SHOWUI_PROMPT";
    public static readonly string HIDEUI_PROMPT = "HIDEUI_PROMPT";

    public static readonly string SHOWUI_MSGBOX = "SHOWUI_MSGBOX";
    public static readonly string SHOWUI_OK_MSGBOX = "SHOWUI_OK_MSGBOX";
    public static readonly string SHOWUI_OKCANCLE_MSGBOX = "SHOWUI_OKCANCLE_MSGBOX";

    public static readonly string SHOWUI_TEXTMSGTIP = "SHOWUI_TEXTMSGTIP";

    public static readonly string SHOWUI_LOADINGUI = "SHOWUI_LOADINGUI";
    public static readonly string HIDEUI_LOADINGUI = "HIDEUI_LOADINGUI";

    public static readonly string SHOWUI_BAGUI = "SHOWUI_BAGUI";
    public static readonly string HIDEUI_BAGUI = "HIDEUI_BAGUI";

    public static readonly string SHOWUI_PLAYERUPUI = "SHOWUI_PLAYERUPUI";
    public static readonly string HIDEUI_PLAYERUPUI = "HIDEUI_PLAYERUPUI";
    public static readonly string SHOWUI_MSGBOXPLAYERUPITEMUI = "SHOWUI_MSGBOXPLAYERUPITEMUI";

    public static readonly string USERDATA_EXPCHANGE = "USERDATA_EXPCHANGE";
    #region Setting
    public static readonly string SHOWUI_SETTINGPANEL = "SHOWUI_SETTINGPANEL ";
    public static readonly string SHOWUI_SETTINGBINDING = "SHOWUI_SETTINGBINDING ";
    #endregion

    #region Task
    public static readonly string SHOWUI_TASKPANEL = "SHOWUI_TASKPANEL ";
    public static readonly string HIDEUI_TASKPANEL = "HIDEUI_TASKPANEL";
    public static readonly string SHOWUI_TASKITEM = "SHOWUI_TASKITEM";
    public static readonly string HIDEUI_TASKITEM = "HIDEUI_TASKITEM";
    public static readonly string SHOWUI_EXTENSIONPANEL = "SHOWUI_EXTENSIONPANEL";
    public static readonly string HIDEUI_EXTENSIONPANEL = "HIDEUI_EXTENSIONPANEL";
    public static readonly string SHOWUI_MSGBOXCOM_TASK = "SHOWUI_MSGBOXCOM_TASK";
    public static readonly string HIDEUI_MSGBOXCOM_TASK = "HIDEUI_MSGBOXCOM_TASK";
    #endregion

    #region ShopUpgrade
    public static readonly string SHOWUI_UPGRADEPANEL = "SHOWUI_UPGRADEPANEL ";
    public static readonly string HIDEUI_UPGRADEPANEL = "HIDEUI_UPGRADEPANEL";
    public static readonly string SHOWUI_CONTENTPANEL = "SHOWUI_CONTENTPANEL ";
    public static readonly string SHOWUI_EXTENDINGPANEL = "SHOWUI_EXTENDINGPANEL ";
    public static readonly string SHOWUI_EXTENSIONFINISHPANEL = "SHOWUI_EXTENSIONFINISHPANEL ";
    public static readonly string HIDEUI_CONTENTPANEL = "HIDEUI_CONTENTPANEL";
    public static readonly string RefreshUI_Furniture_ShelfContent = "RefreshUI_Furniture_ShelfContent";
    #endregion

    #region ShelfUpgrade
    public static readonly string SHOWUI_SHELFUPGRADINGUI = "SHOWUI_SHELFUPGRADINGUI ";
    public static readonly string SHOWUI_SHELFUPGRADEFINISHUI = "SHOWUI_SHELFUPGRADEFINISHUI ";
    #endregion
    public static readonly string SHOWUI_CREATROLEPANEL = "SHOWUI_CREATROLEPANEL";
    public static readonly string HIDEUI_CREATROLEPANEL = "HIDEUI_CREATROLEPANEL";
    public static readonly string SHOWUI_EQUIPLIST = "SHOWUI_EQUIPLIST";
    public static readonly string HIDEUI_EQUIPLIST = "HIDEUI_EQUIPLIST";
    public static readonly string SHOWUI_BuyMakingSlot = "SHOWUI_BuyMakingSlot";
    public static readonly string HIDEUI_BuyMakingSlot = "HIDEUI_BuyMakingSlot";

    public static readonly string SHOWUI_SELFROLEINFO = "SHOWUI_SELFROLEINFO";
    public static readonly string REFRESH_SELFROLEINFO = "REFRESH_SELFROLEINFO";
    public static readonly string SHOWUI_ROLEINFO = "SHOWUI_ROLEINFO";
    public static readonly string HIDEUI_ROLEINFO = "HIDEUI_ROLEINFO";
    public static readonly string SETSPBTNSTATE = "SETSPBTNSTATE";

    public static readonly string REQUESTUSERCUSTOM = "REQUESTUSERCUSTOM";

    public static readonly string SHOWUI_CHANGENAME = "SHOWUI_CHANGENAME";
    public static readonly string HIDEUI_CHANGENAME = "HIDEUI_CHANGENAME";

    public static readonly string SHOWUI_EQUIPITEMUI = "SHOWUI_EQUIPITEMUI";
    public static readonly string HIDEUI_EQUIPITEMUI = "HIDEUI_EQUIPITEMUI";
    public static readonly string SHOWUI_EQUIPRESOLVEUI = "SHOWUI_EQUIPRESOLVEUI";
    public static readonly string HIDEUI_EQUIPRESOLVEUI = "HIDEUI_EQUIPRESOLVEUI";


    public static readonly string SHOWUI_EQUIPINFOUI = "SHOWUI_EQUIPINFOUI";
    public static readonly string SHOWUI_EQUIPINFOUIBYDRAWINGID = "SHOWUI_EQUIPINFOUIBYDRAWINGID";
    public static readonly string HIDEUI_EQUIPINFOUI = "HIDEUI_EQUIPINFOUI";
    public static readonly string RefreshUI_EquipInfoUIStarUp = "RefreshUI_EquipInfoUIStarUp";

    //shop design ui
    // public static readonly string SHOWUI_SHOPDESIGNUI = "SHOWUI_SHOPDESIGNUI";
    // public static readonly string HIDEUI_SHOPDESIGNUI = "HIDEUI_SHOPDESIGNUI";
    public static readonly string SHOWUI_CUSTOMIZEUI = "SHOWUI_CUSTOMIZEUI";
    public static readonly string HIDEUI_CUSTOMIZEUI = "HIDEUI_CUSTOMIZEUI";
    public static readonly string SHOWUI_FURNITUREUI = "SHOWUI_FURNITUREUI";
    public static readonly string HIDEUI_FURNITUREUI = "HIDEUI_FURNITUREUI";
    public static readonly string SHOWUI_TARGETFURN = "SHOWUI_TARGETFURN";
    public static readonly string SHOWUI_SHELFCONTENTUI = "SHOWUI_SHELFCONTENTUI";
    public static readonly string HIDEUI_SHELFCONTENTUI = "HIDEUI_SHELFCONTENTUI";
    public static readonly string SHOWUI_PETUI = "SHOWUI_PETUI";
    public static readonly string HIDEUI_PETUI = "HIDEUI_PETUI";
    public static readonly string SHOWUI_SKINUI = "SHOWUI_SKINUI";
    public static readonly string HIDEUI_SKINUI = "HIDEUI_SKINUI";

    public static readonly string SHOWUI_UNLOCKDRAWINGUI = "SHOWUI_UNLOCKDRAWINGUI";

    public static readonly string SHOWUI_UNLOCKDRAWINGBYWORKERUI = "SHOWUI_UNLOCKDRAWINGBYWORKERUI";

    public static readonly string UseAdvancedEquip = "UseAdvancedEquip";

    public static readonly string TOUCHEVENT_OnPointBlank = "TOUCHEVENT_OnPointBlank";
    public static readonly string TOUCHEVENT_OnPointClick = "TOUCHEVENT_OnPointClick";

    public static readonly string SHOW_FurnitureNumLimit = "SHOW_FurnitureNumLimit";
    public static class AccountEvent
    {
        public static readonly string UI_LoginServer = "UI_LoginServer";

        public static readonly string REQUEST_USERDATA = "REQUEST_USERDATA";
    }

    public static class NoticeEvent
    {
        public static readonly string REQUEST_GATE = "REQUEST_GATE";
        public static readonly string SHOWUI_NOTICE = "SHOWUI_NOTICE";
    }

    public static class BagEvent
    {
        public static readonly string BAG_GET_DATA = "BAG_GET_DATA";

        public static readonly string BAG_DATA_UPDATE = "BAG_DATA_UPDATE";      //背包资源更新完成
        public static readonly string BAG_RES_UPDATE = "BAG_RES_UPDATE";        //资源数量改变
        public static readonly string BAG_EQUIP_UPDATE = "BAG_EQUIP_UPDATE";    //背包装备改变
        public static readonly string BAG_SHOW_ITEMINFO = "BAG_SHOW_ITEMINFO";
        public static readonly string Bag_inventory_numChg = "Bag_inventory_NumChg";//背包容量变化

        public static readonly string Bag_BuyProduction = "Bag_BuyProduction"; //背包资源补充
        public static readonly string Bag_BuyProductionByUnoinCoin = "Bag_BuyProductionByUnoinCoin"; //背包资源补充--公会币
        public static readonly string ShopDesign_Resource_BuyProduction = "ShopDesign_Resource_BuyProduction"; //购买资源

    }

    public static class EquipEvent
    {
        public static readonly string EQUIP_UPDATEINFO = "EQUIP_UPDATEINFO";

        //v2c
        public static readonly string EQUIP_PRODUCTION_SELECT = "EQUIP_PRODUCTION_SELECT"; //装备制造_选择装备
        //
        public static readonly string EQUIP_REMOVE = "EQUIP_REMOVE";
        public static readonly string EQUIP_LOCK = "EQUIP_LOCK";
        public static readonly string EQUIP_UNLOCKEQUIP = "EQUIP_UNLOCKEQUIP";

        public static readonly string EQUIP_STARUP = "EQUIP_STARUP";
        public static readonly string EQUIP_STARUPSUCCESS = "EQUIP_STARUPSUCCESS";


        //EquipFavorite
        public static readonly string EQUIP_FAVORITE = "EQUIP_FAVORITE";

        public static readonly string EQUIP_Required = "EQUIP_Required";

        public static readonly string EQUIP_SHOWMAKELIST = "EQUIP_SHOWMAKELIST";

        public static readonly string EQUIP_SHOWTARGETTYPE = "EQUIP_SHOWTARGETTYPE";

        public static readonly string EQUIP_SHOWREFRESH = "EQUIP_SHOWREFRESH";

        public static readonly string SET_EQUIPFLY = "SET_EQUIPFLY";

        public static readonly string SET_GUIDETARGET = "SET_GUIDETARGET";
    }

    //任务
    public static class TaskEvent
    {
        public static readonly string TASK_GET_DATALIST = "TASK_GET_DATALIST";
        public static readonly string TASK_REPLACE_DATALIST = "TASK_REPLACE_DATALIST";
        public static readonly string TASK_REWARD_DATALIST = "TASK_REWARD_DATALIST";
        public static readonly string TASK_CONTENTCHANGE = "TASK_CONTENTCHANGE";
        public static readonly string TASK_RESHOWTASKPANEL = "TASK_RESHOWTASKPANEL";
        public static readonly string TASK_RESHOWTASKPANELANIM = "TASK_RESHOWTASKPANELANIM";
        public static readonly string TASK_REFRESHUNIONTASKMESS = "TASK_REFRESHUNIONTASKMESS";
        public static readonly string TASK_REFRESHMSGBOX = "TASK_REFRESHMSGBOX";
        public static readonly string TASK_COLLTIMEDOWN = "TASK_COLLTIMEDOWN";

        public static readonly string TASK_SHOW_LIVENESSBOXDESPANEL = "TASK_SHOW_LIVENESSBOXDESPANEL";
        public static readonly string TASK_GETLIVENESSBOXAWARD = "TASK_GETLIVENESSBOXAWARD";

        public static readonly string SHOWTIP_UNIONTASK = "SHOWTIP_UNIONTASK";
        public static readonly string SHOWTIP_DAILYTASK = "SHOWTIP_DAILYTASK";
    }

    public static class ExtensionEvent
    {
        public static readonly string EXTENSION_GET_SEQUENCE = "EXTENSION_GET_SEQUENCE";
        public static readonly string EXTENSION_POST_COINEXTENSION = "EXTENSION_POST_COINEXTENSION";
        public static readonly string EXTENSION_POST_DIAMEXTENSION = "EXTENSION_POST_DIAMEXTENSION";
        public static readonly string EXTENSION_POST_FINISHEXTENSION = "EXTENSION_POST_FINISHEXTENSION";
        public static readonly string EXTENSION_CALLBACKS_SHOPUPGRADE = "EXTENSION_CALLBACKS_SHOPUPGRADE";
        public static readonly string EXTENSION_CALLTAO_SHOPUPGRADE = "EXTENSION_CALLTAO_SHOPUPGRADE";
        public static readonly string EXTENSION_SHOWMSGBOX = "EXTENSION_SHOWMSGBOX";
        public static readonly string EXTENSION_SHOPREFRESH = "EXTENSION_SHOPREFRESH"; //扩建数据刷新
    }

    public static class FurnitureUpgradeEvent
    {
        public static readonly string SHOPUPGRADE_SHOW_INVENTORYUI = "SHOPUPGRADE_SHOW_INVENTORYUI ";
        public static readonly string SHOPUPGRADE_HIDE_INVENTORYUI = "SHOPUPGRADE_HIDE_INVENTORYUI ";

        //升级货架的时候其他货架正在升级时候的消息框
        public static readonly string SHOWMSGBOX = "SHOWMSGBOX ";
        //升级家具的事件
        public static readonly string SHOPUPGRADE_UPGRADEFRUITURE = "SHOPUPGRADE_UPGRADEFRUITURE ";
        //升级已在升级的家具的时间
        public static readonly string SHOPUPGRADE_UPGRADEFRUITURE_Immediately = "SHOPUPGRADE_UPGRADEFRUITURE_Immediately";
        //钻石升级家具存储数据
        public static readonly string SHOPUPGRADE_SAVEDATA = "SHOPUPGRADE_SAVEDATA";
    }

    public static class FurnitureDisplayEvent
    {
        public static readonly string ShelfDisplay_ReBind = "ShelfDisplay_ReBind";  //重新绑定
                                                                                    //把装备放上去
        public static readonly string SHELFUPGRADE_PUTONEQUIP = "SHELFUPGRADE_PUTONEQUIP ";
        //把装备拿下来
        public static readonly string SHELFUPGRADE_TAKEDOWNEQUIP = "SHELFUPGRADE_TAKEDOWNEQUIP ";
        //刷新货架上的物品显示
        public static readonly string SHELFUPGRADE_RESHOWNEQUIP = "SHELFUPGRADE_RESHOWNEQUIP";
        //货架装备变化
        public static readonly string ShelfChange_Equip = "ShelfChange_Equip";
        //货架装备变化效果通知
        public static readonly string ShelfChange_Equip_SFX = "ShelfChange_Equip_SFX";

        //资源篮变化
        public static readonly string ResBoxDisPlay_ReShow = "ResBoxDisPlay_ReShow";

    }

    public static class ProductionEvent
    {
        public static readonly string GET_RES_PRODUCTIONLIST = "GET_RES_PRODUCTIONLIST";
        public static readonly string RES_PRODUCTIONLIST_REFRESHUI = "RES_PRODUCTIONLIST_REFRESHUI";

        public static readonly string GET_EQUIP_DATA = "GET_EQUIP_DATA";                            //获取装备数据
                                                                                                    //   ||
        public static readonly string EQUIP_PRODUCTIONLIST_START = "EQUIP_PRODUCTIONLIST_START";    //开始制作
                                                                                                    //   || 
        public static readonly string EQUIP_PRODUCTIONLIST_UPDATE = "EQUIP_PRODUCTIONLIST_UPDATE";   //倒计时结束 刷新状态
                                                                                                     //   ||     
        public static readonly string EQUIP_PRODUCTIONLIST_MAKED = "EQUIP_PRODUCTIONLIST_MAKED";    //制作完成 收取

        public static readonly string UPDATEUI_EQUIP_MAKESLOT = "UPDATEUI_EQUIP_MAKESLOT";                            //刷新ui 制作槽

        public static readonly string EQUIP_PRODUCTIONLIST_Faster = "EQUIP_PRODUCTIONLIST_Faster";

        public static readonly string UIHandle_BuyMakeSlot = "UIHanlde_BuyMakeSlot";        //点击购买制作位
        public static readonly string UIHanlde_RefreshMakeBtnState = "UIHanlde_RefreshMakeBtnState"; //购买制作位回调

        public static readonly string UIHandle_SHOW_MAKINGSLOTINFO = "UIHandle_SHOW_MAKINGSLOTINFO"; //制作时点击制作栏

        public static readonly string UIRefresh_CheckMakeEquipRes = "UIRefresh_CheckMakeEquipRes";
        public static readonly string UIREFRESH_UPDATEPRODUCTIONTIME = "UIREFRESH_UPDATEPRODUCTIONTIME";
        public static readonly string UIREFRESH_PRODUCTIONREFRESHCOM = "UIREFRESH_PRODUCTIONREFRESHCOM";

        //游戏内
        public static readonly string PRODUCTIONLIST_STATE_Change = "PRODUCTIONLIST_STATE_Change";

        //单机逻辑 申请立即同步数据
        public static readonly string SyncUpdateProductionData = "SyncUpdateProductionData";

    }

    public static class DressUpEvent
    {
        public static readonly string TRANSOTHERSEXDATA = "TRANSOTHERSEXDATA";
        public static readonly string CHANGENAME = "CHANGENAME";
        public static readonly string BUYSINGLEDRESS = "BUYSINGLEDRESS";
        public static readonly string USERCUSTOM = "USERCUSTOM";
    }
    public static class ShopDesignEvent
    {
        public static readonly string ServerData_Ready = "ServerData_Ready";
        public static readonly string PICK_ITEM = "PICK_ITEM";          //选中
        public static readonly string RELEASE_ITEM = "RELEASE_ITEM";
        public static readonly string ROTATE_ITEM = "ROTATE_ITEM";
        public static readonly string Store_Item = "Store_Item";
        public static readonly string Create_Furniture = "Create_Furniture";
        public static readonly string Create_Customize = "Create_Customize";
        public static readonly string Apply = "Apply";
        public static readonly string Create_Failed_Space = "Create_Faild_Space";
        public static readonly string Store_Result = "Store_Result";
        public static readonly string Furniture_Upgrading = "Furniture_Upgrading";
        public static readonly string Furniture_Upgrading_Finish = "Furniture_Upgrading_Finish";
        public static readonly string Cancel_DesignItemHandle = "Cancel_DesignItemHandle";
        //刷新进度条
        public static readonly string reSetContentSliderVal = "reSetContentSliderVal";
        //家具UI标签切换
        public static readonly string FurnitureSelection_TabSelectd = "FurnitureSelection_TabSelectd";
        public static readonly string CustomizeSelection_TabSelectd = "CustomizeSelection_TabSelectd";
        //z
        public static readonly string EDITMODE_CHANGE = "EDITMODE_CHANGE";      //模式切换
        public static readonly string Furniture_Move_Rotate = "Furniture_Move_Rotate";  //柜台、装饰、货架移动和旋转
        public static readonly string Furniture_Data_Update = "Furniture_Data_Update"; //家具更新事件
        public static readonly string Furniture_CANAPPLY = "Furniture_CANAPPLY";
        public static readonly string Furniture_Upgrade_EndRefresh = "Furniture_Upgrade_EndRefresh";

        //家具皮肤相关
        public static readonly string ShowUI_FurnitureSkinUI = "ShowUI_FurnitureSkinUI";
        public static readonly string ShowUI_FurniturePaperUnLockUI = "ShowUI_FurniturePaperUnLockUI";

        public static readonly string SHOPDESIGN_COFIRM = "SHOPDESIGN_COFIRM";

        public static readonly string ShowTrunkUpgradeUI = "showTrunkUpgradeUI";

        //宠物小家相关
        public static readonly string LOOKPETHOUSE = "LOOKPETHOUSE"; //观赏宠物小家
        public static readonly string EXITPETHOUSE = "EXITPETHOUSE"; //退出观赏宠物小家

        //监测扩建或家具升级完成
        public static readonly string CameraMove_CheckExtendUpOrFurnitureUpEnd = "CameraMove_CheckExtendUpOrFurnitureUpEnd";

        //家具在设计模式下是否变化
        public static readonly string Set_DesignChangedFlag = "Set_DesignChangedFlag";


    }
    public static class Map2dEvent
    {
        public static readonly string IndoorInitEnd = "IndoorInitEnd";
    }
    public static class MoveSystemEvent
    {
        public static readonly string Move_Reach_Counter = "MoveSystemEvent_Reach_Counter ";
        public static readonly string Reach_Shelf = "MoveSystemEvent_Reach_Shelf";
        public static readonly string Reach_Decor = "MoveSystemEvent_Reach_Decor";
        public static readonly string Leave = "MoveSystemEvent_Leave";
        public static readonly string Blocked = "MoveSystemEvent_Blocked";
    }

    public static class ShopperEvent
    {
        public static readonly string SHOPPER_GETSHOPPERLIST = "SHOPPER_GETSHOPPERLIST";
        public static readonly string SHOPPER_COMING_NEW = "SHOPPER_COMING_NEW";        //新顾客
        public static readonly string SHOPPER_COMING_REPECT = "SHOPPER_COMING_REPECT"; //重复顾客
        public static readonly string SHOPPER_REMOVE = "SHOPPER_REMOVE";
        public static readonly string SHOPPER_SHOPPERDATACHANGE = "SHOPPER_SHOPPERDATACHANGE";  //顾客数据变化事件， 参数 为顾客uid
        public static readonly string SHOPPER_START_CHECKOUT = "SHOPPER_START_CHECKOUT";          //开始买 UI
        public static readonly string SHOPPER_WAITING = "SHOPPER_WAITING";              //等待
        public static readonly string SHOPPER_START_CHAT = "SHOPPER_START_CHAT";        //闲聊
        public static readonly string SHOPPER_START_DOUBLE = "SHOPPER_START_DOUBLE";    //加价
        public static readonly string SHOPPER_START_DISCOUNT = "SHOPPER_START_DISCOUNT";//打折
        public static readonly string SHOPPER_CHECKOUT = "SHOPPER_CHECKOUT";            //结算
        public static readonly string SHOPPER_REFUSE = "SHOPPER_REFUSE";                //拒绝
        public static readonly string SHOPPER_CANCEL = "SHOPPER_CANCEL";                //取消
        public static readonly string SHOPPER_CHANGE_TAGETEQUIP = "SHOPPER_CHANGE_TAGETEQUIP"; //开始切换顾客目标
        public static readonly string SHOPPER_CHANGE_NEXT = "SHOPPER_CHANGE_NEXT";
        public static readonly string SHOPPER_CHANGE_LAST = "SHOPPER_CHANGE_LAST";
        public static readonly string SHOPPERDATA_GETEND = "SHOPPERDATA_GETEND";
        public static readonly string SHOPPER_BUY_CONFIRM = "SHOPPER_BUY_CONFIRM";
        public static readonly string SHOPPER_BUY_CONFIRM_RESULT = "SHOPPER_BUY_CONFIRM_RESULT";
        public static readonly string SHOPPER_MOVEMENT_NOTIFIER = "SHOPPER_MOVEMENT_NOTIFIER";
        public static readonly string SHOPPER_POPUP = "SHOPPER_POPUP";//泡泡提示 uid, kShopperPopupType
        public static readonly string SHOPPER_HIDEBUBBLE = "SHOPPER_HIDEBUBBLE"; //隐藏泡泡
        public static readonly string SHOPPER_MOVE_UPDATE = "SHOPPER_MOVE_UPDATE";//移动更新 uid, Vector3
        public static readonly string Shopper_ChangeAIstate = "Shopper_ChangeAIstate";//换状态
        public static readonly string SHOPPER_ChangeEquip = "SHOPPER_ChangeEquip";  //换装
        public static readonly string SHOPPER_SellItem = "SHOPPER_SellItem";  //卖出
        public static readonly string SHOPPER_Bargaining = "SHOPPER_Bargaining";//讨价还价
        public static readonly string SHOPPER_StopBargaining = "SHOPPER_StopBargaining";//停止讨价还价
        public static readonly string SHOPPER_LookOrnamental = "SHOPPER_LooKOrnamental"; //顾客观赏加能量
        public static readonly string SHOPPER_ReShowPopupCheckOut = "SHOPPER_ReShowPopupCheckOut"; //顾客刷新气泡

        public static readonly string SHOPPER_SETREFUSEUID = "SHOPPER_SETREFUSEUID";
        public static readonly string SHOPPER_SHOWALLANIM = "SHOPPER_SHOWALLANIM";

        public static readonly string SHOPPER_UPDATEBYENERGY = "SHOPPER_UPDATEBYENERGY";
    }

    public class ReceiveEvent
    {
        public static readonly string NEWITEM_MSG = "NEWITEM_MSG";
        public static readonly string NEWITEM_MSG_REALY = "NEWITEM_MSG_Realy";
        public static readonly string GO_ON = "GO_ON"; //继续下一个
        public static readonly string Equip_IMPROVING_QUALITY = "Equip_IMPROVING_QUALITY";
    }

    public class WorkerCompEvent
    {
        public static readonly string SHOWUI_WORKERINFOUI = "SHOWUI_WORKERINFOUI";

        public static readonly string Worker_UnLock = "Worker_UnLock";
        public static readonly string Worker_ClickToRecruit = "Worker_ClickToRecruit";
        public static readonly string Worker_ExpChange = "Worker_ExpChange";
        public static readonly string Worker_LevelChange = "Worker_LevelChange";
        public static readonly string Worker_DataChg = "Worker_DataChg";

        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string REQUEST_Worker_Recruit = "REQUEST_Worker_Recruit";

    }

    public class ShopkeeperComEvent
    {
        public static readonly string INITCLOTHE = "INITCLOTHE";
        public static readonly string CHANGECLOTHE = "CHANGECLOTHE";
        public static readonly string CHANGESTATETOFREE = "CHANGESTATETOFREE";
        public static readonly string JUDGEROUNDHAVESHOPPER = "JUDGEROUNDHAVESHOPPER";
        public static readonly string ADDROUNDCOUNTERNUM = "ADDROUNDCOUNTERNUM";
        public static readonly string SUBTRACTROUNDCOUNTERNUM = "SUBTRACTROUNDCOUNTERNUM";
        public static readonly string SHOPPEALLNEW = "SHOPPEALLNEW";
        public static readonly string SHOPKEEPER_ACHEIVEMENTREFRESH = "SHOPKEEPER_ACHEIVEMENTREFRESH"; //成就状态刷新
    }

    public class PetCompEvent
    {

        public static readonly string PETDATA_GETEND = "PETDATA_GETEND";//宠物信息获取完毕 生成场景实例
        public static readonly string PET_ONDATACHANGE = "PET_ONDATACHANGE";   //宠物数据刷新
        public static readonly string PET_GOTODOORWAY = "PET_GOTODOORWAY";//宠物去门口看五倍商人
        public static readonly string PET_LEAVEDOORWAY = "PET_LEAVEDOORWAY";//五倍商人到柜台 宠物离开

        public static readonly string PET_TURNPAGE = "PET_TURNPAGE";   //宠物翻页


        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string REQUEST_PET_INFO = "REQUEST_PET_INFO";//获取宠物信息
        public static readonly string REQUEST_PET_UPDATEINFO = "REQUEST_PET_UPDATEINFO";//刷新单个宠物信息
    }

    public class IndoorRole_CanLockWorkerCompEvent
    {
        public static readonly string CHECK_CANLOCKWORKER = "CHECK_CANLOCKWORKER";
    }

    public class MarketCompEvent
    {
        public static readonly string SHOWUI_MARKETUI = "SHOWUI_MARKETUI";
        public static readonly string SHOWUI_MARKETINVENTORYUI = "SHOWUI_MARKETINVENTORYUI";
        public static readonly string SHOWUI_MARKETBUYBOOTHUI = "SHOWUI_MARKETBUYBOOTHUI";
        public static readonly string SHOWUI_MARKETITEMINFOUI = "SHOWUI_MARKETITEMINFOUI";
        public static readonly string SHOWUI_BOOTHITEMINFOUI = "SHOWUI_BOOTHITEMINFOUI";
        public static readonly string BOOTHITEMINFOUI_TURNPAGE = "BOOTHITEMINFOUI_TURNPAGE";
        public static readonly string SHOWUI_SOLDOUTBOOTHITEMAFFIRMUI = "SHOWUI_SOLDOUTBOOTHITEMAFFIRMUI";
        public static readonly string SHOWUI_BOOTHCREATELISTUI = "SHOWUI_BOOTHCREATELISTUI";
        public static readonly string SHOWUI_TOMARKETBYBAGUI = "SHOWUI_TOMARKETBYBAGUI";
        public static readonly string SHOWUI_SUBMITMARKETITEMUI = "SHOWUI_SUBMITMARKETITEMUI";
        public static readonly string SHOWUI_MARKETTRADINGHALLUI = "SHOWUI_MARKETTRADINGHALLUI";
        public static readonly string MARKETUI_MARKETITEMCHANGED = "MARKETUI_MARKETITEMCHANGED"; //成功交易出去
        public static readonly string MARKETUI_MARKETITEMPASTTIME = "MARKETUI_MARKETITEMPASTTIME"; //摊位物品过期
        public static readonly string MARKETUI_MARKETHALLREFRESH = "MARKETUI_MARKETHALLREFRESH"; //刷新交易所内容
        public static readonly string MARKETUI_REQUIREDITEM = "MARKETUI_REQUIREDITEM"; //缺少某个物品要交易



        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string MARKETBOOTH_REQUEST_DATA = "MARKETBOOTH_REQUEST_DATA"; //获取摊位信息
        public static readonly string MARKETBOOTH_BUYFIELD = "MARKETBOOTH_BUYFIELD"; //购买摊位
        public static readonly string MARKET_BOOTHITEMSUBMIT = "MARKET_BOOTHITEMSUBMIT"; //摊位-上架物品
        public static readonly string MARKET_BOOTHITEMRESUBMIT = "MARKET_BOOTHITEMRESUBMIT"; //摊位-重新上架物品
        public static readonly string MARKET_BOOTHITEMDEALWITH = "MARKET_BOOTHITEMDEALWITH"; //摊位-处理物品
        public static readonly string MARKET_BOOTHITEMSOLDOUT = "MARKET_BOOTHITEMSOLDOUT"; //摊位-下架物品
        public static readonly string MARKET_MARKETITEMLIST_REFRESH = "MARKET_MARKETITEMLIST_REFRESH";//刷新一个列表的拍卖物品
        public static readonly string MARKET_MARKETITEM_REFRESH = "MARKET_MARKETITEM_REFRESH";//刷新单个拍卖物品
        public static readonly string MARKET_MARKETITEM_DEALWITH = "MARKET_MARKETITEM_DEALWITH";//买或卖单个物品

        public static readonly string MARKET_REDPOINT_HAVENEWBOOTHDATA = "MARKET_REDPOINT_HAVENEWBOOTHDATA"; //刷新红点


    }
    //储蓄罐 事件
    public class MoneyBoxEvent
    {
        public static readonly string MONEYBOX_SHOWUI = "MONEYBOX_SHOWUI";
        public static readonly string MONEYBOX_HIDEUI = "MONEYBOX_HIDEUI";

        public static readonly string MONEYBOX_REQUEST_DATA = "MONEYBOX_REQUEST_DATA";
        public static readonly string MONEYBOX_REQUEST_REWARDS = "MONEYBOX_REQUEST_REWARDS";

        /////////////////////
        public static readonly string MONEYBOX_ONDATAUPDATE = "MONEYBOX_ONDATAUPDATE";
    }
    //聊天
    public class ChatSysEvent
    {
        public static readonly string CHAT_SHOWVIEW = "CHAT_SHOWVIEW";
        public static readonly string CHAT_UPDATE_NewChatData = "CHAT_UPDATE_NewChatData";
        public static readonly string CHAT_SENDMSG = "CHAT_SENDMSG";
        public static readonly string CHAT_VIEW_UPDATE = "CHAT_VIEW_UPDATE";
        public static readonly string CHAT_CHANNEL_CHANGE = "CHAT_CHANNEL_CHANGE";
        public static readonly string CHAT_REQUEST_DATA = "CHAT_REQUEST_DATA";
    }

    public class ItemChangeEvent
    {
        public static readonly string GOLDNUM_ADD = "GOLDNUM_ADD";
        public static readonly string GOLDNUM_REDUCE = "GOLDNUM_REDUCE";
        public static readonly string GEMNUM_ADD = "GEMNUM_ADD";
        public static readonly string ENERGYNUM_ADD = "ENERGYNUM_ADD";
        public static readonly string ENERGYNUM_REDUCE = "ENERGYNUM_REDUCE";
        public static readonly string ENERGYLIMITNUM_CHANGE = "ENERGYLIMITNUM_CHANGE";
        public static readonly string SELF_UNIONCOIN = "SELF_UNIONCOIN";
        public static readonly string UNION_UNIONCOIN = "UNION_UNIONCOIN";
        public static readonly string ACTIVITY_WORKERGAME_COIN = "ACTIVITY_WORKERGAME_COIN";
    }

    public class MenuEvent
    {
        public static readonly string SET_LOTTERY = "SET_LOTTERY";
        public static readonly string REFRESHMAINUIPAYGIFTBTNS = "REFRESHMAINUIPAYGIFTBTNS";
        public static readonly string SETTOPBTNSTATE = "SETTOPBTNSTATE";
        public static readonly string REFRESHONLINEREWARDBTNS = "REFRESHONLINEREWARDBTNS";
        public static readonly string REFRESHLUXURYBTN = "REFRESHLUXURYBTN";
        public static readonly string REFRESHREFUGEBTN = "REFRESHREFUGEBTN";
        public static readonly string SETREFUGEBTNSTATE = "SETREFUGEBTNSTATE";
    }

    public class LotteryEvent
    {
        public static readonly string LOTTERY_SHOWVIEW = "LOTTERY_SHOWVIEW";
        public static readonly string LOTTERY_REQUESTDATA = "LOTTERY_REQUESTDATA";
        public static readonly string LOTTERY_DATA = "LOTTERY_DATA";
        public static readonly string SINGLE_LOTTERY = "SINGLE_LOTTERY";
        public static readonly string SINGLE_LOTTERY_COM = "SINGLE_LOTTERY_COM";
        public static readonly string TENTH_LOTTERY = "TENTH_LOTTERY";
        public static readonly string JACKPOT_REFRESH = "JACKPOT_REFRESH";
        public static readonly string CUMULATIVE_GET = "CUMULATIVE_GET";
        public static readonly string EXHIBITION_SET = "EXHIBITION_SET";
        public static readonly string FREELOTERRY_OK = "FREELOTERRY_OK";
        public static readonly string TIMEIS_OK = "TIMEIS_OK";
        public static readonly string LOTTERY_REFRESH = "LOTTERY_REFRESH";
        public static readonly string LOTTERYRECORD_SHOWUI = "LOTTERYRECORD_SHOWUI";
        public static readonly string LOTTERYREWARD_SHOWUI = "LOTTERYREWARD_SHOWUI";
        public static readonly string LOTTERYSINGLEREWARD_SHOWUI = "LOTTERYSINGLEREWARD_SHOWUI";
        public static readonly string LOTTERY_GETREWARDCOMPLETE = "LOTTERY_GETREWARDCOMPLETE";
    }

    //拜访店铺
    public class VisitShopEvent
    {
        public static readonly string VISIT_ENTER_SHOP = "VISIT_ENTER_SHOP";
    }

    //社交相关
    public class SocialEvent
    {

        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string REQUEST_OTHERUSERDATA = "REQUEST_OTHERUSERDATA";
    }

    public class RoleEvent
    {
        public static readonly string ROLE_SHOWUI = "ROLE_SHOWUI";
        public static readonly string ROLEISSHOWING_SHOWUI = "ROLEISSHOWING_SHOWUI";
        public static readonly string ROLE_HIDEUI = "ROLE_HIDEUI";
        public static readonly string BUYSLOT_SHOWUI = "BUYSLOT_SHOWUI";
        public static readonly string BUYSLOT_HIDEUI = "BUYSLOT_HIDEUI";
        public static readonly string GETHERO_SHOWUI = "GETHERO_SHOWUI";
        public static readonly string ALLROLERESTING_SHOWUI = "ALLROLERESTING_SHOWUI";
        public static readonly string SINGLEROLERESTING_SHOWUI = "SINGLEROLERESTING_SHOWUI";
        public static readonly string ROLERESTING_HIDEUI = "ROLERESTING_HIDEUI";
        public static readonly string ROLEINFO_SHOWUI = "ROLEINFO_SHOWUI";
        public static readonly string ROLEINFO_HIDEUI = "ROLEINFO_HIDEUI";
        public static readonly string ROLERECRUIT_SHOWUI = "ROLERECRUIT_SHOWUI";
        public static readonly string ROLERECRUIT_HIDEUI = "ROLERECRUIT_HIDEUI";
        public static readonly string ROLEINTRODUCE_SHOWUI = "ROLEINTRODUCE_SHOWUI";
        public static readonly string ROLETALENTINTRODUCE_SHOWUI = "ROLETALENTINTRODUCE_SHOWUI";
        public static readonly string ROLETRANSFER_SHOWUI = "ROLETRANSFER_SHOWUI";
        public static readonly string ROLETRANSFER_HIDEUI = "ROLETRANSFER_HIDEUI";
        public static readonly string ROLEUSEEXPITEM_SHOWUI = "ROLEUSEEXPITEM_SHOWUI";
        public static readonly string ROLEWEAREQUIP_SHOWUI = "ROLEWEAREQUIP_SHOWUI";
        public static readonly string ROLEWEAREQUIP_HIDEUI = "ROLEWEAREQUIP_HIDEUI";
        public static readonly string ROLEEQUIP_SHOWUI = "ROLEEQUIP_SHOWUI";
        public static readonly string ROLEEQUIP_HIDEUI = "ROLEEQUIP_HIDEUI";
        public static readonly string ROLEUSEITEMANIM_SHOW = "ROLEUSEITEMANIM_SHOW";
        public static readonly string ROLEUPGRADE_SHOWUI = "ROLEUPGRADE_SHOWUI";
        public static readonly string ROLETRANSFERCOM_SHOWUI = "ROLETRANSFERCOM_SHOWUI";
        public static readonly string ROLEADVENTUREBYSLOT_SHOWUI = "ROLEADVENTUREBYSLOT_SHOWUI";
        public static readonly string ROLEADVENTURE_HIDEUI = "ROLEADVENTURE_HIDEUI";
        public static readonly string ROLERECRUITSUB_SHOWUI = "ROLERECRUITSUB_SHOWUI";
        public static readonly string ROLERECRUITSUB_HIDEUI = "ROLERECRUITSUB_HIDEUI";

        public static readonly string ROLEINFO_SHOW = "ROLEINFO_SHOW";

        public static readonly string REQUEST_ROLEDATA = "REQUEST_ROLEDATA";
        public static readonly string REQUEST_BUYNEWSLOT = "REQUEST_BUYNEWSLOT";
        public static readonly string REQUEST_RECRUITLIST = "REQUEST_RECRUITLIST";
        public static readonly string REQUEST_RECRUITREFRESH = "REQUEST_RECRUITREFRESH";
        public static readonly string REQUEST_BUYHERO = "REQUEST_BUYHERO";
        public static readonly string REQUEST_WEAREQUIP = "REQUEST_WEAREQUIP";
        public static readonly string REQUEST_HEROWEARALLEQUIP = "REQUEST_HEROWEARALLEQUIP"; //一键换装
        public static readonly string REQUEST_RENAME = "REQUEST_RENAME";
        public static readonly string REQUEST_DISMISSAL = "REQUEST_DISMISSAL";
        public static readonly string REQUEST_USEHEROITEM = "REQUEST_USEHEROITEM";
        public static readonly string REQUEST_HEROTRANSFER = "REQUEST_HEROTRANSFER";
        public static readonly string REQUEST_HEROREFRESH = "REQUEST_HEROREFRESH";
        public static readonly string REQUEST_HERORECOVER = "REQUEST_HERORECOVER";
        public static readonly string REQUEST_HEROEXCHANGELIST = "REQUEST_HEROEXCHANGELIST";
        public static readonly string REQUEST_HEROEXCHANGE = "REQUEST_HEROEXCHANGE";

        public static readonly string RESPONSE_RECRUITLIST = "RESPONSE_RECRUITLIST";
        public static readonly string RESPONSE_RECRUITLISTBAR = "RESPONSE_RECRUITLISTBAR";
        public static readonly string RESPONSE_SETHEROINFODATA = "RESPONSE_SETHEROINFODATA";
        public static readonly string RESPONSE_RESTINGSETDATA = "RESPONSE_RESTINGSETDATA";
        public static readonly string RESPONSE_EXPLORESHIFTIN = "RESPONSE_EXPLORESHIFTIN";
        public static readonly string RESPONSE_HEROSHIFTIN = "RESPONSE_HEROSHIFTIN";
        public static readonly string RESPONSE_HEROTYPECHANGE = "RESPONSE_HEROTYPECHANGE";
        public static readonly string RESPONSE_HEROEQUIPAUTO = "RESPONSE_HEROEQUIPAUTO";

        public static readonly string HIDEGETHEROUI_LIST = "HIDEGETHEROUI_LIST";

        // 装备破损
        public static readonly string SHOWUI_EQUIPDAMAGED = "SHOWUI_EQUIPDAMAGED";
        public static readonly string SHOWUI_EQUIPDAMAGEDINFO = "SHOWUI_EQUIPDAMAGEDINFO";
        public static readonly string REFRESHUI_EQUIPDAMAGEDINFO = "REFRESHUI_EQUIPDAMAGEDINFO";
        public static readonly string SETDAMAGEDEQUPINTRODUCE = "SETDAMAGEDEQUPINTRODUCE";
        public static readonly string REQUEST_REPAIREQUIP = "REQUEST_REPAIREQUIP";
        //public static readonly string REFRESHHEROANDEXPLORE = "REFRESHHEROANDEXPLORE";
        public static readonly string RESPONSE_REPAIREQUIPDATA = "RESPONSE_REPAIREQUIPDATA";
        public static readonly string HIDEUI_EQUIPDAMAGED = "HIDEUI_EQUIPDAMAGED";
        public static readonly string HIDEUI_EQUIPDAMAGEDINFO = "HIDEUI_EQUIPDAMAGEDINFO";

        // 转职预览
        public static readonly string SHOWUI_TRANSFERPREVIEW = "SHOWUI_TRANSFERPREVIEW";
        public static readonly string SHOWUI_ROLEHEROATTVIEW = "SHOWUI_ROLEHEROATTVIEW";

        public static readonly string ROLETRANSFERJUMPTOTOGGLE = "ROLETRANSFERJUMPTOTOGGLE";
    }

    //城市UI界面
    public class CityUIMediatorEvent
    {
        public static readonly string REFRESH_CITYUI_REDPOINT = "REFRESH_CITYUI_REDPOINT";
    }

    //城市建筑物
    public class CityBuildingEvent
    {
        public static readonly string BUILDING_ONCLICK = "BUILDING_ONCLICK";
        public static readonly string CITYBUILDINGINVESTUI_TURNPAGE = "CITYBUILDINGINVESTUI_TURNPAGE";
        public static readonly string SHOWUI_BUILDINGUPFINISH = "SHOWUI_BUILDINGUPFINISH";
        public static readonly string HIDEUI_CITYBUILDINGINVEST = "HIDEUI_CITYBUILDINGINVEST";
        public static readonly string INVESTUI_SETDATA = "INVESTUI_SETDATA";
        public static readonly string SCIENCELABUI_REFRESH = "SCIENCELABUI_REFRESH";
        public static readonly string INVEST_REFRESHUNIONRANK = "INVEST_REFRESHUNIONRANK";


        //---HUD
        public static readonly string HUD_BUILDINGUPADD = "HUD_BUILDINGUPADD";//添加
        public static readonly string HUD_BUILDINGUPREFRESH = "HUD_BUILDINGUPREFRESH";//刷新
        public static readonly string HUD_ALLBUILDINGUPREFRESH = "HUD_ALLBUILDINGUPREFRESH";//刷新
        public static readonly string HUD_BUILDINGUPCLEAR = "HUD_BUILDINGUPCLEAR";//清空

        public static readonly string HUD_SCIENCEBUILDINGREFRESH = "HUD_SCIENCEBUILDINGREFRESH";


        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string CITYBUILDING_GET_DATA = "CITYBUILDING_GET_DATA";
        public static readonly string CITYBUILDING_INVEST = "CITYBUILDING_INVEST";  //投资某个建筑
        public static readonly string CITYBUILDING_INVEST_RANK_DATA = "CITYBUILDING_INVEST_RANK_DATA"; //该建筑的公会成员投资详情

    }

    //副本
    public class ExploreEvent
    {
        public static readonly string EXPLORE_SHOWUI = "EXPLORE_SHOWUI";
        public static readonly string EXPLORE_HIDEUI = "EXPLORE_HIDEUI";
        public static readonly string EXPLOREUSEITEM_SHOWUI = "EXPLOREUSEITEM_SHOWUI";
        public static readonly string EXPLOREINFO_SHOWUI = "EXPLOREINFO_SHOWUI";
        public static readonly string EXPLOREFINISH_SHOWSUCCESSUI = "EXPLOREFINISH_SHOWSUCCESSUI";
        public static readonly string EXPLOREFINISH_SHOWLOSEUI = "EXPLOREFINISH_SHOWLOSEUI";
        public static readonly string EXPLOREUNLOCK_SHOWUI = "EXPLOREUNLOCK_SHOWUI";
        public static readonly string EXPLOREUNLOCK_HIDEUI = "EXPLOREUNLOCK_HIDEUI";
        public static readonly string EXPLOREPREPARE_SHOWUI = "EXPLOREPREPARE_SHOWUI";
        public static readonly string EXPLOREPREPARE_HIDEUI = "EXPLOREPREPARE_HIDEUI";
        public static readonly string EXPLOREUSEITEM_COMPLETE = "EXPLOREUSEITEM_COMPLETE";
        public static readonly string EXPLOREHERO_SHOWUI = "EXPLOREHERO_SHOWUI";
        public static readonly string EXPLOREPREPAREADD_COM = "EXPLOREPREPAREADD_COM";
        public static readonly string EXPLOREPREPAREREMOVE_COM = "EXPLOREPREPAREREMOVE_COM";
        public static readonly string EXPLORE_UPDATESLOTDATA = "EXPLORE_UPDATESLOTDATA";
        public static readonly string EXPLOREBUYSLOT_SHOWUI = "EXPLOREBUYSLOT_SHOWUI";
        public static readonly string EXPLOREBUYSLOT_HIDEUI = "EXPLOREBUYSLOT_HIDEUI";
        public static readonly string ROLEADVENTUREBYSLOT_SHOWUI = "ROLEADVENTUREBYSLOT_SHOWUI";
        public static readonly string HEROUPGRADESTART = "HEROUPGRADESTART";
        public static readonly string HEROUPGRADEEND = "HEROUPGRADEEND";
        public static readonly string EXPLOREUPGRADEEND = "EXPLOREUPGRADEEND";
        public static readonly string EXPLOREFINISH_HIDEUI = "EXPLOREFINISH_HIDEUI";
        public static readonly string EXPLORESELECTHERO_HIDEUI = "EXPLORESELECTHERO_HIDEUI";

        public static readonly string UPGRADEADD = "UPGRADEADD";
        public static readonly string UPGRADENEXT = "UPGRADENEXT";

        public static readonly string REQUEST_EXPLOREGROUPDATA = "REQUEST_EXPLOREGROUPDATA";
        public static readonly string REQUEST_EXPLORESLOTDATA = "REQUEST_EXPLORESLOTDATA";
        public static readonly string REQUEST_BUYEXPLORESLOT = "REQUEST_BUYEXPLORESLOT";
        public static readonly string REQUEST_EXPLORESTART = "REQUEST_EXPLORESTART";
        public static readonly string REQUEST_EXPLOREEND = "REQUEST_EXPLOREEND";
        public static readonly string REQUEST_EXPLOREUNLOCK = "REQUEST_EXPLOREUNLOCK";
        public static readonly string REQUEST_EXPLORESLOTREFRESH = "REQUEST_EXPLORESLOTREFRESH";
        public static readonly string REQUEST_EXPLOREIMMEDIATELY = "REQUEST_EXPLOREIMMEDIATELY";
        public static readonly string REQUEST_EXPLOREREWARDVIP = "REQUEST_EXPLOREREWARDVIP";

        public static readonly string RESPONSE_SETADVENTUREDATA = "RESPONSE_SETADVENTUREDATA";
        public static readonly string RESPONSE_PREPAREREFRESHDATA = "RESPONSE_PREPAREREFRESHDATA";

        public static readonly string RESPONSE_BUYVIPCOMPLETE = "RESPONSE_BUYVIPCOMPLETE";

        public static readonly string REFRESH_SORTHEROLIST = "REFRESH_SORTHEROLIST";

        public static readonly string Explore_JumpToTargetExplore = "Explore_JumpToTargetExplore";
    }

    //公会
    public class UnionEvent
    {
        public static readonly string SHOWUI_UNIONMAIN = "SHOWUI_UNIONMAIN";
        public static readonly string HIDEUI_UNIONMAIN = "HIDEUI_UNIONMAIN";
        public static readonly string SHOWUI_UNIONFINDTOOL = "SHOWUI_UNIONFINDTOOL";
        public static readonly string SHOWUI_CREATEUNION = "SHOWUI_CREATEUNION";
        public static readonly string SHOWUI_UNIONINFO = "SHOWUI_UNIONINFO";
        public static readonly string SHOWUI_EXITUNIONMSGBOX = "SHOWUI_EXITUNIONMSGBOX";
        public static readonly string SHOWUI_UNIONSETSETTING = "SHOWUI_UNIONSETSETTING";
        public static readonly string SHOWUI_UNIONKICKOUTMEMBERCONFIRM = "SHOWUI_UNIONKICKOUTMEMBERCONFIRM";
        public static readonly string SHOWUI_UNIONMEMBERSETTING = "SHOWUI_UNIONMEMBERSETTING";
        public static readonly string SHOWUI_UNIONMSGUPDATEUI = "SHOWUI_UNIONMSGUPDATEUI";
        public static readonly string SHOWUI_UNIONTASKRESULT = "SHOWUI_UNIONTASKRESULT";
        public static readonly string SHOWUI_UNIONTASKRESET = "SHOWUI_UNIONTASKRESET";
        public static readonly string ENTER_UNIONSCENE = "ENTER_UNIONSCENE";

        //----------------------------------------------------------------------------------------------------------------------------
        //----------------------------------------******** 公会基础功能  ********-----------------------------------------------------
        public static readonly string UNION_REQUEST_DATA = "UNION_REQUEST_DATA"; //获取公会信息
        public static readonly string UNION_REQUEST_CREATE = "UNION_REQUEST_CREATE"; //创建公会
        public static readonly string UNION_MSGBOX_ENTER = "UNION_MSGBOX_ENTER"; //加入公会二级确认
        public static readonly string UNION_REQUEST_ENTER = "UNION_REQUEST_ENTER"; //加入公会
        public static readonly string UNION_REQUEST_EXIT = "UNION_REQUEST_EXIT"; //退出公会
        public static readonly string UNION_REQUEST_LIST = "UNION_REQUEST_LIST";//获取公会列表数据（推荐、搜索）
        public static readonly string UNION_REQUEST_FINDPLAYERLIST = "UNION_REQUEST_FINDPLAYERLIST";//获取搜索玩家列表数据
        public static readonly string UNION_REQUEST_SETINFO = "UNION_REQUEST_SETINFO";//设置公会信息
        public static readonly string UNION_REQUEST_SETMEMBERJOB = "UNION_REQUEST_SETMEMBERJOB";//设置公会成员职务
        public static readonly string UNION_REQUEST_KICKOUTMEMBER = "UNION_REQUEST_KICKOUTMEMBER";//公会踢人
        public static readonly string UNION_REQUEST_MSGINFOREFRESH = "UNION_REQUEST_MSGINFOREFRESH";//公会信息刷新数据
        public static readonly string UNION_REQUEST_IMPEACH = "UNION_REQUEST_IMPEACH";//弹劾会长

        //----------------------------------------******** 公会援助功能  ********-----------------------------------------------------
        public static readonly string UNION_REQUEST_MEMBERHELPLIST = "UNION_REQUEST_MEMBERHELPLIST";//获取公会援助列表
        public static readonly string UNION_REQUEST_SETHELP = "UNION_REQUEST_SETHELP";//请求公会援助
        public static readonly string UNION_REQUEST_HELPMEMBER = "UNION_REQUEST_HELPMEMBER";//帮助玩家
        public static readonly string UNION_MEMBERHELPLISTREFRESH = "UNION_MEMBERHELPLISTREFRESH"; //公会援助列表信息刷新

        //----------------------------------------******** 公会悬赏功能  ********-----------------------------------------------------
        public static readonly string UNION_REQUEST_TASKLIST = "UNION_REQUEST_TASKLIST";//获取公会悬赏任务
        public static readonly string UNION_REQUEST_CHECKUNIONTASK = "UNION_REQUEST_CHECKUNIONTASK"; //查看公会任务
        public static readonly string UNION_REQUEST_STARTUNIONTASK = "UNION_STARTUNIONTASK";//接受公会任务
        public static readonly string UNION_REQUEST_CANCELUNIONTASK = "UNION_CANCELUNIONTASK";//取消公会任务
        public static readonly string UNION_REQUEST_REWARDUNIONTASK = "UNION_REWARDUNIONTASK";//结算公会任务
        public static readonly string UNION_REQUEST_GEMREWARDUNIONTASK = "UNION_REQUEST_GEMREWARDUNIONTASK";//钻石快速结算公会任务
        public static readonly string UNION_REQUEST_TASKRANKLIST = "UNION_REQUEST_TASKRANKLIST";//公会贡献排名
        public static readonly string UNION_REQUEST_TASKRESULT = "UNION_REQUEST_TASKRESULT";  //公会周结算展示

        //----------------------------------------******** 公会科技功能  ********-----------------------------------------------------
        public static readonly string UNION_HAVENEWTASKDATA = "UNION_HAVENEWTASKDATA"; //公会悬赏数据发生变化
        public static readonly string UNION_HAVENEWHELPDATA = "UNION_HAVENEWHELPDATA"; //公会援助数据发生变化
        public static readonly string UNION_HAVENEWSCIENCEDATA = "UNION_HAVENEWSCIENCEDATA"; //公会科技数据发生变化

        public static readonly string UNION_REQUEST_SCIENCELIST = "UNION_REQUEST_SCIENCELIST";//获取公会科技列表
        public static readonly string UNION_REQUEST_SCIENCEUPGRADE = "UNION_REQUEST_SCIENCEUPGRADE";//公会科技升级
        public static readonly string UNION_REQUEST_SKILLLIST = "UNION_REQUEST_SKILLLIST";//获取公会buff列表
        public static readonly string UNION_REQUEST_USESKILL = "UNION_REQUEST_USESKILL";//使用公会buff
        public static readonly string UNION_REQUEST_SKILLREFRESH = "UNION_REQUEST_SKILLREFRESH";//公会buff刷新
        public static readonly string UNION_REQUEST_BUFFDETAIL = "UNION_REQUEST_BUFFDETAIL"; //公会主界面buff详情

    }

    public class CombatEvent
    {
        public static readonly string COMBAT_PLAY_REPORT = "COMBAT_PLAY_REPORT";
        public static readonly string COMBAT_INITSCENE = "COMBAT_INITSCENE";
        public static readonly string COMBAT_UNPLAY_REPORT = "COMBAT_UNPLAY_REPORT";
        public static readonly string COMBAT_SCENE_LOADED = "COMBAT_SCENE_LOADED";
        public static readonly string COMBAT_UPDATEUI = "COMBAT_UPDATEUI";
        public static readonly string COMBAT_EXIT = "COMBAT_EXIT";

        public static readonly string COMBAT_SETANDINTOCOMBAT = "COMBAT_SETANDINTOCOMBAT";
        //跳字
        public static readonly string COMBAT_JUMP_TEXT = "COMBAT_JUMP_TEXT";
        //创建子弹
        public static readonly string COMBAT_CREATEBULLET = "COMBAT_CREATEBULLET";
        //ui刷新事件
        public static readonly string COMBAT_VIEW_HPUPDATE = "COMBAT_VIEW_HPUPDATE";
        public static readonly string COMBAT_PLAYSPEED_CHANGE = "COMBAT_PLAYSPEED_CHANGE";

        public static readonly string COMBAT_USE_ANGER = "COMBAT_USE_ANGER";
        public static readonly string COMBAT_USE_SKIP = "COMBAT_USE_SKIP";

        //测试用事件
        public static readonly string COMBAT_ADDBUFF_TEST = "COMBAT_ADDBUFF_TEST";
        public static readonly string COMBAT_USESKILL_TEST = "COMBAT_USESKILL_TEST";

    }

    public class TreasureBoxEvent
    {
        public static readonly string OPENBOX_SHOWUI = "OPENBOX_SHOWUI";
        public static readonly string OPENBOX_HIDEUI = "OPENBOX_HIDEUI";
        public static readonly string BOXINFO_SHOWUI = "BOXINFO_SHOWUI";
        public static readonly string BOXINFO_HIDEUI = "BOXINFO_HIDEUI";
        public static readonly string BOXCOMPLETE_SHOWUI = "BOXCOMPLETE_SHOWUI";
        public static readonly string BOXCOMPLETE_HIDEUI = "BOXCOMPLETE_HIDEUI";
        public static readonly string OPENTBOXUINOTPARA = "OPENTBOXUINOTPARA";

        public static readonly string SETTBOXCAMERA = "SETTBOXCAMERA";
        public static readonly string SETTBOXCAMERAREVERT = "SETTBOXCAMERAREVERT";

        public static readonly string REQUEST_TREASUREBOXDATA = "REQUEST_TREASUREBOXDATA";
        public static readonly string REQUEST_OPENTREASUREBOX = "REQUEST_OPENTREASUREBOX";
    }

    public class GuideEvent
    {
        public static readonly string SHOWGUIDEUI = "SHOWGUIDEUI";
        public static readonly string HIDEGUIDEUI = "HIDEGUIDEUI";
        public static readonly string REALHIDEGUIDEUI = "REALHIDEGUIDEUI";
        public static readonly string SETFINGERPOS = "SETFINGERPOS";
        public static readonly string SETPROMPTPOS = "SETPROMPTPOS";
        public static readonly string NPCCREAT = "NPCCREAT";
        public static readonly string NPCSTATE = "NPCSTATE";
        public static readonly string CLICKUNLOCKFURNITURE = "CLICKUNLOCKFURNITURE";
        public static readonly string FINGERACTIVEFALSE = "FINGERACTIVEFALSE";
        public static readonly string SETTARGET = "SETTARGET";
        public static readonly string SETNPCMOVE = "SETNPCMOVE";
        public static readonly string NPCLEAVE = "NPCLEAVE";
        public static readonly string SETMASKTARGET = "SETMASKTARGET";
        public static readonly string SETTRIGGERMASKTARGET = "SETTRIGGERMASKTARGET";
        public static readonly string WAITPREMASK = "WAITPREMASK";
        public static readonly string REFRESHTASK = "REFRESHTASK";
        public static readonly string SETGNEWTASK = "SETGNEWTASK";
        public static readonly string REQUEST_SETGUIDE = "REQUEST_SETGUIDE";
        public static readonly string REQUEST_SKIPGUIDE = "REQUEST_SKIPGUIDE";
        public static readonly string SETSLOTTARGET = "SETSLOTTARGET";
        public static readonly string SETTRIGGERDIALOG = "SETTRIGGERDIALOG";
        public static readonly string SETTRIGGERMASK = "SETTRIGGERMASK";
        public static readonly string HIDEALLSUBPANEL = "HIDEALLSUBPANEL";
        public static readonly string HIDESKIPBTN = "HIDESKIPBTN";
        public static readonly string SETNETWORKMASK = "SETNETWORKMASK";
    }


    public class UIUnlock
    {
        public static readonly string VIEW_ONSHOW = "VIEW_ONSHOW";
        public static readonly string SHOP_ONLVUP = "SHOP_ONLVUP";
        public static readonly string GUIDE_END = "GUIDE_END";
    }

    //邮件
    public class EmailEvent
    {
        public static readonly string SHOWUI_EmailMainUI = "SHOWUI_EmailMainUI";
        public static readonly string SHOWUI_FeedbackUI = "SHOWUI_FeedbackUI";
        public static readonly string SHOWUI_EmailDetailsUI = "SHOWUI_EmailDetailsUI";

        public static readonly string SHOWUI_RefreshTaskRedPoint = "SHOWUI_RefreshTaskRedPoint";//刷新任务红点

        //----------------------------------------------------------------------------------------------------------------------------

        // -- 邮件 --
        public static readonly string EMAIL_REQUEST_DATA = "EMAIL_REQUEST_DATA"; //获取邮件信息
        public static readonly string EMAIL_REQUEST_ALLREAD = "EMAIL_REQUEST_ALLREAD"; //一键领取
        public static readonly string EMAIL_REQUEST_SINGLEREAD = "EMAIL_REQUEST_SINGLEREAD"; //邮件设为已读
        public static readonly string EMAIL_REQUEST_CLAIMED = "EMAIL_REQUEST_CLAIMED"; //领取邮件
        public static readonly string EMAIL_REQUEST_SINGLEDEL = "EMAIL_REQUEST_SINGLEDEL"; //删除单个邮件

        // -- 反馈 --
        public static readonly string EMAIL_REQUEST_FEEDBACK = "EMAIL_REQUEST_FEEDBACK"; //问题反馈
    }

    // 活动（每日签到）
    public class ActivityEvent
    {
        public static readonly string REQUEST_DAILYGIFTLIST = "REQUEST_DAILYGIFTLIST";
        public static readonly string REQUEST_DAILYGIFTREWARD = "REQUEST_DAILYGIFTREWARD";

        public static readonly string REFRESH_DAILYGIFT = "REFRESH_DAILYGIFT";
        public static readonly string MOVEANIM_DAILYGIFT = "MOVEANIM_DAILYGIFT";
    }

    // 成就
    public class AcheivementEvent
    {
        public static readonly string SHOWUI_ACHEIVEMENTUI = "SHOWUI_ACHEIVEMENTUI";
        public static readonly string HIDEUI_ACHEIVEMENTUI = "HIDEUI_ACHEIVEMENTUI";
        public static readonly string SHOWUI_ACHEIVEMENTINTRODUCE = "SHOWUI_ACHEIVEMENTINTRODUCE";
        public static readonly string ACHEIVEMENTSETDATA = "ACHEIVEMENTSETDATA";
        public static readonly string SHOWUI_ACHEIVEMENTDONEUI = "SHOWUI_ACHEIVEMENTDONEUI";
        public static readonly string SHOWUI_ACHEIVEMENTDONESPECIALUI = "SHOWUI_ACHEIVEMENTDONESPECIALUI";
        public static readonly string SHOWUI_MSGBOX_NEEDACHEIVEMENT = "SHOWUI_MSGBOX_NEEDACHEIVEMENT";

        public static readonly string REQUEST_ACHEIVEMENTCHECK = "REQUEST_ACHEIVEMENTCHECK";
        public static readonly string REQUEST_ACHEIVEMENTAWARD = "REQUEST_ACHEIVEMENTAWARD";
        public static readonly string REQUEST_ACHEIVEMENTROADAWARD = "REQUEST_ACHEIVEMENTROADAWARD";

        public static readonly string RESPONSE_MAINUIREFRESH = "RESPONSE_MAINUIREFRESH";
        public static readonly string HIDEACHEIVEMENTDONEUI_LIST = "HIDEACHEIVEMENTDONEUI_LIST";
        public static readonly string UPDATEAAA = "UPDATEAAA";
    }

    public class SevenDayGoalEvent
    {
        public static readonly string SHOWUI_SEVENDAYUI = "SHOWUI_SEVENDAYUI";
        public static readonly string HIDEUI_SEVENDAYTUI = "HIDEUI_SEVENDAYTUI";

        public static readonly string REQUEST_SEVENDAYCHECK = "REQUEST_SEVENDAYCHECK";
        public static readonly string REQUEST_SEVENDAYAWARD = "REQUEST_SEVENDAYAWARD";
        public static readonly string REQUEST_SEVENDAYLISTAWARD = "REQUEST_SEVENDAYLISTAWARD";

        public static readonly string RESPONSE_SEVENDAYUIREFRESH = "RESPONSE_SEVENDAYUIREFRESH";

        public static readonly string JUMPTOOTHERPANEL = "JUMPTOOTHERPANEL";
        public static readonly string SHOWUI_AWARDINTRODUCE = "SHOWUI_AWARDINTRODUCE";

        public static readonly string SEVENDAY_CONTENTCHANGE = "SEVENDAY_CONTENTCHANGE";
        public static readonly string SEVENDAY_JUMP = "SEVENDAY_JUMP";
    }

    //街道掉落物（捡垃圾）
    public class StreetDropEvent
    {

        public static readonly string PASSBY_INITCREATE = "PASSBY_INITCREATE"; //创建行人

        public static readonly string STREETDROP_DATAREFRESH = "STREETDROP_DATAREFRESH"; //掉落物数据刷新了
        public static readonly string STREETDROP_DEAL = "STREETDROP_DEAL";//处理掉落物

        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string STREETDROP_REQUEST_REFRESH = "STREETDROP_REQUEST_REFRESH"; // 获取(刷新) 单个掉落物
        public static readonly string STREETDROP_REQUEST_CLAIMED = "STREETDROP_REQUEST_CLAIMED"; //领取掉落物
    }

    //全局Buff（全服享有buff）
    public class GlobalBuffEvent
    {
        public static readonly string GLOBALBUFF_SHOWUI_DETAIL = "GLOBALBUFF_SHOWUI_DETAIL"; //buff详情
        public static readonly string GLOBALBUFF_REFRESHUI_BUFFITEM = "GLOBALBUFF_REFRESHUI_BUFFITEM"; //刷新buffitem
        public static readonly string GLOBALBUFF_DELBUFFITEM = "GLOBALBUFF_DELBUFFITEM"; //移除某个buff

        //----------------------------------------------------------------------------------------------------------------------------
        public static readonly string REQEST_GLOBALBUFF_UPDATE = "REQEST_GLOBALBUFF_UPDATE";
        public static readonly string REQEST_GLOBALBUFF_REFRESH = "REQEST_GLOBALBUFF_REFRESH";
    }

    public class CommonEvent
    {
        public static readonly string COMMONTIPS_SETINFO = "COMMONTIPS_SETINFO";
        public static readonly string COMMONREWARD_SETINFO = "COMMONREWARD_SETINFO";
        public static readonly string COMMONMORETIPS_SETINFO = "COMMONMORETIPS_SETINFO";
        public static readonly string COMMONMORETITLECONTENT_SETINFO = "COMMONMORETITLECONTENT_SETINFO";
    }

    public class MainlineTaskEvent
    {
        public static readonly string SETTIMERRESET = "SETTIMERRESET";
        public static readonly string SETFINGTERACTIVE = "SETFINGTERACTIVE";
        public static readonly string SETTARGETTRANSFORM = "SETTARGETTRANSFORM";
        public static readonly string FINDTARGETTRANSFORM = "FINDTARGETTRANSFORM";
        public static readonly string SHOWMAINLINEUI = "SHOWMAINLINEUI";
        public static readonly string HIDEMAINLINEUI = "HIDEMAINLINEUI";
        public static readonly string SHOWMAINLINEDIALOG = "SHOWMAINLINEDIALOG";
        public static readonly string REQUESTMAINLINEDATA = "REQUESTMAINLINEDATA";
        public static readonly string REQUESTMAINLINEREWARD = "REQUESTMAINLINEREWARD";
        public static readonly string REFRESHTASKDATA = "REFRESHTASKDATA";
        public static readonly string NEWTASKPLAYANIM = "NEWTASKPLAYANIM";
        public static readonly string SHOWMAINLINEINFOUI = "SHOWMAINLINEINFOUI";
        public static readonly string REALHIDEPANEL = "REALHIDEPANEL";
        public static readonly string CREATFLOATPREFAB = "CREATFLOATPREFAB";

        //具体操作
        public static readonly string SELECTSCENEFUR = "SELECTSCENEFUR"; //选择场景家具
        public static readonly string SELECTUIFURN = "SELCETUIFURN"; //选择界面家具
        public static readonly string RECRUITHERO = "RECRUITHERO"; //招募英雄
        public static readonly string ADDHEROSLOT = "ADDHEROSLOT"; //增加英雄栏位
        public static readonly string CLICKHEROINFO = "CLICKHEROINFO"; //点击英雄详情
        public static readonly string CLICKSHOPPERPOP = "CLICKSHOPPERPOP"; //点击顾客气泡
        public static readonly string OPENTARGETTBOX = "OPENTARGETTBOX"; //打开指定宝箱
        public static readonly string BUILDINVEST = "BUILDINVEST"; //指定建筑投资
        public static readonly string SCIENCEBUILDINVEST = "SCIENCEBUILDINVEST"; //指定建筑投资
        public static readonly string SELECTTARGETEQUIP = "SELECTTARGETEQUIP"; //选择指定装备
        public static readonly string SELECTTARGETWORKER = "SELECTTARGETWORKER"; //选择指定工匠
        public static readonly string SELECTTARGETTRANSFERHERO = "SELECTTARGETTRANSFERHERO"; //选择指定职业英雄
        public static readonly string SELECTTARGETEXPLORE = "SELECTTARGETEXPLORE"; //选择指定副本
        public static readonly string SELECTTARGETTRANSFERTOGGLE = "SELECTTARGETTRANSFERTOGGLE"; //选择指定转职分页
        public static readonly string SELECTCANRECRUITHERO = "SELECTCANRECRUITHERO"; //选择可以招募的英雄
        public static readonly string SELECTTARGETEQUIPPAGE = "SELECTTARGETEQUIPPAGE"; //选择指定装备分页

        public static readonly string Reset_TargetExplore = "Reset_TargetExplore";
        public static readonly string Reset_TargetEquipType = "Reset_TargetEquipType";
    }
    public static readonly string LOGIN_SDK = "LOGIN_SDK";
    public class BindingEvent
    {
        public static readonly string GETBINDINGAWARD = "GETBINDINGAWARD";          //领取绑定奖励
        public static readonly string CHANGEACCOUNTRLOGIN = "CHANGEACCOUNTRLOGIN";  //切换账号
        public static readonly string BINDINGACCOUNT = "BINDINGACCOUNT";            //绑定当前账号
        public static readonly string UPDATEBINGSTATE = "UPDATEBINGSTATE";          //刷新绑定状态
        public static readonly string UPDATEPLATFORMQUERY = "UPDATEPLATFORMQUERY"; //刷新新的平台进度
    }

    public class GameAdEvent
    {
        public static readonly string GAMEAD_GETDATA = "GAMEAD_GETDATA";
        public static readonly string GAMEAD_START = "GAMEAD_START";
        public static readonly string GAMEAD_END = "GAMEAD_END";
        public static readonly string GAMEAD_UPDATEVIEW = "GAMEAD_UPDATEVIEW";
        public static readonly string GAMEAD_SHOWADVIEW = "GAMEAD_SHOWADVIEW";
    }
}

public class AdEvent
{
    //广告
    public readonly static String RewardedVideoAdOpened = "RewardedVideoAdOpened";
    public readonly static String RewardedVideoAdClosed = "RewardedVideoAdClosed";
    public readonly static String RewardedVideoAvailabilityChanged = "RewardedVideoAvailabilityChanged";
    public readonly static String RewardedVideoAdStarted = "RewardedVideoAdStarted";
    public readonly static String RewardedVideoAdEnded = "RewardedVideoAdEnded";
    public readonly static String RewardedVideoAdRewarded = "RewardedVideoAdRewarded";
    public readonly static String RewardedVideoAdShowFailed = "RewardedVideoAdShowFailed";
    public readonly static String RewardedVideoAdClicked = "RewardedVideoAdClicked";
}
