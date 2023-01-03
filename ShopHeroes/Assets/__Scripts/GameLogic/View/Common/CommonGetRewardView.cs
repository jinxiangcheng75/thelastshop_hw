using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CommonGetRewardView : ViewBase<CommonGetRewardComp>
{
    public override string viewID => ViewPrefabName.CommonGetReward;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();
        contentPane.confirmBtn.ButtonClickTween(() =>
        {
            hide();
        });
    }

    public void setRewardInfo(List<CommonRewardData> rewardsList)
    {
        if (rewardsList.Count > 5)
        {
            //contentPane.gridLayout.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;
            contentPane.gridLayout.constraintCount = 2;
        }
        else
        {
            contentPane.gridLayout.constraintCount = 1;
            //contentPane.gridLayout.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.Flexible;
        }
        AudioManager.inst.PlaySound(25);
        for (int i = 0; i < contentPane.allItems.Count; i++)
        {
            int index = i;
            if (index < rewardsList.Count)
            {
                contentPane.allItems[index].setData(rewardsList[index], index);
            }
            else
            {
                contentPane.allItems[index].clearData();
            }
        }

        var needShowFlyToBagAnimList = rewardsList.FindAll(t => t.itemType == (int)ItemType.HeroCard);

        if (needShowFlyToBagAnimList.Count > 0)
        {
            contentPane.StartCoroutine(showFlyToBagAnim(needShowFlyToBagAnimList));
        }


    }

    IEnumerator showFlyToBagAnim(List<CommonRewardData> list)
    {
        contentPane.confirmBtn.enabled = false;

        contentPane.flyToBagAnimObj.SetActiveTrue();

        for (int i = 0; i < list.Count; i++)
        {
            var data = list[i];
            contentPane.flyToBagRewardItem.SetData(data);
            contentPane.flyToBagRewardItem.gameObject.SetActiveTrue();


            (contentPane.flyToBagRewardItem.transform as RectTransform).DOLocalMoveY(204f, 1.2f).From(410f).SetDelay(0.5f);
            contentPane.flyToBagRewardItem.transform.DOScale(0.5f, 1.2f).From(1f).SetDelay(0.5f).OnComplete(() =>
            {
                contentPane.flyToBagRewardItem.gameObject.SetActiveFalse();
            });

            yield return new WaitForSeconds(1.2f + 0.5f);

        }

        yield return new WaitForSeconds(0.5f);

        contentPane.flyToBagAnimObj.SetActiveFalse();
        contentPane.confirmBtn.enabled = true;

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        HideView();
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }
}
