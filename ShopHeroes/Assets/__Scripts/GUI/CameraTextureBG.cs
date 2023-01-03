using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraTextureBG : MonoBehaviour
{
    public Camera _camera;
    public RawImage _targetImage;

    void Awake()
    {
        // _camera = Camera.main;
        // _targetImage = GetComponent<RawImage>();
        // _targetImage.enabled = false;
    }
    public void showImage()
    {
        // StartCoroutine(startTake());
    }

    IEnumerator startTake()
    {
        yield return null;
        // Texture2D texture = Helper.capture(_camera, 256, 256);
        // yield return null;
        // _targetImage.enabled = true;
        // _targetImage.texture = texture;
    }
    public void hideImage()
    {
        // _targetImage.texture = null;
        // _targetImage.enabled = false;
    }
}
