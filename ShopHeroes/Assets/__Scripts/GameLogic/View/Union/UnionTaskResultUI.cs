using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionTaskResultUI : ViewBase<UnionTaskResultUIComp>
{

    public override string viewID => ViewPrefabName.UnionTaskResultUI;
    public override string sortingLayerName => "popup";

    public override int showType => (int)ViewShowType.normal;

    GraphicDressUpSystem headGraphicSystem;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSettingAndEnergy;

        contentPane.continueBtn.ButtonClickTween(hide);

    }

    public void SetData(Response_Union_TaskResult data)
    {

        contentPane.unionTaskLvTx.text = data.unionTaskLevel.ToString();
        contentPane.unionTokenTx.text = data.unionPoint.ToString();

        if (headGraphicSystem == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.userDress), (EGender)data.gender, callback: (system) =>
            {
                headGraphicSystem = system;
                system.transform.SetParent(contentPane.playerHeadIconTf);
                system.transform.localScale = Vector3.one * 0.4f;
                system.transform.localPosition = Vector3.down * 250f;
                system.SetDirection(RoleDirectionType.Right);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(headGraphicSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.userDress), (EGender)data.gender);
        }

        contentPane.playerNameTx.text = "<color=#ffffff>" + data.name + "</color>" + LanguageManager.inst.GetValueByKey("最有价值的玩家");
        contentPane.playerTokenTx.text = "x" + data.userPoint.ToString();

    }


    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(132);
    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();
    }

    protected override void onHide()
    {
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONTASKRESET);
    }

}
