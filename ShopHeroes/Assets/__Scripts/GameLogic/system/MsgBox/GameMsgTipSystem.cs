using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMsgTipSystem : BaseSystem
{
    MsgBoxUIView msgBoxUIView;
    GameTextTipView gameTextTipView;
    string msgBoxText = "";
    protected override void AddListeners()
    {
        EventController.inst.AddListener<string>(GameEventType.SHOWUI_MSGBOX, ShowMsgBox);
        EventController.inst.AddListener<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, ShowOkMsgBox);
        EventController.inst.AddListener<string, System.Action>(GameEventType.SHOWUI_OKCANCLE_MSGBOX, ShowOkCancleMsgBox);
        EventController.inst.AddListener<string, Color>(GameEventType.SHOWUI_TEXTMSGTIP, ShowMsgTip);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<string>(GameEventType.SHOWUI_MSGBOX, ShowMsgBox);
        EventController.inst.RemoveListener<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, ShowOkMsgBox);
        EventController.inst.RemoveListener<string, System.Action>(GameEventType.SHOWUI_OKCANCLE_MSGBOX, ShowOkCancleMsgBox);
        EventController.inst.RemoveListener<string, Color>(GameEventType.SHOWUI_TEXTMSGTIP, ShowMsgTip);
    }

    private void ShowOkMsgBox(string msg, System.Action callback)
    {
        msgBoxText = msg;
        var _view = GUIManager.OpenView<MsgBoxUIView>((view) =>
        {
            view.setMsgText(msgBoxText);
        });
        _view.okBtnOnclick = callback;
    }

    private void ShowOkCancleMsgBox(string msg, System.Action callback)
    {
        msgBoxText = msg;
        var _view = GUIManager.OpenView<MsgBoxUIView>((view) =>
        {
            view.setMsgTextWithCancleBtn(msgBoxText);
        });
        _view.okBtnOnclick = callback;
    }

    private void ShowMsgBox(string msg)
    {
        // if (msgBoxUIView == null)
        // {
        //     msgBoxUIView = new MsgBoxUIView();
        // }
        // if (!msgBoxUIView.isShowing)
        // {
        //     msgBoxText = msg;
        //     msgBoxUIView.show((view) =>
        //     {
        //         msgBoxUIView.setMsgText(msgBoxText);
        //     });
        // }
        msgBoxText = msg;
        GUIManager.OpenView<MsgBoxUIView>((view) =>
        {
            view.setMsgText(msgBoxText);
        });
    }

    private void ShowMsgTip(string msg, Color color)
    {
        color = Color.white;
        if (gameTextTipView != null && gameTextTipView.isShowing)
        {
            gameTextTipView.ShowText(msg, color);
        }
        else
        {
            gameTextTipView = GUIManager.OpenView<GameTextTipView>((view) =>
            {
                view.ShowText(msg, color);
            });
        }
    }
}
