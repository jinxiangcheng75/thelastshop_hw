using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class liuguangcrl : MonoBehaviour
{
    public Image image;
    public RawImage rawImage;
    public Material liuguangMaterial;
    void OnEnable()
    {
        if (image != null)
            image.material = liuguangMaterial;
        if (rawImage != null)
            rawImage.material = liuguangMaterial;
    }
    void OnDisable()
    {
        if (image != null)
            image.material = null;
        if (rawImage != null)
            rawImage.material = null;
    }
}
