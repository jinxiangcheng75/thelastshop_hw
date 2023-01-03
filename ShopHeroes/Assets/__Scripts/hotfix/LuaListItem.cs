using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[XLua.CSharpCallLua]
[LuaCallCSharp]
public class LuaListItem : MonoBehaviour, IDynamicScrollViewItem
{

    public TextAsset luaTextAsset;
    public List<ViewInjection> injections;
    LuaTable mScriptEnv;
    //InitComp 初始化组件  SetData 设置数据
    public string luaTextFileName;
    Action lua_initCompHandler;
    Action<int> lua_int_setDataHandler;
    Action<object> lua_setDataEndHandler;
    Action lua_clearDataHandler;
    Action luaUpdate;
    Action luaOnEnable;
    Action luaOnDisable;
    Action luaOnDestroy;
    Action<string> luaDataUpdate;
    public Action<int> onitemclick = null;
    private Dictionary<string, GameObject> objList = new Dictionary<string, GameObject>();

    private void Awake()
    {
        objList.Clear();
        foreach (var inje in injections)
        {
            if (string.IsNullOrEmpty(inje.name) || inje.gameObject == null)
                continue;
            objList.Add(inje.name, inje.gameObject);
        }

        string luascripttext = "";
        string luaname = "";
        if (luaTextAsset != null)
        {
            luascripttext = luaTextAsset.text;
            luaname = luaTextAsset.name.Replace(".lua", "");
        }
        else
        {
            if (!string.IsNullOrEmpty(luaTextFileName) && VersionManager.inst != null)
            {
                luascripttext = VersionManager.inst.GetLuaText(luaTextFileName);
            }
            luaname = luaTextFileName;
        }

        if (!string.IsNullOrEmpty(luascripttext))
        {
            //var luaname = luaTextAsset.name.Replace(".lua", "");
            mScriptEnv = XLuaManager.inst.mUniLuaEnv.NewTable();

            LuaTable meta = XLuaManager.inst.mUniLuaEnv.NewTable();
            meta.Set("__index", XLuaManager.inst.mUniLuaEnv.Global);
            mScriptEnv.SetMetaTable(meta);
            meta.Dispose();
            mScriptEnv.Set("self", this);
            XLuaManager.inst.mUniLuaEnv.DoString(luaTextAsset.text, luaname, mScriptEnv);

            lua_initCompHandler = mScriptEnv.Get<Action>("InitComp");
            lua_initCompHandler?.Invoke();

            mScriptEnv.Get("update", out luaUpdate);
            mScriptEnv.Get("onEnable", out luaOnEnable);
            mScriptEnv.Get("onDisable", out luaOnDisable);
            mScriptEnv.Get("onDestroy", out luaOnDestroy);


            lua_int_setDataHandler = mScriptEnv.Get<Action<int>>("SetData");
            lua_setDataEndHandler = mScriptEnv.Get<Action<object>>("SetData");
            lua_clearDataHandler = mScriptEnv.Get<Action>("ClearData");
            luaDataUpdate = mScriptEnv.Get<Action<string>>("DataUpdate");
        }
    }

    public void LuaDataUpdate(string data)
    {
        if (luaDataUpdate != null)
        {
            luaDataUpdate(data);
        }
    }
    public GameObject GetObjByName(string name)
    {
        if (objList.Count > 0)
        {
            GameObject obj;
            objList.TryGetValue(name, out obj);
            return obj;
        }
        else
        {
            foreach (var inje in injections)
            {
                if (inje.name == name)
                    return inje.gameObject;
            }
        }
        return null;
    }
    public Text GetText(string component)
    {
        var obj = GetObjByName(component);
        if (obj == null) return null;
        return obj.GetComponent<Text>();
    }

    public GUIIcon GetGUIIcon(string component)
    {
        var obj = GetObjByName(component);
        if (obj == null) return null;
        return obj.GetComponent<GUIIcon>();
    }

    public Image GetImage(string component)
    {
        var obj = GetObjByName(component);
        if (obj == null) return null;
        return obj.GetComponent<Image>();
    }

    public Button GetButton(string component)
    {
        var obj = GetObjByName(component);
        if (obj == null) return null;
        return obj.GetComponent<Button>();
    }

    public void SetData(int id) //简单类型数据
    {
        lua_int_setDataHandler?.Invoke(id);
    }

    public void SetData(object param) //复杂类型数据
    {
        lua_setDataEndHandler?.Invoke(param);
    }

    public void ClearData()
    {
        lua_clearDataHandler?.Invoke();
    }

    public int index = 0;
    public void onUpdateItem(int _index)
    {
        index = _index;
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

        lua_initCompHandler = null;
        lua_clearDataHandler = null;
        lua_int_setDataHandler = null;
        lua_setDataEndHandler = null;
        luaUpdate = null;
        luaOnEnable = null;
        luaOnDisable = null;
        luaOnDestroy = null;
        if (mScriptEnv != null && !mScriptEnv.IsNull())
        {
            mScriptEnv.Dispose();
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
