using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class GUIIcon : MonoBehaviour
{
    public Sprite defaultIcon;

    Image _image = null;
    public Image iconImage
    {
        get
        {
            if (isInvalid) return null;
            if (_image == null)
            {
                if (gameObject != null)
                    _image = GetComponent<Image>();
            }
            return _image;
        }
    }
    void Start()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
            // if (_image != null)
            // {
            //     _image.sprite = null;
            //     _image.enabled = false;
            // }
            this.enabled = false;
            //_image.enabled = false;
            isInvalid = false;
        }
    }
    void OnDestroy()
    {
        clear();
    }
    string atlas;
    string spriteName;
    string _assetUrl;
    bool needSetNativeSize;

    GSprite _gsprite;
    GSprite mGSprite
    {
        get { return _gsprite; }
        set
        {
            if (iconImage == null) return;
            if (_gsprite != null)
            {
                _gsprite.release();
            }
            _gsprite = value;
            if (_gsprite != null && _gsprite.sprite != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = _gsprite.sprite;
            }
            else
            {
                iconImage.enabled = false;
                //iconImage.sprite = Resources.Load<Sprite>("define");
            }
        }
    }

    public void SetSprite(string _atlas, string _spritename, string outlinecolor = "", bool isBigSprite = false, bool needSetNativeSize = false)
    {
        if (iconImage == null) return;
       
        if (_image != null)
        {
            _image.sprite = null;
            _image.enabled = false;
        }
        if (string.IsNullOrEmpty(_atlas) || string.IsNullOrEmpty(_spritename)) return;
        this.needSetNativeSize = needSetNativeSize;
        atlas = _atlas;
        spriteName = _spritename;
        if (string.IsNullOrEmpty(_spritename))
        {
            iconImage.sprite = defaultIcon;
            return;
        }
        AtlasAssetHandler.inst.GetAtlasSprite(_atlas, _spritename, (gsprite) =>
        {
            if (iconImage == null) return;
            if (spriteName != _spritename || atlas != _atlas) return; //只接纳最后一次的SetSprite
            mGSprite = gsprite;
            if (needSetNativeSize)
                iconImage.SetNativeSize();
        });

        SetOutlineColor(outlinecolor, isBigSprite);
    }
    public void SetSpriteURL(string assetUrl, string outlinecolor = "", bool isBigSprite = false, bool needSetNativeSize = false)
    {
        if (iconImage == null) return;
        if (_image != null)
        {
            _image.sprite = null;
            _image.enabled = false;
        }
        if (string.IsNullOrEmpty(assetUrl))
        {
            iconImage.sprite = defaultIcon;
            return;
        }
        _assetUrl = assetUrl;
        // iconImage.enabled = false;
        // ManagerBinder.inst.Asset.loadMiscAsset<Sprite>(assetUrl, (sprite) =>
        // {
        //     iconImage.sprite = sprite != null ? sprite : Resources.Load<Sprite>("define");
        //     if (needSetNativeSize)
        //         iconImage.SetNativeSize();
        //     iconImage.enabled = true;
        // });
        ManagerBinder.inst.Asset.getSpriteAsync(assetUrl, (gsprite) =>
        {
            if (iconImage == null) return;
            if (_assetUrl != assetUrl) return;  //只接纳最后一次的SetSpriteURL
            mGSprite = gsprite;
            if (needSetNativeSize)
                iconImage.SetNativeSize();
        });
        SetOutlineColor(outlinecolor, isBigSprite);
    }
    bool isInvalid = false;
    void OnEnable()
    {
        isInvalid = false;
    }
    void Awake()
    {
        isInvalid = false;
    }
    public void SetOutlineColor(string outlinecolor = "", bool isBigSprite = false)
    {
        //判断是否有描边
        if (!string.IsNullOrEmpty(outlinecolor))
        {
            if (outlinecolor == GUIHelper.NOTNEEDCLEARMAT) return; //置灰后不再取消
            Material mat = new Material(GUIHelper.GetOutlineMat());
            mat.SetColor("_OutlineColor", GUIHelper.GetColorByColorHex(outlinecolor));
            mat.SetFloat("_Width", isBigSprite ? 8f : 1f);
            iconImage.material = mat;
        }
        else
        {
            iconImage.material = null;
        }
    }

    public void clear()
    {
        isInvalid = true;
        if (iconImage == null) return;

        if (_gsprite != null)
        {
            _gsprite.release();
        }
        _gsprite = null;
        if (iconImage == null) return;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }
}
