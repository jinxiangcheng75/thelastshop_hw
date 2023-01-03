using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;


public class UpgradeAttacher : MonoBehaviour
{
    public SpriteRenderer pb_content;
    public TextMeshPro txt_time;
    public DOTweenAnimation UI_Animation;
    float mTotalTime;
    double mRemainTime;
    float mMaxContentWidth;
    float mContentX;
    int timerId;
    public bool completed;
    public int type = 0;    //0家具升级 1商店扩建
    public int FUid = 0;

    public InputEventListener askAidListener;

    void Awake()
    {
        mMaxContentWidth = pb_content.size.x;
        mContentX = pb_content.transform.localPosition.x;
        completed = true;
        //LanguageManager.inst.AddChangeTextMeshPro(txt_time);
        //LanguageManager.inst.SetText();
    }

    void Start()
    {
        var listener = GetComponent<InputEventListener>() ?? gameObject.AddComponent<InputEventListener>();
        if (listener != null)
            listener.OnClick = OnPointerClick;

        askAidListener.OnClick = onAskAidBtnClick;
    }
    /// <summary>
    /// 设置初始时间
    /// </summary>
    /// <param name="totalTime">完成时间</param>
    /// <param name="remainTime">剩余时间</param>
    double endtime = 0;
    public void setTime(double totalTime, double remainTime)
    {
        completed = false;
        mTotalTime = (int)totalTime;
        mRemainTime = remainTime;
        endtime = GameTimer.inst.serverNow + mRemainTime;
        //mElapsedTime = 0;
        refreshContent();
        clearTimer();
        timerId = GameTimer.inst.AddTimer(1, (int)mRemainTime, refreshContent);

        checkAidBtn();
    }

    public void setFinished()
    {
        completed = true;
        txt_time.text = LanguageManager.inst.GetValueByKey("完成");
        refreshProgressBar(1);
        clearTimer();
        FinishedAnim();

        if (fakeCheck)
        {
            askAidListener.gameObject.SetActiveTrue();
        }
        else
            askAidListener.gameObject.SetActiveFalse();

        HotfixBridge.inst.TriggerLuaEvent("EventSystem_AddEvent", "Design_Levelup_Event", "", 1, (int)kGameState.Shop);
        //EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.CameraMove_CheckExtendUpOrFurnitureUpEnd);
    }

    public void FinishedAnim()
    {
        transform.GetChild(0).DOScale(0.35f, 0.3f).OnComplete(() =>
        {
            transform.GetChild(0).DOScale(0.5f, 0.3f).OnComplete(() =>
            {
                UI_Animation.DOPlayForward();
            });
        });
    }

    void checkAidBtn()
    {

        //判断是否展示请求援助按钮
        if (UserDataProxy.inst.playerData.hasUnion)
        {
            if (type == 0) //家具
            {
                askAidListener.gameObject.SetActive(UserDataProxy.inst.helpFurnitureFlag == 0);
            }
            else if (type == 1) //扩建
            {
                askAidListener.gameObject.SetActive(UserDataProxy.inst.helpShopExtenFlag == 0);
            }
        }
        else
        {
            askAidListener.gameObject.SetActiveFalse();
        }

        if (fakeCheck)
        {
            askAidListener.gameObject.SetActiveTrue();
        }
    }

    bool fakeCheck = false;
    public void setTriggerAidBtn()
    {
        fakeCheck = true;
        checkAidBtn();
    }

    void refreshContent()
    {
        if (mRemainTime == 0 || completed)
            return;
        mRemainTime = endtime - GameTimer.inst.serverNow;
        mRemainTime = Mathf.Clamp((float)mRemainTime, 0, (float)mRemainTime);
        var v = Mathf.Clamp01(1 - ((float)mRemainTime / mTotalTime));
        if (mRemainTime <= 0)
        {
            clearTimer();
            setFinished();
        }
        else
        {
            refreshProgressBar(v);
            txt_time.text = TimeUtils.timeSpanStrip((int)mRemainTime);
        }
    }

    void refreshProgressBar(float v)
    {
        if (pb_content == null)
        {
            clearTimer();
            return;
        }
        pb_content.transform.localScale = new Vector3(v, 1, 1);
    }

    void clearTimer()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    void OnDestroy()
    {
        clearTimer();
    }
    public int getRemainTime()
    {
        return (int)mRemainTime;
    }

    public virtual void Refresh()
    {
    }
    public void setSortingOrder(int order, bool isExtension)
    {
        var renderlist = transform.GetComponentsInChildren<SetRendererOrder>(true);
        foreach (var _render in renderlist)
        {
            _render.setOrder(order, isExtension);
        }
    }

    public void setSortingLayer(string sortingLayerName)
    {
        var renderlist = transform.GetComponentsInChildren<SetRendererOrder>(true);
        foreach (var _render in renderlist)
        {
            _render.setLayer(sortingLayerName);
        }
    }

    public void OnPointerClick(Vector3 mousepos)
    {
        if (!IndoorMap.inst.CanChangeSelectEntity()) return;

        //
        if (type == 0)
        {
            if (completed)
            {
                if (IndoorMapEditSys.inst.currEntityUid != FUid && IndoorMapEditSys.inst.currEntityUid != -1)
                {
                    if (IndoorMap.inst.GetFurnituresByUid(IndoorMapEditSys.inst.currEntityUid, out Furniture furnitureEntity))
                    {
                        furnitureEntity.UnSelected();
                    }

                    if (IndoorMap.inst.GetFurnituresByUid(FUid, out Furniture curFurniture))
                    {
                        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.PICK_ITEM, FUid);

                        curFurniture.OnSelected();
                    }
                }

                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, FUid);
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHELFUPGRADINGUI, UserDataProxy.inst.GetFuriture(FUid));
            }
        }
        else
        {
            if (completed)
            {
                IndoorMapEditSys.inst.shopUpgradeFinish();
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENDINGPANEL);
            }
        }
    }

    void onAskAidBtnClick(Vector3 mousePos)
    {
        if (!IndoorMap.inst.CanChangeSelectEntity()) return;

        AudioManager.inst.PlaySound(8);
        AudioManager.inst.PlaySound(24);
        if (fakeCheck)
        {
            fakeCheck = false;
            askAidListener.gameObject.SetActive(false);
            HotfixBridge.inst.TriggerLuaEvent("Request_GuideTriggerFurnUpgrade");
            return;
        }
        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_SETHELP, type == 0 ? FUid : 99999);
        askAidListener.gameObject.SetActive(false);
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您向联盟请求了帮助"), GUIHelper.GetColorByColorHex("FFFFFF"));
    }



    //     bool mouseDown = true;
    //     void OnMouseDown()
    //     {
    // #if !UNITY_EDITOR
    //         if (Input.touchCount <= 0) return;
    // #endif
    //         if (GUIHelper.isPointerOnUI()) return;
    //         mouseDown = true;
    //     }
    //     void OnMouseExit()
    //     {
    //         mouseDown = false;
    //     }
    //     void OnMouseUp()
    //     {
    //         if (mouseDown)
    //         {
    //             mouseDown = false;
    //             OnPointerClick();
    //         }
    //     }
}