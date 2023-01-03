using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBoxSystem : BaseSystem
{
    OpenTreasureBoxUIView _openBoxView;
    TreasureBoxInfoUIView _boxInfoView;
    TreasureBoxCompleteView _boxCompleteView;
    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener<int>(GameEventType.TreasureBoxEvent.OPENBOX_SHOWUI, showOpenBoxUI);
        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.OPENBOX_HIDEUI, hideOpenBoxUI);
        EventController.inst.AddListener<kTreasureBoxInfoType, int>(GameEventType.TreasureBoxEvent.BOXINFO_SHOWUI, showBoxInfoUI);
        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.BOXINFO_HIDEUI, hideBoxInfoUI);
        EventController.inst.AddListener<List<OneRewardItem>, int>(GameEventType.TreasureBoxEvent.BOXCOMPLETE_SHOWUI, showBoxCompleteUI);
        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.BOXCOMPLETE_HIDEUI, hideBoxCompleteUI);

        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.OPENTBOXUINOTPARA, showBoxUI);
        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.SETTBOXCAMERA, setCameraOrth);
        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.SETTBOXCAMERAREVERT, setCameraRevert);

        EventController.inst.AddListener(GameEventType.TreasureBoxEvent.REQUEST_TREASUREBOXDATA, requestTreasureBoxData);
        EventController.inst.AddListener<int, int>(GameEventType.TreasureBoxEvent.REQUEST_OPENTREASUREBOX, requestOpenTreasureBox);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener<int>(GameEventType.TreasureBoxEvent.OPENBOX_SHOWUI, showOpenBoxUI);
        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.OPENBOX_HIDEUI, hideOpenBoxUI);
        EventController.inst.RemoveListener<kTreasureBoxInfoType, int>(GameEventType.TreasureBoxEvent.BOXINFO_SHOWUI, showBoxInfoUI);
        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.BOXINFO_HIDEUI, hideBoxInfoUI);
        EventController.inst.RemoveListener<List<OneRewardItem>, int>(GameEventType.TreasureBoxEvent.BOXCOMPLETE_SHOWUI, showBoxCompleteUI);
        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.BOXCOMPLETE_HIDEUI, hideBoxCompleteUI);

        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.OPENTBOXUINOTPARA, showBoxUI);
        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.SETTBOXCAMERA, setCameraOrth);
        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.SETTBOXCAMERAREVERT, setCameraRevert);

        EventController.inst.RemoveListener(GameEventType.TreasureBoxEvent.REQUEST_TREASUREBOXDATA, requestTreasureBoxData);
        EventController.inst.RemoveListener<int, int>(GameEventType.TreasureBoxEvent.REQUEST_OPENTREASUREBOX, requestOpenTreasureBox);
    }

    #region ui事件
    void setCameraOrth()
    {
        //Transform camerastartTF = GameObject.Find("CameraStartPoint").transform;
        //if (camerastartTF != null)
        //{
        //    Camera.main.transform.position = camerastartTF.position;
        //    float currvalue = (float)Screen.height / (float)Screen.width;
        //    float designvalue = StaticConstants.designSceneSize.y / StaticConstants.designSceneSize.x;
        //    Camera.main.orthographicSize = (StaticConstants.combatCameraOrthographicSize / designvalue) * currvalue;
        //}
        //else
        //{
        //    Camera.main.transform.position = Vector3.zero;
        //    Camera.main.orthographicSize = 5;
        //}
        float tempOrthographicSize = FGUI.inst.isLandscape ? 3.7f : 9.53f;
        Camera.main.orthographicSize = tempOrthographicSize;
        Camera.main.transform.position = new Vector3(0.34f, 12.8f, 0);
        Camera.main.transform.eulerAngles = new Vector3(9.944f, -0.369f, -0.146f);
    }

    void setCameraRevert()
    {
        Camera.main.transform.eulerAngles = Vector3.zero;
    }

    void showBoxUI()
    {
        GUIManager.OpenView<OpenTreasureBoxUIView>((view) =>
        {
            _openBoxView = view;
            view.OpenTreasureBoxPanel();
        });
    }

    void showOpenBoxUI(int boxId)
    {
        GUIManager.OpenView<OpenTreasureBoxUIView>((view) =>
        {
            _openBoxView = view;
            view.setData(boxId);
        });
    }

    void hideOpenBoxUI()
    {
        GUIManager.HideView<OpenTreasureBoxUIView>();
    }

    void showBoxInfoUI(kTreasureBoxInfoType type, int boxId)
    {
        GUIManager.OpenView<TreasureBoxInfoUIView>((view) =>
        {
            _boxInfoView = view;
            view.setData(type, boxId);
        });
    }

    void hideBoxInfoUI()
    {
        GUIManager.HideView<TreasureBoxInfoUIView>();
    }

    void showBoxCompleteUI(List<OneRewardItem> items, int boxItemId)
    {
        GUIManager.OpenView<TreasureBoxCompleteView>((view) =>
        {
            _boxCompleteView = view;
            _boxCompleteView.setData(boxItemId, items);
        });
    }

    void hideBoxCompleteUI()
    {
        GUIManager.HideView<TreasureBoxCompleteView>();
    }
    #endregion

    #region 网络事件
    void requestTreasureBoxData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_TreasureBox_Data()
        });
    }

    void requestOpenTreasureBox(int boxGroupId, int useGem)
    {
        TreasureBoxDataProxy.inst.isOpening = true;
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_TreasureBox_Open()
            {
                boxGroupId = boxGroupId,
                useGem = useGem
            }
        });
    }
    #endregion
}
