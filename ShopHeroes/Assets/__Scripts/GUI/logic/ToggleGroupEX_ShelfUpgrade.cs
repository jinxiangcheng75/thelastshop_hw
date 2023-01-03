using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleGroupEX_ShelfUpgrade : MonoBehaviour
{
    public GameObject yellowBG;
    public GameObject brownBG;
    public Text text;

    private void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnValueChanged);//监听方法是Bool委托
    }

    private void Start()
    {
        OnValueChanged(GetComponent<Toggle>().isOn);//设置Toggle的初始状态，IsOn值初始默认为false
    }

    private void OnValueChanged(bool value)
    {
        transform.GetChild(0).gameObject.SetActive(value);//获取isOn 的值 控制子物体 的显示隐藏

        if (value)//如果为True执行什么内容
        {
            //brownBG.SetActive(false);
            yellowBG.SetActive(true);

            text.color = GUIHelper.GetColorByColorHex("#653727");
        }
        else
        {
            yellowBG.SetActive(false);
            //brownBG.SetActive(true);

            text.color= GUIHelper.GetColorByColorHex("#613E28");
        }

    }
}
