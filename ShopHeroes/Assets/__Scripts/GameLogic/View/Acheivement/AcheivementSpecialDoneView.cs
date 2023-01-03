using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AcheivementSpecialDoneView : ViewBase<AcheivementSpecialDoneComp>
{
    public override string viewID => ViewPrefabName.AcheivementSpecialDoneUI;
    public override string sortingLayerName => "window";
    public override int showType => (int)ViewShowType.normal;
    int rewardId = 0;
    int count = 0;
    int curAcheivementId = 0;
    protected override void onInit()
    {
        base.onInit();

        contentPane.specialReceiveBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTAWARD, curAcheivementId);
            hide();
        });
    }

    public void setSpecialData(int acheivementId)
    {
        AudioManager.inst.PlaySound(113);
        contentPane.uiAnim.Play("open");
        contentPane.pc1.sortingOrder = 9999;
        var cfg = AcheivementConfigManager.inst.GetConfig(acheivementId);
        if (cfg == null)
        {
            Logger.error("没有成就id是" + acheivementId + "的数据");
            hide();
            return;
        }

        curAcheivementId = acheivementId;
        rewardId = cfg.reward_type;
        count = cfg.reward_num;
        //contentPane.specialTitleText.text = LanguageManager.inst.GetValueByKey(cfg.desc);
        contentPane.specialNameText.text = LanguageManager.inst.GetValueByKey(cfg.name);
        contentPane.specialNumText.text = LanguageManager.inst.GetValueByKey(cfg.condition_des);
        var itemCfg = ItemconfigManager.inst.GetConfig(cfg.reward_type);
        if (itemCfg == null)
        {
            Logger.error("没有itemid是" + cfg.reward_type + "的数据");
            return;
        }
        contentPane.specialIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
        contentPane.specialNNText.text = cfg.reward_num.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.rect);
        contentPane.specialObj.SetActive(true);

        //contentPane.specialTweenTrans.DOScale(1, 0.6f).From(0).SetEase(Ease.OutBack).SetDelay(1).OnComplete(() =>
        //{
        //    AudioManager.inst.PlaySound(110);
        //}).OnStart(() =>
        //{

        //});
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        contentPane.specialObj.SetActive(false);
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }
}
