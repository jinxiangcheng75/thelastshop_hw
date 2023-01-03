using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using DG.Tweening;

public enum PanelType
{
    min = 0,
    exterior = 1,//外观
    fashion = 2,//时装
    max
}

public class ShopkeeperPanelView : ViewBase<ShopkeeperPanelComp>
{
    public override string viewID => ViewPrefabName.ShopkeeperPanel;
    public override string sortingLayerName => "window";
    public override int showType => (int)ViewShowType.normal;
    private List<CustomBuyItemComp> _batchItems;
    private List<RoleSubTypeData> getListData;
    private EGender curSex;

    int timerId;

    bool isGemBuy = false;
    int needGemNum = 0;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        EventController.inst.AddListener(GameEventType.DressUpEvent.USERCUSTOM, FinishCustomChange);
        InitDataList();
        CreatEmptyList();
        AddUIEvent();
        SwitchSex(ShopkeeperDataProxy.inst.curGender);

        var size = FGUI.inst.uiRootTF.sizeDelta;
        float len = size.x > size.y ? size.x + 100 : size.y + 100;
        contentPane.modelCanvas.GetComponent<RectTransform>().sizeDelta = Vector2.one * len;
    }

    private void InitDataList()
    {
        _batchItems = new List<CustomBuyItemComp>();
        getListData = new List<RoleSubTypeData>();
    }

    private void InitRoleTrans()
    {
        ShopkeeperDataProxy.inst.Man.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1);
        ShopkeeperDataProxy.inst.Man.SetAnimationSpeed(1);
        string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.Man.gender, (int)kIndoorRoleActionType.normal_standby);
        ShopkeeperDataProxy.inst.Man.Play(idleAnimationName, true);

        ShopkeeperDataProxy.inst.Woman.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1);
        ShopkeeperDataProxy.inst.Woman.SetAnimationSpeed(1);
        idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.Woman.gender, (int)kIndoorRoleActionType.normal_standby);
        ShopkeeperDataProxy.inst.Woman.Play(idleAnimationName, true);
    }

    private void CreatEmptyList()
    {
        for (int i = 0; i < 11; i++)
        {
            GameObject batchItem = GameObject.Instantiate(contentPane.batchItem.gameObject, contentPane.batchContent);
            batchItem.transform.SetParent(contentPane.batchContent, true);
            batchItem.SetActive(true);
            CustomBuyItemComp item = batchItem.GetComponent<CustomBuyItemComp>();
            _batchItems.Add(item);
        }
    }

    private void AddUIEvent()
    {
        // UI事件注册
        contentPane.exteriorBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.exteriorBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //  {

            //  });
            contentPane.exteriorBtn.gameObject.SetActiveFalse();
            contentPane.fashionBtn.gameObject.SetActiveFalse();
            contentPane.finishBtn.gameObject.SetActive(false);
            contentPane.changeBtnList.SetActive(false);
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPKEEPERSUBPANEL, PanelType.exterior);
        });

        contentPane.fashionBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.fashionBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            contentPane.exteriorBtn.gameObject.SetActiveFalse();
            contentPane.fashionBtn.gameObject.SetActiveFalse();
            contentPane.finishBtn.gameObject.SetActive(false);
            contentPane.changeBtnList.SetActive(false);
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPKEEPERSUBPANEL, PanelType.fashion);
        });

        contentPane.finishBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.finishBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            FinishCustomChange();
        });

        contentPane.cancleBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.cancleBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            ShopkeeperDataProxy.inst.curRole.SetAnimationSpeed(0);
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_PROMPT);
        });

        contentPane.applyBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.applyBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            ApplyChangeClothe();
        });

        contentPane.batchItem.gameObject.SetActive(false);
        contentPane.batchCloseBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.batchCloseBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            contentPane.batchPanel.SetActive(false);
        });

        contentPane.batchAllBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.batchCloseBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            BatchRemoveAllClothe();
        });

        contentPane.batchApplyBtn.ButtonClickTween(() =>
        {

            //float animTime = contentPane.batchApplyBtn.GetComponent<Animator>().GetClipLength("Pressed");
            //GameTimer.inst.AddTimer(animTime * StaticConstants.btnDelayTime, 1, () =>
            //{

            //});
            BatchApplyClothe();
        });
    }

    private void BatchApplyClothe()
    {
        if (!contentPane.batchSelectObj.activeSelf)
        {
            contentPane.batchSelectObj.SetActive(true);
            return;
        }
        else
        {
            if (isGemBuy)
            {
                if (UserDataProxy.inst.playerData.gem >= needGemNum)
                {
                    BuyDress();
                }
                else
                {
                    if (contentPane.batchSelectObj.activeSelf)
                        contentPane.batchSelectObj.SetActive(false);

                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, needGemNum - UserDataProxy.inst.playerData.gem);
                }
            }
            else
            {
                if (contentPane.batchPriceText.color == GUIHelper.GetColorByColorHex("FFFFFF"))
                {
                    BuyDress();
                }
                else
                {
                    if (contentPane.batchSelectObj.activeSelf)
                        contentPane.batchSelectObj.SetActive(false);
                }
            }
        }
    }

    private void BuyDress()
    {
        List<int> ids = new List<int>();
        Dictionary<int, int> tempSaleList = new Dictionary<int, int>();
        if (contentPane.batchGlodIcon.activeSelf)
        {
            for (int i = 0; i < getListData.Count; i++)
            {
                if (getListData[i].config.get_type == 3)
                {
                    if (!tempSaleList.ContainsKey(getListData[i].config.sale_id))
                        tempSaleList.Add(getListData[i].config.sale_id, getListData[i].config.sale_id);
                }
                else
                {
                    if (getListData[i].config.price_money > 0)
                        ids.Add((int)getListData[i].config.id);
                }

            }
        }
        else if (contentPane.batchGemIcon.activeSelf)
        {
            for (int i = 0; i < getListData.Count; i++)
            {
                if (getListData[i].config.get_type == 3)
                {
                    if (!tempSaleList.ContainsKey(getListData[i].config.sale_id))
                        tempSaleList.Add(getListData[i].config.sale_id, getListData[i].config.sale_id);
                }
                else
                    ids.Add((int)getListData[i].config.id);
            }
        }

        if (tempSaleList.Count > 0)
        {
            foreach (var saleId in tempSaleList.Values)
            {
                HotfixBridge.inst.TriggerLuaEvent("CSCallLua_ShowGiftDeatilUI", saleId);
            }

            return;
        }

        ShopkeeperDataProxy.inst.buyType = 1;
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_BuyDress()
            {
                dressIdList = ids
            }
        });
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Custom()
            {
                gender = (int)ShopkeeperDataProxy.inst.curGender,
                userDress = ShopkeeperDataProxy.inst.curGender == EGender.Male ? ShopkeeperDataProxy.inst.manDress : ShopkeeperDataProxy.inst.womanDress
            }
        });

        ShowFinishBtnOrChangeBtn(true);
    }

    private void ApplyChangeClothe()
    {
        // 判断这件衣服是否购买了
        RoleDress tempDress = ShopkeeperDataProxy.inst.curGender == EGender.Male ? ShopkeeperDataProxy.inst.Man.curDress : ShopkeeperDataProxy.inst.Woman.curDress;

        bool flag = ShopkeeperDataProxy.inst.JudgeIsAllFreeOrBuy(ShopkeeperDataProxy.inst.curGender);
        if (flag)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_User_Custom()
                {
                    gender = (int)ShopkeeperDataProxy.inst.curGender,
                    userDress = tempDress
                }
            });
        }
        else
        {
            // 没购买 显示批量购买界面
            contentPane.batchPanel.SetActive(true);
            RefreshBatchData();
        }
    }

    private void RefreshBatchData()
    {
        getListData = ShopkeeperDataProxy.inst.ReturnNotBuyIdList(ShopkeeperDataProxy.inst.curGender);

        Dictionary<int, int> tempSaleList = new Dictionary<int, int>();
        int notGiftNum = 0;
        for (int i = getListData.Count - 1; i >= 0; i--)
        {
            int index = i;
            if (getListData[index].config.get_type == 3)
            {
                if (!tempSaleList.ContainsKey(getListData[index].config.sale_id))
                    tempSaleList.Add(getListData[index].config.sale_id, getListData[index].config.sale_id);
                //getListData.RemoveAt(index);
            }
            else
            {
                notGiftNum++;
            }
        }

        foreach (var saleId in tempSaleList.Values)
        {
            HotfixBridge.inst.TriggerLuaEvent("CSCallLua_ShowGiftDeatilUI", saleId);
        }

        if (getListData.Count == 0)
        {
            if ((EGender)UserDataProxy.inst.playerData.gender == ShopkeeperDataProxy.inst.curGender && ShopkeeperDataProxy.inst.JudgeIsEquale(UserDataProxy.inst.playerData.userDress, ShopkeeperDataProxy.inst.curGender == EGender.Male ? ShopkeeperDataProxy.inst.manDress : ShopkeeperDataProxy.inst.womanDress))
            {
                ShowFinishBtnOrChangeBtn(true);
            }
            contentPane.batchPanel.SetActive(false);
            return;
        }
        else
        {
            if (notGiftNum == 0)
            {
                contentPane.batchPanel.SetActive(false);
            }
        }
        JudgeGlodOrGem();
    }

    private void JudgeGlodOrGem()
    {
        int sumGem = 0;
        int sumGlod = 0;

        for (int i = 0; i < getListData.Count; i++)
        {
            if (getListData[i].config.get_type == 3) continue;
            if (getListData[i].config.price_money > 0)
            {
                contentPane.batchGlodIcon.SetActive(true);
                contentPane.batchGemIcon.SetActive(false);
                break;
            }
            else
            {
                contentPane.batchGemIcon.SetActive(true);
                contentPane.batchGlodIcon.SetActive(false);
            }
        }

        for (int i = 0; i < getListData.Count; i++)
        {
            if (getListData[i].config.get_type == 3) continue;
            if (getListData[i].config.price_money > 0)
                sumGlod += getListData[i].config.price_money;
            else
                sumGem += getListData[i].config.price_diamond;
        }

        // 判断钻石金币数量够不够
        if (sumGlod > 0)
        {
            isGemBuy = false;
            contentPane.batchPriceText.text = sumGlod.ToString();
            if (UserDataProxy.inst.playerData.gold > sumGlod)
                contentPane.batchPriceText.color = GUIHelper.GetColorByColorHex("FFFFFF");
            else
                contentPane.batchPriceText.color = GUIHelper.GetColorByColorHex("FF0000");
        }
        else
        {
            isGemBuy = true;
            contentPane.batchPriceText.text = sumGem.ToString();
            needGemNum = sumGem;
            //if (UserDataProxy.inst.playerData.gem >= sumGem)
            //    contentPane.batchPriceText.color = GUIHelper.GetColorByColorHex("FFFFFF");
            //else
            //    contentPane.batchPriceText.color = GUIHelper.GetColorByColorHex("FF0000");
        }

        RefreshCustomItem(sumGlod > 0);
    }

    private void RefreshCustomItem(bool glodBigThanZero)
    {
        for (int i = 0; i < _batchItems.Count; i++)
        {
            int temp = i;
            if (getListData.Count > i)
            {
                _batchItems[temp].deleteBtn.onClick.RemoveAllListeners();
                _batchItems[i].InitData(getListData[temp], ShopkeeperDataProxy.inst.curGender);
                _batchItems[i].deleteBtn.ButtonClickTween(() =>
                {
                    SingleDeleteClick(_batchItems[temp], true);
                });
                if (glodBigThanZero)
                {
                    if (getListData[temp].config.price_money == 0)
                        _batchItems[i].maskObj.SetActive(true);
                }
                else
                    _batchItems[i].maskObj.SetActive(false);

                if (getListData[temp].config.get_type == 3)
                {
                    _batchItems[i].gameObject.SetActive(false);
                }
            }
            else
                _batchItems[i].ClearData();
        }
    }

    private void SingleDeleteClick(CustomBuyItemComp temp, bool needRefresh)
    {

        int last = 0, first = 0;

        if (ShopkeeperDataProxy.inst.curGender == EGender.Male)
        {
            last = SpineUtils.GetDressIdByType_2(ShopkeeperDataProxy.inst.manDress, temp.data.config.type_2);
            first = SpineUtils.GetDressIdByType_2(ShopkeeperDataProxy.inst.manFirst, temp.data.config.type_2);

            ShopkeeperDataProxy.inst.Man.SwitchClothingByCfg(dressconfigManager.inst.GetConfig(first));
        }
        else
        {
            last = SpineUtils.GetDressIdByType_2(ShopkeeperDataProxy.inst.womanDress, temp.data.config.type_2);
            first = SpineUtils.GetDressIdByType_2(ShopkeeperDataProxy.inst.womanFirst, temp.data.config.type_2);

            ShopkeeperDataProxy.inst.Woman.SwitchClothingByCfg(dressconfigManager.inst.GetConfig(SpineUtils.GetDressIdByType_2(ShopkeeperDataProxy.inst.womanFirst, temp.data.config.type_2)));
        }

        ShopkeeperDataProxy.inst.ChangeState(last, false);
        ShopkeeperDataProxy.inst.ChangeState(first, true);

        temp.ClearData();

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        timerId = GameTimer.inst.AddTimer(0.3f, 1, () =>
          {
              if (needRefresh) RefreshBatchData();
          });
    }

    private void FinishCustomChange()
    {
        for (int i = 0; i < _batchItems.Count; i++)
        {
            _batchItems[i].ClearData();
        }
        getListData.Clear();
        if (contentPane == null) return;
        contentPane.batchSelectObj.SetActive(false);
        contentPane.batchPanel.SetActive(false);
        ShowFinishBtnOrChangeBtn(true);
        hide();
    }

    private void SwitchSex(EGender curSex)
    {
        this.curSex = curSex;
        switch (curSex)
        {
            case EGender.Male:
                ShowCurSexRole(true);
                break;
            case EGender.Female:
                ShowCurSexRole(false);
                break;
        }
    }

    private void ShowCurSexRole(bool isMan)
    {
        ShopkeeperDataProxy.inst.Man.SetActive(isMan);
        ShopkeeperDataProxy.inst.Woman.SetActive(!isMan);
    }



    private void BatchRemoveAllClothe()
    {
        if (getListData != null)
        {
            for (int i = 0; i < getListData.Count; i++)
            {
                int index = i;
                SingleDeleteClick(_batchItems[index], false);
            }

            contentPane.batchPanel.SetActive(false);
            //bool isChanged;
            //if (ShopkeeperDataProxy.inst.curGender == EGender.Male)
            //    isChanged = ShopkeeperDataProxy.inst.JudgeIsEquale(UserDataProxy.inst.playerData.userDress, ShopkeeperDataProxy.inst.manDress);
            //else
            //    isChanged = ShopkeeperDataProxy.inst.JudgeIsEquale(UserDataProxy.inst.playerData.userDress, ShopkeeperDataProxy.inst.womanDress);

            ShowFinishBtnOrChangeBtn(true);
        }
    }

    public void ShowFinishBtnOrChangeBtn(bool isFinish)
    {
        contentPane.finishBtn.gameObject.SetActive(isFinish);
        contentPane.changeBtnList.SetActive(!isFinish);
        contentPane.exteriorBtn.gameObject.SetActiveTrue();
        contentPane.fashionBtn.gameObject.SetActiveTrue();
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(14);
        contentPane.modelCanvas.sortingOrder = _uiCanvas.sortingOrder - 2;
        InitRoleTrans();
        SwitchSex((EGender)ShopkeeperDataProxy.inst.curGender);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(15);

        ShopkeeperDataProxy.inst.Man.transform.parent = null;
        ShopkeeperDataProxy.inst.Man.SetActive(false);
        ShopkeeperDataProxy.inst.Man.transform.position = Vector3.right * 1000f;


        ShopkeeperDataProxy.inst.Woman.transform.parent = null;
        ShopkeeperDataProxy.inst.Woman.SetActive(false);
        ShopkeeperDataProxy.inst.Woman.transform.position = Vector3.right * 1000f;


        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
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
        base.DoHideAnimation();
    }
}
