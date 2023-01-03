using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScienceLabBuildingUnLockHud : MonoBehaviour
{
    [SerializeField]
    private Follow2DTarget follow2DTarget;
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

    public bool FindUnLockBuilding()
    {
        var list = UserDataProxy.inst.GetAllCanShowScienceBuildingData().FindAll(t => t.isNew);

        if (list.Count > 0)
        {
            list.Sort((a, b) => -a.buildingId.CompareTo(b.buildingId));
            var buildingData = list[0];

            WorkerConfig cfg = WorkerConfigManager.inst.GetConfig(buildingData.config.unlock_id);

            if (cfg != null)
            {
                gameObject.SetActive(true);
                icon.SetSprite("worker_atlas", cfg.profession_icon);


                animRoot.DOAnchorPosY(moveBase + moveDis, 1f).From(Vector3.up * moveBase).SetLoops(-1, LoopType.Yoyo).SetDelay(0.3f);

            }
            else
            {
                gameObject.SetActive(false);
                DOTween.Kill(animRoot);
            }
        }
        else
        {
            gameObject.SetActive(false);
            DOTween.Kill(animRoot);
        }

        return gameObject.activeSelf;
    }

    public void OnDestroy()
    {
        DOTween.Kill(animRoot);
    }

}
