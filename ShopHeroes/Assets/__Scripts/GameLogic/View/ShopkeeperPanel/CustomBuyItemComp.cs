using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomBuyItemComp : MonoBehaviour
{
    public Button deleteBtn;
    public Image iconImage;
    public Text goodText;
    public Text priceText;
    public GameObject gemIcon;
    public GameObject glodIcon;
    public GameObject maskObj;
    private RoleSubTypeData _data;
    public RoleSubTypeData data
    {
        get { return _data; }
        set { }
    }

    public void InitData(RoleSubTypeData data, EGender curSex)
    {
        gameObject.SetActive(true);
        _data = data;

        if (data.config.price_money > 0)
        {
            glodIcon.SetActive(true);
            gemIcon.SetActive(false);
            priceText.text = data.config.price_money.ToString();
        }
        else if (data.config.price_diamond > 0)
        {
            gemIcon.SetActive(true);
            glodIcon.SetActive(false);
            priceText.text = data.config.price_diamond.ToString();
        }
        goodText.text = LanguageManager.inst.GetValueByKey(data.config.name);

        string iconName = data.config.icon;
        iconImage.GetComponent<GUIIcon>().SetSprite("ClotheIcon_atlas", iconName);
    }

    public void ClearData()
    {
        if (this == null) return;
        if (gameObject != null)
            gameObject.SetActive(false);
        if (_data != null)
            _data = null;
        if (deleteBtn != null)
            deleteBtn.onClick.RemoveAllListeners();
    }
}
