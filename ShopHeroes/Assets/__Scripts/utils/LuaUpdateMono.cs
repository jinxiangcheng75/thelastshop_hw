using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaUpdateMono : MonoBehaviour
{
    Action updateHandler;

    public void SetUpdateHandler(Action handler) 
    {
        updateHandler = handler;
    }

    // Update is called once per frame
    void Update()
    {

        if (updateHandler != null)
        {
            updateHandler.Invoke();
        }

    }
}
