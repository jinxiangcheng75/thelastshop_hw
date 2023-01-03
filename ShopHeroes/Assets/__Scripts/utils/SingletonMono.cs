using System.Collections;
using System;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    static internal Type m_type = typeof(T);
    public static T inst
    {
        get
        {
            return instance;
        }
    }

    public virtual void init()
    {

    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this as T;

        (instance as SingletonMono<T>).init();
        XLuaManager.inst.HotfixRaw(m_type.Name, instance);
        //init();
    }
}
