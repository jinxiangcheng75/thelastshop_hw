using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AddressableAssets;

[XLua.LuaCallCSharp]
public class ManagerBinder : TSingletonHotfix<ManagerBinder>
{
    // public GameStateManager mStateMgr;
    public NetworkManager mNetworkMgr;
    // AssetManager mAssetMgr;
    AssetCache mAssetCache;
    public GameScenesManager mSceneMgr;
    public bool stateIsChanging = false;
    public kGameState mGameState = kGameState.Update;
    // public List<BaseSystem> gameSysList = new List<BaseSystem>();           //游戏功能
    // public List<IDataModelProx> dataModelProxesList = new List<IDataModelProx>(); //各功能数据
    public bool isReStart = false;
    public void Init(MonoBehaviour mono)
    {
        EventController.inst.TriggerEvent_Lua1("aot", "string");
        EventController.inst.TriggerEvent_Lua2("aot", "string", "string");
        EventController.inst.TriggerEvent_Lua3("aot", "string", "string", "string");
        EventController.inst.TriggerEvent_Lua4("aot", "string", "string", "string", "string");

        EventController.inst.TriggerEvent_Lua1("aot", 1);
        // EventController.inst.TriggerEvent_Lua2("aot", 1, 1);
        // EventController.inst.TriggerEvent_Lua3("aot", 1, 1, 1);
        // EventController.inst.TriggerEvent_Lua4("aot", 1, 1, 1, 1);
        EventController.inst.TriggerEvent_Lua1("aot", true);
        // EventController.inst.TriggerEvent_Lua2("aot", true, true);
        // EventController.inst.TriggerEvent_Lua3("aot", 1, 1, true);
        // EventController.inst.TriggerEvent_Lua4("aot", 1, 1, 1, true);
        EventController.inst.TriggerEvent_Lua1("aot", kFurnitureDisplayType.None);
        EventController.inst.TriggerEvent_Lua1("aot", new UnityEngine.Color(1, 1, 1, 1));
        EventController.inst.TriggerEvent_Lua1("aot", .1f);

        EventController.inst.TriggerEvent_Lua2("aot", "string", new UnityEngine.Color(1, 1, 1, 1));
        EventController.inst.TriggerEvent_Lua2("aot", (System.Int16)1, (System.Int32)1);
        EventController.inst.TriggerEvent_Lua2("aot", (System.Int64)1, (System.Int64)2);
        EventController.inst.TriggerEvent_Lua2("aot", (System.Int32)1, (System.Int32)2);
        EventController.inst.TriggerEvent_Lua2("aot", DesignMode.FloorEdit, (System.Int32)1);
        EventController.inst.TriggerEvent_Lua2("aot", (System.Int32)1, new RoleDress());
        EventController.inst.TriggerEvent_Lua2("aot", (System.Int64)1, (System.Int64)1);
        EventController.inst.TriggerEvent_Lua2("aot", "string", "string");

        EventController.inst.TriggerEvent_Lua3("aot", (System.Int32)1, GameTimer.inst.transform, (System.Int64)1);
        EventController.inst.TriggerEvent_Lua3("aot", (System.Int64)1, (System.Int64)2, (System.Int64)3);
        EventController.inst.TriggerEvent_Lua3("aot", true, true, (System.Int64)1);
        EventController.inst.TriggerEvent_Lua3("aot", true, false, (System.Int32)1);
        EventController.inst.TriggerEvent_Lua3("aot", (System.Int32)1, (System.Int32)1, (System.Int32)1);
        EventController.inst.TriggerEvent_Lua3("aot", (System.Int64)1, (System.Int64)1, (System.Int64)1);
        EventController.inst.TriggerEvent_Lua3("aot", (System.String)"string", (System.String)"string", GameTimer.inst.transform);
        EventController.inst.TriggerEvent_Lua3("aot", (System.String)"string", (System.Int32)1, new List<EquipItem>());



        EventController.inst.TriggerEvent_Lua4("aot", 1, " ", 1, 1);
        EventController.inst.TriggerEvent_Lua4("aot", (System.Int32)1, (System.Int32)1, (System.Int32)1, (System.Boolean)true);
        EventController.inst.TriggerEvent_Lua4("aot", (System.Int32)1, (System.Int32)1, (System.String)"string", new CombatReport());

        Application.logMessageReceived += Application_logMessageReceived;

        if (PlayerPrefs.HasKey("combatPlaySpeed"))
        {
            GameSettingManager.combatPlaySpeed = PlayerPrefs.GetFloat("combatPlaySpeed");
        }
        else
        {
            GameSettingManager.combatPlaySpeed = GameSettingManager.combatDefaultSpeed;
        }

        if (PlayerPrefs.HasKey("soundVolume"))
        {
            AudioManager.inst.SetSoundVolume(PlayerPrefs.GetFloat("soundVolume"));
        }
        else
        {
            PlayerPrefs.SetFloat("soundVolume", 0.8f);
        }

        // LanguageManager.inst.SetFont();
        // (new LoadingSystem()).OnEnter();
        //  mStateMgr = new GameStateManager();
        if (mAssetCache == null)
            mAssetCache = new AssetCache(mono);
        if (mSceneMgr == null)
            mSceneMgr = new GameScenesManager();
        // mNetworkMgr = new NetworkManager();
        //Application.logMessageReceived += Application_logMessageReceived;
    }

    public void InitAfterPatched()
    {

    }

    Action CSCfgManagerInitEnd;
    public void InitCfgManagerAsync(Action callback)
    {
        CSCfgManagerInitEnd = callback;
        initCSConfigs();
        GameTimer.inst.StartCoroutine(loadCfg());
    }
    IEnumerator loadCfg()
    {
        yield return null;
        int index = 0;
        foreach (var cfgmgr in cfgManagerList)
        {
            index++;
            cfgmgr.InitCSVConfig();
            if (index % 10 == 0)
                yield return null;
        }
        if (index >= cfgManagerList.Count)
            CSCfgManagerInitEnd?.Invoke();
    }

    public void ReLoadCfgManagerAsync(Action callback)
    {
        CSCfgManagerInitEnd = callback;
        GameTimer.inst.StartCoroutine(ReLoadCfg());
    }

    IEnumerator ReLoadCfg()
    {
        yield return null;
        FGUI.inst.updateProgressText(LanguageManager.inst.GetValueByKey("正在进入壁垒..."));
        int index = 0;
        foreach (var cfgmgr in cfgManagerList)
        {
            index++;
            cfgmgr.ReLoadCSVConfig();
            if (index % 10 == 0)
                yield return null;
        }
        if (index >= cfgManagerList.Count)
            CSCfgManagerInitEnd?.Invoke();
    }
    List<IConfigManager> cfgManagerList = new List<IConfigManager>();
    private void initCSConfigs()
    {
        cfgManagerList.Clear();
        cfgManagerList.Add(LanguageConfigManager.inst);
        cfgManagerList.Add(ItemconfigManager.inst);
        cfgManagerList.Add(heroupgradeconfigManager.inst);
        cfgManagerList.Add(EquipConfigManager.inst);
        cfgManagerList.Add(TaskItemConfigManager.inst);
        cfgManagerList.Add(VFXConfigManager.inst);
        cfgManagerList.Add(ExtensionConfigManager.inst);
        cfgManagerList.Add(dressconfigManager.inst);
        cfgManagerList.Add(CharacterModelConfigManager.inst);
        cfgManagerList.Add(FieldConfigManager.inst);
        cfgManagerList.Add(CounterUpgradeConfigManager.inst);
        cfgManagerList.Add(ShelfUpgradeConfigManager.inst);
        cfgManagerList.Add(ResourceBinUpgradeConfigManager.inst);
        cfgManagerList.Add(FurnitureClassificationConfigManager.inst);
        cfgManagerList.Add(FurnitureItemiconConfigManager.inst);
        cfgManagerList.Add(TrunkUpgradeConfigManager.inst);
        cfgManagerList.Add(FurnitureConfigManager.inst);
        cfgManagerList.Add(ShopkeeperUpconfigManager.inst);
        cfgManagerList.Add(MusicConfigManager.inst);
        cfgManagerList.Add(AITalkConfigManager.inst);
        cfgManagerList.Add(ShelfDisplayConfigManager.inst);
        cfgManagerList.Add(WorkerConfigManager.inst);
        cfgManagerList.Add(WorkerUpConfigManager.inst);
        cfgManagerList.Add(MarketBoothConfigManger.inst);
        cfgManagerList.Add(MarketEquipLvManager.inst);
        cfgManagerList.Add(CumulativeRewardConfigManager.inst);
        cfgManagerList.Add(WorldParConfigManager.inst);
        cfgManagerList.Add(IndoorGridMapClr.inst);
        cfgManagerList.Add(heroSkillConfigManager.inst);
        cfgManagerList.Add(HeroAttributeConfigManager.inst);
        cfgManagerList.Add(HeroProfessionConfigManager.inst);
        cfgManagerList.Add(BuildingConfigManager.inst);
        cfgManagerList.Add(BuildingCostConfigManager.inst);
        cfgManagerList.Add(BuildingUpgradeConfigManager.inst);
        cfgManagerList.Add(ExploreInstanceConfigManager.inst);
        cfgManagerList.Add(ExploreInstanceLvConfigManager.inst);
        cfgManagerList.Add(MonsterConfigManager.inst);
        cfgManagerList.Add(EquipActionConfigManager.inst);
        cfgManagerList.Add(TreasureBoxConfigManager.inst);
        cfgManagerList.Add(BuffConfigManager.inst);
        cfgManagerList.Add(HeroSkillShowConfigManager.inst);
        cfgManagerList.Add(HeroTalentDBConfigManager.inst);
        cfgManagerList.Add(GuideConfigManager.inst);
        cfgManagerList.Add(AcheivementRoadConfigManager.inst);
        cfgManagerList.Add(AcheivementConfigManager.inst);
        cfgManagerList.Add(GameHelpNavigationConfigManager.inst);
        cfgManagerList.Add(ActiveTaskConfigManager.inst);
        cfgManagerList.Add(SevenDayAwardConfigManager.inst);
        cfgManagerList.Add(SevenDayTaskConfigManger.inst);
        cfgManagerList.Add(StreetRolePosConfigManager.inst);
        cfgManagerList.Add(StreetDropPosConfigManager.inst);
        cfgManagerList.Add(GuideTaskConfigManager.inst);
        cfgManagerList.Add(GuideTriggerConditionConfigManagaer.inst);
        cfgManagerList.Add(GuideTriggerResultConfigManager.inst);
        cfgManagerList.Add(GuideTriggerDialogConfigManager.inst);
        cfgManagerList.Add(GuideTriggerOperationConfigManager.inst);
        cfgManagerList.Add(GlobalBuffConfigManager.inst);
        cfgManagerList.Add(ArtisanNPCConfigManager.inst);
        cfgManagerList.Add(UIUnLockConfigMrg.inst);
        cfgManagerList.Add(GameTipsConfigManager.inst);
        cfgManagerList.Add(UnionTaskPopularityConfigManager.inst);
        cfgManagerList.Add(UnionTaskConfigManager.inst);
        cfgManagerList.Add(UnionLevelConfigManager.inst);
        cfgManagerList.Add(UnionTechnologyConfigManager.inst);
        cfgManagerList.Add(TaskMainConfigManager.inst);
        cfgManagerList.Add(OperationConfigManager.inst);
        cfgManagerList.Add(SevenDayTaskListConfigManager.inst);
        cfgManagerList.Add(UnionResourceConfigManager.inst);
        cfgManagerList.Add(VipLevelConfigManager.inst);
        cfgManagerList.Add(PetConfigManager.inst);
        cfgManagerList.Add(AITalkProbConfigManager.inst);
        cfgManagerList.Add(CameraMoveConfigManager.inst);
        cfgManagerList.Add(FurnitureBuffConfigManager.inst);
        cfgManagerList.Add(HeroBuffConfigManager.inst);
        cfgManagerList.Add(IndoorRoleActionConfigManager.inst);
        cfgManagerList.Add(FurnitureBuyCostConfigManager.inst);
        cfgManagerList.Add(GamePayPricecConfigManager.inst);
        cfgManagerList.Add(UrlUpdateConfigManager.inst);
    }

    public void GC()
    {
        // Resources.UnloadUnusedAssets();
        // System.GC.Collect();
    }

    //开始游戏
    public void StartGame()
    {
        // startGameSystem();
    }
    public void startGameSystem()
    {
        // foreach (var model in dataModelProxesList)
        // {
        //     model.Init();
        // }
        // foreach (var sys in gameSysList)
        // {
        //     sys.OnEnter();
        // }
    }

    public void ExitGameSystem()
    {
        // foreach (var model in dataModelProxesList)
        // {
        //     model.Clear();
        // }

        // foreach (var sys in gameSysList)
        // {
        //     sys.OnExit();
        // }
    }

    public void update()
    {
        // foreach (var sys in gameSysList)
        // {
        //     sys.OnUpdate();
        // }
    }
    public void clear()
    {
        //清理游戏循环
        GameTimer.inst.clearAll();
        //清理模型缓存
        if (CharacterManager.inst != null)
            CharacterManager.inst.UnLoadAllSkeletonDataAsset();
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= Application_logMessageReceived;
    }
    public IAssetCache Asset { get { return mAssetCache; } }

    void Application_logMessageReceived(string condition, string stackTrace, LogType logType)
    {
#if UNITY_EDITOR
        return;
#endif
        if (logType == LogType.Exception || logType == LogType.Error)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Client_Error()
                {
                    err = condition,
                    userId = UserDataProxy.inst.playerData != null ? UserDataProxy.inst.playerData.userUid : "",
                    ver = GameSettingManager.appVersion,
                    stackTrack = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(stackTrace)),
#if UNITY_ANDROID
                    osType = (int)EOsType.Android,
#elif UNITY_IPHONE
                    osType = (int)EOsType.Ios,
#endif
                }
            });
        }
    }



    ///發送遊戲中事件
    public float Behaviorupdatedis = 5;
    public void startSeneEvent(float dis)
    {
        Behaviorupdatedis = dis;
        GameTimer.inst.StartCoroutine(GameEventUpdate());
    }
    float time = 0;
    IEnumerator GameEventUpdate()
    {
        while (true)
        {
            time += Time.deltaTime;
            if (time > Behaviorupdatedis)
            {
                time = 0;
                if (eventlist.Count > 0)
                {
                    sending = true;
                    sendGameEventToServer();
                    yield return new WaitForSeconds(1);
                    sending = false;
                }
            }
            yield return null;
        }
    }
    private List<OneBehavior> eventlist = new List<OneBehavior>();
    private List<OneBehavior> Sendeventlist = new List<OneBehavior>();
    private bool sending = false;
    private int index;
    public void sendGameEventToServer()
    {
        if (eventlist.Count > 0)
        {
            Sendeventlist.Clear();
            foreach (var item in eventlist)
            {
                Sendeventlist.Add(item);
            }
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_User_BehaviorCounter()
                {
                    list = Sendeventlist
                }
            });
            eventlist.Clear();
        }
    }

    public void AddGameEvent(int _type, string eventname, string value)
    {
        if (AccountDataProxy.inst.userId == null) return;
        if (GameSettingManager.HandleEventState == 0) return;
        if (sending) return;
        var msg = new OneBehavior()
        {
            userId = AccountDataProxy.inst.userId == null ? "" : AccountDataProxy.inst.userId,
            optionTime = (int)GameTimer.inst.serverNow,
            type = _type,
            optionName = eventname,
            param = value
        };
        eventlist.Add(msg);
    }
}
