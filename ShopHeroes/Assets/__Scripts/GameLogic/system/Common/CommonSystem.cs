using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonRewardData
{
    public int rewardId;
    public long count;
    public int rarity;
    public int itemType;
    public int specialType = 0;//0 none  1 vip  2 sevenPass

    public CommonRewardData(int rewardId, long count, int rarity, int itemType)
    {
        this.rewardId = rewardId;
        this.count = count;
        this.rarity = rarity;
        this.itemType = itemType;
    }
}

public class CommonSystem : BaseSystem
{
    CommonTipsView commonTips;
    CommonGetRewardView commonRewardView;
    CommonMoreTipsView commonMoreTipsView;

    protected override void AddListeners()
    {
        base.AddListeners();

        EventController.inst.AddListener<CommonRewardData, Transform>(GameEventType.CommonEvent.COMMONTIPS_SETINFO, setCommonTipsInfo);
        EventController.inst.AddListener<List<CommonRewardData>>(GameEventType.CommonEvent.COMMONREWARD_SETINFO, setCommonRewardInfo);
        EventController.inst.AddListener<List<CommonRewardData>, Transform>(GameEventType.CommonEvent.COMMONMORETIPS_SETINFO, setCommonMoreTipsInfo);
        EventController.inst.AddListener<string, string, Transform>(GameEventType.CommonEvent.COMMONMORETITLECONTENT_SETINFO, setTitleContent);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();

        EventController.inst.RemoveListener<CommonRewardData, Transform>(GameEventType.CommonEvent.COMMONTIPS_SETINFO, setCommonTipsInfo);
        EventController.inst.RemoveListener<List<CommonRewardData>>(GameEventType.CommonEvent.COMMONREWARD_SETINFO, setCommonRewardInfo);
        EventController.inst.RemoveListener<List<CommonRewardData>, Transform>(GameEventType.CommonEvent.COMMONMORETIPS_SETINFO, setCommonMoreTipsInfo);
        EventController.inst.RemoveListener<string, string, Transform>(GameEventType.CommonEvent.COMMONMORETITLECONTENT_SETINFO, setTitleContent);
    }

    void setCommonMoreTipsInfo(List<CommonRewardData> allList, Transform trans)
    {
        GUIManager.OpenView<CommonMoreTipsView>((view) =>
        {
            commonMoreTipsView = view;
            view.showIntroducePanel(allList, trans);
        });
    }

    void setTitleContent(string title, string content, Transform trans)
    {
        GUIManager.OpenView<CommonTipsView>((view) =>
        {
            commonTips = view;
            view.showTitleContent(title, content, trans);
        });
    }

    void setCommonTipsInfo(CommonRewardData data, Transform trans)
    {

        if (data.itemType == 1600 || data.itemType == (int)ItemType.EquipmentDrawing) //截胡 展示装备图纸详情
        {
            int equipDrawingId = data.itemType == 1600 ? data.rewardId : ItemconfigManager.inst.GetConfig(data.rewardId).effect;
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUIBYDRAWINGID, equipDrawingId);
            return;
        }
        else if (data.itemType == (int)ItemType.Equip) //展示装备详情
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, "", data.rewardId, new List<EquipItem>());
            return;
        }


        GUIManager.OpenView<CommonTipsView>((view) =>
        {
            commonTips = view;
            view.showIntroducePanel(data, trans);
        });
    }

    void setCommonRewardInfo(List<CommonRewardData> rewardList)
    {
        GUIManager.OpenView<CommonGetRewardView>((view) =>
        {
            commonRewardView = view;
            view.setRewardInfo(rewardList);
        });
    }
}
