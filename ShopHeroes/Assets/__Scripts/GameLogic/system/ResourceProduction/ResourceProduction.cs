using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//测试

public class ResourceProduction
{
    int timerId;

    public bool isActivate;
    public int resItemId;     //生产资源id
    public double duration;    //生产一个用时
    public double lastCollectTime; //资源最后一个刷新时间
    public double nextCollectTime;
    public double time        //当前时间
    {
        get
        {
            if (lastCollectTime <= 0) return 0;
            return GameTimer.inst.serverNow - lastCollectTime;
        }

        set
        {
            if (value == 0)
            {
                lastCollectTime = GameTimer.inst.serverNow;
            }
            else
            {
                lastCollectTime = lastCollectTime + duration + value;
            }
        }
    }
    public double countLimit;  //资源数量上限

    double _nextUnionBuyDateTime;      //联盟币购买冷却时间 结束时间点()
    public int unionBuyCountdownTime//公会币购买倒计时
    {
        get
        {
            return (int)(_nextUnionBuyDateTime - GameTimer.inst.serverNow);
        }
        set
        {
            _nextUnionBuyDateTime = GameTimer.inst.serverNow + value;
        }
    }
    int _unionCanBuyTimes;
    public int unionCanBuyTimes    //公会币可购买次数
    {
        get
        {
            if (_unionCanBuyTimes <= 0 && (_nextUnionBuyDateTime - GameTimer.inst.serverNow) < 0)
            {
                var urcfg = UnionResourceConfigManager.inst.GetConfig(resItemId);
                if (urcfg != null)
                {
                    _unionCanBuyTimes = urcfg.buy_times;
                }
            }
            return _unionCanBuyTimes;
        }
        set { _unionCanBuyTimes = value; }
    }

}

