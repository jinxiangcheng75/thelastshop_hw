using UnityEngine;
using System.Collections;

public class GameStateBattle : IStateBase
{
    bool startLoadScene = false;
    public kGameState getState()
    {
        return kGameState.Battle;
    }

    public void onEnter(IStateTransition transition)
    {
        EventController.inst.AddListener(GameEventType.CombatEvent.COMBAT_SCENE_LOADED, SceneLoadEnd);
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_INITSCENE);
    }
    private void SceneLoadEnd()
    {
        // ManagerBinder.inst.Asset.clear();
        //初始化  战斗角色
        FGUI.inst.StartExcessAnimation(false, false, null);
    }

    public void onExit()
    {
        EventController.inst.RemoveListener(GameEventType.CombatEvent.COMBAT_SCENE_LOADED, SceneLoadEnd);
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_EXIT);
    }

    public void onReset()
    {
    }
}
