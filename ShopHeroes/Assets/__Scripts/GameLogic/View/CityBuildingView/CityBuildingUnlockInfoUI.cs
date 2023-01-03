using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuildingUnlockInfoUI : ViewBase<CityBuildingUnlockInfoUIComp>
{
    public override string viewID => ViewPrefabName.CityBuildingUnlockInfoUI;

    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

    }

    public void SetData(CityBuildingData data) 
    {
        hide();
    }



}
