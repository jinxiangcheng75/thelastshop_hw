using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PrefabTipUI : MonoBehaviour
{
    public Text titleText;
    public Text msgText;
    public Button okbtn;
    public Text okbtn_text;
    public Button okbtn2;
    public Text okbtn2_text;

    public Button okbtn3;
    public Text okbtn3_text;

    public Animator _animator;

    int mType = 0;
    void Start()
    {
        okbtn.onClick.AddListener(() =>
        {
            if (mType == 0)
            {
                //SDKManager.inst.ToGooglePlay();
                PlatformManager.inst.ToAppUpdateUrl();
            }
            else if (mType == 1) //小版本更新
            {
                PlatformManager.inst.ExitApp();
            }
            else if (mType == 2) //重新登陆
            {
                PlatformManager.inst.ExitApp();
            }
            else if (mType == 3) //内容更新完毕
            {
                PlatformManager.inst.ExitApp();
            }
            else
            {
                //EventController.inst.TriggerEvent(GameEventType.GAME_RESTART);
                PlatformManager.inst.Restart();
            }
            FGUI.inst.msgtipUI.gameObject.SetActive(false);
        });

        okbtn2.onClick.AddListener(() =>
        {
            //if (mType == 2)
            //{
            //    // SDKManager.Restart(1);
            //    PlatformManager.inst.ExitApp();
            //}
            FGUI.inst.msgtipUI.gameObject.SetActive(false);
        });

        okbtn3.onClick.AddListener(() =>
        {
            //if (mType == 2)
            //{
            //    PlatformManager.inst.ReLoginGame();
            //}
            FGUI.inst.msgtipUI.gameObject.SetActive(false);
        });
    }
    void OnEnable()
    {
        _animator.Play("show", 0);
        if (LanguageManager.inst != null)
            titleText.text = LanguageManager.inst.GetValueByKey("提示");
    }

    public void setShowInfo(int type)
    {
        if (this.enabled == false) return;
        okbtn.gameObject.SetActive(false);
        okbtn2.gameObject.SetActive(false);
        okbtn3.gameObject.SetActive(false);
        mType = type;
        if (type == 0)
        {
            //大版本
            msgText.text = LanguageManager.inst.GetValueByKey("客户端版本过旧，请更新最新的客户端安装包！");// LanguageManager.inst.curType == LanguageType.TRADITIONAL_CHINESE ? "客戶端版本過舊，請更新最新的客戶端安裝包！" : "客户端版本过旧，请更新最新的客户端安装包！";
            okbtn.gameObject.SetActive(true);
            okbtn_text.text = LanguageManager.inst.GetValueByKey("确定");
        }
        else if (type == 1)
        {
            //小版本
            msgText.text = LanguageManager.inst.GetValueByKey("发现游戏新内容，请重启游戏更新！");
            okbtn.gameObject.SetActive(true);
            okbtn_text.text = LanguageManager.inst.GetValueByKey("确定");
        }
        else if (type == 2)
        {
            //重复登陆
            msgText.text = LanguageManager.inst.GetValueByKey("您的账号在其它设备登陆！");
            //okbtn2.gameObject.SetActive(true);
            //okbtn3.gameObject.SetActive(true);
            //okbtn3_text.text = LanguageManager.inst.GetValueByKey("重新登陆游戏");
            //okbtn2_text.text = LanguageManager.inst.GetValueByKey("退出游戏");
            okbtn.gameObject.SetActive(true);
            okbtn_text.text = LanguageManager.inst.GetValueByKey("退出游戏");
            GameTimer.inst.AddTimer(1, 1, () =>
            {
                Helper.GamePause(true);
            });
        }
        else if (type == 3)
        {
            //更新完毕退出游戏
            msgText.text = LanguageManager.inst.GetValueByKey("内容更新完毕，请重新启动游戏！");
            okbtn.gameObject.SetActive(true);
            okbtn_text.text = LanguageManager.inst.GetValueByKey("确定");
        }
        else
        {
            //异常 封号
            msgText.text = LanguageManager.inst.GetValueByKey("与服务器连接中断");
            okbtn.gameObject.SetActive(true);
            okbtn_text.text = LanguageManager.inst.GetValueByKey("重新连接");
        }
    }


}
