using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideView : ViewBase<GuideComp>
{
    public override string viewID => ViewPrefabName.GuideUI;
    public override string sortingLayerName => "popup_2";

    public override int showType => (int)ViewShowType.normal;

    private GuideInfo info;

    protected override void onInit()
    {
        base.onInit();

        info = GuideDataProxy.inst.CurInfo;
        contentPane.skipBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SKIPGUIDE);
        });
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        if (fingerTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(fingerTimerId);
            fingerTimerId = 0;
        }
    }

    public override void shiftOut()
    {

    }

    public void Guide_OnUIOpen(K_Guide_UI uiType)
    {
        _uiCanvas.sortingOrder = 1;
        switch (uiType)
        {
            case K_Guide_UI.GMask:
                contentPane.gMask.showGMask();
                break;
            case K_Guide_UI.GWhiteMask:
                contentPane.fingerTrans.gameObject.SetActiveTrue();
                contentPane.gMask.showGWhiteMask();
                break;
            case K_Guide_UI.GDialog:
                if (/*GUIManager.GetWindow<MenuUIView>() != null && GUIManager.GetWindow<MenuUIView>().isShowing*/GUIManager.CurrWindow == null || (GUIManager.CurrWindow != null && GUIManager.CurrWindow.viewID == ViewPrefabName.MainUI))
                {
                    EventController.inst.TriggerEvent(GameEventType.MAINUI_SHIFTOUT);
                }
                contentPane.gFullDialog.gameObject.SetActive(true);
                contentPane.gFullDialog.showFullDialog();
                break;
            case K_Guide_UI.GTips:
                contentPane.gTips.showGTips();
                break;
            case K_Guide_UI.GTask:
                contentPane.gTask.showGTask();
                break;
            case K_Guide_UI.GMaskTips:
                //contentPane.arrowTrans.gameObject.SetActiveTrue();
                contentPane.gMask.showGMask();
                //contentPane.gTips.showGTips();
                break;
            case K_Guide_UI.GUnlockNewFurniture:
                if (/*GUIManager.GetWindow<MenuUIView>() != null && GUIManager.GetWindow<MenuUIView>().isShowing*/GUIManager.CurrWindow == null || (GUIManager.CurrWindow != null && GUIManager.CurrWindow.viewID == ViewPrefabName.MainUI))
                {
                    EventController.inst.TriggerEvent(GameEventType.MAINUI_SHIFTOUT);
                }
                contentPane.gUnlockFurniture.showGUnlockFurniture();
                break;
            case K_Guide_UI.GUnlockWorker:
                if (GUIManager.GetWindow<MenuUIView>() != null && GUIManager.GetWindow<MenuUIView>().isShowing)
                {
                    EventController.inst.TriggerEvent(GameEventType.MAINUI_SHIFTOUT);
                }
                contentPane.gUnlockWorker.showGUnlockWorker();
                break;
            case K_Guide_UI.GFinger:
                contentPane.fingerTrans.gameObject.SetActiveTrue();
                break;
            case K_Guide_UI.GShopperMask:
                contentPane.gMask.showGMaskShopper();
                break;
        }
    }

    public void Guide_OnUIHide(K_Guide_UI uiType)
    {
        switch (uiType)
        {
            case K_Guide_UI.GMask:
                contentPane.fingerTrans.gameObject.SetActiveFalse();
                contentPane.gMask.hideGMask();
                break;
            case K_Guide_UI.GWhiteMask:
                contentPane.fingerTrans.gameObject.SetActiveFalse();
                contentPane.gMask.hideGMask();
                break;
            case K_Guide_UI.GDialog:
                //if ((GUIManager.GetWindow<MenuUIView>() == null || !GUIManager.GetWindow<MenuUIView>().isShowing) && ManagerBinder.inst.mGameState == kGameState.Shop)
                //{
                //    if (GUIManager.CurrWindow == null || (GUIManager.CurrWindow != null && GUIManager.CurrWindow.viewID != ViewPrefabName.RolePanelUI))
                //    {
                //        EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPSCENE);
                //    }
                //}
                EventController.inst.TriggerEvent(GameEventType.MAINUI_SHIFTIN);
                contentPane.gFullDialog.hideFullDialog();
                break;
            case K_Guide_UI.GTips:
                contentPane.gTips.hideGTips();
                break;
            case K_Guide_UI.GTask:
                contentPane.gTask.hideGTask();
                break;
            case K_Guide_UI.GMaskTips:
                contentPane.fingerTrans.gameObject.SetActiveFalse();
                contentPane.gMask.hideGMask();
                contentPane.gTips.hideGTips();
                break;
            case K_Guide_UI.GUnlockNewFurniture:
                //if (GUIManager.GetWindow<MenuUIView>() == null || !GUIManager.GetWindow<MenuUIView>().isShowing)
                //{
                //    EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPSCENE);
                //}
                EventController.inst.TriggerEvent(GameEventType.MAINUI_SHIFTIN);
                contentPane.gUnlockFurniture.hideGUnlockFurniture();
                break;
            case K_Guide_UI.GUnlockWorker:
                //if (GUIManager.GetWindow<MenuUIView>() == null || !GUIManager.GetWindow<MenuUIView>().isShowing)
                //{
                //    EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPSCENE);
                //}
                EventController.inst.TriggerEvent(GameEventType.MAINUI_SHIFTIN);
                contentPane.gUnlockWorker.hideGUnlockWorker();
                break;
            case K_Guide_UI.GFinger:
                contentPane.fingerTrans.gameObject.SetActiveFalse();
                break;
            case K_Guide_UI.GNewTask:
                EventController.inst.TriggerEvent(GameEventType.UIUnlock.VIEW_ONSHOW, ViewPrefabName.MainUI);
                contentPane.gNewTask.hideGNewTask();
                break;
        }
    }

    bool needSetParent = false;
    GameObject fingerClone;
    int fingerTimerId = 0;
    public void setFingerPos(Transform btnTrans, int size, bool needSetParent = false)
    {
        var spriteRender = contentPane.arrowTrans.GetComponent<SpriteRenderer>();
        spriteRender.sortingOrder = 9999;
        //size = 50;
        //contentPane.arrowTrans.localScale = new Vector3(size, size, 0);
        Vector2 v2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), FGUI.inst.uiCamera.WorldToScreenPoint(btnTrans.position), FGUI.inst.uiCamera, out v2);
        var rectTrans = btnTrans.GetComponent<RectTransform>();
        float x = 0;
        float y = 0;
        float height = 0;
        if (rectTrans != null)
        {
            Vector2 pivot = rectTrans.pivot;
            //if (pivot.x > 0.5f)
            //{
            //    x = (rectTrans.rect.width * (pivot.x - 0.5f));
            //}
            //else if (pivot.x < 0.5f)
            //{
            //    x = -(rectTrans.rect.width * (0.5f - pivot.x));
            //}
            if (btnTrans.localScale.x > 0)
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
        if (info.m_curCfg.guide_btn_dev != null)
        {
            var offsetPos = info.m_curCfg.guide_btn_dev.Split('|');

            offsetX = float.Parse(offsetPos[0]);
            offsetY = float.Parse(offsetPos[1]);
        }
        if (info.m_curCfg.arrow_direction != 0)
        {
            if (info.m_curCfg.arrow_direction == 1)
            {
                offsetRotateZ = 180;
            }
            else if (info.m_curCfg.arrow_direction == 2)
            {
                offsetRotateZ = 0;
            }
            else if (info.m_curCfg.arrow_direction == 3)
            {
                offsetRotateZ = -90;
            }
            else if (info.m_curCfg.arrow_direction == 4)
            {
                offsetRotateZ = 90;
            }
        }

        offsetX += FGUI.inst.isLandscape ? -25 : 0;

        contentPane.fingerTrans.GetComponent<RectTransform>().anchoredPosition = new Vector3(v2.x + x + offsetX, v2.y/* + y + height*/ /*+ 120*/ + offsetY, 0);
        contentPane.fingerTrans.eulerAngles = new Vector3(0, 0, offsetRotateZ);
        contentPane.fingerTrans.gameObject.SetActiveTrue();

        if (fingerTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(fingerTimerId);
            fingerTimerId = 0;
        }
        //if (needSetParent)
        //{
        //    curFingerObj = btnTrans.gameObject;
        //    this.needSetParent = true;
        //    if (fingerClone == null)
        //        fingerClone = GameObject.Instantiate(contentPane.arrowTrans.gameObject, contentPane.transform);
        //    fingerClone.SetActiveTrue();
        //    fingerClone.transform.SetParent(btnTrans);
        //    fingerClone.transform.localPosition = new Vector3(0, GuideManager.inst.triggerOffset, 0);
        //    contentPane.arrowTrans.gameObject.SetActiveFalse();
        //    curState = ManagerBinder.inst.mGameState;
        //    curWindowViewId = GUIManager.CurrWindow.viewID;
        //    //fingerTimerId = GameTimer.inst.AddTimer(0.2f, JudgeIsSameWindow);
        //}
    }

    //private void JudgeIsSameWindow()
    //{
    //    if (GUIManager.CurrWindow != null)
    //    {
    //        if (curState == kGameState.Num)
    //        {
    //            curState = ManagerBinder.inst.mGameState;
    //        }
    //        if (curWindowViewId == "")
    //        {
    //            curWindowViewId = GUIManager.CurrWindow.viewID;
    //        }
    //        else
    //        {
    //            //Logger.error("window cur = " + curWindowViewId + "  guimanager cur = " + GUIManager.GetCurrWindowViewID() + "   cur state = " + curState + "   managerbinder state = " + ManagerBinder.inst.mGameState);
    //            if (curWindowViewId != GUIManager.GetCurrWindowViewID() || curState != ManagerBinder.inst.mGameState || (curFingerObj != null && !curFingerObj.activeInHierarchy))
    //            {
    //                setFingerFalse();
    //                curWindowViewId = "";
    //                curState = kGameState.Num;
    //                if (GuideManager.inst.isClickTriggerTarget)
    //                {
    //                    GuideManager.inst.isClickTriggerTarget = false;
    //                }
    //                else
    //                {
    //                    GuideManager.inst.FinishGuideTrigger();
    //                }
    //                GameTimer.inst.RemoveTimer(fingerTimerId);
    //                return;
    //            }
    //            else
    //            {
    //                curWindowViewId = GUIManager.CurrWindow.viewID;
    //                curState = ManagerBinder.inst.mGameState;
    //            }
    //        }
    //    }
    //}

    public void setPromptPos(Transform btnTrans, bool isActive)
    {
        contentPane.promptObj.SetActive(isActive);

        if (isActive)
        {
            Vector2 v2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), FGUI.inst.uiCamera.WorldToScreenPoint(btnTrans.position), FGUI.inst.uiCamera, out v2);
            var rectTrans = btnTrans.GetComponent<RectTransform>();
            contentPane.promptObj.GetComponent<RectTransform>().anchoredPosition = new Vector3(v2.x, v2.y + 280, 0);
        }
    }

    public void setFingerFalse()
    {
        contentPane.fingerTrans.gameObject.SetActiveFalse();
        if (needSetParent)
        {
            if (fingerClone != null)
            {
                needSetParent = false;
                GameObject.Destroy(fingerClone);
                //fingerClone.transform.SetParent(contentPane.transform);
                //fingerClone.SetActiveFalse();
                fingerClone = null;
            }
        }
    }

    public void setTriggerMaskTarget(Transform trans)
    {
        //contentPane.gMask.setTriggerTarget(trans);
    }

    public void setPreMask(bool activeState)
    {
        contentPane.gMask.preMask.SetActive(activeState);
    }

    public void setNetworkMask(bool isActive)
    {
        contentPane.networkMask.SetActive(isActive);
    }

    public void refreshTaskData()
    {
        contentPane.gTask.refreshTaskData();
    }

    public void setNewTask(bool isOk)
    {
        contentPane.gNewTask.showGNewTask(isOk);
    }

    public void setSlotTarget(GameObject target)
    {
        contentPane.gMask.setSlotTarget(target);
    }

    public void setTriggerMask(string panelName, string btnName, bool needWait, bool isStrong)
    {
        //contentPane.gMask.showTriggerMask(panelName, btnName, needWait, isStrong);
    }

    public void setSkipBtnNotActive()
    {
        //contentPane.skipBtn.gameObject.SetActive(false);
    }

    public void hideAllSubPanel()
    {
        contentPane.gFullDialog.gameObject.SetActive(false);
        contentPane.gTask.gameObject.SetActive(false);
        contentPane.gTips.gameObject.SetActive(false);
        contentPane.gNewTask.gameObject.SetActive(false);
        contentPane.gMask.gameObject.SetActive(false);
        contentPane.gUnlockFurniture.gameObject.SetActive(false);
        contentPane.gUnlockWorker.gameObject.SetActive(false);
    }
}
