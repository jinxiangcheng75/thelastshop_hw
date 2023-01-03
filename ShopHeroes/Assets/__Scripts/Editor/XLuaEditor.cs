using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XLua;
using HashDict = System.Collections.Generic.Dictionary<string, System.Object>;
//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;

public class XLuaEditor
{
    [MenuItem("XLua/Custom/Check Hotfix folders")]
    static void checkHotfixFolders()
    {
        Logger.log(Application.dataPath);
        FileUtils.checkAndCreateDirectory(PathUtils.LUA_SCRIPTS);
        FileUtils.checkAndCreateDirectory(PathUtils.LUA_ENCRYPTED);
    }

    [MenuItem("XLua/Custom/Encrypt Lua")]
    static void EncryptLua()
    {
        string oriPath = PathUtils.LUA_SCRIPTS;//"Assets/_Lua/scripts/";
        string encPath = PathUtils.LUA_ENCRYPTED;// "Assets/_Lua/encrypted/";
        checkHotfixFolders();
        FileUtils.checkAndCreateDirectory(oriPath);
        FileUtils.checkAndCreateDirectory(encPath);
        byte[] key = System.Text.Encoding.UTF8.GetBytes(EncryptoUtils.des_key);
        FileUtils.EnumeratorDirectoryFiles(oriPath, (string filepath) =>
        {

            byte[] bts = null;
            FileUtils.loadFile(filepath, out bts);

            EncryptoUtils.XORBytes(bts, key);
            string savePath = filepath.Replace(oriPath, encPath);
            savePath = savePath.Replace(".txt.lua", ".bytes");
            savePath = savePath.Replace(".lua", ".bytes");

            FileUtils.saveFile(bts, savePath);
            Logger.log("EncryptLua saved:" + savePath);
        });
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("XLua", "EncryptLua Complete!", "OK");
    }

    [MenuItem("XLua/Custom/Test load encrypto Lua")]
    private static void testLoadEncryptoLuaFile()
    {
        UnityEngine.Object obj = Selection.activeObject;
        if (obj == null)
            return;
        string path = AssetDatabase.GetAssetPath(obj);
        Logger.log("test load lua path:" + path);
        byte[] bts = null;
        FileUtils.loadFile(path, out bts);

        byte[] key = System.Text.Encoding.UTF8.GetBytes(EncryptoUtils.des_key);
        LuaEnv env = new LuaEnv();
        try
        {

            XLuaManager.Instance().addLoaderForEditor(env, key);
            env.DoString("require('" + obj.name + "')", "");

        }
        catch (System.Exception e)
        {

            Debug.LogException(e);

        }
        finally
        {
            env.Dispose();
        }
    }

    // [MenuItem("Build/Generate Patch File")]
    static void GeneratePatchFile()
    {
        Logger.log("GeneratePatchFile start!");
        //patch lua
        //saveFolderFileMD5("lua_hash", "Assets/_Lua/encrypted", "*.bytes");
        //patch configs
        //patch ab
        string hotfixAssetPath = PathUtils.HOTFIX_ASSET;// "Assets/HotfixAssets/";
        FileUtils.checkAndCreateDirectory(hotfixAssetPath);
        string versionStr = "";// AssetConfig.InstalledVersion.ToString();//A.GAME_VERSION.ToString();
        //string uploadFolder = PathUtils.EDITOR_UPLOAD;//"Assets/Upload/";
        //string streamFolder = PathUtils.EDITOR_STREAMING;//"Assets/StreamingAssets/";
        string patchFolder = PathUtils.HOTFIX_PATCH + "patch_" + versionStr + "/";
        FileUtils.checkAndCreateDirectory(patchFolder);
        HashDict fileDict = new HashDict();
        FileUtils.EnumeratorDirectoryFiles(hotfixAssetPath, (file) =>
        {
            if (file.IndexOf(".meta") >= 0)
                return;
            byte[] bts = null;
            FileUtils.loadFile(file, out bts);
            string md5 = EncryptoUtils.MD5Encrypt(bts);
            E_PATCH_ASSET_TYPE ptype = E_PATCH_ASSET_TYPE.Config;
            string ext = "";
            string patchPath = "";
            if (file.IndexOf(PathUtils.LUA_PATH) >= 0)
            {
                ptype = E_PATCH_ASSET_TYPE.Lua;
                if (XLuaManager.ENCRYPT)
                {
                    ext = ".bytes";
                    patchPath = file.Replace(PathUtils.LUA_ENCRYPTED, "");
                }
                else
                {
                    ext = ".lua.txt";
                    patchPath = file.Replace(PathUtils.LUA_SCRIPTS, "");
                }
                patchPath = patchPath.Replace(ext, "");
                patchPath = "lua/" + patchPath;

            }
            HashDict itemDict = new HashDict() {
                {"h", md5 },
                {"t", (int)ptype}
            };

            string relPath = file.Replace(hotfixAssetPath, "");
            fileDict.Add(patchPath, itemDict);
            string finalPatchFileWithHash = patchPath + "_" + md5 + "_" + ext;//relPath.Replace(ext, "_" + md5 + "_" + ext);
            string finalPatchFile = patchPath + ext;
            string finalPatchFolder = patchPath.Substring(0, patchPath.LastIndexOf("/") + 1);
            FileUtils.checkAndCreateDirectory(patchFolder + finalPatchFolder);
            File.Copy(file, patchFolder + finalPatchFileWithHash, true);
            FileUtils.checkAndCreateDirectory(PathUtils.HOTFIX_REMOTE + finalPatchFolder);
            File.Copy(file, PathUtils.HOTFIX_REMOTE + finalPatchFileWithHash, true);
        });
        string patchJson = MiniJSON.Json.Serialize(fileDict);
        string patchDescFile = "nsk_patch_" + versionStr + "_.txt";
        FileUtils.saveTxtFileUTF8(PathUtils.HOTFIX_PATCH + patchDescFile, patchJson);
        //
        string patchFilePath = PathUtils.HOTFIX_PATCH + "patch_" + versionStr + ".zip";
        ZipHelper.ZipFile(PathUtils.HOTFIX_PATCH, patchFilePath, patchDescFile);
        byte[] zipBts = null;
        FileUtils.loadFile(patchFilePath, out zipBts);
        string zipMd5 = EncryptoUtils.MD5Encrypt(zipBts);
        string patchFilePathFinal = patchFilePath.Replace(".zip", "_" + zipMd5 + "_.zip");
        File.Move(patchFilePath, patchFilePathFinal);
        string patchFileRemote = PathUtils.HOTFIX_REMOTE + "patch_" + versionStr + "_" + zipMd5 + "_.zip";
        File.Copy(patchFilePathFinal, patchFileRemote);
        //
        string uploadSaveFile = PathUtils.HOTFIX_PATCH + "upload_" + versionStr + "_.zip";
        ZipHelper.ZipFile(patchFolder, uploadSaveFile, "*.*");
        FileUtils.loadFile(uploadSaveFile, out zipBts);
        string uploadZipMd5 = EncryptoUtils.MD5Encrypt(zipBts);
        File.Move(uploadSaveFile, uploadSaveFile.Replace(".zip", "_" + uploadZipMd5 + "_.zip"));

        Logger.log("GeneratePatchFile end!");
        AssetDatabase.Refresh();
    }

    private static void createVersionFile(string version, string patchfile)
    {
        //
    }

    private static void saveFolderFileMD5(string title, string path, string pattern = "*.*")
    {
        ArrayList alist = new ArrayList();
        FileUtils.EnumeratorDirectoryFiles(path, (string file) =>
        {
            byte[] bts = null;
            FileUtils.loadFile(file, out bts);
            string md5 = EncryptoUtils.MD5Encrypt(bts);

            Hashtable ht = new Hashtable();
            ht.Add("f", file);
            ht.Add("h", md5);
            alist.Add(ht);

        }, pattern);
        string jsonStr = MiniJSON.Json.Serialize(alist);
        FileUtils.saveTxtFileUTF8("Assets/Version/" + title + "_" + TimeUtils.GetTime_yyyyMMdd_HHmm() + ".json", jsonStr);
    }

    private static void compareFileMD5AndExportDiff(string oldFile, string newFile)
    {

        Dictionary<string, string> oldDict = getFileMD5Dict(oldFile);
        Dictionary<string, string> newDict = getFileMD5Dict(newFile);

        List<string> diffFiles = new List<string>();
        foreach (var key in newDict.Keys)
        {

            string newMd5 = newDict[key];
            string oldMd5 = null;
            if (oldDict.TryGetValue(key, out oldMd5))
            {
                if (newMd5 != null && newMd5 == oldMd5)
                {
                    Logger.log("File MD5 diff:" + key + " \nnew:" + newMd5 + "\nold:" + oldMd5);
                    diffFiles.Add(key);
                }
            }
            else
            {
                Logger.log("File MD5 new:" + key);
                diffFiles.Add(key);
            }
        }

        //File.Copy("", "", true);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("", "md5 compare complete!", "ok");
    }

    private static Dictionary<string, string> getFileMD5Dict(string file)
    {
        string ajson = FileUtils.loadTxtFile(file);

        ArrayList list = MiniJSON.Json.Deserialize(ajson) as ArrayList;

        Dictionary<string, string> dict = new Dictionary<string, string>(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            Hashtable ht = list[i] as Hashtable;
            dict[ht["f"].ToString()] = ht["h"].ToString();
        }
        return dict;
    }

    private static void AttachHotFixToUIViewInherits()
    {
        Assembly asm = Assembly.Load("Assembly-CSharp");
        System.Type uiviewType = null;//typeof(MiscTestUI);
        System.Type sgtType = typeof(TSingleton<>);
        asm = uiviewType.Assembly;
        string uiPath = "/";
        foreach (var type in asm.GetTypes())
        {
            if (type.BaseType == uiviewType)
            {//已有cs类文件的ui，在uiviewmanager show的时候判断是否需要hotfix
                AttachHotfixMarkToFile(uiPath + type.Name);
            }
            if (type.BaseType == sgtType)
            {//已有cs类文件的singleton，在init的时候判断是否需要hotfix
                AttachHotfixMarkToFile(uiPath + type.Name);
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("", "Attached [Hotfix] to UI classes", "ok");
    }

    private static void AttachHotfixMarkToFile(string filePath)
    {
        //string filePath = uiPath + type.Name;
        string file = FileUtils.loadTxtFile(filePath);
        if (file.IndexOf("[Hotfix]") < 0)
        {
            file = file.Replace("public class", "[Hotfix]\npublic class");

            FileUtils.saveTxtFileUTF8(filePath, file);
            Logger.log("AttachHotFixMarkToFile success:" + filePath);
        }
    }

    [MenuItem("XLua/Custom/Check Hotfix Tags")]
    static void checkHotfixTags()
    {
        List<System.Type> tplist = HotfixTags.by_property;
        for (int i = 0; i < tplist.Count; i++)
        {
            Logger.log("[Hotfix]:" + tplist[i].Name);
        }
        tplist = HotfixTags.by_field;
        for (int i = 0; i < tplist.Count; i++)
        {
            Logger.log("[Hotfix]:" + tplist[i].Name);
        }
        Logger.log("checkHotfixTags complete");
    }

    [MenuItem("XLua/Custom/Apply Editor Hotfix")]
    static void applyEditoHotfix()
    {

        checkHotfixTags();
        CSObjectWrapEditor.Generator.GenAll();
#if HOTFIX_ENABLE
        XLua.Hotfix.HotfixInject();
#endif

        Logger.log("applyEditoHotfix complete");
    }

    [MenuItem("XLua/Clear XLua Injection")]
    static void clearEditorInjection()
    {
        CSObjectWrapEditor.Generator.ClearAll();
    }

    [MenuItem("Build/XLua/Export Lua FileList")]
    static void ExportLuaFileList()
    {
        string folderPath = "Assets/GameAssets/lua/";
        var files = Directory.GetFiles(folderPath, "*.lua.txt", SearchOption.AllDirectories);
        string[] simplifiedFiles = new string[files.Length];
        string str = "[\n";
        for (int i = 0; i < files.Length; i++)
        {
            string f = files[i];
            string fp = f;//f.Substring(f.LastIndexOf("/Assets/"));
            fp = fp.Replace("\\", "/");
            simplifiedFiles[i] = fp;
            str += "\"" + fp + "\",\n";
        }
        str += "]";
        FileUtils.saveTxtFileUTF8("Assets/GameAssets/settings/lua_filelist.json", str);
        AssetDatabase.Refresh();
        Logger.log("Export Lua File List Complete");

    }

    [MenuItem("XLua/Custom/Test Lua FileList")]
    static void TestLuaFilelist()
    {
        TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameAssets/settings/lua_filelist.json");
        var json = MiniJSON.Json.Deserialize(ta.text);
        Logger.log("json:" + json);
        if (json != null)
        {
            Logger.log("json0:" + (json as System.Collections.Generic.List<System.Object>)[0]);
        }
    }
}

public static class HotfixTags
{

    //静态列表
    [Hotfix]
    public static List<System.Type> by_field
    {
        get
        {
            List<System.Type> tpList = new List<System.Type>()
            {
                //typeof(MiscTestUI),
                //typeof(MiscXluaTest),
                /*typeof(BagItemCallback),
                typeof(FriendItemCallBack),
                typeof(FriendApplyCallBack),
                typeof(MailItemCallback),
                typeof(LogItemCallback),
                typeof(LotteryItem),
                typeof(ShopItem),
                typeof(RechargeItem),
                typeof(yuanbaoItemCallback),*/
                typeof(Shopper),
                typeof(ShopperRambleState),
                typeof(SpineUtils),
            };
            Logger.log("[HotfixTags] by_field");
            return tpList;
        }
    }

    //动态列表
    [Hotfix]
    public static List<System.Type> by_property
    {
        get
        {
            List<System.Type> tpList = (from type in System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes()
                                            //where type.Namespace == "abc"
                                        where type.BaseType == typeof(BaseSystem)
                                        || (type.BaseType != null && type.BaseType.Name == "SingletonMono`1")
                                        || (type.BaseType != null && type.BaseType.Name == "TSingletonHotfix`1")
                                        || (type.BaseType != null && type.BaseType.Name == "ViewBase`1")
                                        || (type.BaseType != null && type.BaseType.Name == "MonoBehaviour")
                                        || (type.BaseType != null && type.BaseType.Name == "Entity")
                                        || type.BaseType == typeof(IDataModelProx)
                                        select type).ToList();
            //== typeof(TSingletonHotfix<>)
            Logger.log("[HotfixTags] by_property processed");
            return tpList;
        }
    }
    //表示不要生成成员的适配代码
    /*[BlackList]
    public static List<List<string>> BlackList = new List<List<string>>() {
        new List<string>() {"UnityEngine.GameObject", "networkView" },
        new List<string>() {"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections" }
    };*/

}


public static class xluaTag
{
    [LuaCallCSharp]
    public static List<System.Type> mymodule_lua_call_cs_list
    {
        get
        {
            List<System.Type> tpList = new List<System.Type>()
            {
            typeof(GameObject),
            typeof(Dictionary<string, int>),
            typeof(List<int>),
            typeof(Transform),
            //typeof(Text),
            typeof(Button),
            typeof(Image),
            typeof(Dropdown),
            typeof(Component),
            typeof(Dictionary<int, int>),
            typeof(DoTweenUtil),
            typeof(GUIHelper),
            typeof(uiWindow),
            typeof(System.Action<uiWindow>),
            typeof(System.Action<System.Object>),
            typeof(UnityEngine.Events.UnityAction<Vector2>),
            typeof(System.Action<Vector2>),
            typeof(System.Action<Vector3>),
            typeof(System.Action<int>),
            typeof(System.Action<int,int>),
            typeof(Mosframe.DynamicVScrollView),
            typeof(EventTriggerListener),
            typeof(EventTriggerListener.VectorDelegate),
            typeof(Entity),
            //typeof(EventController),
            //typeof(GameEventType),
            //typeof(AccountSystem),
            //typeof(AccountDataProxy),
           /* typeof(UIVisitor),
            typeof(UIAnimationController),
            typeof(HeroControler),
            typeof(GUIHelper),
            typeof(PropUseControler),
            typeof(PetData),
            typeof(List<ChatClass_F>),
            typeof(List<ChatClass_Msg>),
            typeof(ChatClass_Msg),
            typeof(XmlBasedData<ChatClass_Msg>),
            typeof(System.Security.SecurityElement),
            typeof(Dropdown),
            typeof(Dictionary<TaskTagType, int>),
            typeof(ParticleSystemRenderer),
            typeof(com.the9.events.EventDelegate<Action>),
            typeof(Log_Type),*/

            //DoTween
            typeof(DG.Tweening.AutoPlay),
            typeof(DG.Tweening.AxisConstraint),
            typeof(DG.Tweening.Ease),
            typeof(DG.Tweening.LogBehaviour),
            typeof(DG.Tweening.LoopType),
            typeof(DG.Tweening.PathMode),
            typeof(DG.Tweening.PathType),
            typeof(DG.Tweening.RotateMode),
            typeof(DG.Tweening.ScrambleMode),
            typeof(DG.Tweening.TweenType),
            typeof(DG.Tweening.UpdateType),

            typeof(DG.Tweening.DOTween),
            typeof(DG.Tweening.DOVirtual),
            typeof(DG.Tweening.EaseFactory),
            typeof(DG.Tweening.Tweener),
            typeof(DG.Tweening.Tween),
            typeof(DG.Tweening.Sequence),
            typeof(DG.Tweening.TweenParams),
            typeof(DG.Tweening.Core.ABSSequentiable),


            typeof(DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions>),
            typeof(DG.Tweening.Core.TweenerCore<string, string, DG.Tweening.Plugins.Options.StringOptions>),

            typeof(DG.Tweening.TweenCallback),
            typeof(DG.Tweening.TweenExtensions),
            typeof(DG.Tweening.TweenSettingsExtensions),
            typeof(DG.Tweening.ShortcutExtensions),
            typeof(DG.Tweening.DOTweenModuleUI),

            };

            Logger.log("[LuaCallCS] : mymodule_lua_call_cs_list");

            return tpList;
        }
    }
    [CSharpCallLua]
    public static List<System.Type> mymodule_cs_call_lua_list = new List<System.Type>()
    {
        typeof(UnityEngine.Events.UnityAction<bool>),
        typeof(UnityEngine.Events.UnityAction<int>),
        typeof(System.Collections.IEnumerator),
        typeof(System.Action),
        typeof(System.Action<float>),
        typeof(System.Action<System.Object>),
        typeof(System.Action<System.Object, int>),
        typeof(System.Action<System.ValueType>),
        typeof(System.Func<System.Type, bool, uiWindow>),
        typeof(System.Func<System.Type, bool>),
        typeof(System.Func<string, bool>),
        typeof(System.Func<int, bool>),
        typeof(System.Func<int, int>),
        typeof(System.Func<int, DirectPurchaseData>),
        typeof(System.Func<int>),
        typeof(System.Func<long>),
        typeof(System.Func<bool>),
        typeof(System.Func<int,string>),
        typeof(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle),
        typeof(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Object>),
        typeof(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.List<string>>),
        typeof(System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<System.Collections.Generic.List<string>>>),
        typeof(System.Action<string, System.Object, System.Object, System.Object, System.Object>),
        typeof(UnityEngine.Events.UnityAction<Vector2>),
        typeof(System.Action<Vector2>),
        typeof(System.Action<Vector3>),
        typeof(System.Action<int>),
        typeof(System.Action<int,int>),
        typeof(System.Predicate<OneUserPetInfo>),
        typeof(System.Predicate<System.Int32>),
        typeof(System.Predicate<ShelfEquip>),
        typeof(EventTriggerListener),
        typeof(EventTriggerListener.VectorDelegate),
        typeof(System.Action<Vector3,Vector3>),
        typeof(System.Comparison<ExploreHeroItemView>),
        typeof(System.Collections.IEnumerator),

        typeof(DG.Tweening.Core.DOGetter<float>),
        typeof(DG.Tweening.Core.DOSetter<float>),

        /*typeof(EventTriggerListener.TimeDelegate),
        typeof(EventTriggerListener.VectorDelegate),
        typeof(EventTriggerListener.VoidDelegate),
        typeof(com.the9.events.EventDelegate),
        typeof(com.the9.events.EventDelegate<int>),
        typeof(com.the9.events.EventDelegate<bool>),
        typeof(com.the9.events.EventDelegate<float>),
        typeof(com.the9.events.EventDelegate<string>),
        typeof(com.the9.events.EventDelegate<Transform>),
        typeof(com.the9.events.EventDelegate<E_SHAKE_TYPE>),
        typeof(com.the9.events.EventDelegate<E_GAME_STATE>),
        typeof(com.the9.events.EventDelegate<GameObject>),
        typeof(com.the9.events.EventDelegate<E_QS_STATE_TYPE>),
        typeof(com.the9.events.EventDelegate<PlayerInfoData>),
        typeof(com.the9.events.EventDelegate<Vector3>),
        typeof(com.the9.events.EventDelegate<LinkTextClickData>),
        typeof(com.the9.events.EventDelegate<FightResult>),
        typeof(com.the9.events.EventDelegate<INetworkPackage>),
        typeof(com.the9.events.EventDelegate<IQSRequest>),
        typeof(com.the9.events.EventDelegate<System.Security.SecurityElement>),
        typeof(com.the9.events.EventDelegate<Action>),
        typeof(com.the9.events.EventDelegate<RankUserInfo>),
        typeof(com.the9.events.EventDelegate<List<ChatClass_Msg>>),
        typeof(com.the9.events.EventDelegate<List<ChatClass_F>>),
        typeof(com.the9.events.EventDelegate<List<ChatClass_Msg>>),
        typeof(com.the9.events.EventDelegate<DayLottery_Start>),
        typeof(com.the9.events.EventDelegate<DayLottery_Dice>),
        typeof(com.the9.events.EventDelegate<DayLottery_List>),
        typeof(com.the9.events.EventDelegate<SlotMachineData>),
        typeof(com.the9.events.EventDelegate<SlotMachineMultDice>),
        typeof(com.the9.events.EventDelegate<SlotMachineGet>),
        typeof(com.the9.events.EventDelegate<SlotMachineAnnouncement>),
        typeof(com.the9.events.EventDelegate<SlotMachineDice>),
        typeof(com.the9.events.EventDelegate<SlotMachineData>),
        typeof(com.the9.events.EventDelegate<TeamListInfo>),
        typeof(com.the9.events.EventDelegate<Members>),
        typeof(com.the9.events.EventDelegate<OfferMemberData>),
        typeof(com.the9.events.EventDelegate<User>),
        typeof(com.the9.events.EventDelegate<LeaderChangeData>),
        typeof(com.the9.events.EventDelegate<ChangePet>),
        typeof(com.the9.events.EventDelegate<PetStatus>),
        typeof(com.the9.events.EventDelegate<MapPoint[]>),
        typeof(com.the9.events.EventDelegate<MoveData>),
        typeof(com.the9.events.EventDelegate<PropData[]>),
        typeof(com.the9.events.EventDelegate<RollData>),
        typeof(com.the9.events.EventDelegate<RollResultData>),
        typeof(com.the9.events.EventDelegate<CreateTeam>),
        typeof(com.the9.events.EventDelegate<OffLineData>),
        typeof(com.the9.events.EventDelegate<OnLineData>),
        typeof(com.the9.events.EventDelegate<Offer>),
        typeof(com.the9.events.EventDelegate<WWChatData>),
        typeof(com.the9.events.EventDelegate<PlayerData>),
        typeof(com.the9.events.EventDelegate<WWCampScore[]>),
        typeof(com.the9.events.EventDelegate<CandiatePlayer[]>),
        typeof(com.the9.events.EventDelegate<CandiatePlayer>),
        typeof(com.the9.events.EventDelegate<Supporter[]>),
        typeof(com.the9.events.EventDelegate<KingInfo>),
        typeof(com.the9.events.EventDelegate<StartMove>),
        typeof(com.the9.events.EventDelegate<EndMove>),
        typeof(com.the9.events.EventDelegate<RankList>),
        typeof(com.the9.events.EventDelegate<GameOver>),
        typeof(com.the9.events.EventDelegate<SetFlag>),
        typeof(com.the9.events.EventDelegate<RemoveFlag>),
        typeof(com.the9.events.EventDelegate<StartWork>),
        typeof(com.the9.events.EventDelegate<WorkResult>),
        typeof(com.the9.events.EventDelegate<FightResult>),
        typeof(com.the9.events.EventDelegate<TourDetail>),
        typeof(com.the9.events.EventDelegate<TS_Sever_AddHP>),

        typeof(com.the9.events.EventDelegate<string,int>),
        typeof(com.the9.events.EventDelegate<bool, string>),
        typeof(com.the9.events.EventDelegate<string, string>),
        typeof(com.the9.events.EventDelegate<int, bool>),
        typeof(com.the9.events.EventDelegate<int, int>),
        typeof(com.the9.events.EventDelegate<Action, string>),
        typeof(com.the9.events.EventDelegate<int, System.Security.SecurityElement>),
        typeof(com.the9.events.EventDelegate<long, string>),
        typeof(com.the9.events.EventDelegate<ChatClass_Error, ChatClass_Msg>),
        typeof(com.the9.events.EventDelegate<ExchangeDoProp, Action>),
        typeof(com.the9.events.EventDelegate<MergeInfo, Action>),
        typeof(com.the9.events.EventDelegate<UserAvatarData[], Action>),
        typeof(com.the9.events.EventDelegate<PetData, Action>),
        typeof(com.the9.events.EventDelegate<JewelryData[],Action>),
        typeof(com.the9.events.EventDelegate<FriendGatherItem[], Action>),
        typeof(com.the9.events.EventDelegate<EventUserData[], int>),
        typeof(com.the9.events.EventDelegate<EventFightDetail, int>),
        typeof(com.the9.events.EventDelegate<int, Action>),
        typeof(com.the9.events.EventDelegate<City[], string>),
        typeof(com.the9.events.EventDelegate<SoulProp, Action>),
        typeof(com.the9.events.EventDelegate<Action, TreeGetItem[]>),
        typeof(com.the9.events.EventDelegate<com.the9.qs.war.WarFightResult, Action>),
        typeof(com.the9.events.EventDelegate<string, AfterUpInfo>),
        typeof(com.the9.events.EventDelegate<string, AfterUpInfo>),

        typeof(com.the9.events.EventDelegate<Vector3, string,int>),
        typeof(com.the9.events.EventDelegate<int, bool, int>),
        typeof(com.the9.events.EventDelegate<E_DARKMAP_ITEM, int, Vector3>),
        typeof(com.the9.events.EventDelegate<int, E_WW_ITEM_TYPE, bool>),
        typeof(com.the9.events.EventDelegate<UserPackage, Offer, Action>),
        typeof(com.the9.events.EventDelegate<double,string,string>),
        typeof(com.the9.events.EventDelegate<Ore, List<Enemy>, List<Enemy>>),

        typeof(com.the9.events.EventDelegate<Vector3, int, int, int>),
        typeof(com.the9.events.EventDelegate<UserPackage, Action, UserWeapon, Info>),
        typeof(com.the9.events.EventDelegate<WeaponData, WeaponData, Action, int>),

        typeof(com.the9.events.EventDelegate<Vector3, E_HUD_FONT_STATUS, E_HUD_FONT_STATUS, E_HUD_NUM_COLOR, int>),
        typeof(com.the9.events.EventDelegate<int, int, int, E_BR_MAP_ITEM_TYPE, bool>),
        typeof(com.the9.events.EventDelegate<E_BR_DIR, int , int, int, int>),

        typeof(com.the9.events.EventDelegate<int, int, float, float, E_UsePropItemType, E_BR_TRAP_TYPE>),
        typeof(com.the9.events.EventDelegate<UserPackage, Offer, Action, Result[], Info,UserWeapon>),

        typeof(DG.Tweening.Core.DOSetter<Vector3>),
        typeof(TweenerCore<Vector3, Vector3, VectorOptions>),*/
    };
}



/**

    **/
