using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDataProxy : TSingletonHotfix<UserDataProxy>, IDataModelProx
{
    #region 玩家属性字段


    private PlayerData _playerData;
    public PlayerData playerData
    {
        get
        {
            return _playerData;
        }
        private set
        {
            _playerData = value;
        }
    }
    #endregion
    #region 任务属性字段
    private Dictionary<int, TaskData> _taskDic;
    private Dictionary<int, ActiveRewardBoxData> _activeBoxDic;//活跃宝箱
    public List<TaskData> taskList
    {
        get { return _taskDic.Values.ToList(); }
    }

    public List<ActiveRewardBoxData> activeBoxList
    {
        get { return _activeBoxDic.Values.ToList(); }
    }

    public int task_refreshNumber { get; set; }
    public int task_nextTime { get { return task_nextEndTime - (int)GameTimer.inst.serverNow; } }
    private int task_nextEndTime;

    public int task_refreshTime { get { return task_refreshEndTime - (int)GameTimer.inst.serverNow; } }
    private int task_refreshEndTime;


    public int currMainTaskGroup = 0;
    public bool task_isUiAnimShow = false;

    private int toEndLeftTimer;

    public int task_activePoint;
    public int task_activePointEnd;

    public bool task_dailyNeedShowRedPoint
    {
        get
        {
            var dateTime = TimeUtils.getDateTimeBySecs(GameTimer.inst.serverNow);
            if (PlayerPrefs.GetInt(AccountDataProxy.inst.account + "_DailyTaskRedPoint_" + dateTime.Year.ToString() + dateTime.Month.ToString() + dateTime.Day.ToString(),-1) != 1)
            {
                return true;
            }

            foreach (var item in taskList)
            {
                if (item.taskState == (int)EDailyTaskState.Reached)
                {
                    return true;
                }
            }


            foreach (var item in activeBoxList)
            {
                if (item.state == (int)ActiveRewardBoxState.canGet)
                {

                    if (item.config.vip_on == 1) //需要VIP
                    {
                        if (playerData.isVip()) //我有vip
                        {
                            var cfg = VipLevelConfigManager.inst.GetConfig(playerData.vipLevel);

                            if (cfg.type_7 != 1 || cfg.type_8 != 1)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }
    }

    public bool task_unionTaskNeedShowRedPoint
    {
        get
        {
            if (selfUnionTask != null && selfUnionTask.data.state == (int)EUnionTaskState.CanReward)
            {
                return true;
            }

            return false;
        }
    }

    public bool task_needShowRedPoint
    {
        get
        {
            return task_dailyNeedShowRedPoint || task_unionTaskNeedShowRedPoint;
        }
    }
    #endregion
    #region 资源属性字段
    //资源生产序列
    public Dictionary<int, ResourceProduction> resSlotList;
    #endregion
    #region 城市属性字段
    public bool cityIsInit;
    List<CityBuildingData> cityBuildings;
    #endregion
    #region 地图属性字段
    private int LocalIncreaseUid = 100000000;
    public int GetFurnitureUid
    {
        get { return LocalIncreaseUid++; }
    }
    IndoorData indoorData = new global::IndoorData();

    public IndoorData shopData
    {
        get { return indoorData; }
    }
    public bool hasIndoorData = false;
    #endregion
    #region 联盟属性字段

    /*
	 public int newUnionEvent = 0; 实时
     public int newUnionScienceEvent = 0;
	 public int newUnionSkillEvent = 0; 实时
	 public int NewUnionBuildEvent = 0; 实时
	 public int NewUnionHelpEvent = 0;
	 public int NewUnionTaskEvent = 0;
     */

    public int union_needRequest_Science = 0;
    public int union_needRequest_MemberHelp = 0;
    public int union_needRequest_Task = 0;


    public UnionDetailInfo unionDetailInfo = new UnionDetailInfo();  //自身公会详细数据

    //援助
    public List<UnionMemberHelpData> union_helpList = new List<UnionMemberHelpData>();
    public int helpFurnitureFlag = 1;//0 可以请求
    public int helpShopExtenFlag = 1;//1 不可以请求

    public List<UnionMemberHelpData> union_aidShowList
    {

        get
        {

            var list = union_helpList.FindAll(t => t.severData.userId != playerData.userUid && t.severData.unionId == playerData.unionId);
            list.Sort((a, b) =>
            {
                int num1 = (a.severData.memberList.Find(f => f.userId == playerData.userUid) == null) ? 0 : 1;
                int num2 = (b.severData.memberList.Find(f => f.userId == playerData.userUid) == null) ? 0 : 1;

                return num1.CompareTo(num2);
            });

            //list.Add(new UnionMemberHelpData(new OneHelpData() { furnitureId = 99999, furnitureUid = 99999, level = 3, userId = "11111", name = "我是卢本伟", memberList = new List<OneUnionMemberHelpData>() { new OneUnionMemberHelpData() { userId = playerData.userUid } }, stateEndTime = 2000 }));

            return list;
        }
        //get { return new List<OneHelpData>() { new OneHelpData() { furnitureId = 99999, furnitureUid = 99999, level = 3,userId = "11111", name = "我是卢本伟", memberList = new List<OneUnionMemberHelpData>(), stateEndTime = 2000 } }; }
    }

    public List<UnionMemberHelpData> union_canAidList
    {
        get { return union_aidShowList.FindAll(t => t.severData.memberList.Find(f => f.userId == playerData.userUid) == null); }
    }

    //悬赏
    public int unionTaskRefreshTime
    {
        get { return unionTaskRefreshEndTime - (int)GameTimer.inst.serverNow; }
    }
    private int unionTaskRefreshEndTime;
    public int unionTaskCancelCoolTime
    {
        get { return unionTaskCancelCoolEndTime - (int)GameTimer.inst.serverNow; }
    }
    private int unionTaskCancelCoolEndTime;
    public int unionTaskLevel = 1;
    public int unionTaskPoint;
    List<UnionTaskData> unionTaskList = new List<UnionTaskData>();

    public UnionTaskData selfUnionTask
    {
        get
        {
            var data = unionTaskList.Find(t => t.data.userId == playerData.userUid);

            return data;
        }
    }

    public List<UnionTaskData> UnionTaskList
    {
        get
        {
            var selfTask = selfUnionTask;

            unionTaskList.Sort((a, b) =>
            {
                int one = a.data.state, two = b.data.state;
                if (a.data.userId == playerData.userUid)
                {
                    one = -1;
                }
                else if (b.data.userId == playerData.userUid)
                {
                    two = -1;
                }

                if (one != -1 && a.data.state == (int)EUnionTaskState.Doing && two != -1 && b.data.state == (int)EUnionTaskState.Doing)
                {
                    return a.data.endTime.CompareTo(b.data.endTime);
                }

                if (one != -1 && a.data.state == (int)EUnionTaskState.Idle && two != -1 && b.data.state == (int)EUnionTaskState.Idle)
                {
                    return a.data.taskId.CompareTo(b.data.taskId);
                }

                return one.CompareTo(two);
            });

            ////测试
            //if (unionTaskList.Find(t => t.data.taskUid == 2) == null)
            //{
            //    unionTaskList.Add(new UnionTaskData(new OneUnionTaskData() { endTime = 100, level = 3, point = 3, process = 1, limit = 4, taskUid = 1, state = 2, taskTarget = 1010, taskId = 1, }));
            //    unionTaskList.Add(new UnionTaskData(new OneUnionTaskData() { endTime = 300, level = 3, point = 2, process = 1, limit = 4, taskUid = 2, state = 0, taskTarget = 1010, taskId = 2, }));
            //    unionTaskList.Add(new UnionTaskData(new OneUnionTaskData() { endTime = 3300, level = 3, point = 1, process = 1, limit = 4, taskUid = 3, state = 3, taskTarget = 1010, taskId = 3, }));
            //}

            return unionTaskList;
        }
    }

    //科技
    long union_uCoin;//联盟积分

    public long Union_uCoin { get { return union_uCoin; } }

    Dictionary<int, UnionScienceData> unionScienceDic = new Dictionary<int, UnionScienceData>();

    public UnionScienceData getUnionScienceDataByType(int type)
    {
        if (unionScienceDic.ContainsKey(type))
        {
            return unionScienceDic[type];
        }

        return null;
    }

    public List<UnionScienceData> UnionScienceList
    {
        get
        {
            //测试
            //if (true)
            //{
            //    for (int i = 1; i <= 21; i++)
            //    {
            //        if (i == 2) continue;

            //        if (i >= 12 && i <= 15)
            //        {
            //            unionBuffDatas.Add(new UnionBuffData(new OneUnionScienceSkillData() { time = 60 * 1800 * 2, type = i, level = 4 }));
            //        }

            //        unionScienceDic.Add(i, new UnionScienceData(new OneUnionScienceData() { level = i == 21 ? 1 : 4, type = i }));
            //    }

            //}


            var list = unionScienceDic.Values.ToList();
            list.Sort((a, b) =>
            {
                int num_1 = a.serverData.type, num_2 = b.serverData.type;

                if (a.serverData.type == 21)
                {
                    num_1 = -1;
                }

                if (b.serverData.type == 21)
                {
                    num_2 = -1;
                }


                return num_1.CompareTo(num_2);
            });

            return list;
        }
    }

    public int UnionLevel
    {
        get
        {
            if (unionScienceDic.ContainsKey((int)EUnionScienceType.UnionLevelUp))
            {
                return unionScienceDic[(int)EUnionScienceType.UnionLevelUp].serverData.level;
            }

            return 1;
        }
    }

    public List<UnionBuffData> UnionBuffDatas
    {
        get
        {
            unionBuffDatas.Sort((a, b) => a.serverData.type.CompareTo(b.serverData.type));
            return unionBuffDatas;
        }
    }

    public UnionBuffData GetUnionBuffData(EUnionScienceType big_Type)
    {
        return unionBuffDatas.Find(t => t.serverData.type == (int)big_Type && t.remainTime > 0);
    }

    List<UnionBuffData> unionBuffDatas = new List<UnionBuffData>();


    #endregion
    public void Clear()
    {
        // 用户数据
        playerData = null;
        // 每日任务数据
        if (_taskDic != null)
            _taskDic.Clear();
        if (taskList != null)
            taskList.Clear();
        // 资源数据
        if (resSlotList != null)
            resSlotList.Clear();
        // 城市数据
        if (cityBuildings != null)
            cityBuildings.Clear();
        // 地图数据
        indoorData = null;
        mapdataIsReady = false;
    }

    public void Init()
    {
        InitPlayerData();
        InitTaskData();
        InitResourceProductionData();
        InitCityData();
        IndoorData();
        InitUnionData();
    }

    private void InitPlayerData()
    {
        // 用户数据
        playerData = new PlayerData();
        Helper.AddNetworkRespListener(MsgType.Response_User_Data_Cmd, GetUserDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_DataChange_Cmd, GetUserDataChangeResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_ChangeName_Cmd, GetChangeNameData);
        Helper.AddNetworkRespListener(MsgType.Response_User_CommonReward_Cmd, OnResponseCommonReward);
    }

    private void InitTaskData()
    {

        // 每日任务数据
        _taskDic = new Dictionary<int, TaskData>();
        _activeBoxDic = new Dictionary<int, ActiveRewardBoxData>();
        Helper.AddNetworkRespListener(MsgType.Response_DailyTask_Change_Cmd, OnDailyTaskChanged);
        Helper.AddNetworkRespListener(MsgType.Response_DailyTask_Data_Cmd, GetDailyTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_DailyTask_Refresh_Cmd, GetDailyTaskRefresh);
        Helper.AddNetworkRespListener(MsgType.Response_DailyTask_Reward_Cmd, GetDailyTaskAward);
        Helper.AddNetworkRespListener(MsgType.Response_Active_Reward_Cmd, GetDailyActiveBoxAward);

    }

    private void InitResourceProductionData()
    {
        // 资源数据
        resSlotList = new Dictionary<int, ResourceProduction>();
        Helper.AddNetworkRespListener(MsgType.Response_Resource_ProductionList_Cmd, GetResourceProductionList);
        Helper.AddNetworkRespListener(MsgType.Response_Resource_ProductionRefresh_Cmd, GetResourceProductionRefresh);
        Helper.AddNetworkRespListener(MsgType.Response_Resource_ProductionChange_Cmd, GetResourceProductionChange);
    }

    private void InitCityData()
    {
        // 城市数据
        cityBuildings = new List<CityBuildingData>();
        Helper.AddNetworkRespListener(MsgType.Response_City_BuildData_Cmd, getAllCityBuildingDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_City_BuildDataChanged_Cmd, getCityBuildingDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_City_BuildingRankData_Cmd, getBuildingRankDataResp);
    }

    private void IndoorData()
    {
        // 地图数据
        indoorData = new IndoorData();
        Helper.AddNetworkRespListener(MsgType.Response_Design_Data_Cmd, OnResponseDesignData);
        Helper.AddNetworkRespListener(MsgType.Response_Design_Move_Cmd, OnResponseDesignMove);
        Helper.AddNetworkRespListener(MsgType.Response_Design_FurnitureChange_Cmd, OnResponseDesignFurnitureChange);
        Helper.AddNetworkRespListener(MsgType.Response_Design_ShelfEquipChange_Cmd, ChangeShelfEquipList);
        Helper.AddNetworkRespListener(MsgType.Response_Design_InStore_Cmd, onDesignInStoreResp);
        Helper.AddNetworkRespListener(MsgType.Response_Design_SetFloor_Cmd, OnDesignSetFloorResp);
        Helper.AddNetworkRespListener(MsgType.Response_Design_SetWall_Cmd, OnDesignSetWallResp);
        Helper.AddNetworkRespListener(MsgType.Response_Design_Buy_Cmd, onDesignBuy);
    }

    private void InitUnionData()
    {
        //援助
        Helper.AddNetworkRespListener(MsgType.Response_Union_MemberHelpList_Cmd, GetUnionMemberHelpListData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_SetHelp_Cmd, GetUnoinSetHelpData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_HelpMember_Cmd, GetUnionHelpMemberData);

        //悬赏
        Helper.AddNetworkRespListener(MsgType.Response_Union_TaskList_Cmd, getUnionTaskListData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_CheckUnionTask_Cmd, getUnionCheckTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_StartUnionTask_Cmd, getUnionStartUnionTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_CancelUnionTask_Cmd, getUnionCancelTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_RewardUnionTask_Cmd, getUnionRewardTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_AccelUnionTask_Cmd, getUnionGemRewardTaskData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_UnionTaskChange_Cmd, getUnionTaskChangedData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_UnionTaskRankList_Cmd, getUnionTaskRankListData);

        //科技
        Helper.AddNetworkRespListener(MsgType.Response_Union_ScienceList_Cmd, GetUnionScienceListData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_ScienceUpgrade_Cmd, GetUnionScienceUpgradeData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_ScienceSkillList_Cmd, GetUnionScienceSkillListData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_ScienceSkillUse_Cmd, GetUnionScienceSkillUseData);
        Helper.AddNetworkRespListener(MsgType.Response_Union_ScienceSkillRefresh_Cmd, GetUnionScienceSkillRefreshData);


    }

    #region 玩家方法
    private void GetChangeNameData(HttpMsgRspdBase msg)
    {
        var data = (Response_User_ChangeName)msg;
        AudioManager.inst.PlaySound(13);
        playerData.playerName = data.nickName;
        if (playerData.freeNameCount > 0)
            playerData.freeNameCount--;
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_CHANGENAME);
    }

    public void OnResponseCommonReward(HttpMsgRspdBase msg)
    {
        var data = (Response_User_CommonReward)msg;
        if (data.rewardList.Count > 1)
        {
            List<CommonRewardData> rewardsList = new List<CommonRewardData>();
            foreach (var item in data.rewardList)
            {
                rewardsList.Add(new CommonRewardData(item.itemId, item.count, 0, item.itemType));
            }
            //调用多个奖励
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = rewardsList });
        }
        else
        {
            int rewardid = 0;
            int equipid = 0;
            if ((ItemType)data.rewardList[0].itemType == ItemType.Equip)
            {
                equipid = data.rewardList[0].itemId;
            }
            else
            {
                rewardid = data.rewardList[0].itemId;
            }
            //调用单个奖励
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", equipid, rewardid, data.rewardList[0].count));
        }
    }
    private void GetUserDataResp(HttpMsgRspdBase msg)
    {
        var data = (Response_User_Data)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            Logger.log("Response_User_Data ：：1");
            //FGUI.inst.StartExcessAnimation(true, false, () =>
            //{
            initGameData(data.userData, data.userExtData, data.unionData, data.freeData, data.vipData, data.petInfo);
            GuideManager.inst.waitStart = true;

            if (AccountDataProxy.inst.needCreatRole)
            {
                HotfixBridge.inst.ChangeState(new StateTransition(kGameState.CreatRole, false));
            }
            else
            {
                HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Shop, true));
            }

            var dateTime = TimeUtils.getDateTimeBySecs(GameTimer.inst.serverNow);
            if (PlayerPrefs.GetString(AccountDataProxy.inst.account + "_LookBack_" + dateTime.Year + dateTime.Month + dateTime.Day, "-1") == "-1")
            {
                EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_REQUEST_DATA);
            }
        }
    }

    private void initGameData(UserData userdata, UserExtData extdata, UnionData unionData, FreeData freeData, VIPInfo vipInfo, OnePetInfo mainPetInfo)
    {
        var wcfg = WorldParConfigManager.inst.GetConfig(124);
        StaticConstants.shopMap_MaxLevel = wcfg == null ? 10 : (int)wcfg.parameters;
        Logger.log("初始化GameUserData  initGameData");
        task_activePointEnd = (int)WorldParConfigManager.inst.GetConfig(150).parameters;
        playerData.InitItemData(userdata, unionData, extdata.freeNameCount, freeData, vipInfo, mainPetInfo);
        ShopkeeperDataProxy.inst.curGender = (EGender)userdata.gender;
        EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.INITCLOTHE);
        //EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_MEMBERHELPLIST);//收到公会数据 就立马请求公会援助列表
    }

    private void GetUserDataChangeResp(HttpMsgRspdBase msg)
    {
        var data = (Response_User_DataChange)msg;
        //EUserDataChangeType
        switch (data.dataType)
        {
            //Exp
            case 1:
                playerData.CurrExp = data.newValue;
                if (MainLineDataProxy.inst.Data != null && MainLineDataProxy.inst.Data.cfg != null && MainLineDataProxy.inst.Data.cfg.task_type == 15)
                {
                    EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, true);
                }
                EventController.inst.TriggerEvent(GameEventType.USERDATA_EXPCHANGE, data.changeValue);
                break;
            //Level
            case 2:
                LevelUp((uint)data.newValue);
                break;
            //Gold
            case 3:
                ItemBagProxy.inst.updateItemNum(StaticConstants.glodID, data.newValue);
                if (data.changeValue > 0)
                {
                    if (data.reason != (int)EItemLogReason.ExplorePass && data.reason != (int)EItemLogReason.Prize)
                    {
                        //AudioManager.inst.PlaySound(33);
                        EventController.inst.TriggerEvent(GameEventType.GOLD_FLY, data.changeValue, data.oldValue, data.newValue);
                    }
                }
                else
                {
                    AudioManager.inst.PlaySound(34);
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.GOLDNUM_ADD, data.oldValue, data.newValue);
                }

                playerData.gold = data.newValue;

                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_ALLBUILDINGUPREFRESH); //刷新城市的所有hud
                break;
            //Gem
            case 4:
                AudioManager.inst.PlaySound(139);
                EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.GEMNUM_ADD, data.oldValue, data.newValue);
                playerData.gem = data.newValue;
                ItemBagProxy.inst.updateItemNum(StaticConstants.gemID, data.newValue);
                break;
            //能量
            case 5:
                if (data.changeValue > 0)
                {
                    AudioManager.inst.PlaySound(31);
                    EventController.inst.TriggerEvent(GameEventType.ENERGY_FLY, data.changeValue, data.oldValue, data.newValue);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.ENERGYNUM_REDUCE, data.oldValue, data.newValue);
                }
                float energyPercent = data.newValue / playerData.energyLimit * 100;
                HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 5, energyPercent);
                playerData.energy = data.newValue;
                EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_UPDATEBYENERGY);
                break;
            case 6:     //能量上限
                playerData.energyLimit = data.newValue;
                EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.ENERGYLIMITNUM_CHANGE);
                break;
            case 7:  //图纸数量
                playerData.drawing = data.newValue;
                ItemBagProxy.inst.updateItemNum(StaticConstants.drawingID, playerData.drawing);
                break;
            case (int)EUserDataChangeType.Worth:
                playerData.worth = data.newValue;
                break;
            case (int)EUserDataChangeType.Prosperity:
                playerData.prosperity = data.newValue;
                EventController.inst.TriggerEvent(GameEventType.MenuEvent.REFRESHLUXURYBTN);
                var topview = GUIManager.GetWindow<TopPlayerInfoView>();
                if (topview != null)
                {
                    topview.UpdateShow();
                }
                break;
            case (int)EUserDataChangeType.MasterCount:
                playerData.masterCount = data.newValue;
                break;
            case (int)EUserDataChangeType.BagLimit://仓库容量
                playerData.bagLimit = data.newValue;
                break;
            case (int)EUserDataChangeType.PileLimit://材料堆叠上限
                playerData.pileLimit = data.newValue;
                break;
            case (int)EUserDataChangeType.FreeDesignBuyCount:
                playerData.designFreeCount = data.newValue;
                break;
            case (int)EUserDataChangeType.FreeEquipImproveCount:
                playerData.equipImproveFreeCount = data.newValue;
                break;
            case (int)EUserDataChangeType.FreeExploreImmediateCount:
                playerData.exploreImmediatelyFreeCount = data.newValue;
                break;
            case (int)EUserDataChangeType.FreeHeroBuyCount:
                playerData.heroBuyFreeCount = data.newValue;
                break;
            case (int)EUserDataChangeType.Invest:
                playerData.invest = data.newValue;
                break;
            case (int)EUserDataChangeType.MemberJob:
                playerData.memberJob = data.newValue;
                break;
            case (int)EUserDataChangeType.UnionCoin:
                AudioManager.inst.PlaySound(144);
                if (data.newValue > playerData.unionCoin)
                {
                    EventController.inst.TriggerEvent(GameEventType.UnionCoin_FLY, data.changeValue, data.oldValue, data.newValue);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.SELF_UNIONCOIN, data.oldValue, data.newValue);
                }

                playerData.unionCoin = data.newValue;
                break;
            case (int)EUserDataChangeType.Pet:

                playerData.mainPetUid = (int)data.newValue;

                break;

            case (int)EUserDataChangeType.UnionHelpCount:

                playerData.unionHelpCount = (int)data.newValue;

                break;

            case (int)EUserDataChangeType.UnionTaskCrownCount:

                playerData.unionTaskCrownCount = (int)data.newValue;

                break;

            default:
                break;
        }
        EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_DATA_UPDATE);
    }

    //升级
    public void LevelUp(uint level)
    {
        RoleDataProxy.inst.CheckHasNewExhcangeHero((int)playerData.level, (int)level);
        uint lastLv = playerData.level;
        playerData.level = level;
        playerData.MaxExp = ShopkeeperUpconfigManager.inst.GetConfig(level < 99 ? level + 1 : 99).experience;
        if (IndoorRoleSystem.inst != null && ManagerBinder.inst.mGameState == kGameState.Shop) IndoorRoleSystem.inst.StreetDropInit();

        //打点=====================================================
        if (level > 5)
        {
            if (level % 5 == 0)
            {
                PlatformManager.inst.GameHandleEventLog("Level_" + level, "");
            }
        }
        else
        {
            PlatformManager.inst.GameHandleEventLog("Level_" + level, "");
        }
        //========================================================================
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new PopUIInfoBase { type = ReceiveInfoUIType.ShopperLvUp });
        HotfixBridge.inst.TriggerLuaEvent("Check_SystemUnlock", (int)level);
        HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 6, (int)level);
        HotfixBridge.inst.TriggerLuaEvent("Check_DirectPurchasePushByLevel", (int)level);
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_GiftSevenBtn");

        if (level >= WorldParConfigManager.inst.GetConfig(100).parameters)
        {
            EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_REQUESTDATA);
        }

        if (lastLv < WorldParConfigManager.inst.GetConfig(180).parameters && level >= WorldParConfigManager.inst.GetConfig(180).parameters)
        {
            EventController.inst.TriggerEvent(GameEventType.MenuEvent.REFRESHLUXURYBTN);
        }

        if (lastLv < WorldParConfigManager.inst.GetConfig(126).parameters && level >= WorldParConfigManager.inst.GetConfig(126).parameters)
        {
            EventController.inst.TriggerEvent(GameEventType.MenuEvent.REFRESHONLINEREWARDBTNS);
        }

        if (lastLv < WorldParConfigManager.inst.GetConfig(330).parameters && level >= WorldParConfigManager.inst.GetConfig(330).parameters)
        {
            HotfixBridge.inst.TriggerLuaEvent("Request_RefugeInfo");
        }

        HotfixBridge.inst.TriggerLuaEvent("Refresh_MainRankBtnActive");
        Spine.Unity.AttachmentTools.AtlasUtilities.ClearCache();
    }
    #endregion
    #region 任务方法
    public void GetDailyTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_DailyTask_Data)msg;

        RefreshTaskData(data.taskList);
        RefreshTaskLivenessData(data.activeRewardList);

        if (toEndLeftTimer != 0)
        {
            GameTimer.inst.RemoveTimer(toEndLeftTimer);
            toEndLeftTimer = 0;
        }
        toEndLeftTimer = GameTimer.inst.AddTimer(data.activeRefreshTime + 1, 1, () => EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_GET_DATALIST), GameTimerType.byServerTime);

        this.task_activePoint = data.activePoint;

        this.task_nextEndTime = data.nextTime + (int)GameTimer.inst.serverNow;
        this.task_refreshEndTime = data.refreshTime + (int)GameTimer.inst.serverNow;
        this.task_refreshNumber = data.refreshNumber;


        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_COLLTIMEDOWN);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_RESHOWTASKPANEL);
    }

    public void GetDailyTaskRefresh(HttpMsgRspdBase msg)
    {
        var data = (Response_DailyTask_Refresh)msg;

        RefreshTaskData(data.taskList);
        RefreshTaskLivenessData(data.activeRewardList);

        this.task_activePoint = data.activePoint;

        this.task_nextEndTime = data.nextTime + (int)GameTimer.inst.serverNow;
        this.task_refreshEndTime = data.refreshTime + (int)GameTimer.inst.serverNow;
        this.task_refreshNumber = data.refreshNumber;

        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_COLLTIMEDOWN);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_RESHOWTASKPANEL);
    }

    public void GetDailyTaskAward(HttpMsgRspdBase msg)
    {
        var data = (Response_DailyTask_Reward)msg;

        //  PlatformManager.inst.GameHandleEventLog("Task", "");
        //  ("Task", "");

        RefreshTaskData(data.taskList);
        RefreshTaskLivenessData(data.activeRewardList);

        this.task_activePoint = data.activePoint;

        this.task_nextEndTime = data.nextTime + (int)GameTimer.inst.serverNow;
        this.task_refreshEndTime = data.refreshTime + (int)GameTimer.inst.serverNow;
        this.task_refreshNumber = data.refreshNumber;

        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_COLLTIMEDOWN);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_RESHOWTASKPANELANIM, data.taskId);

        foreach (var item in data.itemList)
        {
            DealWithAward(new AccessoryData(item));
        }
    }

    public void GetDailyActiveBoxAward(HttpMsgRspdBase msg)
    {
        var data = (Response_Active_Reward)msg;

        RefreshTaskData(data.taskList);
        RefreshTaskLivenessData(data.activeRewardList);

        this.task_activePoint = data.activePoint;

        this.task_nextEndTime = data.nextTime + (int)GameTimer.inst.serverNow;
        this.task_refreshEndTime = data.refreshTime + (int)GameTimer.inst.serverNow;
        this.task_refreshNumber = data.refreshNumber;

        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_COLLTIMEDOWN);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_RESHOWTASKPANEL);

        foreach (var item in data.itemList)
        {
            DealWithAward(new AccessoryData(item));
        }
    }


    public void OnDailyTaskChanged(HttpMsgRspdBase msg)
    {
        var data = (Response_DailyTask_Change)msg;
        if (_taskDic.ContainsKey(data.task.taskId))
        {
            _taskDic[data.task.taskId] = new TaskData(data.task);
        }
        else
        {
            Logger.error("未找到该id的任务  id = " + data.task.taskId);
        }

        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_CONTENTCHANGE, GetTaskDataByTaskId(data.task.taskId));
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_RESHOWTASKPANEL);
    }

    public void RefreshTaskData(List<OneDailyTask> dailyTasks)
    {
        _taskDic.Clear();
        foreach (var item in dailyTasks)
        {
            _taskDic.Add(item.taskId, new TaskData(item));
        }

        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint);
    }

    public void RefreshTaskLivenessData(List<OneActiveReward> boxItems)
    {
        _activeBoxDic.Clear();

        foreach (var item in boxItems)
        {
            _activeBoxDic.Add(item.activeRewardId, new ActiveRewardBoxData(item));
        }

        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint);
    }

    public TaskData GetTaskDataByTaskId(int taskId)
    {
        if (_taskDic.ContainsKey(taskId))
        {
            return _taskDic[taskId];
        }

        Logger.error("没有id是" + taskId + "的任务");
        return null;
    }

    public ActiveRewardBoxData GetActiveRewardBoxData(int activeTaskId)
    {
        if (_activeBoxDic.ContainsKey(activeTaskId))
        {
            return _activeBoxDic[activeTaskId];
        }

        Logger.error("没有id是" + activeTaskId + "的活跃宝箱数据");
        return null;
    }

    public string GetTaskName(TaskData data, bool isTopShow = false)
    {
        string name = LanguageManager.inst.GetValueByKey(data.name);

        switch ((EDailyTaskType)data.taskType)
        {
            case EDailyTaskType.SellItem:
            case EDailyTaskType.MakeItem:

                EquipDrawingsConfig equipConfig = EquipConfigManager.inst.GetEquipDrawingsCfg(data.taskTargetId);

                name = name.Replace("{0}", LanguageManager.inst.GetValueByKey(equipConfig.name));
                if (isTopShow)
                    name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
                else
                    name = name.Replace("{1}", data.parameter_2.ToString());
                break;

            case EDailyTaskType.Double:
            case EDailyTaskType.Discount:
            case EDailyTaskType.Chat:
            case EDailyTaskType.Promote:

                if (isTopShow)
                    name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
                else
                    name = name.Replace("{1}", data.parameter_2.ToString());
                break;

            case EDailyTaskType.SellAmount:

                EquipClassification equipTypeCfg = EquipConfigManager.inst.GetEquipTypeByID(data.taskTargetId);

                name = name.Replace("{0}", LanguageManager.inst.GetValueByKey(equipTypeCfg.name));
                if (isTopShow)
                    name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
                else
                    name = name.Replace("{1}", data.parameter_2.ToString());
                break;

            case EDailyTaskType.ExploreItem:

                itemConfig cfg = ItemconfigManager.inst.GetConfig(data.taskTargetId);

                name = name.Replace("{0}", LanguageManager.inst.GetValueByKey(cfg.name));
                if (isTopShow)
                    name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
                else
                    name = name.Replace("{1}", data.parameter_2.ToString());
                break;


            case EDailyTaskType.MarketSellGold:
            case EDailyTaskType.MarketSellEquip:
            case EDailyTaskType.ExploreHero:
            case EDailyTaskType.RefreshBar:
            case EDailyTaskType.BuildCost:
            case EDailyTaskType.ScienceCost:
            case EDailyTaskType.Gacha:
                if (isTopShow)
                    name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
                else
                    name = name.Replace("{1}", data.parameter_2.ToString());
                break;
        }


        return name;
    }

    public void DealWithAward(AccessoryData accessory)
    {

        switch (accessory.type)
        {
            case ItemType.Glod:
            //case ItemType.Gem:
            case ItemType.Energy:
            case ItemType.Active:
            case ItemType.UnionCoin_self:
            case ItemType.UnionCoin_union:
            case ItemType.UnionRenown:
            case ItemType.MakeSlotNum:
            case ItemType.ExploreSlotNum:
            case ItemType.ShopSizeNum:
            case ItemType.HeroSlotNum:
            case ItemType.MarketSlotNum:
                //case ItemType.Activity_WorkerGameCoin:
                break;
            case ItemType.EquipmentDrawing:
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.ActivateDrawing, "", accessory.config.effect, 0, 1));
                break;

            default:
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", accessory.itemType == (int)ItemType.Equip ? accessory.itemId : 0, accessory.itemType == (int)ItemType.Equip ? 0 : accessory.itemId, accessory.count));
                break;
        }

    }
    #endregion
    #region 资源方法
    private void GetResourceProductionList(HttpMsgRspdBase msg)
    {
        var resList = (Response_Resource_ProductionList)msg;

        foreach (var item in resList.productionList)
        {
            ItemBagProxy.inst.updateItemNum(item.itemId, item.count);
            updateProduction(item.itemId, item.lastCollectTime, item.nextCollectTime, item.collectSpeedTime, item.countLimit, item.lastBuyTime, item.dailyBuyLimit);
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.RES_PRODUCTIONLIST_REFRESHUI, item.itemId);
        }

        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIREFRESH_UPDATEPRODUCTIONTIME);
    }

    private void GetResourceProductionRefresh(HttpMsgRspdBase msg)
    {
        var data = (Response_Resource_ProductionRefresh)msg;
        ItemBagProxy.inst.updateItemNum(data.resource.itemId, data.resource.count);
        updateProduction(data.production.itemId, data.production.lastCollectTime, data.production.nextCollectTime, data.production.collectSpeedTime, data.production.countLimit, data.production.lastBuyTime, data.production.dailyBuyLimit);
        //TODO bug 前面的刷新顶掉了界面
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.RES_PRODUCTIONLIST_REFRESHUI, data.resource.itemId);
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIREFRESH_PRODUCTIONREFRESHCOM, data.production.itemId);
    }

    private void GetResourceProductionChange(HttpMsgRspdBase msg)
    {
        var data = (Response_Resource_ProductionChange)msg;
        ItemBagProxy.inst.updateItemNum(data.production.itemId, data.production.count);
        updateProduction(data.production.itemId, data.production.lastCollectTime, data.production.nextCollectTime, data.production.collectSpeedTime, data.production.countLimit, data.production.lastBuyTime, data.production.dailyBuyLimit);
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.RES_PRODUCTIONLIST_REFRESHUI, data.production.itemId);
    }

    public ResourceProduction GetResProduction(int itemId)
    {
        if (resSlotList.ContainsKey(itemId))
        {
            return resSlotList[itemId];
        }
        return null;
    }

    public int GetResCountLimit(int resid)
    {
        if (resSlotList.ContainsKey(resid))
        {
            return (int)resSlotList[resid].countLimit;
        }
        return 0;
    }
    public void updateProduction(int itemId, double lastCollectTime, double nextCollectTime, int collectTime, int limit, int unionBuyCountdownTime, int unionCanBuyTimes)
    {
        if (resSlotList.ContainsKey(itemId))
        {
            resSlotList[itemId].resItemId = itemId;
            resSlotList[itemId].duration = collectTime;
            resSlotList[itemId].nextCollectTime = nextCollectTime;
            resSlotList[itemId].lastCollectTime = lastCollectTime;
            // resSlotList[itemId].time = collectTime - nextCollectTime;
            resSlotList[itemId].countLimit = limit;
            resSlotList[itemId].unionBuyCountdownTime = unionBuyCountdownTime;
            resSlotList[itemId].unionCanBuyTimes = unionCanBuyTimes;
        }
        else
        {
            var rp = new ResourceProduction();
            rp.isActivate = true; //是否激活
            rp.resItemId = itemId;
            rp.duration = collectTime;
            rp.nextCollectTime = nextCollectTime;
            rp.countLimit = limit;
            rp.lastCollectTime = lastCollectTime;
            rp.unionBuyCountdownTime = unionBuyCountdownTime;
            rp.unionCanBuyTimes = unionCanBuyTimes;
            resSlotList.Add(itemId, rp);
        }
    }
    #endregion
    #region 城市方法
    private void getAllCityBuildingDataResp(HttpMsgRspdBase msg)
    {
        var data = (Response_City_BuildData)msg;
        foreach (var item in data.buildData)
        {
            updateBuildingInfo(item);
        }

        cityIsInit = true;

        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_ALLBUILDINGUPREFRESH);
    }

    private void getCityBuildingDataResp(HttpMsgRspdBase msg)
    {
        var data = (Response_City_BuildDataChanged)msg;

        CityBuildingData oldBuildingData = GetBuildingData(data.buildData.buildId);

        if (oldBuildingData == null) return;
        var oldState = oldBuildingData.state;
        var oldLevel = oldBuildingData.level;

        updateBuildingInfo(data.buildData);

        var info = data.buildData;
        var newBuildingData = new CityBuildingData();
        newBuildingData.SetInfo(info.buildId, info.buildLevel, info.buildState, info.oneSelfCostCount, info.buildCostCount);

        bool needChgNew = false;
        if (oldState == (int)EBuildState.EB_Lock && newBuildingData.state == (int)EBuildState.EB_Unlock) //建筑物解锁
        {
            if (newBuildingData.config.architecture_type == (int)kCityBuildingType.Science)
            {
                needChgNew = true;
            }
            Logger.log($"建筑物 { newBuildingData.config.name} 已解锁");
        }
        else
        {
            if (oldLevel < newBuildingData.level) //建筑物升级
            {
                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HIDEUI_CITYBUILDINGINVEST);
                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.SHOWUI_BUILDINGUPFINISH, newBuildingData);

                if (newBuildingData.config.architecture_type == (int)kCityBuildingType.Science) //升级一次科学工艺
                {
                    PlatformManager.inst.GameHandleEventLog("Research_Upgrade", "");
                }
                else if (newBuildingData.config.architecture_type == (int)kCityBuildingType.Resource) //升级一次建筑
                {
                    PlatformManager.inst.GameHandleEventLog("Building_Upgrade", "");
                }

            }
            else//建筑物投资进度更新
            {
                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.INVESTUI_SETDATA, newBuildingData);
            }
        }


        if (needChgNew)
        {
            GetBuildingData(data.buildData.buildId).isNew = true;
            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_BUILDINGUPREFRESH, 2300);
            //EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_SCIENCEBUILDINGREFRESH);
        }

        //EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_BUILDINGUPREFRESH, data.buildData.buildId);  加了"投资"字样 hud改为全部刷新
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_ALLBUILDINGUPREFRESH);
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.SCIENCELABUI_REFRESH);
    }

    private void getBuildingRankDataResp(HttpMsgRspdBase msg)
    {
        var data = (Response_City_BuildingRankData)msg;

        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.INVEST_REFRESHUNIONRANK, data.buildTopList);
    }

    public CityBuildingData GetBuildingData(int buildingId)
    {
        return cityBuildings.Find((t) => t.buildingId == buildingId);
    }

    public List<CityBuildingData> GetAllScienceBuildingData(kCityBuildingType buildingType)
    {
        return cityBuildings.FindAll((t) => t.config != null && t.config.architecture_type == (int)buildingType);
    }

    public List<CityBuildingData> GetAllCanShowScienceBuildingData() 
    {
        List<CityBuildingData> list = new List<CityBuildingData>();

        var scienceList = GetAllScienceBuildingData(kCityBuildingType.Science);

        list = scienceList.FindAll((t) => 
        {
          
            if (t.state == (int)EBuildState.EB_Lock)
            {
                if (t.config.unlock_type == (int)kCityBuildingUnlockType.NeedOneWorker)
                {

                    WorkerData workerData = RoleDataProxy.inst.GetWorker(t.config.unlock_id);

                    if (workerData != null)
                    {
                        return workerData.state == EWorkerState.CanUnlock;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
            else
            {
                return true;
            }
        });


        return list;
    }

    void updateBuildingInfo(BuildData info)
    {
        CityBuildingData data = GetBuildingData(info.buildId);

        if (data == null)
        {
            data = new CityBuildingData();
            data.SetInfo(info.buildId, info.buildLevel, info.buildState, info.oneSelfCostCount, info.buildCostCount);
            cityBuildings.Add(data);
        }
        else
        {
            data.SetInfo(info.buildId, info.buildLevel, info.buildState, info.oneSelfCostCount, info.buildCostCount);
        }
    }


    //获取建筑物对 探险组 康复时间减少 的 百分比
    public float GetExploreGroupRestTimeSpeedUp(int instanceId, bool isLastLv = false)
    {
        var data = GetBuildingData(3700); //获取康复中心的建筑数据

        float selfEffect = 0;//个人加成

        if (data == null)
        {
            return 0;
        }
        else
        {
            for (int i = isLastLv ? data.level - 1 : data.level; i >= 1; i--)
            {
                var upgradeCfg = BuildingUpgradeConfigManager.inst.GetConfig(3700, i);
                if (upgradeCfg.effect_id == instanceId)
                {
                    selfEffect = upgradeCfg.effect_val;
                    break;
                }
            }
        }

        return selfEffect;
    }

    //获取建筑物对 探险材料 获取产量增加 的 百分比
    public float GetExploreDropMaterialOutputUp(int itemId, bool isLastLv = false)
    {
        var data = GetBuildingData(3800); //获取探险中心的建筑数据

        float selfEffect = 0;//个人加成

        if (data == null)
        {
            return 0;
        }
        else
        {
            for (int i = isLastLv ? data.level - 1 : data.level; i >= 1; i--)
            {
                var upgradeCfg = BuildingUpgradeConfigManager.inst.GetConfig(3800, i);
                if (upgradeCfg.effect_id == itemId)
                {
                    selfEffect = upgradeCfg.effect_val;
                    break;
                }
            }

        }

        return selfEffect;
    }

    #endregion
    #region 地图方法
    public int getFurnitureNum()
    {
        if (indoorData != null)
            return indoorData.allIndoorEntity.Count();
        return 0;
    }
    /// <summary>
    /// 初始化等级场景地格
    /// </summary>
    /// <returns></returns>
    public void InitIndoorData(Response_Design_Data data)
    {
        indoorData.InitIndoorData(data.shopData, data.floorData, data.wallData, data.furnitureList);
        hasIndoorData = true;
    }

    public Vector3 GetOneTrunkPosition(out int trunkUid)
    {
        IndoorData.ShopDesignItem trunk = indoorData.storageBoxList.GetRandomElement();
        if (trunk != null && IndoorMap.inst.gameMapGrid)
        {
            trunkUid = trunk.uid;
            return MapUtils.CellPosToCenterPos(MapUtils.IndoorCellposToMapCellPos(new Vector3Int(trunk.x, trunk.y, 0)));
        }

        trunkUid = -1;
        return Vector3.zero;
    }
    public IndoorData.ShopDesignItem GetFuriture(int uid)
    {
        if (indoorData.allIndoorEntity.ContainsKey(uid))
            return indoorData.allIndoorEntity[uid];
        return null;
    }

    //随机获取一个宠物uid 无则返回-1
    public int GetOnePetUid()
    {
        var list = PetDataProxy.inst.GetNotStorePetDatas();

        list = list.FindAll(t =>
        {
            if (IndoorRoleSystem.inst == null) return false;

            var pet = IndoorRoleSystem.inst.GetPetByUid(t.petUid);

            if (pet == null || pet.GetCurState() == MachinePetState.stayDoorway) return false;

            return true;
        });

        if (list.Count > 0)
        {
            return list[Random.Range(0, list.Count)].petUid;
        }

        return -1;
    }

    //随机获取一个空货架 无则返回-1
    public int GetOneEmptyShelfUid()
    {
        List<int> uids = new List<int>();
        foreach (var item in indoorData.shelfList)
        {
            if (item.equipList.Count == 0)
            {
                uids.Add(item.uid);
            }
        }

        if (uids.Count > 0)
        {
            return uids[Random.Range(0, uids.Count)];
        }

        return -1;
    }


    //随即获取一个货架
    public int GetOneShelfUid()
    {
        var list = indoorData.shelfList.FindAll(t => t.state != (int)EDesignState.InStore);

        var count = list.Count;
        if (count > 0)
        {
            return list[Random.Range(0, count)].uid;
        }
        return -1;
    }

    //根据装备uid获取对应的货架
    public int GetOneShelfUid(string targetEquipUid)
    {
        foreach (var shelf in indoorData.shelfList)
        {

            ShelfEquip targetEquip = shelf.equipList.Find(item => item.equipUid == targetEquipUid);

            if (targetEquip != null)
            {
                return shelf.uid;
            }
        }

        return -1;
    }

    //根据装备uid获取所有对应的货架
    public List<int> GetShelfUidsByEquipUid(string targetEquipUid)
    {

        List<int> results = new List<int>();

        foreach (var shelf in indoorData.shelfList)
        {

            ShelfEquip targetEquip = shelf.equipList.Find(item => item.equipUid == targetEquipUid);

            if (targetEquip != null)
            {
                results.Add(shelf.uid);
            }
        }

        return results;

    }

    //根据装备id获取对应的货架
    public List<int> GetShelfUidsByEquipId(int targetEquipId) 
    {
        List<int> results = new List<int>();

        foreach (var shelf in indoorData.shelfList)
        {

            ShelfEquip targetEquip = shelf.equipList.Find(item => item.equipId == targetEquipId);

            if (targetEquip != null)
            {
                results.Add(shelf.uid);
            }
        }

        return results;
    }

    //根据装备类型获取相应类型的货架
    public int GetOneShelfUid(EquipSubType equipSubType)
    {
        int type_2 = (int)equipSubType;

        foreach (var shelf in indoorData.shelfList)
        {
            ShelfUpgradeConfig shelfConfig = ShelfUpgradeConfigManager.inst.getConfigByType(shelf.config.type_2, shelf.level);


            for (int i = 0; i < 12; i++)
            {
                int[] fields = shelfConfig.getFieldByLevel(i + 1);

                if (fields[0] != -1 && fields[0] != 0)
                {
                    if (fields.Contains(type_2))
                    {
                        return shelf.uid;
                    }
                }
            }
        }

        return -1;
    }


    //随即获取一个装饰物
    public int GetOneDecorUid()
    {
        var list = indoorData.decorList.FindAll(t => t.state != (int)EDesignState.InStore);

        var count = list.Count;
        if (count > 0)
        {
            return list[Random.Range(0, count)].uid;
        }
        return -1;
    }

    //随机获取一个家具（装饰或货架）
    public int GetOneFuritureUid()
    {
        if (indoorData.shelfList.Count == 0 && indoorData.decorList.Count == 0)
        {
            return -1;
        }
        else
        {
            if (Random.Range(0, 1) == 0)
            {
                if (indoorData.shelfList.Count == 0) return GetOneDecorUid();

                return GetOneShelfUid();
            }
            else
            {
                if (indoorData.decorList.Count == 0) return GetOneShelfUid();

                return GetOneDecorUid();
            }
        }
    }

    public List<int> GetRanShelfUids(int n, List<int> shields = null)
    {
        List<int> result = new List<int>();

        var list = indoorData.shelfList.FindAll(t => t.state != (int)EDesignState.InStore);

        int shelfCount = list.Count;


        int hasShelfNum = 0;

        if (shields != null)
        {
            foreach (var item in shields)
            {
                indoorData.shelfList.Find(t => t.uid == item);
                hasShelfNum++;
            }
        }

        int num = 0;
        while (num < n && result.Count + hasShelfNum < shelfCount)
        {
            int uid = GetOneShelfUid();

            if (uid == -1) break;

            if ((shields != null && shields.Contains(uid)) || result.Contains(uid)) continue;

            result.Add(uid);
            num++;
        }

        return result;
    }

    public List<int> GetRanDecorUids(int n, List<int> shields = null)
    {
        List<int> result = new List<int>();

        var list = indoorData.decorList.FindAll(t => t.state != (int)EDesignState.InStore);

        int decorCount = list.Count;

        int hasDecorNum = 0;

        if (shields != null)
        {
            foreach (var item in shields)
            {
                indoorData.decorList.Find(t => t.uid == item);
                hasDecorNum++;
            }
        }

        int num = 0;
        while (num < n && result.Count + hasDecorNum < decorCount)
        {
            int uid = GetOneDecorUid();

            if (uid == -1) break;

            if ((shields != null && shields.Contains(uid)) || result.Contains(uid)) continue;

            result.Add(uid);
            num++;
        }

        return result;
    }

    /// <summary>
    /// 随机获取n个家具（装饰或家具）
    /// </summary>
    /// <param name="shields">已加入的家具ids</param>
    /// <param name="decorNum">需要多少个装饰</param>
    /// <param name="n">需要随机几个</param>
    /// <returns></returns>
    public List<int> GetRanFuritureUids(int n, int decorNum, List<int> shields = null)
    {
        List<int> result = new List<int>();

        if (shields != null) result.AddRange(shields);

        var decors = GetRanDecorUids(decorNum, result);
        //Logger.error("随机到的装饰数量： " + decors.Count);
        result.AddRange(decors);
        result.AddRange(GetRanShelfUids(n - decors.Count, result));
        return result;
    }

    public bool CheckStorableType(int type1)
    {
        var tp = (kTileGroupType)type1;
        switch (tp)
        {
            case kTileGroupType.Counter:
                return true;
            case kTileGroupType.Shelf:
                return true;
            case kTileGroupType.Trunk:
                return true;
            case kTileGroupType.ResourceBin:
                return true;
        }
        return false;
    }

    public int GetFuritureCount(int furitureid)
    {
        int count = 0;
        var cfg = FurnitureConfigManager.inst.getConfig(furitureid);
        if (cfg != null)
        {
            List<IndoorData.ShopDesignItem> furnitures = GetEntitys((kTileGroupType)cfg.type_1);

            bool isStorable = CheckStorableType(cfg.type_1);

            foreach (var fur in furnitures)
            {

                if (isStorable)
                {
                    if (cfg.type_2 == fur.config.type_2)
                    {
                        count++;
                    }
                }
                else
                {
                    if (cfg.id == fur.id)
                    {
                        count++;
                    }
                }


            }
        }
        return count;
    }
    public IndoorData.ShopDesignItem GetCounter()
    {
        if (hasIndoorData)
        {
            return indoorData.counter;
        }
        return null;
    }
    public int GetResourceBinCount(int resitemid)
    {
        var binList = GetEntitys(kTileGroupType.ResourceBin);
        if (binList == null || binList.Count <= 0) return 0;
        var list = binList.FindAll(item => item.resItemId == resitemid);
        return list == null ? 0 : list.Count;
    }


    public List<IndoorData.ShopDesignItem> GetEntitys(kTileGroupType type)
    {
        if (!hasIndoorData) return null;
        switch (type)
        {
            case kTileGroupType.Floor:
                return indoorData.floorList;
            case kTileGroupType.Wall:   //墙纸
                return indoorData.wallPaperList;
            case kTileGroupType.Carpet:     //地毯
                return indoorData.floorClothList;
            case kTileGroupType.Shelf:      //货架
                return indoorData.shelfList;
            case kTileGroupType.Trunk:      //仓库
                return indoorData.storageBoxList;
            case kTileGroupType.ResourceBin: //资源箱子
                return indoorData.resBasketList;
            case kTileGroupType.Furniture:  //室内装饰
                return indoorData.decorList;
            case kTileGroupType.OutdoorFurniture: //室外装饰
                return indoorData.outdoorDecor;
            case kTileGroupType.WallFurniture:  //墙上挂件
                return indoorData.wallDecorList;
            case kTileGroupType.PetHouse:    //宠物小家
                return indoorData.petHouseList;
        }
        return null;
    }

    public RectInt GetIndoorSize()
    {
        return indoorData.size;
    }

    public bool FurnitureCanUpgradeFinish(int furnitureUid)
    {

        var item = GetFuriture(furnitureUid);
        if (item == null || IndoorMap.inst == null) return false;

        BaseUpgradeConfig config = null;


        //通过家具的等级与类型来获得对应家具对应等级的配置并赋给类中的全局变量Config
        switch (item.config.type_1)
        {
            //柜台
            case (int)kTileGroupType.Counter:
                {
                    config = CounterUpgradeConfigManager.inst.getConfig(item.level + 1);
                    break;
                }

            //资源篮
            case (int)kTileGroupType.ResourceBin:
                {
                    config = ResourceBinUpgradeConfigManager.inst.getConfigByType(item.config.type_2, item.level + 1);
                    break;
                }

            //货架
            case (int)kTileGroupType.Shelf:
                {
                    config = ShelfUpgradeConfigManager.inst.getConfigByType(item.config.type_2, item.level + 1);
                    break;
                }

            //储物箱
            case (int)kTileGroupType.Trunk:
                {
                    config = TrunkUpgradeConfigManager.inst.getConfig(item.level + 1);
                    break;
                }

            default:
                {
                    Debug.LogError("检查家具升级空间是否足够 未能找到对应类别");
                    break;
                }
        }

        if (config != null)
        {

            if (config.furniture_id != item.id)
            {
                if (IndoorMap.inst.GetFurnituresByUid(furnitureUid, out Furniture furnitureEntity))
                {
                    //先把它自身设置为不占取格子
                    IndoorMap.inst.SetIndoorGridFlags(furnitureUid, furnitureEntity.cellpos.x, furnitureEntity.cellpos.y, furnitureEntity.dir, 0);

                    FurnitureConfig furnitureCfg = FurnitureConfigManager.inst.getConfig(config.furniture_id);
                    if (furnitureCfg != null)
                    {
                        bool furnitureHandleResults = IndoorMap.inst.CheckFurnituresOccGrid(furnitureCfg, furnitureEntity.cellpos.x, furnitureEntity.cellpos.y, furnitureEntity.dir);


                        if (!furnitureHandleResults)
                        {

                            if (IndoorMapEditSys.inst.currEntityUid != furnitureUid) //当前选中对象与升级对象不一致时
                            {
                                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI);

                                if (IndoorMapEditSys.inst.currEntityUid == -1)
                                {
                                    EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, furnitureUid);
                                    furnitureEntity.OnSelected();
                                }
                                else
                                {
                                    //设置为占取格子
                                    IndoorMap.inst.SetIndoorGridFlags(furnitureUid, furnitureEntity.cellpos.x, furnitureEntity.cellpos.y, furnitureEntity.dir, 1);
                                }
                            }

                            if (IndoorMapEditSys.inst.shopDesignMode != DesignMode.FurnitureEdit)
                            {
                                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.FurnitureEdit, furnitureUid);
                            }

                            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("空间不足以升级"), GUIHelper.GetColorByColorHex("FFD907"));
                        }
                        else
                        {
                            if (IndoorMapEditSys.inst.currEntityUid != -1 && IndoorMapEditSys.inst.currEntityUid != furnitureUid)
                            {
                                //设置为占取格子
                                IndoorMap.inst.SetIndoorGridFlags(furnitureUid, furnitureEntity.cellpos.x, furnitureEntity.cellpos.y, furnitureEntity.dir, 1);
                            }
                        }

                        return furnitureHandleResults;
                    }
                    else
                    {
                        if (IndoorMapEditSys.inst.currEntityUid != -1 && IndoorMapEditSys.inst.currEntityUid != furnitureUid)
                        {
                            //设置为占取格子
                            IndoorMap.inst.SetIndoorGridFlags(furnitureUid, furnitureEntity.cellpos.x, furnitureEntity.cellpos.y, furnitureEntity.dir, 1);
                        }
                    }
                }
            }
            else
            {
                //Logger.error("模型未改变 直接升级");
                return true;
            }

        }

        return false;
    }

    public int GetCurrentUpgradefurniture()
    {
        foreach (var f in indoorData.allIndoorEntity.Values)
        {
            if (f.type == 2) continue;
            if (f.state == 1 || f.state == 2)
            {
                return f.uid;
            }
        }
        return 0;
    }

    public IndoorData.ShopDesignItem getNearFurniture(int uid, int type, bool isLeft)
    {
        var list = GetEntitys((kTileGroupType)type).FindAll((t) => t.state != (int)EDesignState.InStore);
        int index = list.FindIndex(t => t.uid == uid);
        index += (isLeft ? -1 : 1);

        if (index == list.Count) index = 0;
        if (index == -1) index = list.Count - 1;

        return list[index];
    }
    #region 数据刷新

    public void updateShopData(ShopData shopData)
    {
        indoorData.setShopData(shopData);
        D2DragCamera.inst.updateCameMaxZoom(shopData.shopLevel);
        EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_CALLTAO_SHOPUPGRADE);
    }

    public void updateFurnitureData(OneFurniture data)
    {
        bool changeModel = false;
        var furniture = GetFuriture(data.furnitureUid);
        if (furniture != null)
        {
            if (data.furnitureId != furniture.id)
                changeModel = true;
        }
        indoorData.SetFurnitureData(data);
        //刷新家具数据事件
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Data_Update, data.furnitureUid, changeModel);
        //刷新存放上限显示
        if (IndoorMapEditSys.inst != null)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOW_FurnitureNumLimit, IndoorMapEditSys.inst.shopDesignMode != DesignMode.normal);
        }
    }

    public void setFurniturePos(int uid, int x, int y, int dir)
    {
        if (indoorData.allIndoorEntity.ContainsKey(uid))
        {
            var data = indoorData.allIndoorEntity[uid];
            data.x = x;
            data.y = y;
            data.dir = dir;
            //服务器验证
            moveItem(data.type, uid, x, y, dir);
        }
    }
    //移动
    public void moveItem(int type_1, int uid, int _x, int _y, int rot)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Move()
            {
                designType = type_1,
                uid = uid,
                x = _x,
                y = _y,
                rotate = rot,
            }
        });
    }

    //购买
    public void buyItem(int id, int type, int _x, int _y, int rot)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Buy()
            {
                designType = type,
                x = _x,
                y = _y,
                furnitureId = id,
                rotate = rot
            }
        });
    }

    public void requestLayout()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Data()
        });
        if (mapdataIsReady)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ServerData_Ready);
        }
    }

    //刷新地板
    public void setFloor(RectInt size, int floorid, bool buy)
    {
        if (floorid <= 0) return;
        List<OneFloor> list = new List<OneFloor>();
        shopData.floorList.ForEach(floor =>
        {
            if (floor.x >= size.xMin && floor.x < size.xMax && floor.y >= size.yMin && floor.y < size.yMax)
            {
                floor.id = floorid;
            }
            OneFloor f = new OneFloor();
            f.x = floor.x;
            f.y = floor.y;
            f.furnitureId = floor.id;
            list.Add(f);
        });
        var buyfloorid = buy ? floorid : 0;
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_SetFloor()
            {
                buyFloorId = buyfloorid,
                floorList = list
            }
        });
    }
    public void requestFloor()
    {
        shopData.updateBaseMapFloor();
        List<OneFloor> list = new List<OneFloor>();
        shopData.floorList.ForEach(floor =>
        {
            OneFloor f = new OneFloor();
            f.x = floor.x;
            f.y = floor.y;
            f.furnitureId = floor.id;
            list.Add(f);
        });
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_SetFloor()
            {
                floorList = list
            }
        }
        );
    }
    //修改墙纸
    public void SetWall(int start, int count, int wallid, bool buy)
    {
        if (wallid <= 0) return;
        List<OneWall> list = new List<OneWall>();
        shopData.wallPaperList.ForEach(wall =>
        {
            OneWall w = new OneWall();
            w.index = wall.index;
            w.x = wall.x;
            w.y = wall.y;
            if (wall.index < start || wall.index >= (start + count))
            {
                w.furnitureId = wall.id;
            }
            else
            {
                w.furnitureId = wallid;
            }
            list.Add(w);
        });
        var buywallid = buy ? wallid : 0;
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_SetWall()
            {
                buyWallId = buywallid,
                wallList = list
            }
        }
        );
    }
    //刷新墙纸
    public void requestWall()
    {
        shopData.updateBaseMapWall();
        List<OneWall> list = new List<OneWall>();
        shopData.wallPaperList.ForEach((wall) =>
        {
            OneWall w = new OneWall();
            w.x = wall.x;
            w.y = wall.y;
            w.index = wall.index;
            w.furnitureId = wall.id;
            list.Add(w);
        });
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_SetWall()
            {
                wallList = list
            }
        });
    }
    #endregion

    void OnDesignSetFloorResp(HttpMsgRspdBase msg)
    {
        if (IndoorMap.inst == null || !IndoorMap.inst.isInit) return;
        var data = (Response_Design_SetFloor)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            shopData.InitIndoorFloorData(data.floorData);
        }
        //刷新
        if (IndoorMap.inst != null)
            IndoorMap.inst.RefreshFloor(shopData);
    }
    void OnDesignSetWallResp(HttpMsgRspdBase msg)
    {
        if (IndoorMap.inst == null || !IndoorMap.inst.isInit) return;
        var data = (Response_Design_SetWall)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            shopData.InitIndoorWallPaperData(data.wallData);
        }
        //刷新
        if (IndoorMap.inst != null)
            IndoorMap.inst.RefreshWall(shopData);
    }
    //HttpMsgRspdBase msg
    void onDesignInStoreResp(HttpMsgRspdBase msg)
    {
        var data = (Response_Design_InStore)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Store_Result, data.uid, data.designType);
            if ((EDesignType)data.designType == EDesignType.PetFurniture)
            {

            }
            // EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Show_EditMenus);
        }
    }
    void onDesignBuy(HttpMsgRspdBase msg)
    {
        var data = (Response_Design_Buy)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            var furnitureCfg = FurnitureConfigManager.inst.getConfig(data.furnitureId);
            if (furnitureCfg != null)
            {
                if (furnitureCfg.id == 18001)
                {
                    PlatformManager.inst.GameHandleEventLog("Res_Woodbin", "");
                }
                else if (furnitureCfg.id == 20001)
                {
                    PlatformManager.inst.GameHandleEventLog("Res_Chemical", "");
                }
                else if ((kTileGroupType)furnitureCfg.type_1 == kTileGroupType.Furniture)
                {
                    PlatformManager.inst.GameHandleEventLog("Decorate", "");
                }
            }

        }
        else
        {
            //购买失败 可能是外挂自行发送的
            //Debug.LogError("Response_Design_Buy:失败， 检查前后端配置");
            if (IndoorMap.inst != null)
                IndoorMap.inst.ClearNewFurniture();
        }
    }

    bool mapdataIsReady = false;
    private void OnResponseDesignData(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Design_Data;
        data.furnitureList.Sort((a, b) => { return sortByLevel(a.furnitureId, b.furnitureId, a.level, b.level); });  //排个序
        InitIndoorData(data);
        if (!mapdataIsReady)
        {
            mapdataIsReady = true;
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ServerData_Ready);
        }

    }

    public void OnResponseDesignMove(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Design_Move;

        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Set_DesignChangedFlag);
        }
    }

    private void OnResponseDesignFurnitureChange(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Design_FurnitureChange;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            updateFurnitureData(data.furniture);

            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Set_DesignChangedFlag);
        }
    }

    private void ChangeShelfEquipList(HttpMsgRspdBase msg)
    {
        Response_Design_ShelfEquipChange data = (Response_Design_ShelfEquipChange)msg;
        IndoorData.ShopDesignItem shelf = GetFuriture(data.shelfUid);

        //Logger.error("Response_Design_ShelfEquipChange————————");

        if (data.onOrOff == 1)
        {
            EquipItem equip = ItemBagProxy.inst.GetEquipItem(data.shelfEquip.equipId);
            if (equip == null)
            {
                Logger.error("货架上架装备时 未在本地缓存找到该装备 equipId:" + data.shelfEquip.equipId + "   前端货架装备列表不添加此装备");
            }
            else
            {
                shelf.equipList.Add(data.shelfEquip);
                //Logger.error("添加货架装备  货架名称 : " + shelf.config.name + "   货架上装备数量： " + shelf.equipList.Count);
            }
        }
        else
        {
            //Logger.error("删除前  货架装备  货架名称 : " + shelf.config.name + "   货架上装备数量： " + shelf.equipList.Count);
            shelf.equipList.Remove(shelf.equipList.Find(t => t.fieldId == data.shelfEquip.fieldId));
            //Logger.error("删除后  货架装备  货架名称 : " + shelf.config.name + "   货架上装备数量： " + shelf.equipList.Count);

        }

        EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip, shelf, data);
    }
    int sortByLevel(int id1, int id2, int level1, int level2)
    {
        if (id1 < id2)
        {
            return -1;
        }
        else if (id1 > id2)
        {
            return 1;
        }
        else
        {
            return level1.CompareTo(level2);
        }
    }
    #endregion
    #region 联盟方法

    #region 援助
    private void GetUnionMemberHelpListData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_MemberHelpList)msg;

        helpFurnitureFlag = data.furnitureFlag;
        helpShopExtenFlag = data.shopFlag;

        onUnionMemberHelpListChg(data.helpList);
    }

    private void GetUnoinSetHelpData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_SetHelp)msg;

        helpFurnitureFlag = data.furnitureFlag;
        helpShopExtenFlag = data.shopFlag;

        onUnionMemberHelpListChg(data.helpList);
    }

    private void GetUnionHelpMemberData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_HelpMember)msg;

        onUnionMemberHelpListChg(data.helpList);
    }

    private void onUnionMemberHelpListChg(List<OneHelpData> helpList)
    {

        foreach (var item in union_helpList)
        {
            item.ClearTimer();
        }

        union_helpList.Clear();

        foreach (var data in helpList)
        {
            union_helpList.Add(new UnionMemberHelpData(data));
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_MEMBERHELPLISTREFRESH);
        HotfixBridge.inst.TriggerLuaEvent("UNION_MEMBERHELPLISTREFRESH");
    }
    #endregion

    #region 悬赏

    public UnionTaskData GetCheckTurnPageData(bool isLeft, UnionTaskData data)
    {

        var list = UnionTaskList.FindAll(t => t.data.state == (int)EUnionTaskState.Idle);

        int index = list.IndexOf(data);

        if (index == -1)
        {
            Logger.error("未能从已有的idle状态的联盟悬赏中筛选到此条任务 任务uid：" + data.data.taskUid);
            return data;
        }
        else
        {
            index += isLeft ? -1 : 1;

            if (index == -1)
            {
                index = list.Count - 1;
            }
            else if (index == list.Count)
            {
                index = 0;
            }
        }


        return list[index];
    }

    int uniontaskRefreshTimer, unionTaskCancelCoolTimer;

    private void refreshUnionTaskTimers(int unionTaskRefreshTime, int unionTaskCancelCoolTime, int unionTaskLevel, int unionTaskPoint)
    {
        this.unionTaskRefreshEndTime = unionTaskRefreshTime + (int)GameTimer.inst.serverNow;
        this.unionTaskCancelCoolEndTime = unionTaskCancelCoolTime + (int)GameTimer.inst.serverNow;
        this.unionTaskLevel = unionTaskLevel;
        this.unionTaskPoint = unionTaskPoint;


        if (uniontaskRefreshTimer != 0)
        {
            GameTimer.inst.RemoveTimer(uniontaskRefreshTimer);
            uniontaskRefreshTimer = 0;
        }

        if (unionTaskCancelCoolTimer != 0)
        {
            GameTimer.inst.RemoveTimer(unionTaskCancelCoolTimer);
            unionTaskCancelCoolTimer = 0;
        }


        if (unionTaskRefreshTime > 0)
        {
            uniontaskRefreshTimer = GameTimer.inst.AddTimer(unionTaskRefreshTime, 1, uniontaskRefreshTimerMethod, GameTimerType.byServerTime);
        }

        if (unionTaskCancelCoolTime > 0)
        {
            unionTaskCancelCoolTimer = GameTimer.inst.AddTimer(unionTaskCancelCoolTime, 1, unionTaskCancelCoolTimerMethod, GameTimerType.byServerTime);
        }

    }

    private void uniontaskRefreshTimerMethod()
    {
        if (unionTaskRefreshTime > 0)
        {
            uniontaskRefreshTimer = GameTimer.inst.AddTimer(unionTaskRefreshTime, 1, uniontaskRefreshTimerMethod, GameTimerType.byServerTime);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_TASKLIST);
        }
    }

    private void unionTaskCancelCoolTimerMethod()
    {
        if (unionTaskCancelCoolTime > 0)
        {
            unionTaskCancelCoolTimer = GameTimer.inst.AddTimer(unionTaskCancelCoolTime, 1, unionTaskCancelCoolTimerMethod, GameTimerType.byServerTime);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_TASKLIST);
        }
    }

    private void refreshUnoinTaskList(List<OneUnionTaskData> taskList)
    {
        foreach (var item in unionTaskList)
        {
            item.ClearTimer();
        }
        unionTaskList.Clear();

        foreach (var item in taskList)
        {
            UnionTaskData data = new UnionTaskData(item);
            unionTaskList.Add(data);
        }
    }

    private void refreshOneUnionTask(OneUnionTaskData data)
    {
        var oneData = unionTaskList.Find(t => t.data.taskUid == data.taskUid);

        if (oneData != null)
        {
            oneData.SetInfo(data);
        }
        else
        {
            Logger.error("给定公会任务中不包含此任务 任务uid：" + data.taskUid);
        }
    }

    private void refreshUnionTaskData(int unionTaskRefreshTime, int unionTaskCancelCoolTime, int unionTaskLevel, int unionTaskPoint, List<OneUnionTaskData> taskList)
    {
        refreshUnionTaskTimers(unionTaskRefreshTime, unionTaskCancelCoolTime, unionTaskLevel, unionTaskPoint);
        refreshUnoinTaskList(taskList);
    }

    private void getUnionTaskListData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_TaskList)msg;

        refreshUnionTaskData(data.unionTaskRefreshTime, data.unionTaskCancelCoolTime, data.unionTaskLevel, data.unionTaskPoint, data.taskList);

        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS);

        HotfixBridge.inst.TriggerLuaEvent("onUnionTaskDataChanged");

    }

    private void getUnionCheckTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_CheckUnionTask)msg;

        refreshOneUnionTask(data.taskData);

        HotfixBridge.inst.TriggerLuaEvent("RefreshUnionTaskCheckView", unionTaskList.Find(t => t.data.taskUid == data.taskData.taskUid));
    }

    private void getUnionStartUnionTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_StartUnionTask)msg;

        refreshOneUnionTask(data.taskData);

        HotfixBridge.inst.TriggerLuaEvent("getUnionStartUnionTaskData");
    }

    private void getUnionCancelTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_CancelUnionTask)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        refreshUnionTaskData(data.unionTaskRefreshTime, data.unionTaskCancelCoolTime, data.unionTaskLevel, data.unionTaskPoint, data.taskList);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS);
        HotfixBridge.inst.TriggerLuaEvent("HideUI_UnionSelfTaskDetailUI");
        HotfixBridge.inst.TriggerLuaEvent("onUnionTaskDataChanged");

    }

    private void getUnionRewardTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_RewardUnionTask)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        refreshUnionTaskData(data.unionTaskRefreshTime, data.unionTaskCancelCoolTime, data.unionTaskLevel, data.unionTaskPoint, data.taskList);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS);

        //TODO 奖励是否需要额外展示
        HotfixBridge.inst.TriggerLuaEvent("HideUI_UnionSelfTaskDetailUI");
        HotfixBridge.inst.TriggerLuaEvent("onUnionTaskDataChanged");

    }

    private void getUnionGemRewardTaskData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_AccelUnionTask)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        refreshUnionTaskData(data.unionTaskRefreshTime, data.unionTaskCancelCoolTime, data.unionTaskLevel, data.unionTaskPoint, data.taskList);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS);

        //TODO 奖励是否需要额外展示
        HotfixBridge.inst.TriggerLuaEvent("HideUI_UnionSelfTaskDetailUI");
        HotfixBridge.inst.TriggerLuaEvent("onUnionTaskDataChanged");

    }

    private void getUnionTaskChangedData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_UnionTaskChange)msg;

        refreshUnionTaskData(data.unionTaskRefreshTime, data.unionTaskCancelCoolTime, data.unionTaskLevel, data.unionTaskPoint, data.taskList);

        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint);
        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS);
        HotfixBridge.inst.TriggerLuaEvent("onUnionTaskDataChanged");
    }

    private void getUnionTaskRankListData(HttpMsgRspdBase msg)
    {
        var data = (Response_Union_UnionTaskRankList)msg;

        if (data.rankList != null && data.rankList.Count > 1)
        {
            data.rankList.Sort((a, b) => b.point.CompareTo(a.point));
        }

        HotfixBridge.inst.TriggerLuaEvent("getUnionRenownRankList", data.rankList);
    }


    #endregion

    #region 科技

    void onUnionCoidChanged()
    {
        var unionMainUI = GUIManager.GetWindow<UnionMainUI>();

        if (unionMainUI != null && unionMainUI.isShowing)
        {
            unionMainUI.UpdateTokensMess();
        }

        EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.UNION_UNIONCOIN, Union_uCoin, Union_uCoin);

    }

    private void setUnionScienceData(OneUnionScienceData data)
    {
        if (unionScienceDic.ContainsKey(data.type))
        {
            unionScienceDic[data.type].SetInfo(data);
        }
        else
        {
            unionScienceDic.Add(data.type, new UnionScienceData(data));
        }
    }

    private void setUnionSciendceSkillData(OneUnionScienceSkillData data)
    {
        UnionBuffData buffData = unionBuffDatas.Find((t) => t.serverData.type == data.type);

        if (buffData != null)
        {
            buffData.SetInfo(data);
        }
        else
        {
            buffData = new UnionBuffData(data);
            unionBuffDatas.Add(buffData);
        }

    }

    private void GetUnionScienceListData(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Union_ScienceList;

        data.scienceList.Sort((a, b) => -a.type.CompareTo(b.type));

        foreach (var item in data.scienceList)
        {
            setUnionScienceData(item);
        }

        union_uCoin = data.unionPoint;

        //刷新UI
        onUnionCoidChanged();
        HotfixBridge.inst.TriggerLuaEvent("onUnionScienceDataChanged");
    }

    private void GetUnionScienceUpgradeData(HttpMsgRspdBase msg)
    {
        AudioManager.inst.PlaySound(126);

        var data = msg as Response_Union_ScienceUpgrade;

        data.scienceList.Sort((a, b) => -a.type.CompareTo(b.type));

        foreach (var item in data.scienceList)
        {
            setUnionScienceData(item);
        }

        union_uCoin = data.unionPoint;

        //刷新UI
        onUnionCoidChanged();
        HotfixBridge.inst.TriggerLuaEvent("onUnionScienceDataChanged");
    }

    private void GetUnionScienceSkillListData(HttpMsgRspdBase msg)
    {

        var data = msg as Response_Union_ScienceSkillList;

        foreach (var item in data.skillList)
        {
            setUnionSciendceSkillData(item);
        }

        union_uCoin = data.unionPoint;

        onUnionCoidChanged();
        onScienceSkillDataChanged();
    }


    private void GetUnionScienceSkillUseData(HttpMsgRspdBase msg)
    {
        AudioManager.inst.PlaySound(126);

        var data = msg as Response_Union_ScienceSkillUse;

        setUnionSciendceSkillData(data.skill);
        union_uCoin = data.unionPoint;

        onUnionCoidChanged();
        onScienceSkillDataChanged();
    }

    private void GetUnionScienceSkillRefreshData(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Union_ScienceSkillRefresh;

        setUnionSciendceSkillData(data.skill);
        union_uCoin = data.unionPoint;

        onUnionCoidChanged();
        onScienceSkillDataChanged();
    }

    private void onScienceSkillDataChanged()
    {
        UnionMainUI unionMainUI = GUIManager.GetWindow<UnionMainUI>();
        if (unionMainUI != null && unionMainUI.isShowing)
        {
            unionMainUI.UpdateUnoinBuffInfo();
        }

        EquipListUIView equipListUIView = GUIManager.GetWindow<EquipListUIView>();
        if (equipListUIView != null && equipListUIView.isShowing)
        {
            equipListUIView.UpdateUnoinBuffInfo();
        }

        ExplorePanelView explorePanelView = GUIManager.GetWindow<ExplorePanelView>();
        if (explorePanelView != null && explorePanelView.isShowing)
        {
            explorePanelView.UpdateUnoinBuffInfo();
        }

        //lua 刷新UI
        HotfixBridge.inst.TriggerLuaEvent("onUnionScienceDataChanged");
    }


    #endregion

    #endregion

}
