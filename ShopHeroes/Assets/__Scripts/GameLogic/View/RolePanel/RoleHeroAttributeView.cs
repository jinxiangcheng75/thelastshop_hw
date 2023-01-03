using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleHeroAttributeView : ViewBase<RoleHeroAttributeInfoComp>
{
    public override string viewID => ViewPrefabName.RoleHeroAttributeInfo;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.ButtonClickTween(hide);
    }

    public void setData(HeroPropertyData data)
    {
        contentPane.hpText.text = data.hp_basic.ToString();
        contentPane.attText.text = data.atk_basic.ToString();
        contentPane.defText.text = data.def_basic.ToString();
        contentPane.spdText.text = data.spd_basic.ToString();
        contentPane.dodgeText.text = data.dodge_basic.ToString();
        contentPane.accText.text = data.acc_basic.ToString();
        contentPane.toughText.text = data.tough_basic.ToString();
        contentPane.criText.text = data.cri_basic.ToString();
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
