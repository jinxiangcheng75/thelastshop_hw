using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCityUIMediator : BaseSystem
{

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.CityUIMediatorEvent.REFRESH_CITYUI_REDPOINT, refreshCityUIRedPoint);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.CityUIMediatorEvent.REFRESH_CITYUI_REDPOINT, refreshCityUIRedPoint);
    }

    void refreshCityUIRedPoint()
    {
        var cityUI = GUIManager.GetWindow<CityMainView>();

        if (cityUI != null && cityUI.isShowing)
        {
            cityUI.RefreshRedPoints();
        }

    }

}
