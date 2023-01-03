using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdRewardedVideoSystem : BaseSystem
{
    private bool RewardedVideoPlaying = false;
    protected override void AddListeners()
    {
        EventController.inst.AddListener<Int32>(GameEventType.GameAdEvent.GAMEAD_START, RequestAdOrder);
        EventController.inst.AddListener<string>(AdEvent.RewardedVideoAdClosed, OnGameAdVideoClose);
        EventController.inst.AddListener<string>(AdEvent.RewardedVideoAdRewarded, OnGameAdVideoRewarded);
        EventController.inst.AddListener<admsgboxInfo>(GameEventType.GameAdEvent.GAMEAD_SHOWADVIEW, showAdSeletView);
        Helper.AddNetworkRespListener(MsgType.Response_AD_Start_Cmd, OnAdorderRspd);
        Helper.AddNetworkRespListener(MsgType.Response_AD_End_Cmd, OnAdEndRspd);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<Int32>(GameEventType.GameAdEvent.GAMEAD_START, RequestAdOrder);
        EventController.inst.RemoveListener<string>(AdEvent.RewardedVideoAdClosed, OnGameAdVideoClose);
        EventController.inst.RemoveListener<string>(AdEvent.RewardedVideoAdRewarded, OnGameAdVideoRewarded);
        EventController.inst.RemoveListener<admsgboxInfo>(GameEventType.GameAdEvent.GAMEAD_SHOWADVIEW, showAdSeletView);
    }

    int currAdType = 0;
    string currAdid = "";
    bool isRewarded = false;
    bool isVideoClosed = false;

    //请求看广告订单
    private void RequestAdOrder(Int32 _adtype)
    {
        if (RewardedVideoPlaying) return;

        //判断广告是否准备好
        if (!PlatformManager.inst.IsRewardedVideoAvailable())
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("广告准备中......"), GUIHelper.GetColorByColorHex("#FF2828"));
            return;
        }
        currAdid = "";
        isRewarded = false;
        isVideoClosed = false;
        currAdType = _adtype;
        var sysAdData = RewardedVideoDataProxy.inst.GetAdData(_adtype.ToString());
        if (sysAdData != null)
        {
            if (sysAdData.useableCount > 0)
            {
                sysAdData.useableCount--;
            }
        }
        else
        {
            if (_adtype != (int)EAdType.DailyBox)
            {
                Logger.error("RequestAdOrder , msg:获取不到对应系统的广告数据! 广告类型: " + _adtype);
            }
        }

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_AD_Start()
            {
                adType = _adtype
            }
        });
    }
    private void OnAdorderRspd(HttpMsgRspdBase msg)
    {
        var data = (Response_AD_Start)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
             if (!PlatformManager.inst.IsRewardedVideoAvailable()) return;
            if (currAdType == data.adType)
            {
                RewardedVideoPlaying = true;
                currAdid = data.adId;
                isRewarded = false;
                isVideoClosed = false;
                //播放视频
                FGUI.inst.showGlobalMask(3);
                PlatformManager.inst.PlayRewardedVideo(currAdType.ToString());
            }
        }
    }

    public void showAdSeletView(admsgboxInfo uiinfo)
    {
        var adui = GUIManager.OpenView<AdSeletUIView>((view) =>
        {
            view.updateUI(uiinfo);
        });
    }
    //广告播放完毕
    private void OnGameAdVideoClose(string adType)
    {
        if (currAdType != int.Parse(adType))
        {
            isRewarded = false;
            RewardedVideoPlaying = false;
            return;
        }
        isRewarded = true;
        //
        RequestVideoRewarded();
    }
    //达到广告奖励条件
    private void OnGameAdVideoRewarded(string adType)
    {
        currAdType = int.Parse(adType);
        isVideoClosed = true;
        RequestVideoRewarded();
    }

    //请求视频奖励
    private void RequestVideoRewarded()
    {
        if (isRewarded && isVideoClosed)
        {
            //满足条件发消息
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_AD_End()
                {
                    adType = currAdType,
                    adId = currAdid
                }
            });
            isRewarded = false;
            isVideoClosed = false;
        }
        RewardedVideoPlaying = false;
    }

    private void OnAdEndRspd(HttpMsgRspdBase msg)
    {
        var data = (Response_AD_End)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            if (currAdType == data.adType)
                VideoRewardedHandle(currAdType);
            currAdType = 0;
        }
    }

    private void VideoRewardedHandle(int adtype)
    {
        switch (adtype)
        {
            case 1:  //转盘
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY, 4);   //广告转盘
                break;
            case 2: //转盘重置
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_REFRESH, 4);  //转盘重置
                break;
            case 3:
                HotfixBridge.inst.TriggerLuaEvent("RequestDailyBoxReward", 3);  //每日广告
                break;
            default:

                break;
        }
    }

}
