using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkeletonGraphic))]
public class GraphicDressUpSystem : DressUpBase
{
    protected override string repackedShaderPath => "Spine/SkeletonGraphic";

    private SkeletonGraphic _skeletonGraphic;

    public SkeletonGraphic SkeletonGraphic { get { return _skeletonGraphic; } }

    public override Skeleton Skeleton { get => base.Skeleton; set => _skeletonGraphic.Skeleton = value; }

    public override SkeletonDataAsset SkeletonDataAsset { get => base.SkeletonDataAsset; set => _skeletonGraphic.skeletonDataAsset = value; }

    public override float skeletonAlpha
    {
        get { return SkeletonGraphic.color.a; }
        set
        {
            Color color = SkeletonGraphic.color;
            color.a = value;
            SkeletonGraphic.color = color;
        }
    }

    public override Color skeletonColor
    {
        get { return SkeletonGraphic.color; }
        set { SkeletonGraphic.color = value; }
    }

    protected override void Awake()
    {
        _skeletonGraphic = GetComponent<SkeletonGraphic>();
        base.Awake();

        _skeletonGraphic.raycastTarget = false;
    }

    public override void Initialize(bool overwrite)
    {
        if (_skeletonGraphic != null) _skeletonGraphic.Initialize(overwrite);
    }

    public override void Init(EGender gender, string nudeBodyAssetPath, List<int> defaultDress = null, bool initRepacked = true, Action repackedCallback = null)
    {
        base.Init(gender, nudeBodyAssetPath, defaultDress, initRepacked, repackedCallback);
        _skeletonGraphic.allowMultipleCanvasRenderers = true;
    }

    protected override void repacked(string shaderPath)
    {
        base.repacked(shaderPath);
        _skeletonGraphic.allowMultipleCanvasRenderers = false;
    }

    public override void Clear()
    {
        base.Clear();
    }

}
