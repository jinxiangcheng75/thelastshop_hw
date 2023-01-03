using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSkinUI : ViewBase<FurnitureSkinUIComp>
{

    public override string viewID => ViewPrefabName.FurnitureSkinUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(hide);

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
    }

    protected override void DoHideAnimation()
    {
        HideView();
    }


}
