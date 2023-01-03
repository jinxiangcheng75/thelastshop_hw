using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    string _playeruname = "";
    public string userUid;
    public string playerName    //玩家名字
    {
        get
        {
            if (LanguageManager.inst != null)
            {
                return LanguageManager.inst.GetValueByKey(_playeruname);
            }
            return _playeruname;
        }
        set
        {
            _playeruname = value;
        }
    }
    public int guideId;

    public uint level;          //等级
    public long MaxExp;
    public long CurrExp;

    public uint gender;
    public uint freeNameCount;
    public RoleDress userDress;

    public long gold = 0;
    public long gem = 0;
    public long energy = 0;
    public long energyLimit = 0;
    public long drawing = 0;

    public long bagLimit = 0;
    public long pileLimit = 0;

    public long worth = 0;//净价值
    public long invest = 0;//投资
    public long prosperity = 0;
    public long masterCount = 0;//精通

    //公会
    public string unionId = "";
    public string unionName = "";
    public int unionLevel = 0;
    public long memberJob = 0; //enum EUnionJob


    public long unionCoin = 0;  //个人的公会币
    public int unionHelpCount = 0;//帮助次数
    public int unionTaskCrownCount = 0;//悬赏次数

    //免费次数
    public long designFreeCount = 0;
    public long heroBuyFreeCount = 0;
    public long equipImproveFreeCount = 0;
    public long exploreImmediatelyFreeCount = 0;

    //会员
    public int vipLevel = 0;
    public int vipState = 0; // 0 notBuy 1 buyEffective 2 buyOverdue

    public bool hasUnion { get { return unionId != ""; } }

    public bool hasMainPet { get { return mainPetUid != 0; } }
    public int mainPetUid;

    public int curLuxuryLevel;

    public bool isVip()
    {
        return vipState == (int)K_Vip_State.Vip;
    }

    public void InitItemData(UserData userdata, UnionData unionData, int _freeNameCount, FreeData freeData, VIPInfo vipInfo, OnePetInfo mainPetInfo)
    {
        playerName = userdata.nickName;
        level = (uint)userdata.level;
        MaxExp = ShopkeeperUpconfigManager.inst.GetConfig(level < 99 ? level + 1 : 99).experience;
        CurrExp = userdata.exp;
        gender = (uint)userdata.gender;
        userDress = userdata.userDress;
        gold = userdata.gold;
        gem = userdata.gem;
        energy = userdata.energy;
        energyLimit = userdata.energyLimit;
        drawing = userdata.drawing;
        unionCoin = userdata.unionCoin;
        unionHelpCount = userdata.unionHelpCount;
        unionTaskCrownCount = userdata.unionTaskCrownCount;

        ItemBagProxy.inst.updateItemNum(StaticConstants.drawingID, drawing);
        bagLimit = userdata.bagLimit;
        pileLimit = userdata.pileLimit;

        worth = userdata.worth;
        invest = userdata.invest;
        prosperity = userdata.prosperity;
        masterCount = userdata.masterCount;

        guideId = userdata.guideId;
        GuideDataProxy.inst.setGuideData(guideId);

        freeNameCount = (uint)_freeNameCount;

        designFreeCount = freeData.freeDesignBuyCount;
        heroBuyFreeCount = freeData.freeHeroBuyCount;
        equipImproveFreeCount = freeData.freeEquipImproveCount;
        exploreImmediatelyFreeCount = freeData.freeExploreImmediateCount;

        mainPetUid = mainPetInfo.petUid;

        SetUnionData(unionData);
        setVipData(vipInfo);
    }

    public void SetUnionData(UnionData unionData)
    {
        //-------------------------公会---------------------
        unionId = unionData.unionId;
        unionName = unionData.unionName;
        unionLevel = unionData.unionLevel;
        memberJob = unionData.memberJob;

        //Logger.error("公会id ：" + unionId);
        //Logger.error("公会名称 ：" + unionName);
        //Logger.error("公会等级 ：" + + unionLevel);
        //Logger.error("公会职务 ：" + + memberJob);
    }

    public void setVipData(VIPInfo vipInfo)
    {
        vipLevel = vipInfo.level;
        vipState = vipInfo.state;
    }

}

//playerinfoPanel展示时的数据
public class PlayerInfoData
{
    public UserData userData;
    public UnionData unionData;
    public OnePetInfo petInfo;
    public VIPInfo vipInfo;
    public int lastActiveTime;
    public string userUid;

    public PlayerInfoData(UserData _userData, UnionData _unionData, OnePetInfo _petInfo, VIPInfo _vipInfo, int _lastActiveTime, string _userUid)
    {
        userData = _userData;
        unionData = _unionData;
        petInfo = _petInfo;
        vipInfo = _vipInfo;
        lastActiveTime = _lastActiveTime;
        userUid = _userUid;
    }

}

//public class UserDataProxy : TSingletonHotfix<UserDataProxy>, IDataModelProx
//{
//    private PlayerData _playerData;

//    public Response_User_Data resp_UserDataObj;

//    public PlayerData playerData
//    {
//        get
//        {
//            return _playerData;
//        }
//        private set
//        {
//            _playerData = value;
//        }
//    }

//    public void Init()
//    {
//        NetworkEvent.SetCallback(MsgType.Response_User_Data_Cmd,
//        (successResp) =>
//        {
//            resp_UserDataObj = (Response_User_Data)successResp;
//            GetUserDataResp(resp_UserDataObj);
//        },
//        (failedResp) =>
//        {
//            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("获取玩家数据失败"));
//        });


//        NetworkEvent.SetCallback(MsgType.Response_User_DataChange_Cmd,
//        (successResp) =>
//        {
//            Response_User_DataChange resp_UserDataChangeObj = (Response_User_DataChange)successResp;

//            GetUserDataChangeResp(resp_UserDataChangeObj);

//        },
//        (failedResp) =>
//        {
//            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("获取玩家数据失败"));
//        });

//        playerData = new PlayerData();
//    }

//    private void GetUserDataResp(Response_User_Data data)
//    {
//        if (data.errorCode == (int)EErrorCode.EEC_Success)
//        {
//            FGUI.inst.StartExcessAnimation(true, false, () =>
//            {
//                initGameData(data.userData, data.userExtData, data.unionData);
//                //进入游戏界面
//                GameStateEvent.inst.changeState(new StateTransition(kGameState.Shop, false));
//                //GuideManager.inst.proceedGuide(false);
//                GuideManager.inst.waitStart = true;
//            });
//        }
//    }

//    private void initGameData(UserData userdata, UserExtData extdata, UnionData unionData)
//    {
//        playerData.InitItemData(userdata, unionData, extdata.freeNameCount);
//        ShopkeeperDataProxy.inst.curGender = (EGender)userdata.gender;
//        EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.INITCLOTHE);
//    }

//    private void GetUserDataChangeResp(Response_User_DataChange data)
//    {
//        //EUserDataChangeType
//        switch (data.dataType)
//        {
//            //Exp
//            case 1:
//                playerData.CurrExp = data.newValue;
//                break;
//            //Level
//            case 2:
//                LevelUp((uint)data.newValue);
//                break;
//            //Gold
//            case 3:
//                if (data.changeValue > 0)
//                {
//                    AudioManager.inst.PlaySound(33);
//                    EventController.inst.TriggerEvent(GameEventType.GOLD_FLY, data.newValue);
//                }
//                else
//                {
//                    AudioManager.inst.PlaySound(34);
//                }
//                EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.GOLDNUM_ADD, playerData.gold, data.newValue);
//                playerData.gold = data.newValue;
//                break;
//            //Gem
//            case 4:
//                EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.GEMNUM_ADD, playerData.gem, data.newValue);
//                playerData.gem = data.newValue;
//                break;
//            case 5:
//                if (data.newValue > playerData.energy)
//                {
//                    AudioManager.inst.PlaySound(31);
//                    EventController.inst.TriggerEvent(GameEventType.ENERGY_FLY, data.newValue - playerData.energy);
//                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.ENERGYNUM_ADD, playerData.energy, data.newValue);
//                }
//                else
//                {
//                    AudioManager.inst.PlaySound(32);
//                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.ENERGYNUM_REDUCE, playerData.energy, data.newValue);
//                }
//                playerData.energy = data.newValue;

//                break;
//            case 6:     //能量上限
//                playerData.energyLimit = data.newValue;
//                break;
//            case 7:  //图纸数量
//                playerData.drawing = data.newValue;
//                ItemBagProxy.inst.updateItemNum(StaticConstants.drawingID, playerData.drawing);
//                break;
//            case (int)EUserDataChangeType.BagLimit:
//                playerData.bagLimit = data.newValue;
//                break;
//            case (int)EUserDataChangeType.PileLimit:
//                playerData.pileLimit = data.newValue;
//                break;
//            default:
//                break;
//        }
//        EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_DATA_UPDATE);
//    }

//    //升级
//    public void LevelUp(uint level)
//    {
//        playerData.level = level;
//        playerData.MaxExp = ShopkeeperUpconfigManager.inst.GetConfig(level < 99 ? level + 1 : 99).experience;
//         PlatformManager.inst.GameHandleEventLog("Level_" + level, "");
//        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new PopUIInfoBase { type = ReceiveInfoUIType.ShopperLvUp });
//    }


//    public void Clear()
//    {

//    }
//}
