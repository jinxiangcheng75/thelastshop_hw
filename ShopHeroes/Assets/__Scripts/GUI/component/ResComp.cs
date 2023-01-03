using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResComp : MonoBehaviour
{
    public GUIIcon icon;
    public Text count;
    public Slider makeBar;
    public float maxValue = 1;
    public float value = 0;
    public bool isFull = true;
    private bool canUpdate = false;
    void Awake()
    {
        canUpdate = false;
        makeBar.gameObject.SetActive(false);
    }
    public void UpdateMakeProgress(float progress)
    {
        makeBar.value = progress;
    }

    public void RefreshMakeBar(bool full, float maxvalue, float _value)
    {
        isFull = full;
        maxValue = maxvalue;
        value = _value;
        makeBar.maxValue = maxValue;
        canUpdate = !isFull && maxValue >= 0;
        UpdateMakeProgress(value);
        makeBar.gameObject.SetActive(!isFull);
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
                makeBar.gameObject.SetActive(false);
                canUpdate = false;
            }
        }
    }
    private string currIconAtlasUrl = "";
    private string currIconName = "";
    public void setIcon(string atlasname, string iconname)
    {
        currIconAtlasUrl = atlasname;
        currIconName = iconname;
        if (icon == null)
            icon = GetComponent<GUIIcon>();
        icon.SetSprite(atlasname, iconname);
    }
}
