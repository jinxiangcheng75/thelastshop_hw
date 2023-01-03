using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionMsgUpdateUI : ViewBase<UnionMsgUpdateUIComp>
{

    public override string viewID => ViewPrefabName.UnionMsgUpdateUI;
    public override string sortingLayerName => "popup";


    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.ignoreBtn.ButtonClickTween(hide);
        contentPane.nextBtn.ButtonClickTween(nextMsg);
    }


    int _index;
    List<UnionMsgData> _unionUpdateMsgs;
    GraphicDressUpSystem headGraphicSystem;

    public void SetData(List<UnionMsgData> unionMsgDatas)
    {
        _unionUpdateMsgs = unionMsgDatas;
        _index = -1;

        nextMsg();
    }

    private void nextMsg()
    {
        _index++;

        if (_index < _unionUpdateMsgs.Count)
        {
            contentPane.uiAnimator.CrossFade("show", 0f);
            contentPane.uiAnimator.Update(0f);
            contentPane.uiAnimator.Play("show");
            setData(_unionUpdateMsgs[_index]);
        }
        else
        {
            hide();
        }

    }

    void setData(UnionMsgData data)
    {
        if (headGraphicSystem == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.userDress), (EGender)data.gender, callback: (system) =>
            {
                headGraphicSystem = system;
                system.transform.SetParent(contentPane.headGraphicTf);
                system.transform.localScale = Vector3.one * 0.4f;
                system.transform.localPosition = Vector3.down * 250f;
                system.SetDirection(RoleDirectionType.Left);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(headGraphicSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.userDress), (EGender)data.gender);
        }

        contentPane.lvTx.text = data.level.ToString();
        contentPane.nickNameTx.text = LanguageManager.inst.GetValueByKey(data.nickName);


        switch ((EUnionMessageType)data.unionMsgType)
        {
            case EUnionMessageType.EnterUnion:
                contentPane.memberJobIcon.gameObject.SetActive(false);
                contentPane.unionMsgTx.text = LanguageManager.inst.GetValueByKey("进入了公会");
                break;

            case EUnionMessageType.ChangeJobToMember:
            case EUnionMessageType.ChangeJobToManager:
            case EUnionMessageType.ChangeJobToPresident:
                int index = Array.IndexOf(StaticConstants.unionJobArray, data.memberJob); //原来的
                int curIndex = data.unionMsgType - (int)EUnionMessageType.ChangeJobToMember; //现在的
                bool isUp = index < curIndex;

                contentPane.unionMsgTx.text = isUp ? LanguageManager.inst.GetValueByKey("晋升为{0}", "<color=#6df35f>" + LanguageManager.inst.GetValueByKey(StaticConstants.unionJobNameArray[curIndex]) + "</color>") : LanguageManager.inst.GetValueByKey("被降级为{0}", "<color=#ff5555>" + LanguageManager.inst.GetValueByKey(StaticConstants.unionJobNameArray[curIndex]) + "</color>");

                contentPane.memberJobIcon.gameObject.SetActive(true);
                contentPane.memberJobIcon.SetSprite("union_atlas", StaticConstants.unionJobIconArray[curIndex]);

                break;

            case EUnionMessageType.LeaveUnion:
                contentPane.memberJobIcon.gameObject.SetActive(false);
                contentPane.unionMsgTx.text = LanguageManager.inst.GetValueByKey("离开了联盟");
                break;

            case EUnionMessageType.UnionKickOut:
                contentPane.memberJobIcon.gameObject.SetActive(false);
                contentPane.unionMsgTx.text = LanguageManager.inst.GetValueByKey("被移除出了联盟");
                break;
        }

        var a = (EUnionMessageType)data.unionMsgType;//新的
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
