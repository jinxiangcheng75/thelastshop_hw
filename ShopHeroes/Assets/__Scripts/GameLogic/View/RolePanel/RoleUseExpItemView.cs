using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Mosframe;

public class RoleUseExpItemView : ViewBase<RoleUseExpItemComp>
{
    public override string viewID => ViewPrefabName.RoleUseExpItemUI;
    public override string sortingLayerName => "popup";
    int curIndex = -1;
    private RoleHeroData data;
    private List<Item> expItemList;
    ExpItemUI lastItem;
    bool isRunning = false;
    bool isLimit = false;

    GraphicDressUpSystem graphicDressUp;
    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;

        AddUIEvent();
        InitComponent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.useBtn.ButtonClickTween(() =>
        {
            if (curIndex == -1)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("先选择一个道具"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            if (data.currentState == 2)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("探险期间不可使用道具"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level + 1);
            if (upgradeCfg == null)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("等级已满不能使用道具"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            if (isLimit)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("已经达到等级上限，请去升级英雄之家！"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            double count = ItemBagProxy.inst.GetItem(curIndex).count;
            if (count > 0)
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_USEHEROITEM, curIndex, data.uid);
            else
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("数量不足"), GUIHelper.GetColorByColorHex("FF2828"));
        });
    }

    private void InitComponent()
    {
        expItemList = new List<Item>();
        contentPane.scroll.itemRenderer = listitemRenderer;
        contentPane.scroll.itemUpdateInfo = listitemRenderer;
    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (listItemCount > expItemList.Count)
        {
            listItemCount = expItemList.Count;
        }
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.scroll.totalItemCount = count1;
    }

    int listItemCount = 0;
    private void listitemRenderer(int index, IDynamicScrollViewItem item)
    {
        BtnList itemScript = (BtnList)item;

        for (int i = 0; i < 3; ++i)
        {
            int itemIndex = index * 3 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < expItemList.Count)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                ExpItemUI tempItem = itemScript.buttonList[i].GetComponent<ExpItemUI>();
                tempItem.setData(expItemList[itemIndex]);
                tempItem.clickHandler = setCurItemId;
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void setCurItemId(ExpItemUI tempItem, int itemId)
    {
        if (lastItem != null && tempItem != lastItem) lastItem.selectObj.SetActive(false);
        curIndex = itemId;
        lastItem = tempItem;
        RoleDataProxy.inst.curSelectItemId = itemId;
    }

    public void setData(int heroUid)
    {
        data = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        contentPane.levelText.text = data.level.ToString();
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(data.nickName);
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        contentPane.heroBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        contentPane.typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        contentPane.typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level + 1);
        //var curUpgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level);
        if (upgradeCfg != null)
        {
            var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);
            if (data.level >= cityData.effectVal)
            {
                isLimit = true;
                contentPane.notMaxObj.SetActive(false);
                contentPane.maxObj.SetActive(false);
                contentPane.limitObj.SetActive(true);
            }
            else
            {
                isLimit = false;
                contentPane.notMaxObj.SetActive(true);
                contentPane.maxObj.SetActive(false);
                contentPane.levelSlider.maxValue = upgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence));
                contentPane.levelSlider.value = Mathf.Max(contentPane.levelSlider.maxValue * 0.05f, data.exp);
                contentPane.scheduleCurText.text = data.exp.ToString();
                contentPane.scheduleEndText.text = ((int)contentPane.levelSlider.maxValue).ToString();
                contentPane.limitObj.SetActive(false);
                contentPane.useBtn.interactable = true;
            }
        }
        else
        {
            contentPane.levelSlider.maxValue = 1;
            contentPane.levelSlider.value = 1;
            contentPane.notMaxObj.SetActive(false);
            contentPane.maxObj.SetActive(true);
            contentPane.limitObj.SetActive(false);
            contentPane.useBtn.interactable = false;
        }
        setHeroHeadIcon();
        setExpItemContent();
    }

    private void setExpItemContent()
    {
        expItemList = ItemBagProxy.inst.GetItemsByType(ItemType.HeroExp);
        SetListItemTotalCount(expItemList.Count);
    }

    private void setHeroHeadIcon()
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender, callback: system =>
            {
                graphicDressUp = system;
                system.transform.SetParent(contentPane.headParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender);
        }
    }

    public void useExpItemAnim(HeroInfo curInfo)
    {
        RoleHeroData lastData = new RoleHeroData();
        lastData = new RoleHeroData();
        var tempData = RoleDataProxy.inst.GetHeroDataByUid(curInfo.heroUid);
        lastData.level = tempData.level;
        lastData.exp = tempData.exp;
        lastData.intelligence = tempData.intelligence;
        RoleDataProxy.inst.RecordHeroData(curInfo);
        int exp = 0;
        if (lastData.level < curInfo.level)
        {
            var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(curInfo.level + 1);
            contentPane.levelText.text = curInfo.level.ToString();
            var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);
            if (upgradeCfg == null)
            {
                contentPane.maxObj.SetActive(true);
                contentPane.notMaxObj.SetActive(false);
                contentPane.levelSlider.maxValue = 1;
                contentPane.levelSlider.value = 1;
            }
            else
            {
                if (curInfo.level >= cityData.effectVal)
                {
                    isLimit = true;
                    contentPane.notMaxObj.SetActive(false);
                    contentPane.maxObj.SetActive(false);
                    contentPane.limitObj.SetActive(true);
                    contentPane.levelSlider.maxValue = 1;
                    contentPane.levelSlider.value = 1;
                }
                else
                {
                    contentPane.levelSlider.value = 0;
                    contentPane.maxObj.SetActive(false);
                    contentPane.notMaxObj.SetActive(true);
                    contentPane.levelSlider.maxValue = upgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(curInfo.aptitude));
                    contentPane.scheduleEndText.text = ((int)contentPane.levelSlider.maxValue).ToString();
                    contentPane.levelSlider.value = curInfo.exp;
                    contentPane.scheduleCurText.text = curInfo.exp.ToString();
                }
            }
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUPGRADE_SHOWUI, curInfo.heroUid);
            #region 废弃代码
            //var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(curInfo.level);
            //if (!isRunning)
            //    exp = lastData.exp;
            //else
            //    exp = (int)contentPane.levelSlider.value;
            //contentPane.levelSlider.maxValue = upgradeCfg.exp;
            //contentPane.scheduleEndText.text = upgradeCfg.exp.ToString();
            //DOTween.To(() => exp, x => exp = x, upgradeCfg.exp, 0.7f).SetEase(Ease.OutExpo).OnUpdate(() =>
            //{
            //    contentPane.levelSlider.value = exp;
            //    contentPane.scheduleCurText.text = exp.ToString();
            //}).OnComplete(() =>
            //{
            //    upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(curInfo.level + 1);
            //    contentPane.levelText.text = "Lv." + curInfo.level;
            //    exp = 0;
            //    if (upgradeCfg == null)
            //    {
            //        contentPane.maxObj.SetActive(true);
            //        contentPane.notMaxObj.SetActive(false);
            //        contentPane.levelSlider.maxValue = 1;
            //        contentPane.levelSlider.value = 1;
            //        GameTimer.inst.AddTimer(0.7f, 1, () =>
            //          {
            //              //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUSEEXPITEM_HIDEUI);
            //              EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUPGRADE_SHOWUI, curInfo.heroUid);
            //          });
            //    }
            //    else
            //    {
            //        contentPane.levelSlider.value = 0;
            //        contentPane.maxObj.SetActive(false);
            //        contentPane.notMaxObj.SetActive(true);
            //        contentPane.scheduleEndText.text = upgradeCfg.exp.ToString();
            //        contentPane.levelSlider.maxValue = upgradeCfg.exp;
            //        DOTween.To(() => exp, x => exp = x, curInfo.exp, 0.7f).SetEase(Ease.OutExpo).OnUpdate(() =>
            //        {
            //            contentPane.levelSlider.value = exp;
            //            contentPane.scheduleCurText.text = exp.ToString();
            //        }).OnComplete(() =>
            //        {
            //            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUSEEXPITEM_HIDEUI);
            //            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEUPGRADE_SHOWUI, curInfo.heroUid);
            //        });
            //    }
            //});
            #endregion
        }
        else
        {
            var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(lastData.level + 1);
            if (upgradeCfg == null)
            {
                contentPane.maxObj.SetActive(true);
                contentPane.notMaxObj.SetActive(false);
                contentPane.levelSlider.maxValue = 1;
                contentPane.levelSlider.value = 1;
            }
            else
            {
                contentPane.maxObj.SetActive(false);
                contentPane.notMaxObj.SetActive(true);
                contentPane.levelSlider.maxValue = upgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(lastData.intelligence));
                if (!isRunning)
                    exp = lastData.exp;
                else
                    exp = (int)contentPane.levelSlider.value;
                contentPane.scheduleEndText.text = ((int)contentPane.levelSlider.maxValue).ToString();
                DOTween.To(() => exp, x => exp = x, curInfo.exp, 0.7f).SetEase(Ease.OutExpo).OnUpdate(() =>
                   {
                       contentPane.levelSlider.value = exp;
                       contentPane.scheduleCurText.text = exp.ToString();
                   });
            }
            isRunning = true;
        }

        #region 废弃代码
        //data = RoleDataProxy.inst.GetHeroDataByUid(data.uid);
        ////var upgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level + 1);
        ////var curUpgradeCfg = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(data.level);


        //int val = (int)contentPane.levelSlider.value;

        //DOTween.To(() => contentPane.levelSlider.value, x => contentPane.levelSlider.value = x, data.exp, 0.7f).SetEase(Ease.OutExpo);
        //DOTween.To(() => val, x => val = x, data.exp, 0.7f).SetEase(Ease.OutExpo).OnUpdate(() =>
        //{
        //    contentPane.scheduleCurText.text = val.ToString();
        //}).
        //OnComplete(() =>
        //{
        //    if (upgradeCfg == null && data.exp >= curUpgradeCfg.exp)
        //    {
        //        contentPane.notMaxObj.SetActive(false);
        //        contentPane.maxObj.SetActive(true);
        //    }
        //});
        #endregion

        setExpItemContent();
    }

    protected override void onShown()
    {
        //contentPane.scroll.ScrollToTop();
    }

    protected override void onHide()
    {
        RoleDataProxy.inst.curSelectItemId = -1;
        curIndex = -1;
        isRunning = false;
        if (lastItem != null)
        {
            lastItem.selectObj.SetActive(false);
            lastItem = null;
        }
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.uid);
    }
}
