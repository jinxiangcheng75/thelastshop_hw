using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperSingleBuyView : ViewBase<ShopkeeperSingleBuyCom>
{
    public override string viewID => ViewPrefabName.ShopkeeperBuySingleUI;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });
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
        var clipInfo = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(clipInfo, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    public void setData(RoleSubTypeData _curData)
    {
        contentPane.buyBtn.onClick.RemoveAllListeners();
        bool judgeType = (PanelType)_curData.config.type_1 == PanelType.exterior;
        string[] titles = judgeType ? StaticConstants.exTypes : StaticConstants.faTypes;
        int typeTwo = _curData.config.type_2;
        // 判断显示的文字
        if (judgeType)
            contentPane.topText.text = LanguageManager.inst.GetValueByKey("解锁") + LanguageManager.inst.GetValueByKey(titles[typeTwo]);
        else
            contentPane.topText.text = LanguageManager.inst.GetValueByKey("解锁") + LanguageManager.inst.GetValueByKey(titles[typeTwo - 20]);

        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(_curData.config.name);

        string iconName = _curData.config.icon;
        contentPane.icon.SetSprite("ClotheIcon_atlas", iconName);

        // 判断当前钻石/金币数量是否高于需要钻石/金币数量如果不够不发消息字体变红 够发网络消息字体变白
        if (_curData.config.price_money > 0)
        {
            contentPane.goldObj.SetActive(true);
            contentPane.gemObj.SetActive(false);
            contentPane.priceText.text = _curData.config.price_money.ToString();

            if (UserDataProxy.inst.playerData.gold > _curData.config.price_money)
            {
                contentPane.priceText.color = GUIHelper.GetColorByColorHex("FFFFFF");
                contentPane.buyBtn.onClick.AddListener(() =>
                {

                    float animTime = contentPane.buyBtn.GetComponent<Animator>().GetClipLength("Pressed");
                    GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
                    {
                        BuyGoodMethod(_curData);
                    });
                });
            }
            else
                contentPane.priceText.color = GUIHelper.GetColorByColorHex("FF0000");
        }
        else if (_curData.config.price_diamond > 0)
        {
            contentPane.gemObj.SetActive(true);
            contentPane.goldObj.SetActive(false);
            contentPane.priceText.text = _curData.config.price_diamond.ToString();

            if (UserDataProxy.inst.playerData.gem >= _curData.config.price_diamond)
            {
                //contentPane.priceText.color = Color.white;
                contentPane.buyBtn.ButtonClickTween(() =>
                {

                    BuyGoodMethod(_curData);
                });
            }
            else
            {
                //    contentPane.priceText.color = Color.red;
                contentPane.buyBtn.ButtonClickTween(() =>
                {
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, _curData.config.price_diamond - UserDataProxy.inst.playerData.gem);
                });
            }
        }
    }

    private void BuyGoodMethod(RoleSubTypeData _curData)
    {
        if (!contentPane.sureAgainObj.activeSelf)
            contentPane.sureAgainObj.SetActive(true);
        else
        {
            List<int> dressList = new List<int>();
            dressList.Add((int)_curData.config.id);
            ShopkeeperDataProxy.inst.buyType = 0;
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_User_BuyDress()
                {
                    dressIdList = dressList
                }
            });

            contentPane.sureAgainObj.SetActive(false);
        }
    }

    protected override void onHide()
    {
        contentPane.sureAgainObj.SetActive(false);
    }
}
