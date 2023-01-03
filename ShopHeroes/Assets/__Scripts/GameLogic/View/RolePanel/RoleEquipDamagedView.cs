using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleEquipDamagedView : ViewBase<RoleEquipDamagedComp>
{
    public override string viewID => ViewPrefabName.RoleEquipDamagedUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
        base.onInit();

        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.allIgnoreBtn.ButtonClickTween(() =>
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });

        contentPane.allShowBtn.ButtonClickTween(() =>
        {
            hide();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new PopUIInfoBase { type = ReceiveInfoUIType.ExploreEquipDamagedInfo });
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });
    }

    public void setData()
    {
        if (ExploreDataProxy.inst.currExploreData == null) return;
        var data = ExploreDataProxy.inst.currExploreData.heroInfo.FindAll(t => t.brokenEquip.equipId != 0);

        for (int i = 0; i < contentPane.damagedEquipList.Count; i++)
        {
            int index = i;
            if (index >= data.Count)
            {
                contentPane.damagedEquipList[index].gameObject.SetActive(false);
            }
            else
            {
                contentPane.damagedEquipList[index].gameObject.SetActive(true);
                contentPane.damagedEquipList[index].setData(data[index].brokenEquip.equipId);
            }
        }

        contentPane.allShowText.text = LanguageManager.inst.GetValueByKey("维修") + "(" + data.Count + ")";
    }

    public void setIntroduceData(Transform pos, int equipId)
    {
        contentPane.introduce.gameObject.SetActive(true);
        contentPane.introduce.setData(pos, equipId);
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        AudioManager.inst.PlaySound(11);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();
    }
}
