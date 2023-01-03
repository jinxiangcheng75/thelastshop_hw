using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mosframe;

public class ShopkeeperSubUIView : ViewBase<ShopkeeperSubUIComp>
{
    public override string viewID => ViewPrefabName.ShopkeeperSubPanel;
    public override string sortingLayerName => "popup";
    EGender curGender;
    PanelType curPanel;
    List<RoleSubTypeData> needDatas;
    ShopClothingItemComp lastItem;
    int lastItemId;
    int lastItemType;
    int exterionIndex = -1, fashionIndex = -1;
    bool allJudge;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.ButtonClickTween(() =>
        {

            JudgeIsSame();
        });
        contentPane.bgCloseBtn.onClick.AddListener(() =>
        {

            JudgeIsSame();
        });
        contentPane.maleBtn.ButtonClickTween(() =>
        {
            onSexSelectChange(0);
        });
        contentPane.femaleBtn.ButtonClickTween(() =>
        {
            onSexSelectChange(1);
        });
        InitComponent();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSettingAndCantClickGem;
    }

    private void JudgeIsSame()
    {
        hide();
        bool judge;
        if (curGender == EGender.Male)
        {
            judge = ShopkeeperDataProxy.inst.JudgeIsEquale(UserDataProxy.inst.playerData.userDress, ShopkeeperDataProxy.inst.Man.curDress);
        }
        else
        {
            judge = ShopkeeperDataProxy.inst.JudgeIsEquale(UserDataProxy.inst.playerData.userDress, ShopkeeperDataProxy.inst.Woman.curDress);
        }

        if (curGender != (EGender)UserDataProxy.inst.playerData.gender)
        {
            judge = false;
        }
        allJudge = judge;
    }

    private void InitComponent()
    {
        needDatas = new List<RoleSubTypeData>();
        contentPane.group.OnSelectedIndexValueChange = onSelectChange;
        //contentPane.sexGroup.OnSelectedIndexValueChange = onSexSelectChange;
        contentPane.fashionGroup.OnSelectedIndexValueChange = onFashionSelectChange;
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        contentPane.fashionScrollView.itemRenderer = listitemRenderer;
        contentPane.fashionScrollView.itemUpdateInfo = listitemRenderer;
    }

    EGender lastGender;

    private void onFashionSelectChange(int index)
    {
        //if (contentPane.fashionGroup.NotNeedInvokeAction)
        //{
        //    contentPane.fashionGroup.NotNeedInvokeAction = false;
        //    return;
        //}
        AudioManager.inst.PlaySound(11);
        var toggle = contentPane.fashionGroup.togglesBtn[index];

        int selectedIndex = toggle.GetComponent<ItemSubType>().subType;

        if (selectedIndex == fashionIndex && lastGender == curGender) return;

        fashionIndex = selectedIndex;
        contentPane.topText.text = LanguageManager.inst.GetValueByKey(StaticConstants.faTypes[fashionIndex - (int)FashionType.Clothe]);
        needDatas = ShopkeeperDataProxy.inst.GetCurSexTypeSubDatas((uint)curPanel, (uint)fashionIndex, curGender);
        lastGender = curGender;

        SetFashionListItemTotalCount(needDatas.Count);
    }

    private void onSelectChange(int index)
    {
        //if (contentPane.group.NotNeedInvokeAction)
        //{
        //    contentPane.group.NotNeedInvokeAction = false;
        //    return;
        //}

        AudioManager.inst.PlaySound(11);
        var toggle = contentPane.group.togglesBtn[index];

        int selectedIndex = toggle.GetComponent<ItemSubType>().subType;

        if (selectedIndex == exterionIndex && lastGender == curGender) return;

        exterionIndex = selectedIndex;

        contentPane.oneObj.SetActive(false);
        contentPane.twoObj.SetActive(true);
        if (exterionIndex == 0)
        {
            switchSex();
            contentPane.oneObj.SetActive(true);
            contentPane.twoObj.SetActive(false);
            onSexSelectChange((int)(curGender) - 1);
        }

        contentPane.topText.text = LanguageManager.inst.GetValueByKey(StaticConstants.exTypes[exterionIndex]);
        needDatas = ShopkeeperDataProxy.inst.GetCurSexTypeSubDatas((uint)curPanel, (uint)exterionIndex, curGender);
        lastGender = curGender;

        SetListItemTotalCount(needDatas.Count);
    }

    private void switchSex()
    {
        if (curGender == EGender.Male)
        {
            contentPane.maleBtn.transform.GetChild(1).gameObject.SetActive(true);
            contentPane.femaleBtn.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            contentPane.maleBtn.transform.GetChild(1).gameObject.SetActive(false);
            contentPane.femaleBtn.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void onSexSelectChange(int index)
    {
        curGender = (EGender)(index + 1);
        ShopkeeperDataProxy.inst.curGender = (EGender)(index + 1);
        ShopkeeperDataProxy.inst.Man.SetActive(curGender == EGender.Male);
        ShopkeeperDataProxy.inst.Woman.SetActive(curGender == EGender.Female);

        int startIndex = 0;
        int endIndex = 0;
        int initIndex = 0;
        if (curGender == EGender.Male)
        {
            startIndex = 7;
            endIndex = 14;
            initIndex = 0;
        }
        else
        {
            startIndex = 0;
            endIndex = 7;
            initIndex = 7;
        }

        contentPane.group.selectedIndex = initIndex;

        for (int i = 0; i < contentPane.group.togglesBtn.Count; i++)
        {
            contentPane.group.togglesBtn[i].gameObject.SetActive(true);
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            contentPane.group.togglesBtn[i].gameObject.SetActive(false);
            contentPane.group.togglesBtn[i].isOn = false;
        }

        //onSelectChange(initIndex);
    }

    int listItemCount = 0;
    int listFashionItemCount = 0;
    private void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        int count1 = listItemCount / 4;
        if (listItemCount % 4 > 0)
        {
            count1++;
        }
        contentPane.scrollView.totalItemCount = count1;
        contentPane.scrollView.ScrollToTop();
    }

    private void SetFashionListItemTotalCount(int count)
    {
        listFashionItemCount = count;
        if (listFashionItemCount < 0)
        {
            listFashionItemCount = 0;
        }
        int count1 = listFashionItemCount / 4;
        if (listFashionItemCount % 4 > 0)
        {
            count1++;
        }
        contentPane.fashionScrollView.totalItemCount = count1;
        //contentPane.fashionScrollView.ScrollToTop();
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;
        int count = curPanel == PanelType.exterior ? listItemCount : listFashionItemCount;
        for (int i = 0; i < 4; ++i)
        {
            int itemIndex = index * 4 + i;
            var item = itemScript.buttonList[i].GetComponent<ShopClothingItemComp>();
            item.clearAnim();
            if (itemIndex >= count)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < count)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                item.InitData(needDatas[itemIndex], curGender, SmallItemClickHandler, itemIndex);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void SmallItemClickHandler(ShopClothingItemComp item, bool isAuto)
    {
        if (item.Data.config.guide == 0 && item.Data.config.get_type == 3 && !HotfixBridge.inst.GetDirectPurchaseDataById(item.Data.config.sale_id, out DirectPurchaseData directPurchaseData))
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("服饰尚未解锁"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }
        if (item.Data.config.guide == 0 && item.Data.config.get_type == 4)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("通过大亨之路解锁该服饰"), Color.white);
            return;
        }
        if (isAuto && item.Data.config.type_2 == lastItemType && item.Data.config.id != lastItemId)
        {
            lastItem.selectImg.SetActive(false);
            lastItem.select.gameObject.SetActive(false);
            lastItem.buySelectObj.SetActiveFalse();
            lastItem.clickTime = false;
            //ShopkeeperDataProxy.inst.ChangeState(lastItemId, false);
        }

        lastItem = new ShopClothingItemComp();
        lastItem.selectImg = item.selectImg;
        lastItem.select = item.select;
        lastItem.buySelectObj = item.buySelectObj;
        lastItem.clickTime = item.clickTime;
        lastItemId = item.Data.config.id;
        lastItemType = item.Data.config.type_2;

        if (isAuto)
        {
            JudgeClickAgain(item);
            //item.btnAnim.Play("click");
            //float animTime = item.btnAnim.GetClipLength("ShopkeeperItem_click") + item.btnAnim.GetClipLength("ShopkeeperItem_ClickUp");
            //GameTimer.inst.AddTimer(animTime * 0.5f, 1, () =>
            //{

            //});
            //GameTimer.inst.AddTimer(animTime, 1, () =>
            //  {
            //      item.btnAnim.Play("Normal");
            //  });

            item.selectImg.SetActive(true);
            item.select.gameObject.SetActive(true);
            item.buySelectObj.SetActiveTrue();
            ShopkeeperDataProxy.inst.ResetSameTypeState(needDatas);
            ShopkeeperDataProxy.inst.ChangeState(item.Data.config.id, true);

            if (curGender == EGender.Male)
            {
                ShopkeeperDataProxy.inst.Man.SwitchClothingByCfg(item.Data.config);
            }
            else
            {
                ShopkeeperDataProxy.inst.Woman.SwitchClothingByCfg(item.Data.config);
            }

            //JudgeClickAgain(item);
        }
        else
        {
            item.selectImg.SetActive(true);
            item.select.gameObject.SetActive(true);
            ShopkeeperDataProxy.inst.ResetSameTypeState(needDatas);
            ShopkeeperDataProxy.inst.ChangeState(item.Data.config.id, true);
            if (curGender == EGender.Male)
            {
                ShopkeeperDataProxy.inst.Man.SwitchClothingByCfg(item.Data.config);
            }
            else
            {
                ShopkeeperDataProxy.inst.Woman.SwitchClothingByCfg(item.Data.config);
            }

        }
        //Logger.error("changeVal = " + item.Data.config.id);
    }

    private void JudgeClickAgain(ShopClothingItemComp curTem)
    {
        if (curTem.selectImg.activeSelf && curTem.Data.config.guide == 0/* && curTem.clickTime*/)
        {
            if (curTem.Data.config.get_type != 3)
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SINGLEBUY, curTem.Data);
            else if (curTem.Data.config.get_type == 3)
            {
                HotfixBridge.inst.TriggerLuaEvent("CSCallLua_ShowGiftDeatilUI", curTem.Data.config.sale_id);
            }
        }
        //else
        //{
        //    //ShopkeeperDataProxy.inst.ChangeState(curTem.Data.config.id, true);

        //    //curTem.selectImg.SetActive(true);
        //    //curTem.select.gameObject.SetActive(true);
        //    curTem.clickTime = true;
        //    // 换装逻辑编写
        //    //ShopkeeperDataProxy.inst.Man.SwitchClothingByCfg(curTem.Data.config);
        //    //ShopkeeperDataProxy.inst.Woman.SwitchClothingByCfg(curTem.Data.config);
        //}
    }

    public void setData(PanelType panelType)
    {
        //ShopkeeperDataProxy.inst.Man.SetUIPosition(ShopkeeperDataProxy.inst.Man.transform, _uiCanvas.sortingOrder + 1);
        //ShopkeeperDataProxy.inst.Woman.SetUIPosition(ShopkeeperDataProxy.inst.Man.transform, _uiCanvas.sortingOrder + 1);
        curPanel = panelType;
        contentPane.exteriorObj.SetActive(panelType == PanelType.exterior);
        contentPane.fashionObj.SetActive(panelType == PanelType.fashion);

        curGender = ShopkeeperDataProxy.inst.curGender;
        //onSexSelectChange((int)curGender - 1);

        contentPane.group.OnEnableMethod(curGender == EGender.Male ? 0 : 7);
        contentPane.fashionGroup.OnEnableMethod();
    }

    public void BuyDressComplete()
    {
        if (curPanel == PanelType.exterior)
        {
            needDatas = ShopkeeperDataProxy.inst.GetCurSexTypeSubDatas((uint)curPanel, (uint)exterionIndex, curGender);
            contentPane.scrollView.refresh();
        }
        else if (curPanel == PanelType.fashion)
        {
            needDatas = ShopkeeperDataProxy.inst.GetCurSexTypeSubDatas((uint)curPanel, (uint)fashionIndex, curGender);
            contentPane.fashionScrollView.refresh();
        }
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        if (!GameSettingManager.inst.needShowUIAnim)
            EventController.inst.TriggerEvent(GameEventType.JUDGESHOPKEEPERDRESS, allJudge);
        exterionIndex = -1;
        fashionIndex = -1;
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.windowAnimator.CrossFade("show", 0f);
        contentPane.windowAnimator.Update(0f);
        contentPane.windowAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.windowAnimator.Play("hide");
        var animatorInfo = contentPane.windowAnimator.GetClipLength("ShopkeeperSub_hide");
        GameTimer.inst.AddTimer(animatorInfo * 0.65f, 1, () =>
          {
              EventController.inst.TriggerEvent(GameEventType.JUDGESHOPKEEPERDRESS, allJudge);
          });
        GameTimer.inst.AddTimer(animatorInfo, 1, () =>
          {
              contentPane.windowAnimator.CrossFade("null", 0f);
              contentPane.windowAnimator.Update(0f);
              this.HideView();
          });
    }
}
