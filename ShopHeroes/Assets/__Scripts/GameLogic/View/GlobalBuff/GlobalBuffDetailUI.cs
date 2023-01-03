using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GlobalBuffDetailUI : ViewBase<GlobalBuffDetailUIComp>
{

    public override string viewID => ViewPrefabName.GlobalBuffDetailUI;
    public override string sortingLayerName => "popup";

    GlobalBuffData _data;
    LoopEventcomp timerComp;

    protected override void onInit()
    {
        topResPanelType = TopPlayerShowType.all;
        isShowResPanel = true;
        contentPane.okBtn.ButtonClickTween(hide);
    }


    public void SetData(GlobalBuffType buffType)
    {
        GlobalBuffData data = GlobalBuffDataProxy.inst.GetGlobalBuffData(buffType);

        if (data == null)
        {
            Logger.error("该buff类型不存在  buffType: " + buffType);
            hide();
            return;
        }

        _data = data;
        System.DateTime dateTime = TimeUtils.getDateTimeBySecs(GameTimer.inst.serverNow);
        contentPane.gameDateTimeTx.text = LanguageManager.inst.GetValueByKey("{0}月{1}日 救赎日38年", dateTime.Month.ToString(), dateTime.Day.ToString());
        contentPane.topLeftTx.text = LanguageManager.inst.GetValueByKey(data.title_0);
        contentPane.titleTx.text = LanguageManager.inst.GetValueByKey(data.title_1);
        contentPane.buffIcon.SetSprite(data.config.type_atlas, data.config.type_icon);

        string[] strs = data.title_2.Split('+');
        if (strs.Length >= 2)
        {
            contentPane.effectTx.text = strs[0] + "<size=48><color=#2f9400>+" + strs[1] + "</color></size>";
        }
        else
        {
            contentPane.effectTx.text = "--";
        }

        contentPane.desTx.text = LanguageManager.inst.GetValueByKey(data.title_3);
        contentPane.remainTip.text = LanguageManager.inst.GetValueByKey(data.title_1) + " <size=32>" + LanguageManager.inst.GetValueByKey("结束还有：") + "</size>";
        contentPane.advanceNoticeTx.text = data.herald == "-1" ? LanguageManager.inst.GetValueByKey("02号堡垒正在筹备神秘活动，堡垒日报将实时报道相关信息。") : LanguageManager.inst.GetValueByKey(data.herald);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.advanceNoticeTf);
        contentPane.okBtnTx.text = LanguageManager.inst.GetValueByKey(data.con);
        (contentPane.okBtn.transform as RectTransform).sizeDelta = new Vector2(contentPane.okBtnTx.preferredWidth + 84, 85);
        countdownMethod();
        setTimer();
    }

    void setTimer()
    {
        if (timerComp != null)
        {
            GameTimer.inst.removeLoopTimer(timerComp);
            timerComp = null;
        }

        timerComp = GameTimer.inst.AddLoopTimerComp(contentPane.remainTimeTx.gameObject, 1, countdownMethod, _data.remainTime);
    }

    void countdownMethod()
    {
        if (_data.remainTime > 0)
        {
            contentPane.remainTimeTx.text = TimeUtils.timeSpan3Str(_data.remainTime);
            contentPane.clockHand.DOLocalRotate(Vector3.forward * TimeUtils.timeSpanAngle(_data.remainTime), 0.5f);
        }
        else
        {
            //数据统一刷新 回调中关闭界面
            contentPane.remainTimeTx.text = TimeUtils.timeSpan3Str(1);
            contentPane.clockHand.rotation = Quaternion.Euler(0, 0, TimeUtils.timeSpanAngle(1));
        }
    }

    public void DelOneBuff(int buffUid)
    {
        if (_data.buffuId == buffUid)
        {
            hide();
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

    protected override void onHide()
    {

        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        if (timerComp != null)
        {
            GameTimer.inst.removeLoopTimer(timerComp);
            timerComp = null;
        }
    }

}
