using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuyProductionUI : ViewBase<ResourceBuyProductionUIComp>
{

    public override string viewID => ViewPrefabName.ResourceBuyProductionUI;
    public override string sortingLayerName => "window";

    int itemId;
    int lackCount;
    int limitCount;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;

        contentPane.closeButton.onClick.AddListener(hide);
        contentPane.gemBuyButton.onClick.AddListener(onGemBuyBtnClick);

        contentPane.unionCoinButton.ButtonClickTween(onUnionCoinButtonClick);


    }

    protected override void onHide()
    {
        base.onHide();
        contentPane.gemConfimObj.SetActive(false);
        clearUnionCountdownTimer();
    }

    private void onGemBuyBtnClick()
    {

        int needGem = DiamondCountUtils.GetEquipMakeMaterialsReFullCost(lackCount);

        if (needGem > UserDataProxy.inst.playerData.gem)
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("ff2828"));
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, needGem - UserDataProxy.inst.playerData.gem);
        }
        else
        {

            if (contentPane.gemConfimObj.activeSelf)
            {
                EventController.inst.TriggerEvent(GameEventType.BagEvent.Bag_BuyProduction, itemId);
            }
            else
            {
                contentPane.gemConfimObj.SetActive(true);
            }

        }
    }

    private void onUnionCoinButtonClick()
    {
        if (contentPane.needUnionCoinTx.color == GUIHelper.GetColorByColorHex("fd4f4f"))
        {
            string tip = UserDataProxy.inst.playerData.hasUnion ? LanguageManager.inst.GetValueByKey("联盟币不足") : LanguageManager.inst.GetValueByKey("请先加入联盟");
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, tip, GUIHelper.GetColorByColorHex("ff2828"));
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.BagEvent.Bag_BuyProductionByUnoinCoin, itemId);
        }
    }

    private int getInventoryNum()
    {
        int count = (int)ItemBagProxy.inst.resItemCount(itemId);

        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        if (rp != null)
        {
            limitCount = (int)rp.countLimit;
            lackCount = (int)rp.countLimit - count;
        }

        return count;
    }

    public void setItem(int itemId)
    {
        this.itemId = itemId;

        contentPane.currInventoryTx.text = "<size=56>" + getInventoryNum().ToString() + "</size><color=#bb887c>" + "/" + limitCount.ToString() + "</color>";
        contentPane.remainTx.text = string.Empty;
        contentPane.replenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "X" + lackCount;
        int needGem = DiamondCountUtils.GetEquipMakeMaterialsReFullCost(lackCount);
        //contentPane.needGemTx.color = UserDataProxy.inst.playerData.gem >= needGem ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("fd4f4f");
        contentPane.needGemTx.text = needGem.ToString("N0");

        itemConfig item = ItemconfigManager.inst.GetConfig(itemId);
        contentPane.icon.SetSprite(item.atlas, item.icon);
        contentPane.slider.gameObject.SetActive(true);

        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        if (rp != null && rp.isActivate)
        {
            GUIHelper.SetUIGray(contentPane.gemBuyButton.transform, false);
            contentPane.gemBuyButton.interactable = true;

            bool isEnough = ItemBagProxy.inst.resItemCount(itemId) >= limitCount;
            if (isEnough)
            {
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIRefresh_CheckMakeEquipRes);
            }
            else
            {
                UnionResourceConfig union_resCfg = UnionResourceConfigManager.inst.GetConfig(itemId);

                bool haveStore = rp.unionCanBuyTimes > 0;
                contentPane.unionCoinButton.gameObject.SetActive(haveStore);
                contentPane.unionBuyCountdownObj.gameObject.SetActive(!haveStore);

                if (haveStore)
                {
                    contentPane.remainTx.text = haveStore ? LanguageManager.inst.GetValueByKey("剩余{0}次", rp.unionCanBuyTimes.ToString()) : "";
                    contentPane.unoin_replenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "x" + Mathf.CeilToInt((float)(rp.countLimit * union_resCfg.item_num / 10000.0));
                    contentPane.needUnionCoinTx.text = union_resCfg.price.ToString("N0");
                    contentPane.needUnionCoinTx.color = UserDataProxy.inst.playerData.unionCoin >= union_resCfg.price ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("fd4f4f");
                }
                else
                {
                    contentPane.remainTx.text = LanguageManager.inst.GetValueByKey("下次倒计时");
                    setUnionCountdownTimer();
                }

                contentPane.slider.gameObject.SetActive(true);
                contentPane.replenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "x" + lackCount;
                contentPane.sliderCtrl.RefreshMakeBar((float)rp.duration, (float)rp.time);
            }
        }
        else
        {
            contentPane.slider.value = 0;
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("请先摆放对应的资源篮"), Color.white);

            GUIHelper.SetUIGray(contentPane.gemBuyButton.transform, true);
            contentPane.gemBuyButton.interactable = false;
        }


    }

    public void RefreshResProduction(int itemId)
    {
        bool isSameOne = this.itemId == itemId;
        if (!isSameOne) return;

        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        if (rp != null && rp.isActivate)
        {
            GUIHelper.SetUIGray(contentPane.gemBuyButton.transform, false);
            contentPane.gemBuyButton.interactable = true;

            bool isEnough = ItemBagProxy.inst.resItemCount(itemId) >= rp.countLimit;
            if (isEnough)
            {
                hide();
            }
            else
            {
                setItem(itemId);
                //getInventoryNum();
                //contentPane.currInventoryTx.text = getInventoryNum().ToString() + "<color=#F32E00>/" + rp.countLimit + "</color>";
                //contentPane.replenishCountTx.text = LanguageManager.inst.GetValueByKey("补充") + "X" + lackCount;
                //contentPane.needGemTx.text = DiamondCountUtils.GetEquipMakeMaterialsReFullCost(lackCount).ToString("N0");
                //contentPane.sliderCtrl.RefreshMakeBar((float)rp.duration, (float)rp.time);
            }
        }
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


    int unionCountDownTimer;

    void clearUnionCountdownTimer()
    {
        if (unionCountDownTimer != 0)
        {
            GameTimer.inst.RemoveTimer(unionCountDownTimer);
            unionCountDownTimer = 0;
        }
    }

    void setUnionCountdownTimer()
    {
        clearUnionCountdownTimer();

        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        unionCountdownTimerMethod();
        unionCountDownTimer = GameTimer.inst.AddTimer(1, rp.unionBuyCountdownTime, unionCountdownTimerMethod);
    }

    void unionCountdownTimerMethod()
    {
        ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
        contentPane.unionBuyCountdownTx.text = TimeUtils.timeSpan3Str(rp.unionBuyCountdownTime);
    }


}
