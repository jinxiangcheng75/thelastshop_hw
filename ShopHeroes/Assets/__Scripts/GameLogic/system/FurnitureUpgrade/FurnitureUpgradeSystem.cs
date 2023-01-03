public class FurnitureUpgradeSystem : BaseSystem
{
    private FurnitureUpgradeView infoPanelView;
    private MsgBox_FurnitureUpgradeView msgBoxView;
    private FurnitureUpgradingView upgradingPanelView;
    private FurnitureUpgradeFinishView finishPanelView;

    private int funrUid = 0;
    private int costType = 0; // 0 - 金币 1 - 钻石

    protected override void OnInit()
    {
        ShelfUpgradeCallBack();
    }

    protected override void AddListeners()
    {
        EventController.inst.AddListener<int>(GameEventType.ShopDesignEvent.Furniture_Upgrading, Furniture_Refresh);
        EventController.inst.AddListener<int>(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, furnitureUpgradeFinish);
        EventController.inst.AddListener<IndoorData.ShopDesignItem>(GameEventType.RefreshUI_Furniture_ShelfContent, RefreshShelfContent);
        EventController.inst.AddListener<IndoorData.ShopDesignItem>(GameEventType.SHOWUI_UPGRADEPANEL, ShowUpgradePanel);
        EventController.inst.AddListener<int, int, int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE, PostFrunitureUpgrade);
        EventController.inst.AddListener<int, int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE_Immediately, PostFrunitureUpgrade_Immediately);
        EventController.inst.AddListener<IndoorData.ShopDesignItem>(GameEventType.SHOWUI_SHELFUPGRADINGUI, ShowUpgradingPanel);
        EventController.inst.AddListener<IndoorData.ShopDesignItem>(GameEventType.SHOWUI_SHELFUPGRADEFINISHUI, ShowUpgradeFinishPanel);
        EventController.inst.AddListener<IndoorData.ShopDesignItem>(GameEventType.FurnitureUpgradeEvent.SHOWMSGBOX, ShowOtherUpgradingMSGBOX);
        EventController.inst.AddListener<int, int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SAVEDATA, SaveUpgradeData);
        ServerResponse();
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<int>(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, furnitureUpgradeFinish);
        EventController.inst.RemoveListener<int>(GameEventType.ShopDesignEvent.Furniture_Upgrading, Furniture_Refresh);
        EventController.inst.RemoveListener<IndoorData.ShopDesignItem>(GameEventType.RefreshUI_Furniture_ShelfContent, RefreshShelfContent);
        EventController.inst.RemoveListener<IndoorData.ShopDesignItem>(GameEventType.SHOWUI_UPGRADEPANEL, ShowUpgradePanel);
        EventController.inst.RemoveListener<int, int, int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE, PostFrunitureUpgrade);
        EventController.inst.RemoveListener<int, int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_UPGRADEFRUITURE_Immediately, PostFrunitureUpgrade_Immediately);
        EventController.inst.RemoveListener<IndoorData.ShopDesignItem>(GameEventType.SHOWUI_SHELFUPGRADINGUI, ShowUpgradingPanel);
        EventController.inst.RemoveListener<IndoorData.ShopDesignItem>(GameEventType.SHOWUI_SHELFUPGRADEFINISHUI, ShowUpgradeFinishPanel);
        EventController.inst.RemoveListener<IndoorData.ShopDesignItem>(GameEventType.FurnitureUpgradeEvent.SHOWMSGBOX, ShowOtherUpgradingMSGBOX);
        EventController.inst.RemoveListener<int, int>(GameEventType.FurnitureUpgradeEvent.SHOPUPGRADE_SAVEDATA, SaveUpgradeData);
    }

    private void ServerResponse()
    {
        //家具完成
        NetworkEvent.SetCallback(MsgType.Response_Design_Finish_Cmd,
    (successResp) =>
    {
        var res = successResp as Response_Design_Finish;
        if (res.errorCode == (int)EErrorCode.EEC_Success)
        {
            ShowUpgradeFinishPanel(UserDataProxy.inst.GetFuriture(res.uid));
            D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
        }
    },
    (failedResp) =>
    {

    });
    }
    //显示扩建面板正在扩建的方法
    private void ShowOtherUpgradingMSGBOX(IndoorData.ShopDesignItem item)
    {
        GUIManager.OpenView<MsgBox_FurnitureUpgradeView>(view =>
        {
            msgBoxView = view;
            msgBoxView.SetData(item);
        });
    }

    //显示家具升级的面板
    private void ShowUpgradePanel(IndoorData.ShopDesignItem item)
    {
        //GUIManager.OpenView<FurnitureUpgradeView>((view) =>
        //{
        //    infoPanelView = view;
        //    view.setData(item);
        //});

        HotfixBridge.inst.TriggerLuaEvent("ShowUI_FurnitureUpgradePanel", item);

    }

    //刷新货架内容
    void RefreshShelfContent(IndoorData.ShopDesignItem shelf)
    {
        //if (infoPanelView != null && infoPanelView.isShowing)
        //{
        //    infoPanelView.RefreshShelfGridItem(shelf);
        //}

        HotfixBridge.inst.TriggerLuaEvent("FurnitureUpgradePanel_RefreshShelfGridItem", shelf);


    }

    //显示家具正在升级的面板
    private void ShowUpgradingPanel(IndoorData.ShopDesignItem item)
    {
        GUIManager.OpenView<FurnitureUpgradingView>(view =>
        {
            upgradingPanelView = view;
            upgradingPanelView.SetData(item);
        });
    }

    //显示家具升级完成的面板
    private void ShowUpgradeFinishPanel(IndoorData.ShopDesignItem item)
    {

        //GUIManager.OpenView<FurnitureUpgradeFinishView>(view =>
        //{
        //    finishPanelView = view;
        //    finishPanelView.SetData(item);
        //});

        HotfixBridge.inst.TriggerLuaEvent("ShowUI_FurnitureUpgradeFinishView", item);

    }

    //发送家具升级的请求
    public void PostFrunitureUpgrade(int designType, int furniture_uid, int kind)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Upgrade()
            {
                designType = designType,
                uid = furniture_uid,
                useGem = kind
            }
        });
    }

    public void PostFrunitureUpgrade_Immediately(int designType, int furniture_uid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Immediately()
            {
                designType = designType,
                uid = furniture_uid,
            }
        });
    }

    public void SaveUpgradeData(int uid, int costType)
    {
        funrUid = uid;
        this.costType = costType;
    }

    //升级的回调
    public void ShelfUpgradeCallBack()
    {
        NetworkEvent.SetCallback(MsgType.Response_Design_Upgrade_Cmd,
        (successResp) =>
        {
            var resp = (Response_Design_Upgrade)successResp;
            AudioManager.inst.PlaySound(24);
            PlatformManager.inst.GameHandleEventLog("Jiaju_Upgrade", "");
            //GUIManager.HideView<FurnitureUpgradeView>();
            HotfixBridge.inst.TriggerLuaEvent("HideUI_FurnitureUpgradePanel");

            if (resp.errorCode == (int)EErrorCode.EEC_Success)
            {
                if (costType == 1)
                {
                    var cfg = UserDataProxy.inst.GetFuriture(funrUid);
                    ShowUpgradeFinishPanel(cfg);
                }
            }
        },
        (failedResp) =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("家具升级失败！"));
        });
    }

    public void furnitureUpgradeFinish(int uid)
    {

        if (!UserDataProxy.inst.FurnitureCanUpgradeFinish(uid)) //先前端检测空间是否足够升级
        {
            return;
        }

        var furniture = UserDataProxy.inst.GetFuriture(uid);
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Finish()
            {
                uid = furniture.uid,
                designType = furniture.type,
            }
        });
    }

    public void Furniture_Refresh(int uid)
    {
        var furniture = UserDataProxy.inst.GetFuriture(uid);
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_Refresh()
            {
                uid = furniture.uid,
                designType = furniture.type,
            }
        });
    }

}

