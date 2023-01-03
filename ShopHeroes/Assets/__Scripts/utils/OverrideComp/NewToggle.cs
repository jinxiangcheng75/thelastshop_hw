using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewToggle : Toggle
{
    public int curToggleIndex;

    public event Action<int> onClickHandle;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onClickHandle?.Invoke(curToggleIndex);
    }
}
