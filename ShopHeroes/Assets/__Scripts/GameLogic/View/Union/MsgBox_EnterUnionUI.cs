using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgBox_EnterUnionUI : ViewBase<MsgBox_EnterUnionComp>
{
    public override string sortingLayerName => "popup";
    public override string viewID => ViewPrefabName.MsgBox_EnterUnion;

    private string _unionId;

    protected override void onInit()
    {
        base.onInit();
        contentPane.cancelBtn.ButtonClickTween(hide);
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.okBtn.ButtonClickTween(onOKBtnClick);
    }

    private void onOKBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_ENTER,_unionId);
        hide();
    }

    public void SetContent(string unionId, string unionName)
    {
        _unionId = unionId;
        contentPane.contentTx.text = LanguageManager.inst.GetValueByKey("你确认要加入{0}吗？", unionName);
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

}
