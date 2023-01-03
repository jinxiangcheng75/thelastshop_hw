using System;

public class TSingleton<T> where T : new()
{
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
            (m_instance as TSingleton<T>).init();
        }
        return m_instance;
    }

    virtual public void init() { }

    public virtual void Release()
    {
        m_instance = default(T);
    }
}