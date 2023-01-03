using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToSubmitMarketItemData
{
    public kMarketItemType marketType;
    public int itemType;
    public int itemId;
    public int itemQuality;
    public bool isSuper;
}

public class Request_submitBoothItemData //上架
{
    public int itemType;
    public int itemId;
    public int itemQuality;
    public int itemNum;
    public int buyOrSell;//0 报价 1 请求
    public int moneyType;//0 金币 1 钻石
    public int unitPrice;//单价
    public int timeIndex;//时间下标
}

public class Request_marketItemData //刷新单个
{
    public int buyOrSell;//0 购买大厅 1 出售大厅
    public int itemType;//0 装备 1 资源
    public int itemId;
    public int itemQuality;
}

public class Request_marketItemListData //刷新列表
{
    public int buyOrSell;//0 购买列表 1 出售列表
    public int itemType;//0 装备  1 资源  2 推荐
    public List<int> subTypes = new List<int>();//小类型集合
    public List<int> levels = new List<int>();//等阶集合
    public List<int> qualitys = new List<int>();//品质集合
    public int isHot; //0 不推荐 1为请求推荐列表
}


//交易所
public class MarketSystem : BaseSystem
{

    MarketInventoryUIView _marketInventoryUI;
    MarketUIView _marketUI;
    BoothItemInfoUI _boothItemInfoUI;
    MarketBuyBoothUI _marketBuyBoothUI;
    BoothCreateListUI _boothCreateUI;
    SoldOutBoothItemAffirmUI _soldOutBoothItemAffirmUI;
    SubmitMarketItemUI _submitMarketItemUI;
    ToMarketByBagUI _toMarketByBagUI;
    MarketTradingHallUI _marketTradingHallUI;
    MarketItemInfoUI _marketItemInfoUI;

    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener<int[], int, bool, int>(GameEventType.MarketCompEvent.SHOWUI_MARKETINVENTORYUI, showMarketInventoryUI);
        EventController.inst.AddListener(GameEventType.MarketCompEvent.SHOWUI_MARKETUI, showMarketUI);
        EventController.inst.AddListener(GameEventType.MarketCompEvent.SHOWUI_MARKETBUYBOOTHUI, showMarketBuyBoothUI);
        EventController.inst.AddListener(GameEventType.MarketCompEvent.SHOWUI_BOOTHCREATELISTUI, showBoothCreateUI);
        EventController.inst.AddListener<MarketItem, kMarketTradingHallType>(GameEventType.MarketCompEvent.SHOWUI_MARKETITEMINFOUI, showMarketItemInfoUI);
        EventController.inst.AddListener<BoothItem>(GameEventType.MarketCompEvent.SHOWUI_BOOTHITEMINFOUI, showBoothItemInfoUI);
        EventController.inst.AddListener<bool, int>(GameEventType.MarketCompEvent.BOOTHITEMINFOUI_TURNPAGE, setTurnPageBoothItemInfoUI);
        EventController.inst.AddListener<BoothItem>(GameEventType.MarketCompEvent.SHOWUI_SOLDOUTBOOTHITEMAFFIRMUI, showSoldOutBoothItemAffirmUI);
        EventController.inst.AddListener<kMarketItemType>(GameEventType.MarketCompEvent.SHOWUI_TOMARKETBYBAGUI, showToMarketByBagUI);
        EventController.inst.AddListener<ToSubmitMarketItemData>(GameEventType.MarketCompEvent.SHOWUI_SUBMITMARKETITEMUI, showSubmitMarketItemUI);
        EventController.inst.AddListener<kMarketTradingHallType>(GameEventType.MarketCompEvent.SHOWUI_MARKETTRADINGHALLUI, showMarketTradingHallUI);
        EventController.inst.AddListener<BoothItem>(GameEventType.MarketCompEvent.MARKETUI_MARKETITEMCHANGED, marketItemHasChanged);
        EventController.inst.AddListener<BoothItem>(GameEventType.MarketCompEvent.MARKETUI_MARKETITEMPASTTIME, marketItemPastTime);
        EventController.inst.AddListener(GameEventType.MarketCompEvent.MARKETUI_MARKETHALLREFRESH, marketTradingHallRefresh);
        EventController.inst.AddListener<int, int, int, bool>(GameEventType.MarketCompEvent.MARKETUI_REQUIREDITEM, showMarketItemInfoUIByRequied);


        EventController.inst.AddListener(GameEventType.MarketCompEvent.MARKET_REDPOINT_HAVENEWBOOTHDATA, hasNewBoothDataChged);



        //********************************************************************************************************************
        EventController.inst.AddListener(GameEventType.MarketCompEvent.MARKETBOOTH_REQUEST_DATA, requestMarketBoothData);
        EventController.inst.AddListener<int>(GameEventType.MarketCompEvent.MARKETBOOTH_BUYFIELD, requestBuyMarketBooth);
        EventController.inst.AddListener<Request_submitBoothItemData>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMSUBMIT, request_submitBoothItem);
        EventController.inst.AddListener<int>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMRESUBMIT, request_reSubmitBoothItem);
        EventController.inst.AddListener<int>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMDEALWITH, request_DealWithBoothItem);
        EventController.inst.AddListener<int>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMSOLDOUT, request_soldOutBoothItem);
        EventController.inst.AddListener<Request_marketItemData>(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, request_marketItem);
        EventController.inst.AddListener<Request_marketItemListData, bool>(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, request_marketItemList);
        EventController.inst.AddListener<Request_marketItemData, int, int>(GameEventType.MarketCompEvent.MARKET_MARKETITEM_DEALWITH, request_dealWithMarketItem);


        //***************************************************************************************************************
        Helper.AddNetworkRespListener(MsgType.Response_Market_BoothData_Cmd, getBoothDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Market_BoothCount_Cmd, buyBoothResp);
        Helper.AddNetworkRespListener(MsgType.Response_Market_OneItemRef_Cmd, getMarketItemResp);
        Helper.AddNetworkRespListener(MsgType.Response_Market_OneItemList_Cmd, getMarketItemListResp);
        Helper.AddNetworkRespListener(MsgType.Response_Market_BuyOrSellOne_Cmd, getDealMarketItemResp);
    }


    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener<int[], int, bool, int>(GameEventType.MarketCompEvent.SHOWUI_MARKETINVENTORYUI, showMarketInventoryUI);
        EventController.inst.RemoveListener(GameEventType.MarketCompEvent.SHOWUI_MARKETUI, showMarketUI);
        EventController.inst.RemoveListener(GameEventType.MarketCompEvent.SHOWUI_MARKETBUYBOOTHUI, showMarketBuyBoothUI);
        EventController.inst.RemoveListener(GameEventType.MarketCompEvent.SHOWUI_BOOTHCREATELISTUI, showBoothCreateUI);
        EventController.inst.RemoveListener<MarketItem, kMarketTradingHallType>(GameEventType.MarketCompEvent.SHOWUI_MARKETITEMINFOUI, showMarketItemInfoUI);
        EventController.inst.RemoveListener<BoothItem>(GameEventType.MarketCompEvent.SHOWUI_BOOTHITEMINFOUI, showBoothItemInfoUI);
        EventController.inst.RemoveListener<bool, int>(GameEventType.MarketCompEvent.BOOTHITEMINFOUI_TURNPAGE, setTurnPageBoothItemInfoUI);
        EventController.inst.RemoveListener<BoothItem>(GameEventType.MarketCompEvent.SHOWUI_SOLDOUTBOOTHITEMAFFIRMUI, showSoldOutBoothItemAffirmUI);
        EventController.inst.RemoveListener<kMarketItemType>(GameEventType.MarketCompEvent.SHOWUI_TOMARKETBYBAGUI, showToMarketByBagUI);
        EventController.inst.RemoveListener<ToSubmitMarketItemData>(GameEventType.MarketCompEvent.SHOWUI_SUBMITMARKETITEMUI, showSubmitMarketItemUI);
        EventController.inst.RemoveListener<kMarketTradingHallType>(GameEventType.MarketCompEvent.SHOWUI_MARKETTRADINGHALLUI, showMarketTradingHallUI);
        EventController.inst.RemoveListener<BoothItem>(GameEventType.MarketCompEvent.MARKETUI_MARKETITEMCHANGED, marketItemHasChanged);
        EventController.inst.RemoveListener<BoothItem>(GameEventType.MarketCompEvent.MARKETUI_MARKETITEMPASTTIME, marketItemPastTime);
        EventController.inst.RemoveListener(GameEventType.MarketCompEvent.MARKETUI_MARKETHALLREFRESH, marketTradingHallRefresh);
        EventController.inst.RemoveListener<int, int, int, bool>(GameEventType.MarketCompEvent.MARKETUI_REQUIREDITEM, showMarketItemInfoUIByRequied);


        EventController.inst.RemoveListener(GameEventType.MarketCompEvent.MARKET_REDPOINT_HAVENEWBOOTHDATA, hasNewBoothDataChged);

        //********************************************************************************************************************
        EventController.inst.RemoveListener(GameEventType.MarketCompEvent.MARKETBOOTH_REQUEST_DATA, requestMarketBoothData);
        EventController.inst.RemoveListener<int>(GameEventType.MarketCompEvent.MARKETBOOTH_BUYFIELD, requestBuyMarketBooth);
        EventController.inst.RemoveListener<Request_submitBoothItemData>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMSUBMIT, request_submitBoothItem);
        EventController.inst.RemoveListener<int>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMRESUBMIT, request_reSubmitBoothItem);
        EventController.inst.RemoveListener<int>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMDEALWITH, request_DealWithBoothItem);
        EventController.inst.RemoveListener<int>(GameEventType.MarketCompEvent.MARKET_BOOTHITEMSOLDOUT, request_soldOutBoothItem);
        EventController.inst.RemoveListener<Request_marketItemData>(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, request_marketItem);
        EventController.inst.RemoveListener<Request_marketItemListData, bool>(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, request_marketItemList);
        EventController.inst.RemoveListener<Request_marketItemData, int, int>(GameEventType.MarketCompEvent.MARKET_MARKETITEM_DEALWITH, request_dealWithMarketItem);


    }

    int boothNum;
    List<BoothItem> boothList;

    #region 购买出售大厅临时数据


    List<MarketItem> buyHallEquipList;// 购买大厅 装备缓存
    List<MarketItem> buyHallMaterialList;// 购买大厅 材料缓存
    List<MarketItem> sellHallEquipList;//出售大厅 装备缓存
    List<MarketItem> sellHallMaterialList;//出售大厅 材料缓存


    #endregion

    protected override void OnInit()
    {
        base.OnInit();

        boothList = new List<BoothItem>();

        buyHallEquipList = new List<MarketItem>();
        buyHallMaterialList = new List<MarketItem>();
        sellHallEquipList = new List<MarketItem>();
        sellHallMaterialList = new List<MarketItem>();


    }


    private List<MarketItem> getCurMarketItemList(int buyOrSell, int itemType, int[] subTypes, List<int> levels, List<int> qualitys)
    {
        List<MarketItem> result = null;

        if (buyOrSell == 0)
        {
            if (itemType == 0)
            {
                result = buyHallEquipList.FindAll((item) => subTypes.Contains(item.equipConfig.equipDrawingsConfig.sub_type) && qualitys.Contains(item.itemQuality) && levels.Contains(item.equipConfig.equipDrawingsConfig.level));
            }
            else
            {
                result = buyHallMaterialList; // 资料没有分小类型 无等阶 无品质
            }
        }
        else
        {
            if (itemType == 0)
            {
                result = sellHallEquipList.FindAll((item) => subTypes.Contains(item.equipConfig.equipDrawingsConfig.sub_type) && qualitys.Contains(item.itemQuality) && levels.Contains(item.equipConfig.equipDrawingsConfig.level));
            }
            else
            {
                result = sellHallMaterialList; // 资料没有分小类型 无等阶 无品质
            }
        }

        return result;
    }

    private void showMarketUI()
    {
        _marketUI = GUIManager.OpenView<MarketUIView>();
    }


    bool isFirstTime;

    private void marketUIRefreshMyList(int boothNum, List<BoothItem> boothItems)
    {
        if (_marketUI != null && _marketUI.isShowing)
        {
            _marketUI.RefreshMyList(boothNum, boothItems);
        }
        else //没开面板 刷新
        {
            if (!isFirstTime)
            {
                isFirstTime = true;

                bool needShowRedPoint = false;

                for (int i = 0; i < boothItems.Count; i++)
                {
                    bool showRedPoint = boothItems[i].exchangeNum > 0 || boothItems[i].remainTime <= 0;
                    if (showRedPoint) needShowRedPoint = true;
                }

                MarketDataProxy.inst.redPointShow = needShowRedPoint;
                EventController.inst.TriggerEvent(GameEventType.REFRESHMAINUIREDPOINT);
                EventController.inst.TriggerEvent(GameEventType.CityUIMediatorEvent.REFRESH_CITYUI_REDPOINT);
            }
        }
    }


    private void showMarketInventoryUI(int[] equipTypes, int maxLevel, bool canChangeLevel, int marketInventoryFromType)
    {
        //GUIManager.HideView<ShelfInventoryUIView>();
        _marketInventoryUI = GUIManager.OpenView<MarketInventoryUIView>((marketInventoryUI) =>
        {
            marketInventoryUI.GetItemLists(equipTypes, maxLevel, canChangeLevel, marketInventoryFromType);
        });
    }

    private void showMarketItemInfoUI(MarketItem marketItem, kMarketTradingHallType hallType)
    {

        _marketItemInfoUI = GUIManager.OpenView<MarketItemInfoUI>((marketItemInfoUI) =>
       {
           marketItemInfoUI.SetData(marketItem, hallType);
       });
    }

    private void showMarketItemInfoUIByRequied(int itemType, int equipOrItemId, int needNum, bool allQuality)
    {
        if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(101).parameters)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(101).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
            return;
        }


        _marketItemInfoUI = GUIManager.OpenView<MarketItemInfoUI>((marketItemInfoUI) =>
         {
             marketItemInfoUI.SetRequiredData(itemType, equipOrItemId, needNum, allQuality);
         });
    }

    private void showBoothItemInfoUI(BoothItem boothItem)
    {
        //if (_boothItemInfoUI != null && _boothItemInfoUI.isShowing)
        //{
        //    _boothItemInfoUI.SetData(boothItem, boothList.Count);
        //}
        //else
        //{
        _boothItemInfoUI = GUIManager.OpenView<BoothItemInfoUI>((boothItemInfoUI) =>
        {
            boothItemInfoUI.SetData(boothItem, boothList.Count);
        });
        //}
    }

    private void setTurnPageBoothItemInfoUI(bool isLeft, int boothField)
    {
        int index = 0;

        var list = boothList.FindAll(t => t.exchangeNum == 0);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].boothField == boothField)
            {
                index = i;
                break;
            }
        }

        index += isLeft ? -1 : 1;

        if (index == list.Count) index = 0;
        if (index == -1) index = list.Count - 1;


        showBoothItemInfoUI(list[index]);
    }

    private void hideBoothItemInfoUI()
    {
        GUIManager.HideView<BoothItemInfoUI>();
    }

    private void showSoldOutBoothItemAffirmUI(BoothItem boothItem)
    {
        _soldOutBoothItemAffirmUI = GUIManager.OpenView<SoldOutBoothItemAffirmUI>((soldOutBoothItemAffirmUI) =>
        {
            soldOutBoothItemAffirmUI.SetInfo(boothItem);
        });
    }

    private void hideSoldOutBoothItemAffirmUI()
    {
        GUIManager.HideView<SoldOutBoothItemAffirmUI>();
    }


    private void showMarketBuyBoothUI()
    {
        GUIManager.OpenView<MarketBuyBoothUI>((marketBuyBoothUI) =>
        {
            //_marketBuyBoothUI = marketBuyBoothUI;
            marketBuyBoothUI.Init(boothNum);
        });
    }


    private void hideMarketBuyBoothUI()
    {
        GUIManager.HideView<MarketBuyBoothUI>();
    }

    private void showBoothCreateUI()
    {
        GUIManager.OpenView<BoothCreateListUI>();
    }

    private void hideBoothCreateUI()
    {
        GUIManager.HideView<BoothCreateListUI>();
    }

    private void showToMarketByBagUI(kMarketItemType type) //0 报价 1 请求 2 公会买 
    {

        GUIManager.OpenView<ToMarketByBagUI>((toMarketByBagUI) =>
        {
            //_toMarketByBagUI = toMarketByBagUI;
            toMarketByBagUI.SetMarketItemType(type);
        });
    }

    private void hideToMarketByBagUI()
    {
        GUIManager.HideView<ToMarketByBagUI>();
    }

    private void showSubmitMarketItemUI(ToSubmitMarketItemData toSubmitMarketItemData) //0 装备 1 资源
    {
        _submitMarketItemUI = GUIManager.OpenView<SubmitMarketItemUI>((submitMarketItemUI) =>
        {
            submitMarketItemUI.SetData(toSubmitMarketItemData);
        });
    }

    private void hideSubmitMarketItemUI()
    {
        GUIManager.HideView<SubmitMarketItemUI>();
    }

    private void showMarketTradingHallUI(kMarketTradingHallType type)
    {
        _marketTradingHallUI = GUIManager.OpenView<MarketTradingHallUI>((marketTradingHallUI) =>
        {
            _marketTradingHallUI.SetTradingHallType(type);
        });
    }

    private void marketTradingHallRefresh()
    {
        if (_marketTradingHallUI != null && _marketTradingHallUI.isShowing)
        {
            _marketTradingHallUI.TimingRefreshMethod(true);
        }

        if (_marketInventoryUI != null && _marketInventoryUI.isShowing)
        {
            _marketInventoryUI.TimingRefreshMethod(true);
        }
    }

    private void marketItemHasChanged(BoothItem data) //有交易出去的情况
    {
        if (_soldOutBoothItemAffirmUI != null && _soldOutBoothItemAffirmUI.isShowing)
        {
            _soldOutBoothItemAffirmUI.NeedHide(data);
            _boothItemInfoUI.NeedHide(data);
        }
        else if (_boothItemInfoUI != null && _boothItemInfoUI.isShowing)
        {
            _boothItemInfoUI.NeedHide(data);
        }
    }

    private void marketItemPastTime(BoothItem data) //有过期的情况
    {
        if (_soldOutBoothItemAffirmUI != null && _soldOutBoothItemAffirmUI.isShowing)
        {
            _soldOutBoothItemAffirmUI.NeedHide(data);
            _boothItemInfoUI.NeedRefresh(data, boothList.Count);
        }
        else if (_boothItemInfoUI != null && _boothItemInfoUI.isShowing)
        {
            _boothItemInfoUI.NeedRefresh(data, boothList.Count);
        }
    }

    #region 个人摊位

    //请求摊位数据
    private void requestMarketBoothData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_BoothData()
        });
    }

    //上架单个摊位物品
    private void request_submitBoothItem(Request_submitBoothItemData data)
    {

        hideBoothCreateUI();
        hideToMarketByBagUI();
        hideSubmitMarketItemUI();
        hideBoothItemInfoUI();

        FGUI.inst.showGlobalMask(0.5f);

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_ListedItems()
            {
                listedItems = new ListedItems() { buyOrSell = data.buyOrSell, itemType = data.itemType, itemId = data.itemId, itemNum = data.itemNum, itemQuality = data.itemQuality, moneyType = data.moneyType, unitPrice = data.unitPrice, timeIndex = data.timeIndex }
            }
        });

        PlatformManager.inst.GameHandleEventLog("Trade_Shangjia", "");
    }

    //重新上架单个摊位物品
    public void request_reSubmitBoothItem(int boothField)
    {
        hideBoothItemInfoUI();

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_UpdatePut()
            {
                boothFileId = boothField
            }
        });
    }


    //处理单个摊位物品
    private void request_DealWithBoothItem(int boothField)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_DealWith
            {
                dealLocation = boothField
            }
        });


    }

    //下架单个摊位物品
    private void request_soldOutBoothItem(int boothField)
    {
        hideSoldOutBoothItemAffirmUI();
        hideBoothItemInfoUI();

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_OffShelf()
            {
                offShelf = boothField
            }
        });
    }

    private void hasNewBoothDataChged()
    {
        if (_marketUI != null && _marketUI.isShowing)
        {
            MarketDataProxy.inst.redPointShow = false;
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETBOOTH_REQUEST_DATA);
        }
        else
        {
            MarketDataProxy.inst.redPointShow = true;
            EventController.inst.TriggerEvent(GameEventType.REFRESHMAINUIREDPOINT);
            EventController.inst.TriggerEvent(GameEventType.CityUIMediatorEvent.REFRESH_CITYUI_REDPOINT);
        }
    }

    private void getBoothDataResp(HttpMsgRspdBase msg)
    {
        Response_Market_BoothData data = msg as Response_Market_BoothData;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        boothNum = data.Count;
        for (int i = 0; i < boothList.Count; i++)
        {
            boothList[i].ClearTimer();
        }
        boothList.Clear();

        for (int i = 0; i < data.saleList.Count; i++)
        {
            BoothItem item = new BoothItem();
            var info = data.saleList[i];
            item.SetInfo(info.boothFileId, info.itemType, info.itemId, info.itemQuality, info.remainNum, info.marketType, info.moneyType, info.unitPrice, info.exchangeNum, info.timeIndex, info.remainTime);
            boothList.Add(item);
        }

        marketUIRefreshMyList(boothNum, boothList);
    }

    //购买摊位
    private void requestBuyMarketBooth(int costType) // 1 gold 2 gem
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_BoothCount()
            {
                money = costType,
                slot = boothNum + 1
            }
        });

    }

    private void buyBoothResp(HttpMsgRspdBase msg)
    {
        Response_Market_BoothCount data = msg as Response_Market_BoothCount;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        boothNum = data.Count;

        hideMarketBuyBoothUI();
        if (_marketUI != null && _marketUI.isShowing)
        {
            _marketUI.AddBoothNumCallBack(boothNum);
        }
    }

    #endregion


    #region 买卖大厅

    //请求单个拍卖物品
    private void request_marketItem(Request_marketItemData data)
    {
        //todo data.isSuper
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_OneItemRef()
            {
                buyOrSell = data.buyOrSell,
                itemType = data.itemType,
                itemId = data.itemId,
                itemQuality = data.itemQuality
            }
        });
    }

    private void getMarketItemResp(HttpMsgRspdBase msg) //需要参数  i. 单个拍卖物品   ii.来自购买大厅还是出售大厅  0 购买大厅 1 出售大厅
    {
        Response_Market_OneItemRef data = msg as Response_Market_OneItemRef;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        MarketItem marketItem = new MarketItem();
        marketItem.SetInfo(data.sellOneItem.putAwayTime, data.sellOneItem.itemType, data.sellOneItem.itemId, data.sellOneItem.itemQuality, data.sellOneItem.marketNum, data.sellOneItem.goldPrice, data.sellOneItem.gemPrice);

        //摊位item详情
        if (_boothItemInfoUI != null && _boothItemInfoUI.isShowing)
            _boothItemInfoUI.timingRefreshCallBackMethod(marketItem);


        //上架单个拍卖物品的回调
        if (_submitMarketItemUI != null && _submitMarketItemUI.isShowing)
            _submitMarketItemUI.timingRefreshCallBackMethod(marketItem);

        //拍卖item详情
        if (_marketItemInfoUI != null && _marketItemInfoUI.isShowing)
        {
            _marketItemInfoUI.SetData(marketItem, kMarketTradingHallType.selfBuy);
        }

    }


    //请求多个拍卖物品
    private void request_marketItemList(Request_marketItemListData data, bool toSever)
    {
        //if (toSever) //发送网络协议
        //{
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_OneItemList()
            {
                buyOrSell = data.buyOrSell,
                itemType = data.itemType,
                subTypesList = data.subTypes.ToList(),
                levelsList = data.levels.ToList(),
                qualityList = data.qualitys.ToList(),
                findAllItem = data.isHot,
            }
        });
        //    }
        //        else
        //        {
        //            List<MarketItem> marketItemList = getCurMarketItemList(data.buyOrSell, data.itemType, data.subTypes, data.levels, data.qualitys);

        //            //交易大厅
        //            if (_marketTradingHallUI != null && _marketTradingHallUI.isShowing)
        //            {
        //                _marketTradingHallUI.RefreshSuperList(marketItemList);
        //            }

        ////货架交易面板
        //if (_marketInventoryUI != null && _marketInventoryUI.isShowing)
        //{
        //    _marketInventoryUI.RefreshSuperList(marketItemList);
        //}
        //        }

    }


    //合并去重
    private void combineHallCache(ref List<MarketItem> hallCacheList, List<MarketItem> screenedList, List<MarketItem> fromSeverList)
    {

        Dictionary<int, MarketItem> dic = new Dictionary<int, MarketItem>();

        foreach (var citem in screenedList)
        {
            dic.Add(citem.itemId * 1000 + citem.itemQuality, citem);
        }

        foreach (var sitem in fromSeverList)
        {
            if (dic.ContainsKey(sitem.itemId * 1000 + sitem.itemQuality))
            {
                if (sitem.marketNum == 0)
                {
                    hallCacheList.Remove(dic[sitem.itemId * 1000 + sitem.itemQuality]);
                }
                else
                {
                    dic[sitem.itemId * 1000 + sitem.itemQuality].SetInfo(sitem.putawayTime, sitem.itemType, sitem.itemId, sitem.itemQuality, sitem.marketNum, sitem.goldPrice, sitem.gemPrice);
                }
            }
            else
            {
                hallCacheList.Add(sitem);
            }
        }
    }

    //更新本地缓存池
    private void updateMarketHallCache(List<MarketItem> marketItemList, int hallType, int itemType, int[] subTypes = null)
    {
        List<MarketItem> tempList;

        if (hallType == 0)
        {
            if (itemType == 0)
            {
                tempList = buyHallEquipList.FindAll((item) => subTypes.Contains(item.equipConfig.equipDrawingsConfig.sub_type));
                combineHallCache(ref buyHallEquipList, tempList, marketItemList);
            }
            else
            {
                buyHallMaterialList = marketItemList;
            }
        }
        else
        {
            if (itemType == 0)
            {
                tempList = sellHallEquipList.FindAll((item) => subTypes.Contains(item.equipConfig.equipDrawingsConfig.sub_type));
                combineHallCache(ref sellHallEquipList, tempList, marketItemList);
            }
            else
            {
                sellHallMaterialList = marketItemList;
            }
        }

    }

    //获取到多个拍卖物品的回调
    private void getMarketItemListResp(HttpMsgRspdBase msg) //需要参数 i.来自购买大厅还是出售大厅  0 购买大厅 1 出售大厅   ii.物品类型  0 装备 1 资源  iii.物品分类  int[]  iv.列表  物品列表
    {
        Response_Market_OneItemList data = msg as Response_Market_OneItemList;

        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            List<MarketItem> marketItemList = new List<MarketItem>();

            foreach (var item in data.sellOneItemList)
            {
                MarketItem marketItem = new MarketItem();
                marketItem.SetInfo(item.putAwayTime, item.itemType, item.itemId, item.itemQuality, item.marketNum, item.goldPrice, item.gemPrice);
                marketItemList.Add(marketItem);
            }

            ////缓存池更新完毕后立即刷新UI
            //updateMarketHallCache(marketItemList, data.buyOrSell, data.itemType, data.itemSort.ToArray());

            ////交易大厅
            //if (_marketTradingHallUI != null && _marketTradingHallUI.isShowing)
            //    _marketTradingHallUI.TimingRefreshMethod(false);

            ////货架交易面板
            //if (_marketInventoryUI != null && _marketInventoryUI.isShowing)
            //{
            //    _marketInventoryUI.TimingRefreshMethod(false);
            //}


            ////直接拿服务器的数据进行刷新
            //交易大厅
            if (_marketTradingHallUI != null && _marketTradingHallUI.isShowing)
            {
                _marketTradingHallUI.RefreshSuperList(data.buyOrSell, data.findAllItem == 1 ? 2 : data.itemType, marketItemList, false);
            }

            //货架交易面板
            if (_marketInventoryUI != null && _marketInventoryUI.isShowing)
            {
                _marketInventoryUI.RefreshSuperList(data.buyOrSell, data.itemType, data.itemSort, marketItemList, false);
            }
        }
        else if (data.errorCode == (int)EErrorCode.EEC_TransactionNotOpen)
        {
            //交易大厅
            if (_marketTradingHallUI != null && _marketTradingHallUI.isShowing)
            {
                _marketTradingHallUI.RefreshSuperList(data.buyOrSell, data.findAllItem == 1 ? 2 : data.itemType, new List<MarketItem>(), true);
            }

            //货架交易面板
            if (_marketInventoryUI != null && _marketInventoryUI.isShowing)
            {
                _marketInventoryUI.RefreshSuperList(data.buyOrSell, data.itemType, data.itemSort, new List<MarketItem>(), true);
            }
        }

    }

    //买卖单个物品
    private void request_dealWithMarketItem(Request_marketItemData data, int moneyType, int costMoney) // 0 金币 1 钻石   //花费金额
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Market_BuyOrSellOne()
            {
                buyOrSell = data.buyOrSell,
                itemType = data.itemType,
                itemId = data.itemId,
                itemQuality = data.itemQuality,
                moneyType = moneyType,
                costMoney = costMoney,
                payload = MarketDataProxy.inst.Payload.IfNullThenEmpty(),
            }
        });

    }

    //买卖单个物品的回调
    private void getDealMarketItemResp(HttpMsgRspdBase msg) //需要参数  i.是否成功买卖到 0 失败 1 成功 ii. 单个拍卖物品   iii.来自购买大厅还是出售大厅  0 购买大厅 1 出售大厅  iv.失败原因
    {
        Response_Market_BuyOrSellOne data = msg as Response_Market_BuyOrSellOne;

        if (data.reason == 1) //成功
        {
            if (data.comeWhere == (int)kMarketTradingHallType.selfBuy) //成功买到
            {
                PlatformManager.inst.GameHandleEventLog("Trade_Buy", "");
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("购买成功"), GUIHelper.GetColorByColorHex("FFD907"));

                var equiplistUIView = GUIManager.GetWindow<EquipListUIView>();
                if (equiplistUIView != null && equiplistUIView.isShowing)
                {
                    equiplistUIView.RefreshListItemsInfo();
                }

            }
            else if (data.comeWhere == (int)kMarketTradingHallType.selfSell)  //成功卖出
            {
                PlatformManager.inst.GameHandleEventLog("Trade_Soldout", "");
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("出售成功"), GUIHelper.GetColorByColorHex("FFD907"));
            }

            //2022-8-19后增逻辑 Payload
            HotfixBridge.inst.TriggerLuaEvent("Market_BuyOrSellOneSuccessed", data.payload, data.sellOneItem.itemType, data.sellOneItem.itemId, data.sellOneItem.itemQuality);
        }

        //买卖单个拍卖物品结果 关闭也执行相关逻辑
        if (_marketItemInfoUI != null && _marketItemInfoUI.isShowing)
        {
            MarketItem marketItem = new MarketItem();
            marketItem.SetInfo(data.sellOneItem.putAwayTime, data.sellOneItem.itemType, data.sellOneItem.itemId, data.sellOneItem.itemQuality, data.sellOneItem.marketNum, data.sellOneItem.goldPrice, data.sellOneItem.gemPrice);
            _marketItemInfoUI.refreshCallBackMethod(marketItem, (kMarketTradingHallType)data.comeWhere, data.reason);
        }

        //2022-8-19后增逻辑 清空本地Payload
        MarketDataProxy.inst.Payload = string.Empty;

    }

    #endregion


}
