using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfGridComp : MonoBehaviour
{
    [Header("-按钮组件-")]
    //普通货架的点击事件
    public Button equipBtn;
    public Button infoBtn;

    [Header("-显隐-")]
    public Image img_lock;
    public Image img_type_content;
    public Image img_Equip_Bg;
    public Image img_add;

    [Header("-防具货架小类型的图片-")]
    public GUIIcon[] typeIconGUI;

    [Header("-格子上的可替换图片组件-")]
    public GUIIcon equipIcon;
    public GUIIcon qualityIcon;

    //可展示装备类型id数组
    public int[] equipIdArray;

    //是否显示小类型
    public bool isShowTypes;

    //当前Item的物品信息
    public ShelfEquip equipItem;

    public int field;//对应位置

    public GameObject obj_superEquipSign;

    int _shelfUid;


    private void Awake()
    {
        infoBtn.onClick.AddListener(() =>
        {
            if (equipItem != null)
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, equipItem.equipUid, 0, new List<EquipItem>());
        }
            );

        equipBtn.onClick.AddListener(() =>
        {
            if (img_lock.gameObject.activeSelf) return;

            if (equipItem != null)
            {
                //卸下装备
                EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.SHELFUPGRADE_TAKEDOWNEQUIP, field, _shelfUid, equipItem.equipUid);
            }
            else
            {
                //打开物品栏
                EventController.inst.TriggerEvent(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SHOW_INVENTORYUI, equipIdArray, _shelfUid);
            }
        }
            );

    }

    public void SetTypes(int[] equipIdArray)
    {
        this.equipIdArray = equipIdArray;
    }


    public void SetData(ShelfEquip item, int shelfUid)
    {

        _shelfUid = shelfUid;

        gameObject.SetActive(true);
        img_lock.gameObject.SetActive(false);

        if (item == null)
        {
            Clear();
            return;
        }

        this.equipItem = item;

        img_Equip_Bg.gameObject.SetActive(true);
        img_add.gameObject.SetActive(false);
        img_type_content.gameObject.SetActive(false);

        EquipItem equip = ItemBagProxy.inst.GetEquipItem(item.equipId);

        if (equip == null)
        {
            Logger.error("未找到该装备  id = " + item.equipId);
            return;
        }

        obj_superEquipSign.SetActive(equip.quality > StaticConstants.SuperEquipBaseQuality);

        if (equip.quality == 1 || equip.quality == StaticConstants.SuperEquipBaseQuality + 1)
            qualityIcon.gameObject.SetActive(false);
        else
        {
            qualityIcon.gameObject.SetActive(true);
            qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[equip.quality - 1]);
            GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), equip.quality);
        }
        equipIcon.SetSprite(equip.equipConfig.equipDrawingsConfig.atlas, equip.equipConfig.equipDrawingsConfig.icon);
    }

    public void Lock()
    {
        gameObject.SetActive(true);
        img_lock.gameObject.SetActive(true);
        img_add.gameObject.SetActive(false);
        img_Equip_Bg.gameObject.SetActive(false);
        img_type_content.gameObject.SetActive(false);
    }

    public void Clear()
    {
        img_Equip_Bg.gameObject.SetActive(false);
        equipItem = null;
        ShowTypes();
    }

    public void ShowTypes()
    {
        img_type_content.gameObject.SetActive(isShowTypes);
        img_add.gameObject.SetActive(!isShowTypes);

        if (isShowTypes)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i < equipIdArray.Length)
                {
                    typeIconGUI[i].gameObject.SetActive(true);

                    EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equipIdArray[i]);
                    typeIconGUI[i].SetSprite(classcfg.Atlas, classcfg.icon);
                }
                else
                {
                    typeIconGUI[i].gameObject.SetActive(false);
                }
            }
        }
    }

}
