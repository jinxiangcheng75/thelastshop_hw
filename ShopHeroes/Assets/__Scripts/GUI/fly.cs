using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class fly : MonoBehaviour
{

    Action callback;
    bool isExecute;
    bool isOver;
    RectTransform self;
    float timer;
    public float time;
    public float time2;

    public RectTransform tt;

    public AnimationCurve first_XCurve;
    public AnimationCurve first_YCurve;

    public AnimationCurve secondCurve;


    Vector2 startPos;
    Vector2 endPos;


    //新
    [Header("新")]
    public float range = 150f;
    public float time_one = 0.5f;
    public float time_two_low = 0.4f;
    public float time_two_up = 0.8f;
    public float delay = 0.1f;
    public AnimationCurve AnimOneCurve;
    public AnimationCurve AnimTwoCurve;


    // Start is called before the first frame update
    void Start()
    {
        isExecute = false;
        isOver = false;
        if (tt == null) return;


        // 10月15日调整


        Animator anim = GetComponent<Animator>();

        if (anim != null)
        {
            anim.Play(0, 0, UnityEngine.Random.Range(0f, 1f));
        }

        Vector3 centerPos = new Vector3(0, -(Screen.height / 8f), 0);

        GetComponent<RectTransform>().DOAnchorPos(centerPos, 0.001f).onComplete = () =>
        {
            GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(UnityEngine.Random.Range(-range, range), centerPos.y + UnityEngine.Random.Range(-range, range), 0), time_one).onComplete = () =>
            {
                GetComponent<RectTransform>().DOMove(tt.position, UnityEngine.Random.Range(time_two_low, time_two_up)).SetEase(secondCurve).SetDelay(delay).onComplete = () =>
                {
                    callback?.Invoke();
                    isExecute = true;
                    GameObject.Destroy(this.gameObject);
                };
            };
        };


        //

        //Animator anim = GetComponent<Animator>();

        //if (anim != null)
        //{
        //    anim.Play(0, 0, UnityEngine.Random.Range(0f, 1f));
        //}


        //self = transform as RectTransform;

        //timer = 0;
        //if (time == 0) time = 1;
        //if (time2 == 0) time2 = 0.5f;

        //time = UnityEngine.Random.Range(time - 0.1f, time + 0.1f);

        //self.anchoredPosition = new Vector2(0, -(Screen.height / 8f));
        //startPos = self.anchoredPosition;
        //endPos = startPos + Vector2.right * UnityEngine.Random.Range(-600f, 600f) + Vector2.up * UnityEngine.Random.Range(50f, 200f);

    }

    //private void Update()
    //{
    //    if (isOver) return;

    //    if (timer >= time)
    //    {
    //        isOver = true;
    //        Vector2 pos = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

    //        self.DOAnchorPos(tt.anchoredPosition, UnityEngine.Random.Range(time2 - 0.1f, time2 + 0.1f)).SetEase(secondCurve).onComplete = () =>
    //        {
    //             callback?.Invoke();
    //             isExecute = true;
    //             GameObject.Destroy(this.gameObject);
    //        };
    //    }
    //    else
    //    {
    //        //Vector2 move = tt.anchoredPosition - self.anchoredPosition;
    //        float xRate = first_XCurve.Evaluate(timer / time);
    //        float yRate = first_YCurve.Evaluate(timer / time);

    //        Vector2 effectPos = new Vector2((endPos.x - startPos.x) * (xRate), (endPos.y - startPos.y) * yRate);

    //        timer += Time.deltaTime;

    //        self.anchoredPosition = startPos + effectPos;
    //    }


    //}

    public void SetCallback(Action callback)
    {
        this.callback = callback;
    }

    private void OnDestroy()
    {
        if (!isExecute)
            callback?.Invoke();
    }

}
