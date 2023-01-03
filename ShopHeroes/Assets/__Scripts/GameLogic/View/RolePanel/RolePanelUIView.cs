using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RolePanelUIView : ViewBase<RolePanelComp>
{
    public override string viewID => ViewPrefabName.RolePanelUI;
    public override int showType => (int)ViewShowType.pullUp;
    public override string sortingLayerName => "window";

    public kRoleType roleType = kRoleType.max;
    private List<RoleHeroData> heroDatas;
    private List<WorkerData> workerDatas;
    const int itemCountPerRow = 3;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;
        AddUIEvent();
        InitComponent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
    }

    private void InitComponent()
    {
        heroDatas = new List<RoleHeroData>();
        workerDatas = new List<WorkerData>();
        contentPane.topToggleGroup.OnSelectedIndexValueChange = typeSelectedChange;
        //contentPane.topToggleGroup.SetToggleSize(new Vector2(326, 170), new Vector2(326, 138));
        if (contentPane.scrollView != null)
        {
            contentPane.scrollView.itemRenderer = listitemRenderer;
            //contentPane.scrollView.itemUpdateInfo = listitemRenderer;
            contentPane.scrollView.activeFalse = this.activeFalseRenderer;
        }
    }

    public override void shiftIn()
    {
        base.shiftIn();
        //RoleDataProxy.inst.ResetHeroGraphicDressCache();
        roleType = kRoleType.max;
        typeSelectedChange(contentPane.topToggleGroup.selectedIndex);
        setTextData();
    }

    public void toggleIndexSet(int index)
    {
        //contentPane.topToggleGroup.selectedIndex = index;
        contentPane.topToggleGroup.OnEnableMethod(index);
    }

    public void typeSelectedChange(int index)
    {
        //if (contentPane.topToggleGroup.NotNeedInvokeAction)
        //{
        //    contentPane.topToggleGroup.NotNeedInvokeAction = false;
        //    return;
        //}
        //if (roleType == (kRoleType)index) return;
        AudioManager.inst.PlaySound(11);
        roleType = (kRoleType)index;
        if (roleType == kRoleType.Hero)
        {
            heroDatas = RoleDataProxy.inst.HeroListSort;
            curListCount = heroDatas.Count;
            SetListItemTotalCount(curListCount);
        }
        else if (roleType == kRoleType.Artisan)
        {
            workerDatas = RoleDataProxy.inst.WorkerList.FindAll(t => t.canShow);
            workerDatas.Sort();
            curListCount = workerDatas.Count;
            SetListItemTotalCount(curListCount);
            contentPane.worker_redPointObj.SetActive(RoleDataProxy.inst.workerRedPointShow);
        }

        if (!FGUI.inst.isLandscape)
        {
            contentPane.scrollRect.vertical = true;
        }
        else
        {
            contentPane.scrollRect.horizontal = true;
        }
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            if (!FGUI.inst.isLandscape)
            {
                contentPane.scrollRect.vertical = false;
            }
            else
            {
                contentPane.scrollRect.horizontal = false;
            }
        }

    }

    int listItemCount = 0;
    int curListCount;
    int allCount = 0;
    bool fieldState = false;
    bool haveResting = false;
    bool haveFieldOrNotMax = false;
    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < itemCountPerRow; ++i)
        {
            int itemIndex = index * itemCountPerRow + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.name = "item";
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);
                RoleListItemUI roleItem = itemScript.buttonList[i].GetComponent<RoleListItemUI>();
                if (roleType == kRoleType.Hero)
                {
                    if (itemIndex < allCount)
                    {
                        if ((itemIndex == 1 && haveFieldOrNotMax) || (itemIndex == 0 && !haveFieldOrNotMax))
                        {
                            roleItem.InitExchangeHeroData();
                            continue;
                        }
                        if ((itemIndex == 2 && haveFieldOrNotMax) || (itemIndex == 1 && !haveFieldOrNotMax))
                        {
                            roleItem.InitRecruitData();
                            continue;
                        }
                        if (itemIndex == 0 && haveFieldOrNotMax)
                        {
                            if (fieldState)
                                roleItem.InitAddNew();
                            else
                                roleItem.InitAddFieldData();
                            continue;
                        }
                        if ((haveResting && itemIndex == 3) || (haveResting && itemIndex == 2 && !haveFieldOrNotMax))
                        {
                            roleItem.InitAllRestingData();
                            continue;
                        }
                        //roleItem.InitRecruitData();
                    }
                    else
                    {
                        var tempData = heroDatas[itemIndex - allCount];
                        roleItem.InitHeroData(tempData);
                    }
                }
                else if (roleType == kRoleType.Artisan)
                {
                    roleItem.InitWorkerData(workerDatas[itemIndex]);
                }
            }
            else
            {
                itemScript.buttonList[i].gameObject.name = "item";
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void activeFalseRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList item = (BtnList)obj;
        for (int i = 0; i < itemCountPerRow; ++i)
        {
            item.buttonList[i].gameObject.name = "item";
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

        allCount = 0;
        if (roleType == kRoleType.Hero)
        {
            //fieldState = RoleDataProxy.inst.JudgeHeroFieldIsMax;
            //if (!fieldState)
            //{

            //}

            listItemCount += 2;
            allCount += 2;

            if (!RoleDataProxy.inst.JudgeHeroFieldIsMax || RoleDataProxy.inst.FieldNumAbtractHeroNum > 0)
            {
                listItemCount += 1;
                allCount += 1;
                haveFieldOrNotMax = true;

                if (RoleDataProxy.inst.FieldNumAbtractHeroNum > 0 || RoleDataProxy.inst.JudgeHeroFieldIsMax)
                {
                    fieldState = true; // addNew
                }
                else if (RoleDataProxy.inst.FieldNumAbtractHeroNum <= 0 && !RoleDataProxy.inst.JudgeHeroFieldIsMax)
                {
                    fieldState = false; // addField
                }
            }
            else
            {
                haveFieldOrNotMax = false;
            }

            haveResting = RoleDataProxy.inst.GetRestingStateHeroCount().Count > 0;
            if (haveResting)
            {
                listItemCount++;
                allCount++;
            }
            //int result = RoleDataProxy.inst.FieldNumAbtractHeroNum;
            //if (result > 0)
            //{
            //    listItemCount += result;
            //    allCount += result;
            //}
        }

        int count1 = listItemCount / itemCountPerRow;
        if (listItemCount % itemCountPerRow > 0)
        {
            count1++;
        }
        contentPane.scrollView.totalItemCount = count1;
    }

    protected override void onShown()
    {
        //RoleDataProxy.inst.ResetHeroGraphicDressCache();

        setData();
    }

    public void setData()
    {
        typeSelectedChange(contentPane.topToggleGroup.selectedIndex);
        setTextData();
    }


    protected override void onHide()
    {
        RoleDataProxy.inst.ClearHeroGraphicDressCache();
    }

    private void setTextData()
    {
        string heroText = RoleDataProxy.inst.GetIdleStateHeroCount + "/" + RoleDataProxy.inst.HeroList.Count;
        int workerNum = RoleDataProxy.inst.WorkerList.FindAll(t => t.state == EWorkerState.Unlock).Count;
        contentPane.heroNumText.text = heroText;
        contentPane.selectHeroNumText.text = heroText;
        contentPane.workerNumText.text = workerNum + "/" + RoleDataProxy.inst.WorkerList.FindAll(t => t.canShow).Count;
        contentPane.selectWorkerNumText.text = workerNum + "/" + RoleDataProxy.inst.WorkerList.FindAll(t => t.canShow).Count;

        contentPane.worker_redPointObj.SetActive(RoleDataProxy.inst.workerRedPointShow);
        contentPane.hero_redPointObj.SetActive((RoleDataProxy.inst.costValue == 0 && RoleDataProxy.inst.recruitIsNew) || (RoleDataProxy.inst.hasCanExchangeHero && RoleDataProxy.inst.exchangeIsNew) || (RoleDataProxy.inst.hasCanBuyField && RoleDataProxy.inst.FieldNumAbtractHeroNum <= 0));
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
