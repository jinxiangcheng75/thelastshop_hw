using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CumulativeItemUI : MonoBehaviour
{
    public Button clickBtn;
    public GUIIcon icon;
    private CumulativeRewardData _data;
    public CumulativeRewardData Data { get { return _data; } }

    private void Awake()
    {
        clickBtn.ButtonClickTween(() =>
        {
            List<CommonRewardData> allData = new List<CommonRewardData>();
            if (_data.reward_item_id1 != -1)
            {
                CommonRewardData tempData = new CommonRewardData(_data.reward_item_id1, _data.reward_item_num1, 1, _data.reward_type1);
                allData.Add(tempData);
            }
            if (_data.reward_item_id2 != -1)
            {
                CommonRewardData tempData = new CommonRewardData(_data.reward_item_id2, _data.reward_item_num2, 1, _data.reward_type2);
                allData.Add(tempData);
            }
            if (_data.reward_item_id3 != -1)
            {
                CommonRewardData tempData = new CommonRewardData(_data.reward_item_id3, _data.reward_item_num3, 1, _data.reward_type3);
                allData.Add(tempData);
            }

            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONMORETIPS_SETINFO, allData, clickBtn.transform);
        });
    }

    public void InitData(CumulativeRewardData data)
    {
        _data = data;
        if (data.sequence == 3)
        {
            icon.SetSprite("lottery_atlas", "renwu_baoxiang3");
        }
        else
        {
            icon.SetSprite("lottery_atlas", "renwu_baoxiang2");
        }
    }

    public void ClearData()
    {
        _data = null;
    }
}
