using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetStayBySeeState : StateBase
{

    Pet _pet;

    public PetStayBySeeState(Pet pet)
    {
        _pet = pet;
    }

    public override int getState()
    {
        return (int)MachinePetState.stayBySee;
    }

    public override void onEnter(StateInfo info)
    {
        Logger.log("!@#$%^&___宠物进入了 被观赏 的状态");
        _pet.stopMove();
        _pet.Character.Play("idle", true);
    }


    public override void onUpdate()
    {

    }

    public override void onExit()
    {

    }

}
