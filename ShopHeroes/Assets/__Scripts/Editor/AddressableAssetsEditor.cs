using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Pipeline.Utilities;

public static class AddressableAssetsEditor
{

    public static void SetAddressableID(this GameObject go, string id)
    {

    }
    public static void SetAddressableItems()
    {
        //UnityEditor.AddressableAssets.Settings.AddressableAssetSettings
        AssetImporter ai = AssetImporter.GetAtPath("");
        //AddressableAssetSettings;
        //Material.Create("");
    }

    public static void SetAddressableID(this UnityEngine.Object obj, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("id is null or empty");
            return;
        }
        var entry = GetAddressableAssetEntry(obj);
        if (entry != null)
        {
            entry.address = id;
        }
    }

    public static string GetAddressableID(this GameObject go)
    {
        return GetAddressableID(go as UnityEngine.Object);
    }

    public static string GetAddressableID(this UnityEngine.Object obj)
    {
        AddressableAssetEntry entry = GetAddressableAssetEntry(obj);
        return entry != null ? entry.address : string.Empty;
    }

    public static AddressableAssetEntry GetAddressableAssetEntry(UnityEngine.Object obj)
    {
        AddressableAssetSettings stt = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetEntry entry = null;
        string guid = string.Empty;
        long localID = 0;
        bool foundAsset = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out localID);
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (foundAsset && (path.ToLower().Contains("assets")))
        {
            if (stt != null)
            {
                entry = stt.FindAssetEntry(guid);
            }
        }
        return entry;
    }

    public static void RefreshAddressables()
    {
        var stt = AddressableAssetSettingsDefaultObject.Settings;
        var group = stt.DefaultGroup;
        var customGroup = stt.FindGroup("MyGroup");
        var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/ScriptableObjects/" });

        var entryList = new List<AddressableAssetEntry>();
        for (int i = 0; i < guids.Length; i++)
        {
            var entry = stt.CreateOrMoveEntry(guids[i], group, readOnly: false, postEvent: false);
            entry.address = AssetDatabase.GUIDToAssetPath(guids[i]);
            entry.labels.Add("myLabel");
            entryList.Add(entry);
        }
        stt.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entryList, true);
    }

    [MenuItem("Addressables/0. Clean Build 清除所有构建信息")]
    static void CleanBuild()
    {
        AddressableAssetSettings.CleanPlayerContent(null);
        BuildCache.PurgeCache(true);
        var projPath = Application.dataPath.Replace("/Assets", "");
        var remoteBuildPath = projPath + "/ServerData/" + getBuildTarget();
        if (Directory.Exists(remoteBuildPath))
        {
            Directory.Delete(remoteBuildPath, true);
        }
        Directory.CreateDirectory(remoteBuildPath);
    }

    [MenuItem("Addressables/1. Build Player Content 整个重新构建")]
    public static void BuildContent()
    {
        AddressableAssetSettings.BuildPlayerContent();
        PackRemoteAddressableAssets();
        LogMainBuildAssets();
    }

    [MenuItem("Addressables/2. Update Previous Build 只更新构建")]
    public static void UpdatePreviousBuild()
    {
        var defaultSett = AddressableAssetSettingsDefaultObject.Settings;
        if (defaultSett == null)
        {
            Debug.LogError("no default Addressable setting");
            return;
        }
        var contentBinPath = Application.dataPath + "/AddressableAssetsData/" + getBuildTarget() + "addressables_content_state.bin";
        var res = ContentUpdateScript.BuildContentUpdate(defaultSett, contentBinPath);
        if (!string.IsNullOrEmpty(res.Error))
        {
            Debug.LogError(res.Error);
        }
        else
        {
            PackRemoteAddressableAssets();
            LogUpdateBuildAssets();
        }
        Debug.Log("UpdatePreviousBuild output:" + res.OutputPath);
    }

    // [MenuItem("Addressables/3. Pack Remote Assets To StreamingAssets 打包构建到StreamingAssets")]
    static void PackRemoteAddressableAssets()
    {
        //var remoteBuildPath = AddressableAssetSettingsDefaultObject.Settings.buildSettings.bundleBuildPath;
        var projPath = Application.dataPath.Replace("/Assets", "");
        var remoteBuildPath = projPath + "/ServerData/" + getBuildTarget();
        Debug.Log("Addressables remoteBuildPath :  " + remoteBuildPath);
        string savePath = Application.streamingAssetsPath + "/" + "AddressablePack.zip";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        Debug.Log("savePath: " + savePath);
        ZipHelper.ZipFile(remoteBuildPath, savePath, "*.*");
        Debug.Log("PackRemoteAddressableAssets  complete");

        AssetDatabase.Refresh();
    }

    [MenuItem("Addressables/4. Log main Build Content 记录构建文件信息")]
    static void LogMainBuildAssets()
    {
        LogBuildAssetsImpl("main_build.json");
    }

    [MenuItem("Addressables/5. Log update Build Content 记录更新构建文件信息")]
    static void LogUpdateBuildAssets()
    {
        LogBuildAssetsImpl("update_build.json");
    }

    static void LogBuildAssetsImpl(string fileName)
    {
        var projPath = Application.dataPath.Replace("/Assets", "");
        var remoteBuildPath = projPath + "/ServerData/" + getBuildTarget();
        //
        var files = Directory.GetFiles(remoteBuildPath, "*.*", SearchOption.AllDirectories);
        var jsonStr = "[\n";
        for (int i = 0; i < files.Length; i++)
        {
            var f = files[i];
            f = f.Replace("\\", "/");
            f = f.Replace("\\\\", "/");
            f = f.Replace(remoteBuildPath, "");
            jsonStr += "{\n\t";
            if (f.IndexOf(".bundle") >= 0)
            {
                var ff = f.Replace(".bundle", "");
                var idx = ff.LastIndexOf("_");
                ff = ff.Substring(0, idx);
                jsonStr += "\"i\":" + "\"" + ff + "\",";
                jsonStr += "\"f\":" + "\"" + f + "\"";
            }
            else
            {
                jsonStr += "\"i\":" + "\"" + f + "\",";
                jsonStr += "\"f\":" + "\"" + f + "\"";
            }
            if (i == files.Length - 1)
                jsonStr += "\n}\n";
            else
                jsonStr += "\n},\n";
        }
        jsonStr += "]";

        string saveFolder = Application.dataPath + "/BuildManifest/" + getBuildTarget();
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
        string savePath = saveFolder + fileName;

        if (File.Exists(savePath))
        {
            var copyPath = savePath.Replace(".json", "_bak_" + TimeUtils.GetTime_yyyyMMdd_HHmm() + ".json");
            FileUtil.CopyFileOrDirectory(savePath, copyPath);
            File.Delete(savePath);
        }

        FileUtils.saveTxtFileUTF8(savePath, jsonStr);

        AssetDatabase.Refresh();
        Debug.Log("Build Assets logged : " + savePath);
    }

    [MenuItem("Addressables/6. Compare Update Build 对比构建信息")]
    static void compareUpdateBuild()
    {
        var mainDict = new Dictionary<string, string>();
        var updateDict = new Dictionary<string, string>();
        var mainPath = Application.dataPath + "/BuildManifest/" + getBuildTarget();
        var mainStr = FileUtils.loadTxtFile(mainPath + "main_build.json");
        var updateStr = FileUtils.loadTxtFile(mainPath + "update_build.json");
        mainDict = getBuildJsonDict(mainStr);
        updateDict = getBuildJsonDict(updateStr);
        var newList = new List<string>();
        foreach (var key in updateDict.Keys)
        {
            if (mainDict.ContainsKey(key))
            {
                var mainVal = mainDict[key];
                var updateVal = updateDict[key];
                if (updateVal != mainVal)
                {
                    newList.Add(updateVal);
                }
            }
            else
            {
                newList.Add(updateDict[key]);
            }
        }
        var nstr = MiniJSON.Json.Serialize(newList);
        FileUtils.saveTxtFileUTF8(mainPath + "build_diff.json", nstr);
    }

    static Dictionary<string, string> getBuildJsonDict(string str)
    {
        var dict = new Dictionary<string, string>();
        var mainObj = MiniJSON.Json.Deserialize(str);
        Debug.Log(mainObj.GetType().FullName);
        var objlist = mainObj as List<System.Object>;
        foreach (var item in objlist)
        {
            var sub = item as Dictionary<string, System.Object>;
            var key = sub["i"] as string;
            var val = sub["f"] as string;
            if (dict.ContainsKey(key))
            {
                Debug.LogError("key already added");
            }
            dict.Add(key, val);
        }
        return dict;
    }

    static string getBuildTarget()
    {
#if UNITY_ANDROID
        return "Android/";
#elif UNITY_IOS
         return "iOS/";
#else
        return "Windows/";
#endif
    }

    [MenuItem("Assets/Addressables/Move To Content Update Group")]
    static void MoveToAddressableGroup()
    {
        var objs = Selection.objects;

        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            //ContentUpdate
            moveEntryToGroup(obj, "ContentUpdate");
        }

        AssetDatabase.Refresh();
    }

    static void moveEntryToGroup(UnityEngine.Object obj, string groupName)
    {

        AddressableAssetSettings stt = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetEntry entry = GetAddressableAssetEntry(obj);
        if (entry == null)
        {
            Debug.Log("entry not found:" + obj);
            return;
        }
        var group = stt.FindGroup(groupName);
        if (group != null)
        {
            stt.MoveEntry(entry, group);
            Debug.Log("entry move obj:" + obj.name + " group:" + groupName);
        }
    }

    [MenuItem("Addressables/99. Move All ContentUpdate To Update Group 移动所有更改内容从Local Group 到 Update Group")]
    static void MoveContentUpdateToUpdateGroup()
    {
        var stt = AddressableAssetSettingsDefaultObject.Settings;
        var path = ContentUpdateScript.GetContentStateDataPath(true);
        var modifiedEntries = ContentUpdateScript.GatherModifiedEntriesWithDependencies(stt, path);
        foreach (var key in modifiedEntries.Keys)
        {
            Debug.Log("Updated Content:" + key.address);
            var vals = modifiedEntries[key];
            MoveLocalEntryToUpdateGroup(key, stt);
            for (int i = 0; i < vals.Count; i++)
            {
                var depEntry = vals[i];
                Debug.Log("Update Content dep:" + depEntry.address);
                //MoveLocalEntryToUpdateGroup(depEntry, stt);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("MoveContentUpdateToUpdateGroup Completed");
    }

    static void MoveLocalEntryToUpdateGroup(AddressableAssetEntry entry, AddressableAssetSettings setting)
    {
        var fromGroup = entry.parentGroup;
        var toGroupName = fromGroup.Name.Replace("Local", "Update");
        var toGroup = setting.FindGroup(toGroupName);
        if (toGroup != null)
        {
            setting.MoveEntry(entry, toGroup);
        }
        else
        {
            Debug.LogError("MoveLocalEntryToUpdateGroup toGroup == null entry:" + entry.address + " toGroup:" + toGroupName);
        }
    }

    [MenuItem("Addressables/100. Move All Update Group To Local Group 移动所有Update Group内容到Local Group")]
    static void MoveUpdateEntryBackToLocalGroup()
    {
        var stt = AddressableAssetSettingsDefaultObject.Settings;
        var groups = stt.groups;
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            if (group.Name.IndexOf("Update") >= 0)
            {
                var entries = group.entries;
                AddressableAssetGroup toGroup = null;
                foreach (var entry in entries)
                {
                    var fromGroup = entry.parentGroup;
                    var toGroupName = fromGroup.Name.Replace("Update", "Local");
                    toGroup = stt.FindGroup(toGroupName);
                    break;
                }
                if (toGroup != null)
                {
                    List<AddressableAssetEntry> moveEntries = new List<AddressableAssetEntry>(entries);
                    stt.MoveEntries(moveEntries, toGroup);
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("MoveUpdateEntryBackToLocalGroup");
    }
}

class CustomAssetsPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var stt = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("");
        /*var group = stt.FindGroup("LuaScript");
        foreach(var str in importedAssets) {
            if(str.EndsWith(".lua")) {
                var guid = AssetDatabase.AssetPathToGUID(str);
                var entry = stt.CreateOrMoveEntry(guid, group, true, false);
                entry.SetLabel("LuaScript", true);
            }
        }*/
    }

    //static void OnPostprocessMaterial (Material mat) { }
    //static void OnPostprocessTexture (Texture2D tex) { }
}