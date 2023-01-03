using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class ToggleGroupEX : ToggleGroup
{
    private int currIndex = 0;
    public List<Toggle> togglesBtn;

    public UnityAction<int> OnSelectedIndexValueChange;
    protected override void Awake()
    {
        if (togglesBtn.Count > 0)
            togglesBtn[0].isOn = true;
        //  base.Start();
        for (int i = 0; i < togglesBtn.Count; i++)
        {
            togglesBtn[i].onValueChanged.RemoveAllListeners();
            togglesBtn[i].onValueChanged.AddListener((ison) =>
            {
                if (ison)
                {
                    OnvalueChange();
                    if (OnSelectedIndexValueChange != null)
                        OnSelectedIndexValueChange.Invoke(selectedIndex);
                }
            });
        }
        OnvalueChange();
    }
    public void StartMethod()
    {
        if (transform.name != "subtype (1)") return;
        for (int i = 0; i < transform.childCount; i++)
        {
            //if(transform.GetChild(i).name== "subTypeitem_sub(Clone)")
            //{
            togglesBtn.Add(transform.GetChild(i).GetComponent<Toggle>());
            //}
        }
        togglesBtn[0].isOn = true;
        //  base.Start();
        for (int i = 0; i < togglesBtn.Count; i++)
        {
            togglesBtn[i].onValueChanged.RemoveAllListeners();
            togglesBtn[i].onValueChanged.AddListener((ison) =>
            {
                if (ison)
                {
                    OnMyvalueChange();
                    if (OnSelectedIndexValueChange != null)
                        OnSelectedIndexValueChange.Invoke(selectedIndex);
                }
            });
        }
        OnMyvalueChange();

    }
    private void OnMyvalueChange()
    {

        foreach (Toggle toggle in togglesBtn)
        {
            for (int i = 0; i < toggle.graphic.transform.childCount; i++)
            {
                toggle.graphic.transform.GetChild(i).gameObject.SetActive(toggle.isOn);
            }
        }
    }
    private void OnvalueChange()
    {
        //float offsetX = 0;
        foreach (Toggle toggle in togglesBtn)
        {
            //(toggle.transform as RectTransform).sizeDelta = toggle.isOn ? new Vector2(260f, 105.5f) : new Vector2(213f, 91f);
            //(toggle.transform as RectTransform).anchoredPosition = new Vector2(offsetX, 0);
            for (int i = 0; i < toggle.graphic.transform.childCount; i++)
            {
                toggle.graphic.transform.GetChild(i).gameObject.SetActive(toggle.isOn);
            }

            //offsetX += toggle.isOn ? 260f : 213;
        }

    }
    public int selectedIndex
    {
        get
        {
            if (togglesBtn[currIndex].isOn) return currIndex;
            int index = 0;
            foreach (var t in togglesBtn)
            {
                if (t.isOn)
                {
                    currIndex = index;
                    return index;
                }
                else
                {
                    index++;
                }
            }
            return 0;
        }
        set
        {
            if (currIndex == value) return;
            currIndex = value;
            togglesBtn[currIndex].isOn = true;
        }
    }

    private void OnEnable()
    {
        if (currIndex < togglesBtn.Count)
        {
            togglesBtn[currIndex].isOn = true;
        }
        else
        {
            currIndex = 0;
            if (togglesBtn.Count > 0)
                togglesBtn[0].isOn = true;
        }
    }
}
