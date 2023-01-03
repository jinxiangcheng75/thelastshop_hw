using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public class NewScrollRect : ScrollRect
{
    public event Action<Vector2> onBeginDragHandle;
    public event Action<Vector2> onDragHandle;
    public event Action<Vector2> onEndDragHandle;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        onBeginDragHandle?.Invoke(eventData.delta);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        onDragHandle?.Invoke(eventData.delta);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        onEndDragHandle?.Invoke(eventData.delta);
    }
}
