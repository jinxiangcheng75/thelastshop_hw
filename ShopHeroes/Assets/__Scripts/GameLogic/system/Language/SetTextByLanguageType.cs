using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetTextByLanguageType : MonoBehaviour
{
    public LanguageType languageType = LanguageType.NONE;
    private Dictionary<Text, string> translateDic;
    public Text thisText;
    public string key;

    void Awake()
    {
        translateDic = new Dictionary<Text, string>();
        Text[] texts = GetComponentsInChildren<Text>(true);

        if (LanguageManager.inst == null) return;
        foreach (var item in texts)
        {
            var translateStr = LanguageManager.inst.CheckLanguageCanChange(item.text);
            if (translateStr != "-1")
            {
                translateDic.Add(item, translateStr);
            }
        }

        //if (gameObject.name == ViewPrefabName.MainUI + "(Clone)")
        //{
        //    LanguageManager.inst.ChangeLangeuageEvent -= SetLanguageText;
        //    LanguageManager.inst.ChangeLangeuageEvent += SetLanguageText;
        //}

        if (gameObject.name == ViewPrefabName.TopPlayerInfoPanel)
        {
            var view = GUIManager.GetWindow<TopPlayerInfoView>();
            LanguageManager.inst.ChangeLangeuageEvent -= SetLanguageText;
            LanguageManager.inst.ChangeLangeuageEvent += SetLanguageText;
            if (view != null)
            {
                LanguageManager.inst.ChangeLangeuageEvent -= view.updateResUI;
                LanguageManager.inst.ChangeLangeuageEvent += view.updateResUI;
            }
        }

        if (gameObject.name == ViewPrefabName.MainUI || gameObject.name == ViewPrefabName.SettingPanel || gameObject.name == ViewPrefabName.CityUI)
        {
            LanguageManager.inst.ChangeLangeuageEvent -= SetLanguageText;
            LanguageManager.inst.ChangeLangeuageEvent += SetLanguageText;
        }
    }

    void OnEnable()
    {
        SetLanguageText();
    }
    void OnDisable()
    {

    }
    void OnDestroy()
    {
        LanguageManager.inst.ChangeLangeuageEvent -= SetLanguageText;
    }

    public void SetLanguageText()
    {
        if (LanguageManager.inst == null || languageType == LanguageManager.inst.curType) return;
        languageType = LanguageManager.inst.curType;
        Text[] texts = GetComponentsInChildren<Text>(true);
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
            LanguageManager.inst.SetFontByText(item);
        }

        foreach (var item in translateDic)
        {
            if (item.Key == null) continue;
            if (item.Key.gameObject == null)
            {
                continue;
            }
            item.Key.text = LanguageManager.inst.GetValueByKey(item.Value);
            LanguageManager.inst.SetFontByText(item.Key);
        }
    }
}
