using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class TakePhotoToImage : MonoBehaviour
{
    public RawImage _taregetTexture;
    // Start is called before the first frame update

    private bool updateTexture = false;
    private int tKey;
    void Awake()
    {
        _taregetTexture = GetComponent<RawImage>() ?? gameObject.AddComponent<RawImage>();
        updateTexture = false;
        _taregetTexture.enabled = false;
    }

    public void setTexture(int heroKey, int sex, List<int> dressIds, List<int> equips)
    {
        if (heroKey > 0)
        {
            _taregetTexture.enabled = true;
            tKey = heroKey;
            if (TakePhoto.inst != null)
            {
                TakePhoto.inst.TakeToPhoto(heroKey, sex, dressIds, equips);
                updateTexture = true;
            }
        }
        else
        {
            _taregetTexture.enabled = false;
        }
    }

    public void ClearTexture()
    {
        if (_taregetTexture == null) return;
        _taregetTexture.texture = null;
        _taregetTexture.enabled = false;
        updateTexture = false;
    }
    public void setTexByTex(Texture2D tex)
    {
        _taregetTexture.texture = tex;
    }
    // Update is called once per frame
    void Update()
    {
        if (updateTexture && TakePhoto.inst != null)
        {
            var texture = TakePhoto.inst.GetTexture2D(tKey);
            if (texture != null)
            {
                _taregetTexture.enabled = true;
                _taregetTexture.texture = texture;
                updateTexture = false;
            }
        }
    }

    void OnDestroy()
    {
        ClearTexture();
        // if (TakePhoto.inst != null)
        // {
        //     TakePhoto.inst.clearTexture2d();
        // }
    }
}
