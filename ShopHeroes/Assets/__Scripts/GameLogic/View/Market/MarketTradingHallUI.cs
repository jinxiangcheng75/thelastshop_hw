using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MarketTradingHallUI : ViewBase<MarketTradingHallUIComp>
{
    public override string viewID => ViewPrefabName.MarketTradingHallUI;
    public override int showType => (int)ViewShowType.pullUp;
    public override string sortingLayerName => "window";

    private int[][] subTypeGroup = new int[][] //大分类 与子分类关系
   {
        new int[]{ },
        new int[]{1,2,3,4,5,8,6,7},
        new int[]{9,10,11,12,13,14,15,16,17},
        new int[]{18,19,20,21,22,23},
        new int[]{ },//6 8 item表
   };

    //EmarketItemSortType
    private int[][] curSortType = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 } };
    private int[][][] sortTypeGroup = new int[][][] //购买大厅 出售大厅 next: 装备 资源 next: 排序类型
    {
        new int[][]{ new int[] { 0,1,2,3,4,5,6,7 },new int[] { 2,3,5,7 } },
        new int[][]{ new int[] { 0,1,2,3,4,5,6,7 },new int[] { 2,3,5,7 } },
    };

    private List<MarketSubTypeItem> subItemList;

    //当前选中大类型 小类型
    private int _itemType // 0 装备 1 资源 2 推荐
    {
        get
        {
            int itemType = -1;

            if (_bigType == 0)
            {
                itemType = 2;
            }
            else if (_bigType >= 1 && _bigType <= 3)
            {
                itemType = 0;
            }
            else if (_bigType == 4)
            {
                itemType = 1;
            }

            return itemType;
        }
    }

    private int _bigType; // 0 推荐  1 武器  2 防具  3 配件  4 其他
    private int _smallType;

    //等阶筛选
    private int curCanUseLevelMax;
    private int levelToggleIsOnCount;
    private List<int> levelScreenResult;

    //品质筛选
    private int qualityToggleIsOnCount;
    private List<int> qualityScreenResult;


    private kMarketTradingHallType tradingHallType; //交易所类型 购买大厅 出售大厅


    //定期刷新
    private int refreshTimer;
    private int maskTimer;
    private const int RefreshTime = 5;


    public override void shiftIn()
    {
    }

    public override void shiftOut()
    {

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");
        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;


        levelScreenResult = new List<int>();
        qualityScreenResult = new List<int>();

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.sortBtn.onClick.AddListener(onSortBtnClick);
        contentPane.bigToggleGroup.OnSelectedIndexValueChange = typeSelectedChange; //大分类改变

        contentPane.superList.itemRenderer = listitemRenderer;
        contentPane.superList.itemUpdateInfo = listitemRenderer;
        contentPane.superList.scrollByItemIndex(0);
        //contentPane.superList.totalItemCount = 0;

        contentPane.allTypeToggle.onValueChanged.AddListener(allTypeToggleOnValueChanged);


        subItemList = new List<MarketSubTypeItem>();
        foreach (Transform item in contentPane.subToggleGroup.transform)
        {
            MarketSubTypeItem subItem = item.GetComponent<MarketSubTypeItem>();
            subItem.Init();
            subItem.onSelectHandler = subTypeSelectedChange;
            subItemList.Add(subItem);
        }

        contentPane.allLevelToggle.onValueChanged.AddListener((isOn) =>
        {

            for (int i = 0; i < contentPane.allLevelToggle.graphic.transform.childCount; i++)
            {
                contentPane.allLevelToggle.graphic.transform.GetChild(i).gameObject.SetActive(contentPane.allLevelToggle.isOn);
            }

            if (isOn)
            {
                allLevelToggleIsOn(false);
            }
            else
            {
                contentPane.allLevelToggle.isOn = levelToggleIsOnCount == 0;
            }
        });

        contentPane.levelScreenBtn.onClick.AddListener(() => { contentPane.levelScreenObj.SetActive(true); });
        contentPane.levelScreenObjBtn.onClick.AddListener(onLevelScreenApplyBtnClick);
        contentPane.levelScreenCancelBtn.onClick.AddListener(onLevelScreenCancelBtnClick);
        contentPane.levelScreenApplyBtn.onClick.AddListener(onLevelScreenApplyBtnClick);

        for (int i = 0; i < contentPane.levelScreenToggles.Length; i++)
        {
            int index = i;
            Toggle toggle = contentPane.levelScreenToggles[index];
            toggle.onValueChanged.AddListener((isOn) =>
            {

                for (int k = 0; k < toggle.graphic.transform.childCount; k++)
                {
                    toggle.graphic.transform.GetChild(k).gameObject.SetActive(toggle.isOn);
                }

                if (!isOn)
                {
                    levelToggleIsOnCount -= 1;
                    contentPane.allLevelToggle.isOn = levelToggleIsOnCount == 0;
                    //contentPane.levelScreenToggles[index].isOn = true;
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, "至少保留一个选项", GUIHelper.GetColorByColorHex("FF2828"));
                }
                else
                {
                    levelToggleIsOnCount += 1;
                    if (MarketEquipLvManager.inst.GetCurMarketLevel() == 1)
                    {
                        contentPane.levelScreenToggles[index].isOn = false;
                        return;
                    }
                    contentPane.allLevelToggle.isOn = MarketEquipLvManager.inst.GetCurMarketLevel() == levelToggleIsOnCount;
                }

            });
        }

        contentPane.qualityScreenBtn.onClick.AddListener(() => { contentPane.qualityScreenObj.SetActive(true); });
        contentPane.qualityScreenObjBtn.onClick.AddListener(onQualityScreenApplyBtnClick);
        contentPane.qualityScreenCancelBtn.onClick.AddListener(onQualityScreenCancelBtnClick);
        contentPane.qualityScreenApplyBtn.onClick.AddListener(onQualityScreenApplyBtnClick);

        contentPane.allQualityToggle.onValueChanged.AddListener((isOn) =>
        {

            for (int i = 0; i < contentPane.allQualityToggle.graphic.transform.childCount; i++)
            {
                contentPane.allQualityToggle.graphic.transform.GetChild(i).gameObject.SetActive(contentPane.allQualityToggle.isOn);
            }

            if (isOn)
            {
                allQualityToggleIsOn(false);
            }
            else
            {
                contentPane.allQualityToggle.isOn = qualityToggleIsOnCount == 0;
            }
        });

        for (int i = 0; i < contentPane.qualityScreenToggles.Length; i++)
        {
            int index = i;
            Toggle toggle = contentPane.qualityScreenToggles[index];
            toggle.onValueChanged.AddListener((isOn) =>
            {

                for (int k = 0; k < toggle.graphic.transform.childCount; k++)
                {
                    toggle.graphic.transform.GetChild(k).gameObject.SetActive(toggle.isOn);
                }

                if (!isOn)
                {
                    qualityToggleIsOnCount -= 1;
                    contentPane.allQualityToggle.isOn = qualityToggleIsOnCount == 0;
                    //contentPane.qualityScreenToggles[index].isOn = true;
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, "至少保留一个选项", GUIHelper.GetColorByColorHex("FF2828"));
                }
                else
                {
                    qualityToggleIsOnCount += 1;
                    contentPane.allQualityToggle.isOn = qualityToggleIsOnCount == contentPane.qualityScreenToggles.Length;
                }

            });
        }


    }

    private void allLevelToggleIsOn(bool isCover)
    {
        curCanUseLevelMax = MarketEquipLvManager.inst.GetCurMarketLevel();
        if (isCover) levelScreenResult.Clear();

        for (int i = 0; i < contentPane.levelScreenToggles.Length; i++)
        {
            if (i < curCanUseLevelMax)
            {
                contentPane.levelScreenToggles[i].isOn = false;
                contentPane.levelScreenToggles[i].interactable = true;
                GUIHelper.SetUIGray(contentPane.levelScreenToggles[i].transform, false);
                if (isCover) levelScreenResult.Add(i + 1);
            }
            else
            {
                contentPane.levelScreenToggles[i].isOn = false;
                contentPane.levelScreenToggles[i].interactable = false;
                GUIHelper.SetUIGray(contentPane.levelScreenToggles[i].transform, true);
            }
        }
    }

    private void allQualityToggleIsOn(bool isCover)
    {
        if (isCover) qualityScreenResult.Clear();
        for (int i = 0; i < contentPane.qualityScreenToggles.Length; i++)
        {
            contentPane.qualityScreenToggles[i].isOn = false;
            if (isCover)
            {
                qualityScreenResult.Add(i + 1); //普通的
                qualityScreenResult.Add(StaticConstants.SuperEquipBaseQuality + i + 1); //超凡的
            }
        }
    }

    public void SetTradingHallType(kMarketTradingHallType type)
    {
        tradingHallType = type;

        contentPane.title_1.text = LanguageManager.inst.GetValueByKey(tradingHallType == kMarketTradingHallType.selfBuy ? "购买物品" : "卖出物品");
        contentPane.title_1.color = tradingHallType == kMarketTradingHallType.selfBuy ? GUIHelper.GetColorByColorHex("#78f452") : GUIHelper.GetColorByColorHex("#37e7fd");
        contentPane.title_2.text = LanguageManager.inst.GetValueByKey(tradingHallType == kMarketTradingHallType.selfBuy ? "最近报价" : "最近请求");

        contentPane.topBgIcon.SetSprite("market_atlas_newAdd", tradingHallType == kMarketTradingHallType.selfBuy ? "shichang_dinglv" : "shichang_dinglan");

        contentPane.titleTypeIcon.SetSprite("market_atlas", tradingHallType == kMarketTradingHallType.selfBuy ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
        contentPane.titleTypeIcon.transform.localScale = new Vector3(tradingHallType == kMarketTradingHallType.selfBuy ? -1 : 1, 1, 1);

        //等阶筛选
        contentPane.allLevelToggle.isOn = true;
        allLevelToggleIsOn(true);


        //品质筛选
        contentPane.allQualityToggle.isOn = true;
        allQualityToggleIsOn(true);


        contentPane.bigToggleGroup.OnEnableMethod();

        refreshTimer = GameTimer.inst.AddTimer(RefreshTime, () => TimingRefreshMethod(false));
    }

    protected override void onHide()
    {
        base.onHide();
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETUI);

        GameTimer.inst.RemoveTimer(maskTimer);
        maskTimer = 0;
        GameTimer.inst.RemoveTimer(refreshTimer);
        refreshTimer = 0;
    }

    private void onSortBtnClick()
    {
        int subIndex = _itemType != 1 ? 0 : 1;
        curSortType[(int)tradingHallType][subIndex] += 1;
        if (curSortType[(int)tradingHallType][subIndex] >= sortTypeGroup[(int)tradingHallType][subIndex].Length)
        {
            curSortType[(int)tradingHallType][subIndex] = 0;
        }

        contentPane.sortText.text = MarketDataProxy.GetSortText((EmarketItemSortType)sortTypeGroup[(int)tradingHallType][subIndex][curSortType[(int)tradingHallType][subIndex]]);
        MarketDataProxy.MarketItemListSort(ref curMarketList, (EmarketItemSortType)sortTypeGroup[(int)tradingHallType][subIndex][curSortType[(int)tradingHallType][subIndex]]);

        contentPane.superList.refresh();
    }

    //大类型选中
    private void typeSelectedChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        _bigType = index;

        //Logger.error("bigType: " + _bigType + "   _itemType : " + _itemType);

        int[] smallTypes = subTypeGroup[index];

        //contentPane.allTypeToggle.gameObject.SetActive(isEquip);

        for (int i = 0; i < subItemList.Count; i++)
        {
            subItemList[i].gameObject.SetActive(false);
        }

        switch (_itemType)
        {
            case 0: //装备
                for (int i = 0; i < smallTypes.Length; i++)
                {
                    EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(smallTypes[i]);
                    MarketSubTypeItem item = subItemList[i];
                    item.gameObject.SetActive(true);
                    item.bigType = _bigType;
                    item.smallType = classcfg.id;
                    item.selectdIcon.SetSprite(classcfg.Atlas, classcfg.icon);
                    string[] arr = classcfg.icon.Split('2');
                    item.unSelectIcon.SetSprite(classcfg.Atlas, arr[0] + "1");
                }

                contentPane.levelScreenBtn.gameObject.SetActive(true);
                contentPane.qualityScreenBtn.gameObject.SetActive(true);
                contentPane.subToggleGroup.OnEnableMethod();
                break;
            case 1: //资源
                TimingRefreshMethod(true);

                contentPane.levelScreenBtn.gameObject.SetActive(false);
                contentPane.qualityScreenBtn.gameObject.SetActive(false);
                break;
            case 2: //推荐
                TimingRefreshMethod(true);

                contentPane.levelScreenBtn.gameObject.SetActive(false);
                contentPane.qualityScreenBtn.gameObject.SetActive(false);
                break;
        }

        int subIndex = _itemType != 1 ? 0 : 1;
        contentPane.sortText.text = MarketDataProxy.GetSortText((EmarketItemSortType)sortTypeGroup[(int)tradingHallType][subIndex][curSortType[(int)tradingHallType][subIndex]]);

    }

    //小类型选择
    private void subTypeSelectedChange(int bigType, int smallType)
    {
        if (_bigType == 0 || _bigType == 4) //推荐或其他 没有小类型 不继续执行
        {
            return;
        }

        _smallType = smallType;

        contentPane.allTypeToggle.isOn = false;

        switch (_itemType)
        {
            case 0: //装备
                TimingRefreshMethod(true);
                break;
            case 1: //资源

                break;
            case 2: //推荐

                break;
        }
    }

    private void showRefreshMask()
    {
        contentPane.refreshMask.SetActive(true);
        contentPane.superListContent.SetActive(false);
        maskTimer = GameTimer.inst.AddTimer(0.5f, 1, () =>
        {
            contentPane.refreshMask.SetActive(false);
            contentPane.superListContent.SetActive(true);
        });
    }

    public void TimingRefreshMethod(bool toSever)
    {
        if (toSever) showRefreshMask();


        switch (_itemType)
        {
            case 0: //装备
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
                { buyOrSell = (int)tradingHallType, itemType = 0, subTypes = /*subTypeGroup[_bigType]*/ new List<int> { _smallType }, levels = levelScreenResult, qualitys = qualityScreenResult }, toSever);
                break;
            case 1: //资源
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
                { buyOrSell = (int)tradingHallType, itemType = 1, subTypes = new List<int> { (int)ItemType.TaskMaterial, (int)ItemType.Box } }, toSever);
                break;
            case 2: //推荐
                List<int> subTypes = new List<int>();
                subTypes.AddRange(subTypeGroup[1]);
                subTypes.AddRange(subTypeGroup[2]);
                subTypes.AddRange(subTypeGroup[3]);

                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
                { buyOrSell = (int)tradingHallType, itemType = 0, subTypes = subTypes, isHot = 1 }, toSever);
                break;
        }
    }

    //无限滑动

    int listItemCount = 0;
    int curListCount;
    List<MarketItem> curMarketList = new List<MarketItem>();

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 6; ++i)
        {
            int itemIndex = index * 6 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < curListCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                AuctionItem item = itemScript.buttonList[i].GetComponent<AuctionItem>();
                item.SetData(curMarketList[itemIndex], tradingHallType);
                item.onClickHandler = itemOnClick;
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (listItemCount > curListCount)
        {
            listItemCount = curListCount;
        }
        int count1 = listItemCount / 6;
        if (listItemCount % 6 > 0)
        {
            count1++;
        }
        contentPane.superList.totalItemCount = count1;
        contentPane.tx_nothingTip.gameObject.SetActive(count1 == 0);
    }

    private void itemOnClick(MarketItem marketItem)
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETITEMINFOUI, marketItem, tradingHallType);
    }


    public void RefreshSuperList(int buyOrSell, int itemtype, List<MarketItem> marketItemList, bool isMaintenancing) //是否正在维护中
    {
        contentPane.tx_nothingTip.text = LanguageManager.inst.GetValueByKey(isMaintenancing ? "市场正在维护中..." : "已空！");

        if (marketItemList == null || itemtype != _itemType || this.tradingHallType != (kMarketTradingHallType)buyOrSell)
        {
            SetListItemTotalCount(0);
            //Logger.log("MarketHall Refresh Error");
            return;
        }

        curMarketList = marketItemList;
        int subIndex = itemtype != 1 ? 0 : 1;
        MarketDataProxy.MarketItemListSort(ref curMarketList, (EmarketItemSortType)sortTypeGroup[(int)tradingHallType][subIndex][curSortType[(int)tradingHallType][subIndex]]);
        curListCount = curMarketList.Count;
        SetListItemTotalCount(curListCount);
    }


    private void allTypeToggleOnValueChanged(bool isOn)
    {
        if (isOn)
        {
            AudioManager.inst.PlaySound(11);
            subItemList.Find(t => t.bigType == _bigType && t.smallType == _smallType).SetUnSeleted();
            int[] smallTypes = subTypeGroup[_bigType];

            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
            { buyOrSell = (int)tradingHallType, itemType = 0, subTypes = subTypeGroup[_bigType].ToList(), levels = levelScreenResult, qualitys = qualityScreenResult }, false);
        }
        else
        {
            subItemList.Find(t => t.bigType == _bigType && t.smallType == _smallType).SetSeleted();
        }
    }

    //等阶筛选取消
    private void onLevelScreenCancelBtnClick()
    {
        contentPane.levelScreenObj.SetActive(false);

        if (!contentPane.allLevelToggle.isOn)
        {
            for (int i = 0; i < contentPane.levelScreenToggles.Length; i++)
            {
                int index = i;
                Toggle toggle = contentPane.levelScreenToggles[index];

                toggle.isOn = levelScreenResult.Contains(index + 1);
            }
        }

    }

    //等阶筛选应用
    private void onLevelScreenApplyBtnClick()
    {
        contentPane.levelScreenObj.SetActive(false);

        List<int> temp = new List<int>(levelScreenResult);

        if (!contentPane.allLevelToggle.isOn)
        {
            levelScreenResult.Clear();

            for (int i = 0; i < contentPane.levelScreenToggles.Length; i++)
            {
                int index = i;
                Toggle toggle = contentPane.levelScreenToggles[index];

                if (toggle.isOn)
                    levelScreenResult.Add(index + 1);
            }
        }
        else
        {
            allLevelToggleIsOn(true);
        }

        if (!Enumerable.SequenceEqual(temp, levelScreenResult))
        {
            //EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
            //{ buyOrSell = (int)tradingHallType, itemType = 0, subTypes = contentPane.allTypeToggle.isOn ? subTypeGroup[_bigType] : new int[] { _smallType }, levels = levelScreenResult, qualitys = qualityScreenResult }, false);
            //showRefreshMask();
            TimingRefreshMethod(true);
        }
    }

    //品质筛选取消
    private void onQualityScreenCancelBtnClick()
    {
        contentPane.qualityScreenObj.SetActive(false);

        if (!contentPane.allQualityToggle.isOn)
        {
            for (int i = 0; i < contentPane.qualityScreenToggles.Length; i++)
            {
                int index = i;
                Toggle toggle = contentPane.qualityScreenToggles[index];

                toggle.isOn = qualityScreenResult.Contains(index + 1);
            }
        }

    }

    //品质筛选应用
    private void onQualityScreenApplyBtnClick()
    {
        contentPane.qualityScreenObj.SetActive(false);

        List<int> temp = new List<int>(qualityScreenResult);

        if (!contentPane.allQualityToggle.isOn)
        {
            qualityScreenResult.Clear();

            for (int i = 0; i < contentPane.qualityScreenToggles.Length; i++)
            {
                int index = i;
                Toggle toggle = contentPane.qualityScreenToggles[index];

                if (toggle.isOn)
                {
                    qualityScreenResult.Add(index + 1); //普通装备
                    qualityScreenResult.Add(StaticConstants.SuperEquipBaseQuality + index + 1); //超凡装备
                }
            }
        }
        else
        {
            allQualityToggleIsOn(true);
        }

        if (!Enumerable.SequenceEqual(temp, qualityScreenResult))
        {
            //EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
            //{ buyOrSell = (int)tradingHallType, itemType = 0, subTypes = contentPane.allTypeToggle.isOn ? subTypeGroup[_bigType] : new int[] { _smallType }, levels = levelScreenResult, qualitys = qualityScreenResult }, false);
            //showRefreshMask();
            TimingRefreshMethod(true);
        }
    }

}
