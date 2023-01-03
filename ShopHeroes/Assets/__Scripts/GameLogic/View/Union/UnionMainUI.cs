using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnionMainUI : ViewBase<UnionMainUIComp>
{

    public override string viewID => ViewPrefabName.UnionMainUI;

    public override string sortingLayerName => "window";

    protected override void onInit()
    {

        contentPane.selfUnionTokenBtn.ButtonClickTween(onSelfUnionTokenBtnClick);
        contentPane.unionCoinBtn.ButtonClickTween(onUnionCoinBtnClick);

        contentPane.unionBuffDetailBgBtn.onClick.AddListener(() =>
        {
            clearBuffDetailTimer();
            contentPane.unionBuffDetailBgBtn.gameObject.SetActiveFalse();
        });

        contentPane.tipMaskBtn.onClick.AddListener(onTipMaskBtnClick);

        contentPane.cityBtn.ButtonClickTween(onCityBtnClick);
        contentPane.findBtn.ButtonClickTween(onFindBtnClick);
        contentPane.aidBtn.ButtonClickTween(onAidBtnClick);
        contentPane.taskBtn.ButtonClickTween(onTaskBtnClick);
        contentPane.detailBtn.onClick.AddListener(onDetailBtnClick);
        contentPane.wealBtn.ButtonClickTween(onWealBtnClick);
    }

    protected override void onShown()
    {
        SetUnoinDetailInfo();
        UpdateTokensMess();
        UpdateUnoinBuffInfo();
        RefreshAidRedPointMess();
        setButtonFlag();
    }

    public void SetUnoinDetailInfo()
    {
        contentPane.unionLvTx.text = UserDataProxy.inst.unionDetailInfo.unionLevel.ToString();
        contentPane.unionNameTx.text = UserDataProxy.inst.unionDetailInfo.unionName;
        //contentPane.unionNameBgTf.sizeDelta = new Vector2(contentPane.unionNameTx.preferredWidth + 60, 56);
        contentPane.unionMemberNumTx.text = LanguageManager.inst.GetValueByKey("成员数") + " <color=#ffc000>" + UserDataProxy.inst.unionDetailInfo.memberList.Count + "/" + UserDataProxy.inst.unionDetailInfo.memberNumLimit + "</color>";
        contentPane.worthTx.text = LanguageManager.inst.GetValueByKey("净价值") + " <color=#ffc000>" + UserDataProxy.inst.unionDetailInfo.memberList.Sum(t => t.worth).ToString("N0") + "</color>";
    }

    public void UpdateTokensMess()
    {
        contentPane.selfUnionTokenTx.text = UserDataProxy.inst.playerData.unionCoin.ToString("N0");
        contentPane.unionCoinTx.text = UserDataProxy.inst.Union_uCoin.ToString("N0");
    }

    public void UpdateUnoinBuffInfo()
    {
        foreach (var item in contentPane.unionBuffItems)
        {
            item.UpdateData();
        }

        clearBuffDetailTimer();
        contentPane.unionBuffDetailBgBtn.gameObject.SetActiveFalse();
    }

    UnionBuffData _curBuffData;
    public void ShowBuffDetailInfo(RectTransform rect, UnionBuffData buffData)
    {
        Vector3 pos = new Vector3(rect.anchoredPosition.x - 50f, contentPane.unionBuffDetailTf.anchoredPosition.y, 0);
        if (pos.x + contentPane.unionBuffDetailTf.sizeDelta.x >= Screen.width)
        {
            pos.x = rect.anchoredPosition.x - contentPane.unionBuffDetailTf.sizeDelta.x + 50f;
            contentPane.unionBuffDetailBgTf.localScale = Vector3.one;
        }
        else
        {
            contentPane.unionBuffDetailBgTf.localScale = new Vector3(-1, 1, 1);
        }

        contentPane.unionBuffDetailTf.anchoredPosition = pos;
        contentPane.unionBuffDetailBgBtn.gameObject.SetActiveTrue();

        _curBuffData = buffData;
        contentPane.unionBuffNameTx.text = LanguageManager.inst.GetValueByKey(buffData.config.name);
        contentPane.unionBuffDesTx.text = LanguageManager.inst.GetValueByKey(buffData.config.skill_desc, "<color=#4dff6e>" + buffData.config.add2_num.ToString() + "</color>");
        contentPane.unionBuffFillBgTf.anchorMax = new Vector2(buffData.config.now_level / 5.0f, 1);

        setBuffDetailTimer();

    }

    LoopEventcomp buffDetailTimerComp;

    void clearBuffDetailTimer()
    {
        if (buffDetailTimerComp != null)
        {
            GameTimer.inst.removeLoopTimer(buffDetailTimerComp);
            buffDetailTimerComp = null;
        }
    }


    void setBuffDetailTimer()
    {

        clearBuffDetailTimer();

        int ticks = _curBuffData.remainTime;

        contentPane.unionBuffCountdownTx.text = TimeUtils.timeSpanStrip(_curBuffData.remainTime);
        contentPane.unoinBuffSlider.value = _curBuffData.remainTime / (_curBuffData.config.skill_time * 60f);

        buffDetailTimerComp = GameTimer.inst.AddLoopTimerComp(contentPane.unionBuffCountdownTx.gameObject, 1, () => 
        {
            contentPane.unionBuffCountdownTx.text = TimeUtils.timeSpanStrip(_curBuffData.remainTime);
            contentPane.unoinBuffSlider.value = _curBuffData.remainTime / (_curBuffData.config.skill_time * 60f); //_curBuffData.serverData.time / (_curBuffData.config.skill_time * 60 * _curBuffData.config.now_level) * (_curBuffData.config.now_level / 5.0f);
        });
    }

    private void onTipMaskBtnClick()
    {
        contentPane.tipMaskBtn.gameObject.SetActive(false);
        contentPane.curOpenTipObj.SetActive(false);
    }


    void onSelfUnionTokenBtnClick()
    {
        contentPane.selfUnionCoinTipObj.SetActive(true);
        contentPane.curOpenTipObj = contentPane.selfUnionCoinTipObj;
        contentPane.tipMaskBtn.gameObject.SetActive(true);
    }

    void onUnionCoinBtnClick()
    {
        contentPane.unionCoinTipObj.SetActive(true);
        contentPane.curOpenTipObj = contentPane.unionCoinTipObj;
        contentPane.tipMaskBtn.gameObject.SetActive(true);
    }

    void onCityBtnClick()
    {
        HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
    }

    void onFindBtnClick()
    {
        var buildingData = UserDataProxy.inst.GetBuildingData(2200);
        if (buildingData.state == (int)EBuildState.EB_Lock)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("建筑物 {0} 尚未解锁", LanguageManager.inst.GetValueByKey(buildingData.config.name)), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONFINDTOOL);
    }

    void onAidBtnClick()
    {
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionAidUIView");
    }

    void onTaskBtnClick()
    {
        int unionTaskUnlockLv = (int)WorldParConfigManager.inst.GetConfig(2030).parameters;
        if (UserDataProxy.inst.playerData.level < unionTaskUnlockLv)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", unionTaskUnlockLv.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
            return;
        }

        HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionTaskUIView");
    }

    void onDetailBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONINFO);
    }

    void onWealBtnClick()
    {
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionWealUIView");
    }

    public void RefreshAidRedPointMess()
    {
        var showList = UserDataProxy.inst.union_canAidList;

        if (showList.Count == 0)
        {
            contentPane.aidRedPointObj.SetActive(false);
        }
        else
        {
            contentPane.aidRedPointObj.SetActive(true);
            contentPane.aidRedPointNumTx.text = showList.Count.ToString();
        }

    }

    void setButtonFlag()
    {
        bool unionScienceFlag = WorldParConfigManager.inst.GetConfig(3001).parameters == 1;
        contentPane.wealBtn.gameObject.SetActive(unionScienceFlag);
    }


}
