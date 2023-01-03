using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupMarget : ToggleGroup
{
    public List<Toggle> togglesBtn;
    private bool isInit;
    private int currIndex;
    private Vector2 _selected, _unselected;

    public Action<int> OnSelectedIndexValueChange;

    [SerializeField]
    private bool NotNeedSetSize = false, NotNeedEnable = false, IsSetOffsetX = true;
    //public bool NotNeedInvokeAction = false;

    public void SetToggleSize(Vector2 selected, Vector2 unselected, bool needEnable = true)
    {
        _selected = selected;
        _unselected = unselected;

        if (isInit)
        {
            for (int i = 0; i < togglesBtn.Count; i++)
                togglesBtn[i].onValueChanged.RemoveAllListeners();
        }

        Init();
        if (needEnable)
            OnEnableMethod();
    }


    protected override void Awake()
    {
        if (NotNeedSetSize)
        {
            Init();
        }

        if (!NotNeedEnable)
        {
            OnEnableMethod();
        }

    }

    private void Init()
    {
        isInit = true;
        currIndex = 0;
        for (int i = 0; i < togglesBtn.Count; i++)
        {
            //togglesBtn[i].onValueChanged.RemoveAllListeners();
            togglesBtn[i].onValueChanged.AddListener((ison) =>
            {
                if (ison)
                {
                    OnvalueChange();
                    OnSelectedIndexValueChange?.Invoke(selectedIndex);
                }
            });
        }
    }

    public void ClearTogglesBtn()
    {
        currIndex = 0;
        togglesBtn.Clear();
    }

    private void OnvalueChange()
    {
        if (!isInit) return;

        float offsetX = 0;

        foreach (Toggle toggle in togglesBtn)
        {
            if (!NotNeedSetSize)
            {
                (toggle.transform as RectTransform).sizeDelta = toggle.isOn ? _selected : _unselected;
                (toggle.transform as RectTransform).anchoredPosition = new Vector2(offsetX, (toggle.transform as RectTransform).anchoredPosition.y);

                offsetX += !IsSetOffsetX ? 0 : toggle.isOn ? _selected.x : _unselected.x;
            }

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

    public void onEnable()
    {
        if (togglesBtn == null || togglesBtn.Count == 0 || NotNeedEnable) return;

        OnEnableMethod();
    }

    public void OnEnableMethod(int index = 0)
    {
        /*if (index == selectedIndex)*/
        togglesBtn[selectedIndex].isOn = false;
        currIndex = index;
        togglesBtn[index].isOn = true;
    }

}
