using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request_createUnionClientData
{
    public string unionName;
    public EUnionEnter enterSetting;
    public int enterLevel;
    public long enterInvest;
    public int useGem;  //0 新币  1 金条
}

public class UnionDetailInfo
{
    public string unionId = "";
    public string unionName = "";
    public int unionLevel = 0;
    public string presidentUserId = "";
    public string presidentNickName = "";
    public int enterSetting = 0; //enum EUnionEnter
    public int enterLevel = 0;
    public long enterInvest = 0;
    public int memberNumLimit;
    public List<UnionMemberInfo> memberList = new List<UnionMemberInfo>();

    public void SetInfo(UnionDetailData data)
    {
        this.unionId = data.unionId;
        this.unionName = data.unionName;
        this.unionLevel = data.unionLevel;
        this.presidentUserId = data.presidentUserId;
        this.presidentNickName = data.presidentNickName;
        this.enterSetting = data.enterSetting;
        this.enterLevel = data.enterLevel;
        this.enterInvest = data.enterInvest;
        this.memberNumLimit = data.memberNumLimit;

        this.memberList.Clear();
        foreach (var item in data.memberList)
        {
            UnionMemberInfo info = new UnionMemberInfo();
            info.SetInfo(item);
            this.memberList.Add(info);
        }

        //排个序
        this.memberList.Sort((a, b) =>
        {
            if (a.memberJob > b.memberJob)
            {
                return -1;
            }
            else if (a.memberJob < b.memberJob)
            {
                return 1;
            }
            else
            {
                if (a.invest > b.invest)
                {
                    return -1;
                }
                else if (a.invest < b.invest)
                {
                    return 1;
                }
                else
                {
                    return -a.level.CompareTo(b.level);
                }
            }
        });
    }

}

public class UnionMemberInfo
{
    public string userId = "";
    string _nickName;
    public string nickName
    {
        get
        {
            if (LanguageManager.inst != null)
            {
                return LanguageManager.inst.GetValueByKey(_nickName);
            }
            return _nickName;
        }
        set
        {
            _nickName = value;
        }
    }
    public int gender = 0;
    public int memberJob = 0; //enum EUnionJob
    public int level = 0;
    public long invest = 0;
    public long worth = 0;
    public RoleDress roleDress = new RoleDress();

    public void SetInfo(UnionMember data)
    {
        this.userId = data.userId;
        this.nickName = data.nickName;
        this.memberJob = data.memberJob;
        this.level = data.level;
        this.gender = data.gender;
        this.roleDress = data.roleDress;
        this.invest = data.invest;
        this.worth = data.worth;
    }

}


/// <summary>
/// 公会系统
/// </summary>
public class UnionSystem : BaseSystem
{
    UnionMainUI _unionMainUI;
    UnionFindToolUI _unionFindToolUI;
    UnionCreateUI _unionCreateUI;
    UnionInfoUI _unionInfoUI;
    MsgBox_ExitUnionUI _msgBox_ExitUnionUI;
    UnionSetSettingUI _unionSetSettingUI;
    UnionMemberSettingUI _unionMemberSettingUI;
    UnionMsgUpdateUI _unionMsgUpdateUI;
    KickOutUnionMemberAffirmUI _kickOutUnionMemberAffirmUI;
    UnionTaskResetUI _unionTaskResetUI;

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_UNIONMAIN, showUnionMainUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.HIDEUI_UNIONMAIN, hideUnionMainUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_UNIONFINDTOOL, showUnionFindToolUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_CREATEUNION, showUnionCreateUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_UNIONINFO, showSelfUnionInfoUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_EXITUNIONMSGBOX, showUnionExitMsgBox);
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_UNIONSETSETTING, showUnionSetSettingUI);
        EventController.inst.AddListener<string, int>(GameEventType.UnionEvent.SHOWUI_UNIONMEMBERSETTING, showUnionMemberSettingUI);
        EventController.inst.AddListener<string>(GameEventType.UnionEvent.SHOWUI_UNIONKICKOUTMEMBERCONFIRM, showUnionKickoutMemberConfirmUI);
        EventController.inst.AddListener<List<UnionMsgData>>(GameEventType.UnionEvent.SHOWUI_UNIONMSGUPDATEUI, showUnionMsgUpdateUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_MEMBERHELPLISTREFRESH, refreshUnionAidRedPointMess);
        EventController.inst.AddListener<RectTransform, UnionBuffData>(GameEventType.UnionEvent.UNION_REQUEST_BUFFDETAIL, showUnionBuffDetail);
        EventController.inst.AddListener<string, string>(GameEventType.UnionEvent.UNION_MSGBOX_ENTER, showUnionEnterBox);
        EventController.inst.AddListener<Response_Union_TaskResult>(GameEventType.UnionEvent.SHOWUI_UNIONTASKRESULT, showUnionTaskResultUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.SHOWUI_UNIONTASKRESET, showUnionTaskReSetUI);
        EventController.inst.AddListener(GameEventType.UnionEvent.ENTER_UNIONSCENE, enterUnionScene);



        //----------------------------------------------------------------------------------------------------------------------------
        EventController.inst.AddListener<string>(GameEventType.UnionEvent.UNION_REQUEST_DATA, request_unionData);
        EventController.inst.AddListener<Request_createUnionClientData>(GameEventType.UnionEvent.UNION_REQUEST_CREATE, request_createUnion);
        EventController.inst.AddListener<string>(GameEventType.UnionEvent.UNION_REQUEST_ENTER, request_enterUnion);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_EXIT, request_exitUnion);
        EventController.inst.AddListener<string>(GameEventType.UnionEvent.UNION_REQUEST_LIST, request_findUnionData);
        EventController.inst.AddListener<string>(GameEventType.UnionEvent.UNION_REQUEST_FINDPLAYERLIST, request_findPlayerList);
        EventController.inst.AddListener<string, int, int, long>(GameEventType.UnionEvent.UNION_REQUEST_SETINFO, request_setUnionInfo);
        EventController.inst.AddListener<string, int>(GameEventType.UnionEvent.UNION_REQUEST_SETMEMBERJOB, request_setUnionMemberJob);
        EventController.inst.AddListener<string>(GameEventType.UnionEvent.UNION_REQUEST_KICKOUTMEMBER, request_kickoutUnionMember);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_MSGINFOREFRESH, request_UnionMessageInfoRefresh);

        //援助
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_MEMBERHELPLIST, request_UnionMemberHelpList);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_SETHELP, request_UnoinSetHelp);
        EventController.inst.AddListener<int, string, int, int>(GameEventType.UnionEvent.UNION_REQUEST_HELPMEMBER, request_UnionHelpMember);

        //悬赏
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_TASKLIST, request_unionTaskList);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_CHECKUNIONTASK, request_checkUnionTask);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_STARTUNIONTASK, request_startUnionTask);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_CANCELUNIONTASK, request_cancelUnionTask);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_REWARDUNIONTASK, request_rewardUnionTask);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_GEMREWARDUNIONTASK, request_gemRewardUnionTask);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_TASKRANKLIST, request_unionTaskRankList);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_TASKRESULT, request_unionTaskResult);

        //科技
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_SCIENCELIST, request_unionScienceList);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_SCIENCEUPGRADE, request_unionScienceUpgrade);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_REQUEST_SKILLLIST, request_unionSkillList);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_USESKILL, request_useUnionSkill);
        EventController.inst.AddListener<int>(GameEventType.UnionEvent.UNION_REQUEST_SKILLREFRESH, request_unionSkillRefresh);


        //心跳
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_HAVENEWTASKDATA, haveNewTaskData);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_HAVENEWHELPDATA, haveNewHelpData);
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_HAVENEWSCIENCEDATA, haveNewScienceData);

    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_UNIONMAIN, showUnionMainUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.HIDEUI_UNIONMAIN, hideUnionMainUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_UNIONFINDTOOL, showUnionFindToolUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_CREATEUNION, showUnionCreateUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_UNIONINFO, showSelfUnionInfoUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_EXITUNIONMSGBOX, showUnionExitMsgBox);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_UNIONSETSETTING, showUnionSetSettingUI);
        EventController.inst.RemoveListener<string, int>(GameEventType.UnionEvent.SHOWUI_UNIONMEMBERSETTING, showUnionMemberSettingUI);
        EventController.inst.RemoveListener<string>(GameEventType.UnionEvent.SHOWUI_UNIONKICKOUTMEMBERCONFIRM, showUnionKickoutMemberConfirmUI);
        EventController.inst.RemoveListener<List<UnionMsgData>>(GameEventType.UnionEvent.SHOWUI_UNIONMSGUPDATEUI, showUnionMsgUpdateUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_MEMBERHELPLISTREFRESH, refreshUnionAidRedPointMess);
        EventController.inst.RemoveListener<RectTransform, UnionBuffData>(GameEventType.UnionEvent.UNION_REQUEST_BUFFDETAIL, showUnionBuffDetail);
        EventController.inst.RemoveListener<string, string>(GameEventType.UnionEvent.UNION_MSGBOX_ENTER, showUnionEnterBox);
        EventController.inst.RemoveListener<Response_Union_TaskResult>(GameEventType.UnionEvent.SHOWUI_UNIONTASKRESULT, showUnionTaskResultUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.SHOWUI_UNIONTASKRESET, showUnionTaskReSetUI);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.ENTER_UNIONSCENE, enterUnionScene);



        //----------------------------------------------------------------------------------------------------------------------------
        EventController.inst.RemoveListener<string>(GameEventType.UnionEvent.UNION_REQUEST_DATA, request_unionData);
        EventController.inst.RemoveListener<Request_createUnionClientData>(GameEventType.UnionEvent.UNION_REQUEST_CREATE, request_createUnion);
        EventController.inst.RemoveListener<string>(GameEventType.UnionEvent.UNION_REQUEST_ENTER, request_enterUnion);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_EXIT, request_exitUnion);
        EventController.inst.RemoveListener<string>(GameEventType.UnionEvent.UNION_REQUEST_LIST, request_findUnionData);
        EventController.inst.RemoveListener<string>(GameEventType.UnionEvent.UNION_REQUEST_FINDPLAYERLIST, request_findPlayerList);
        EventController.inst.RemoveListener<string, int, int, long>(GameEventType.UnionEvent.UNION_REQUEST_SETINFO, request_setUnionInfo);
        EventController.inst.RemoveListener<string, int>(GameEventType.UnionEvent.UNION_REQUEST_SETMEMBERJOB, request_setUnionMemberJob);
        EventController.inst.RemoveListener<string>(GameEventType.UnionEvent.UNION_REQUEST_KICKOUTMEMBER, request_kickoutUnionMember);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_MSGINFOREFRESH, request_UnionMessageInfoRefresh);

        //援助
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_MEMBERHELPLIST, request_UnionMemberHelpList);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_SETHELP, request_UnoinSetHelp);
        EventController.inst.RemoveListener<int, string, int, int>(GameEventType.UnionEvent.UNION_REQUEST_HELPMEMBER, request_UnionHelpMember);

        //悬赏
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_TASKLIST, request_unionTaskList);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_CHECKUNIONTASK, request_checkUnionTask);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_STARTUNIONTASK, request_startUnionTask);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_CANCELUNIONTASK, request_cancelUnionTask);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_REWARDUNIONTASK, request_rewardUnionTask);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_GEMREWARDUNIONTASK, request_gemRewardUnionTask);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_TASKRANKLIST, request_unionTaskRankList);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_TASKRESULT, request_unionTaskResult);


        //科技
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_SCIENCELIST, request_unionScienceList);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_SCIENCEUPGRADE, request_unionScienceUpgrade);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_REQUEST_SKILLLIST, request_unionSkillList);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_USESKILL, request_useUnionSkill);
        EventController.inst.RemoveListener<int>(GameEventType.UnionEvent.UNION_REQUEST_SKILLREFRESH, request_unionSkillRefresh);


        //心跳
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_HAVENEWTASKDATA, haveNewTaskData);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_HAVENEWHELPDATA, haveNewHelpData);
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_HAVENEWSCIENCEDATA, haveNewScienceData);

    }


    //int requestUnionDetailTimingTimer;
    //float unionDetailtimingTime;
    //float request_UnionDetailTimingTime = 30f; //30秒定期请求公会数据


    protected override void OnInit()
    {
        //unionDetailtimingTime = 0;
        //requestUnionDetailTimingTimer = GameTimer.inst.AddTimer(1, timingRequest_UnionDetail);

        Helper.AddNetworkRespListener(MsgType.Response_Union_Data_Cmd, getUnionDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_List_Cmd, getFindUnionDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_UserData_Cmd, getFindPlayerDataResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_Create_Cmd, getCreateUnionResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_Enter_Cmd, getEnterUnionResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_Leave_Cmd, getExitUnionResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_SetInfo_Cmd, getSetUnionInfoResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_SetUserRole_Cmd, getSetUnionMemberJobResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_KickOut_Cmd, getKickoutUnionMemberResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_MessageInfoRefresh_Cmd, getUnionMessageInfoRefreshResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_TaskResult_Cmd, getUnionResultResp);
        Helper.AddNetworkRespListener(MsgType.Response_Union_KickOutMessage_Cmd, getUnionKickOutMessageResp);

    }

    int unionMsgUid = 0;

    //void timingRequest_UnionDetail() //定时请求
    //{
    //    if (ManagerBinder.inst.mGameState == kGameState.Union)
    //    {
    //        unionDetailtimingTime += 1;

    //        if (unionDetailtimingTime >= request_UnionDetailTimingTime)
    //        {
    //            unionDetailtimingTime = 0;
    //            request_unionData("");
    //        }
    //    }
    //}


    private void showUnionFindToolUI()
    {
        GUIManager.OpenView<UnionFindToolUI>();
    }

    private void showUnionCreateUI()
    {
        GUIManager.OpenView<UnionCreateUI>();
    }

    private void showUnionEnterBox(string unionId, string unionName)
    {
        GUIManager.OpenView<MsgBox_EnterUnionUI>((view) =>
        {
            view.SetContent(unionId, unionName);
        });
    }

    private void showUnionTaskResultUI(Response_Union_TaskResult data)
    {
        GUIManager.OpenView<UnionTaskResultUI>((view) =>
        {
            view.SetData(data);
        });
    }

    private void showUnionTaskReSetUI()
    {
        GUIManager.OpenView<UnionTaskResetUI>();
    }

    private void showUnionInfoUI(UnionDetailInfo data)
    {
        GUIManager.OpenView<UnionInfoUI>((view) =>
        {
            view.SetData(data);
        });
    }

    private void showSelfUnionInfoUI()
    {
        if (!UserDataProxy.inst.playerData.hasUnion) //没有公会
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("请先加入一个联盟"), GUIHelper.GetColorByColorHex("FFFFFF"));
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONFINDTOOL);
        }
        else
        {
            showUnionInfoUI(UserDataProxy.inst.unionDetailInfo);
        }
    }

    private void showUnionExitMsgBox()
    {
        GUIManager.OpenView<MsgBox_ExitUnionUI>((view) =>
        {
            view.SetContent();
        });
    }

    private void showUnionSetSettingUI()
    {
        GUIManager.OpenView<UnionSetSettingUI>();
    }

    private void showUnionKickoutMemberConfirmUI(string _userId)
    {
        GUIManager.OpenView<KickOutUnionMemberAffirmUI>(view =>
        {
            view.SetData(_userId);
        });
    }

    private void showUnionMemberSettingUI(string userUid, int memberJob)
    {
        GUIManager.OpenView<UnionMemberSettingUI>(view =>
        {
            view.SetData(userUid, memberJob);
        });
    }

    void showUnionMainUI()
    {
        GUIManager.OpenView<UnionMainUI>();
    }

    void hideUnionMainUI()
    {
        GUIManager.HideView<UnionMainUI>();
    }

    void showUnionMsgUpdateUI(List<UnionMsgData> unionMsgDatas)
    {
        GUIManager.OpenView<UnionMsgUpdateUI>(view =>
        {
            view.SetData(unionMsgDatas);
        });
    }

    void refreshUnionAidRedPointMess()
    {
        var unionMainUI = GUIManager.GetWindow<UnionMainUI>();

        if (unionMainUI != null && unionMainUI.isShowing)
        {
            unionMainUI.RefreshAidRedPointMess();
        }
    }

    void showUnionBuffDetail(RectTransform rect, UnionBuffData buffData)
    {
        var unionMainUI = GUIManager.GetWindow<UnionMainUI>();

        if (unionMainUI != null && unionMainUI.isShowing)
        {
            unionMainUI.ShowBuffDetailInfo(rect, buffData);
        }
    }

    public void enterUnionScene()
    {
        if (!UserDataProxy.inst.playerData.hasUnion) //没有公会
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONINFO);
        }
        else
        {
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Union, true));
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_DATA, "");
        }
    }

    //请求公会详细数据
    private void request_unionData(string unionId)  //留空为请求自身公会数据
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_Data()
                {
                    unionId = unionId
                }
            });
        }
    }

    private void getUnionDataResp(HttpMsgRspdBase msg)
    {
        Response_Union_Data data = msg as Response_Union_Data;

        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            if (data.unionDetailData.unionId == UserDataProxy.inst.playerData.unionId) //为自己公会的
            {

                var memberData = data.unionDetailData.memberList.Find(t => t.userId == UserDataProxy.inst.playerData.userUid);

                if (memberData != null)//自己职位要及时更新
                {
                    UserDataProxy.inst.playerData.memberJob = memberData.memberJob;
                }

                UserDataProxy.inst.unionDetailInfo.SetInfo(data.unionDetailData);
                refreshUnionSceneMembers();

                UnionMainUI unionMainUI = GUIManager.GetWindow<UnionMainUI>();
                if (unionMainUI != null && unionMainUI.isShowing)
                {
                    unionMainUI.SetUnoinDetailInfo();
                }

                //if (!ManagerBinder.inst.stateIsChanging && ManagerBinder.inst.mGameState != kGameState.Union)
                //{
                //    showUnionInfoUI(UserDataProxy.inst.unionDetailInfo);
                //}
            }
            else
            {
                UnionDetailInfo unionDetailInfo = new UnionDetailInfo();
                unionDetailInfo.SetInfo(data.unionDetailData);

                showUnionInfoUI(unionDetailInfo);
            }
        }

    }


    //请求创建公会
    private void request_createUnion(Request_createUnionClientData clientData)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_Create()
                {
                    unionName = clientData.unionName,
                    enterSetting = (int)clientData.enterSetting,
                    enterLevel = clientData.enterLevel,
                    useGem = clientData.useGem,
                    enterInvest = clientData.enterInvest
                }
            });
        }
    }

    private bool getCreateUnionFailResp(HttpMsgRspdBase msg)
    {
        Response_Union_Create data = msg as Response_Union_Create;

        if (data.errorCode == (int)EErrorCode.EEC_Union_NoUnionName)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("联盟名称为空！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_NextUnionId)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("未知错误，无法创建联盟，请稍后重试！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_UnionNameExisted)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("该联盟名称已存在！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_Other)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("未知错误，无法创建联盟，请稍后重试！"), GUIHelper.GetColorByColorHex("FF2828"));
        }

        return data.errorCode != (int)EErrorCode.EEC_Success;
    }

    private void getCreateUnionResp(HttpMsgRspdBase msg)
    {
        if (getCreateUnionFailResp(msg)) return;

        Response_Union_Create data = msg as Response_Union_Create;
        PlatformManager.inst.GameHandleEventLog("Create_Guild", "");
        UserDataProxy.inst.playerData.SetUnionData(data.unionData);
        GUIManager.HideView<UnionCreateUI>();
        GUIManager.HideView<UnionFindToolUI>();

        UserDataProxy.inst.playerData.SetUnionData(data.unionData);
        if (data.unionData.unionId != "")
        {
            enterUnionScene();
        }
    }


    //请求加入公会
    private void request_enterUnion(string unionId)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_Enter()
                {
                    unionId = unionId
                }
            });
        }
    }

    private bool getEnterUnionFailResp(HttpMsgRspdBase msg)
    {
        Response_Union_Enter data = msg as Response_Union_Enter;

        if (data.errorCode == (int)EErrorCode.EEC_Union_HaveNoUnionData)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("没有此联盟信息，请加入其它联盟！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_NoMemberLimitData)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("此联盟人数上限信息错误，请稍后重试或加入其它联盟！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_ReachedMemberLimit)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("该联盟已满员，请加入其它联盟！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_IsDeleted)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("此联盟已解散，请加入其它联盟！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_CantEnterUnion)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("未知错误，无法加入联盟，请稍后重试！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_LevelLimit)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("未达到此联盟加入的最低等级，无法加入！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_SettingPersonal)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("此联盟为私人联盟，无法加入！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (data.errorCode == (int)EErrorCode.EEC_Union_InvestLimit) 
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("未达到此联盟加入的最低投资额，无法加入！"), GUIHelper.GetColorByColorHex("FF2828"));
        }

        //未达到此联盟加入的最低投资额，无法加入！

        return data.errorCode != (int)EErrorCode.EEC_Success;
    }

    private void getEnterUnionResp(HttpMsgRspdBase msg)
    {
        if (getEnterUnionFailResp(msg)) return;

        Response_Union_Enter data = msg as Response_Union_Enter;
        UserDataProxy.inst.playerData.SetUnionData(data.unionData);
        PlatformManager.inst.GameHandleEventLog("Join_Guild", "");
        GUIManager.HideView<UnionCreateUI>();
        GUIManager.HideView<UnionFindToolUI>();
        GUIManager.HideView<UnionInfoUI>();

        if (data.unionId != "")
        {
            enterUnionScene();
        }
    }


    //请求退出公会
    private void request_exitUnion()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_Leave()
            });
        }
    }

    private void getExitUnionResp(HttpMsgRspdBase msg)
    {
        Response_Union_Leave data = msg as Response_Union_Leave;
        UserDataProxy.inst.playerData.SetUnionData(data.unionData);
        //GUIManager.HideView<UnionCreateUI>();
        GUIManager.HideView<UnionFindToolUI>();
        //request_unionData("");

        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您已经离开了联盟"), Color.white);

        if (ManagerBinder.inst.mGameState == kGameState.Union && !ManagerBinder.inst.stateIsChanging)
        {
            HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
        }
    }

    //请求搜索公会数据
    /// <param name="unionName">留空为推荐列表</param>
    private void request_findUnionData(string unionName)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_List()
                {
                    searchName = unionName
                }
            });
        }
    }

    private void getFindUnionDataResp(HttpMsgRspdBase msg)
    {
        Response_Union_List data = msg as Response_Union_List;

        UnionFindToolUI window = GUIManager.GetWindow<UnionFindToolUI>();

        if (window != null && window.isShowing)
        {
            window.GetUnionSimpleDatas(data.unionList);
        }

    }

    //请求搜索玩家数据
    private void request_findPlayerList(string playerName)
    {

        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_UserData()
                {
                    searchName = playerName
                }
            });
        }
    }

    private void getFindPlayerDataResp(HttpMsgRspdBase msg)
    {
        Response_Union_UserData data = msg as Response_Union_UserData;

        UnionFindToolUI window = GUIManager.GetWindow<UnionFindToolUI>();

        if (window != null && window.isShowing)
        {
            window.GetFindPlayerDatas(data.unionMemberList);
        }


    }

    private void request_setUnionInfo(string unionName, int enterSetting, int enterLevel, long enterInvest)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_SetInfo()
                {
                    unionName = unionName,
                    enterSetting = enterSetting,
                    enterLevel = enterLevel,
                    enterInvest = enterInvest,
                }
            });
        }
    }

    private void getSetUnionInfoResp(HttpMsgRspdBase msg)
    {
        Response_Union_SetInfo data = msg as Response_Union_SetInfo;
        UserDataProxy.inst.unionDetailInfo.SetInfo(data.unionDetailData);
        refreshUnionSceneMembers();

        GUIManager.HideView<UnionSetSettingUI>();

        showUnionInfoUI(UserDataProxy.inst.unionDetailInfo);
    }


    private void request_setUnionMemberJob(string targetId, int job)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_SetUserRole()
                {
                    targetId = targetId,
                    unionJob = job
                }
            });
        }
    }

    private void getSetUnionMemberJobResp(HttpMsgRspdBase msg)
    {
        Response_Union_SetUserRole data = msg as Response_Union_SetUserRole;
        UserDataProxy.inst.unionDetailInfo.SetInfo(data.unionDetailData);
        refreshUnionSceneMembers();

        GUIManager.HideView<UnionMemberSettingUI>();

        var infoUI = GUIManager.GetWindow<UnionInfoUI>();
        if (infoUI != null && infoUI.isShowing)
        {
            infoUI.SetData(UserDataProxy.inst.unionDetailInfo);
        }

        var roleInfoUI = GUIManager.GetWindow<PlayerInfoPanelView>();
        if (roleInfoUI != null && roleInfoUI.isShowing)
        {
            if (roleInfoUI.curUserId == data.userId)
            {
                EventController.inst.TriggerEvent(GameEventType.SocialEvent.REQUEST_OTHERUSERDATA, data.userId);
            }
        }

    }

    private void request_kickoutUnionMember(string targetId)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_KickOut()
                {
                    targetId = targetId,
                }
            });
        }
    }

    private void getKickoutUnionMemberResp(HttpMsgRspdBase msg)
    {
        Response_Union_KickOut data = msg as Response_Union_KickOut;
        UserDataProxy.inst.unionDetailInfo.SetInfo(data.unionDetailData);
        refreshUnionSceneMembers();

        GUIManager.HideView<UnionMemberSettingUI>();
        GUIManager.HideView<KickOutUnionMemberAffirmUI>();
        GUIManager.HideView<PlayerInfoPanelView>();

        var infoUI = GUIManager.GetWindow<UnionInfoUI>();
        if (infoUI != null && infoUI.isShowing)
        {
            infoUI.SetData(UserDataProxy.inst.unionDetailInfo);
        }
    }

    private void request_UnionMessageInfoRefresh()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Union_MessageInfoRefresh()
                {
                    unionMsgUid = unionMsgUid
                }
            });
        }
    }

    private void request_UnionMemberHelpList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_MemberHelpList()
            {
            }
        });
    }

    private void request_UnoinSetHelp(int furnitureUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_SetHelp() //99999为扩建id
            {
                furnitureUid = furnitureUid,
            }
        });
    }

    private void request_UnionHelpMember(int furnitureUid, string userId, int useGem, int helpAll)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_HelpMember()
            {
                furnitureUid = furnitureUid,
                userId = userId,
                useGem = useGem,
                helpAll = helpAll,
            }
        });
    }

    private void request_unionTaskList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_TaskList()
            {
            }
        });
    }

    private void request_checkUnionTask(int taskUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_CheckUnionTask()
            {
                taskUid = taskUid,
            }
        });
    }

    private void request_startUnionTask(int taskUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_StartUnionTask()
            {
                taskId = taskUid,
            }
        });
    }

    private void request_cancelUnionTask(int taskUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_CancelUnionTask()
            {
                taskId = taskUid,
            }
        });
    }

    private void request_rewardUnionTask(int taskUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_RewardUnionTask()
            {
                taskId = taskUid,
            }
        });
    }

    private void request_gemRewardUnionTask(int taskUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_AccelUnionTask()
            {
                taskId = taskUid,
            }
        });
    }

    private void request_unionTaskRankList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_UnionTaskRankList()
            {

            }
        });
    }

    private void request_unionTaskResult()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_TaskResult()
            {

            }
        });
    }

    private void request_unionScienceList()
    {

        List<OneUnionScienceData> _scienceList = new List<OneUnionScienceData>();

        foreach (var item in UserDataProxy.inst.UnionScienceList)
        {
            _scienceList.Add(item.serverData);
        }

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_ScienceList()
            {
                scienceList = _scienceList
            }
        });
    }

    private void request_unionScienceUpgrade(int type)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_ScienceUpgrade()
            {
                type = type,
            }
        });
    }

    private void request_unionSkillList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_ScienceSkillList()
            {
            }
        });
    }

    private void request_useUnionSkill(int type)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_ScienceSkillUse()
            {
                type = type,
            }
        });
    }

    private void request_unionSkillRefresh(int type)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_ScienceSkillRefresh()
            {
                type = type,
            }
        });
    }



    private void getUnionMessageInfoRefreshResp(HttpMsgRspdBase msg)
    {
        Response_Union_MessageInfoRefresh data = msg as Response_Union_MessageInfoRefresh;
        unionMsgUid = data.unionMsgUid;

        if (data.unionMsgList.Count > 0)
        {
            showUnionMsgUpdateUI(data.unionMsgList);
        }
    }

    private void getUnionResultResp(HttpMsgRspdBase msg)
    {
        Response_Union_TaskResult data = msg as Response_Union_TaskResult;


        if (data.showFlag == 0) //显示
        {
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutUnionTaskResult { type = ReceiveInfoUIType.UnoinTaskResult, data = data });
        }

    }

    private void getUnionKickOutMessageResp(HttpMsgRspdBase msg)
    {
        Response_Union_KickOutMessage data = msg as Response_Union_KickOutMessage;

        if (data.showFlag == 1)
        {
            GameTimer.inst.AddTimer(4, 1, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您已经被踢出联盟"), Color.red);
            });
        }
    }

    private void refreshUnionSceneMembers()
    {
        if (UnionMap.inst != null)
        {
            UnionMap.inst.RefreshRoles();
        }
    }


    private void haveNewTaskData()
    {
        if (GUIManager.GetViewIsShowingByViewID(ViewPrefabName.TaskPanel) || GUIManager.GetViewIsShowingByViewID("UnionTaskUIView"))
        {
            UserDataProxy.inst.union_needRequest_Task = 0;
            request_unionTaskList();
        }
        else
        {
            UserDataProxy.inst.union_needRequest_Task = 1;
        }
    }

    private void haveNewHelpData()
    {
        if (GUIManager.GetViewIsShowingByViewID("UnionAidUIView") || ManagerBinder.inst.mGameState == kGameState.Town || ManagerBinder.inst.mGameState == kGameState.Union)
        {
            UserDataProxy.inst.union_needRequest_MemberHelp = 0;
            request_UnionMemberHelpList();
        }
        else
        {
            UserDataProxy.inst.union_needRequest_MemberHelp = 1;
        }
    }

    private void haveNewScienceData()
    {
        if (GUIManager.GetViewIsShowingByViewID("UnionWealUI"))
        {
            UserDataProxy.inst.union_needRequest_Science = 0;
            request_unionScienceList();
        }
        else
        {
            UserDataProxy.inst.union_needRequest_Science = 1;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        //if (requestUnionDetailTimingTimer != 0) GameTimer.inst.RemoveTimer(requestUnionDetailTimingTimer);
    }

}
