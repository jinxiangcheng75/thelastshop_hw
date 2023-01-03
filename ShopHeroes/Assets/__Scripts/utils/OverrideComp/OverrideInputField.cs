using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverrideInputField : InputField
{
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (eventData.selectedObject != null) 
        {
            this.contentType = ContentType.IntegerNumber;
        }
    }
}
