using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class uiWindow
{
    //渲染顺序
    public uint UIorder = 0;
    //
    public Type viewType;
    //是否正在显示
    public bool isShowing = false;
    protected bool isInit = false;
    protected bool isCache = false;
    //是否显示顶部资源栏
    public bool isShowResPanel = false;
    public TopPlayerShowType topResPanelType = TopPlayerShowType.noSettingAndEnergy; //顶不资源默认显示类型
    public float windowAnimTime;

    //UI需要添加点击背景关闭事件，则将背景添加 Button 组件。并把对象名字命名为CoverBG。
    public Button CoverBGBtn;

    public virtual int showType
    {
        get { return (int)ViewShowType.dataInfo; }
    }
    public virtual string viewID
    {
        get { return ""; }
    }
    public virtual Transform GetRootTf() { return null; }
    public virtual string sortingLayerName { get { return "normal"; } }
    protected virtual void onInit() { }
    protected virtual void onShown() { }
    protected virtual void onHide() { }
    public virtual void MgrShowView(System.Action<uiWindow> _onshow = null) { }
    public virtual void MgrHideview(bool isCache) { }
    public virtual void ViewUpdate() { }
    public virtual void DefineTextFont() { }
    public virtual void shiftOut()
    {
    }
    public virtual void shiftIn()
    {
    }
    public virtual void DestroySelf()
    {
    }
}
