using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgBox_NeedAchievement : ViewBase<MsgBox_NeedAchievementComp>
{
    public override string sortingLayerName => "popup";
    public override string viewID => ViewPrefabName.MsgBox_NeedAchievement;


    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.ButtonClickTween(hide);
    }

    public void SetData(int achievementId,string content)
    {
        contentPane.acheivementItem.setData(AcheivementDataProxy.inst.GetAcheivementDataById(achievementId));
        contentPane.contentTx.text = content;
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
