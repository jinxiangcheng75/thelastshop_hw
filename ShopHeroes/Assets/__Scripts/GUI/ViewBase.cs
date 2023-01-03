using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ViewBase<T> : uiWindow, IDisposable
{
    //画布
    protected Canvas _uiCanvas;
    protected T contentPane;
    protected GameObject contentObject;
    public LuaBehaviour luaBehaviour;
    public ViewBase()
    {
        init();
    }
    void applyHotfix()
    {
        System.Type tp = GetType();
        XLuaManager.inst.HotfixRaw(tp.Name, this);
    }

    public void init()
    {
        string UIFileName = viewID + (FGUI.inst.isLandscape ? "L" : "");
        // var that = this;
        //加载预制件
        ManagerBinder.inst.Asset.InstantiateUIAsync(UIFileName, (uiobj) =>
        {
            contentObject = uiobj;
            if (uiobj != null)
            {
                OnEndLoad();
            }
            else
            {
#if UNITY_EDITOR
                ManagerBinder.inst.Asset.InstantiateUIAsync(viewID, (obj) =>
                {
                    contentObject = obj;
                    OnEndLoad();
                });
#endif
            }
        });
    }

    private void OnEndLoad()
    {
        if (contentObject != null)
        {
            applyHotfix();

            contentObject.SetActive(false);
            contentObject.name = viewID;
            var rtf = contentObject.GetComponent<RectTransform>();
            rtf.SetParent(FGUI.inst.uiHideRootTF, false);
            rtf.anchorMin = Vector2.zero;
            rtf.anchorMax = Vector2.one;
            rtf.anchoredPosition = FGUI.inst.isLandscape ? new Vector2(25f, 0) : Vector2.zero;
            rtf.sizeDelta = FGUI.inst.isLandscape ? new Vector2(-50f, 0) : Vector2.zero;
            _uiCanvas = contentObject.GetComponent<Canvas>();
            luaBehaviour = contentObject.GetComponent<LuaBehaviour>();
            contentPane = contentObject.GetComponent<T>();
            isShowing = false;
            onInit();
            isInit = true;
            var bg = contentObject.transform.Find("CoverBG");
            if (bg != null)
            {
                CoverBGBtn = bg.GetComponent<Button>() ?? bg.gameObject.AddComponent<Button>();
                CoverBGBtn.onClick.AddListener(hide);
                CoverBGBtn.transition = Selectable.Transition.None;
                CoverBGBtn.interactable = false;
            }
            System.Type tp = GetType();
            if (needOpen)
            {
                MgrShowView(onShowCallBack);
            }
        }
    }


    public override string sortingLayerName
    {
        get
        {
            if (_uiCanvas == null) return "normal";
            return _uiCanvas.sortingLayerName;
        }
    }
    public void setRenderSorting(string sortingName, uint order)
    {
        _uiCanvas.sortingLayerName = sortingName;
        //获得当前order 暂时都为0
        _uiCanvas.sortingOrder = (int)order;
        //设置UI层级
        _uiCanvas.sortingLayerName = sortingName;
        //此界面下挂的特效渲染层级
        GUIHelper.setRandererSortinglayer(_uiCanvas.transform, sortingName, (int)order);
    }

    public override Transform GetRootTf()
    {
        if (contentObject != null)
            return contentObject.transform;
        return base.GetRootTf();
    }
    bool needOpen = false;
    Action<uiWindow> onShowCallBack = null;
    /// <summary>
    /// 打开界面
    /// </summary>
    public void show()
    {
        Debug.LogError("正在使用旧的方式打开界面：" + this.viewID);
        var thisWindow = GUIManager.GetWindowByViewId(this.viewID);
        if (thisWindow != null)
        {
            GUIManager.showView(thisWindow, null);
        }
        else
        {
            Debug.LogError("请使用GUIManger打开界面");
        }
    }

    public override void MgrShowView(System.Action<uiWindow> _onshow = null)
    {
        if (CoverBGBtn != null)
            CoverBGBtn.interactable = false;
        if (!isInit)
        {
            needOpen = true;
            onShowCallBack = _onshow;
            return;
        }
        if (contentObject == null)
        {
            init();
            needOpen = true;
            onShowCallBack = _onshow;
            return;
        }
        onShowCallBack = _onshow;
        if (!isShowing && contentObject)
        {
            contentObject.SetActive(true);
            var rtf = contentObject.GetComponent<RectTransform>();
            rtf.SetParent(FGUI.inst.uiRootTF, false);
            rtf.anchorMin = Vector2.zero;
            rtf.anchorMax = Vector2.one;
            rtf.anchoredPosition = FGUI.inst.isLandscape ? new Vector2(25f, 0) : Vector2.zero;
            rtf.sizeDelta = FGUI.inst.isLandscape ? new Vector2(-50f, 0) : Vector2.zero;
            rtf.localScale = Vector3.one;
        }
        if (_uiCanvas.sortingLayerName == "Default") _uiCanvas.sortingLayerName = sortingLayerName;
        setRenderSorting(sortingLayerName == "normal" ? _uiCanvas.sortingLayerName : sortingLayerName, UIorder);
        //延时响应
        isShowing = true;

        if (CoverBGBtn != null)
            CoverBGBtn.interactable = true;
        if (GameSettingManager.inst.needShowUIAnim)
        {
            DoShowAnimation();
        }
        else
        {
            onShown();
        }
        //显示回调

        if (onShowCallBack != null)
        {
            onShowCallBack.Invoke(this);
        }

        EventController.inst.TriggerEvent(GameEventType.UIUnlock.VIEW_ONSHOW, viewID);
        HotfixBridge.inst.TriggerLuaEvent("View_OnShow", viewID);
        onShowCallBack = null;
    }

    virtual protected void DoShowAnimation()
    {
        onShown();
    }
    virtual protected void DoHideAnimation()
    {
        HideView();
    }

    protected void HideView()
    {
        onHide();
        isShowing = false;
        contentObject.SetActive(false);
        if (isCache)
        {
            contentObject.transform.SetParent(FGUI.inst.uiHideRootTF);
        }
        else
        {
            DestroySelf();
        }
    }

    public override void MgrHideview(bool isCache)
    {
        needOpen = false;
        this.isCache = isCache;
        if (isShowing && _uiCanvas)
        {
            if (GameSettingManager.inst.needShowUIAnim)
            {
                DoHideAnimation();
            }
            else
            {
                HideView();
            }
        }
        else
        {
            if (!isCache) DestroySelf();
        }
    }

    public void hide()
    {
        GUIManager.closeView(this.viewType);
    }

    public override void DestroySelf()
    {
        this.Dispose();
    }
    public void Dispose()
    {
        isShowing = false;
        needOpen = false;
        onShowCallBack = null;
        if (contentObject != null)
        {
            beforeDispose();
            GameObject.Destroy(contentObject);
        }
        contentObject = null;
    }

    protected virtual void beforeDispose() { }

    public override void shiftIn()
    {
        HotfixBridge.inst.TriggerLuaEvent("View_OnShow", viewID);
        base.shiftIn();
        contentObject.SetActive(true);
    }

    public override void shiftOut()
    {
        base.shiftOut();
        contentObject.SetActive(false);
    }
}
