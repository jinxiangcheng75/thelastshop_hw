using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class DOTweenViewAnimator : MonoBehaviour
{
    public event UnityAction<bool> onAnimComplete;

    //public DOTweenAnimation dgAnim;

    public Vector3 animVal;
    bool isShow;
    void Start()
    {
        /*if(dgAnim != null) {
            dgAnim.autoPlay = false;
            animVal = dgAnim.endValueV3;
            dgAnim.onComplete.AddListener(animComplete);
        }*/
    }

    void animComplete()
    {
        onAnimComplete?.Invoke(isShow);
    }

    public void show()
    {
        /*if(dgAnim != null) {
            isShow = true;
            //dgAnim.endValueV3 = animVal;
            dgAnim.isFrom = false;
            dgAnim.DOPlay();

        }else
            onAnimComplete?.Invoke(true);*/
    }

    public void hide()
    {
        /*if(dgAnim != null) {
            isShow = false;
            //dgAnim.endValueV3 = -animVal;
            dgAnim.isFrom = true;
            dgAnim.DOPlay();
        } else
            onAnimComplete?.Invoke(false);*/
    }

    void OnDestroy()
    {
        /*if(dgAnim != null)
            dgAnim.onComplete.RemoveAllListeners();
        */
    }
}
