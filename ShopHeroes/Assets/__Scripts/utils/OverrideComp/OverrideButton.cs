using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverrideButton : Button
{

    public float holdTime;
    bool isHold;
    const float SPEED = 1.2f;
    const float FASTESTTIME = 0.008f;
    float invokeTime;
    float timer;
    float invokeCount;
    List<int> timerIds;

    Action clickCallback;

    public void SetClickCallback(Action callback) 
    {
        clickCallback = callback;
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        invokeTime = 0.36f;
        holdTime = 2;
        timer = 0;
        invokeCount = 0;
        isHold = true;
        timerIds = new List<int>();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        foreach (var item in timerIds)
            GameTimer.inst.RemoveTimer(item);

        timerIds.Clear();
        StopCoroutine("clickInvoke");

        isHold = false;

    }

    private void Update()
    {

        if (isHold)
        {
            holdTime += Time.deltaTime;
            timer += Time.deltaTime;


            if (timer >= invokeTime)
            {
                if (invokeCount <= 3) invokeTime = 0.24f;
                else invokeTime = Mathf.Pow(1 / 2.4f, holdTime);

                if (invokeTime < FASTESTTIME) invokeTime = FASTESTTIME;

                if (invokeCount <= 3)
                {
                    timerIds.Add(GameTimer.inst.AddTimerFrame(10, 4, () => clickCallback?.Invoke()));
                    invokeCount++;
                }
                else
                    StartCoroutine(clickInvoke(Mathf.FloorToInt(Mathf.Pow(holdTime, SPEED))));

                timer = 0;
            }

        }

    }

    IEnumerator clickInvoke(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return null;
            clickCallback?.Invoke();
        }
    }

}
