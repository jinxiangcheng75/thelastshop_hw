using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLockEquipUIMediator : BaseSystem
{
    UnLockDrawingUIView unLockDrawingUIView;
    UnLockDrawingByWorkerUIView unLockDrawingByWorkerUIView;

    protected override void AddListeners()
    {
        EventController.inst.AddListener<int>(GameEventType.SHOWUI_UNLOCKDRAWINGUI, showLockDrawingUI);
        EventController.inst.AddListener<int>(GameEventType.EquipEvent.EQUIP_UNLOCKEQUIP, UnLockEquipHandle);
        EventController.inst.AddListener<EquipDrawingsConfig>(GameEventType.SHOWUI_UNLOCKDRAWINGBYWORKERUI, showLockDrawingByWorkerUI);
        Helper.AddNetworkRespListener(MsgType.Response_Equip_Activate_Cmd, OnEquipActivateResp);
    }
    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<int>(GameEventType.EquipEvent.EQUIP_UNLOCKEQUIP, UnLockEquipHandle);
        EventController.inst.RemoveListener<int>(GameEventType.SHOWUI_UNLOCKDRAWINGUI, showLockDrawingUI);
        EventController.inst.RemoveListener<EquipDrawingsConfig>(GameEventType.SHOWUI_UNLOCKDRAWINGBYWORKERUI, showLockDrawingByWorkerUI);
    }

    private void showLockDrawingUI(int _equipDrawingId)
    {
        GUIManager.OpenView<UnLockDrawingUIView>((view) =>
        {
            view.showInfo(_equipDrawingId);
        });
    }
    private void UnLockEquipHandle(int _equipDrawingId)
    {
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(_equipDrawingId);
        if (UserDataProxy.inst.playerData.drawing >= cfg.activate_drawing)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Equip_Activate() { equipDrawingId = _equipDrawingId }
            });

        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("解锁图纸需要碎片不足"));
        }
    }

    private void OnEquipActivateResp(HttpMsgRspdBase msg)
    {
        Response_Equip_Activate data = (Response_Equip_Activate)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            // EquipDataProxy.inst.updateEquipInfo(data.equipInfo);
            //激活页面
            Logger.log($"图纸{data.equipInfo.equipDrawingId}激活成功");
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.ActivateDrawing, "", data.equipInfo.equipDrawingId, 0, 1));
            //EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
            EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_SHOWREFRESH);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_SHOWREFRESH);
        }
    }

    private void showLockDrawingByWorkerUI(EquipDrawingsConfig cfg)
    {
        GUIManager.OpenView<UnLockDrawingByWorkerUIView>((view) =>
        {
            view.showInfo(cfg);
        });
    }

}
