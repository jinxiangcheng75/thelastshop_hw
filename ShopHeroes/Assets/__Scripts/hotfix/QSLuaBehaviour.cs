using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;

[Hotfix]
public class QSLuaBehaviour : MonoBehaviour
{
    public string m_luaName;
    public string m_luaPath;

    public Text[] m_texts;
    public Button[] m_btns;
    public Image[] m_imgs;
    public GameObject[] m_gos;
    void Awake()
    {
        XLuaManager.inst.DoString("require('" + m_luaPath + "')", m_luaName);
    }

    // Use this for initialization
    void Start()
    {

    }
}
