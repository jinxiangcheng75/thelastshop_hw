using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SkeletonRenderer))]
public class DressUpSystem : DressUpBase
{
    protected override string repackedShaderPath => "Universal Render Pipeline/2D/Spine/Skeleton Lit";

    public override Skeleton Skeleton { get => base.Skeleton; set => _skeletonRenderer.skeleton = value; }

    public override SkeletonDataAsset SkeletonDataAsset { get => base.SkeletonDataAsset; set => _skeletonRenderer.skeletonDataAsset = value; }

    private SkeletonRenderer _skeletonRenderer;

    public SkeletonRenderer SkeletonRenderer { get { return _skeletonRenderer; } }


    public override float skeletonAlpha
    {
        get { return Skeleton.GetColor().a; }
        set
        {
            Color color = Skeleton.GetColor();
            color.a = value;
            Skeleton.SetColor(color);
        }
    }

    public override Color skeletonColor
    {
        get { return Skeleton.GetColor(); }
        set { Skeleton.SetColor(value); }
    }

    public bool activeSelf
    {
        get { return _skeletonRenderer.meshRenderer.enabled; }
    }


    protected override void Awake()
    {
        _skeletonRenderer = GetComponent<SkeletonRenderer>();

        base.Awake();
    }

    public override void Initialize(bool overwrite)
    {
        if (_skeletonRenderer != null) _skeletonRenderer.Initialize(overwrite);
    }

    public override void Clear()
    {
        base.Clear();
    }

    /// <summary>
    /// 设置角色显隐
    /// </summary>
    /// <param name="active"></param>
    public void SetActive(bool active)
    {
        _skeletonRenderer.meshRenderer.enabled = active;
    }

    /// <summary>
    /// 设置角色在UI中的位置
    /// </summary>
    /// <param name="tf">父物体</param>
    /// <param name="scale">大小</param>
    public void SetUIPosition(Transform tf, string sortingLayerName, int sortingOrder, float scale = 1)
    {
        // 获取新英雄大小60 记录一下
        _skeletonRenderer.meshRenderer.enabled = true;
        transform.SetParent(tf);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * 80 * scale;
        gameObject.ChangeLayer(LayerMask.NameToLayer("UI"));
        SetSortingAndOrderLayer(sortingLayerName, sortingOrder);


        if (isInDressing)
        {
            pubOverAllClothingEndHandler = null;
            pubOverAllClothingEndHandler += () => repacked("");
        }
        else
        {
            repacked("");//填空 换取默认材质 不受光照影响 可在UI显示
        }
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="sortingLayerName"></param>
    /// <param name="sortingOrder"></param>
    public void SetSortingAndOrderLayer(string sortingLayerName, int sortingOrder)
    {
        _skeletonRenderer.meshRenderer.sortingLayerName = sortingLayerName;
        _skeletonRenderer.meshRenderer.sortingOrder = sortingOrder;
    }
}