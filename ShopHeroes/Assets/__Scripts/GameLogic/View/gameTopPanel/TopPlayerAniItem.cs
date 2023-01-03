using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPlayerAniItem : MonoBehaviour
{
    public Slider slider;
    public GUIIcon characterIcon;
    public Text tipsTx;
    [HideInInspector]
    public int workerID;

    public Action delAniCallBack;

    private float timer;

    private bool isInventoryShowing;

    private bool isTaskShowing;
    public float startPos, endPos;
    public void SetData(float sliderVal, string atlas, string iconName, string tips, bool isShowing)
    {
        slider.value = sliderVal;
        characterIcon?.SetSprite(atlas, iconName);
        tipsTx.text = tips;
        timer = 2;

        if (!isShowing)
        {
            gameObject.SetActiveTrue();
            playAnim(0.3f);
        }
        else
        {
            if (DOTween.IsTweening(transform as RectTransform))
                DOTween.Kill(transform as RectTransform, true);
            (transform as RectTransform).DOAnchorPos3DX(endPos, (endPos - (transform as RectTransform).anchoredPosition3D.x) / (endPos - startPos) * 0.3f);
        }
    }

    public void SetData(float sliderVal, string atlas, string iconName, string tips)
    {
        slider.value = sliderVal;
        characterIcon?.SetSprite(atlas, iconName);
        tipsTx.text = tips;

        timer = 2;
        gameObject.SetActiveTrue();
        if (!isInventoryShowing)
            playAnim(0.3f);
        else
        {
            if (DOTween.IsTweening(transform as RectTransform))
                DOTween.Kill(transform as RectTransform, true);
            (transform as RectTransform).DOAnchorPos3DX(endPos, (endPos - (transform as RectTransform).anchoredPosition3D.x) / (endPos - startPos) * 0.3f);
        }
    }

    public void SetTaskData(float sliderVal, string atlas, string iconName, string contentTxt)
    {
        slider.value = sliderVal;

        characterIcon?.SetSprite(atlas, iconName);

        tipsTx.text = contentTxt;

        timer = 2;
        gameObject.SetActiveTrue();
        if (!isTaskShowing)
            playAnim(0.3f);
    }

    private void playAnim(float duration)
    {
        (transform as RectTransform).DOAnchorPos3DX(endPos, duration).From(startPos).OnStart(() => { isInventoryShowing = true; isTaskShowing = true; });
    }


    private void Update()
    {

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            //播放回去的动画
            timer = 9999;
            (transform as RectTransform).DOAnchorPos3DX(startPos, 0.4f).From(endPos).OnComplete(() =>
            {
                isInventoryShowing = false;
                isTaskShowing = false;
                delAniCallBack?.Invoke();
                gameObject.SetActive(false);
            });
        }

    }

}
