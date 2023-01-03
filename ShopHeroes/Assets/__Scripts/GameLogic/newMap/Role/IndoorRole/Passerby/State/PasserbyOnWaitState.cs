using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasserbyOnWaitState : StateBase
{
    Passerby _passerby;
    StateMachine _machine;

    public PasserbyOnWaitState(Passerby passerby, StateMachine machine)
    {
        _passerby = passerby;
        _machine = machine;
    }

    public override int getState()
    {
        return (int)MachinePasserbyState.wait;
    }

    public override void onEnter(StateInfo info)
    {
        if (_passerby.Character != null) _passerby.Character.SetActive(false);
        _passerby.stopMove();

        if (IndoorRoleSystem.inst != null) IndoorRoleSystem.inst.SetPasserbyOriCachePos(_passerby.passerbyUid);

        //Logger.error("街道行人 进入了 暂停 的状态");
    }

    public override void onUpdate()
    {

    }


    public override void onExit()
    {
    }

}
