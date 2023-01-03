using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBatch : MonoBehaviour
{

    public bool isBatchBetter;
    public bool isBatchH;
    public bool isBatchV;


    Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (_img == null || _img.sprite == null) return;

        var defaultX = _img.sprite.rect.width;
        var defaultY =_img.sprite.rect.height;

        var sizeDelta = FGUI.inst.uiRootTF.sizeDelta;
        float x = sizeDelta.x;
        float y = sizeDelta.y;

        if (isBatchBetter) //正方形 匹配屏幕较长一侧
        {
            var len = Mathf.Max(x,y) + 100f; //多加50 防止横屏左侧露出
            _img.rectTransform.sizeDelta = Vector2.one * len;
        }
        else if (isBatchH) //等比例放大缩小 按宽等比
        {
            var scale = x / defaultX;
            _img.rectTransform.sizeDelta = new Vector2(defaultX * scale,defaultY * scale);
        }
        else if (isBatchV) //等比例放大缩小 按高等比
        {
            var scale = y / defaultY;
            _img.rectTransform.sizeDelta = new Vector2(defaultX * scale, defaultY * scale);
        }
    }

}
