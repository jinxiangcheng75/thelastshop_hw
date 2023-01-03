using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameStartHotfix : MonoBehaviour
{
    public string AssetHost = "";
    void Awake()
    {
        //Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
        //  AddressableConfig.RuntimePath = AssetHost;
    }
    // Start is called before the first frame update
    void Start()
    {
        VersionManager.inst.CheckUpdate(this, onUpdateComplete);
    }

    void onUpdateComplete()
    {
        //start lua env
        ManagerBinder.inst.Init(this);
        Logger.log("start lua env");
        XLuaManager.inst.DoString("require \"GameStart\"");
    }
}
