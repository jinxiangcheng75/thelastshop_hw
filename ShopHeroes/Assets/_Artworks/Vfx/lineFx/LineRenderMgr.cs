using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderMgr : MonoBehaviour
{
    public Transform[] Trans;
    
    public enum eCurveType
    {
        Line,
        OutIn,
    }

    public eCurveType CurveType;

    private LineRenderer _lineRenderer;
    private Material mat;
    private Animator _animation;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        mat = _lineRenderer.material;

        _animation = GetComponent<Animator>();
    }

    private void OnEnable()
    {
//        FadeIn();
        //_animation.Play("line_appear");
    }

    private void OnDisable()
    {
    }

    [ContextMenu("FadeDisableTest")]
    void FadeDisableTest()
    {
        FadeDisable(null);
    }
    public void FadeDisable(Action action)
    {
        //var clip = _animation.GetClip("line_disappear");
        //TimerHeap.AddTimer((uint) (1000 * clip.length), 0, () =>
        //{
        //});
        //StartCoroutine(DelayAction(clip.length, action));
        //_animation.Play("line_disappear");
    }

    IEnumerator DelayAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        if (action != null)
            action();
    }

//    void FadeIn()
//    {
//        DOTween.To(() => 0f, (alpha) =>
//        {
//            mat.SetFloat("_alpha", alpha);
//        }, 1f, 0.2f);
//    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] posVecs = new Vector3[Trans.Length];
        
        for (int i = 0; i < Trans.Length; i++)
        {
            if (Trans[i] == null)
                return;
            posVecs[i] = Trans[i].position;
        }

        _lineRenderer.SetPositions(posVecs);
    }
}
