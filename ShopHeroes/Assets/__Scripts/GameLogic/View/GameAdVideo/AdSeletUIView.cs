using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class admsgboxInfo
{
    public int adtype;
    public string title;
    public string msg;
    public int resid;
    public long rescount;
    public System.Action lookAdCall;
    public System.Action useResCall;
}
public class AdSeletUIView : ViewBase<AdSeletUIComp>
{
    public override string viewID => ViewPrefabName.MsgBox_AdSeletUI;
    public override int showType => (int)ViewShowType.popup;
    public override string sortingLayerName => "popup";

    LoopEventcomp loopcomp;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = false;
        contentPane.closebtn.ButtonClickTween(hide);
        var wcfg = WorldParConfigManager.inst.GetConfig(181);
        if (wcfg != null)
        {
            var adcooltime = wcfg.parameters;
            int lasttime = SaveManager.inst.GetInt("lastADPlayTime");
            if (lasttime > 0)
            {
                var t = TimeUtils.GetNowSeconds() - lasttime;
                if (t < adcooltime)
                {
                    contentPane.AdBtn.interactable = false;
                    contentPane.adBtntext.text = TimeUtils.timeSpan4Str((int)adcooltime - t);

                    loopcomp = GameTimer.inst.AddLoopTimerComp(contentPane.adBtntext.gameObject, 1, () =>
                    {
                        var _t = TimeUtils.GetNowSeconds() - lasttime;
                        if (_t >= adcooltime)
                        {
                            contentPane.AdBtn.interactable = true;
                            GameTimer.inst.removeLoopTimer(loopcomp);
                            contentPane.adBtntext.text = LanguageManager.inst.GetValueByKey("免费");
                        }
                        else
                        {
                            contentPane.AdBtn.interactable = false;
                            contentPane.adBtntext.text = TimeUtils.timeSpan4Str((int)adcooltime - _t);
                        }
                    });
                }
                else
                {
                    contentPane.AdBtn.interactable = true;
                    contentPane.adBtntext.text = LanguageManager.inst.GetValueByKey("免费");
                }
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
        float animTime = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animTime, 1, this.hideanimationPlayEnd);
    }

    private void hideanimationPlayEnd()
    {
        contentPane.uiAnimator.CrossFade("null", 0f);
        contentPane.uiAnimator.Update(0f);
        HideView();
    }
    admsgboxInfo currInfo;
    public void updateUI(admsgboxInfo adInfo)
    {

        currInfo = adInfo;

        if (currInfo != null)
        {
            contentPane.titleText.text = LanguageManager.inst.GetValueByKey(currInfo.title);
            contentPane.msgText.text = LanguageManager.inst.GetValueByKey(currInfo.msg);
            //itemConfig item = ItemconfigManager.inst.GetConfig(info.resid);
            Item item = ItemBagProxy.inst.GetItem(currInfo.resid);
            if (item != null)
            {
                contentPane.resIcon.gameObject.SetActive(true);
                contentPane.resIcon.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);
                if (item.ID == StaticConstants.gemID)
                {
                    contentPane.okBtnText.text = currInfo.rescount.ToString();
                    //GUIHelper.SetUIGrayColor(contentPane.okBtn.transform, UserDataProxy.inst.playerData.gem < currInfo.rescount ? 0.6f : 1f);
                }
                else
                {
                    contentPane.okBtnText.text = String.Format("{0}/{1}", item.count, currInfo.rescount);
                    GUIHelper.SetUIGrayColor(contentPane.okBtn.transform, item.count < currInfo.rescount ? 0.6f : 1f);
                }
                contentPane.AdBtn.ButtonClickTween(() =>
                {
                    // if (!SDKManager.inst.IsRewardedVideoAvailable())
                    // {
                    //     EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("广告准备中......"), GUIHelper.GetColorByColorHex("#FF2828"));
                    // }
                    currInfo.lookAdCall?.Invoke();
                    hide();
                });

                contentPane.okBtn.ButtonClickTween(() =>
                {
                    if (contentPane.confimImg.enabled)
                    {
                        contentPane.confimImg.enabled = false;
                        currInfo.useResCall?.Invoke();
                        hide();
                    }
                    else
                    {
                        contentPane.confimImg.enabled = true;
                    }
                });
            }
            else
            {
                contentPane.resIcon.gameObject.SetActive(false);
                hide();
            }
        }
    }
}
