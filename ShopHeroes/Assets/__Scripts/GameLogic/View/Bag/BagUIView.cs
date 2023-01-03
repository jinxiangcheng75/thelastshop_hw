using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BagUIView : ViewBase<BagComp>
{
    public override string viewID => ViewPrefabName.BagUI;
    public override int showType => (int)ViewShowType.pullUp;
    public int currState = 0;  //0--装备  1--符文 2--任务材料 3 
    public int[] types = { (int)ItemType.Blueprint,
    (int)ItemType.Box,
    (int)ItemType.Hero,
    (int)ItemType.SpecialEquipment,
    (int)ItemType.EquipmentDrawing,
    (int)ItemType.Turntable,
    (int)ItemType.HeroExp,
    (int)ItemType.HeroTransfer,
    (int)ItemType.ExploreTimeItem,
    (int)ItemType.ExploreAddYieldItem,
    (int)ItemType.ExploreAttBonus,
    (int)ItemType.RecoverHeroItem,
    (int)ItemType.ExploreExpBonusItem,
    (int)ItemType.RepairEquipmentItem,
    (int)ItemType.HeroCard,
    (int)ItemType.Activity_WorkerGameCoin,
    (int)ItemType.EquipStarUp,
    (int)ItemType.HeroPropertyUp,
    54, // 常驻悬赏 活动代币
    56, // 酒吧刷新券
    };

    const int itemCountPerRow = 3;
    int listItemCount = 0;

    public List<EquipItem> bagEquipList; //背包裝備列表
    List<Item> bagItemList; //背包道具資源列表
    SortType currEquipSortType = SortType.Time;

    bool needShowAni;

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.noSetting;
        isShowResPanel = true;
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });

        contentPane.sortordButton.onClick.AddListener(sortBtnOnclick);

        contentPane.toggleGroup.OnSelectedIndexValueChange = mainTypeSelectedChange;
        _init();
    }

    private void mainTypeSelectedChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        needShowAni = false;
        currState = index;
        contentPane.sortordButton.transform.parent.gameObject.SetActive(currState == 0);
        if (currState == 1)
            bagItemList = ItemBagProxy.inst.GetItemsByType(ItemType.TaskMaterial);
        else if (currState == 2)
        {
            bagItemList = ItemBagProxy.inst.GetItemsByTypes(types, false);
        }
        if (bagItemList != null)
        {
            bagItemList = bagItemList.FindAll(item => item.itemConfig.type == (int)ItemType.Box || item.count > 0);
        }
        showInfo();
        contentPane.loopListView.ScrollToTop();
    }
    private void sortBtnOnclick()
    {

        int index = (int)currEquipSortType + 1;
        if (index >= (int)SortType.Num)
        {
            index = 0;
        }
        currEquipSortType = (SortType)index;
        ItemBagProxy.EquipItemSort(ref this.bagEquipList, currEquipSortType);
        SetSortBtnText();
        contentPane.loopListView.ScrollToTop();
        contentPane.loopListView.refresh();

        //refreshList();
    }
    private void SetSortBtnText()
    {
        string btnName = "";
        switch (currEquipSortType)
        {
            case SortType.Number:
                btnName = "数量";
                break;
            case SortType.Level:
                btnName = "阶级";
                break;
            case SortType.Price:
                btnName = "价值";
                break;
            case SortType.Quality:
                btnName = "稀有度";
                break;
            case SortType.Time:
                btnName = "最近";
                break;
            case SortType.SubType:
                btnName = "类型";
                break;
        }
        contentPane.sortordButton.GetComponentInChildren<Text>().text = LanguageManager.inst.GetValueByKey("排序方式：{0}", LanguageManager.inst.GetValueByKey(btnName));

        //记录排序类型
        SaveManager.inst.SaveInt("BagUIView_SortList", (int)currEquipSortType);
    }

    public override void shiftIn()
    {
        base.shiftIn();
        if (currState == 1)
            bagItemList = ItemBagProxy.inst.GetItemsByType(ItemType.TaskMaterial);
        else if (currState == 2)
            bagItemList = ItemBagProxy.inst.GetItemsByTypes(types, false);

        if (bagItemList != null)
        {
            bagItemList = bagItemList.FindAll(item => item.itemConfig.type == (int)ItemType.Box || item.count > 0);
        }
        //SetSortBtnText();
        contentPane.loopListView.refresh();
        showInfo();
    }
    protected override void onShown()
    {
        //特殊资源
        if (currState == 1)
            bagItemList = ItemBagProxy.inst.GetItemsByType(ItemType.TaskMaterial);
        else if (currState == 2)
            bagItemList = ItemBagProxy.inst.GetItemsByTypes(types, false);

        if (bagItemList != null)
        {
            bagItemList = bagItemList.FindAll(item => item.itemConfig.type == (int)ItemType.Box || item.count > 0);
        }
        bagEquipList = ItemBagProxy.inst.GetAllEquipItem();
        if (SaveManager.inst.HasKey("BagUIView_SortList"))
        {
            currEquipSortType = (SortType)SaveManager.inst.GetInt("BagUIView_SortList");
        }
        ItemBagProxy.EquipItemSort(ref this.bagEquipList, currEquipSortType);
        SetSortBtnText();
        if (contentPane.toggleGroup.selectedIndex == currState)
            showInfo();
        else
            contentPane.toggleGroup.selectedIndex = currState;
        //
        contentPane.loopListView.ScrollToTop();
    }

    protected override void DoShowAnimation()
    {
        needShowAni = true;
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");

        contentPane.maskObj.SetActiveTrue();

        GameTimer.inst.AddTimer(getItemCount() <= 9 ? getItemCount() * 0.02f + 0.28f : 0.46f, 1, () =>
        {
            contentPane.maskObj.SetActiveFalse();
            needShowAni = false;
        }); //播放动画禁止滑动

    }

    private void showInfo()
    {
        if (currState == 0)
        {
            contentPane.bagLimitText.text = LanguageManager.inst.GetValueByKey("容量：") + ItemBagProxy.inst.GetEquipInventory() + "/" + ItemBagProxy.inst.bagCountLimit;
        }
        else if (currState == 1)
        {
            double count = 0;
            bagItemList.ForEach(item => { count += item.count; });
            contentPane.bagLimitText.text = LanguageManager.inst.GetValueByKey("堆叠上限：") + UserDataProxy.inst.playerData.pileLimit;
        }
        else if (currState == 2)
        {
            contentPane.bagLimitText.text = "";
        }
        SetListItemTotalCount(getItemCount());
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            if (contentPane == null) return;
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    private void ChangeStart(int state)
    {
        if (currState != state)
        {
            currState = state;
            contentPane.sortordButton.transform.parent.gameObject.SetActive(currState == 0);
            showInfo();
        }
    }

    private void _init()
    {
        if (contentPane.loopListView != null)
        {
            contentPane.loopListView.itemRenderer = this.listitemRenderer;
            // contentPane.loopListView.totalItemCount = 0;
            // contentPane.loopListView.scrollByItemIndex(0);
        }
    }

    private int getItemCount()
    {
        if (currState == 0 && bagEquipList != null)
        {
            return bagEquipList.Count;
        }
        else if (currState == 1 || currState == 2)
        {
            return bagItemList.Count;
        }

        return 0;
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < itemCountPerRow; ++i)
        {
            int itemIndex = index * itemCountPerRow + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < getItemCount())
            {
                var item = itemScript.buttonList[i].GetComponent<bagListItem>();
                itemScript.buttonList[i].gameObject.SetActive(true);
                //获取data  赋值
                setBagItemData(itemIndex, item);
                item.itemOnclick = ListItemOnClick;
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }
    // fromType --- 0 - item本身 1 - infoBtn
    private void ListItemOnClick(string uid, int fromType)
    {
        if (currState == 0)
        {
            //打开装备信息
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, uid, 0, bagEquipList);
        }
        else if (currState == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, int.Parse(uid));
        }
        else if (currState == 2)
        {
            if (fromType == 1)
            {
                EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, int.Parse(uid));
                return;
            }
            var itemCfg = ItemconfigManager.inst.GetConfig(int.Parse(uid));
            var itemType = (ItemType)itemCfg.type;
            if (itemType == ItemType.Box)
            {

                //var boxData = TreasureBoxDataProxy.inst.GetDataByBoxId(itemCfg.id);
                var boxData = ItemBagProxy.inst.GetItem(itemCfg.id);
                if (boxData.count > 0)
                {
                    hide();
                    TreasureBoxDataProxy.inst.newBoxGroupId = itemCfg.id;
                    TreasureBoxDataProxy.inst.isBackToTown = false;
                    HotfixBridge.inst.ChangeState(new StateTransition(kGameState.TBox, true));
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_REQUIREDITEM, 1, itemCfg.id, 1, true);
                }
            }
            else if (itemType == ItemType.HeroExp)
            {
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
            }
            else if (itemType == ItemType.RecoverHeroItem)
            {
                if (itemCfg.effect == 1)
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
                }
                else
                {
                    if (RoleDataProxy.inst.GetRestingStateHeroCount().Count > 0)
                        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ALLROLERESTING_SHOWUI);
                    else
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您的角色已经休息完毕了"), GUIHelper.GetColorByColorHex("FF2828"));
                }
            }
            else if (itemType == ItemType.Turntable)
            {
                if (UIUnLockConfigMrg.inst.HasBtnMatchedCfg("toggle_lottery"))
                {
                    if (!UIUnLockConfigMrg.inst.GetBtnInteractable("toggle_lottery"))
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("转盘功能尚未解锁"), GUIHelper.GetColorByColorHex("FF2828"));
                        return;
                    }
                }
                else
                {
                    float lotteryUnlockLevel = WorldParConfigManager.inst.GetConfig(100).parameters;
                    if (UserDataProxy.inst.playerData.level < lotteryUnlockLevel)
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("转盘{0}级可解锁", lotteryUnlockLevel.ToString()), GUIHelper.GetColorByColorHex("FF2828"));
                        return;
                    }
                }

                HotfixBridge.inst.TriggerLuaEvent("ShowUI_WelfareUI", 2);
            }
            else if (itemType == ItemType.HeroCard)
            {
                if (RoleDataProxy.inst.FieldNumAbtractHeroNum <= 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("当前英雄栏位已满请先扩充栏位"), GUIHelper.GetColorByColorHex("FF2828"));
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_USEHEROITEM, int.Parse(uid), 0);
                }
            }
            else if (itemType == ItemType.HeroPropertyUp) //英雄属性提升道具
            {
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLE_SHOWUI);
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, int.Parse(uid));
            }
        }
    }
    void setBagItemData(int itemIndex, bagListItem item)
    {
        if (currState == 0)
        {
            if (itemIndex < 0 || itemIndex >= bagEquipList.Count)
            { return; }
            item.setData(bagEquipList[itemIndex], itemIndex, needShowAni);
        }
        else if (currState == 1 /*|| currState == 2*/)
        {
            if (itemIndex < 0 || itemIndex >= bagItemList.Count)
            { return; }
            item.setData(bagItemList[itemIndex], itemIndex, needShowAni);
        }
        else if (currState == 2)
        {
            if (itemIndex < 0 || itemIndex >= bagItemList.Count)
            { return; }
            item.setOtherData(bagItemList[itemIndex], itemIndex, needShowAni);
        }
    }
    public void refreshList()
    {
        bagItemList = ItemBagProxy.inst.GetItemsByType(ItemType.TaskMaterial);
        bagEquipList = ItemBagProxy.inst.GetAllEquipItem();
        ItemBagProxy.EquipItemSort(ref this.bagEquipList, currEquipSortType);
        showInfo();
    }
    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        int count1 = listItemCount / itemCountPerRow;
        if (listItemCount % itemCountPerRow > 0)
        {
            count1++;
        }
        contentPane.loopListView.totalItemCount = count1;
    }
}
