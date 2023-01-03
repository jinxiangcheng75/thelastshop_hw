using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GlobalBuffType
{
    none,
    sell_priceUp,//售价提升
    sell_expUp,//经验提升
    sell_energyUp,//体力提升
    sell_doublePriceUp,//加价提升
    make_equipSpeedUp,//制作速度提升
    make_equipRarityUp,//制作稀有度提升
    make_materialsSpeedUp,//材料生产速度提升
    explore_speedUp,//副本冒险速度提升
    hero_restTimeDown,//英雄休息时间缩减
    explore_dropUp,//副本掉落提升
    explore_heroExpUp,//副本结算英雄经验提升
    hero_rarityUp,//高品质英雄出现概率提升
    box_uniqueAwardUp,//宝箱开出稀有物品概率提升


    //预留 特殊类型
    sell_doubleEnergyDown,//讨价还价双倍所需体力减少
    make_equipDoubleUp,//制作出现双倍装备概率提升
}




//全局buff数据
public class GlobalBuffData
{
    public int buffuId;
    public GlobalBuffType buffType;
    public ActivityBuff buffInfo;
    public int remainTime //剩余时间
    {
        get 
        {
            return endTime - (int)GameTimer.inst.serverNow;
        }
    }
    int endTime;//结束时间

    public GlobalBuffConfig config;
    List<ActivityBuffText> textList;

    int timer;

    public GlobalBuffData(int _buffUid, ActivityBuff _buffInfo)
    {
        SetInfo(_buffUid, _buffInfo);
    }

    public void SetInfo(int _buffUid, ActivityBuff _buffInfo)
    {
        buffuId = _buffUid;
        buffInfo = _buffInfo;
        config = GlobalBuffConfigManager.inst.GetConfig(_buffInfo.buffType);
        this.buffType = (GlobalBuffType)_buffInfo.buffType;
        this.endTime = (int)GameTimer.inst.serverNow + _buffInfo.activityTime;

        textList = _buffInfo.textList;

        setTimer();
    }

    public string title_0 { get { return textList.Find(t => t.lang == (int)LanguageManager.inst.curType).buffTitle; } }
    public string title_1 { get { return textList.Find(t => t.lang == (int)LanguageManager.inst.curType).buffTitle1; } }
    public string title_2 { get { return textList.Find(t => t.lang == (int)LanguageManager.inst.curType).buffTitle2; } }
    public string title_3 { get { return textList.Find(t => t.lang == (int)LanguageManager.inst.curType).buffText; } }
    public string con { get { return textList.Find(t => t.lang == (int)LanguageManager.inst.curType).buffConform; } }
    public string herald { get { return textList.Find(t => t.lang == (int)LanguageManager.inst.curType).buffHerald; } }



    void setTimer()
    {
        if (timer != 0)
        {
            GameTimer.inst.RemoveTimer(timer);
            timer = 0;
        }

        timer = GameTimer.inst.AddTimer(remainTime, 1, countdown, GameTimerType.byServerTime);
    }

    void countdown()
    {
        if (remainTime > 0)
        {
            timer = GameTimer.inst.AddTimer(remainTime, 1, countdown, GameTimerType.byServerTime);
        }
        else
        {
            GlobalBuffDataProxy.inst.ClearOneGlobalBuffData(buffuId); //前端先删掉他
            // 发消息刷新buff
            EventController.inst.TriggerEvent<int>(GameEventType.GlobalBuffEvent.REQEST_GLOBALBUFF_UPDATE, buffuId);
        }
    }

    public void ClearTimer()
    {
        if (timer != 0)
        {
            GameTimer.inst.RemoveTimer(timer);
        }
    }

}
