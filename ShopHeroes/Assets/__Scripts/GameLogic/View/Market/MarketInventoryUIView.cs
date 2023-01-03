using Mosframe;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum MarketInventoryFromType //从哪进入的此界面
{
    byShelf = 0,// 货架
    byHeroWearEquip = 1,// 英雄装备
}

public class MarketInventoryUIView : ViewBase<MarketInventoryUIComp>
{

    public override string viewID => ViewPrefabName.MarketInventoryUI;

    public override string sortingLayerName => "window";


    int listItemCount = 0;
    int[] mTypeidArray;
    int mMarketInventoryFromType = 0;
    private List<MarketItem> curMarketList = new List<MarketItem>();

    //定期刷新
    private int refreshTimer;
    private int maskTimer;

    //等阶筛选
    private int curCanUseLevelMax;
    private int levelToggleIsOnCount;
    private List<int> levelScreenResult;

    //品质筛选
    private int qualityToggleIsOnCount;
    private List<int> qualityScreenResult;

    EmarketItemSortType currSortType = EmarketItemSortType.Quality;

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;

        levelScreenResult = new List<int>();
        qualityScreenResult = new List<int>();

        contentPane.superList.itemRenderer = listitemRenderer;
        contentPane.superList.itemUpdateInfo = listitemRenderer;
        contentPane.superList.scrollByItemIndex(0);
        //contentPane.superList.totalItemCount = 0;

        contentPane.closeBtn.onClick.AddListener(hide);

        contentPane.sortBtn.onClick.AddListener(onSortBtnClick);


        contentPane.levelScreenBtn.onClick.AddListener(() => { contentPane.levelScreenObj.SetActive(true); });
        contentPane.levelScreenObjBtn.onClick.AddListener(onLevelScreenApplyBtnClick);
        contentPane.levelScreenCancelBtn.onClick.AddListener(onLevelScreenCancelBtnClick);
        contentPane.levelScreenApplyBtn.onClick.AddListener(onLevelScreenApplyBtnClick);

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

    protected override void onHide()
    {
        base.onHide();


        GameTimer.inst.RemoveTimer(maskTimer);
        maskTimer = 0;
        GameTimer.inst.RemoveTimer(refreshTimer);
        refreshTimer = 0;
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

    public void GetItemLists(int[] typeidArray, int maxLevel, bool canChangeLevel, int marketInventoryFromType)
    {
        mTypeidArray = typeidArray;
        curCanUseLevelMax = maxLevel;
        mMarketInventoryFromType = marketInventoryFromType;


        contentPane.levelScreenBtn.gameObject.SetActive(canChangeLevel);

        //等阶筛选
        contentPane.allLevelToggle.isOn = true;
        allLevelToggleIsOn(true);

        //品质筛选
        contentPane.allQualityToggle.isOn = true;
        allQualityToggleIsOn(true);


        TimingRefreshMethod(true);

        contentPane.sortText.text = MarketDataProxy.GetSortText(currSortType);

        //refreshTimer = GameTimer.inst.AddTimer(RefreshTime, () => TimingRefreshMethod(true));
    }

    public void TimingRefreshMethod(bool toSever)
    {
        showRefreshMask();

        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData()
        { buyOrSell = 0, itemType = 0, subTypes = mTypeidArray.ToList(), levels = levelScreenResult, qualitys = qualityScreenResult }, toSever);
    }


    public void RefreshSuperList(int buyOrSell, int itemType, List<int> subTypes, List<MarketItem> marketItemList, bool isMaintenancing) //是否正在维护中
    {
        contentPane.tx_nothingTip.text = LanguageManager.inst.GetValueByKey(isMaintenancing ? "市场正在维护中..." : "已空！");

        if (marketItemList == null || itemType == 1 || kMarketTradingHallType.selfBuy != (kMarketTradingHallType)buyOrSell || !Enumerable.SequenceEqual(mTypeidArray, subTypes))
        {
            SetListItemTotalCount(0);
            //Logger.error("Shelf-MarketInventory Refresh Error");
            return;
        }

        curMarketList = marketItemList;
        MarketDataProxy.MarketItemListSort(ref this.curMarketList, currSortType);
        SetListItemTotalCount(curMarketList.Count);
    }


    private void allLevelToggleIsOn(bool isCover)
    {
        //curCanUseLevelMax = MarketEquipLvManager.inst.GetCurMarketLevel();
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
                qualityScreenResult.Add(i + 1); //普通装备
                qualityScreenResult.Add(StaticConstants.SuperEquipBaseQuality + i + 1); //超凡装备
            }
        }
    }



    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 3; ++i)
        {
            int itemIndex = index * 3 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < curMarketList.Count)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                MarketListItem item = itemScript.buttonList[i].GetComponent<MarketListItem>();
                item.setData(curMarketList[itemIndex], mMarketInventoryFromType);
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
        if (listItemCount > curMarketList.Count)
        {
            listItemCount = curMarketList.Count;
        }
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.superList.totalItemCount = count1;
        contentPane.tx_nothingTip.gameObject.SetActive(count1 == 0);
    }

    private void onSortBtnClick()
    {
        int index = (int)currSortType + 1;
        if (index >= (int)EmarketItemSortType.max)
        {
            index = 0;
        }
        currSortType = (EmarketItemSortType)index;
        MarketDataProxy.MarketItemListSort(ref this.curMarketList, currSortType);
        contentPane.sortText.text = MarketDataProxy.GetSortText(currSortType);
        contentPane.superList.ScrollToTop();
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
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData() { buyOrSell = 0, itemType = 0, subTypes = mTypeidArray.ToList(), levels = levelScreenResult, qualitys = qualityScreenResult }, false);
            showRefreshMask();
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
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEMLIST_REFRESH, new Request_marketItemListData() { buyOrSell = 0, itemType = 0, subTypes = mTypeidArray.ToList(), levels = levelScreenResult, qualitys = qualityScreenResult }, false);
            showRefreshMask();
        }


    }

}
