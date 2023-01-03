using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;

public class AddressableConfig
{
    public static string addressableRuntimePath = "";
    public const string LuaExt = ".lua.txt";
    public static string LuaFileListKey = "Assets/GameAssets/settings/lua_filelist.json";
    public const string HotfixMark = "hotfix/";
    public const string LuaPath = "Assets/GameAssets/lua/";
}

public class VersionManager : TSingletonHotfix<VersionManager>
{
    Dictionary<string, TextAsset> mLuaFilesDict = new Dictionary<string, TextAsset>();
    Dictionary<string, bool> mHotfixMap = new Dictionary<string, bool>();
    List<string> mRequiredFiles = new List<string>();
    MonoBehaviour mMono;
    System.Action mUpdateCallback;
    List<System.Object> mLuaFileList;
    protected override void init()
    {

    }
    public void Clear()
    {
        mLuaFilesDict.Clear();
        mHotfixMap.Clear();
        mRequiredFiles.Clear();
    }
    public void CheckUpdate(MonoBehaviour mono, System.Action callback)
    {
        mMono = mono;
        mUpdateCallback = callback;
        //mMono.StartCoroutine(checkUpdateCatalogs());
        loadLuaFileList();
    }

    public byte[] GetLuaFile(string key)
    {
        mLuaFilesDict.TryGetValue(key, out TextAsset ta);
        if (ta != null)
        {
            return ta.bytes;
        }
        return null;
    }

    public string GetLuaText(string key)
    {
        mLuaFilesDict.TryGetValue(key, out TextAsset ta);
        if (ta != null)
        {
            return ta.text;
        }
        return null;
    }

    public void ReleaseLuaFile(string key)
    {
        mLuaFilesDict.TryGetValue(key, out TextAsset ta);
        if (ta != null)
        {
            mLuaFilesDict.Remove(key);
            Addressables.Release(ta);
        }
    }

    public bool CheckNeedHotfix(string key)
    {
        string kk = "hotfix/" + key + "Hotfix";
        log("CheckNeedHotfix :" + kk);
        mHotfixMap.TryGetValue(kk, out bool isNeeded);
        return isNeeded;
    }

    void onCatalogUpdated(List<string> catalogItems)
    {
        loadLuaFileList();
    }

    void loadLuaFileList()
    {
        mMono.StartCoroutine(loadTextAsset(AddressableConfig.LuaFileListKey, onLuaFileListLoaded));
    }

    void onLuaFileListLoaded(TextAsset ta)
    {
        log("onLuaFileListLoaded");
        getResourceLocation();
        var list = MiniJSON.Json.Deserialize(ta.text) as List<System.Object>;
        mLuaFileList = list;
        //mMono.StartCoroutine(loadTextAssetList(list, onLuaFileLoaded, onAllLuaLoaded));
        mMono.StartCoroutine(loadTextAssetsAll(list, onLuaFileLoaded2, onAllLuaLoaded));
    }

    void onLuaFileLoaded(string key, TextAsset ta)
    {
        var simplifiedKey = key.Replace(AddressableConfig.LuaPath, "");
        simplifiedKey = simplifiedKey.Replace(AddressableConfig.LuaExt, "");
#if UNITY_EDITOR
        log("lua loaded key:" + key + " simplified:" + simplifiedKey);
#endif
        if (!mLuaFilesDict.ContainsKey(simplifiedKey))
        {
            mLuaFilesDict.Add(simplifiedKey, ta);
        }
        else
        {
            Debug.LogError("onLuaFileLoaded duplicated key:" + key);
        }

        if (key.IndexOf(AddressableConfig.HotfixMark) >= 0)
        {

#if UNITY_EDITOR
            log("lua hotfix added:" + simplifiedKey);
#endif

            mHotfixMap.Add(simplifiedKey, true);
        }
    }

    void onAllLuaLoaded()
    {
        log("all lua loaded");

        //mMono.StartCoroutine(loadTextAsset("Assets/GameAssets/configs.zip.bytes", (ta)=> {
        //    var bts = ta.bytes;
        //    ConfigAssetHandler.ParseData(bts);
        //}));

        mUpdateCallback?.Invoke();
    }

    IEnumerator loadTextAsset(string key, System.Action<TextAsset> callback)
    {
        yield return null;
        var aop = Addressables.LoadAssetAsync<TextAsset>(key);
        yield return aop;
        if (aop.Status == AsyncOperationStatus.Succeeded)
        {
            log("loadTextAsset success:" + key);
            callback?.Invoke(aop.Result);

        }
        else
        {
            log("");
        }
    }


    IEnumerator loadTextAssetList(List<System.Object> list, System.Action<string, TextAsset> callback, System.Action allLoadCallback)
    {
        yield return null;
        log("start load assetList count:" + list.Count);
        int total = list.Count;
        int index = 0;
        while (index < total)
        {
            var key = list[index] as string;

            var aop = Addressables.LoadAssetAsync<TextAsset>(key);
            yield return aop;
            if (aop.Status == AsyncOperationStatus.Succeeded)
            {
                //
                callback?.Invoke(key, aop.Result);
                //mLuaFilesDict.Add(key, aop.Result);
                index++;
            }
        }
        allLoadCallback?.Invoke();
    }

    IEnumerator loadTextAssetsAll(List<System.Object> list, System.Action<TextAsset> itemCallback, System.Action allLoadCallback)
    {

        log("LoadTextAssetsAll Start");

        var handle = Addressables.LoadAssetsAsync<TextAsset>("lua", (tobject) =>
        {
            //  Logger.log($"加载lua文件{tobject.name}完成后");
            itemCallback?.Invoke(tobject);
        });

        handle.Completed += (objs) =>
        {
            allLoadCallback?.Invoke();
        };
        yield return null;
        // var aop = Addressables.LoadAssetsAsync<TextAsset>(list, itemCallback, Addressables.MergeMode.Union);
        // yield return aop;
        // if (aop.Status == AsyncOperationStatus.Succeeded)
        // {
        //     //var assetList = aop.Result;
        //     log("LoadTextAssetsAll Success");
        // }
        // else
        // {
        //     Debug.LogError("LoadTextAssetsAll Failed");
        // }
        //yield return new WaitForSeconds(1);

    }

    void onLuaFileLoaded2(TextAsset ta)
    {
        var fname = ta.name;
        for (int i = 0; i < mLuaFileList.Count; i++)
        {
            var f = mLuaFileList[i] as string;
            if (f.IndexOf("/" + fname) >= 0)
            {
                onLuaFileLoaded(f, ta);
            }
        }
    }

    void getResourceLocation()
    {
        var locs = Addressables.ResourceLocators;
        string str = "locations:\n";
        foreach (var loc in locs)
        {
            str += loc.LocatorId + "\n";
            //(loc as ResourceLocationMap).Locations
        }
        Logger.log(str);
    }

    void log(string msg)
    {
        Logger.log("[VersionManager] " + msg);
    }
}

public class HttpsCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        //Simply return true no matter what
        return true;
    }
}