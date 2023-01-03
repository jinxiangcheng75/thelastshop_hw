using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AcheivementUIView : ViewBase<AcheivementComp>
{
    public override string viewID => ViewPrefabName.AcheivementUI;
    public override string sortingLayerName => "window";

    List<AcheivementAwardItem> awardItems;
    List<AcheivementData> acheivementList;
    int listItemCount = 0;
    bool isShowLevelGrowth = false;
    bool isShowGemGet = false;
    GameObject curEffect;

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.noSettingAndEnergy;
        isShowResPanel = true;
        InitData();
        AddUIEvent();
        CreatAwardItem();
        contentPane.leftObj.SetActive(false);
        isShowLevelGrowth = false;
        isShowGemGet = false;
    }

    private void CreatAwardItem()
    {
        var list = AcheivementRoadConfigManager.inst.GetAllConfig();
        var roadRect = contentPane.roadSlider.GetComponent<RectTransform>();
        roadRect.sizeDelta = new Vector2(384 * list.Length, roadRect.sizeDelta.y);
        contentPane.scrollRect.content.sizeDelta = new Vector2(384 * list.Length + 100, contentPane.scrollRect.content.sizeDelta.y);
        contentPane.roadSlider.maxValue = list.Length;
        for (int i = 0; i < list.Length; i++)
        {
            int index = i;
            GameObject go = GameObject.Instantiate(contentPane.awardItemPrefab.gameObject, contentPane.contentRect.transform);
            float x = 0;
            if (index == 0) x = 480;
            else
                x = 480 + 380 * index;
            go.transform.localPosition = new Vector3(x, go.transform.localPosition.y, 0);
            go.SetActive(true);
            var item = go.GetComponent<AcheivementAwardItem>();
            item.setData(list[index]);
            awardItems.Add(item);
        }
    }

    private void InitData()
    {
        awardItems = new List<AcheivementAwardItem>();
        acheivementList = new List<AcheivementData>();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.scrollRect.onValueChanged.AddListener(OnScrollViewChange);
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        //contentPane.scrollView.scrollByItemIndex()
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        AcheivementItem item = (AcheivementItem)obj;

        if (index == 0 && isShowLevelGrowth) //lua成长礼包露出
        {
            item.c_obj.SetActive(false);
            item.lua_obj.SetActive(true);

            LuaListItem luaItem = item.GetComponent<LuaListItem>();
            if (luaItem != null)
            {
                luaItem.SetData(1);
            }
        }
        else if (index == 0 && !isShowLevelGrowth && isShowGemGet) //lua金条礼包露出
        {
            item.c_obj.SetActive(false);
            item.lua_obj.SetActive(true);

            LuaListItem luaItem = item.GetComponent<LuaListItem>();
            if (luaItem != null)
            {
                luaItem.SetData(2);
            }
        }
        else if (index == 1 && isShowLevelGrowth && isShowGemGet)   //lua金条礼包露出
        {
            item.c_obj.SetActive(false);
            item.lua_obj.SetActive(true);

            LuaListItem luaItem = item.GetComponent<LuaListItem>();
            if (luaItem != null)
            {
                luaItem.SetData(2);
            }
        }
        else
        {
            item.c_obj.SetActive(true);
            item.lua_obj.SetActive(false);

            if (index > listItemCount)
            {
                item.gameObject.SetActive(false);
            }

            if (index <= listItemCount)
            {
                item.gameObject.SetActive(true);
                item.setData(acheivementList[index - (isShowLevelGrowth ? 1 : 0) - (isShowGemGet ? 1 : 0)]);
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

    }

    private void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }

        contentPane.scrollView.totalItemCount = count;
    }

    private void OnScrollViewChange(Vector2 v2)
    {
        if (contentPane.scrollRect.horizontalNormalizedPosition <= 0)
        {
            if (contentPane.leftObj.activeSelf)
                contentPane.leftObj.SetActive(false);
            contentPane.scrollRect.horizontalNormalizedPosition = 0;
        }
        else if (contentPane.scrollRect.horizontalNormalizedPosition >= 1)
        {
            if (contentPane.rightObj.activeSelf)
                contentPane.rightObj.SetActive(false);
            contentPane.scrollRect.horizontalNormalizedPosition = 1;
        }
        else
        {
            if (!contentPane.leftObj.activeSelf)
                contentPane.leftObj.SetActive(true);
            if (!contentPane.rightObj.activeSelf)
                contentPane.rightObj.SetActive(true);
        }
    }

    public void setAcheivementRoadData()
    {
        var cfg = AcheivementRoadConfigManager.inst.GetConfig(AcheivementDataProxy.inst.AcheivementRoadId);
        AcheivementRoadConfigData lastCfg = null;
        if (cfg.index != 1)
        {
            lastCfg = AcheivementRoadConfigManager.inst.GetConfigByIndex(cfg.index - 1);
        }
        int offset = lastCfg == null ? 0 : lastCfg.need_point;
        contentPane.roadIcon.SetSprite(cfg.altas, cfg.frame);
        contentPane.roadNameText.text = LanguageManager.inst.GetValueByKey(cfg.name);
        var tempData = AcheivementDataProxy.inst.acheivementRoadList.Find(t => t.state == EAchievementRoadRewardState.CanReward);
        int finishCount = 0;
        if (tempData != null)
        {
            finishCount = tempData.index;
        }
        else
        {
            tempData = AcheivementDataProxy.inst.acheivementRoadList.Find(t => t.state == EAchievementRoadRewardState.Disable);
            if(tempData != null)
            {
                finishCount = tempData.index;
            }
        }
        int para = finishCount <= 1 ? 0 : 344 + 390 * (finishCount - 2);
        //int para = finishCount > 1 ? 380 + 410 * (finishCount - 1) : finishCount == 1 ? 380 : 0;
        contentPane.contentRect.DOAnchorPos3DX(-para, 0.5f);
        contentPane.roadSlider.value = cfg.index == 1 ? 0 : cfg.index - 1;
        contentPane.roadSlider.value += (float)(AcheivementDataProxy.inst.AchievementRoadPoint - offset) / (cfg.need_point - offset);
        if (contentPane.roadSlider.value <= 0.05f)
        {
            contentPane.roadSlider.value = 0.05f;
        }
        contentPane.roadScheduleText.text = AcheivementDataProxy.inst.AchievementRoadPoint.ToString();

        if (curEffect != null)
        {
            curEffect.SetActive(false);
        }
        curEffect = contentPane.effects[cfg.level_color - 2];
        curEffect.SetActive(true);

        foreach (var item in awardItems)
        {
            item.refreshData();
        }
    }

    protected override void onShown()
    {

        contentPane.scrollRect.horizontalNormalizedPosition = 0;
        contentPane.scrollView.ScrollToTop();
        setData();
    }

    protected override void onHide()
    {

    }

    public void setData()
    {
        acheivementList = AcheivementDataProxy.inst.acheivementList;

        WorldParConfig worldParConfig = WorldParConfigManager.inst.GetConfig(340);
        isShowLevelGrowth = !((worldParConfig != null && worldParConfig.parameters > UserDataProxy.inst.playerData.level) || HotfixBridge.inst.HasBuyLevelGrowth());

        worldParConfig = WorldParConfigManager.inst.GetConfig(363);
        isShowGemGet = worldParConfig == null || worldParConfig.parameters <= UserDataProxy.inst.playerData.level;

        SetListItemTotalCount(acheivementList.Count + (isShowLevelGrowth ? 1 : 0) + (isShowGemGet ? 1 : 0));
        setAcheivementRoadData();
        contentPane.scrollView.refresh();
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
    }

    protected override void DoHideAnimation()
    {
        HideView();
    }
}
