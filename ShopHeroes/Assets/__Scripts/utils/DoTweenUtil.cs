using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Spine.Unity;

public static class DoTweenUtil
{

    public static TweenerCore<string, string, StringOptions> DOText(this TextMeshPro target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
    {
        if (endValue == null)
        {
            if (Debugger.logPriority > 0) Debugger.LogWarning("You can't pass a NULL string to DOText: an empty string will be used instead to avoid errors");
            endValue = "";
        }
        TweenerCore<string, string, StringOptions> t = DOTween.To(() => target.text, x => target.text = x, endValue, duration);
        t.SetOptions(richTextEnabled, scrambleMode, scrambleChars)
            .SetTarget(target);
        return t;
    }

    public static void ClickTween(this Transform tf)
    {
        DOTween.Kill(tf, true);
        float symbol = tf.localScale.x >= 0 ? 1 : -1;

        var scale = tf.localScale;

        var staetScale = new Vector3(symbol * scale.x * 0.8f, scale.y * 0.8f, scale.z * 0.8f);

        tf.DOScale(staetScale, 0.1f).From(scale).OnComplete(() =>
        {
            tf.DOScale(scale * 1.2f, 0.06f).From(staetScale).OnComplete(() =>
            {
                tf.DOScale(scale, 0.1f).From(scale * 1.2f);
            });
        });

    }

    public static void ClickTween(this RectTransform tf, TweenCallback callback)
    {
        DOTween.Kill(tf, true);
        float symbol = tf.localScale.x >= 0 ? 1 : -1;

        var anim = tf.GetComponent<Animator>();
        var scale = tf.localScale;

        var staetScale = new Vector3(symbol * 0.8f, 0.8f, 0.8f);
        bool isCom = false;

        if (GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.m_curCfg != null)
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver && GuideDataProxy.inst.CurInfo.m_curCfg.btn_name != "makeButton" && GuideDataProxy.inst.CurInfo.m_curCfg.btn_name != "btn_furniture")
            {
                isCom = true;
                callback?.Invoke();
            }
        }

        tf.DOScale(staetScale, 0.05f).From(scale).OnComplete(() =>
        {
            tf.DOScale(new Vector3(symbol * 1, 1, 1), 0.03f).From(staetScale).OnComplete(() =>
            {
                if (anim != null) anim.enabled = true;
                if (GuideDataProxy.inst.CurInfo.isAllOver || !isCom)
                {
                    callback?.Invoke();
                }
                //callback?.Invoke();
            });
        }).OnStart(() =>
        {
            if (anim != null) anim.enabled = false;
        });

        //tf.DOScale(new Vector3(symbol * 0.8f, 0.8f, 0.8f), 0.1f).From(new Vector3(symbol * 1, 1, 1)).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        //{
        //    if (anim != null) anim.enabled = true;
        //    callback?.Invoke();
        //}).OnStart(() =>
        //{
        //    if (anim != null) anim.enabled = false;
        //});
    }

    public static void ButtonClickTween(this Button button, TweenCallback clickListener)
    {
        button.onClick.AddListener(() =>
        {
            (button.transform as RectTransform).ClickTween(null);
            clickListener?.Invoke();
        });
    }

    //从透明度到0
    static void FadeOutlineAndShadow_a_to_0(this Text text, float a, float duration, bool needReset = true)
    {
        var shadows = text.GetComponents<Shadow>();

        foreach (var shadow in shadows)
        {
            DOTween.Kill(shadow, true);

            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => shadow.effectColor, x => shadow.effectColor = x, 0, duration);
            t.SetTarget(shadow).From(a).OnComplete(() =>
            {
                if (!needReset) return;
                var color = shadow.effectColor;
                color.a = a;
                shadow.effectColor = color;
            });
        }

    }

    //从0到透明度
    static void FadeOutlineAndShadow_0_to_a(this Text text, float a, float duration, float delay)
    {
        var shadows = text.GetComponents<Shadow>();

        foreach (var shadow in shadows)
        {
            DOTween.Kill(shadow, true);
            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => shadow.effectColor, x => shadow.effectColor = x, a, duration).SetDelay(delay);
            t.SetTarget(shadow).From(0);
        }

    }

    /// <summary>
    /// 从透明度到0
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="a"></param>
    /// <param name="duration"></param>
    /// <param name="needReset">是否需要将透明度还原到a</param>
    public static void FadeTransparentTween(this Graphic graphic, float a, float duration, bool needReset = true)
    {
        DOTween.Kill(graphic, true);

        graphic.DOFade(0, duration).From(a).OnComplete(() =>
        {
            if (!needReset) return;
            var color = graphic.color;
            color.a = a;
            graphic.color = color;
        });

        Text text = graphic.GetComponent<Text>();
        text?.FadeOutlineAndShadow_a_to_0(a, duration, needReset);
    }

    public static void Fade_a_To_0_All(this Transform transform, float a, float duration, bool needReset = true)
    {

        var graphics = transform.GetComponentsInChildren<Graphic>();

        foreach (var item in graphics)
        {
            FadeTransparentTween(item, a, duration, needReset);
        }
    }

    /// <summary>
    /// 从0到透明度
    /// </summary>
    public static void FadeFromTransparentTween(this Graphic graphic, float a, float duration, float delay = 0)
    {

        DOTween.Kill(graphic, true);
        graphic.DOFade(a, duration).From(0).SetDelay(delay);

        Text text = graphic.GetComponent<Text>();
        text?.FadeOutlineAndShadow_0_to_a(a, duration, delay);
    }


    public static void Fade_0_To_a_All(this Transform transform, float a, float duration, float delay = 0)
    {

        var graphics = transform.GetComponentsInChildren<Graphic>();

        foreach (var item in graphics)
        {
            FadeFromTransparentTween(item, a, duration, delay);
        }
    }

    public static void MainUIBtnCommonTween_Front(RectTransform imgTf, Text text)
    {
        imgTf.DOScale(1f, 0.1f).From(0.3f).OnComplete(() =>
        {
            text.DOFade(1f, 0.3f).From(0.3f).OnStart(() =>
            {
                text.gameObject.SetActiveTrue();
            });
        }).OnStart(() =>
        {
            imgTf.gameObject.SetActiveTrue();
        });
    }

    /// <summary>
    /// dotween做的计时器
    /// </summary>
    /// <param name="timer">间隔时间</param>
    /// <param name="ticks">执行次数 -1为无限循环</param>
    /// <param name="callBack">每次的回调</param>
    /// <returns>返回计时tween</returns>
    public static Tween Timer(float timer, int ticks, TweenCallback callBack)
    {
        float temp = 0;
        return DOTween.To(() => temp, v => temp = v, 0, timer).SetLoops(ticks).OnStepComplete(callBack);
    }


    //返回重叠面积占比 rect1:rect2
    public static bool RectTransformOverlap(RectTransform rect1, RectTransform rect2, out float scale)
    {

        Vector2 a = GUIHelper.GetFGuiCameraUIPointByWorldPos(rect1.position) + rect1.rect.center;
        Vector2 b = GUIHelper.GetFGuiCameraUIPointByWorldPos(rect2.position) + rect2.rect.center;

        Rect r1 = rect1.rect;
        Rect r2 = rect2.rect;

        //轴心点为中心点 暂不需要考虑上下
        float rect1MinX = a.x + r1.xMin;
        float rect1MaxX = a.x + r1.xMax;
        float rect1MinY = a.y + r1.yMin;
        float rect1MaxY = a.y + r1.yMax;

        float rect2MinX = b.x + r2.xMin;
        float rect2MaxX = b.x + r2.xMax;
        float rect2MinY = b.y + r2.yMin;
        float rect2MaxY = b.y + r2.yMax;

        bool isOverlap = rect1MinX < rect2MaxX && rect2MinX < rect1MaxX;

        if (isOverlap)
        {
            float width = 0;

            if (rect1MinX < rect2MinX) //从左侧进入折叠
            {
                width = rect1MaxX - rect2MinX;
            }
            else  //从右侧进入折叠
            {
                width = rect2MaxX - rect1MinX;
            }

            scale = width / r2.width;
        }
        else
        {
            //Logger.error("尚未重叠");
            scale = -1;
        }

        return isOverlap;
    }


    #region 获取动画片段长度
    public static float GetClipLength(this Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
            return 0;

        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] tAnimationClips = ac.animationClips;
        if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
        AnimationClip tAnimationClip;
        for (int i = 0, tLen = tAnimationClips.Length; i < tLen; i++)
        {
            tAnimationClip = ac.animationClips[i];
            if (null != tAnimationClip && tAnimationClip.name == clip)
            {
                return tAnimationClip.length;
            }
        }

        return 0;
    }

    #endregion


    #region Spine角色渐变


    public static TweenerCore<Color, Color, ColorOptions> Fade(this DressUpBase system, float endValue, float duration)
    {
        Color color = system.skeletonColor;

        TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => color, x => system.skeletonColor = x, endValue, duration);
        t.SetTarget(color);

        return t;
    }

    #endregion

}
