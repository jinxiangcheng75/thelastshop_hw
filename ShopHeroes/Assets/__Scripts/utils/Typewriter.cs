using System;
using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;

/*

* 功能：文字打字机

*/

public class Typewriter : BaseMeshEffect

{

    //用于记录当前到了哪个字
    private int m_Num = 0;
    //总字数
    private int m_StrNum = 0;
    //记录刷新时间
    private float m_TimeCount = 0;
    //是否开始显示打字
    private bool isStartShow = false;
    //要显示的字符串
    private string m_ShowString = "";
    //每个字的出现间隔
    private float m_TimeSpace = 0;
    //要显示的Text组件
    private Text m_Text;
    private Outline m_outline;
    private Action _endCompeleteCallback;
    private Action<float> _stepCallback;


    protected override void Awake()
    {
        m_Text = gameObject.GetComponent<Text>();
        m_outline = gameObject.GetComponent<Outline>();
    }

    /*

      在打字状态时，每个时间间隔更新一次

    */

    void Update()
    {

        if (isStartShow)
        {

            if (Time.time - m_TimeCount > m_TimeSpace)
            {

                m_TimeCount = Time.time;

                if (m_Num < m_StrNum)
                {
                    m_Num++;
                    _stepCallback?.Invoke((float)m_Num / m_StrNum);
                }

                else

                {

                    m_Num = m_StrNum;
                    m_StrNum = 0;

                    isStartShow = false;

                    if (_endCompeleteCallback != null)
                    {
                        _endCompeleteCallback.Invoke();
                        _endCompeleteCallback = null;
                        _stepCallback = null;
                    }


                }

                graphic.SetVerticesDirty();

            }

        }

    }

    /*

        重写ModifyMesh方法，更新文字顶点

    */

    bool m_StrNumInit;

    public override void ModifyMesh(VertexHelper vh)
    {

        if (!IsActive() || !isStartShow) return;

        List<UIVertex> vertexList = new List<UIVertex>();

        vh.GetUIVertexStream(vertexList);

        if (m_StrNumInit)
        {
            m_StrNumInit = false;
            m_StrNum = vertexList.Count / 6;

        }

        vertexList = SetTextVertex(vertexList);
        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);

    }

    private List<UIVertex> SetTextVertex(List<UIVertex> vertexList)
    {

        List<UIVertex> tmpVertexList = new List<UIVertex>();

        int count = m_Num * 6;

        count = Mathf.Min(vertexList.Count, count);

        for (int i = 0; i < count; i++)
        {
            //var vertex = vertexList[i];
            tmpVertexList.Add(vertexList[i]);
        }

        return tmpVertexList;
    }


    private void InitTweenStringInfo()
    {

        if (m_outline != null && m_outline.enabled)
        {
            m_outline.enabled = false;
            Logger.log("typewriter 已将outline组件失活");
        }

        m_TimeCount = Time.time;

        m_Num = 0;

        m_StrNum = 0;

        m_ShowString = "";

        m_Text.text = string.Empty;

    }

    public void StartTweenString(string str, float time, Action endCallback = null, Action<float> stepCallback = null)
    {
        if (string.IsNullOrEmpty(str))
        {
            return;
        }

        InitTweenStringInfo();

        _endCompeleteCallback = endCallback;
        _stepCallback = stepCallback;
        isStartShow = true;
        m_StrNumInit = true;

        m_TimeSpace = time < 0 ? 0.02f : time;
        m_ShowString = str;
        m_Text.text = m_ShowString;

    }

    public void StopTweenString()
    {
        isStartShow = false;
        m_StrNum = 0;
    }

    public void GoToEnd()
    {
        if (m_StrNum > 0)
        {
            m_Num = m_StrNum - 1;
            //if (_endCompeleteCallback != null)
            //{
            //    _endCompeleteCallback.Invoke();
            //    _endCompeleteCallback = null;
            //}
        }
        isStartShow = true;
    }

}