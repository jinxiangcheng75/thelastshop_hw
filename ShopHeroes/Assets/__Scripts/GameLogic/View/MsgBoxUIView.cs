using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MsgBoxUIView : ViewBase<MsgBoxComp>
{
    public override string viewID => ViewPrefabName.MsgBoxUI;
    public override string sortingLayerName => "top";

    public System.Action okBtnOnclick;
    protected override void onInit()
    {
        base.onInit();
        //contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.okBtn.ButtonClickTween(() =>
        {
            if (okBtnOnclick != null)
            {
                okBtnOnclick.Invoke();
                okBtnOnclick = null;
                hide();
            }
            else
            {
                hide();
            }
        });

        contentPane.cancleBtn.ButtonClickTween(hide);
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
        this.contentPane.msgText.text = "";
    }

    public void setMsgText(string msg)
    {
        contentPane.cancleBtn.gameObject.SetActive(false);
        var okBtnRect = contentPane.okBtn.gameObject.GetComponent<RectTransform>();
        okBtnRect.anchoredPosition = new Vector2(0, okBtnRect.anchoredPosition.y);
        this.contentPane.msgText.text = msg;
    }

    public void setMsgTextWithCancleBtn(string msg)
    {
        contentPane.cancleBtn.gameObject.SetActive(true);
        var okBtnRect = contentPane.okBtn.gameObject.GetComponent<RectTransform>();
        okBtnRect.anchoredPosition = new Vector2(210, okBtnRect.anchoredPosition.y);
        this.contentPane.msgText.text = LanguageManager.inst.GetValueByKey(msg);
    }
}
