using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ToggleGroupLottery : ToggleGroup
{
    [SerializeField]
    private List<Toggle> togglesBtn;
    private bool isInit;
    private int currIndex = 0;

    public Action<int> OnSelectedIndexValueChange;
    [SerializeField]
    private bool NotNeedSetSize = false, NotNeedEnable = false;
    public bool NotNeedInvokeAction = false;

    public void SetToggleEvent()
    {
        isInit = true;
        for (int i = 0; i < togglesBtn.Count; i++)
        {
            togglesBtn[i].onValueChanged.RemoveAllListeners();
            togglesBtn[i].onValueChanged.AddListener((ison) =>
            {
                if (ison)
                {
                    OnvalueChange();
                    OnSelectedIndexValueChange?.Invoke(selectedIndex);
                }
            });
        }
        togglesBtn[0].isOn = true;
    }

    private void OnvalueChange()
    {
        if (!isInit) return;

        foreach (Toggle toggle in togglesBtn)
        {
            for (int i = 0; i < toggle.graphic.transform.childCount; i++)
            {
                toggle.graphic.transform.GetChild(i).gameObject.SetActive(toggle.isOn);
            }
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

    public void OnEnable()
    {
        if (togglesBtn == null || togglesBtn.Count == 0 || NotNeedEnable) return;

        togglesBtn[selectedIndex].isOn = false;
        currIndex = 0;
        togglesBtn[0].isOn = true;
    }
}
