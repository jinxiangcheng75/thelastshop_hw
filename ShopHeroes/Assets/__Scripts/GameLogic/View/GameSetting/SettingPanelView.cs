using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class SettingPanelView : ViewBase<SettingPanelComp>
{
    public override string viewID => ViewPrefabName.SettingPanel;
    public override string sortingLayerName => "window";

    public List<Text> settingTexts;

    private List<string> keys;

    protected override void onInit()
    {
        contentPane.soundEffectSlider.onValueChanged.AddListener((value) =>
        {
            SetSoundVolume(value / 100);
        });

        contentPane.soundEffectTog.onValueChanged.AddListener((value) =>
        {
            OnSoundEffectTogDown(value);
        });

        contentPane.dropdown.onValueChanged.AddListener((t) => SetLanguage(contentPane.dropdown));

        contentPane.musicEffectSlider.onValueChanged.AddListener((value) =>
        {
            SetMusicVolume(value / 100);
        });

        contentPane.musicEffectTog.onValueChanged.AddListener((value) =>
        {
            OnMusicEffectTogDown(value);
        });

        keys = new List<string>();

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.soundEffectSlider.maxValue = 100;
        contentPane.musicEffectSlider.maxValue = 100;

        settingTexts = new List<Text>(Transform.FindObjectsOfType<Text>());
        GetDropDownIndex();

        contentPane.uiAnimToggle.onValueChanged.AddListener((ison) =>
        {
            GameSettingManager.inst.SetNeedShowUIAnim(ison);
        });

        //contentPane.url_facebookBtn.ButtonClickTween(() => Application.OpenURL(StaticConstants.URL_FacebookFanPage));
        //contentPane.url_discordBtn.ButtonClickTween(() => Application.OpenURL(StaticConstants.URL_Discord));

        GUIHelper.SetUIGray(contentPane.muteSoundEffectImgObj.transform, true);
        GUIHelper.SetUIGray(contentPane.muteMusicEffectImgObj.transform, true);

        contentPane.bindingBtn.onClick.AddListener(onBindingBtnClick);
        contentPane.closeSelectUIBtn.onClick.AddListener(() =>
        {
            contentPane.selectUITF.gameObject.SetActive(false);
            contentPane.bindingPanelBG.enabled = false;
        });
        contentPane.closeBindingUIBtn.onClick.AddListener(() =>
        {
            contentPane.bindingUITF.gameObject.SetActive(false);
            contentPane.bindingPanelBG.enabled = false;
        });
        contentPane.closeChangeUIBtn.onClick.AddListener(() =>
        {
            contentPane.changeUITF.gameObject.SetActive(false);
            contentPane.bindingPanelBG.enabled = false;
        });
        if (FGUI.inst.isLandscape)
            contentPane.hToggle.isOn = true;
        else
            contentPane.vToggle.isOn = true;

        contentPane.hToggle.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (!FGUI.inst.isLandscape)
                {
                    OnSceneOrientationChange(SceneRotatingType.LockLandscape);
                    //GUIManager.BackMainView();
                }
            }
        });
        contentPane.vToggle.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (FGUI.inst.isLandscape)
                {
                    OnSceneOrientationChange(SceneRotatingType.LockVertical);
                    //GUIManager.BackMainView();
                }
            }
        });

        if (WorldParConfigManager.inst.GetConfig(148) != null)
        {
            int _value = (int)WorldParConfigManager.inst.GetConfig(148).parameters;
            contentPane.dropdown.interactable = _value == 1;
        }
        else
        {
            contentPane.dropdown.interactable = false;
        }
    }

    private void OnSceneOrientationChange(SceneRotatingType type)
    {
        if (type == SceneRotatingType.LockLandscape)
        {
            //设置为锁定横屏
            FGUI.inst.setScreenOrientation(true);
        }
        else if (type == SceneRotatingType.LockVertical)
        {
            //设置为锁定竖屏
            FGUI.inst.setScreenOrientation(false);
        }
        else
        {
            //设置成自由旋转
        }
    }
    public void SetLanguage(Dropdown dropdown)
    {
        int index = dropdown.value;
        if ((int)LanguageManager.inst.curType == index) return;
        LanguageState(index);
        dropdown.captionText.text = dropdown.options[index].text;
        SetSoundVolume(AudioManager.inst.soundVolume);
        SetMusicVolume(AudioManager.inst.musicVolume);
    }

    //设置当前的DropDown的value，从而改变DropDown的显示Toggle
    private void GetDropDownIndex()
    {
        contentPane.dropdown.value = (int)LanguageManager.inst.curType;
    }

    public void LanguageState(int index)
    {
        LanguageManager.inst.LanguageState((LanguageType)index);
    }

    private void OnSoundEffectTogDown(bool value)
    {
        SetSoundVolume(value ? 1 : 0);
        contentPane.soundEffectSlider.value = value ? contentPane.soundEffectSlider.maxValue : 0;
    }

    private void OnMusicEffectTogDown(bool value)
    {
        SetMusicVolume(value ? 1 : 0);
        contentPane.musicEffectSlider.value = value ? contentPane.musicEffectSlider.maxValue : 0;
    }

    protected override void onShown()
    {
        var c = contentPane;
        for (int i = 0; i < c.dropdown.options.Count; i++)
        {
            keys.Add(c.dropdown.options[i].text);
        }
        contentPane.soundEffectSlider.value = AudioManager.inst.soundVolume * 100;
        contentPane.musicEffectSlider.value = AudioManager.inst.musicVolume * 100;

        contentPane.soundEffectTog.isOn = AudioManager.inst.soundVolume > 0;
        contentPane.musicEffectTog.isOn = AudioManager.inst.musicVolume > 0;

        contentPane.uiAnimToggle.isOn = GameSettingManager.inst.needShowUIAnim;
        updateBindingState();
        EventController.inst.AddListener<UserData>(GameEventType.BindingEvent.UPDATEPLATFORMQUERY, updateBindingQuery);
    }

    protected override void beforeDispose()
    {
        EventController.inst.RemoveListener<UserData>(GameEventType.BindingEvent.UPDATEPLATFORMQUERY, updateBindingQuery);
        base.beforeDispose();
    }

    protected override void onHide()
    {
        EventController.inst.RemoveListener<UserData>(GameEventType.BindingEvent.UPDATEPLATFORMQUERY, updateBindingQuery);
        base.onHide();
    }
    public void updateBindingState()
    {
        contentPane.bindingPanelBG.enabled = false;
        contentPane.bindingUITF.gameObject.SetActive(false);
        contentPane.selectUITF.gameObject.SetActive(false);
        contentPane.changeUITF.gameObject.SetActive(false);

        if (AccountDataProxy.inst.currbindingType != EBindingType.None)
        {
            //已绑定
            if (AccountDataProxy.inst.bindingClaimState)
            {
                contentPane.bindingBtnText.text = LanguageManager.inst.GetValueByKey("领取奖励");
                contentPane.bindingAwardPanel.gameObject.SetActive(true);
            }
            else
            {
                contentPane.bindingBtnText.text = LanguageManager.inst.GetValueByKey("已连接");
                contentPane.bindingAwardPanel.gameObject.SetActive(false);
            }
        }
        else
        {
            //未绑定
            contentPane.bindingAwardPanel.gameObject.SetActive(true);
            contentPane.bindingBtnText.text = LanguageManager.inst.GetValueByKey("未连接");
        }

        contentPane.bindingAwardCountText.text = WorldParConfigManager.inst.GetConfig(8100).parameters.ToString();
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        //contentPane.uiAnimator.CrossFade("show", 0f);
        //contentPane.uiAnimator.Update(0f);
        //contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();
        //contentPane.uiAnimator.Play("hide");
        //float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");

        //GameTimer.inst.AddTimer(animLength, 1, () =>
        //{
        //    contentPane.uiAnimator.CrossFade("null", 0f);
        //    contentPane.uiAnimator.Update(0f);
        //    this.HideView();
        //});
    }

    private void SetSoundVolume(float value)
    {
        AudioManager.inst.SoundMute(value == 0);
        AudioManager.inst.SetSoundVolume(value);
        contentPane.norSoundEffectImgObj.SetActive(value != 0);
        contentPane.muteSoundEffectImgObj.SetActive(value == 0);
        contentPane.soundEffectText.text = value == 0 ? LanguageManager.inst.GetValueByKey("{0} ", $"（{LanguageManager.inst.GetValueByKey("关闭")}）") : LanguageManager.inst.GetValueByKey("（{0}）", $"{(int)(AudioManager.inst.soundVolume * 100)}%");
    }

    private void SetMusicVolume(float value)
    {
        AudioManager.inst.StopMusic(value == 0);
        AudioManager.inst.SetMusicVolume(value);
        contentPane.norMusicEffectImgObj.SetActive(value != 0);
        contentPane.muteMusicEffectImgObj.SetActive(value == 0);
        contentPane.musicEffectText.text = value == 0 ? LanguageManager.inst.GetValueByKey("{0} ", $"（{LanguageManager.inst.GetValueByKey("关闭")}）") : LanguageManager.inst.GetValueByKey("（{0}）", $"{(int)(AudioManager.inst.musicVolume * 100)}%");
    }

    public void SetBindingPanel()
    {
        onBindingBtnClick();
    }

    private void onBindingBtnClick()
    {
        if (AccountDataProxy.inst.currbindingType != EBindingType.None)
        {
            //已绑定
            if (AccountDataProxy.inst.bindingClaimState)
            {
                //领取奖励
                EventController.inst.TriggerEvent(GameEventType.BindingEvent.GETBINDINGAWARD);
            }
            else
            {
                //contentPane.changeUITF.gameObject.SetActive(true);
            }
        }
        else
        {
            //未绑定
            openSelectPlatformUI();
        }
    }

    //
    void openSelectPlatformUI()
    {
        contentPane.bindingPanelBG.enabled = true;
        contentPane.selectUITF.gameObject.SetActive(true);


        var animator = contentPane.selectUITF.GetComponent<Animator>();
        if (animator != null)
        {
            animator.CrossFade("show", 0f);
            animator.Update(0f);
            animator.Play("show");
        }

        //contentPane.selectUITF.localScale = Vector3.one * .2f;
        // contentPane.selectUITF.DOScale(Vector3.one, .3f).SetEase(Ease.OutElastic);
        contentPane.googleBtn.onClick.RemoveAllListeners();
        contentPane.googleBtn.onClick.AddListener(() =>
        {
            //登陆并获取sdk token
            currAccountPlatform = 1;
            EventController.inst.TriggerEvent(GameEventType.LOGIN_SDK);
        });
    }


    void updateBindingQuery(UserData data)
    {
        if (data == null)
        {
            openBindingUi(1);
        }
        else
        {
            openChangeAccontUI(data);
        }
    }

    int currAccountPlatform = 0;
    // 0 = test, 1 = google, 2 = appstore, 3 = FB
    public void openBindingUi(int platform)
    {
        currAccountPlatform = platform;
        contentPane.bindingPanelBG.enabled = true;
        contentPane.selectUITF.gameObject.SetActive(false);
        contentPane.bindingUITF.gameObject.SetActive(true);

        var animator = contentPane.selectUITF.GetComponent<Animator>();
        if (animator != null)
        {
            animator.CrossFade("show", 0f);
            animator.Update(0f);
            animator.Play("show");
        }

        contentPane.okBtn.onClick.RemoveAllListeners();
        contentPane.okBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.BindingEvent.BINDINGACCOUNT);
        });
        contentPane.cancelBtn.onClick.RemoveAllListeners();
        contentPane.cancelBtn.onClick.AddListener(() =>
        {
            updateBindingState();
        });

        contentPane.playerName.text = UserDataProxy.inst.playerData.playerName;
        contentPane.playerLv.text = UserDataProxy.inst.playerData.level.ToString();
        contentPane.platformName.text = AccountDataProxy.inst.platformUseName;
    }
    public void openChangeAccontUI(UserData data)
    {
        contentPane.bindingPanelBG.enabled = true;
        contentPane.selectUITF.gameObject.SetActive(false);
        contentPane.changeUITF.gameObject.SetActive(true);
        contentPane.changeUITF.GetComponent<Animator>().Play("show");
        contentPane.remoteLevelText.text = data.level.ToString();
        contentPane.remoteNiceNameText.text = data.nickName;
        contentPane.changeAccountBtn.onClick.RemoveAllListeners();
        contentPane.changeAccountBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.BindingEvent.CHANGEACCOUNTRLOGIN);
            FGUI.inst.showGlobalMask(5);
        });
        contentPane.cancelChangeBtn.onClick.RemoveAllListeners();
        contentPane.cancelChangeBtn.onClick.AddListener(() =>
        {
            updateBindingState();
        });

        contentPane.currNiceNameText.text = UserDataProxy.inst.playerData.playerName;
        contentPane.currLevelText.text = UserDataProxy.inst.playerData.level.ToString();

        contentPane.remoteLevelText.text = data.level.ToString();
        contentPane.remoteNiceNameText.text = data.nickName;
        contentPane.platformName2.text = AccountDataProxy.inst.platformUseName;
    }
}

