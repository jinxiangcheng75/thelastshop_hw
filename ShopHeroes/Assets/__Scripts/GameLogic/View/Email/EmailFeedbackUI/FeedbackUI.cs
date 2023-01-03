using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackUI : ViewBase<FeedbackUIComp>
{
    public override string viewID => ViewPrefabName.FeedbackUI;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.sendBtn.ButtonClickTween(onSendBtnClick);

    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
        contentPane.inputField.text = string.Empty;
    }

    bool checkContent()
    {
        int length = contentPane.inputField.text.Length;
        if (length < 5 || length > 200)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("反馈问题要在5-200字之间"), GUIHelper.GetColorByColorHex("FF2828"));
            return false;
        }

        if (!EmailDataProxy.inst.canSendFeedback)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("距下次反馈问题还需{0}秒", (EmailDataProxy.inst.sendFeedbackTimingTime - (int)GameTimer.inst.serverNow).ToString()), GUIHelper.GetColorByColorHex("FF2828"));
            return false;
        }


        return true;
    }

    void onSendBtnClick()
    {
        if (!checkContent()) return;

        EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_FEEDBACK, contentPane.inputField.text);
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("反馈成功"),GUIHelper.GetColorByColorHex("FFD907"));

        hide();
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
