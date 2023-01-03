using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionFindToolUI : ViewBase<UnionFindToolUIComp>
{
    public override string viewID => ViewPrefabName.UnionFindToolUI;
    public override string sortingLayerName => "window";

    string outStr;
    int curIndex;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = false;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.createBtn.ButtonClickTween(onCreateBtnClick);
        contentPane.exitBtn.ButtonClickTween(onExitBtnClick);

        contentPane.toggleGroup.OnSelectedIndexValueChange = onSelectedIndexValueChange;
        contentPane.union_superList.scrollByItemIndex(0);
        contentPane.union_superList.itemRenderer = unionListitemRenderer;
        contentPane.union_superList.itemUpdateInfo = unionListitemRenderer;
        contentPane.player_superList.scrollByItemIndex(0);
        contentPane.player_superList.itemRenderer = playerListitemRenderer;
        contentPane.player_superList.itemUpdateInfo = playerListitemRenderer;

        contentPane.union_IF.onValueChanged.AddListener(onUnion_IFonValueChged);
        contentPane.player_IF.onValueChanged.AddListener(onPlayer_IFonValueChged);

    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
        contentPane.union_IF.text = "";
        contentPane.player_IF.text = "";

        bool noUnion = !UserDataProxy.inst.playerData.hasUnion;//未加入公会

        contentPane.createBtn.gameObject.SetActive(noUnion);
        contentPane.exitBtn.gameObject.SetActive(!noUnion);
    }

    private void onCreateBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_CREATEUNION);
    }

    void onExitBtnClick()
    {
        if (HotfixBridge.inst.HaveTimeLimitActivitySelfScore())
        {
            System.Action callback = () =>
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

    private void onSelectedIndexValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        foreach (var item in contentPane.toggleLinkObjs)
        {
            item.SetActive(false);
        }

        contentPane.toggleLinkObjs[index].SetActive(true);

        setState(index);

    }

    private void onUnion_IFonValueChged(string value)
    {
        if (string.IsNullOrEmpty(value) || WordFilter.inst.filter(value, out outStr, check_only: true))
        {
            unionItemDatas.Clear();
            setUnionListNum(0);
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_LIST, value);
    }

    private void onPlayer_IFonValueChged(string value)
    {
        if (string.IsNullOrEmpty(value) || WordFilter.inst.filter(value, out outStr, check_only: true))
        {
            playerItemDatas.Clear();
            setPlayerListNum(0);
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_FINDPLAYERLIST, value);
    }


    //设置状态
    private void setState(int index)
    {
        curIndex = index;
        switch (index)
        {
            case 0: setHotUnionData(); break;
            case 1: setFindUnionData(); break;
            case 2: setFindPlayerData(); break;
        }

    }

    private void setHotUnionData()
    {
        foreach (var item in contentPane.unionHotItems)
            item.gameObject.SetActive(false);

        contentPane.hot_loadingTips.gameObject.SetActive(true);
        contentPane.hot_nothingTipObj.gameObject.SetActive(false);

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_LIST, "");
    }

    public void GetUnionSimpleDatas(List<UnionSimpleData> datas)
    {
        if (curIndex == 0)
        {

            contentPane.hot_loadingTips.gameObject.SetActive(false);
            contentPane.hot_nothingTipObj.gameObject.SetActive(datas.Count == 0);

            for (int i = 0; i < contentPane.unionHotItems.Length; i++)
            {

                if (i < datas.Count)
                {
                    contentPane.unionHotItems[i].SetData(datas[i]);
                }
                else
                {
                    contentPane.unionHotItems[i].gameObject.SetActive(false);
                }

            }

        }
        else if (curIndex == 1)
        {
            unionItemDatas = datas;
            setUnionListNum(unionItemDatas.Count);
        }

    }

    public void GetFindPlayerDatas(List<UnionMemberData> datas)
    {
        if (curIndex == 2)
        {

            playerItemDatas = datas;
            setPlayerListNum(playerItemDatas.Count);
        }
    }

    private void setFindUnionData()
    {
        setUnionListNum(unionItemDatas.Count);
    }

    private void setFindPlayerData()
    {
        setPlayerListNum(playerItemDatas.Count);
    }


    //无限滑动
    List<UnionSimpleData> unionItemDatas = new List<UnionSimpleData>();
    List<UnionMemberData> playerItemDatas = new List<UnionMemberData>();

    private void unionListitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        UnionListItem item = (UnionListItem)obj;
        item.SetData(unionItemDatas[index], contentPane.union_IF.text, index);
    }

    private void playerListitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        PlayerListItem item = (PlayerListItem)obj;
        item.SetData(playerItemDatas[index], contentPane.player_IF.text, index);
    }

    private void setUnionListNum(int count)
    {
        contentPane.union_superList.totalItemCount = count;
        contentPane.union_nothingTips.gameObject.SetActive(count == 0);
    }

    private void setPlayerListNum(int count)
    {
        contentPane.player_superList.totalItemCount = count;
        contentPane.player_nothingTips.gameObject.SetActive(count == 0);
    }

    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
    }

}
