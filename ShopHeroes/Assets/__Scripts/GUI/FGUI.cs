using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Spine.Unity;
using DG.Tweening;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
public class FGUI : SingletonMono<FGUI>
{
    public RectTransform uiRootTF;
    public RectTransform uiHideRootTF;
    public Camera uiCamera;
    public RectTransform hudPlanel;
    public Transform vfxPlanel;
    public Image holdTopTf;
    public Animator sceneExcess;
    public Image img_blackMask;
    public GameObject obj_loopJumpAnim;
    public Text tx_loopJumpBottom;
    public GameObject HealthTip;
    public Follow2DTarget buildingHUD;
    public Follow2DTarget workerRecruitHUD;
    public Follow2DTarget scienceLabUnlockHUD;
    public Follow2DTarget unionAidHUD;
    public Follow2DTarget ruinsHUD;
    public Camera rendererTextureCamera;
    public Transform photoPoint;
    public Transform startvideotf;
    public AudioSource videoMusic;
    public GameObject loginStaticImgObj;
    public GameObject loginanimObj_en;
    public GameObject loginanimObj;
    public GameObject loginanimObj_justFont;
    public Image globalMask;
    public Canvas uiRootCanvas;
    public CanvasGroup uiRootCanvasGroup;
    public NetworkErrorPanel networkErrorPanel;
    public PrivacyPanel privacyPanel;
    public Transform HudCanvas;
    public GameLoadingTips gameLoadingTips;
    public GameObject wifiObj;
    public Image wifiImg;

    public Button sdkLoginBtn;

    //是否横屏
    public bool isLandscape { get; set; }

    //flytf
    public RectTransform unionCoinTargetTf_L;
    public RectTransform energyTargetTf_L;
    public RectTransform goldFlyTargetTf_L;
    public RectTransform gemFlyTargetTf_L;

    public RectTransform unionCoinTargetTf_;
    public RectTransform energyTargetTf_;
    public RectTransform goldFlyTargetTf_;
    public RectTransform gemFlyTargetTf_;

    public GameObject golbalAwaitMask;

    public RectTransform unionCoinTargetTf
    {
        get
        {
            return isLandscape ? unionCoinTargetTf_L : unionCoinTargetTf_;
        }
    }

    public RectTransform energyTargetTf
    {
        get
        {
            return isLandscape ? energyTargetTf_L : energyTargetTf_;
        }
    }
    public RectTransform goldFlyTargetTf
    {
        get
        {
            return isLandscape ? goldFlyTargetTf_L : goldFlyTargetTf_;
        }
    }
    public RectTransform gemFlyTargetTf
    {
        get
        {
            return isLandscape ? gemFlyTargetTf_L : gemFlyTargetTf_;
        }
    }

    // public CameraTextureBG guiBgVFX;
    [Header("英雄头像换装缓存位置")]
    public Transform heroGraphicCacheParent;

    public Transform cityHudCanvas;

    public float bangsize = 0;

    private void Start()
    {
        sdkLoginBtn.onClick.AddListener(() =>
        {
            PlatformManager.inst.LoginSdk();
        });
    }

    public override void init()
    {
        base.init();

        var _safeArea = Screen.safeArea;
        bangsize = Mathf.Min((float)_safeArea.y, 50f);
        //
        networkErrorPanel.hide();
        privacyPanel.hidePrivacyPanel();
        wifiObj.SetActive(false);

        heights = new int[] { 1136, 1334, 1792, 1920, 2340, 2688 };

        screenAR = Screen.width / Screen.height;

        int _orientation = SaveManager.inst.GetInt("screenorientation", false); //1为横 0为竖
        RotateScreen(_orientation == 1);


        closecheckFPS = true;
        int arrvalue = SaveManager.inst.GetInt("arrow");
        if (arrvalue > 0 && arrvalue <= heights.Length)
        {
            arrow = arrvalue;
        }
        else
        {
            //float len = isLandscape ? Screen.width : Screen.height;
            // float min = 9999999;
            // int minIndex = 0;
            // for (int i = 0; i < heights.Length; i++)
            // {
            //     if (Mathf.Abs(heights[i] - len) < min)
            //     {
            //         min = Mathf.Abs(heights[i] - len);
            //         minIndex = i;
            //     }
            // }
            var h = Mathf.Max(Screen.height, Screen.width);
            if (h > StaticConstants.designSceneSize.y)
            {
                arrow += 1;
            }
            else
            {
                arrow = 4;
            }

        }

        //界面解锁
        Logger.log("Screen   w:" + Screen.width);

    }

    Tween wifiTween;
    public void setWifiObj(bool isShow, float showTime = 0.3f)
    {
        if (wifiTween != null)
        {
            wifiTween.Kill(true);
        }

        wifiObj.SetActive(isShow);

        if (isShow)
            wifiTween = wifiImg.DOFade(0, showTime).From(1).SetLoops(-1, LoopType.Yoyo);
    }

    GameObject loginbgObj;
    GameObject loginFontObj;
    bool loginBgFinish = false;
    bool loginFontFinish = false;
    public void SetLoginBGVisible(bool visible)
    {
        if (!visible)
        {

            if (loginbgObj != null)
            {
                GameObject.Destroy(loginbgObj);
            }

            if (loginFontObj != null)
            {
                GameObject.Destroy(loginFontObj);
            }

            //if (loginStaticImgObj != null)
            //{
            //    loginStaticImgObj.SetActive(false);
            //}

        }
        else
        {
            //if (loginbgObj == null)
            //{
            //    loginbgObj = GameObject.Instantiate(loginanimObj, Vector2.zero, Quaternion.identity, startvideotf); //GameObject.Instantiate(LanguageManager.inst.curType == LanguageType.ENGLISH ? loginanimObj_en : loginanimObj, Vector2.zero, Quaternion.identity, startvideotf);
            //    //loginbgObj.SetActive(false);
            //}

            //if (loginFontObj == null)
            //{
            //    loginFontObj = GameObject.Instantiate(loginanimObj_justFont, Vector2.zero, Quaternion.identity, startvideotf); //GameObject.Instantiate(LanguageManager.inst.curType == LanguageType.ENGLISH ? loginanimObj_en : loginanimObj, Vector2.zero, Quaternion.identity, startvideotf);
            //    //loginbgObj.SetActive(false);
            //}

            RefreshLoginBG();
            //SetLoginInfo();

            //if (loginStaticImgObj != null)
            //{
            //    loginStaticImgObj.SetActive(true);
            //}

            // if (Screen.orientation == ScreenOrientation.Portrait)
            // {

            //}
        }
    }

    public void RefreshLoginBG()
    {
        loginBgFinish = false;
        loginFontFinish = false;


        int loginBgVer = PlayerPrefs.GetInt("loginBgVer", -1);
        string loginAssetName = "-1";
        if (loginBgVer == -1)
        {
            loginAssetName = "2";
        }
        else
        {
            loginAssetName = loginBgVer.ToString();
            if (ManagerBinder.inst.mGameState == kGameState.Login)
            {
                string wdpAseetName = WorldParConfigManager.inst.GetConfig(10000).parameters.ToString();
                if (loginAssetName != wdpAseetName)
                {
                    loginAssetName = wdpAseetName;

                }
            }
        }
        var aop = Addressables.InstantiateAsync("SkeletonLogin_BG" + loginAssetName);
        aop.Completed += (handle) =>
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (loginbgObj != null)
                {
                    GameObject.Destroy(loginbgObj);
                }
                PlayerPrefs.SetInt("loginBgVer", int.Parse(loginAssetName));
                loginbgObj = GameObject.Instantiate(handle.Result, Vector2.zero, Quaternion.identity, startvideotf);
                if (handle.Result != null)
                {
                    Destroy(handle.Result);
                }
            }
            else
            {
                Logger.log("[AssetCache] asyncInstantiate failed : SkeletonLogin_BG");
                if (handle.IsValid() && handle.OperationException != null)
                {
                    Logger.error(handle.OperationException.ToString());
                }
                // loginbgObj = GameObject.Instantiate(loginanimObj, Vector2.zero, Quaternion.identity, startvideotf);

            }

            loginBgFinish = true;

            if (loginBgFinish && loginFontFinish)
            {
                SetLoginInfo();
            }
        };

        var aopFont = Addressables.InstantiateAsync("SkeletonLogin_Font" + loginAssetName);
        aopFont.Completed += (handle) =>
        {
            if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (loginFontObj != null)
                {
                    GameObject.Destroy(loginFontObj);
                }
                loginFontObj = GameObject.Instantiate(handle.Result, Vector2.zero, Quaternion.identity, startvideotf);
                if (handle.Result != null)
                {
                    Destroy(handle.Result);
                }
            }
            else
            {
                Logger.log("[AssetCache] asyncInstantiate failed : SkeletonLogin_Font");
                if (handle.IsValid() && handle.OperationException != null)
                {
                    Logger.error(handle.OperationException.ToString());
                }
                // loginFontObj = GameObject.Instantiate(loginanimObj_justFont, Vector2.zero, Quaternion.identity, startvideotf);
            }

            loginFontFinish = true;

            if (loginBgFinish && loginFontFinish)
            {
                SetLoginInfo();
            }
        };
    }

    public void SetLoginInfo()
    {

        if (isLandscape)
        {
            var sh = Mathf.Min(Screen.height, Screen.width);
            var sw = Mathf.Max(Screen.height, Screen.width);
            float w = (float)sh * (float)GameDesignSceneSize.x / (float)GameDesignSceneSize.y;
            float s2 = (float)sw / (float)w;

            if (loginbgObj != null)
            {
                loginbgObj.transform.localScale = Vector3.one * (GameDesignSceneSize.x * .5f * s2);

                var skeleton = loginbgObj.transform.GetChild(0).GetComponent<SkeletonAnimation>();
                if (skeleton != null)
                {
                    skeleton.AnimationName = "idle";//"loop2";
                }
            }

            if (loginFontObj != null)
            {
                loginFontObj.transform.localScale = Vector3.one * (GameDesignSceneSize.x * .5f * s2);

                var fontSkeleton = loginFontObj.transform.GetChild(0).GetComponent<SkeletonAnimation>();
                if (fontSkeleton != null)
                {
                    fontSkeleton.AnimationName = "screen_heng";//"loop2";
                }
            }


        }
        else
        {
            var sw = Mathf.Min(Screen.height, Screen.width);
            var sh = Mathf.Max(Screen.height, Screen.width);
            var h = (float)sw * (float)GameDesignSceneSize.y / (float)GameDesignSceneSize.x;
            float s = (float)sh / (float)h;

            if (loginbgObj != null)
            {
                loginbgObj.transform.localScale = Vector3.one * (GameDesignSceneSize.y * .5f * s);

                var skeleton = loginbgObj.transform.GetChild(0).GetComponent<SkeletonAnimation>();
                if (skeleton != null)
                {
                    skeleton.AnimationName = "idle";//"loop";
                }
            }

            if (loginFontObj != null)
            {
                loginFontObj.transform.localScale = Vector3.one * (GameDesignSceneSize.y * .5f * s);

                var fontSkeleton = loginFontObj.transform.GetChild(0).GetComponent<SkeletonAnimation>();
                if (fontSkeleton != null)
                {
                    fontSkeleton.AnimationName = "screen_shu";//"loop";
                }
            }

        }

        if (loginbgObj != null)
            loginbgObj.transform.localPosition = Vector3.zero;
        if (loginFontObj != null)
            loginFontObj.transform.localPosition = Vector3.zero;
    }
    // void Update()
    // {
    //     if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
    //     {
    //         if (!isLandscape)
    //             RotateScreen(true);
    //     }
    //     else if (Input.deviceOrientation == DeviceOrientation.Portrait)
    //     {
    //         if (isLandscape)
    //             RotateScreen(false);
    //     }
    // }
    //改变横竖屏

    //游戏设计分辨率
    public Vector2Int GameDesignSceneSize
    {
        get
        {
            if (isLandscape)
            {
                return StaticConstants.designSceneSizeL;
            }
            else
            {
                return StaticConstants.designSceneSize;
            }
        }
    }
#if UNITY_EDITOR
    public void setgameWindowSize(int width, int height)
    {
        GameViewUtils.switchOrientation(width, height);
    }
#endif
    public void setScreenOrientation(bool landscape)
    {
        RotateScreen(landscape);

#if !UNITY_EDITOR
PlatformManager.inst.ScreenOrientationChange(isLandscape ? 1 : 0);
#endif

        if (HotfixBridge.inst != null)
            HotfixBridge.inst.CallBackAndChangeMainView();

        HotfixBridge.inst.TriggerLuaEvent("HideUI_GuideTask");
        HotfixBridge.inst.TriggerLuaEvent("ShowUI_GuideTask");
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REALHIDEPANEL);
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SHOWMAINLINEUI);
    }
    public void RotateScreen(bool landscape)
    {
        if (PlatformManager.inst != null)
        {
            PlatformManager.inst.setFloatBtnVisbale(false);
        }
        isLandscape = landscape;
#if UNITY_EDITOR
        if (Screen.width > Screen.height != isLandscape)
        {
            setgameWindowSize(Screen.height, Screen.width);
        }
#endif
        if (landscape)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;   //左横屏
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;    //竖屏
        }
        SaveManager.inst.SaveInt("screenorientation", landscape ? 1 : 0, false);

        Debug.Log((landscape ? "LandscapeLeft>>>>" : "Portrait>>>>") + "Screen.height : " + Screen.height.ToString() + "---Screen.width: " + Screen.width.ToString() + "----GameDesignSceneSize:" + GameDesignSceneSize.ToString());

        CanvasScaler canvasScaler = uiRootTF.GetComponent<CanvasScaler>();
        canvasScaler.referenceResolution = GameDesignSceneSize;

        if (!isLandscape)
        {
            var w = Mathf.Min(Screen.height, Screen.width);
            var h = Mathf.Max(Screen.height, Screen.width);
            var _h = (float)w * GameDesignSceneSize.y / GameDesignSceneSize.x;
            //竖屏逻辑
            if ((float)h >= _h)
            {
                canvasScaler.matchWidthOrHeight = 0;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 1;
            }
        }
        else
        {
            var h = Mathf.Min(Screen.height, Screen.width);
            var w = Mathf.Max(Screen.height, Screen.width);
            var _w = (float)h * GameDesignSceneSize.x / GameDesignSceneSize.y;
            if ((float)w >= _w)
            {
                canvasScaler.matchWidthOrHeight = 1;// - ((float)h / (float)w);
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 0;//(float)h / (float)w;
            }
        }
        CanvasScaler hideCanvasScaler = uiHideRootTF.GetComponent<CanvasScaler>();
        hideCanvasScaler.referenceResolution = GameDesignSceneSize;
        hideCanvasScaler.matchWidthOrHeight = canvasScaler.matchWidthOrHeight;

        if (D2DragCamera.inst != null && (ManagerBinder.inst.mGameState == kGameState.Town || ManagerBinder.inst.mGameState == kGameState.Shop))
        {
            D2DragCamera.inst.OnSceneRotate();
        }

        if (GameTimer.inst != null)
        {
            GameTimer.inst.AddTimer(1, 1, () =>
            {
                PlatformManager.inst.setFloatBtnVisbale(true);
            });
        }

    }
    #region  动态分辨率
    private int arrow = 0;
    private int[] heights;
    private int[] widths;

    public void AddResolution()
    {
        if (arrow < heights.Length - 1)
            arrow++;

        turnResolution = true;
    }

    public void subResolution()
    {
        if (arrow > 0)
            arrow--;
        turnResolution = true;
    }
    bool turnResolution = false;

    bool closecheckFPS = false;

#if !UNITY_IOS

    void LateUpdate()
    {
        //return;
        if (closecheckFPS) return;
        if (turnResolution)
        {
            if (arrow <= 3)
            {
                QualitySettings.DecreaseLevel(true);//降低quality
            }

            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                screenAR = (float)Screen.width / (float)Screen.height;
                //CanvasScaler canvasScaler = uiRootTF.GetComponent<CanvasScaler>();
                var w = heights[arrow] * /*canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y)*/screenAR;
                Screen.SetResolution((int)w, heights[arrow], true);
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                screenAR = (float)Screen.height / (float)Screen.width;
                var h = heights[arrow] * screenAR;
                Screen.SetResolution(heights[arrow], (int)h, true);
            }
            SaveManager.inst.SaveInt("arrow", arrow + 1);
            turnResolution = false;
        }
        else
        {
            AutoGameScreenSize();
        }
    }

#endif

    float checkTimes = 0;
    float screenAR = 0;
    void AutoGameScreenSize()
    {
        if (arrow > 0 && ShowFPS.inst.fps < 32)
        {
            if (ManagerBinder.inst == null || ManagerBinder.inst.mGameState != kGameState.Shop)
                return;
            if (checkTimes > 20)
            {
                arrow--;
                if (arrow < 0 || arrow >= heights.Length)
                {
                    arrow = 0;
                }
                turnResolution = true;
                checkTimes = 0;

                if (D2DragCamera.inst != null)
                {
                    D2DragCamera.inst.beForcedFalse = true;
                    D2DragCamera.inst.SetVolumeBloomActive(false);
                }

            }
            else
            {
                checkTimes += Time.deltaTime;
            }
        }
        else
        {
            checkTimes = 0;
            if (arrow <= 0)
            {
                ShowFPS.inst.enabled = false;
                closecheckFPS = true;
            }
        }
    }
    #endregion


    int maskTimerid = 0;
    public void showGlobalMask(float selftime) //多少秒自动关闭
    {
        if (maskTimerid > 0)
        {
            GameTimer.inst.RemoveTimer(maskTimerid);
            maskTimerid = 0;
        }
        globalMask.raycastTarget = true;
        maskTimerid = GameTimer.inst.AddTimer(selftime, 1, () =>
        {
            maskTimerid = 0;
            globalMask.raycastTarget = false;
        });
    }

    public void setGlobalMaskActice(bool active)
    {
        if (maskTimerid > 0)
        {
            GameTimer.inst.RemoveTimer(maskTimerid);
            maskTimerid = 0;
        }

        globalMask.raycastTarget = active;
    }

    //通过canvasGroup设置所有UI是否可以交互
    public void SetAllUIInteractable(bool interactable)
    {
        if (uiRootCanvasGroup != null)
        {
            uiRootCanvasGroup.interactable = interactable;
        }
    }

    public void SetAllUIAlpha(float alpha)
    {
        if (uiRootCanvasGroup != null)
        {
            uiRootCanvasGroup.alpha = alpha;
        }
    }

    //FGUI.inst.sceneExcess.DOPlayForward();
    public void StartExcessAnimation(bool forward, bool toshop, UnityAction callback = null)
    {
        if (forward)
        {
            Logger.log("*************************************************************************************************");
            if (D2DragCamera.inst != null)
            {
                D2DragCamera.inst.CameraChangeSceneAnim(true);
            }
            sceneExcess.gameObject.SetActive(true);
            sceneExcess.speed = 2;
            UIAnimatorManger.inst.SetAnimatorValue(sceneExcess, "open", false).OnComplete = () =>
            {
                if (callback != null)
                {
                    callback();
                }
            };
        }
        else
        {
            if (D2DragCamera.inst != null)
            {
                D2DragCamera.inst.CameraChangeSceneAnim(false);
            }
            sceneExcess.speed = 2;
            UIAnimatorManger.inst.SetAnimatorValue(sceneExcess, "open", true).OnComplete = () =>
            {
                sceneExcess.gameObject.SetActive(false);
                if (callback != null)
                {
                    callback();
                }
            };
        }
    }

    int tx_loopJumpBottomAnimTimer;
    int index = 0;
    public void SetLoopJumpLoadingAnim(bool onOff)
    {
        if (onOff)
        {
            string text = LanguageManager.inst.GetValueByKey("正在进入商店中");

            tx_loopJumpBottom.text = text;

            tx_loopJumpBottomAnimTimer = GameTimer.inst.AddTimer(0.35f, () =>
            {

                var temp = text;

                for (int i = 0; i <= index; i++)
                {
                    temp += ".";
                }

                tx_loopJumpBottom.text = temp;

                index++;
                if (index > 2) index = 0;

            });

        }
        else
        {
            if (tx_loopJumpBottomAnimTimer != 0)
            {
                GameTimer.inst.RemoveTimer(tx_loopJumpBottomAnimTimer);
                tx_loopJumpBottomAnimTimer = 0;
            }
        }

        obj_loopJumpAnim.SetActive(onOff);
    }

    public void SetBlackMaskFade(float fade, float time = 0, UnityAction callback = null)
    {
        if (time <= 0)
        {
            GUIHelper.SetUIGrayColor(img_blackMask.transform, 1, fade);
            callback?.Invoke();
        }
        else
        {
            img_blackMask.DOFade(1, time).From(0, true).SetEase(Ease.InQuart).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
    }

    public void BlackMaskAnimation(UnityAction to1Callback = null, UnityAction endCallback = null, float to1Time = 1f, float to0Time = 1.5f)
    {

        if (to1Time <= 0f)
        {

            img_blackMask.DOFade(0, to0Time).From(1, true).SetEase(Ease.InQuart).OnComplete(() =>
            {
                endCallback?.Invoke();
            }).OnStart(() =>
            {
                to1Callback?.Invoke();
            });
        }
        else
        {
            img_blackMask.DOFade(1, to1Time).From(0, true).OnComplete(() =>
            {
                to1Callback?.Invoke();

                img_blackMask.DOFade(0, to0Time).From(1, true).SetEase(Ease.InQuart).OnComplete(() =>
                {
                    endCallback?.Invoke();
                });

            });
        }

    }

    private void showGameTips(bool show, bool gotoShop)
    {
        if (gameLoadingTips != null)
        {
            if (show)
            {
                string str = GameTipsConfigManager.inst.GetRandemTipsStr();
                if (string.IsNullOrEmpty(str)) return;
                gameLoadingTips.gameObject.SetActive(true);
                gameLoadingTips.cityImage.enabled = !gotoShop;
                gameLoadingTips.shopImage.enabled = gotoShop;
                gameLoadingTips.tipText.font = LanguageManager.inst.curFont;
                gameLoadingTips.tipText.text = LanguageManager.inst.GetValueByKey(str);
            }
            else
            {
                gameLoadingTips.gameObject.SetActive(false);
            }
        }
    }
    List<FollowBloodstrip> bloodsHUDList = new List<FollowBloodstrip>();
    public FollowBloodstrip CreaterHealthTip(int key, Transform targetTF)
    {
        var newGo = GameObject.Instantiate<GameObject>(HealthTip, Vector3.one * 5000, Quaternion.identity, this.hudPlanel);
        newGo.SetActive(true);
        FollowBloodstrip tip = newGo.GetComponent<FollowBloodstrip>();
        if (tip != null)
        {
            tip.key = key;
            tip.target = targetTF;
            bloodsHUDList.Add(tip);
        }
        return tip;
    }
    public void DestroyHealthTip(int key)
    {
        var tip = bloodsHUDList.Find(item => item.key == key);
        if (tip != null)
        {
            GameObject.Destroy(tip.gameObject);
            bloodsHUDList.Remove(tip);
        }
    }


    public HouseComp CreaterHousePlane(Transform target)
    {
        if (target == null) return null;
        var newGo = GameObject.Instantiate<GameObject>(buildingHUD.gameObject, Vector3.one * 5000, Quaternion.identity, this.cityHudCanvas);
        newGo.SetActive(true);
        newGo.transform.localScale = Vector3.one * 8;
        HouseComp tip = newGo.GetComponent<HouseComp>();
        if (tip != null)
        {
            tip.followTarget = target;
        }
        return tip;
    }

    public WorkerRecruitHud CreateWorkerRecruitHUD(Transform target)
    {
        if (target == null) return null;
        var newGo = GameObject.Instantiate<GameObject>(workerRecruitHUD.gameObject, Vector3.one * 5000, Quaternion.identity, this.cityHudCanvas);
        newGo.SetActive(true);
        newGo.transform.localScale = Vector3.one * 8;
        WorkerRecruitHud hud = newGo.GetComponent<WorkerRecruitHud>();
        if (hud != null)
        {
            hud.followTarget = target;
        }
        return hud;
    }

    public ScienceLabBuildingUnLockHud CreateBuildingUnLockHUD(Transform target)
    {
        if (target == null) return null;

        var newGo = GameObject.Instantiate<GameObject>(scienceLabUnlockHUD.gameObject, Vector3.one * 5000, Quaternion.identity, this.cityHudCanvas);
        newGo.SetActive(true);
        newGo.transform.localScale = Vector3.one * 8;
        ScienceLabBuildingUnLockHud hud = newGo.GetComponent<ScienceLabBuildingUnLockHud>();
        if (hud != null)
        {
            hud.followTarget = target;
        }
        return hud;
    }

    public UnionAidHUD CreateUnionAidHUD(Transform target)
    {
        if (target == null) return null;

        var newGo = GameObject.Instantiate<GameObject>(unionAidHUD.gameObject, Vector3.one * 5000, Quaternion.identity, this.cityHudCanvas);
        newGo.SetActive(true);
        newGo.transform.localScale = Vector3.one * 8;
        UnionAidHUD hud = newGo.GetComponent<UnionAidHUD>();
        if (hud != null)
        {
            hud.followTarget = target;
        }
        return hud;
    }

    public LuaListItem CreatRuinsHouseHUD(Transform target)
    {
        if (target == null) return null;

        var newGo = GameObject.Instantiate<GameObject>(ruinsHUD.gameObject, Vector3.one * 5000, Quaternion.identity, this.cityHudCanvas);
        newGo.SetActive(true);
        newGo.transform.localScale = Vector3.one * 8;
        newGo.transform.localPosition += new Vector3(0, 2800, 0);
        LuaListItem hud = newGo.GetComponent<LuaListItem>();
        if (hud != null)
        {
            var targetFollow = hud.GetComponent<Follow2DTarget>();
            if (targetFollow != null)
                targetFollow.target = target;
        }
        return hud;
    }

    public void onMouseClick(Vector3 mousepos)
    {
        //当触摸 点击 播放特效
        EffectManager.inst.Spawn(3010, Vector3.zero, (gamevfx) =>
        {
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.position = Camera.main.ScreenToWorldPoint(mousepos);
            // gamevfx.transform.parent = FGUI.inst.vfxPlanel;
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 101);
        });
    }

    [Header("更新loading条")]
    #region loadingBar
    public GameObject loadingPanle;
    public Slider progressSlider;
    public Image proglressBg;
    public Text proglressText;

    public PrefabTipUI msgtipUI;
    public void showLoading()
    {
        if (loadingPanle != null)
        {
            loadingPanle.SetActive(true);
            progressSlider.gameObject.SetActive(false);
            proglressBg.enabled = false;
            proglressText.text = "";// LanguageManager.inst.curType == LanguageType.TRADITIONAL_CHINESE ? "" : CNrevations; ;
        }
    }

    public void updateProgressText(string str)
    {
        proglressText.text = str;

    }

    public void updateProglressBar(string str, float proglress, float dur)
    {
        proglressText.text = str;
        progressSlider.gameObject.SetActive(true);
        progressSlider.value = proglress;
        if (dur > 0)
        {
            progressSlider.DOValue(proglress, dur);
        }
        else
        {
            progressSlider.value = proglress;
        }
    }

    public void hideLoading()
    {
        if (loadingPanle != null)
        {
            loadingPanle.SetActive(false);
            progressSlider.gameObject.SetActive(false);
            proglressBg.enabled = false;
            proglressText.text = "";
        }
    }
    #endregion


    public void ShowGolbalAwaitMask()
    {
        if (golbalAwaitMask != null)
        {
            setGlobalMaskActice(true);
            golbalAwaitMask.SetActive(true);
        }
    }

    int hideGolbalAwaitMaskTimer;
    public void HideGolbalAwaitMask()
    {
        if (hideGolbalAwaitMaskTimer != 0)
        {
            GameTimer.inst.RemoveTimer(hideGolbalAwaitMaskTimer);
            hideGolbalAwaitMaskTimer = 0;
        }

        if (golbalAwaitMask != null)
        {
            setGlobalMaskActice(false);
            golbalAwaitMask.SetActive(false);
        }
    }

    public void TimerHideGolbalAwaitMask(float delay = 15f, System.Action callback = null)
    {
        if (hideGolbalAwaitMaskTimer != 0)
        {
            GameTimer.inst.RemoveTimer(hideGolbalAwaitMaskTimer);
            hideGolbalAwaitMaskTimer = 0;
        }

        hideGolbalAwaitMaskTimer = GameTimer.inst.AddTimer(delay, 1, () =>
         {
             HideGolbalAwaitMask();
             callback?.Invoke();
         });
    }

}
