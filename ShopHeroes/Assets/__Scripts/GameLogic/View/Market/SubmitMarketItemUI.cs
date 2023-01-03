using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.InputField;

public class SubmitMarketItemUI : ViewBase<SubmitMarketItemUIComp>
{
    public override string viewID => ViewPrefabName.SubmitMarketItemUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;


        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.backBtn.ButtonClickTween(onBackBtnClick);
        contentPane.nextBtn.ButtonClickTween(onNextBtnClick);

        contentPane.toggleGroup.OnSelectedIndexValueChange = OnSelectedValueChange;
        contentPane.Num_superDelBtn.onClick.AddListener(Num_superDelBtnOnClick);
        contentPane.Num_delBtn.onClick.AddListener(Num_delBtnOnClick);
        contentPane.Num_addBtn.onClick.AddListener(Num_addBtnOnClick);
        contentPane.Num_superAddBtn.onClick.AddListener(Num_superAddBtnOnClick);

        contentPane.Price_superDelBtn.onClick.AddListener(Price_superDelBtnOnClick);
        contentPane.Price_delBtn.onClick.AddListener(Price_delBtnOnClick);
        contentPane.Price_addBtn.onClick.AddListener(Price_addBtnOnClick);
        contentPane.Price_superAddBtn.onClick.AddListener(Price_superAddBtnOnClick);
        contentPane.Price_inputField.onEndEdit.AddListener(Price_inputFieldOnValueChanged);
        contentPane.Price_changeMoneyBtn.onClick.AddListener(() => Price_onMoneyTypeChanged(price_curMonetyType == 0 ? 1 : 0));

        contentPane.Rarity_ToggleGroup.OnSelectedIndexValueChange = Rarity_onToggleIndexValueChanged;
        contentPane.Rarity_toggle_isSuper.onValueChanged.AddListener(Rarity_onSuperToggleValueChanged);

        contentPane.tipsBtn.onClick.AddListener(() => { contentPane.tipsObj.SetActive(true); });
        contentPane.tipsMaskBtn.onClick.AddListener(() => { contentPane.tipsObj.SetActive(false); });

        contentPane.Time_delBtn.onClick.AddListener(Time_delBtnOnClick);
        contentPane.Time_addBtn.onClick.AddListener(Time_addBtnOnClick);

    }

    protected override void onHide()
    {
        base.onHide();

        Clear();
        GameTimer.inst.RemoveTimer(refreshTimer);
        refreshTimer = 0;
    }


    private kMarketItemType _marketType;
    private int curIndex;
    private int curItemType, id, num, quality, unitPrice, afterTaxPrice, timeIndex;  //物品类型 物品ID 数量 质量 单价 税后单价 时间下标

    private bool isEquip, isEndStep, isThree;


    private EquipConfig curEquipCfg;
    private itemConfig curItemCfg;

    private int s_marketGoldPrice;
    private int s_marketGemPrice;

    private int refreshTimer;
    private int timer;

    private const int RefreshTime = 5;

    private void Clear()  //每次进行清除
    {
        num_submitLimitNum = 0;
        num_curItemCount = 0;

        rarity_curIndex = 0;

        price_basicsPrice = 0;
        price_curPrice = 0;

        s_marketGoldPrice = 0;
        s_marketGemPrice = 0;
        timeIndex = 0;
        curIndex = 0;
        curEquipCfg = null;
        curItemCfg = null;
    }

    private void Init() //每次进行初始化
    {
        contentPane.Num_toggleTx.text = "-";

        contentPane.Rarity_toggleTx.text = "-";
        contentPane.Rarity_toggleTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);

        contentPane.Price_toggleTx.text = "-";
        contentPane.Price_toggleIcon.SetSprite("__common_1", "zhuejiemian_meiyuan");

        contentPane.confirmUIObj.SetActive(false);
        contentPane.toggleGroup.gameObject.SetActive(true);
        contentPane.toggleGroup.OnEnableMethod();
        contentPane.toggleLinkObj.gameObject.SetActive(true);
        isEndStep = false;
    }

    public void SetData(ToSubmitMarketItemData toSubmitMarketItemData)  //itemType 0 装备 1 物品
    {
        _marketType = toSubmitMarketItemData.marketType;
        curItemType = toSubmitMarketItemData.itemType;
        id = toSubmitMarketItemData.itemId;
        quality = toSubmitMarketItemData.itemQuality;

        isEquip = curItemType == 0;

        curEquipCfg = isEquip ? EquipConfigManager.inst.GetEquipInfoConfig(id, quality) : null;
        curItemCfg = !isEquip ? ItemconfigManager.inst.GetConfig(id) : null;

        contentPane.toggleGroup.ClearTogglesBtn();
        switch (_marketType)
        {
            case kMarketItemType.selfSell: //报价
                SetToggleGroupNum(true);
                break;
            case kMarketItemType.selfBuy:  //请求
                SetToggleGroupNum(curItemType == 1);
                break;
            case kMarketItemType.UnionBuy:
                break;
        }

        Init();
        showTopInfo();

        timer = 0;
        refreshTimer = GameTimer.inst.AddTimer(1, timingRefreshMethod);
    }

    private void timingRefreshMethod()
    {
        if (timer > 0) timer -= 1;
    }

    //定时刷新回调
    public void timingRefreshCallBackMethod(MarketItem marketItem)
    {
        s_marketGoldPrice = marketItem.goldPrice;
        s_marketGemPrice = marketItem.gemPrice;
        contentPane.Num_marketNumTx.text = marketItem.marketNum > 100 ? "100+" : marketItem.marketNum.ToString();
        contentPane.Price_marketLowestPriceTx.text = price_curMonetyType == 0 ? marketItem.goldPrice.ToString("N0") : marketItem.gemPrice.ToString("N0");
    }

    private void SetToggleGroupNum(bool isThree)
    {
        if (isThree)
        {
            contentPane.toggles[1].gameObject.SetActive(false);
            contentPane.toggleGroup.togglesBtn.AddRange(new Toggle[] { contentPane.toggles[0], contentPane.toggles[2], contentPane.toggles[3] });
            contentPane.toggleGroup.SetToggleSize(new Vector2(370, 128f), new Vector2(322, 128f));
        }
        else
        {
            contentPane.toggles[1].gameObject.SetActive(true);
            contentPane.toggleGroup.togglesBtn.AddRange(contentPane.toggles);
            contentPane.toggleGroup.SetToggleSize(new Vector2(264, 128f), new Vector2(250, 128f));
        }

        this.isThree = isThree;
    }

    private void showTopInfo()
    {
        string atlas = isEquip ? curEquipCfg.equipDrawingsConfig.atlas : curItemCfg.atlas;
        string icon = isEquip ? curEquipCfg.equipDrawingsConfig.icon : curItemCfg.icon;

        int level = isEquip ? curEquipCfg.equipDrawingsConfig.level : 1;
        string name = isEquip ? curEquipCfg.name : curItemCfg.name;

        contentPane.Top_qualityIcon.gameObject.SetActive(isEquip);
        contentPane.Top_subTypeIcon.gameObject.SetActive(isEquip);
        if (isEquip)
        {
            contentPane.Top_qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[curEquipCfg.equipQualityConfig.quality - 1]);
            GUIHelper.showQualiyIcon(contentPane.Top_qualityIcon.GetComponent<RectTransform>(), curEquipCfg.equipQualityConfig.quality);

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(curEquipCfg.equipDrawingsConfig.sub_type);
            contentPane.Top_subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

            contentPane.Top_iconBg.SetSprite("__common_1", curEquipCfg.equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");
            contentPane.Top_superEquipSignObj.SetActive(curEquipCfg.equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality);
        }
        else
        {
            contentPane.Top_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
            contentPane.Top_superEquipSignObj.SetActive(false);
        }

        contentPane.Top_itemIcon.SetSprite(atlas, icon);
        contentPane.Top_lvTx.text = level.ToString();
        contentPane.Top_nameTx.text = LanguageManager.inst.GetValueByKey(name.ToString());
        contentPane.Top_nameTx.color = isEquip ? GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[curEquipCfg.equipQualityConfig.quality - 1]) : Color.white;

        contentPane.Top_typeTx.text = LanguageManager.inst.GetValueByKey(_marketType == kMarketItemType.selfSell ? "报价" : "请求");
        contentPane.Top_typeTx.color = _marketType == kMarketItemType.selfSell ? GUIHelper.GetColorByColorHex("#78f452") : GUIHelper.GetColorByColorHex("#3eeeff");
        contentPane.Top_arrowIcon.SetSprite("market_atlas", _marketType == kMarketItemType.selfSell ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
        contentPane.Top_arrowIcon.transform.localScale = new Vector3(_marketType == kMarketItemType.selfSell ? -1 : 1, 1, 1);

        contentPane.topBgIcon.SetSprite("market_atlas_newAdd", _marketType == kMarketItemType.selfSell ? "shichang_banner2" : "shichang_banner1");
    }

    private void onBackBtnClick()
    {
        if (contentPane.toggleGroup.selectedIndex == 0)
        {
            hide();
        }
        else
        {
            if (contentPane.confirmUIObj.activeSelf)
            {
                contentPane.confirmUIObj.SetActive(false);
                contentPane.toggleGroup.gameObject.SetActive(true);
                contentPane.toggleLinkObj.gameObject.SetActive(true);
                contentPane.toggleGroup.selectedIndex = contentPane.toggleGroup.togglesBtn.Count - 1;
                isEndStep = false;
            }
            else
            {
                contentPane.toggleGroup.selectedIndex -= 1;
            }

        }
    }


    private void onNextBtnClick()
    {

        if (isEndStep)
        {
            if (_marketType == kMarketItemType.selfBuy) //自己买 判断金币或钻石是否足够 
            {
                if (price_curMonetyType == 0)
                {
                    if (UserDataProxy.inst.playerData.gold < num * (long)afterTaxPrice)
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足！"), GUIHelper.GetColorByColorHex("FF2828"));
                        return;
                    }
                }
                else
                {
                    if (UserDataProxy.inst.playerData.gem < num * (long)afterTaxPrice)
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足！"), GUIHelper.GetColorByColorHex("FF2828"));
                        return;
                    }
                }
            }

            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_BOOTHITEMSUBMIT, new Request_submitBoothItemData()
            { itemType = curItemType, itemId = id, itemQuality = quality, itemNum = num, buyOrSell = (int)_marketType, moneyType = price_curMonetyType, unitPrice = unitPrice, timeIndex = this.timeIndex });
            return;
        }

        if (contentPane.toggleGroup.selectedIndex < contentPane.toggleGroup.togglesBtn.Count - 1)
        {
            contentPane.toggleGroup.selectedIndex += 1;
            contentPane.rightBtnTx.text = LanguageManager.inst.GetValueByKey("下一步");
            isEndStep = false;
        }
        else
        {
            contentPane.toggleGroup.gameObject.SetActive(false);
            contentPane.toggleLinkObj.gameObject.SetActive(false);
            contentPane.qualityObj.SetActive(false);
            contentPane.confirmUIObj.SetActive(true);
            contentPane.rightBtnTx.text = LanguageManager.inst.GetValueByKey("确认");
            isEndStep = true;



            contentPane.numTx.text = num.ToString();
            if (_marketType != kMarketItemType.selfSell)
            {
                contentPane.qualityObj.SetActive(true);
                contentPane.qualityTx.text = LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[quality - 1]);
                contentPane.qualityTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[quality - 1]);
            }
            contentPane.unitPriceTx.text = afterTaxPrice.ToString("N0");
            contentPane.continueTimeTx.text = StaticConstants.time_times[timeIndex] + LanguageManager.inst.GetValueByKey("小时");

            contentPane.unitMoneyIcon.SetSprite("__common_1", price_curMonetyType == 0 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");

            contentPane.tips_tip2.text = LanguageManager.inst.GetValueByKey(_marketType != kMarketItemType.selfSell ? "您支付（每个）" : "您将获得（每个）");
            contentPane.tips_content1.text = unitPrice.ToString("N0");
            contentPane.tips_content2.text = afterTaxPrice.ToString("N0");
        }
    }

    private void OnSelectedValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        int temp = curIndex;

        if (isThree)
        {
            curIndex = index == 0 ? 0 : index + 1;
            curIndex = Mathf.Clamp(curIndex, 0, contentPane.toggleLinks.Length - 1);
        }
        else
        {
            curIndex = index;
        }

        foreach (var item in contentPane.toggleLinks)
        {
            item.SetActive(false);
        }

        contentPane.toggleLinks[curIndex].SetActive(true);

        SetCurState(curIndex >= temp);
    }


    private void SetCurState(bool isFromLeftOrRight) //true 下一步 false 返回
    {

        switch (curIndex) // 0 数量 1 稀有度 2 价格 3 持续时间
        {
            case 0: SetNumState(isFromLeftOrRight); break;
            case 1: SetRarityState(isFromLeftOrRight); break;
            case 2: SetPriceState(isFromLeftOrRight); break;
            case 3: SetTimeState(isFromLeftOrRight); break;
        }

    }

    #region 数量

    int num_submitLimitNum;
    int num_curItemCount;


    private void Num_superAddBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        num_curItemCount += 5;
        Num_SetInputFieldTx();
    }

    private void Num_addBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        num_curItemCount += 1;
        Num_SetInputFieldTx();
    }

    private void Num_delBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        num_curItemCount -= 1;
        Num_SetInputFieldTx();
    }

    private void Num_superDelBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        num_curItemCount -= 5;
        Num_SetInputFieldTx();
    }

    private void Num_SetInputFieldTx()
    {
        num_curItemCount = Mathf.Clamp(num_curItemCount, 1, num_submitLimitNum);
        contentPane.Num_inputField.text = num_curItemCount.ToString();
    }

    private void SetNumState(bool isFromLeftOrRight)
    {
        //------定时刷新
        if (timer <= 0)
        {
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, new Request_marketItemData() { buyOrSell = (int)_marketType, itemType = curItemType, itemId = id, itemQuality = quality });
            timer = RefreshTime;
        }

        if (!isFromLeftOrRight)
        {
            contentPane.Rarity_ToggleGroup.OnEnableMethod();
            contentPane.Rarity_toggle_isSuper.isOn = false;
        }

        //-----
        contentPane.Num_toggleTx.text = "-";
        Num_SetInputFieldTx();


        //-----
        if (_marketType == kMarketItemType.selfSell)
        {
            num_submitLimitNum = isEquip ? ItemBagProxy.inst.getEquipNumber(curEquipCfg.equipDrawingId, curEquipCfg.equipQualityConfig.quality) : (int)ItemBagProxy.inst.GetItem(curItemCfg.id).count;
            contentPane.Num_content1.text = num_submitLimitNum.ToString();
            num_submitLimitNum = Mathf.Clamp(num_submitLimitNum, 1, isEquip ? MarketDataProxy.inst.GetSubmitEquipLimit(_marketType) : MarketDataProxy.inst.GetSubmittMaterialLimit(_marketType));

            contentPane.Num_tip1.text = LanguageManager.inst.GetValueByKey("物品栏中：");
        }
        else
        {
            contentPane.Num_tip1.text = LanguageManager.inst.GetValueByKey("上限：");
            num_submitLimitNum = isEquip ? MarketDataProxy.inst.GetSubmitEquipLimit(_marketType) : MarketDataProxy.inst.GetSubmittMaterialLimit(_marketType); //  装备请求数量上限为25，材料请求数量上限为50。
            contentPane.Num_content1.text = num_submitLimitNum.ToString();
        }


    }
    #endregion

    #region 稀有度

    int rarity_curIndex;
    bool rarity_isSuper;

    private void Rarity_onToggleIndexValueChanged(int qualityIndex)
    {
        rarity_curIndex = qualityIndex;
        contentPane.Top_qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[qualityIndex]);
        GUIHelper.showQualiyIcon(contentPane.Top_qualityIcon.GetComponent<RectTransform>(), qualityIndex + 1);
        contentPane.Top_nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[qualityIndex]);
    }

    private void Rarity_onSuperToggleValueChanged(bool isOn)
    {
        if (!isEquip || _marketType == kMarketItemType.selfSell) return;

        for (int i = 0; i < contentPane.Rarity_toggle_isSuper.graphic.transform.childCount; i++)
        {
            contentPane.Rarity_toggle_isSuper.graphic.transform.GetChild(i).gameObject.SetActive(contentPane.Rarity_toggle_isSuper.isOn);
        }

        if (isOn)
        {
            rarity_isSuper = true;
            contentPane.Top_nameTx.text = LanguageManager.inst.GetValueByKey("<color=#fb1470>超凡</color>") + "<color=#ffffff>·</color>" + LanguageManager.inst.GetValueByKey(curEquipCfg.equipDrawingsConfig.name);

            contentPane.Top_iconBg.SetSprite("__common_1", "cktb_wupinkuang_super");
            contentPane.Top_superEquipSignObj.SetActive(true);
        }
        else
        {
            rarity_isSuper = false;
            contentPane.Top_nameTx.text = LanguageManager.inst.GetValueByKey(curEquipCfg.equipDrawingsConfig.name);
            contentPane.Top_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
            contentPane.Top_superEquipSignObj.SetActive(false);
        }

    }

    private void SetRarityState(bool isFromLeftOrRight)
    {
        if (isFromLeftOrRight)
        {
            num = int.Parse(contentPane.Num_inputField.text);
            contentPane.Num_toggleTx.text = num.ToString();
        }
        else
        {
        }

        contentPane.Rarity_toggle_isSuper.gameObject.SetActive(isEquip && _marketType == kMarketItemType.selfBuy);
        rarity_isSuper = false;

        contentPane.Rarity_toggleTx.text = "-";
        contentPane.Rarity_toggleTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);

        contentPane.Rarity_ToggleGroup.selectedIndex = rarity_curIndex;
    }

    #endregion

    #region 价格
    int price_basicsPrice;
    int price_curPrice;
    int price_curMonetyType; // 0 金币 1 钻石

    private void Price_superAddBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        price_curPrice += price_curMonetyType == 0 ? Mathf.CeilToInt(price_basicsPrice * 0.5f) : 5;
        Price_inputFieldOnValueChanged(price_curPrice.ToString());
    }

    private void Price_addBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        price_curPrice += price_curMonetyType == 0 ? Mathf.CeilToInt(price_basicsPrice * 0.05f) : 1;
        Price_inputFieldOnValueChanged(price_curPrice.ToString());
    }

    private void Price_delBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        price_curPrice -= price_curMonetyType == 0 ? Mathf.CeilToInt(price_basicsPrice * 0.05f) : 1;
        Price_inputFieldOnValueChanged(price_curPrice.ToString());
    }

    private void Price_superDelBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        price_curPrice -= price_curMonetyType == 0 ? Mathf.CeilToInt(price_basicsPrice * 0.5f) : 5;
        Price_inputFieldOnValueChanged(price_curPrice.ToString());
    }


    private void Price_inputFieldOnValueChanged(string value)
    {

        long inputPrice = -1;
        value = value.Replace(",", "");
        int length = value.Length > 10 ? 10 : value.Length;
        long.TryParse(value.Substring(0, length), out inputPrice);
        int inputResult = inputPrice > int.MaxValue ? int.MaxValue : (int)inputPrice;

        contentPane.Price_inputField.contentType = InputField.ContentType.Standard;
        contentPane.Price_inputField.lineType = LineType.MultiLineNewline;
        contentPane.Price_inputField.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
        price_curPrice = price_curMonetyType == 0 ? Mathf.Clamp(inputResult, Mathf.CeilToInt(price_basicsPrice * 0.2f + 1), price_basicsPrice * 10) : Mathf.Clamp(inputResult, 2, isEquip ? EquipConfigManager.inst
            .GetEquipSubmitGemMax(curEquipCfg.equipDrawingId, curEquipCfg.equipQualityConfig.quality) : curItemCfg.amount_cap);
        contentPane.Price_inputField.text = price_curPrice.ToString("N0");

    }

    private void Price_onMoneyTypeChanged(int moneyType) // 0 金币 1 钻石
    {
        var acheivementData = AcheivementDataProxy.inst.GetAcheivementDataById((int)WorldParConfigManager.inst.GetConfig(154).parameters);
        if (moneyType == 1 && acheivementData != null && acheivementData.state == EAchievementState.Doing) //钻石有判断条件
        {
            EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_MSGBOX_NEEDACHEIVEMENT, (int)WorldParConfigManager.inst.GetConfig(154).parameters, LanguageManager.inst.GetValueByKey("在市场使用金条之前，您需要先解锁此成就。"));
            return;
        }

        price_curMonetyType = moneyType;

        string iconName = moneyType == 0 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai";
        contentPane.Price_toggleIcon.SetSprite("__common_1", iconName);
        contentPane.Price_basicIcon.SetSprite("__common_1", iconName);
        contentPane.Price_lowestIcon.SetSprite("__common_1", iconName);
        contentPane.Time_moneyIcon.SetSprite("__common_1", iconName);
        contentPane.Price_toggleCheckmask.SetSprite("__common_1", iconName);


        //EquipItem equipItem = null;

        //if (isEquip)
        //{
        //    equipItem = ItemBagProxy.inst.GetEquipItem(curEquipCfg.equipDrawingId, quality);
        //}

        price_curPrice = price_basicsPrice = moneyType == 1 ? StaticConstants.lowestGemPrice : isEquip ? /*(equipItem == null ? curEquipCfg.equipQualityConfig.price_gold : equipItem.sellPrice)*/curEquipCfg.equipQualityConfig.auction_price : curItemCfg.low_price_m;
        contentPane.Price_basicsPriceTx.text = moneyType == 1 ? LanguageManager.inst.GetValueByKey("不适用") : price_basicsPrice.ToString("N0");
        contentPane.Price_inputField.text = price_curPrice.ToString("N0");

        contentPane.Price_marketLowestPriceTx.text = price_curMonetyType == 0 ? s_marketGoldPrice.ToString("N0") : s_marketGemPrice.ToString("N0");

    }

    private void SetPriceState(bool isFromLeftOrRight)
    {
        //------定时刷新
        if (timer <= 0)
        {
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, new Request_marketItemData() { buyOrSell = (int)_marketType, itemType = curItemType, itemId = id, itemQuality = quality });
            timer = RefreshTime;
        }

        if (isFromLeftOrRight)
        {
            if (isEquip && !isThree)
            {
                quality = contentPane.Rarity_ToggleGroup.selectedIndex + 1 + (rarity_isSuper ? StaticConstants.SuperEquipBaseQuality : 0);
                curEquipCfg = EquipConfigManager.inst.GetEquipInfoConfig(id, quality);
            }

            if (!isThree)
            {
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, new Request_marketItemData() { buyOrSell = (int)_marketType, itemType = curItemType, itemId = id, itemQuality = quality });
                timer = RefreshTime;
            }

            num = int.Parse(contentPane.Num_inputField.text);
            contentPane.Num_toggleTx.text = num.ToString();
            contentPane.Rarity_toggleTx.text = LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[quality - 1]);
            contentPane.Rarity_toggleTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[quality - 1]);

            Price_onMoneyTypeChanged(0);
        }
        else
        {
            contentPane.Price_basicsPriceTx.text = price_basicsPrice.ToString("N0");
        }

        contentPane.Price_toggleTx.text = "-";
        contentPane.Price_inputField.text = price_curPrice.ToString("N0");


    }

    #endregion


    #region 持续时间

    private void Time_addBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        timeIndex += 1;
        refreshTimeInfo();
    }

    private void Time_delBtnOnClick()
    {
        AudioManager.inst.PlaySound(125);
        timeIndex -= 1;
        refreshTimeInfo();
    }

    private void refreshTimeInfo()
    {
        timeIndex = Mathf.Clamp(timeIndex, 0, StaticConstants.time_times.Length - 1);

        contentPane.Time_inputField.text = StaticConstants.time_times[timeIndex] + LanguageManager.inst.GetValueByKey("小时");
        contentPane.Time_taxRateTx.text = (StaticConstants.time_taxRates[price_curMonetyType][timeIndex] * 100) + "%";

        if (_marketType != kMarketItemType.selfSell)
        {
            afterTaxPrice = Mathf.CeilToInt(unitPrice / (1f - StaticConstants.time_taxRates[price_curMonetyType][timeIndex]));
        }
        else
        {
            afterTaxPrice = Mathf.CeilToInt(unitPrice * (1f - StaticConstants.time_taxRates[price_curMonetyType][timeIndex]));
        }

        contentPane.Time_content2Tx.text = afterTaxPrice.ToString("N0");
    }

    private void SetTimeState(bool isFromLeftOrRight)
    {

        if (isFromLeftOrRight)
        {
            unitPrice = price_curPrice;
            contentPane.Price_toggleTx.text = AbbreviationUtility.AbbreviateNumber(unitPrice, 2);

            contentPane.Time_tip2.text = LanguageManager.inst.GetValueByKey(_marketType != kMarketItemType.selfSell ? "您将支付：" : "您将收到：");

            refreshTimeInfo();

        }
        else
        {

        }

    }



    #endregion

}
