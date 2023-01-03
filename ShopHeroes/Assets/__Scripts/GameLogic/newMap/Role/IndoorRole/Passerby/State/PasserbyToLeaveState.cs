using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//街道行人离开的状态
public class PasserbyToLeaveState : StateBase
{
    Passerby _passerby;
    StateMachine _machine;

    public PasserbyToLeaveState(Passerby passerby, StateMachine machine)
    {
        _passerby = passerby;
        _machine = machine;
    }

    bool canLeave;

    public override int getState()
    {
        return (int)MachinePasserbyState.toLeave;
    }



    public override void onEnter(StateInfo info)
    {
        canLeave = false;


        //Logger.error("街道行人 进入了 离开 的状态");
    }

    public override void onUpdate()
    {
        if (!canLeave && moveLeave())
        {
            if (_passerby.Character != null) _passerby.Character.SetActive(true);
            canLeave = true;
        }

        if (canLeave && !_passerby.isMoving) //到地方了
        {
            IndoorRoleSystem.inst.RemovePasserby(_passerby.passerbyUid);
        }
    }


    public override void onExit()
    {

    }

    //离开
    bool moveLeave()
    {
        Vector3Int endPos = Vector3Int.zero;

        if (_passerby.data.streetDropData != null) //下完蛋走的
        {
            endPos = _passerby.data.streetDropData.dropPos;
            if (_passerby.Character.direction == RoleDirectionType.Left) endPos.y = 38;
            else endPos.y = -10;
        }
        else
        {
            endPos = new Vector3Int(_passerby.data.config.pos_x, _passerby.data.config.pos_y, 0);
            if (_passerby.data.config.direction == 0) endPos.y = -10;
            else endPos.y = 38;
        }

        Stack<PathNode> movepath = IndoorMap.inst.FindPath(_passerby.currCellPos, endPos);
        if (movepath.Count > 1)
        {
            _passerby.move(movepath);
            return true;
        }

        if (_passerby.Character != null) _passerby.Character.SetActive(false);
        return false;
    }
}
