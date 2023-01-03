using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mosframe;

public class LotteryUIView : ViewBase<LotteryUIComp>
{
    public override string viewID => ViewPrefabName.LotteryPanel;

    public override string sortingLayerName => "window";
    private kSingleType singleType = kSingleType.max;
    private kTenthType tenthType = kTenthType.max;
    bool isLottering;
    bool isRefreshing;
    private int timeId;
    bool isIdle = true;
    Tween panTween;
    Tween tween;
    float curRotateZ = -15;
    float needGemNum;

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.lottery;
        isShowResPanel = true;
        AddUIEvent();
        IdleAnimPlay();
        var itemCfg = ItemconfigManager.inst.GetConfig(140001);
        contentPane.singleItemIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
        contentPane.tenthItemIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
    }

    private void AddUIEvent()
    {
        contentPane.singleBtn.onClick.AddListener(SingleLotteryAnim);
        contentPane.tenEvenBtn.onClick.AddListener(TenthLotteryAnim);
        contentPane.refreshBtn.onClick.AddListener(PlayRefreshAnim);
        contentPane.closeBtn.onClick.AddListener(() =>
        {
            if (isRefreshing) return;
            if (isLottering) return;
            hide();
        });
        contentPane.helpBtn.ButtonClickTween(() => { HotfixBridge.inst.TriggerLuaEvent("ShowProbabilityPublic", 1); });
        contentPane.recordBtn.ButtonClickTween(() =>
        {
            if (isRefreshing) return;
            if (isLottering) return;
            EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERYRECORD_SHOWUI);
        });
    }

    // 刷新奖池数据
    public void RefreshItemData()
    {
        contentPane.singleRedPoint.SetActive(LotteryDataProxy.inst.hasSingleRedPoint);
        contentPane.refreshRedPoint.SetActive(LotteryDataProxy.inst.hasRefreshRedPoint);

        if (LotteryDataProxy.inst.singleIsNew)
        {
            LotteryDataProxy.inst.singleIsNew = false;
        }
        if (LotteryDataProxy.inst.prizeIsNew)
        {
            LotteryDataProxy.inst.prizeIsNew = false;
        }

        List<LotteryData> tempList = LotteryDataProxy.inst.GetLotteryDatas();
        RefreshBtnText();
        RefreshJackpotData(tempList);
        SetCumulativeSlider();
    }

    // 刷新抽奖按钮上的文字（方式）
    public void RefreshBtnText()
    {
        var proxy = LotteryDataProxy.inst;

        if (timeId > 0)
        {
            GameTimer.inst.RemoveTimer(timeId);
            timeId = 0;
        }

        if (proxy.prizeCoolTime > 0)
        {
            contentPane.timeText.text = LanguageManager.inst.GetValueByKey("下次重置:") + TimeUtils.timeSpan3Str((int)proxy.prizeCoolTime);
            timeId = GameTimer.inst.AddTimer(1, () =>
             {
                 contentPane.timeText.text = LanguageManager.inst.GetValueByKey("下次重置:") + TimeUtils.timeSpan3Str((int)proxy.prizeCoolTime);
                 if (proxy.prizeCoolTime <= 0)
                 {
                     var _addata = RewardedVideoDataProxy.inst.GetAdData("2");
                     if (_addata.isVip && _addata.vipAwardCount > 0)
                     {
                         contentPane.timeText.text = LanguageManager.inst.GetValueByKey("免费重置");
                         GameTimer.inst.RemoveTimer(timeId);
                         timeId = 0;
                     }
                     else
                     {
                         contentPane.timeText.text = LanguageManager.inst.GetValueByKey("重置转盘");
                         GameTimer.inst.RemoveTimer(timeId);
                         timeId = 0;
                     }
                 }
             });
        }
        var adrewardeddata = RewardedVideoDataProxy.inst.GetAdData("2");
        if ((adrewardeddata.isVip && adrewardeddata.vipAwardCount > 0) || (!adrewardeddata.isVip && adrewardeddata.useableCount > 0))
        {
            LotteryDataProxy.inst.prizePoolState = 1;
        }
        else
        {
            LotteryDataProxy.inst.prizePoolState = 0;
        }

        contentPane.payText.gameObject.SetActive(LotteryDataProxy.inst.prizePoolState == 0);
        contentPane.freeRefreshObj.SetActive(LotteryDataProxy.inst.prizePoolState != 0);

        contentPane.payText.text = LotteryDataProxy.inst.gemConsume.ToString();
        contentPane.payText.color = LotteryDataProxy.inst.gemConsume > UserDataProxy.inst.playerData.gem ? Color.red : Color.white;
        double luckTurntableNum = ItemBagProxy.inst.GetItem(140001).count;
        SetSingleText(luckTurntableNum);
        SetTenthText(luckTurntableNum);
    }

    private void SetSingleText(double itemNum)
    {
        var _addata = RewardedVideoDataProxy.inst.GetAdData("1");
        if ((_addata.isVip && _addata.vipAwardCount > 0) || (!_addata.isVip && _addata.useableCount > 0))
        {
            singleType = kSingleType.Free;
        }
        else if (itemNum >= 1)
        {
            singleType = kSingleType.UseItem;
            contentPane.singleItemNumText.text = itemNum + "/1";
            contentPane.singleItemNumText.color = itemNum >= 1 ? Color.white : Color.red;
        }
        else
        {
            singleType = kSingleType.UseGem;
            var needNum = WorldParConfigManager.inst.GetConfig(111).parameters;
            contentPane.singleGemNumText.text = needNum.ToString();
            contentPane.singleGemNumText.color = UserDataProxy.inst.playerData.gem >= needNum ? Color.white : Color.red;
        }
        contentPane.singleFree.SetActive(singleType == kSingleType.Free);
        contentPane.singleItem.SetActive(singleType == kSingleType.UseItem);
        contentPane.singleGem.SetActive(singleType == kSingleType.UseGem);
    }

    private void SetTenthText(double itemNum)
    {
        var needItemdNum = WorldParConfigManager.inst.GetConfig(113).parameters;
        if (itemNum >= needItemdNum)
        {
            tenthType = kTenthType.UseItem;

            contentPane.tenthItemNumText.text = itemNum + "/" + needItemdNum;
            contentPane.tenthItemNumText.color = itemNum >= needItemdNum ? Color.white : Color.red;
        }
        else
        {
            tenthType = kTenthType.UseGem;
            needGemNum = WorldParConfigManager.inst.GetConfig(112).parameters;
            var playerData = UserDataProxy.inst.playerData;
            if ((K_Vip_State)playerData.vipState == K_Vip_State.Vip)
            {
                contentPane.vipObj.SetActive(true);
                int typeState = VipLevelConfigManager.inst.GetValByLevelAndType(playerData.vipLevel, K_Vip_Type.LotteryTenthPriceReduce);
                needGemNum = typeState != 0 ? needGemNum * (1 - 0.1f) : needGemNum;
            }
            else
            {
                contentPane.vipObj.SetActive(false);
            }
            contentPane.tenthGemNumText.text = needGemNum.ToString();
            contentPane.tenthGemNumText.color = UserDataProxy.inst.playerData.gem >= needGemNum ? Color.white : Color.red;
        }

        contentPane.tenthItem.SetActive(tenthType == kTenthType.UseItem);
        contentPane.tenthGem.SetActive(tenthType == kTenthType.UseGem);
    }

    // 奖池数据
    private void RefreshJackpotData(List<LotteryData> tempList)
    {
        for (int i = 0; i < contentPane.awards.Count; i++)
        {
            int index = i;
            LotteryData tempData = LotteryDataProxy.inst.GetCorrespodingBoxNumData(index);
            if (tempData != null)
            {
                contentPane.awards[index].InitData(tempData);
            }
        }
    }

    // 刷新进度条
    public void SetCumulativeSlider()
    {
        CumulativeRewardData tempData = LotteryDataProxy.inst.curCumulativeData;

        if (tempData == null) return;

        contentPane.cumulativeSlider.maxValue = tempData.cumulative_times;
        contentPane.cumulativeSlider.value = Mathf.Max(tempData.cumulative_times * 0.1f, LotteryDataProxy.inst.LotteryCount);
        contentPane.firstStageText.text = (LotteryDataProxy.inst.LotteryCount > tempData.cumulative_times ? tempData.cumulative_times : LotteryDataProxy.inst.LotteryCount) + "/" + tempData.cumulative_times;

        RefreshCumulativeAwardsData();
    }

    public void RefreshCumulativeData(bool isChangeNext)
    {
        CumulativeRewardData tempData = LotteryDataProxy.inst.curCumulativeData;

        if (tempData == null) return;
        if (!isChangeNext)
        {
            contentPane.cumulativeSlider.maxValue = tempData.cumulative_times;
            contentPane.cumulativeSlider.DOValue(LotteryDataProxy.inst.LotteryCount, 0.7f).SetEase(Ease.OutCubic).OnUpdate(() =>
            {
                contentPane.firstStageText.text = (int)contentPane.cumulativeSlider.value + "/" + tempData.cumulative_times;
            }).SetDelay(0.3f).OnComplete(() =>
            {
                if (LotteryDataProxy.inst.waitWorkerId != 0)
                {
                    var itemCfg = ItemconfigManager.inst.GetConfig(LotteryDataProxy.inst.waitWorkerId);
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutWorker { type = ReceiveInfoUIType.UnlockWorker, workerId = itemCfg.effect });
                    LotteryDataProxy.inst.waitWorkerId = 0;
                }
                isLottering = false;
            });
        }
        else
        {
            contentPane.cumulativeSlider.DOValue(contentPane.cumulativeSlider.maxValue, 0.6f).SetEase(Ease.OutCubic).OnUpdate(() =>
            {
                contentPane.firstStageText.text = (int)contentPane.cumulativeSlider.value + "/" + contentPane.cumulativeSlider.maxValue;
            }).OnComplete(() =>
            {
                contentPane.cumulativeSlider.maxValue = tempData.cumulative_times;
                contentPane.cumulativeSlider.value = 0;
                contentPane.cumulativeSlider.DOValue(LotteryDataProxy.inst.LotteryCount, 0.6f).SetEase(Ease.OutCubic).OnUpdate(() =>
                {
                    contentPane.firstStageText.text = (int)contentPane.cumulativeSlider.value + "/" + tempData.cumulative_times;
                    EventController.inst.TriggerEvent(GameEventType.LotteryEvent.CUMULATIVE_GET, LotteryDataProxy.inst.tempList);
                    LotteryDataProxy.inst.tempList.Clear();
                    if (LotteryDataProxy.inst.waitWorkerId != 0)
                    {
                        var itemCfg = ItemconfigManager.inst.GetConfig(LotteryDataProxy.inst.waitWorkerId);
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutWorker { type = ReceiveInfoUIType.UnlockWorker, workerId = itemCfg.effect });
                        LotteryDataProxy.inst.waitWorkerId = 0;
                    }
                    isLottering = false;
                });
            }).SetDelay(0.3f);
        }

        RefreshCumulativeAwardsData();
    }

    // 刷新累计抽取奖品数据
    public void RefreshCumulativeAwardsData()
    {
        contentPane.cumulative.InitData(LotteryDataProxy.inst.curCumulativeData);
    }

    private void SingleLotteryAnim()
    {
        if (isRefreshing) return;
        if (isLottering) return;

        //判断是否有转盘广告次数
        var _addata = RewardedVideoDataProxy.inst.GetAdData("1");
        if (_addata != null)
        {
            if (_addata.isVip && _addata.vipAwardCount > 0) //如果是vip
            {
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY, 3);
                return;
            }
            else
            {
                if (_addata.useableCount > 0)
                {
                    var _needNum = WorldParConfigManager.inst.GetConfig(111).parameters;
                    //打开广告确认界面
                    EventController.inst.TriggerEvent(GameEventType.GameAdEvent.GAMEAD_SHOWADVIEW, new admsgboxInfo()
                    {
                        title = LanguageManager.inst.GetValueByKey("转盘"),
                        msg = LanguageManager.inst.GetValueByKey("选择不同的方式转动一次转盘！"),
                        adtype = 1,
                        resid = ItemBagProxy.inst.GetItem(140001).count < 1 ? StaticConstants.gemID : 140001,
                        rescount = ItemBagProxy.inst.GetItem(140001).count < 1 ? (int)_needNum : 1,
                        lookAdCall = () => { EventController.inst.TriggerEvent(GameEventType.GameAdEvent.GAMEAD_START, 1); },
                        useResCall = () =>
                        {
                            lottering();
                        }
                    });
                    return;
                }
            }
        }

        lottering();
    }

    private void lottering()
    {
        var needNum = WorldParConfigManager.inst.GetConfig(111).parameters;
        if (singleType == kSingleType.UseGem && UserDataProxy.inst.playerData.gem < needNum)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("#FF2828"));
            return;
        }
        if (singleType == kSingleType.UseItem && ItemBagProxy.inst.GetItem(140001).count < 1)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("抽奖道具不足"), GUIHelper.GetColorByColorHex("#FF2828"));
            return;
        }

        contentPane.singleRedPoint.SetActive(LotteryDataProxy.inst.hasSingleRedPoint);
        switch (singleType)
        {
            case kSingleType.Free:
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY, 0);
                isLottering = true;
                break;
            case kSingleType.UseItem:
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY, 1);
                isLottering = true;
                break;
            case kSingleType.UseGem:
                if (contentPane.singleConfigImg.enabled)
                {
                    contentPane.singleConfigImg.enabled = false;
                    EventController.inst.TriggerEvent(GameEventType.LotteryEvent.SINGLE_LOTTERY, 2);
                    isLottering = true;
                }
                else
                {
                    contentPane.singleConfigImg.enabled = true;
                }
                break;
            default:
                break;
        }
    }
    // 单抽网络消息回调触发
    public void GetSingleLotteryData(List<JackpotData> data, int type)
    {
        panTween.Kill();
        isIdle = false;
        GetLotteryDataAndStartAnim(data, type);
        //RefreshBtnText();
    }

    private void GetLotteryDataAndStartAnim(List<JackpotData> data, int type)
    {
        SetBtnActive(false, 0);
        PlatLotteryAnim(data, type);
    }

    private void PlatLotteryAnim(List<JackpotData> getData, int type)
    {
        RefreshBtnText();
        var topview = GUIManager.GetWindow<TopPlayerInfoView>();
        if (topview != null)
        {
            topview.UpdateShow();
        }
        float randomOffset = Random.Range(3, 16);
        AudioManager.inst.PlaySound(108);
        contentPane.panTrans.DOLocalRotate(new Vector3(0, 0, -1080), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.InCirc).SetUpdate(UpdateType.Normal).OnComplete(() =>
        {
            contentPane.panTrans.DOLocalRotate(new Vector3(0, 0, -1080 - (15 + 30 * getData[getData.Count - 1].boxNum + randomOffset)), 4.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).SetUpdate(UpdateType.Normal).OnComplete(() =>
             {
                 contentPane.panTrans.DOLocalRotate(new Vector3(0, 0, contentPane.panTrans.localEulerAngles.z + randomOffset), 1).SetEase(Ease.InCubic).SetUpdate(UpdateType.Normal).OnComplete(() =>
                 {
                     AudioManager.inst.PlaySound(107);
                     for (int i = 0; i < getData.Count; i++)
                     {
                         int index = i;
                         if (index < getData.Count - 1)
                             contentPane.awards[getData[index].boxNum].SetHighLight();
                         else
                             contentPane.awards[getData[index].boxNum].SetHighLight(() =>
                             {
                                 LotteryComAnim(getData, type);
                             });
                     }
                 });
             });
        });
    }

    private void TenthLotteryAnim()
    {
        if (isRefreshing) return;
        if (isLottering) return;
        if (tenthType == kTenthType.UseGem && UserDataProxy.inst.playerData.gem < needGemNum)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("#FF2828"));
            return;
        }
        if (tenthType == kTenthType.UseItem && ItemBagProxy.inst.GetItem(140001).count < 9)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("抽奖道具不足"), GUIHelper.GetColorByColorHex("#FF2828"));
            return;
        }

        switch (tenthType)
        {
            case kTenthType.UseItem:
                EventController.inst.TriggerEvent(GameEventType.LotteryEvent.TENTH_LOTTERY, 1);
                isLottering = true;
                break;
            case kTenthType.UseGem:
                if (contentPane.tenConfigImg.enabled)
                {
                    contentPane.tenConfigImg.enabled = false;
                    EventController.inst.TriggerEvent(GameEventType.LotteryEvent.TENTH_LOTTERY, 2);
                    isLottering = true;
                }
                else
                {
                    contentPane.tenConfigImg.enabled = true;
                }
                break;
            default:
                break;
        }
    }

    private void SetBtnActive(bool isTrue, int activeType)
    {
        if (activeType == 0)
            contentPane.refreshBtn.enabled = (isTrue);

        contentPane.singleBtn.enabled = (isTrue);
        contentPane.tenEvenBtn.enabled = (isTrue);
    }

    private void LotteryComAnim(List<JackpotData> listData, int type)
    {
        if (listData == null || listData.Count <= 0) return;
        //isLottering = false;
        SetBtnActive(true, 0);
        if (type == 0)
        {
            var firstData = listData[0];
            CommonRewardData commonData = new CommonRewardData(firstData.prizeId, firstData.prizeCount, firstData.rarity, firstData.type);
            EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERYSINGLEREWARD_SHOWUI, commonData);
        }
        else
        {
            List<CommonRewardData> rewardList = new List<CommonRewardData>();
            for (int i = 0; i < listData.Count; i++)
            {
                int index = i;
                CommonRewardData tempData = new CommonRewardData(listData[index].prizeId, listData[index].prizeCount, listData[index].rarity, listData[index].type);
                rewardList.Add(tempData);
            }
            EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERYREWARD_SHOWUI, rewardList);
        }
        curRotateZ = contentPane.panTrans.localEulerAngles.z;
        List<LotteryData> tempList = LotteryDataProxy.inst.GetLotteryDatas();
        RefreshJackpotData(tempList);
        IdleAnimPlay();
    }

    private void IdleAnimPlay()
    {
        isIdle = true;
        tween = contentPane.panTrans.DOLocalRotate(new Vector3(0, 0, curRotateZ + 15), 5).OnComplete(() =>
        {
            if (isIdle)
            {
                panTween = contentPane.panTrans.DOLocalRotate(new Vector3(0, 0, curRotateZ - 15), 6.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }).SetEase(Ease.OutSine);
    }

    private void PlayRefreshAnim()
    {
        if (isRefreshing) return;
        if (isLottering) return;
        //判断是否有广告重置次数
        var _addata = RewardedVideoDataProxy.inst.GetAdData("2");
        if (_addata != null)
        {
            if (_addata.isVip && _addata.vipAwardCount > 0) //如果是vip
            {
                RefreshLottery(3);
                return;
            }
            else
            {
                if (_addata.useableCount > 0)
                {
                    var _needNum = LotteryDataProxy.inst.gemConsume;
                    //打开广告确认界面
                    EventController.inst.TriggerEvent(GameEventType.GameAdEvent.GAMEAD_SHOWADVIEW, new admsgboxInfo()
                    {
                        title = LanguageManager.inst.GetValueByKey("重置"),
                        msg = LanguageManager.inst.GetValueByKey("选择不同的方式重置一次转盘!"),
                        adtype = 1,
                        resid = StaticConstants.gemID,
                        rescount = (int)_needNum,
                        lookAdCall = () => { EventController.inst.TriggerEvent(GameEventType.GameAdEvent.GAMEAD_START, 2); },
                        useResCall = () =>
                        {
                            RefreshLottery(2);
                        }
                    });
                    return;
                }
            }
        }
        RefreshLottery(2);
    }
    int currRefreshType = 0;
    public void RefreshLottery(int type)
    {
        currRefreshType = type;
        FGUI.inst.showGlobalMask(1f);
        if ((currRefreshType == 2) && LotteryDataProxy.inst.gemConsume > UserDataProxy.inst.playerData.gem)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }
        var _addata = RewardedVideoDataProxy.inst.GetAdData("2");

        if (currRefreshType == 2 && (_addata != null && _addata.useableCount <= 0))
        {
            if (!contentPane.refreshConfigImg.enabled)
            {
                contentPane.refreshConfigImg.enabled = true;
                return;
            }
        }
        contentPane.refreshConfigImg.enabled = false;

        contentPane.refreshRedPoint.SetActive(LotteryDataProxy.inst.hasRefreshRedPoint);
        isRefreshing = true;
        SetBtnActive(false, 1);
        panTween.Kill();
        tween.Kill();
        isIdle = false;
        contentPane.panAnimator.enabled = true;

        contentPane.panAnimator.CrossFade("up", 0f);
        contentPane.panAnimator.Update(0f);
        contentPane.panAnimator.Play("up");
        AudioManager.inst.PlaySound(105);

        GameTimer.inst.AddTimer(contentPane.panAnimator.GetClipLength("lottery_up") + 0.3f, 1, () =>
          {
              EventController.inst.TriggerEvent(GameEventType.LotteryEvent.JACKPOT_REFRESH, (currRefreshType != 3 && currRefreshType != 4) ? LotteryDataProxy.inst.gemConsume : 0, currRefreshType);
              contentPane.panAnimator.CrossFade("down", 0f);
              contentPane.panAnimator.Update(0f);
              contentPane.panAnimator.Play("down");
              GameTimer.inst.AddTimer(contentPane.panAnimator.GetClipLength("lottery_down"), 1, () =>
              {
                  SetBtnActive(true, 1);
                  isRefreshing = false;
                  contentPane.backBg.gameObject.SetActive(true);
                  curRotateZ = contentPane.panTrans.localEulerAngles.z;
                  contentPane.panAnimator.enabled = false;
                  IdleAnimPlay();
                  contentPane.backBg.DOFade(1, 0.5f).OnComplete(() =>
                  {
                      contentPane.backBg.DOFade(0, 1).OnComplete(() =>
                      {
                          contentPane.backBg.gameObject.SetActive(false);
                      });
                  });
                  contentPane.refreshText.DOFade(1, 0.5f).OnComplete(() =>
                  {
                      contentPane.refreshText.DOFade(0, 1);
                  });
              });
          });
    }
    protected override void onShown()
    {

        if (panTween != null)
            panTween.Play();
    }

    protected override void onHide()
    {

        panTween.Pause();
        var topview = GUIManager.GetWindow<TopPlayerInfoView>();
        if (topview != null)
        {
            topview.UpdateShow();
        }

        if (timeId > 0)
        {
            GameTimer.inst.RemoveTimer(timeId);
            timeId = 0;
        }
    }
}
