using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorePrepareItemView : MonoBehaviour
{
    public GUIIcon icon;
    public Text timeText;
    public Toggle selfToggle;
    int timerId;
    ExploreItemData data;

    public void setData(ExploreItemData _data, int group)
    {
        data = _data;

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (data.type != 4)
        {
            itemConfig cfg = ItemconfigManager.inst.GetConfig(data.id);
            timeText.text = LanguageManager.inst.GetValueByKey(cfg.name);
            icon.SetSprite(cfg.atlas, cfg.icon);
        }
        else
        {
            MonsterConfigData cfg = MonsterConfigManager.inst.GetConfig(data.id);
            icon.SetSprite(cfg.monster_atlas, cfg.monster_icon);
        }

        if (data.unlockState == 0)
        {
            timeText.text = LanguageManager.inst.GetValueByKey("{0}级解锁", data.unlockLevel.ToString());
            //data.unlockLevel + LanguageManager.inst.GetValueByKey("级") + LanguageManager.inst.GetValueByKey("解锁");
            selfToggle.interactable = false;
            GUIHelper.SetUIGray(selfToggle.transform, true);
        }
        else
        {
            if (data.type != 4)
            {
                //timeText.gameObject.SetActive(false);
                selfToggle.interactable = true;
                GUIHelper.SetUIGray(selfToggle.transform, false);
            }
            else
            {
                ExploreGroup groupData = ExploreDataProxy.inst.GetGroupDataByGroupId(group);
                if (groupData.groupData.bossExploreState != 1)
                {
                    selfToggle.interactable = false;
                    GUIHelper.SetUIGray(selfToggle.transform, true);
                    if (groupData.groupData.bossRemainTime > 0)
                    {
                        timeText.text = TimeUtils.timeSpan2Str(groupData.groupData.bossRemainTime);
                        timeText.gameObject.SetActive(true);
                        if (timerId == 0)
                        {
                            timerId = GameTimer.inst.AddTimer(1, () =>
                            {
                                if (groupData.groupData.bossRemainTime <= 0)
                                    timeText.text = TimeUtils.timeSpan2Str(1);
                                else
                                    timeText.text = TimeUtils.timeSpan2Str(groupData.groupData.bossRemainTime);
                            });
                        }
                    }
                    else
                        timeText.gameObject.SetActive(false);
                }
                else
                {
                    timeText.gameObject.SetActive(false);
                    selfToggle.interactable = true;
                    GUIHelper.SetUIGray(selfToggle.transform, false);
                }
            }
        }
    }

    public void clearData()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
