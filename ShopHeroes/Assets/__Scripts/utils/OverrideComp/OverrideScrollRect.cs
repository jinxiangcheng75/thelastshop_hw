using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverrideScrollRect : ScrollRect
{

    public event Action<Vector2> onBeginDragHandle;
    public event Action<Vector2> onDragHandle;
    public event Action<Vector2> onEndDragHandle;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        //base.OnBeginDrag(eventData);

        //Logger.error("begin : " + eventData.delta.x);

        try
        {
            onBeginDragHandle?.Invoke(eventData.delta);
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            throw new System.Exception(ex.Message);
#endif
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //base.OnDrag(eventData);

        //Logger.error("drag : " + eventData.delta.x);

        try
        {
            onDragHandle?.Invoke(eventData.delta);
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            throw new System.Exception(ex.Message);
#endif
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //base.OnEndDrag(eventData);

        //Logger.error("end : " + eventData.delta.x);

        try
        {
            onEndDragHandle?.Invoke(eventData.delta);
        }
        catch (Exception ex)
        {
#if UNITY_EDITOR
            throw new System.Exception(ex.Message);
#endif
        }
    }

    public void SetSpringback(bool leftToRight, int count = 1)
    {
        StopAllCoroutines();
        StartCoroutine(Springback(leftToRight, count));
    }

    IEnumerator Springback(bool leftToRight, int count = 1)
    {
        if (enabled)
        {
            Vector2 v2 = new Vector2(10 * (leftToRight ? 1 : -1) * (count > 1 ? 2 : 1), 0);
            onBeginDragHandle?.Invoke(v2);

            for (int k = 0; k < (count > 1 ? 11 + 7 * (count - 2) : 11); k++)
            {
                onDragHandle?.Invoke(v2);
                yield return new WaitForSeconds(0.005f);
            }

            onEndDragHandle?.Invoke(Vector2.zero);
        }
    }

}
