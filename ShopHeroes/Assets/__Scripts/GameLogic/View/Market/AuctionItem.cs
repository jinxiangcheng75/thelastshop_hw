using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuctionItem : MonoBehaviour
{

    public Text lvText;
    public Text marketNumText;
    public Text inventoryNumText;
    public Text goldNumText;
    public Text gemNumText;
    public Text offetTip;
    public GameObject goldAndGemObj;
    public GameObject grayMask;

    public GUIIcon itemBgIcon;
    public GUIIcon itemIcon;
    public GUIIcon qualityIcon;
    public GUIIcon typeIcon;

    public GameObject obj_superEquipSign;

    public Action<MarketItem> onClickHandler;

    private MarketItem _item;

    private void Start()
    {
        Button selfBtn = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        if (selfBtn != null)
        {
            selfBtn.ButtonClickTween(() =>
            {
                if (_item != null) onClickHandler?.Invoke(_item);
            });
        }
    }

    public void SetData(MarketItem item, kMarketTradingHallType hallType)
    {
        _item = item;

        //目前没有公会
        offetTip.enabled = false;
        goldAndGemObj.SetActive(true);

        int inventoryNum = 0;

        if (item.itemType == 0) //装备
        {
            EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(_item.itemId, _item.itemQuality);
            lvText.text = equipConfig.equipDrawingsConfig.level.ToString();
            itemIcon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon);
            qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[_item.itemQuality - 1]);
            GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), _item.itemQuality);
            inventoryNum = ItemBagProxy.inst.getEquipNumber(_item.itemId, _item.itemQuality);

            obj_superEquipSign.SetActive(_item.itemQuality > StaticConstants.SuperEquipBaseQuality);
            itemBgIcon.SetSprite("__common_1", _item.itemQuality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        }
        else
        {
            lvText.text = "1";
            itemConfig itemConfig = ItemconfigManager.inst.GetConfig(_item.itemId);
            itemIcon.SetSprite(itemConfig.atlas, itemConfig.icon);
            qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[0]);
            GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), 0);
            inventoryNum = (int)ItemBagProxy.inst.GetItem(_item.itemId).count;

            obj_superEquipSign.SetActive(false);
            itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");

        }

        marketNumText.text = item.marketNum > 100 ? "100+" : item.marketNum.ToString();
        goldNumText.text = AbbreviationUtility.AbbreviateNumber(item.goldPrice, 2);
        gemNumText.text = AbbreviationUtility.AbbreviateNumber(item.gemPrice, 2);

        inventoryNum = Mathf.Max(inventoryNum, 0);
        inventoryNumText.text = inventoryNum.ToString();

        grayMask.SetActive(false);

        typeIcon.SetSprite("market_atlas", hallType == kMarketTradingHallType.selfBuy ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
        var scale = typeIcon.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (hallType == kMarketTradingHallType.selfBuy ? -1 : 1);
        typeIcon.transform.localScale = scale;

        if (hallType == kMarketTradingHallType.selfBuy)
        {
            grayMask.SetActive(false);
        }
        else
        {
            grayMask.SetActive(inventoryNum == 0);//未拥有该物品，加层遮罩提示
        }
    }


}
