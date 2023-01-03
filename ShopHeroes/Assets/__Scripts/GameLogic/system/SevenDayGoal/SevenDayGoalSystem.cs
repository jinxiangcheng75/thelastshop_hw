using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SevenDayGoalSystem : BaseSystem
{
    SevenDayGoalView sevenDayView;
    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener(GameEventType.SevenDayGoalEvent.SHOWUI_SEVENDAYUI, showSevenDayUI);
        EventController.inst.AddListener(GameEventType.SevenDayGoalEvent.HIDEUI_SEVENDAYTUI, hideSevenDayUI);

        EventController.inst.AddListener(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYCHECK, requestSevenDayCheck);
        EventController.inst.AddListener<int>(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYAWARD, requestSevenDayAward);
        EventController.inst.AddListener<int>(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYLISTAWARD, requestSevenDayListAward);

        EventController.inst.AddListener(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH, refreshSevenDayUI);

        EventController.inst.AddListener<SevenDayGoalSingle>(GameEventType.SevenDayGoalEvent.JUMPTOOTHERPANEL, JumpPanelOperation);

        EventController.inst.AddListener<int>(GameEventType.SevenDayGoalEvent.SEVENDAY_JUMP, sevenDayJump);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener(GameEventType.SevenDayGoalEvent.SHOWUI_SEVENDAYUI, showSevenDayUI);
        EventController.inst.RemoveListener(GameEventType.SevenDayGoalEvent.HIDEUI_SEVENDAYTUI, hideSevenDayUI);

        EventController.inst.RemoveListener(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYCHECK, requestSevenDayCheck);
        EventController.inst.RemoveListener<int>(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYAWARD, requestSevenDayAward);
        EventController.inst.RemoveListener<int>(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYLISTAWARD, requestSevenDayListAward);

        EventController.inst.RemoveListener(GameEventType.SevenDayGoalEvent.RESPONSE_SEVENDAYUIREFRESH, refreshSevenDayUI);

        EventController.inst.RemoveListener<SevenDayGoalSingle>(GameEventType.SevenDayGoalEvent.JUMPTOOTHERPANEL, JumpPanelOperation);

        EventController.inst.RemoveListener<int>(GameEventType.SevenDayGoalEvent.SEVENDAY_JUMP, sevenDayJump);
    }

    void sevenDayJump(int id)
    {
        GUIManager.OpenView<SevenDayGoalView>((view) =>
        {
            sevenDayView = view;
            view.JumpTarget(id);
        });
    }

    void showSevenDayUI()
    {
        sevenDayView = GUIManager.OpenView<SevenDayGoalView>();
    }

    void hideSevenDayUI()
    {
        GUIManager.HideView<SevenDayGoalView>();
    }

    void refreshSevenDayUI()
    {
        if (sevenDayView != null && sevenDayView.isShowing)
        {
            sevenDayView.RefreshData();
        }
    }

    void requestSevenDayCheck()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Activity_SevenDayCheck()
        });
    }

    void requestSevenDayAward(int id)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Activity_SevenDayReward()
            {
                sevenDayTaskId = id
            }
        });
    }

    void requestSevenDayListAward(int id)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Activity_SevenDayListReward()
            {
                sevenDayListId = id
            }
        });
    }

    void JumpPanelOperation(SevenDayGoalSingle data)
    {
        switch ((K_SevenDay_Type)data.cfg.type)
        {
            case K_SevenDay_Type.MakeMoney:
            case K_SevenDay_Type.FurnitureLvUp:
            case K_SevenDay_Type.LevelUp:
            case K_SevenDay_Type.EnergyUp:
            case K_SevenDay_Type.SellEquip:
            case K_SevenDay_Type.MarkUpSale:
                EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.HIDEUI_SEVENDAYTUI);
                break;
            case K_SevenDay_Type.MakeTargetEquip:
                // 前往制造装备界面
                //GUIManager.GetWindow<EquipListUIView>(true).setShowListSubtype(cfg.sub_type);
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_PRODUCTION_SELECT, -1);
                break;
            case K_SevenDay_Type.UnlockEquip:
            case K_SevenDay_Type.ReceiveDrawing:
            case K_SevenDay_Type.UnlockTargetLvEquip:
            case K_SevenDay_Type.EquipmentMastery:
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_PRODUCTION_SELECT, -1);
                break;
            case K_SevenDay_Type.BuyFurniture:
                // 前往家居界面
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_FURNITUREUI);
                break;
            case K_SevenDay_Type.StoreExpasion:
                // 前往商店扩建界面
                btn_expandOnClick();
                break;
            case K_SevenDay_Type.Union:
                // 前往工会界面
                EventController.inst.TriggerEvent(GameEventType.UnionEvent.ENTER_UNIONSCENE);
                break;
            case K_SevenDay_Type.TechnologyUpgrading:
                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, 2300);
                break;
            case K_SevenDay_Type.BuildUpgrading:
            case K_SevenDay_Type.BuildTargetLv:

                break;
            case K_SevenDay_Type.HeroRecruit:
                RoleDataProxy.inst.enterType = 0;
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI);
                break;
            case K_SevenDay_Type.HeroTransfer:
                if (RoleDataProxy.inst.HeroList.Count > 0)
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
                else
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI);
                break;
            case K_SevenDay_Type.HeroRarity:
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI);
                break;
            case K_SevenDay_Type.HeroWearEquipLv:
                if (RoleDataProxy.inst.HeroList.Count > 0)
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, RoleDataProxy.inst.HeroList[0].uid);
                else
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI);
                break;
            case K_SevenDay_Type.ExploreUpgrading:
            case K_SevenDay_Type.ExploreChallenges:
            case K_SevenDay_Type.ExploreCount:
                var slotData = ExploreDataProxy.inst.GetFreeSlotData();
                if (slotData != null)
                {
                    ExploreDataProxy.inst.curSlotId = slotData.slotId;
                    EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLORE_SHOWUI);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("当前没有空余槽位"), Color.white);
                }
                break;
            case K_SevenDay_Type.AddMakeSlot:
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_BuyMakingSlot);
                break;
            case K_SevenDay_Type.OpenTreasureBoxCount:
                break;
            case K_SevenDay_Type.PrestigePromotion:
                break;
            case K_SevenDay_Type.MarketTransactions:
                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, 2100);
                break;
            case K_SevenDay_Type.EquipmentExchange:
                break;
        }
    }

    void btn_expandOnClick()
    {
        EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;
        switch (state)
        {
            case EDesignState.Idle:
                if (UserDataProxy.inst.shopData.shopLevel == StaticConstants.shopMap_MaxLevel)
                {
                    //已经到达最高级别
                    //Hidebtn_expand();
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("已达到最大面积不可扩建"), GUIHelper.GetColorByColorHex("FF2828"));
                    break;
                }
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENSIONPANEL);
                break;
            case EDesignState.Upgrading:
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENDINGPANEL);
                break;
            case EDesignState.Finished:
                IndoorMapEditSys.inst.shopUpgradeFinish();
                break;
        }
    }
}

