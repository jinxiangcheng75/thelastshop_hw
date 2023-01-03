using System;

public class TSingletonHotfix<T> where T : new()
{
    public bool isInit = false;
    static T m_instance;
    static internal Type m_type = typeof(T);
    public static T inst
    {
        get { return Instance(); }
    }
    public static T Instance()
    {
        if (m_instance == null)
        {
            m_instance = new T();
            XLuaManager.inst.HotfixRaw(m_type.Name, m_instance);
            (m_instance as TSingletonHotfix<T>).init();
            (m_instance as TSingletonHotfix<T>).isInit = true;
        }
        return m_instance;
    }

    virtual protected void init() { }

    public void Release()
    {
        XLuaManager.inst.HotfixDisposeRaw(m_type.Name, null);
        m_instance = default(T);
    }
}
