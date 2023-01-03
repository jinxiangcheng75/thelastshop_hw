using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBagSystem : BaseSystem
{
    BagUIView bagUIView;
    ItemInfoUIView itemInfoUIView;
    protected override void AddListeners()
    {
        base.AddListeners();
        EventController.inst.AddListener(GameEventType.SHOWUI_BAGUI, openBagUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_BAGUI, hideBagUI);
        EventController.inst.AddListener(GameEventType.BagEvent.BAG_GET_DATA, GetBagData);
        EventController.inst.AddListener<string, int>(GameEventType.EquipEvent.EQUIP_REMOVE, removeEquip);
        EventController.inst.AddListener<string, bool>(GameEventType.EquipEvent.EQUIP_LOCK, EquipLock);
        EventController.inst.AddListener<int>(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, showItemInfo);
        EventController.inst.AddListener<int>(GameEventType.BagEvent.Bag_BuyProduction, request_BuyProduction);
        EventController.inst.AddListener<int>(GameEventType.BagEvent.Bag_BuyProductionByUnoinCoin, request_BuyProductionByUnionCoin);

        NetworkEvent.SetCallback(MsgType.Response_Bag_Data_Cmd,
        (successResp) =>
        {
            this.OnBagDataResp((Response_Bag_Data)successResp);
        },
        (failedResp) =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("背包数据获取失败!"));
        });

        NetworkEvent.SetCallback(MsgType.Response_Bag_ResourceChange_Cmd,
        (successResp) =>
        {
            this.OnResourceChangeResp((Response_Bag_ResourceChange)successResp);
        },
        (failedResp) =>
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, "背包数据获取失败!");
        });

        Helper.AddNetworkRespListener(MsgType.Response_Equip_BagEquipChange_Cmd, OnEquipChangeResp);
        Helper.AddNetworkRespListener(MsgType.Response_Bag_Del_Cmd, OnRemoveEquipResp);
        Helper.AddNetworkRespListener(MsgType.Response_Bag_LockEquip_Cmd, OnEquipLockResp);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        EventController.inst.RemoveListener(GameEventType.BagEvent.BAG_GET_DATA, GetBagData);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_BAGUI, openBagUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_BAGUI, hideBagUI);



        EventController.inst.RemoveListener<string, int>(GameEventType.EquipEvent.EQUIP_REMOVE, removeEquip);
        EventController.inst.RemoveListener<string, bool>(GameEventType.EquipEvent.EQUIP_LOCK, EquipLock);
        EventController.inst.RemoveListener<int>(GameEventType.BagEvent.Bag_BuyProduction, request_BuyProduction);
        EventController.inst.RemoveListener<int>(GameEventType.BagEvent.Bag_BuyProductionByUnoinCoin, request_BuyProductionByUnionCoin);
        EventController.inst.RemoveListener<int>(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, showItemInfo);
    }

    private void showItemInfo(int itemid)
    {
        GUIManager.OpenView<ItemInfoUIView>(view =>
        {
            view.ShowInfo(itemid);
        });
    }

    private void openBagUI()
    {
        GUIManager.OpenView<BagUIView>();
    }
    private void hideBagUI()
    {
        GUIManager.HideView<BagUIView>();
    }

    private void removeEquip(string uid, int count)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Bag_Del()
            {
                delType = 2,
                delCount = count,
                equipUid = uid
            }
        });
    }
    private void EquipLock(string uid, bool _lock)
    {
        if (_lock)
        {
            AudioManager.inst.PlaySound(123);
        }

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Bag_LockEquip()
            {
                equipUid = uid,
                lockOrUnlock = _lock ? 1 : 0
            }
        });
    }
    private void OnEquipLockResp(HttpMsgRspdBase msg)
    {
        Response_Bag_LockEquip data = (Response_Bag_LockEquip)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //删除成功
            Logger.log("操作成功，锁定或解锁装备");
        }
    }
    private void OnRemoveEquipResp(HttpMsgRspdBase msg)
    {

        Response_Bag_Del data = (Response_Bag_Del)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //删除成功
            AudioManager.inst.PlaySound(127);

            Logger.log("操作成功，删除装备");
            EventController.inst.TriggerEvent(GameEventType.HIDEUI_EQUIPRESOLVEUI);

        }
    }

    //购买资源
    private void request_BuyProduction(int itemId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Resource_BuyProduction()
            {
                itemId = itemId
            }
        });
    }

    //购买资源 联盟币
    private void request_BuyProductionByUnionCoin(int itemId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Resource_BuyProductionDaily()
            {
                itemId = itemId
            }
        });
    }


    private void OnResourceChangeResp(Response_Bag_ResourceChange data)
    {
        ItemBagProxy.inst.updateItemNum(data.resource.itemId, data.resource.count);
        //刷新res 显示
        EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_RES_UPDATE, data.resource.itemId);
    }

    //背包装备变化
    private void OnEquipChangeResp(HttpMsgRspdBase msg)
    {
        Response_Equip_BagEquipChange data = (Response_Equip_BagEquipChange)msg;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        int beforeNum = ItemBagProxy.inst.GetEquipInventory();

        ItemBagProxy.inst.updateEquipNum(data.bagEquip.equipUid, data.bagEquip.equipId, data.bagEquip.count, data.bagEquip.getTime, data.bagEquip.isLock == 1, data.bagEquip.onShelfCount);

        int afterNum = ItemBagProxy.inst.GetEquipInventory();

        if (!GameSettingManager.inst.needShowUIAnim)
            EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_EQUIP_UPDATE);

        var bagUIView = GUIManager.GetWindow<BagUIView>();

        if (bagUIView != null && bagUIView.isShowing)
        {
            bagUIView.refreshList();
        }

        if (beforeNum != afterNum)
        {
            EventController.inst.TriggerEvent(GameEventType.BagEvent.Bag_inventory_numChg);
        }
    }

    private void OnBagDataResp(Response_Bag_Data data)
    {
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //资源
            foreach (var res in data.bagResourceList)
            {
                ItemBagProxy.inst.updateItemNum(res.itemId, res.count);
            }
            //装备
            foreach (var equip in data.bagEquipList)
            {
                ItemBagProxy.inst.updateEquipNum(equip.equipUid, equip.equipId, equip.count, equip.getTime, equip.isLock == 1, equip.onShelfCount);
            }
            //背包数据变化
            EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_DATA_UPDATE);
            //请求资源生产队列 （测试）
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.GET_RES_PRODUCTIONLIST);
        }
    }
    //请求背包数据
    private void GetBagData()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Bag_Data()
            });
        }
    }

}
