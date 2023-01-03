using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateCreatRole : IStateBase
{
    BaseSystem createRoleSystem;
    public kGameState getState()
    {
        return kGameState.CreatRole;
    }

    public void onEnter(IStateTransition transition)
    {
        Logger.log("GameStateCreatRole.onEnter");
        CreatRoleProxy.inst.Init();     //创角
        createRoleSystem = new CreatRoleSystem();
        // FGUI.inst.startvideotf.gameObject.SetActive(false);
        createRoleSystem.OnEnter();
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_CREATROLEPANEL);
        FGUI.inst.StartExcessAnimation(false, false);
    }

    public void onExit()
    {
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_CREATROLEPANEL);
        CreatRoleProxy.inst.Clear();
        createRoleSystem.OnExit();
    }

    public void onReset()
    {

    }
}
