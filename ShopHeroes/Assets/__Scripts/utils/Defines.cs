

public interface IDataModelProx
{
    void Init();
    void Clear();  //数据清理
}

// 收到物品或奖励所显示UI的类型
public enum ReceiveInfoUIType //越靠前优先级越高
{
    LookBack, //玩家首次登陆有系统消息界面
    UnoinTaskResult,//联盟周结算界面
    BuyGoodsComplete, // 礼包购买完成
    VipBuyComplete,//特权卡购买完成
    CommonReward,//通用领奖
    ShopperLvUp,        //店主升级
    StarUpEffectTrigger_return, //升星效果触发 - 返还材料
    StarUpEffectTrigger_double, //升星效果触发 - 双重制作
    StarUpEffectTrigger_super, //升星效果触发 - 超凡装备 注 升星效果触发类型中间不要插入其他类型~
    AdvacedEquip,       //制作出高品质的装备
    DrawingUpLv,        //图纸升级
    UnLockDrawing,      //解锁
    GetItem,            //(获得道具)
    ActivateDrawing,    //已激活图纸
    DrawingStarUp,      //图纸升星
    // HeroLvUP,           //英雄升级
    // ExploreUP,          //副本升级
    ExploreEquipDamaged, //副本装备破损
    ExploreEquipDamagedInfo, //副本装备破损修复界面
    ExploreAward,       //副本结算
    UnlockWorker, //解锁工匠
    WorkerUp, //工匠升级
    LuxuryUp,//豪华度升级
    GlobalBuff,//主题全服buff
    SpecialAchievement,//特殊成就弹出
    DirectPurchasePush, //礼包推送
    UnionTaskTip,//悬赏任务tip
    DailyTaskTip,//日常任务tip
    Hint,//提示界面
    UnlockSystem,//解锁新系统 
    GuideTrigger,//触发式引导
}

//排序类型
public enum SortType
{
    Number,
    Price,
    Level,
    Quality,
    Time,
    SubType,
    Num
}
public enum ViewShowType
{
    normal = 0, //默认主界面  直接弹出 如 主界面，城市界面， 设计界面 等
    popup = 1,  //弹出界面
    dataInfo = 2, //物品或人物数据 界面.
    pullUp = 3,   //从底部或侧面拉起界面， 仓库、制作、聊天等
}
//市场item排序类型
public enum EmarketItemSortType  //1.物品阶数+/-。2.物品价值+/- 3.物品稀有度。4.最近选择。5.物品类型。6.物品数量。
{
    LevelDescending,
    LevelAscending,
    ValueDescending,
    ValueAscending,
    Quality,
    Near,
    Type,
    Num,
    max
}

//物品类型
/*
1-金币资源 
2-钻石资源 
3-能量资源 
4-图纸资源
5-可生产资源
6-副本资源
7-袋子资源
8-宝箱资源
9-钥匙资源
10-城市突袭成就资源

*/
public enum ItemType
{
    Glod = 1,               //金币，
    Gem = 2,                //钻石 
    Energy = 3,             //能量
    Blueprint = 4,          //设计图
    Material = 5,           //资源 石头、木头等
    TaskMaterial = 6,       //任务产出资源
    Bag = 7,                //袋子
    Box = 8,                //箱子
    Key = 9,                //钥匙
    FusionStone = 10,       //融合石资源
    Activity = 11,          //活动资源
    WarriorCoin = 12,       //勇士币
    Hero = 13,              //英雄
    Craftsman = 14,         //工匠
    SpecialEquipment = 15,  //特殊装备材料
    EquipmentDrawing = 16,  //指定装备的图纸
    Turntable = 17,         //转盘开启消耗
    HeroExp = 18,           //英雄经验卡
    HeroTransfer = 19,      //英雄转职道具
    ExploreTimeItem = 20,   //副本减少冒险休息时间道具
    ExploreAddYieldItem = 21,//副本增加产量道具
    ExploreAttBonus = 22,   //副本攻击加成道具
    RecoverHeroItem = 23,   //恢复英雄道具
    ExploreExpBonusItem = 24,//副本经验获得加成道具
    RepairEquipmentItem = 25,//修复装备道具
    Equip = 26,              //装备
    Active = 27,             //活跃度
    Gift = 28,              //礼包
    UnionCoin_self = 29,     //联盟币
    UnionCoin_union = 30,    //联盟积分
    UnionRenown = 31,        //联盟声望
    Furniture = 40,          //家具
    ShopkeeperDress = 41,    //店主装扮
    HeroCard = 42,           //英雄卡
    MakeSlotNum = 43,        //制作槽位数
    ExploreSlotNum = 44,     //冒险队列数
    ShopSizeNum = 45,        //店铺扩建等级
    HeroSlotNum = 46,        //英雄栏位数
    MarketSlotNum = 47,      //交易栏位数
    Activity_WorkerGameCoin = 48, //巧匠大赛活动币
    EquipStarUp = 49,        //装备升星道具
    HeroPropertyUp = 50,     //英雄属性提升道具
    PetSkin = 51,            //宠物皮肤
}

#region 装备


//装备大类型
public enum EquipType
{
    Weapon = 1,      //武器
    Armor = 2,       //服饰 帽子 鞋 手套等
    Other = 3,       //药品 工具 暗器，饰品等
}

public enum EquipSubType
{
    //武器
    sharp = 1,          //利刃
    blunt = 2,          //钝器
    bow = 3,            //弓弩
    handgun = 4,        //轻武器
    rifle = 5,          //步枪
    heavy = 6,          //重型武器
    Biochemical = 7,    //狙击枪
    bomb = 8,           //散弹枪
    //防具
    lightly_coat = 9,   //轻甲
    heavy_coat = 10,    //重甲
    coat = 11,          //便衣
    helmet = 12,        //头盔
    cap = 13,           //帽子
    cuff = 14,          //轻裤子   
    lightly_pants = 15, //重裤子
    heavy_pants = 16,   //鞋子
    shoe = 17,          //靴子
    //其他
    drug = 18,           //药品
    tool = 19,           //工具
    hiddenWeapon = 20,   //暗器
    throwing = 21,       //投掷类
    acc = 22,     //饰品
    shield = 23,        //盾牌
    max
}


public enum kMapDistrictType
{
    Indoor,
    Outdoor,
}


public enum kShelfType
{
    None,
    ThermalWeapon,      //
    ColdeWeapon,
    Armor,
    Misc,
    Num
}

public enum kResourceBinType
{
    None,
    Iron,
    Wood,
    Leather,
    Herb,
    Steel,
    Ironwood,
    Fabric,
    Oil,
    Jewel,
    Ether,
    Num
}

public enum kTrunkType
{
    None,
    Trunk,
    WallTrunk
}


[System.Flags]
public enum kTileLayerType
{
    Ground = 0,
    Floor = 1 << 0,
    Wall = 1 << 1,
    Carpet = 1 << 2,
    WallFurniture = 1 << 3,
    Furniture = 1 << 4,
    OutdoorFurniture = 1 << 5,
}
public enum kMapEditMode
{
    None,
    Furniture,
    Scaler,
}
public enum kScalerMode
{
    FourCorner, //for floor 
    TwoEdge,    //for wall
    FourEdge,   //not use
}
public enum kScalerNode
{
    Up,
    Right,
    Down,
    Left
}

public enum kDesignMode
{
    None,
    Pick,
    Edit,
    Create
}
#endregion
#region 2dMap
public enum kQueueState
{
    None,
    DirectQueue,
    EnterQueue,
    Placeholder,
}

public enum MapType
{
    WORLD,  //世界地图
    INDOOR  //室内地图
}

public enum kTileGroupType
{
    None,//
    Wall,//
    Floor,
    Carpet,
    WallFurniture,//墙壁装饰
    Furniture,//装饰
    Counter,//柜台
    Shelf,  //货架
    Trunk, //储物箱
    ResourceBin,//资源容器
    OutdoorFurniture,//室外装饰
    PetHouse,//宠物窝
}


public enum DesignMode
{
    normal = 0,             //默认状态
    modeSelection = 1,      //设计选择选择（设计主界面）
    FurnitureEdit = 2,      //家具编辑模式
    FloorEdit = 3,          //地板编辑
    WallEdit = 4,           //墙纸编辑
    LookPetHouse = 5,       //观赏宠物小家
}


public enum kFurnitureDisplayType
{
    ShelfAndTrunk,
    ResourceBin,
    Furniture,//
    OutdoorFurniture,
    Carpet,
    Extra,//
    None,
}

public enum kCustomizeDisplayType
{
    Floor,
    Wall,
    Extra,
    None
}
#endregion 2dMap
#region 换装

/// <summary>
/// 外观  1-皮肤颜色，2-发型，3-发型颜色，4-五官，5-五官颜色，8-眼睛颜色
/// </summary>
public enum FacadeType
{
    Sex = 0,
    ModelColor = 1, // 皮肤颜色
    Hair = 2, // 发型
    HairColor = 3, // 发型颜色
    Face = 4,//五官
    FaceColor = 5,//五官颜色
    EyesColor = 8, // 眼睛颜色

    max
    //BearStyle = 4, // 胡子
    //BearColor = 5, // 胡子颜色
    //MakeUp = 6, // 妆颜
    //MakeUpColor = 7, // 妆颜颜色
}

/// <summary>
/// 时装 20-时装衣服，21-时装裤子，22-时装帽子，23-时装鞋子，25-时装手持，
/// </summary>
public enum FashionType
{
    Clothe = 20, // 时装衣服
    Pants = 21, // 时装裤子
    HeadHat = 22, // 时装帽子
    Shoes = 23, // 时装鞋子
    Weapon = 25, // 时装手持

    max
    //Glasses = 24, // 时装眼镜
}


#endregion

#region 交易所

//摊位状态
public enum kBoothStateType  //0 未激活  1 可扩建  2 就绪  3 已挂上物品
{
    Lock,
    Extension,
    OK,
    HasItem
}

//上架物品类型
public enum kMarketItemType //0 报价 1 请求 2 公会请求
{
    selfSell,
    selfBuy,
    UnionBuy
}

//交易所类型
public enum kMarketTradingHallType // 0 购买大厅 1 出售大厅
{
    selfBuy,
    selfSell
}
#endregion

#region 抽奖
public enum kLotteryType
{
    World,
    Myself,
    max
}

public enum kSingleType
{
    Free,
    UseItem,
    UseGem,
    max
}

public enum kTenthType
{
    UseItem,
    UseGem,
    max
}

public enum kLotteryPopupType
{
    Cumulative,
    Jackpot,
    Lottery,
    GetCumulative,
    max
}
#endregion

#region 角色系统
public enum kRoleType
{
    Hero, // 英雄
    Artisan, // 工匠
    max
}

public enum kRoleItemType
{
    Hero,
    AddField,
    RecruitHero,
    RecoverHero,
    Worker,
    ExchangeHero,
    AddNew,
    max
}

public enum kRoleHeroChange
{
    UseExpItem,
    UseRestItem,
    ExploreImmediately,
    max
}

public enum kRoleRecruitDirec
{
    Front,
    Back
}

public enum kRoleWorkerGetType
{
    turntable = 1,//转盘
    guide = 2,//引导
    buy = 3,//雇佣
    sevenDay = 4,//七日活动
    buildingLink = 5, //建筑解锁前提且可雇佣
    giftLink = 6,//礼包解锁
}

public enum kRolePropertyType 
{
    hp = 1,
    def = 2,
    atk = 3,
}

#endregion

#region 城市建筑

public enum kCityBuildingType
{
    Resource = 1,
    Functional = 2,
    Science = 3,
}

public enum kCityBuildingUnlockType
{
    Unlock = 0, //直接解锁
    BuildingLv = 1, //指定建筑达到等级
    ShopLv = 2, // 店主达到等级
    NeedOneWorker = 3, //解锁指定工匠
}

#endregion

#region 副本
public enum kExploreSlotState
{
    Idle = 0,
    Exploring = 1,
    Finish = 2,
    max
}

public enum kExploreItemUpgradeType
{
    HeroUpgrade, // 英雄升级
    ExploreUpgrade // 副本升级
}
#endregion

#region 宝箱
public enum kTreasureBoxInfoType
{
    Exclusive = 0,
    Chance = 1,
    max
}
#endregion
#region 战斗
public enum BattleActionType
{
    skill = 1,      //释放技能, 包括普通攻击、技能攻击
    addBuff = 2,    //加BUFF
    removeBuff = 3, //减BUFF
    buff = 4        //BUFF效果
}
#endregion


#region 动画

public enum ESlotAnimType
{
    Normal = 0,
    Refresh = 1,
    MakeEnd = 2,
    JustAnim = 3,
    Draging = 4,

    max
}

#endregion

#region 新手引导
public enum K_Guide_Type
{
    FullScreenDialog = 1, // 全屏对话
    Tip, // tips
    RestrictClick, // 限制点击
    UnlockFurniture, // 解锁家具面板
    UnlockWorker, // 解锁工匠
    Task, // 任务界面
    NPCCreat, // 创建npc
    NPCState, // npc移动或播放动画
    ClickUnlockFurn, // 点击家具 解锁
    WeakGuide, // 弱引导
    WeakGuideAndTask, // 弱引导和任务
    ClickNpcPrompt,
    ResearchEquip, // 研究装备
    JudgeData, // 先不管这个
    CreatNewFurniture, // 不管
    GiveEquip, // 赠送装备
    RestrictShopper, // 限制点击顾客
    End = 20,
    JudgeExploreTime,
    JudgeMakeEquipSlot,
    EmptyOperation,
    TipsAndRestrictClick,
    RestrictAndJudgeHeroTime,
    GetItemPanel,
    Max
}

public enum K_Guide_Trigger
{
    CompleteStep = 1,
    ArriveLevel,
    ArriveEquipNum,
    ArriveTarget,
    FirstStart,
    HaveTargetInBag,
    TargetPanelOpen,
    WaitTargetShopper,
    Max
}

public enum K_Guide_End
{
    None = 1,
    ClickTarget,
    ArriveLevel,
    ArriveEquipNum,
    DialogFinish,
    CreatNpcFinish,
    MoveToTarget,
    ClickCountArrive,
    Max
}

public enum K_Guide_UI
{
    GMask,
    GWhiteMask,
    GDialog,
    GTips,
    GTask,
    GMaskTips,
    GUnlockNewFurniture,
    GUnlockWorker,
    GFinger,
    GShopperMask,
    GNewTask,
    Max
}

public enum K_Guide_WeakGuide
{
    GFindByName = 1,
    GFindeById,
    GFindeByIndex,
    GFindCustom,
    Max
}

public enum K_Guide_ArrowDirect
{
    Normal = 1,
    UI = 2,
    CityHouse = 3
}

public enum K_Guide_TriggerType
{
    Min = 0,
    OpenPanel = 1,
    Dialog = 2,
    FinishGuide = 3,
    WaitShopper = 4,
    DialogAndFinger = 5,
    WaitAnyoneFurniture = 6,
    WaitPanelOpen = 7
}
#endregion

#region 成就
public enum K_Acheivement_AwardState
{
    None = 0,
    CanReward = 1,
    Rewarded = 2
}
#endregion

#region 七日目标
public enum K_SevenDay_Type
{
    None = 0,
    MakeMoney = 1, // 赚取金币
    MakeTargetEquip = 2, // 制作指定类的装备
    UnlockEquip = 3, // 解锁装备
    ReceiveDrawing = 4, // 获取图纸
    UnlockTargetLvEquip = 5, // 装备等级解锁
    EquipmentMastery = 6, // 装备精通
    BuyFurniture = 7, // 家具购买
    FurnitureLvUp = 8, // 家具升级
    StoreExpasion = 9, // 店铺扩建
    Union = 10, // 公会类
    TechnologyUpgrading = 11, // 科技升级
    BuildUpgrading = 12, // 建筑升级
    BuildTargetLv = 13, // 指定建筑等级
    HeroRecruit = 14, // 英雄招募
    HeroTransfer = 15, // 英雄转职
    HeroRarity = 16, // 英雄稀有度
    HeroWearEquipLv = 17, // 装备穿戴等级
    ExploreUpgrading = 18, // 副本升级
    ExploreChallenges = 19, // 副本挑战
    AddMakeSlot = 20, // 增加制作槽
    LevelUp = 21, // 等级提升
    OpenTreasureBoxCount = 22, // 宝箱开启数量
    PrestigePromotion = 23, // 声望提升
    MarketTransactions = 24, // 市场交易
    EnergyUp = 25, // 能量提升
    ExploreCount = 26, // 冒险次数
    SellEquip = 27, // 贩卖装备
    MarkUpSale = 28, // 加价出售
    EquipmentExchange = 29, // 装备兑换
    HeroLevelUp = 30, // 提升英雄等级
}
#endregion

#region 行为
public enum K_Operation_Type
{
    Normal = 0,//常规类型
    SelectSceneFuniture = 1,//选中能升级的家具
    BuyOrSetFurniture = 2,//购买或放置家具
    RecruitHero = 3,//招募英雄
    AddHeroSlot = 4,//增加英雄栏位
    SelectHeroInfo = 5,//点击英雄详情
    SelectShopper = 6,//点击顾客气泡
    OpenTargetTbox = 7,//打开指定宝箱
    BuildInvest = 8,//建筑投资
    ClickScience = 9,//点击科学院
    SelectCanRecruitHero = 10,//选择能进行招募的英雄
    SelectTargetEquipPage = 11,//选择指定装备分页
    SelectTargetWorker = 12,//选择指定工匠
    SelectTrasnferTargetHero = 13,//选择转职需要英雄
    SelectTargetExplore = 14,//选择指定副本
    SelectTransferTargetToggle = 15,//选择指定的转职页签
    SelectTargetEquip = 16,//选择指定的装备
}

public enum K_Operation_Finger
{
    Normal = 0,
    SceneFurn = 1,
    ShopperPop = 2,
    BuildInvest = 3,
    max
}

public enum K_Operation_DataType
{
    MainLine,//主线任务
    HyperLink,//超链
    SevenDay,//七日
    NewFunction,//新功能
    NoviceTask,//待办任务
    DailyTask,//每日活跃任务
}
#endregion

#region 特权
public enum K_Vip_State
{
    NotBuy = 0,
    Vip = 1,
    Overdue = 2
}

public enum K_Vip_Type
{
    UnlockRepairEquipPower = 1, // 解锁新币修理破损装备权限
    RepairEquipReduce = 2, // 破损装备修理费用减少
    UnlockActivityReward = 3, // 解锁活动特权奖励领取权限
    ExploreDropAdd = 4, // 副本掉落道具增加
    OpenTBoxAdd = 5, // 开启宝箱道具奖励增加
    PiggyRewardAdd = 6, // 小猪储蓄罐奖励增加
    UnlockTaskFourthReward = 7, // 解锁每日活跃任务第四个奖励
    CanReceiveTaskFourthReward = 8, // 可领取每日活跃任务第四个奖励
    LotteryTenthPriceReduce = 9, // 转盘十连抽价格减少
    MallBuyGoodsPriceReduce = 10, // 商城购买商品价格减少
    RecruitHeroPriceReduce = 11, // 招募英雄价格减少
    ReceiveTargetDecoration = 12, // 家具赠送
    FurnitureBuff = 13, // 家具buff
}
#endregion

#region 镜头移动类型

public enum kCameraMoveType
{
    none,
    shopExtend = 1,//店铺扩建
    furnitureUp = 2,//家具升级
    shopperDeal = 3,//顾客买卖
    citySecene = 4,//城市场景

}

#endregion

//public void 
// public static bool autoRotating = false;        //自由旋转(暂时不使用)
// public static bool lockLandscape = true;        //锁定左横屏
// public static bool lockVertical = false;        //锁定竖屏
public enum SceneRotatingType
{
    LockLandscape = 0,
    LockVertical = 1,
    AutoRotating = 2
}