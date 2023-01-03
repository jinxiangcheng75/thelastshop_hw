using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扩建面板    #陆泓屹
/// </summary>

public class ExtensionSystem : BaseSystem
{
    private ExtensionPanelView panelView;
    private ExtendingPanelView extendingPanelView;
    private ExtensionFinishView finishPanelView;

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_EXTENSIONPANEL, ShowExtensionPanel);
        EventController.inst.AddListener(GameEventType.HIDEUI_EXTENSIONPANEL, HideExtensionPanel);
        EventController.inst.AddListener(GameEventType.ExtensionEvent.EXTENSION_POST_COINEXTENSION, SendCoinExtension);
        EventController.inst.AddListener(GameEventType.ExtensionEvent.EXTENSION_POST_DIAMEXTENSION, SendDimoExtension);
        EventController.inst.AddListener(GameEventType.SHOWUI_EXTENDINGPANEL, ShowExtendingPanel);
        EventController.inst.AddListener(GameEventType.SHOWUI_EXTENSIONFINISHPANEL, ShowExtensionFinishPanel);
        EventController.inst.AddListener(GameEventType.ExtensionEvent.EXTENSION_POST_FINISHEXTENSION, PostExtensionFinishRequest);

        ExtensionCallbacks();
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_EXTENSIONPANEL, ShowExtensionPanel);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_EXTENSIONPANEL, HideExtensionPanel);
        EventController.inst.RemoveListener(GameEventType.ExtensionEvent.EXTENSION_POST_COINEXTENSION, SendCoinExtension);
        EventController.inst.RemoveListener(GameEventType.ExtensionEvent.EXTENSION_POST_DIAMEXTENSION, SendDimoExtension);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_EXTENDINGPANEL, ShowExtendingPanel);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_EXTENSIONFINISHPANEL, ShowExtensionFinishPanel);
        EventController.inst.RemoveListener(GameEventType.ExtensionEvent.EXTENSION_POST_FINISHEXTENSION, PostExtensionFinishRequest);
    }

    private void PostExtensionFinishRequest()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_ShopImmediately() { }
        });
    }

    private void ShowExtensionFinishPanel()
    {
        GUIManager.OpenView<ExtensionFinishView>();
    }

    private void ShowExtendingPanel()
    {
        GUIManager.OpenView<ExtendingPanelView>();
    }

    public void ExtensionCallbacks()
    {
        //发送扩建请求的回调
        NetworkEvent.SetCallback(MsgType.Response_Design_ShopUpgrade_Cmd,
       (successResp) =>
       {
           Response_Design_ShopUpgrade resp_obj = (Response_Design_ShopUpgrade)successResp;
           if (resp_obj.errorCode == (int)EErrorCode.EEC_Success)
           {
               if (resp_obj.shopData.shopLevel == 2)
               {
                   PlatformManager.inst.GameHandleEventLog("Expand_1", "");
               }
               else if (resp_obj.shopData.shopLevel == 3)
               {
                   PlatformManager.inst.GameHandleEventLog("Expand_2", "");
               }
               else if (resp_obj.shopData.shopLevel == 4)
               {
                   PlatformManager.inst.GameHandleEventLog("Expand_3", "");
               }
               else if (resp_obj.shopData.shopLevel == 5)
               {
                   PlatformManager.inst.GameHandleEventLog("Expand_4", "");
               }
               else if (resp_obj.shopData.shopLevel == 6)
               {
                   PlatformManager.inst.GameHandleEventLog("Expand_5", "");
               }

               if (UserDataProxy.inst.shopData.shopLevel < resp_obj.shopData.shopLevel && resp_obj.shopData.state == 0)
               {
                   UserDataProxy.inst.updateShopData(resp_obj.shopData);
                   EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENSIONFINISHPANEL);
               }
               else
               {
                   UserDataProxy.inst.updateShopData(resp_obj.shopData);
               }
               HideExtensionPanel();

           }
       },
       (failedResp) =>
       {
           EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("请求扩建失败！"));
       });

        //直接扩建完成请求的回调
        NetworkEvent.SetCallback(MsgType.Response_Design_ShopImmediately_Cmd,
        (successResp) =>
        {
            Response_Design_ShopImmediately resp_ShopUpgradeImmediately = (Response_Design_ShopImmediately)successResp;
            if (resp_ShopUpgradeImmediately.errorCode == (int)EErrorCode.EEC_Success)
            {
                UserDataProxy.inst.updateShopData(resp_ShopUpgradeImmediately.shopData);
                HideExtensionPanel();
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENSIONFINISHPANEL);
            }
            else
            {
                Logger.log("Response_Design_ShopImmediately  errorcode = " + resp_ShopUpgradeImmediately.errorCode.ToString());
            }
        },
        (failedResp) =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("请求扩建失败！"));
        });

        NetworkEvent.SetCallback(MsgType.Response_Design_ShopRefresh_Cmd,
        (successResp) =>
        {
            var resp_obj = successResp as Response_Design_ShopRefresh;
            UserDataProxy.inst.shopData.setShopData(resp_obj.shopData);
            if (IndoorMap.inst != null) IndoorMap.inst.updateUpgradePop();
        },
        (failedResp) =>
        {

        });
        NetworkEvent.SetCallback(MsgType.Response_Design_ShopFinish_Cmd,
            (successResp) =>
            {
                var res = successResp as Response_Design_ShopFinish;
                // setShopData(res.shopData);
                if (res.errorCode == (int)EErrorCode.EEC_Success)
                {
                    UserDataProxy.inst.updateShopData(res.shopData);
                    hideExtendingPanel();
                    HideExtensionPanel();
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENSIONFINISHPANEL);
                    D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
                }
            },
            (failedResp) =>
            {

            });
    }

    //发送此账户的扩建数据（金币）
    private void SendCoinExtension()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_ShopUpgrade() { useGem = 0 }
        });
    }

    //发送此账户的立即扩建数据（钻石）
    private void SendDimoExtension()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_ShopUpgrade() { useGem = 1 }
        });
    }

    private void ShowExtensionPanel()
    {
        GUIManager.OpenView<ExtensionPanelView>();
    }

    private void hideExtendingPanel()
    {
        GUIManager.HideView<ExtendingPanelView>();
    }

    private void HideExtensionPanel()
    {
        GUIManager.HideView<ExtensionPanelView>();
    }
}
