using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SevenDayAwardItem : MonoBehaviour
{
    public Image gouObj;
    public GameObject effectObj;
    public Text stateText;
    public Text numText;
    public GUIIcon icon;
    public Button selfBtn;
    public Image selectObj;
    public RectTransform iconRect;

    SevenDayGoalData curData;
    CommonRewardData commonData;

    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            if (curData.listState == ESevenDayTaskState.CanReward)
            {
                EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYLISTAWARD, curData.id);
            }
            else if (curData.listState == ESevenDayTaskState.Doing || curData.listState == ESevenDayTaskState.NotUnlock)
            {
                // 介绍事件
                //EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, curData.cfg.reward, selfBtn.transform, (long)curData.cfg.reward_number);
            }
        });
    }

    public void setData(SevenDayGoalData data)
    {
        curData = data;
        //commonData = new CommonRewardData(curData.cfg.reward,curData.cfg.reward_number,curData.cfg.)
        gouObj.enabled = false;
        effectObj.SetActive(false);
        stateText.enabled = false;

        if (data.cfg.reward_number > 1)
            numText.text = "x" + AbbreviationUtility.AbbreviateNumber(data.cfg.reward_number);
        else
            numText.text = "∞";
        numText.enabled = data.listState != ESevenDayTaskState.CanReward;
        var itemCfg = ItemconfigManager.inst.GetConfig(data.cfg.reward);
        if ((ItemType)itemCfg.type == ItemType.EquipmentDrawing)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemCfg.effect);
            icon.SetSprite(equipCfg.atlas, equipCfg.icon);
        }
        else
        {
            icon.SetSprite(itemCfg.atlas, itemCfg.icon);
        }
        iconRect.sizeDelta = new Vector2(90, 90);
        isStart = false;

        if (data.listState == ESevenDayTaskState.NotUnlock)
        {
            GUIHelper.SetUIGray(icon.transform, false);
            icon.iconImage.color = GUIHelper.GetColorByColorHex("#7D7D7D");
        }
        else if (data.listState == ESevenDayTaskState.Rewarded)
        {
            gouObj.enabled = true;
            GUIHelper.SetUIGray(icon.transform, true);
        }
        else if (data.listState == ESevenDayTaskState.Doing)
        {
            GUIHelper.SetUIGray(icon.transform, false);
            icon.iconImage.color = Color.white;
        }
        else if (data.listState == ESevenDayTaskState.CanReward)
        {
            GUIHelper.SetUIGray(icon.transform, false);
            effectObj.SetActive(true);
            icon.iconImage.color = Color.white;
            stateText.enabled = true;
            iconRect.sizeDelta = new Vector2(110, 110);
            isStart = true;
        }
    }

    public void setSelectObj(bool isSelect)
    {
        selectObj.enabled = isSelect;
    }

    float timer = 0;
    int maxCount = 12;
    int isAb = 1;
    bool isStart = false;
    private void Update()
    {
        if (isStart)
        {
            timer += Time.deltaTime;

            if (timer >= 5)
            {
                timer = 0;
                maxCount = 12;
                PlayShakeAnim();
            }
        }
    }

    private void PlayShakeAnim()
    {
        if (maxCount > 0)
        {
            maxCount--;
            isAb = -isAb;
            icon.transform.DORotate(new Vector3(0, 0, isAb * 18 * maxCount / 10), 0.1f * maxCount / 20).OnComplete(() =>
            {
                PlayShakeAnim();
            });
        }
    }

    public void clearData()
    {
        DOTween.Kill(icon.transform);
    }
}
