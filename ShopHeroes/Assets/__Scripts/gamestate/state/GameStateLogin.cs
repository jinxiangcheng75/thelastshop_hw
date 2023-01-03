using UnityEngine;
using System.Collections;

public class GameStateLogin : IStateBase
{
    public void onEnter(IStateTransition transition)
    {
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_LOGIN);
        //FGUI.inst.StartExcessAnimation(false, null);
    }

    public void onExit()
    {
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_LOGIN);

    }

    public void onReset()
    {

    }

    public kGameState getState()
    {
        return kGameState.Login;
    }
}
