using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Item
{
    public int ID;
    public double count;           //数量

    public itemConfig itemConfig;

    public Item(int id, double _numble, itemConfig cfg)
    {
        ID = id;
        count = _numble;
        if (cfg.type != (int)ItemType.Box)
        {
            count = Mathf.Max((int)_numble, 0);
        }
        itemConfig = cfg;
    }
}

public class EquipItem
{
    public string itemUid = "";
    public int equipid = 0;
    public int ID;
    public double count;           //数量
    public double getTime = 0;
    public bool isLock = false;
    public EquipConfig equipConfig;
    public int onShelfCount = 0;
    public int quality = 1;
    //public int fightingSum { get { return equipConfig == null ? 0 : equipConfig.fightingSum; } }
    public int sellPrice
    {
        get
        {
            var subcfg = EquipConfigManager.inst.GetEquipQualityConfig(equipid);
            var equipData = EquipDataProxy.inst.GetEquipData(ID);
            int basePrice = subcfg.price_gold;

            int addPrice = 0;

            //里程碑
            float val = (equipData == null ? 0 : equipData.sellAddition - 1);
            addPrice = Mathf.RoundToInt(basePrice * val);

            //家具buff---

            //小类型
            val = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.sell_subTypePriceUp, equipConfig.equipDrawingsConfig.sub_type);
            addPrice += Mathf.CeilToInt(basePrice * val);

            //全部类型
            val = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.sell_allPriceUp);
            addPrice += Mathf.CeilToInt(basePrice * val);

            //---

            int price = basePrice + addPrice;


            //全服buff
            var sellBuff = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.sell_priceUp);
            if (sellBuff != null)
            {
                val = sellBuff.buffInfo.buffParam / 100f;
                price += Mathf.CeilToInt(price * val);
            }

            //豪华度加成
            if (equipData != null)
            {
                int luxuryBuff = HotfixBridge.inst.GetLuxuryBuff(equipData.mainType);
                if (luxuryBuff != -1)
                {
                    val = luxuryBuff / 100f;
                    price += Mathf.CeilToInt(price * val);
                }
            }

            return price;
        }
    }
    public EquipItem(string uid, int _equipid, double _numble, double _gettime, EquipConfig equipCfg)
    {
        itemUid = uid;
        equipid = _equipid;
        ID = equipCfg.equipDrawingId;
        count = _numble;
        getTime = _gettime;
        equipConfig = equipCfg;
        quality = equipConfig.equipQualityConfig.quality;
    }

    public float GetEquipFightingSum(HeroTalentDataBase talentCfg)
    {
        float fightingSum = 0;

        if(equipConfig != null)
        {
            fightingSum = equipConfig.GetFightingSum(talentCfg);
        }

        return fightingSum;
    }
}

/// <summary>
/// 处理 资源仓库数据+装备仓库数据
/// </summary>
/// 
public class ItemBagProxy : TSingletonHotfix<ItemBagProxy>, IDataModelProx
{
    public Dictionary<int, Item> resItems = new Dictionary<int, Item>();      //所有消耗资源列表 （货币、生产资源、人物资源） key  为 ID

    public Dictionary<string, EquipItem> equipItems = new Dictionary<string, EquipItem>();    //所有装备列表 key = userId

    public Dictionary<int, List<Item>> resItemsBytype = new Dictionary<int, List<Item>>();

    public long bagCountLimit { get { return UserDataProxy.inst.playerData.bagLimit; } }

    public void Init()
    {
        resItems.Clear();
        itemConfig[] cfgs = ItemconfigManager.inst.GetAllConfig();
        foreach (itemConfig cfg in cfgs)
        {
            var item = new Item(cfg.id, -1, cfg);
            if (!resItemsBytype.ContainsKey(cfg.type) || resItemsBytype[cfg.type] == null)
            {
                resItemsBytype[cfg.type] = new List<Item>();
            }
            resItemsBytype[cfg.type].Add(item);
            resItems.Add(cfg.id, item);
        }
    }

    //清理数据(再次调用前先Init)
    public void Clear()
    {
        resItems.Clear();
        equipItems.Clear();
        resItemsBytype.Clear();
    }

    public double resItemCount(int resId)
    {
        if (resId == 10009)
        {
            return HotfixBridge.inst.GetActivity_WorkerGameCoinCount();
        }

        return resItems[resId].count;
    }

    #region 装备背包
    //
    //获取已有的装备列表
    public List<EquipItem> GetAllEquipItem()
    {
        return equipItems.Values.ToList();
    }

    //获取货架上的装备
    public List<EquipItem> GetOnShelfEquipItems()
    {
        return GetAllEquipItem().FindAll(item => item.onShelfCount > 0);
    }

    //获取不同资源

    //根据类型获取仓库中装备列表
    public List<EquipItem> GetEquipItemsByType(EquipSubType type)
    {
        return GetAllEquipItem().FindAll(item =>
        {
            return item.equipConfig.equipDrawingsConfig.sub_type == (int)type;
        });
    }

    public EquipItem GetEquipItem(string uid)
    {
        EquipItem item;
        equipItems.TryGetValue(uid, out item);
        return item;
    }
    public EquipItem GetEquipItem(int equipid)
    {
        return GetAllEquipItem().Find(i => i.equipid == equipid);
    }

    public EquipItem GetEquipItem(int equipdrawingid, int quality)
    {
        return GetAllEquipItem().Find(i => i.equipConfig.equipDrawingId == equipdrawingid && i.equipConfig.equipQualityConfig.quality == quality);
    }

    //inbox=true 只取箱子里面的装备 ，筛选条件 types 装备小类型 数组
    public List<EquipItem> GetEquipItemsByType(bool inbox, int[] types)
    {
        List<EquipItem> list = new List<EquipItem>();
        foreach (var equip in equipItems.Values)
        {
            for (int i = 0; i < types.Length; i++)
            {
                // list
                if (equip.equipConfig.equipDrawingsConfig.sub_type == types[i])
                {
                    if (inbox)
                    {
                        if (equip.count > equip.onShelfCount)
                        {
                            list.Add(equip);
                        }
                    }
                    else
                    {
                        list.Add(equip);
                    }
                    break;
                }
            }
        }
        return list;
    }

    public List<EquipItem> GetEquipItemsByTypeAndMaxLevel(bool inbox, int[] types, int maxLevel)
    {
        List<EquipItem> list = new List<EquipItem>();
        foreach (var equip in equipItems.Values)
        {
            for (int i = 0; i < types.Length; i++)
            {
                // list
                if (equip.equipConfig.equipDrawingsConfig.sub_type == types[i] && equip.equipConfig.equipDrawingsConfig.level <= maxLevel)
                {
                    if (inbox)
                    {
                        if (equip.count > equip.onShelfCount)
                        {
                            list.Add(equip);
                        }
                    }
                    else
                    {
                        list.Add(equip);
                    }
                    break;
                }
            }
        }
        return list;
    }

    public List<EquipItem> GetEquipItemsByHero(int[] sub_types, int maxLevel)
    {
        List<EquipItem> equips = GetEquipItemsByTypeAndMaxLevel(false, sub_types, maxLevel);

        return equips;
    }

    //根据装备小类型及装备等阶获取可穿戴该装备的 所有相关英雄职业 及 最低佩戴等级
    public KeyValuePair<string, string>[] GetCanWearHeroInfosByEquip(int equip_subType, int equip_lv, out int canWearFloorLv)
    {
        //所有相关英雄职业
        List<HeroProfessionConfigData> canWearHeroProfessionCfgs = HeroProfessionConfigManager.inst.GetCanWearHeroProfessionsByEquipSubType(equip_subType);

        Dictionary<int, bool> typeHeroGradeLvOneCanWearDic = new Dictionary<int, bool>();

        HashSet<KeyValuePair<string, string>> set = new HashSet<KeyValuePair<string, string>>();
        foreach (var item in canWearHeroProfessionCfgs)
        {
            if (item.hero_grade == 1) //星级为1的
            {
                typeHeroGradeLvOneCanWearDic[item.type] = true;
                set.Add(new KeyValuePair<string, string>(item.atlas, item.ocp_icon));
            }
            else
            {
                if (!typeHeroGradeLvOneCanWearDic.ContainsKey(item.type) || !typeHeroGradeLvOneCanWearDic[item.type])
                {
                    set.Add(new KeyValuePair<string, string>(item.atlas, item.ocp_icon));
                }
            }
        }
        var arr = set.ToArray();

        //最低佩戴等级
        canWearFloorLv = heroupgradeconfigManager.inst.GetCanWearFloorLvByEquipLv(equip_lv);

        return arr;
    }

    public int GetEquipInventory()
    {
        return (int)GetAllEquipItem().Sum(item => item.count);
    }
    //所有品质的同一件商品
    public int getEquipAllNumber(int equipdrawingid)
    {
        int num = (int)GetAllEquipItem().FindAll(i => i.ID == equipdrawingid).Sum(equip => equip.count);
        return num;
    }

    private int getEquipNumberBySuperQuipRange(List<EquipItem> allEquipItems, int quality, int minxQuality, int maxQuality)
    {
        int num = 0;

        num = (int)allEquipItems.FindAll(t => t.quality >= minxQuality && t.quality <= maxQuality && t.quality >= quality).Sum(equip => equip.count);

        return num;
    }

    //与它同品质或更高品质的
    public int getEquipNumberBySuperQuip(int equipid)
    {
        int num = 0;
        var econfig = EquipConfigManager.inst.GetEquipInfoConfig(equipid);

        //先找到所有该drawingID的equipItems
        var allEquipItem = GetAllEquipItem().FindAll(i => i.ID == econfig.equipDrawingId);
        //先找普通的
        num += getEquipNumberBySuperQuipRange(allEquipItem, econfig.equipQualityConfig.quality, 1, 5);
        //再找超凡的
        num += getEquipNumberBySuperQuipRange(allEquipItem, econfig.equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality ? econfig.equipQualityConfig.quality : econfig.equipQualityConfig.quality + StaticConstants.SuperEquipBaseQuality, StaticConstants.SuperEquipBaseQuality + 1, StaticConstants.SuperEquipBaseQuality + 5);

        return num;
    }
    //取得装备库存数量
    public int getEquipNumber(int equipid)
    {
        int num = (int)GetAllEquipItem().FindAll(i => i.equipid == equipid).Sum(equip => equip.count);
        return num;
    }

    public int getEquipNumber(int equipid, int quality)
    {
        int num = (int)GetAllEquipItem().FindAll(i => i.equipConfig.equipDrawingId == equipid && i.equipConfig.equipQualityConfig.quality == quality).Sum(equip => equip.count);
        return num;
    }
    public int getEquipNumber(string equipuid)
    {
        if (equipItems.ContainsKey(equipuid))
        {
            return (int)equipItems[equipuid].count;
        }
        else
        {
            return 0;
        }
    }
    //设置背包装备数量

    public void updateEquipNum(string _uid, int _id, double count, double gettime, bool islock, int onShelfCount)
    {
        if (equipItems.ContainsKey(_uid))
        {
            if (count == 0)
            {
                equipItems.Remove(_uid);
            }
            else
            {
                var item = equipItems[_uid];
                item.count = count;
                item.getTime = gettime;
                item.isLock = islock;
                item.onShelfCount = onShelfCount;
                equipItems[_uid] = item;
            }
        }
        else
        {
            var cfg = EquipConfigManager.inst.GetEquipInfoConfig(_id);
            if (count > 0 && cfg != null)
            {
                var item = new EquipItem(_uid, _id, count, gettime, cfg);
                item.isLock = islock;
                item.onShelfCount = onShelfCount;
                equipItems.Add(_uid, item);
            }
        }
    }
    #endregion
    //设置资源数量
    public void updateItemNum(int itemId, long Count)
    {
        Item item;
        if (resItems.TryGetValue(itemId, out item))
        {
            if (item.count == Count) return;
            if (item.count == 0 && Count > 0)
            {
                HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 9, itemId);
            }
            item.count = Count;
            if (item.count < 0)
            {
                item.count = 0;
            }

            if ((ItemType)item.itemConfig.type == ItemType.Material) //资源 
            {
                EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.ResBoxDisPlay_ReShow);
            }

            if ((ItemType)item.itemConfig.type == ItemType.Box || (ItemType)item.itemConfig.type == ItemType.Key)
            {
                TreasureBoxDataProxy.inst.AddBoxData(item);
            }
        }
        else
        {
            Logger.error("玩家数据未初始化！！");
        }
    }
    public Item GetItem(int itemId)
    {
        if (resItems.ContainsKey(itemId))
        {
            return resItems[itemId];
        }
        return null;
    }

    public List<Item> GetItemsByTypes(ItemType[] subtypes, bool needAll)
    {
        List<Item> result = new List<Item>();

        for (int i = 0; i < subtypes.Length; i++)
        {
            int index = i;
            if (subtypes[index] == ItemType.Box)
            {
                var tempList = GetItemsByType(subtypes[index]);
                if (tempList != null)
                {
                    foreach (var item in tempList)
                    {
                        if (item.count != -1)
                        {
                            result.Add(item);
                        }
                        else if (GetItem(item.ID + StaticConstants.tboxAndKeyOffset).count > 0)
                        {
                            updateItemNum(item.ID, 0);
                            result.Add(item);
                        }
                    }
                }
            }
            else if (subtypes[index] == ItemType.Activity_WorkerGameCoin)
            {
                long workerGameCoinCount = HotfixBridge.inst.GetActivity_WorkerGameCoinCount();
                if (workerGameCoinCount > 0)
                {
                    result.Add(new Item(10009, workerGameCoinCount, ItemconfigManager.inst.GetConfig(10009)));
                }

            }
            else
            {
                var items = GetItemsByType(subtypes[index]);
                if (items != null)
                {
                    result.AddRange(needAll ? items : items.FindAll(t => t.count > 0));
                }
            }
        }

        result.Sort((a, b) =>
        {
            if (a.itemConfig.type == (int)ItemType.HeroCard)
            {
                return -1;
            }
            else if (b.itemConfig.type == (int)ItemType.HeroCard)
            {
                return 1;
            }
            else
            {
                return a.itemConfig.type.CompareTo(b.itemConfig.type);
            }
        });

        return result;
    }

    public List<Item> GetItemsByTypes(int[] subtypes, bool needAll)
    {
        List<Item> result = new List<Item>();

        for (int i = 0; i < subtypes.Length; i++)
        {
            int index = i;
            if (subtypes[index] == (int)ItemType.Box)
            {
                var tempList = GetItemsByType(subtypes[index]);
                if (tempList != null)
                {
                    foreach (var item in tempList)
                    {
                        if (item.count != -1)
                        {
                            result.Add(item);
                        }
                        else if (GetItem(item.ID + StaticConstants.tboxAndKeyOffset).count > 0)
                        {
                            updateItemNum(item.ID, 0);
                            result.Add(item);
                        }
                    }
                }
            }
            else if (subtypes[index] == (int)ItemType.Activity_WorkerGameCoin)
            {
                long workerGameCoinCount = HotfixBridge.inst.GetActivity_WorkerGameCoinCount();
                if (workerGameCoinCount > 0)
                {
                    result.Add(new Item(10009, workerGameCoinCount, ItemconfigManager.inst.GetConfig(10009)));
                }

            }
            else
            {
                var items = GetItemsByType(subtypes[index]);
                if (items != null)
                {
                    result.AddRange(needAll ? items : items.FindAll(t => t.count > 0));
                }
            }
        }

        result.Sort((a, b) =>
        {
            if (a.itemConfig.type == (int)ItemType.HeroCard)
            {
                return -1;
            }
            else if (b.itemConfig.type == (int)ItemType.HeroCard)
            {
                return 1;
            }
            else
            {
                return a.itemConfig.type.CompareTo(b.itemConfig.type);
            }
        });

        return result;
    }

    public List<Item> GetItemsByType(ItemType type)
    {
        List<Item> items;
        if (resItemsBytype.TryGetValue((int)type, out items))
        {
            return items;
        }
        Logger.error("未找到该类型的items :" + type.ToString()  + "    itemType : " + (int)type);
        return null;
    }

    public List<Item> GetItemsByType(int type)
    {
        List<Item> items;
        if (resItemsBytype.TryGetValue(type, out items))
        {
            return items;
        }
        Logger.error("未找到该类型的items :" + type.ToString() + "    itemType : " + (int)type);
        return null;
    }

    public Item GetItemsByType(ItemType type,int effectType)
    {
        var list = GetItemsByType(type);
        if (list != null)
        {
            return list.Find(t => t.itemConfig.effect == effectType);
        }
        return null;
    }

    public bool CheckRes(int id, int count)
    {
        Item item = GetItem(id);
        if (item != null)
        {
            return item.count >= count;
        }
        return false;
    }

    public string GetItemTypeStr(ItemType type)
    {
        string result = "";

        switch (type)
        {
            case ItemType.Glod:
                break;
            case ItemType.Gem:
                break;
            case ItemType.Energy:
                break;
            case ItemType.Blueprint:
                result = "装备图纸解锁";
                break;
            case ItemType.Material:
                result = "制作基本材料";
                break;
            case ItemType.TaskMaterial:
                result = "装备制作零件";
                break;
            case ItemType.Bag:
                break;
            case ItemType.Box:
                result = "探索宝箱";
                break;
            case ItemType.Key:
                result = "宝箱开启道具";
                break;
            case ItemType.FusionStone:
                break;
            case ItemType.Activity:
                break;
            case ItemType.WarriorCoin:
                break;
            case ItemType.Hero:
                break;
            case ItemType.Craftsman:
                result = "工人";
                break;
            case ItemType.SpecialEquipment:
                break;
            case ItemType.EquipmentDrawing:
                result = "装备图纸";
                break;
            case ItemType.Turntable:
                result = "转盘开启道具";
                break;
            case ItemType.HeroExp:
                result = "英雄经验卡";
                break;
            case ItemType.HeroTransfer:
                result = "英雄转职进阶道具";
                break;
            case ItemType.ExploreTimeItem:
                result = "探索加速道具";
                break;
            case ItemType.ExploreAddYieldItem:
                result = "探索战利品道具";
                break;
            case ItemType.ExploreAttBonus:
                result = "探索强化战力道具";
                break;
            case ItemType.RecoverHeroItem:
                result = "英雄恢复道具";
                break;
            case ItemType.ExploreExpBonusItem:
                result = "探索强化经验道具";
                break;
            case ItemType.RepairEquipmentItem:
                result = "修复损坏装备道具";
                break;
            case ItemType.Equip:
                break;
            case ItemType.Active:
                break;
            case ItemType.Gift:
                break;
            case ItemType.UnionCoin_self:
            case ItemType.UnionCoin_union:
            case ItemType.UnionRenown:
                result = "联盟道具";
                break;
            case ItemType.Furniture:
                result = "获得家具道具";
                break;
            case ItemType.ShopkeeperDress:
                result = "获得店主装饰道具";
                break;
            case ItemType.HeroCard:
                result = "获得指定英雄道具";
                break;
            case ItemType.MakeSlotNum:
                break;
            case ItemType.ExploreSlotNum:
                break;
            case ItemType.ShopSizeNum:
                break;
            case ItemType.HeroSlotNum:
                break;
            case ItemType.MarketSlotNum:
                break;
            default:
                break;
        }


        return LanguageManager.inst.GetValueByKey(result);
    }

    #region 排序
    //背包装备列表排序
    public static void EquipItemSort(ref List<EquipItem> list, SortType type)
    {
        if (list.Count <= 1) return;
        switch (type)
        {
            case SortType.Number:
                list.Sort(CompareByCount);
                break;
            case SortType.Price:
                list.Sort(CompareByPrice);
                break;
            case SortType.Level:
                list.Sort(CompareByLevel);
                break;
            case SortType.Quality:
                list.Sort(CompareByQuality);
                break;
            case SortType.Time:
                list.Sort((equip1, equip2) => -equip1.getTime.CompareTo(equip2.getTime));
                break;
            case SortType.SubType:
                list.Sort(CompareByType);
                break;
        }
    }

    //价值
    public static int CompareByPrice(EquipItem c1, EquipItem c2)
    {
        if (c1.sellPrice == c2.sellPrice)
        {
            if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
            {
                if (c1.sellPrice == c2.sellPrice)
                {
                    return 0;
                }
                else
                {
                    return c1.sellPrice < c2.sellPrice ? 1 : -1;
                }
            }
            else
            {
                return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
            }
        }
        return c1.sellPrice < c2.sellPrice ? 1 : -1;
    }

    //类型
    //a) 规则：
    //i.从前向后：利刃，钝器，弓弩，轻型武器，步枪，重型武器，狙击枪，霰弹枪，轻型护甲，重型护甲，便衣，头盔，帽子，轻装裤，重装裤，靴子，鞋子，药品，工具，暗器，投掷类，首饰，盾牌
    //ii.相同类型：从前到后，价值从大到小
    public static int CompareByType(EquipItem c1, EquipItem c2)
    {
        if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
        {
            if (c1.sellPrice == c2.sellPrice)
            {
                return 0;
            }
            else
            {
                return c1.sellPrice < c2.sellPrice ? 1 : -1;
            }
        }
        return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
    }

    //数量
    //a) 规则：
    //i.从前到后，数量由多到少
    //ii.相同的数量，按照类型
    //iii. 相同类型：从前到后，价值从高到低
    public static int CompareByCount(EquipItem c1, EquipItem c2)
    {
        if (c1.count == c2.count)
        {
            if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
            {
                if (c1.sellPrice == c2.sellPrice)
                {
                    return 0;
                }
                else
                {
                    return c1.sellPrice < c2.sellPrice ? 1 : -1;
                }
            }
            else
            {
                return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
            }
        }
        return c1.count < c2.count ? 1 : -1;
    }
    //    5. 阶级
    //a) 规则
    //i.从前到后，等级由高到低
    //ii. 相同等级，按照装备类型
    //iii. 相同类型：从前到后，价值从高到低

    public static int CompareByLevel(EquipItem c1, EquipItem c2)
    {
        if (c1.equipConfig.equipDrawingsConfig.level == c2.equipConfig.equipDrawingsConfig.level)
        {
            if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
            {
                if (c1.sellPrice == c2.sellPrice)
                {
                    return 0;
                }
                else
                {
                    return c1.sellPrice < c2.sellPrice ? 1 : -1;
                }
            }
            else
            {
                return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
            }
        }
        return c1.equipConfig.equipDrawingsConfig.level < c2.equipConfig.equipDrawingsConfig.level ? 1 : -1;
    }
    //6. 稀有度
    //a) 从前到后，品质从高到低
    //b) 相同品质，从前到后，按照装备等级
    //c) 相同等级，按照装备类型
    public static int CompareByQuality(EquipItem c1, EquipItem c2)
    {
        if (c1.quality == c2.quality)
        {
            if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
            {
                if (c1.sellPrice == c2.sellPrice)
                {
                    return 0;
                }
                else
                {
                    return c1.sellPrice < c2.sellPrice ? 1 : -1;
                }
            }
            else
            {
                return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
            }
        }
        return c1.quality < c2.quality ? 1 : -1;
    }
    #endregion
}