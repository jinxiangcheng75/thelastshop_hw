using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class AcheivementItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon icon;
    public Text pointNumText;
    public Text nameText;
    public Text descText;
    public Slider slider;
    public Text scheduleText;
    public GUIIcon awardIcon;
    public Text awardNumText;
    public Button selfBtn;
    public Image normalImg;
    public Image gouImg;
    //public Button rewardBtn;
    public RectTransform btnRecttrans;
    public Image grayImg;
    public Image canGetImg;

    public GameObject c_obj;
    public GameObject lua_obj;

    public int index = 0;
    private AcheivementData data;

    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            if (data.state == EAchievementState.Done)
            {
                EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTAWARD, data.id);
            }
        });
        //rewardBtn.onClick.AddListener(() =>
        //{
        //    if (data.state == EAchievementState.Doing)
        //        EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, data.itemCfg.id, rewardBtn.transform, (long)data.rewardNum);
        //});
    }

    public void setData(AcheivementData _data)
    {
        data = _data;

        icon.SetSprite(data.atlas, data.icon);
        pointNumText.text = data.rewardPoints.ToString();
        nameText.text = LanguageManager.inst.GetValueByKey(data.name);
        string replaceStr = string.Empty;
        if (data.group == 11)
        {
            var workerCfg = WorkerConfigManager.inst.GetConfig(data.condition_type);
            replaceStr = LanguageManager.inst.GetValueByKey(workerCfg.name);
        }
        else
        {
            replaceStr = LanguageManager.inst.GetValueByKey(data.condition_des);
        }
        descText.text = LanguageManager.inst.GetValueByKey(data.desc, replaceStr);
        slider.maxValue = data.limit;
        slider.value = data.process;
        string limitStr = data.group == 123 ? "1" : AbbreviationUtility.AbbreviateNumber(data.limit, 0);
        string processStr = data.group == 123 ? data.state == EAchievementState.Done ? "1" : "0" : AbbreviationUtility.AbbreviateNumber(data.process, 0);
        if (data.process > data.limit)
        {
            scheduleText.text = limitStr + "/" + limitStr;
        }
        else
        {
            scheduleText.text = processStr + "/" + limitStr;
        }

        awardIcon.SetSprite(data.itemCfg.atlas, data.itemCfg.icon);
        awardNumText.text = data.rewardNum.ToString();

        setState();
    }

    private void setState()
    {
        gouImg.enabled = false;
        //awardIcon.gameObject.SetActive(true);
        awardNumText.enabled = true;
        selfBtn.gameObject.SetActive(true);
        switch (data.state)
        {
            case EAchievementState.Doing:
                GUIHelper.SetUIGray(selfBtn.transform, false);
                canGetImg.enabled = false;
                grayImg.enabled = false;
                normalImg.enabled = true;
                selfBtn.GetComponent<Image>().enabled = false;
                slider.gameObject.SetActive(true);
                btnRecttrans.anchoredPosition = new Vector2(btnRecttrans.anchoredPosition.x, -30);
                break;
            case EAchievementState.Done:
                GUIHelper.SetUIGray(selfBtn.transform, false);
                canGetImg.enabled = true;
                grayImg.enabled = false;
                slider.gameObject.SetActive(false);
                selfBtn.GetComponent<Image>().enabled = true;
                btnRecttrans.anchoredPosition = new Vector2(btnRecttrans.anchoredPosition.x, 0);
                break;
            case EAchievementState.Rewarded:
                grayImg.enabled = true;
                canGetImg.enabled = false;
                awardNumText.enabled = false;
                normalImg.enabled = true;
                gouImg.enabled = true;
                selfBtn.gameObject.SetActive(false);
                //awardIcon.gameObject.SetActive(false);
                slider.gameObject.SetActive(false);
                descText.text = LanguageManager.inst.GetValueByKey("完成");
                break;
        }
    }

    public void clearData()
    {

    }

    public void onUpdateItem(int index)
    {
        this.index = index;
    }
}
