using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//摊位Item
public class BoothItem
{
    public int boothField;//摊位下标
    public int itemType;//资源还是装备  0 装备 1 资源
    public int itemId;//对应物品id
    public int itemQuality;//对应物品质量
    public int remainNum;//剩余数量
    public int marketType;//商品类型 0 报价  1 请求  
    public int moneyType;//钱还是钻  0 金币  1 钻石
    public int unitPrice;//物品单价
    public int exchangeNum;//交易数量  报价为钱  请求为收到数量
    public int timeIndex;//时间下标 0 - 12小时 1 - 24小时 2 - 48小时
    public double remainTime //剩余时间
    {
        get { return endTime - GameTimer.inst.serverNow; }
    }
    public double endTime;

    public bool redPoint;

    public BoothItem() { }

    public void SetInfo(int _boothField, int _itemType, int _itemId, int _itemQuality, int _remainNum, int _marketType, int _moneyType, int _unitPrice, int _exchangeNum, int _timeIndex, double _remainTime)
    {
        this.boothField = _boothField;
        this.itemType = _itemType;
        this.itemId = _itemId;
        this.itemQuality = _itemQuality;
        this.remainNum = _remainNum;
        this.marketType = _marketType;
        this.moneyType = _moneyType;
        this.unitPrice = _unitPrice;
        this.exchangeNum = _exchangeNum;
        this.timeIndex = _timeIndex;
        this.endTime = GameTimer.inst.serverNow + _remainTime;

        refreshRedPoint();

        if (remainTime > 0)
        {
            setTimer();
        }
    }

    void refreshRedPoint()
    {
        if (MarketDataProxy.inst.redPointShow)
        {
            this.redPoint = exchangeNum > 0 || remainTime <= 0;
        }
    }

    int timerId = 0;

    void setTimer()
    {
        ClearTimer();
        timerId = GameTimer.inst.AddTimer((int)remainTime, 1, countdown, GameTimerType.byServerTime);
    }

    void countdown()
    {
        if (remainTime > 0)
        {
            timerId = GameTimer.inst.AddTimer((int)remainTime, 1, countdown, GameTimerType.byServerTime);
        }
        else
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETBOOTH_REQUEST_DATA);
        }
    }

    public void ClearTimer() 
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

}

//拍卖Item
public class MarketItem
{
    public double putawayTime;//上架时间
    public int itemType;//资源还是装备  0 装备 1 资源
    public int itemId;//对应物品id
    public int itemQuality;//对应物品品质
    public int marketNum;//交易所数量
    public int goldPrice;//金币价格
    public int gemPrice;//钻石价格

    public EquipConfig equipConfig;
    public itemConfig itemConfig;

    public MarketItem() { }

    public void SetInfo(double _putawayTime, int _type, int _itemId, int _quality, int _marketNum, int _goldPrice, int _gemPrice)
    {
        this.putawayTime = _putawayTime;
        this.itemType = _type;
        this.itemId = _itemId;
        this.itemQuality = _quality;
        this.marketNum = _marketNum;
        this.goldPrice = _goldPrice;
        this.gemPrice = _gemPrice;


        if (_type == 0) equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(this.itemId, this.itemQuality);
        else itemConfig = ItemconfigManager.inst.GetConfig(this.itemId);
    }

}


//交易所 
public class MarketDataProxy : TSingletonHotfix<MarketDataProxy>
{

    public bool redPointShow;

    public string Payload 
    {
        get;
        set;
    }

    public int GetSubmitEquipLimit(kMarketItemType type)
    {

        int limit = (int)WorldParConfigManager.inst.GetConfig(152).parameters;

        UnionScienceData data = UserDataProxy.inst.getUnionScienceDataByType(type == kMarketItemType.selfBuy ? (int)EUnionScienceType.MarketBuyLimit : (int)EUnionScienceType.MarketSoldLimit);

        if (data != null && data.serverData.level != 0)
        {
            limit += data.config.add_num;
        }

        return limit;

    }

    public int GetSubmittMaterialLimit(kMarketItemType type)
    {
        int limit = (int)WorldParConfigManager.inst.GetConfig(153).parameters;

        UnionScienceData data = UserDataProxy.inst.getUnionScienceDataByType(type == kMarketItemType.selfBuy ? (int)EUnionScienceType.MarketBuyLimit : (int)EUnionScienceType.MarketSoldLimit);

        if (data != null && data.serverData.level != 0)
        {
            limit += data.config.add_num;
        }

        return limit;
    }

    #region 排序相关

    public static string GetSortText(EmarketItemSortType sortType)
    {
        string str = "";

        switch (sortType)
        {
            case EmarketItemSortType.LevelDescending: str = "物品阶数-"; break;
            case EmarketItemSortType.LevelAscending: str = "物品阶数+"; break;
            case EmarketItemSortType.ValueDescending: str = "物品价值-"; break;
            case EmarketItemSortType.ValueAscending: str = "物品价值+"; break;
            case EmarketItemSortType.Quality: str = "稀有度"; break;
            case EmarketItemSortType.Near: str = "最近"; break;
            case EmarketItemSortType.Type: str = "类型"; break;
            case EmarketItemSortType.Num: str = "数量"; break;
            case EmarketItemSortType.max: break;
        }

        return LanguageManager.inst.GetValueByKey("排序方式：") + LanguageManager.inst.GetValueByKey(str);
    }


    //本地 持有装备排序
    public static void MyHoldingEquipListSort(ref List<EquipItem> list, EmarketItemSortType sortType)
    {
        switch (sortType)
        {
            case EmarketItemSortType.LevelDescending:
                list.Sort((c1, c2) => CompareByLevel(c1, c2, false));
                break;
            case EmarketItemSortType.LevelAscending:
                list.Sort((c1, c2) => CompareByLevel(c1, c2, true));
                break;
            case EmarketItemSortType.ValueDescending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, false));
                break;
            case EmarketItemSortType.ValueAscending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, true));
                break;
            case EmarketItemSortType.Quality:
                list.Sort((c1, c2) => CompareByQualiy(c1, c2));
                break;
            case EmarketItemSortType.Near:
                list.Sort((c1, c2) => -c1.getTime.CompareTo(c2.getTime));
                break;
            case EmarketItemSortType.Type:
                list.Sort((c1, c2) => CompareByType(c1, c2));
                break;
            case EmarketItemSortType.Num:
                list.Sort((c1, c2) => CompareByNum(c1, c2));
                break;
            case EmarketItemSortType.max:
                break;
        }

    }

    //本地 装备配置表排序
    public static void EquipConfigListSort(ref List<EquipDrawingsConfig> list, EmarketItemSortType sortType)
    {
        switch (sortType)
        {
            case EmarketItemSortType.LevelDescending:
                list.Sort((c1, c2) => CompareByLevel(c1, c2, false));
                break;
            case EmarketItemSortType.LevelAscending:
                list.Sort((c1, c2) => CompareByLevel(c1, c2, true));
                break;
            case EmarketItemSortType.ValueDescending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, false));
                break;
            case EmarketItemSortType.ValueAscending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, true));
                break;
            case EmarketItemSortType.Type:
                list.Sort((c1, c2) => CompareByType(c1, c2));
                break;
            case EmarketItemSortType.Num:
                list.Sort((c1, c2) => CompareByNum(c1, c2));
                break;
            case EmarketItemSortType.max:
                break;
        }

    }

    //本地 资源排序
    public static void ItemListSort(ref List<Item> list, EmarketItemSortType sortType)
    {
        switch (sortType)
        {
            case EmarketItemSortType.ValueDescending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, false));
                break;
            case EmarketItemSortType.ValueAscending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, true));
                break;
            case EmarketItemSortType.Num:
                list.Sort((c1, c2) => c1.count.CompareTo(c2.count) * -1);
                break;
            case EmarketItemSortType.max:
                break;
        }

    }

    //服务器 市场物品排序
    public static void MarketItemListSort(ref List<MarketItem> list, EmarketItemSortType sortType)
    {
        switch (sortType)
        {
            case EmarketItemSortType.LevelDescending:
                list.Sort((c1, c2) => CompareByLevel(c1, c2, false));
                break;
            case EmarketItemSortType.LevelAscending:
                list.Sort((c1, c2) => CompareByLevel(c1, c2, true));
                break;
            case EmarketItemSortType.ValueDescending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, false));
                break;
            case EmarketItemSortType.ValueAscending:
                list.Sort((c1, c2) => CompareByValue(c1, c2, true));
                break;
            case EmarketItemSortType.Quality:
                list.Sort((c1, c2) => CompareByQualiy(c1, c2));
                break;
            case EmarketItemSortType.Near:
                list.Sort((c1, c2) => -c1.putawayTime.CompareTo(c2.putawayTime));
                break;
            case EmarketItemSortType.Type:
                list.Sort((c1, c2) => CompareByType(c1, c2));
                break;
            case EmarketItemSortType.Num:
                list.Sort((c1, c2) => CompareByNum(c1, c2));
                break;
            case EmarketItemSortType.max:
                break;
        }
    }

    /*
    1.物品阶数+/-
    i. 物品阶数从高到底
    ii. 物品品质从高到低
    iii. 物品价值从高到低
    */

    //已有装备 等阶升降序
    public static int CompareByLevel(EquipItem c1, EquipItem c2, bool isAscending)
    {
        int baseNumber = isAscending ? -1 : 1;

        if (c1.equipConfig.equipDrawingsConfig.level == c2.equipConfig.equipDrawingsConfig.level)
        {
            if (c1.quality == c2.quality)
            {
                return c1.sellPrice.CompareTo(c2.sellPrice) * -1;
            }
            return c1.quality < c2.quality ? 1 : -1;
        }

        return (c1.equipConfig.equipDrawingsConfig.level < c2.equipConfig.equipDrawingsConfig.level ? 1 : -1) * baseNumber;
    }

    //装备配置表 等阶升降序
    public static int CompareByLevel(EquipDrawingsConfig c1, EquipDrawingsConfig c2, bool isAscending)
    {
        int baseNumber = isAscending ? -1 : 1;

        if (c1.level == c2.level)
        {
            return EquipConfigManager.inst.GetEquipQualityConfig(c1.id, 1).price_gold.CompareTo(EquipConfigManager.inst.GetEquipQualityConfig(c2.id, 1).price_gold) * -1;
        }

        return (c1.level < c2.level ? 1 : -1) * baseNumber;
    }

    //市场物品 等阶升降序
    public static int CompareByLevel(MarketItem c1, MarketItem c2, bool isAscending)
    {
        int baseNumber = isAscending ? -1 : 1;

        if (c1.equipConfig.equipDrawingsConfig.level == c2.equipConfig.equipDrawingsConfig.level)
        {
            if (c1.itemQuality == c2.itemQuality)
            {
                return c1.goldPrice.CompareTo(c2.goldPrice) * -1;
            }
            return c1.itemQuality < c2.itemQuality ? 1 : -1;
        }

        return (c1.equipConfig.equipDrawingsConfig.level < c2.equipConfig.equipDrawingsConfig.level ? 1 : -1) * baseNumber;
    }




    /*
     2. 物品价值+/-
     i. 物品价值从高到低
     ii. 物品阶数从高到底
     iii. 物品品质从高到低
    */

    //已有装备 价值升降序 （为默认排序规则）
    public static int CompareByValue(EquipItem c1, EquipItem c2, bool isAscending)
    {
        int baseNumber = isAscending ? -1 : 1;

        if (c1.sellPrice == c2.sellPrice)
        {
            if (c1.equipConfig.equipDrawingsConfig.level == c2.equipConfig.equipDrawingsConfig.level)
            {
                return c1.quality.CompareTo(c2.quality) * -1;
            }
            return c1.equipConfig.equipDrawingsConfig.level < c2.equipConfig.equipDrawingsConfig.level ? 1 : -1;
        }

        return (c1.sellPrice < c2.sellPrice ? 1 : -1) * baseNumber;
    }

    //装备配置表 价值升降序
    public static int CompareByValue(EquipDrawingsConfig c1, EquipDrawingsConfig c2, bool isAscending)
    {
        int baseNumber = isAscending ? -1 : 1;

        int c1Val = EquipConfigManager.inst.GetEquipQualityConfig(c1.id, 1).price_gold;
        int c2Val = EquipConfigManager.inst.GetEquipQualityConfig(c2.id, 1).price_gold;

        if (c1Val == c2Val)
        {
            return c1.level.CompareTo(c2.level) * -1;
        }

        return (c1Val < c2Val ? 1 : -1) * baseNumber;
    }

    //本地 资源 价格升降序
    public static int CompareByValue(Item c1, Item c2, bool isAscending)
    {
        int baseNumber = isAscending ? 1 : -1;

        return c1.itemConfig.low_price_m.CompareTo(c2.itemConfig.low_price_m) * baseNumber;
    }

    //市场物品 价值升降序 （为默认排序规则）
    public static int CompareByValue(MarketItem c1, MarketItem c2, bool isAscending)
    {
        int baseNumber = isAscending ? -1 : 1;

        if (c1.goldPrice == c2.goldPrice)
        {
            int c1Lv = c1.itemType == 0 ? c1.equipConfig.equipDrawingsConfig.level : 1;
            int c2Lv = c2.itemType == 0 ? c2.equipConfig.equipDrawingsConfig.level : 1;

            if (c1Lv == c2Lv)
            {
                return c1.itemQuality.CompareTo(c2.itemQuality) * -1;
            }
            return c1Lv < c2Lv ? 1 : -1;
        }

        return (c1.goldPrice < c2.goldPrice ? 1 : -1) * baseNumber;
    }

    /*
     3. 物品稀有度递减
     i. 物品品质从高到低
     ii. 物品价值从高到低
     iii. 物品阶数从高到底
     */

    //已有装备 品质降序
    public static int CompareByQualiy(EquipItem c1, EquipItem c2)
    {
        if (c1.quality == c2.quality)
        {
            if (c1.sellPrice == c2.sellPrice)
            {
                return c1.equipConfig.equipDrawingsConfig.level.CompareTo(c2.equipConfig.equipDrawingsConfig.level) * -1;
            }
            return c1.sellPrice < c2.sellPrice ? 1 : -1;
        }

        return c1.quality < c2.quality ? 1 : -1;
    }

    //市场物品 品质降序
    public static int CompareByQualiy(MarketItem c1, MarketItem c2)
    {
        if (c1.itemQuality == c2.itemQuality)
        {
            if (c1.goldPrice == c2.goldPrice)
            {
                return c1.equipConfig.equipDrawingsConfig.level.CompareTo(c2.equipConfig.equipDrawingsConfig.level) * -1;
            }
            return c1.goldPrice < c2.goldPrice ? 1 : -1;
        }

        return c1.itemQuality < c2.itemQuality ? 1 : -1;
    }

    /*
     5.物品类型
     i. 物品类型从上方种类排序从左至右
     ii. 物品价值从高到低
     iii. 物品阶数从高到底
     iv. 物品品质从高到低
     */

    //已有装备 类型排序
    public static int CompareByType(EquipItem c1, EquipItem c2)
    {
        if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
        {
            return CompareByValue(c1, c2, false);
        }
        return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
    }

    //装备配置表 类型排序
    public static int CompareByType(EquipDrawingsConfig c1, EquipDrawingsConfig c2)
    {

        if (c1.sub_type == c2.sub_type)
        {
            return CompareByValue(c1, c2, false);
        }

        return c1.sub_type < c2.sub_type ? -1 : 1;
    }

    //市场物品 类型排序
    public static int CompareByType(MarketItem c1, MarketItem c2)
    {
        if (c1.equipConfig.equipDrawingsConfig.sub_type == c2.equipConfig.equipDrawingsConfig.sub_type)
        {
            return CompareByValue(c1, c2, false);
        }
        return c1.equipConfig.equipDrawingsConfig.sub_type < c2.equipConfig.equipDrawingsConfig.sub_type ? -1 : 1;
    }

    /*
    4. 物品数量递减
    i. 物品数量递减
    ii. 物品价值从高到低
    iii. 物品阶数从高到底
    iv. 物品品质从高到低
     */

    //已有装备 数量排序
    public static int CompareByNum(EquipItem c1, EquipItem c2)
    {
        if (c1.count == c2.count)
        {
            return CompareByValue(c1, c2, false);
        }
        return c1.count < c2.count ? 1 : -1;
    }

    //装备配置表 数量排序
    public static int CompareByNum(EquipDrawingsConfig c1, EquipDrawingsConfig c2)
    {

        int c1Num = ItemBagProxy.inst.getEquipNumber(c1.id, 1);
        int c2Num = ItemBagProxy.inst.getEquipNumber(c2.id, 1);

        if (c1Num == c2Num)
        {
            return CompareByValue(c1, c2, false);
        }
        return c1Num < c2Num ? 1 : -1;
    }

    //市场物品 数量排序
    public static int CompareByNum(MarketItem c1, MarketItem c2)
    {
        if (c1.marketNum == c2.marketNum)
        {
            return CompareByValue(c1, c2, false);
        }
        return c1.marketNum < c2.marketNum ? 1 : -1;
    }

    #endregion

}
