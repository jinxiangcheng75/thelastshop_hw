using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtendingPanelComp : MonoBehaviour
{
    public Button closeBtn;
    public Button immediatelyBtn;
    public Button finishBtn;
    public Button confirmBtn;

    public Text timeText;
    public Text diamCountText1;
    public Text diamCountText2;
    public Slider remainTimeSlider;
}

public class ExtendingPanelView : ViewBase<ExtendingPanelComp>
{
    public override string viewID => ViewPrefabName.ExtendingPanelView;
    public override string sortingLayerName => "window";

    private int state = 0; //扩建中 =0 ，扩建完成 = 1
    private int maxtime = 0;
    public int remainTimerId = 0;
    public int remainTime { private get; set; }
    public int sliderValueTime = 0;
    private int immediatelyDiamCount = 0;

    //当前阶段的商店尺寸配置
    private ExtensionConfig oldConfig = null;

    //下一阶段的商店尺寸配置
    private ExtensionConfig newConfig = null;

    protected override void onInit()
    {
        var c = contentPane;
        c.closeBtn.ButtonClickTween(hide);
        c.immediatelyBtn.onClick.AddListener(() => c.confirmBtn.gameObject.SetActiveTrue());
        c.confirmBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_POST_FINISHEXTENSION);
            hide();
        });

        contentPane.finishBtn.onClick.AddListener(() =>
        {
            IndoorMapEditSys.inst.shopUpgradeFinish();
        });
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
        oldConfig = ExtensionConfigManager.inst.GetExtensionConfig(UserDataProxy.inst.shopData.shopLevel);
        newConfig = ExtensionConfigManager.inst.GetExtensionConfig(UserDataProxy.inst.shopData.shopLevel + 1);

        // remainTime = ShopDesignDataProxy.inst.mShopUpgradingAttacher.getRemainTime();

        contentPane.immediatelyBtn.gameObject.SetActiveTrue();
        contentPane.confirmBtn.gameObject.SetActiveFalse();

        ShowInfo();
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
        GameTimer.inst.RemoveTimer(remainTimerId);
        state = 0;
        DOTween.Pause("Do_1");
    }

    public void ShowInfo()
    {
        maxtime = UserDataProxy.inst.shopData.stateTime;
        contentPane.remainTimeSlider.maxValue = maxtime;
        sliderValueTime = maxtime - UserDataProxy.inst.shopData.leftTime;
        remainTime = UserDataProxy.inst.shopData.leftTime;
        contentPane.remainTimeSlider.value = sliderValueTime;
        TimeUpdate();
        remainTimerId = GameTimer.inst.AddTimer(1, TimeUpdate);
        contentPane.remainTimeSlider.DOValue(contentPane.remainTimeSlider.maxValue, remainTime).SetEase(Ease.Linear).SetId("Do_1");
    }

    private void TimeUpdate()
    {
        if (!isShowing) return;

        if (remainTime > 0)
        {
            SetState(1);
            sliderValueTime++;
            remainTime--;
            contentPane.timeText.text = TimeUtils.timeSpanStrip(remainTime);
            //刷新钻石消耗
            immediatelyDiamCount = DiamondCountUtils.GetFurnitureUpgradeDiamonds(remainTime);
            contentPane.diamCountText1.text = immediatelyDiamCount.ToString("N0");
            contentPane.diamCountText1.color = UserDataProxy.inst.playerData.gem >= immediatelyDiamCount ? Color.white : GUIHelper.GetColorByColorHex("FD4F4F");
            contentPane.diamCountText2.text = immediatelyDiamCount.ToString("N0");

            contentPane.immediatelyBtn.interactable = UserDataProxy.inst.playerData.gem >= immediatelyDiamCount;
        }
        else
        {
            //完成升级
            SetState(2);
        }
    }

    public void SetState(int state)
    {
        if (state == 1)
        {
            this.state = 1;
        }
        else if (state == 2)
        {
            this.state = 2;
            contentPane.timeText.text = LanguageManager.inst.GetValueByKey("扩建完成！");
            GameTimer.inst.RemoveTimer(remainTimerId);
        }

        contentPane.immediatelyBtn.gameObject.SetActive(this.state == 1);
        contentPane.finishBtn.gameObject.SetActive(this.state == 2);
    }
}