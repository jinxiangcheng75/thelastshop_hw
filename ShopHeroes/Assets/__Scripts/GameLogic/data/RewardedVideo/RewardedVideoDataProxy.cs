using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardedVideoAdData
{
    public class OneSysRewardedVideoAdData
    {
        public string adtype = "0";      //系统类型
        public int useableCount = 0;      //可看广告次数
        public int vipAwardCount = 0;    //可vip特权次数
        public int limitCount = 0;  //当前系统观看次数限制
        public int vipLimitCount = 0; //当前vip可使用特权次数限制

        public bool isVip
        {
            get
            {
                if (UserDataProxy.inst == null || UserDataProxy.inst.playerData == null) return false;
                return UserDataProxy.inst.playerData.isVip();
            }
        }
    }
    public Dictionary<string, OneSysRewardedVideoAdData> adDataList = new Dictionary<string, OneSysRewardedVideoAdData>();
    public void setData(string adtype, int limit, int viplimit, int count, int vipcount)
    {
        OneSysRewardedVideoAdData data;
        adDataList.TryGetValue(adtype, out data);
        if (data == null) data = new OneSysRewardedVideoAdData();

        data.limitCount = limit;
        data.vipLimitCount = viplimit;
        data.useableCount = count;
        data.vipAwardCount = vipcount;
        adDataList[adtype] = data;
    }
}
public class RewardedVideoDataProxy : TSingletonHotfix<RewardedVideoDataProxy>, IDataModelProx
{
    private RewardedVideoAdData addata;
    public void Clear()
    {
        EventController.inst.RemoveListener(GameEventType.GameAdEvent.GAMEAD_GETDATA, updateAdData);
    }
    public void Init()
    {
        addata = new RewardedVideoAdData();
        EventController.inst.AddListener(GameEventType.GameAdEvent.GAMEAD_GETDATA, updateAdData);
        Helper.AddNetworkRespListener(MsgType.Response_AD_UserData_Cmd, onAdDataUpdate); //刷新用户广告数据

    }
    public RewardedVideoAdData.OneSysRewardedVideoAdData GetAdData(string type)
    {
        if (addata.adDataList.ContainsKey(type))
        {
            return addata.adDataList[type];
        }
        return null;
    }

    //请求数据刷新
    private void updateAdData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_AD_UserData()
        });
    }
    private void onAdDataUpdate(HttpMsgRspdBase msg)
    {
        var data = (Response_AD_UserData)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            for (int i = 0; i < data.adList.Count; i++)
            {
                var _data = data.adList[i];
                addata.setData(_data.adType.ToString(), _data.adTotalCount, _data.vipTotalCount, _data.adDayRemain, _data.vipDayRemain);
            }
            EventController.inst.TriggerEvent(GameEventType.GameAdEvent.GAMEAD_UPDATEVIEW);
        }
    }
}
