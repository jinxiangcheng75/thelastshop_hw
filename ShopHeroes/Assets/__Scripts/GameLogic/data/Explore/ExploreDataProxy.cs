using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;

public class ExploreGroup
{
    public ExploreGroupData groupData;
    public float restTimePercent = 1;
    public float exploreTimePercent = 1;
    public float ExploreTimePercent
    {
        get
        {
            float tempPercent = exploreTimePercent;
            var buff = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.explore_speedUp);
            if (buff != null)
            {
                tempPercent = tempPercent * (1 - buff.buffInfo.buffParam / 100.0f);
            }
            var data = UserDataProxy.inst.GetUnionBuffData(EUnionScienceType.ExploreSkill);
            if (data != null)
            {
                tempPercent = tempPercent * (1 - data.config.add2_num / 100.0f);
            }
            return tempPercent;
        }
        private set { }
    }
    public float addExpPercent = 1;
    public float AddExpPercent
    {
        get
        {
            float tempPercent = addExpPercent;
            var buff = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.explore_heroExpUp);
            if (buff != null)
            {
                tempPercent = tempPercent * (1 + buff.buffInfo.buffParam / 100.0f);
            }
            var data = UserDataProxy.inst.GetUnionBuffData(EUnionScienceType.ExpSkill);
            if (data != null)
            {
                tempPercent = tempPercent * (1 + data.config.add2_num / 100.0f);
            }
            return tempPercent;
        }
        private set { }
    }
    public int[] dropCount = new int[3];
    public int[] dfficLevel = new int[3];
    public int difficult = 1; // 1 - 简单 2 - 正常 3 - 困难
    public int unlockNextExploreLevel = 0;
    public bool nextIsUnlock;
    public List<ExploreItemData> explores = new List<ExploreItemData>();
    int timerId;
    int bossEndTime = 0;
    public void setData(ExploreGroupData data)
    {
        groupData = new ExploreGroupData();
        groupData.groupId = data.groupId;
        groupData.level = data.level;
        groupData.exp = data.exp;
        groupData.bossExploreState = data.bossExploreState;
        groupData.bossRemainTime = data.bossRemainTime;
        bossEndTime = data.bossRemainTime + (int)GameTimer.inst.serverNow;
        groupData.groupState = data.groupState;
        ReInit();
    }

    private void ReInit()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        calculateTime();
    }

    private void calculateTime()
    {
        if (explores[explores.Count - 1].type == 4 && explores[explores.Count - 1].unlockState == 1)
        {
            if (timerId == 0 && groupData.bossExploreState != 1 && groupData.bossExploreState != 4)
            {
                timerId = GameTimer.inst.AddTimer(1, () =>
                {
                    groupData.bossRemainTime = bossEndTime - (int)GameTimer.inst.serverNow;
                    groupData.bossRemainTime = Mathf.Clamp(groupData.bossRemainTime, 0, groupData.bossRemainTime);
                    if (groupData.bossRemainTime <= 0)
                    {
                        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREGROUPDATA);
                        GameTimer.inst.RemoveTimer(timerId);
                        timerId = 0;
                    }
                });
            }
            else
            {
                if (groupData.bossExploreState == 1)
                {
                    GameTimer.inst.RemoveTimer(timerId);
                    timerId = 0;
                }
            }
        }
    }
}

public class ExploreItemData
{
    public int type; // 1 - 普通 2 - boss
    public int id;
    public int unlockState; // 0 - 未解锁 1 - 解锁
    public int unlockLevel = 0;
    public List<int> awards = new List<int>();

    public void setData(int type, int id, int group, int unlockState = 0)
    {
        awards.Clear();
        this.type = type;
        this.id = id;
        this.unlockState = unlockState;
        if (type != 4)
        {
            unlockLevel = ExploreInstanceLvConfigManager.inst.GetConfigDataByTypeAndIndex(4, type - 1);
            awards.Add(id);
        }
        else
        {
            unlockLevel = ExploreInstanceLvConfigManager.inst.GetConfigDataByTypeAndIndex(5, 0);
            ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(group);
            awards.Add(cfg.drop1_id);
            awards.Add(cfg.drop2_id);
            awards.Add(cfg.drop3_id);
        }
    }
}

public class ExploreSlotData
{
    public int slotId = 0;
    public int exploreId = 0;
    public int exploreTotalTime = 0;
    public int exploringRemainTime = 0;
    public int exploringEndTime;
    public int exploreState = 0; //enum EExploringState
    public int exploreType = 0;
    public int useItemId = 0;
    public int slotType = 0;
    public List<int> heroInfoUIds = new List<int>();

    int timerId;

    public void setData(ExploreSlot slotData)
    {
        slotId = slotData.slotId;
        exploreId = slotData.exploreId;
        exploreTotalTime = slotData.exploreTotalTime;
        exploringRemainTime = slotData.exploringRemainTime;
        exploringEndTime = (int)GameTimer.inst.serverNow + exploringRemainTime;
        exploreState = slotData.exploreState;
        exploreType = slotData.exploreType;
        useItemId = slotData.useItemId;
        slotType = slotData.slotType;
        heroInfoUIds = new List<int>();
        heroInfoUIds.Clear();

        for (int i = 0; i < slotData.heroInfoUIds.Count; i++)
        {
            int index = i;
            if (slotData.heroInfoUIds[index] != 0 && slotData.heroInfoUIds[index] != -1)
                heroInfoUIds.Add(slotData.heroInfoUIds[index]);
        }

        calculateTime();
    }

    private void calculateTime()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (exploreState == 1)
        {
            timerId = GameTimer.inst.AddTimer(1, update);
        }
    }

    private void update()
    {
        if (exploringRemainTime > 0)
        {
            exploringRemainTime = exploringEndTime - (int)GameTimer.inst.serverNow;
            if (exploringRemainTime <= 0)
            {
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTREFRESH, slotId);
                GameTimer.inst.RemoveTimer(timerId);
                timerId = 0;
            }
        }
    }
}

public class ExploreDataProxy : TSingletonHotfix<ExploreDataProxy>, IDataModelProx
{
    private Dictionary<int, ExploreGroup> exploreDic;
    private List<ExploreSlotData> slotList = new List<ExploreSlotData>();
    public int curSlotId;
    public bool isOpenExplorePanel;

    public string refugeAtlas;
    public string refugeIcon;

    public int slotType;

    public bool needShowRefugePanel;

    public ExploreGroupData exploreGroupData;
    public List<RoleHeroData> heroInfos = new List<RoleHeroData>();
    public List<ExploreSlotData> exploreSlotList
    {
        get
        {
            slotList.Sort((a, b) => { return a.exploringRemainTime.CompareTo(b.exploringRemainTime); });
            return slotList;
        }
        private set { slotList = value; }
    }
    public bool HasFinishExplore
    {
        get
        {
            return slotList.FindIndex(t => (EExploringState)t.exploreState == EExploringState.EE_Finish) != -1;
        }
        private set { }
    }

    public int slotCount
    {
        get { return exploreSlotList.Count; }
    }
    public List<ExploreGroup> ExploreList
    {
        get
        {
            return exploreDic.Values.ToList();
        }
    }

    public int SlotIdleCount
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < exploreSlotList.Count; i++)
            {
                if (exploreSlotList[i].exploreState == 0)
                    sum++;
            }

            return sum;
        }
    }

    public bool HasDamagedEquip
    {
        get
        {
            if (currExploreData != null)
            {
                foreach (var item in currExploreData.heroInfo)
                {
                    if (item.brokenEquip.equipId != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        private set { }
    }

    public void Clear()
    {
        exploreDic.Clear();
        slotList.Clear();
        exploreGroupData = null;
        heroInfos.Clear();
        exploreSlotList.Clear();
        ExploreList.Clear();
    }

    public void Init()
    {
        exploreDic = new Dictionary<int, ExploreGroup>();
        slotList = new List<ExploreSlotData>();

        Helper.AddNetworkRespListener(MsgType.Response_Explore_BuySlot_Cmd, GetBuySlotData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_Start_Cmd, GetExploreStartData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_Unlock_Cmd, GetExploreUnlockData);
        Helper.AddNetworkRespListener(MsgType.Response_ExploreSlot_Data_Cmd, GetExploreSlotData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_Immediately_Cmd, GetImmediatelyData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_Refresh_Cmd, GetExploreSlotRefreshData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_Data_Cmd, GetGroupData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_End_Cmd, GetExploreEndData);
        Helper.AddNetworkRespListener(MsgType.Response_Explore_RewardVip_Cmd, GetExploreVipReward);
    }

    private void GetExploreVipReward(HttpMsgRspdBase msg)
    {
        Response_Explore_RewardVip data = (Response_Explore_RewardVip)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREFINISH_HIDEUI);
    }

    private void GetExploreEndData(HttpMsgRspdBase msg)
    {
        Response_Explore_End data = (Response_Explore_End)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        heroInfos.Clear();
        currExploreData = (Response_Explore_End)msg;
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_HIDEUI);

        var tempData = GetMakeSlot(currExploreData.exploreSlot.slotId);
        if (tempData != null)
            tempData.setData(currExploreData.exploreSlot);
        //AddExploreData(data.exploreGroupData);
#if UNITY_EDITOR
        var path = Application.dataPath.Replace("/Assets", "") + "/combatReport.txt";
        {
            File.WriteAllText(path, msg.GetJsonParams(), Encoding.UTF8);
        }
#endif
        //进入战斗
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_SETANDINTOCOMBAT, currExploreData.exploreId, 1, "", currExploreData.combatReport);
        if (data.exploreSlot.slotType != 2)
        {
            exploreGroupData = new ExploreGroupData();
            var lastData = GetGroupDataByGroupId(data.exploreGroupData.groupId);
            exploreGroupData.level = lastData.groupData.level;
            exploreGroupData.exp = lastData.groupData.exp;
            exploreGroupData.groupId = lastData.groupData.groupId;
            AddExploreData(data.exploreGroupData);
        }

        foreach (var item in currExploreData.heroInfo)
        {
            RoleHeroData roleData = new RoleHeroData();
            var lastRoleData = RoleDataProxy.inst.GetHeroDataByUid(item.heroUid);
            roleData.level = lastRoleData.level;
            roleData.exp = lastRoleData.exp;
            roleData.uid = lastRoleData.uid;
            roleData.fightingNum = lastRoleData.fightingNum;
            roleData.config = lastRoleData.config;
            heroInfos.Add(roleData);
            RoleDataProxy.inst.AddHeroData(item);
        }
        //heroInfos = currExploreData.heroInfo;
    }

    void GetGroupData(HttpMsgRspdBase msg)
    {
        Response_Explore_Data data = (Response_Explore_Data)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        for (int i = 0; i < data.exploreGroupData.Count; i++)
        {
            if (i + 1 == data.exploreGroupData.Count)
            {
                AddExploreData(data.exploreGroupData[i], true);
            }
            else
            {
                AddExploreData(data.exploreGroupData[i]);
            }
        }

        if (isOpenExplorePanel)
        {
            isOpenExplorePanel = false;
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_SHOWUI);
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.RESPONSE_PREPAREREFRESHDATA);
        }
    }

    void GetExploreSlotRefreshData(HttpMsgRspdBase msg)
    {
        Response_Explore_Refresh data = (Response_Explore_Refresh)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        var tempData = GetMakeSlot(data.exploreSlotList.slotId);
        if (tempData != null)
            tempData.setData(data.exploreSlotList);
        else
            AddSlotData(data.exploreSlotList);

        if (data.exploreSlotList.slotType != 2)
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.RESPONSE_SETADVENTUREDATA, data.exploreSlotList.slotId);
        else
            HotfixBridge.inst.TriggerLuaEvent("RefreshUI_RefugeAdventure", data.exploreSlotList.slotId);

        if (ManagerBinder.inst.mGameState == kGameState.Town)
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, ESlotAnimType.Refresh);
        }
        else if (ManagerBinder.inst.mGameState == kGameState.Shop)
        {
            EventController.inst.TriggerEvent(GameEventType.REFRESHMAINUIREDPOINT);
        }
    }

    void GetImmediatelyData(HttpMsgRspdBase msg)
    {
        Response_Explore_Immediately data = (Response_Explore_Immediately)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        ExploreSlotData slotData = GetMakeSlot(data.exploreSlot.slotId);
        if (slotData == null)
            AddSlotData(data.exploreSlot);
        else
            slotData.setData(data.exploreSlot);

        if(slotData != null && slotData.exploreState != 1)
        {
            PlatformManager.inst.RemoveOneNotificationWithID("Explore_" + slotData.exploreId);
        }

        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEADVENTURE_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.RESPONSE_HEROSHIFTIN);
        //EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, ESlotAnimType.Normal);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREEND, data.exploreSlot.slotId);
    }

    private void GetExploreSlotData(HttpMsgRspdBase msg)
    {
        Response_ExploreSlot_Data data = (Response_ExploreSlot_Data)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        for (int i = 0; i < data.exploreSlotList.Count; i++)
        {
            ExploreSlotData slotData = GetMakeSlot(data.exploreSlotList[i].slotId);

            if (slotData == null)
            {
                slotData = AddSlotData(data.exploreSlotList[i]);
            }
            else
            {
                slotData.setData(data.exploreSlotList[i]);
            }

            if (slotData.exploreState == 1)
            {
                string title = LanguageManager.inst.GetValueByKey("生存几何");
                string body = "";
                var instanceCfg = ExploreInstanceConfigManager.inst.GetConfig(slotData.exploreId);
                if (instanceCfg != null)
                {
                    body = LanguageManager.inst.GetValueByKey("您的英雄小队在{0}已完成探索，快来收取战利品！", LanguageManager.inst.GetValueByKey(instanceCfg.instance_name));
                }
                PlatformManager.inst.AddLocalNotice(title, "", body, 1, slotData.exploringEndTime, "Explore_" + slotData.exploreId);
            }
        }

        if (ManagerBinder.inst.mGameState == kGameState.Town)
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, ESlotAnimType.Normal);
        }
    }

    private void GetBuySlotData(HttpMsgRspdBase msg)
    {
        Response_Explore_BuySlot data = (Response_Explore_BuySlot)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        AddSlotData(data.exploreSlot);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREBUYSLOT_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, ESlotAnimType.Normal);
    }

    private void GetExploreStartData(HttpMsgRspdBase msg)
    {
        Response_Explore_Start data = (Response_Explore_Start)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        var tempData = GetMakeSlot(data.exploreSlot.slotId);
        if (tempData != null)
        {
            tempData.setData(data.exploreSlot);

            if(tempData.exploreState == 1)
            {
                string title = LanguageManager.inst.GetValueByKey("生存几何");
                string body = "";
                var instanceIdCfg = ExploreInstanceConfigManager.inst.GetConfig(tempData.exploreId);
                if (instanceIdCfg != null)
                {
                    body = LanguageManager.inst.GetValueByKey("您的英雄小队在{0}已完成探索，快来收取战利品！", LanguageManager.inst.GetValueByKey(instanceIdCfg.instance_name));
                }
                PlatformManager.inst.AddLocalNotice(title, "", body, 1, tempData.exploringEndTime, "Explore_" + tempData.exploreId);
            }
        }


        for (int i = 0; i < data.heroInfo.Count; i++)
        {
            RoleDataProxy.inst.AddHeroData(data.heroInfo[i]);
        }

        var instanceCfg = ExploreInstanceConfigManager.inst.GetConfig(data.exploreSlot.exploreId);
        PlatformManager.inst.GameHandleEventLog("Explore_map_0" + instanceCfg.instance_group, "");

        //EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREPREPARE_HIDEUI);
        //EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_HIDEUI);
        GUIManager.BackMainView();
        if (ManagerBinder.inst.mGameState == kGameState.Town)
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, ESlotAnimType.Normal);
    }

    public Response_Explore_End currExploreData;

    public void OpenExploreEndUI()
    {
        if (currExploreData == null)
        {
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
            return;
        }
        if (currExploreData.resultState == 0)
        {
            // 成功
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREFINISH_SHOWSUCCESSUI, currExploreData.rewardList, currExploreData.exploreSlot.slotId, currExploreData.exploreId);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREFINISH_SHOWLOSEUI, currExploreData.exploreId);
        }
        slotType = currExploreData.exploreSlot.slotType;
        currExploreData = null;
    }
    private void GetExploreUnlockData(HttpMsgRspdBase msg)
    {
        Response_Explore_Unlock data = (Response_Explore_Unlock)msg;
        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;
        AddExploreData(data.exploreGroupData, true);

        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUNLOCK_HIDEUI);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_SHOWUI);
    }

    public ExploreSlotData GetMakeSlot(int slotId)
    {
        return exploreSlotList.Find(t => t.slotId == slotId);
    }

    public ExploreSlotData AddSlotData(ExploreSlot slot)
    {
        var slotData = new ExploreSlotData();
        slotData.setData(slot);
        exploreSlotList.Add(slotData);
        return slotData;
    }

    public ExploreGroup AddExploreData(ExploreGroupData groupData, bool isLast = false)
    {
        List<ExploreInstanceLvConfigData> tempList = ExploreInstanceLvConfigManager.inst.GetSameGroupData(groupData.groupId, groupData.level);
        ExploreInstanceConfigData instanceData = ExploreInstanceConfigManager.inst.GetConfigByGroupId(groupData.groupId);

        if (tempList == null || tempList.Count <= 0)
        {
            Logger.error("没有" + groupData.groupId + "这个id并且等级为" + groupData.level + "的数据");
            return null;
        }
        ExploreGroup tempData = new ExploreGroup();
        int caculateLevel = 0;

        if (exploreDic.ContainsKey(groupData.groupId))
        {
            caculateLevel = exploreDic[groupData.groupId].groupData.level;
            if (caculateLevel == 1) caculateLevel = 0;
            tempData = exploreDic[groupData.groupId];
        }
        else
        {
            ExploreItemData itemData = new ExploreItemData();
            itemData.setData(1, instanceData.drop1_id, groupData.groupId);
            tempData.explores.Add(itemData);
            itemData = new ExploreItemData();
            itemData.setData(2, instanceData.drop2_id, groupData.groupId);
            tempData.explores.Add(itemData);
            itemData = new ExploreItemData();
            itemData.setData(3, instanceData.drop3_id, groupData.groupId);
            tempData.explores.Add(itemData);
            itemData = new ExploreItemData();
            itemData.setData(4, instanceData.boss_id, groupData.groupId);
            tempData.explores.Add(itemData);
            tempData.unlockNextExploreLevel = ExploreInstanceLvConfigManager.inst.GetConfigDataByTypeAndIndex(5, 0, true, groupData.groupId);
            for (int i = 0; i < tempData.dfficLevel.Length; i++)
            {
                tempData.dfficLevel[i] = ExploreInstanceLvConfigManager.inst.GetConfigDataByTypeAndIndex(5, i);
            }
            caculateLevel = 0;
        }
        tempData.setData(groupData);
        if (groupData.groupState != 2)
        {
            if (exploreDic.ContainsKey(groupData.groupId))
                exploreDic[groupData.groupId] = tempData;
            else
                exploreDic.Add(groupData.groupId, tempData);
            return exploreDic[groupData.groupId];
        }
        for (int i = caculateLevel; i < tempList.Count; i++)
        {
            int index = i;

            switch (tempList[index].effect_type)
            {
                case 1:
                    int[] typeId = tempList[index].effect_id;
                    if (typeId[0] == 1)
                    {
                        tempData.dropCount[0] = tempList[index].effect_num;
                    }
                    else if (typeId[0] == 2)
                    {
                        tempData.dropCount[1] = tempList[index].effect_num;
                    }
                    else if (typeId[0] == 3)
                    {
                        tempData.dropCount[2] = tempList[index].effect_num;
                    }
                    break;
                case 2:
                    tempData.restTimePercent = (1 - (float)tempList[index].effect_num / 100);
                    break;
                case 3:
                    tempData.exploreTimePercent = (1 - (float)tempList[index].effect_num / 100);
                    break;
                case 4:
                    int tempId = 0;
                    if (tempList[index].effect_id[0] == 1)
                    {
                        tempId = instanceData.drop1_id;
                    }
                    else if (tempList[index].effect_id[0] == 2)
                    {
                        tempId = instanceData.drop2_id;
                    }
                    else if (tempList[index].effect_id[0] == 3)
                    {
                        tempId = instanceData.drop3_id;
                    }
                    tempData.explores.Find(t => t.id == tempId).unlockState = 1;
                    break;
                case 5:
                    for (int k = 0; k < tempList[index].effect_id.Length; k++)
                    {
                        ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfig(tempList[index].effect_id[k]);
                        if (cfg.instance_group == groupData.groupId)
                        {
                            if (cfg.instance_type == 2 && tempData.explores[tempData.explores.Count - 1].unlockState == 0/*  && tempData.explores.Find(t => t.id == cfg.boss_id).unlockState == 0*/)
                            {
                                tempData.explores.Find(t => t.id == cfg.boss_id).unlockState = 1;
                            }
                            tempData.difficult = cfg.difficulty;
                        }
                        else
                        {
                            tempData.nextIsUnlock = true;
                            if (exploreDic.ContainsKey(cfg.instance_group) && exploreDic[cfg.instance_group].groupData.groupState == 0)
                            {
                                exploreDic[cfg.instance_group].groupData.groupState = 1;
                                AddExploreData(exploreDic[cfg.instance_group].groupData, true);
                            }
                        }
                    }
                    break;
                case 6:
                    tempData.addExpPercent = (1 + (float)tempList[index].effect_num / 100);
                    break;
            }
        }

        if (exploreDic.ContainsKey(groupData.groupId))
            exploreDic[groupData.groupId] = tempData;
        else
            exploreDic.Add(groupData.groupId, tempData);

        if (isLast && !exploreDic.ContainsKey(groupData.groupId + 1))
        {
            if (ExploreInstanceConfigManager.inst.JudgeHasTargetGroupData(groupData.groupId + 1))
            {
                if (exploreDic[groupData.groupId].nextIsUnlock)
                {
                    ExploreGroupData temporaryData = new ExploreGroupData();
                    temporaryData.groupId = groupData.groupId + 1;
                    temporaryData.level = 1;
                    temporaryData.exp = 0;
                    temporaryData.bossExploreState = 1;
                    temporaryData.bossRemainTime = 0;
                    temporaryData.groupState = 1;
                    AddExploreData(temporaryData, true);

                }
                else
                {
                    ExploreGroupData temporaryData = new ExploreGroupData();
                    temporaryData.groupId = groupData.groupId + 1;
                    temporaryData.level = 1;
                    temporaryData.exp = 0;
                    temporaryData.bossExploreState = 1;
                    temporaryData.bossRemainTime = 0;
                    temporaryData.groupState = 0;
                    AddExploreData(temporaryData, true);
                }
            }
        }

        return exploreDic[groupData.groupId];
    }

    public ExploreGroup GetGroupDataByGroupId(int groupId)
    {
        if (exploreDic.ContainsKey(groupId))
        {
            return exploreDic[groupId];
        }

        //Logger.error("没有组为" + groupId + "的数据");
        return null;
    }

    public ExploreSlotData GetSlotDataById(int slotId)
    {
        return exploreSlotList.Find(t => t.slotId == slotId);
    }

    public ExploreSlotData GetRefugeSlotData()
    {
        return exploreSlotList.Find(t => t.slotType == 2 && t.exploreId != 0);
    }

    public bool IsHaveRefugeSlot()
    {
        bool isHave = false;
        for (int i = 0; i < exploreSlotList.Count; i++)
        {
            if (exploreSlotList[i].slotType == 2 && exploreSlotList[i].exploreId != 0)
            {
                isHave = true;
                break;
            }
        }

        return isHave;
    }

    public ExploreSlotData GetMakeSlotByIndex(int index)
    {
        return index >= slotCount ? null : exploreSlotList[index];
    }

    public ExploreSlotData GetFreeSlotData()
    {
        for (int i = 0; i < exploreSlotList.Count; i++)
        {
            if (exploreSlotList[i].exploreState == 0)
            {
                return exploreSlotList[i];
            }
        }

        return null;
    }

    public int GetSlotIdByHeroUid(int heroUid)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            int index = i;
            if (slotList[index].heroInfoUIds.Find(t => t == heroUid) != 0)
            {
                return slotList[index].slotId;
            }
        }
        return -1;
    }

    public List<int> GetAllWorkEquipMakeSlot()
    {
        List<int> result = new List<int>();

        slotList.ForEach(item =>
        {
            if (item.exploreState != 0)
            {
                result.Add(item.slotId);
            }
        });

        return result;
    }

    public RoleHeroData GetLastHeroDataByUid(int uid)
    {
        if (heroInfos != null && heroInfos.Count > 0)
        {
            var info = heroInfos.Find(t => t.uid == uid);
            return info;
        }
        return null;
    }
}
