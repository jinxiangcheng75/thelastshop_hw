using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopTypeItemComp : MonoBehaviour, IPointerClickHandler
{
    public Action<uint, uint, GameObject> changeTypeHandler;

    public Image typeImg;
    public Image icon;
    public GameObject colorLogo;
    private ShopkeeperData data;
    public uint bigType;

    public ShopkeeperData Data { get { return data; } set { } }

    public void InitData(ShopkeeperData data, uint bigType)
    {
        this.data = data;

        //if (bigType == 1 && (data.typeStr == 2 || data.typeStr == 4 || data.typeStr == 5 || data.typeStr == 6 || data.typeStr == 7))
        //{
        //    gameObject.SetActive(false);
        //}

        if (bigType == 1 && (data.typeStr == 1 || data.typeStr == 3 || data.typeStr == 5 || data.typeStr == 7 || data.typeStr == 8))
        {
            colorLogo.SetActive(true);
        }
        else
        {
            colorLogo.SetActive(false);
        }

        this.bigType = bigType;
        icon.GetComponent<GUIIcon>().SetSprite(data.atlasName, data.iconName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        changeTypeHandler?.Invoke(bigType, data.typeStr, this.gameObject);
    }
}
