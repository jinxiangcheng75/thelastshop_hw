using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcheivementSystem : BaseSystem
{
    AcheivementUIView acheivementView;
    AcheivementDoneUIView acheivementDoneView;
    AcheivementSpecialDoneView acheivementSpecialDoneView;
    MsgBox_NeedAchievement msgBox_needAchievement;

    protected override void AddListeners()
    {
        base.AddListeners();
        EventController.inst.AddListener(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTUI, showAcheivementUI);
        EventController.inst.AddListener(GameEventType.AcheivementEvent.HIDEUI_ACHEIVEMENTUI, hideAcheivementUI);
        EventController.inst.AddListener(GameEventType.AcheivementEvent.ACHEIVEMENTSETDATA, setAcheivementData);
        EventController.inst.AddListener<int>(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONEUI, showAcheivementDoneUI);
        EventController.inst.AddListener(GameEventType.AcheivementEvent.HIDEACHEIVEMENTDONEUI_LIST, hideAcheivementDoneUI);
        EventController.inst.AddListener<int>(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONESPECIALUI, showAcheivementSpecialDoneUI);
        EventController.inst.AddListener<int, string>(GameEventType.AcheivementEvent.SHOWUI_MSGBOX_NEEDACHEIVEMENT, showMsgBos_NeedAcheivement);
        EventController.inst.AddListener(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTCHECK, requestAcheivementCheck);
        EventController.inst.AddListener<int>(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTAWARD, requestAcheivementAward);
        EventController.inst.AddListener<int>(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTROADAWARD, requestAcheivementRoadAward);
        EventController.inst.AddListener(GameEventType.AcheivementEvent.UPDATEAAA, showAcuPDATE);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        EventController.inst.RemoveListener(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTUI, showAcheivementUI);
        EventController.inst.RemoveListener(GameEventType.AcheivementEvent.HIDEUI_ACHEIVEMENTUI, hideAcheivementUI);
        EventController.inst.RemoveListener(GameEventType.AcheivementEvent.ACHEIVEMENTSETDATA, setAcheivementData);
        EventController.inst.RemoveListener<int>(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONEUI, showAcheivementDoneUI);
        EventController.inst.RemoveListener(GameEventType.AcheivementEvent.HIDEACHEIVEMENTDONEUI_LIST, hideAcheivementDoneUI);
        EventController.inst.RemoveListener<int>(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTDONESPECIALUI, showAcheivementSpecialDoneUI);
        EventController.inst.RemoveListener<int, string>(GameEventType.AcheivementEvent.SHOWUI_MSGBOX_NEEDACHEIVEMENT, showMsgBos_NeedAcheivement);
        EventController.inst.RemoveListener(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTCHECK, requestAcheivementCheck);
        EventController.inst.RemoveListener<int>(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTAWARD, requestAcheivementAward);
        EventController.inst.RemoveListener<int>(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTROADAWARD, requestAcheivementRoadAward);
    }

    void showAcheivementSpecialDoneUI(int acheivementId)
    {
        GUIManager.OpenView<AcheivementSpecialDoneView>((view) =>
        {
            acheivementSpecialDoneView = view;
            view.setSpecialData(acheivementId);
        });
    }

    void showMsgBos_NeedAcheivement(int acheivementId, string content)
    {
        GUIManager.OpenView<MsgBox_NeedAchievement>((view) =>
        {
            view.SetData(acheivementId, content);
        });
    }

    void hideAcheivementDoneUI()
    {
        GUIManager.HideView<AcheivementDoneUIView>();
    }

    void showAcuPDATE()
    {
        GUIManager.OpenView<AcheivementUIView>((view) =>
        {
            view.setAcheivementRoadData();
        });
    }

    void showAcheivementUI()
    {
        requestAcheivementCheck();
        GUIManager.OpenView<AcheivementUIView>((view) =>
        {
            acheivementView = view;
        });
    }

    void hideAcheivementUI()
    {
        GUIManager.HideView<AcheivementUIView>();
    }

    void showAcheivementDoneUI(int id)
    {
        GUIManager.OpenView<AcheivementDoneUIView>((view) =>
        {
            acheivementDoneView = view;
            view.setData(id);
        });
    }

    void setAcheivementData()
    {
        acheivementView = GUIManager.GetWindow<AcheivementUIView>();
        if (acheivementView != null && acheivementView.isShowing)
        {
            acheivementView.setData();
        }
    }

    void requestAcheivementCheck()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Achievement_Check()
        });
    }

    void requestAcheivementAward(int id)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Achievement_Reward()
            {
                achievementId = id
            }
        });
    }

    void requestAcheivementRoadAward(int id)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_AchievementRoad_Reward()
            {
                achievementRoadId = id
            }
        });
    }
}
