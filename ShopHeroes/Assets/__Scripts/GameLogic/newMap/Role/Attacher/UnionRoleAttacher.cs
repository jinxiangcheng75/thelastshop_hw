using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UnionRoleAttacher : MonoBehaviour
{

    public TextMeshPro lvTx;
    public TextMeshPro nameTx;
    public SpriteRenderer attacherBgRender;
    public SpriteRenderer lvBgRender;
    public MeshRenderer lvTxRender;
    public MeshRenderer nameTxRender;

    public InputEventListener inputEventListener;

    public void SetClickHandler(Action<Vector3> action)
    {
        inputEventListener.OnClick = action;
    }

    public void SetInfo(int level, string nickName)
    {
        lvTx.text = level.ToString();
        nameTx.text = nickName;

        float x = Math.Max(3, nameTx.preferredWidth * nameTx.rectTransform.localScale.x + 1);
        var sizeDelta = new Vector2(x, attacherBgRender.size.y);
        attacherBgRender.size = sizeDelta;
        (attacherBgRender.transform as RectTransform).sizeDelta = sizeDelta;
    }


    public void SetLayerAndSortingOrder(string layerName, int sortingOrder)
    {
        attacherBgRender.sortingLayerName = lvBgRender.sortingLayerName = layerName;
        attacherBgRender.sortingOrder = lvBgRender.sortingOrder = sortingOrder;

        lvTxRender.sortingLayerName = nameTxRender.sortingLayerName = layerName;
        lvTxRender.sortingOrder = nameTxRender.sortingOrder = sortingOrder + 1;
    }

}
