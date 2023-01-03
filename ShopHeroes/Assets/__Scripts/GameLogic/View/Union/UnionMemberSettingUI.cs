using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionMemberSettingUI : ViewBase<UnionMemberSettingUIComp>
{

    public override string viewID => ViewPrefabName.UnionMemberSettingUI;

    public override string sortingLayerName => "popup";


    int _job;

    int job
    {
        get { return _job; }
        set
        {
            if (value >= jobMax)
            {
                _job = 0;
            }
            else if (value <= -1)
            {
                _job = Mathf.Clamp(jobMax - 1, 0, jobMax);
            }
            else
            {
                _job = value;
            }

            if (_job == initJob)
            {
                GUIHelper.SetUIGray(contentPane.confirmBtn.transform, true);
                contentPane.confirmBtn.interactable = false;
                contentPane.confirmBtn.GetComponent<ButtonEx>().enabled = false;
            }
            else
            {
                GUIHelper.SetUIGray(contentPane.confirmBtn.transform, false);
                contentPane.confirmBtn.interactable = true;
                contentPane.confirmBtn.GetComponent<ButtonEx>().enabled = true;
            }

            contentPane.jobIcon.SetSprite("union_atlas", StaticConstants.unionJobIconArray[_job]);
            contentPane.jobTx.text = LanguageManager.inst.GetValueByKey(StaticConstants.unionJobNameArray[_job]);
        }
    }

    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.kickoutBtn.ButtonClickTween(onKickoutBtnClick);
        contentPane.confirmBtn.ButtonClickTween(onConfirmBtnClick);

        contentPane.leftBtn.onClick.AddListener(() => job--);
        contentPane.rightBtn.onClick.AddListener(() => job++);
    }

    string curUserUid;

    int jobMax;
    int initJob;
    public void SetData(string userUid, int memberJob)
    {
        curUserUid = userUid;

        switch ((EUnionJob)UserDataProxy.inst.playerData.memberJob)
        {
            case EUnionJob.Common:
                hide();
                break;
            case EUnionJob.Manager:
                jobMax = 1 - 1;
                break;
            case EUnionJob.President:
                jobMax = 2 + 1;
                break;
        }

        switch ((EUnionJob)memberJob)
        {
            case EUnionJob.Common:
                job = initJob = 0;
                break;
            case EUnionJob.Manager:
                job = initJob = 1;
                break;
            case EUnionJob.President:
                job = initJob = 2;
                break;
        }
    }

    private void onKickoutBtnClick()
    {
        EventController.inst.TriggerEvent<string>(GameEventType.UnionEvent.SHOWUI_UNIONKICKOUTMEMBERCONFIRM, curUserUid);
    }

    private void onConfirmBtnClick()
    {
        EventController.inst.TriggerEvent<string, int>(GameEventType.UnionEvent.UNION_REQUEST_SETMEMBERJOB, curUserUid, StaticConstants.unionJobArray[job]);
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
        base.onHide();
        AudioManager.inst.PlaySound(11);
    }
}
