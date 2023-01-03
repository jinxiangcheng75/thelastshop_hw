using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryRecordView : ViewBase<LotteryRecordComp>
{
    public override string viewID => ViewPrefabName.LotteryRecordUI;
    public override string sortingLayerName => "popup";

    private kLotteryType lotteryType = kLotteryType.max;
    List<Recording> curRecordingList;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.group.OnSelectedIndexValueChange = ChangeWorldOrMyself;
        contentPane.scrollView.itemRenderer = this.listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = this.listitemRenderer;
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        ExhibitionItemUI item = (ExhibitionItemUI)obj;
        if (index < curRecordingList.Count)
        {
            item.gameObject.SetActive(true);
            item.setData(curRecordingList[index], lotteryType == kLotteryType.World);
        }
        else
        {
            item.gameObject.SetActive(false);
        }
    }

    private void ChangeWorldOrMyself(int index)
    {
        AudioManager.inst.PlaySound(11);
        lotteryType = (kLotteryType)index;
        SetExhibitionData();
    }

    public void SetExhibitionData()
    {
        //if (contentPane.group.NotNeedInvokeAction)
        //{
        //    contentPane.group.NotNeedInvokeAction = false;
        //    return;
        //}
        curRecordingList = LotteryDataProxy.inst.GetRecordingByType(lotteryType);

        contentPane.scrollView.totalItemCount = curRecordingList.Count;
        contentPane.scrollView.refresh();
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
        if (lotteryType == kLotteryType.max)
        {
            contentPane.group.OnEnableMethod(0);
        }
        else
        {
            contentPane.group.OnEnableMethod((int)lotteryType);
        }
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
