using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class FlyVfxCtrl : MonoBehaviour
{
    Image image;
    Action onAnimEndHandler;
    bool isExecute;

    private void Awake()
    {
        image = GetComponent<Image>();
    }


    public void SetSprite(Sprite sprite, bool needSetNativeSize = true, float scale = 1)
    {
        if (image != null)
        {
            image.sprite = sprite;
            if (needSetNativeSize) image.SetNativeSize();
            image.transform.localScale = Vector3.one * scale;
        }
    }

    public void SetTarget(Vector3 oriPos, Vector3 endPos, float duration, Action endHandler = null)
    {
        gameObject.SetActiveTrue();
        transform.position = oriPos;
        onAnimEndHandler = endHandler;

        Vector3 scale = transform.localScale;

        transform.DOScale(scale, duration).From(1);
        transform.DOMove(endPos, duration).SetEase(Ease.InQuad).OnComplete(() => GameObject.Destroy(gameObject));
    }

    public void ClearEndHandler() 
    {
        isExecute = true;
        onAnimEndHandler = null;
    }

    void onAnimEndMethod()
    {
        if (isExecute) return;

        isExecute = true;
        onAnimEndHandler?.Invoke();
    }

    private void OnDestroy()
    {
        onAnimEndMethod();
    }


}
