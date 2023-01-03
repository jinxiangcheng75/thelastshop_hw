using UnityEngine;

[RequireComponent(typeof(UnionRoleAttacher))]
public class UnionRole : RoleBase
{
    protected DressUpSystem _character; //可直接播放动画 改变方向
    protected UnionRoleAttacher _attacher;//头顶信息

    public DressUpSystem Character { get { return _character; } }

    public InputEventListener inputListener;

    UnionMemberInfo _info;
    bool _init;

    protected override void Init()
    {
        inputListener.OnClick += onClickHandler;
        _attacher = GetComponent<UnionRoleAttacher>();
        _attacher.SetClickHandler(onClickHandler);
    }

    string getString(EUnionJob eUnionJob)
    {
        string result = "";
        switch (eUnionJob)
        {
            case EUnionJob.Common:
                result = "成员";
                break;
            case EUnionJob.Manager:
                result = "管理员";
                break;
            case EUnionJob.President:
                result = "会长";
                break;
        }

        return result;
    }

    public void SetData(UnionMemberInfo info, Transform parent, string sortingLayer, int order)
    {
        _init = true;
        _info = info;

        bool needTurn = parent.name.EndsWith("-1");

        if (_character == null)
        {
            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)info.gender), SpineUtils.RoleDressToUintList(info.roleDress), (EGender)info.gender, callback: (dressUpSystem) =>
            {
                _character = dressUpSystem;
                _character.transform.SetParent(transform);
                _character.transform.localScale = Vector3.one;
                _character.transform.localPosition = Vector3.zero;
                _character.SetSortingAndOrderLayer(sortingLayer, order);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                _character.Play(idleAnimationName, true);

                transform.SetParent(parent);
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;

                _character.SetDirection(needTurn ? RoleDirectionType.Right : RoleDirectionType.Left);

            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(_character, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)info.gender), SpineUtils.RoleDressToUintList(info.roleDress), (EGender)info.gender);
            _character.transform.SetParent(transform);
            _character.transform.localScale = Vector3.one;
            _character.transform.localPosition = Vector3.zero;
            _character.SetSortingAndOrderLayer(sortingLayer, order);

            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;

            _character.SetDirection(needTurn ? RoleDirectionType.Right : RoleDirectionType.Left);

        }

        _attacher.SetInfo(_info.level, _info.nickName + "[" + LanguageManager.inst.GetValueByKey(getString((EUnionJob)info.memberJob)) + "]");
        _attacher.SetLayerAndSortingOrder(sortingLayer, order);
    }

    bool isOtherSideHasPeople { get { return transform.parent.parent.GetComponentsInChildren<UnionRole>(false).Length > 1; } }


    void onClickHandler(Vector3 mousePos)
    {
        EventController.inst.TriggerEvent(GameEventType.SocialEvent.REQUEST_OTHERUSERDATA, _info.userId);
    }


    float timer;
    float curTime = -1;
    float time_min = 5;
    float time_max = 8;
    bool isChating = false; //在聊天！？

    private void Update()
    {
        if (!_init) return;

        if (curTime == -1)
        {
            curTime = Random.Range(time_min, time_max);
        }

        timer += Time.deltaTime;

        if (timer > curTime)
        {
            timer = 0;
            curTime = -1;

            //随机动作

            int ran = Random.Range(0, 10);

            if (ran < 5) //聊天
            {

                if (isOtherSideHasPeople) //对面有哥们
                {
                    string bargainingAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.haggle);
                    _character.Play(bargainingAnimationName, true);
                    isChating = true;
                }
                else
                {

                }

            }
            else if (ran < 8) //踮脚
            {
                string specialIdleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.special_standby);
                _character.Play(specialIdleAnimationName, completeDele: t =>
                {
                    if (this == null) return;
                    string idleAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                    _character.Play(idleAniName, true);
                });

            }
            else //普通待机
            {
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                _character.Play(idleAnimationName, true);
                isChating = false;
            }


        }


    }

    public void SetActive(bool active)
    {
        if (_character != null) _character.SetActive(active);
        if (_attacher != null) _attacher.gameObject.SetActive(active);
    }

}
