using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopSubUIComp : MonoBehaviour
{
    public Button acheivementBtn;
    public Button emailBtn;
    public Button moneyBoxBtn;
    public Button settingBtn;
    public Button bgBtn;
    public Button rankBtn;
    public Button exchangeBtn;
    public GameObject emailRedPoint;
    public GameObject acheivementRedPoint;
    public GameObject moneyBoxRedPoint;
    public GameObject settingRedPoint;
}

public class TopSubUIView : ViewBase<TopSubUIComp>
{

    public override string viewID => ViewPrefabName.TopSubUI;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "popup_2";

    protected override void onInit()
    {
        base.onInit();
        //topResPanelType = TopPlayerShowType.all;
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.none;
        contentPane.acheivementBtn.ButtonClickTween(() =>
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTUI);
        });

        contentPane.emailBtn.ButtonClickTween(() =>
        {
            hide();
            //EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_DATA);
            EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_EmailMainUI);
        });

        contentPane.moneyBoxBtn.ButtonClickTween(() =>
        {
            if (WorldParConfigManager.inst.GetConfig(138) == null) return;
            int moneyBoxUnLockLv = (int)WorldParConfigManager.inst.GetConfig(138).parameters;
            if (UserDataProxy.inst.playerData.level < moneyBoxUnLockLv)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", moneyBoxUnLockLv.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                return;
            }
            if (MoneyBoxDataProxy.inst.moneyBoxData == null) return;
            hide();
            EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_SHOWUI);
        });

        contentPane.settingBtn.ButtonClickTween(() =>
        {
            hide();
            if (GuideDataProxy.inst.CurInfo.isAllOver)
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SETTINGPANEL);
        });

        contentPane.bgBtn.onClick.AddListener(() =>
        {
            hide();
        });

        contentPane.rankBtn.ButtonClickTween(() =>
        {
            int minLv = 999;
            if (WorldParConfigManager.inst.GetConfig(341) != null)
                minLv = (int)WorldParConfigManager.inst.GetConfig(341).parameters;
            if (UserDataProxy.inst.playerData.level < minLv)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", minLv.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                return;
            }
            hide();
            HotfixBridge.inst.TriggerLuaEvent("Request_RankList", 1);
        });

        contentPane.exchangeBtn.ButtonClickTween(() =>
        {
            // 打开兑换界面
            hide();
            HotfixBridge.inst.TriggerLuaEvent("Open_ExchangeUI");
        });
    }

    protected override void onShown()
    {
        UpdateRedDots();
        int limitLv = MoneyBoxDataProxy.inst.moneyBoxData != null ? MoneyBoxDataProxy.inst.moneyBoxData.startMinLv : 1;
        GUIHelper.SetUIGray(contentPane.moneyBoxBtn.transform, UserDataProxy.inst.playerData.level < limitLv);

        EventController.inst.AddListener(GameEventType.UpdateGameRedPoints, UpdateRedDots);

        if (WorldParConfigManager.inst.GetConfig(341) != null)
        {
            var minLv = WorldParConfigManager.inst.GetConfig(341).parameters;
            GUIHelper.SetUIGray(contentPane.rankBtn.transform, UserDataProxy.inst.playerData.level < minLv);
        }

        var exchangeCfg = WorldParConfigManager.inst.GetConfig(8400);
        if(exchangeCfg != null)
        {
            contentPane.exchangeBtn.gameObject.SetActive(exchangeCfg.parameters == 1);
        }
        else
        {
            contentPane.exchangeBtn.gameObject.SetActive(false);
        }
    }

    protected override void onHide()
    {
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateRedDots);
    }

    protected override void beforeDispose()
    {
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateRedDots);
    }

    void UpdateRedDots()
    {
        contentPane.emailRedPoint.SetActive(EmailDataProxy.inst.needShowRedPoint);
        contentPane.acheivementRedPoint.SetActive(AcheivementDataProxy.inst.NeedRedPoint);
        //contentPane.moneyBoxRedPoint.SetActive(MoneyBoxDataProxy.inst.NeedShowRedPoint);
        contentPane.settingRedPoint.SetActive(AccountDataProxy.inst.currbindingType != EBindingType.None && AccountDataProxy.inst.bindingClaimState);
    }

}
