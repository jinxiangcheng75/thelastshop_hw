using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgBox_ExitUnionUI : ViewBase<MsgBox_ExitUnionComp>
{

    public override string sortingLayerName => "popup";
    public override string viewID => ViewPrefabName.MsgBox_ExitUnion;

    bool isExit;

    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.okBtn.ButtonClickTween(onOKBtnClick);
    }

    private void onOKBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_EXIT);
        isExit = true;
        hide();
    }

    public void SetContent() 
    {
        isExit = false;
        contentPane.contentTx.text = LanguageManager.inst.GetValueByKey("你确认要离开{0}吗？离开后你将无法享受联盟共享改造与研究技术等级加成！", UserDataProxy.inst.playerData.unionName);
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
        if (isExit) GUIManager.HideView<UnionInfoUI>();
    }

}
