using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mosframe;

public class RoleListItemUI : MonoBehaviour, IDynamicScrollViewItem
{
    public GameObject addFieldObj; // 扩展槽位
    public GameObject recruitHeroObj; // 招募英雄
    public GameObject recoverHeroObj; // 全部恢复
    public GameObject exchangeObj; // 兑换英雄
    public GameObject addNewObj; // 扩招槽位后获取新英雄
    public RoleHeroItemComp heroObj; // 英雄信息
    public WorkerItemComp workerItem; // 工匠信息
    public Button selfBtn;

    [Header("other component")]
    public Text itemNumText;
    public Text gemNumText;
    public Text bottomText;
    public Text middleText;
    public GameObject recruitRedPoint;
    public GameObject exchangeRedPoint;
    public GameObject addFieldRedPoint;

    private WorkerData _workerData;
    private RoleHeroData data;

    private kRoleItemType type = kRoleItemType.max;
    int timerId = 0;
    public int index = 0;

    private void Awake()
    {
        selfBtn.onClick.AddListener(BtnClickMethod);
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public void InitAddNew()
    {
        gameObject.name = "addNew";
        type = kRoleItemType.AddNew;
        setActivef();
    }

    public void InitExchangeHeroData()
    {
        gameObject.name = "exchangeHero";
        type = kRoleItemType.ExchangeHero;
        setActivef();
        exchangeRedPoint.SetActive(RoleDataProxy.inst.hasCanExchangeHero && RoleDataProxy.inst.exchangeIsNew);
    }

    public void InitWorkerData(WorkerData data)
    {
        gameObject.name = data.id.ToString();
        type = kRoleItemType.Worker;
        setActivef();
        _workerData = data;
        workerItem.InitWorkerData(data);
    }

    public void InitHeroData(RoleHeroData data)
    {
        gameObject.name = data.uid.ToString();
        type = kRoleItemType.Hero;
        setActivef();
        this.data = data;
        heroObj.InitHeroData(data);
    }

    public void InitAddFieldData()
    {
        gameObject.name = "addSlot";
        type = kRoleItemType.AddField;
        setActivef();
        var proxyData = RoleDataProxy.inst;
        addFieldRedPoint.SetActive(RoleDataProxy.inst.hasCanBuyField);
        //bottomText.text = proxyData.HeroList.Count + "/" + proxyData.heroFieldCount;
        var fieldCfg = FieldConfigManager.inst.GetFieldConfig(1, proxyData.heroFieldCount + 1);
        if (fieldCfg != null)
        {
            if (UserDataProxy.inst.playerData.level >= fieldCfg.level)
                middleText.text = LanguageManager.inst.GetValueByKey("可解锁一个栏位");
            else
                middleText.text = LanguageManager.inst.GetValueByKey("<color=#B63801>{0}级</color>+1个栏位", fieldCfg.level.ToString());
        }
    }

    public void InitAllRestingData()
    {
        gameObject.name = "allResting";
        type = kRoleItemType.RecoverHero;
        setActivef();

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        Item itemData = ItemBagProxy.inst.GetItem(150001);
        itemNumText.text = itemData.count.ToString();
        itemNumText.color = itemData.count >= 1 ? Color.white : Color.red;

        var allRestingHero = RoleDataProxy.inst.GetRestingStateHeroCount();
        int result = 0;
        //int result = 0;
        for (int i = 0; i < allRestingHero.Count; i++)
        {
            int index = i;
            result += DiamondCountUtils.GetHeroRestingFastCost(allRestingHero[index].remainTime);
        }

        //result = DiamondCountUtils.GetHeroRestingFastCost(allRemainTime);
        gemNumText.text = result.ToString("N0");
        gemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;


        timerId = GameTimer.inst.AddTimer(1, () =>
         {
             result = 0;
             for (int i = 0; i < allRestingHero.Count; i++)
             {
                 result += DiamondCountUtils.GetHeroRestingFastCost(allRestingHero[i].remainTime);
             }
             //result = DiamondCountUtils.GetHeroRestingFastCost(allRemainTime);
             gemNumText.text = result.ToString("N0");
             gemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
         });
    }

    public void InitRecruitData()
    {
        gameObject.name = "recruitHero";
        type = kRoleItemType.RecruitHero;
        setActivef();
        recruitRedPoint.SetActive(RoleDataProxy.inst.costValue <= 0 && RoleDataProxy.inst.recruitIsNew);
    }

    private void BtnClickMethod()
    {
        bool flag = false;
        switch (type)
        {
            case kRoleItemType.Hero:
                if (data.currentState != 0)
                {
                    flag = true;
                    timerId = heroObj.timerId;
                };
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.uid);
                break;
            case kRoleItemType.AddField:
                RoleDataProxy.inst.slotComType = 1;
                RoleDataProxy.inst.hasCanBuyField = false;
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.BUYSLOT_SHOWUI, RoleDataProxy.inst.heroFieldCount);
                break;
            case kRoleItemType.RecruitHero:
                RoleDataProxy.inst.enterType = 0;
                RoleDataProxy.inst.recruitIsNew = false;
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUIT_SHOWUI);
                break;
            case kRoleItemType.RecoverHero:
                flag = true;
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ALLROLERESTING_SHOWUI);
                break;
            case kRoleItemType.Worker:
                if (_workerData.state == EWorkerState.Unlock)
                {
                    EventController.inst.TriggerEvent(GameEventType.WorkerCompEvent.SHOWUI_WORKERINFOUI, _workerData);
                }
                else if (_workerData.state == EWorkerState.CanUnlock)
                {
                    EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, _workerData.id, true, null);
                }
                else if (_workerData.state == EWorkerState.Locked)
                {
                    EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, _workerData.id, true, null);
                }
                break;
            case kRoleItemType.ExchangeHero:
                if (RoleDataProxy.inst.exchangeIsNew)
                {
                    RoleDataProxy.inst.exchangeIsNew = false;
                }
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_RoleExchange");
                break;
            case kRoleItemType.AddNew:
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_RoleAddNew");
                break;
            default:
                break;
        }

        if (flag && timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    private void setActivef()
    {
        addFieldObj.SetActive(type == kRoleItemType.AddField);
        recruitHeroObj.SetActive(type == kRoleItemType.RecruitHero);
        recoverHeroObj.SetActive(type == kRoleItemType.RecoverHero);
        heroObj.gameObject.SetActive(type == kRoleItemType.Hero);
        workerItem.gameObject.SetActive(type == kRoleItemType.Worker);
        exchangeObj.SetActive(type == kRoleItemType.ExchangeHero);
        addNewObj.SetActive(type == kRoleItemType.AddNew);
    }

    private void OnDisable()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
