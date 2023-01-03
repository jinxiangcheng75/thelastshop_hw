using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//街道行人创建的状态
public class PasserbyOnCreateState : StateBase
{
    Passerby _passerby;
    StateMachine _machine;

    public PasserbyOnCreateState(Passerby passerby, StateMachine machine)
    {
        _passerby = passerby;
        _machine = machine;
    }

    public override int getState()
    {
        return (int)MachinePasserbyState.onCreate;
    }

    public override void onEnter(StateInfo info)
    {


        //Logger.error("街道行人 进入了 被创建 的状态");

        _passerby.Character.SetActive(true);
        setPos();

        if (_passerby.data.streetDropData == null) //普通路人
        {
            _machine.SetState((int)MachinePasserbyState.toLeave, null);
        }
        else //会下蛋的路人
        {
            if (_passerby.data.streetDropData.state == StreetDropState.notDrop)
            {
                _machine.SetState((int)MachinePasserbyState.toDrop, null);
            }
            else
            {
                _machine.SetState((int)MachinePasserbyState.toLeave, null);
            }
        }
    }

    public override void onUpdate()
    {
    }


    public override void onExit()
    {

    }

    Vector3Int getOriPos()
    {
        Vector3Int oriPos = IndoorRoleSystem.inst.GetPasserbyOriCachePos(_passerby.passerbyUid);
        IndoorRoleSystem.inst.DelPasserbyOriCachePos(_passerby.passerbyUid);

        if (oriPos == Vector3Int.zero) oriPos = new Vector3Int(_passerby.data.config.pos_x, _passerby.data.config.pos_y, 0);//若无缓存 返还表中初始位置

        return oriPos;
    }

    void setPos()
    {
        Vector3Int oriPos = getOriPos();

        _passerby.SetCellPos(oriPos);
        _passerby.Character.SetDirection(_passerby.data.config.direction == 0 ? RoleDirectionType.Left : RoleDirectionType.Right);
        _passerby.UpdateSortingOrder();
    }


}
