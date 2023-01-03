using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using XLua;

[System.Serializable]
public class ViewInjection
{
    public string name;
    public GameObject gameObject;
}

[XLua.CSharpCallLua]
[LuaCallCSharp]
public class LuaBehaviour : MonoBehaviour
{
    public TextAsset luaTextAsset;
    public Animator uiAnimator;
    public ViewInjection[] injections;

    public string luaTextAssetName;
    [HideInInspector]
    public Transform mTransform;
    [HideInInspector]
    public GameObject mGameobject;
    LuaTable mScriptEnv;

    Action luaAwake;
    Action luaStart;
    Action luaUpdate;
    Action luaOnEnable;
    Action luaOnDisable;
    Action luaOnDestroy;

    private Dictionary<string, GameObject> objList = new Dictionary<string, GameObject>();

    void Awake()
    {
        mTransform = transform;
        mGameobject = gameObject;

        if (injections != null)
        {
            foreach (var inje in injections)
            {
                if (string.IsNullOrEmpty(inje.name) || inje.gameObject == null)
                    continue;
                objList.Add(inje.name, inje.gameObject);
            }
        }
        if (mScriptEnv == null)
        {
            string luascripttext = "";
            string luaname = "";
            if (luaTextAsset != null)
            {
                luascripttext = luaTextAsset.text;
                luaname = luaTextAsset.name.Replace(".lua", "");
            }
            else
            {

                if (!string.IsNullOrEmpty(luaTextAssetName) && VersionManager.inst != null)
                {
                    luascripttext = VersionManager.inst.GetLuaText(luaTextAssetName);
                }
                luaname = luaTextAssetName;
            }

            if (!string.IsNullOrEmpty(luascripttext))
            {
                mScriptEnv = XLuaManager.inst.mUniLuaEnv.NewTable();

                LuaTable meta = XLuaManager.inst.mUniLuaEnv.NewTable();
                meta.Set("__index", XLuaManager.inst.mUniLuaEnv.Global);
                mScriptEnv.SetMetaTable(meta);
                meta.Dispose();
                mScriptEnv.Set("self", this);
                XLuaManager.inst.mUniLuaEnv.DoString(luascripttext, luaname, mScriptEnv);
                luaAwake = mScriptEnv.Get<Action>("awake");
                mScriptEnv.Get("start", out luaStart);
                mScriptEnv.Get("update", out luaUpdate);
                mScriptEnv.Get("onEnable", out luaOnEnable);
                mScriptEnv.Get("onDisable", out luaOnDisable);
                mScriptEnv.Get("ondestroy", out luaOnDestroy);
                if (luaAwake != null)
                {
                    luaAwake();
                }
            }
        }
    }
    public void setluaTxt(string filename)
    {
        string scriptlus = "";
        if (VersionManager.inst != null)
        {
            scriptlus = VersionManager.inst.GetLuaText(filename);
        }
        if (!string.IsNullOrEmpty(scriptlus) && mScriptEnv == null)
        {
            mScriptEnv = XLuaManager.inst.mUniLuaEnv.NewTable();
            LuaTable meta = XLuaManager.inst.mUniLuaEnv.NewTable();
            meta.Set("__index", XLuaManager.inst.mUniLuaEnv.Global);
            mScriptEnv.SetMetaTable(meta);
            meta.Dispose();
            mScriptEnv.Set("self", this);
            XLuaManager.inst.mUniLuaEnv.DoString(scriptlus, filename, mScriptEnv);
            luaAwake = mScriptEnv.Get<Action>("awake");
            mScriptEnv.Get("start", out luaStart);
            mScriptEnv.Get("update", out luaUpdate);
            mScriptEnv.Get("onEnable", out luaOnEnable);
            mScriptEnv.Get("onDisable", out luaOnDisable);
            mScriptEnv.Get("ondestroy", out luaOnDestroy);
            if (luaAwake != null)
            {
                luaAwake();
            }
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
            if (injections != null)
            {
                foreach (var inje in injections)
                {
                    if (inje.name == name)
                        return inje.gameObject;
                }
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
        objList.Clear();

        luaAwake = null;
        luaStart = null;
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

