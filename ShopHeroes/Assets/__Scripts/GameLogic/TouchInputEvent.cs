using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

//2dMap 点击处理
public class TouchInputEvent : SingletonMono<TouchInputEvent>
{
    private Dictionary<int, InputEventListener> listenerList = new Dictionary<int, InputEventListener>();

    public Action<GameObject, Vector3> OnTouchDown;
    public Action<GameObject, Vector3> OnTouchUp;
    public Action<Vector3> OnDrag;

    bool touchDown = false;
    bool isDrag = false;
    Vector3 lastTouchPosition;
    Vector3 currentTouchPosition;
    InputEventListener currHitObject;


    //判断是否点中物品
    public bool isPointerOnCollider2D = false;
    public InputEventListener GetListenerByInstanceId(int instanceid)
    {
        InputEventListener l = null;
        listenerList.TryGetValue(instanceid, out l);
        return l;
    }
    public void ClearListenerList()
    {
        listenerList.Clear();
    }
    public void AddListenerToEventList(int instanceId, InputEventListener listener)
    {
        if (listenerList.ContainsKey(instanceId))
        {
            listenerList[instanceId] = listener;
            return;
        }
        listenerList.Add(instanceId, listener);
    }

    public void RemoveListener(int instanceId)
    {
        if (listenerList.ContainsKey(instanceId))
        {
            listenerList.Remove(instanceId);
        }
    }

    void Update()
    {
        if (ManagerBinder.inst.mGameState == kGameState.Update
        || ManagerBinder.inst.mGameState == kGameState.Login
        || ManagerBinder.inst.mGameState == kGameState.Splash
        || ManagerBinder.inst.mGameState == kGameState.Preload
        )
            return;
        #region new
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Input.mousePosition;
            currentTouchPosition = lastTouchPosition;

#else
        if (Input.touchCount <= 0) return;
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                lastTouchPosition = Input.GetTouch(0).position;
                currentTouchPosition = lastTouchPosition;
#endif
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETFINGTERACTIVE, false);
            if (RuinsMap.inst != null)
            {
                RuinsMap.inst.setUpdateState(false);
            }
            GoOperationManager.inst.setFingerTimeReset(false);
            //HotfixBridge.inst.TriggerLuaEvent("DoOperation_TimeFalse");
            HotfixBridge.inst.TriggerLuaEvent("HideFingerFromTouch");
            if (GUIHelper.isPointerOnUI()) return;
            touchDown = true;
            Collider2D[] col = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(lastTouchPosition));
            if (col.Length > 0)
            {
                foreach (Collider2D c in col)
                {
                    if (c.name == "IndoorMask" && c.gameObject.layer == 15)
                    {
                        return;
                    }
                    var listener = GetListenerByInstanceId(c.gameObject.GetInstanceID());
                    if (listener != null && listener.TOnMouseDown(Input.mousePosition))
                    {
                        currHitObject = listener;
                        isPointerOnCollider2D = true;
                        break;
                    }
                }
            }
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButton(0))
        {
            currentTouchPosition = Input.mousePosition;
#else
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                currentTouchPosition = Input.GetTouch(0).position;
#endif
            if (!touchDown) return;
            if (GUIHelper.isPointerOnUI()) return;
            if (IndoorMap.inst != null && IndoorMap.inst.indoorMask.activeSelf) return;
            if (null != Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(currentTouchPosition)) && (Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(currentTouchPosition)).gameObject.layer == 15 || Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(currentTouchPosition)).gameObject.name == "sp_popup"))
            {
                Logger.log("12121212121212121212121221");
                return;
            }
            if (null != Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(currentTouchPosition)) && Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(currentTouchPosition)).gameObject.name == "我是修复进度条")
            {
                Logger.log("343434343434343434343434");
                return;
            }
            if (Vector3.Distance(currentTouchPosition, lastTouchPosition) > 1f)
            {
                //移动
                isDrag = true;
                if (currHitObject != null)
                {
                    var listener = GetListenerByInstanceId(currHitObject.gameObject.GetInstanceID());
                    if (listener != null)
                    {
                        Logger.log("565656565656565656565656565");
                        listener.TOnDray(lastTouchPosition, currentTouchPosition);
                    }
                }
            }
            lastTouchPosition = currentTouchPosition;
        }
#if UNITY_EDITOR
        else if (Input.GetMouseButtonUp(0))
        {
            lastTouchPosition = Input.mousePosition;
#else
        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            lastTouchPosition = Input.GetTouch(0).position;
#endif

            if (FGUI.inst != null)
            {
                FGUI.inst.onMouseClick(lastTouchPosition);
            }
            if (touchDown)
            {
                if (!isDrag)
                {
                    Collider2D[] cols = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(lastTouchPosition));
                    if (cols.Length > 0)
                    {
                        foreach (Collider2D c in cols)
                        {
                            var listener = GetListenerByInstanceId(c.gameObject.GetInstanceID());
                            if (listener != null)
                            {
                                if (currHitObject == listener)
                                {
                                    listener.TOnClick(lastTouchPosition);
                                    EventController.inst.TriggerEvent(GameEventType.TOUCHEVENT_OnPointClick, c.gameObject.layer);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //没有点中场景物体
                        EventController.inst.TriggerEvent(GameEventType.TOUCHEVENT_OnPointBlank);
                    }
                }

                if (currHitObject != null)
                {
                    currHitObject.TOnMouseUp(lastTouchPosition);
                }
            }
            if (GUIManager.CurrWindow != null && (GUIManager.GetCurrWindowViewID() == ViewPrefabName.MainUI || GUIManager.GetCurrWindowViewID() == ViewPrefabName.CityUI))
            {
                GoOperationManager.inst.setFingerTimeReset(true);
            }
            if (RuinsMap.inst != null)
            {
                RuinsMap.inst.setUpdateState(true);
            }

            isDrag = false;
            touchDown = false;
            isPointerOnCollider2D = false;
            currHitObject = null;
            currentTouchPosition = Vector3.zero;
            lastTouchPosition = Vector3.zero;
        }
#if !UNITY_EDITOR
        }
#endif
        #endregion
    }
}
