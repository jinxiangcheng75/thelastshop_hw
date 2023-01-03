
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox_FurnitureUpgradeComp : MonoBehaviour
{
    public Text msgTxt;
    public Text diamCount;

    public Button closeBtn;
    public Button cancelBtn;
    public Button immediatelyBtn;
    public GameObject confirmObj;
    public Button yesBtn;
    public Button noBtn;

    [Header("-显隐部分-")]
    public GameObject upgradingStateObj;
    public GameObject finishStateObj;

    [Header("动画")]
    public Animator uiAnimator;

}

public class MsgBox_FurnitureUpgradeView : ViewBase<MsgBox_FurnitureUpgradeComp>
{
    public override string viewID => ViewPrefabName.FurnitureUpgradeMsgBoxUI;
    public override string sortingLayerName => "window";

    IndoorData.ShopDesignItem item;

    private int timerId;
    private BaseUpgradeConfig config = null;

    //当前正在升级的家具立即完成升级所需要的钻石数
    private int diamCount;

    protected override void onInit()
    {
        var c = contentPane;

        c.closeBtn.onClick.AddListener(hide);
        c.cancelBtn.onClick.AddListener(hide);

        c.immediatelyBtn.onClick.AddListener(() =>
        {

            if (!contentPane.confirmObj.activeSelf)
            {
                contentPane.confirmObj.SetActiveTrue();
                return;
            }

            if (UserDataProxy.inst.playerData.gem < diamCount)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            //if (!UserDataProxy.inst.FurnitureCanUpgradeFinish(item.uid)) //为钻石 先前端检测空间是否足够升级  这个只是先刷新为finished状态
            //{
            //    return;
            //}

            EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SAVEDATA, item.uid, 1);
            EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE_Immediately, item.type, item.uid);
            hide();
        });

        c.noBtn.onClick.AddListener(hide);
        c.yesBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, item.uid);
            hide();
        });
    }

    public void SetData(IndoorData.ShopDesignItem item)
    {
        this.item = item;

        var c = contentPane;

        GetDiamCount();
        timerId = GameTimer.inst.AddTimer(1, GetDiamCount);

        switch ((EDesignState)item.state)
        {
            case EDesignState.Upgrading:
                {
                    c.msgTxt.text = LanguageManager.inst.GetValueByKey("{0}正在升级，要花费金条来立即完成升级吗？", LanguageManager.inst.GetValueByKey(item.config.name.Substring(2)));
                    c.diamCount.text = diamCount.ToString();
                    c.diamCount.color = UserDataProxy.inst.playerData.gem < diamCount ? GUIHelper.GetColorByColorHex("FD4F4F") : GUIHelper.GetColorByColorHex("FFFFFF");
                    c.upgradingStateObj.SetActiveTrue();
                    c.finishStateObj.SetActiveFalse();
                    break;
                }

            case EDesignState.Finished:
                {
                    c.msgTxt.text = LanguageManager.inst.GetValueByKey("{0}准备好升级了，完成它来开始新的升级吧！", LanguageManager.inst.GetValueByKey(item.config.name.Substring(2)));
                    c.finishStateObj.SetActiveTrue();
                    c.upgradingStateObj.SetActiveFalse();
                    break;
                }
            default: break;
        }
    }

    protected override void onHide()
    {
        GameTimer.inst.RemoveTimer(timerId);
        contentPane.confirmObj.SetActiveFalse();
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

    private void GetDiamCount()
    {
        GetContentData();
        diamCount = DiamondCountUtils.GetFurnitureUpgradeDiamonds((int)item.entityState.leftTime);
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
}
