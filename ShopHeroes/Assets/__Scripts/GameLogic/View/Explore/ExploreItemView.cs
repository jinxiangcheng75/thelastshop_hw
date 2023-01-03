using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class ExploreItemView : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon icon;
    public GUIIcon nextUnlockIcon;
    public Button upgradeBtn;
    public Text nameText;
    public Text levelText;
    public Text scheduleText;
    public Text lockText;
    public Text lockNameText;
    public Slider scheduleSlider;
    public GameObject lockObj;
    public Text lockNotMoneyText;
    public GameObject unlockObj;
    public Transform topTrans;
    public List<ExploreSmallItemView> smallItems;
    ExploreGroup data;
    public GameObject nextUpObj;
    public GameObject nextObj;
    public GameObject unlockMoneyObj;
    public Text goldText;
    public Image grayImg;
    public int index = 0;
    private void Awake()
    {
        //selfBtn.onClick.AddListener(() =>
        //{
        //    if (data.groupData.groupState == 1)
        //        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUNLOCK_SHOWUI, data.groupData.groupId);
        //    else if (data.groupData.groupState == 2)
        //        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREPREPARE_SHOWUI, data.groupData.groupId);
        //});
        upgradeBtn.onClick.AddListener(() =>
        {
            if (data.groupData.groupState == 2)
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREINFO_SHOWUI, data.groupData.groupId);
            else if (data.groupData.groupState == 1)
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUNLOCK_SHOWUI, data.groupData.groupId);
        });
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public void setData(ExploreGroup groupData, int index)
    {
        data = groupData;
        int lockState = 0;
        ExploreInstanceConfigData cfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(data.groupData.groupId);
        nameText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name);
        lockNameText.text = LanguageManager.inst.GetValueByKey(cfg.instance_name);
        icon.SetSprite(StaticConstants.exploreAtlas, cfg.instance_icon);
        levelText.text = LanguageManager.inst.GetValueByKey("{0}级", data.groupData.level.ToString());
        lockNotMoneyText.enabled = false;
        grayImg.enabled = false;
        if (groupData.groupData.groupState == 2)
        {
            lockState = 1;
            //GUIHelper.SetUIGray(topTrans, false);
            unlockObj.SetActive(true);
            lockObj.SetActive(false);
            upgradeBtn.gameObject.SetActive(true);
            ExploreInstanceLvConfigData nextLvCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(data.groupData.groupId, data.groupData.level + 1);

            if (nextLvCfg != null)
            {
                nextObj.SetActiveTrue();
                if (nextLvCfg.effect_type == 1 || nextLvCfg.effect_type == 6)
                    nextUpObj.SetActiveTrue();
                else
                    nextUpObj.SetActiveFalse();

                if (nextLvCfg.effect_type == 5)
                {
                    var exploreCfg = ExploreInstanceConfigManager.inst.GetConfig(nextLvCfg.effect_id[0]);
                    if (cfg.instance_group != exploreCfg.instance_group)
                    {
                        nextUnlockIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon, needSetNativeSize: true);
                    }
                    else
                    {
                        nextUnlockIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                        nextUnlockIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon);
                    }
                }
                else
                {
                    nextUnlockIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    nextUnlockIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon);
                }

                //nextUnlockIcon.SetSprite(nextLvCfg.effect_atlas, nextLvCfg.effect_icon);
                scheduleText.text = data.groupData.exp + "/" + nextLvCfg.need_instance_exp;
                scheduleSlider.maxValue = nextLvCfg.need_instance_exp;
                scheduleSlider.value = Mathf.Max(nextLvCfg.need_instance_exp * 0.05f, groupData.groupData.exp);
            }
            else
            {
                nextObj.SetActiveFalse();
                scheduleText.text = "max";
                scheduleSlider.maxValue = 1;
                scheduleSlider.value = 1;
            }
        }
        else if (groupData.groupData.groupState == 1)
        {
            lockState = 1;
            //GUIHelper.SetUIGray(topTrans, false);
            unlockObj.SetActive(false);
            lockObj.SetActive(true);
            upgradeBtn.gameObject.SetActive(true);
            lockText.text = LanguageManager.inst.GetValueByKey(LanguageManager.inst.GetValueByKey("花费"));
            unlockMoneyObj.SetActive(true);
            var lvCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(data.groupData.groupId, 1);
            goldText.text = lvCfg.unlock_gold.ToString();
            goldText.color = lvCfg.unlock_gold > UserDataProxy.inst.playerData.gold ? Color.red : Color.white;
        }
        else if (groupData.groupData.groupState == 0)
        {
            lockState = 0;
            grayImg.enabled = true;
            lockNotMoneyText.enabled = true;
            //GUIHelper.SetUIGray(topTrans, true);
            unlockObj.SetActive(false);
            lockObj.SetActive(true);
            upgradeBtn.gameObject.SetActive(false);
            ExploreInstanceConfigData tempData = ExploreInstanceConfigManager.inst.GetConfigByGroupId(groupData.groupData.groupId - 1);
            lockText.text = LanguageManager.inst.GetValueByKey("{0}到达{1}级可解锁", LanguageManager.inst.GetValueByKey(tempData.instance_name), data.unlockNextExploreLevel.ToString());
            unlockMoneyObj.SetActive(false);
            //LanguageManager.inst.GetValueByKey(tempData.instance_name) + LanguageManager.inst.GetValueByKey("到达") + data.unlockNextExploreLevel + LanguageManager.inst.GetValueByKey("级") + LanguageManager.inst.GetValueByKey("可解锁");
        }

        setSmallItemData(lockState, index);
    }
    private void setSmallItemData(int lockState, int bigIndex)
    {
        for (int i = 0; i < smallItems.Count; i++)
        {
            int index = i;
            smallItems[index].gameObject.SetActiveFalse();
            smallItems[index].setData(data, index, lockState, bigIndex);
        }
    }
}
