using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatRolePopupView : ViewBase<CreatRolePopupComp>
{
    public override string viewID => ViewPrefabName.CreatRolePopupPanel;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();

        contentPane.group.OnSelectedIndexValueChange = typeSelectChange;
    }

    private void typeSelectChange(int index)
    {

    }

    public void setDataBySex(int sex)
    {
        for (int i = 0; i < contentPane.group.togglesBtn.Count; i++)
        {
            
        }
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {

    }
}
