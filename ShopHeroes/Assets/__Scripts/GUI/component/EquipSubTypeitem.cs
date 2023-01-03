using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipSubTypeitem : MonoBehaviour
{
    [HideInInspector]
    public EquipSubType type;
    public GUIIcon typeIcon;
    public GUIIcon Icon;
    public int index = 1;

    public GameObject redPoint;
    public System.Action<int> OnSelect;
    private Toggle te;

    bool init;

    private void Awake()
    {
        if (init) return;
        te = this.GetComponent<Toggle>();
        te.onValueChanged.AddListener(onToggleValueChange);
        init = true;
    }

    public void setSelect()
    {
        if (!init)
        {
            te = this.GetComponent<Toggle>();
            te.onValueChanged.AddListener(onToggleValueChange);
        }

        te.isOn = true;


    }
    private void onToggleValueChange(bool value)
    {
        if (value)
        {
            OnSelect?.Invoke(index);
        }
    }

    public void setRed(bool show)
    {
        redPoint.SetActive(show);
    }
}
