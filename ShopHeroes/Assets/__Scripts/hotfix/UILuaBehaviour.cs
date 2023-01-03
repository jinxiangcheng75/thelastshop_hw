using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;

public class UILuaBehaviour : MonoBehaviour
{
    public string mLuaName;
    public string mLuaPath;

    public Text[] texts;
    public Button[] buttons;
    public Image[] images;
    public GameObject[] gos;

    public Animator uiAnimator;

    LuaTable mScriptEnv;
    interface ILuaBehaviour
    {
        void Awake();
        void Start();
        void OnEnable();
        void OnDisable();
        void OnDestroy();
    }
    ILuaBehaviour mBehaviourHandler;
    void Awake()
    {
        mScriptEnv = XLuaManager.inst.getScriptEnv(mLuaName, this);
        var luaStr = VersionManager.inst.GetLuaText(mLuaPath);
        if (luaStr != null)
        {
            Logger.log("LuaBehaviour Awake Lua:" + mLuaPath);
            XLuaManager.inst.DoString(luaStr, mLuaName, mScriptEnv);
            mBehaviourHandler = mScriptEnv.Get<ILuaBehaviour>(mLuaName);
            mBehaviourHandler.Awake();
        }
        else
        {
            Debug.LogError("LuaBehaviour Lua not found : " + mLuaPath);
        }

    }

    void OnEnable()
    {
        mBehaviourHandler.OnEnable();
    }

    void OnDisable()
    {
        mBehaviourHandler.OnDisable();
    }

    // Use this for initialization
    void Start()
    {
        mBehaviourHandler.Start();
    }

    void OnDestroy()
    {
        mBehaviourHandler.OnDestroy();
    }
}