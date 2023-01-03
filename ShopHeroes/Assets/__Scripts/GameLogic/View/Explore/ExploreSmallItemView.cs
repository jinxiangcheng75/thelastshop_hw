using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreSmallItemView : MonoBehaviour
{
    public GUIIcon icon;
    public Text nameText;
    public Text numText;
    public Text timeText;
    public GameObject notBossObj;
    public Image bossNameBg;
    public Image bossBgImg;
    public Image grayImg;
    public Image bossGrayImg;
    public Image lockImg;
    public GameObject bossKeyObj;
    public GUIIcon keyIcon;
    public Text lockText;
    public Slider lockSlider;
    public Text lockScheduleText;
    ExploreGroup groupData;
    ExploreItemData data;
    public Button selfBtn;
    int timerId;

    private void Awake()
    {
        selfBtn.ButtonClickTween(() =>
        {
            if (groupData.groupData.groupState == 1)
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUNLOCK_SHOWUI, groupData.groupData.groupId);
            else if (groupData.groupData.groupState == 2)
            {
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREPREPARE_SHOWUI, groupData.groupData.groupId, data.type - 1);
            }
        });
    }

    public void setData(/*ExploreItemData itemData*/ExploreGroup groupData, int exploreIndex, int lockState, int index)
    {
        this.groupData = groupData;
        data = groupData.explores[exploreIndex];
        var instanceCfg = ExploreInstanceConfigManager.inst.GetConfigByGroupId(groupData.groupData.groupId);
        lockImg.enabled = data.unlockState != 1 && groupData.groupData.groupState == 2;
        bossBgImg.enabled = data.type == 4 ? true : false;
        grayImg.enabled = false;
        bossGrayImg.enabled = false;
        bossNameBg.enabled = data.type == 4 ? true : false;
        bossKeyObj.SetActive(groupData.groupData.groupState == 2 && data.type == 4 && data.unlockState == 1);
        lockText.enabled = data.unlockState != 1 && groupData.groupData.groupState == 2;
        lockSlider.gameObject.SetActive(data.unlockState != 1 && groupData.groupData.groupState == 2);
        lockText.text = LanguageManager.inst.GetValueByKey("{0}{1}级解锁", LanguageManager.inst.GetValueByKey(instanceCfg.instance_name), data.unlockLevel.ToString());

        if (data.unlockState != 1)
        {
            long curExp = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(groupData.groupData.level, groupData.groupData.groupId) + groupData.groupData.exp;
            long targetExp = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(data.unlockLevel, groupData.groupData.groupId);

            lockSlider.maxValue = targetExp;
            lockSlider.value = Mathf.Max(targetExp * 0.05f, curExp);
            lockScheduleText.text = LanguageManager.inst.GetValueByKey("经验:") + curExp + "/" + targetExp;
        }

        if (data.type != 4)
        {
            //timeText.gameObject.SetActive(data.unlockState == 0);
            timeText.color = GUIHelper.GetColorByColorHex("#a26574");
            timeText.text = LanguageManager.inst.GetValueByKey("探索区域");
            notBossObj.SetActive(data.unlockState == 1);
            Item item = ItemBagProxy.inst.GetItem(data.id);
            icon.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);
            gameObject.name = item.ID.ToString();
            nameText.text = LanguageManager.inst.GetValueByKey(item.itemConfig.name);
            if (data.unlockState == 1)
            {
                //GUIHelper.SetUIGray(transform.GetChild(0), false);
                numText.text = item.count.ToString() + "/" + UserDataProxy.inst.playerData.pileLimit;
                numText.color = item.count >= UserDataProxy.inst.playerData.pileLimit ? GUIHelper.GetColorByColorHex("#57ff3b") : Color.white;
                selfBtn.interactable = true;
            }
            else
            {
                grayImg.enabled = true;
                //timeText.text = LanguageManager.inst.GetValueByKey("{0}级解锁", data.unlockLevel.ToString());
                //data.unlockLevel + LanguageManager.inst.GetValueByKey("级") + LanguageManager.inst.GetValueByKey("解锁");
                selfBtn.interactable = false;
            }
        }
        else if (data.type == 4)
        {
            //timeText.gameObject.SetActive(true);
            timeText.color = GUIHelper.GetColorByColorHex("#ffffff");
            notBossObj.SetActive(false);
            MonsterConfigData cfg = MonsterConfigManager.inst.GetConfig(data.id);
            gameObject.name = cfg.id.ToString();
            nameText.text = LanguageManager.inst.GetValueByKey(cfg.monster_name);
            icon.SetSprite(cfg.monster_atlas, cfg.monster_icon);
            if (data.unlockState == 1)
            {

                keyIcon.SetSprite("item_atlas", (60000 + groupData.groupData.groupId).ToString());
                //ExploreGroup tempData = ExploreDataProxy.inst.GetGroupDataByGroupId(group);
                if (groupData.groupData.bossExploreState == 1)
                {
                    //GUIHelper.SetUIGray(transform.GetChild(0), false);
                    selfBtn.interactable = true;
                    timeText.text = LanguageManager.inst.GetValueByKey("可挑战");
                    timeText.color = Color.green;
                }
                else if (groupData.groupData.bossExploreState == 2)
                {
                    //GUIHelper.SetUIGray(transform.GetChild(0), true);
                    bossGrayImg.enabled = true;
                    selfBtn.interactable = false;
                    timeText.text = LanguageManager.inst.GetValueByKey("挑战中");
                    timeText.color = Color.yellow;
                }
                else if (groupData.groupData.bossExploreState == 3)
                {
                    //GUIHelper.SetUIGray(transform.GetChild(0), true);
                    bossGrayImg.enabled = true;
                    selfBtn.interactable = false;
                    timeText.color = Color.red;
                    timeText.text = TimeUtils.timeSpanStrip(groupData.groupData.bossRemainTime);
                    if (groupData.groupData.bossRemainTime > 0)
                    {
                        if (timerId == 0)
                        {
                            timerId = GameTimer.inst.AddTimer(1, () =>
                            {
                                if (groupData.groupData.bossRemainTime <= 0)
                                {
                                    timeText.text = "1" + LanguageManager.inst.GetValueByKey("秒");
                                    GameTimer.inst.RemoveTimer(timerId);
                                    timerId = 0;
                                }
                                else
                                    timeText.text = TimeUtils.timeSpanStrip(groupData.groupData.bossRemainTime);
                            });
                        }
                    }
                }
                else if (groupData.groupData.bossExploreState == 4)
                {
                    //GUIHelper.SetUIGray(transform.GetChild(0), true);
                    bossGrayImg.enabled = true;
                    selfBtn.interactable = false;
                    timeText.text = LanguageManager.inst.GetValueByKey("就绪");
                    timeText.color = Color.green;
                }
            }
            else
            {
                bossGrayImg.enabled = true;
                //GUIHelper.SetUIGray(transform.GetChild(0), true);
                selfBtn.interactable = false;
                //timeText.gameObject.SetActive(lockState == 1);
                //timeText.color = GUIHelper.GetColorByColorHex("00ff00");
                timeText.text = LanguageManager.inst.GetValueByKey("挑战头目");
                //timeText.text = LanguageManager.inst.GetValueByKey("{0}级解锁", data.unlockLevel.ToString());
                //data.unlockLevel + LanguageManager.inst.GetValueByKey("级") + LanguageManager.inst.GetValueByKey("解锁");
            }
        }
        setAnim(index);
    }

    private void setAnim(int index)
    {
        //if (GameSettingManager.inst.needShowUIAnim)
        //{
        //    GameTimer.inst.AddTimer(0.3f + 0.12f * index, 1, () =>
        //    {
        //        if (this == null) return;
        //        gameObject.SetActive(true);
        //        var animator = selfBtn.GetComponent<Animator>();
        //        if (animator != null)
        //        {
        //            animator.CrossFade("show", 0f);
        //            animator.Update(0f);
        //            animator.Play("show");
        //        }
        //    });
        //}
        //else
        //{
        //    gameObject.SetActive(true);
        //}

        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
