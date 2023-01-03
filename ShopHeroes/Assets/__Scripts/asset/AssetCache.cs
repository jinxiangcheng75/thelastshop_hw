using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.AddressableAssets.Addressables;

public interface IAssetCache
{
    void InstantiateUIAsync(string assetKey, System.Action<GameObject> callback);
    void getSpriteAsync(string spriteName, System.Action<GSprite> callback);
    void loadPrefabAsync(string assetKey, Transform parent, System.Action<GameObject> callback);
    void loadMiscAsset<T>(string assetKey, System.Action<T> callback);
    void unloadMiscAsset<T>(string assetKey, T assetObj);
    void InstantiatePrefabAsync(string assetKey, System.Action<GameObject> callback);
    AsyncOperationHandle LoadMiscAssetsAsync(List<string> urls);
    void clear();
}
public interface IAssetInstanceHandler
{
    void handleInstanceDestroy(string key, GameObject go);
}
public interface IAssetManualHandler
{
    void handleAllDestroyed(string key, bool removeKey = true);
}

#region 游戏图集加载管理
public class AssetLoader
{
    public string assetName;
    Action<string, AsyncOperationHandle> callback;
    public bool loading = false;
    public AsyncOperationHandle LoadAtlas(string key, System.Action<string, AsyncOperationHandle> _callback)
    {
        assetName = key;
        callback += _callback;
        return LoadAtlasAsync();
    }
    public void AddListenerCallBack(System.Action<string, AsyncOperationHandle> _callback)
    {
        callback += _callback;
    }
    AsyncOperationHandle LoadAtlasAsync()
    {
        loading = true;
        AsyncOperationHandle handle = Addressables.LoadAssetAsync<SpriteAtlas>(assetName);
        handle.Completed += loadAtlasEnd;
        return handle;
    }
    void loadAtlasEnd(AsyncOperationHandle atlashandle)
    {
        //if (callback != null)
        callback?.Invoke(assetName, atlashandle);
        callback = null;
        loading = false;
    }
    public void Release()
    {
        callback = null;
        loading = false;
    }
}
public class GSprite
{
    public AsyncOperationHandle atlasOperationHandle;
    public string handlename;
    public string spritename;
    public bool isAtlasSprite = true;
    public static Dictionary<string, int> referenceCounts = new Dictionary<string, int>();
    public static void addReference(string handlename)
    {
        if (referenceCounts.ContainsKey(handlename))
        {
            referenceCounts[handlename] += 1;
        }
        else
        {
            referenceCounts.Add(handlename, 1);
        }
    }
    public static void subReference(string handlename)
    {
        if (referenceCounts.ContainsKey(handlename))
        {
            referenceCounts[handlename] -= 1;
        }
    }

    public static int SpriteReferenceCount(string handlename)
    {
        if (referenceCounts.ContainsKey(handlename))
        {
            return referenceCounts[handlename];
        }
        return 0;
    }


    public GSprite(string _name, string _spritename, AsyncOperationHandle _atlasOperationHandle)
    {
        handlename = _name;
        spritename = _spritename;
        isAtlasSprite = !string.IsNullOrEmpty(handlename);
        atlasOperationHandle = _atlasOperationHandle;
        if (isAtlasSprite)
            GSprite.addReference(HandleKey);
    }

    string HandleKey
    {
        get
        {
            return isAtlasSprite ? handlename : spritename;
        }
    }
    public Sprite sprite
    {
        get
        {
            if (atlasOperationHandle.IsValid())
            {
                if (isAtlasSprite)
                {
                    SpriteAtlas atlas = atlasOperationHandle.Result as SpriteAtlas;
                    if (atlas != null)
                    {
                        return atlas.GetSprite(spritename);
                    }
                }
                else
                {
                    return atlasOperationHandle.Result as Sprite;
                }
            }
            return null;
        }
    }

    public void release()
    {
        if (isAtlasSprite)
        {
            subReference(HandleKey);
            if (SpriteReferenceCount(HandleKey) <= 0)
            {
                if (atlasOperationHandle.IsValid())
                    Addressables.Release(atlasOperationHandle);
            }
        }
        else
        {
            Addressables.Release(atlasOperationHandle);
        }
    }
}

//游戏图集资源管理
public class AtlasAssetHandler : TSingletonHotfix<AtlasAssetHandler>
{
    Dictionary<string, AsyncOperationHandle> spriteaAtlasList = new Dictionary<string, AsyncOperationHandle>();
    //
    Dictionary<string, AssetLoader> atlasLoaderList = new Dictionary<string, AssetLoader>();

    ///检测图集是否已缓存
    public bool CheckSpriteAtlas(string name)
    {
        if (spriteaAtlasList.ContainsKey(name))
        {
            if (spriteaAtlasList[name].IsValid())
            {
                if (spriteaAtlasList[name].Status == AsyncOperationStatus.Succeeded)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void AtlasLoad(string assetKey, AsyncOperationHandle atlashandle)
    {
        if (spriteaAtlasList.ContainsKey(assetKey))
        {
            spriteaAtlasList[assetKey] = atlashandle;
        }
        else
        {
            spriteaAtlasList.Add(assetKey, atlashandle);
        }
        atlasLoaderList.Remove(assetKey);
    }


    //强制释放列表所有图集（UI使用中的图集除外）
    public void ReleaseAtlasList()
    {
        foreach (var atlashandle in spriteaAtlasList.Values)
        {
            if (atlashandle.IsValid())
            {
                Addressables.Release(atlashandle);
            }
        }
        spriteaAtlasList.Clear();
    }

    //批量加载图集
    public void LoadAtlasMultiAnync(List<string> keys, System.Action callback)
    {

    }

    //释放图集
    public void ReleaseAtlas(string name)
    {
        if (spriteaAtlasList.ContainsKey(name))
        {
            if (spriteaAtlasList[name].IsValid())
            {
                Addressables.Release(spriteaAtlasList[name]);
            }
        }
        else
        {
            if (atlasLoaderList.ContainsKey(name))
            {
                atlasLoaderList[name].Release();
                atlasLoaderList.Remove(name);
            }
        }
    }

    //异步load图集（预加载用）
    public void LoadAtlasAsync(string name, System.Action<string, AsyncOperationHandle> callback)
    {
        if (spriteaAtlasList.ContainsKey(name))
        {
            if (spriteaAtlasList[name].IsValid() && spriteaAtlasList[name].Status == AsyncOperationStatus.Succeeded)
            {
                if (spriteaAtlasList[name].Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(name, spriteaAtlasList[name]);
                    return;
                }
            }
        }
        if (atlasLoaderList.ContainsKey(name))
        {
            atlasLoaderList[name].AddListenerCallBack(callback);
        }
        else
        {
            AssetLoader loader = new AssetLoader();
            atlasLoaderList.Add(name, loader);
            loader.LoadAtlas(name, AtlasLoad);
            loader.AddListenerCallBack(callback);
        }
    }

    //调用获取Gsprite  使用后记得自己释放
    public void GetAtlasSprite(string atlasName, string spriteName, System.Action<GSprite> callback)
    {
        if (spriteaAtlasList.ContainsKey(atlasName))
        {
            if (spriteaAtlasList[atlasName].IsValid())
            {
                if (spriteaAtlasList[atlasName].Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(new GSprite(atlasName, spriteName, spriteaAtlasList[atlasName]));
                    return;
                }
            }
        }
        //加载图集
        Logger.log("需要加载图集：" + atlasName);
        LoadAtlasAsync(atlasName, (atlas, opHandle) =>
        {
            string sn = spriteName;
            callback?.Invoke(new GSprite(atlasName, sn, opHandle));
        });
    }
}
#endregion
public class AssetCache : IAssetCache, IAssetInstanceHandler, IAssetManualHandler
{
    Dictionary<string, SpriteAtlas> mSpriteAtlasCaches;
    Dictionary<string, GameObject> mUIInsCaches;

    //Dictionary<string, UIDependencyData> mUIDependencyDict;
    MonoBehaviour mMono;
    private AssetCache mThis;
    public AssetCache(MonoBehaviour mono)
    {
        mThis = this;
        mMono = mono;
        mSpriteAtlasCaches = new Dictionary<string, SpriteAtlas>();
        //mInstanceCaches = new Dictionary<string, GameObject>();
        mUIInsCaches = new Dictionary<string, GameObject>();
        SpriteAtlasManager.atlasRegistered += SpriteAtlasManager_atlasRegistered;
        SpriteAtlasManager.atlasRequested += SpriteAtlasManager_atlasRequested;
    }

    void SpriteAtlasManager_atlasRequested(string arg1, System.Action<SpriteAtlas> arg2)
    {
        Logger.info("[AssetCache] atlas requested : " + arg1);
    }

    void SpriteAtlasManager_atlasRegistered(SpriteAtlas obj)
    {
        Logger.info("[AssetCache] atlas registered : " + obj.name);
    }

    /// <summary>
    ///返回UI prefab的实例
    /// </summary>
    /// <param name="assetKey"></param>
    /// <param name="callback"></param>
    public void InstantiateUIAsync(string assetKey, System.Action<GameObject> callback)
    {
        //load spriteatlas
        //load 
        if (string.IsNullOrEmpty(assetKey))
        {
            Logger.error("InstantiateUIAsync failed assetKey is null");
            callback?.Invoke(null);
            return;
        }
        InstantiatePrefabAsync(assetKey, callback);
    }


    /// <summary>
    /// 获取缓存的Sprite，没有则返回null
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public Sprite getSprite(string atlasName, string spriteName)
    {
        SpriteAtlas sa = null;
        // atlasName = (atlasName.IndexOf(".spriteatlas") < 0) ? (atlasName + ".spriteatlas") : atlasName;
        if (mSpriteAtlasCaches.TryGetValue(atlasName, out sa))
        {
            return sa.GetSprite(spriteName);
        }
        return null;
    }
    /// <summary>
    /// 异步获取Sprite,如果Atlas未加载，则加载后返回。 只能异步加载。
    /// </summary>
    /// <param name="spriteName"></param>
    /// <param name="callback"></param>
    public void getSpriteAsync(string spriteName, System.Action<GSprite> callback)
    {
        Addressables.LoadAssetAsync<Sprite>(spriteName).Completed += (handle) =>
        {
            string str = spriteName;
            callback?.Invoke(new GSprite(string.Empty, str, handle));
        };
    }
    /// <summary>
    /// 直接返回加载的资源，会自动附加AssetManualAttacher，请不要去掉
    /// </summary>
    /// <param name="assetKey"></param>
    /// <param name="callback"></param>

    Dictionary<string, System.Action<GameObject>> loadPrefabCallbacks = new Dictionary<string, Action<GameObject>>();
    public void loadPrefabAsync(string assetKey, Transform parent, System.Action<GameObject> callback)
    {
        Addressables.InstantiateAsync(assetKey, parent).Completed += (aoc) =>
        {
            if (aoc.IsValid() && aoc.Status == AsyncOperationStatus.Succeeded)
            {
                var newObj = aoc.Result as GameObject;
                try
                {
                    AssetManualAttacher att = newObj.GetComponent<AssetManualAttacher>() ?? newObj.AddComponent<AssetManualAttacher>();
                }
                catch
                {

                }
                // if (att != null)
                //  att.attach(assetKey, this);
                callback?.Invoke(newObj);
            }
        };
    }
    /// <summary>
    /// 实例化资源，返回的GameObject会自动附加AssetInstanceAttacher，请不要去掉
    /// </summary>
    /// <param name="assetKey"></param>
    /// <param name="callback"></param>
    public void InstantiatePrefabAsync(string assetKey, System.Action<GameObject> callback)
    {
        AsyncInstantiate(assetKey, (go) =>
         {
             if (go != null)
             {
                 AssetInstanceAttacher att = go.GetComponent<AssetInstanceAttacher>();
                 if (att == null)
                 {
                     att = go.AddComponent<AssetInstanceAttacher>();
                 }
                 att.mHandler = mThis;
                 att.mAssetKey = assetKey;
                 callback?.Invoke(go);
             }
             else
             {
                 callback?.Invoke(null);
             }
         });
    }
    /// <summary>
    /// 加载非prefab类资源时使用，强行加载prefab会抛出异常
    /// </summary>
    /// <param name="assetKey"></param>
    /// <param name="callback"></param>
    Dictionary<string, List<AsyncOperationHandle>> miscAssetHandleList = new Dictionary<string, List<AsyncOperationHandle>>();
    void addHandleToList(string key, AsyncOperationHandle handle)
    {
        if (miscAssetHandleList.ContainsKey(key))
        {
            miscAssetHandleList[key].Add(handle);
        }
        else
        {
            miscAssetHandleList.Add(key, new List<AsyncOperationHandle>());
            miscAssetHandleList[key].Add(handle);
        }
    }

    public AsyncOperationHandle LoadMiscAssetsAsync(List<string> urls)
    {
        // IEnumerable<string> query = urls;
        AsyncOperationHandle handle = Addressables.LoadAssetsAsync<Sprite>((IEnumerable)urls, (sp) => { }, MergeMode.None);
        return handle;
    }
    public void loadMiscAsset<T>(string assetKey, System.Action<T> callback)
    {
        Addressables.LoadAssetAsync<T>(assetKey).Completed += (handle) =>
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                callback.Invoke(handle.Result);

            }
            else
            {
                Addressables.Release(handle);
            }
        };
    }

    /// <summary>
    /// 卸载非prefab类资源时使用
    /// </summary>
    /// <param name="assetKey"></param>
    /// <param name="assetObj"></param>
    public void unloadMiscAsset<T>(string assetKey, T assetObj)
    {
        AsyncUnload<T>(assetKey, assetObj);
    }

    public void handleInstanceDestroy(string assetkey, GameObject go)
    {
        Addressables.ReleaseInstance(go);
    }


    public void clear()
    {
        mUIInsCaches.Clear();
        mSpriteAtlasCaches.Clear();


        foreach (List<AsyncOperationHandle> values in miscAssetHandleList.Values)
        {
            foreach (AsyncOperationHandle handle in values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }
        miscAssetHandleList.Clear();

        AtlasAssetHandler.inst.ReleaseAtlasList();
    }

    static IEnumerator AsyncLoad<T>(string assetKey, System.Action<T> callback)
    {
        var aop = Addressables.LoadAssetAsync<T>(assetKey);
        yield return aop;
        // yield return new WaitForSeconds(0.02f);
        if (aop.Status == AsyncOperationStatus.Succeeded)
        {
            callback(aop.Result);
        }
        else
        {
            Logger.log("[AssetCache] asyncLoad failed : " + assetKey);
            if (aop.OperationException != null)
            {
                Logger.logException(aop.OperationException);
            }
        }
    }

    static void AsyncUnload<T>(string assetKey, T assetObj)
    {
        Logger.log("[AssetCache] asyncUnload assetKey:" + assetKey);
        Addressables.Release<T>(assetObj);
    }

    static IEnumerator AsyncLoadMulti<T>(IList<System.Object> keys, System.Action<IList<T>> callback)
    {
        Logger.log("AsyncLoadMulti key:" + keys);
        int count = keys.Count;
        List<T> loadedList = new List<T>(count);
        IEnumerable<System.Object> query = keys;
        var aop = Addressables.LoadAssetsAsync<T>(query, (obj) =>
        {
#if UNITY_EDITOR
            Logger.log("loaded obj:" + obj);
#endif
            loadedList.Add(obj);
            if (loadedList.Count == count)
            {
                callback(loadedList);
            }
        }, Addressables.MergeMode.Union);
        yield return aop;
        if (aop.Status != AsyncOperationStatus.Succeeded)
        {

            Logger.log("[AssetCache] asyncLoadMulti failed : " + keys[0]);
            if (aop.OperationException != null)
            {
                Logger.logException(aop.OperationException);
            }
        }
    }

    static void AsyncInstantiate(string assetKey, System.Action<GameObject> callback)
    {
        Logger.log("AsyncInstantiate key:" + assetKey);
        var aop = Addressables.InstantiateAsync(assetKey);
        aop.Completed += (handle) =>
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                callback(handle.Result);
            }
            else
            {
                Logger.log("[AssetCache] asyncInstantiate failed : " + assetKey);
                if (handle.IsValid() && handle.OperationException != null)
                {
                    //Logger.logException(handle.OperationException);
                    Logger.error(handle.OperationException.ToString());
                }
                callback?.Invoke(null);
            }
        };
        // yield return aop;
    }

    void log(string msg)
    {
        Logger.log(msg);
    }

    static Dictionary<string, AssetReferenceAtlasedSprite> atlasedList = new Dictionary<string, AssetReferenceAtlasedSprite>();
    //直接获取 reference 自行判断是否有效
    public static AssetReferenceAtlasedSprite LoadAtlas(string key)
    {
        if (atlasedList.ContainsKey(key))
        {
            return atlasedList[key];
        }
        else
        {
            var reference = new AssetReferenceAtlasedSprite(key);
            reference.LoadAssetAsync();
            atlasedList.Add(key, reference);
            return reference;
        }
    }

    public void handleAllDestroyed(string key, bool removeKey = true)
    {
        throw new NotImplementedException();
    }
}

public class UIDependencyData
{
    public string uiName;
    public IList<System.Object> atlasList;

}

//NEW
/*
加载资源的类型  
*/
public enum LoadResourceType
{
    gameGUI = 0,        //游戏界面
    prefab = 1,         //游戏对象 
    atlas = 2,          //图集
    sprite = 3,         //sprite图片
    videoClip = 4,      //视频
    spine = 5,          //spine文件
    csv = 6,            //
}
