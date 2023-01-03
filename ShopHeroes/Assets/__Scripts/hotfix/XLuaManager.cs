using System;
using System.Text;
using System.Collections.Generic;
using XLua;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IXluaManager
{
    void registerLua(string path, string hash);
}

public class XLuaManager : TSingleton<XLuaManager>
{
    //
    public const bool ENCRYPT = false;
    public const string LUA_ENCYPT_EXT = ".bytes";
    public const string LUA_TXT_EXT = ".lua.txt";
    public LuaEnv mUniLuaEnv;//唯一luaenv环境
    private Dictionary<string, HotfixItem> m_hotfixMarkDict;
    private byte[] m_encKey = null;
    private bool ENABLE_ENCRYPT = ENCRYPT;
    private bool ENABLE_DEBUG = false;
    private bool m_inited = false;

    public override void init()
    {
        if (m_inited == false)
        {
            InitLuaEnv();
        }
    }

    public void InitLuaEnv()
    {
        if (mUniLuaEnv == null)
            mUniLuaEnv = new LuaEnv();
        if (m_hotfixMarkDict == null)
            m_hotfixMarkDict = new Dictionary<string, HotfixItem>();
        else
            m_hotfixMarkDict.Clear();
        m_encKey = System.Text.Encoding.UTF8.GetBytes(EncryptoUtils.des_key);

#if UNITY_EDITOR
        if (ENABLE_DEBUG)
        {
            TextAsset ld = AssetDatabase.LoadAssetAtPath<TextAsset>(PathUtils.RESOURCES + "LuaDebug.lua.txt");
            DoString(ld.text, "LuaDebug");
            TextAsset tt = AssetDatabase.LoadAssetAtPath<TextAsset>(PathUtils.RESOURCES + "LuaDebugBreak.lua.txt");
            DoString(tt.text, "LuaDebugBreak");
        }
#endif
        //addTextLoader(mUniLuaEnv);
        initSkipList();
        addAddressableLoader(mUniLuaEnv);
        m_inited = true;
    }

    void initSkipList()
    {
        //mLoaderSkipDict = new Dictionary<string, bool>();
    }

#if UNITY_EDITOR
    public void addLoaderForEditor(LuaEnv env, byte[] key)
    {
        addEncryptLoader(env, key);
    }
#endif

    private string getRootPath()
    {
#if UNITY_EDITOR
        /*if (ENABLE_ENCRYPT)
            return PathUtils.LUA_ENCRYPTED;
        else
            return PathUtils.LUA_SCRIPTS;*/

        return PathUtils.HOTFIX_SAVE;
#else
        return Application.persistentDataPath + "/";
#endif
    }

    void addAddressableLoader(LuaEnv env)
    {
        env.AddLoader((ref string filename) =>
        {
            log("try load lua:" + filename);
            var luaBytes = VersionManager.inst.GetLuaFile(filename);
            if (luaBytes == null)
            {
                logError("try load lua failed:" + filename);
            }
            return luaBytes;
        });
    }

    private void addEncryptLoader(LuaEnv env, byte[] encKey)
    {
#if UNITY_EDITOR
        Logger.log("addEncryptLoader");
#endif
        env.AddLoader((ref string filename) =>
        {
            byte[] bts = null;
            string fpath = getRootPath() + filename + LUA_ENCYPT_EXT;
#if UNITY_EDITOR
            if (Logger.ENABLED)
                Logger.info("[XLuaManager]addEncryptLoader filename:" + fpath);
#endif
            FileUtils.loadFile(fpath, out bts);
            //decrypt bts
            EncryptoUtils.XORBytes(bts, encKey);
#if UNITY_EDITOR
            string f = System.Text.Encoding.UTF8.GetString(bts);
            if (Logger.ENABLED)
                Logger.info("[XLuaManager]addEncryptLoader content:" + f);
#endif
            return bts;
        });
    }

    private void addTextLoader(LuaEnv env)
    {
#if UNITY_EDITOR
        Logger.log("addTextLoader");
#endif
        env.AddLoader((ref string filename) =>
        {
            byte[] bts = null;
            string fpath = getRootPath() + filename + LUA_TXT_EXT;
#if UNITY_EDITOR
            if (Logger.ENABLED)
                Logger.info("[XLuaManager]addTextLoader filename:" + fpath);
#endif
            string fstr = FileUtils.loadTxtFile(fpath);
#if UNITY_EDITOR
            if (Logger.ENABLED)
                Logger.info("[XLuaManager]addTextLoader content:" + fstr);
#endif
            bts = System.Text.Encoding.UTF8.GetBytes(fstr);
            return bts;
        });
    }

    public void registerLua(string path, string hash)
    {
        m_hotfixMarkDict.Add(path, new HotfixItem()
        {
            enabled = true,
            file = path
        });
    }

    public void SetHotfixList(List<string> pathList)
    {
        for (int i = 0; i < pathList.Count; i++)
        {
            var p = pathList[i];
            m_hotfixMarkDict.Add(p, new HotfixItem()
            {
                enabled = true,
                file = p
            });
        }
    }
    [LuaCallCSharp]
    public void HotfixRaw(string className, System.Object target)
    {
        if (!VersionManager.inst.CheckNeedHotfix(className))
        {
            return;
        }
        Logger.log("Has Hotfix:" + className);
        var scriptEnv = getScriptEnv(className, target);
        mUniLuaEnv.DoString("require(\"hotfix/" + className + "Hotfix" + "\")", className, scriptEnv);
    }

    public void HotfixDisposeRaw(string className, LuaTable scriptEnv)
    {
        scriptEnv.Dispose();
    }

    public void HotfixSystem(string className, System.Object target, bool useScriptEnv = false)
    {
        Hotfix("lua/system/" + className, target, useScriptEnv);
    }

    public void HotfixAuto(string className, System.Object target, bool useScriptEnv = false)
    {
        if (className.IndexOf("DataProxy") >= 0)
            Hotfix("lua/data/" + className, target, useScriptEnv);
        else if (className.IndexOf("Config") >= 0)
            Hotfix("lua/config/" + className, target, useScriptEnv);
    }

    public void Hotfix(string className, System.Object target, bool useScriptEnv = false)
    {
        HotfixItem res = null;
        if (m_hotfixMarkDict.TryGetValue(className, out res))
        {
            if (res != null && res.enabled)
            {
                if (Logger.ENABLED)
                    Logger.info("[XLuaManager] Hotfix applied :" + className);
                LuaTable tb = null;
                if (useScriptEnv)
                    tb = getNewScriptEnviroment(className, mUniLuaEnv, target);
                mUniLuaEnv.DoString("require('" + res.file + "')", className, tb);
                res.use_script_env = true;
                res.script_table = tb;
            }
            else
            {
                if (Logger.ENABLED)
                    Logger.info("[XLuaManager] Hotfix disabled :" + className);
            }
        }
        else
        {
            if (Logger.ENABLED)
                Logger.info("[XLuaManager] Hotfix not found :" + className);
        }
    }

    public void DisposeHotfixUI(string className)
    {
        DisposeHotfix("lua/ui/" + className);
    }
    public void DisposeHotfixSystem(string className)
    {
        DisposeHotfix("lua/system/" + className);
    }

    public void DisposeHotfix(string className)
    {
        HotfixItem res = null;
        if (m_hotfixMarkDict.TryGetValue(className, out res))
        {
            if (res != null && res.use_script_env)
            {
                if (Logger.ENABLED)
                    Logger.info("[XLuaManager] DisposeHotfix applied :" + className);
                res.script_table.Dispose();
            }
            else
            {
                if (Logger.ENABLED)
                    Logger.info("[XLuaManager] DisposeHotfix didnot use script env :" + className);
            }
        }
        else
        {
            if (Logger.ENABLED)
                Logger.info("[XLuaManager] DisposeHotfix not found :" + className);
        }
    }

    public LuaTable getScriptEnv(string chunkName, System.Object target)
    {
        LuaTable scriptEnv = mUniLuaEnv.NewTable();
        LuaTable meta = mUniLuaEnv.NewTable();
        meta.Set("__index", mUniLuaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", target);
        return scriptEnv;
    }

    private LuaTable getNewScriptEnviroment(string chunkName, LuaEnv luaenv, System.Object target)
    {
        LuaTable tb = luaenv.NewTable();
        LuaTable meta = luaenv.NewTable();
        meta.Set("__Index", luaenv.Global);
        tb.SetMetaTable(meta);
        meta.Dispose();

        tb.Set("self", target);
        return tb;
    }

    public void DoString(string luaString, string chunk = "chunk", LuaTable scriptEnv = null)
    {
        mUniLuaEnv.DoString(luaString, chunk, scriptEnv);
    }

    public T GetGlobal<T>(string varName)
    {
        return mUniLuaEnv.Global.Get<T>(varName);
    }

    public void Update()
    {
        if (mUniLuaEnv != null)
            mUniLuaEnv.Tick();
    }

    public void Dispose()
    {
        // HotfixBridge.inst.Release();


        if (mUniLuaEnv.translator.AllDelegateBridgeReleased())
        {
            mUniLuaEnv.Dispose();
        }
        else
        {
            Debug.LogError("AllDelegateBridgeReleased: false");
        }
        mUniLuaEnv = null;
        m_inited = false;
    }

    void log(string msg)
    {
        Logger.log(msg);
    }

    void logError(string msg)
    {
        Debug.LogError(msg);
    }
}

class HotfixItem
{
    public bool enabled;
    public bool use_script_env;
    public LuaTable script_table;
    public string file;
}

public enum E_PATCH_ASSET_TYPE
{
    Config,
    Assetbundle_Info,
    Lua,
}