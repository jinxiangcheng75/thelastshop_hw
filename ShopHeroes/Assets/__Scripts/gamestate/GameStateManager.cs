using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.poptiger.events;

public enum kGameState
{
    Splash,//闪屏
    Preload,//预加载
    Login,//登录
    Loading,//加载
    CreatRole,//创角
    Shop,//商店
    Town,//城市
    Tavern,//酒馆
    Battle,//战斗
    Union,//公会
    VisitShop, // 拜访店铺
    TBox, // 宝箱
    Update,
    Ruins,//废墟
    Num
}

public class GameStateEvent : TSingletonHotfix<GameStateEvent>
{
    public event EventDelegate<IStateTransition> onChangeState;
    public void changeState(IStateTransition transition) { if (onChangeState != null) onChangeState(transition); }
}

public class GameStateManager
{
    public IStateBase lastState;
    public IStateBase nextState;
    public IStateBase mCurState;
    IStateBase[] mStates;
    public GameStateManager()
    {
        mStates = new IStateBase[(int)kGameState.Num];
        init();
    }

    void init()
    {
        mStates[(int)kGameState.Splash] = new GameStateSplash();
        mStates[(int)kGameState.Preload] = new GameStatePreload();
        mStates[(int)kGameState.Login] = new GameStateLogin();
        mStates[(int)kGameState.CreatRole] = new GameStateCreatRole();
        mStates[(int)kGameState.Loading] = new GameStateLoading();
        mStates[(int)kGameState.Shop] = new GameStateShop();
        mStates[(int)kGameState.Town] = new GameStateTown();
        mStates[(int)kGameState.Tavern] = new GameStateTavern();
        mStates[(int)kGameState.Battle] = new GameStateBattle();
        GameStateEvent.inst.onChangeState += changeState;
    }

    IStateTransition targetTransition;
    void changeState(IStateTransition transition)
    {

        targetTransition = transition;
        //FGUI.inst.sceneExcess.DOPlayForward();
        if (!transition.loading && kGameState.Loading != transition.state && transition.state != kGameState.Preload && transition.state != kGameState.Login)
        {
            Logger.log("[State] changeState : " + transition.state + "  loading:" + transition.loading);
            FGUI.inst.StartExcessAnimation(true, transition.state == kGameState.Shop, null);
        }
        GameTimer.inst.AddTimer(0.5f, 1, doStateChange);
    }

    void doStateChange()
    {
        D2DragCamera.inst.onsceneChange = true;
        if (mCurState != null)
        {
            mCurState.onExit();
            lastState = mCurState;
        }
        if (targetTransition.loading)
        {
            IStateBase loadingState = mStates[(int)kGameState.Loading];
            mCurState = loadingState;
            loadingState.onEnter(targetTransition);
            return;
        }
        IStateBase nextState = mStates[(int)targetTransition.state];
        mCurState = nextState;
        nextState.onEnter(targetTransition);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    public void clear()
    {
        GameStateEvent.inst.onChangeState -= changeState;
    }
}
