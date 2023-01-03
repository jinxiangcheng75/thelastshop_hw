using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopkeeperPromptComp : MonoBehaviour
{
    public Button closeBtn;
    public Button cancleBtn;
    public Button confirmBtn;
    public Animator windowAnim;
}

public class ShopkeeperPromptView : ViewBase<ShopkeeperPromptComp>
{
    public override string viewID => ViewPrefabName.ShopkeeperPromptPanel;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSettingAndEnergy;

        contentPane.closeBtn.ButtonClickTween(() =>
        {
            //float animTime = contentPane.closeBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.Man.gender, (int)kIndoorRoleActionType.normal_standby);
            ShopkeeperDataProxy.inst.Man.Play(idleAnimationName, true);
            idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.Woman.gender, (int)kIndoorRoleActionType.normal_standby);
            ShopkeeperDataProxy.inst.Woman.Play(idleAnimationName, true);
            hide();
            float time = contentPane.windowAnim.GetClipLength("common_popUpUI_hide");
            GameTimer.inst.AddTimer(time * 0.45f, 1, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPKEEPERPANEL);
            });
        });

        contentPane.cancleBtn.ButtonClickTween(() =>
        {
            //float animTime = contentPane.cancleBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.Man.gender, (int)kIndoorRoleActionType.normal_standby);
            ShopkeeperDataProxy.inst.Man.Play(idleAnimationName, true);
            idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.Woman.gender, (int)kIndoorRoleActionType.normal_standby);
            ShopkeeperDataProxy.inst.Woman.Play(idleAnimationName, true);
            hide();
            float time = contentPane.windowAnim.GetClipLength("common_popUpUI_hide");
            GameTimer.inst.AddTimer(time * 0.45f, 1, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPKEEPERPANEL);
            });
        });

        contentPane.confirmBtn.onClick.AddListener(() =>
        {
            //float animTime = contentPane.confirmBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            if (ShopkeeperDataProxy.inst.curGender != (EGender)UserDataProxy.inst.playerData.gender)
            {
                ShopkeeperDataProxy.inst.curGender = (EGender)UserDataProxy.inst.playerData.gender;
            }
            ShopkeeperDataProxy.inst.ChangeListState(SpineUtils.RoleDressToUintList(ShopkeeperDataProxy.inst.manDress), false);
            ShopkeeperDataProxy.inst.ChangeListState(SpineUtils.RoleDressToUintList(ShopkeeperDataProxy.inst.womanDress), false);
            ShopkeeperDataProxy.inst.ChangeListState(SpineUtils.RoleDressToUintList(ShopkeeperDataProxy.inst.manFirst), true);
            ShopkeeperDataProxy.inst.ChangeListState(SpineUtils.RoleDressToUintList(ShopkeeperDataProxy.inst.womanFirst), true);
            ShopkeeperDataProxy.inst.Man.OverallClothing(SpineUtils.RoleDressToUintList(ShopkeeperDataProxy.inst.manFirst));
            ShopkeeperDataProxy.inst.Woman.OverallClothing(SpineUtils.RoleDressToUintList(ShopkeeperDataProxy.inst.womanFirst));
            hide();
            float time = contentPane.windowAnim.GetClipLength("common_popUpUI_hide");
            GameTimer.inst.AddTimer(time * 0.45f, 1, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.SETSPBTNSTATE, true);
                EventController.inst.TriggerEvent(GameEventType.HIDEUI_SHOPKEEPERPANEL);
            });
        });
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.windowAnim.CrossFade("show", 0f);
        contentPane.windowAnim.Update(0f);
        contentPane.windowAnim.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.windowAnim.Play("hide");
        float animTime = contentPane.windowAnim.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animTime, 1, () =>
        {
            contentPane.windowAnim.CrossFade("null", 0f);
            contentPane.windowAnim.Update(0f);
            this.HideView();
        });
    }
}
