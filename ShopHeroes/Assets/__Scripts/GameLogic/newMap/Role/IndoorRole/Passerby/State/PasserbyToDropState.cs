using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//街道行人去下蛋的状态
public class PasserbyToDropState : StateBase
{
    Passerby _passerby;
    StateMachine _machine;

    public PasserbyToDropState(Passerby passerby, StateMachine machine)
    {
        _passerby = passerby;
        _machine = machine;
    }

    bool canMove;

    public override int getState()
    {
        return (int)MachinePasserbyState.toDrop;
    }

    public override void onEnter(StateInfo info)
    {
        canMove = false;

        //Logger.error("街道行人 进入了 去下蛋 的状态");

        if (_passerby.data.streetDropData.state == StreetDropState.waitPick)
        {
            _machine.SetState((int)MachinePasserbyState.toLeave, null);
            return;
        }

        //_passerby.Character.skeletonColor = Color.green; //临时 与众不同
    }

    public override void onUpdate()
    {

        if (!canMove && moveToDrop())
        {
            canMove = true;
        }

        if (canMove && !_passerby.isMoving) //到地方了
        {
            _machine.SetState((int)MachinePasserbyState.toLeave, null);
        }

    }


    public override void onExit()
    {
        //下蛋
        var streetDrop = IndoorMap.inst.CreateStreetDropItem(_passerby.data.streetDropData);
        IndoorRoleSystem.inst.AddStreetDrop(streetDrop);

        //_passerby.Character.skeletonColor = Color.white;//回归大众
    }

    //去下蛋
    bool moveToDrop()
    {
        var endpos = _passerby.data.streetDropData.dropPos;

        Stack<PathNode> movepath = IndoorMap.inst.FindPath(_passerby.currCellPos, endpos);
        if (movepath.Count > 1)
        {
            _passerby.move(movepath);
            return true;
        }
        return false;
    }

}
