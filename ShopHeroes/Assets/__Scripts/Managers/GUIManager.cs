using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//目前
public class GUIManager : TSingletonHotfix<GUIManager>
{

    #region  窗口界面管理
    private Dictionary<Type, uiWindow> windowList = new Dictionary<Type, uiWindow>();
    private static Dictionary<string, uiWindow> nameViewList = new Dictionary<string, uiWindow>();
    //当前打开界面序列（只管理 ）
    public static List<uiWindow> showViewList = new List<uiWindow>();
    public static TopPlayerShowType curLuaTopPlayerShowType = TopPlayerShowType.none;

    public static T GetWindow<T>(bool needNew = false) where T : uiWindow
    {
        var view = HotfixBridge.inst.GetWindow(typeof(T), needNew) as T;
        return view;
        // if (!windowList.ContainsKey(typeof(T)))
        // {
        //     if (!needNew) return null;
        //     try
        //     {
        //         T obj = System.Activator.CreateInstance<T>();
        //         obj.type = typeof(T);
        //         uiWindow win = (uiWindow)obj;
        //         windowList.Add(typeof(T), win);
        //         if (!nameViewList.ContainsKey(win.viewID))
        //         {
        //             nameViewList.Add(win.viewID, win);
        //         }
        //         else
        //         {
        //             nameViewList[win.viewID] = win;
        //         }
        //         return obj;
        //     }
        //     catch (Exception e)
        //     {
        //         Logger.error(e.Message);
        //         return null;
        //     }
        // }
        // else
        // {
        //     if (windowList[typeof(T)] == null)
        //     {
        //         windowList.Remove(typeof(T));
        //         return GetWindow<T>(needNew);
        //     }
        //     return (T)windowList[typeof(T)];
        // }
    }

    public static bool Showing<T>() where T : uiWindow
    {
        return HotfixBridge.inst.Showing(typeof(T));
        // T view = GetWindow<T>();
        // if (view == null)
        // {
        //     return false;
        // }
        // return view.isShowing;
    }
    public static uiWindow GetWindowByViewId(string viewid)
    {
        return HotfixBridge.inst.GetWindowByViewId(viewid);
        // if (nameViewList.ContainsKey(viewid))
        // {
        //     return nameViewList[viewid];
        // }
        // return null;
    }
    public static string GetCurrWindowViewID()
    {
        return HotfixBridge.inst.GetCurrWindowViewID();
    }

    public static bool GetViewIsShowingByViewID(string viewID)
    {
        return HotfixBridge.inst.GetViewIsShowingByViewID(viewID);
    }

    public static void showView(uiWindow view, System.Action<uiWindow> onpenCallBack = null)
    {
        HotfixBridge.inst.ShowView(view, (obj) =>
        {
            if (onpenCallBack != null)
            {
                onpenCallBack(obj as uiWindow);
            }
        });

        // if (view != null)
        // {
        //     if (windowList.ContainsKey(view.type))
        //     {
        //         if (!view.isShowing)
        //         {
        //             view.MgrShowView((v) =>
        //             {
        //                 if (onpenCallBack != null)
        //                 {
        //                     onpenCallBack(v);
        //                 }
        //                 if (view.sortingLayerName == "window" || view.sortingLayerName == "popup")
        //                 {
        //                     if (view.sortingLayerName == "window" && showViewList.Count - 1 >= 0)
        //                     {
        //                         if (showViewList[showViewList.Count - 1].sortingLayerName == "window")
        //                             showViewList[showViewList.Count - 1].shiftOut();
        //                     }

        //                     int index = showViewList.FindIndex((item) => v.viewID == item.viewID);
        //                     if (index >= 0)
        //                     {
        //                         showViewList.RemoveAt(index);
        //                     }
        //                     showViewList.Add(v);
        //                 }
        //             });
        //             EventController.inst.TriggerEvent(GameEventType.UI_TOPTESPANEL_ShiftOut, !view.isShowResPanel);
        //         }
        //         else
        //         {
        //             view.shiftIn();
        //             if (onpenCallBack != null)
        //             {
        //                 onpenCallBack(view);
        //             }
        //         }
        //     }
        // }
    }
    public static T OpenView<T>(System.Action<T> onpenCallBack = null) where T : uiWindow
    {
        if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver)
        {
            if (typeof(T) != typeof(GameTextTipView))
            {
                float time = 0.1f;
                var worldParCfg = WorldParConfigManager.inst.GetConfig(8307);
                if (worldParCfg != null)
                {
                    time = worldParCfg.parameters / 1000;
                }
                FGUI.inst.showGlobalMask(time);
            }
        }
        else
        {
            if (typeof(T) != typeof(GameTextTipView))
            {
                FGUI.inst.showGlobalMask(0.5f);
            }
        }

        return HotfixBridge.inst.OpenView(typeof(T), (obj) =>
        {
            if (onpenCallBack != null)
            {
                onpenCallBack(obj as T);
            }
        }) as T;
        // T view = GetWindow<T>(true);
        // if (!view.isShowing)
        // {
        //     view.MgrShowView((v) =>
        //     {
        //         if (onpenCallBack != null)
        //         {
        //             onpenCallBack((T)v);
        //         }
        //         if (v.sortingLayerName == "window" || v.sortingLayerName == "popup")
        //         {
        //             int index = showViewList.FindIndex((item) => v.viewID == item.viewID);
        //             if (index >= 0)
        //             {
        //                 showViewList.RemoveAt(index);
        //             }
        //             showViewList.Add(view);
        //             if (showViewList.Count > 1)
        //             {
        //                 if (v.sortingLayerName == "window")
        //                     showViewList[showViewList.Count - 2].shiftOut();
        //             }
        //             EventController.inst.TriggerEvent(GameEventType.UI_TOPTESPANEL_ShiftOut, !v.isShowResPanel);
        //         }

        //     });

        // }
        // else
        // {
        //     view.shiftIn();
        //     if (onpenCallBack != null)
        //     {
        //         onpenCallBack(view);
        //     }
        //     if (view.sortingLayerName == "window" || view.sortingLayerName == "popup")
        //         EventController.inst.TriggerEvent(GameEventType.UI_TOPTESPANEL_ShiftOut, !view.isShowResPanel);
        // }

        // return view;
    }
    public static void HideView<T>() where T : uiWindow
    {
        HotfixBridge.inst.HideView(typeof(T));

        // if (windowList.ContainsKey(typeof(T)))
        // {
        //     T view = GetWindow<T>();
        //     if (view != null && view.isShowing)
        //     {
        //         view.MgrHideview();
        //         showViewList.Remove(view);
        //     }
        // }
        // //设置当前界面打开状态
        // if (CurrWindow != null)
        // {
        //     if (CurrWindow.sortingLayerName == "window")
        //         CurrWindow.shiftIn();
        //     EventController.inst.TriggerEvent(GameEventType.UI_TOPTESPANEL_ShiftOut, !CurrWindow.isShowResPanel);
        // }
    }

    public static void closeView(Type viewtype)
    {
        HotfixBridge.inst.CloseView(viewtype);
        // if (nameViewList.ContainsKey(viewid))
        // {
        //     uiWindow view = nameViewList[viewid];
        //     if (view != null && view.isShowing)
        //     {
        //         view.MgrHideview();
        //         showViewList.Remove(view);
        //     }
        // }
        // //设置当前界面打开状态
        // if (CurrWindow != null)
        // {
        //     //if (showViewList[showViewList.Count - 1].sortingLayerName == "window")
        //     CurrWindow.shiftIn();
        //     EventController.inst.TriggerEvent(GameEventType.UI_TOPTESPANEL_ShiftOut, !CurrWindow.isShowResPanel);
        // }
    }

    public static uiWindow CurrWindow
    {
        get
        {
            // if (showViewList.Count > 0)
            // {
            //     return showViewList[showViewList.Count - 1];
            // }
            // return null;
            return HotfixBridge.inst.GetCurrWindow();
        }
    }

    //关闭除主界面以外的所有界面(打开设置界面前调用)
    public static void CloseAllUI()
    {
        List<uiWindow> viewlist = allShowingView;
        foreach (var view in viewlist)
        {
            if (view.isShowing)
            {
                if (view.viewID != ViewPrefabName.MainUI && view.viewID != ViewPrefabName.CityUI && (view.sortingLayerName == "window" || view.sortingLayerName == "popup"))
                {
                    closeView(view.viewType);
                }
            }
        }
    }

    public static List<uiWindow> allShowingView
    {
        get
        {
            return HotfixBridge.inst.AllShowingView();
        }
    }

    //只针对商店场景和城市场景
    public static void BackMainView()
    {
        HotfixBridge.inst.CallBackMainView();
    }

    public static void ClearAll()
    {
        HotfixBridge.inst.CallClearAllView();
    }
    #endregion

}
