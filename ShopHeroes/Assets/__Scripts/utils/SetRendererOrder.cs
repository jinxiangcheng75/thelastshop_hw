using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRendererOrder : MonoBehaviour
{
    public int noneOrder = 0;
    public Renderer _renderer;
    void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }
    }
    public void setOrder(int order, bool isExtension)
    {
        if (_renderer != null)
        {
            _renderer.sortingOrder = order + noneOrder;
            if (isExtension) setLayer("Actors_PickUp");
        }
    }

    public void setLayer(string sortingLayerName)
    {
        if (_renderer != null)
        {
            _renderer.sortingLayerName = sortingLayerName;
        }
    }
}
