using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class AcheivementData : IComparable<AcheivementData>
{
    public int id;
    public string name;
    public string desc;
    public int group;
    public EAchievementState state;
    public long process;
    public long limit;
    public itemConfig itemCfg;
    public int rewardNum;
    public int rewardPoints;
    public string atlas;
    public string icon;
    public int condition_type;
    public string condition_des;

    public int CompareTo(AcheivementData other)
    {
        if (this.state.CompareTo(other.state) == 0)
        {
            return this.id.CompareTo(other.id);
        }
        else
        {
            if (this.state == EAchievementState.Done)
            {
                return -1;
            }
            else if (other.state == EAchievementState.Done)
            {
                return 1;
            }
            else
            {
                return this.state.CompareTo(other.state);
            }
        }
    }

    public void setData(OneAchievement data)
    {
        id = data.id;
        var acheivementCfg = AcheivementConfigManager.inst.GetConfig(id);
        name = acheivementCfg.name;
        desc = acheivementCfg.desc;
        group = acheivementCfg.group;
        state = (EAchievementState)data.states;
        process = data.process;
        limit = (EAchievementType)acheivementCfg.group != EAchievementType.GachaRankHero ? acheivementCfg.condition_num : 1;
        itemCfg = ItemconfigManager.inst.GetConfig(acheivementCfg.reward_type);
        rewardNum = acheivementCfg.reward_num;
        rewardPoints = acheivementCfg.reward_points;
        atlas = acheivementCfg.atlas;
        icon = acheivementCfg.icon;
        condition_type = acheivementCfg.condition_type;
        condition_des = acheivementCfg.condition_des;
    }
}

public class AcheivementRoadData
{
    public int id;
    public int index;
    public EAchievementRoadRewardState state;

    public void setData(OneAchievementRoad data)
    {
        id = data.achievementRoadId;
        var cfg = AcheivementRoadConfigManager.inst.GetConfig(data.achievementRoadId);
        if (cfg != null)
        {
            index = cfg.index;
        }
        state = (EAchievementRoadRewardState)data.achievementRoadState;
    }
}

public class AcheivementDataProxy : TSingletonHotfix<AcheivementDataProxy>, IDataModelProx
{
    private Dictionary<int, Dictionary<int, AcheivementData>> allAcheivementDic;
    private Dictionary<int, AcheivementData> acheivementDic;
    private Dictionary<int, AcheivementRoadData> acheivementRoadDic;
    private int achievementRoadId;
    private EAchievementRoadState achievementRoadState;
    private int achievementRoadPoint;
    private int achievementroadPointLimit;

    public bool isWaitExploreEndPanel;
    public int waitAcheivementId;
    public bool NeedRedPoint
    {
        get
        {
            bool needRedPoint = false;
            if (acheivementList == null) return false;
            if (acheivementRoadList == null) return false;
            if (acheivementList.FindIndex(t => t.state == EAchievementState.Done) != -1)
            {
                needRedPoint = true;
            }
            if (acheivementRoadList.FindIndex(t => t.state == EAchievementRoadRewardState.CanReward) != -1)
            {
                needRedPoint = true;
            }
            return needRedPoint;
        }
    }
    public int AcheivementRoadId
    {
        get { return achievementRoadId; }
        private set { }
    }

    public int AchievementRoadPoint
    {
        get { return achievementRoadPoint; }
        private set { }
    }
    public int AchievementroadPointLimit
    {
        get { return achievementroadPointLimit; }
        private set { }
    }
    //public List<AcheivementData> acheivementList
    //{
    //    get
    //    {
    //        var list = acheivementDic.Values.ToList().FindAll(t => t.state != EAchievementState.NotDoable);
    //        list.Sort((x, y) => x.CompareTo(y));
    //        return list;
    //    }
    //    private set { }
    //}

    public List<AcheivementData> acheivementList
    {
        get
        {
            List<AcheivementData> list = new List<AcheivementData>();
            foreach (var item in allAcheivementDic.Values)
            {
                bool isFirst = true;
                foreach (var smallData in item.Values)
                {
                    if (smallData.state == EAchievementState.Done || smallData.state == EAchievementState.Rewarded)
                    {
                        list.Add(smallData);
                    }
                    else if (smallData.state == EAchievementState.Doing)
                    {
                        if (isFirst)
                        {
                            list.Add(smallData);
                            isFirst = false;
                        }
                    }
                }
            }
            list.Sort((x, y) => x.CompareTo(y));
            return list;
        }
        private set { }
    }

    public List<AcheivementRoadData> acheivementRoadList
    {
        get { return acheivementRoadDic.Values.ToList(); }
        private set { }
    }

    public void Clear()
    {
        if (acheivementDic != null)
            acheivementDic.Clear();
        if (acheivementRoadDic != null)
            acheivementRoadDic.Clear();
        if (acheivementRoadDic != null)
            acheivementRoadDic.Clear();
    }

    public void Init()
    {
        allAcheivementDic = new Dictionary<int, Dictionary<int, AcheivementData>>();
        acheivementDic = new Dictionary<int, AcheivementData>();
        acheivementRoadDic = new Dictionary<int, AcheivementRoadData>();

        Helper.AddNetworkRespListener(MsgType.Response_Achievement_Check_Cmd, GetAcheivementData);
        Helper.AddNetworkRespListener(MsgType.Response_Achievement_Reward_Cmd, GetAcheivementRewardData);
        Helper.AddNetworkRespListener(MsgType.Response_AchievementRoad_Reward_Cmd, GetAcheivementRoadAwardData);
        //Helper.AddNetworkRespListener(MsgType.Response_Achievement_Done_Cmd, GetAcheivementDoneData);
        Helper.AddNetworkRespListener(MsgType.Response_Achievement_Change_Cmd, GetAcheivementChangeData);
    }

    public AcheivementData GetAcheivementDataById(int id)
    {
        foreach (var item in allAcheivementDic)
        {
            if (item.Value.ContainsKey(id))
            {
                return item.Value[id];
            }
        }

        return null;
    }

    void GetAcheivementData(HttpMsgRspdBase msg)
    {
        var data = (Response_Achievement_Check)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        UpdateAllData(data.achievementRoadId, data.achievementRoadState, data.achievementRoadPoint, data.achievementList, data.achievementRoadList);
        JudgeShopkeeper();
    }

    void GetAcheivementRewardData(HttpMsgRspdBase msg)
    {
        var data = (Response_Achievement_Reward)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        UpdateAllData(data.achievementRoadId, data.achievementRoadState, data.achievementRoadPoint, data.reward, data.achievementRoadList);
        EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.ACHEIVEMENTSETDATA);

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
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, data.rewardItemList[0].itemId, data.rewardItemList[0].count));
        }
        //EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONREWARD_SETINFO, rewardList);
        JudgeShopkeeper();

        //判断是否需要弹出五星评价
        WorldParConfig wcfg = WorldParConfigManager.inst.GetConfig(8103);
        if (wcfg != null)
        {
            var id = (int)wcfg.parameters;
            //Logger.error("worldpar配置弹出评价的成就id为：" + wcfg.parameters + "   当前完成的成就id为（后端下发）：" + data.achievementId);
            if (data.achievementId == id)
            {
                //直接弹出五星好评
                PlatformManager.inst.show5star();
                //Logger.error("成就id一致，调用评价接口！！！");
            }
        }
    }

    void GetAcheivementRoadAwardData(HttpMsgRspdBase msg)
    {
        var data = (Response_AchievementRoad_Reward)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        UpdateAllData(data.achievementRoadId, data.achievementRoadState, data.achievementRoadPoint, acheivementRoadList: data.achievementRoadList);
        EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.ACHEIVEMENTSETDATA);
        List<CommonRewardData> rewardList = new List<CommonRewardData>();
        for (int i = 0; i < data.rewardItemList.Count; i++)
        {
            int index = i;
            CommonRewardData tempData = new CommonRewardData(data.rewardItemList[index].itemId, data.rewardItemList[index].count, data.rewardItemList[index].quality, data.rewardItemList[index].itemType);
            rewardList.Add(tempData);
        }
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = rewardList });
        //EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONREWARD_SETINFO, rewardList);
        JudgeShopkeeper();

        //GameTimer.inst.AddTimer(1, 1, () =>
        //{
        //    PlatformManager.inst.show5star(2);
        //});
    }

    //void GetAcheivementDoneData(HttpMsgRspdBase msg)
    //{
    //    var data = (Response_Achievement_Done)msg;
    //    UpdateAcheivementData(data.achievement);
    //    EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONEUI, data.achievement.id);
    //    JudgeShopkeeper();
    //}

    void GetAcheivementChangeData(HttpMsgRspdBase msg)
    {
        var data = (Response_Achievement_Change)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        foreach (var item in data.achievementChangeList)
        {
            UpdateAcheivementData(item.achievement);
            if ((EAchievementChangeType)item.type == EAchievementChangeType.Change)
            {
                EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.ACHEIVEMENTSETDATA);
            }
            else if ((EAchievementChangeType)item.type == EAchievementChangeType.Done)
            {
                EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.ACHEIVEMENTSETDATA);
                var cfg = AcheivementConfigManager.inst.GetConfig(item.achievement.id);
                if (cfg == null)
                {
                    Logger.error("没有成就id是" + item.achievement.id + "的数据");
                    return;
                }
                if (cfg.if_special == 1)
                {
                    //EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONESPECIALUI, item.achievement.id);
                    if (ManagerBinder.inst.mGameState == kGameState.Shop)
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutVal { type = ReceiveInfoUIType.SpecialAchievement, val = item.achievement.id });
                }
                else if (cfg.if_special == 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONEUI, item.achievement.id);
                }
                JudgeShopkeeper();
            }
        }
    }

    private void UpdateAllData(int achievementRoadId, int achievementRoadState, int achievementRoadPoint, List<OneAchievement> acheivementList = null, List<OneAchievementRoad> acheivementRoadList = null)
    {
        this.achievementRoadId = achievementRoadId;
        this.achievementRoadState = (EAchievementRoadState)achievementRoadState;
        this.achievementRoadPoint = achievementRoadPoint;

        if (acheivementList != null)
        {
            foreach (var item in acheivementList)
            {
                UpdateAcheivementData(item);
            }
        }

        if (acheivementRoadList != null)
        {
            foreach (var item in acheivementRoadList)
            {
                UpdateAcheivementRoadData(item);
            }
        }
    }

    private void UpdateAcheivementData(OneAchievement _data)
    {
        var cfg = AcheivementConfigManager.inst.GetConfig(_data.id);
        if (cfg == null)
        {
            Logger.error("没有成就id是" + _data.id + "的数据");
            return;
        }
        if (!allAcheivementDic.ContainsKey(cfg.group))
        {
            allAcheivementDic.Add(cfg.group, new Dictionary<int, AcheivementData>());
        }

        if (allAcheivementDic.ContainsKey(cfg.group))
        {
            var subData = allAcheivementDic[cfg.group];
            if (subData != null)
            {
                if (subData.ContainsKey(cfg.id))
                {
                    subData[cfg.id].setData(_data);
                }
                else
                {
                    AcheivementData data = new AcheivementData();
                    data.setData(_data);
                    subData.Add(cfg.id, data);
                }
            }
            else
            {
                Logger.error("没有组id是" + cfg.group + "的数据");
            }
        }

        if (acheivementDic.ContainsKey(cfg.group))
        {
            acheivementDic[cfg.group].setData(_data);
        }
        else
        {
            AcheivementData data = new AcheivementData();
            data.setData(_data);
            acheivementDic.Add(data.group, data);
        }
    }

    private void UpdateAcheivementRoadData(OneAchievementRoad _data)
    {
        if (acheivementRoadDic.ContainsKey(_data.achievementRoadId))
        {
            acheivementRoadDic[_data.achievementRoadId].setData(_data);
        }
        else
        {
            AcheivementRoadData data = new AcheivementRoadData();
            data.setData(_data);
            acheivementRoadDic.Add(data.id, data);
        }
    }

    public AcheivementRoadData GetAcheivementRoadDataById(int id)
    {
        if (acheivementRoadDic.ContainsKey(id))
        {
            return acheivementRoadDic[id];
        }

        //Logger.error("没有id是" + id + "的成就之路数据");
        return null;
    }

    private void JudgeShopkeeper()
    {
        if (ManagerBinder.inst.mGameState != kGameState.Shop) return;

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null)
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver) return;
        }

        //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.SHOPKEEPER_ACHEIVEMENTREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("SHOPKEEPER_ACHEIVEMENTREFRESH");
    }
}
