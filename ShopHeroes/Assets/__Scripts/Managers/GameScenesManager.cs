using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
public class GameScenesManager
{
    //场景加载

    bool startLoadScene = false;

    public void loadSceneAsync(string name, LoadSceneMode mode, System.Action loaded)
    {
        if (startLoadScene) return;
        GameTimer.inst.StartCoroutine(loadIndoorMap(name, mode, loaded));
    }

    IEnumerator loadIndoorMap(string name, LoadSceneMode mode, System.Action loaded)
    {
        if (!startLoadScene)
        {
            startLoadScene = true;
            //加载场景(Add)
            AsyncOperationHandle operation = ManagerBinder.inst.mSceneMgr.loadScene(name);
            while (!operation.IsDone)
            {
                yield return null;
            }
            yield return 0;
            if (loaded != null)
            {
                loaded.Invoke();
            }
        }
    }

    public void UnLoadScene(string name)
    {
        startLoadScene = false;
        // GameTimer.inst.StopCoroutine(loadIndoorMap());
        if (currScene == name)
        {
            Addressables.UnloadSceneAsync(currHandle);
            if (extiaScene.ContainsKey(name))
            {
                Addressables.UnloadSceneAsync(extiaScene[name]);
                extiaScene.Remove(name);
            }
        }
    } 

    public void ToStartScene()
    {
        if (!string.IsNullOrEmpty(currScene))
        {
            UnLoadScene(currScene);
        }
        loadScene("ReLogin");
    }
    //////////////new
    public string currScene = "";
    public string lastScene = "";

    AsyncOperationHandle currHandle;
    private Dictionary<string, AsyncOperationHandle<SceneInstance>> extiaScene = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
    public AsyncOperationHandle loadScene(string name, string extia = null)
    {
        if (currScene != name)
        {
            if (extiaScene.ContainsKey(currScene))
            {
                Addressables.UnloadSceneAsync(extiaScene[currScene]);
                extiaScene.Remove(currScene);
            }
            currScene = name;
            currHandle = Addressables.LoadSceneAsync(currScene);
            currHandle.Completed += (handle) =>
            {
                if (!string.IsNullOrEmpty(extia))
                {
                    var extiaHandle = Addressables.LoadSceneAsync(extia, LoadSceneMode.Additive);
                    extiaScene.Add(currScene, extiaHandle);
                }
            };
        }
        return currHandle;
    }

}
