using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBoxInfoUIView : ViewBase<TreasureBoxInfoUICom>
{
    public override string viewID => ViewPrefabName.TreasureBoxInfoUI;
    public override string sortingLayerName => "window";

    kTreasureBoxInfoType curBoxType = kTreasureBoxInfoType.max;
    private List<ExclusiveItemData> itemList;
    TreasureBoxData data;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(hide);
        InitComponent();
    }

    private void InitComponent()
    {
        itemList = new List<ExclusiveItemData>();
        contentPane.group.OnSelectedIndexValueChange = typeSelectChange;
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;

    }

    int listItemCount = 0;
    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 4; ++i)
        {
            int itemIndex = index * 4 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < itemList.Count)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);
                var item = itemScript.buttonList[i].GetComponent<ExclusiveItem>();
                item.setData(itemList[itemIndex]);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (listItemCount > itemList.Count)
        {
            listItemCount = itemList.Count;
        }
        int count1 = listItemCount / 4;
        if (listItemCount % 4 > 0)
        {
            count1++;
        }
        contentPane.scrollView.totalItemCount = count1;
    }

    private void typeSelectChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        setType((kTreasureBoxInfoType)index);
    }

    public void setData(kTreasureBoxInfoType boxType, int boxId)
    {
        data = TreasureBoxDataProxy.inst.GetDataByBoxId(boxId);
        var boxCfg = ItemconfigManager.inst.GetConfig(data.boxItemId);
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(boxCfg.name);
        contentPane.boxIcon.SetSprite(boxCfg.atlas, boxCfg.icon);
        if (contentPane.group.selectedIndex == (int)boxType)
        {
            //typeSelectChange((int)boxType);
            contentPane.group.OnEnableMethod((int)boxType);
        }
        else
        {
            contentPane.group.selectedIndex = (int)boxType;
        }
        //setType(boxType);
    }

    public void setType(kTreasureBoxInfoType boxType)
    {
        curBoxType = boxType;
        if (boxType == kTreasureBoxInfoType.Exclusive)
        {
            showExclusicePanel();
        }
        else if (boxType == kTreasureBoxInfoType.Chance)
        {
            showChancePanel();
        }
    }

    private void showExclusicePanel()
    {
        //if (contentPane.group.NotNeedInvokeAction)
        //{
        //    contentPane.group.NotNeedInvokeAction = false;
        //    return;
        //}
        contentPane.exclusiveObj.SetActive(true);
        contentPane.infoObj.gameObject.SetActive(false);

        itemList = data.items;
        SetListItemTotalCount(itemList.Count);
    }

    private void showChancePanel()
    {
        contentPane.exclusiveObj.SetActive(false);
        contentPane.infoObj.gameObject.SetActive(true);

        contentPane.infoObj.setData(data.boxItemId);
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
