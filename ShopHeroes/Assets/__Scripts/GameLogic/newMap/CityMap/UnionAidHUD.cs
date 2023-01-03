using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionAidHUD : MonoBehaviour
{
    [SerializeField]
    private Follow2DTarget follow2DTarget;
    [SerializeField]
    private Text tips;
    [SerializeField]
    private GUIIcon icon;
    [SerializeField]
    private RectTransform animRoot;

    public float moveDis = 20;
    public float moveBase = 100;

    public Transform followTarget
    {
        get { return follow2DTarget.target; }
        set { follow2DTarget.target = value; }
    }

    public void CanShowUnionAid()
    {

        if (UserDataProxy.inst.playerData.hasUnion)
        {
            if (UserDataProxy.inst.union_aidShowList.Count > 0)
            {
                DOTween.Kill(animRoot);
                animRoot.anchoredPosition3D = Vector3.zero;

                if (UserDataProxy.inst.union_canAidList.Count > 0) //有可以免费帮助的
                {
                    icon.SetSprite("union_atlas", "zhuejiemian_yuanzhulv");
                    animRoot.DOAnchorPosY(moveBase + moveDis, 1f).From(Vector3.up * moveBase).SetLoops(-1, LoopType.Yoyo).SetDelay(0.6f);
                }
                else
                {
                    icon.SetSprite("union_atlas", "zhuejiemian_yuanzhullan");
                }

                gameObject.SetActiveTrue();

            }
            else
            {
                gameObject.SetActiveFalse();
            }
        }
        else
        {
            gameObject.SetActiveFalse();
        }

    }

    public void SetFont()
    {
        tips.font = LanguageManager.inst.curFont;
        tips.text = LanguageManager.inst.GetValueByKey("联盟援助");
    }

    public void OnDestroy()
    {
        DOTween.Kill(animRoot);
    }

}
