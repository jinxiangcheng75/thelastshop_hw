using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticConstants
{
    #region GameSetting
    public static Vector2Int designSceneSize = new Vector2Int(1080, 1920);      //游戏设计尺寸（竖屏！！！！！）
    public static Vector2Int designSceneSizeL = new Vector2Int(1920, 1080);      //游戏设计尺寸（横屏！！！！！）
    public static float combatCameraOrthographicSize = 4.0f;            //战斗场景相机 标准设计尺寸下OrthographicSize  场景相机根据实际屏幕匹配size（保持场景宽度不变，竖屏！！！！！）
    public static float combatCameraOrthographicLSize = 2.6f;
    // public static readonly string photoSavePath = UnityEngine.Application.dataPath + "/GUI2D/Texture2D/";//UnityEngine.Application.persistentDataPath + "/";
    #endregion
    public static int glodID = 10001; //金币id
    public static int gemID = 10002; //钻石id
    public static int energyID = 10003; //体力
    public static int drawingID = 10004; //图纸
    #region InDoorMap
    public static Vector3 CellSize = new Vector3(1f, .5f, 0);
    public static int shopMap_MaxLevel = 10;   //商店扩建最大等级
    public static int IndoorMaxX = 18;
    public static int IndoorMaxY = 34;
    public static int IndoorOffsetX = 6;
    public static int IndoorOffsetY = 0;
    //
    public static int OutDoorMaxX = 5;
    public static int OutDoorMaxY = 34;
    public static Vector2Int floorSize = Vector2Int.one * 2;
    public static int floorDefaultId = 10001;
    public static int wallDefaultId = 11001;
    public static Vector3Int shopKeeperPoint = new Vector3Int(1, 1, 0); //如果是店主对应柜台位置 
    public static Vector3Int counterAreaOffset = new Vector3Int(-2, -2, 2);  //三面 0朝向 左下，右下，右上
    public static Vector3Int indoorGatePoint = new Vector3Int(0, 20, 0);
    public static float moveSpeed = 4.8f;

    #endregion
    #region 资源路径
    public static readonly string atlasPath = "Assets/GUI2D/SpriteAtlas/";
    public static string spinePath = "Assets/Spine/SpineExport/";//spine资源路径
    public static string shopkeeperBehaviorTreePath = "Assets/__Scripts/ShopkeeperAI/ShopkeeperBehavior.asset"; // 店主行为树
    #endregion
    #region 装备

    public static int SuperEquipBaseQuality = 100;
    //品质
    public static Dictionary<int, string> qualityNames = new Dictionary<int, string>
    {

        {0, "普通" },//普通装备
        {1, "高级" },
        {2, "无暇" },
        {3, "史诗" },
        {4, "传奇" },

        {SuperEquipBaseQuality + 0, "普通" }, //超凡装备
        {SuperEquipBaseQuality + 1, "高级" },
        {SuperEquipBaseQuality + 2, "无暇" },
        {SuperEquipBaseQuality + 3, "史诗" },
        {SuperEquipBaseQuality + 4, "传奇" },
    };

    public static Dictionary<int, string> qualityColorSprict = new Dictionary<int, string>
    {
        {0, "cangku_yuanpinzhi1" }, //普通装备
        {1, "cangku_yuanpinzhi2" },
        {2, "cangku_yuanpinzhi3" },
        {3, "cangku_yuanpinzhi4" },
        {4, "cangku_yuanpinzhi5" },

        {SuperEquipBaseQuality + 0, "cangku_yuanpinzhi1" }, //超凡装备
        {SuperEquipBaseQuality + 1, "cangku_yuanpinzhi2" },
        {SuperEquipBaseQuality + 2, "cangku_yuanpinzhi3" },
        {SuperEquipBaseQuality + 3, "cangku_yuanpinzhi4" },
        {SuperEquipBaseQuality + 4, "cangku_yuanpinzhi5" },
     };

    public static Dictionary<int, string> qualityColor = new Dictionary<int, string>
    {
        {0, "" }, //普通装备
        {1, "#5eff44" },
        {2, "#46c4ff" },
        {3, "#e95aff" },
        {4, "#ff9626" },

        {SuperEquipBaseQuality + 0, "" }, //超凡装备
        {SuperEquipBaseQuality + 1, "#5eff44" },
        {SuperEquipBaseQuality + 2, "#46c4ff" },
        {SuperEquipBaseQuality + 3, "#e95aff" },
        {SuperEquipBaseQuality + 4, "#ff9626" },
    };

    public static Dictionary<int, string> qualityTxtColor = new Dictionary<int, string>
    {
        {0, "#ffffff" }, //普通装备
        {1, "#5eff44" },
        {2, "#46c4ff" },
        {3, "#e95aff" },
        {4, "#ff9626" },

        {SuperEquipBaseQuality + 0, "#ffffff" }, //超凡装备
        {SuperEquipBaseQuality + 1, "#5eff44" },
        {SuperEquipBaseQuality + 2, "#46c4ff" },
        {SuperEquipBaseQuality + 3, "#e95aff" },
        {SuperEquipBaseQuality + 4, "#ff9626" },
    };

    public static Dictionary<int,string> qualityicon = new Dictionary<int, string>
    {
        {0, "cangku_pinzhi1" }, //普通装备
        {1, "cangku_pinzhi2" },
        {2, "cangku_pinzhi3" },
        {3, "cangku_pinzhi4" },
        {4, "cangku_pinzhi5" },

        {SuperEquipBaseQuality + 0, "cangku_pinzhi1" }, //超凡装备
        {SuperEquipBaseQuality + 1, "cangku_pinzhi2" },
        {SuperEquipBaseQuality + 2, "cangku_pinzhi3" },
        {SuperEquipBaseQuality + 3, "cangku_pinzhi4" },
        {SuperEquipBaseQuality + 4, "cangku_pinzhi5" },
    };

    public static int EquipStarUpItemId = 70001;
    //装备类型对应icon名称
    /*
    0-分隔符
    1-利刃
    2-钝器
    3-弓弩
    4-轻型武器
    5-步枪
    6-重型武器
    7-狙击枪
    8-霰弹枪
    9-轻型护甲
    10-重型护甲
    11-便衣
    12-头盔
    13-帽子
    14-轻装裤
    15-重装裤
    16-靴子
    17-鞋子
    18-面罩
    19-药品
    20-工具
    21-暗器
    22-投掷类
    23-首饰
    24-盾牌
    */
    //public static string[] EquipSubTypeClickSprites = {
    //    "icon_fengefu",
    //    "icon_liren2",
    //    "icon_dunqi2",
    //    "icon_gongnu2",
    //    "icon_qingwu2",
    //    "icon_buqiang2",
    //    "icon_zhongwu2",
    //    "icon_ju2",
    //    "icon_sandan2",
    //    "icon_qingjia2",
    //    "icon_zhongjia2",
    //    "icon_bianyi2",
    //    "icon_toukui2",
    //    "icon_maozi2",
    //    "icon_zhongku2",
    //    "icon_qingku2",
    //    "icon_xuezi2",
    //    "icon_xiezi2",
    //    "icon_yaopin2",
    //    "icon_gongju2",
    //    "icon_yanqi2",
    //    "icon_touzhi2",
    //    "icon_shoushi2",
    //    "icon_dunpai2",
    //};
    #endregion

    #region 升级信息
    public static string[] PlayerUpitemDesc = {
        "市场阶级",
        "新英雄栏位",
        "新贸易栏位",
        "新制作栏位",
        "商铺扩建",
        "新冒险栏位",
        "新勇士",
        "新工匠",
        "新英雄职业",
        "新家具",
        "新装饰",
    };

    public static string[] PlayerUpMsgBoxTip_Top = {
        "新市场层级已解锁",
        "新英雄栏位已解锁",
        "新贸易栏位已解锁",
        "新制作栏位已解锁",
        "新的空间已解锁",
        "新冒险栏位已解锁",
        "新勇士可用",
        "新工匠可用",
        "新英雄职业解锁",
        "新家具可用",
        "新装饰可用",
    };

    public static string[] PlayerUpMsgBoxTip_Bottom = {
        "新品阶物品现已登录市场!",
        "一个新的英雄栏位现在可使用",
        "一个新的贸易栏位现在可使用",
        "一个新的制作栏位现在可使用",
        "一个新的空间现在可使用",
        "一个新的冒险栏位现在可使用",
        "新勇士可用",
        "新工匠可用",
        "新英雄职业解锁",
        "新家具现已开放购买!",
        "新装饰现已开放购买!",
    };

    //升级类型对应icon名称
    /*
     市场等级 0
     英雄栏位 1
     贸易栏位 2
     制作栏位 3
     店铺扩建限制 4
     冒险栏位 5
     勇士id 6
     工匠id 7
     英雄职业 8
     新家具 9
     新装饰 10
    */
    public static string[] PlayerUpTypeSprites = {
        "icon_shichang",
        "icon_yingxiong",
        "icon_jiaoyisuo",
        "icon_zhizao",
        "icon_kuojian",
        "icon_maoxian",
        "",
        "",
        "",
        "icon_jiaju",
        "icon_waiguan",
    };

    #endregion

    #region 换装
    public static string[] exTypes = new string[] { "外观", "肤色", "发型", "发色", "五官", "五官颜色", "", "", "眼睛颜色" };
    public static string[] faTypes = new string[] { "衣服", "裤子", "帽子", "鞋子", "", "手持" };

    //外观 1-皮肤颜色，2-发型，3-发型颜色，4-五官，5-五官颜色，8-眼睛颜色

    //外观 男
    public static Dictionary<int, string> dressupIconYellow_Facade_Boy_Dic = new Dictionary<int, string>
    {
        {1,"dianzhu_fuse1"},
        {2,"dianzhu_faxing1"},
        {3,"dianzhu_faxing1"},
        {4,"dianzhu_wuguannan1"},
        {5,"dianzhu_wuguannan1"},
        {8,"dianzhu_yanjing1"},

    };

    //外观 女
    public static Dictionary<int, string> dressupIconYellow_Facade_Girl_Dic = new Dictionary<int, string>
    {
        {1,"dianzhu_fuse1"},
        {2,"dianzhu_faxingnv1"},
        {3,"dianzhu_faxingnv1"},
        {4,"dianzhu_wuguannv1"},
        {5,"dianzhu_wuguannv1"},
        {8,"dianzhu_yanjing1"},
    };

    //时装 20-时装衣服，21-时装裤子，22-时装帽子，23-时装鞋子，25-时装手持
    public static Dictionary<int, string> dressupIconYellow_FashionDic = new Dictionary<int, string>
    {
        {20,"dianzhu_xxshangyi1"},
        {21,"dianzhu_xxkuzi1"},
        {22,"dianzhu_xxmaozi1"},
        {23,"dianzhu_xxxuezi1"},
        {25,"dianzhu_xxshouchi1"},
    };

    //-----------------------------------------------顾客表现相关换装
    public static readonly string woman_weapon_slotName = "woman_pistol_front_r";//女 手持武器位置
    public static readonly string woman_shield_slotName = "woman_shield_front";//女 手持盾牌位置
    public static readonly string woman_back_one_slotName = "weapon_onehand";//女 腰间
    public static readonly string woman_back_two_slotName = "weapon_twohand";//女 背后
    public static readonly string woman_back_shield_slotName = "woman_shield_front2";//女 盾牌存放位置

    public static readonly string man_weapon_slotName = "man_guntwo_front_r";//男 手持武器位置
    public static readonly string man_shield_slotName = "man_shield_front";//男 手持盾牌位置
    public static readonly string man_back_one_slotName = "weapon_onehand";//男 腰间
    public static readonly string man_back_two_slotName = "weapon_twohand";//男 背后
    public static readonly string man_back_shield_slotName = "man_shield_front2";//男 盾牌存放位置
    #endregion

    #region 创角
    public static string[] types = new string[] { "肤色", "发型", "发色", "五官", "五官颜色", "", "", "眼睛颜色" };
    public static int[] roleCreatIndex = new int[] { 1, 2, 3, 4, 5, 8 };
    #endregion

    #region 交易所

    //上架时间及货币类型调整税率

    public static int[] time_times = { 12, 24, 48 };
    public static float[][] time_taxRates = { new float[] { 0.1f, 0.12f, 0.15f }, new float[] { 0.2f, 0.22f, 0.25f } };

    //钻石定价对应稀有度系数
    public static Dictionary<int, float> qualityGemCoefficient = new Dictionary<int, float>()
    {
        {0,1}, //普通
        {1,1.75f},
        {2,4},
        {3,9},
        {4,17.5f},

        {100,1 * 3},//超凡
        {101,1.75f * 3},
        {102,4 * 3},
        {103,9 * 3},
        {104,17.5f * 3},
    };

    //钻石最低价
    public static int lowestGemPrice = 2;
    #endregion

    public static int heroLvLimitHouseID = 2000;
    public static int tboxAndKeyOffset = 10000;
    #region 角色
    public static string roleHeadIconAtlasName = "portrait_atlas";//人物头像图集
    public static string roleAtlasName = "hero_atlas";
    public static string staticAtlasName = "StaticIcon";
    public static string[] roleIntelligenceColor =
    {
        "#ffffff",
        "#5aff44",
        "#2ff8ff",
        "#e95aff",
        "#ff911c"
    };

    public static string[] roleEquipSubType =
    {
        "利刃",
        "钝器",
        "弓弩",
        "轻型武器",
        "步枪",
        "重型武器",
        "狙击枪",
        "霞弹枪",
        "轻型护甲",
        "重型护甲",
        "便衣",
        "头盔",
        "帽子",
        "轻装裤",
        "重装裤",
        "鞋子",
        "靴子",
        "药品",
        "工具",
        "暗器",
        "投掷类",
        "首饰",
        "盾牌"
    };

    public static int[] roleWorldParId =
    {
        230,
        231,
        232,
        233
    };

    public static int[] roleWorldParNId =
    {
        234,
        235,
        236,
        237
    };

    public static int[] roleWorldParMId =
    {
        238,
        239,
        240,
        241
    };

    public static string[] roleRecruitBgIconName =
    {
        "yingxiong_zhaomudi2",
        "yingxiong_zhaomudi1",
        "yingxiong_zhaomudi3",
        "yingxiong_zhaomudi4"
    };

    public static string[] roleTalentBgIconName =
    {
        "tanxian_tianfukuanglv",
        "tanxian_tianfukuanglan",
        "tanxian_tianfukuang",
        "tanxian_tianfukuangchen"
    };

    public static Dictionary<int,string> roleEquipQualityIconName = new Dictionary<int, string>
    {
        {0, "yingxiong_zhuangbeidi6"},
        {1, "yingxiong_zhuangbeidi2"},
        {2, "yingxiong_zhuangbeidi3"},
        {3, "yingxiong_zhuangbeidi4"},
        {4, "yingxiong_zhuangbeidi5"},
        {5, "yingxiong_zhuangbeidi6"},

        {SuperEquipBaseQuality + 0, "yingxiong_zhuangbeidi6"},
        {SuperEquipBaseQuality + 1, "yingxiong_zhuangbeidi2"},
        {SuperEquipBaseQuality + 2, "yingxiong_zhuangbeidi3"},
        {SuperEquipBaseQuality + 3, "yingxiong_zhuangbeidi4"},
        {SuperEquipBaseQuality + 4, "yingxiong_zhuangbeidi5"},
        {SuperEquipBaseQuality + 5, "yingxiong_zhuangbeidi6"},
        

    };

    public static string[] roleHeroBgIconName =
    {
        "tanxian_touxiangklv",
        "tanxian_touxiangklan",
        "tanxian_touxiangkzi",
        "tanxian_touxiangkcheng",
    };

    public static string[] roleTalentQualityStr =
    {
        "优秀",
        "稀有",
        "史诗",
        "传奇"
    };

    public static Dictionary<int, int> talentDataBaseEntry = new Dictionary<int, int>
    {
        {0,0 },
        {26, 1},
        {27, 2},
        {28, 3},
        {29, 4},
        {30, 5},
        {31, 6},
        {32, 7},
        {33, 8},
        {34, 9},
        {35, 10},
        {36, 11},
        {37, 12},
        {38, 13},
        {39, 14},
        {40, 15},
        {41, 16},
        {42, 17},
        {43, 18},
        {44, 19},
        {45, 20},
        {46, 21},
        {47, 22},
        {48, 23},
        {51, 1},
        {52, 2},
        {53, 1},
        {54, 2},
        {55, 1},
        {56, 2},
    };

    public static string[] roleIntelligenceStr =
    {
        "N",
        "R",
        "SR",
        "SSR"
    };

    public static string[] roleIntelligenceIconStr =
    {
        "yingxiong_jibien",
        "yingxiong_jibier",
        "yingxiong_jibiesr",
        "yingxiong_jibiessr"
    };

    public static Vector3Int specialRoleInitPos = new Vector3Int(0, 22, 0);

    #endregion

    #region 公会
    public static string[] unionRange = new string[] { "完全公开", "私人" };
    public static int[] unionJobArray = { (int)EUnionJob.Common, (int)EUnionJob.Manager, (int)EUnionJob.President };
    public static string[] unionJobNameArray = { "成员", "管理员", "会长" };
    public static string[] unionJobIconArray = { "gonghui_huiyuan", "gonghui_huiyuan", "gonghui_huizhang" };
    public const int UnionEnterLvMax = 99;
    #endregion

    #region 副本
    public static string exploreAtlas = "Explore_atlas";
    public static string[] diffType = new string[]
    {
        "简单",
        "正常",
        "困难"
    };
    public static string[] diffIconName = new string[]
    {
        "yingxiong_jiandantubiao",
        "yingxiong_zhengchangtubiao",
        "yingxiong_kunnnatubiao"
    };
    public static string[] diffColor = new string[]
    {
        "#74f73a",
        "#e762ff",
        "#f66060"
    };
    public static string[] getKeyProbability = new string[]
    {
        "25%",
        "35%",
        "50%"
    };
    public static string[] heroFaceIconName = new string[]
    {
        "cangku_xiaolian3",
        "cangku_xiaolian1",
        "cangku_xiaolian2"
    };
    public static string[] heroTypeBgIconName = new string[]
    {
        "tanxian_sexihong",
        "tanxian_sexilv",
        "tanxian_sexilan"
    };
    #endregion

    #region 家具
    public static string funitureItemAtlasName = "shopdesign_atlas";

    public static string[] furnitureSubTypeNames = new string[]
    {
        "",
        "墙壁",
        "地板",
        "地毯",
        "墙壁装饰",
        "室内地面装饰",
        "柜台",
        "货架",
        "储物箱",
        "资源篮",
        "室外装饰",
        "宠物小家",
    };

    public static string[] funitureItemIcons = new string[]
    {
        "",
        "zhuejiemian_tili",
        "jiaju_neirong",
        "jiaju_neirong",
        "jiaju_diejia",
        "icon_ziyuan3",
        "icon_ziyuan7",
        "icon_ziyuan10",
        "icon_ziyuan9",
        "icon_ziyuan8",
        "icon_ziyuan1",
        "icon_ziyuan6",
        "icon_ziyuan4",
        "icon_ziyuan5",
        "icon_ziyuan2",
        "jiaju_bianjikongjian",
        "zhizuo_daojishi"
    };

    public static string[] furnitureSubTypeIconNames = new string[]
    {
        "",
        "jiaju_xxiaobai7",
        "jiaju_xxiaobai6",
        "jiaju_xxiaobai5",
        "jiaju_xxiaobai3",
        "jiaju_xxiaobai3",
        "",
        "jiaju_xxiaobai1",
        "jiaju_xxiaobai8",
        "jiaju_xxiaobai2",
        "jiaju_xxiaobai4",
    };

    #endregion

    #region UI动画
    //------------主界面
    public const int ShowSlotNum = 4;
    public const float SlotOffset = 6.6f;
    public const float btnDelayTime = 1.15f;
    #endregion

    #region 新手引导
    public static string guideAtlas = "guide_atlas";
    #endregion

    public static string commonAtlas = "common_atlas";
    #region 获得道具底(品质)
    public static string[] GetItemBgIcon = new string[]
    {
        "yingxiong_jinengdi",
        "yingxiong_jinenglvdi",
        "yingxiong_jinenglandi",
        "yingxiong_jinengzidi",
        "yingxiong_jinengchendi",
    };
    public static string[] GetLotteryBgIcon = new string[]
    {
        "yingxiong_jinengdi",
        "yingxiong_jinengzidi",
        "yingxiong_jinengchendi",
    };
    public static string[] GetTreasureBoxBgIcon = new string[]
    {
        "",
        "shoudao_tubiaok1",
        "shoudao_tubiaok4",
        "shoudao_tubiaok5"
    };
    public static string[] GetLotteryQualityBgIcon = new string[]
    {
        "zhuanpan_xiyou",
        "zhuanpan_chuanqi",
    };
    #endregion

    #region 店铺角色AI
    public const int noneWeightBase = 1000;
    public const int haveWeightBase = 2000;
    public const int notHaveWeightBase = 3000;
    public const int petWeightBase = 4000;
    public const int shopkeeperWeightBase = 5000;
    #endregion
}
