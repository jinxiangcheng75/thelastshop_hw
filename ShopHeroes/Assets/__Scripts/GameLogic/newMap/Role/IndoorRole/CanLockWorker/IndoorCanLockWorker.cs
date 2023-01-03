using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndoorCanLockWorker : IndoorRole
{

    public Transform tf_actorNode;

    int _workerId = -1;

    float otherIdleAnimTimer;
    public float otherIdleAnimTime = 5f;

    float talkTimer;
    float talkTimeSpacing = 10f;

    float talkBubbleShowTime = 2f;

    public InputEventListener inputListener;

    protected override void Init()
    {
        base.Init();
        inputListener.OnClick += onClickHandler;

        talkTimeSpacing = WorldParConfigManager.inst.GetConfig(177).parameters;
        talkBubbleShowTime = WorldParConfigManager.inst.GetConfig(178).parameters;

    }

    public void SetData(int workerId)
    {
        var workerCfg = WorkerConfigManager.inst.GetConfig(workerId);

        if (_character == null)
        {
            _workerId = workerId;

            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(workerCfg.model, callback: (system) =>
            {
                _character = system;
                _character.transform.SetParent(tf_actorNode);
                _character.transform.localPosition = Vector3.zero;
                _character.SetDirection(RoleDirectionType.Left);
                onCharacterCreated();
            });
        }
        else
        {
            if (_workerId != workerId) //模型发生变化 才刷新
            {
                _workerId = workerId;

                CharacterManager.inst.ReSetCharacterByModel(_character, workerCfg.model, repackedCallback: (system) =>
                {
                    onCharacterCreated();
                });
            }
        }

    }

    void onClickHandler(Vector3 mousePos)
    {
        if (_character != null && isVisible && _workerId != -1)
        {
            EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, _workerId, false, null);
        }
    }

    void onCharacterCreated()
    {
        gameObject.name = "canlockWorker_" + _workerId;
        gameObject.transform.position = new Vector3(-9.6f, 5.2f, 0);

        setOrder(1, "Actors_PickUp");
        SetTalkSpacing(talkBubbleShowTime);

        string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
        _character.Play(idleAnimationName, true);

        Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRanomTalkMsg(6, 100 + _workerId)));

    }


    private void Update()
    {
        if (_character != null && isVisible)
        {
            otherIdleAnimTimer += Time.deltaTime;

            if (otherIdleAnimTimer >= otherIdleAnimTime)
            {
                otherIdleAnimTimer = 0;
                otherIdleAnim();
            }

            talkTimer += Time.deltaTime;

            if (talkTimer >= talkTimeSpacing)
            {
                talkTimer = 0;
                Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRanomTalkMsg(6, 100 + _workerId)));
            }

        }

    }

    //换个姿势
    private void otherIdleAnim()
    {
        if (Helper.randomResult(50)) // 1/2 的概率
        {
            string specialIdleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.special_standby);
            _character.Play(specialIdleAnimationName, completeDele: t =>
            {
                if (this == null) return;
                string idleAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                _character.Play(idleAniName, true);
            });
        }
    }


}
