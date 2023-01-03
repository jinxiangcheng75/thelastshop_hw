using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NetworkErrorPanel : SingletonMono<NetworkErrorPanel>
{
    public Button okBtn;
    public Text okBtnText;
    public Transform bgTF;
    public Transform msgWindow;
    public Transform awaitTF;
    private INetworkPackage lastpkg;
    public Transform errorWindow;
    public Text tipText;
    public Text msgText;
    // int FailedNunber = 0;
    // bool responseError = false;

    void Start()
    {
        okBtn.onClick.AddListener(() =>
        {
            if (NetworkManager.inst != null)
            {
                NetworkManager.inst.PauseKeepAlive(false);
            }
            hide();
        });
    }
    public void showState(int state)
    {
        errorWindow.gameObject.SetActive(false);
        bgTF.gameObject.SetActive(false);
        awaitTF.gameObject.SetActive(false);
        msgWindow.gameObject.SetActive(false);
        if (state == 0)
        {
            bgTF.gameObject.SetActive(false);
            awaitTF.gameObject.SetActive(false);
            msgWindow.gameObject.SetActive(false);
        }
        else if (state == 1)
        {
            bgTF.gameObject.SetActive(true);
            awaitTF.gameObject.SetActive(false);
            msgWindow.gameObject.SetActive(true);
            tipText.font = LanguageManager.inst.curFont;
            tipText.text = LanguageManager.inst.GetValueByKey("提示");
            okBtnText.text = LanguageManager.inst.GetValueByKey("确定");
            msgText.font = LanguageManager.inst.curFont;
            msgText.text = LanguageManager.inst.GetValueByKey("无法与服务器建立连接。请检查您的网络。");
        }
        else if (state == 2)
        {
            bgTF.gameObject.SetActive(true);
            awaitTF.gameObject.SetActive(true);
        }
        else if (state == 3)
        {
            Logger.log("sdkchushihuashibai!!!!!!!!!!!!!!!!");
            bgTF.gameObject.SetActive(true);
            errorWindow.gameObject.SetActive(true);
        }
    }
    private void ReResponse()
    {
        if (lastpkg != null)
        {
            //重发
            //ManagerBinder.inst.mNetworkMgr.send(lastpkg);
        }
    }
    public void hide()
    {
        showState(0);
    }
}
