using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AcheivementDoneUIView : ViewBase<AcheivementDoneComp>
{
    public override string viewID => ViewPrefabName.AcheivementDoneUI;
    public override string sortingLayerName => "top";

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.ignore;
    }

    public void setData(int acheivementId)
    {
        AudioManager.inst.PlaySound(112);
        var cfg = AcheivementConfigManager.inst.GetConfig(acheivementId);
        if (cfg == null)
        {
            Logger.error("没有成就id是" + acheivementId + "的数据");
            hide();
            return;
        }
        contentPane.bigNameText.text = LanguageManager.inst.GetValueByKey(cfg.name);
        contentPane.pointText.text = cfg.reward_points.ToString();
        contentPane.icon.SetSprite(cfg.atlas, cfg.icon);
        float movePos = FGUI.inst.isLandscape ? 2000 : 1000;
        //AudioManager.inst.PlaySound(103);
        contentPane.bgRect.DOAnchorPos3DX(0, 0.5f).From(-movePos).OnComplete(() =>
        {
            contentPane.bgRect.DOAnchorPos3DX(movePos, 0.5f).SetDelay(1).OnComplete(() =>
            {
                hide();
            });
        });
    }

    protected override void onShown()
    {

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

    }

    protected override void DoHideAnimation()
    {
        HideView();
    }

    protected override void onHide()
    {

    }
}
