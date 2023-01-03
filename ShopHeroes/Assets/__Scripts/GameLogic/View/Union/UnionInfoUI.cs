using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnionInfoUI : ViewBase<UnionInfoUIComp>
{
    public override string viewID => ViewPrefabName.UnionInfoUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = false;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.member_superList.scrollByItemIndex(0);
        contentPane.member_superList.itemRenderer = memberListitemRenderer;
        contentPane.member_superList.itemUpdateInfo = memberListitemRenderer;

        contentPane.toggleGroup.OnSelectedIndexValueChange = onSelectedIndexValueChange;


        contentPane.joinUnionBtn.ButtonClickTween(onJoinUnionBtnClick);
        contentPane.setUnionBtn.ButtonClickTween(onSetUnionBtnClick);
        contentPane.findUnionBtn.ButtonClickTween(onFindUnionBtn);
        contentPane.exitUnionBtn.ButtonClickTween(onExitUnionBtn);

    }


    int _index;
    private void onSelectedIndexValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        _index = index;

        foreach (var item in contentPane.toggleLinkObjs)
        {
            item.SetActive(false);
        }

        contentPane.toggleLinkObjs[index].SetActive(true);

        if (index == 1)
        {
            contentPane.member_superList.totalItemCount = memberItemDatas.Count;
        }

    }

    UnionDetailInfo _data;
    public void SetData(UnionDetailInfo data)
    {
        _data = data;

        contentPane.unionLvTx.text = data.unionLevel.ToString();
        contentPane.unionNameTx.text = data.unionName;
        contentPane.unionUidTx.text = "#" + data.unionId;
        contentPane.memberNumTx.text = LanguageManager.inst.GetValueByKey("成员：") + "<color=#ffc54a>" + data.memberList.Count + "/" + data.memberNumLimit + "</color>";

        contentPane.netWorthTips.text = data.memberList.Sum(t => t.worth).ToString("N0");
        contentPane.unionPresidentTips.text = LanguageManager.inst.GetValueByKey(data.presidentNickName);
        contentPane.visitOpennessTips.text = LanguageManager.inst.GetValueByKey(data.enterSetting == (int)EUnionEnter.Personal ? "私人" : "完全公开");
        contentPane.lowestLvTips.text = data.enterLevel.ToString();
        contentPane.enterInvestTips.text = data.enterInvest.ToString("N0");
        memberItemDatas = data.memberList;

        bool selfUnion = data.unionId == UserDataProxy.inst.playerData.unionId;

        if (!selfUnion && !UserDataProxy.inst.playerData.hasUnion && data.enterSetting != (int)EUnionEnter.Personal)
        {
            contentPane.joinUnionBtn.gameObject.SetActive(true);

            GUIHelper.SetUIGray(contentPane.joinUnionBtn.transform, data.memberList.Count >= data.memberNumLimit || data.enterLevel > UserDataProxy.inst.playerData.level || data.enterInvest > UserDataProxy.inst.playerData.invest);

        }
        else
        {
            contentPane.joinUnionBtn.gameObject.SetActive(false);
        }

        contentPane.exitUnionBtn.gameObject.SetActive(selfUnion);
        contentPane.findUnionBtn.gameObject.SetActive(selfUnion);
        contentPane.setUnionBtn.gameObject.SetActive(selfUnion);

        if (selfUnion)
        {
            bool justCommon = UserDataProxy.inst.playerData.memberJob == (int)EUnionJob.Common;
            GUIHelper.SetUIGray(contentPane.setUnionBtn.transform, justCommon);
            contentPane.setUnionBtn.interactable = !justCommon;
            contentPane.setUnionBtn.GetComponent<ButtonEx>().enabled = !justCommon;
        }

        contentPane.toggleGroup.OnEnableMethod(_index);
    }

    private void onJoinUnionBtnClick()
    {
        if (_data.memberList.Count >= _data.memberNumLimit)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("该联盟已满员，请加入其它联盟！"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        if (_data.enterLevel > UserDataProxy.inst.playerData.level)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您需要至少是{0}级才能加入这个联盟。", _data.enterLevel.ToString()), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        if (_data.enterInvest > UserDataProxy.inst.playerData.invest)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您需要至少是{0}投资额才能加入这个联盟。", _data.enterInvest.ToString("N0")), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_MSGBOX_ENTER, _data.unionId, _data.unionName);
    }

    private void onSetUnionBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONSETSETTING);
    }

    private void onFindUnionBtn()
    {
        hide();
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONFINDTOOL);
    }

    private void onExitUnionBtn()
    {
        if (HotfixBridge.inst.HaveTimeLimitActivitySelfScore())
        {
            Action callback = () => 
            {
                EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_EXITUNIONMSGBOX);
            };
            HotfixBridge.inst.TriggerLuaEvent("ShowTipsAgainAffirmUI", LanguageManager.inst.GetValueByKey("提示"), LanguageManager.inst.GetValueByKey("活动期间已累计的积分将清零"), callback);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_EXITUNIONMSGBOX);
        }

    }

    //无限滑动
    List<UnionMemberInfo> memberItemDatas;

    private void memberListitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        MemberListItem item = (MemberListItem)obj;
        item.SetData(memberItemDatas[index], _data.unionId);
    }

    protected override void onShown()
    {
        base.onShown();
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
    }
}
