using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.U2D;
using System.IO;
using System.Collections.Generic;

public class UIDependenciesExporter
{
    [MenuItem("Build/Export UI Dependencies")]
    public static void ExportUIDependencies()
    {
        //EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");

        //1. load ui
        //2. list image button
        //3. find sprite path
        //4. load sprite atlas files
        //5. match sprite path with atlas path
        //6. export dependencies file
        //ui
        var uiDict = new Dictionary<string, UIDesc>();
        string uiFolder = "Assets/Prefabs/GUI/";
        string testPath = "Assets/Prefabs/GUI/testui.prefab";
        string[] deppp = AssetDatabase.GetDependencies(testPath);
        string[] files = Directory.GetFiles(uiFolder, "*.prefab", SearchOption.AllDirectories);
        //UnityEngine.Object[] uis = AssetDatabase.(uiFolder);
        for (int i = 0; i < files.Length; i++)
        {
            var f = files[i];
            var ui = AssetDatabase.LoadMainAssetAtPath(f) as GameObject;
            if (ui == null)
                continue;
            if (f.ToLower().IndexOf("shopkeeper") >= 0)
            {
                Logger.log("");
            }
            Image[] imgs = ui.GetComponentsInChildren<Image>();
            Button[] btns = ui.GetComponentsInChildren<Button>();
            var uiDesc = new UIDesc()
            {
                UIName = ui.name,
                spritePathDict = new Dictionary<string, int>()
            };
            foreach (var img in imgs)
            {
                var sp = img.sprite;
                if (sp != null)
                {
                    var path = AssetDatabase.GetAssetPath(sp);
                    if ("Resources/unity_builtin_extra".Equals(path)) continue;
                    var dir = path.Substring(0, path.LastIndexOf("/"));
                    uiDesc.spritePathDict[dir] = 1;
                }
            }
            foreach (var btn in btns)
            {
                var sp = btn.spriteState.selectedSprite;
                if (sp != null)
                {
                    var path = AssetDatabase.GetAssetPath(sp);
                    if ("Resources/unity_builtin_extra".Equals(path)) continue;
                    var dir = path.Substring(0, path.LastIndexOf("/"));
                    uiDesc.spritePathDict[dir] = 1;
                }
            }
            if (uiDesc.spritePathDict.Count > 0)
                uiDict[ui.name] = uiDesc;
        }
        //atlas 
        var atlasDict = new Dictionary<string, AtlasDesc>();
        string atlasFolder = "Assets/GUI2D/SpriteAtlas/";
        var pathToAtlasDict = new Dictionary<string, string>();
        var atlasFiles = Directory.GetFiles(atlasFolder, "*.spriteatlas", SearchOption.AllDirectories);
        //UnityEngine.Object[] atlases = AssetDatabase.LoadAllAssetsAtPath(atlasFolder);
        for (int i = 0; i < atlasFiles.Length; i++)
        {
            var ff = atlasFiles[i];
            var als = AssetDatabase.LoadMainAssetAtPath(ff) as SpriteAtlas;
            //var als = atlases[i] as SpriteAtlas;
            if (als == null) continue;
            UnityEngine.Object[] packables = als.GetPackables();
            var atlasDesc = new AtlasDesc()
            {
                AtlasName = als.name,
                atlasPathDict = new Dictionary<string, int>()
            };
            for (int j = 0; j < packables.Length; j++)
            {
                var obj = packables[j];
                Logger.log("asset:" + obj);
                string path = AssetDatabase.GetAssetPath(obj);
                Logger.log("assetpath: " + path);
                //atlasDesc.atlasPathDict[path] = 1;
                pathToAtlasDict[path] = als.name;
            }
        }
        //compare
        List<System.Object> depList = new List<object>();
        foreach (var ui in uiDict.Values)
        {
            var depItem = new Dictionary<string, System.Object>();
            depItem.Add("ui", ui.UIName);
            List<string> alist = new List<string>();
            depItem.Add("a", alist);
            foreach (var path in ui.spritePathDict.Keys)
            {
                if (pathToAtlasDict.ContainsKey(path))
                {
                    alist.Add(pathToAtlasDict[path]);
                }
            }
            if (alist.Count > 0)
                depList.Add(depItem);
        }

        //serialize
        string ss = MiniJSON.Json.Serialize(depList);
        //string savePath = Application.streamingAssetsPath + "/uidependency.json";
        string savePath = PathUtils.AASettingsPath + "uidependency.json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        FileUtils.saveTxtFileUTF8(savePath, ss);
    }

    [MenuItem("Build/Check Sprite Atlas")]
    static void checkSpriteAtlas()
    {
        SpriteAtlas sa = Selection.activeObject as SpriteAtlas;

        Debug.Log(sa);
        if (sa == null) return;

        SpriteAtlasPackingSettings settings = sa.GetPackingSettings();
        UnityEngine.Object[] packables = sa.GetPackables();
        for (int i = 0; i < packables?.Length; i++)
        {
            var obj = packables[i];
            Logger.log("asset:" + obj);
            string path = AssetDatabase.GetAssetPath(obj);
            Logger.log("assetpath: " + path);
        }
    }

    public class UIDesc
    {
        public string UIName;
        public Dictionary<string, int> spritePathDict;
        public List<string> atlasList;
    }

    public class AtlasDesc
    {
        public string AtlasName;
        public Dictionary<string, int> atlasPathDict;
    }
}