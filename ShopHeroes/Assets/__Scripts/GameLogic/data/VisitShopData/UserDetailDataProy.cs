using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//其他玩家基本数据
public class UserIndoorData
{
    public string userUId;
    public UserData userData;
    public GuildData guildData;
    public ShopData shopData;
    public FloorData floorData;
    public WallData wallData;
    public List<OneFurniture> furnitureList;
}
//其他玩家数据
public class UserDetailDataProy : TSingletonHotfix<UserDetailDataProy>, IDataModelProx
{
    public UserIndoorData currVisitUserData;    //当前所拜访玩家的数据
    public void Clear()
    {
        currVisitUserData = null;
    }

    public void Init()
    {
        currVisitUserData = new UserIndoorData();

        Helper.AddNetworkRespListener(MsgType.Response_User_Detail_Cmd, OnResponseUserDetail);
    }

    void OnResponseUserDetail(HttpMsgRspdBase msg)
    {
        var data = (Response_User_Detail)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            currVisitUserData.userUId = data.userId;
            currVisitUserData.userData = data.userData;
            currVisitUserData.guildData = data.guildData;
            currVisitUserData.shopData = data.shopData;
            currVisitUserData.floorData = data.floorData;
            currVisitUserData.wallData = data.wallData;
            currVisitUserData.furnitureList = data.furnitureList;
        }
        else
        {
            Logger.error("Response_User_Detail error:" + data.message);
        }
    }

}
