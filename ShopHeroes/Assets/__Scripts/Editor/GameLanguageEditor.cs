using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class GameLanguageEditor : Editor
{
    static string guiPrefabPath = "Assets/Prefabs/GUI";

    [MenuItem("GameGUIPrefabTool/检查所有界面的按钮添加点击冷却时间")]
    public static void AddUIButtonCoolTime()
    {
        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { guiPrefabPath });
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar($"当前界面>{obj.name}", "检测中......", (float)i / (float)(allPath.Length - 1));
                var buttons = obj.GetComponentsInChildren<Button>(true);
                bool change = false;
                foreach (var btn in buttons)
                {
                    if (btn.transition != Selectable.Transition.Animation)
                    {
                        btn.transition = Selectable.Transition.Animation;
                        //btn.transition = Selectable.Transition.None;
                        change = true;
                    }
                    GameObject _go = btn.gameObject;
                    //GameObject.DestroyImmediate(btn, true);
                    ButtonEx ex = _go.GetComponent<ButtonEx>();
                    if (ex != null)
                    {
                        // // ex.clickDistance = 0.6f;
                        // // change = true;
                        // Animator animator = _go.GetComponent<Animator>();
                        // if (animator == null)
                        // {
                        //     animator = _go.AddComponent<Animator>();
                        //     animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/GUI2D/Animation/buttonAnim.controller");
                        //     change = true;
                        // }
                        // else
                        // {
                        //     if (_go.transform.localScale.x < 0 && _go.GetComponent<OverrideAnimatorButton>() == null)
                        //     {
                        //         Logger.error(obj.name + "      " + btn.name);
                        //         btn.transition = Selectable.Transition.None;
                        //         DestroyImmediate(animator, true);
                        //     }
                        // }
                        continue;
                    }
                    change = true;
                    ex = _go.AddComponent<ButtonEx>();
                    Logger.log(_go.name + "冷却时间为：" + ex.clickDistance);
                }
                if (change)
                {
                    var newPrefab = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                    PrefabUtility.ApplyPrefabInstance(newPrefab, InteractionMode.AutomatedAction);
                    GameObject.DestroyImmediate(newPrefab);
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
    [MenuItem("本地化/所有界面的文本替换默认字体字体")]
    public static void CheckAllUIPrefab()
    {
        Font defFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextFont/AlibabaPuHuiTi-Bold.ttf") as Font;
        if (defFont == null)
        {
            Debug.LogError("找不到默认字体");
            return;
        }
        // LanguageConfigManager.inst.InitCSVConfig();
        EditorUtility.DisplayProgressBar("执行中", "查找所有UIPrefab", 0);
        // StringBuilder definedstr = new StringBuilder();
        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { guiPrefabPath });
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar($"当前界面>{obj.name}", "检查并提取text文字......", (float)i / (float)(allPath.Length - 1));
                //  Logger.log($"当前界面:{obj.name}");
                var texts = obj.GetComponentsInChildren<Text>(true);
                Regex regex = new Regex("[\u4e00-\u9fa5]");
                foreach (var text in texts)
                {
                    if (text.font == null)
                    {
                        text.font = defFont;
                    }
                    text.font = defFont;
                    // if (!string.IsNullOrEmpty(text.text))
                    // {
                    //     //  Logger.log($"提取文本内容：:{text.text}");
                    //     if (regex.IsMatch(text.text))
                    //     {
                    //         definedstr.AppendLine(text.text);
                    //     }
                    // }
                    //}
                }
            }
            var newPrefab = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            PrefabUtility.ApplyPrefabInstance(newPrefab, InteractionMode.AutomatedAction);
            GameObject.DestroyImmediate(newPrefab);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // if (definedstr.Length > 0)
        // {
        //     var savefilepath = EditorUtility.SaveFilePanel("Save File", "", "GameLanguageKeys.txt", "txt");
        //     FileStream stream = new FileStream(savefilepath, FileMode.Create);
        //     byte[] data = Encoding.UTF8.GetBytes(definedstr.ToString());
        //     stream.Write(data, 0, data.Length);
        //     stream.Flush();
        //     stream.Close();
        //     definedstr.Clear();
        // }
        // else
        // {
        //     Logger.log("未找到任何UI文本！");
        // }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("本地化/设置所有文本行间距为——0.75")]
    public static void SetTextLineSpacing()
    {
        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Prefabs/GUI" });
        string[] allPathL = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Prefabs/GUI_L" });
        var guilist = allPath.Concat(allPathL).ToArray();
        for (int i = 0; i < guilist.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guilist[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar($"竖版界面>{obj.name}", "设置行间距......", (float)i / (float)(guilist.Length - 1));
                //  Logger.log($"当前界面:{obj.name}");
                var texts = obj.GetComponentsInChildren<Text>(true);
                Regex regex = new Regex("[\u4e00-\u9fa5]");
                foreach (var text in texts)
                {
                    if (text.lineSpacing == 1)
                    {
                        text.lineSpacing = 0.75f;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            var newPrefab = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            PrefabUtility.ApplyPrefabInstance(newPrefab, InteractionMode.AutomatedAction);
            GameObject.DestroyImmediate(newPrefab);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }
//随即调整界面的颜色值

private static bool Char_Match(string input, string source)
{
    bool result = false;
    Regex r = new Regex(input);
    Match m = r.Match(source);
    if (m.Success)
        result = true;
    return result;
}

    [MenuItem("Tools/调整界面的颜色值(随机调整到 Color（0.98-1，0.98-1， 0.98-1）)")]
    public static void SetGUIImageColor()
    {
        string matchstr = "EquipMakeUI.prefab EquipMakeUIL.prefab ShopperUI.prefab ShopperUIL.prefab";

        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Prefabs/GUI" });
        string[] allPathL = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Prefabs/GUI_L" });
        var guilist = allPath.Concat(allPathL).ToArray();
        for (int i = 0; i < guilist.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guilist[i]);
            EditorUtility.DisplayProgressBar($"界面>{path}", "处理中......", (float)i / (float)(guilist.Length - 1));
            if (Char_Match(Path.GetFileName(path), matchstr))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (obj != null)
                {
                    {
                        var images = obj.GetComponentsInChildren<Image>(true);
                        foreach (var image in images)
                        {
                             if(image.color.r > 0.97f && image.color.g > 0.97f && image.color.b > 0.97f)
                                image.color = new Color(1f - Random.Range(0f, 0.02f), 1f - Random.Range(0f, 0.02f), 1f - Random.Range(0f, 0.02f), image.color.a);
                        }
                        
                        var newPrefab = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                        newPrefab.transform.localScale = Vector3.one;
                        PrefabUtility.ApplyPrefabInstance(newPrefab, InteractionMode.AutomatedAction);
                        GameObject.DestroyImmediate(newPrefab);
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }


    // [MenuItem("本地化/检查所有代码提取中文文本")]
    public static void CheckAllScripts()
    {
        string configFolder = "Assets/__Scripts/";
        string[] assetPaths = Directory.GetFiles(configFolder, "*.cs", SearchOption.AllDirectories);
        EditorUtility.DisplayProgressBar("执行中", "查找脚本中的中文", 0);
        StringBuilder definedstr = new StringBuilder();
        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            EditorUtility.DisplayProgressBar($"当前界面>{asset.name}", "检查并提取text文字......", (float)i / (float)(assetPaths.Length - 1));
            var lines = asset.text.Split(new char[2] { '\r', '\n' });
            foreach (var line in lines)
            {
                if (!Regex.IsMatch(line, @"Debug.") && !Regex.IsMatch(line, @"Logger.") && !line.StartsWith("//"))
                {
                    foreach (Match match in Regex.Matches(line, "\"([^\"]*)\""))
                    {
                        if (Regex.IsMatch(match.ToString(), @"[\u4e00-\u9fa5]"))
                        {
                            string str = match.ToString();//.Substring(1, match.Length - 2);
                            definedstr.AppendLine(str);
                        }
                    }
                }
            }
        }

        if (definedstr.Length > 0)
        {
            var savefilepath = EditorUtility.SaveFilePanel("Save File", "", "ScriptsLanguageKeys.txt", "txt");
            FileStream stream = new FileStream(savefilepath, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(definedstr.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
            definedstr.Clear();
        }
        else
        {
            Logger.log("未找到任何UI文本！");
        }
        EditorUtility.ClearProgressBar();
    }


    public class lkey
    {
        public string languages_game;
    }

    [MenuItem("本地化/提取游戏中所有中文（基于translate表增量）")]
    public static void findAllCHStr()
    {
        //读取translate表 包括增量表
        string config_name = "translate";
        List<lkey> languagecfgs = CSVParser.GetConfigsFromCache<lkey>(config_name, CSVParser.STRING_SPLIT);
        int incrementIndex = 1;
        while (CsvCfgCatalogMgr.inst.IsContainsCsvByName(config_name + "_" + incrementIndex.ToString("D2")))
        {
            languagecfgs.AddRange(CSVParser.GetConfigsFromCache<lkey>(config_name + "_" + incrementIndex.ToString("D2"), CSVParser.STRING_SPLIT));
            incrementIndex++;
        }

        StringBuilder definedstr = new StringBuilder();
        //lua代码
        string luaFolder = "Assets/GameAssets/lua";
        string[] lua_assetPaths = Directory.GetFiles(luaFolder, "*.lua.txt", SearchOption.AllDirectories);
        EditorUtility.DisplayProgressBar("执行中", "查找lua脚本中的中文", 0);

        for (int i = 0; i < lua_assetPaths.Length; i++)
        {
            string assetPath = lua_assetPaths[i];
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            var lines = asset.text.Split(new char[2] { '\r', '\n' });
            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, @"GetValueByKey"))
                {
                    foreach (Match match in Regex.Matches(line, "\"([^\"]*)\""))
                    {
                        if (Regex.IsMatch(match.ToString(), @"[\u4e00-\u9fa5]"))
                        {
                            string str = match.ToString();//.Substring(1, match.Length - 2);
                            str = str.Replace("\"", "");
                            if (!string.IsNullOrEmpty(str))
                            {
                                if (languagecfgs.FindIndex(K => K.languages_game == str) < 0)
                                {
                                    definedstr.AppendLine(str);
                                    languagecfgs.Add(new lkey() { languages_game = str });
                                }
                            }
                        }
                    }
                }
            }
        }
        //代码
        string sprFolder = "Assets/__Scripts/";
        string[] assetPaths = Directory.GetFiles(sprFolder, "*.cs", SearchOption.AllDirectories);
        EditorUtility.DisplayProgressBar("执行中", "查找脚本中的中文", 0);

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];
            if (Regex.IsMatch(assetPath, @"/Editor")) continue;
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            EditorUtility.DisplayProgressBar($"当前界面>{asset.name}", "检查并提取text文字......", (float)i / (float)(assetPaths.Length - 1));
            var lines = asset.text.Split(new char[2] { '\r', '\n' });
            foreach (var line in lines)
            {
                if (!Regex.IsMatch(line, @"Debug.") && !Regex.IsMatch(line, @"Logger.") && !Regex.IsMatch(line, @"Header") && !line.StartsWith("//"))
                {
                    foreach (Match match in Regex.Matches(line, "\"([^\"]*)\""))
                    {
                        if (Regex.IsMatch(match.ToString(), @"[\u4e00-\u9fa5]"))
                        {
                            string str = match.ToString();//.Substring(1, match.Length - 2);
                            str = str.Replace("\"", "");
                            if (!string.IsNullOrEmpty(str))
                            {
                                if (languagecfgs.FindIndex(K => K.languages_game == str) < 0)
                                {
                                    definedstr.AppendLine(str);
                                    languagecfgs.Add(new lkey() { languages_game = str });
                                }
                            }
                        }
                    }
                }
            }
        }

        //UI界面
        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { guiPrefabPath });
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar($"当前界面>{obj.name}", "检查并提取text文字......", (float)i / (float)(allPath.Length - 1));
                //  Logger.log($"当前界面:{obj.name}");
                var texts = obj.GetComponentsInChildren<Text>(true);
                Regex regex = new Regex("[\u4e00-\u9fa5]");
                foreach (var text in texts)
                {
                    if (regex.IsMatch(text.text))
                    {
                        string str = text.text;
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (languagecfgs.FindIndex(K => K.languages_game == str) < 0)
                            {
                                definedstr.AppendLine(str);
                                languagecfgs.Add(new lkey() { languages_game = str });
                            }
                        }
                    }
                }
            }
        }

        //配置表
        string configFolder = "Assets/Configs/";
        string[] cfgAssetPaths = Directory.GetFiles(configFolder, "*.csv", SearchOption.AllDirectories);
        EditorUtility.DisplayProgressBar("执行中", "查找配置中的中文", 0);
        for (int i = 0; i < cfgAssetPaths.Length; i++)
        {
            string cpath = cfgAssetPaths[i];
            EditorUtility.DisplayProgressBar($"当前配置>{cpath}", "检查并提取text文字......", (float)i / (float)(cfgAssetPaths.Length - 1));

            if (Regex.IsMatch(cpath, @"translate")) continue;
            string csv = FileUtils.loadTxtFile(cpath);

            string[] lines = null;

            if (csv.IndexOf(CSVParser.WIN_LINEFEED) >= 0)
            {
                lines = csv.Split(CSVParser.RNSTR_SPLIT, System.StringSplitOptions.None);
            }
            else
            {
                lines = csv.Split(CSVParser.NLINE_SPLIT, System.StringSplitOptions.None);
            }


            for (int l = 3; l < lines.Length; l++)
            {
                string[] values = lines[l].Split(',');
                foreach (var str in values)
                {
                    if (Regex.IsMatch(str, @"[\u4e00-\u9fa5]"))
                    {
                        if (languagecfgs.FindIndex(K => K.languages_game == str) < 0)
                        {
                            definedstr.AppendLine(str);
                            languagecfgs.Add(new lkey() { languages_game = str });
                        }
                    }
                }
            }
        }

        languagecfgs.Clear();
        if (definedstr.Length > 0)
        {
            var savefilepath = EditorUtility.SaveFilePanel("Save File", "", "GameLanguageKeys" + TimeUtils.GetTime_yyyyMMdd_HHmm() + ".txt", "txt");
            FileStream stream = new FileStream(savefilepath, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(definedstr.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
            definedstr.Clear();
        }
        else
        {
            Logger.log("未找到任何UI文本！");
        }
        //AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("本地化/删除界面多语言脚本")]
    public static void DelAllUILanguage()
    {
        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { guiPrefabPath });
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar($"当前界面>{obj.name}", "删除中......", (float)i / (float)(allPath.Length - 1));
                var texts = obj.GetComponentsInChildren<SetTextByLanguageType>(true);

                for (int k = texts.Length - 1; k >= 0; k--)
                {
                    DestroyImmediate(texts[k], true);
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }



    [MenuItem("本地化/给界面根节点加上多语言脚本")]
    public static void AddLanguageComForUIRoot()
    {
        string[] allPath = AssetDatabase.FindAssets("t:prefab", new string[] { guiPrefabPath });
        for (int i = 0; i < allPath.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (obj != null)
            {
                EditorUtility.DisplayProgressBar($"当前界面>{obj.name}", "添加中......", (float)i / (float)(allPath.Length - 1));
                var languageCom = obj.GetComponent<SetTextByLanguageType>();
                if (languageCom == null)
                {
                    obj.AddComponent<SetTextByLanguageType>();
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }


    [MenuItem("GameGUIPrefabTool/关闭选中对象除按钮外的所有Graphic射线检测")]
    public static void SetGraphicRaycastTargetFalse()
    {
        var objs = Selection.gameObjects;

        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];

            if (obj.transform.parent != null && obj.transform.parent.name != "Canvas (Environment)") //不是prefab的根节点
            {
                continue;
            }

            var graphics = obj.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                if (graphic.gameObject.transform.parent != null && graphic.gameObject.transform.parent.name == obj.name)
                {
                    graphic.raycastTarget = true;
                    continue;
                }

                if (graphic.GetComponent<Selectable>() != null) //Button InputField ScrollRect Toggle DropDown
                {
                    graphic.raycastTarget = true;
                    continue;
                }

                graphic.raycastTarget = false;
            }

            var toggles = obj.GetComponentsInChildren<Toggle>(true);
            foreach (var toggle in toggles)
            {
                if (toggle.graphic != null)
                {
                    toggle.graphic.raycastTarget = true; //toggle的再打开
                }
            }

            var scrollRects = obj.GetComponentsInChildren<ScrollRect>(true);
            foreach (var scrollRect in scrollRects)
            {
                if (scrollRect.GetComponent<Graphic>() == null && scrollRect.viewport != null && scrollRect.viewport.GetComponent<Graphic>() != null)
                {
                    scrollRect.viewport.GetComponent<Graphic>().raycastTarget = true; //如果父级scrollrect没有graphic就打开子级的graphic 允许拖动。
                }
            }

            var newPrefab = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            PrefabUtility.ApplyPrefabInstance(newPrefab, InteractionMode.AutomatedAction);
            GameObject.DestroyImmediate(newPrefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

}
