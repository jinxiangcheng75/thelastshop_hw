using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateVisitShop : IStateBase
{
    public kGameState getState()
    {
        return kGameState.VisitShop;
    }
    public void onEnter(IStateTransition transition)
    {
    }
    public void onExit()
    {
    }

    public void onReset()
    {
    }
}
