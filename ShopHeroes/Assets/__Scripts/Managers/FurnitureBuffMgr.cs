using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FurnitureBuffType
{

    none,
    sell_subTypePriceUp = 1,//提升小分类售价
    sell_allPriceUp = 2,//提升所有装备的出售价格

    make_subTypeSpeedUp = 3,//提升小分类制作速度
    make_allSpeedUp = 4,//提升所有制作速度
    make_subTypeQualityUp = 5,//提升小分类制作高品质概率
    make_allQualityUp = 6,//提升所有制作高品质概率

    res_allSpeedUp = 7,//提升所有资源的生产速度
    res_subTypeSpeedUp = 8,//提升小分类资源的生产速度

    hero_qualityUp = 9,//提升**品质及以上的英雄刷新概率

    equip_damageRaceDown = 10, //减少所有装备损坏概率

}



public class FurnitureBuffMgr : TSingletonHotfix<FurnitureBuffMgr>
{

    protected override void init()
    {

    }

    //提升装备小分类售价
    float getSubtypeEquipSellPriceUp(EquipSubType subType) 
    {
        float val = 0;

        var indoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.Furniture).FindAll((t) => t.state != (int)EDesignState.InStore);
        var outdoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.OutdoorFurniture).FindAll((t) => t.state != (int)EDesignState.InStore);

        List<IndoorData.ShopDesignItem> furnitures = new List<IndoorData.ShopDesignItem>();
        furnitures.AddRange(indoorFurnitures);
        furnitures.AddRange(outdoorFurnitures);


        for (int i = 0; i < furnitures.Count; i++)
        {
            var item = furnitures[i];

            if (item.config.buff_ids != null && item.config.buff_ids.Length > 0)
            {
                for (int k = 0; k < item.config.buff_ids.Length; k++)
                {
                    int furnitureBuffId = item.config.buff_ids[k];
                    FurnitureBuffConfigData buffCfg = FurnitureBuffConfigManager.inst.GetConfig(furnitureBuffId);

                    if (buffCfg != null && buffCfg.type == (int)FurnitureBuffType.sell_subTypePriceUp && buffCfg.parameter_1 == (int)subType)
                    {
                        val += buffCfg.parameter_2;//param 2
                    }
                }
            }
        }

        return val;
    }

    //提升所有装备的出售价格
    float getAllEquipSellPriceUp() 
    {
        float val = 0;

        var indoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.Furniture).FindAll((t) => t.state != (int)EDesignState.InStore);
        var outdoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.OutdoorFurniture).FindAll((t) => t.state != (int)EDesignState.InStore);

        List<IndoorData.ShopDesignItem> furnitures = new List<IndoorData.ShopDesignItem>();
        furnitures.AddRange(indoorFurnitures);
        furnitures.AddRange(outdoorFurnitures);


        for (int i = 0; i < furnitures.Count; i++)
        {
            var item = furnitures[i];

            if (item.config.buff_ids != null && item.config.buff_ids.Length > 0)
            {
                for (int k = 0; k < item.config.buff_ids.Length; k++)
                {
                    int furnitureBuffId = item.config.buff_ids[k];
                    FurnitureBuffConfigData buffCfg = FurnitureBuffConfigManager.inst.GetConfig(furnitureBuffId);

                    if (buffCfg != null && buffCfg.type == (int)FurnitureBuffType.sell_allPriceUp)
                    {
                        val += buffCfg.parameter_1;//param 1
                    }
                }
            }
        }

        return val;
    }

    //提升装备小分类制作速度
    float getSubtypeEquipMakeSpeedUp(EquipSubType subType) 
    {
        float val = 0;

        var indoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.Furniture).FindAll((t) => t.state != (int)EDesignState.InStore);
        var outdoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.OutdoorFurniture).FindAll((t) => t.state != (int)EDesignState.InStore);

        List<IndoorData.ShopDesignItem> furnitures = new List<IndoorData.ShopDesignItem>();
        furnitures.AddRange(indoorFurnitures);
        furnitures.AddRange(outdoorFurnitures);


        for (int i = 0; i < furnitures.Count; i++)
        {
            var item = furnitures[i];

            if (item.config.buff_ids != null && item.config.buff_ids.Length > 0)
            {
                for (int k = 0; k < item.config.buff_ids.Length; k++)
                {
                    int furnitureBuffId = item.config.buff_ids[k];
                    FurnitureBuffConfigData buffCfg = FurnitureBuffConfigManager.inst.GetConfig(furnitureBuffId);

                    if (buffCfg != null && buffCfg.type == (int)FurnitureBuffType.make_subTypeSpeedUp && buffCfg.parameter_1 == (int)subType)
                    {
                        val += buffCfg.parameter_2;//param 2
                    }
                }
            }
        }

        return val;
    }

    //提升所有装备制作速度
    float getAllEquipMakeSpeedUp() 
    {
        float val = 0;

        var indoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.Furniture).FindAll((t) => t.state != (int)EDesignState.InStore);
        var outdoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.OutdoorFurniture).FindAll((t) => t.state != (int)EDesignState.InStore);

        List<IndoorData.ShopDesignItem> furnitures = new List<IndoorData.ShopDesignItem>();
        furnitures.AddRange(indoorFurnitures);
        furnitures.AddRange(outdoorFurnitures);


        for (int i = 0; i < furnitures.Count; i++)
        {
            var item = furnitures[i];

            if (item.config.buff_ids != null && item.config.buff_ids.Length > 0)
            {
                for (int k = 0; k < item.config.buff_ids.Length; k++)
                {
                    int furnitureBuffId = item.config.buff_ids[k];
                    FurnitureBuffConfigData buffCfg = FurnitureBuffConfigManager.inst.GetConfig(furnitureBuffId);

                    if (buffCfg != null && buffCfg.type == (int)FurnitureBuffType.make_allSpeedUp)
                    {
                        val += buffCfg.parameter_1;//param 1
                    }
                }
            }
        }

        return val;
    }

    //减少所有装备损坏概率
    float getEquipDamageRaceDown()
    {
        float val = 0;

        var indoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.Furniture).FindAll((t) => t.state != (int)EDesignState.InStore);
        var outdoorFurnitures = UserDataProxy.inst.GetEntitys(kTileGroupType.OutdoorFurniture).FindAll((t) => t.state != (int)EDesignState.InStore);

        List<IndoorData.ShopDesignItem> furnitures = new List<IndoorData.ShopDesignItem>();
        furnitures.AddRange(indoorFurnitures);
        furnitures.AddRange(outdoorFurnitures);


        for (int i = 0; i < furnitures.Count; i++)
        {
            var item = furnitures[i];

            if (item.config.buff_ids != null && item.config.buff_ids.Length > 0) 
            {
                for (int k = 0; k < item.config.buff_ids.Length; k++)
                {
                    int furnitureBuffId = item.config.buff_ids[k];
                    FurnitureBuffConfigData buffCfg = FurnitureBuffConfigManager.inst.GetConfig(furnitureBuffId);

                    if (buffCfg!= null && buffCfg.type == (int)FurnitureBuffType.equip_damageRaceDown)
                    {
                        val += buffCfg.parameter_1;//param 1
                    }
                }
            }
        }

        return val;
    }

    public float GetBuffValByType(FurnitureBuffType buffType, int subType = 0)
    {
        float val = 0;

        switch (buffType)
        {
            case FurnitureBuffType.none:
                break;
            case FurnitureBuffType.sell_subTypePriceUp:
                val = getSubtypeEquipSellPriceUp((EquipSubType)subType);
                break;
            case FurnitureBuffType.sell_allPriceUp:
                val = getAllEquipSellPriceUp();
                break;
            case FurnitureBuffType.make_subTypeSpeedUp:
                val = getSubtypeEquipMakeSpeedUp((EquipSubType)subType);
                break;
            case FurnitureBuffType.make_allSpeedUp:
                val = getAllEquipMakeSpeedUp();
                break;
            case FurnitureBuffType.make_subTypeQualityUp:
                break;
            case FurnitureBuffType.make_allQualityUp:
                break;
            case FurnitureBuffType.res_allSpeedUp:
                break;
            case FurnitureBuffType.res_subTypeSpeedUp:
                break;
            case FurnitureBuffType.hero_qualityUp:
                break;
            case FurnitureBuffType.equip_damageRaceDown:
                val = getEquipDamageRaceDown();
                break;
            default:
                break;
        }

        return val <= 0 ? 0 : val / 100f;
    }


}
