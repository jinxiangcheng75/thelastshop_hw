
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureUpgradingComp : MonoBehaviour
{
    public Button closeBtn;
    public Button immediatelyBtn;
    public GameObject confirmObj;
    public Button finishBtn;

    public Text typeText;
    public Text timeText;
    public Text diamCountText;

    public Slider remainTimeSlider;

    [Header("图片部分")]
    public GUIIcon nextLevelImg;

    [Header("动画")]
    public Animator uiAnimator;

}

public class FurnitureUpgradingView : ViewBase<FurnitureUpgradingComp>
{
    public override string viewID => ViewPrefabName.FurnitureUpgradingPanel;
    public override string sortingLayerName => "popup";
    IndoorData.ShopDesignItem item;

    private int state = 0; //升级中 =0 ，升级完成 = 1
    public int remainTimerId = 0;
    int remainTime = 0;
    public int sliderValueTime = 0;
    private int immediatelyDiamCount = 0;
    private BaseUpgradeConfig config = null;

    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.finishBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, item.uid);
            hide();
        });

        contentPane.immediatelyBtn.onClick.AddListener(() =>
        {
            if (!contentPane.confirmObj.activeSelf)
            {
                contentPane.confirmObj.SetActiveTrue();
            }
            else
            {
                if (immediatelyDiamCount > UserDataProxy.inst.playerData.gem)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, "金条不足", GUIHelper.GetColorByColorHex("ff2828"));
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, immediatelyDiamCount - UserDataProxy.inst.playerData.gem);
                    return;
                }

                //if (!UserDataProxy.inst.FurnitureCanUpgradeFinish(item.uid)) //为钻石 先前端检测空间是否足够升级 这个只是先刷新为finished状态
                //{
                //    return;
                //}

                EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SAVEDATA, item.uid, 1);
                EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE_Immediately, item.type, item.uid);
                hide();
            }

        });
    }

    public void SetData(IndoorData.ShopDesignItem item)
    {
        this.item = item;

        contentPane.immediatelyBtn.gameObject.SetActiveTrue();

        // item = FurnitureUpgradeProxy.inst.tileSelectItem;

        GetContentData();

        contentPane.nextLevelImg.SetSprite(item.config.atlas, item.config.icon);
        contentPane.typeText.text = LanguageManager.inst.GetValueByKey(item.config.name.Substring(2));

        ShowInfo();
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
        contentPane.confirmObj.SetActiveFalse();
        GameTimer.inst.RemoveTimer(remainTimerId);
        DOTween.Pause("Do_1");
        state = 0;
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
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    private void GetContentData()
    {
        switch (item.config.type_1)
        {
            //柜台
            case (int)kTileGroupType.Counter:
                {
                    config = CounterUpgradeConfigManager.inst.getConfig(item.level + 1);
                    break;
                }
            //资源篮
            case (int)kTileGroupType.ResourceBin:
                {
                    config = ResourceBinUpgradeConfigManager.inst.getConfigByType(item.config.type_2, item.level + 1);
                    break;
                }
            //货架
            case (int)kTileGroupType.Shelf:
                {
                    config = ShelfUpgradeConfigManager.inst.getConfigByType(item.config.type_2, item.level + 1);
                    break;
                }
            //储物箱
            case (int)kTileGroupType.Trunk:
                {
                    config = TrunkUpgradeConfigManager.inst.getConfig(item.level + 1);
                    break;
                }
            default:
                {
                    Debug.LogError("未能找到对应类别");
                    break;
                }
        }
    }

    public void ShowInfo()
    {
        contentPane.remainTimeSlider.maxValue = (int)item.entityState.stateTime;
        sliderValueTime = (int)(item.entityState.stateTime - item.entityState.leftTime);
        contentPane.remainTimeSlider.value = sliderValueTime;
        remainTime = (int)item.entityState.leftTime;
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
            remainTime--;
            contentPane.timeText.text = TimeUtils.timeSpanStrip(remainTime);

            immediatelyDiamCount = DiamondCountUtils.GetFurnitureUpgradeDiamonds(remainTime);
            contentPane.diamCountText.text = immediatelyDiamCount.ToString("N0");

            //contentPane.diamCountText.color = immediatelyDiamCount > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("fd4f4f") : GUIHelper.GetColorByColorHex("FFFFFF");
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
            contentPane.timeText.text = LanguageManager.inst.GetValueByKey("升级完成！");
            GameTimer.inst.RemoveTimer(remainTimerId);
        }

        contentPane.immediatelyBtn.gameObject.SetActive(this.state == 1);
        contentPane.finishBtn.gameObject.SetActive(this.state == 2);
    }
}