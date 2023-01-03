using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TObject = System.Object;

public class PopUIInfoBase : TObject
{
    public int extend = 0;  //高品质使用(特殊参数)。 队列加塞。
    public ReceiveInfoUIType type;
}
public class queueItem : PopUIInfoBase
{
    public string equipuid = "";
    public int equipid = 0;
    public int itemid = 0;
    public long count = 1;

    public queueItem(ReceiveInfoUIType _type, string _uid, int _equipid, int _itemid, long _count)
    {
        type = _type;
        equipuid = _uid;
        equipid = _equipid;
        itemid = _itemid;
        count = _count;
        extend = 0;
    }
}

public class Award_AboutWorker : PopUIInfoBase
{
    public int workerId;//工匠id
}

public class Award_AboutVal : PopUIInfoBase
{
    public int val; // 参数
}

public class Award_AboutDirectPurchase : PopUIInfoBase
{
    public string directPurchaseUid; //礼包uid
    public bool needShowToggle;//是否显示设置弹出toggle
}

public class Award_AboutGlobalBuff : PopUIInfoBase
{
    public GlobalBuffType buffType;//全服buff类型
}

public class Award_AboutCommon : PopUIInfoBase
{
    public List<CommonRewardData> allRewardList;
}

public class Award_AboutUnionTaskResult : PopUIInfoBase
{
    public Response_Union_TaskResult data;
}

public class Award_AboutBuyGoods : PopUIInfoBase
{
    public string atlas;
    public string icon;
    public string tag;
}

public class Award_AboutHint : PopUIInfoBase
{
    public int helpId = 0;
    public System.Action skipBtnPrepositionHandler = null;
    public int value = 0;
}

public class ReceiveAwardUIMediator : BaseSystem
{
    ReceiveAwardUIView receiveAwardUIView;
    List<PopUIInfoBase> msgQueue = new List<PopUIInfoBase>();
    protected override void AddListeners()
    {
        EventController.inst.AddListener<PopUIInfoBase>(GameEventType.ReceiveEvent.NEWITEM_MSG, AddReceive);
        EventController.inst.AddListener<string>(GameEventType.ReceiveEvent.NEWITEM_MSG_REALY, addReceive_realy);
        EventController.inst.AddListener(GameEventType.ReceiveEvent.GO_ON, nextmsg);
        EventController.inst.AddListener<string>(GameEventType.ReceiveEvent.Equip_IMPROVING_QUALITY, EquipImprovingQuality);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_MakeImprove_Cmd, OnEquipMakeImprove);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<PopUIInfoBase>(GameEventType.ReceiveEvent.NEWITEM_MSG, AddReceive);
        EventController.inst.RemoveListener<string>(GameEventType.ReceiveEvent.NEWITEM_MSG_REALY, addReceive_realy);
        EventController.inst.RemoveListener(GameEventType.ReceiveEvent.GO_ON, nextmsg);
        EventController.inst.RemoveListener<string>(GameEventType.ReceiveEvent.Equip_IMPROVING_QUALITY, EquipImprovingQuality);
    }

    public override void ReInitSystem()
    {
        base.ReInitSystem();
        ismsguishow = false;
        msgQueue = new List<PopUIInfoBase>();
    }

    public static bool ismsguishow = false;
    private void AddReceive(PopUIInfoBase item)
    {
        msgQueue.Add(item);
        if (msgQueue.Count > 1)
        {
            msgQueue.Sort((x1, x2) => x1.type.CompareTo(x2.type));    //先按优先级排序。
            msgQueue.Sort((x1, x2) => -x1.extend.CompareTo(x2.extend)); //再次排序
        }
        if (ismsguishow) return;

        HotfixBridge.inst.TriggerLuaEvent("EventSystem_AddEvent", GameEventType.ReceiveEvent.NEWITEM_MSG_REALY, JsonUtility.ToJson(item), 0);

    }

    private void addReceive_realy(string jsonData)  //PopUIInfoBase
    {
        PopUIInfoBase item = JsonUtility.FromJson<PopUIInfoBase>(jsonData);

        if (item.type == ReceiveInfoUIType.GetItem && msgQueue.Count == 1)
        {
            showUI();
        }
        else
        {
            nextmsg();
        }
    }

    private void showUI()
    {
        ismsguishow = true;
        if (msgQueue.Count <= 0 || (msgQueue.Count > 0 && msgQueue[0].type != ReceiveInfoUIType.UnionTaskTip && msgQueue[0].type != ReceiveInfoUIType.DailyTaskTip)) 
        {
            if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
            {
                float time = 0.1f;
                var worldParCfg = WorldParConfigManager.inst.GetConfig(8307);
                if (worldParCfg != null)
                {
                    time = worldParCfg.parameters / 1000;
                }
                FGUI.inst.showGlobalMask(time);
            }
            else
            {
                FGUI.inst.showGlobalMask(0.5f);
            }
        }
        if (msgQueue.Count <= 0)
        {
            GUIManager.HideView<ReceiveAwardUIView>();
            ismsguishow = false;
            return;
        }
        var item = msgQueue[0];
        PopMsgView(item);
        msgQueue.Remove(item);
        //Logger.error("show弹窗UI了 剩余数量：" + msgQueue.Count);
    }



    private void PopMsgView(PopUIInfoBase info)
    {
        switch (info.type)
        {
            case ReceiveInfoUIType.StarUpEffectTrigger_double:
            case ReceiveInfoUIType.StarUpEffectTrigger_return:
            case ReceiveInfoUIType.StarUpEffectTrigger_super:
                {
                    Award_AboutVal data = info as Award_AboutVal;
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_EquipStarUpEffectTrigger", data);
                    break;
                }
            case ReceiveInfoUIType.LookBack:
                {
                    HotfixBridge.inst.TriggerLuaEvent("Open_LookBackUI");
                }
                break;
            case ReceiveInfoUIType.UnoinTaskResult:
                {
                    var data = (info as Award_AboutUnionTaskResult).data;
                    EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONTASKRESULT, data);
                }
                break;
            case ReceiveInfoUIType.ShopperLvUp:
                {
                    EventController.inst.TriggerEvent(GameEventType.UIUnlock.SHOP_ONLVUP);
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_PLAYERUPUI);
                }
                break;
            case ReceiveInfoUIType.ExploreEquipDamaged:
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.SHOWUI_EQUIPDAMAGED);
                }
                break;
            case ReceiveInfoUIType.ExploreEquipDamagedInfo:
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.SHOWUI_EQUIPDAMAGEDINFO);
                }
                break;
            case ReceiveInfoUIType.ExploreAward:
                {
                    ExploreDataProxy.inst.OpenExploreEndUI();
                    ismsguishow = false;
                }
                break;
            case ReceiveInfoUIType.UnlockWorker:
                {
                    int workerId = (info as Award_AboutWorker).workerId;
                    EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.Worker_UnLock, workerId);
                }
                break;
            case ReceiveInfoUIType.WorkerUp:
                {
                    int workerId = (info as Award_AboutWorker).workerId;
                    EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.Worker_LevelChange, workerId);
                }
                break;
            case ReceiveInfoUIType.GlobalBuff:
                {
                    GlobalBuffType buffType = (info as Award_AboutGlobalBuff).buffType;
                    EventController.inst.TriggerEvent(GameEventType.GlobalBuffEvent.GLOBALBUFF_SHOWUI_DETAIL, buffType);
                }
                break;
            case ReceiveInfoUIType.BuyGoodsComplete:
                {
                    var data = (info as Award_AboutBuyGoods);
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_MallBuyVipOtherComUI", data.atlas, data.icon, data.tag);
                }
                break;
            case ReceiveInfoUIType.VipBuyComplete:
                {
                    var data = (info as Award_AboutVal);
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_VipGetRewardUI", data.val);
                }
                break;
            case ReceiveInfoUIType.CommonReward:
                {
                    var list = (info as Award_AboutCommon).allRewardList;
                    EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONREWARD_SETINFO, list);
                }
                break;
            case ReceiveInfoUIType.SpecialAchievement:
                {
                    int achievementId = (info as Award_AboutVal).val;
                    EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONESPECIALUI, achievementId);
                }
                break;
            case ReceiveInfoUIType.DirectPurchasePush:
                {
                    string giftUid = (info as Award_AboutDirectPurchase).directPurchaseUid;
                    bool needShowToggle = (info as Award_AboutDirectPurchase).needShowToggle;
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GiftDetailUIByUid", giftUid, needShowToggle);
                }
                break;
            case ReceiveInfoUIType.UnlockSystem:
                {
                    int level = (info as Award_AboutVal).val;
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_SystemUnlock", level);
                }
                break;
            case ReceiveInfoUIType.GuideTrigger:
                {
                    int groupId = (info as Award_AboutVal).val;
                    HotfixBridge.inst.TriggerLuaEvent("StartGuideTrigger", groupId);
                }
                break;
            case ReceiveInfoUIType.LuxuryUp:
                {
                    int luxuryLevel = (info as Award_AboutVal).val;
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_LuxuryLevelUp", luxuryLevel);
                }
                break;
            case ReceiveInfoUIType.UnionTaskTip:
                HotfixBridge.inst.TriggerLuaEvent("Show_UnionTaskTip");
                break;
            case ReceiveInfoUIType.DailyTaskTip:
                HotfixBridge.inst.TriggerLuaEvent("Show_DailyTaskTip");
                break;
            case ReceiveInfoUIType.Hint:
                {
                    var data = info as Award_AboutHint;
                    HotfixBridge.inst.TriggerLuaEvent("QueueResp_ShowGameHintUI", data.helpId, data.skipBtnPrepositionHandler, data.value);
                }
                break;
            default:
                {
                    if (receiveAwardUIView != null && receiveAwardUIView.isShowing)
                    {
                        receiveAwardUIView.SetShowInfo((queueItem)info);
                    }
                    else
                    {
                        GUIManager.OpenView<ReceiveAwardUIView>((view) =>
                        {
                            receiveAwardUIView = view;
                            receiveAwardUIView.SetShowInfo((queueItem)info);
                        });
                    }
                }
                break;
        }
    }

    private void EquipImprovingQuality(string equipuid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Equip_MakeImprove() { equipUid = equipuid }
        });
    }
    private void nextmsg()
    {
        //  if (ismsguishow) return;
        ismsguishow = true;
        if (msgQueue.Count > 0)
        {
            if (msgQueue[0].type != ReceiveInfoUIType.UnionTaskTip && msgQueue[0].type != ReceiveInfoUIType.DailyTaskTip) 
            {
                if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
                {
                    float time = 0.1f;
                    var worldParCfg = WorldParConfigManager.inst.GetConfig(8307);
                    if (worldParCfg != null)
                    {
                        time = worldParCfg.parameters / 1000;
                    }
                    FGUI.inst.showGlobalMask(time);
                }
                else
                {
                    FGUI.inst.showGlobalMask(0.5f);
                }
            }
            if (receiveAwardUIView != null && receiveAwardUIView.isShowing)
            {
                //receiveAwardUIView.closeCurrUIPanel();
                GUIManager.HideView<ReceiveAwardUIView>();
            }

            if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
            {
                float time = 0.1f;
                var worldParCfg = WorldParConfigManager.inst.GetConfig(8307);
                if (worldParCfg != null)
                {
                    time = worldParCfg.parameters / 1000;
                }
                GameTimer.inst.AddTimer(time, 1, showUI);
            }
            else
            {
                GameTimer.inst.AddTimer(0.5f, 1, showUI);
            }
            return;
        }
        GUIManager.HideView<ReceiveAwardUIView>();
        //FGUI.inst.setGlobalMaskActice(false);
        ismsguishow = false;

        HotfixBridge.inst.TriggerLuaEvent("EventSystem_EventEnd");

    }

    //钻石升级装备品质返回
    private void OnEquipMakeImprove(HttpMsgRspdBase msg)
    {
        Response_Equip_MakeImprove data = (Response_Equip_MakeImprove)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            var item = new queueItem(ReceiveInfoUIType.AdvacedEquip, data.bagEquip.equipUid, data.bagEquip.equipId, 0, 1);
            item.extend = 1;
            EquipDataProxy.inst.toequipUid = data.bagEquip.equipUid;
            //显示高品质
            AddReceive(item);
        }
        nextmsg();
    }
}
