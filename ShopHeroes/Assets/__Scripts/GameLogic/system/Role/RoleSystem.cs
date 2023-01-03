using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 英雄系统
public class RoleSystem : BaseSystem
{
    RolePanelUIView _rolePanelUI;
    RoleBuySlotView _buySlotUI;
    GetHeroPanelView _getHeroUI;
    RoleRestingView _roleRestingUI;
    RoleHeroInfoView _roleInfoUI;
    RoleRecruitBarView _roleRecruitUI;
    RoleIntroducePanelView _roleIntroduceUI;
    RoleTransferView _roleTransferUI;
    RoleUseExpItemView _roleUseExpUI;
    HeroWearEquipView _heroWearEquipView;
    RoleEquipView _roleEquipUI;
    RoleUpgradeView _roleUpgradeUI;
    RoleAdventureView _roleAdventureUI;
    RoleRecruitSubView _roleRecruitSubUI;
    RoleHeroAttributeView _roleHeroAttributeUI;

    //---------------------- 工匠 -------------------------------
    WorkerInfoView _workerInfoUI;
    WorkerRecruitUI _workerRecruitUI;
    UnlockWorkerUI _unlockWorkerUI;
    WorkerUpUI _workerUpUI;


    //-------------------装备破损--------------------//
    RoleEquipDamagedView equipDamagedUI;
    RoleEquipDamagedInfoView equipDamagedInfoUI;

    //------------------转职预览------------------//
    RoleTransferPreviewUI previewUI;


    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener(GameEventType.RoleEvent.ROLE_SHOWUI, showRoleUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLE_HIDEUI, hideRoleUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.BUYSLOT_SHOWUI, showBuyRoleSlotUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.BUYSLOT_HIDEUI, hideBuyRoleSlotUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.GETHERO_SHOWUI, showGetHeroUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ALLROLERESTING_SHOWUI, showAllRoleRestingUI);
        EventController.inst.AddListener<RoleHeroData, int>(GameEventType.RoleEvent.SINGLEROLERESTING_SHOWUI, showSingleRoleRestingUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLERESTING_HIDEUI, hideRoleRestingUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLEINFO_SHOWUI, showRoleInfoUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLEINFO_HIDEUI, hideRoleInfoUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI, showRoleRecruitUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLERECRUIT_HIDEUI, hideRoleRecruitUI);
        EventController.inst.AddListener<Transform, HeroSkillShowConfig>(GameEventType.RoleEvent.ROLEINTRODUCE_SHOWUI, showRoleIntroduceUI);
        EventController.inst.AddListener<Transform, HeroTalentDataBase>(GameEventType.RoleEvent.ROLETALENTINTRODUCE_SHOWUI, showRoleTalentIntroduceUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLETRANSFER_SHOWUI, showRoleTransferUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLETRANSFER_HIDEUI, hideRoleTransferUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLEUSEEXPITEM_SHOWUI, showRoleUseExpItemUI);
        EventController.inst.AddListener<int[], int, int, int>(GameEventType.RoleEvent.ROLEWEAREQUIP_SHOWUI, showHeroWearEquipUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLEWEAREQUIP_HIDEUI, hideHeroWearEquipUI);
        EventController.inst.AddListener<HeroEquip, int>(GameEventType.RoleEvent.ROLEEQUIP_SHOWUI, showRoleEquipUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLEEQUIP_HIDEUI, hideRoleEquipUI);
        EventController.inst.AddListener<HeroInfo>(GameEventType.RoleEvent.ROLEUSEITEMANIM_SHOW, showUseExpItemAnim);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLEUPGRADE_SHOWUI, showRoleUpgradeUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLETRANSFERCOM_SHOWUI, showRoleTransferComUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLEADVENTUREBYSLOT_SHOWUI, showAdventureUIBySlot);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLEADVENTURE_HIDEUI, hideAdventureUI);
        EventController.inst.AddListener<RoleRecruitData, int>(GameEventType.RoleEvent.ROLERECRUITSUB_SHOWUI, showRecruitSubUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLERECRUITSUB_HIDEUI, hideRecruitSubUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.ROLEINFO_SHOW, showRoleInfoUI);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.RESPONSE_HEROEQUIPAUTO, roleEquipAutoEnd);


        //--工匠
        EventController.inst.AddListener<WorkerData>(GameEventType.WorkerCompEvent.SHOWUI_WORKERINFOUI, showWorkerInfoUI);
        EventController.inst.AddListener<int>(GameEventType.WorkerCompEvent.Worker_UnLock, showWorkerUnlockUI);
        EventController.inst.AddListener<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, clickWorkerRecruitHandler);
        EventController.inst.AddListener<int>(GameEventType.WorkerCompEvent.Worker_LevelChange, showWorkerUpUI);
        EventController.inst.AddListener<int, EItemType>(GameEventType.WorkerCompEvent.REQUEST_Worker_Recruit, request_workerRecruit);



        //EventController.inst.AddListener(GameEventType.WorkerCompEvent.REQUEST_Worker_Recruit);//发送给后端要解锁工匠的请求



        //
        EventController.inst.AddListener(GameEventType.RoleEvent.REQUEST_ROLEDATA, requestRoleData);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.REQUEST_BUYNEWSLOT, requestBuyNewSlot);
        EventController.inst.AddListener(GameEventType.RoleEvent.REQUEST_RECRUITLIST, requestRecruitRoleListData);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, requestRecruitRefresh);
        EventController.inst.AddListener<int, int>(GameEventType.RoleEvent.REQUEST_BUYHERO, requestHeroBuy);
        EventController.inst.AddListener<int, int, int, string>(GameEventType.RoleEvent.REQUEST_WEAREQUIP, requestWearEquip);
        EventController.inst.AddListener<int, List<HeroEquipAuto>>(GameEventType.RoleEvent.REQUEST_HEROWEARALLEQUIP, requestWearAllEquips);
        EventController.inst.AddListener<string, int>(GameEventType.RoleEvent.REQUEST_RENAME, requestHeroRename);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.REQUEST_DISMISSAL, requestDismissalHero);
        EventController.inst.AddListener<int, int>(GameEventType.RoleEvent.REQUEST_USEHEROITEM, requestUseHeroItem);
        EventController.inst.AddListener<int, int>(GameEventType.RoleEvent.REQUEST_HEROTRANSFER, requestHeroTransfer);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.REQUEST_HEROREFRESH, requestHeroRefresh);
        EventController.inst.AddListener<int, int>(GameEventType.RoleEvent.REQUEST_HERORECOVER, requestRecoverHero);
        EventController.inst.AddListener(GameEventType.RoleEvent.REQUEST_HEROEXCHANGELIST, requestHeroExchangeList);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.REQUEST_HEROEXCHANGE, requestHeroExchange);

        EventController.inst.AddListener(GameEventType.RoleEvent.RESPONSE_RECRUITLIST, responseRecruitRoleListData);
        EventController.inst.AddListener(GameEventType.RoleEvent.RESPONSE_RECRUITLISTBAR, responseRecruitRoleListRecruitData);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.RESPONSE_SETHEROINFODATA, response_SetHeroInfoData);
        EventController.inst.AddListener<RoleHeroData, int>(GameEventType.RoleEvent.RESPONSE_RESTINGSETDATA, response_SetRestingData);
        EventController.inst.AddListener(GameEventType.RoleEvent.RESPONSE_EXPLORESHIFTIN, response_ExploreShiftIn);
        EventController.inst.AddListener(GameEventType.RoleEvent.RESPONSE_HEROSHIFTIN, response_HeroShiftIn);
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, response_HeroTypeChange);

        // 装备破损
        EventController.inst.AddListener<Transform, int>(GameEventType.RoleEvent.SETDAMAGEDEQUPINTRODUCE, setDamagedEquipIntroduce);
        EventController.inst.AddListener(GameEventType.RoleEvent.SHOWUI_EQUIPDAMAGED, showEquipDamagedUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.HIDEUI_EQUIPDAMAGED, hideEquipDamagedUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.SHOWUI_EQUIPDAMAGEDINFO, showEquipDamagedInfoUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.REFRESHUI_EQUIPDAMAGEDINFO, refreshEquipDamagedInfoUI);
        EventController.inst.AddListener(GameEventType.RoleEvent.HIDEUI_EQUIPDAMAGEDINFO, hideEquipDamagedInfoUI);
        EventController.inst.AddListener<int, int>(GameEventType.RoleEvent.REQUEST_REPAIREQUIP, requestRepairEquip);
        EventController.inst.AddListener(GameEventType.RoleEvent.RESPONSE_REPAIREQUIPDATA, response_RepairEquip);

        // 转职预览
        EventController.inst.AddListener<int>(GameEventType.RoleEvent.SHOWUI_TRANSFERPREVIEW, showTransferPreview);
        // 具体属性
        EventController.inst.AddListener<HeroPropertyData>(GameEventType.RoleEvent.SHOWUI_ROLEHEROATTVIEW, showHeroAttributeInfo);

        EventController.inst.AddListener<int>(GameEventType.RoleEvent.ROLETRANSFERJUMPTOTOGGLE, roleTransferJumpToToggle);

        EventController.inst.AddListener(GameEventType.RoleEvent.ROLEISSHOWING_SHOWUI, showShowingRoleUI);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLE_SHOWUI, showRoleUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLE_HIDEUI, hideRoleUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.BUYSLOT_SHOWUI, showBuyRoleSlotUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.BUYSLOT_HIDEUI, hideBuyRoleSlotUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.GETHERO_SHOWUI, showGetHeroUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ALLROLERESTING_SHOWUI, showAllRoleRestingUI);
        EventController.inst.RemoveListener<RoleHeroData, int>(GameEventType.RoleEvent.SINGLEROLERESTING_SHOWUI, showSingleRoleRestingUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLERESTING_HIDEUI, hideRoleRestingUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLEINFO_SHOWUI, showRoleInfoUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLEINFO_HIDEUI, hideRoleInfoUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI, showRoleRecruitUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLERECRUIT_HIDEUI, hideRoleRecruitUI);
        EventController.inst.RemoveListener<Transform, HeroSkillShowConfig>(GameEventType.RoleEvent.ROLEINTRODUCE_SHOWUI, showRoleIntroduceUI);
        EventController.inst.RemoveListener<Transform, HeroTalentDataBase>(GameEventType.RoleEvent.ROLETALENTINTRODUCE_SHOWUI, showRoleTalentIntroduceUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLETRANSFER_SHOWUI, showRoleTransferUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLETRANSFER_HIDEUI, hideRoleTransferUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLEUSEEXPITEM_SHOWUI, showRoleUseExpItemUI);
        EventController.inst.RemoveListener<int[], int, int, int>(GameEventType.RoleEvent.ROLEWEAREQUIP_SHOWUI, showHeroWearEquipUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLEWEAREQUIP_HIDEUI, hideHeroWearEquipUI);
        EventController.inst.RemoveListener<HeroEquip, int>(GameEventType.RoleEvent.ROLEEQUIP_SHOWUI, showRoleEquipUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLEEQUIP_HIDEUI, hideRoleEquipUI);
        EventController.inst.RemoveListener<HeroInfo>(GameEventType.RoleEvent.ROLEUSEITEMANIM_SHOW, showUseExpItemAnim);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLEUPGRADE_SHOWUI, showRoleUpgradeUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLETRANSFERCOM_SHOWUI, showRoleTransferComUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLEADVENTUREBYSLOT_SHOWUI, showAdventureUIBySlot);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLEADVENTURE_HIDEUI, hideAdventureUI);
        EventController.inst.RemoveListener<RoleRecruitData, int>(GameEventType.RoleEvent.ROLERECRUITSUB_SHOWUI, showRecruitSubUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLERECRUITSUB_HIDEUI, hideRecruitSubUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLEINFO_SHOW, showRoleInfoUI);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.RESPONSE_HEROEQUIPAUTO, roleEquipAutoEnd);


        //--工匠
        EventController.inst.RemoveListener<WorkerData>(GameEventType.WorkerCompEvent.SHOWUI_WORKERINFOUI, showWorkerInfoUI);
        EventController.inst.RemoveListener<int>(GameEventType.WorkerCompEvent.Worker_UnLock, showWorkerUnlockUI);
        EventController.inst.RemoveListener<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, clickWorkerRecruitHandler);
        EventController.inst.RemoveListener<int>(GameEventType.WorkerCompEvent.Worker_LevelChange, showWorkerUpUI);
        EventController.inst.RemoveListener<int, EItemType>(GameEventType.WorkerCompEvent.REQUEST_Worker_Recruit, request_workerRecruit);
        //
        EventController.inst.RemoveListener(GameEventType.RoleEvent.REQUEST_ROLEDATA, requestRoleData);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.REQUEST_BUYNEWSLOT, requestBuyNewSlot);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.REQUEST_RECRUITLIST, requestRecruitRoleListData);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, requestRecruitRefresh);
        EventController.inst.RemoveListener<int, int>(GameEventType.RoleEvent.REQUEST_BUYHERO, requestHeroBuy);
        EventController.inst.RemoveListener<int, int, int, string>(GameEventType.RoleEvent.REQUEST_WEAREQUIP, requestWearEquip);
        EventController.inst.RemoveListener<int, List<HeroEquipAuto>>(GameEventType.RoleEvent.REQUEST_HEROWEARALLEQUIP, requestWearAllEquips);
        EventController.inst.RemoveListener<string, int>(GameEventType.RoleEvent.REQUEST_RENAME, requestHeroRename);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.REQUEST_DISMISSAL, requestDismissalHero);
        EventController.inst.RemoveListener<int, int>(GameEventType.RoleEvent.REQUEST_USEHEROITEM, requestUseHeroItem);
        EventController.inst.RemoveListener<int, int>(GameEventType.RoleEvent.REQUEST_HEROTRANSFER, requestHeroTransfer);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.REQUEST_HEROREFRESH, requestHeroRefresh);
        EventController.inst.RemoveListener<int, int>(GameEventType.RoleEvent.REQUEST_HERORECOVER, requestRecoverHero);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.REQUEST_HEROEXCHANGELIST, requestHeroExchangeList);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.REQUEST_HEROEXCHANGE, requestHeroExchange);

        EventController.inst.RemoveListener(GameEventType.RoleEvent.RESPONSE_RECRUITLIST, responseRecruitRoleListData);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.RESPONSE_RECRUITLISTBAR, responseRecruitRoleListRecruitData);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.RESPONSE_SETHEROINFODATA, response_SetHeroInfoData);
        EventController.inst.RemoveListener<RoleHeroData, int>(GameEventType.RoleEvent.RESPONSE_RESTINGSETDATA, response_SetRestingData);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.RESPONSE_EXPLORESHIFTIN, response_ExploreShiftIn);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.RESPONSE_HEROSHIFTIN, response_HeroShiftIn);
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.RESPONSE_HEROTYPECHANGE, response_HeroTypeChange);

        // 装备破损
        EventController.inst.RemoveListener<Transform, int>(GameEventType.RoleEvent.SETDAMAGEDEQUPINTRODUCE, setDamagedEquipIntroduce);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.SHOWUI_EQUIPDAMAGED, showEquipDamagedUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.HIDEUI_EQUIPDAMAGED, hideEquipDamagedUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.SHOWUI_EQUIPDAMAGEDINFO, showEquipDamagedInfoUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.REFRESHUI_EQUIPDAMAGEDINFO, refreshEquipDamagedInfoUI);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.HIDEUI_EQUIPDAMAGEDINFO, hideEquipDamagedInfoUI);
        EventController.inst.RemoveListener<int, int>(GameEventType.RoleEvent.REQUEST_REPAIREQUIP, requestRepairEquip);
        EventController.inst.RemoveListener(GameEventType.RoleEvent.RESPONSE_REPAIREQUIPDATA, response_RepairEquip);

        // 转职预览
        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.SHOWUI_TRANSFERPREVIEW, showTransferPreview);
        // 具体属性
        EventController.inst.RemoveListener<HeroPropertyData>(GameEventType.RoleEvent.SHOWUI_ROLEHEROATTVIEW, showHeroAttributeInfo);

        EventController.inst.RemoveListener<int>(GameEventType.RoleEvent.ROLETRANSFERJUMPTOTOGGLE, roleTransferJumpToToggle);

        EventController.inst.RemoveListener(GameEventType.RoleEvent.ROLEISSHOWING_SHOWUI, showShowingRoleUI);
    }

    #region 界面事件
    void roleTransferJumpToToggle(int heroId)
    {
        if (_roleTransferUI == null)
        {
            GUIManager.OpenView<RoleTransferView>((view) =>
            {
                _roleTransferUI = view;
                view.jumpToTargetToggle(heroId);
            });
        }
        if (_roleTransferUI != null && _roleTransferUI.isShowing)
        {
            _roleTransferUI.jumpToTargetToggle(heroId);
        }
    }

    // 具体属性
    void showHeroAttributeInfo(HeroPropertyData data)
    {
        GUIManager.OpenView<RoleHeroAttributeView>((view) =>
        {
            _roleHeroAttributeUI = view;
            view.setData(data);
        });
    }

    // 转职预览
    void showTransferPreview(int heroId)
    {
        GUIManager.OpenView<RoleTransferPreviewUI>((view) =>
        {
            previewUI = view;
            view.setData(heroId);
        });
    }

    // 装备破损
    void hideEquipDamagedInfoUI()
    {
        GUIManager.HideView<RoleEquipDamagedInfoView>();
    }

    void showEquipDamagedInfoUI()
    {
        GUIManager.OpenView<RoleEquipDamagedInfoView>((view) =>
        {
            equipDamagedInfoUI = view;
            view.setData();
        });
    }

    void refreshEquipDamagedInfoUI()
    {
        equipDamagedInfoUI = GUIManager.GetWindow<RoleEquipDamagedInfoView>();
        if (equipDamagedInfoUI != null && equipDamagedInfoUI.isShowing)
        {
            equipDamagedInfoUI.refreshBuyVip();
        }
    }

    void hideEquipDamagedUI()
    {
        GUIManager.HideView<RoleEquipDamagedView>();
    }

    void showEquipDamagedUI()
    {
        GUIManager.OpenView<RoleEquipDamagedView>((view) =>
        {
            equipDamagedUI = view;
            view.setData();
        });
    }

    void setDamagedEquipIntroduce(Transform trans, int equipId)
    {
        equipDamagedUI = GUIManager.GetWindow<RoleEquipDamagedView>();
        if (equipDamagedUI != null && equipDamagedUI.isShowing)
        {
            equipDamagedUI.setIntroduceData(trans, equipId);
        }
    }

    // 角色列表界面
    void showRoleUI()
    {
        GUIManager.OpenView<RolePanelUIView>((view) =>
        {
            _rolePanelUI = view;
            //_rolePanelUI.toggleIndexSet(0);
        });
    }

    void showShowingRoleUI()
    {
        _rolePanelUI = GUIManager.GetWindow<RolePanelUIView>();
        if (_rolePanelUI != null && _rolePanelUI.isShowing)
        {
            _rolePanelUI.setData();
        }
    }

    void hideRoleUI()
    {
        GUIManager.HideView<RolePanelUIView>();
    }

    // 购买角色槽位界面
    void showBuyRoleSlotUI(int fieldCount)
    {
        GUIManager.OpenView<RoleBuySlotView>((view) =>
        {
            _buySlotUI = view;
            if (_rolePanelUI != null)
                _rolePanelUI.roleType = kRoleType.max;
            view.Init(fieldCount);
        });
    }

    void hideBuyRoleSlotUI()
    {
        GUIManager.HideView<RoleBuySlotView>();
    }

    // 获得新英雄界面
    void showGetHeroUI(int heroUid)
    {
        GUIManager.OpenView<GetHeroPanelView>((view) =>
        {
            _getHeroUI = view;
            view.InitData(heroUid);
        });
    }

    // 英雄休息界面
    void showAllRoleRestingUI()
    {
        GUIManager.OpenView<RoleRestingView>((view) =>
        {
            _roleRestingUI = view;
            view.setAllData();
        });
    }

    void showSingleRoleRestingUI(RoleHeroData roleData, int isFromHeroInfo)
    {
        GUIManager.OpenView<RoleRestingView>((view) =>
        {
            _roleRestingUI = view;
            view.setSingleData(roleData, isFromHeroInfo);
        });
    }

    void hideRoleRestingUI()
    {
        GUIManager.HideView<RoleRestingView>();
    }

    // 英雄信息界面
    void showRoleInfoUI(int heroUid)
    {
        _roleInfoUI = GUIManager.OpenView<RoleHeroInfoView>((view) =>
        {
            view.setHeroInfoData(heroUid);
        });
    }

    void showRoleInfoUI()
    {
        _roleInfoUI = GUIManager.OpenView<RoleHeroInfoView>();
    }

    void roleEquipAutoEnd(int heroUid)
    {
        _roleInfoUI = GUIManager.GetWindow<RoleHeroInfoView>();
        if (_roleInfoUI != null && _roleInfoUI.isShowing)
        {
            _roleInfoUI.RoleEquipAutoEnd(heroUid);
        }
    }


    void hideRoleInfoUI()
    {
        GUIManager.HideView<RoleHeroInfoView>();
    }

    // 英雄招募界面
    void showRoleRecruitUI()
    {
        GUIManager.OpenView<RoleRecruitBarView>((view) =>
        {
            _roleRecruitUI = view;
            view.InitRecruitHeroData();
        });
    }

    void hideRoleRecruitUI()
    {
        GUIManager.HideView<RoleRecruitBarView>();
    }

    // 技能天赋介绍界面
    void showRoleIntroduceUI(Transform pos, HeroSkillShowConfig skillData)
    {
        GUIManager.OpenView<RoleIntroducePanelView>((view) =>
        {
            _roleIntroduceUI = view;
            view.SetTextAndVectorPos(pos, skillData);
        });
    }

    void showRoleTalentIntroduceUI(Transform pos, HeroTalentDataBase talentData)
    {
        GUIManager.OpenView<RoleIntroducePanelView>((view) =>
        {
            _roleIntroduceUI = view;
            view.SetTalentData(pos, talentData);
        });
    }

    // 英雄转职界面
    void showRoleTransferUI(int heroUid)
    {
        GUIManager.OpenView<RoleTransferView>((view) =>
        {
            _roleTransferUI = view;
            view.setData(heroUid);
        });
    }

    void hideRoleTransferUI()
    {
        GUIManager.HideView<RoleTransferView>();
    }

    //_roleUseExpUI
    // 英雄使用经验道具界面
    void showRoleUseExpItemUI(int heroUid)
    {
        GUIManager.OpenView<RoleUseExpItemView>((view) =>
        {
            _roleUseExpUI = view;
            view.setData(heroUid);
        });
    }

    // 仓库中的装备界面
    void showHeroWearEquipUI(int[] ids, int heroUid, int equipFieldId, int onOrOff)
    {
        GUIManager.OpenView<HeroWearEquipView>((view) =>
        {
            _heroWearEquipView = view;
            view.GetItemLists(ids, heroUid, equipFieldId, onOrOff);
        });
    }

    void hideHeroWearEquipUI()
    {
        GUIManager.HideView<HeroWearEquipView>();
    }

    // 英雄装备界面
    void showRoleEquipUI(HeroEquip heroEquip, int heroUid)
    {
        GUIManager.OpenView<RoleEquipView>((view) =>
        {
            _roleEquipUI = view;
            view.setEquipData(heroEquip, heroUid);
        });
    }

    void hideRoleEquipUI()
    {
        GUIManager.HideView<RoleEquipView>();
    }

    void showUseExpItemAnim(HeroInfo curInfo)
    {
        _roleUseExpUI = GUIManager.GetWindow<RoleUseExpItemView>();
        if (_roleUseExpUI != null && _roleUseExpUI.isShowing)
        {
            _roleUseExpUI.useExpItemAnim(curInfo);
        }
        //GUIManager.OpenView<RoleUseExpItemView>((view) =>
        //{
        //    _roleUseExpUI = view;
        //    view.useExpItemAnim(curInfo);
        //});
    }

    // 英雄升级界面
    void showRoleUpgradeUI(int heroUid)
    {
        GUIManager.OpenView<RoleUpgradeView>((view) =>
      {
          _roleUpgradeUI = view;
          view.setData(heroUid, 1);
      });
    }

    // 英雄进阶完毕界面
    void showRoleTransferComUI(int heroUid)
    {
        GUIManager.OpenView<RoleUpgradeView>((view) =>
        {
            _roleUpgradeUI = view;
            view.setTransferData(heroUid);
        });
    }

    void showAdventureUIBySlot(int slotId)
    {
        //GUIManager.OpenView<RoleAdventureView>((view) =>
        //{
        //    _roleAdventureUI = view;
        //    view.setAdventureDataBySlot(slotId);
        //});
    }

    void hideAdventureUI()
    {
        GUIManager.HideView<RoleAdventureView>();
    }

    void showRecruitSubUI(RoleRecruitData heroData, int index)
    {
        GUIManager.OpenView<RoleRecruitSubView>((view) =>
        {
            _roleRecruitSubUI = view;
            view.setData(heroData, index);
        });
    }

    void hideRecruitSubUI()
    {
        GUIManager.HideView<RoleRecruitSubView>();
    }

    #region 工匠
    private void showWorkerInfoUI(WorkerData workerData)
    {
        GUIManager.OpenView<WorkerInfoView>((view) =>
        {
            view.Init(workerData);
        });
    }

    private void showWorkerUnlockUI(int workerId)
    {
        GUIManager.OpenView<UnlockWorkerUI>((view) =>
        {
            view.SetData(workerId);
        });
    }

    private void showWorkerUpUI(int workerId)
    {
        GUIManager.OpenView<WorkerUpUI>((view) =>
        {
            view.SetData(workerId);
        });
    }

    private void clickWorkerRecruitHandler(int workerId, bool showNear, System.Action callback)
    {
        WorkerData workerData = RoleDataProxy.inst.GetWorker(workerId);

        if (workerData != null)
        {

            switch (workerData.state)
            {
                case EWorkerState.Locked:
                case EWorkerState.CanUnlock:

                    //switch ((kRoleWorkerGetType)workerData.config.get_type)
                    //{
                    //    case kRoleWorkerGetType.turntable: //转盘
                    //    case kRoleWorkerGetType.guide://引导
                    //    case kRoleWorkerGetType.buy://雇佣
                    //    case kRoleWorkerGetType.sevenDay://签到

                    //        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey(workerData.config.lock_des), GUIHelper.GetColorByColorHex("FF2828"));

                    //        break;

                    //    default:

                    //        break;
                    //}

                    GUIManager.OpenView<WorkerRecruitUI>((view) =>
                    {
                        view.SetData(workerData, showNear, callback);
                    });

                    break;
                case EWorkerState.Unlock:
                    break;
            }
        }

    }

    #endregion

    #endregion

    #region 网络消息事件
    // 兑换英雄
    void requestHeroExchange(int id)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Exchange()
            {
                id = id
            }
        });
    }

    // 获取兑换列表
    void requestHeroExchangeList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_ExchangeList()
        });
    }

    // 装备破损
    void requestRepairEquip(int heroUid, int costType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_FixBrokenEquip()
            {
                heroUid = heroUid,
                costType = costType
            }
        });
    }

    // 获取角色列表
    void requestRoleData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Data()
        });
    }

    // 购买新的槽位
    void requestBuyNewSlot(int buyType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_FieldUnlock()
            {
                useGem = buyType
            }
        });
    }

    // 获取招募列表
    void requestRecruitRoleListData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_BuyList()
        });
    }

    // 获取招募刷新列表
    void requestRecruitRefresh(int costType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_BuyListRefresh()
            {
                refreshCostType = costType
            }
        });
    }

    // 招募英雄
    void requestHeroBuy(int index, int costType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Buy()
            {
                heroIndex = index,
                costType = costType
            }
        });
    }

    // 穿戴装备
    void requestWearEquip(int heroUid, int equipFieldId, int onOrOff, string equipUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Equip()
            {
                heroUid = heroUid,
                equipField = equipFieldId,
                onOrOff = onOrOff,
                equipUid = equipUid
            }
        });
    }

    //一键换装
    void requestWearAllEquips(int heroUid, List<HeroEquipAuto> equips)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_EquipAuto()
            {
                heroUid = heroUid,
                equip = equips
            }
        });
    }

    // 更改英雄名称
    void requestHeroRename(string nickName, int heroUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Setting()
            {
                heroUid = heroUid,
                nickName = nickName
            }
        });
    }

    // 解雇英雄
    void requestDismissalHero(int heroUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Fire()
            {
                heroUid = heroUid
            }
        });
    }

    // 使用道具
    void requestUseHeroItem(int itemId, int heroUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_UseItem()
            {
                itemId = itemId,
                heroId = heroUid
            }
        });
    }

    void requestRecoverHero(int heroUid, int costType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Recover()
            {
                heroUid = heroUid,
                costType = costType
            }
        });
    }

    // 英雄转职
    void requestHeroTransfer(int heroUid, int targetHeroId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_Transfer()
            {
                heroUid = heroUid,
                targetHeroId = targetHeroId
            }
        });
    }

    // 英雄刷新
    void requestHeroRefresh(int heroUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_DataRefresh()
            {
                heroUid = heroUid,
            }
        });
    }

    // 招募工匠
    void request_workerRecruit(int workerId, EItemType costType)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_WorkerUnlock()
            {
                workerId = workerId,
                unlockItemType = (int)costType,
            }
        });
    }


    void responseRecruitRoleListData()
    {
        _roleRecruitUI = GUIManager.GetWindow<RoleRecruitBarView>();
        if (_roleRecruitUI != null && _roleRecruitUI.isShowing)
        {
            _roleRecruitUI.InitRecruitHeroData();
        }
    }

    void responseRecruitRoleListRecruitData()
    {
        _roleRecruitUI = GUIManager.GetWindow<RoleRecruitBarView>();
        if (_roleRecruitUI != null && _roleRecruitUI.isShowing)
        {
            _roleRecruitUI.doCardsAnim();
        }
    }

    void response_SetHeroInfoData(int heroUid)
    {
        _roleInfoUI = GUIManager.GetWindow<RoleHeroInfoView>();
        if (_roleInfoUI != null && _roleInfoUI.isShowing)
        {
            if (_roleInfoUI.heroUid == heroUid)
                _roleInfoUI.setHeroInfoData(heroUid);
        }
    }

    void response_SetRestingData(RoleHeroData heroData, int isFromHeroInfo)
    {
        _roleRestingUI = GUIManager.GetWindow<RoleRestingView>();
        if (_roleRestingUI != null && _roleRestingUI.isShowing)
        {
            if (_roleRestingUI.restType == 1)
            {
                _roleRestingUI.setAllData();
            }
            else if (_roleRestingUI.restType == 0)
            {
                _roleRestingUI.setSingleData(heroData, isFromHeroInfo);
            }
        }
    }

    void response_ExploreShiftIn()
    {
        var exploreSelectHero = GUIManager.GetWindow<ExploreSelectHeroView>();
        if (exploreSelectHero != null && exploreSelectHero.isShowing)
        {
            exploreSelectHero.shiftIn();
        }
    }

    void response_HeroShiftIn()
    {
        _roleInfoUI = GUIManager.GetWindow<RoleHeroInfoView>();
        if (_roleInfoUI != null && _roleInfoUI.isShowing)
        {
            _roleInfoUI.shiftIn();
        }
    }

    void response_HeroTypeChange(int index)
    {
        _rolePanelUI = GUIManager.GetWindow<RolePanelUIView>();
        if (_rolePanelUI != null && _rolePanelUI.isShowing)
        {
            _rolePanelUI.toggleIndexSet(index);
        }
        else
        {
            GUIManager.OpenView<RolePanelUIView>((view) =>
            {
                _rolePanelUI = view;
                _rolePanelUI.toggleIndexSet(index);

            });
        }
      
    }

    void response_RepairEquip()
    {
        equipDamagedInfoUI = GUIManager.GetWindow<RoleEquipDamagedInfoView>();
        if (equipDamagedInfoUI != null && equipDamagedInfoUI.isShowing)
        {
            equipDamagedInfoUI.ResponseRepairData();
        }
    }

    #endregion
}
