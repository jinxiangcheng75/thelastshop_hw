using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopperDataProxy : TSingletonHotfix<ShopperDataProxy>, IDataModelProx
{
    public List<Vector3Int> shopperToIndoorCanTalkPosList;

    private Dictionary<int, ShopperData> shopperQueueList;


    private void addCanTalkPos(int x_start, int x_end, int y_start, int y_end)
    {
        for (int x = x_start; x <= x_end; x++)
        {
            for (int y = y_start; y <= y_end; y++)
            {
                shopperToIndoorCanTalkPosList.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    private void getShopperToIndoorCanTalkPosList()
    {
        int x1_start = 0, x1_end = 1, y1_start = 22, y1_end = 22;

        shopperToIndoorCanTalkPosList = new List<Vector3Int>();

        addCanTalkPos(x1_start, x1_end, y1_start, y1_end);
    }

    public void Init()
    {
        shopperQueueList = new Dictionary<int, ShopperData>();

        getShopperToIndoorCanTalkPosList();

        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Change_Cmd, OnShopperStateChangeResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Data_Cmd, OnGetShopperDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Energy_Cmd, onShopperLookOrnamentalOver);
    }

    private void OnShopperStateChangeResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Change data = (Response_Shopper_Change)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            ShopperData _shopper;
            if (!shopperQueueList.TryGetValue(data.shopper.shopperUid, out _shopper))
            {
                //if (data.shopper)
                _shopper = new ShopperData(data.shopper);

                if (_shopper.data.shopperState != 100) //离开
                {
                    shopperQueueList.Add(_shopper.data.shopperUid, _shopper);
                    //新顾客 事件 （判断场景中是否有重复的存在 拒绝让他离开）
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_COMING_NEW, data.shopper.shopperUid);
                }
                else
                {
                    //判断场景中是否有重复的存在 直接删除
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_COMING_REPECT, data.shopper.shopperUid);
                }
            }
            else
            {
                bool isSameDemand = _shopper.data.targetEquipId == data.shopper.targetEquipId;
                _shopper.data = data.shopper;
                _shopper.leaveFlag = data.flag;
                _shopper.SetTimer();

                //需求变化刷新一下气泡
                if (!isSameDemand) EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_ReShowPopupCheckOut, _shopper.data.shopperUid);
                //顾客数据改变事件
                EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_SHOPPERDATACHANGE, _shopper.data.shopperUid);
            }

            //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.JUDGEROUNDHAVESHOPPER);
        }
    }
    public void RemoveShopper(int uid)
    {
        if (shopperQueueList.ContainsKey(uid))
        {
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REMOVE, uid);
            shopperQueueList.Remove(uid);
        }
    }
    // 初始列表
    private void OnGetShopperDataResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Data data = (Response_Shopper_Data)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            if (shopperQueueList.Count > 0)
                shopperQueueList.Clear();
            foreach (var shopper in data.shopperList)
            {
                var _shopper = new ShopperData(shopper);
                shopperQueueList.Add(_shopper.data.shopperUid, _shopper);
            }
            //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.CHANGESTATETOFREE);
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPERDATA_GETEND);
            GoOperationManager.inst.isInitShopperData = true;
        }
    }

    private void onShopperLookOrnamentalOver(HttpMsgRspdBase msg)
    {
        Response_Shopper_Energy data = (Response_Shopper_Energy)msg;
    }

    public int ShopperCount()
    {
        return shopperQueueList.Count;
    }
    public List<ShopperData> GetShopperList()
    {
        return shopperQueueList.Values.ToList();
    }

    public List<int> GetShopperIdList()
    {
        List<int> ids = new List<int>();

        foreach (var item in shopperQueueList.Values)
        {

            if (item.data.shopperType != (int)EShopperType.Buy && item.data.shopperType != (int)EShopperType.Sell)
            {
                continue;
            }

            Shopper shopper = IndoorRoleSystem.inst.GetShopperByUid(item.data.shopperUid);
            if (shopper != null)
            {
                if (shopper.shopperData.data.shopperState == 99 && shopper.GetCurState() == MachineShopperState.queuing)
                {
                    ids.Add(item.data.shopperUid);
                }
            }
        }
        return ids;
    }

    //获取尚未离开的顾客uid列表
    public string GetNotLeaveShopperUidList()
    {
        string result = "";

        foreach (var item in shopperQueueList.Values)
        {
            if (item.data.shopperState != 100)
            {
                result += item.data.shopperUid + "|";
            }
            //Shopper shopper = IndoorRoleSystem.inst.GetShopperByUid(item.data.shopperUid);
            //if (shopper != null)
            //{
            //    if (shopper.GetCurState() != MachineShopperState.readyToLeave && shopper.GetCurState() != MachineShopperState.leave)
            //    {
            //        result += item.data.shopperUid + "|";
            //    }
            //}
        }

        if (result.Length > 0)
        {
            result = result.Substring(0, result.Length - 1);
        }

        return result;
    }

    public ShopperData GetShopperData(int shopperuid)
    {
        if (shopperQueueList.ContainsKey(shopperuid))
        {
            return shopperQueueList[shopperuid];
        }
        return null;
    }

    //public bool JudgeIsHaveShopperQueuing()
    //{
    //    foreach (var item in shopperQueueList.Values)
    //    {
    //        //Logger.error("状态是" + item.data.shopperState);
    //        if (IndoorMapEditSys.inst.GetShopperActor(item.data.shopperUid).ActorAI.isInDoor && item.data.shopperState == 99)
    //            return true;
    //    }

    //    return false;
    //}

    public void Clear()
    {
        if (shopperQueueList != null)
            shopperQueueList.Clear();
        if (shopperToIndoorCanTalkPosList != null)
            shopperToIndoorCanTalkPosList.Clear();
    }

}
