using UnityEngine;
using System.Collections;

public class GameStateSplash : IStateBase
{

    public void onEnter(IStateTransition transition)
    {
        GameTimer.inst.StartCoroutine(delayChange());
    }

    IEnumerator delayChange()
    {
        yield return new WaitForSeconds(1f);
        // FGUI.inst.startvideotf.gameObject.SetActive(true);
        // FGUI.inst.videoMusic.Play();
        yield return new WaitForSeconds(1f);
        FGUI.inst.StartExcessAnimation(false, false, null);
        yield return new WaitForSeconds(1.5f);
        //GameStateEvent.inst.changeState(new StateTransition(kGameState.Preload, false));
        HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Preload, false));
        //FGUI.inst.sceneExcess.DOPlayBackwards();

    }

    public void onExit()
    {
        GameTimer.inst.StopCoroutine(delayChange());
    }

    public void onReset()
    {

    }

    public kGameState getState()
    {
        return kGameState.Splash;
    }
}
