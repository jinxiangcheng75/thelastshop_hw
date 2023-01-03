using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotterySystem : BaseSystem
{
    LotteryUIView lotteryUIView;
    LotteryRecordView lotteryRecordView;
    LotteryGetRewardView lotteryRewardView;
    protected override void OnInit()
    {
    }

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.LotteryEvent.LOTTERY_SHOWVIEW, showLotteryUI);
        EventController.inst.AddListener(GameEventType.LotteryEvent.LOTTERY_REQUESTDATA, RequestLotteryData);
        EventController.inst.AddListener(GameEventType.LotteryEvent.LOTTERY_DATA, RefreshLotteryData);
        EventController.inst.AddListener<int>(GameEventType.LotteryEvent.SINGLE_LOTTERY, RequestSingleLottery);
        EventController.inst.AddListener<int>(GameEventType.LotteryEvent.TENTH_LOTTERY, RequestTenthLottery);
        EventController.inst.AddListener<int, int>(GameEventType.LotteryEvent.JACKPOT_REFRESH, RequestJackpotRefresh);
        EventController.inst.AddListener<List<JackpotData>, int>(GameEventType.LotteryEvent.SINGLE_LOTTERY_COM, ResponseSigleLottery);
        EventController.inst.AddListener<List<ShowPopupData>>(GameEventType.LotteryEvent.CUMULATIVE_GET, CumulativeGet);
        EventController.inst.AddListener(GameEventType.LotteryEvent.FREELOTERRY_OK, SetLotteryText);
        EventController.inst.AddListener(GameEventType.LotteryEvent.TIMEIS_OK, RequestTimeOverState);

        EventController.inst.AddListener(GameEventType.LotteryEvent.LOTTERYRECORD_SHOWUI, showLotteryRecord);
        EventController.inst.AddListener<List<CommonRewardData>>(GameEventType.LotteryEvent.LOTTERYREWARD_SHOWUI, showLotteryGetReward);
        EventController.inst.AddListener<CommonRewardData>(GameEventType.LotteryEvent.LOTTERYSINGLEREWARD_SHOWUI, showLotterySingleGetReward);

        EventController.inst.AddListener<bool>(GameEventType.LotteryEvent.LOTTERY_GETREWARDCOMPLETE, GetRewardComplete);

        EventController.inst.AddListener<int>(GameEventType.LotteryEvent.LOTTERY_REFRESH, RefreshLottery);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.LotteryEvent.LOTTERY_SHOWVIEW, showLotteryUI);
        EventController.inst.RemoveListener(GameEventType.LotteryEvent.LOTTERY_REQUESTDATA, RequestLotteryData);
        EventController.inst.RemoveListener(GameEventType.LotteryEvent.LOTTERY_DATA, RefreshLotteryData);
        EventController.inst.RemoveListener<int>(GameEventType.LotteryEvent.SINGLE_LOTTERY, RequestSingleLottery);
        EventController.inst.RemoveListener<int>(GameEventType.LotteryEvent.TENTH_LOTTERY, RequestTenthLottery);
        EventController.inst.RemoveListener<int, int>(GameEventType.LotteryEvent.JACKPOT_REFRESH, RequestJackpotRefresh);
        EventController.inst.RemoveListener<List<JackpotData>, int>(GameEventType.LotteryEvent.SINGLE_LOTTERY_COM, ResponseSigleLottery);
        EventController.inst.RemoveListener<List<ShowPopupData>>(GameEventType.LotteryEvent.CUMULATIVE_GET, CumulativeGet);
        EventController.inst.RemoveListener(GameEventType.LotteryEvent.FREELOTERRY_OK, SetLotteryText);
        EventController.inst.RemoveListener(GameEventType.LotteryEvent.TIMEIS_OK, RequestTimeOverState);

        EventController.inst.RemoveListener(GameEventType.LotteryEvent.LOTTERYRECORD_SHOWUI, showLotteryRecord);
        EventController.inst.RemoveListener<List<CommonRewardData>>(GameEventType.LotteryEvent.LOTTERYREWARD_SHOWUI, showLotteryGetReward);
        EventController.inst.RemoveListener<CommonRewardData>(GameEventType.LotteryEvent.LOTTERYSINGLEREWARD_SHOWUI, showLotterySingleGetReward);

        EventController.inst.RemoveListener<bool>(GameEventType.LotteryEvent.LOTTERY_GETREWARDCOMPLETE, GetRewardComplete);
        EventController.inst.RemoveListener<int>(GameEventType.LotteryEvent.LOTTERY_REFRESH, RefreshLottery);
    }

    void showLotterySingleGetReward(CommonRewardData data)
    {
        GUIManager.OpenView<LotteryGetRewardView>((view) =>
        {
            lotteryRewardView = view;
            view.setSingleRewardInfo(data);
        });
    }

    void showLotteryGetReward(List<CommonRewardData> allItems)
    {
        GUIManager.OpenView<LotteryGetRewardView>((view) =>
        {
            lotteryRewardView = view;
            view.setRewardInfo(allItems);
        });
    }

    void showLotteryRecord()
    {
        GUIManager.OpenView<LotteryRecordView>((view) =>
        {
            lotteryRecordView = view;
        });
    }

    void RequestTimeOverState()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Prize_Free()
        });
    }

    void SetLotteryText()
    {
        if (lotteryUIView != null && lotteryUIView.isShowing)
        {
            lotteryUIView.RefreshBtnText();
        }
        //GUIManager.OpenView<LotteryUIView>((view) =>
        //{
        //    lotteryUIView = view;
        //});
    }

    // 请求单抽
    void RequestSingleLottery(int payType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Lottery()
            {
                money = payType,
            }
        });
    }

    void CumulativeGet(List<ShowPopupData> listData)
    {
        List<CommonRewardData> rewardList = new List<CommonRewardData>();
        for (int i = 0; i < listData.Count; i++)
        {
            int index = i;
            CommonRewardData tempData = new CommonRewardData(listData[index].itemId, listData[index].itemNum, listData[index].itemConfig.property, listData[index].itemConfig.type);
            rewardList.Add(tempData);
        }
        if (rewardList.Count > 1)
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = rewardList });
        else
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, rewardList[0].rewardId, rewardList[0].count));

    }

    void GetRewardComplete(bool isChangeNext)
    {
        if (lotteryUIView != null && lotteryUIView.isShowing)
        {
            lotteryUIView.RefreshCumulativeData(isChangeNext);
        }
    }

    void RequestTenthLottery(int payType)
    {

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_LotteryTenth()
            {
                money = payType,
            }
        });
    }

    void RefreshLottery(int usetype)
    {
        //改为后端主动发 前端播放一下 重置动画
        //仅播放转盘重置动画
        HotfixBridge.inst.TriggerLuaEvent("Lottery_JackpotJumpAnim", false);

        //HotfixBridge.inst.TriggerLuaEvent("WelfareLottery_refresh", 2);
    }
    // 请求奖池数据
    void RequestLotteryData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Jackpot_Data()
        });
    }

    // 请求刷新奖池
    void RequestJackpotRefresh(int payGem, int adtype)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Jackpot_Refresh()
            {
                useType = adtype
            }
        });
    }

    void ResponseSigleLottery(List<JackpotData> data, int type)
    {
        PlatformManager.inst.GameHandleEventLog("LuckyDraw", "");

        HotfixBridge.inst.TriggerLuaEvent("WelfareLottery_getJackpotDatas", data, type);

        if (lotteryUIView != null && lotteryUIView.isShowing)
        {
            lotteryUIView.GetSingleLotteryData(data, type);
        }
    }

    void showLotteryUI()
    {
        GUIManager.OpenView<LotteryUIView>((view) =>
        {
            lotteryUIView = view;
            lotteryUIView.RefreshItemData();
            //EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_REQUESTDATA);
        });
    }

    void RefreshLotteryData()
    {
        if (lotteryUIView != null && lotteryUIView.isShowing)
        {
            lotteryUIView.RefreshItemData();
        }

        //GUIManager.OpenView<LotteryUIView>((view) =>
        //{
        //    lotteryUIView = view;
        //    lotteryUIView.RefreshItemData();
        //});
    }

    void hideLotteryUI()
    {
        GUIManager.HideView<LotteryUIView>();
    }
}
