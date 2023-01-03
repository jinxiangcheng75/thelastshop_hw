using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class MarketUIView : ViewBase<MarketUIComp>
{

    public override string viewID => ViewPrefabName.MarketUI;

    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;



        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.buyMarketBtn.ButtonClickTween(() => { EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETTRADINGHALLUI, kMarketTradingHallType.selfBuy); });
        contentPane.sellMarketBtn.ButtonClickTween(() => { EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETTRADINGHALLUI, kMarketTradingHallType.selfSell); });

    }

    protected override void onShown()
    {
        base.onShown();
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETBOOTH_REQUEST_DATA);
    }

    protected override void onHide()
    {
        base.onHide();

        if (_boothList != null && _boothList.Count > 0)
        {
            for (int i = 0; i < _boothList.Count; i++)
            {
                _boothList[i].redPoint = false;
            }
        }
        MarketDataProxy.inst.redPointShow = false;

    }

    List<BoothItem> _boothList;
    //您的列表
    public void RefreshMyList(int boothNum, List<BoothItem> boothList)
    {
        _boothList = boothList;

        for (int i = 0; i < contentPane.boothItems.Count; i++)
        {
            if (i < boothNum)
            {
                if (i < boothList.Count)
                {
                    contentPane.boothItems[i].SetData(boothList[i]);
                }
                else
                {
                    contentPane.boothItems[i].SetState(kBoothStateType.OK);
                }
            }
            else if (i == boothNum)
            {
                contentPane.boothItems[i].SetState(kBoothStateType.Extension);
            }
            else
            {
                contentPane.boothItems[i].SetState(kBoothStateType.Lock);
            }

        }
    }

    public void AddBoothNumCallBack(int boothNum)
    {
        contentPane.boothItems[boothNum - 1].SetState(kBoothStateType.OK);

        if (boothNum > contentPane.boothItems.Count - 1) return;
        contentPane.boothItems[boothNum].SetState(kBoothStateType.Extension);
    }

}
