using UnityEngine;
using System.Collections;

public class GameStateTavern : IStateBase
{
    public kGameState getState()
    {
        return kGameState.Tavern;
    }

    public void onEnter(IStateTransition transition)
    {
        FGUI.inst.StartExcessAnimation(false, false, null);
    }

    public void onExit()
    {
        throw new System.NotImplementedException();
    }

    public void onReset()
    {
        throw new System.NotImplementedException();
    }
}
