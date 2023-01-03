using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


public class FurnitureDisplayData
{
    public int id;
    public int uid = -1;
    public FurnitureConfig cfg;
    public int level = 1;
    public int storeNum;
    public int placedNum;

    public float costNum
    {
        get
        {
            if (UserDataProxy.inst.CheckStorableType(cfg.type_1))
            {
                var count = UserDataProxy.inst.GetFuritureCount(id);

                if (count == 0)
                {
                    return cfg.cost_num;
                }
                else
                {
                    var costCfg = FurnitureBuyCostConfigManager.inst.GetConfig(id, count + 1);
                    return costCfg == null ? cfg.cost_num : costCfg.cost_num;
                    //return cfg.cost_num * Mathf.Pow(cfg.cost_times, placedNum + storeNum);
                }
            }
            else
            {
                return cfg.cost_num;
            }
        }
    }
}

public class CustomizeDisplayData
{
    public int id;
    public FurnitureConfig cfg;
    public bool owned;
}


public class ShopDesignSystem : BaseSystem
{
    ShopDesignUIView mDesignView;
    // FurnitureSelectionUIView mFurnitureSelView;
    CustomizeSelectionUIView mCustomizeSelView;
    FurnitureSkinUI mFurnitureSkinUI;
    FurniturePaperUnlockUI mFurniturePaperUnlockUI;


    //FurnitureSelectionUIView显示相关逻辑
    List<CustomizeDisplayData>[] mCustomizeDataGroup;       //外观
    List<FurnitureDisplayData>[] mFurnitureDataGroup;       //家具
    Dictionary<int, FurnitureConfig> enforceHasOneDic;      //强制保留

    bool isInit = false;

    protected override void AddListeners()
    {
        var e = EventController.inst;
        e.AddListener(GameEventType.SHOWUI_FURNITUREUI, onShowFurnitureSelectionUI);
        e.AddListener<Int16, int>(GameEventType.SHOWUI_TARGETFURN, showTargetFurnitureSelectionUI);
        e.AddListener(GameEventType.HIDEUI_FURNITUREUI, onHideFurnitureSelectionUI);
        e.AddListener(GameEventType.SHOWUI_CUSTOMIZEUI, onShowCustomizeSelectionUI);
        e.AddListener(GameEventType.HIDEUI_CUSTOMIZEUI, onHideCustomizeSelectionUI);
        e.AddListener(GameEventType.ShopDesignEvent.ShowUI_FurnitureSkinUI, onShowFurnitureSkinUI);
        e.AddListener(GameEventType.ShopDesignEvent.ShowUI_FurniturePaperUnLockUI, onShowFurniturePaperUnLockUI);
        //////////////////////////////////////////////////////////////////////////////////////////

        e.AddListener<kFurnitureDisplayType>(GameEventType.ShopDesignEvent.FurnitureSelection_TabSelectd, onFurnitureSelViewTabSelectd);
        e.AddListener<kCustomizeDisplayType>(GameEventType.ShopDesignEvent.CustomizeSelection_TabSelectd, onCustomizeSelViewTabSelectd);
        e.AddListener<int, bool>(GameEventType.ShopDesignEvent.Furniture_Data_Update, onFurnitureDataUpdate);
        e.AddListener(GameEventType.ShopDesignEvent.reSetContentSliderVal, setShopDesignUISlider);
        e.AddListener<int>(GameEventType.ShopDesignEvent.Furniture_Upgrade_EndRefresh, upgradeEndShowItemTitle);
        e.AddListener(GameEventType.ShopDesignEvent.ShowTrunkUpgradeUI, showTrunkUpgradeUI);
    }

    protected override void RemoveListeners()
    {
        var e = EventController.inst;
        e.RemoveListener(GameEventType.SHOWUI_FURNITUREUI, onShowFurnitureSelectionUI);
        e.RemoveListener<Int16, int>(GameEventType.SHOWUI_TARGETFURN, showTargetFurnitureSelectionUI);
        e.RemoveListener(GameEventType.HIDEUI_FURNITUREUI, onHideFurnitureSelectionUI);
        e.RemoveListener(GameEventType.SHOWUI_CUSTOMIZEUI, onShowCustomizeSelectionUI);
        e.RemoveListener(GameEventType.HIDEUI_CUSTOMIZEUI, onHideCustomizeSelectionUI);
        e.RemoveListener(GameEventType.ShopDesignEvent.ShowUI_FurnitureSkinUI, onShowFurnitureSkinUI);
        e.RemoveListener(GameEventType.ShopDesignEvent.ShowUI_FurniturePaperUnLockUI, onShowFurniturePaperUnLockUI);


        e.RemoveListener<kFurnitureDisplayType>(GameEventType.ShopDesignEvent.FurnitureSelection_TabSelectd, onFurnitureSelViewTabSelectd);
        e.RemoveListener<kCustomizeDisplayType>(GameEventType.ShopDesignEvent.CustomizeSelection_TabSelectd, onCustomizeSelViewTabSelectd);
        e.RemoveListener<int, bool>(GameEventType.ShopDesignEvent.Furniture_Data_Update, onFurnitureDataUpdate);
        e.RemoveListener(GameEventType.ShopDesignEvent.reSetContentSliderVal, setShopDesignUISlider);
        e.RemoveListener<int>(GameEventType.ShopDesignEvent.Furniture_Upgrade_EndRefresh, upgradeEndShowItemTitle);
        e.RemoveListener(GameEventType.ShopDesignEvent.ShowTrunkUpgradeUI, showTrunkUpgradeUI);
    }


    void Init()
    {
        mFurnitureDataGroup = new List<FurnitureDisplayData>[(int)kFurnitureDisplayType.None];
        for (int i = 0; i < mFurnitureDataGroup.Length; i++)
        {
            mFurnitureDataGroup[i] = new List<FurnitureDisplayData>();
        }

        mCustomizeDataGroup = new List<CustomizeDisplayData>[(int)kCustomizeDisplayType.None];
        for (int i = 0; i < mCustomizeDataGroup.Length; i++)
        {
            mCustomizeDataGroup[i] = new List<CustomizeDisplayData>();
        }

        enforceHasOneDic = new Dictionary<int, FurnitureConfig>();

    }

    protected override void OnInit()
    {
        base.OnInit();
        isInit = false;
        Init();
    }

    public override void ReInitSystem()
    {
        base.ReInitSystem();
        isInit = false;
        Init();
    }

    void upgradeEndShowItemTitle(int uid)
    {
        //var mDesignView = GUIManager.GetWindow<ShopDesignUIView>();

        //if (mDesignView != null && mDesignView.isShowing)
        //    mDesignView.onPickItem(uid);

        HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_OnPickItem", uid);

    }

    void setShopDesignUISlider()
    {
        //var mDesignView = GUIManager.GetWindow<ShopDesignUIView>();

        //if (mDesignView != null && mDesignView.isShowing)
        //    mDesignView.reSetContentSliderVal();

        HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_ReSetContentSliderVal");

    }

    void onShowFurnitureSelectionUI()
    {
        if (IndoorMapEditSys.inst.shopDesignMode != DesignMode.modeSelection)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
        }
        //GUIManager.OpenView<FurnitureSelectionUIView>();
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_FurnitureSelectionUI");

    }

    void showTargetFurnitureSelectionUI(Int16 type, int furnId)
    {
        if (IndoorMapEditSys.inst.shopDesignMode != DesignMode.modeSelection)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
        }

        //GUIManager.OpenView<FurnitureSelectionUIView>(view =>
        //{
        //    mFurnitureSelView = view;
        //    view.showTargetType((kFurnitureDisplayType)type, furnId);
        //});

        HotfixBridge.inst.TriggerLuaEvent("FurnitureSelectionUI_ShowTargetType", type, furnId);

    }

    private void onFurnitureSelViewTabSelectd(kFurnitureDisplayType dtype)
    {
        //var furnitureSelView = GUIManager.GetWindow<FurnitureSelectionUIView>();

        //if (furnitureSelView != null && furnitureSelView.isShowing)
        //{
        //    furnitureSelView.refreshData(getDisplayGroup(dtype));
        //}

        HotfixBridge.inst.TriggerLuaEvent("FurnitureSelectionUI_RefreshListData", getDisplayGroup(dtype));

    }

    private void onCustomizeSelViewTabSelectd(kCustomizeDisplayType dtype)
    {
        //var customizeSelView = GUIManager.GetWindow<CustomizeSelectionUIView>();

        //if (customizeSelView != null && customizeSelView.isShowing)
        //    customizeSelView.refreshData(getDisplayGroup(dtype));

        HotfixBridge.inst.TriggerLuaEvent("CustomizeSelectionUI_RefreshListData", getDisplayGroup(dtype));

    }
    void onHideFurnitureSelectionUI()
    {
        //GUIManager.HideView<FurnitureSelectionUIView>();
        HotfixBridge.inst.TriggerLuaEvent("HideUI_FurnitureSelectionUI");
    }

    void onShowCustomizeSelectionUI()
    {
        //GUIManager.OpenView<CustomizeSelectionUIView>();
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_CustomizeSelectionUI");

    }

    void onHideCustomizeSelectionUI()
    {
        //GUIManager.HideView<CustomizeSelectionUIView>();
        HotfixBridge.inst.TriggerLuaEvent("HideUI_CustomizeSelectionUI");

    }

    void onShowFurnitureSkinUI()
    {
        GUIManager.OpenView<FurnitureSkinUI>();
    }

    void onShowFurniturePaperUnLockUI()
    {
        GUIManager.OpenView<FurniturePaperUnlockUI>();
    }

    void initFurnitureDislayItems(kFurnitureDisplayType dtype, FurnitureConfig cfg)
    {
        var list = mFurnitureDataGroup[(int)dtype];
        kTileGroupType type = (kTileGroupType)cfg.type_1;

        if (type == kTileGroupType.PetHouse) return; //宠物窝不进

        //bool hasOne = false;
        int level = 0;
        int storeNum = 0;
        int placedNum = 0;
        int uid = -1;
        level = getSelectionData(cfg.type_1, cfg.type_2, cfg.id, out storeNum, out placedNum, out uid);
        switch (type)
        {
            case kTileGroupType.Shelf:
            case kTileGroupType.Trunk:
            case kTileGroupType.ResourceBin:
                var hasFurniture = list.Find(t => t.id == cfg.id);
                if (hasFurniture == null && (cfg.is_show == 1 || level >= 6))
                {
                    hasFurniture = new FurnitureDisplayData() { id = cfg.id, cfg = cfg, level = level, storeNum = storeNum, placedNum = placedNum, uid = uid };
                    list.Add(hasFurniture);
                }

                if (hasFurniture == null) return;

                //if (hasFurniture.level >= 6)
                //{
                //    enforceHasOne(hasFurniture.id, true);
                //}

                if (cfg.is_show == 1 && !isInit)
                {
                    enforceHasOneDic.Add(cfg.type_1 * 100 + cfg.type_2, cfg);
                }
                break;
            //门内外家具
            default:
                list.Add(new FurnitureDisplayData() { cfg = cfg, id = cfg.id, storeNum = storeNum, placedNum = placedNum, uid = uid });
                break;
        }
    }

    public void initDisplayItems()
    {

        foreach (var cfg in FurnitureConfigManager.inst.getList())
        {
            var dtype = getDisplayType(cfg.type_1);
            var ctype = getCustomizeType(cfg.type_1);
            if (dtype != kFurnitureDisplayType.None)
            {
                initFurnitureDislayItems(dtype, cfg);
            }
            else if (ctype != kCustomizeDisplayType.None && (cfg.is_show == 1))
            {
                var clist = mCustomizeDataGroup[(int)ctype];
                clist.Add(new CustomizeDisplayData()
                {
                    id = cfg.id,
                    cfg = cfg,
                    owned = false
                });
            }
        }

    }

    public List<FurnitureDisplayData> getDisplayGroup(kFurnitureDisplayType displayType)
    {
        if (!isInit)
        {
            initDisplayItems();
            isInit = true;
        }



        ////礼包类型的未解锁不展示
        List<FurnitureDisplayData> furnitureDisplayDatas = mFurnitureDataGroup[(int)displayType].FindAll(t =>
        {
            if (t.cfg.cost_type != 4)
            {
                return true;
            }
            else
            {

                if (UserDataProxy.inst.GetFuritureCount(t.id) > 0)
                {
                    return true;
                }
                else
                {

                    if (HotfixBridge.inst.GetDirectPurchaseDataById(t.cfg.cost_num, out DirectPurchaseData directPurchaseData))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        });

        furnitureDisplayDatas = furnitureDisplayDatas.FindAll(item => item.cfg.show_lv <= UserDataProxy.inst.playerData.level || item.storeNum > 0); //或者持有此家具 且存储数量不为0 UserDataProxy.inst.GetFuritureCount(item.id) > 0

        ////排序
        furnitureDisplayDatas.Sort((a, b) => { return sortByOrder(a, b); });

        return furnitureDisplayDatas;
    }


    public List<CustomizeDisplayData> getDisplayGroup(kCustomizeDisplayType displayType)
    {
        if (!isInit)
        {
            initDisplayItems();
            isInit = true;
        }


        ////礼包类型的未解锁不展示
        List<CustomizeDisplayData> furnitureDisplayDatas = mCustomizeDataGroup[(int)displayType].FindAll(t =>
        {
            if (t.cfg.cost_type != 4)
            {
                return true;
            }
            else
            {
                bool hase = t.cfg.type_1 == 1 ? UserDataProxy.inst.shopData.hasWall(t.cfg.id) : UserDataProxy.inst.shopData.hasFloor(t.cfg.id);

                if (hase)
                {
                    return true;
                }
                else
                {
                    if (HotfixBridge.inst.GetDirectPurchaseDataById(t.cfg.cost_num, out DirectPurchaseData directPurchaseData))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        });

        furnitureDisplayDatas = furnitureDisplayDatas.FindAll(item => item.cfg.show_lv <= UserDataProxy.inst.playerData.level || (item.cfg.type_1 == 1 ? UserDataProxy.inst.shopData.hasWall(item.cfg.id) : UserDataProxy.inst.shopData.hasFloor(item.cfg.id)));
        ////排序
        furnitureDisplayDatas.Sort((a, b) => { return sortByOrder(a, b); });

        return furnitureDisplayDatas;
    }

    int getOrderByUnLockByCostType(int cost_type)
    {
        ///3 4 1 2 特权 礼包 新币 钻石  根据类型重排优先级

        if (cost_type == 3)
        {
            cost_type = 1;
        }
        else if (cost_type == 4)
        {
            cost_type = 2;
        }
        else if (cost_type == 1)
        {
            cost_type = 3;
        }
        else if (cost_type == 2)
        {
            cost_type = 4;
        }

        return cost_type;
    }


    int sortByOrder(CustomizeDisplayData a, CustomizeDisplayData b)
    {
        uint playerLv = UserDataProxy.inst.playerData.level;

        bool a_unLock = false;
        bool b_unLock = false;

        a_unLock = a.cfg.unlock_lv <= playerLv;
        b_unLock = b.cfg.unlock_lv <= playerLv;


        if (getDisplayType(a.cfg.type_1) == kFurnitureDisplayType.ResourceBin)
        {

            ResourceBinUpgradeConfig resUpCfg = null;

            if (a_unLock)
            {
                resUpCfg = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(a.cfg.type_2, 1);
                a_unLock = !(resUpCfg != null && UserDataProxy.inst.GetBuildingData(resUpCfg.build_id) != null && (UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).state == (int)EBuildState.EB_Lock || UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).level < resUpCfg.build_level)); //资源篮 并且未解锁
            }

            if (b_unLock)
            {
                resUpCfg = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(b.cfg.type_2, 1);
                b_unLock = !(resUpCfg != null && UserDataProxy.inst.GetBuildingData(resUpCfg.build_id) != null && (UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).state == (int)EBuildState.EB_Lock || UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).level < resUpCfg.build_level)); //资源篮 并且未解锁
            }
        }

        if (a.cfg.cost_type == 3)
        {
            if (a_unLock)
            {
                if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                {
                    a_unLock = false;
                }
                else
                {
                    a_unLock = UserDataProxy.inst.playerData.vipLevel >= a.cfg.cost_num;
                }
            }
        }

        if (b.cfg.cost_type == 3)
        {
            if (b_unLock)
            {
                if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                {
                    b_unLock = false;
                }
                else
                {
                    b_unLock = UserDataProxy.inst.playerData.vipLevel >= b.cfg.cost_num;
                }
            }
        }

        if (a_unLock && !b_unLock)
        {
            return b.cfg.cost_type == 3 ? 1 : -1;
        }
        else if (!a_unLock && b_unLock)
        {
            return a.cfg.cost_type == 3 ? -1 : 1;
        }
        else if (!a_unLock && !b_unLock)
        {

            if (a.cfg.cost_type == 3 && b.cfg.cost_type != 3)
            {
                return -1;
            }
            else if (a.cfg.cost_type != 3 && b.cfg.cost_type == 3)
            {
                return 1;
            }
            else
            {
                return a.cfg.unlock_lv.CompareTo(b.cfg.unlock_lv);
            }
        }
        else
        {

            int a_order = getOrderByUnLockByCostType(a.cfg.cost_type);
            int b_order = getOrderByUnLockByCostType(b.cfg.cost_type);

            if (a_order == b_order)
            {
                if (a.cfg.cost_type == 1)//新币
                {
                    return a.cfg.cost_num.CompareTo(b.cfg.cost_num);
                }
                else if (a.cfg.cost_type == 2)//金条
                {
                    return a.cfg.cost_num.CompareTo(b.cfg.cost_num);
                }
                else if (a.cfg.cost_type == 3)//特权
                {
                    return a.id.CompareTo(b.id);
                }
                else if (a.cfg.cost_type == 4) //礼包
                {
                    return a.id.CompareTo(b.id);
                }
            }

            return a_order.CompareTo(b_order);
        }

    }

    int sortByOrder(FurnitureDisplayData a, FurnitureDisplayData b)
    {
        uint playerLv = UserDataProxy.inst.playerData.level;

        bool a_unLock = false;
        bool b_unLock = false;

        a_unLock = a.cfg.unlock_lv <= playerLv;
        b_unLock = b.cfg.unlock_lv <= playerLv;


        if (getDisplayType(a.cfg.type_1) == kFurnitureDisplayType.ResourceBin)
        {

            ResourceBinUpgradeConfig resUpCfg = null;

            if (a_unLock)
            {
                resUpCfg = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(a.cfg.type_2, 1);
                a_unLock = !(resUpCfg != null && UserDataProxy.inst.GetBuildingData(resUpCfg.build_id) != null && (UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).state == (int)EBuildState.EB_Lock || UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).level < resUpCfg.build_level)); //资源篮 并且未解锁
            }

            if (b_unLock)
            {
                resUpCfg = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(b.cfg.type_2, 1);
                b_unLock = !(resUpCfg != null && UserDataProxy.inst.GetBuildingData(resUpCfg.build_id) != null && (UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).state == (int)EBuildState.EB_Lock || UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).level < resUpCfg.build_level)); //资源篮 并且未解锁
            }
        }

        if (a.cfg.cost_type == 3)
        {
            if (a_unLock)
            {
                if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                {
                    a_unLock = false;
                }
                else
                {
                    a_unLock = UserDataProxy.inst.playerData.vipLevel >= a.cfg.cost_num;
                }
            }
        }

        if (b.cfg.cost_type == 3)
        {
            if (b_unLock)
            {
                if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                {
                    b_unLock = false;
                }
                else
                {
                    b_unLock = UserDataProxy.inst.playerData.vipLevel >= b.cfg.cost_num;
                }
            }
        }

        if (a_unLock && !b_unLock)
        {
            return b.cfg.cost_type == 3 ? 1 : -1;
        }
        else if (!a_unLock && b_unLock)
        {
            return a.cfg.cost_type == 3 ? -1 : 1;
        }
        else if (!a_unLock && !b_unLock)
        {

            if (a.cfg.cost_type == 3 && b.cfg.cost_type != 3)
            {
                return -1;
            }
            else if (a.cfg.cost_type != 3 && b.cfg.cost_type == 3)
            {
                return 1;
            }
            else
            {
                return a.cfg.unlock_lv.CompareTo(b.cfg.unlock_lv);
            }
        }
        else
        {

            int a_order = getOrderByUnLockByCostType(a.cfg.cost_type);
            int b_order = getOrderByUnLockByCostType(b.cfg.cost_type);

            if (a_order == b_order)
            {
                if (a.cfg.cost_type == 1)//新币
                {
                    return a.costNum.CompareTo(b.costNum);
                }
                else if (a.cfg.cost_type == 2)//金条
                {
                    return a.costNum.CompareTo(b.costNum);
                }
                else if (a.cfg.cost_type == 3)//特权
                {
                    return a.id.CompareTo(b.id);
                }
                else if (a.cfg.cost_type == 4) //礼包
                {
                    return a.id.CompareTo(b.id);
                }
            }

            return a_order.CompareTo(b_order);
        }

    }

    kFurnitureDisplayType getDisplayType(int firstType)
    {
        kTileGroupType type = (kTileGroupType)firstType;
        switch (type)
        {
            case kTileGroupType.Carpet:
                return kFurnitureDisplayType.Carpet;
            case kTileGroupType.WallFurniture:
                return kFurnitureDisplayType.Furniture;
            case kTileGroupType.Furniture:
                return kFurnitureDisplayType.Furniture;
            case kTileGroupType.Shelf:
                return kFurnitureDisplayType.ShelfAndTrunk;
            case kTileGroupType.Trunk:
                return kFurnitureDisplayType.ShelfAndTrunk;
            case kTileGroupType.ResourceBin:
                return kFurnitureDisplayType.ResourceBin;
            case kTileGroupType.OutdoorFurniture:
                return kFurnitureDisplayType.OutdoorFurniture;
        }
        return kFurnitureDisplayType.None;
    }

    kCustomizeDisplayType getCustomizeType(int type1)
    {
        kTileGroupType type = (kTileGroupType)type1;
        switch (type)
        {
            case kTileGroupType.Wall:
                return kCustomizeDisplayType.Wall;
            case kTileGroupType.Floor:
                return kCustomizeDisplayType.Floor;
        }
        return kCustomizeDisplayType.None;
    }

    public int getSelectionData(int type1, int type2, int furnitureId, out int storeNum, out int placedNum, out int uid)
    {
        int level = 1;
        uid = -1;
        storeNum = 0;
        placedNum = 0;

        var tp = (kTileGroupType)type1;

        kFurnitureDisplayType displayType = getDisplayType(type1);

        List<IndoorData.ShopDesignItem> furnitures = UserDataProxy.inst.GetEntitys(tp);

        List<IndoorData.ShopDesignItem> subList = furnitures == null ? new List<IndoorData.ShopDesignItem>() : (displayType == kFurnitureDisplayType.ShelfAndTrunk || displayType == kFurnitureDisplayType.ResourceBin) ? furnitures.FindAll(t => t != null && t.config.type_2 == type2) : furnitures.FindAll(t => t != null && t.config.id == furnitureId);

        foreach (var item in subList)
        {
            if (item.state == (int)EDesignState.InStore && item.id == furnitureId)
            {
                storeNum += 1;

                if (level <= item.level)
                {
                    level = item.level;
                    uid = item.uid;
                }
            }
        }

        placedNum = subList.Count - storeNum;

        return level;
    }

    public void showTrunkUpgradeUI() //找一个场景中级别比较低的仓库打开升级界面
    {
        List<IndoorData.ShopDesignItem> trunklist = UserDataProxy.inst.shopData.storageBoxList;
        if (trunklist.Count > 0)
        {
            IndoorData.ShopDesignItem lv = trunklist[0];
            foreach (var trunk in trunklist)
            {
                if (trunk.level < lv.level)
                {
                    lv = trunk;
                }
            }
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_UPGRADEPANEL, lv);
        }
    }

    /// <summary>
    /// 强制保留
    /// </summary>
    /// <param name="id">家具ID</param>
    /// <param name="addOrDel">强制添加或删除</param>
    void enforceHasOne(int id, bool addOrDel)
    {
        var cfg = FurnitureConfigManager.inst.getConfig(id);
        var dtype = getDisplayType(cfg.type_1);
        var dlist = mFurnitureDataGroup[(int)dtype];

        if (!enforceHasOneDic.ContainsKey(cfg.type_1 * 100 + cfg.type_2)) return;

        var enforceCfg = enforceHasOneDic[cfg.type_1 * 100 + cfg.type_2];

        FurnitureDisplayData data = dlist.Find(t => t.cfg == cfg);
        FurnitureDisplayData enforceData = dlist.Find(t => t.cfg == enforceCfg);

        if (addOrDel)
        {
            if (data != null && (enforceData == null || enforceData.storeNum == 0))
            {
                dlist.Remove(enforceData);
            }
        }
        else
        {
            if (data != null && enforceData == null)
            {
                int uid = -1;
                int storeNum = 0;
                int placedNum = 0;

                getSelectionData(enforceCfg.type_1, enforceCfg.type_2, cfg.id, out storeNum, out placedNum, out uid);
                dlist.Add(new FurnitureDisplayData() { id = enforceCfg.id, cfg = enforceCfg, level = 1, storeNum = storeNum, placedNum = placedNum });
            }
        }



        dlist.Sort((a, b) => { return sortByOrder(a, b); });
    }

    private void onFurnitureDataUpdate(int changeFurnitureUid, bool change)
    {
        IndoorData.ShopDesignItem item = UserDataProxy.inst.GetFuriture(changeFurnitureUid);


        if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.currEntityUid == changeFurnitureUid)
        {
            upgradeEndShowItemTitle(changeFurnitureUid);
        }


        var dtype = getDisplayType(item.config.type_1);
        if (dtype == kFurnitureDisplayType.None) return;
        var list = mFurnitureDataGroup[(int)dtype];

        int level = 0;

        if (checkNeedForAbleType(item.config.type_1))
        {
            foreach (var cfg in FurnitureConfigManager.inst.getList())
            {
                if (cfg.type_1 * 100 + cfg.type_2 == item.config.type_1 * 100 + item.config.type_2)
                {

                    level = getSelectionData(cfg.type_1, cfg.type_2, cfg.id, out int storeNum, out int placedNum, out int uid);

                    var hasFurniture = list.Find(t => t.id == cfg.id);

                    if (hasFurniture == null && storeNum != 0)
                    {
                        hasFurniture = new FurnitureDisplayData() { id = cfg.id, cfg = cfg, level = level, storeNum = storeNum, placedNum = placedNum, uid = uid };
                        list.Add(hasFurniture);
                    }
                    else if (hasFurniture != null)
                    {
                        //存储数量为0
                        if (storeNum == 0 && cfg.is_show == 0)
                        {
                            //enforceHasOne(cfg.id, false);
                            list.Remove(hasFurniture);

                        }
                        else
                        {
                            hasFurniture.level = level;
                            hasFurniture.storeNum = storeNum;
                            hasFurniture.placedNum = placedNum;
                            hasFurniture.uid = uid;

                            //是否在出现高阶架子时隐藏掉无贮藏数量的初级架子
                            //if (level >= 6) enforceHasOne(cfg.id, true);
                        }

                    }
                }
            }
        }
        else
        {
            var furniture = list.Find(t => t.id == item.id);
            var cfg = item.config;

            level = getSelectionData(cfg.type_1, cfg.type_2, cfg.id, out int storeNum, out int placedNum, out int uid);

            if (furniture != null)
            {
                furniture.level = level;
                furniture.storeNum = storeNum;
                furniture.placedNum = placedNum;
                furniture.uid = uid;
            }
        }

        list.Sort((a, b) => { return sortByOrder(a, b); });
    }

    bool checkNeedForAbleType(int type1)
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

}
