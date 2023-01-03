using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasserbyData
{
    public StreetRoleOriPosConfig config;

    public StreetDropData streetDropData;

    public PasserbyData(StreetRoleOriPosConfig cfg)
    {
        config = cfg;
    }

    public PasserbyData(StreetDropData _streetDropData)
    {
        config = StreetRolePosConfigManager.inst.GetDropConfig();
        streetDropData = _streetDropData;
    }

}

public enum MachinePasserbyState
{
    none,
    wait, //等待
    onCreate,//创建
    toDrop,//去下蛋
    toLeave,//离开
}

//街道路人
public class Passerby : IndoorRole
{
    [HideInInspector]
    public int passerbyUid;
    [HideInInspector]
    public PasserbyData data;

    public uint creatShopkeeperLv;
    StateMachine _machine;

    protected override void Init()
    {
        base.Init();

        gameObject.name = "Passerby";

        _machine = new StateMachine();

        _machine.Init(new List<IState> {
           new PasserbyOnWaitState(this,_machine),
           new PasserbyOnCreateState(this,_machine),
           new PasserbyToDropState(this,_machine),
           new PasserbyToLeaveState(this,_machine),
        });

    }

    public void SetData(int _passerbyUid, PasserbyData _data)
    {
        passerbyUid = _passerbyUid;
        data = _data;

        creatRole();
    }

    private void Update()
    {
        if (_machine != null) _machine.OnUpdate();
    }

    public MachinePasserbyState GetCurState()
    {
        if (_machine != null) return (MachinePasserbyState)_machine.GetCurState();

        return MachinePasserbyState.none;
    }

    public void SetState(MachinePasserbyState state, StateInfo Info = null)
    {
        if (_machine != null) _machine.SetState((int)state, Info);
    }

    void creatRole()
    {
        if (_character == null)
        {
            dressconfigManager.inst.GetRandomDressList(out EGender gender, out List<int> facadeList, out List<int> equipList);
            int weaponType_2 = -1;

            foreach (var equipid in equipList)
            {
                var cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipid);
                if (cfg.equipDrawingsConfig.type == 1)
                {
                    weaponType_2 = cfg.equipDrawingsConfig.sub_type;
                    break;
                }
            }

            CharacterManager.inst.GetCharacterByHero<DressUpSystem>(gender, equipList, facadeList, callback: (system) =>
            {
                _character = system;
                var go = _character.gameObject;
                go.transform.SetParent(_attacher.actorParent);
                go.transform.localPosition = Vector3.zero;

                StartCoroutine(placeWeapon(weaponType_2));
            });

            creatShopkeeperLv = UserDataProxy.inst.playerData.level;
        }
        else
        {
            if (creatShopkeeperLv == UserDataProxy.inst.playerData.level)
            {
                onCreateEnd();
            }
            else
            {
                ReSetDress();
            }
        }

    }

    public void ReSetDress()
    {
        dressconfigManager.inst.GetRandomDressList(out EGender gender, out List<int> facadeList, out List<int> equipList);
        int weaponType_2 = -1;

        foreach (var equipid in equipList)
        {
            var cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipid);
            if (cfg.equipDrawingsConfig.type == 1)
            {
                weaponType_2 = cfg.equipDrawingsConfig.sub_type;
                break;
            }
        }

        CharacterManager.inst.ReSetCharacterByHero(_character, gender, equipList, facadeList, true, (system) =>
        {
            if (weaponType_2 != -1)//携带了武器
            {
                string weaponSlotName = gender == EGender.Male ? StaticConstants.man_weapon_slotName : StaticConstants.woman_weapon_slotName;
                string placeWeaponSlotName = SpineUtils.GetPlaceWeaponSlotName(gender, (EquipSubType)weaponType_2, out string temp);

                EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(weaponType_2);
                if (classcfg != null)
                {
                    float slotRot = gender == EGender.Male ? classcfg.m_rotation_angle : classcfg.g_rotation_angle;
                    _character.AttToAnotherSlot(weaponSlotName, placeWeaponSlotName, slotRot, classcfg.GetSlotPos(gender), classcfg.GetSlotScale(gender));
                }
            }

            onCreateEnd();
        });

        creatShopkeeperLv = UserDataProxy.inst.playerData.level;
    }

    public void Pause()
    {
        SetState(MachinePasserbyState.wait);
    }

    public void Resume()
    {
        SetState(MachinePasserbyState.onCreate);
    }

    /// <summary>
    /// 初始生成时 将武器放到背后或腰间
    /// </summary>
    IEnumerator placeWeapon(int weaponEquipType2)
    {
        while (_character.isInDressing)
        {
            yield return null;
        }


        if (weaponEquipType2 != -1)//携带了武器
        {
            EGender gender = _character.gender;
            string weaponSlotName = gender == EGender.Male ? StaticConstants.man_weapon_slotName : StaticConstants.woman_weapon_slotName;
            string placeWeaponSlotName = SpineUtils.GetPlaceWeaponSlotName(gender, (EquipSubType)weaponEquipType2, out string temp);

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(weaponEquipType2);
            if (classcfg != null)
            {
                float slotRot = gender == EGender.Male ? classcfg.m_rotation_angle : classcfg.g_rotation_angle;
                _character.AttToAnotherSlot(weaponSlotName, placeWeaponSlotName, slotRot, classcfg.GetSlotPos(gender), classcfg.GetSlotScale(gender));
            }
        }

        onCreateEnd();
    }

    void onCreateEnd()
    {
        if (ManagerBinder.inst.mGameState != kGameState.Shop)
        {
            Pause();
        }
        else
        {
            SetState(MachinePasserbyState.onCreate);
        }

        setOrder(2);
    }

    protected override void onMoveStepComplete(Vector3 curPos, Vector3 nextPos)
    {
        if (_character != null) _character.SetDirection(getDir(MapUtils.WorldPosToCellPos(curPos), MapUtils.WorldPosToCellPos(nextPos)));
    }

    protected override void onMoveEndComplete()
    {
        if (_character != null)
        {
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
            _character.Play(idleAnimationName, true);
        }
    }

}
