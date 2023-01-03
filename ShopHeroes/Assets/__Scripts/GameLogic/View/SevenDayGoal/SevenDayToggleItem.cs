using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SevenDayToggleItem : MonoBehaviour
{
    public Image gouObj;
    public Image redPointObj;
    public Image lockObj;
    public Text lableText_off;
    public Text lableText_on;
    public Image offImg;

    public void setItemState(SevenDayGoalData data)
    {
        gouObj.enabled = false;
        redPointObj.enabled = false;
        lockObj.enabled = false;

        lableText_off.rectTransform.anchoredPosition = lableText_on.rectTransform.anchoredPosition = data.listState == ESevenDayTaskState.NotUnlock ? new Vector2(16, 6): new Vector2(36, 6);

        if (data.listState == ESevenDayTaskState.NotUnlock)
        {
            lockObj.enabled = true;
            if (data.cfg.day == SevenDayGoalDataProxy.inst.curDay + 1)
            {
                lockObj.color = Color.white;
                offImg.SetUIGrayColor(1);
            }
            else
            {
                lockObj.color = GUIHelper.GetColorByColorHex("#7D7D7D");
                offImg.SetUIGrayColor(0.5f);
            }

            lableText_off.color = GUIHelper.GetColorByColorHex("#ABABAB");
        }
        else
        {
            lableText_off.color = Color.white;
            bool isAllReceive = true;
            bool haveCanReward = false;
            foreach (var item in data.sevenDayDic.Values)
            {
                if (/*item.state != ESevenDayTaskState.Rewarded && */item.state != ESevenDayTaskState.VIPRewarded)
                {
                    isAllReceive = false;
                    if (item.state == ESevenDayTaskState.CanReward || (item.state == ESevenDayTaskState.Rewarded && SevenDayGoalDataProxy.inst.SevenDayFlag))
                    {
                        haveCanReward = true;
                        break;
                    }
                }
            }

            if (isAllReceive && data.listState == ESevenDayTaskState.Rewarded)
            {
                gouObj.enabled = true;
            }
            else if (haveCanReward || (data.listState == ESevenDayTaskState.CanReward && SevenDayGoalDataProxy.inst.SevenDayFlag))
            {
                if (data.cfg.day <= SevenDayGoalDataProxy.inst.curDay)
                    redPointObj.enabled = true;
            }
        }
    }
}
