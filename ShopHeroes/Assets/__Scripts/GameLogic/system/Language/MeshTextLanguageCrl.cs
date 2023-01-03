using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeshTextLanguageCrl : MonoBehaviour
{
    LanguageType curLanguageType = LanguageType.NONE;
    private Dictionary<TextMeshPro, string> translateDic;

    public string text;
    public TextMeshPro meshtext;
    void Awake()
    {
        translateDic = new Dictionary<TextMeshPro, string>();
        TextMeshPro[] textMeshes = GetComponentsInChildren<TextMeshPro>(true);

        if (LanguageManager.inst == null) return;
        foreach (var item in textMeshes)
        {
            var translateStr = LanguageManager.inst.CheckLanguageCanChange(item.text);
            if (translateStr != "-1")
            {
                translateDic.Add(item, translateStr);
            }
        }

        if (LanguageManager.inst != null)
        {
            LanguageManager.inst.ChangeLangeuageEvent += SetLanguageText;
        }
    }

    public void SetLanguageText()
    {
        if (this == null) return;
        if (!enabled) return;
        if (curLanguageType == LanguageManager.inst.curType) return;
        curLanguageType = LanguageManager.inst.curType;

        var texts = GetComponentsInChildren<TextMeshPro>(true);
        foreach (var item in texts)
        {
            if (!translateDic.ContainsKey(item))
            {
                var translateStr = LanguageManager.inst.CheckLanguageCanChange(item.text);
                if (translateStr != "-1")
                {
                    translateDic.Add(item, translateStr);
                }
            }
            LanguageManager.inst.SetFontByTextMeshPro(item);
        }

        foreach (var item in translateDic)
        {
            item.Key.text = LanguageManager.inst.GetValueByKey(item.Value);
        }
    }

    void OnEnable()
    {
        SetLanguageText();
    }

    private void OnDestroy()
    {
        LanguageManager.inst.ChangeLangeuageEvent -= SetLanguageText;
    }
}
