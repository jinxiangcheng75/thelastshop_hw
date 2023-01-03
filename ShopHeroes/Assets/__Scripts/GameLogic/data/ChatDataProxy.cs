using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChatData
{
    public int index;
    public string userId;
    public int level;
    public string nickName;
    public string content;
    public bool isSelf;
    public int chatTime;
    public int gender;
    public RoleDress roleDress;
    public ChatData(ChatContent data, int _servertime)
    {
        index = data.chatIndex;
        userId = data.userId;
        level = data.level;
        nickName = data.nickName;
        content = data.content;
        isSelf = AccountDataProxy.inst.userId == userId;
        chatTime = (int)GameTimer.inst.serverNow - (_servertime - data.chatTime);
        gender = data.gender;
        roleDress = data.roleDress;
    }

}
public class ChatDataProxy : TSingletonHotfix<ChatDataProxy>, IDataModelProx
{
    public int chatIndex_world = 0;
    public int chatIndex_union = 0;
    public int chatIndex_sysmsg = 0;
    public bool hasRedTip = false;
    public bool isBanChat = false;
    public int newChatType = -1; // 0 - wolrd 1 - union 2 - system
    public List<ChatData> worldChannelList = new List<ChatData>();
    public List<ChatData> unionChannelList = new List<ChatData>();
    public List<ChatData> sysMsgChannelList = new List<ChatData>();

    public Dictionary<string, GraphicDressUpSystem> chatHeadPool;
    public void Clear()
    {
        chatIndex_world = 0;
        chatIndex_union = 0;
        chatIndex_sysmsg = 0;
        worldChannelList.Clear();
        unionChannelList.Clear();
        sysMsgChannelList.Clear();
        chatHeadPool.Clear();
        hasRedTip = false;
    }
    public void Init()
    {
        chatIndex_world = 0;
        chatIndex_union = 0;
        chatHeadPool = new Dictionary<string, GraphicDressUpSystem>();
        Helper.AddNetworkRespListener(MsgType.Response_Chat_Data_Cmd, OnResponseChatData);
    }

    public List<ChatData> GetChatList(int channel)
    {
        switch (channel)
        {
            case 0:
                //setGraphicDressDic(worldChannelList, channel);
                return worldChannelList;
            case 2:
                //setGraphicDressDic(unionChannelList, channel);
                return unionChannelList;
            case 3:
                return sysMsgChannelList;
            default:
                return null;
        }
    }

    public void setGraphicDressDic(List<ChatData> list, int channel)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int index = i;
            if (!chatHeadPool.ContainsKey(list[index].index + "_" + channel))
            {
                CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)list[index].gender), SpineUtils.RoleDressToHeadDressIdList(list[index].roleDress), (EGender)list[index].gender, callback: (system) =>
                {
                    system.gameObject.name = list[index].index + "_" + channel;
                    system.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
                    chatHeadPool.Add(list[index].index + "_" + channel, system);
                });
            }
        }
    }

    public GraphicDressUpSystem getGraphicDressByIndexAndChannel(int chatIndex, int channel)
    {
        if (chatHeadPool.ContainsKey(chatIndex + "_" + channel))
        {
            return chatHeadPool[chatIndex + "_" + channel];
        }

        Logger.error("输出 没有chatIndex是" + chatIndex + "       channel是" + channel + "的graphicDressUp");
        return null;
    }

    public void removeChatHeadPool(int chatIndex, int channel)
    {
        if (chatHeadPool.ContainsKey(chatIndex + "_" + channel))
        {
            GameObject.Destroy(chatHeadPool[chatIndex + "_" + channel].gameObject);
            chatHeadPool.Remove(chatIndex + "_" + channel);
        }
    }

    public void clearChatHeadPool()
    {
        foreach (var item in chatHeadPool.Values)
        {
            if (item != null)
            {
                GameObject.Destroy(item.gameObject);
            }
        }

        chatHeadPool.Clear();
    }
    //

    public void setWorldList(Response_Chat_Data data)
    {
        foreach (var item in data.chatList)
        {
            if (worldChannelList.FindIndex(i => i.index == item.chatIndex) < 0)
                worldChannelList.Add(new ChatData(item, data.serverTime));
        }
        if (worldChannelList.Count > 0)
        {
            string ids = SaveManager.inst.GetString("Chat_BlockUserIds");
            var infos = ids.Split('|');
            worldChannelList.RemoveAll(t => infos.Contains(t.userId));

            worldChannelList.Sort((item1, item2) => item2.index.CompareTo(item1.index));
            if (worldChannelList.Count > 0)
                chatIndex_world = worldChannelList[0].index;
            //超过100条 截取最新的
            if (worldChannelList.Count > 100)
            {
                //for (int i = 100; i < worldChannelList.Count; i++)
                //{
                //    int index = i;
                //    removeChatHeadPool(worldChannelList[index].index, 0);
                //}
                worldChannelList.RemoveRange(100, worldChannelList.Count - 100);
            }
        }
    }

    public void setUnionList(Response_Chat_Data data)
    {
        foreach (var item in data.chatList)
        {
            if (unionChannelList.FindIndex(i => i.index == item.chatIndex) < 0)
                unionChannelList.Add(new ChatData(item, data.serverTime));
        }
        if (unionChannelList.Count > 0)
        {
            string ids = SaveManager.inst.GetString("Chat_BlockUserIds");
            var infos = ids.Split('|');
            unionChannelList.RemoveAll(t => infos.Contains(t.userId));

            unionChannelList.Sort((item1, item2) => item2.index.CompareTo(item1.index));
            if (unionChannelList.Count > 0)
                chatIndex_union = unionChannelList[0].index;
            //超过100条 截取最新的
            if (unionChannelList.Count > 100)
            {
                //for (int i = 100; i < unionChannelList.Count; i++)
                //{
                //    int index = i;
                //    removeChatHeadPool(unionChannelList[index].index, 2);
                //}
                unionChannelList.RemoveRange(100, unionChannelList.Count - 100);
            }
        }
    }

    public void setRaceLampList(Response_Chat_Data data)
    {
        data.chatList.Sort((a, b) => a.chatIndex.CompareTo(b.chatIndex));

        foreach (var item in data.chatList)
        {
            string[] arr = item.content.Split('|');

            if (arr.Length >= 2)
            {
                int.TryParse(arr[0], out int type);
                int.TryParse(arr[1], out int itemId);
                HotfixBridge.inst.TriggerLuaEvent("AddRaceLampTip", type, itemId, item.nickName, arr.Length > 2 ? arr[2] : "");
            }
        }
    }

    public void setSystemList(Response_Chat_Data data)
    {
        foreach (var item in data.chatList)
        {
            if (sysMsgChannelList.FindIndex(i => i.index == item.chatIndex) < 0)
                sysMsgChannelList.Add(new ChatData(item, data.serverTime));
        }
        if (sysMsgChannelList.Count > 0)
        {
            sysMsgChannelList.Sort((item1, item2) => item2.index.CompareTo(item1.index));
            chatIndex_sysmsg = sysMsgChannelList[0].index;
            //超过100条 截取最新的
            if (sysMsgChannelList.Count > 100)
            {
                sysMsgChannelList.RemoveRange(100, sysMsgChannelList.Count - 100);
            }
        }
    }

    public List<ChatData> GetLookBackDataList()
    {
        List<ChatData> getList = new List<ChatData>();

        if (sysMsgChannelList.Count > 0)
        {
            int maxCount = sysMsgChannelList.Count >= 10 ? 10 : sysMsgChannelList.Count;
            getList.AddRange(sysMsgChannelList.GetRange(0, maxCount));
        }

        return getList;
    }

    void OnResponseChatData(HttpMsgRspdBase msg)
    {
        var data = (Response_Chat_Data)msg;

        if (data.errorCode == 105) //禁言标记
        {
            isBanChat = true;
        }
        else
        {
            isBanChat = false;
        }
        Logger.log($"收到聊天{data.chatList.Count}条");
        //  if (data.chatList.Count <= 0) return;
        switch (data.chatChannel)
        {
            case (int)EChatChannel.World:
                {
                    if (data.chatList.Count > 0)
                    {
                        int world_Index = SaveManager.inst.GetInt("chat_world");
                        if (world_Index != data.chatList[data.chatList.Count - 1].chatIndex)
                        {
                            newChatType = 0;
                            SaveManager.inst.SaveInt("chat_world", data.chatList[data.chatList.Count - 1].chatIndex);
                        }
                    }

                    setWorldList(data);
                }
                break;
            case (int)EChatChannel.Union:
                {
                    if (data.chatList.Count > 0)
                    {
                        int union_Index = SaveManager.inst.GetInt("chat_union");
                        if (union_Index != data.chatList[data.chatList.Count - 1].chatIndex)
                        {
                            newChatType = 1;
                            SaveManager.inst.SaveInt("chat_union", data.chatList[data.chatList.Count - 1].chatIndex);
                        }
                    }

                    setUnionList(data);
                }
                break;
            case (int)EChatChannel.RaceLamp: //全服通知 跑马灯
                {
                    setRaceLampList(data);
                }
                break;
            case (int)EChatChannel.System: //消息
                {
                    if (data.chatList.Count > 0)
                    {
                        int system_Index = SaveManager.inst.GetInt("chat_system");
                        if (system_Index != data.chatList[data.chatList.Count - 1].chatIndex)
                        {
                            newChatType = 2;
                            SaveManager.inst.SaveInt("chat_system", data.chatList[data.chatList.Count - 1].chatIndex);
                        }
                    }

                    setSystemList(data);
                }
                break;
        }
        EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_VIEW_UPDATE);
    }
}
