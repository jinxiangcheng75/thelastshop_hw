using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Copy from UnityEngine.UI
/// </summary>
/// <typeparam name="T"></typeparam>
public class ScalableObjectPool<T> where T : new()
{
    private Stack<T> mStack;
    private bool m_enableNew;

    public int CountAll { get; private set; }
    public int CountActive { get { return CountAll - mStack.Count; } }
    public int CountInactive { get { return mStack.Count; } }
    public System.Action<T> m_actionGet;
    public System.Action<T> m_actionRelease;

    public ScalableObjectPool(System.Action<T> actionGet, System.Action<T> actionRelease, int capacity = 0, bool enableNew = true)
    {
        m_actionGet = actionGet;
        m_actionRelease = actionRelease;
        if (capacity == 0)
            mStack = new Stack<T>();
        else
            mStack = new Stack<T>(capacity);
        m_enableNew = enableNew;
    }

    public T Get()
    {
        T element;
        if (mStack.Count == 0)
        {
            if (m_enableNew)
            {
                element = new T();// System.Activator.CreateInstance<T>();//new T();//
                CountAll++;
            }
            else
            {
                return default(T);
            }
        }
        else
        {
            element = mStack.Pop();
        }
        if (m_actionGet != null)
            m_actionGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (mStack.Count > 0 && ReferenceEquals(mStack.Peek(), element))
        {
            //Logger.error("[ScalableObjectPool]Release do not release same object to Pool");
            return;
        }
        if (m_actionRelease != null)
            m_actionRelease(element);
        mStack.Push(element);
    }

    public void Clear()
    {
        mStack.Clear();
    }
}
