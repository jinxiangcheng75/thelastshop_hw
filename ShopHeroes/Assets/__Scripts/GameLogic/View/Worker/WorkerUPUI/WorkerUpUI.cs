using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerUpUI : ViewBase<WorkerUpComp>
{

    public override string viewID => ViewPrefabName.WorkerUpUI;
    public override string sortingLayerName => "window";

    DressUpSystem _dressUpSystem;

    protected override void onInit()
    {
        base.onInit();

        contentPane.continueBtn.onClick.AddListener(onContinueBtnClick);
    }


    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
        var clipInfo = contentPane.uiAnimator.GetClipLength("show");
        if (_dressUpSystem != null) _dressUpSystem.Fade(1, 0.3f).From(0);
        //DoTweenUtil.Fade_0_To_a_All(contentPane.topBg, 1, 0.5f);
        //DoTweenUtil.Fade_0_To_a_All(contentPane.continueBtn.transform, 1, 0.2f, clipInfo);
    }

    protected override void DoHideAnimation()
    {
        HideView();
    }

    public void SetData(int workerId)
    {
        WorkerData worker = RoleDataProxy.inst.GetWorker(workerId);
        if (worker == null)
        {
            hide();
            return;
        }

        if (_dressUpSystem == null)
        {
            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(worker.config.model, callback: (system) =>
            {
                _dressUpSystem = system;
                GUIHelper.setRandererSortinglayer(_uiCanvas.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2);
                system.SetUIPosition(contentPane.workerTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, 0.8f);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)system.gender, (int)kIndoorRoleActionType.normal_standby);
                system.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(_dressUpSystem, worker.config.model);
            GUIHelper.setRandererSortinglayer(_uiCanvas.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2);
            _dressUpSystem.SetUIPosition(contentPane.workerTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, 0.8f);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_dressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
            _dressUpSystem.Play(idleAnimationName, true);
        }
        AudioManager.inst.PlaySound(25);
        contentPane.lvTipText.text = LanguageManager.inst.GetValueByKey("恭喜，员工已经达到等级{0}！", worker.level.ToString()); ;
        contentPane.professionTx.text = LanguageManager.inst.GetValueByKey(worker.config.profession);
        contentPane.lvText.text = worker.level.ToString();

        contentPane.professionIcon.SetSprite("worker_atlas", worker.config.profession_icon);
        contentPane.lastSpeedTx.text = "+" + worker.lastSpeed + "%";
        contentPane.curSpeedTx.text = "+" + worker.addSpeed + "%";

    }

    void onContinueBtnClick()
    {
        hide();
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(25);
    }

    protected override void onHide()
    {
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }
}
