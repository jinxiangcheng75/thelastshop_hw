using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//科学院UI
public class ScienceLabUI : ViewBase<ScienceLabUIComp>
{
    public override string viewID => ViewPrefabName.ScienceLabUI;

    public override string sortingLayerName => "window";


    protected override void onInit()
    {
        base.onInit();

        topResPanelType = TopPlayerShowType.noSettingAndEnergy;
        isShowResPanel = true;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.superList.itemRenderer = listitemRenderer;
        contentPane.superList.itemUpdateInfo = listitemRenderer;
        contentPane.superList.scrollByItemIndex(0);
        //contentPane.superList.totalItemCount = 0;


    }

    protected override void onShown()
    {
        base.onShown();

        Refresh();
    }

    void getCanShowBuildingDatas() 
    {
        list = UserDataProxy.inst.GetAllCanShowScienceBuildingData();

    }

    public void Refresh()
    {
        getCanShowBuildingDatas();
        curListCount = list.Count;
        SetListItemTotalCount(curListCount);
        contentPane.superList.ScrollToTop();
    }

    //无限滑动

    int listItemCount = 0;
    int curListCount;
    List<CityBuildingData> list;

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

                WorkerInvestItem item = itemScript.buttonList[i].GetComponent<WorkerInvestItem>();
                SetItemData(ref item, itemIndex);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    void SetItemData(ref WorkerInvestItem item, int itemIndex)
    {
        item.SetData(list[itemIndex]);
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
    }



    protected override void onHide()
    {
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_BUILDINGUPREFRESH, 2300);
    }

}
