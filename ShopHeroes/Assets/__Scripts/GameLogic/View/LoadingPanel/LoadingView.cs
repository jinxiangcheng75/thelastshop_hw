using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : ViewBase<LoadingComp>
{
    public override string viewID => ViewPrefabName.LoadingPanel;
    public override string sortingLayerName => "top";
    protected override void onInit()
    {
        base.onInit();
        contentPane.loadingSlider.value = 0;
    }
    protected override void onHide()
    {
        base.onHide();
    }
    protected override void onShown()
    {
        base.onShown();

        contentPane.loadingSlider.value = 0;
        FGUI.inst.StartCoroutine(LoadingSliderStart());
        //EventController.inst.AddListener<float, int, int>(AssetLoadEvent.LOAD_PROGRESS, onLoadProgress);

    }
    private async void loadBackGround()
    {
        await contentPane.backGroundBG.LoadAssetAsync<Sprite>().Task;
        contentPane.loadingBG.sprite = contentPane.backGroundBG.Asset as Sprite;
    }
    IEnumerator LoadingSliderStart()
    {
        // 现在用的是伪加载
        while (true)
        {
            if (contentPane.loadingSlider.value < 1)
            {
                contentPane.loadingSlider.value += 0.036f;
                contentPane.loadingText.text = LanguageManager.inst.GetValueByKey("正在加载......") + Mathf.Floor(contentPane.loadingSlider.value * 100) + "%";
                yield return new WaitForSeconds(0.05f);
            }
            else
                break;
        }
    }

    public void onLoadProgress(float p, int loaded, int total)
    {
        contentPane.loadingText.text = LanguageManager.inst.GetValueByKey("正在加载......") + Mathf.Floor(p * 100) + "% (";//+ loaded + " / " + total;
        contentPane.loadingSlider.value = p;
    }
}
