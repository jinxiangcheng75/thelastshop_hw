using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TaskMsgBoxView : ViewBase<MsgBox_TaskComp>
{
    public override string viewID => ViewPrefabName.TaskMsgBoxUI;
    public override string sortingLayerName => "popup";

    private int task_id;
    private LoopEventcomp timerComp;

    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.cd_OK_Btn.ButtonClickTween(() =>
        {
            hide();
        });

        contentPane.refresh_OK_Btn.ButtonClickTween
            (() => OnRefreshOKBtnClick());

        contentPane.refresh_CANCEL_Btn.ButtonClickTween(() =>
        {
            hide();
        });

    }

    private void removeTimer() 
    {
        if (timerComp != null)
        {
            GameTimer.inst.removeLoopTimer(timerComp);
            timerComp = null;
        }
    }

    protected override void onHide()
    {
        removeTimer();
    }

    //请求此次刷新过后的任务列表
    private void OnRefreshOKBtnClick()
    {

        EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_REPLACE_DATALIST, task_id);
        this.hide();
    }


    public void setComMsgText(int task_id)
    {
        bool isOnCD = UserDataProxy.inst.task_refreshNumber <= 0;

        contentPane.cdStateObj.gameObject.SetActive(isOnCD);
        contentPane.refreshStateObj.gameObject.SetActive(!isOnCD);

        contentPane.infoNameTxt.text = LanguageManager.inst.GetValueByKey("重置任务");

        removeTimer();

        if (isOnCD)
        {
            contentPane.msgTxt.text = LanguageManager.inst.GetValueByKey("刷新机会用尽，下次倒计时：{0}", TimeUtils.timeSpanStrip(UserDataProxy.inst.task_refreshTime));
            timerComp = GameTimer.inst.AddLoopTimerComp(contentPane.msgTxt.gameObject, 1, () =>
            {
                contentPane.msgTxt.text = LanguageManager.inst.GetValueByKey("刷新机会用尽，下次倒计时：{0}", TimeUtils.timeSpanStrip(UserDataProxy.inst.task_refreshTime));
            });
        }
        else
        {
            contentPane.msgTxt.text = LanguageManager.inst.GetValueByKey("您还有{0}次刷新机会，确定要刷新当前的这个任务吗？", UserDataProxy.inst.task_refreshNumber.ToString());
        }

        this.task_id = task_id;
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
        GameTimer.inst.AddTimer(animTime, 1, () =>
          {
              contentPane.uiAnimator.CrossFade("null", 0f);
              contentPane.uiAnimator.Update(0f);
              HideView();
          });
    }
}
