using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShelfInventoryUIView : ViewBase<ShelfInventoryComp>
{
    public override string viewID => ViewPrefabName.ShelfInventoryUI;
    public override string sortingLayerName => "window";


    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;


        contentPane.closeInventoryBtn.onClick.AddListener(() =>
        {
            hide();
        });

        contentPane.suggestItemList.itemRenderer = this.listitemRenderer;
        contentPane.suggestItemList.itemUpdateInfo = this.listitemRenderer;
        contentPane.suggestItemList.scrollByItemIndex(0);
        //contentPane.suggestItemList.totalItemCount = 0;
    }


    public int[] typeIdArray;
    public int shelfUid;
    private bool mNeedShowAnim;

    protected override void onShown()
    {
        //处理从场景里拖到UI上的货架层级叠上去的情况
        _uiCanvas.sortingOrder += 10;
    }

    public void GetItemLists(int[] typeIdArray, int shelfUid, bool needShowAnim)
    {
        this.typeIdArray = typeIdArray;
        this.shelfUid = shelfUid;
        mNeedShowAnim = needShowAnim;

        currOnShelfEquips = ItemBagProxy.inst.GetEquipItemsByType(true, typeIdArray);
        currOnShelfEquips = currOnShelfEquips.FindAll(t => !t.isLock);

        if (currOnShelfEquips.Count > 0)
        {
            SetListItemTotalCount(currOnShelfEquips.Count);
        }
        else
        {
            listItemCount = 1;
            contentPane.suggestItemList.totalItemCount = 1;
        }

        contentPane.suggestItemList.refresh();
    }

    int listItemCount = 0;
    private List<EquipItem> currOnShelfEquips;

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 3; i++)
        {
            int itemIndex = index * 3 + i;

            if (itemIndex > listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }


            if (itemIndex < currOnShelfEquips.Count + 1)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                ShelfInventoryItem item = itemScript.buttonList[i].GetComponent<ShelfInventoryItem>();

                if (itemIndex == 0) item.SetExchangeBtn(typeIdArray, mNeedShowAnim);
                else item.setData(currOnShelfEquips[itemIndex - 1], itemIndex, shelfUid, mNeedShowAnim);
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
        if (listItemCount > currOnShelfEquips.Count)
        {
            listItemCount = currOnShelfEquips.Count;
        }
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.suggestItemList.totalItemCount = count1 + 1;
    }


    protected override void onHide()
    {
        base.onHide();
        currOnShelfEquips = null;
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
