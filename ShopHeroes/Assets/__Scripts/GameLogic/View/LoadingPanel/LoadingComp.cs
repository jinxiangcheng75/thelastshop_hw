using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
public class LoadingComp : MonoBehaviour
{
    [Header("-加载UI-")]
    public Slider loadingSlider;
    public Text loadingText;
    public Text versionText;
    public Image loadingBG;

    [Header("-断开连接-")]
    public GameObject disconnectObj;
    public Button contactBtn;
    public Button reconnectBtn;
    public Button dicCloseBtn;

    [Header("-错误-")]
    public GameObject errorBoj;
    public Button retryBtn;
    public Button errorCloseBtn;

    public AssetReference backGroundBG;
}
