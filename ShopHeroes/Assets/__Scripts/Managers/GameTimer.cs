using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

/// <summary>
/// 游戏定时器
/// </summary>
/// 

public enum GameTimerType
{
    none = 1, //unity引擎的deltaTime
    byServerTime = 2, //基于服务器时间
    max,
}

[XLua.CSharpCallLua]
[LuaCallCSharp]
public class GameTimer : SingletonMono<GameTimer>
{
    public class Timer
    {
        public int id;
        public bool isActive;

        private float rate;
        private int ticks;
        private int ticksElapsed;
        private float last;
        private Action callBack;
        private int frameCount;
        public GameTimerType tickType;

        private double startTime;


        public Timer(int _id, float _rate, int frame, int _ticks, Action _callback, GameTimerType _tickType)
        {
            id = _id;
            rate = _rate < 0 ? 0 : _rate;
            ticks = _ticks < 0 ? 0 : _ticks;
            callBack = _callback;
            last = 0;
            ticksElapsed = 0;
            isActive = true;
            frameCount = frame;
            tickType = _tickType;
            startTime = GameTimer.inst.serverNow;
        }

        public bool Tick()
        {
            if (!isActive) return false;
            if (rate > 0)
            {
                if (tickType == GameTimerType.none) last += Time.deltaTime;
                else if (tickType == GameTimerType.byServerTime) last = (float)(GameTimer.inst.serverNow - startTime);

                if (isActive && last >= rate)
                {
                    last = 0;
                    if (tickType == GameTimerType.byServerTime) startTime = GameTimer.inst.serverNow;
                    ticksElapsed++;
                    if (ticks > 0 && ticks <= ticksElapsed)
                    {
                        isActive = false;
                        GameTimer.inst.RemoveTimer(this);
                    }
                    try
                    {
                        callBack.Invoke();
                    }
                    catch (Exception ex)
                    {

#if UNITY_EDITOR
                        throw new System.Exception(ex.Message);
#endif
                        Debug.LogError("计时器调用异常：" + ex.Message + "(查找问题参考，bug请忽略)");
                        isActive = false;
                        GameTimer.inst.RemoveTimer(this);
                    }
                    return true;
                }
            }
            else
            {
                last += 1;
                if (isActive && last > frameCount)
                {
                    last = 0;
                    ticksElapsed++;
                    if (ticks > 0 && ticks < ticksElapsed)
                    {
                        isActive = false;
                        GameTimer.inst.RemoveTimer(this);
                        return true;
                    }
                    try
                    {
                        callBack.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Logger.log("计时器调用异常：" + ex.Message, "#ff0000");
                        isActive = false;
                        GameTimer.inst.RemoveTimer(this);
                    }
                    return true;
                }
            }
            return false;
        }
    }
    private int idCounter;
    int maxeruptCount = 100;
    private List<Timer> timers;
    private List<Timer> removalPending;

    public override void init()
    {
        timers = new List<Timer>();
        removalPending = new List<Timer>();
    }

    public int AddTimer(float rate, Action callBack)
    {
        return AddTimer(rate, 0, callBack);
    }

    /// <summary>
    /// Creates new timer
    /// </summary>
    /// <param name="rate">定时器间隔</param>
    /// <param name="ticks">定时器 调用次数</param>
    /// <param name="callBack">定时器 回调方法</param>
    /// <returns>Timer GUID</returns>
    /// 
    public int AddTimer(float rate, int ticks, Action callBack, GameTimerType tickType = GameTimerType.none)
    {
        Timer newTimer = new Timer(++idCounter, rate, -1, ticks, callBack, tickType);
        timers.Add(newTimer);
        return newTimer.id;
    }

    public int AddTimerFrame(int frame, int ticks, Action callBack)
    {
        Timer newTimer = new Timer(++idCounter, -1, frame, ticks, callBack, GameTimerType.none);
        timers.Add(newTimer);
        return newTimer.id;
    }
    public Timer GetTimer(int timerId)
    {
        return timers.Find(i => i.id == timerId);
    }

    public void RemoveTimer(int timerId)
    {
        var _time = timers.Find(item => item.id == timerId);
        if (_time != null)
        {
            _time.isActive = false;
            removalPending.Add(_time);
        }
    }
    public void RemoveTimer(Timer timer)
    {
        removalPending.Add(timer);
    }
    public void clearAll()
    {
        removalPending.Clear();
        timers.Clear();
    }

    void Remove()
    {
        if (removalPending.Count > 0)
        {
            foreach (Timer time in removalPending)
            {
                timers.Remove(time);
            }
            removalPending.Clear();
        }
    }

    int currTickIndex = 0;
    void Tick()
    {
        if (currTickIndex >= timers.Count)
        {
            currTickIndex = 0;
        }
        if (timers.Count <= 0) return;

        int count = 0;
        while (count < maxeruptCount)
        {
            if (timers[currTickIndex] != null)
            {
                timers[currTickIndex].Tick();
                if (timers[currTickIndex].tickType == GameTimerType.none)
                {
                    count++;
                }
            }
            currTickIndex++;
            if (currTickIndex >= timers.Count)
            {
                return;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Remove();
        Tick();
        Remove();
    }

    //服务器时间
    double offsetItem = 0; //本地时间与服务器时间偏差
    public void setServerTime(double sed)
    {
        var localtime = TimeUtils.GetNowSeconds();
        offsetItem = localtime - sed;
    }

    public double serverNow     //当前服务器的时间(秒)
    {
        get
        {
            return TimeUtils.GetNowSeconds() - offsetItem;
        }
    }


    //========================================================================================
    public LoopEventcomp AddLoopTimerComp(GameObject target, float distance, System.Action callfun, int loop = -1)
    {
        if (target != null)
        {
            var loopEvent = target.GetComponent<LoopEventcomp>() ?? target.AddComponent<LoopEventcomp>();
            loopEvent.distance = distance;
            loopEvent._event = callfun;
            loopEvent._time = 0;
            loopEvent.loop = loop;
            loopEvent._loopcount = 0;
            return loopEvent;
        }
        return null;
    }

    public void removeLoopTimer(LoopEventcomp comp)
    {
        if (comp != null)
        {
            Destroy(comp);
        }
    }
}
