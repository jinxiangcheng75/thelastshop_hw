using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class InputEventListener : MonoBehaviour
{
    public Action<Vector3> MouseUp;
    public Action<Vector3> MouseDown;
    public Action<Vector3, Vector3> MouseDrag;
    public Action<Vector3> OnClick;
    public Action MousePointBlank;
    void Awake()
    {
        if (TouchInputEvent.inst != null)
        {
            TouchInputEvent.inst.AddListenerToEventList(this.gameObject.GetInstanceID(), this);
        }
    }
    void OnDestroy()
    {
        if (TouchInputEvent.inst != null)
        {
            TouchInputEvent.inst.RemoveListener(this.gameObject.GetInstanceID());
        }
    }
    void OnEnable()
    {
        if (TouchInputEvent.inst != null)
        {
            TouchInputEvent.inst.AddListenerToEventList(this.gameObject.GetInstanceID(), this);
        }
    }
    void OnDisable()
    {
        if (TouchInputEvent.inst != null)
        {
            TouchInputEvent.inst.RemoveListener(this.gameObject.GetInstanceID());
        }
    }
    public virtual bool TOnClick(Vector3 mousepos)
    {
        if (OnClick != null)
        {
            OnClick(mousepos);
        }
        return true;
    }
    public virtual bool TOnDray(Vector3 mousePos1, Vector3 mousePos2)
    {
        if (MouseDrag != null)
        {
            MouseDrag(mousePos1, mousePos2);
        }
        return true;
    }
    public virtual bool TOnMouseDown(Vector3 mousepos)
    {
        if (MouseDown != null)
        {
            MouseDown(mousepos);
        }
        return true;
    }
    public virtual bool TOnMouseUp(Vector3 mousepos)
    {
        if (MouseUp != null)
        {
            MouseUp.Invoke(Input.mousePosition);

        }
        return true;
    }

}
