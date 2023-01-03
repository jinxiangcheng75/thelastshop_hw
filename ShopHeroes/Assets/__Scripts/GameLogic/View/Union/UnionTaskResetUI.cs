using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionTaskResetUI : ViewBase<UnionTaskResetUIComp>
{

    public override string viewID => ViewPrefabName.UnionTaskResetUI;
    public override string sortingLayerName => "window";

    int timerId;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSettingAndEnergy;

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.toRenownBtn.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionTaskUIView");
        });

    }

    public void SetData()
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        contentPane.countdownTx.text = TimeUtils.timeSpanStrip(UserDataProxy.inst.unionTaskRefreshTime, true);

        timerId = GameTimer.inst.AddTimer(1, () =>
        {
             TimeUtils.timeSpanStrip(UserDataProxy.inst.unionTaskRefreshTime, true);
        });

    }

    protected override void onShown()
    {
        SetData();
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
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }


}
