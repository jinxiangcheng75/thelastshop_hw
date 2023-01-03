using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
public class LuaComponent : MonoBehaviour
{
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;

    private static Action luaAwake;
    private static Action luaStart;
    private static Action luaUpdate;
    private static Action luaOnEnable;
    private static Action luaOnDisable;
    private static Action luaOnDestroy;
    private static LuaTable scriptEnv;

    public static void Add(GameObject go, LuaTable tableClass)
    {
        scriptEnv = XLuaManager.inst.mUniLuaEnv.NewTable();

        LuaTable meta = XLuaManager.inst.mUniLuaEnv.NewTable();
        meta.Set("__index", XLuaManager.inst.mUniLuaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        go.AddComponent<LuaComponent>();
        scriptEnv = tableClass;

        luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("onEnable", out luaOnEnable);
        scriptEnv.Get("onDisable", out luaOnDisable);
        scriptEnv.Get("ondestroy", out luaOnDestroy);
        if (luaAwake != null)
        {
            luaAwake();
        }
    }



    void OnEnable()
    {
        if (luaOnEnable != null)
        {
            luaOnEnable();
        }
    }

    void OnDisable()
    {
        if (luaOnDisable != null)
        {
            luaOnDisable();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaAwake = null;
        luaStart = null;
        luaUpdate = null;
        luaOnEnable = null;
        luaOnDisable = null;
        luaOnDestroy = null;
        if (scriptEnv != null && !scriptEnv.IsNull())
        {
            scriptEnv.Dispose();
        }
    }
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
    }
}
