
using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using Object = UnityEngine.Object;
using System.Text;
using System.Collections.Generic;
[XLua.LuaCallCSharp]
public static class UnityUtils
{

    public static bool EqualsNull(System.Object obj) {
        return obj == null;
    }

    public static System.Object ReturnNull () {
        return null;
    }

    public static bool IsNull(this GameObject go)
    {
        return System.Object.ReferenceEquals(go, null);
    }

    public static bool IsNull(this System.Object obj) {
        return System.Object.ReferenceEquals(obj, null);
    }

    public static bool IsNull(this MonoBehaviour mono)
    {
        return System.Object.ReferenceEquals(mono, null);
    }

    public static bool IsNull(this Transform trans)
    {
        return System.Object.ReferenceEquals(trans, null);
    }


    public static void setLocalX(this Transform trans, float x)
    {
        Vector3 p = trans.localPosition;
        p.x = x;
        trans.localPosition = p;
    }

    public static void setLocalY(this Transform trans, float y)
    {
        Vector3 p = trans.localPosition;
        p.y = y;
        trans.localPosition = p;
    }

    public static void LocalPostionZero(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
    }

    public static void SetActiveTrue(this GameObject go)
    {
        if (!go.activeSelf)
            go.SetActive(true);
    }

    public static void SetActiveFalse(this GameObject go)
    {
        if (go.activeSelf)
            go.SetActive(false);
    }

    public static void ChangeLayer(this GameObject go, int layer)
    {
        go.layer = layer;
        foreach (var t in go.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = layer;
        }
    }

    public static string IfNullThenEmpty(this string str)
    {
        return (str == null ? string.Empty : str);
    }

    public static GameObject FindHideChildGameObject(this GameObject parent, string childName)
    {
        if (parent.name == childName)
        {
            return parent;
        }
        if (parent.transform.childCount < 1)
        {
            return null;
        }
        GameObject obj = null;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject go = parent.transform.GetChild(i).gameObject;
            obj = FindHideChildGameObject(go, childName);
            if (obj != null)
            {
                break;
            }
        }
        return obj;
    }
}