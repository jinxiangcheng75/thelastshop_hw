using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;


[CSharpCallLua]
//场景物件坐标位置和方向控制
public class ActorPosCrl : MonoBehaviour
{
    //网格位置
    private Vector3Int mapCell = Vector3Int.zero;
    private Transform _transform;

    public string luaScriptName = "ActorPosCrl";
    //lua
    private LuaTable scriptEnv;
    private Action luaAwake;
    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;
    private Action luaOnEnable;
    private Action luaOnDisable;

    public Vector3 tfPosition
    {
        get
        {
            if (_transform == null)
                _transform = this.transform;
            return _transform.position;
        }
    }

    public Vector3Int mMapCell
    {
        get { return mapCell; }
        private set { mapCell = value; }
    }

    public void SetPosition(Vector3Int cell)
    {
        mMapCell = cell;
        if (_transform == null)
            return;
        _transform.position = MapUtils.CellPosToWorldPos(cell);
    }
    public void SetPosition(Vector3 pos)
    {
        //先转到地图块
        var cell = MapUtils.WorldPosToCellPos(pos);
        //在设置位置
        SetPosition(cell);
    }

    void Awake()
    {
        _transform = this.transform;

        // LuaTable meta = XLuaManager.inst.mUniLuaEnv.NewTable();
        // meta.Set("__index", XLuaManager.inst.mUniLuaEnv.Global);
        // scriptEnv.SetMetaTable(meta);
        // meta.Dispose();
        // scriptEnv.Set("self", this);

        // XLuaManager.inst.DoString(@"require 'ActorPosCrl'", "ActorPosCrl", scriptEnv);

        // Action luaAwake = scriptEnv.Get<Action>("awake");
        // scriptEnv.Get("start", out luaStart);
        // scriptEnv.Get("update", out luaUpdate);
        // scriptEnv.Get("ondestroy", out luaOnDestroy);
        // scriptEnv.Get("onEnable", out luaOnEnable);
        // scriptEnv.Get("onDisable", out luaOnDisable);
        // if (luaAwake != null)
        // {
        //     luaAwake();
        // }
    }

    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
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
    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        luaOnEnable = null;
        luaOnDisable = null;
        if (scriptEnv != null)
            scriptEnv.Dispose();
    }
}
