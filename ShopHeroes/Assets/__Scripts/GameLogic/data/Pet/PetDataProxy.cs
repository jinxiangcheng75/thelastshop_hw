using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetDataProxy : TSingletonHotfix<PetDataProxy>, IDataModelProx
{
    private Dictionary<int, PetData> petDic;
    private Dictionary<int, bool> pethouseFeedTipsDic;

    private List<OneUserPetInfo> canChangePetIds;

    public List<OneUserPetInfo> CanChangePetIds
    {
        get
        {
            return canChangePetIds;
        }
    }

    private int boothNum;

    public int BoothNum
    {
        get
        {
            return boothNum;
        }
    }


    public void Init()
    {
        petDic = new Dictionary<int, PetData>();
        pethouseFeedTipsDic = new Dictionary<int, bool>();
        canChangePetIds = new List<OneUserPetInfo>();
        addPetNetListner();

        //测试
        //canChangePetIds = new List<int>() { 1,3 };
        //updatePetItemInfo(new OnePetInfo() {  furnitureUid = 150003 , petId = 1 , petUid = 1, petLevel = 1, petNextFeedTime = 999999});
    }


    void addPetNetListner()
    {
        Helper.AddNetworkRespListener(MsgType.Response_User_PetInfo_Cmd, getPetInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_PetChangeName_Cmd, getPetChangeNameResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_PetFeed_Cmd, getFeedPetInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_PetInfoUpdate_Cmd, getPetUpdateInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_BuyPetSlot_Cmd, getBuyPetSlotInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_BuyPet_Cmd, getBuyPetBreedInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_SetPetSlot_Cmd, getPetChangeDressInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_User_SetMainPet_Cmd, getSetMainPetInfoResp);

    }

    public List<PetData> GetAllPetDatas()
    {
        return petDic.Values.ToList();
    }

    public List<PetData> GetHasPetDatas()//获取有宠物数据的数据列表 
    {
        if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.shopDesignMode == DesignMode.LookPetHouse)
        {
            return petDic.Values.ToList().FindAll(t => t.petInfo.petId != 0 && t.petInfo.petUid != 0 && t.petInfo.petState != (int)EPetState.Store);
        }
        else
        {
            return petDic.Values.ToList().FindAll(t => t.petInfo.petId != 0 && t.petInfo.petUid != 0);
        }
    }

    public List<PetData> GetNotStorePetDatas()
    {
        return GetAllPetDatas().FindAll(t => t.petInfo.petState != (int)EPetState.Store);
    }

    public PetData GetPetDataByPetUid(int petUid)
    {
        if (petDic.ContainsKey(petUid))
        {
            return petDic[petUid];
        }
        return null;
    }

    public PetData GetPetDataByFurnitureUid(int furnitureUid)
    {
        foreach (var item in petDic.Values)
        {
            if (item.petInfo.furnitureUid == furnitureUid)
            {
                return item;
            }
        }

        return null;
    }

    public bool GetPethouseNeedFeedTips(int petUid)
    {
        if (pethouseFeedTipsDic.ContainsKey(petUid))
        {
            return pethouseFeedTipsDic[petUid];
        }

        return false;
    }

    public void SetPethouseNeedFeedTipsFlag(int petUid, bool flag)
    {
        if (pethouseFeedTipsDic.ContainsKey(petUid))
        {
            pethouseFeedTipsDic[petUid] = flag;
        }
    }


    void updatePetItemInfo(OnePetInfo petInfo)
    {
        if (petDic.ContainsKey(petInfo.petUid))
        {
            petDic[petInfo.petUid].SetData(petInfo);
        }
        else
        {
            petDic[petInfo.petUid] = new PetData(petInfo);
        }

    }



    void getPetInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_PetInfo;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        if (petDic.Count > 0) petDic.Clear();

        boothNum = data.petCount;
        canChangePetIds = data.petInfoList;

        foreach (var item in data.petList)
        {
            updatePetItemInfo(item);
            pethouseFeedTipsDic[item.petUid] = item.petNextFeedTime <= 0;
        }

    }

    void getFeedPetInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_PetFeed;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            var petData = GetPetDataByPetUid(data.petInfo.petUid);
            AudioManager.inst.PlaySound(petData.petCfg.pet_music_id);
            int lastLevel = petData.petInfo.petLevel;
            int curLevel = data.petInfo.petLevel;

            updatePetItemInfo(data.petInfo);

            if (lastLevel < curLevel) //升级了
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_PetLevelUpUI", petData);
            }

            HotfixBridge.inst.TriggerLuaEvent("HideUI_FeedPetUI");
            HotfixBridge.inst.TriggerLuaEvent("onPetDataChged", data.petInfo.petUid);
        }
    }


    void getPetChangeNameResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_PetChangeName;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        updatePetItemInfo(data.petInfo);
        AudioManager.inst.PlaySound(13);
        HotfixBridge.inst.TriggerLuaEvent("onPetDataChged", data.petInfo.petUid);
    }


    void getPetUpdateInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_PetInfoUpdate;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        updatePetItemInfo(data.petInfo);
        pethouseFeedTipsDic[data.petInfo.petUid] = data.petInfo.petNextFeedTime <= 0;

        EventController.inst.TriggerEvent(GameEventType.PetCompEvent.PET_ONDATACHANGE, data.petInfo.petUid);

        HotfixBridge.inst.TriggerLuaEvent("onPetDataChged", data.petInfo.petUid);
    }

    void getBuyPetSlotInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_BuyPetSlot;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        boothNum = data.petCount;

        updatePetItemInfo(data.petInfo);

        HotfixBridge.inst.TriggerLuaEvent("onPetSlotsDataChged");

    }

    void getBuyPetBreedInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_BuyPet;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        canChangePetIds = data.petInfoList;

        HotfixBridge.inst.TriggerLuaEvent("onPetBreedsDataChged");
    }

    void getPetChangeDressInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_SetPetSlot;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        if (data.createFlag == 1) //创建宠物
        {
            if (data.furnitureInfo.state == (int)EDesignState.InStore)
            {
                HotfixBridge.inst.TriggerLuaEvent("HideUI_CreateNewPetUI");
                HotfixBridge.inst.TriggerLuaEvent("HideUI_MainPetUI");

                var fData = UserDataProxy.inst.GetFuriture(data.furnitureInfo.furnitureUid);

                if (fData != null)
                {
                    FurnitureDisplayData displayData = new FurnitureDisplayData() { cfg = fData.config, uid = fData.uid, id = fData.id, storeNum = 0 /* 1 ?? */ };

                    EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Create_Furniture, displayData);
                    HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Create_Furniture", displayData);
                }
            }
        }
        else //更换品种
        {
            HotfixBridge.inst.TriggerLuaEvent("HideUI_ChangePetDressUpUI");
            HotfixBridge.inst.TriggerLuaEvent("HideUI_PetSettingUI");
            HotfixBridge.inst.TriggerLuaEvent("HideUI_PetDetailUI");
            HotfixBridge.inst.TriggerLuaEvent("onPetSlotsDataChged");


            if (data.furnitureInfo.state != (int)EDesignState.InStore)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_PetHouseDesignUI", GetPetDataByFurnitureUid(data.furnitureInfo.furnitureUid)); //刷新UI

                if (IndoorMapEditSys.inst != null && IndoorMap.inst != null)
                {
                    if (IndoorMap.inst.GetFurnituresByUid(data.furnitureInfo.furnitureUid, out Furniture furniture))
                    {
                        IndoorMapEditSys.inst.ShowPethouseCharacter(furniture, false);
                    }
                }
            }
        }
    }

    void getSetMainPetInfoResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_User_SetMainPet;

        if (data.errorCode != (int)EErrorCode.EEC_Success) return;

        UserDataProxy.inst.playerData.mainPetUid = data.mainPetUid;

        HotfixBridge.inst.TriggerLuaEvent("onMainPetUidChged");
    }


    public void Clear()
    {
        if (petDic != null) petDic.Clear();
        petDic = null;
    }


}
