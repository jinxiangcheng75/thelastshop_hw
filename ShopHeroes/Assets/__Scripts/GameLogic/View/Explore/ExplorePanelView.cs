using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mosframe;
using DG.Tweening;

public class ExplorePanelView : ViewBase<ExplorePanelCom>
{
    public override string viewID => ViewPrefabName.ExplorePanelUI;
    public override string sortingLayerName => "window";
    List<ExploreGroup> groupData;
    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;

        AddUIEvent();
        InitComponent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });
    }

    private void InitComponent()
    {
        groupData = new List<ExploreGroup>();
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        contentPane.scrollView.scrollByItemIndex(0);
    }

    int listItemCount = 0;
    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        ExploreItemView itemScript = (ExploreItemView)obj;

        if (index >= listItemCount)
        {
            itemScript.gameObject.SetActive(false);
            return;
        }

        if (index < listItemCount)
        {
            itemScript.gameObject.SetActive(true);
            itemScript.setData(groupData[index], index);
        }
        else
        {
            itemScript.gameObject.SetActive(false);
        }
    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
            listItemCount = 0;
        contentPane.scrollView.totalItemCount = count;
    }

    public void SetDataList(List<ExploreGroup> listData)
    {
        groupData = listData;
        SetListItemTotalCount(groupData.Count);
    }

    public void setData()
    {
        //contentPane.scrollView.updateListItemInfo();
        //contentPane.scrollView.ScrollToTop();
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            if (FGUI.inst.isLandscape)
            {
                contentPane.scrollRect.horizontal = false;
            }
            else
            {
                contentPane.scrollRect.vertical = false;
            }
        }
        SetDataList(ExploreDataProxy.inst.ExploreList);
    }

    public void jumpToTargetExplore(int exploreGroupId)
    {
        int index = 0;
        for (int i = 0; i < groupData.Count; i++)
        {
            if (groupData[i].groupData.groupId == exploreGroupId)
            {
                index = i;
                break;
            }
        }
        contentPane.scrollView.scrollByItemIndex(index);
    }

    protected override void onShown()
    {
        UpdateUnoinBuffInfo();
    }

    public void UpdateUnoinBuffInfo()
    {
        foreach (var item in contentPane.unionBuffItems)
        {
            item.UpdateData();
        }
    }


    public override void shiftIn()
    {
        base.shiftIn();

        setData();
        //if (GameSettingManager.inst.needShowUIAnim)
        //    DoShowAnimation();
    }

    protected override void onHide()
    {

        contentPane.scrollView.totalItemCount = 0;
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        //contentPane.windowAnim.CrossFade("show", 0f);
        //contentPane.windowAnim.Update(0f);
        //contentPane.windowAnim.Play("show");
        //contentPane.maskObj.SetActiveTrue();
        ////contentPane.windowAnim.enabled = false;
        //contentPane.windowAnim.Update(0f);
        ////se = DOTween.Sequence().AppendInterval(0).AppendCallback(() =>
        ////{
        ////    contentPane.windowAnim.enabled = true;
        ////}).Play();

        //GameTimer.inst.AddTimer(groupData.Count * 0.12f + 0.3f, 1, () =>
        //  {
        //      //contentPane.windowAnim.enabled = false;
        //      contentPane.maskObj.SetActiveFalse();
        //  });
    }

    protected override void DoHideAnimation()
    {
        //contentPane.windowAnim.enabled = true;
        contentPane.windowAnim.Play("hide");
        float animTime = contentPane.windowAnim.GetClipLength("commonBagUI_hide");
        GameTimer.inst.AddTimer(animTime, 1, () =>
          {
              contentPane.windowAnim.CrossFade("null", 0f);
              contentPane.windowAnim.Update(0f);
              this.HideView();
          });
    }
}
