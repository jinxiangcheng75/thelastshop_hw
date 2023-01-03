using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameAnimationEvent : MonoBehaviour
{
    public Action onPlayStart;
    public Action onPlayStop;
    public void OnAnimation_Start()
    {
        Logger.log("GameAnimationEvent 开始动画");
        if (onPlayStart != null)
        {
            onPlayStart();
            // onPlayStart = null;
        }
    }

    public void OnAnimation_Stop()
    {
        Logger.log("GameAnimationEvent 动画播放结束");
        if (onPlayStop != null)
        {
            onPlayStop();
            // onPlayStop = null;
        }
    }
}
