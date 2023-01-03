
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureUpgradeFinishComp : MonoBehaviour
{
    public Button confirmBtn;

    public GUIIcon nextLevelImg;

    public Text finishUpgradeText;

    public GameObject itemContent;

    public GameObject[] itemsArray;

    public UpgradeFinishItemComp[] itemComps;
}

public class FurnitureUpgradeFinishView : ViewBase<FurnitureUpgradeFinishComp>
{
    public override string viewID => ViewPrefabName.FurnitureUpgradeFinishPanel;
    public override string sortingLayerName => "window";

    IndoorData.ShopDesignItem item = null;

    public List<ItemInfo> itemInfoList;
    List<string> resBinImgList;

    protected override void onInit()
    {
        itemInfoList = new List<ItemInfo>();
        resBinImgList = new List<string>();

        contentPane.confirmBtn.ButtonClickTween(() =>
        {
            AudioManager.inst.PlaySound(62);
            hide();
        });
    }
    public FurnitureConfig oldConfig;


    public void SetData(IndoorData.ShopDesignItem item)
    {
        this.item = item;

        for (int i = 0; i < contentPane.itemsArray.Length; i++)
        {
            contentPane.itemsArray[i].gameObject.SetActiveFalse();
        }

        contentPane.nextLevelImg.SetSpriteURL(item.config.icon_big);
        contentPane.finishUpgradeText.text = LanguageManager.inst.GetValueByKey("{0}升级到了等级{1}！", LanguageManager.inst.GetValueByKey(item.config.name.Substring(2)), item.level.ToString());
        oldConfig = null;
        switch (item.config.type_1)
        {
            //柜台
            case (int)kTileGroupType.Counter:
                {
                    ShowCounterUpgradeDatas();
                    break;
                }

            //资源篮
            case (int)kTileGroupType.ResourceBin:
                {
                    ShowResourceUpgradeDatas();
                    break;
                }

            //货架
            case (int)kTileGroupType.Shelf:
                {
                    ShowShelfUpgradeDatas();
                    break;
                }

            //储物箱
            case (int)kTileGroupType.Trunk:
                {
                    ShowStorageUpgradeDatas();
                    break;
                }

            default:
                {
                    Debug.LogError("未能找到对应类别");
                    break;
                }
        }
        if (oldConfig != null)
        {
            //判断升级前后占地大小是否有变化
            if (oldConfig.width * oldConfig.height < item.config.width * item.config.height)
            {
                itemInfoList.Add(new ItemInfo()
                {
                    isShow = true,
                    itemName = "大小",
                    atlas = StaticConstants.funitureItemAtlasName,
                    icon = StaticConstants.funitureItemIcons[15],
                    oldValue = $"{oldConfig.height}x{oldConfig.width}",
                    newValue = $"{item.config.height}x{item.config.width}"
                });
            }
        }

        var c = contentPane;

        for (int i = 0; i < itemInfoList.Count; i++)
        {
            c.itemComps[i].gameObject.SetActiveTrue();
            //c.itemComps[i].itemIconImg.sprite = ManagerBinder.inst.Asset.getSprite("" + itemInfoList[i].atlas, itemInfoList[i].icon);
            //c.itemComps[i].itemIconImg.SetNativeSize();
            //c.itemComps[i].itemIconImg.SetSprite(itemInfoList[i].atlas, itemInfoList[i].icon, needSetNativeSize: true);

            c.itemComps[i].itemIconImg.SetSprite(itemInfoList[i].atlas, itemInfoList[i].icon);
            // ManagerBinder.inst.Asset.getSpriteAsync(itemInfoList[i].atlas, itemInfoList[i].icon, (sprite) =>
            //  {
            //      if (sprite.rect.size == Vector2.one * 256)
            //      {
            //          c.itemComps[i].itemIconImg.iconImage.rectTransform.sizeDelta = Vector2.one * 128;
            //      }
            //      else
            //      {
            //          c.itemComps[i].itemIconImg.iconImage.rectTransform.sizeDelta = sprite.rect.size;
            //      }

            //  });
            c.itemComps[i].itemName.text = LanguageManager.inst.GetValueByKey(itemInfoList[i].itemName);
            c.itemComps[i].oldValue.text = itemInfoList[i].oldValue;
            c.itemComps[i].newValue.text = itemInfoList[i].newValue;
        }
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(25);
    }

    protected override void onHide()
    {
        for (int i = 0; i < contentPane.itemsArray.Length; i++)
        {
            contentPane.itemsArray[i].gameObject.SetActiveFalse();
        }

        AudioManager.inst.PlaySound(11);
        itemInfoList.Clear();
    }

    //升级完成时展示柜台的升级数据
    private void ShowCounterUpgradeDatas()
    {
        var lastCounterConfig = FurnitureUpgradeConfigManager.inst.GetCounterUpgradeConfig(item.level - 1);
        if (lastCounterConfig == null) return;
        var nextCounterConfig = FurnitureUpgradeConfigManager.inst.GetCounterUpgradeConfig(item.level);
        //能量有变化
        if (lastCounterConfig.energy < nextCounterConfig.energy)
        {
            itemInfoList.Add(new ItemInfo()
            {
                isShow = true,
                itemName = "售卖获得的能量",
                atlas = StaticConstants.funitureItemAtlasName,
                icon = StaticConstants.funitureItemIcons[1],
                oldValue = $"{lastCounterConfig.energy}",
                newValue = $"{nextCounterConfig.energy}"
            });
        }
        oldConfig = FurnitureConfigManager.inst.getConfig(lastCounterConfig.furniture_id);
    }

    //升级完成时展示储存箱的升级数据
    private void ShowStorageUpgradeDatas()
    {
        var lastTrunkConfig = FurnitureUpgradeConfigManager.inst.GetTrunkUpgradeConfig(item.level - 1);
        if (lastTrunkConfig == null) return;
        var nextTrunkConfig = FurnitureUpgradeConfigManager.inst.GetTrunkUpgradeConfig(item.level);
        //物品栏大小
        if (lastTrunkConfig.space < nextTrunkConfig.space)
        {
            itemInfoList.Add(new ItemInfo()
            {
                isShow = true,
                itemName = "物品栏大小",
                atlas = StaticConstants.funitureItemAtlasName,
                icon = StaticConstants.funitureItemIcons[3],
                oldValue = $"{lastTrunkConfig.space}",
                newValue = $"{nextTrunkConfig.space}"
            });
        }

        //堆叠上限
        if (lastTrunkConfig.pile_space < nextTrunkConfig.pile_space)
        {
            itemInfoList.Add(new ItemInfo()
            {
                isShow = true,
                itemName = "堆叠上限",
                atlas = StaticConstants.funitureItemAtlasName,
                icon = StaticConstants.funitureItemIcons[4],
                oldValue = $"{lastTrunkConfig.pile_space}",
                newValue = $"{nextTrunkConfig.pile_space}"
            });
        }
        oldConfig = FurnitureConfigManager.inst.getConfig(lastTrunkConfig.furniture_id);
    }

    //升级完成时展示货架的升级数据
    private void ShowShelfUpgradeDatas()
    {
        var lastShelfConfig = FurnitureUpgradeConfigManager.inst.GetShelfUpgradeConfig(item.config.type_2, item.level - 1);
        if (lastShelfConfig == null) return;
        var nextShelfConfig = FurnitureUpgradeConfigManager.inst.GetShelfUpgradeConfig(item.config.type_2, item.level);
        //能量上限
        if (lastShelfConfig.energy < nextShelfConfig.energy)
        {
            itemInfoList.Add(new ItemInfo()
            {
                isShow = true,
                itemName = "能量上限",
                atlas = StaticConstants.funitureItemAtlasName,
                icon = StaticConstants.funitureItemIcons[1],
                oldValue = $"{lastShelfConfig.energy}",
                newValue = $"{nextShelfConfig.energy}"
            });
        }

        //容量
        if (lastShelfConfig.store < nextShelfConfig.store)
        {
            itemInfoList.Add(new ItemInfo()
            {
                isShow = true,
                itemName = "容量",
                atlas = StaticConstants.funitureItemAtlasName,
                icon = StaticConstants.funitureItemIcons[2],
                oldValue = $"{lastShelfConfig.store}",
                newValue = $"{nextShelfConfig.store}"
            });
        }

        oldConfig = FurnitureConfigManager.inst.getConfig(lastShelfConfig.furniture_id);
    }

    //升级完成时资源篮的升级数据
    private void ShowResourceUpgradeDatas()
    {
        var lastResBinConfig = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(item.config.type_2, item.level - 1);
        if (lastResBinConfig == null) return;
        var nextResBinConfig = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(item.config.type_2, item.level);

        //资源容量
        if (lastResBinConfig.store < nextResBinConfig.store)
        {
            itemInfoList.Add(new ItemInfo()
            {
                isShow = true,
                itemName = "容量",
                atlas = StaticConstants.funitureItemAtlasName,
                icon = StaticConstants.funitureItemIcons[GetResBinAtlasAndIcon()],
                oldValue = $"{lastResBinConfig.store}",
                newValue = $"{nextResBinConfig.store}"
            });

            resBinImgList.Clear();
        }
        oldConfig = FurnitureConfigManager.inst.getConfig(lastResBinConfig.furniture_id);
    }

    private int GetResBinAtlasAndIcon()
    {
        var nextResBinConfig = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(item.config.type_2, item.level);
        FurnitureConfig cfg = FurnitureConfigManager.inst.getConfig(nextResBinConfig.furniture_id);
        return cfg.iconitem_id[0];
    }

    public string GetItemIconAtlas(FurnitureConfig config, int iconSerial)
    {
        return FurnitureItemiconConfigManager.inst.getConfig(config.iconitem_id[iconSerial]).atlas;
    }
}