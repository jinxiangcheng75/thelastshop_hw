using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SevenDayGoalSingle : IComparable<SevenDayGoalSingle>
{
    public int id;
    public int process;
    public int limit;
    public int rewardId1;
    public int rewardId2;
    public int rewardId3;
    public ESevenDayTaskState state;
    public SevenDayTaskConfigData cfg;

    public int CompareTo(SevenDayGoalSingle other)
    {
        if (this.state.CompareTo(other.state) == 0)
        {
            return this.id.CompareTo(other.id);
        }
        else
        {
            if (this.state == ESevenDayTaskState.CanReward)
            {
                return -1;
            }
            else if (other.state == ESevenDayTaskState.CanReward)
            {
                return 1;
            }
            else if (this.state == ESevenDayTaskState.Rewarded && SevenDayGoalDataProxy.inst.SevenDayFlag)
            {
                return -1;
            }
            else if (other.state == ESevenDayTaskState.Rewarded && SevenDayGoalDataProxy.inst.SevenDayFlag)
            {
                return 1;
            }
            else
            {
                return this.state.CompareTo(other.state);
            }
        }
    }

    public void setData(OneDayTask data)
    {
        id = data.id;
        process = data.param;
        limit = data.limit;
        rewardId1 = data.reward1Id;
        rewardId2 = data.reward2Id;
        rewardId3 = data.reward3Id;
        state = (ESevenDayTaskState)data.state;
        cfg = SevenDayTaskConfigManger.inst.GetConfig(id);
    }
}

public class SevenDayGoalData
{
    public int id;
    public int listRewardId;
    public ESevenDayTaskState listState;
    public Dictionary<int, SevenDayGoalSingle> sevenDayDic;
    public SevenDayAwardConfigData cfg;

    public List<SevenDayGoalSingle> sevenDayList
    {
        get
        {
            var list = sevenDayDic.Values.ToList();
            list.Sort((x, y) => x.CompareTo(y));
            return list;
        }
        private set { }
    }

    public int GetCountByState(ESevenDayTaskState state)
    {
        if (sevenDayDic == null)
        {
            return -1;
        }

        int count = sevenDayDic.Values.ToList().FindAll(t => t.state == state).Count;
        return count;
    }

    public SevenDayGoalSingle GetSingleDataById(int id)
    {
        SevenDayGoalSingle tempData = new SevenDayGoalSingle();
        if (sevenDayDic.TryGetValue(id, out tempData))
        {
            return tempData;
        }

        //Logger.error("没有id是" + id + "的七日引导");
        return null;
    }

    public SevenDayGoalData()
    {
        sevenDayDic = new Dictionary<int, SevenDayGoalSingle>();
    }

    public void setData(SevenDayTask data)
    {
        id = data.taskListId;
        listRewardId = data.taskListRewardId;
        listState = (ESevenDayTaskState)data.taskListState;

        for (int i = 0; i < data.taskList.Count; i++)
        {
            int index = i;
            updateSingleData(data.taskList[index]);
        }

        cfg = SevenDayAwardConfigManager.inst.GetConfig(id);
    }

    public void updateSingleData(OneDayTask data)
    {
        if (sevenDayDic.ContainsKey(data.id))
        {
            sevenDayDic[data.id].setData(data);
        }
        else
        {
            SevenDayGoalSingle tempData = new SevenDayGoalSingle();
            tempData.setData(data);
            sevenDayDic.Add(data.id, tempData);
        }
    }
}

public class SevenDayGoalDataProxy : TSingletonHotfix<SevenDayGoalDataProxy>, IDataModelProx
{
    private Dictionary<int, SevenDayGoalData> sevenDayDic;
    int timerId = 0;
    public SevenDayGoalSingle waitSingleData = null;
    public bool isAllOver = false;

    public int curDay;

    public PayProduct sevenDayPassPayProduct
    {
        get;
        private set;
    }

    public bool SevenDayFlag //是否购买了大亨之路通行证
    {
        get;
        private set;
    }

    public bool isShowRedPoint
    {
        get
        {
            if (sevenDayDic == null) return false;
            bool isShow = false;
            foreach (var item in sevenDayDic.Values)
            {
                if (item.listState == ESevenDayTaskState.NotUnlock) continue;
                if (item.listState == ESevenDayTaskState.CanReward && SevenDayFlag)
                {
                    isShow = true;
                    break;
                }
                else
                {
                    foreach (var subItem in item.sevenDayDic.Values)
                    {
                        if (subItem.state == ESevenDayTaskState.CanReward || (subItem.state == ESevenDayTaskState.Rewarded && SevenDayFlag))
                        {
                            isShow = true;
                            break;
                        }
                    }
                }
            }

            return isShow;
        }
    }

    public void Clear()
    {
        if (sevenDayDic != null)
            sevenDayDic.Clear();
        timerId = 0;
        waitSingleData = null;
        isAllOver = false;
        curDay = 0;
    }

    public void Init()
    {
        sevenDayDic = new Dictionary<int, SevenDayGoalData>();

        Helper.AddNetworkRespListener(MsgType.Response_Activity_SevenDayCheck_Cmd, GetSevenDayCheckData);
        Helper.AddNetworkRespListener(MsgType.Response_Activity_SevenDayReward_Cmd, GetSevenDayReward);
        Helper.AddNetworkRespListener(MsgType.Response_Activity_SevenDayListReward_Cmd, GetSevenDayListRewardData);
        Helper.AddNetworkRespListener(MsgType.Response_Activity_SevenDayChange_Cmd, GetSevenDayChange);
        Helper.AddNetworkRespListener(MsgType.Response_Activity_SevenDayListChange_Cmd, GetSevenDayListChange);
    }

    private void GetSevenDayCheckData(HttpMsgRspdBase msg)
    {
        var data = (Response_Activity_SevenDayCheck)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        sevenDayPassPayProduct = data.payProduct;
        this.SevenDayFlag = data.sevenDayFlag == 1;

        foreach (var item in data.sevenDayList)
        {
            UpdateDicData(item);
        }
        CalculateRefreshTime(data.refreshTime);
        checkSevenDayIsAllOver();
        curDay = data.nowDay;
        EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 1);
    }

    private void GetSevenDayReward(HttpMsgRspdBase msg)
    {
        var data = (Response_Activity_SevenDayReward)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        UpdateDicData(data.sevenDayTask);
        //var cfg = SevenDayTaskConfigManger.inst.GetConfig(data.sevenDayTask.id);
        EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 1);


        if (data.rewardItemList.Count > 1)
        {
            List<CommonRewardData> rewardList = new List<CommonRewardData>();
            for (int i = 0; i < data.rewardItemList.Count; i++)
            {
                int index = i;
                CommonRewardData tempData = new CommonRewardData(data.rewardItemList[index].itemId, data.rewardItemList[index].count, data.rewardItemList[index].quality, data.rewardItemList[index].itemType);
                rewardList.Add(tempData);
            }
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = rewardList });
        }
        else
        {
            if (data.rewardItemList.Count > 0)
            {
                queueItem queueData = new queueItem(ReceiveInfoUIType.GetItem, "", 0, 0, data.rewardItemList[0].count);
                if (data.rewardItemList[0].itemType == 26)
                {
                    queueData.equipid = data.rewardItemList[0].itemId;
                }
                else
                {
                    queueData.itemid = data.rewardItemList[0].itemId;
                }
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, queueData);
            }
            else
            {
                Logger.error("没有给七日任务的奖励数据");
            }
        }
        checkSevenDayIsAllOver();
    }

    private void GetSevenDayListRewardData(HttpMsgRspdBase msg)
    {
        var data = (Response_Activity_SevenDayListReward)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        PlatformManager.inst.GameHandleEventLog("Collect_7Day", "");
        UpdateDicData(data.sevenDayInfo);
        //var cfg = SevenDayAwardConfigManager.inst.GetConfig(data.sevenDayInfo.taskListId);
        EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 1);

        List<CommonRewardData> rewardList = new List<CommonRewardData>();
        for (int i = 0; i < data.rewardItemList.Count; i++)
        {
            int index = i;
            CommonRewardData tempData = new CommonRewardData(data.rewardItemList[index].itemId, data.rewardItemList[index].count, data.rewardItemList[index].quality, data.rewardItemList[index].itemType);
            rewardList.Add(tempData);
        }
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = rewardList });
        checkSevenDayIsAllOver();
    }

    private void GetSevenDayChange(HttpMsgRspdBase msg)
    {
        var data = (Response_Activity_SevenDayChange)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        this.SevenDayFlag = data.sevenDayFlag == 1;

        if (SevenDayFlag)
        {
            HotfixBridge.inst.TriggerLuaEvent("HideUI_SevenDayBuyPassUI");
        }

        for (int i = 0; i < data.sevenDayTaskList.Count; i++)
        {
            int index = i;

            if (index == data.sevenDayTaskList.Count - 1)
            {
                var taskCfg = SevenDayTaskConfigManger.inst.GetConfig(data.sevenDayTaskList[index].id);
                var allSevenData = GetDataByDayIndex(taskCfg.day);
                if (allSevenData == null) continue;
                //Logger.error("singleData = " + taskCfg);
                var singleData = allSevenData.GetSingleDataById(taskCfg.id);
                if (singleData == null) continue;
                bool isArrive = singleData.process >= singleData.limit * 0.5f;
                UpdateDicData(data.sevenDayTaskList[index]);
                if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver && allSevenData.listState != ESevenDayTaskState.NotUnlock)
                {
                    var cfg = SevenDayTaskConfigManger.inst.GetConfig(data.sevenDayTaskList[index].id);
                    var _data = GetDataByDayIndex(cfg.day);
                    if (_data == null) continue;
                    singleData = _data.GetSingleDataById(cfg.id);
                    if (singleData == null) continue;
                    if (!isArrive)
                    {
                        isArrive = singleData.process >= singleData.limit * 0.5f;
                    }
                    else
                    {
                        isArrive = false;
                    }
                    if (data.sevenDayTaskList[index].state == 2 || isArrive)
                    {
                        if (GUIManager.CurrWindow != null)
                        {
                            var curWindow = GUIManager.CurrWindow;
                            if (curWindow != null && curWindow.viewID != ViewPrefabName.MainUI)
                                waitSingleData = singleData;
                            else
                            {
                                if (UserDataProxy.inst.playerData.level >= WorldParConfigManager.inst.GetConfig(135).parameters && UserDataProxy.inst.currMainTaskGroup >= 9999)
                                    EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.SEVENDAY_CONTENTCHANGE, singleData);
                            }
                        }
                    }
                }
            }
            else
            {
                UpdateDicData(data.sevenDayTaskList[index]);
            }
        }
        EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 1);
        EventController.inst.TriggerEvent(GameEventType.REFRESHMAINUIREDPOINT);
    }

    private void GetSevenDayListChange(HttpMsgRspdBase msg)
    {
        var data = (Response_Activity_SevenDayListChange)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        UpdateDicData(data.sevenDayTaskList);
        EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 1);
    }

    private void UpdateDicData(SevenDayTask _data)
    {
        var cfg = SevenDayAwardConfigManager.inst.GetConfig(_data.taskListId);
        if (cfg == null)
        {
            return;
        }
        if (sevenDayDic.ContainsKey(cfg.day))
        {
            sevenDayDic[cfg.day].setData(_data);
        }
        else
        {
            SevenDayGoalData data = new SevenDayGoalData();
            data.setData(_data);
            sevenDayDic.Add(cfg.day, data);
        }
    }

    private void checkSevenDayIsAllOver()
    {
        bool allOver = true;
        foreach (var item in sevenDayDic.Values)
        {
            if (/*item.listState != ESevenDayTaskState.Rewarded && */item.listState != ESevenDayTaskState.VIPRewarded)
            {
                allOver = false;
                break;
            }
        }

        if (allOver)
        {
            isAllOver = true;
            PlayerPrefs.SetInt(UserDataProxy.inst.playerData.userUid + "SevenDayState", 1);
        }
    }

    private void UpdateDicData(OneDayTask _data)
    {
        var cfg = SevenDayTaskConfigManger.inst.GetConfig(_data.id);
        if (cfg == null)
        {
            return;
        }
        if (sevenDayDic.ContainsKey(cfg.day))
        {
            sevenDayDic[cfg.day].updateSingleData(_data);
        }
        else
        {
            SevenDayGoalData data = new SevenDayGoalData();
            data.updateSingleData(_data);
            sevenDayDic.Add(cfg.day, data);
        }
    }

    private void CalculateRefreshTime(int refreshTime)
    {
        if (refreshTime == 99999) return;
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        timerId = GameTimer.inst.AddTimer(refreshTime, 1, () =>
          {
              //refreshTime -= 1;
              //refreshTime = Mathf.Clamp(refreshTime, 0, refreshTime);

              //if (refreshTime <= 0)
              //{
              EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYCHECK);
              GameTimer.inst.RemoveTimer(timerId);
              timerId = 0;
              //}
          });
    }

    public SevenDayGoalData GetDataByDayIndex(int day)
    {
        if (sevenDayDic == null || sevenDayDic.Count <= 0)
        {
            Logger.error("七日目标的数据列表是空的");
            return null;
        }

        if (sevenDayDic.ContainsKey(day))
            return sevenDayDic[day];

        return null;
    }

    public string setTaskDescByType(SevenDayGoalSingle data)
    {
        string str = "";
        switch ((K_SevenDay_Type)data.cfg.type)
        {
            case K_SevenDay_Type.MakeMoney:
            case K_SevenDay_Type.UnlockEquip:
            case K_SevenDay_Type.StoreExpasion:
            case K_SevenDay_Type.TechnologyUpgrading:
            case K_SevenDay_Type.BuildUpgrading:
            case K_SevenDay_Type.HeroRecruit:
            case K_SevenDay_Type.HeroTransfer:
            case K_SevenDay_Type.AddMakeSlot:
            case K_SevenDay_Type.LevelUp:
            case K_SevenDay_Type.OpenTreasureBoxCount:
            case K_SevenDay_Type.PrestigePromotion:
            case K_SevenDay_Type.MarketTransactions:
            case K_SevenDay_Type.EnergyUp:
            case K_SevenDay_Type.ExploreCount:
            case K_SevenDay_Type.SellEquip:
            case K_SevenDay_Type.MarkUpSale:
            case K_SevenDay_Type.EquipmentExchange:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, AbbreviationUtility.AbbreviateNumber(data.cfg.parameter_1[0], 2));
                break;
            case K_SevenDay_Type.EquipmentMastery:
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(equipCfg.name));
                break;
            case K_SevenDay_Type.MakeTargetEquip:
                var equipClassficationCfg = EquipConfigManager.inst.GetEquipTypeByID(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(equipClassficationCfg.name), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.ReceiveDrawing:
                var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(equipDrawingCfg.name));
                break;
            case K_SevenDay_Type.UnlockTargetLvEquip:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, data.cfg.parameter_1[0].ToString(), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.BuyFurniture:
                var furnitureCfg = FurnitureConfigManager.inst.getConfig(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(furnitureCfg.name), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.FurnitureLvUp:
                var furnitureLvUpCfg = FurnitureConfigManager.inst.getConfig(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(furnitureLvUpCfg.name), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.Union:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des);
                break;
            case K_SevenDay_Type.BuildTargetLv:
                var buildCfg = BuildingConfigManager.inst.GetConfig(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(buildCfg.name), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.HeroRarity:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des);
                break;
            case K_SevenDay_Type.HeroWearEquipLv:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, data.cfg.parameter_1[0].ToString(), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.ExploreUpgrading:
                var exploreCfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(data.cfg.parameter_1[0]);
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, LanguageManager.inst.GetValueByKey(exploreCfg.instance_name), data.cfg.parameter_2[0].ToString());
                break;
            case K_SevenDay_Type.ExploreChallenges:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des);
                break;
            case K_SevenDay_Type.HeroLevelUp:
                str = LanguageManager.inst.GetValueByKey(data.cfg.type_des, data.cfg.parameter_1[0].ToString(), data.cfg.parameter_2[0].ToString());
                break;
        }

        return str;
    }
}
