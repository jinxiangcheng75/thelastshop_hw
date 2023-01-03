using UnityEngine;
using System.Collections;

public class GameStateLoading : IStateBase
{

    IStateTransition mTransition;
    public void onEnter(IStateTransition transition)
    {
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_LOADINGUI);
        mTransition = transition;
        // FGUI.inst.StartExcessAnimation(false, null);
    }

    void loadAsset()
    {
        // ManagerBinder.inst.Asset.loadSceneAsync(getScene(mTransition.state), () =>
        // {
        //     GameStart.Inst.StartCoroutine(delayChange());
        // });
    }

    IEnumerator delayChange()
    {
        yield return new WaitForSeconds(2f);
        //  GameStateEvent.inst.changeState(new StateTransition(mTransition.state, false));

    }

    public void onExit()
    {
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_LOADINGUI);
    }

    public void onReset()
    {

    }
    public kGameState getState()
    {
        return kGameState.Loading;
    }
}
