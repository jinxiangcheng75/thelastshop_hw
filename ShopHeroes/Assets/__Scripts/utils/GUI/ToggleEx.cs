using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class ToggleEx : MonoBehaviour
{
    public List<GameObject> checkmarks;
    void Start()
    {
        var toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(valuechanged);
    }

    void valuechanged(bool on)
    {
        checkmarks.ForEach(item =>
        {
            item.SetActive(on);
        });
    }
}
