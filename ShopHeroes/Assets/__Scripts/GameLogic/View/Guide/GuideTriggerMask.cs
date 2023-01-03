using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideTriggerMask : MaskableGraphic, ICanvasRaycastFilter
{
    private RectTransform inner_trans;
    private RectTransform outer_trans;//背景区域

    private Vector2 inner_rt;//镂空区域的右上角坐标
    private Vector2 inner_lb;//镂空区域的左下角坐标
    private Vector2 outer_rt;//背景区域的右上角坐标
    private Vector2 outer_lb;//背景区域的左下角坐标

    [Header("是否实时刷新")]
    [Space(25)]
    public bool realtimeRefresh;

    public GameObject preMask;
    public SpriteRenderer finger;
    public RectTransform fingerRect;

    public bool isReRefresh = true;

    protected override void Awake()
    {
        base.Awake();

        outer_trans = GetComponent<RectTransform>();
        if (finger != null)
        {
            finger.sortingOrder = 9999;
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (inner_trans == null)
        {
            base.OnPopulateMesh(vh);
            return;
        }

        vh.Clear();

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        //0 outer左下角
        vertex.position = new Vector3(outer_lb.x, outer_lb.y);
        vh.AddVert(vertex);
        //1 outer左上角
        vertex.position = new Vector3(outer_lb.x, outer_rt.y);
        vh.AddVert(vertex);
        //2 outer右上角
        vertex.position = new Vector3(outer_rt.x, outer_rt.y);
        vh.AddVert(vertex);
        //3 outer右下角
        vertex.position = new Vector3(outer_rt.x, outer_lb.y);
        vh.AddVert(vertex);
        //4 inner左下角
        vertex.position = new Vector3(inner_lb.x, inner_lb.y);
        vh.AddVert(vertex);
        //5 inner左上角
        vertex.position = new Vector3(inner_lb.x, inner_rt.y);
        vh.AddVert(vertex);
        //6 inner右上角
        vertex.position = new Vector3(inner_rt.x, inner_rt.y);
        vh.AddVert(vertex);
        //7 inner右下角
        vertex.position = new Vector3(inner_rt.x, inner_lb.y);
        vh.AddVert(vertex);

        //绘制三角形
        vh.AddTriangle(0, 1, 4);
        vh.AddTriangle(1, 4, 5);
        vh.AddTriangle(1, 5, 2);
        vh.AddTriangle(2, 5, 6);
        vh.AddTriangle(2, 6, 3);
        vh.AddTriangle(6, 3, 7);
        vh.AddTriangle(4, 7, 3);
        vh.AddTriangle(0, 4, 3);
    }

    /// <summary>
    /// 过滤掉射线检测
    /// </summary>
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        if (inner_trans == null)
        {
            return true;
        }
        return !RectTransformUtility.RectangleContainsScreenPoint(inner_trans, screenPos, eventCamera);
    }

    /// <summary>
    /// 计算边界
    /// </summary>
    private void CalcBounds()
    {
        if (inner_trans == null)
        {
            return;
        }
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(outer_trans, inner_trans);
        inner_rt = bounds.max;
        inner_lb = bounds.min;
        outer_rt = outer_trans.rect.max;
        outer_lb = outer_trans.rect.min;
    }

    public void ShowGMask(RectTransform targetRect)
    {
        inner_trans = targetRect;
        gameObject.SetActiveTrue();
        CalcBounds();

        if (preMask != null)
        {
            preMask.SetActive(false);
        }
    }
    Transform target;
    string targetOffset;
    public void ShowFinger(Transform targetTrans, string offset)
    {
        if (finger == null || fingerRect == null)
        {
            return;
        }

        finger.sortingOrder = 9999;
        fingerRect.gameObject.SetActive(true);
        target = targetTrans;
        targetOffset = offset;

        float offsetX = 0, offsetY = 0, offsetRotateZ = 0;
        if (offset != null)
        {
            var splitStr = offset.Split('|');
            if (0 < splitStr.Length)
                offsetX = float.Parse(splitStr[0]);
            if (1 < splitStr.Length)
                offsetY = float.Parse(splitStr[1]);
            if (2 < splitStr.Length)
                offsetRotateZ = float.Parse(splitStr[2]);
        }

        Vector2 v2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), FGUI.inst.uiCamera.WorldToScreenPoint(targetTrans.position), FGUI.inst.uiCamera, out v2);
        var rectTrans = targetTrans.GetComponent<RectTransform>();
        float x = 0;
        float y = 0;
        float height = 0;
        if (rectTrans != null)
        {
            Vector2 pivot = rectTrans.pivot;
            if (targetTrans.localScale.x > 0)
            {
                if (pivot.x > 0.5f)
                {
                    x = -(rectTrans.rect.width * (pivot.x - 0.5f));
                }
                else if (pivot.x < 0.5f)
                {
                    x = (rectTrans.rect.width * (0.5f - pivot.x));
                }
                if (pivot.y > 0.5f)
                {
                    y = -(rectTrans.rect.height * (pivot.y - 0.5f));
                }
                else if (pivot.y > 0.5f)
                {
                    y = (rectTrans.rect.height * (0.5f - pivot.y));
                }
            }
            else
            {
                if (pivot.x < 0.5f)
                {
                    x = (rectTrans.rect.width * (pivot.x - 0.5f));
                }
                else if (pivot.x > 0.5f)
                {
                    x = -(rectTrans.rect.width * (0.5f - pivot.x));
                }
                if (pivot.y > 0.5f)
                {
                    y = -(rectTrans.rect.height * (pivot.y - 0.5f));
                }
                else if (pivot.y > 0.5f)
                {
                    y = (rectTrans.rect.height * (0.5f - pivot.y));
                }
            }

            height = rectTrans.rect.height / 2;
        }

        fingerRect.anchoredPosition = new Vector3(v2.x + x + offsetX, v2.y + y/* + height*/ + offsetY, 0);
        fingerRect.transform.eulerAngles = new Vector3(0, 0, offsetRotateZ);
    }

    public void HideFinger()
    {
        fingerRect.gameObject.SetActive(false);
        target = null;
    }

    public float reSetactiveTime = 0;
    private void Update()
    {
        reSetactiveTime += Time.deltaTime;
        if (isReRefresh)
        {
            if (realtimeRefresh == false)
            {
                return;
            }

            if (reSetactiveTime >= 2)
            {
                //gameObject.SetActive(false);
                //gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform as RectTransform);
                reSetactiveTime = 0;
            }
            //计算边界
            CalcBounds();
            //刷新
            SetAllDirty();

            if (target != null /*&& targetOffset != null*/)
            {
                ShowFinger(target, targetOffset);
            }
            if (inner_trans == null)
            {
                isReRefresh = false;
            }
        }
    }
}
