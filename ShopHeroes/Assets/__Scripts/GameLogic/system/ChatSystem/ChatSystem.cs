using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSystem : BaseSystem
{
    ChatMsgView chatMsgView;

    int chatChannel = 0;

    int updateChatListTimerid = 0;
    protected override void OnInit()
    {

    }
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.ChatSysEvent.CHAT_SHOWVIEW, showChatView);
        EventController.inst.AddListener<string, int>(GameEventType.ChatSysEvent.CHAT_SENDMSG, SendChat);
        EventController.inst.AddListener(GameEventType.ChatSysEvent.CHAT_VIEW_UPDATE, OnChatDataUpdate);
        EventController.inst.AddListener<int>(GameEventType.ChatSysEvent.CHAT_CHANNEL_CHANGE, ChannelChange);
        EventController.inst.AddListener(GameEventType.ChatSysEvent.CHAT_UPDATE_NewChatData, haveNewChatData);

        EventController.inst.AddListener(GameEventType.ChatSysEvent.CHAT_REQUEST_DATA, GetChatListData);
        // updateChatListTimerid = GameTimer.inst.AddTimer(5, GetChatListData); //5秒获取一次聊天记录

        Helper.AddNetworkRespListener(MsgType.Response_Chat_Send_Cmd, onResponseChatSend);
    }
    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.ChatSysEvent.CHAT_SHOWVIEW, showChatView);
        EventController.inst.RemoveListener<string, int>(GameEventType.ChatSysEvent.CHAT_SENDMSG, SendChat);
        EventController.inst.RemoveListener(GameEventType.ChatSysEvent.CHAT_VIEW_UPDATE, OnChatDataUpdate);
        EventController.inst.RemoveListener<int>(GameEventType.ChatSysEvent.CHAT_CHANNEL_CHANGE, ChannelChange);
        EventController.inst.RemoveListener(GameEventType.ChatSysEvent.CHAT_UPDATE_NewChatData, haveNewChatData);

        EventController.inst.RemoveListener(GameEventType.ChatSysEvent.CHAT_REQUEST_DATA, GetChatListData);

        GameTimer.inst.RemoveTimer(updateChatListTimerid);
    }



    void showChatView()
    {
        //chatMsgView = GUIManager.OpenView<ChatMsgView>((view) =>
        //{
        //    chatMsgView = view;
        //    ChatDataProxy.inst.hasRedTip = false;
        //    GetChatListData();
        //});
        HotfixBridge.inst.TriggerLuaEvent("OpenChatView");
    }
    void hideChatView()
    {
        GUIManager.HideView<ChatMsgView>();
    }

    public void haveNewChatData()
    {
        ////判断聊天界面是否打开
        //if (chatMsgView != null && chatMsgView.isShowing)
        //{
        //    GetChatListData();
        //    ChatDataProxy.inst.hasRedTip = false;
        //}
        //else
        //{
        //    //只显示红点
        //    ChatDataProxy.inst.hasRedTip = true;
        //}

        ////判断聊天界面是否打开
        if (GUIManager.GetViewIsShowingByViewID("ChatView")) 
        {
            GetChatListData();
            ChatDataProxy.inst.hasRedTip = false;
        }
        else
        {
            //只显示红点
            ChatDataProxy.inst.hasRedTip = true;
        }

    }
    void GetChatListData()
    {
        // if (!chatMsgView.isShowing) return; //界面没有打开不更新
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Chat_Data()
            {
                //chatChannel = (int)EChatChannel.World,
                chatIndex = ChatDataProxy.inst.chatIndex_world,
                unionChatIndex = ChatDataProxy.inst.chatIndex_union,
                systemMsgIndex = ChatDataProxy.inst.chatIndex_sysmsg,
                unionId = UserDataProxy.inst.playerData.unionId
            }
        });
    }
    private void ChannelChange(int channel)
    {
        chatChannel = channel;
        if (chatMsgView != null && chatMsgView.isShowing)
        {
            chatMsgView.UpdateView(ChatDataProxy.inst.GetChatList(chatChannel));
        }
    }

    private void OnChatDataUpdate()
    {
        //if (chatMsgView != null && chatMsgView.isShowing)
        //{
        //    chatChannel = SaveManager.inst.GetInt("chatCurrTable");
        //    if (chatChannel == 0)
        //        chatMsgView.UpdateView(ChatDataProxy.inst.GetChatList(chatChannel));
        //    else if (chatChannel == 2)
        //        chatMsgView.UpdateView(ChatDataProxy.inst.GetChatList(chatChannel));
        //    else if (chatChannel == 3)
        //        chatMsgView.UpdateView(ChatDataProxy.inst.GetChatList(chatChannel));
        //}
        HotfixBridge.inst.TriggerLuaEvent("UpdateChatView");
    }
    void SendChat(string msg, int channel)
    {

        if (string.IsNullOrEmpty(msg)) return;

        if (channel == 0)
        {
            var spaceTime = SaveManager.inst.GetInt("ChatSpace");
            if (spaceTime != 0)
            {
                if (spaceTime > GameTimer.inst.serverNow)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您的发言太频繁了，请稍后尝试！"), GUIHelper.GetColorByColorHex("Fe4747"));
                    return;
                }
            }

            double timer = 30;
            var cfg = WorldParConfigManager.inst.GetConfig(353);
            if (cfg != null)
            {
                timer = cfg.parameters;
            }

            SaveManager.inst.SaveInt("ChatSpace", (int)(GameTimer.inst.serverNow + timer));
        }

        //屏蔽字库
        string str;
        if (WordFilter.inst.filter(msg, out str))
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("发送内容中包含敏感内容"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Chat_Send()
            {
                chatChannel = channel,
                content = str
            }
        });
    }

    public void onResponseChatSend(HttpMsgRspdBase msg)
    {
        Response_Chat_Send data = (Response_Chat_Send)msg;
        if (data.errorCode == 0)
        {
            GetChatListData();
        }
        else if (data.errorCode == 105)
        {
            EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, LanguageManager.inst.GetValueByKey("您的账号已被禁止发送聊天信息，请联系官方运营团队！"), null);
            return;
        }
    }
}
