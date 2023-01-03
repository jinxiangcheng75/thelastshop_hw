using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理 功能逻辑 ， 服务器消息
/// </summary>
public class BaseSystem : IMediator
{
    public BaseSystem()
    {
        XLuaManager.inst.HotfixRaw(GetType().Name, this);
    }

    protected bool isEnter = false;
    protected virtual void AddListeners()
    {
    }
    protected virtual void RemoveListeners()
    {
    }

    public virtual void ReInitSystem()
    {

    }
    //初始化
    protected virtual void OnInit() { }

    public virtual void OnEnter()
    {
        AddListeners();
        isEnter = true;
        OnInit();
    }

    public virtual void OnExit()
    {
        RemoveListeners();
        isEnter = false;
    }

    public virtual void OnUpdate()
    {
    }
}
