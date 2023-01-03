using UnityEngine;
using System.Collections;

public interface IStateTransition
{
    kGameState state { get; }
    bool loading { get; }
}

public struct StateTransition : IStateTransition
{
    public kGameState state { get; }

    public bool loading { get; }

    public StateTransition(kGameState _state, bool _loading)
    {
        state = _state;
        loading = _loading;
    }
}

public interface IStateBase
{
    void onEnter(IStateTransition transition);
    void onExit();
    void onReset();
    kGameState getState();
}

public abstract class AbstractGameStateBase : IStateBase
{
    public kGameState getState()
    {
        throw new System.NotImplementedException();
    }

    public void onEnter(IStateTransition transition)
    {
        throw new System.NotImplementedException();
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
