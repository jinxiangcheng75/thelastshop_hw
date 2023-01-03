using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSystem : BaseSystem
{
    private SettingPanelView panelView;

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_SETTINGPANEL, ShowSettingPanel);
        EventController.inst.AddListener(GameEventType.SHOWUI_SETTINGBINDING, ShowSettingBinding);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_SETTINGPANEL, ShowSettingPanel);
        EventController.inst.RemoveListener(GameEventType.SHOWUI_SETTINGBINDING, ShowSettingBinding);
    }

    private void ShowSettingBinding()
    {
        GUIManager.CloseAllUI();

        if (panelView == null)
        {
            GUIManager.OpenView<SettingPanelView>((view) =>
            {
                panelView = view;
                view.SetBindingPanel();
            });
        }
        else
        {
            if (panelView.isShowing)
                panelView.SetBindingPanel();
            else
            {
                GUIManager.OpenView<SettingPanelView>((view) =>
                {
                    panelView = view;
                    view.SetBindingPanel();
                });
            }
        }
    }

    private void ShowSettingPanel()
    {
        GUIManager.CloseAllUI();
        //  if (panelView == null)
        {
            panelView = GUIManager.OpenView<SettingPanelView>();
        }

        //panelView.show();
    }
}
