using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMakerSlot
{
    public int slotId;
    public bool isAdvanced;  // 是否是高级的
    public int level;
    public bool working = false;    //工作中
    public int equipDrawingId;            //当前制造的装备图纸id
    public double totalTime = 0;      //制造总用时
    public double currTime = 0;       //当前用时
    public double endTime = 0; //结束时间
    public double countDown = 0;
    public int makeState = 0; //空闲 0 制作中 1  完成 2

    public System.Action<int, float> coolTimeChange;

    private Tween timerTween;    //计时器tween
    private double timer = 0; //内置计时器 

    int timerId = 0;
    public EquipMakerSlot(int _slotId, int _level = 1)
    {
        slotId = _slotId;
        level = _level;
        ReInit();
        // isAdvanced = _addvanced;
    }
    public void ReInit()
    {
        CancelWork();
        equipDrawingId = 0;
        totalTime = 0;
        currTime = 0;
        makeState = 0;
    }
    public void UpLevel()
    {
        isAdvanced = true;
    }

    public void StartWork(int _equipDrawingId, double _countdown, double _totalTime, int state)
    {
        CancelWork();
        this.equipDrawingId = _equipDrawingId;
        if (makeState == 1 && state == 2)
        {
            if (ManagerBinder.inst.mGameState == kGameState.Shop)
            {
                AudioManager.inst.PlaySound(40);
            }
        }
        makeState = state;
        currTime = _countdown;
        totalTime = _totalTime;
        endTime = currTime + GameTimer.inst.serverNow;

        if (makeState == 1)
        {
            // timerTween = DoTweenUtil.Timer(1f, -1, () => update());
            timer = GameTimer.inst.serverNow;
            timerId = GameTimer.inst.AddTimer(1f, update);
        }
        else
        {
            CancelWork();
        }
        //刷新UI
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.PRODUCTIONLIST_STATE_Change);
    }

    private void update()
    {
        if (currTime > 0)
        {
            currTime -= GameTimer.inst.serverNow - timer;
            timer = GameTimer.inst.serverNow;
            // if (timer >= 1)
            // timer = 0;

            int curtime = (int)currTime;
            if (curtime <= 0)
            {
                //CancelWork();
                curtime = 0;
                Logger.log("id为" + slotId + "的槽位已就绪      装备" + equipDrawingId.ToString() + "制造完成");
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_UPDATE, slotId);
            }
            else
            {
                //刷新 状态
            }

            if (coolTimeChange != null)
            {
                coolTimeChange.Invoke(equipDrawingId, (float)currTime);
            }
        }
        else
        {
            CancelWork();
            if (coolTimeChange != null)
            {
                coolTimeChange.Invoke(equipDrawingId, (float)currTime);
            }
        }
    }
    public void EndMake()
    {
        ReInit();
    }

    //取消
    public void CancelWork()
    {
        // if (timerTween != null)
        // {
        //     timerTween.Kill(true);
        // }
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
        }
    }


}
public class EquipSystem : BaseSystem
{
    EquipListUIView equipListUIView;
    EquipInfoUIView equipInfoUIView;
    LackResUIView lackResUIView;    //资源不足
    RequiredUIView requiredUI;      //没有装备库存
    ResourceBuyProductionUI _resourceBuyProductionUI; //购买资源

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_EQUIPLIST, openEquipListUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_EQUIPLIST, hideEquipListUI);

        EventController.inst.AddListener<int>(GameEventType.EquipEvent.EQUIP_PRODUCTION_SELECT, SelectEquip);  //点击了生产槽位

        EventController.inst.AddListener(GameEventType.EquipEvent.EQUIP_UPDATEINFO, getEquipInfo);

        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_MAKED, Equipmaked);
        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, EquipMakeStart);
        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_UPDATE, EquipSlotUpdate);
        EventController.inst.AddListener(GameEventType.ProductionEvent.UIRefresh_CheckMakeEquipRes, _checkMakeEquipRes);

        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.RES_PRODUCTIONLIST_REFRESHUI, RefreshResProductionBar);

        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.UIHandle_BuyMakeSlot, BuyMakingSlot);

        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.UIHandle_SHOW_MAKINGSLOTINFO, ShowMakingSlotInfo);

        EventController.inst.AddListener<int, int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_Faster, EquipMakeFaster);

        EventController.inst.AddListener<int>(GameEventType.BagEvent.BAG_RES_UPDATE, OnResItemChange);  //资源道具 数量发生改变

        EventController.inst.AddListener<int, List<EquipData>>(GameEventType.SHOWUI_EQUIPINFOUI, showEquipDrawingsInfoUI);
        EventController.inst.AddListener<int>(GameEventType.SHOWUI_EQUIPINFOUIBYDRAWINGID, showEquipInfoByEquipDrawingId);
        EventController.inst.AddListener(GameEventType.HIDEUI_EQUIPINFOUI, hideEquipDrawingsInfoUI);
        EventController.inst.AddListener<int, bool>(GameEventType.EquipEvent.EQUIP_FAVORITE, EquipFavorite);
        EventController.inst.AddListener<int, int, int, bool>(GameEventType.EquipEvent.EQUIP_Required, OpenRequiredUI);

        EventController.inst.AddListener<int, bool>(GameEventType.EquipEvent.EQUIP_SHOWMAKELIST, ShowMakeEquipList);
        EventController.inst.AddListener<int>(GameEventType.EquipEvent.EQUIP_SHOWTARGETTYPE, showTargetEquipType);
        EventController.inst.AddListener(GameEventType.EquipEvent.EQUIP_SHOWREFRESH, RefreshEquipListUI);
        EventController.inst.AddListener(GameEventType.RefreshUI_EquipInfoUIStarUp, refreshEquipInfoUIStarUp);

        EventController.inst.AddListener<int>(GameEventType.BagEvent.ShopDesign_Resource_BuyProduction, showResourceBuyProductionUI);


        EventController.inst.AddListener<int>(GameEventType.EquipEvent.EQUIP_STARUP, request_equipStarUP);

        EventController.inst.AddListener(GameEventType.ProductionEvent.SyncUpdateProductionData, SyncEquipMakerData);


        ///******************************************
        NetworkEvent.SetCallback(MsgType.Response_Equip_Data_Cmd,
        (successResp) =>
        {
            OnEquipDataResp((Response_Equip_Data)successResp);
        },
        (failedResp) =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("更新装备数据失败!"));
        });

        Helper.AddNetworkRespListener(MsgType.Response_Equip_MakeStart_Cmd, EquipMakeStartResp);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_MakeEnd_Cmd, EquipMakeEndResp);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_MakeRefresh_Cmd, EquipMakeRefreshResp);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_EquipInfoChange_Cmd, EquipInfoChange);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_BuySlot_Cmd, EquipBuySlotResp);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_MakeFaster_Cmd, EquipMakeFasterResp);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_MakingList_Cmd, OnEquipMakingListUpdate);
        Helper.AddNetworkRespListener(MsgType.Response_Hero_UseEquipItem_Cmd, equipStarUpResp);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_EQUIPLIST, openEquipListUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_EQUIPLIST, hideEquipListUI);

        EventController.inst.RemoveListener(GameEventType.EquipEvent.EQUIP_UPDATEINFO, getEquipInfo);
        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_MAKED, Equipmaked);
        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, EquipMakeStart);
        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_UPDATE, EquipSlotUpdate);
        EventController.inst.RemoveListener(GameEventType.ProductionEvent.UIRefresh_CheckMakeEquipRes, _checkMakeEquipRes);


        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.RES_PRODUCTIONLIST_REFRESHUI, RefreshResProductionBar);
        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.UIHandle_BuyMakeSlot, BuyMakingSlot);
        EventController.inst.RemoveListener<int>(GameEventType.EquipEvent.EQUIP_PRODUCTION_SELECT, SelectEquip);

        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.UIHandle_SHOW_MAKINGSLOTINFO, ShowMakingSlotInfo);
        EventController.inst.RemoveListener<int, int>(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_Faster, EquipMakeFaster);
        EventController.inst.RemoveListener<int>(GameEventType.BagEvent.BAG_RES_UPDATE, OnResItemChange);  //资源道具 数量发生改变
        EventController.inst.RemoveListener<int, List<EquipData>>(GameEventType.SHOWUI_EQUIPINFOUI, showEquipDrawingsInfoUI);
        EventController.inst.RemoveListener<int>(GameEventType.SHOWUI_EQUIPINFOUIBYDRAWINGID, showEquipInfoByEquipDrawingId);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_EQUIPINFOUI, hideEquipDrawingsInfoUI);
        EventController.inst.RemoveListener<int, bool>(GameEventType.EquipEvent.EQUIP_FAVORITE, EquipFavorite);

        EventController.inst.RemoveListener<int, int, int, bool>(GameEventType.EquipEvent.EQUIP_Required, OpenRequiredUI);

        EventController.inst.RemoveListener<int, bool>(GameEventType.EquipEvent.EQUIP_SHOWMAKELIST, ShowMakeEquipList);
        EventController.inst.RemoveListener<int>(GameEventType.EquipEvent.EQUIP_SHOWTARGETTYPE, showTargetEquipType);
        EventController.inst.RemoveListener(GameEventType.EquipEvent.EQUIP_SHOWREFRESH, RefreshEquipListUI);
        EventController.inst.RemoveListener(GameEventType.RefreshUI_EquipInfoUIStarUp, refreshEquipInfoUIStarUp);
        EventController.inst.RemoveListener<int>(GameEventType.EquipEvent.EQUIP_STARUP, request_equipStarUP);
        EventController.inst.RemoveListener<int>(GameEventType.BagEvent.ShopDesign_Resource_BuyProduction, showResourceBuyProductionUI);

        EventController.inst.RemoveListener(GameEventType.ProductionEvent.SyncUpdateProductionData, SyncEquipMakerData);
    }

    private void showTargetEquipType(int equipdrawingId)
    {
        var view = GUIManager.GetWindow<EquipListUIView>();

        if (view != null && view.isShowing)
        {
            bool typetablevisble = true;
            var cfg = WorldParConfigManager.inst.GetConfig(163);
            var value_1 = cfg == null ? 0 : (int)cfg.parameters;
            if (UserDataProxy.inst.playerData.level < value_1)
            {
                cfg = WorldParConfigManager.inst.GetConfig(162);
                var value_2 = cfg == null ? 0 : (int)cfg.parameters;
                if (EquipDataProxy.inst.GetEquipDatas().Count < value_2)
                {
                    typetablevisble = false;
                }
            }

            EquipDrawingsConfig equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingId);
            if (typetablevisble)
            {
                view.setShowListSubtype(equipCfg.sub_type);
            }
            else
            {
                view.setMainToggle();
            }

            view.curHighLightEqiupDrawingId = equipdrawingId;
            view.SetUIInfo();
        }

        //EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingId);
        //equipListUIView.setShowListSubtype(cfg.sub_type);

        //var equipView = GUIManager.GetWindow<EquipListUIView>();
        //if (equipView != null && equipView.isShowing)
        //{
        //    equipView.showInfo();
        //}
    }

    private void ShowMakeEquipList(int equipdrawingId, bool needJumpType = true)
    {
        GUIManager.OpenView<EquipListUIView>((view) =>
        {
            EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingId);
            if (needJumpType)
                view.setShowListSubtype(cfg.sub_type);
            else
                view.setMainToggle();
            view.curHighLightEqiupDrawingId = equipdrawingId;
            view.SetUIInfo();
        });
    }
    private void OnResItemChange(int equipId)
    {
        equipListUIView = GUIManager.GetWindow<EquipListUIView>();
        if (equipListUIView != null && equipListUIView.isShowing)
        {
            equipListUIView.updateResListUI(equipId);
        }
    }

    private void RefreshEquipListUI()
    {
        equipListUIView = GUIManager.GetWindow<EquipListUIView>();
        if (equipListUIView != null && equipListUIView.isShowing)
        {
            equipListUIView.UpdateUIInfo();
        }
    }

    private void OpenRequiredUI(int equipdrawingId, int quality, int count, bool allQuality)
    {
        GUIManager.OpenView<RequiredUIView>(view =>
        {
            view.SetInfo(equipdrawingId, quality, count, allQuality);
        });
    }
    #region 装备制造*******************************************************************************
    private int currSlotHandler = -1;
    private void SelectEquip(int _soltId)
    {
        currSlotHandler = _soltId;
        //打开装备选择界面
        openEquipListUI();
    }

    EquipMakingView equipMakingView;
    private void ShowMakingSlotInfo(int slotid)
    {
        EquipMakerSlot _slot = EquipDataProxy.inst.GetMakeSlot(slotid);
        AudioManager.inst.PlaySound(38);

        equipMakingView = GUIManager.OpenView<EquipMakingView>((view) =>
        {
            view.showInfo(_slot);
        });
    }

    #endregion ****************************************************************************************
    private void openEquipListUI()
    {
        GUIManager.OpenView<EquipListUIView>((view) =>
        {
            equipListUIView = view;
            view.SetUIInfo();
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.Reset_TargetEquipType);
        });
    }

    private void hideEquipListUI()
    {
        GUIManager.HideView<EquipListUIView>();
    }


    #region 缺少要求

    List<needMaterialsInfo> lackResItems;
    int _lackResEquipDrawingId;

    public void showHelpLink(itemConfig item)
    {

    }
    //制作资源
    private void showLackResUIView()
    {
        //判断是否有资源材料
        foreach (var item in lackResItems)
        {
            if (item.type == 0)
            {
                //判断资源工厂是否解锁
                helpConfig hcfg_1 = GameHelpNavigationConfigManager.inst.GetHelpConfigBytyp(1, item.needId);
                if (hcfg_1 != null)
                {
                    //判断是否已解锁
                    CityBuildingData buildingData = UserDataProxy.inst.GetBuildingData(hcfg_1.triggered_val_2[0]);
                    if (buildingData != null)
                    {
                        if (buildingData.state == 0)
                        {
                            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", hcfg_1.id);
                            return;
                        }
                    }
                }

                itemConfig icfg = ItemconfigManager.inst.GetConfig(item.needId);
                helpConfig hcfg = GameHelpNavigationConfigManager.inst.GetHelpConfigBytyp(2, item.needId);
                //判断商店内是否有资源篮子
                if (hcfg != null)
                {
                    bool haseF = false;
                    foreach (int fid in hcfg.triggered_val_2)
                    {
                        if (UserDataProxy.inst.shopData.hasFurnitureInShop(fid))
                        {
                            haseF = true;
                            break;
                        }

                    }
                    if (!haseF)
                    {
                        HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", hcfg.id);
                        return;
                    }
                    else
                    {
                        checkLackResUI();
                    }
                }
                else
                {
                    checkLackResUI();
                }
            }
            else
            {
                checkLackResUI();
            }
        }

    }

    private void checkLackResUI()
    {
        lackResUIView = GUIManager.GetWindow<LackResUIView>();
        if (lackResUIView == null)
        {
            if (!EquipDataProxy.inst.lackResIsShowing)
            {
                lackResUIView = GUIManager.OpenView<LackResUIView>(view =>
                {
                    view.setData(lackResItems);
                });
            }
            EquipDataProxy.inst.lackResIsShowing = true;
        }
        else
        {
            if (lackResUIView.isShowing)
            {
                lackResUIView.setData(lackResItems);
            }
            else
            {
                if (!EquipDataProxy.inst.lackResIsShowing)
                {
                    lackResUIView = GUIManager.OpenView<LackResUIView>(view =>
                    {
                        view.setData(lackResItems);
                    });
                }
                EquipDataProxy.inst.lackResIsShowing = true;
            }
        }
    }

    //检查制作资源

    private void _checkMakeEquipRes()
    {
        bool result = CheckMakeEquipRes(_lackResEquipDrawingId);
        if (result)
        {
            GUIManager.HideView<LackResUIView>();
        }
    }

    private bool CheckMakeEquipRes(int _equipDrawingid)
    {
        //最优先检查工匠

        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(_equipDrawingid);
        if (cfg != null)
        {


            int workerId = Array.Find<int>(cfg.artisan_id, t => RoleDataProxy.inst.GetWorker(t).state != EWorkerState.Unlock);
            if (workerId > 0) //说明有未解锁的
            {
                WorkerConfig workerConfig = WorkerConfigManager.inst.GetConfig(workerId);
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("需要招募工匠{0}", LanguageManager.inst.GetValueByKey(workerConfig.name)), GUIHelper.GetColorByColorHex("FF2828"));

                return false;
            }
            else //工匠等级不足
            {

                workerId = -1;
                int workerLv = -1;
                //工匠
                for (int i = 0; i < cfg.artisan_id.Length; i++)
                {
                    int id = cfg.artisan_id[i];
                    WorkerData workerData = RoleDataProxy.inst.GetWorker(id);
                    if (workerData != null && workerData.state == EWorkerState.Unlock)
                    {
                        if (workerData.level < cfg.artisan_lv[i])
                        {
                            workerId = id;
                            workerLv = cfg.artisan_lv[i];
                            break;
                        }
                    }
                }


                if (workerId != -1)
                {
                    WorkerConfig workerConfig = WorkerConfigManager.inst.GetConfig(workerId);
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("需要工匠{0}达到等级{1}", LanguageManager.inst.GetValueByKey(workerConfig.name), workerLv.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return false;
                }

            }

        }


        lackResItems = new List<needMaterialsInfo>();
        _lackResEquipDrawingId = _equipDrawingid;

        EquipData data = EquipDataProxy.inst.GetEquipData(_equipDrawingid);
        if (data == null)
        {
            return false;
        }

        bool result = true;

        for (int i = 0; i < data.needRes.Length; i++)
        {
            if (!checkRes(data.needRes[i]))
            {
                result = false;
            }
        }
        //先检查特殊材料
        if (data.specialRes_1.needId > 0)
        {
            if (!checkRes(data.specialRes_1))
            {
                result = false;
            }
        }

        if (data.specialRes_2.needId > 0)
        {
            if (!checkRes(data.specialRes_2))
            {
                result = false;
            }
        }
        if (!result)//如果有需要补充的资源
        {
            AudioManager.inst.PlaySound(39);
            showLackResUIView();
        }

        return result;
    }
    //检测 并打开资源不足界面
    private bool checkRes(needMaterialsInfo info)
    {
        bool result = true;

        if (info.type == 0 || info.type == 2)
        {
            int count = (int)ItemBagProxy.inst.resItemCount(info.needId);
            if (count < info.needCount)
            {
                lackResItems.Add(info);
                result = false;
            }
        }
        else if (info.type == 1)
        {
            // EquipQualityConfig eqCfg = EquipConfigManager.inst.GetEquipQualityConfig(info.needId);
            int count = ItemBagProxy.inst.getEquipNumberBySuperQuip(info.needId);
            if (count < info.needCount)
            {
                lackResItems.Add(info);
                result = false;
            }
        }

        return result;
    }

    private void showResourceBuyProductionUI(int itemID)
    {
        _resourceBuyProductionUI = GUIManager.OpenView<ResourceBuyProductionUI>(view =>
        {
            view.setItem(itemID);
        });
    }

    #endregion


    private void checkMakeSlotDirectPurchase(int needGemNum)
    {
        HotfixBridge.inst.TriggerLuaEvent("CSCallLua_CheckMakeSlotDirectPurchase", EquipDataProxy.inst.mskeSlotCount + 1, needGemNum);
    }

    //购买制造槽位
    //type = 0 金币购买 type = 1 钻石购买
    private void BuyMakingSlot(int type)
    {
        var cfg = FieldConfigManager.inst.GetFieldConfig(3, EquipDataProxy.inst.mskeSlotCount + 1);
        if (cfg != null)
        {
            if (type == 0)
            {
                if (UserDataProxy.inst.playerData.gold < cfg.money)
                {
                    //金币不足
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
                if (UserDataProxy.inst.playerData.level < cfg.level)
                {
                    //等级不足
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
            }
            else
            {
                if (UserDataProxy.inst.playerData.gem < cfg.diamond)
                {
                    //钻石不足
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("FF2828"));

                    //礼包判定 拥有礼包打开
                    checkMakeSlotDirectPurchase(cfg.diamond);
                    return;
                }
            }
        }
        else
        {
            return;
        }
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_BuySlot()
            {
                useGem = type
            }
        });
    }


    //刷新 装配统计数据
    private void EquipInfoChange(HttpMsgRspdBase msg)
    {
        Response_Equip_EquipInfoChange data = (Response_Equip_EquipInfoChange)msg;
        EquipData equipinfo = EquipDataProxy.inst.GetEquipData(data.equipInfo.equipDrawingId);
        bool isnew = false;
        if (equipinfo != null)
        {
            //判断是否是图纸升级
            if (data.equipInfo.progressLevel > equipinfo.progressLevel)
            {
                if (equipinfo.progresInfoList[data.equipInfo.progressLevel - 1].type != 7)
                {
                    EquipDataProxy.inst.updateEquipInfo(data.equipInfo, false);
                    //图纸升级
                    Logger.log("图纸升级" + data.equipInfo.equipDrawingId);
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.DrawingUpLv, "", data.equipInfo.equipDrawingId, 0, 1));
                    return;
                }
            }
        }
        else
        {
            Logger.log("新解锁图纸" + data.equipInfo.equipDrawingId);
            if (data.quiet != 1)
            {
                if (data.equipInfo.equipState == 1)
                {
                    PlatformManager.inst.GameHandleEventLog("Unlock Drawing", data.equipInfo.equipDrawingId.ToString());
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.UnLockDrawing, "", data.equipInfo.equipDrawingId, 0, 1));
                }
                else if (data.equipInfo.equipState == 2)
                {

                    if (!TreasureBoxDataProxy.inst.isOpening)
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.ActivateDrawing, "", data.equipInfo.equipDrawingId, 0, 1));
                }
            }
            EquipDataProxy.inst.SetNewEquipDrawing(data.equipInfo.equipDrawingId);  //新图纸
            data.equipInfo.lastMakeTime = data.equipInfo.activateTime;
            isnew = true;
        }
        EquipDataProxy.inst.updateEquipInfo(data.equipInfo, isnew);
    }

    //购买制作位返回
    private void EquipBuySlotResp(HttpMsgRspdBase msg)
    {
        Response_Equip_BuySlot data = (Response_Equip_BuySlot)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            EquipDataProxy.inst.AddMakeSlot(data.makingSlot.slotId);
        }
        //关闭购买制作槽位的界面
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_BuyMakingSlot);
        //刷新主界面
        //EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Normal);
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIHanlde_RefreshMakeBtnState);
    }

    //装备制作开始
    private void EquipMakeStart(int _equipDrawingid)
    {

        if (currSlotHandler == -1)
        {
            //查找一个空闲slot
            currSlotHandler = EquipDataProxy.inst.GetIdleEquipMakeSlot();
        }

        if (currSlotHandler == -1)
        {
            AudioManager.inst.PlaySound(39);
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("没有空的槽"), Color.white);
            return;
        }
        if (!CheckMakeEquipRes(_equipDrawingid))
        {
            AudioManager.inst.PlaySound(39);
            return;
        }
        //检测消耗有没有高阶
        EquipData data = EquipDataProxy.inst.GetEquipData(_equipDrawingid);
        if (data == null)
        {
            return;
        }

        List<UseAdvancedOrLockEquipTipsData> datas = new List<UseAdvancedOrLockEquipTipsData>();

        if (data.specialRes_1.needId > 0)
        {
            if (data.specialRes_1.type == 1)
            {
                int count = 0;
                var equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(data.specialRes_1.needId);
                var hcount = data.specialRes_1.needCount;
                int commonQuality = equipconfig.quality;

                for (int q = commonQuality; q <= 5; q++) //普通装备
                {
                    var equipItem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);

                    if (equipItem != null)
                    {
                        count = (int)equipItem.count;

                        if (count > 0)
                        {
                            equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(equipconfig.equip_id, q);

                            if (equipItem.isLock)
                            {
                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                            }

                            if (q > commonQuality)
                            {
                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                            }
                        }
                        if (count >= hcount)
                        {
                            break;
                        }
                        hcount -= count;
                    }
                }

                if (count < hcount) //超凡装备
                {
                    for (int q = equipconfig.quality > StaticConstants.SuperEquipBaseQuality ? equipconfig.quality : StaticConstants.SuperEquipBaseQuality + equipconfig.quality; q <= StaticConstants.SuperEquipBaseQuality + 5; q++)
                    {

                        var equipItem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                        if (equipItem != null)
                        {
                            count = (int)equipItem.count;
                            if (count > 0)
                            {
                                equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(equipconfig.equip_id, q);

                                if (equipItem.isLock)
                                {
                                    datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                                }

                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                            }
                            if (count >= hcount)
                            {
                                break;
                            }
                            hcount -= count;
                        }
                    }
                }
            }
        }

        if (data.specialRes_2.needId > 0)
        {
            if (data.specialRes_2.type == 1)
            {
                int count = 0;
                var equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(data.specialRes_2.needId);
                var hcount = data.specialRes_2.needCount;
                int commonQuality = equipconfig.quality;

                for (int q = commonQuality; q <= 5; q++) //普通装备
                {
                    var equipItem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);

                    if (equipItem != null)
                    {
                        count = (int)equipItem.count;

                        if (count > 0)
                        {
                            equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(equipconfig.equip_id, q);

                            if (equipItem.isLock)
                            {
                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                            }

                            if (q > commonQuality)
                            {
                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                            }
                        }
                        if (count >= hcount)
                        {
                            break;
                        }
                        hcount -= count;

                    }
                }

                if (count < hcount)
                {
                    for (int q = equipconfig.quality > StaticConstants.SuperEquipBaseQuality ? equipconfig.quality : StaticConstants.SuperEquipBaseQuality + equipconfig.quality; q <= StaticConstants.SuperEquipBaseQuality + 5; q++) //超凡装备
                    {
                        var equipItem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                        if (equipItem != null)
                        {
                            count = (int)equipItem.count;
                            if (count > 0)
                            {
                                equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(equipconfig.equip_id, q);

                                if (equipItem.isLock)
                                {
                                    datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 0, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("物品已锁定"), content = LanguageManager.inst.GetValueByKey("此物品当前已锁定。任要使用？") });
                                }

                                datas.Add(new UseAdvancedOrLockEquipTipsData() { type = 1, equipId = equipconfig.id, title = LanguageManager.inst.GetValueByKey("正在使用高稀物品"), content = LanguageManager.inst.GetValueByKey("会被交出。仍要继续吗？") });
                            }
                            if (count >= hcount)
                            {
                                break;
                            }
                            hcount -= count;
                        }
                    }

                }
            }
        }

        if (datas.Count > 0)
        {
            System.Action callback = () =>
            {
                requestEquipMake(_equipDrawingid);
            };
            //EventController.inst.TriggerEvent(GameEventType.UseAdvancedEquip, callback, needEquips, LanguageManager.inst.GetValueByKey("会被用于这次制作，仍要继续吗？"));
            HotfixBridge.inst.TriggerLuaEvent("UseAdvancedOrLockEquipTipsEvent.UseAdvancedOrLockEquipFromCS", datas, callback);
        }
        else
        {
            requestEquipMake(_equipDrawingid);
        }
    }
    #region  制造列表
    #region 装备制作本地逻辑
    public void LocalMakeEquip(MakeStartSlot makedata)
    {
        EquipData data = EquipDataProxy.inst.GetEquipData(makedata.equipDrawingId);
        if (data != null)
        {
            //临时扣除资源
            int _count = 0;
            if (data.specialRes_1.needId > 0)
            {
                if (data.specialRes_1.type == 2)
                {
                    var item = ItemBagProxy.inst.GetItem(data.specialRes_1.needId);
                    if (item != null)
                    {
                        ItemBagProxy.inst.updateItemNum(data.specialRes_1.needId, (long)item.count - data.specialRes_1.needCount);
                    }

                }
                else if (data.specialRes_1.type == 1)
                {
                    var equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(data.specialRes_1.needId);
                    _count = data.specialRes_1.needCount;
                    for (int q = equipconfig.quality; q <= 5; q++) //先普通装备
                    {
                        var equipitem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                        if (equipitem != null)
                        {
                            var _targetcount = equipitem.count >= _count ? equipitem.count - _count : 0;
                            _count = _count - (int)equipitem.count;
                            ItemBagProxy.inst.updateEquipNum(equipitem.itemUid, equipitem.ID, _targetcount, equipitem.getTime, equipitem.isLock, equipitem.onShelfCount);
                            if (_count <= 0)
                            {
                                break;
                            }

                        }
                    }

                    if (_count > 0) //再超凡装备
                    {
                        for (int q = equipconfig.quality > StaticConstants.SuperEquipBaseQuality ? equipconfig.quality : StaticConstants.SuperEquipBaseQuality + equipconfig.quality; q <= StaticConstants.SuperEquipBaseQuality + 5; q++)
                        {
                            var equipitem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                            if (equipitem != null)
                            {
                                var _targetcount = equipitem.count >= _count ? equipitem.count - _count : 0;
                                _count = _count - (int)equipitem.count;
                                ItemBagProxy.inst.updateEquipNum(equipitem.itemUid, equipitem.ID, _targetcount, equipitem.getTime, equipitem.isLock, equipitem.onShelfCount);
                                if (_count <= 0)
                                {
                                    break;
                                }

                            }
                        }
                    }

                }
            }

            if (data.specialRes_2.needId > 0)
            {
                if (data.specialRes_2.type == 2)
                {
                    var item = ItemBagProxy.inst.GetItem(data.specialRes_2.needId);
                    if (item != null)
                    {
                        ItemBagProxy.inst.updateItemNum(data.specialRes_2.needId, (long)item.count - data.specialRes_2.needCount);
                    }

                }
                else if (data.specialRes_2.type == 1)
                {
                    var equipconfig = EquipConfigManager.inst.GetEquipQualityConfig(data.specialRes_2.needId);
                    _count = data.specialRes_2.needCount;
                    for (int q = equipconfig.quality; q <= 5; q++) //先普通装备
                    {
                        var equipitem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                        if (equipitem != null)
                        {
                            var _targetcount = equipitem.count >= _count ? equipitem.count - _count : 0;
                            _count = _count - (int)equipitem.count;
                            ItemBagProxy.inst.updateEquipNum(equipitem.itemUid, equipitem.ID, _targetcount, equipitem.getTime, equipitem.isLock, equipitem.onShelfCount);
                            if (_count <= 0)
                            {
                                break;
                            }

                        }
                    }

                    if (_count > 0) //再超凡装备
                    {
                        for (int q = equipconfig.quality > StaticConstants.SuperEquipBaseQuality ? equipconfig.quality : StaticConstants.SuperEquipBaseQuality + equipconfig.quality; q <= StaticConstants.SuperEquipBaseQuality + 5; q++)
                        {
                            var equipitem = ItemBagProxy.inst.GetEquipItem(equipconfig.equip_id, q);
                            if (equipitem != null)
                            {
                                var _targetcount = equipitem.count >= _count ? equipitem.count - _count : 0;
                                _count = _count - (int)equipitem.count;
                                ItemBagProxy.inst.updateEquipNum(equipitem.itemUid, equipitem.ID, _targetcount, equipitem.getTime, equipitem.isLock, equipitem.onShelfCount);
                                if (_count <= 0)
                                {
                                    break;
                                }

                            }
                        }
                    }

                }
            }
            for (int i = 0; i < data.needRes.Length; i++)
            {
                var needres = data.needRes[i];
                var item = ItemBagProxy.inst.GetItem(needres.needId);
                if (item != null)
                {
                    ItemBagProxy.inst.updateItemNum(needres.needId, (long)item.count - needres.needCount);
                }

                RefreshResProductionBar(needres.needId);
            }

            //开始制作
            EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(makedata.slotId);
            if (makerslot != null)
            {
                if (makedata.equipDrawingId > 0)
                {
                    EquipData _equipdata = EquipDataProxy.inst.GetEquipData(makedata.equipDrawingId);
                    if (_equipdata != null)
                    {
                        _equipdata.isNew = false;
                    }

                    EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(makedata.equipDrawingId);
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("开始制作") + LanguageManager.inst.GetValueByKey(cfg.name), GUIHelper.GetColorByColorHex("FFD907"));
                    makerslot.StartWork(makedata.equipDrawingId, data.makeTime, data.makeTime, (int)EMakingState.Making);
                }
            }
            //刷新UI todo 本地逻辑刷新类型，，
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Normal);

        }

    }
    #endregion
    public List<MakeStartSlot> _makeStartList = new List<MakeStartSlot>();
    private void SyncEquipMakerData()
    {
        if (_makeStartList == null || _makeStartList.Count == 0)
        {
            return;
        }

        Request_Equip_MakeStart mskes = new Request_Equip_MakeStart();
        foreach (var slot in _makeStartList)
        {
            mskes.makeStartList.Add(slot);
        }
        _makeStartList.Clear();
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = mskes
        });
    }
    int SendMakeidTimerId = 0;

    private void CheckEquipMakerList()
    {
        if (SendMakeidTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(SendMakeidTimerId);
        }
        if (EquipDataProxy.inst.GetIdleEquipMakeSlot() == -1)
        {
            SyncEquipMakerData();
        }
        else
        {
            SendMakeidTimerId = GameTimer.inst.AddTimer(1, 1, () =>
            {
                SyncEquipMakerData();
                SendMakeidTimerId = 0;
            });
        }
    }

    private void requestEquipMake(int _equipDrawingid)
    {
        if (!NetworkManager.inst.isOnline)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("无法链接服务器"), Color.white);
            return;
        }

        AudioManager.inst.PlaySound(38);

        MakeStartSlot maker = new MakeStartSlot();
        maker.slotId = currSlotHandler;
        maker.equipDrawingId = (int)_equipDrawingid;
        maker.startTime = (int)GameTimer.inst.serverNow;
        _makeStartList.Add(maker);
        LocalMakeEquip(maker);
        if (EquipDataProxy.inst.GetIdleEquipMakeSlot(currSlotHandler) == -1)
        {
            hideEquipListUI();
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTIMERRESET, true);
            GUIManager.HideView<LackResUIView>();
        }
        currSlotHandler = -1;
        CheckEquipMakerList();
    }

    private void EquipMakeStartResp(HttpMsgRspdBase msg)
    {
        var data = (Response_Equip_MakeStart)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            foreach (var slot in data.makingList)
            {
                EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(slot.slotId);
                if (makerslot != null)
                {
                    makerslot.StartWork(slot.equipDrawingId, slot.remainTime, slot.endTime - slot.startTime, slot.makingState);
                }
            }
            // foreach (var slot in EquipDataProxy.inst.equipSlotList)
            // {
            //     MakingSlot makerslot = data.makingList.Find(s => s.slotId == slot.slotId);
            //     if (makerslot != null)
            //     {
            //         if (makerslot.equipDrawingId > 0)
            //         {
            //             EquipData _equipdata = EquipDataProxy.inst.GetEquipData(makerslot.equipDrawingId);
            //             if (_equipdata != null)
            //             {
            //                 _equipdata.isNew = false;
            //             }
            //             EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(makerslot.equipDrawingId);
            //             slot.StartWork(makerslot.equipDrawingId, makerslot.remainTime, makerslot.endTime - makerslot.startTime, makerslot.makingState);
            //         }
            //         var _mskeslot = _makeStartList.Find(s => s.slotId == makerslot.slotId);
            //         if (_mskeslot != null) _makeStartList.Remove(_mskeslot);
            //     }
            //     else
            //     {
            //         slot.StartWork(-1, 0, 0, 0);
            //         var _mskeslot = _makeStartList.Find(s => s.slotId == slot.slotId);
            //         if (_mskeslot != null) _makeStartList.Remove(_mskeslot);
            //     }
            //     //_makeStartList.Clear();
            // }
            //刷新UI
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Refresh);
        }
    }
    #endregion 制造列表
    private void EquipMakeRefreshResp(HttpMsgRspdBase msg)
    {
        var data = (Response_Equip_MakeRefresh)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(data.makingSlot.slotId);
            if (makerslot != null)
            {
                if (data.makingSlot.equipDrawingId > 0)
                {
                    makerslot.StartWork(data.makingSlot.equipDrawingId, data.makingSlot.remainTime, data.makingSlot.endTime - data.makingSlot.startTime, data.makingSlot.makingState);
                }
            }
            if (equipMakingView != null && equipMakingView.isShowing)
            {
                equipMakingView.setState(data.makingSlot.slotId, data.makingSlot.makingState);
            }
            var mskeslot = _makeStartList.Find(slot => slot.slotId == makerslot.slotId);
            if (mskeslot != null) _makeStartList.Remove(mskeslot);
        }

        //刷新UI
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Refresh);

    }
    private void EquipMakeEndResp(HttpMsgRspdBase msg)
    {
        var data = (Response_Equip_MakeEnd)msg;

        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            bool canFly = true;

            EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(data.makingSlot.slotId);
            if (makerslot != null)
            {
                makerslot.ReInit();
                var mskeslot = _makeStartList.Find(slot => slot.slotId == makerslot.slotId);
                if (mskeslot != null) _makeStartList.Remove(mskeslot);
            }

            //判断升星效果触发
            if (data.starEffectType > 0)
            {
                ReceiveInfoUIType receiveInfoUIType = ReceiveInfoUIType.StarUpEffectTrigger_return + data.starEffectType - 1;
                Award_AboutVal triggerData = new Award_AboutVal() { type = receiveInfoUIType, val = data.bagEquip.equipId };
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, triggerData);

            }

            EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(data.bagEquip.equipId);
            if (eqcfg != null)
            {
                //获得高品质装备 
                if (eqcfg.quality > 1)
                {
                    var item = new queueItem(ReceiveInfoUIType.AdvacedEquip, data.bagEquip.equipUid, data.bagEquip.equipId, 0, 1);
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, item);
                    //双重制作且是高品质 额外弹窗一次高品质
                    if (data.starEffectType > 0 && (ReceiveInfoUIType.StarUpEffectTrigger_return + data.starEffectType - 1) == ReceiveInfoUIType.StarUpEffectTrigger_double)
                    {
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, item);
                    }
                }
                //图纸碎片数量
                if (data.rewardPiece > 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, StaticConstants.drawingID, data.rewardPiece));
                }
                if (canFly)
                {
                    SetEquipFly(data.makingSlot.slotId, data.bagEquip.equipUid, data.toStoreBasket);
                }
                //判断是否需要货架
                if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
                {
                    EquipDrawingsConfig drawingcfg = EquipConfigManager.inst.GetEquipDrawingsCfg(eqcfg.equip_id);
                    helpConfig hcfg = GameHelpNavigationConfigManager.inst.GetHelpConfigBytyp(3, drawingcfg.sub_type);
                    if (hcfg != null)
                    {
                        bool hasF = false;
                        foreach (int fid in hcfg.triggered_val_2)
                        {
                            if (UserDataProxy.inst.shopData.hasFurnitureInShop(fid))
                            {
                                hasF = true;
                                break;
                            }
                        }
                        if (!hasF)
                        {
                            if (!HotfixBridge.inst.GetTriggerIsTrig(hcfg.triggered_val_2[0]))
                            {
                                HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 2, hcfg.triggered_val_2[0]);
                            }
                            else
                            {
                                if (!GuideManager.inst.isInTriggerGuide)
                                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", hcfg.id);
                            }
                        }
                    }
                }
            }
        }
        else if (data.errorCode == 300) //前端单机缓存数据与服务器数据不匹配 要领取的槽位装备后端不存在 前端将移除他
        {
            EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(data.makingSlot.slotId);
            if (makerslot != null)
            {
                makerslot.ReInit();
                var mskeslot = _makeStartList.Find(slot => slot.slotId == makerslot.slotId);
                if (mskeslot != null) _makeStartList.Remove(mskeslot);
            }

            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("数据异常，已重新同步"), Color.white);

        }

        //刷新UI
        EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.MakeEnd);
        if (equipMakingView != null && equipMakingView.isShowing)
        {
            // equipMakingView.hide();
            GUIManager.HideView<EquipMakingView>();
        }

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            if (GuideDataProxy.inst.CurInfo.m_curCfg.btn_view == "mainUI" && GuideDataProxy.inst.CurInfo.m_curCfg.btn_name == "1")
            {
                if (ItemBagProxy.inst.getEquipAllNumber(GuideManager.inst.CurrEquipId) > 0 || GuideManager.inst.CurrEquipId == -1)
                {
                    GuideDataProxy.inst.CurInfo.isClickTarget = true;
                    var btn = GuideManager.inst.curTargetBtn.GetComponent<UnityEngine.UI.Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveListener(GuideManager.inst.curFunc);
                    }
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETPROMPTPOS, GuideManager.inst.curTargetBtn.transform, false);
                    GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
                }
            }
        }
    }
    private void SetEquipFly(int slotId, string equipUid, int toStoreBasket)
    {
        EventController.inst.TriggerEvent(GameEventType.EquipEvent.SET_EQUIPFLY, slotId, equipUid, toStoreBasket);
    }
    private void EquipMakeFasterResp(HttpMsgRspdBase msg)
    {
        var data = (Response_Equip_MakeFaster)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(data.makingSlot.slotId);
            if (makerslot != null)
            {
                if (data.makingSlot.equipDrawingId > 0)
                {
                    makerslot.StartWork(data.makingSlot.equipDrawingId, data.makingSlot.remainTime, data.makingSlot.endTime - data.makingSlot.startTime, data.makingSlot.makingState);
                }
            }
            //刷新UI
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Normal);
            if (equipMakingView != null && equipMakingView.isShowing)
            {
                equipMakingView.setState(data.makingSlot.slotId, data.makingSlot.makingState);
            }
        }

    }
    //提前完成
    private void EquipMakeFaster(int slotid, int isgem = 0)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_MakeFaster()
            {
                slotId = slotid,
                useGem = isgem
            }
        });
    }
    //装备制作时间完成
    private void Equipmaked(int slotid)
    {
        if (ItemBagProxy.inst.GetEquipInventory() >= ItemBagProxy.inst.bagCountLimit)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("仓库已满，请升级您的保险柜！"), GUIHelper.GetColorByColorHex("FFD907"));
            return;
        }
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_MakeEnd()
            {
                slotId = slotid
            }
        });
    }

    //装备刷新
    private void EquipSlotUpdate(int slotid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_MakeRefresh()
            {
                slotId = slotid
            }
        });
    }
    //收藏
    private void EquipFavorite(int _equipDrawingId, bool isfavorite)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_FavoriteEquip()
            {
                equipDrawingId = _equipDrawingId,
                favorite = isfavorite ? 1 : 0
            }
        });
    }
    //请求更新装备数据
    private void getEquipInfo()
    {
        //请求消息
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_Data()
        });
    }

    private void OnEquipDataResp(Response_Equip_Data data)
    {
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            // Logger.log("获取装备信息data==：" + data.GetJsonParams());
            //装备info
            foreach (var eInfo in data.equipList)
            {
                EquipDataProxy.inst.updateEquipInfo(eInfo);
            }
            //制作槽
            EquipDataProxy.inst.ClearMakeSlots();

            foreach (MakingSlot slot in data.makingList)
            {
                EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(slot.slotId);
                if (makerslot == null)
                {
                    makerslot = EquipDataProxy.inst.AddMakeSlot(slot.slotId);
                }
                if (slot.equipDrawingId > 0)
                {
                    makerslot.StartWork(slot.equipDrawingId, slot.remainTime, slot.endTime - slot.startTime, slot.makingState);
                }
            }

            //刷新UI
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Normal);
        }
    }

    private void OnEquipMakingListUpdate(HttpMsgRspdBase msg)
    {
        Response_Equip_MakingList data = (Response_Equip_MakingList)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //制作槽
            EquipDataProxy.inst.ClearMakeSlots();
            foreach (MakingSlot slot in data.makingList)
            {
                EquipMakerSlot makerslot = EquipDataProxy.inst.GetMakeSlot(slot.slotId);
                if (makerslot == null)
                {
                    makerslot = EquipDataProxy.inst.AddMakeSlot(slot.slotId);
                }
                if (slot.equipDrawingId > 0)
                {
                    makerslot.StartWork(slot.equipDrawingId, slot.remainTime, slot.endTime - slot.startTime, slot.makingState);
                }

            }

            //刷新UI
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, ESlotAnimType.Normal);
        }
    }
    //刷新UI显示
    public void RefreshResProductionBar(int itemId)
    {

        equipListUIView = GUIManager.GetWindow<EquipListUIView>();
        if (equipListUIView != null && equipListUIView.isShowing)
        {
            equipListUIView.RefreshResProductionBar(itemId);
        }

        if (lackResUIView != null && lackResUIView.isShowing)
        {
            lackResUIView.RefreshResProduction(itemId);
        }

        if (_resourceBuyProductionUI != null && _resourceBuyProductionUI.isShowing)
        {
            _resourceBuyProductionUI.RefreshResProduction(itemId);
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private void showEquipDrawingsInfoUI(int equipid, List<EquipData> datalist)
    {
        EquipData data = EquipDataProxy.inst.GetEquipData(equipid);

        GUIManager.OpenView<EquipInfoUIView>(view =>
        {
            view.ShowInfo(data, datalist);
        });
    }

    private void showEquipInfoByEquipDrawingId(int equipDrawingId)
    {
        GUIManager.OpenView<EquipInfoUIView>(view =>
        {
            view.ShowInfo(equipDrawingId);
        });
    }

    private void refreshEquipInfoUIStarUp()
    {
        var view = GUIManager.GetWindow<EquipInfoUIView>();

        if (view != null && view.isShowing)
        {
            view.RefreshStarUpInfo();
        }
    }

    private void hideEquipDrawingsInfoUI()
    {
        GUIManager.HideView<EquipInfoUIView>();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    ///装备升星

    private void request_equipStarUP(int equipId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Hero_UseEquipItem()
            {
                equipId = equipId
            }
        });
    }

    private void equipStarUpResp(HttpMsgRspdBase msg)
    {
        Response_Hero_UseEquipItem data = (Response_Hero_UseEquipItem)msg;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        if (data.itemUse == (int)EItemUse.Success)
        {
            queueItem queueData = new queueItem(ReceiveInfoUIType.DrawingStarUp, "", data.equipDrawingId, 0, 0);
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, queueData);
            EventController.inst.TriggerEvent(GameEventType.RefreshUI_EquipInfoUIStarUp);
            EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_STARUPSUCCESS, data.equipDrawingId);
            Logger.log("升星道具使用成功");
        }
    }

    ///


}