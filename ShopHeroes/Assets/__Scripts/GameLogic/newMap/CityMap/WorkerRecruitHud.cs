using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WorkerRecruitHud : MonoBehaviour
{
    [SerializeField]
    private Follow2DTarget follow2DTarget;
    [SerializeField]
    private GUIIcon icon;
    [SerializeField]
    private Text tips;

    [SerializeField]
    private RectTransform animRoot;

    public float moveDis = 20;
    public float moveBase = 100;

    public Transform followTarget
    {
        get { return follow2DTarget.target; }
        set { follow2DTarget.target = value; }
    }

    public WorkerData Data { get { return _data; } }

    WorkerData _data;

    public void FindCanLockWorker()
    {
        var list = RoleDataProxy.inst.WorkerList.FindAll(t => t.state == EWorkerState.CanUnlock);

        if (list.Count > 0)
        {

            list.Sort((a, b) => 
            {
                if (a.config.get_type == 6 && b.config.get_type != 6)
                {
                    return 1;
                }
                else if (a.config.get_type != 6 && b.config.get_type == 6)
                {
                    return -1;
                }
                else if (a.config.get_type == 6 && b.config.get_type == 6)
                {
                    return a.config.id.CompareTo(b.config.id);
                }
                else
                {
                    return a.config.cost_money.CompareTo(b.config.cost_money);
                }
            });
            _data = list[0];

            icon.SetSprite(StaticConstants.roleHeadIconAtlasName, _data.config.icon);
            gameObject.SetActiveTrue();


            animRoot.DOAnchorPosY(moveBase + moveDis, 1f).From(Vector3.up * moveBase).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            gameObject.SetActiveFalse();
            DOTween.Kill(animRoot);
        }
    }

    public void SetFont()
    {
        if (tips == null) return;
        tips.font = LanguageManager.inst.curFont;
        tips.text = LanguageManager.inst.GetValueByKey("招募");
    }

    public void OnDestroy()
    {
        DOTween.Kill(animRoot);
    }

}
