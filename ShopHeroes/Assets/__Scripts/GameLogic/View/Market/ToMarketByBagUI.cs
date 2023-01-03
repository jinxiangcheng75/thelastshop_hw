using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToMarketByBagUI : ViewBase<ToMarketByBagUIComp>
{

    public override string viewID => ViewPrefabName.ToMarketByBagUI;

    public override string sortingLayerName => "window";


    private int[][] subTypeGroup = new int[][] //大分类 与子分类关系
    {
        new int[]{1,2,3,4,5,8,6,7},
        new int[]{9,10,11,12,13,14,15,16,17},
        new int[]{18,19,20,21,22,23},
        new int[]{ },
    };

    //EmarketItemSortType
    private int[][] curSortType = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 } };
    private int[][][] sortTypeGroup = new int[][][] //报价 请求 next: 装备 资源 next: 排序类型
    {
        new int[][]{ new int[] { 0,1,2,3,4,5,6,7 },new int[] { 2,3,7 } },
        new int[][]{ new int[] { 0,1,2,3,6,7 },new int[] { 2,3,7 } },
    };


    private List<MarketSubTypeItem> subItemList;
    private bool isEquip;


    //等阶筛选
    private int curCanUseLevelMax;
    private int levelToggleIsOnCount;
    private List<int> levelScreenResult;

    //品质筛选
    private int qualityToggleIsOnCount;
    private List<int> qualityScreenResult;

    private int _bigType, _smallType; //大类型 0 推荐 1 武器 2 防具 3 配件 4 资源


    private kMarketItemType marketType;
    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;


        levelScreenResult = new List<int>();
        qualityScreenResult = new List<int>();

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.sortBtn.onClick.AddListener(onSortBtnClick);

        if (FGUI.inst.isLandscape)
        {
            contentPane.bigToggleGroup.SetToggleSize(new Vector2(200, 124), new Vector2(200, 124));
        }
        else
        {
            contentPane.bigToggleGroup.SetToggleSize(new Vector2(320, 124), new Vector2(246, 124));
        }

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

            if (subItem == null) continue;

            subItem.Init();
            subItem.onSelectHandler = subTypeSelectedChange;
            subItemList.Add(subItem);
        }


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
                qualityScreenResult.Add(i + 1);//普通装备
                qualityScreenResult.Add(StaticConstants.SuperEquipBaseQuality + i + 1);//超凡装备
            }
        }
    }

    public void SetMarketItemType(kMarketItemType type)
    {
        marketType = type;

        contentPane.titleTypeIcon.SetSprite("market_atlas", marketType == kMarketItemType.selfSell ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
        contentPane.titleTypeIcon.transform.localScale = new Vector3(marketType == kMarketItemType.selfSell ? -1 : 1, 1, 1);
        contentPane.topBgIcon.SetSprite("market_atlas_newAdd", marketType == kMarketItemType.selfSell ? "shichang_dinglv" : "shichang_dinglan");

        //等阶筛选
        contentPane.allLevelToggle.isOn = true;
        allLevelToggleIsOn(true);


        //品质筛选
        contentPane.allQualityToggle.isOn = true;
        allQualityToggleIsOn(true);

        contentPane.bigToggleGroup.OnEnableMethod();

    }


    private void onSortBtnClick()
    {
        int subIndex = isEquip ? 0 : 1;
        curSortType[(int)marketType][subIndex] += 1;
        if (curSortType[(int)marketType][subIndex] >= sortTypeGroup[(int)marketType][subIndex].Length)
        {
            curSortType[(int)marketType][subIndex] = 0;
        }

        contentPane.sortText.text = MarketDataProxy.GetSortText((EmarketItemSortType)sortTypeGroup[(int)marketType][subIndex][curSortType[(int)marketType][subIndex]]);

        if (marketType == kMarketItemType.selfSell)
        {
            if (isEquip)
            {
                MarketDataProxy.MyHoldingEquipListSort(ref curSelfHaveEquipList, (EmarketItemSortType)sortTypeGroup[(int)marketType][subIndex][curSortType[(int)marketType][subIndex]]);
            }
            else
            {
                MarketDataProxy.ItemListSort(ref curMaterialsAndBoxList, (EmarketItemSortType)sortTypeGroup[(int)marketType][subIndex][curSortType[(int)marketType][subIndex]]);
            }

        }
        else
        {
            if (isEquip)
            {
                MarketDataProxy.EquipConfigListSort(ref curEquipDrawingList, (EmarketItemSortType)sortTypeGroup[(int)marketType][subIndex][curSortType[(int)marketType][subIndex]]);
            }
            else
            {
                MarketDataProxy.ItemListSort(ref curMaterialsAndBoxList, (EmarketItemSortType)sortTypeGroup[(int)marketType][subIndex][curSortType[(int)marketType][subIndex]]);
            }
        }

        contentPane.superList.refresh();
    }

    //大类型选中
    private void typeSelectedChange(int index)
    {

        _bigType = index;
        int[] smallTypes = subTypeGroup[index];

        isEquip = index != 3;

        contentPane.allTypeToggle.gameObject.SetActive(isEquip);

        for (int i = 0; i < subItemList.Count; i++)
        {
            subItemList[i].gameObject.SetActive(false);
        }

        if (isEquip) //装备
        {
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
            contentPane.qualityScreenBtn.gameObject.SetActive(marketType != kMarketItemType.selfBuy);
            contentPane.subToggleGroup.OnEnableMethod();

            contentPane.allTypeToggle.isOn = true;

        }
        else //资源
        {
            curMaterialsAndBoxList = ItemBagProxy.inst.GetItemsByTypes(new ItemType[] { ItemType.TaskMaterial, ItemType.Box }, true);

            if (marketType == kMarketItemType.selfSell) curMaterialsAndBoxList = curMaterialsAndBoxList.FindAll(t => t.count > 0);

            MarketDataProxy.ItemListSort(ref curMaterialsAndBoxList, (EmarketItemSortType)sortTypeGroup[(int)marketType][1][curSortType[(int)marketType][1]]);
            curListCount = curMaterialsAndBoxList.Count;
            SetListItemTotalCount(curListCount);

            contentPane.levelScreenBtn.gameObject.SetActive(false);
            contentPane.qualityScreenBtn.gameObject.SetActive(false);
        }

        int subIndex = isEquip ? 0 : 1;
        contentPane.sortText.text = MarketDataProxy.GetSortText((EmarketItemSortType)sortTypeGroup[(int)marketType][subIndex][curSortType[(int)marketType][subIndex]]);
    }

    //小类型选择
    private void subTypeSelectedChange(int bigType, int smallType)
    {
        //Logger.error("bigType  = " + bigType + "  smallType = " + smallType + "    marketType = " + marketType);
        _smallType = smallType;

        contentPane.allTypeToggle.isOn = false;

        if (isEquip) //装备
        {
            if (marketType == kMarketItemType.selfSell) //自己卖是拉自身有的
            {
                setSelfHaveEquipList();
            }
            else
            {
                setEquipDrawingList();
            }
            SetListItemTotalCount(curListCount);
        }
        else  //资源
        {

        }

    }


    //无限滑动

    int listItemCount = 0;
    int curListCount;
    List<EquipItem> curSelfHaveEquipList;
    List<EquipDrawingsConfig> curEquipDrawingList;
    List<Item> curMaterialsAndBoxList;

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

            if (itemIndex < curListCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                ToMarketByBagListItem item = itemScript.buttonList[i].GetComponent<ToMarketByBagListItem>();
                SetItemData(ref item, itemIndex);
                item.onClickHandler = itemOnClick;
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    void SetItemData(ref ToMarketByBagListItem item, int itemIndex)
    {
        if (isEquip)
        {
            if (marketType == kMarketItemType.selfSell)
                item.SetData(curSelfHaveEquipList[itemIndex]);
            else
                item.SetData(curEquipDrawingList[itemIndex]);
        }
        else
        {
            item.SetData(curMaterialsAndBoxList[itemIndex]);
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
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.superList.totalItemCount = count1;
        contentPane.nothingTipObj.SetActive(count1 == 0);
    }

    private void itemOnClick(int itemType, int itemId, int itemQuality, bool isSuper)
    {
        ToSubmitMarketItemData toSubmitMarketItemData = new ToSubmitMarketItemData();
        toSubmitMarketItemData.marketType = marketType;
        toSubmitMarketItemData.itemType = itemType;
        toSubmitMarketItemData.itemId = itemId;
        toSubmitMarketItemData.itemQuality = itemQuality;
        toSubmitMarketItemData.isSuper = isSuper;

        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_SUBMITMARKETITEMUI, toSubmitMarketItemData);
    }

    private void allTypeToggleOnValueChanged(bool isOn)
    {
        if (isOn)
        {
            subItemList.Find(t => t.bigType == _bigType && t.smallType == _smallType).SetUnSeleted();
            int[] smallTypes = subTypeGroup[_bigType];

            if (marketType != kMarketItemType.selfBuy)
            {
                setSelfHaveEquipList(smallTypes);
            }
            else
            {
                setEquipDrawingList(smallTypes);
            }
        }
        else
        {
            subItemList.Find(t => t.bigType == _bigType && t.smallType == _smallType).SetSeleted();
        }

        for (int i = 0; i < contentPane.allTypeToggle.graphic.transform.childCount; i++)
        {
            contentPane.allTypeToggle.graphic.transform.GetChild(i).gameObject.SetActive(contentPane.allTypeToggle.isOn);
        }

    }

    //筛选后刷新UI
    private void setSelfHaveEquipList(int[] smallTypes = null)
    {
        curSelfHaveEquipList = smallTypes == null ? ItemBagProxy.inst.GetEquipItemsByType((EquipSubType)_smallType) : ItemBagProxy.inst.GetEquipItemsByType(false, smallTypes);
        curSelfHaveEquipList = curSelfHaveEquipList.FindAll((item) => qualityScreenResult.Contains(item.quality) && levelScreenResult.Contains(item.equipConfig.equipDrawingsConfig.level));
        MarketDataProxy.MyHoldingEquipListSort(ref curSelfHaveEquipList, (EmarketItemSortType)sortTypeGroup[(int)marketType][0][curSortType[(int)marketType][0]]);
        curListCount = curSelfHaveEquipList.Count;
        SetListItemTotalCount(curListCount);
    }

    private void setEquipDrawingList(int[] smallTyps = null)
    {
        curEquipDrawingList = smallTyps == null ? EquipConfigManager.inst.GetEquipConfigsByType((EquipSubType)_smallType) : EquipConfigManager.inst.GetEquipConfigsByTypes(smallTyps);
        curEquipDrawingList = curEquipDrawingList.FindAll((item) => levelScreenResult.Contains(item.level));
        MarketDataProxy.EquipConfigListSort(ref curEquipDrawingList, (EmarketItemSortType)sortTypeGroup[(int)marketType][0][curSortType[(int)marketType][0]]);
        curListCount = curEquipDrawingList.Count;
        SetListItemTotalCount(curListCount);
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

        if (marketType != kMarketItemType.selfBuy)
        {
            setSelfHaveEquipList(contentPane.allTypeToggle.isOn ? subTypeGroup[_bigType] : null);
        }
        else
        {
            setEquipDrawingList(contentPane.allTypeToggle.isOn ? subTypeGroup[_bigType] : null);
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

        if (!contentPane.allQualityToggle.isOn)
        {
            qualityScreenResult.Clear();

            for (int i = 0; i < contentPane.qualityScreenToggles.Length; i++)
            {
                int index = i;
                Toggle toggle = contentPane.qualityScreenToggles[index];

                if (toggle.isOn)
                {
                    qualityScreenResult.Add(index + 1);//普通装备
                    qualityScreenResult.Add(StaticConstants.SuperEquipBaseQuality + i + 1);//超凡装备
                }
            }
        }
        else
        {
            allQualityToggleIsOn(true);
        }


        //品质筛选 只有自己卖的时候有
        setSelfHaveEquipList(contentPane.allTypeToggle.isOn ? subTypeGroup[_bigType] : null);
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        AudioManager.inst.PlaySound(11);
    }

}
