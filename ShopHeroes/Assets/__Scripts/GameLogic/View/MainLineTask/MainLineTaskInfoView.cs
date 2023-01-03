using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLineTaskInfoView : ViewBase<MainLineTaskInfoComp>
{
    public override string viewID => ViewPrefabName.MainlineInfoUI;
    public override string sortingLayerName => "window";

    MainlineData data;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.all;
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.bgBtn.onClick.AddListener(hide);
        contentPane.goBtn.ButtonClickTween(() =>
        {
            hide();

            GoOperationManager.inst.StartMainline(MainLineDataProxy.inst.Data.cfg.id);
        });
    }

    public void setData(MainlineData mainLineData)
    {
        if (mainLineData == null) return;
        if (mainLineData.cfg == null) return;
        data = mainLineData;
        //EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTARGETTRANSFORM, contentPane.goBtn.transform, true, K_Operation_Finger.Normal);
        string desText = LanguageManager.inst.GetValueByKey(data.cfg.des_2);
        desText = desText.Replace("\\n", "\n");
        contentPane.descText.text = desText;
        if (data.state != EMainTaskState.Idle)
        {
            contentPane.shceduleSlider.maxValue = 1;
            contentPane.shceduleSlider.value = 1;
            contentPane.scheduleText.text = LanguageManager.inst.GetValueByKey("完成");
        }
        else
        {
            long thisParam = 0, thisLimit = 0;
            if (data.cfg.task_type == 11)
            {
                thisLimit = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(data.limit, data.cfg.condition_id);
                var exploreCfg = ExploreDataProxy.inst.GetGroupDataByGroupId(data.cfg.condition_id);
                if (exploreCfg == null)
                {
                    thisParam = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(1, data.cfg.condition_id);
                }
                else
                {
                    thisParam = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(exploreCfg.groupData.level, data.cfg.condition_id) + exploreCfg.groupData.exp;
                }
            }
            else if (data.cfg.task_type == 15)
            {
                thisLimit = ShopkeeperUpconfigManager.inst.GetExpVal(data.limit);
                thisParam = ShopkeeperUpconfigManager.inst.GetExpVal((int)UserDataProxy.inst.playerData.level) + UserDataProxy.inst.playerData.CurrExp;
            }
            else
            {
                thisParam = data.param;
                thisLimit = data.limit;
            }
            contentPane.shceduleSlider.maxValue = thisLimit;
            contentPane.shceduleSlider.value = Mathf.Max(thisLimit * 0.05f, thisParam);
            contentPane.scheduleText.text = thisParam + "/" + thisLimit;
        }

        for (int i = 0; i < contentPane.allRewards.Count; i++)
        {
            int index = i;
            if (data.rewards.Count - 1 < index)
            {
                contentPane.allRewards[index].gameObject.SetActive(false);
            }
            else
            {
                contentPane.allRewards[index].gameObject.SetActive(true);
                contentPane.allRewards[index].setData(data.rewards[index]);
            }
        }
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {

    }
}
