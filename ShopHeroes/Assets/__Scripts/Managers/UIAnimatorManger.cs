using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimatorCrl
{
    public Animator _animator;
    public delegate void AnimatorOnPlayComplete();
    private AnimatorOnPlayComplete onPlayComplete;
    private GameAnimationEvent playEvent;
    public AnimatorCrl(Animator animator)
    {
        _animator = animator;
        //onPlayComplete = onplayend;
        playEvent = _animator.gameObject.GetComponent<GameAnimationEvent>() ?? _animator.gameObject.AddComponent<GameAnimationEvent>();
        playEvent.onPlayStop = onplayend;
    }

    public AnimatorOnPlayComplete OnComplete
    {
        get
        {
            return onPlayComplete;
        }
        set
        {
            onPlayComplete = value;
        }
    }

    private void onplayend()
    {
        if (onPlayComplete != null)
        {
            onPlayComplete();
            onPlayComplete = null;
        }
    }
}
public class UIAnimatorManger : SingletonMono<UIAnimatorManger>
{
    private List<AnimatorCrl> animatorList = new List<AnimatorCrl>();
    public AnimatorCrl SetAnimatorValue(Animator animator, string parametername, int value)
    {
        int index = animatorList.FindIndex(Crl => Crl._animator == animator);
        if (index >= 0)
        {
            animator.SetInteger(parametername, value);
            return animatorList[index];
        }
        AnimatorCrl crl = new AnimatorCrl(animator);
        animator.SetInteger(parametername, value);
        animatorList.Add(crl);
        return crl;
    }

    public AnimatorCrl SetAnimatorValue(Animator animator, string parametername, float value)
    {
        int index = animatorList.FindIndex(Crl => Crl._animator == animator);
        if (index >= 0)
        {
            animator.SetFloat(parametername, value);
            return animatorList[index];
        }
        AnimatorCrl crl = new AnimatorCrl(animator);
        animator.SetFloat(parametername, value);
        animatorList.Add(crl);
        return crl;
    }
    public AnimatorCrl SetAnimatorValue(Animator animator, string parametername, bool value)
    {
        int index = animatorList.FindIndex(Crl => Crl._animator == animator);
        if (index >= 0)
        {
            animator.SetBool(parametername, value);
            return animatorList[index];
        }
        AnimatorCrl crl = new AnimatorCrl(animator);
        animator.SetBool(parametername, value);
        animatorList.Add(crl);
        return crl;
    }

    public AnimatorCrl SetAnimatorValue(Animator animator, string parametername)
    {
        int index = animatorList.FindIndex(Crl => Crl._animator == animator);
        if (index >= 0)
        {
            animator.SetTrigger(parametername);
            return animatorList[index];
        }
        AnimatorCrl crl = new AnimatorCrl(animator);
        animator.SetTrigger(parametername);
        animatorList.Add(crl);
        return crl;
    }

    // List<AnimatorCrl> removeList = new List<AnimatorCrl>();
    // void Update()
    // {
    //     foreach (var crl in animatorList)
    //     {
    //         AnimatorStateInfo stateInfo = crl._animator.GetCurrentAnimatorStateInfo(0);
    //         if (stateInfo.normalizedTime >= 0.005f)
    //         {
    //             crl.OnComplete();
    //             removeList.Add(crl);
    //         }
    //     }
    //     if (removeList.Count > 0)
    //     {
    //         foreach (var crl in removeList)
    //         {
    //             animatorList.Remove(crl);
    //         }
    //         removeList.Clear();
    //     }
    // }
}
