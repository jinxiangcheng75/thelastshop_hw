using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSortingUti : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.GetComponentInChildren<Canvas>().sortingLayerName = "window";
    }
}
