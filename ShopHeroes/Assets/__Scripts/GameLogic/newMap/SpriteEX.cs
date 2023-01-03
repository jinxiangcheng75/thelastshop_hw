using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteEX : MonoBehaviour
{
    GSprite _gsprite;

    public GSprite mGSprite
    {
        get { return _gsprite; }
        set
        {
            if (_gsprite != null)
            {
                _gsprite.release();
            }
            _gsprite = value;
            if (_gsprite != null && _gsprite.sprite != null)
            {
                showSprite(_gsprite.sprite);
            }
            else
            {
                showSprite(null);
                //iconImage.sprite = Resources.Load<Sprite>("define");
            }
        }
    }
    SpriteRenderer rander;
    void Start()
    {
        if (rander == null)
        {
            rander = GetComponent<SpriteRenderer>();
        }
        //  showSprite();
    }
    public void showSprite(Sprite sprite)
    {
        if (rander == null)
        {
            rander = GetComponent<SpriteRenderer>();
        }

        rander.sprite = sprite;
        // if (!string.IsNullOrEmpty(_url))
        // {
        //     ManagerBinder.inst.Asset.loadMiscAsset<Sprite>(_url, (sprite) =>
        //     {
        //         if (sprite != null)
        //         {
        //             if (rander != null)
        //             {
        //                 rander.sprite = sprite;
        //             }
        //         }
        //     });
        // }
    }
    void OnDestroy()
    {
        if (_gsprite != null)
        {
            _gsprite.release();
        }
    }
}
