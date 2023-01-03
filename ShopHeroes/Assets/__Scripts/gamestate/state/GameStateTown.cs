using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameStateTown : IStateBase
{
    public kGameState getState()
    {
        return kGameState.Town;
    }
    public void onEnter(IStateTransition transition)
    {
        // ManagerBinder.inst.Asset.clear();
        //获取城市建筑信息
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.CITYBUILDING_GET_DATA);
        //获取冒险槽位信息
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREGROUPDATA);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTDATA);
        //TreasureBoxDataProxy.inst.isInit = false;
        //加载场景
        startLoadScene = false;
        GameTimer.inst.StartCoroutine(loadIndoorMap());
    }
    bool startLoadScene = false;
    IEnumerator loadIndoorMap()
    {

        if (!startLoadScene)
        {
            startLoadScene = true;
            //加载场景(Add)
            AsyncOperationHandle operation = ManagerBinder.inst.mSceneMgr.loadScene("CityScene_light");
            while (!operation.IsDone)
            {
                yield return null;
            }
            AudioManager.inst.PlayMusic(2);
            FGUI.inst.StartExcessAnimation(false, false);
            GUIManager.OpenView<CityMainView>();
            GUIManager.OpenView<TopPlayerInfoView>();
        }
    }

    public void onExit()
    {
        startLoadScene = false;
        GameTimer.inst.StopCoroutine(loadIndoorMap());
        GUIManager.HideView<CityMainView>();
        SceneManager.UnloadSceneAsync("CityScene_light");
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_BUILDINGUPCLEAR);//清理HUD
    }

    public void onReset()
    {

    }
}
