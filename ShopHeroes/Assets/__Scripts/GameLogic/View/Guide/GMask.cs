using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GMask : MaskableGraphic, ICanvasRaycastFilter
{
    private RectTransform inner_trans;
    private RectTransform outer_trans;//背景区域

    private Vector2 inner_rt;//镂空区域的右上角坐标
    private Vector2 inner_lb;//镂空区域的左下角坐标
    private Vector2 outer_rt;//背景区域的右上角坐标
    private Vector2 outer_lb;//背景区域的左下角坐标

    [Header("是否实时刷新")]
    [Space(25)]
    public bool realtimeRefresh;

    public GameObject guidePrefab;
    public GuideDust dustObj;
    public GameObject preMask;
    private GameObject inputEventListenerObj;

    public bool canTiming = false;
    public float cumulativeTime = 0;
    public int clickSum = 0;
    public int refreshClickSum = 0;
    public bool isReRefresh;

    protected override void Awake()
    {
        base.Awake();

        outer_trans = GetComponent<RectTransform>();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (inner_trans == null)
        {
            base.OnPopulateMesh(vh);
            return;
        }

        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        //0 outer左下角
        vertex.position = new Vector3(outer_lb.x, outer_lb.y);
        vh.AddVert(vertex);
        //1 outer左上角
        vertex.position = new Vector3(outer_lb.x, outer_rt.y);
        vh.AddVert(vertex);
        //2 outer右上角
        vertex.position = new Vector3(outer_rt.x, outer_rt.y);
        vh.AddVert(vertex);
        //3 outer右下角
        vertex.position = new Vector3(outer_rt.x, outer_lb.y);
        vh.AddVert(vertex);
        //4 inner左下角
        vertex.position = new Vector3(inner_lb.x, inner_lb.y);
        vh.AddVert(vertex);
        //5 inner左上角
        vertex.position = new Vector3(inner_lb.x, inner_rt.y);
        vh.AddVert(vertex);
        //6 inner右上角
        vertex.position = new Vector3(inner_rt.x, inner_rt.y);
        vh.AddVert(vertex);
        //7 inner右下角
        vertex.position = new Vector3(inner_rt.x, inner_lb.y);
        vh.AddVert(vertex);

        //绘制三角形
        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(1, 4, 5);
        vh.AddTriangle(1, 5, 2);
        vh.AddTriangle(2, 5, 6);
        vh.AddTriangle(2, 6, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(4, 7, 3);
        vh.AddTriangle(0, 4, 3);
    }

    /// <summary>
    /// 过滤掉射线检测
    /// </summary>
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        if (inner_trans == null)
        {
            return true;
        }
        return !RectTransformUtility.RectangleContainsScreenPoint(inner_trans, screenPos, eventCamera);
    }

    /// <summary>
    /// 计算边界
    /// </summary>
    private void CalcBounds()
    {
        if (inner_trans == null)
        {
            return;
        }
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(outer_trans, inner_trans);
        inner_rt = bounds.max;
        inner_lb = bounds.min;
        outer_rt = outer_trans.rect.max;
        outer_lb = outer_trans.rect.min;
    }
    float delayTime;
    int animTimerId = 0;
    int preBtnEventTimerId = 0;

    public void showGMask()
    {
        var cfg = GuideDataProxy.inst.CurInfo;
        isReRefresh = false;
        if (cfg.m_curCfg == null) return;
        color = new Color(0, 0, 0, cfg.m_curCfg.mask_color == 0 ? 0 : cfg.m_curCfg.mask_color / 100.0f);
        if (cfg.m_curCfg.btn_name != "0")
        {
            preMask.SetActive(true);
            var curWindow = GUIManager.CurrWindow;
            delayTime = 0.1f;
            if (cfg.m_curCfg == null) return;
            preBtnEventTimerId = GameTimer.inst.AddTimer(delayTime, () =>
             {
                 if (cfg.m_curCfg == null)
                 {
                     GameTimer.inst.RemoveTimer(preBtnEventTimerId);
                     preBtnEventTimerId = 0;
                 }

                 if (cfg != null && cfg.m_curCfg != null && cfg.m_curCfg.btn_name != null)
                 {
                     if (cfg.m_curCfg.btn_name.Contains("-"))
                     {
                         var btn_names = cfg.m_curCfg.btn_name.Split('-');
                         GameObject panel = FGUI.inst.uiRootTF.Find(cfg.m_curCfg.btn_view) != null ? FGUI.inst.uiRootTF.Find(cfg.m_curCfg.btn_view).gameObject : null;
                         if (panel != null)
                         {
                             for (int i = 0; i < btn_names.Length; i++)
                             {
                                 panel = panel.FindHideChildGameObject(btn_names[i]);
                                 if (panel == null)
                                 {
                                     Logger.error("没有找到按钮名称是" + cfg.m_curCfg.btn_name + "的按钮");
                                     return;
                                 }
                             }

                             inner_trans = panel.GetComponent<RectTransform>();


                             var eventTriggerListener = inner_trans.GetComponent<EventTriggerListener>();
                             if (eventTriggerListener != null)
                             {
                                 eventTriggerListener.onDown += itemOnPointerDown;
                                 eventTriggerListener.onUp += itemOnPointerUp;
                                 eventTriggerListener.onDrag += itemOnDrag;
                             }
                             else
                             {
                                 Button targetBtn = inner_trans.GetComponent<Button>();
                                 if (targetBtn != null)
                                 {
                                     targetBtn.onClick.AddListener(clickToNext);
                                 }
                             }

                             GameTimer.inst.RemoveTimer(preBtnEventTimerId);
                             preBtnEventTimerId = 0;

                             wakou(cfg, 0.1f);
                         }
                     }
                     else
                     {
                         GameObject panel = FGUI.inst.uiRootTF.Find(cfg.m_curCfg.btn_view) != null ? FGUI.inst.uiRootTF.Find(cfg.m_curCfg.btn_view).gameObject : null;
                         if (panel != null)
                         {
                             if (cfg.m_curCfg.btn_name == "1" && ManagerBinder.inst.mGameState == kGameState.Shop && !ManagerBinder.inst.stateIsChanging)
                             {
                                 EventController.inst.TriggerEvent(GameEventType.EquipEvent.SET_GUIDETARGET);
                             }
                             else
                             {
                                 if (panel.FindHideChildGameObject(cfg.m_curCfg.btn_name) == null) return;
                                 var tempRect = panel.FindHideChildGameObject(cfg.m_curCfg.btn_name).GetComponent<RectTransform>();
                                 inner_trans = tempRect;

                                 var eventTriggerListener = inner_trans.GetComponent<EventTriggerListener>();
                                 if (eventTriggerListener != null)
                                 {
                                     eventTriggerListener.onDown += itemOnPointerDown;
                                     eventTriggerListener.onUp += itemOnPointerUp;
                                     eventTriggerListener.onDrag += itemOnDrag;
                                 }
                                 else
                                 {
                                     Button targetBtn = inner_trans.GetComponent<Button>();
                                     if (targetBtn != null)
                                     {
                                         targetBtn.onClick.AddListener(clickToNext);
                                     }
                                 }
                                 //Logger.error("targetBtnname = " + targetBtn.name + "inner trans" + inner_trans.name);
                                 if (inner_trans.name == "btn_done")
                                 {
                                     D2DragCamera.inst.RestoreOrthgraphicSize();
                                 }
                                 wakou(cfg, 0.1f);
                             }
                             GameTimer.inst.RemoveTimer(preBtnEventTimerId);
                             preBtnEventTimerId = 0;
                         }
                     }
                 }
             });
        }
    }

    private void wakou(GuideInfo cfg, float delayTime)
    {
        if (cfg.m_curCfg == null) return;
        GameTimer.inst.AddTimer(delayTime, 1, () =>
        {
            canTiming = true;
            cumulativeTime = 0;
            clickSum = 0;
            if (cfg == null || cfg.m_curCfg == null || cfg.m_curCfg.btn_name == null) return;
            if (cfg.m_curCfg.btn_name.Contains("-"))
            {
                if ((K_Guide_Type)cfg.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, inner_trans.transform, 140, false);
                gameObject.SetActiveTrue();
                CalcBounds();
                FGUI.inst.setGlobalMaskActice(false);
                preMask.SetActive(false);
                isReRefresh = true;
            }
            else
            {
                FGUI.inst.setGlobalMaskActice(false);
                if (inner_trans == null) return;
                if (!inner_trans.gameObject.activeInHierarchy)
                {
                    if (animTimerId > 0)
                    {
                        GameTimer.inst.RemoveTimer(animTimerId);
                        animTimerId = 0;
                    }

                    animTimerId = GameTimer.inst.AddTimer(0.5f, () =>
                    {
                        
                        if (inner_trans == null)
                        {
                            if ((K_Guide_Type)cfg.m_curCfg.guide_type != K_Guide_Type.RestrictClick && (K_Guide_Type)cfg.m_curCfg.guide_type != K_Guide_Type.TipsAndRestrictClick)
                            {
                                GameTimer.inst.RemoveTimer(animTimerId);
                                animTimerId = 0;
                                return;
                            }
                            GameObject panel = FGUI.inst.uiRootTF.Find(cfg.m_curCfg.btn_view) != null ? FGUI.inst.uiRootTF.Find(cfg.m_curCfg.btn_view).gameObject : null;
                            inner_trans = panel.FindHideChildGameObject(cfg.m_curCfg.btn_name).GetComponent<RectTransform>();
                            Button targetBtn = inner_trans.GetComponent<Button>();
                            if (targetBtn != null)
                            {
                                targetBtn.onClick.AddListener(clickToNext);
                            }
                        }
                        if (inner_trans != null && inner_trans.gameObject.activeInHierarchy)
                        {
                            if ((K_Guide_Type)cfg.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
                            isReRefresh = true;
                            if(this != null)
                            {
                                preMask.SetActive(false);
                                gameObject.SetActiveTrue();
                            }
                            
                            CalcBounds();
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, inner_trans.transform, 140, false);
                            GameTimer.inst.RemoveTimer(animTimerId);
                            animTimerId = 0;
                        }
                    });
                }
                else
                {
                    isReRefresh = true;
                    if (cfg.m_curCfg.btn_name == "1")
                    {
                        //GameTimer.inst.AddTimer(1.5f, 1, () =>
                        //  {
                        //      isReRefresh = false;
                        //  });
                    }
                    if ((K_Guide_Type)cfg.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                        EventController.inst.TriggerEvent(GameEventType.GuideEvent.SHOWGUIDEUI, K_Guide_UI.GTips);
                    preMask.SetActive(false);
                    gameObject.SetActiveTrue();
                    CalcBounds();
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, inner_trans.transform, 140, false);
                    if (cfg.m_curCfg.btn_name == "1")
                    {
                        if (ManagerBinder.inst.mGameState == kGameState.Shop)
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETPROMPTPOS, inner_trans.transform, true);
                    }
                }
            }
        });
    }

    public void setSlotTarget(GameObject target)
    {
        if (target == null) return;
        GuideDataProxy.inst.curMakeSlotName = target.name;
        inner_trans = target.GetComponent<RectTransform>();
        Button targetBtn = inner_trans.GetComponent<Button>();
        targetBtn.onClick.AddListener(clickToNext);
        wakou(GuideDataProxy.inst.CurInfo, 0.1f);
    }

    InputEventListener shopperTarget;
    public void showGMaskShopper()
    {
        var shopperDatas = ShopperDataProxy.inst.GetShopperList();
        for (int i = 0; i < shopperDatas.Count; i++)
        {
            if (shopperDatas[i].data.targetEquipId == int.Parse(GuideDataProxy.inst.CurInfo.m_curCfg.conditon_param_1) && shopperDatas[i].data.shopperState == 99)
            {
                //var target = IndoorMapEditSys.inst.GetShopperActor(shoppers[i].data.shopperUid);
                //var eventListener = target.GetComponent<InputEventListener>();
                var target = IndoorRoleSystem.inst.GetShopperByUid(shopperDatas[i].data.shopperUid);
                var eventListener = target.Attacher.mouseListener;


                shopperTarget = eventListener;
                if (eventListener != null)
                    eventListener.OnClick += shopperClickToNext;
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, target.Attacher.sp_bgIcon.transform /*target.actorAttacher.sp_bg.transform*/, 150, false);
                preMask.SetActive(false);
                break;
            }
        }
    }

    private void shopperClickToNext(Vector3 v3)
    {
        EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
        GuideDataProxy.inst.CurInfo.isClickTarget = true;
        shopperTarget.OnClick -= shopperClickToNext;
        GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
    }

    GameObject currSelectItemDown;
    private void itemOnPointerDown(GameObject go)
    {
        currSelectItemDown = go;
    }

    private void itemOnPointerUp(GameObject go)
    {
        if (currSelectItemDown == go)
        {
            clickToNext(go);
            currSelectItemDown = null;
        }
    }

    void itemOnDrag(GameObject go, Vector2 data)
    {
        if (currSelectItemDown != null)
        {
            currSelectItemDown = null;
        }
    }

    private void clickToNext(GameObject go)
    {
        GuideDataProxy.inst.CurInfo.isClickTarget = true;
        EventTriggerListener eventTriggerListener = new EventTriggerListener();
        //if (inner_trans != null)
        eventTriggerListener = inner_trans.GetComponent<EventTriggerListener>();
        //eventTriggerListener.onClick -= clickToNext;
        eventTriggerListener.onDown -= itemOnPointerDown;
        eventTriggerListener.onUp -= itemOnPointerUp;
        GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
    }

    private void clickToNext()
    {
        if (inner_trans != null)
        {
            if (null != inner_trans.GetComponent<MakeSlot>())
            {
                var tempSlot = inner_trans.GetComponent<MakeSlot>();
                if (tempSlot.makeState == 1) return;
                GuideManager.inst.CurrEquipId = tempSlot.CurrEquipId == 0 ? -1 : tempSlot.CurrEquipId;
                GuideManager.inst.curTargetBtn = inner_trans.gameObject;
                GuideManager.inst.curFunc = clickToNext;
                return;
            }
            GuideDataProxy.inst.CurInfo.isClickTarget = true;
            var btn = inner_trans.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveListener(clickToNext);
            }
            if (GuideDataProxy.inst.CurInfo.m_curCfg.btn_name == "1")
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETPROMPTPOS, inner_trans.transform, false);
            }
            GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
        }
    }

    public void showGWhiteMask()
    {
        IndoorMap.inst.indoorMask.SetActiveTrue();
        var cfg = GuideDataProxy.inst.CurInfo;
        if (cfg == null || cfg.m_curCfg == null)
        {
            Logger.error("修复家具引导 cfg数据是空");
            return;
        }
        int furnitureUId = 0;

        if (cfg.m_curCfg.conditon_param_1 != null)
        {
            furnitureUId = int.Parse(cfg.m_curCfg.conditon_param_1);
        }

        if (furnitureUId == 0)
        {
            Logger.error("引导修复家具 当前步骤id是" + cfg.m_curCfg.id);
        }
        IndoorMap.inst.GetFurnituresByUid(furnitureUId, out parentFurn);
        if (parentFurn == null)
        {
            Logger.error("引导修复家具 没有找到uid是" + furnitureUId + "的家具");
            skipThisGuide(cfg);
            return;
        }
        if (curFurn != null)
        {
            Destroy(curFurn.gameObject);
            curFurn = null;
        }
        curFurn = GameObject.Instantiate(parentFurn.gameObject).GetComponent<Furniture>();
        if (curFurn == null)
        {
            Logger.error("引导修复家具 克隆的对象没有取到Furniture组件");
            skipThisGuide(cfg);
            return;
        }
        spider = curFurn./*transform.GetChild(3).*/GetComponentInChildren<SpiderWeb>();
        parentFurn.gameObject.SetActiveFalse();
        parentSpider = parentFurn./*transform.GetChild(3).*/GetComponentInChildren<SpiderWeb>();
        curFurn.gameObject.ChangeLayer(15);
        curFurn.transform.position = new Vector3(parentFurn.transform.position.x, parentFurn.transform.position.y, -2);
        if (IndoorMap.inst == null || IndoorMap.inst.indoorMask == null)
        {
            Logger.error("引导修复家具 当前场景indoormap.inst是空");
            skipThisGuide(cfg);
            return;
        }
        curFurn.transform.SetParent(IndoorMap.inst.indoorMask.transform);
        var renders = curFurn.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].sortingLayerName = "map_Actor";
        }
        curFurn.name = "我是克隆的";
        var eventListener = curFurn./*transform.GetChild(3).*/GetComponentInChildren<InputEventListener>();
        if (eventListener != null)
        {
            var cloneObjTrans = eventListener.transform;
            cloneObjTrans.localPosition = new Vector3(cloneObjTrans.localPosition.x, cloneObjTrans.localPosition.y, -2);
            eventListener.OnClick = cloneObjClickEvent;
        }
        else
        {
            skipThisGuide(cfg);
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, curFurn.transform, 150, false);
    }

    private void skipThisGuide(GuideInfo cfg)
    {
        GuideDataProxy.inst.CurInfo.val = cfg.m_curCfg.end_param;
        GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickCountArrive, 0);
    }

    GuideAttacher guideAttacher;
    Furniture curFurn;
    Furniture parentFurn;
    SpiderWeb spider;
    SpiderWeb parentSpider;
    private void cloneObjClickEvent(Vector3 pos)
    {
        AudioManager.inst.PlaySound(64);
        GuideDataProxy.inst.CurInfo.val++;

        if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.m_curCfg != null && (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type != K_Guide_Type.ClickUnlockFurn)
        {
            if (curFurn != null)
            {
                Destroy(curFurn.gameObject);
                curFurn = null;
            }
        }

        if (curFurn == null)
        {
            skipThisGuide(GuideDataProxy.inst.CurInfo);
            return;
        }

        if (guideAttacher == null)
        {
            if (curFurn != null && curFurn.uid == 70001)
            {
                curFurn.PopUIRoot.position += new Vector3(0.24f, 0, 0);
            }
            guideAttacher = GameObject.Instantiate(guidePrefab, curFurn.PopUIRoot).GetComponent<GuideAttacher>();
            if (guideAttacher == null)
            {
                skipThisGuide(GuideDataProxy.inst.CurInfo);
                if (curFurn != null)
                    Destroy(curFurn.gameObject);
                curFurn = null;
                return;
            }
            guideAttacher.setSchedule(GuideDataProxy.inst.CurInfo.val, GuideDataProxy.inst.CurInfo.m_curCfg.end_param);
            curFurn.transform.DOScaleY(0.0009f, 0.1f).From(0.001f).OnComplete(() =>
            {
                if (curFurn != null)
                    curFurn.transform.DOScaleY(0.001f, 0.1f).From(0.0009f);
            });
            dustObj.transform.SetParent(curFurn.transform, false);
            dustObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            dustObj.gameObject.SetActive(true);
            dustObj.PlayDust();
            dustObj.setDustPos(curFurn.uid);

            if (GuideDataProxy.inst.CurInfo.val >= GuideDataProxy.inst.CurInfo.m_curCfg.end_param)
            {
                if (guideAttacher != null)
                    guideAttacher.gameObject.SetActiveFalse();
                if (parentFurn != null)
                    parentFurn.gameObject.SetActiveTrue();
                dustObj.transform.SetParent(transform, false);
                dustObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, 9999);
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
                if (curFurn != null)
                    Destroy(curFurn.gameObject);
                curFurn = null;
                GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickCountArrive, 0);
            }
        }
        else
        {
            dustObj.PlayDust();
            if (GuideDataProxy.inst.CurInfo.val < GuideDataProxy.inst.CurInfo.m_curCfg.end_param)
            {
                curFurn.transform.DOScaleY(0.0009f, 0.1f).From(0.001f).OnComplete(() =>
                {
                    if (curFurn != null)
                        curFurn.transform.DOScaleY(0.001f, 0.1f).From(0.0009f);
                });
                guideAttacher.setSchedule(GuideDataProxy.inst.CurInfo.val, GuideDataProxy.inst.CurInfo.m_curCfg.end_param);
            }
            else
            {
                guideAttacher.setSchedule(GuideDataProxy.inst.CurInfo.val, GuideDataProxy.inst.CurInfo.m_curCfg.end_param);
                curFurn.transform.DOScaleY(0.0009f, 0.1f).From(0.001f).OnComplete(() =>
                {
                    if (curFurn == null)
                    {
                        skipThisGuide(GuideDataProxy.inst.CurInfo);
                        return;
                    }
                    curFurn.transform.DOScaleY(0.001f, 0.1f).From(0.0009f).OnComplete(() =>
                    {
                        //IndoorMap.inst.indoorMask.SetActiveFalse();
                        if (guideAttacher != null)
                            guideAttacher.gameObject.SetActiveFalse();
                        if (parentFurn != null)
                            parentFurn.gameObject.SetActiveTrue();
                        dustObj.transform.SetParent(transform, false);
                        dustObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, 9999);
                        EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
                        if (curFurn != null)
                            Destroy(curFurn.gameObject);
                        curFurn = null;
                        GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickCountArrive, 0);
                    });
                });
            }
        }

        if (spider != null)
            spider.setSpiderClear(GuideDataProxy.inst.CurInfo.val);
        if (parentSpider != null)
            parentSpider.setSpiderClear(GuideDataProxy.inst.CurInfo.val);
    }

    public void hideGMask()
    {
        gameObject.SetActiveFalse();
        if (inner_trans != null)
        {
            inner_trans = null;
        }
    }

    float intervalTime = 0;
    public float reSetactiveTime = 0;
    private void Update()
    {
        if (canTiming)
        {
            cumulativeTime += Time.deltaTime;
            reSetactiveTime += Time.deltaTime;
            if (isReRefresh)
            {
                if (realtimeRefresh == false)
                {
                    return;
                }

                if (reSetactiveTime >= 2)
                {
                    //gameObject.SetActive(false);
                    //gameObject.SetActive(true);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform as RectTransform);
                    reSetactiveTime = 0;
                }
                //计算边界
                CalcBounds();
                //刷新
                SetAllDirty();
                if (inner_trans == null)
                {
                    isReRefresh = false;
                }
                else
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, inner_trans.transform, 140, false);
            }

            if (cumulativeTime >= 5)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (clickSum == 0)
                    {
                        clickSum++;
                        intervalTime = Time.time;
                    }
                    else
                    {
                        if (Time.time - intervalTime <= 0.4f)
                        {
                            clickSum++;
                        }
                        if (Time.time - intervalTime > 0.5f)
                        {
                            clickSum = 0;
                        }
                        intervalTime = Time.time;
                        if (clickSum >= 10)
                        {
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SKIPGUIDE);
                            clickSum = 0;
                        }
                    }
                }
            }
        }
    }
}
