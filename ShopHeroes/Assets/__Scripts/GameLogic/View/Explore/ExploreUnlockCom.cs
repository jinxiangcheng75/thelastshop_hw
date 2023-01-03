using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreUnlockCom : MonoBehaviour
{
    public GUIIcon headIcon;
    public Button closeBtn;
    public Text desText;
    public List<GUIIcon> icons;
    public Text goldNumText;
    public Text gemNumText;
    public Button goldBtn;
    public Button gemBtn;
}

public class ExploreUnlockView : ViewBase<ExploreUnlockCom>
{
    public override string viewID => ViewPrefabName.ExploreUnlockUI;
    public override string sortingLayerName => "popup";

    ExploreGroup data;
    ExploreInstanceLvConfigData cfg;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.goldBtn.onClick.AddListener(() => UnlockExplore(0));
        contentPane.gemBtn.onClick.AddListener(() => UnlockExplore(1));
    }

    public void setData(int groupId)
    {
        data = ExploreDataProxy.inst.GetGroupDataByGroupId(groupId);
        cfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(groupId, 1);
        var instanceCfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(groupId);
        contentPane.desText.text = LanguageManager.inst.GetValueByKey("解锁{0}冒险吗？", LanguageManager.inst.GetValueByKey(cfg.instance_name));
        //"解锁" + cfg.instance_name + "冒险吗？";
        contentPane.goldNumText.text = cfg.unlock_gold.ToString();
        contentPane.gemNumText.text = cfg.unlock_diamond.ToString();
        contentPane.goldNumText.color = cfg.unlock_gold > UserDataProxy.inst.playerData.gold ? Color.red : Color.white;
        //contentPane.gemNumText.color = cfg.unlock_diamond > UserDataProxy.inst.playerData.gem ? Color.red : Color.white;
        contentPane.headIcon.SetSprite(StaticConstants.exploreAtlas, instanceCfg.instance_icon);
        setSmallItemIcon();
    }

    private void setSmallItemIcon()
    {
        for (int i = 0; i < data.explores.Count - 1; i++)
        {
            int index = i;
            itemConfig itemCfg = ItemconfigManager.inst.GetConfig(data.explores[index].id);
            contentPane.icons[index].SetSprite(itemCfg.atlas, itemCfg.icon);
        }
    }

    private void UnlockExplore(int costType)
    {
        if (costType == 0)
        {
            if (cfg.unlock_gold > UserDataProxy.inst.playerData.gold)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else
        {
            if (cfg.unlock_diamond > UserDataProxy.inst.playerData.gem)
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, cfg.unlock_diamond - UserDataProxy.inst.playerData.gem);
                return;
            }
        }

        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREUNLOCK, data.groupData.groupId, costType);
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
    }
}
