using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverrideAnimatorButton : Button
{
    public Animator selfAnimator;
    public bool isItem = false;
    float time;

    protected override void Awake()
    {
        selfAnimator = gameObject.GetComponent<Animator>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!interactable) return;
        if (!isItem)
        {
            selfAnimator.SetTrigger("Pressed");
        }
        else
        {
            selfAnimator.Play("clickDown");
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (!interactable) return;
        if (!isItem)
        {
            selfAnimator.SetTrigger("Selected");
        }
        else
        {
            var animInfo = selfAnimator.GetCurrentAnimatorStateInfo(0);
            if (animInfo.normalizedTime >= 1.0f && animInfo.IsName("clickDown"))
            {
                selfAnimator.Play("clipUp");
            }
            else
            {
                selfAnimator.Play("clickDown"/*, 0, animInfo.normalizedTime*/);
                GameTimer.inst.AddTimer(animInfo.length, 1, () =>
                 {
                     selfAnimator.Play("clipUp");
                 });
            }
        }
    }

    public void setTrigger(string triggerName)
    {
        if (selfAnimator != null)
            selfAnimator.SetTrigger(triggerName);
    }
}
