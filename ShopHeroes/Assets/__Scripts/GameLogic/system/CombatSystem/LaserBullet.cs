using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBullet : bullet
{
    public Transform start;
    public Transform end;
    public override void shot()
    {
        this.gameObject.SetActive(true);
        if (targetFighter != null)
        {
            targetPos = targetFighter.hitTF.position;
        }
        start.position = startFighter.attkTF.position;
        end.position = targetPos;
        time = 0;
        GameTimer.inst.AddTimer(lifeTime, 1, TweeningOnComplete);
    }

    float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            dispose();
            time = -99999;
        }
    }
}
