using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivacyPanel : SingletonMono<PrivacyPanel>
{
    public Button confimBtn;
    public Button refuseBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (confimBtn != null)
        {
            confimBtn.ButtonClickTween(() =>
            {
                hidePrivacyPanel();
                if (PlatformManager.inst != null)
                {
                    PlatformManager.inst.PrivacyCallBack();
                }
            });
        }

        if (refuseBtn != null)
        {
            refuseBtn.ButtonClickTween(() =>
            {
                PlatformManager.inst.ExitApp();
            });
        }
    }

    public void hidePrivacyPanel()
    {
        gameObject.SetActive(false);
    }

    public void showPrivacyPanel()
    {
        gameObject.SetActive(true);
    }
}
