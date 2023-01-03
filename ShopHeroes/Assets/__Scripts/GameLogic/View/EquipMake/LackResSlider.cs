using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LackResSlider : MonoBehaviour
{
    public Slider makeBar;
    public float maxValue = 1;
    public float value = 0;
    private bool canUpdate = false;
    void Awake()
    {
        canUpdate = false;
        makeBar.gameObject.SetActive(false);
    }

    private void UpdateMakeProgress(float progress)
    {
        makeBar.value = progress;
    }

    public void RefreshMakeBar(float maxvalue, float _value)
    {
        maxValue = maxvalue;
        value = _value;
        makeBar.maxValue = maxValue;
        canUpdate = maxValue >= 0;
        UpdateMakeProgress(value);
    }

    private void Update()
    {
        if (canUpdate)
        {
            if (value < maxValue)
            {
                value += Time.deltaTime;
                UpdateMakeProgress(value);
            }
            else
            {
                canUpdate = false;
            }
        }
    }
   
}
