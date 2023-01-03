using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketSubTypeItem : MonoBehaviour
{
    public GUIIcon unSelectIcon;
    public GUIIcon selectdIcon;

    public int bigType;//大类型 0 推荐 1 武器 2 防具 3 配件 4 资源
    public int smallType; //小类型

    public Action<int, int> onSelectHandler;

    Toggle toggle;

    public void Init() 
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(ison =>
        {
            if (ison) onSelectHandler?.Invoke(bigType, smallType);
        });
    }
    
    public void SetSeleted() 
    {
        toggle.isOn = false;
        toggle.isOn = true;
    }

    public void SetUnSeleted() 
    {
        toggle.isOn = false;
        for (int i = 0; i < toggle.graphic.transform.childCount; i++)
        {
            toggle.graphic.transform.GetChild(i).gameObject.SetActive(toggle.isOn);
        }
    }

}
