using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetStayDoorWay : StateBase
{

    Pet _pet;

    public PetStayDoorWay(Pet pet)
    {
        _pet = pet;
    }

    public override int getState()
    {
        return (int)MachinePetState.stayDoorway;
    }

    public override void onEnter(StateInfo info)
    {
        Logger.log("!@#$%^&___宠物进入了 去门口 的状态");

        if (!moveToDoorway())
        {
            _pet.SetState(MachinePetState.ramble);
        }
        else
        {
            _pet.moveEndCompleteHandler = () =>
            {
                _pet.Character.Play("call",true);
            };
        }
    }

    public override void onUpdate()
    {

    }

    public override void onExit()
    {
        _pet.moveEndCompleteHandler = null;
    }


    //进屋
    private bool moveToDoorway()
    {
        var endPos = IndoorMap.inst.GetIndoorPoint();

        if (endPos == Vector3Int.zero)
        {
            return false;
        }
        else
        {
            Stack<PathNode> movepath = IndoorMap.inst.FindPath(_pet.currCellPos, endPos);
            if (movepath.Count > 1)
            {
                _pet.move(movepath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
