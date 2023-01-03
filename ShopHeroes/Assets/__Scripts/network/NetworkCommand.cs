using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class NetworkCommand
{

    public static Dictionary<int, string> CommandUrlDict;
    public static void Init()
    {
        var Dict = new Dictionary<int, string>();
        CommandUrlDict = Dict;

        Dict.Add(MsgType.BASE_CMD, "game/test");
        Dict.Add(MsgType.Request_Csv_List_Cmd, "game/gate");
        Dict.Add(MsgType.Request_Csv_Load_Cmd, "game/gate");
        Dict.Add(MsgType.Request_Gate_Cmd, "game/gate");
        Dict.Add(MsgType.Request_User_Login_Cmd, "game/login");
        Dict.Add(MsgType.Request_User_Create_Cmd, "game/info");
        Dict.Add(MsgType.Request_User_Data_Cmd, "game/info");
        Dict.Add(MsgType.Request_Bag_Data_Cmd, "game/info");
        Dict.Add(MsgType.Request_Resource_ProductionList_Cmd, "game/info");
        Dict.Add(MsgType.Request_Resource_ProductionRefresh_Cmd, "game/info");
        Dict.Add(MsgType.Request_DailyTask_Data_Cmd, "game/info");
        Dict.Add(MsgType.Request_DailyTask_Reward_Cmd, "game/info");
        Dict.Add(MsgType.Request_DailyTask_Refresh_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_Data_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_MakeStart_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_MakeRefresh_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_MakeEnd_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_BuySlot_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_MakeFaster_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_MakeImprove_Cmd, "game/info");
        Dict.Add(MsgType.Request_Equip_Activate_Cmd, "game/info");

        //Dict.Add(MsgType.Request_Equip_BuySlot_Cmd, "game/info");
        Dict.Add(MsgType.Request_User_ChangeName_Cmd, "game/info");
        Dict.Add(MsgType.Request_User_DressList_Cmd, "game/info");
        Dict.Add(MsgType.Request_User_BuyDress_Cmd, "game/info");
        Dict.Add(MsgType.Request_User_Custom_Cmd, "game/info");
        Dict.Add(MsgType.Request_Bag_Del_Cmd, "game/info");
        Dict.Add(MsgType.Request_Bag_LockEquip_Cmd, "game/info");

        Dict.Add(MsgType.Request_Shopper_Chat_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Checkout_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Coming_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Data_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Discount_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Double_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Refuse_Cmd, "game/info");
        Dict.Add(MsgType.Request_Shopper_Recommend_Cmd, "game/info");
        Dict.Add(MsgType.Request_Design_Data_Cmd, "game/info");

        Dict.Add(MsgType.Request_Hero_Data_Cmd, "game/info");
        Dict.Add(MsgType.Request_Hero_FieldUnlock_Cmd, "game/info");
        Dict.Add(MsgType.Request_Hero_Buy_Cmd, "game/info");
        Dict.Add(MsgType.Request_Hero_Equip_Cmd, "game/info");
        Dict.Add(MsgType.Request_Hero_WarriorRankUp_Cmd, "game/info");
        Dict.Add(MsgType.Request_User_BehaviorCounter_Cmd, "game/counter");
        //Dict.Add(MsgType.Request_Jackpot_Data_Cmd, "game/info");
        hotfixProcess();
    }

    static void hotfixProcess()
    {

    }


    public static string GetUrl(int cmd)
    {
        string s = string.Empty;
        if (!CommandUrlDict.TryGetValue(cmd, out s))
        {
#if UNITY_EDITOR
            Logger.warning("cmd : " + cmd + " not register url !!!");
#endif
            return "game/info";
        }
        return s;
    }
}
