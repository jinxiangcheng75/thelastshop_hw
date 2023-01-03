using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathUtils
{

    public const string HOTFIX = "Assets/Hotfix/";
    public const string HOTFIX_ASSET = HOTFIX + "Asset/";
    public const string HOTFIX_PATCH = HOTFIX + "Patch/";
    public const string HOTFIX_REMOTE = HOTFIX + "Remote/";
    public const string HOTFIX_SAVE = HOTFIX + "Save/";
    public const string LUA_PATH = HOTFIX_ASSET + "lua/";
    public const string LUA_SCRIPTS = LUA_PATH + "scripts/";
    public const string LUA_ENCRYPTED = LUA_PATH + "encrypted/";
    public const string CFG_PATH = HOTFIX + "configs/";

    public const string EDITOR_UPLOAD = "Assets/Upload/";
    public const string EDITOR_STREAMING = "Assets/StreamingAssets/";
    public const string RESOURCES = "Assets/Resources/";
    public const string WWW_REMOTE = "Hotfix/Remote/";
    public const string WWW_SAVE = "Hotfix/Save/";


#if UNITY_EDITOR_WIN || UNITY_ANDROID
    private static string ABAFFIX = "Android";
#elif UNITY_IOS
    private static string ABAFFIX = "iOS";
#elif UNITY_STANDALONE_OSX
    private static string ABAFFIX = "OSX";
#else
    private static string ABAFFIX = "Android";
#endif
    public static readonly string ABALL_NAME = "aball_" + ABAFFIX + ".json";
    public static readonly string ABSTREAM_NAME = "abs_streaming_" + ABAFFIX + ".json";

    public const string AASettingsPath = "Assets/GameAssets/settings/";
    public const string AALuaPath = "Assets/GameAssets/lua/";
}

public enum E_PATH_CONVOY
{
    MAP,

}