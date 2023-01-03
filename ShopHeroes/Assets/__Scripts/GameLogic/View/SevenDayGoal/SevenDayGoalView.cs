using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mosframe;
using DG.Tweening;

public class SevenDayGoalView : ViewBase<SevenDayGoalComp>
{
    public override string viewID => ViewPrefabName.SevenDayGoalUI;
    public override string sortingLayerName => "window";

    int listItemCount = 0;
    SevenDayGoalData curData;
    CommonRewardData commonData;
    List<SevenDayGoalSingle> tasks;
    //SevenDayAwardItem lastItem;
    int curIndex = 0;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = false;
        InitComponent();
        AddUIEvent();
    }

    private void InitComponent()
    {
        tasks = new List<SevenDayGoalSingle>();
        contentPane.group.OnSelectedIndexValueChange = typeSelectedChange;
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });
        contentPane.rewardTrans.ButtonClickTween(() =>
        {
            if (curData == null) return;
            if (curData.listState == ESevenDayTaskState.CanReward)
            {
                EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYLISTAWARD, curData.id);
            }
            else if (curData.listState == ESevenDayTaskState.Doing || curData.listState == ESevenDayTaskState.NotUnlock)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("成功获取当日所有奖励可领取大奖"), GUIHelper.GetColorByColorHex("FF2828"));
            }
            //else if (curData.listState == ESevenDayTaskState.Doing || curData.listState == ESevenDayTaskState.NotUnlock)
            //{
            //    // 介绍事件
            //    EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, curData.cfg.reward, contentPane.rewardTrans.transform, (long)curData.cfg.reward_number);
            //}
        });
        contentPane.rewardBtn.ButtonClickTween(() =>
        {
            if (curData.listState != ESevenDayTaskState.Rewarded)
            {
                EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, contentPane.tipsTrans.transform);
            }
        });
    }

    public void RefreshData()
    {
        setDayToggleData();
        //setAwardItemListData();
        typeSelectedChange(contentPane.group.selectedIndex);
    }

    //private void setAwardItemListData()
    //{
    //    var proxy = SevenDayGoalDataProxy.inst;
    //    for (int i = 0; i < contentPane.awardItemList.Count; i++)
    //    {
    //        int index = i;
    //        contentPane.awardItemList[index].setData(proxy.GetDataByDayIndex(index + 1));
    //    }
    //}

    private void setDayToggleData()
    {
        for (int i = 0; i < contentPane.toggleItems.Count; i++)
        {
            int index = i;
            var toggleData = SevenDayGoalDataProxy.inst.GetDataByDayIndex(index + 1);
            contentPane.toggleItems[index].setItemState(toggleData);
        }
    }

    private void setCurDateData()
    {
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(curData.cfg.title);
        contentPane.listDescText.text = LanguageManager.inst.GetValueByKey(curData.cfg.slogan);

        if (curData.cfg.reward_number > 1)
            contentPane.rewardNumText.text = "x" + AbbreviationUtility.AbbreviateNumber(curData.cfg.reward_number);
        else
            contentPane.rewardNumText.text = "∞";
        //contentPane.rewardNumText.enabled = curData.listState != ESevenDayTaskState.Rewarded;

        var itemCfg = ItemconfigManager.inst.GetConfig(curData.cfg.reward);
        if ((ItemType)itemCfg.type == ItemType.EquipmentDrawing)
        {
            contentPane.tuzhiImg.enabled = true;
            var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemCfg.effect);
            contentPane.rewardIcon.SetSprite(equipCfg.atlas, equipCfg.icon);
        }
        else
        {
            contentPane.tuzhiImg.enabled = false;
            contentPane.rewardIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
        }

        contentPane.lockImg.enabled = curData.listState == ESevenDayTaskState.NotUnlock;
        contentPane.gouImg.enabled = curData.listState == ESevenDayTaskState.Rewarded;
        contentPane.effectObj.SetActive(curData.listState == ESevenDayTaskState.CanReward);
        GUIHelper.SetUIGray(contentPane.rewardBtn.transform, (curData.listState == ESevenDayTaskState.Rewarded || curData.listState == ESevenDayTaskState.NotUnlock));
        contentPane.rewardTrans.gameObject.SetActive(curData.listState == ESevenDayTaskState.Doing || curData.listState == ESevenDayTaskState.CanReward);
        GUIHelper.SetUIGray(contentPane.rewardTrans.transform, curData.listState == ESevenDayTaskState.Doing);
        contentPane.promptText.enabled = curData.listState == ESevenDayTaskState.Rewarded || curData.listState == ESevenDayTaskState.NotUnlock;

        if (curData.listState == ESevenDayTaskState.Rewarded)
        {
            contentPane.promptText.text = LanguageManager.inst.GetValueByKey("已领取");
            contentPane.promptText.color = GUIHelper.GetColorByColorHex("#7deb60");
        }
        else if (curData.listState == ESevenDayTaskState.NotUnlock)
        {
            contentPane.promptText.text = LanguageManager.inst.GetValueByKey("未开启");
            contentPane.promptText.color = Color.white;
        }

        if (curData.listState == ESevenDayTaskState.CanReward)
            contentPane.btnIcon.SetSprite("__common_1", "icon_lvse");
        else
            contentPane.btnIcon.SetSprite("__common_1", "icon_huise");

        contentPane.rewardBtnText.text = LanguageManager.inst.GetValueByKey(curData.listState == ESevenDayTaskState.Rewarded ? "已领取" : "领取");

        if (curData.listState == ESevenDayTaskState.NotUnlock)
        {
            contentPane.descText.text = LanguageManager.inst.GetValueByKey("未解锁");
            contentPane.descText.color = GUIHelper.GetColorByColorHex("#ff3e3e");
        }
        else
        {
            int doneCount = curData.GetCountByState(ESevenDayTaskState.Rewarded);
            doneCount += curData.GetCountByState(ESevenDayTaskState.VIPRewarded);
            contentPane.descText.text = LanguageManager.inst.GetValueByKey("今日试炼已完成 ") + doneCount + "/" + curData.sevenDayDic.Count;
            contentPane.descText.color = Color.white;
        }


    }

    public void JumpTarget(int targetId)
    {
        var cfg = SevenDayTaskConfigManger.inst.GetConfig(targetId);
        contentPane.group.selectedIndex = cfg.day - 1;
        //typeSelectedChange(cfg.day - 1);
        var tempList = SevenDayGoalDataProxy.inst.GetDataByDayIndex(cfg.day).sevenDayList;
        int targetIndex = tempList.FindIndex(t => t.id == targetId);
        contentPane.scrollView.scrollByItemIndex(targetIndex);
    }

    public void typeSelectedChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        curData = SevenDayGoalDataProxy.inst.GetDataByDayIndex(index + 1);
        commonData = new CommonRewardData(curData.cfg.reward, curData.cfg.reward_number, 1, curData.cfg.type);
        if (curData == null)
        {
            return;
        }
        if (curData.listState == ESevenDayTaskState.NotUnlock && curData.cfg.day != SevenDayGoalDataProxy.inst.curDay + 1)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("第{0}天解锁", curData.cfg.day.ToString()), GUIHelper.GetColorByColorHex("FF2828"));
            contentPane.group.selectedIndex = curIndex;
            return;
        }
        curIndex = index;
        //if (lastItem != null)
        //    lastItem.setSelectObj(false);
        //contentPane.awardItemList[index].setSelectObj(true);
        //lastItem = contentPane.awardItemList[index];
        tasks = curData.sevenDayList;
        SetListItemTotalCount(tasks.Count);
        setCurDateData();
        contentPane.scrollView.ScrollToTop();
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        SevenDayTaskItem item = (SevenDayTaskItem)obj;

        if (index >= listItemCount)
        {
            item.gameObject.SetActive(false);
        }

        if (index < listItemCount)
        {
            item.gameObject.SetActive(true);
            item.setData(tasks[index]);
        }
        else
        {
            item.gameObject.SetActive(false);
        }
    }

    private void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }

        contentPane.scrollView.totalItemCount = count;
    }

    protected override void onShown()
    {

        contentPane.group.OnEnableMethod(SevenDayGoalDataProxy.inst.curDay - 1);
        //typeSelectedChange(/*contentPane.group.selectedIndex*/SevenDayGoalDataProxy.inst.curDay - 1);
        //setAwardItemListData();
        setDayToggleData();

        if (SevenDayGoalDataProxy.inst.curDay > 2)
        {
            contentPane.toggleRect.horizontalNormalizedPosition = 1;
        }
        else
        {
            contentPane.toggleRect.horizontalNormalizedPosition = 0;
        }
    }

    protected override void onHide()
    {

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();

    }
}
