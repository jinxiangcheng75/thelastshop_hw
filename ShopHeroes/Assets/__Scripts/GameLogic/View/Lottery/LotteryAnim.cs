using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryAnim : MonoBehaviour
{
    public Transform dipanTrans;
    public Transform zhenTrans;

    bool isIdle = false;
    int index = 0;
    //void Update()
    //{
    //if (Input.GetKeyDown(KeyCode.V))
    //{
    //    index = Random.Range(0, 13);
    //    setPanRotate();
    //    dipanTrans.DOLocalRotate(new Vector3(0, 0, -720), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.InCirc).OnComplete(() =>
    //    {
    //        dipanTrans.DOLocalRotate(new Vector3(0, 0, -1080 - (15 + 30 * index) - 15), 4.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).OnComplete(() =>
    //        {
    //            dipanTrans.DOLocalRotate(new Vector3(0, 0, dipanTrans.localEulerAngles.z + 15), 1).SetEase(Ease.InCubic);
    //        });
    //    });
    //}
    //}

    public void setIdle(bool idleState)
    {
        isIdle = idleState;
    }
}
