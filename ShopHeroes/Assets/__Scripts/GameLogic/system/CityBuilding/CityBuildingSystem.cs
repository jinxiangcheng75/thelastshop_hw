using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//城市建筑系统
public class CityBuildingSystem : BaseSystem
{

    CityBuildingInvestUI _investUI;
    ScienceLabUI _scienceLabUI;
    CityBuildingUpFinishUI _levelUpFinishUI;
    CityBuildingUnlockInfoUI _unlockInfoUI;
    CityBuildingLockDetailUI _lockDetailUI;

    Dictionary<int, HouseComp> cityBuildingHudDic;

    protected override void AddListeners()
    {
        EventController.inst.AddListener<int>(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, onBuildingClick);
        EventController.inst.AddListener<bool, int>(GameEventType.CityBuildingEvent.CITYBUILDINGINVESTUI_TURNPAGE, turnPageCityBuildingInvestUI);
        EventController.inst.AddListener<CityBuildingData>(GameEventType.CityBuildingEvent.SHOWUI_BUILDINGUPFINISH, showBuildingUpFinishUI);
        EventController.inst.AddListener(GameEventType.CityBuildingEvent.HIDEUI_CITYBUILDINGINVEST, hideCityBuildingInvestUI);
        EventController.inst.AddListener<CityBuildingData>(GameEventType.CityBuildingEvent.INVESTUI_SETDATA, investUISetData);
        EventController.inst.AddListener(GameEventType.CityBuildingEvent.SCIENCELABUI_REFRESH, scienceUIRefresh);
        EventController.inst.AddListener<List<BuildTopList>>(GameEventType.CityBuildingEvent.INVEST_REFRESHUNIONRANK, investRefreshUnionRank);

        //---HUD
        EventController.inst.AddListener<int, HouseComp>(GameEventType.CityBuildingEvent.HUD_BUILDINGUPADD, addHouseHudById);
        EventController.inst.AddListener<int>(GameEventType.CityBuildingEvent.HUD_BUILDINGUPREFRESH, refreshHouseInfoHudById);
        EventController.inst.AddListener(GameEventType.CityBuildingEvent.HUD_ALLBUILDINGUPREFRESH, refreshHouseInfoHuds);
        EventController.inst.AddListener(GameEventType.CityBuildingEvent.HUD_BUILDINGUPCLEAR, clearHouseHuds);


        //----------------------------------------------------------------------------------------------------------------------------
        EventController.inst.AddListener(GameEventType.CityBuildingEvent.CITYBUILDING_GET_DATA, request_getBuildingData);
        EventController.inst.AddListener<int, int, int>(GameEventType.CityBuildingEvent.CITYBUILDING_INVEST, request_investBuilding);
        EventController.inst.AddListener<int>(GameEventType.CityBuildingEvent.CITYBUILDING_INVEST_RANK_DATA, request_buildingRankData);


    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<int>(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, onBuildingClick);
        EventController.inst.RemoveListener<bool, int>(GameEventType.CityBuildingEvent.CITYBUILDINGINVESTUI_TURNPAGE, turnPageCityBuildingInvestUI);
        EventController.inst.RemoveListener<CityBuildingData>(GameEventType.CityBuildingEvent.SHOWUI_BUILDINGUPFINISH, showBuildingUpFinishUI);
        EventController.inst.RemoveListener(GameEventType.CityBuildingEvent.HIDEUI_CITYBUILDINGINVEST, hideCityBuildingInvestUI);
        EventController.inst.RemoveListener<CityBuildingData>(GameEventType.CityBuildingEvent.INVESTUI_SETDATA, investUISetData);
        EventController.inst.RemoveListener(GameEventType.CityBuildingEvent.SCIENCELABUI_REFRESH, scienceUIRefresh);
        EventController.inst.RemoveListener<List<BuildTopList>>(GameEventType.CityBuildingEvent.INVEST_REFRESHUNIONRANK, investRefreshUnionRank);

        //---HUD
        EventController.inst.RemoveListener<int, HouseComp>(GameEventType.CityBuildingEvent.HUD_BUILDINGUPADD, addHouseHudById);
        EventController.inst.RemoveListener<int>(GameEventType.CityBuildingEvent.HUD_BUILDINGUPREFRESH, refreshHouseInfoHudById);
        EventController.inst.RemoveListener(GameEventType.CityBuildingEvent.HUD_ALLBUILDINGUPREFRESH, refreshHouseInfoHuds);
        EventController.inst.RemoveListener(GameEventType.CityBuildingEvent.HUD_BUILDINGUPCLEAR, clearHouseHuds);


        //----------------------------------------------------------------------------------------------------------------------------
        EventController.inst.RemoveListener(GameEventType.CityBuildingEvent.CITYBUILDING_GET_DATA, request_getBuildingData);
        EventController.inst.RemoveListener<int, int, int>(GameEventType.CityBuildingEvent.CITYBUILDING_INVEST, request_investBuilding);
        EventController.inst.RemoveListener<int>(GameEventType.CityBuildingEvent.CITYBUILDING_INVEST_RANK_DATA, request_buildingRankData);


    }

    //初始化
    protected override void OnInit()
    {
        cityBuildingHudDic = new Dictionary<int, HouseComp>();
    }


    void request_getBuildingData()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_City_BuildData()
            });
        }
    }

    void request_investBuilding(int buildingId, int moneyType, int costCount)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_City_BuildCost()
                {
                    buildId = buildingId,
                    costMoneyType = moneyType,
                    buildCostCount = costCount,
                }
            });
        }
    }

    void request_buildingRankData(int buildingId)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_City_BuildingRankData()
                {
                    buildId = buildingId,
                }
            });
        }
    }

    private void onBuildingClick(int buildingId)
    {
        CityBuildingData buildingData = UserDataProxy.inst.GetBuildingData(buildingId);
        if (buildingData == null)
        {
            Logger.error("未找到对应建筑物id相关的数据    id : " + buildingId);
            return;
        }

        if (buildingData.state == 0 && buildingData.config.unlock_type != (int)kCityBuildingUnlockType.NeedOneWorker) //未解锁 并且不是工匠解锁条件
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("建筑物 {0} 尚未解锁", LanguageManager.inst.GetValueByKey(buildingData.config.name)), GUIHelper.GetColorByColorHex("FF2828"));
            showBuildingLockDetailUI(buildingData);
            return;
        }

        GUIManager.HideView<CityBuildingLockDetailUI>();

        Logger.log($"建筑物 {buildingData.config.name} 被点击");


        if (buildingId == 2100) EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETUI);//交易所
        //else if (buildingId == 2200) //商会联盟
        //{
        //    if (!UserDataProxy.inst.playerData.hasUnion) //没有公会
        //    {
        //        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONINFO);
        //    }
        //    else
        //    {
        //        HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Union, true));
        //        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_DATA, "");
        //    }
        //}
        else if (buildingId == 2300) showScienceLabUI();//科学院
        else showCityBuildingInvestUI(buildingData);

    }

    private void showCityBuildingInvestUI(CityBuildingData data)
    {

        if (_investUI != null && _investUI.isShowing)
        {
            _investUI.SetData(data);
        }
        else
        {
            GUIManager.OpenView<CityBuildingInvestUI>((view) =>
            {
                _investUI = view;
                _investUI.SetData(data);
            });
        }

    }

    private void investUISetData(CityBuildingData newBuildingData)
    {
        if (_investUI != null && _investUI.isShowing)
        {
            _investUI.SetData(newBuildingData);
        }
    }

    private void scienceUIRefresh()
    {
        if (_scienceLabUI != null && _scienceLabUI.isShowing)
        {
            _scienceLabUI.Refresh();
        }
    }

    private void investRefreshUnionRank(List<BuildTopList> buildTopList)
    {
        if (_investUI != null && _investUI.isShowing)
        {
            _investUI.RefreashUnionRankListInfo(buildTopList);
        }
    }

    private void hideCityBuildingInvestUI()
    {
        GUIManager.HideView<CityBuildingInvestUI>();
    }

    private void turnPageCityBuildingInvestUI(bool isLeft, int buildingId)
    {
        List<CityBuildingData> list = new List<CityBuildingData>();
        var buildingData = UserDataProxy.inst.GetBuildingData(buildingId);

        if ((kCityBuildingType)buildingData.config.architecture_type == kCityBuildingType.Science)
        {
            list = UserDataProxy.inst.GetAllCanShowScienceBuildingData();
        }
        else
        {
            list = UserDataProxy.inst.GetAllScienceBuildingData((kCityBuildingType)buildingData.config.architecture_type).FindAll(t => t.state != (int)EBuildState.EB_Lock);
        }

        int index = list.IndexOf(buildingData);

        index = isLeft ? index - 1 : index + 1;

        if (index == list.Count) index = 0;
        if (index == -1) index = list.Count - 1;

        showCityBuildingInvestUI(list[index]);
    }

    private void showScienceLabUI()
    {
        _scienceLabUI = GUIManager.OpenView<ScienceLabUI>();
    }

    private void showBuildingUpFinishUI(CityBuildingData data)
    {
        GUIManager.OpenView<CityBuildingUpFinishUI>((view) =>
        {
            view.SetData(data);
        });
    }

    //建筑物解锁信息面板
    private void showBuildingUnlockInfoUI(CityBuildingData data)
    {
        GUIManager.OpenView<CityBuildingUnlockInfoUI>((view) =>
        {
            view.SetData(data);
        });
    }

    //建筑物未解锁详情面板
    private void showBuildingLockDetailUI(CityBuildingData data)
    {
        GUIManager.OpenView<CityBuildingLockDetailUI>((view) =>
        {
            view.SetData(data);
        });
    }

    #region 建筑物HUD

    //添加建筑物hud
    void addHouseHudById(int buildingId, HouseComp hud)
    {
        cityBuildingHudDic[buildingId] = hud;
    }

    //刷新所有建筑物hud
    void refreshHouseInfoHuds()
    {
        if (ManagerBinder.inst.mGameState != kGameState.Town) return;

        foreach (var key in cityBuildingHudDic.Keys)
        {
            cityBuildingHudDic[key].setHouseInfo(key);
        }

    }

    //刷新建筑物hud
    void refreshHouseInfoHudById(int buildingId)
    {
        var buildingData = UserDataProxy.inst.GetBuildingData(buildingId);

        //科学院hud刷新  --科研项目 
        if (buildingData != null && buildingData.config.architecture_type == 3)
        {
            buildingId = 2300;
        }

        if (cityBuildingHudDic.TryGetValue(buildingId, out HouseComp houseHud))
        {
            houseHud.setHouseInfo(buildingId);
        }
    }

    //清空建筑物hud
    void clearHouseHuds()
    {
        cityBuildingHudDic.Clear();
    }

    #endregion

}
