using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoginView : ViewBase<GameLoginComp>
{
    public override string viewID => ViewPrefabName.GameLoginUI;
    public override int showType => (int)ViewShowType.normal;
    protected override void onInit()
    {
        base.onInit();
        contentPane.loginBtn.onClick.AddListener(loginGame);
#if !UNITY_EDITOR //&& false  /////////////////////////测试打开
        contentPane.accountInput.gameObject.SetActive(false);
        contentPane.accountInput.transform.parent.localScale = Vector3.zero;
#endif

        if (contentPane.adBtn != null)
        {
            contentPane.adBtn.onClick.AddListener(() =>
            {
                //SDKManager.inst.PlayRewardedVideo("1");
            }
            );
        }

        contentPane.ageBtn.onClick.AddListener(() =>
        {
            contentPane.ageObjBtn.gameObject.SetActive(true);
        });

        contentPane.ageObjBtn.onClick.AddListener(() =>
        {
            contentPane.ageObjBtn.gameObject.SetActive(false);
        });

        contentPane.ageCloseBtn.onClick.AddListener(() =>
        {
            contentPane.ageObjBtn.gameObject.SetActive(false);
        });
    }

    private void loginGame()
    {
        PlatformManager.inst.GameHandleEventLog("Start", "");
        EventController.inst.TriggerEvent(GameEventType.AccountEvent.UI_LoginServer, contentPane.accountInput.text, contentPane.passwordInput.text);  //登陆
        FGUI.inst.showGlobalMask(5.0f);
    }

    protected override void onShown()
    {
        if (AccountDataProxy.inst.mNotice != null)
        {
            var curLanguageData = AccountDataProxy.inst.mNotice.Find(t => t.lang == (int)LanguageManager.inst.curType);
            EventController.inst.TriggerEvent(GameEventType.NoticeEvent.SHOWUI_NOTICE, curLanguageData);
        }

        var c = contentPane;
        var l = LanguageManager.inst;
        c.密码Txt.text = l.GetValueByKey("密码");
        c.用户名Txt.text = l.GetValueByKey("用户名");
        (c.passwordInput.placeholder as Text).text = l.GetValueByKey("密码") + "...";
        (c.accountInput.placeholder as Text).text = l.GetValueByKey("输入用户名") + "...";
        c.登入Txt.text = l.GetValueByKey("登录");

        contentPane.vText.text = $"app:v{GameSettingManager.appVersion}_res:v{GameSettingManager.resVersion}";
        //contentPane.accountInput.text = "";
        //contentPane.passwordInput.text = "";
        if (PlayerPrefs.HasKey("account"))
        {
            contentPane.accountInput.text = PlayerPrefs.GetString("account");
        }
    }

    public void setLastAccountInput(string account)
    {
        contentPane.accountInput.text = account;
        contentPane.loginBtn.gameObject.SetActive(true);
    }

    protected override void onHide()
    {
    }
}
