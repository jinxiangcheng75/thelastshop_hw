using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBoxData
{
    public int currOrderIndex = 0;  //当前第几阶存储罐 0 ----- 6 
    public int currState = 0;     //状态: 0 未开始 1 进行中 2 待领取
    public int lastStateEndTime = 0;//上一阶结束时间
    private int stateStartTime = 0;  //下一阶段开始时间
    public int startMinLv = 0;
    public int stateCoolTime  //当前阶段剩余冷却时间
    {
        get
        {
            return stateStartTime - TimeUtils.GetNowSeconds();
        }
        set
        {
            stateStartTime = TimeUtils.GetNowSeconds() + value;
        }
    }
    public int targetGoldCount = 0; //目标金币数
    public int hasBeenStored = 0; //已经存储金币数
}

public class MoneyBoxDataProxy : TSingletonHotfix<MoneyBoxDataProxy>, IDataModelProx
{
    MoneyBoxData data;
    int timerId;

    public bool NeedShowRedPoint
    {
        get
        {
            if (data != null && data.currState == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void Clear()
    {
        data = null;
    }

    public void Init()
    {
        Helper.AddNetworkRespListener(MsgType.Response_PiggyBank_BaseData_Cmd, dataUpdate);
    }

    //
    public MoneyBoxData moneyBoxData
    {
        get { return data; }
        private set { data = value; }
    }
    void dataUpdate(HttpMsgRspdBase msg)
    {
        Response_PiggyBank_BaseData data = (Response_PiggyBank_BaseData)msg;
        if (moneyBoxData == null)
            moneyBoxData = new MoneyBoxData();
        updateData(data.piggyBankData);
        EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_ONDATAUPDATE);
        //福利整合 若储蓄罐打开 更新储蓄罐
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_WelfareContent", 4);
    }

    public void updateData(PiggyBankData data)
    {
        moneyBoxData.currOrderIndex = data.currOrderIndex;
        moneyBoxData.currState = data.piggyBankState;
        moneyBoxData.hasBeenStored = data.hasBeenStored;
        moneyBoxData.targetGoldCount = data.targetGoldCount;
        moneyBoxData.stateCoolTime = data.stateCoolTime;
        moneyBoxData.startMinLv = data.startMinLevel;

        setTimer();
    }

    void setTimer()
    {
        clearTimer();

        if (data == null || data.currState == 2 || data.stateCoolTime <= 0)
        {
            return;
        }

        var worldParCfg = WorldParConfigManager.inst.GetConfig(138);
        if (worldParCfg != null && worldParCfg.parameters > UserDataProxy.inst.playerData.level)
        {
            return;
        }

        timerId = GameTimer.inst.AddTimer(data.stateCoolTime, 1, timerMethod, GameTimerType.byServerTime);

    }

    void timerMethod()
    {
        if (data.stateCoolTime > 0)
        {
            timerId = GameTimer.inst.AddTimer(data.stateCoolTime, 1, timerMethod, GameTimerType.byServerTime);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_DATA);
        }
    }

    void clearTimer()
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

}
