using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LotteryData
{
    public int type;
    public int rarity;
    public string name;
    public int itemId;
    public int itemNum;
    public int prizeState;
    public int index;
    public itemConfig itemConfig;

    public void setData(JackpotData data)
    {
        type = data.type;
        rarity = data.rarity;
        name = data.name;
        itemId = data.prizeId;
        itemNum = data.prizeCount;
        prizeState = data.prizeState;
        index = data.boxNum;
        itemConfig = ItemconfigManager.inst.GetConfig(itemId);
    }
}

public class CumulativeRewardData
{
    public int uid;
    public int sequence;
    public int cumulative_times;
    public int reward_type1;
    public string reward_name1;
    public int reward_item_id1;
    public int reward_item_num1;
    public int reward_type2;
    public string reward_name2;
    public int reward_item_id2;
    public int reward_item_num2;
    public int reward_type3;
    public string reward_name3;
    public int reward_item_id3;
    public int reward_item_num3;

    public CumulativeRewardData(int uid)
    {
        this.uid = uid;
    }

    public CumulativeRewardData() { }
}

public class ShowPopupData
{
    public int itemId;
    public int itemNum;
    public itemConfig itemConfig;

    public ShowPopupData(int itemId, int itemNum)
    {
        this.itemId = itemId;
        this.itemNum = itemNum;
        this.itemConfig = ItemconfigManager.inst.GetConfig(itemId);
    }
}

public class LotteryDataProxy : TSingletonHotfix<LotteryDataProxy>, IDataModelProx
{
    private Dictionary<int, LotteryData> jackpotDic;
    public CumulativeRewardData curCumulativeData;
    private List<Recording> worldRecords;
    private List<Recording> selfRecords;
    private int lotteryCount;
    public int prizePoolState;
    public int singleFreeState;
    public int prizeCoolTime;
    public int singleFreeTime;
    public int gemConsume;
    public int waitWorkerId = 0;
    int poolTimerId = 0;
    int singleTimerId = 0;
    public List<ShowPopupData> tempList = new List<ShowPopupData>();

    public bool singleIsNew;
    public bool prizeIsNew;

    int prizeEndTime = 0;
    int singleEndTime = 0;
    public int LotteryCount
    {
        get
        {
            return lotteryCount;
        }
    }

    public bool hasSingleRedPoint
    {
        get
        {
            if (singleFreeState == 1 && singleIsNew)
                return true;
            return false;
        }
    }

    public bool hasRefreshRedPoint
    {
        get
        {
            if (prizePoolState != (int)EPrizePoolState.hasBeenReset && prizeIsNew)
                return true;

            return false;
        }
    }

    public void Clear()
    {
        if (jackpotDic != null) jackpotDic.Clear();
        jackpotDic = null;
        curCumulativeData = null;
        if (worldRecords != null) worldRecords.Clear();
        worldRecords = null;
        if (selfRecords != null) selfRecords.Clear();
        selfRecords = null;
    }

    public void Init()
    {
        jackpotDic = new Dictionary<int, LotteryData>();
        curCumulativeData = new CumulativeRewardData();
        worldRecords = new List<Recording>();
        selfRecords = new List<Recording>();
        Helper.AddNetworkRespListener(MsgType.Response_Jackpot_Data_Cmd, GetJackpotData);
        Helper.AddNetworkRespListener(MsgType.Response_Jackpot_Refresh_Cmd, JackDataRefresh);
        Helper.AddNetworkRespListener(MsgType.Response_User_Lottery_Cmd, GetSingleLottery);
        Helper.AddNetworkRespListener(MsgType.Response_User_LotteryTenth_Cmd, GetLenthLottery);
        Helper.AddNetworkRespListener(MsgType.Response_Roulette_Recording_Cmd, GetRecordingData);
        Helper.AddNetworkRespListener(MsgType.Response_Cumulative_Reward_Cmd, GetCumulativeRewardData);
        Helper.AddNetworkRespListener(MsgType.Response_Prize_Free_Cmd, SetLotteryText);
    }

    void GetJackpotData(HttpMsgRspdBase msg)
    {
        //获取数据
        var data = (Response_Jackpot_Data)msg;
        if (data.prizeBaseData.Count <= 0) return;
        prizePoolState = data.prizePoolStatus;
        singleFreeState = data.freeState;
        prizeCoolTime = data.zeroTime;
        prizeEndTime = data.zeroTime + (int)GameTimer.inst.serverNow;
        singleFreeTime = data.freeTime;
        singleEndTime = data.freeTime + (int)GameTimer.inst.serverNow;
        gemConsume = data.gemConsume;
        lotteryCount = data.grandTotalNum;
        InitExhibitionData(data.recording, true);
        GetCumulativeData(data.grandTotal);
        for (int i = 0; i < data.prizeBaseData.Count; i++)
        {
            int index = i;
            AddLotteryData(data.prizeBaseData[index]);
        }
        calculateTime();

        if (singleFreeState == 1)
        {
            singleIsNew = true;
        }
        if (prizePoolState != (int)EPrizePoolState.hasBeenReset)
        {
            prizeIsNew = true;
        }
        //EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_DATA);
    }

    void calculateTime()
    {
        // if (prizePoolState != 1)
        {
            if (poolTimerId > 0)
            {
                GameTimer.inst.RemoveTimer(poolTimerId);
                poolTimerId = 0;
            }
            if (prizeEndTime <= GameTimer.inst.serverNow) return;
            poolTimerId = GameTimer.inst.AddTimer(1, () =>
             {
                 prizeCoolTime = prizeEndTime - (int)GameTimer.inst.serverNow;
                 if (prizeCoolTime <= 0)
                 {
                     EventController.inst.TriggerEvent(GameEventType.LotteryEvent.TIMEIS_OK);
                     GameTimer.inst.RemoveTimer(poolTimerId);
                     poolTimerId = 0;
                 }
             });
        }

        if (singleFreeState != 1)
        {
            if (singleTimerId > 0)
            {
                GameTimer.inst.RemoveTimer(singleTimerId);
                singleTimerId = 0;
            }

            if (singleEndTime <= GameTimer.inst.serverNow) return;

            singleTimerId = GameTimer.inst.AddTimer(1, () =>
            {
                singleFreeTime = singleEndTime - (int)GameTimer.inst.serverNow;
                if (singleFreeTime <= 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.LotteryEvent.TIMEIS_OK);
                    GameTimer.inst.RemoveTimer(singleTimerId);
                    singleTimerId = 0;
                }
            });
        }
    }

    private void AddLotteryData(JackpotData data)
    {
        if (jackpotDic.ContainsKey(data.boxNum))
        {
            jackpotDic[data.boxNum].setData(data);
        }
        else
        {
            LotteryData tempData = new LotteryData();
            tempData.setData(data);
            jackpotDic.Add(tempData.index, tempData);
        }
    }
    // ???????????
    void SetLotteryText(HttpMsgRspdBase msg)
    {
        var data = (Response_Prize_Free)msg;
        prizePoolState = data.prizePoolStatus;
        singleFreeState = data.freeState;
        prizeCoolTime = data.zeroTime;
        //if (prizePoolState == 1) gemConsume = 0;
        EventController.inst.TriggerEvent(GameEventType.LotteryEvent.FREELOTERRY_OK);
        HotfixBridge.inst.TriggerLuaEvent("WelfareLottery_refreshBtnTx");
        calculateTime();

        if (singleFreeState == 1)
        {
            singleIsNew = true;
        }
        if (prizePoolState != (int)EPrizePoolState.hasBeenReset)
        {
            prizeIsNew = true;
        }
    }

    void GetRecordingData(HttpMsgRspdBase msg)
    {
        var data = (Response_Roulette_Recording)msg;
        InitExhibitionData(data.recording, false);
    }

    void InitExhibitionData(List<Recording> listData, bool resetList)
    {
        if (resetList)
        {
            worldRecords = listData.FindAll(t => (kLotteryType)t.type == kLotteryType.World);
            selfRecords = listData.FindAll(t => (kLotteryType)t.type == kLotteryType.Myself);
        }
        else
        {
            for (int i = 0; i < listData.Count; i++)
            {
                int index = i;
                if ((kLotteryType)listData[index].type == kLotteryType.World)
                {
                    worldRecords.Add(listData[index]);
                }
                else
                {
                    selfRecords.Add(listData[index]);
                }
            }
        }

        if (worldRecords.Count > WorldParConfigManager.inst.GetConfig(114).parameters)
        {
            worldRecords.RemoveRange(0, (int)WorldParConfigManager.inst.GetConfig(114).parameters);
        }

        if (selfRecords.Count > WorldParConfigManager.inst.GetConfig(115).parameters)
        {
            selfRecords.RemoveRange(0, (int)WorldParConfigManager.inst.GetConfig(115).parameters);
        }
    }

    public List<Recording> GetRecordingByType(kLotteryType type)
    {
        List<Recording> tempList = new List<Recording>();
        switch (type)
        {
            case kLotteryType.World:
                tempList = worldRecords;
                break;
            case kLotteryType.Myself:
                tempList = selfRecords;
                break;
            default:
                break;
        }
        return tempList;
    }

    void GetCumulativeRewardData(HttpMsgRspdBase msg)
    {
        var data = (Response_Cumulative_Reward)msg;
        lotteryCount = data.grandTotalNum;
        CumulativeRewardData getData = GetCumulativeData(data.grandTotal);
        GetCumulativeData(data.nextGrandTotal);
        tempList.Clear();
        tempList.Add(new ShowPopupData(getData.reward_item_id1, getData.reward_item_num1));
        if (getData.reward_item_id2 != -1)
            tempList.Add(new ShowPopupData(getData.reward_item_id2, getData.reward_item_num2));
        if (getData.reward_item_id3 != -1)
            tempList.Add(new ShowPopupData(getData.reward_item_id3, getData.reward_item_num3));
        //EventController.inst.TriggerEvent(GameEventType.LotteryEvent.CUMULATIVE_GET, tempList);
    }

    void JackDataRefresh(HttpMsgRspdBase msg)
    {
        //jackpotDic.Clear();
        var data = (Response_Jackpot_Refresh)msg;
        prizeCoolTime = data.JackpotDataRefresh.zeroTime;
        gemConsume = data.JackpotDataRefresh.gemConsume;
        prizePoolState = data.JackpotDataRefresh.prizePoolStatus;

        for (int i = 0; i < data.prizeBaseData.Count; i++)
        {
            int index = i;
            AddLotteryData(data.prizeBaseData[index]);
        }

        if (prizePoolState != (int)EPrizePoolState.hasBeenReset)
        {
            prizeIsNew = true;
        }

        EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_DATA);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent",2);
        calculateTime();
    }

    public LotteryData GetCorrespodingBoxNumData(int boxNum)
    {
        if (jackpotDic.Count <= 0) return null;

        if (jackpotDic.ContainsKey(boxNum))
        {
            return jackpotDic[boxNum];
        }

        return null;
    }

    void GetSingleLottery(HttpMsgRspdBase msg)
    {
        var data = (Response_User_Lottery)msg;
        lotteryCount = data.prizedPropsCount;
        singleFreeState = data.freeState;
        singleFreeTime = data.freeTime;
        SetLotteryJackpotData(data.prizeBaseData, data.grandTotal);
        if (data.prizeBaseData[0].type == (int)ItemType.Craftsman)
        {
            waitWorkerId = data.prizeBaseData[0].prizeId;
        }
        EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY_COM, data.prizeBaseData, 0);

        calculateTime();
    }

    void GetLenthLottery(HttpMsgRspdBase msg)
    {
        var data = (Response_User_LotteryTenth)msg;
        lotteryCount = data.prizedPropsCount;
        SetLotteryJackpotData(data.prizeBaseData, data.grandTotal);
        var workerData = data.prizeBaseData.Find(t => t.type == (int)ItemType.Craftsman);
        if (workerData != null)
            waitWorkerId = workerData.prizeId;
        EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY_COM, data.prizeBaseData, 1);
    }

    void SetLotteryJackpotData(List<JackpotData> listData, GrandTotal grandTotal)
    {
        for (int i = 0; i < listData.Count; i++)
        {
            int index = i;
            JackpotData tempData = listData[index];
            AddLotteryData(tempData);
        }
        GetCumulativeData(grandTotal);
    }

    public List<LotteryData> GetLotteryDatas()
    {
        if (jackpotDic == null || jackpotDic.Count <= 0) return null;
        return jackpotDic.Values.ToList();
    }

    CumulativeRewardData GetCumulativeData(GrandTotal curData)
    {
        if (curData == null) return null;

        curCumulativeData = new CumulativeRewardData(curData.id);
        curCumulativeData.reward_item_id1 = curData.rewardItemId1;
        curCumulativeData.reward_item_id2 = curData.rewardItemId2;
        curCumulativeData.reward_item_id3 = curData.rewardItemId3;
        curCumulativeData.reward_item_num1 = curData.rewardItemNum1;
        curCumulativeData.reward_item_num2 = curData.rewardItemNum2;
        curCumulativeData.reward_item_num3 = curData.rewardItemNum3;
        curCumulativeData.reward_type1 = curData.rewardType1;
        curCumulativeData.reward_type2 = curData.rewardType2;
        curCumulativeData.reward_type3 = curData.rewardType3;
        curCumulativeData.reward_name1 = curData.rewardName1;
        curCumulativeData.reward_name2 = curData.rewardName2;
        curCumulativeData.reward_name3 = curData.rewardName3;
        curCumulativeData.cumulative_times = curData.cumulativeTimes;
        curCumulativeData.sequence = curData.rewardSequence;
        return curCumulativeData;
    }
}
