using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailSystem : BaseSystem
{
    EmailUI _emailUI;               //邮件主界面
    FeedbackUI _feedbackUI;         //反馈问题
    EmailDetailsUI _emailDetailsUI; //邮件详情

    float timingTime;
    float request_timingTime = 30f; //30秒定期请求邮件数据

    int mailIdMax = 0; //邮件最大id

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.EmailEvent.SHOWUI_EmailMainUI, showEmailMainUI);
        EventController.inst.AddListener(GameEventType.EmailEvent.SHOWUI_FeedbackUI, showFeedbackUI);
        EventController.inst.AddListener<EmailData>(GameEventType.EmailEvent.SHOWUI_EmailDetailsUI, showEmailDetailsUI);


        //----------------------------------------------------------------------------------------------------------------------------
        EventController.inst.AddListener(GameEventType.EmailEvent.EMAIL_REQUEST_DATA, request_emailData);
        EventController.inst.AddListener(GameEventType.EmailEvent.EMAIL_REQUEST_ALLREAD, request_emailAllRead);
        EventController.inst.AddListener<int>(GameEventType.EmailEvent.EMAIL_REQUEST_SINGLEREAD, request_emailSingleRead);
        EventController.inst.AddListener<int>(GameEventType.EmailEvent.EMAIL_REQUEST_CLAIMED, request_emailClaimed);
        EventController.inst.AddListener<int>(GameEventType.EmailEvent.EMAIL_REQUEST_SINGLEDEL, request_emailSingleDel);

        //--反馈--
        EventController.inst.AddListener<string>(GameEventType.EmailEvent.EMAIL_REQUEST_FEEDBACK, request_feedback);

    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.EmailEvent.SHOWUI_EmailMainUI, showEmailMainUI);
        EventController.inst.RemoveListener(GameEventType.EmailEvent.SHOWUI_FeedbackUI, showFeedbackUI);
        EventController.inst.RemoveListener<EmailData>(GameEventType.EmailEvent.SHOWUI_EmailDetailsUI, showEmailDetailsUI);


        //----------------------------------------------------------------------------------------------------------------------------
        EventController.inst.RemoveListener(GameEventType.EmailEvent.EMAIL_REQUEST_DATA, request_emailData);
        EventController.inst.RemoveListener(GameEventType.EmailEvent.EMAIL_REQUEST_ALLREAD, request_emailAllRead);
        EventController.inst.RemoveListener<int>(GameEventType.EmailEvent.EMAIL_REQUEST_SINGLEREAD, request_emailSingleRead);
        EventController.inst.RemoveListener<int>(GameEventType.EmailEvent.EMAIL_REQUEST_CLAIMED, request_emailClaimed);
        EventController.inst.RemoveListener<int>(GameEventType.EmailEvent.EMAIL_REQUEST_SINGLEDEL, request_emailSingleDel);

        //--反馈--
        EventController.inst.RemoveListener<string>(GameEventType.EmailEvent.EMAIL_REQUEST_FEEDBACK, request_feedback);


    }

    public override void ReInitSystem()
    {
        base.ReInitSystem();
        mailIdMax = 0;
    }

    protected override void OnInit()
    {
        Helper.AddNetworkRespListener(MsgType.Response_Mail_List_Cmd, getEmailDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Mail_Read_Cmd, getEmailAlreadyRead);
        Helper.AddNetworkRespListener(MsgType.Response_Mail_Delete_Cmd, getSingleEmailDelResp);
        Helper.AddNetworkRespListener(MsgType.Response_Mail_Claimed_Cmd, getemailClaimedDataResp);

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    void showEmailMainUI()
    {
        GUIManager.OpenView<EmailUI>();
    }

    void showFeedbackUI()
    {
        GUIManager.OpenView<FeedbackUI>();
    }

    void showEmailDetailsUI(EmailData data)
    {
        _emailDetailsUI = GUIManager.OpenView<EmailDetailsUI>(view =>
         {
             view.SetData(data);
         });
    }

    //void timingRequest() //定时请求
    //{
    //    if (ManagerBinder.inst.mGameState == kGameState.Shop)  //在商店内刷新邮件
    //    {
    //        timingTime += 1;
    //        if (timingTime >= request_timingTime)
    //        {
    //            timingTime = 0;
    //            request_emailData();
    //        }
    //    }
    //}

    //void feedbackTiming() //定时反馈问题
    //{
    //    if (EmailDataProxy.inst.sendFeedbackTimingTime > 0)
    //        EmailDataProxy.inst.sendFeedbackTimingTime -= 1;
    //}

    void request_emailData()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Mail_List()
                {
                    mailId = mailIdMax,
                }
            });
        }
    }

    void request_emailAllRead()
    {

        //一键领取
        if (EmailDataProxy.inst.needShowRedPoint)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Mail_Claimed()
                {
                    mailId = 0,
                }
            });
        }
        else
        {

        }


    }

    void getEmailDataResp(HttpMsgRspdBase msg)
    {
        Response_Mail_List data = msg as Response_Mail_List;

        EmailDataProxy.inst.Clear();

        if (data.mailList.Count > 0)
        {
            mailIdMax = 0;

            foreach (OneMail item in data.mailList)
            {
                EmailDataProxy.inst.UpdateEmailData(item);

                mailIdMax = Mathf.Max(mailIdMax, item.mailId);
            }
        }

    }

    //邮件标记为已读
    void request_emailSingleRead(int mailId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Mail_Read()
            {
                mailId = mailId,
            }
        });
    }

    void getEmailAlreadyRead(HttpMsgRspdBase msg)
    {
        Response_Mail_Read data = msg as Response_Mail_Read;
        EmailData email = EmailDataProxy.inst.GetEmailByID(data.mailId);

        if (email != null) email.state = (int)EMailStatus.Read;

        var emailUI = GUIManager.GetWindow<EmailUI>();
        if (emailUI != null && emailUI.isShowing) emailUI.Refresh();

    }

    //领取附件邮件
    void request_emailClaimed(int mailId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Mail_Claimed()
            {
                mailId = mailId,
            }
        });
    }

    void getemailClaimedDataResp(HttpMsgRspdBase msg)
    {
        Response_Mail_Claimed data = msg as Response_Mail_Claimed;


        foreach (OneMail info in data.mailList)
        {
            EmailData email = EmailDataProxy.inst.GetEmailByID(info.mailId);

            if (email != null) email.SetInfo(info);

            //if (_emailDetailsUI != null && _emailDetailsUI.isShowing) _emailDetailsUI.NeedRefresh(info.mailId);
            GUIManager.HideView<EmailDetailsUI>();
        }

        var emailUI = GUIManager.GetWindow<EmailUI>();

        if (emailUI != null && emailUI.isShowing) emailUI.Refresh();

        //处理奖励

        if (data.itemList != null)
        {
            if (data.itemList.Count == 1)
            {
                UserDataProxy.inst.DealWithAward(new AccessoryData(data.itemList[0]));
            }
            else if (data.itemList.Count > 1)
            {
                List<CommonRewardData> commonList = new List<CommonRewardData>();
                foreach (var item in data.itemList)
                {
                    CommonRewardData tempData = new CommonRewardData(item.itemId, item.count, item.quality, item.itemType);
                    commonList.Add(tempData);
                }

                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = commonList });
            }
        }
        //foreach (OneRewardItem item in data.itemList)
        //{
        //    dealWithAward(new AccessoryData(item));
        //}


    }

    void request_emailSingleDel(int mailId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Mail_Delete()
            {
                mailId = mailId,
            }
        });
    }

    void getSingleEmailDelResp(HttpMsgRspdBase msg)
    {
        Response_Mail_Delete data = msg as Response_Mail_Delete;

        if (EmailDataProxy.inst.DelEmailById(data.mailId))
        {
            var emailUI = GUIManager.GetWindow<EmailUI>();
            if (emailUI != null && emailUI.isShowing) emailUI.Refresh();

            var emailDetailsUI = GUIManager.GetWindow<EmailDetailsUI>();
            if (emailDetailsUI != null && emailDetailsUI.isShowing) emailDetailsUI.NeedHide(data.mailId);

        }
        else
        {
            Logger.error("本地无此邮件,, mailId ： " + data.mailId);
        }

    }

    //反馈
    void request_feedback(string content)
    {

        //添加时间间隔
        EmailDataProxy.inst.sendFeedbackTimingTime = (int)GameTimer.inst.serverNow + (int)WorldParConfigManager.inst.GetConfig(120).parameters;

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Mail_Feedback()
            {
#if UNITY_IPHONE || UNITY_IOS
                osType = (int)EOsType.Ios,
#elif UNITY_ANDROID || UNITY_EDITOR
                osType = (int)EOsType.Android,
#endif
                ver = GameSettingManager.appVersion,
                feedback = content,
            }
        });
    }

}
