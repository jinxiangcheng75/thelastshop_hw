using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : BaseSystem
{
    private PlayerInfoPanelView _roleInfoPanel;
    private ShopkeeperPanelView _shopkeeperPanel;
    private ChangeNameView _changeNamePanel;
    private PlayerUpView _playerUpView;
    private MsgBoxPlayerUpItemUIView _msgBoxView;
    private ShopkeeperSubUIView _subView;
    private ShopkeeperSingleBuyView _singleBuyView;
    private ShopkeeperPromptView _promptView;


    protected override void AddListeners()
    {

        EventController.inst.AddListener(GameEventType.SHOWUI_SELFROLEINFO, showSelfRoleInfoPanel);
        EventController.inst.AddListener(GameEventType.REFRESH_SELFROLEINFO, refreshSelfRoleInfoPanel);
        EventController.inst.AddListener<PlayerInfoData>(GameEventType.SHOWUI_ROLEINFO, ShowRoleInfoPanel);
        EventController.inst.AddListener(GameEventType.HIDEUI_ROLEINFO, HideRoleInfoPanel);
        EventController.inst.AddListener<bool>(GameEventType.SETSPBTNSTATE, setSpBtnState);
        EventController.inst.AddListener(GameEventType.SHOWUI_SHOPKEEPERPANEL, ShowShopkeeperPanelView);
        EventController.inst.AddListener(GameEventType.HIDEUI_SHOPKEEPERPANEL, HideShopkeeperPanelView);
        EventController.inst.AddListener<RoleSubTypeData>(GameEventType.SHOWUI_SINGLEBUY, showSingleBuyUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_SINGLEBUY, hideSingleBuyUI);
        EventController.inst.AddListener(GameEventType.SHOWUI_CHANGENAME, ShowChangeNamePanel);
        EventController.inst.AddListener(GameEventType.HIDEUI_CHANGENAME, HideChangeNamePanel);
        EventController.inst.AddListener(GameEventType.SHOWUI_PLAYERUPUI, ShowPlayerUpView);
        EventController.inst.AddListener(GameEventType.HIDEUI_PLAYERUPUI, HidePlayerUpView);
        EventController.inst.AddListener<PlayerUpItemData>(GameEventType.SHOWUI_MSGBOXPLAYERUPITEMUI, ShowMsgBoxPlayerUpItemView);

        EventController.inst.AddListener(GameEventType.DressUpEvent.BUYSINGLEDRESS, BuyDressComplete);

        EventController.inst.AddListener(GameEventType.ShopkeeperComEvent.INITCLOTHE, InitRoleClothe);
        EventController.inst.AddListener<PanelType>(GameEventType.SHOWUI_SHOPKEEPERSUBPANEL, showSubShopkeeperUI);
        EventController.inst.AddListener<bool>(GameEventType.JUDGESHOPKEEPERDRESS, judgeShopkeeperDress);

        EventController.inst.AddListener(GameEventType.SHOWUI_PROMPT, showPromptUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_PROMPT, hidePromptUI);


        EventController.inst.AddListener<string>(GameEventType.SocialEvent.REQUEST_OTHERUSERDATA, request_getOtherUserData);
        EventController.inst.AddListener<int, RoleDress>(GameEventType.REQUESTUSERCUSTOM, requestCustom);


        Helper.AddNetworkRespListener(MsgType.Response_Union_SearchUserData_Cmd, getOtherUserData);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_SELFROLEINFO, showSelfRoleInfoPanel);
        EventController.inst.RemoveListener(GameEventType.REFRESH_SELFROLEINFO, refreshSelfRoleInfoPanel);
        EventController.inst.RemoveListener<PlayerInfoData>(GameEventType.SHOWUI_ROLEINFO, ShowRoleInfoPanel);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_ROLEINFO, HideRoleInfoPanel);
        EventController.inst.RemoveListener<bool>(GameEventType.SETSPBTNSTATE, setSpBtnState);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_SHOPKEEPERPANEL, ShowShopkeeperPanelView);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_SHOPKEEPERPANEL, HideShopkeeperPanelView);
        EventController.inst.RemoveListener<RoleSubTypeData>(GameEventType.SHOWUI_SINGLEBUY, showSingleBuyUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_SINGLEBUY, hideSingleBuyUI);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_CHANGENAME, ShowChangeNamePanel);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_CHANGENAME, HideChangeNamePanel);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_PLAYERUPUI, ShowPlayerUpView);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_PLAYERUPUI, HidePlayerUpView);
        EventController.inst.RemoveListener<PlayerUpItemData>(GameEventType.SHOWUI_MSGBOXPLAYERUPITEMUI, ShowMsgBoxPlayerUpItemView);

        EventController.inst.RemoveListener(GameEventType.DressUpEvent.BUYSINGLEDRESS, BuyDressComplete);

        EventController.inst.RemoveListener(GameEventType.ShopkeeperComEvent.INITCLOTHE, InitRoleClothe);
        EventController.inst.RemoveListener<PanelType>(GameEventType.SHOWUI_SHOPKEEPERSUBPANEL, showSubShopkeeperUI);
        EventController.inst.RemoveListener<bool>(GameEventType.JUDGESHOPKEEPERDRESS, judgeShopkeeperDress);

        EventController.inst.RemoveListener(GameEventType.SHOWUI_PROMPT, showPromptUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_PROMPT, hidePromptUI);

        EventController.inst.RemoveListener<string>(GameEventType.SocialEvent.REQUEST_OTHERUSERDATA, request_getOtherUserData);
        EventController.inst.RemoveListener<int, RoleDress>(GameEventType.REQUESTUSERCUSTOM, requestCustom);
    }

    void requestCustom(int gender, RoleDress dress)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_Custom()
            {
                gender = gender,
                userDress = dress
            }
        });
    }

    void showSelfRoleInfoPanel()
    {
        GUIManager.OpenView<PlayerInfoPanelView>((view) =>
        {
            _roleInfoPanel = view;
            view.ShowSelfInfo();
        });
    }

    void refreshSelfRoleInfoPanel()
    {
        if (_roleInfoPanel != null && _roleInfoPanel.isShowing)
        {
            _roleInfoPanel.ShowSelfInfo();
        }
    }

    private void ShowRoleInfoPanel(PlayerInfoData data)
    {
        GUIManager.OpenView<PlayerInfoPanelView>((view) =>
        {
            view.RefreshUIData(data);
        });
    }

    private void HideRoleInfoPanel()
    {
        GUIManager.HideView<PlayerInfoPanelView>();
    }

    void setSpBtnState(bool state)
    {
        GUIManager.OpenView<ShopkeeperPanelView>(view =>
        {
            _shopkeeperPanel = view;
            view.ShowFinishBtnOrChangeBtn(state);
        });
    }

    private void ShowShopkeeperPanelView()
    {
        _shopkeeperPanel = GUIManager.OpenView<ShopkeeperPanelView>();
    }

    private void HideShopkeeperPanelView()
    {
        GUIManager.HideView<ShopkeeperPanelView>();
    }

    private void ShowChangeNamePanel()
    {
        _changeNamePanel = GUIManager.OpenView<ChangeNameView>();
    }

    private void HideChangeNamePanel()
    {


        if (_changeNamePanel != null && _changeNamePanel.isShowing)
            GUIManager.HideView<ChangeNameView>();
    }

    private void InitRoleClothe()
    {
        ShopkeeperDataProxy.inst.InitClotheOnRole();
    }

    private void ShowPlayerUpView()
    {
        GUIManager.OpenView<PlayerUpView>();
    }

    private void HidePlayerUpView()
    {
        GUIManager.HideView<PlayerUpView>();
    }

    private void ShowMsgBoxPlayerUpItemView(PlayerUpItemData data)
    {
        GUIManager.OpenView<MsgBoxPlayerUpItemUIView>((msgBoxView) =>
        {
            _msgBoxView = msgBoxView;
            _msgBoxView.setMsgText(data);
        });
    }

    void showSubShopkeeperUI(PanelType type)
    {
        GUIManager.OpenView<ShopkeeperSubUIView>((view) =>
        {
            _subView = view;
            view.setData(type);
        });
    }

    void judgeShopkeeperDress(bool judge)
    {
        GUIManager.OpenView<ShopkeeperPanelView>((view) =>
        {
            _shopkeeperPanel = view;
            view.ShowFinishBtnOrChangeBtn(judge);
        });
    }

    void showSingleBuyUI(RoleSubTypeData id)
    {
        GUIManager.OpenView<ShopkeeperSingleBuyView>((view) =>
        {
            _singleBuyView = view;
            view.setData(id);
        });
    }

    void hideSingleBuyUI()
    {
        GUIManager.HideView<ShopkeeperSingleBuyView>();
    }

    void BuyDressComplete()
    {
        if (_subView != null && _subView.isShowing)
        {
            _subView.BuyDressComplete();
        }
        //GUIManager.OpenView<ShopkeeperSubUIView>((view) =>
        //{
        //    _subView = view;
        //    view.BuyDressComplete();
        //});
    }

    void showPromptUI()
    {
        GUIManager.OpenView<ShopkeeperPromptView>();
    }

    void hidePromptUI()
    {
        GUIManager.HideView<ShopkeeperPromptView>();
    }

    void request_getOtherUserData(string userId)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Union_SearchUserData()
            {
                userId = userId
            }
        });
    }

    void getOtherUserData(HttpMsgRspdBase msg)
    {
        Response_Union_SearchUserData data = msg as Response_Union_SearchUserData;
        PlayerInfoData playerInfoData = new PlayerInfoData(data.userData, data.unionData, data.petInfo, data.vipData, data.lastActiveTime <= 0 ? -1 : data.serverTime - data.lastActiveTime, data.userId);

        if (data.userId == UserDataProxy.inst.playerData.userUid) //自己职位要及时更新
        {
            UserDataProxy.inst.playerData.memberJob = data.unionData.memberJob;
        }
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_ROLEINFO, playerInfoData);
    }
}
