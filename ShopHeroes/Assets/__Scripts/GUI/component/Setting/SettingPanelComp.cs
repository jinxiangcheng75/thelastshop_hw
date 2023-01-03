using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelComp : MonoBehaviour
{
    public Button closeBtn;
    public Slider soundEffectSlider;
    public Slider musicEffectSlider;
    public Text settingText;
    public Text languageText;
    public Text soundEffectText;
    public Text musicEffectText;
    public Toggle soundEffectTog;
    public Toggle musicEffectTog;
    public Dropdown dropdown;
    public Text label;
    public GameObject norSoundEffectImgObj;
    public GameObject muteSoundEffectImgObj;
    public GameObject norMusicEffectImgObj;
    public GameObject muteMusicEffectImgObj;
    public Toggle uiAnimToggle;
    public Button url_facebookBtn;
    public Button url_discordBtn;
    public Button RotateButton;
    [Header("动画")]
    public Animator uiAnimator;
    [Header("账号绑定")]
    public Button bindingBtn;
    public Text bindingBtnText;
    public Image bindingPanelBG;
    public Transform bindingAwardPanel;
    public GUIIcon bindingAwardIcon;
    public Text bindingAwardCountText;

    [Header("选择账号平台")]
    public Transform selectUITF;
    public Button closeSelectUIBtn;
    public Button googleBtn;
    public Button fbBtn;

    [Header("绑定账号")]
    public Transform bindingUITF;
    public Button closeBindingUIBtn;
    public Button okBtn;
    public Button cancelBtn;
    public Text playerName;
    public Text playerLv;
    public Text platformName;

    [Header("切换账号")]
    public Transform changeUITF;
    public Button closeChangeUIBtn;
    public Button changeAccountBtn;
    public Button cancelChangeBtn;
    public Text currNiceNameText;
    public Text currLevelText;

    public Text remoteLevelText;
    public Text remoteNiceNameText;
    public Text platformName2;

    [Header("切换横竖屏")]
    public Toggle hToggle;
    public Toggle vToggle;
}

