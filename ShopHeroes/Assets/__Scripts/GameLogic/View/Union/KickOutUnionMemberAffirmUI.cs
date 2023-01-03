using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickOutUnionMemberAffirmUI : ViewBase<KickOutUnionMemberAffirmUIComp>
{

    public override string viewID => ViewPrefabName.KickOutUnionMemberAffirmUI;
    public override string sortingLayerName => "popup";


    string curUserUid;


    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.cancelBtn.ButtonClickTween(hide);

        contentPane.confirmBtn.ButtonClickTween(onConfirmBtnClick);
    }

    public void SetData(string _curUserUid)
    {
        curUserUid = _curUserUid;
    }


    void onConfirmBtnClick()
    {
        EventController.inst.TriggerEvent<string>(GameEventType.UnionEvent.UNION_REQUEST_KICKOUTMEMBER, curUserUid);
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

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
        curUserUid = string.Empty;
    }


}
