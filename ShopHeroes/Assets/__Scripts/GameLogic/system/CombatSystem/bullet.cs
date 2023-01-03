using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/*
*/
public class bullet : MonoBehaviour
{
    [HideInInspector]
    public FighterClr startFighter;
    [HideInInspector]
    public FighterClr targetFighter;
    [HideInInspector]
    public Vector3 targetPos = Vector3.zero;
    [HideInInspector]
    public float lifeTime = 0.2f;
    [HideInInspector]
    public bool isleft = true;
    void Start()
    {

    }
    public virtual void shot()
    {
        this.gameObject.SetActive(true);
        if (targetFighter != null)
        {
            targetPos = targetFighter.hitTF.position;
        }
        //  transform.localScale = isleft ? Vector3.one : new Vector3(-1, 1, 1);
        var rot = Vector3.zero;
        rot.y *= isleft ? 0f : 180f;
        transform.Rotate(rot);
        //位移动画
        transform.DOMove(targetPos, lifeTime).SetEase(Ease.Linear).onComplete = TweeningOnComplete;
    }
    //到达目标
    protected virtual void TweeningOnComplete()
    {
        Logger.log("子弹攻击完成");
        //Invoke("dispose", 0.5f);
        dispose();
    }
    public virtual void dispose()
    {
        GameObject.Destroy(this.gameObject);
    }
}
