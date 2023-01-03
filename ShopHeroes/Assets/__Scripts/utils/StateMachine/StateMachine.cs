using System.Collections.Generic;

public class StateMachine
{
    private IState _curState;
    private IState _lastState;
    private Dictionary<int, IState> _stateDic;

    public StateMachine()
    {
        _stateDic = new Dictionary<int, IState>();
    }

    public void Init(List<IState> states)
    {
        foreach (var item in states)
        {
            AddState(item);
        }
    }

    public void AddState(IState state)
    {
        if (!_stateDic.ContainsKey(state.getState()))
        {
            _stateDic.Add(state.getState(), state);
        }
        else
        {
            Logger.error("重复key值的状态");
        }
    }

    public void DelState(int state)
    {
        if (_stateDic.ContainsKey(state))
        {
            _stateDic.Remove(state);
        }
    }

    public void SetState(int state, StateInfo info)
    {
        if (_stateDic.ContainsKey(state))
        {
            if (_lastState != null)
            {
                _lastState.onExit();
            }

            _curState = _stateDic[state];
            _curState.onEnter(info);

            _lastState = _curState;
        }
    }

    public int GetCurState()
    {
        if (_curState != null) return _curState.getState();

        return -1;
    }

    public void OnUpdate()
    {
        if (_curState != null)
        {
            _curState.onUpdate();
        }
    }



}
