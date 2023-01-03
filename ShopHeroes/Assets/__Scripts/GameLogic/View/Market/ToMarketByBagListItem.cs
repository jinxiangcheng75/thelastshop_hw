using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToMarketByBagListItem : MonoBehaviour
{
    public GameObject lvObj;
    public GameObject numObj;
    public Text lvTx;
    public Text sumTx;
    public Text nameTx;
    public Text goldTx;
    public GUIIcon itemBgIcon;
    public GUIIcon itemIcon;
    public GUIIcon qualityIcon;

    public GameObject obj_superEquipSign;

    private int itemType, itemID, itemQuality; //0 装备 1 资源
    private bool isSuper;

    public Action<int, int, int, bool> onClickHandler;

    private void Awake()
    {
        var button = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        button.ButtonClickTween(onButtonClick);
    }

    public void SetData(EquipItem data)
    {
        lvObj.SetActive(true);
        numObj.SetActive(true);
        qualityIcon.gameObject.SetActive(true);
        obj_superEquipSign.SetActive(data.quality > StaticConstants.SuperEquipBaseQuality);
        itemBgIcon.SetSprite("__common_1", data.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        itemType = 0;
        itemID = data.ID;
        itemQuality = data.quality;
        isSuper = data.equipConfig.equipQualityConfig.extraordinary_equip_id <= 0;

        lvTx.text = data.equipConfig.equipDrawingsConfig.level.ToString();
        sumTx.text = "x" + data.count;
        goldTx.text = data.equipConfig.equipQualityConfig.auction_price.ToString("N0");
        nameTx.text = data.equipConfig.name;
        nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[data.quality - 1]);


        itemIcon.SetSprite(data.equipConfig.equipDrawingsConfig.atlas, data.equipConfig.equipDrawingsConfig.icon);
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[data.quality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), data.quality);
    }

    public void SetData(EquipDrawingsConfig data)
    {
        lvObj.SetActive(true);
        qualityIcon.gameObject.SetActive(true);
        obj_superEquipSign.SetActive(false);
        itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");

        itemType = 0;
        itemID = data.id;
        itemQuality = 1;

        lvTx.text = data.level.ToString();

        //EquipItem equipItem = ItemBagProxy.inst.GetEquipItem(data.id, 1);

        int num = ItemBagProxy.inst.getEquipNumber(itemID, itemQuality);
        numObj.SetActive(num != 0);
        sumTx.text = "x" + num;

        EquipQualityConfig equipQualityConfig = EquipConfigManager.inst.GetEquipQualityConfig(data.id, 1);
        isSuper = equipQualityConfig.extraordinary_equip_id <= 0;

        goldTx.text = equipQualityConfig.auction_price.ToString("N0"); //equipItem == null ? equipQualityConfig.price_gold.ToString("N0") : equipItem.sellPrice.ToString("N0");
        nameTx.text = LanguageManager.inst.GetValueByKey(data.name);
        nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[itemQuality - 1]);



        itemIcon.SetSprite(data.atlas, data.icon);
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[itemQuality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), itemQuality);
    }

    public void SetData(Item data)
    {
        numObj.SetActive(true);
        lvObj.SetActive(false);
        qualityIcon.gameObject.SetActive(false);
        obj_superEquipSign.SetActive(false);
        itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");


        itemType = 1;
        itemID = data.ID;
        itemQuality = 1;
        isSuper = false;

        numObj.SetActive(data.count != 0);
        sumTx.text = "x" + data.count;
        nameTx.text = LanguageManager.inst.GetValueByKey(data.itemConfig.name);
        nameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);
        goldTx.text = data.itemConfig.low_price_m.ToString("N0");


        itemIcon.SetSprite(data.itemConfig.atlas, data.itemConfig.icon);

    }


    private void onButtonClick()
    {
        onClickHandler?.Invoke(itemType, itemID, itemQuality, isSuper);
    }

}
