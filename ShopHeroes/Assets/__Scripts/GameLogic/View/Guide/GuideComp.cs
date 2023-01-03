using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideComp : MonoBehaviour
{
    public GMask gMask;
    public GFullDialog gFullDialog;
    public GTips gTips;
    public GUnlockFurniture gUnlockFurniture;
    public GUnlockWorker gUnlockWorker;
    public GTask gTask;
    public GNewTask gNewTask;
    public Transform arrowTrans;
    public GameObject promptObj;
    public Button skipBtn;
    public GameObject networkMask;
    public Transform fingerTrans;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SKIPGUIDE);
        }
#else

#endif
    }
}
