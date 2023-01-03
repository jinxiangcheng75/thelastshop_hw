using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailUI : ViewBase<EmailUIComp>
{

    public override string viewID => ViewPrefabName.EmailUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.feedbackBtn.ButtonClickTween(onFeedbackBtnClick);
        contentPane.allReceiveBtn.ButtonClickTween(onAllReceiveBtnClick);

        contentPane.superList.itemRenderer = listitemRenderer;
        contentPane.superList.itemUpdateInfo = listitemRenderer;
        contentPane.superList.scrollByItemIndex(0);

    }

    protected override void onShown()
    {

        Refresh();
    }

    public void Refresh()
    {
        if (EmailDataProxy.inst == null) return;
        emailItemList = EmailDataProxy.inst.GetAllEmailDatas();

        if (emailItemList.Count == 0)
        {
            contentPane.nothingTipsObj.SetActiveTrue();
        }
        else
        {
            contentPane.nothingTipsObj.SetActiveFalse();
        }

        contentPane.superList.totalItemCount = emailItemList.Count;
        contentPane.superList.ScrollToTop();
    }


    //反馈
    void onFeedbackBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_FeedbackUI);
    }

    //一键领取
    void onAllReceiveBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_ALLREAD);
    }


    ///无限滑动
    List<EmailData> emailItemList;

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        EmailItem item = obj as EmailItem;
        item.SetData(emailItemList[index]);
    }

    protected override void onHide()
    {

    }
}
