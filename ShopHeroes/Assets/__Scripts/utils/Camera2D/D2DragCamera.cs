using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class D2DragCamera : SingletonMono<D2DragCamera>
{
    public new Camera camera;
    public bool dragEnabled = true;

    [Range(-2, 2)]
    public float dragSpeed = -0.06f;

    [Range(.3f, 10f)]
    public float _minZoom = 3f;
    private float _maxZoom = 8;

    public float minZoom
    {
        get
        {
            _minZoom = (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1) * 3f;
            return _minZoom;
        }
    }
    public float maxZoom
    {
        get
        {
            return _maxZoom * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1);
        }
        set
        {
            if (_maxZoom != value)
            {
                _maxZoom = value;
                currentDistance = targetDistance = _maxZoom * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1);
                // camera.orthographicSize = currentDistance;
            }
        }
    }
    public void OnSceneRotate()
    {
        if (ManagerBinder.inst == null) return;
        camera.orthographicSize = minZoom;
        if (ManagerBinder.inst.mGameState == kGameState.Town)
        {
            updateCameMaxZoom(0, kCameraMoveType.citySecene);
        }
        else if (ManagerBinder.inst.mGameState == kGameState.Shop)
        {
            updateCameMaxZoom(UserDataProxy.inst.shopData.shopLevel);
        }
        currentDistance = maxZoom; //切换屏幕后 将高度设为最高
        setCameraDOOrthoSizeTween(maxZoom, 0.2f);
    }
    public void updateCameMaxZoom(int shoplevel = 0, kCameraMoveType type = kCameraMoveType.shopExtend)
    {
        if (CameraMoveConfigManager.inst != null)
        {
            var cfg = CameraMoveConfigManager.inst.GetConfg(type, shoplevel);
            maxZoom = cfg.zoom1 / 100f;
        }
    }
    [Range(0.1f, 10f)]
    public float zoomStepSize = 0.3f;
    CameraBounds cameraBounds;
    public bool isMoving;
    public bool npcIsMoving;
    public Volume volume;
    Bloom bloom;

    public void SetVolumeBloomFloat(float threshold, float intensity, float scatter)
    {

        if (volume != null)
        {
            if (bloom == null)
            {
                volume.profile.TryGet<Bloom>(out bloom);
            }

            if (bloom != null)
            {
                bloom.threshold.overrideState = true;
                bloom.threshold.value = threshold;

                bloom.intensity.overrideState = true;
                bloom.intensity.value = intensity;

                bloom.scatter.overrideState = true;
                bloom.scatter.value = scatter;
            }
        }
    }

    public bool beForcedFalse = false;
    public void SetVolumeBloomActive(bool active, bool isCombat = false)
    {
        if (beForcedFalse) return;

        if (volume != null)
        {
            if (bloom == null)
            {
                volume.profile.TryGet<Bloom>(out bloom);
            }

            if (bloom != null)
            {
                if (isCombat)
                {
                    WorldParConfig worldParConfig = WorldParConfigManager.inst.GetConfig(8205);
                    if (worldParConfig != null && worldParConfig.parameters == 2)
                    {
                        bloom.active = active;
                    }
                }
                else
                {
                    bloom.active = active;
                }
            }
        }
    }

    void Start()
    {
        camera = Camera.main;
    }
    void Update()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        if (cameraBounds == null)
        {
            // if (ManagerBinder.inst.mStateMgr.mCurState.getState() == kGameState.Shop)
            if (ManagerBinder.inst.mGameState == kGameState.Shop)
            {
                if (IndoorMap.inst != null)
                {
                    cameraBounds = IndoorMap.inst.cameraBounds;
                }
            }
            else if (ManagerBinder.inst.mGameState == kGameState.Town)
            {
                if (CityMap.inst != null)
                {
                    cameraBounds = CityMap.inst.cameraBounds;
                }
            }
            else if (ManagerBinder.inst.mGameState == kGameState.VisitShop)
            {
                if (GameObject.Find("CameraBounds") != null)
                    cameraBounds = GameObject.Find("CameraBounds").GetComponent<CameraBounds>();
            }
            else if (ManagerBinder.inst.mGameState == kGameState.Union)
            {
                if (UnionMap.inst != null)
                {
                    cameraBounds = UnionMap.inst.cameraBounds;
                }
            }
            else if (ManagerBinder.inst.mGameState == kGameState.Ruins)
            {
                if (RuinsMap.inst != null)
                {
                    cameraBounds = RuinsMap.inst.cameraBounds;
                }
            }
        }
    }

    //操作
    [Range(0, 10)]
    public float damper = 2f;
    [Range(0, 100)]
    public float wheelSensitivity = 1f;
    Vector2 lastpos1;
    Vector2 lastpos2;
    Vector3 currentPan, targetPan;
    bool m_isSinleFinger;
    float targetDistance;
    float currentDistance;

    bool touchDown = false;

    Vector3 lastPosition;
    float lastOrthographicSize;
    void panControl()
    {
#if !UNITY_EDITOR
        if (Input.touchCount >= 1)
        {
            currentPan = targetPan = transform.position;
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (GUIHelper.isPointerOnUI()) return;
                    if (GUIHelper.CheckHitItem(Input.mousePosition, LayerMask.NameToLayer("Actors_Role")))
                    {
                        return;
                    }
                    touchDown = true;
                    lastpos1 = Input.GetTouch(0).position;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (!touchDown) return;
                    if (isMoving) return;
                    if (npcIsMoving) return;
                    if (onsceneChange) return;
                    if (!Helper.isPoniterInScene()) return;
                    if (IndoorMap.inst != null && IndoorMap.inst.indoorMask.activeSelf) return;
                    if (null != Physics2D.OverlapPoint(camera.ScreenToWorldPoint(Input.GetTouch(0).position)) && Physics2D.OverlapPoint(camera.ScreenToWorldPoint(Input.GetTouch(0).position)).gameObject.name == "我是修复进度条")
                    {
                        return;
                    }
                    if (null != Physics2D.OverlapPoint(camera.ScreenToWorldPoint(Input.GetTouch(0).position)) && (Physics2D.OverlapPoint(camera.ScreenToWorldPoint(Input.GetTouch(0).position)).gameObject.layer == 15 || Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)).gameObject.name == "sp_popup"))
                    {
                        return;
                    }
                    if (touchDown)
                    {
                        float xBase = 1;
                        if (FGUI.inst != null && FGUI.inst.isLandscape)
                        {
                            WorldParConfig cfg = WorldParConfigManager.inst.GetConfig(8204);
                            if (cfg != null) xBase = cfg.parameters;
                        }
                        float x, y;
                        x = Input.GetTouch(0).deltaPosition.x / 800 * camera.orthographicSize * xBase;
                        y = Input.GetTouch(0).deltaPosition.y / 800 * camera.orthographicSize;
                        targetPan.x -= x;
                        targetPan.y -= y;
                        // currentPan = targetPan;//Vector3.Lerp(currentPan, targetPan, (damper) * Time.deltaTime);
                        clearCameraTweeners();
                        transform.position = targetPan;
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    touchDown = false;
                }
                m_isSinleFinger = true;
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            if (GUIHelper.CheckHitItem(Input.mousePosition, 13))
            {
                return;
            }
            touchDown = true;

            //EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETFINGTERACTIVE, false);
            //GoOperationManager.inst.setFingerTimeReset(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchDown = false;

            //if (GUIManager.CurrWindow != null && (GUIManager.CurrWindow.viewID == ViewPrefabName.MainUI || GUIManager.CurrWindow.viewID == ViewPrefabName.CityUI))
            //{
            //    GoOperationManager.inst.setFingerTimeReset(true);
            //}
        }
        if (Input.GetMouseButton(0))
        {
            if (!touchDown) return;
            if (isMoving) return;
            if (npcIsMoving) return;
            if (onsceneChange) return;
            if (IndoorMap.inst != null && IndoorMap.inst.indoorMask.activeSelf) return;
            if (!Helper.isPoniterInScene()) return;
            if (null != Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) && Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)).gameObject.name == "我是修复进度条")
            {
                return;
            }
            if (null != Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) && (Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)).gameObject.layer == 15 || Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)).gameObject.name == "sp_popup"))
            {
                return;
            }
            float x, y;
            x = Input.GetAxis("Mouse X") * dragSpeed;
            y = Input.GetAxis("Mouse Y") * dragSpeed;
            x *= camera.orthographicSize;
            y *= camera.orthographicSize;
            // transform.Translate(x, y, 0);
            clearCameraTweeners();
            transform.position += new Vector3(x, y, 0);
        }
#endif
    }

    public void zoomControl()
    {
        if (GuideDataProxy.inst == null || GuideDataProxy.inst.CurInfo == null || !GuideDataProxy.inst.CurInfo.isAllOver || onsceneChange) return;
#if !UNITY_EDITOR
        if (Input.touchCount > 1)
        {
            currentDistance = targetDistance = camera.orthographicSize;
            if (m_isSinleFinger)
            {
                lastpos1 = Input.GetTouch(0).position;
                lastpos2 = Input.GetTouch(1).position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                var tempPos1 = Input.GetTouch(0).position;
                var tempPos2 = Input.GetTouch(1).position;

                float lastTouchDistance = Vector3.Distance(lastpos1, lastpos2);
                float currentTouchDistance = Vector3.Distance(tempPos1, tempPos2);

                targetDistance -= (currentTouchDistance - lastTouchDistance) * Time.deltaTime * wheelSensitivity;
                currentDistance = Mathf.Lerp(currentDistance, targetDistance, damper * Time.deltaTime);

                if (!isMoving && !npcIsMoving)
                {
                    camera.orthographicSize = currentDistance;

                    lastpos1 = tempPos1;
                    lastpos2 = tempPos2;
                    m_isSinleFinger = false;
                    clearCameraTweeners();
                    currentDistance = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
                }
                    clampZoom();

            }

        }
#else
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            {
                if (!isMoving && !npcIsMoving)
                {
                    camera.orthographicSize = camera.orthographicSize - zoomStepSize;
                    currentDistance = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
                    clearCameraTweeners();
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0) // back            
            {
                if (!isMoving && !npcIsMoving)
                {
                    camera.orthographicSize = camera.orthographicSize + zoomStepSize;
                    currentDistance = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
                    clearCameraTweeners();
                }
            }
            clampZoom();


        }
#endif

    }

    public bool onsceneChange = false;

    private void clampZoom()
    {
        if (onsceneChange) return;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
    }
    //边界
    Vector3 bl;
    Vector3 tr;
    private void cameraClamp()
    {
        if (onsceneChange) return;
        tr = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, -transform.position.z));
        bl = camera.ScreenToWorldPoint(new Vector3(0, 0, -transform.position.z));

        if (cameraBounds == null)
        {
            Logger.log("Clamp Camera Enabled but no Bounds has been set.");
            return;
        }

        float boundsMaxX = cameraBounds.pointa.x;
        float boundsMinX = cameraBounds.transform.position.x;
        float boundsMaxY = cameraBounds.pointa.y;
        float boundsMinY = cameraBounds.transform.position.y;

        if (tr.x > boundsMaxX)
        {
            transform.position = new Vector3(transform.position.x - (tr.x - boundsMaxX), transform.position.y, transform.position.z);
        }

        if (tr.y > boundsMaxY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (tr.y - boundsMaxY), transform.position.z);
        }

        if (bl.x < boundsMinX)
        {
            transform.position = new Vector3(transform.position.x + (boundsMinX - bl.x), transform.position.y, transform.position.z);
        }

        if (bl.y < boundsMinY)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + (boundsMinY - bl.y), transform.position.z);
        }
    }

    //
    Tweener cameraTweener;
    public void CameraChangeSceneAnim(bool far)
    {
        if (cameraTweener != null)
        {
            DOTween.Kill("canmerDO_1");
        }

        if (AccountDataProxy.inst.NeedCreatRole) { onsceneChange = false; return; }
        if (ManagerBinder.inst.mGameState == kGameState.Shop || ManagerBinder.inst.mGameState == kGameState.Town || ManagerBinder.inst.mGameState == kGameState.Union || ManagerBinder.inst.mGameState == kGameState.Ruins)
        {
            FGUI.inst.showGlobalMask(1f);
            if (ManagerBinder.inst.mGameState == kGameState.Shop && UserDataProxy.inst.shopData != null)
            {
                updateCameMaxZoom(UserDataProxy.inst.shopData.shopLevel, kCameraMoveType.shopExtend);
            }
            else
            {
                if (ManagerBinder.inst.mGameState == kGameState.Town)
                {
                    updateCameMaxZoom(0, kCameraMoveType.citySecene);
                }
                else if (ManagerBinder.inst.mGameState == kGameState.Union)
                {
                    maxZoom = 8f;
                }
                else if (ManagerBinder.inst.mGameState == kGameState.Ruins)
                {
                    maxZoom = 5.5f;
                }
            }
            if (far)
            {
                onsceneChange = true;
                // camera.orthographicSize = 8;
                float target = ManagerBinder.inst.mGameState == kGameState.Shop ? maxZoom + 2f : maxZoom;
                cameraTweener = setCameraDOOrthoSizeTween(target, 0.8f, ease: Ease.OutQuad, tweenCallback: () =>
                {
                    isMoving = false;
                    npcIsMoving = false;
                    onsceneChange = false;
                });

                cameraTweener.SetId("canmerDO_1");
            }
            else
            {
                onsceneChange = true;
                if (ManagerBinder.inst.mGameState == kGameState.Shop)
                    camera.orthographicSize = maxZoom + 2f;

                cameraTweener = setCameraDOOrthoSizeTween(maxZoom, 0.8f, ease: Ease.OutQuad, tweenCallback: () =>
                {
                    isMoving = false;
                    npcIsMoving = false;
                    onsceneChange = false;
                });

                cameraTweener.SetId("canmerDO_1");
            }
        }
    }
    public void LookToPosition(Vector3 pos, bool saveInitPos, float cameraOrthoSizeEndVal, Action compHandler = null, bool canBreak = false, Ease ea = Ease.OutSine)
    {
        if (!canBreak) isMoving = true;
        if (!saveInitPos)
        {
            lastPosition = transform.position;
            lastOrthographicSize = camera.orthographicSize;
        }

        setDOMoveTween(pos, 0.7f, ea);

        if (cameraOrthoSizeEndVal == 0)
        {
            isMoving = false;
            compHandler?.Invoke();
            return;
        }
        var setTime = camera.orthographicSize / 10.0f;

        setCameraDOOrthoSizeTween(cameraOrthoSizeEndVal, setTime, ease: ea, tweenCallback: () =>
        {
            isMoving = false;
            compHandler?.Invoke();
        });

    }

    public void LookToPosition(Vector3 pos, Action compHandler = null)
    {
        isMoving = true;

        setDOMoveTween(pos, 1f, Ease.OutSine, () =>
         {
             isMoving = false;
             compHandler?.Invoke();
         });

    }

    public void RestorePositionAndOrthgraphicSize()
    {
        isMoving = true;

        setDOMoveTween(lastPosition, 0.8f, Ease.OutCirc);
        currentDistance = Mathf.Min(currentDistance, maxZoom);
        setCameraDOOrthoSizeTween(currentDistance, 0.8f, ease: Ease.OutCirc, tweenCallback: () =>
            {
                isMoving = false;
            });
    }

    public void setCameraPositionAndSaveLastPos(Vector3 pos)
    {
        lastPosition = pos;
        camera.transform.position = pos;
    }

    public GameObject npc;
    public void SetNpc(GameObject npc, bool saveInitPos = false, float size = 4.5f)
    {
        this.npc = npc;

        if (saveInitPos)
        {
            lastPosition = transform.position;
            lastOrthographicSize = camera.orthographicSize;
        }

        //this.transform.DOMove(npc.transform.position, 0.5f).SetEase(Ease.OutSine);
        npcIsMoving = true;
        var setTime = camera.orthographicSize / 10.0f;

        setCameraDOOrthoSizeTween(size, setTime);

    }

    public void RestoreOrthgraphicSize()
    {
        var setTime = camera.orthographicSize / 10.0f;
        camera.transform.position = new Vector3(-8, 7, 0);

        setCameraDOOrthoSizeTween(maxZoom, setTime);

    }

    //public void MoveFollowNpc(Vector3 npcPos)
    //{
    //    //transform.position = Vector3.Lerp(transform.position, npcPos, Time.deltaTime * 3);
    //    this.transform.DOMove(npcPos, 0.5f);
    //}


    Tweener cameraDOOrthoSizeTweener;

    Tweener setCameraDOOrthoSizeTween(float size, float time, Ease ease = Ease.Unset, Action tweenCallback = null)
    {
        if (cameraDOOrthoSizeTweener != null)
        {
            cameraDOOrthoSizeTweener.Kill();
        }

        // minZoom = Mathf.Max(minZoom, 3f);
        size = Mathf.Max(size, minZoom);

        cameraDOOrthoSizeTweener = camera.DOOrthoSize(size, time).SetEase(ease).OnComplete(() =>
        {
            tweenCallback?.Invoke();
        });

        return cameraDOOrthoSizeTweener;
    }


    Tweener selfDOMoveTweener;

    void setDOMoveTween(Vector3 pos, float time, Ease ease = Ease.Unset, Action tweenCallback = null)
    {
        if (selfDOMoveTweener != null)
        {
            selfDOMoveTweener.Kill();
        }

        selfDOMoveTweener = this.transform.DOMove(pos, time).SetEase(ease).OnComplete(() =>
        {
            tweenCallback?.Invoke();
        });

    }

    void clearCameraTweeners()
    {

        if (cameraDOOrthoSizeTweener != null)
        {
            cameraDOOrthoSizeTweener.Kill();
        }

        if (selfDOMoveTweener != null)
        {
            selfDOMoveTweener.Kill();
        }

    }

    void LateUpdate()
    {
        if (npcIsMoving)
        {
            if (npc == null) { npcIsMoving = false; return; }
            transform.position = npc.transform.position;
            return;
        }

        if (cameraBounds == null) return;
#if UNITY_EDITOR
        if (GUIHelper.isPointerOnUI()) return;
#else
        if (Input.touchCount >= 1 && GUIHelper.isPointerOnUI()) return;
#endif
        if (IndoorMap.inst.isFurnitureOccupyDrag) return;

        if (IndoorMapEditSys.inst != null && (IndoorMapEditSys.inst.shopDesignMode == DesignMode.FloorEdit || IndoorMapEditSys.inst.shopDesignMode == DesignMode.WallEdit || IndoorMapEditSys.inst.shopDesignMode == DesignMode.LookPetHouse))
        {
            return;
        }

        if (isMoving) return;
        if (npcIsMoving) return;
        if (dragEnabled)
        {
            panControl();
        }
        cameraClamp();
        zoomControl();
    }
}
