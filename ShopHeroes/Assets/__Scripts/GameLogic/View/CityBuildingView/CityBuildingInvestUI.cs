using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CityBuildingInvestUI : ViewBase<CityBuildingInvestUIComp>
{

    public override string viewID => ViewPrefabName.CityBuildingInvestUI;

    public override string sortingLayerName => "window";

    Image showSliderImg;
    int clickCount;
    ulong goldCost, gemCost;
    CityBuildingData _data;
    bool isMax;
    bool isHasRankMess;
    int _index = 0;

    DressUpSystem roleDress;

    protected override void onInit()
    {
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.selfUnionToken;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.delBtn.GetComponent<OverrideButton>().SetClickCallback(onDelBtnClick);
        contentPane.delBtn.onClick.AddListener(() =>
        {
            AudioManager.inst.PlaySound(125);
            onDelBtnClick();
        });

        contentPane.addBtn.GetComponent<OverrideButton>().SetClickCallback(onAddBtnClick);
        contentPane.addBtn.onClick.AddListener(() =>
        {
            AudioManager.inst.PlaySound(125);
            onAddBtnClick();
        });

        contentPane.leftBtn.ButtonClickTween(() => onTurnPageBtnClick(true));
        contentPane.science_leftBtn.ButtonClickTween(() => onTurnPageBtnClick(true));
        contentPane.rightBtn.ButtonClickTween(() => onTurnPageBtnClick(false));
        contentPane.science_rightBtn.ButtonClickTween(() => onTurnPageBtnClick(false));

        contentPane.invest_slider.onValueChanged.AddListener(onSliderValueChanged);

        contentPane.goldBtn.ButtonClickTween(() => onCostBtnClick(0, goldCost));
        contentPane.gemBtn.ButtonClickTween(() =>
        {
            if (contentPane.gemAffirmObj.activeSelf)
            {
                onCostBtnClick(1, gemCost);
                contentPane.gemTip.text = LanguageManager.inst.GetValueByKey(_data.config.architecture_type == 1 ? "改造" : "研究");
                contentPane.gemAffirmObj.SetActive(false);
            }
            else
            {
                contentPane.gemAffirmObj.SetActive(true);
                contentPane.gemTip.text = LanguageManager.inst.GetValueByKey("确定");
            }
        });

        contentPane.toggleGroup.OnSelectedIndexValueChange = OnSelectedValueChange;

        contentPane.superList.itemRenderer = listitemRenderer;
        contentPane.superList.itemUpdateInfo = listitemRenderer;
        contentPane.superList.scrollByItemIndex(0);
        //contentPane.superList.totalItemCount = 0;

        showSliderImg = contentPane.fillTween.GetComponent<Image>();
    }

    protected override void onShown()
    {
        base.onShown();

        contentPane.maskCanvas.sortingLayerName = _uiCanvas.sortingLayerName;
        contentPane.maskCanvas.sortingOrder = _uiCanvas.sortingOrder - 2;
        showSliderImg.DOFade(0.4f, 0.8f).From(0).SetLoops(-1, LoopType.Yoyo);
    }

    protected override void onHide()
    {
        base.onHide();

        DOTween.Kill(showSliderImg);
    }

    private void onTurnPageBtnClick(bool isLeft)
    {
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.CITYBUILDINGINVESTUI_TURNPAGE, isLeft, _data.buildingId);
    }

    string getBuildingTypeTx(int architecture_type)
    {

        string result = "";

        switch (architecture_type)
        {
            case 1: result = "资源"; break;
            case 3: result = "工匠"; break;
            case 4: result = "特殊"; break;
        }

        return result;
    }

    void setCommonContent()
    {
        contentPane.buildingDesTx.text = LanguageManager.inst.GetValueByKey(_data.config.introduction_dec);
        contentPane.lvTx.text = _data.level.ToString();
        contentPane.buildingNameTip.text = LanguageManager.inst.GetValueByKey(_data.config.name);
        contentPane.buildingTypeTx.text = LanguageManager.inst.GetValueByKey(getBuildingTypeTx(_data.config.architecture_type));
        contentPane.gemAffirmObj.SetActive(false);
        clickCount = 0;
    }

    #region 个人部分

    void setMiddle()
    {
        var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(this._data.buildingId, this._data.level + 1);

        contentPane.oldValueTip.text = this._data.GetValueTipStr(true);

        if (_data.buildingId == 3700)
        {
            var cfg = nextUpCfg == null ? _data.upgradeConfig : nextUpCfg;

            contentPane.oldValueTx.text = (UserDataProxy.inst.GetExploreGroupRestTimeSpeedUp(cfg.effect_id) * 100) + "%";
        }
        else if (_data.buildingId == 3800)
        {
            var cfg = nextUpCfg == null ? _data.upgradeConfig : nextUpCfg;

            contentPane.oldValueTx.text = (UserDataProxy.inst.GetExploreDropMaterialOutputUp(cfg.effect_id) * 100) + "%";
        }
        else
        {
            contentPane.oldValueTx.text = _data.upgradeConfig.GetEffectDec();
        }

        contentPane.newValueTx.text = nextUpCfg == null ? LanguageManager.inst.GetValueByKey("已满级") : nextUpCfg.GetEffectDec();
        contentPane.onlyNewValueTx.text = nextUpCfg == null ? LanguageManager.inst.GetValueByKey("已满级") : "+" + nextUpCfg.GetEffectDec();

        contentPane.oldValueTx.gameObject.SetActive(_data.buildingId != 3800);
        contentPane.onlyNewValueTx.gameObject.SetActive(_data.buildingId == 3800);

    }


    void workerDeal()
    {
        if (_data.config.architecture_type != 3) return;

        WorkerData workerData = RoleDataProxy.inst.GetWorker(this._data.config.unlock_id);
        if (workerData != null)
        {
            contentPane.workerLvBgObj.SetActive(workerData.state == EWorkerState.Unlock);

            contentPane.workerLvTx.text = workerData.level.ToString();
            contentPane.workerIcon.SetSprite(StaticConstants.roleHeadIconAtlasName, workerData.config.icon);
            contentPane.science_roleNameTx.text = LanguageManager.inst.GetValueByKey(workerData.config.name);
            contentPane.science_roleTalkTx.text = LanguageManager.inst.GetValueByKey(workerData.config.desc);


            if (roleDress == null)
            {
                CharacterManager.inst.GetCharacterByModel<DressUpSystem>(workerData.config.model, callback: (sys) =>
                {
                    roleDress = sys;
                    roleDress.SetUIPosition(contentPane.science_roleTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1);
                    string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)roleDress.gender, (int)kIndoorRoleActionType.normal_standby);
                    roleDress.SetDirection(FGUI.inst.isLandscape ? RoleDirectionType.Right : RoleDirectionType.Left);
                    roleDress.Play(idleAnimationName, true);
                });
            }
            else
            {
                CharacterManager.inst.ReSetCharacterByModel(roleDress, workerData.config.model);
                roleDress.SetUIPosition(contentPane.science_roleTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)roleDress.gender, (int)kIndoorRoleActionType.normal_standby);
                roleDress.SetDirection(FGUI.inst.isLandscape ? RoleDirectionType.Right : RoleDirectionType.Left);
                roleDress.Play(idleAnimationName, true);
            }

        }
        else
        {
            Logger.error("未获取到对应工匠 工匠ID ： " + this._data.config.unlock_id);
        }
    }

    void setPensonalContent()
    {
        bool isResOrSpecial = _data.config.architecture_type == 1 || _data.config.architecture_type == 4;


        //top
        contentPane.investToggleTip.text = contentPane.investToggleBgTip.text = contentPane.goldTip.text = contentPane.gemTip.text = LanguageManager.inst.GetValueByKey(isResOrSpecial ? "改造" : "研究");
        contentPane.investDes.text = LanguageManager.inst.GetValueByKey(_data.config.functional_dec);
        //contentPane.unionshareLvObj.SetActive(UserDataProxy.inst.playerData.unionId != "");

        //middle
        //contentPane.rankDes.text = LanguageManager.inst.GetValueByKey(_data.config.shared_dec);
        //contentPane.invest_levelUpDes.text = UserDataProxy.inst.playerData.unionId == "" ? "<size=26>" + LanguageManager.inst.GetValueByKey("请先加入一个联盟") + "</size>" : LanguageManager.inst.GetValueByKey("<size=26>联盟技术</size>") + "\n+" + this._data.GetShareEffectDec();
        setMiddle();

        //bottom
        contentPane.invest_sliderTip.text = LanguageManager.inst.GetValueByKey((isResOrSpecial ? "改造" : "研究") + "所需次数");
        contentPane.invest_slider.maxValue = _data.needClickCount;
        contentPane.invest_slider.value = _data.costCount;
        contentPane.curSliderFill.anchorMax = new Vector2(contentPane.invest_slider.value / contentPane.invest_slider.maxValue, 1);
        onAddBtnClick();

        if (BuildingUpgradeConfigManager.inst.GetConfig(this._data.buildingId, this._data.level + 1) == null) //满级
        {
            contentPane.goldNumTx.color = contentPane.gemNumTx.color = Color.red;
            contentPane.goldNumTx.text = contentPane.gemNumTx.text = LanguageManager.inst.GetValueByKey("已满级");
            isMax = true;
        }
        else
        {
            isMax = false;
        }

        //specialDeal

        if (!FGUI.inst.isLandscape)
        {
            contentPane.BottomTf.anchoredPosition = isResOrSpecial ? Vector2.down * 676 : Vector2.down * 760;
        }

        contentPane.investTop.SetActive(isResOrSpecial);
        contentPane.scienceTop.SetActive(!isResOrSpecial);
        contentPane.itemIcon.gameObject.SetActive(isResOrSpecial);

        if (isResOrSpecial)
        {
            string icon = _data.GetIconAndAtlas(out string atlas, _data.level);
            contentPane.itemIcon.SetSprite(atlas, icon);
        }

        contentPane.workerObj.SetActive(_data.config.architecture_type == 3);
        workerDeal();
    }

    #endregion

    void setShareContent()
    {
        //if (UserDataProxy.inst.playerData.unionId == "")
        //{
        //    //没有公会呢
        //    return;
        //}

        //bool isResBuilding = _data.config.architecture_type == 1;
        //商会共享等级部分

        //contentPane.toggle_rank_lvTx.text = contentPane.unionShareLvTx.text = _data.unionShareLevel + "";
        //contentPane.rank_slider.maxValue = _data.shareUpgradeCfg.guild_click_num;
        //contentPane.rank_slider.value = _data.unionShareCostCount;
        //contentPane.rank_slideTx.text = _data.unionShareCostCount + "/" + _data.shareUpgradeCfg.guild_click_num;
        //contentPane.rank_levelUpDes.text = "<size=26>" + LanguageManager.inst.GetValueByKey(/*_data.shareUpgradeCfg.effect_dec*/"联盟技术") + "</size>\n+" + _data.GetShareEffectDec();
    }

    public void SetData(CityBuildingData data)
    {
        bool isSameBuilding = _data == null ? false : _data.buildingId == data.buildingId;

        _data = data;
        isHasRankMess = false;

        bool needSetUrl = _data.config.architecture_type == 1 || _data.config.architecture_type == 4;
        if (needSetUrl && !isSameBuilding) contentPane.buildingIcon.SetSpriteURL(_data.config.big_icon);

        setCommonContent();
        setPensonalContent();
        setShareContent();

        OnSelectedValueChange(_index);
    }


    private void onSliderValueChanged(float value)
    {

        contentPane.invest_sliderChgTx.text = _data.costCount + "(+" + clickCount + ")/" + "<color=#54f942>" + _data.needClickCount + "</color>";
        contentPane.fillTween.anchorMax = new Vector2(contentPane.invest_slider.value / contentPane.invest_slider.maxValue, 1);
        contentPane.addSelfUnionTokenTx.text = "+" + (clickCount * (int)WorldParConfigManager.inst.GetConfig(3004).parameters);

        //计算消耗钻石金币数量
        goldCost = BuildingCostConfigManager.inst.GetInvestCost(_data.oneSelfCostCount, clickCount, _data.config.cost_grade, out gemCost);
        contentPane.goldNumTx.text = AbbreviationUtility.AbbreviateNumber(goldCost, 2);
        contentPane.gemNumTx.text = AbbreviationUtility.AbbreviateNumber(gemCost, 2);

        contentPane.goldNumTx.color = (ulong)UserDataProxy.inst.playerData.gold < goldCost ? Color.red : Color.white;
        //contentPane.gemNumTx.color = (ulong)UserDataProxy.inst.playerData.gem < gemCost ? Color.red : Color.white;
        contentPane.gemNumTx.color = Color.white;
    }

    private void onDelBtnClick()
    {

        if (contentPane.invest_slider.value > _data.costCount + 1)
        {
            clickCount -= 1;
            contentPane.invest_slider.value -= 1;
        }
    }

    private void onAddBtnClick()
    {

        if (contentPane.invest_slider.value != contentPane.invest_slider.maxValue && !isMax)
        {
            goldCost = BuildingCostConfigManager.inst.GetInvestCost(_data.oneSelfCostCount, clickCount + 1, _data.config.cost_grade, out gemCost);
            if ((ulong)UserDataProxy.inst.playerData.gold < goldCost && (ulong)UserDataProxy.inst.playerData.gem < gemCost && clickCount != 0)
            {
                goldCost = BuildingCostConfigManager.inst.GetInvestCost(_data.oneSelfCostCount, clickCount, _data.config.cost_grade, out gemCost);
                return;
            }

            clickCount += 1;
            contentPane.invest_slider.value += 1;
        }
    }

    private void onCostBtnClick(int moneyType, ulong moneyNum)
    {
        if (isMax)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("建筑物已达到最高级"), Color.white);
            return;
        }

        if (moneyType == 0)
        {
            if ((ulong)UserDataProxy.inst.playerData.gold < moneyNum)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else
        {
            if ((ulong)UserDataProxy.inst.playerData.gem < moneyNum)
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, moneyNum - (ulong)UserDataProxy.inst.playerData.gem);
                return;
            }
        }

        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.CITYBUILDING_INVEST, _data.buildingId, moneyType, clickCount);
    }

    private void OnSelectedValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        _index = index;

        foreach (var item in contentPane.toggleLinks)
        {
            item.SetActive(false);
        }


        contentPane.toggleLinks[index].SetActive(true);

        if (index == 1) //商会联盟共享等级
        {
            refreshUnionObjInfo();
        }

    }

    void refreshUnionObjInfo()
    {

        //if (UserDataProxy.inst.playerData.unionId == "")
        //{
        //    contentPane.hasUnionObj.SetActiveFalse();
        //    contentPane.notHasUnionObj.SetActiveTrue();
        //}
        //else
        //{
        //    contentPane.hasUnionObj.SetActiveTrue();
        //    contentPane.notHasUnionObj.SetActiveFalse();

        if (!isHasRankMess)
        {
            contentPane.superList.totalItemCount = 0;
            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.CITYBUILDING_INVEST_RANK_DATA, _data.buildingId);
        }
        //}

    }

    public void RefreashUnionRankListInfo(List<BuildTopList> rankList)
    {
        isHasRankMess = true;

        this.rankList = rankList;
        this.rankList.Sort((a, b) => -a.investNum.CompareTo(b.investNum));

        contentPane.superList.totalItemCount = this.rankList.Count;
    }

    ///无限滑动
    List<BuildTopList> rankList;

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        InvestRankItem item = (InvestRankItem)obj;
        item.SetData(rankList[index], _data.config.cost_grade);
    }
}
