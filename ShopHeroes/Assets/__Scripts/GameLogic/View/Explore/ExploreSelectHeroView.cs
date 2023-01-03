using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mosframe;

public class ExploreSelectHeroView : ViewBase<ExploreSelectHeroCom>
{
    public override string viewID => ViewPrefabName.ExploreSelectHeroUI;
    public override string sortingLayerName => "window";
    List<RoleHeroData> allNotSelectHeroes;
    float canAddExpNum = 0;
    int index;
    int exploreId;
    bool isHaveRestHero;
    float curItemAddPercent;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        contentPane.scrollView.scrollByItemIndex(0);
    }

    public override void shiftIn()
    {
        base.shiftIn();

        if (exploreId != 0)
            setData(index, exploreId, curItemAddPercent);
    }

    public void setData(int index, int exploreId, float itemAddPercent)
    {
        this.index = index;
        RoleDataProxy.inst.curExploreSelectHeroIndex = index;
        curItemAddPercent = itemAddPercent;
        this.exploreId = exploreId;
        var instanceCfg = ExploreInstanceConfigManager.inst.GetConfig(exploreId);
        var groupData = ExploreDataProxy.inst.GetGroupDataByGroupId(instanceCfg.instance_group);
        //contentPane.fightingText.text = instanceCfg.recommend_power.ToString();
        canAddExpNum = instanceCfg.hero_exp * groupData.AddExpPercent * curItemAddPercent;
        allNotSelectHeroes = RoleDataProxy.inst.GetNotFightingStateHeroList();
        //allNotSelectHeroes = allHeroes.FindAll(t => !t.isSelect);
        SetListItemTotalCount(allNotSelectHeroes.Count);
        contentPane.emptyText.enabled = allNotSelectHeroes.Count <= 0;
    }

    public void RemoveHeroComplete(int heroUid, int index)
    {
        this.index = index;
        //allHeroes.Find(t => t.uid == heroUid).isSelect = false;
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREPREPAREREMOVE_COM, heroUid, index);
    }

    private void AddHeroComplete(int heroUid)
    {
        //allHeroes.Find(t => t.uid == heroUid).isSelect = true;
        hide();
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREPREPAREADD_COM, heroUid, index);
    }

    int listItemCount = 0;

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 3; ++i)
        {
            int itemIndex = index * 3 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);
                var item = itemScript.buttonList[i].GetComponent<ExploreSelectHeroItem>();
                if (isHaveRestHero && itemIndex == 0)
                {
                    item.setAllTreatData();
                    continue;
                }
                itemIndex = isHaveRestHero ? itemIndex - 1 : itemIndex;
                item.setData(allNotSelectHeroes[itemIndex], AddHeroComplete, canAddExpNum);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }

        isHaveRestHero = RoleDataProxy.inst.GetRestingStateHeroCount().Count > 0;
        if (isHaveRestHero)
            listItemCount++;
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.scrollView.totalItemCount = count1;
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
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
        contentPane.uiAnimator.Play("hide");

        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });

    }
}
