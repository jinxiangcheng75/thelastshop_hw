using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class XLuaGenericHelper
{
    [LuaCallCSharp]
    public static Component GetComponent(GameObject go, string compName)
    {
        return go.GetComponent(compName);
    }

    [LuaCallCSharp]
    public static Component GetComponent(GameObject go, System.Type type)
    {
#if UNITY_EDITOR
        Logger.log("[XLuaGenericHelper]GetComponent type:" + type.Name);
#endif
        return go.GetComponent(type);
    }

    [LuaCallCSharp]
    public static Component[] GetComponentsInChildren(GameObject go, System.Type type)
    {
        return go.GetComponentsInChildren(type);
    }

    [LuaCallCSharp]
    public static Button GetButton(GameObject go)
    {
        return go.GetComponent<Button>();
    }
    [LuaCallCSharp]
    public static Text GetText(GameObject go)
    {
        return go.GetComponent<Text>();
    }

    [LuaCallCSharp]
    public static Image GetImage(GameObject go)
    {
        return go.GetComponent<Image>();
    }
    [LuaCallCSharp]
    public static ScrollRect GetScrollRect(GameObject go)
    {
        return go.GetComponent<ScrollRect>();
    }
    [LuaCallCSharp]
    public static InputField GetInputField(GameObject go)
    {
        return go.GetComponent<InputField>();
    }

    [LuaCallCSharp]
    public static RectTransform GetRectTransform(GameObject go)
    {
        return go.GetComponent<RectTransform>();
    }
    [LuaCallCSharp]
    public static Canvas GetCanvas(GameObject go)
    {
        return go.GetComponent<Canvas>();
    }

    [LuaCallCSharp]
    public static Camera GetCamera(GameObject go)
    {
        return go.GetComponent<Camera>();
    }

    //LuaCallCshap白名单
    [LuaCallCSharp]
    public static List<System.Type> m_LuaCallCShapList = new List<System.Type>()
    {
        typeof(GameObject),
        //typeof(UIVisitor),
        typeof(Dropdown),
    };
}