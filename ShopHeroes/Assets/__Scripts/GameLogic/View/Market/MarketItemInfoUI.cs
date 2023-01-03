using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketItemInfoUI : ViewBase<MarketItemInfoComp>
{
    public override string viewID => ViewPrefabName.MarketItemInfoUI;
    public override string sortingLayerName => "window";

    private MarketItem _item;
    private kMarketTradingHallType _hallType;

    private bool isBuy;

    //private bool isInDealing; //是否正在交易

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;


        //isInDealing = false;

        contentPane.infoBtn.onClick.AddListener(() =>
        {
            if (_item != null)
            {
                var cfg = EquipConfigManager.inst.GetEquipQualityConfig(_item.itemId, _item.itemQuality);
                if (_item.itemType == 0) EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, "", cfg.id, new List<EquipItem>());
                else EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, _item.itemId);
            }
        }
         );

        contentPane.closeBtn.onClick.AddListener(onCloseBtnClick);
        contentPane.maskBtn.onClick.AddListener(onCloseBtnClick);
        contentPane.goldBtn.onClick.AddListener(() => onBtnClick(0, _item.goldPrice));
        contentPane.gemBtn.onClick.AddListener(() =>
        {
            if (contentPane.gemAffirmObj.activeSelf)
            {
                onBtnClick(1, _item.gemPrice);
            }
            else
            {
                contentPane.gemAffirmObj.SetActive(true);
                contentPane.gemTip.text = LanguageManager.inst.GetValueByKey("确定");
            }
        }
        );
    }

    private void onCloseBtnClick()
    {
        /*if (!isInDealing)*/
        hide();
    }

    protected override void onHide()
    {
        base.onHide();
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_MARKETHALLREFRESH);

        isRequied = false;
    }

    private void onBtnClick(int moneyType, int costMoney)
    {
        //判断是否可以交易
        if (isBuy)
        {
            if (_item.itemType == 0) //装备 要判断物品栏容量
            {
                if (ItemBagProxy.inst.GetEquipInventory() >= ItemBagProxy.inst.bagCountLimit)
                {
                    if (UserDataProxy.inst.playerData.gold < _item.goldPrice)
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("物品栏已满"), GUIHelper.GetColorByColorHex("FFD907"));
                        return;
                    }
                }
            }


            if (moneyType == 0)
            {
                if (UserDataProxy.inst.playerData.gold < _item.goldPrice)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
            }
            else
            {
                if (UserDataProxy.inst.playerData.gem < _item.gemPrice)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, _item.gemPrice - UserDataProxy.inst.playerData.gem);
                    return;
                }
            }
        }
        else
        {
            if (_item.itemType == 0)
            {
                EquipItem item = ItemBagProxy.inst.GetEquipItem(_item.itemId, _item.itemQuality);

                if (item == null || item.count <= 0 || item.isLock)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您没有满足要求的物品，") + "\n" + LanguageManager.inst.GetValueByKey("或是它在您的物品栏中已被标记为锁定。"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
            }
            else
            {
                if (ItemBagProxy.inst.GetItem(_item.itemId).count <= 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您没有满足要求的物品，") + "\n" + LanguageManager.inst.GetValueByKey("或是它在您的物品栏中已被标记为锁定。"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
            }
        }


        if (moneyType == 1)
        {
            var acheivementData = AcheivementDataProxy.inst.GetAcheivementDataById((int)WorldParConfigManager.inst.GetConfig(154).parameters);
            if (acheivementData != null && acheivementData.state == EAchievementState.Doing) //钻石有判断条件
            {
                EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_MSGBOX_NEEDACHEIVEMENT, (int)WorldParConfigManager.inst.GetConfig(154).parameters, LanguageManager.inst.GetValueByKey("在市场使用金条之前，您需要先解锁此成就。"));
                return;
            }
        }

        contentPane.goldProcessingTip.text = contentPane.gemProcessingTip.text = LanguageManager.inst.GetValueByKey("正在处理......");
        onRefreshMethod(moneyType, costMoney);
    }

    //刷新方法
    private void onRefreshMethod(int moneyType, int costMoney)
    {
        contentPane.goldBtn.enabled = false;
        contentPane.gemBtn.enabled = false;

        setBtnRefreshAni(true);

        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEM_DEALWITH, new Request_marketItemData() { buyOrSell = (int)_hallType, itemType = _item.itemType, itemId = _item.itemId, itemQuality = _item.itemQuality }, moneyType, costMoney);
    }

    //刷新回调
    public void refreshCallBackMethod(MarketItem marketItem, kMarketTradingHallType hallType, int reason) //1 : 成功， -1:价格不匹配,-2:交易改变(取消),-3:对象为自己
    {
        SetData(marketItem, hallType);

        if (reason != 1) //失败
        {
            string showString = "";
            switch (reason)
            {
                case -1: showString = "价格已发生变动，请重试"; break;
                case -2: showString = "无法找到这笔交易的列表。请刷新后重试"; break;
                case -3: showString = hallType == kMarketTradingHallType.selfBuy ? "您无法购买自己的报价！" : "您无法满足自己的请求！"; break;
                default: showString = "交易失败 请重试"; break;
            }
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey(showString), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (isRequied && marketItem.itemType == 0 && ItemBagProxy.inst.getEquipNumber(_needEquipOrItemId) >= _needNum)//是需求的 判断是否达到需求 达到需求关闭(仅装备)
        {
            hide();
        }
        else if (!isRequied && marketItem.marketNum <= 0) //非需求的 市场上无此物品关闭
        {
            hide();
        }
    }

    private bool isRequied, _allQuality;
    private int _needEquipOrItemId, _needNum;
    public void SetRequiredData(int itemType, int equipIdOrItemId, int needNum, bool _allQuality)
    {
        EquipConfig equipCfg = null;
        itemConfig itemConfig = null;

        if (itemType == 0)
        {
            equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(equipIdOrItemId);
        }
        else
        {
            itemConfig = ItemconfigManager.inst.GetConfig(equipIdOrItemId);
        }


        isRequied = true;
        this._allQuality = itemType == 0 ? _allQuality : false;
        _needEquipOrItemId = equipIdOrItemId;
        _needNum = needNum;


        string atlas = itemType == 0 ? equipCfg.equipDrawingsConfig.atlas : itemConfig.atlas;
        string icon = itemType == 0 ? equipCfg.equipDrawingsConfig.big_icon : itemConfig.icon;
        //contentPane.itemIcon.SetSprite(atlas, icon);
        contentPane.itemIcon.SetSpriteURL(icon);

        int quality = 0;

        if (itemType == 0) //装备
        {

            contentPane.subTypeIcon.gameObject.SetActiveTrue();

            quality = equipCfg.equipQualityConfig.quality;
            contentPane.equipQuality.gameObject.SetActiveTrue();
            contentPane.equipQuality.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[quality - 1]);
            GUIHelper.showQualiyIcon(contentPane.equipQuality.GetComponent<RectTransform>(), quality, 300);
            contentPane.itemNameText.text = equipCfg.name;
            contentPane.levelText.text = equipCfg.equipDrawingsConfig.level.ToString();
            contentPane.inventoryNumText.text = ItemBagProxy.inst.getEquipNumber(equipIdOrItemId, quality).ToString("N0");

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equipCfg.equipDrawingsConfig.sub_type);
            contentPane.subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

        }
        else //资源
        {
            contentPane.equipQuality.gameObject.SetActiveFalse();
            contentPane.subTypeIcon.gameObject.SetActiveFalse();

            contentPane.itemNameText.text = LanguageManager.inst.GetValueByKey(itemConfig.name);
            contentPane.levelText.text = string.Empty;
            contentPane.inventoryNumText.text = ItemBagProxy.inst.GetItem(equipIdOrItemId).count.ToString("N0");

        }

        contentPane.marketNumText.text = contentPane.goldLowPriceText.text = "0";

        contentPane.titleText.text = LanguageManager.inst.GetValueByKey("通过市场购买");
        contentPane.goldTip.text = LanguageManager.inst.GetValueByKey("新币价格");
        contentPane.gemTip.text = LanguageManager.inst.GetValueByKey("金条价格");

        contentPane.goldProcessingTip.text = contentPane.gemProcessingTip.text = LanguageManager.inst.GetValueByKey("正在获取......");

        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, new Request_marketItemData() { buyOrSell = 0, itemType = itemType, itemId = itemType == 0 ? equipCfg.equipDrawingId : equipIdOrItemId, itemQuality = quality });
        setBtnRefreshAni(true);
    }

    public void SetData(MarketItem item, kMarketTradingHallType hallType)
    {
        _hallType = hallType;
        setBtnRefreshAni(false);
        contentPane.gemAffirmObj.SetActive(false);
        setInfo(item);
    }

    private void setInfo(MarketItem item)
    {
        _item = item;


        isBuy = _hallType == kMarketTradingHallType.selfBuy;

        contentPane.titleText.text = LanguageManager.inst.GetValueByKey(isBuy ? "通过市场购买" : "通过物品栏满足");
        contentPane.goldTip.text = LanguageManager.inst.GetValueByKey(isBuy ? "新币价格" : "新币收入");
        contentPane.gemTip.text = LanguageManager.inst.GetValueByKey(isBuy ? "金条价格" : "金条收入");


        contentPane.marketNumText.text = item.marketNum > 100 ? "100+" : item.marketNum.ToString();

        bool isNotHaveGoldDeal = _item.goldPrice <= 0;
        bool isNotHaveGemDeal = _item.gemPrice <= 0;

        GUIHelper.SetUIGray(contentPane.goldBtn.transform, isNotHaveGoldDeal);
        contentPane.goldBtn.enabled = !isNotHaveGoldDeal;

        GUIHelper.SetUIGray(contentPane.gemBtn.transform, isNotHaveGemDeal);
        contentPane.gemBtn.enabled = !isNotHaveGemDeal;


        contentPane.goldPriceText.text = isNotHaveGoldDeal ? "-" : item.goldPrice.ToString("N0");
        contentPane.goldPriceText.color = isBuy ? (isNotHaveGoldDeal ? Color.white : UserDataProxy.inst.playerData.gold < _item.goldPrice ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white) : Color.white;
        contentPane.gemPriceText.text = isNotHaveGemDeal ? "-" : item.gemPrice.ToString("N0");
        //contentPane.gemPriceText.color = isBuy ? (isNotHaveGoldDeal ? Color.white : UserDataProxy.inst.playerData.gem < _item.gemPrice ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white) : Color.white;


        if (_item.itemType == 0)//装备
        {
            //contentPane.itemIcon.SetSprite(_item.equipConfig.equipDrawingsConfig.atlas, _item.equipConfig.equipDrawingsConfig.icon);
            contentPane.itemIcon.SetSpriteURL(_item.equipConfig.equipDrawingsConfig.big_icon);
            contentPane.equipQuality.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[_item.itemQuality - 1]);
            GUIHelper.showQualiyIcon(contentPane.equipQuality.GetComponent<RectTransform>(), _item.itemQuality, 300);
            contentPane.itemNameText.text = item.equipConfig.name;
            contentPane.levelText.text = item.equipConfig.equipDrawingsConfig.level.ToString();

            contentPane.inventoryNumText.text = ItemBagProxy.inst.getEquipNumber(_item.itemId, _item.itemQuality).ToString("N0");

            //EquipItem equipItem = ItemBagProxy.inst.GetEquipItem(_item.itemId, _item.itemQuality);

            contentPane.goldLowPriceText.text = _item.equipConfig.equipQualityConfig.auction_price.ToString("N0"); /*equipItem == null ? _item.equipConfig.equipQualityConfig.price_gold.ToString("N0") : equipItem.sellPrice.ToString("N0")*/;

            contentPane.subTypeIcon.gameObject.SetActiveTrue();
            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(item.equipConfig.equipDrawingsConfig.sub_type);
            contentPane.subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

            contentPane.obj_superEquipSign.SetActive(_item.itemQuality > StaticConstants.SuperEquipBaseQuality);
            contentPane.itemBgIcon.SetSprite("__common_1", _item.itemQuality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        }
        else//资源 
        {

            //contentPane.itemIcon.SetSprite(_item.itemConfig.atlas, _item.itemConfig.icon);
            contentPane.itemIcon.SetSpriteURL(_item.itemConfig.icon);
            contentPane.equipQuality.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[0]);
            GUIHelper.showQualiyIcon(contentPane.equipQuality.GetComponent<RectTransform>(), 0, 300);
            contentPane.itemNameText.text = LanguageManager.inst.GetValueByKey(item.itemConfig.name);
            contentPane.levelText.text = "1";

            contentPane.inventoryNumText.text = ItemBagProxy.inst.GetItem(_item.itemId).count.ToString("N0");

            contentPane.goldLowPriceText.text = _item.itemConfig.low_price_m.ToString("N0");
            contentPane.subTypeIcon.gameObject.SetActiveFalse();

            contentPane.obj_superEquipSign.SetActive(false);
            contentPane.itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");
        }


        //TODO 判断是否为公会满足 

    }


    //按钮置灰并无法点击
    private void setBtnRefreshAni(bool isInDealing) //是否为交易中
    {
        contentPane.goldCanClickObj.SetActive(!isInDealing);
        contentPane.goldProcessingObj.SetActive(isInDealing);
        contentPane.goldBtn.enabled = !isInDealing;
        contentPane.gemCanClickObj.SetActive(!isInDealing);
        contentPane.gemProcessingObj.SetActive(isInDealing);
        contentPane.gemBtn.enabled = !isInDealing;

        //this.isInDealing = isInDealing;
        //TODO ani
    }

}
