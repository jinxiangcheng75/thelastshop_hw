using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainLineTaskFinger : MonoBehaviour
{
    public SpriteRenderer finger;
    public RectTransform fingerRect;
    private bool isStart;
    private Transform targetTrans;
    private float time;
    private bool isUpdateRefreshFinger;
    K_Operation_Finger curType = K_Operation_Finger.max;

    float fingerTime;
    float targetLevel;

    private void Start()
    {
        fingerTime = WorldParConfigManager.inst.GetConfig(161).parameters;
        targetLevel = WorldParConfigManager.inst.GetConfig(160).parameters;
    }

    public void setBoolState(bool curState)
    {
        if (curState)
        {
            //Logger.error("自动找目标");
            time = 0;
            isStart = curState;
            //if (GUIManager.CurrWindow != null && GUIManager.CurrWindow.viewID == ViewPrefabName.MainUI)
            //{
            //    time = 0;
            //    isStart = curState;
            //}
        }
        else
            isStart = curState;
    }

    public void setTargetTrans(Transform target, bool needSetPosImmediately, K_Operation_Finger type)
    {
        targetTrans = target;

        if (needSetPosImmediately) setFingerPos(target, type);
    }

    public void setTargetTrans(Vector3 target, bool needSetPosImmediately, K_Operation_Finger type)
    {
        if (needSetPosImmediately) setFingerPos(target, type);
    }

    public void setFingerState(bool isAlive)
    {
        //Logger.error("设置手指的显隐" + isAlive);
        fingerRect.gameObject.SetActive(isAlive);
        if (!isAlive)
        {
            isUpdateRefreshFinger = false;
        }
    }

    private void setFingerPos(Transform targetTrans, K_Operation_Finger type)
    {
        //Logger.error("设置手指的位置了");
        isUpdateRefreshFinger = true;
        fingerRect.gameObject.SetActive(true);
        if (targetTrans.name == "Sure" && (GUIManager.GetCurrWindowViewID() != ViewPrefabName.MainUI && GUIManager.GetCurrWindowViewID() != ViewPrefabName.CityUI))
        {
            fingerRect.gameObject.SetActive(false);
        }

        Vector2 v2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), FGUI.inst.uiCamera.WorldToScreenPoint(targetTrans.position), FGUI.inst.uiCamera, out v2);
        var rectTrans = targetTrans.GetComponent<RectTransform>();
        float x = 0;
        float y = 0;
        float height = 0;
        if (rectTrans != null)
        {
            Vector2 pivot = rectTrans.pivot;
            if (targetTrans.localScale.x > 0)
            {
                if (pivot.x > 0.5f)
                {
                    x = -(rectTrans.rect.width * (pivot.x - 0.5f));
                }
                else if (pivot.x < 0.5f)
                {
                    x = (rectTrans.rect.width * (0.5f - pivot.x));
                }
                if (pivot.y > 0.5f)
                {
                    y = -(rectTrans.rect.height * (pivot.y - 0.5f));
                }
                else if (pivot.y > 0.5f)
                {
                    y = (rectTrans.rect.height * (0.5f - pivot.y));
                }
            }
            else
            {
                if (pivot.x < 0.5f)
                {
                    x = (rectTrans.rect.width * (pivot.x - 0.5f));
                }
                else if (pivot.x > 0.5f)
                {
                    x = -(rectTrans.rect.width * (0.5f - pivot.x));
                }
                if (pivot.y > 0.5f)
                {
                    y = -(rectTrans.rect.height * (pivot.y - 0.5f));
                }
                else if (pivot.y > 0.5f)
                {
                    y = (rectTrans.rect.height * (0.5f - pivot.y));
                }
            }

            height = rectTrans.rect.height / 2;
        }

        float offsetX = 0, offsetY = 0, offsetRotateZ = 0;
        if (GoOperationManager.inst.CurData != null && GoOperationManager.inst.CurData.offset != null)
        {
            var splitStr = GoOperationManager.inst.CurData.offset.Split('|');
            if (0 < splitStr.Length)
                offsetX = float.Parse(splitStr[0]);
            if (1 < splitStr.Length)
                offsetY = float.Parse(splitStr[1]);
            if (2 < splitStr.Length)
                offsetRotateZ = float.Parse(splitStr[2]);
        }

        offsetX += FGUI.inst.isLandscape ? -25 : 0;

        if (targetTrans.name != "Sure" && targetTrans.name != "goBtn" && !GoOperationManager.inst.isInitOperation)
        {
            isUpdateRefreshFinger = false;
            fingerRect.DOAnchorPos3D(new Vector3(v2.x + x + offsetX, v2.y + y /*+ height + 40*/ + offsetY, 0), 0.5f).From(0).OnStart(() =>
            {
                fingerRect.transform.eulerAngles = new Vector3(0, 0, offsetRotateZ);
            });
            GoOperationManager.inst.isInitOperation = true;
        }
        else
        {
            fingerRect.anchoredPosition = new Vector3(v2.x + x + offsetX, v2.y + y /*+ height + 40*/ + offsetY, 0);
            fingerRect.transform.eulerAngles = new Vector3(0, 0, offsetRotateZ);
        }
        // this.targetTrans = null;
    }

    private void setFingerPos(Vector3 targetTrans, K_Operation_Finger type)
    {
        //Logger.error("设置手指的位置了");
        isUpdateRefreshFinger = true;
        fingerRect.gameObject.SetActive(true);
        if ((GUIManager.GetCurrWindowViewID() != ViewPrefabName.MainUI && GUIManager.GetCurrWindowViewID() != ViewPrefabName.CityUI))
        {
            fingerRect.gameObject.SetActive(false);
        }

        float offsetX = 0, offsetY = 0, offsetRotateZ = 0;
        if (GoOperationManager.inst.CurData != null && GoOperationManager.inst.CurData.offset != null)
        {
            var splitStr = GoOperationManager.inst.CurData.offset.Split('|');
            if (0 < splitStr.Length)
                offsetX = float.Parse(splitStr[0]);
            if (1 < splitStr.Length)
                offsetY = float.Parse(splitStr[1]);
            if (2 < splitStr.Length)
                offsetRotateZ = float.Parse(splitStr[2]);
        }

        fingerRect.position = new Vector3(targetTrans.x /*+ offsetX*/, targetTrans.y /*+ offsetY*/, 0);
        fingerRect.transform.eulerAngles = new Vector3(0, 0, offsetRotateZ);
        // this.targetTrans = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (ManagerBinder.inst.mGameState != kGameState.Shop && ManagerBinder.inst.mGameState != kGameState.Town) return;
        if (isStart && UserDataProxy.inst.playerData.level < targetLevel && WorldParConfigManager.inst.GetConfig(164).parameters != 0)
        {
            time += Time.deltaTime;
            if (time >= fingerTime)
            {
                time = 0;
                if (targetTrans != null && GUIManager.CurrWindow != null && (GUIManager.GetCurrWindowViewID() == ViewPrefabName.MainUI || GUIManager.GetCurrWindowViewID() == ViewPrefabName.CityUI) && !GuideManager.inst.isInTriggerGuide)
                {
                    if ((GoOperationManager.inst.CurData != null && GoOperationManager.inst.CurData.type == K_Operation_DataType.MainLine) || GoOperationManager.inst.CurData == null)
                    {
                        //Logger.error("设置重新开始了");
                        //GoOperationManager.inst.index = -1;
                        if (!fingerRect.gameObject.activeSelf)
                            fingerRect.gameObject.SetActive(true);
                        setFingerPos(targetTrans, K_Operation_Finger.Normal);
                        isStart = false;
                    }
                }
            }
        }

        if (isUpdateRefreshFinger)
        {
            if (targetTrans != null)
            {
                setFingerPos(targetTrans, curType);
            }
        }
    }
}
