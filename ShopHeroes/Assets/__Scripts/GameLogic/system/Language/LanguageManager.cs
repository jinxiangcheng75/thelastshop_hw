using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public delegate void ChangeLanguage();

public class LanguageManager : SingletonMono<LanguageManager>
{
    public LanguageType curType;

    public Font curFont;
    public TMP_FontAsset curTmProFont;
    public Font CN_Font;
    public Font TC_Font;
    public Font EN_Font;
    public TMP_FontAsset CN_Tmp_Font;
    public TMP_FontAsset TC_Tmp_Font;
    public TMP_FontAsset EN_Tmp_Font;

    private const string NonBreakingSpace = "\u00A0";//不换行空格的Unicode编码
    private const string NextLine = "\\n"; //换行符

    public event ChangeLanguage ChangeLangeuageEvent;

    public void Clear()
    {
        ChangeLangeuageEvent = null;
    }

    void Start()
    {
        //if (PlayerPrefs.HasKey("languageType"))
        //{
        //    var curType = (LanguageType)Enum.Parse(typeof(LanguageType), PlayerPrefs.GetString("languageType"), false);
        //    LanguageState(curType);
        //}
        //else
        //{
        //    //SDKManager.inst.GetLanguage();
        //}
        var curType = LanguageType.SIMPLIFIED_CHINESE;
        if (PlayerPrefs.HasKey("languageType"))
        {
            curType = (LanguageType)Enum.Parse(typeof(LanguageType), PlayerPrefs.GetString("languageType"), false);
        }
        LanguageState(curType);
    }
    public string GetValueByKey(string key, params string[] paras)
    {
        if (key == null)
        {
            Debug.LogError("LanguageManager2 翻译传入key 值无效！！");
            return "";
        }
        if (string.IsNullOrEmpty(key))
        {
            return "";
        }

        if (paras == null)
        {
            Debug.LogError("LanguageManager.GetValueByKey 翻译传入 paras 值无效！！当前 key == " + key);
            return key;
        }
        string value = "";
        switch (curType)
        {
            case LanguageType.SIMPLIFIED_CHINESE:
                {
                    value = LanguageConfigManager.inst.GetCNLanguageConfig(key);
                    break;
                }
            case LanguageType.TRADITIONAL_CHINESE:
                {
                    value = LanguageConfigManager.inst.GetTRCNLanguageConfig(key);
                    break;
                }
            case LanguageType.ENGLISH:
                {
                    value = LanguageConfigManager.inst.GetENGLanguageConfig(key);
                    break;
                }
            default:
                {
                    Debug.LogError("未找到对应语言的类型！");
                    break;
                }
        }
        if (paras.Length <= 0) return value;
        if (value.Contains("{0}") && 0 < paras.Length)
        {
            value = value.Replace("{0}", paras[0]);
        }

        if (value.Contains("{1}") && 1 < paras.Length)
        {
            value = value.Replace("{1}", paras[1]);
        }

        if (value.Contains("{2}") && 2 < paras.Length)
        {
            value = value.Replace("{2}", paras[2]);
        }

        if (value.Contains("{3}") && 3 < paras.Length)
        {
            value = value.Replace("{3}", paras[3]);
        }

        if (value.Contains("{4}") && 4 < paras.Length)
        {
            value = value.Replace("{4}", paras[4]);
        }

        value.Replace(" ", NonBreakingSpace);
        //if (value)
        value = value.Replace(NextLine, "\n");
        return value;
    }

    public string GetValueByKey(string key)
    {
        if (key == null)
        {
            //  Debug.LogError("LanguageManager1 翻译传入key 值无效！！");
            return "";
        }
        if (string.IsNullOrEmpty(key))
        {
            return "";
        }
        string value = "";
        if (LanguageConfigManager.inst == null) return key;
        switch (curType)
        {
            case LanguageType.SIMPLIFIED_CHINESE:
                {
                    value = LanguageConfigManager.inst.GetCNLanguageConfig(key.IfNullThenEmpty());
                    break;
                }
            case LanguageType.TRADITIONAL_CHINESE:
                {
                    value = LanguageConfigManager.inst.GetTRCNLanguageConfig(key.IfNullThenEmpty());
                    break;
                }
            case LanguageType.ENGLISH:
                {
                    value = LanguageConfigManager.inst.GetENGLanguageConfig(key.IfNullThenEmpty());
                    break;
                }
            default:
                {
                    Debug.LogError("未找到对应语言的类型！");
                    value = key;
                    break;
                }
        }

        value = value.IfNullThenEmpty().Replace(" ", NonBreakingSpace);
        value = value.Replace(NextLine, "\n");
        return value;
    }

    public string CheckLanguageCanChange(string key)
    {
        return LanguageConfigManager.inst.CheckContentLanguage(key);
    }

    //这个方法将会设置客户端内的语言类型
    public void LanguageState(LanguageType type)
    {
        if (curType == type) return;
        curType = type;
        PlayerPrefs.SetString("languageType", type.ToString());
        SetFont();
        ChangeLangeuageEvent?.Invoke();
    }

    //设置字体
    public void SetFont()
    {
        switch (curType)
        {
            case LanguageType.SIMPLIFIED_CHINESE:
                {
                    curFont = CN_Font;
                    curTmProFont = CN_Tmp_Font;
                    break;
                }
            case LanguageType.TRADITIONAL_CHINESE:
                {
                    curFont = TC_Font;
                    curTmProFont = TC_Tmp_Font;
                    break;
                }
            case LanguageType.ENGLISH:
                {
                    curFont = EN_Font;
                    curTmProFont = EN_Tmp_Font;
                    break;
                }
            default:
                {
                    //Debug.LogError("未找到对应语言的类型！");
                    break;
                }
        }
    }

    public void SetFontByText(Text text)
    {
        if (text != null)
            text.font = curFont;
    }

    public void SetFontByTextMeshPro(TextMeshPro tmPro)
    {
        tmPro.font = curTmProFont;
        tmPro.fontStyle = FontStyles.Normal;
    }
}

public enum LanguageType
{
    SIMPLIFIED_CHINESE,//简体中文
    TRADITIONAL_CHINESE,//繁体中文
    ENGLISH,//英文

    NONE,
}