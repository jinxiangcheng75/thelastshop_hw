using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoothItemInfoUI : ViewBase<BoothItemInfoUIComp>
{

    public override string viewID => ViewPrefabName.BoothItemInfoUI;
    public override string sortingLayerName => "window";

    private BoothItem _item;
    private LoopEventcomp timerComp;

    protected override void onInit()
    {
        base.onInit();

        base.isShowResPanel = true;


        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.infoBtn.onClick.AddListener(() =>
        {
            if (_item != null)
            {
                var cfg = EquipConfigManager.inst.GetEquipQualityConfig(_item.itemId, _item.itemQuality);
                if (_item.itemType == 0) EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, "", cfg.id, new List<EquipItem>());
                else EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, _item.itemId);
            }
        });
        contentPane.cancelBtn.ButtonClickTween(onCancelBtnClick);
        contentPane.reSubmitBtn.ButtonClickTween(onReSubmitBtnClick);
        contentPane.leftBtn.ButtonClickTween(() => onTurnPageBtnClick(true));
        contentPane.rightBtn.ButtonClickTween(() => onTurnPageBtnClick(false));

    }


    public void SetData(BoothItem boothItem, int boothListCount)
    {
        _item = boothItem;
        contentPane.leftBtn.gameObject.SetActive(!(boothListCount == 1));
        contentPane.rightBtn.gameObject.SetActive(!(boothListCount == 1));

        bool isGold = _item.moneyType == 0;

        contentPane.unitMoneyIcon.SetSprite("__common_1", isGold ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");
        contentPane.lowestMoneyIcon.SetSprite("__common_1", isGold ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");

        if (_item.itemType == 0)
        {
            EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(_item.itemId, _item.itemQuality);
            //contentPane.itemIcon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon);
            contentPane.itemIcon.SetSpriteURL(equipConfig.equipDrawingsConfig.big_icon);
            contentPane.qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[_item.itemQuality - 1]);
            GUIHelper.showQualiyIcon(contentPane.qualityIcon.GetComponent<RectTransform>(), _item.itemQuality, 300);
            contentPane.nameTx.text = equipConfig.name;
            contentPane.nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1]);
            contentPane.levelTx.text = equipConfig.equipDrawingsConfig.level.ToString();
            contentPane.priceTx.text = _item.unitPrice.ToString("N0");
            contentPane.inventoryNumTx.text = ItemBagProxy.inst.getEquipNumber(_item.itemId, _item.itemQuality).ToString();

            contentPane.subTypeIcon.gameObject.SetActive(true);

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equipConfig.equipDrawingsConfig.sub_type);
            contentPane.subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);
            contentPane.obj_superEquipSign.SetActive(_item.itemQuality > StaticConstants.SuperEquipBaseQuality);
            contentPane.itemBgIcon.SetSprite("__common_1", _item.itemQuality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        }
        else
        {
            itemConfig itemConfig = ItemconfigManager.inst.GetConfig(_item.itemId);
            //contentPane.itemIcon.SetSprite(itemConfig.atlas, itemConfig.icon);
            contentPane.itemIcon.SetSpriteURL(itemConfig.icon);
            contentPane.qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[0]);
            GUIHelper.showQualiyIcon(contentPane.qualityIcon.GetComponent<RectTransform>(), 0, 300);
            contentPane.nameTx.text = LanguageManager.inst.GetValueByKey(itemConfig.name);
            contentPane.nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);
            contentPane.levelTx.text = 1.ToString();
            contentPane.priceTx.text = _item.unitPrice.ToString("N0");
            contentPane.inventoryNumTx.text = ItemBagProxy.inst.GetItem(itemConfig.id).count.ToString();

            contentPane.subTypeIcon.gameObject.SetActive(false);
            contentPane.obj_superEquipSign.SetActive(false);
            contentPane.itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");
        }


        contentPane.arrowIcon.SetSprite("market_atlas", _item.marketType == 0 ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
        contentPane.arrowIcon.transform.localScale = new Vector3(_item.marketType == 0 ? -1 : 1, 1, 1);

        contentPane.typeTx.text = LanguageManager.inst.GetValueByKey(_item.marketType == 0 ? "报价" : "请求");
        contentPane.typeTx.color = _item.marketType == 0 ? GUIHelper.GetColorByColorHex("#78f452") : GUIHelper.GetColorByColorHex("#37e7fd");
        contentPane.continueTimeTx.text = StaticConstants.time_times[_item.timeIndex] + LanguageManager.inst.GetValueByKey("小时");

        contentPane.topBgIcon.SetSprite("market_atlas_newAdd", _item.marketType == 0 ? "shichang_banner2" : "shichang_banner1");

        contentPane.marketNumTx.text = "";
        contentPane.lowestPriceTx.text = "";

        contentPane.numTx.text = _item.remainNum.ToString();


        contentPane.reSubmitBtn.gameObject.SetActive((int)_item.remainTime <= 0);
        contentPane.countDownObj.SetActive((int)_item.remainTime > 0);
        contentPane.countDownTx.text = TimeUtils.timeSpanStrip((int)_item.remainTime);


        if (timerComp != null)
        {
            GameTimer.inst.removeLoopTimer(timerComp);
            timerComp = null;
        }

        timerComp = GameTimer.inst.AddLoopTimerComp(contentPane.countDownTx.gameObject, 1, timerCountDown, (int)_item.remainTime);

        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_MARKETITEM_REFRESH, new Request_marketItemData() { buyOrSell = boothItem.marketType, itemType = boothItem.itemType, itemId = boothItem.itemId, itemQuality = boothItem.itemQuality });
    }

    //刷新回调
    public void timingRefreshCallBackMethod(MarketItem marketItem)
    {
        contentPane.marketNumTx.text = marketItem.marketNum > 100 ? "100+" : marketItem.marketNum.ToString();
        contentPane.lowestPriceTx.text = _item.moneyType == 0 ? marketItem.goldPrice.ToString("N0") : marketItem.gemPrice.ToString("N0");
    }


    private void onTurnPageBtnClick(bool isLeft)
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.BOOTHITEMINFOUI_TURNPAGE, isLeft, _item.boothField);
    }

    private void onCancelBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_SOLDOUTBOOTHITEMAFFIRMUI, _item);
    }

    private void onReSubmitBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_BOOTHITEMRESUBMIT, _item.boothField);
    }

    private void timerCountDown()
    {
        if (contentPane.reSubmitBtn.gameObject.activeSelf) return; //已经是 过期状态

        contentPane.countDownTx.text = TimeUtils.timeSpanStrip((int)_item.remainTime);

        if (_item.remainTime <= 0)
        {
            contentPane.countDownTx.text = TimeUtils.timeSpanStrip(1);
        }
    }

    //过期情况 是否需要刷新
    public void NeedRefresh(BoothItem item, int boothListCount)
    {
        if (item.boothField == _item.boothField && !contentPane.reSubmitBtn.gameObject.activeSelf) SetData(item, boothListCount);
    }


    public void NeedHide(BoothItem item)
    {
        if (item.boothField == _item.boothField) hide();
    }

    protected override void onHide()
    {
        if (timerComp != null)
        {
            GameTimer.inst.removeLoopTimer(timerComp);
            timerComp = null;
        }
    }

}
