using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBoxSys : BaseSystem
{
    MoneyBoxView moneyBoxView;
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.MoneyBoxEvent.MONEYBOX_SHOWUI, ShowMoneyBoxView);
        EventController.inst.AddListener(GameEventType.MoneyBoxEvent.MONEYBOX_HIDEUI, hideMoneyBoxView);

        EventController.inst.AddListener(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_DATA, RequestMoneyBoxData);
        EventController.inst.AddListener(GameEventType.MoneyBoxEvent.MONEYBOX_ONDATAUPDATE, updateView);

        EventController.inst.AddListener(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_REWARDS, GetRewards);
        Helper.AddNetworkRespListener(MsgType.Response_PiggyBank_Props_Cmd, OnGetRewards);
    }
    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.MoneyBoxEvent.MONEYBOX_SHOWUI, ShowMoneyBoxView);
        EventController.inst.RemoveListener(GameEventType.MoneyBoxEvent.MONEYBOX_HIDEUI, hideMoneyBoxView);

        EventController.inst.RemoveListener(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_DATA, RequestMoneyBoxData);
        EventController.inst.RemoveListener(GameEventType.MoneyBoxEvent.MONEYBOX_ONDATAUPDATE, updateView);

        EventController.inst.RemoveListener(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_REWARDS, GetRewards);
    }

    protected void OnGetRewards(HttpMsgRspdBase msg)
    {
        Response_PiggyBank_Props data = (Response_PiggyBank_Props)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //获得一次小猪奖励
            PlatformManager.inst.GameHandleEventLog("Piggy", "");
            PlatformManager.inst.GameHandleEventLog("Piggy", $"{data.rewardId}:{data.rewardNum}");

            if (data.rewardId > 0)
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, data.rewardId, data.rewardNum));
        }
        MoneyBoxDataProxy.inst.updateData(data.piggyBankData);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 4);
    }
    void ShowMoneyBoxView()
    {
        if (MoneyBoxDataProxy.inst.moneyBoxData == null) return;
        GUIManager.OpenView<MoneyBoxView>((view) =>
        {
            moneyBoxView = view;
            view.UpdateUI();
        });
        //if (moneyBoxView == null)
        //{
        //    moneyBoxView = new MoneyBoxView();
        //}
        //if (moneyBoxView.isShowing)
        //{
        //    moneyBoxView.UpdateUI();
        //}
        //else
        //{
        //    moneyBoxView.show();
        //}
    }

    void hideMoneyBoxView()
    {
        GUIManager.HideView<MoneyBoxView>();
        //if (moneyBoxView != null && moneyBoxView.isShowing)
        //{
        //    moneyBoxView.hide();
        //}
    }

    void updateView()
    {
        if (moneyBoxView != null && moneyBoxView.isShowing)
        {
            moneyBoxView.UpdateUI();
        }
    }


    //Request_Piggy_Bank_Cmd
    private void RequestMoneyBoxData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_PiggyBank_ClickData()
        });
    }

    //领取奖励
    private void GetRewards()
    {
        if (MoneyBoxDataProxy.inst.moneyBoxData.currState == 2)
        {
            //领取
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_PiggyBank_Props()
                {
                    receive = 1
                }
            });
            //hideMoneyBoxView();
        }
        else
        {
            //刷新数据
            RequestMoneyBoxData();
        }
    }
}
