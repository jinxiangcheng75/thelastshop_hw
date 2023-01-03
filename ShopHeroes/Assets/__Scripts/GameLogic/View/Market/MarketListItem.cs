using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketListItem : MonoBehaviour
{

    public Text lvText;
    public Text nameText;
    public Text numText;
    public Text costText;
    public GUIIcon itemBgIcon;
    public GUIIcon goldIcon;
    public GUIIcon itemIcon;
    public GUIIcon qualityIcon;
    public List<RoleEquipItem> allProperty;
    public GameObject allPropertyObj;
    public Text abrasionText;
    public GameObject abrasionObj;

    public GameObject obj_superEquipSign;

    private MarketItem _item;

    private void Start()
    {
        var self = gameObject.GetComponent<Button>();
        self.onClick.AddListener(() =>
        {
            if (_item != null)
            {
                //查看详细物品市场信息
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETITEMINFOUI, _item, kMarketTradingHallType.selfBuy);
            }
        });
    }

    void setEquipPropertyInfo(int equipId) 
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(equipId);
        for (int i = 0; i < allProperty.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                allProperty[index].gameObject.SetActive(true);
                allProperty[index].valText.text = propertyList[index].ToString();
            }
            else
            {
                allProperty[index].gameObject.SetActive(false);
            }
        }
    }

    void setEqiupInfo(int marketInventoryFromType) 
    {
        lvText.text = _item.equipConfig.equipDrawingsConfig.level.ToString();
        nameText.text = _item.equipConfig.name;

        itemIcon.SetSprite(_item.equipConfig.equipDrawingsConfig.atlas, _item.equipConfig.equipDrawingsConfig.icon);
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[_item.itemQuality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), _item.itemQuality);

        obj_superEquipSign.SetActive(_item.itemQuality > StaticConstants.SuperEquipBaseQuality);
        itemBgIcon.SetSprite("__common_1", _item.itemQuality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        if (marketInventoryFromType == (int)MarketInventoryFromType.byShelf)
        {
            abrasionObj.SetActive(false);
            allPropertyObj.SetActive(false);
        }
        else if (marketInventoryFromType == (int)MarketInventoryFromType.byHeroWearEquip) 
        {
            abrasionObj.SetActive(true);
            allPropertyObj.SetActive(true);

            abrasionText.text = RoleDataProxy.inst.GetEquipDamagerVal(_item.equipConfig.equipQualityConfig.quality) / 100.0f + "%";
            setEquipPropertyInfo(_item.equipConfig.equipQualityConfig.id);
        }

    }

    void setMatInfo() 
    {
        abrasionObj.SetActive(false);
        allPropertyObj.SetActive(false);
        obj_superEquipSign.SetActive(false);

        itemBgIcon.SetSprite("__common_1","cktb_wupinkuang");

        lvText.text = 1.ToString();
        nameText.text = LanguageManager.inst.GetValueByKey(_item.itemConfig.name);

        itemIcon.SetSprite(_item.itemConfig.atlas, _item.itemConfig.icon);
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[0]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), 0);
    }

    public void setData(MarketItem marketItem, int marketInventoryFromType)
    {
        _item = marketItem;

        if (_item.itemType == 0)//装备
        {
            setEqiupInfo(marketInventoryFromType);
        }
        else//资源 
        {
            setMatInfo();
        }

        numText.text = _item.marketNum > 100 ? "100+" : _item.marketNum.ToString();
        bool isGem = _item.goldPrice <= 0;
        costText.text = isGem ? _item.gemPrice.ToString("N0") : _item.goldPrice.ToString("N0");
        string iconName = isGem ? "zhuejiemian_jinkuai" : "zhuejiemian_meiyuan";
        Vector2 size = isGem ? Vector2.one * 70f : Vector2.one * 56f;
        goldIcon.iconImage.rectTransform.sizeDelta = size;
        goldIcon.SetSprite("__common_1", iconName);
    }

}
