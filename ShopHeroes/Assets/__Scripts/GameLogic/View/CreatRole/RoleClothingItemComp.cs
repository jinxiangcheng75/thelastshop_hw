using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoleClothingItemComp : MonoBehaviour, IPointerClickHandler
{
    public Action<FacadeType, RoleClothingItemComp> typeClickHandle;

    public Image selectImg;
    public Image icon;
    public GameObject colorLogo;

    private RoleTypeData _data;

    public RoleTypeData Data
    {
        get { return _data; }
        set { }
    }

    public void InitData(RoleTypeData data, EGender sex)
    {
        _data = data;

        //if (data.typeStr == 2 || data.typeStr == 4 || data.typeStr == 5 || data.typeStr == 6 || data.typeStr == 7)
        //{
        //    gameObject.SetActive(false);
        //}

        if (data.typeStr == 1 || data.typeStr == 3 || data.typeStr == 5 || data.typeStr == 7 || data.typeStr == 8)
        {
            colorLogo.SetActive(true);
        }
        else
        {
            colorLogo.SetActive(false);
        }

        icon.GetComponent<GUIIcon>().SetSprite(data.atlasName, data.iconName);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        typeClickHandle?.Invoke((FacadeType)_data.typeStr, this);
    }
}
