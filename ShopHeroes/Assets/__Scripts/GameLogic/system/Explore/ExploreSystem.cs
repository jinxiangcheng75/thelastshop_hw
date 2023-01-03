using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upgradeItem
{
    public kExploreItemUpgradeType type;
    public int heroUid;
    public int intoType;
    public int exploreGroupId;
}

// 副本系统
public class ExploreSystem : BaseSystem
{
    ExplorePanelView _exploreView;
    ExploreUseItemView _exploreUseItemView;
    ExploreInfoView _exploreInfoView;
    ExploreUnlockView _exploreUnlockView;
    ExplorePrepareView _explorePrepareView;
    ExploreSelectHeroView _exploreSelectHeroView;
    ExploreBuySlotView _exploreBuySlotView;
    CityMainView _cityView;
    RoleAdventureView _roleAdventureView;
    ExploreFinishView _exploreFinishView;
    RoleUpgradeView _roleUpgradeView;

    List<upgradeItem> itemLists = new List<upgradeItem>();
    bool upgradePanelIsShow = false;
    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLORE_SHOWUI, showExploreUI);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLORE_HIDEUI, hideExploreUI);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.EXPLOREUSEITEM_SHOWUI, showExploreUseItemUI);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.EXPLOREINFO_SHOWUI, showExploreInfoUI);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.EXPLOREUNLOCK_SHOWUI, showExploreUnlockUI);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLOREUNLOCK_HIDEUI, hideExploreUnlockUI);
        EventController.inst.AddListener<int, int>(GameEventType.ExploreEvent.EXPLOREPREPARE_SHOWUI, showExplorePrepareUI);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLOREPREPARE_HIDEUI, hideExplorePrepareUI);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.EXPLOREUSEITEM_COMPLETE, useItemComplete);
        EventController.inst.AddListener<int, int, float>(GameEventType.ExploreEvent.EXPLOREHERO_SHOWUI, showExploreSelectHeroUI);
        EventController.inst.AddListener<int, int>(GameEventType.ExploreEvent.EXPLOREPREPAREADD_COM, explorePrepareAddHeroCom);
        EventController.inst.AddListener<int, int>(GameEventType.ExploreEvent.EXPLOREPREPAREREMOVE_COM, explorePrepareRemoveHeroCom);
        EventController.inst.AddListener<ESlotAnimType>(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, updateCityUISlotData);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.EXPLOREBUYSLOT_SHOWUI, showExploreBuySlot);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLOREBUYSLOT_HIDEUI, hideExploreBuySlot);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.ROLEADVENTUREBYSLOT_SHOWUI, showAdventureUIBySlot);
        EventController.inst.AddListener<List<OneRewardItem>, int, int>(GameEventType.ExploreEvent.EXPLOREFINISH_SHOWSUCCESSUI, showFinishSuccessUI);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.EXPLOREFINISH_SHOWLOSEUI, showFinishLoseUI);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLOREFINISH_HIDEUI, hideExploreFinishUI);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLORESELECTHERO_HIDEUI, hideExploreSelectHeroUI);

        EventController.inst.AddListener(GameEventType.ExploreEvent.HEROUPGRADESTART, exploreEndHeroUpgradeStart);
        EventController.inst.AddListener(GameEventType.ExploreEvent.HEROUPGRADEEND, exploreEndHeroUpgradeEnd);
        EventController.inst.AddListener(GameEventType.ExploreEvent.EXPLOREUPGRADEEND, exploreEndExploreUpgradeEnd);
        EventController.inst.AddListener<upgradeItem>(GameEventType.ExploreEvent.UPGRADEADD, AddUpgradePanel);
        EventController.inst.AddListener(GameEventType.ExploreEvent.UPGRADENEXT, nextPanel);

        EventController.inst.AddListener(GameEventType.ExploreEvent.REQUEST_EXPLOREGROUPDATA, requestExploreGroupData);
        EventController.inst.AddListener(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTDATA, requestExploreSlotData);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.REQUEST_BUYEXPLORESLOT, requestExploreBuySlotData);
        EventController.inst.AddListener<ExploreSlot, int>(GameEventType.ExploreEvent.REQUEST_EXPLORESTART, requestExploreStartData);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLOREEND, requestExploreEndData);
        EventController.inst.AddListener<int, int>(GameEventType.ExploreEvent.REQUEST_EXPLOREUNLOCK, requestExploreUnlockData);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTREFRESH, requestExploreSlotRefreshData);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLOREIMMEDIATELY, requestExploreSlotImmediately);
        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLOREREWARDVIP, requestExploreVipReward);

        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.RESPONSE_SETADVENTUREDATA, responseSetAdventureData);
        EventController.inst.AddListener(GameEventType.ExploreEvent.RESPONSE_PREPAREREFRESHDATA, responsePrepareRefreshData);

        EventController.inst.AddListener(GameEventType.ExploreEvent.RESPONSE_BUYVIPCOMPLETE, buyVipComplete);

        EventController.inst.AddListener(GameEventType.ExploreEvent.REFRESH_SORTHEROLIST, sortHeroList);

        EventController.inst.AddListener<int>(GameEventType.ExploreEvent.Explore_JumpToTargetExplore, jumpToTargetExplore);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLORE_SHOWUI, showExploreUI);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLORE_HIDEUI, hideExploreUI);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.EXPLOREUSEITEM_SHOWUI, showExploreUseItemUI);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.EXPLOREINFO_SHOWUI, showExploreInfoUI);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.EXPLOREUNLOCK_SHOWUI, showExploreUnlockUI);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLOREUNLOCK_HIDEUI, hideExploreUnlockUI);
        EventController.inst.RemoveListener<int, int>(GameEventType.ExploreEvent.EXPLOREPREPARE_SHOWUI, showExplorePrepareUI);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLOREPREPARE_HIDEUI, hideExplorePrepareUI);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.EXPLOREUSEITEM_COMPLETE, useItemComplete);
        EventController.inst.RemoveListener<int, int, float>(GameEventType.ExploreEvent.EXPLOREHERO_SHOWUI, showExploreSelectHeroUI);
        EventController.inst.RemoveListener<int, int>(GameEventType.ExploreEvent.EXPLOREPREPAREADD_COM, explorePrepareAddHeroCom);
        EventController.inst.RemoveListener<int, int>(GameEventType.ExploreEvent.EXPLOREPREPAREREMOVE_COM, explorePrepareRemoveHeroCom);
        EventController.inst.RemoveListener<ESlotAnimType>(GameEventType.ExploreEvent.EXPLORE_UPDATESLOTDATA, updateCityUISlotData);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.EXPLOREBUYSLOT_SHOWUI, showExploreBuySlot);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLOREBUYSLOT_HIDEUI, hideExploreBuySlot);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.ROLEADVENTUREBYSLOT_SHOWUI, showAdventureUIBySlot);
        EventController.inst.RemoveListener<List<OneRewardItem>, int, int>(GameEventType.ExploreEvent.EXPLOREFINISH_SHOWSUCCESSUI, showFinishSuccessUI);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.EXPLOREFINISH_SHOWLOSEUI, showFinishLoseUI);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLOREFINISH_HIDEUI, hideExploreFinishUI);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLORESELECTHERO_HIDEUI, hideExploreSelectHeroUI);

        EventController.inst.RemoveListener(GameEventType.ExploreEvent.HEROUPGRADESTART, exploreEndHeroUpgradeStart);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.HEROUPGRADEEND, exploreEndHeroUpgradeEnd);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.EXPLOREUPGRADEEND, exploreEndExploreUpgradeEnd);
        EventController.inst.RemoveListener<upgradeItem>(GameEventType.ExploreEvent.UPGRADEADD, AddUpgradePanel);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.UPGRADENEXT, nextPanel);

        EventController.inst.RemoveListener(GameEventType.ExploreEvent.REQUEST_EXPLOREGROUPDATA, requestExploreGroupData);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTDATA, requestExploreSlotData);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.REQUEST_BUYEXPLORESLOT, requestExploreBuySlotData);
        EventController.inst.RemoveListener<ExploreSlot, int>(GameEventType.ExploreEvent.REQUEST_EXPLORESTART, requestExploreStartData);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLOREEND, requestExploreEndData);
        EventController.inst.RemoveListener<int, int>(GameEventType.ExploreEvent.REQUEST_EXPLOREUNLOCK, requestExploreUnlockData);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTREFRESH, requestExploreSlotRefreshData);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLOREIMMEDIATELY, requestExploreSlotImmediately);
        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.REQUEST_EXPLOREREWARDVIP, requestExploreVipReward);

        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.RESPONSE_SETADVENTUREDATA, responseSetAdventureData);
        EventController.inst.RemoveListener(GameEventType.ExploreEvent.RESPONSE_PREPAREREFRESHDATA, responsePrepareRefreshData);

        EventController.inst.RemoveListener(GameEventType.ExploreEvent.RESPONSE_BUYVIPCOMPLETE, buyVipComplete);

        EventController.inst.RemoveListener(GameEventType.ExploreEvent.REFRESH_SORTHEROLIST, sortHeroList);

        EventController.inst.RemoveListener<int>(GameEventType.ExploreEvent.Explore_JumpToTargetExplore, jumpToTargetExplore);
    }

    void jumpToTargetExplore(int exploreGroupId)
    {
        _exploreView = GUIManager.GetWindow<ExplorePanelView>();
        if (_exploreView != null && _exploreView.isShowing)
        {
            _exploreView.jumpToTargetExplore(exploreGroupId);
        }
    }

    void sortHeroList()
    {
        _explorePrepareView = GUIManager.GetWindow<ExplorePrepareView>();
        if (_explorePrepareView != null && _explorePrepareView.isShowing)
        {
            _explorePrepareView.SortAllHeroList();
        }
    }

    void buyVipComplete()
    {
        _exploreFinishView = GUIManager.GetWindow<ExploreFinishView>();
        if (_exploreFinishView != null && _exploreFinishView.isShowing)
        {
            _exploreFinishView.vipBuyRefresh();
        }
    }

    void hideExploreFinishUI()
    {
        _exploreFinishView = GUIManager.GetWindow<ExploreFinishView>();
        if (_exploreFinishView != null && _exploreFinishView.isShowing)
        {
            _exploreFinishView.ExitExplore();
        }
    }

    private void AddUpgradePanel(upgradeItem item)
    {

        itemLists.Add(item);
        if (itemLists.Count > 1)
        {
            itemLists.Sort((x1, x2) => x1.type.CompareTo(x2.type));
        }
        if (upgradePanelIsShow) return;
        //if (_roleUpgradeView == null || !_roleUpgradeView.isShowing)
        //{

        //}
        nextPanel();
    }

    private void nextPanel()
    {
        //if (upgradePanelIsShow) return;
        upgradePanelIsShow = true;
        if (itemLists.Count > 0)
        {
            if (_roleUpgradeView != null && _roleUpgradeView.isShowing)
            {
                _roleUpgradeView.CloseAllPanel();
            }
            GameTimer.inst.AddTimer(0.2f, 1, showUpgradePanel);
            return;
        }
        //if (_roleUpgradeView != null || _roleUpgradeView.isShowing)
        //{
        //    _roleUpgradeView.hide();
        //    upgradePanelIsShow = false;
        //}
        GUIManager.HideView<RoleUpgradeView>();
        upgradePanelIsShow = false;
    }

    private void showUpgradePanel()
    {
        if (itemLists.Count <= 0)
        {
            GUIManager.HideView<RoleUpgradeView>();
            upgradePanelIsShow = false;
            return;
        }
        upgradeItem tempItem = itemLists[0];
        GUIManager.OpenView<RoleUpgradeView>((view) =>
        {
            _roleUpgradeView = view;
            _roleUpgradeView.ShowUpgradePanel(tempItem);
            upgradePanelIsShow = false;
        });
        itemLists.Remove(tempItem);
    }

    void exploreEndHeroUpgradeStart()
    {
        GUIManager.OpenView<ExploreFinishView>((view) =>
        {
            _exploreFinishView = view;
            view.AddHeroCount();
        });
    }

    void exploreEndHeroUpgradeEnd()
    {
        _exploreFinishView = GUIManager.GetWindow<ExploreFinishView>();
        if (_exploreFinishView != null && _exploreFinishView.isShowing)
        {
            _exploreFinishView.RemoveHeroCount();
        }
        //GUIManager.OpenView<ExploreFinishView>((view) =>
        //{
        //    _exploreFinishView = view;
        //    view.RemoveHeroCount();
        //});
    }

    void exploreEndExploreUpgradeEnd()
    {
        GUIManager.OpenView<ExploreFinishView>((view) =>
        {
            _exploreFinishView = view;
            view.ExploreLevelUpComplete();
        });
    }

    #region ui界面逻辑
    void showExploreBuySlot(int slotCount)
    {
        GUIManager.OpenView<ExploreBuySlotView>((view) =>
        {
            _exploreBuySlotView = view;
            view.Init(slotCount);
        });
    }

    void hideExploreBuySlot()
    {
        GUIManager.HideView<ExploreBuySlotView>();
    }

    // 副本界面
    void showExploreUI()
    {
        GUIManager.OpenView<ExplorePanelView>((view) =>
        {
            _exploreView = view;
            view.setData();
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.Reset_TargetExplore);
        });
    }

    void hideExploreUI()
    {
        GUIManager.HideView<ExplorePanelView>();
    }

    // 使用副本道具界面
    void showExploreUseItemUI(int itemType)
    {
        GUIManager.OpenView<ExploreUseItemView>((view) =>
        {
            _exploreUseItemView = view;
            view.GetItemLists(itemType);
        });
    }

    void useItemComplete(int itemId)
    {
        GUIManager.OpenView<ExplorePrepareView>((view) =>
        {
            _explorePrepareView = view;
            view.SelectItemComplete(itemId);
        });
    }

    void showExploreInfoUI(int group)
    {
        GUIManager.OpenView<ExploreInfoView>((view) =>
        {
            _exploreInfoView = view;
            view.setData(group);
        });
    }

    void showExploreUnlockUI(int group)
    {
        GUIManager.OpenView<ExploreUnlockView>((view) =>
       {
           _exploreUnlockView = view;
           view.setData(group);
       });
    }

    void hideExploreUnlockUI()
    {
        GUIManager.HideView<ExploreUnlockView>();
    }

    void showExplorePrepareUI(int group, int index)
    {
        GUIManager.OpenView<ExplorePrepareView>((view) =>
        {
            _explorePrepareView = view;
            view.setData(group, index);
        });
    }

    void hideExplorePrepareUI()
    {
        GUIManager.HideView<ExplorePrepareView>();
    }

    void showExploreSelectHeroUI(int index, int exploreId, float itemAddPercent)
    {
        GUIManager.OpenView<ExploreSelectHeroView>((view) =>
        {
            _exploreSelectHeroView = view;
            view.setData(index, exploreId, itemAddPercent);
        });
    }

    void hideExploreSelectHeroUI()
    {
        GUIManager.HideView<ExploreSelectHeroView>();
    }

    void exploreRemoveHeroCom(int heroUid, int index)
    {
        GUIManager.OpenView<ExploreSelectHeroView>((view) =>
        {
            _exploreSelectHeroView = view;
            view.RemoveHeroComplete(heroUid, index);
        });
    }

    void explorePrepareAddHeroCom(int heroUid, int index)
    {
        //GUIManager.OpenView<ExplorePrepareView>((view) =>
        //{
        //    _explorePrepareView = view;
        //    view.AddHeroCom(heroUid, index);
        //});
        if (_explorePrepareView != null && _explorePrepareView.isShowing)
        {
            _explorePrepareView.AddHeroCom(heroUid, index);
        }
    }

    void explorePrepareRemoveHeroCom(int heroUid, int index)
    {
        GUIManager.OpenView<ExplorePrepareView>((view) =>
        {
            _explorePrepareView = view;
            view.RemoveHeroCom(heroUid, index);
        });
    }

    void updateCityUISlotData(ESlotAnimType animType)
    {
        var cityUI = GUIManager.GetWindow<CityMainView>();

        if (cityUI != null && cityUI.isShowing)
        {
            cityUI.updateMakeSlots(animType);
        }
    }
    void showAdventureUIBySlot(int slotId)
    {
        GUIManager.OpenView<RoleAdventureView>((view) =>
        {
            _roleAdventureView = view;
            view.setAdventureDataBySlot(slotId);
        });
    }

    void showFinishSuccessUI(List<OneRewardItem> bagResources, int slotId, int exploreId)
    {
        GUIManager.OpenView<ExploreFinishView>((view) =>
        {
            _exploreFinishView = view;
            view.setExploreSuccessFinishData(bagResources, slotId, exploreId);
        });
    }

    void showFinishLoseUI(int exploreId)
    {
        GUIManager.OpenView<ExploreFinishView>((view) =>
            {
                _exploreFinishView = view;
                view.setExploreLoseFinishData(exploreId);
            });
    }

    #endregion

    #region 网络消息
    void requestExploreGroupData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_Data()
        });
    }

    void requestExploreSlotData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_ExploreSlot_Data()
        });
    }

    void requestExploreBuySlotData(int useGem)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_BuySlot()
            {
                useGem = useGem
            }
        });
    }

    void requestExploreStartData(ExploreSlot data, int useItemId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_Start()
            {
                exploreSlotId = data.slotId,
                exploreId = data.exploreId,
                exploreType = data.exploreType,
                useItemId = useItemId,
                heroInfoUIds = data.heroInfoUIds
            }
        });
    }

    void requestExploreEndData(int exploreSlotId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_End()
            {
                exploreSlotId = exploreSlotId
            }
        });
    }

    void requestExploreUnlockData(int group, int useGem)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_Unlock()
            {
                group = group,
                useGem = useGem
            }
        });
    }

    void requestExploreSlotRefreshData(int exploreSlotId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_Refresh()
            {
                slotId = exploreSlotId
            }
        });
    }

    void requestExploreSlotImmediately(int exploreSlotId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_Immediately()
            {
                slotId = exploreSlotId
            }
        });
    }

    void requestExploreVipReward(int exploreSlotId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Explore_RewardVip()
            {
                exploreSlotId = exploreSlotId
            }
        });
    }

    void responseSetAdventureData(int slotId)
    {
        _roleAdventureView = GUIManager.GetWindow<RoleAdventureView>();
        if (_roleAdventureView != null && _roleAdventureView.isShowing)
        {
            _roleAdventureView.setAdventureDataBySlot(slotId);
        }
    }

    void responsePrepareRefreshData()
    {
        _explorePrepareView = GUIManager.GetWindow<ExplorePrepareView>();
        if (_explorePrepareView != null && _explorePrepareView.isShowing)
        {
            _explorePrepareView.refreshData();
        }
    }
    #endregion
}
