using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger, IPointerDownHandler, IPointerUpHandler
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void VectorDelegate(GameObject go, Vector2 data);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VectorDelegate onDrag;
    public VectorDelegate onStartDrag;
    public VectorDelegate onEndDrag;

    public bool enableDrag;
    public bool enablePass;
    public float clickTime = 0.5f;
    float mPressDownStart = 0;
    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            if (Time.realtimeSinceStartup - mPressDownStart > clickTime)
            {
                return;
            }
            onClick(gameObject);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null)
            onDown(gameObject);

        mPressDownStart = Time.realtimeSinceStartup;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null)
            onEnter(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null)
            onExit(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
            onUp(gameObject);

    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag(gameObject, eventData.delta);
        }
        if (enablePass) PassUp(eventData, ExecuteEvents.dragHandler);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onStartDrag != null)
        {
            onStartDrag(gameObject, eventData.delta);
        }
        if (enablePass)
            PassUp(eventData, ExecuteEvents.beginDragHandler);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
        {
            onEndDrag(gameObject, eventData.delta);
        }
        if (enablePass)
            PassUp(eventData, ExecuteEvents.endDragHandler);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null)
            onSelect(gameObject);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null)
            onUpdateSelect(gameObject);
    }

    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> func) where T : IEventSystemHandler
    {
        List<RaycastResult> res = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, res);
        GameObject cur = data.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < res.Count; i++)
        {
            if (cur != res[i].gameObject)
            {
                ExecuteEvents.Execute(res[i].gameObject, data, func);
            }
        }
    }

    public void PassUp<T>(PointerEventData data, ExecuteEvents.EventFunction<T> func) where T : IEventSystemHandler
    {
        if (this.transform.parent == null)
            return;
        GameObject go = ExecuteEvents.GetEventHandler<T>(this.transform.parent.gameObject);
        ExecuteEvents.Execute(go, data, func);
    }

    public void clear()
    {
        onClick = null;
        onDown = null;
        onEnter = null;
        onExit = null;
        onUp = null;
        onSelect = null;
        onUpdateSelect = null;
        onDrag = null;
        onStartDrag = null;
        onEndDrag = null;
    }
}

