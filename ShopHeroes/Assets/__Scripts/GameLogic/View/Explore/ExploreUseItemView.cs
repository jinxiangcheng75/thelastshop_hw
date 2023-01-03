using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mosframe;

public class ExploreUseItemView : ViewBase<ExploreUseItemCom>
{
    public override string viewID => ViewPrefabName.ExploreUseItemUI;
    public override string sortingLayerName => "window";
    List<Item> allItems;
    List<TreasureBoxData> allBoxes;
    int curType;
    int listCount;
    protected override void onInit()
    {
        base.onInit();
        allItems = new List<Item>();
        allBoxes = new List<TreasureBoxData>();
        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        contentPane.scrollView.scrollByItemIndex(0);
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

            if (itemIndex < listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);
                SelectBagItem item = itemScript.buttonList[i].GetComponent<SelectBagItem>();
                if (curType == 6)
                {
                    item.setExploreItemData(allItems[itemIndex], OnItemClick);
                }
                else if (curType == 100)
                {
                    item.setTreasureBoxItemData(allBoxes[itemIndex], OnBoxClick);
                }
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnItemClick(int itemId)
    {
        hide();
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUSEITEM_COMPLETE, itemId);
    }

    private void OnBoxClick(int boxId)
    {
        hide();
        EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.OPENBOX_SHOWUI, boxId);
    }

    public void GetItemLists(int itemType)
    {
        curType = itemType;
        if (itemType == 6)
        {
            contentPane.titleText.text = LanguageManager.inst.GetValueByKey("选择强化");
            allItems = ItemBagProxy.inst.GetItemsByTypes(new ItemType[] { ItemType.ExploreTimeItem, ItemType.ExploreAddYieldItem, ItemType.ExploreAttBonus, ItemType.ExploreExpBonusItem }, true);

            if (allItems.Count > 0)
            {
                contentPane.emptyTBox.gameObject.SetActive(false);
                SetListItemTotalCount(allItems.Count);
            }
            else
            {
                contentPane.emptyTBox.text = LanguageManager.inst.GetValueByKey("无加成道具");
                contentPane.emptyTBox.gameObject.SetActive(true);
                contentPane.scrollView.totalItemCount = 0;
            }
        }
        else if (itemType == 100)
        {
            contentPane.titleText.text = LanguageManager.inst.GetValueByKey("选择宝箱");
            allBoxes = TreasureBoxDataProxy.inst.boxList;
            contentPane.emptyTBox.text = LanguageManager.inst.GetValueByKey("探险击杀头目可以获得宝箱！");
            contentPane.emptyTBox.gameObject.SetActive(allBoxes.Count <= 0);
            SetListItemTotalCount(allBoxes.Count);
        }

        contentPane.scrollView.refresh();
    }

    int listItemCount = 0;
    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (curType == 6)
        {
            if (listItemCount > allItems.Count)
            {
                listItemCount = allItems.Count;
            }
        }
        else if (curType == 100)
        {
            if (listItemCount > allBoxes.Count)
            {
                listItemCount = allBoxes.Count;
            }
        }

        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.scrollView.totalItemCount = count1;
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
        contentPane.scrollView.ScrollToTop();
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
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
}
