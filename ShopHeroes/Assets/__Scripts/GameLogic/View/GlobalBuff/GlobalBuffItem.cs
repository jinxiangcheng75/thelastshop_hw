using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//全局buff item
public class GlobalBuffItem : MonoBehaviour
{
    public GUIIcon buffIcon;
    public Text effectTx;
    //public Text titleTx;
    public Text remainTimeTx;

    int timer;
    GlobalBuffData _data;

    private void Start()
    {
        var selfBtn = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        if (selfBtn != null)
        {
            selfBtn.onClick.AddListener(onClick);
        }
    }

    private void onClick()
    {
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutGlobalBuff { type = ReceiveInfoUIType.GlobalBuff, buffType = _data.buffType });
    }

    public void SetData(GlobalBuffData data)
    {
        if (data == null) Clear();

        _data = data;
        buffIcon.SetSprite(_data.config.type_atlas, _data.config.type_icon);
        var strs = _data.title_2.Split('+');
        if (strs.Length >= 2)
        {
            effectTx.text = "+" + _data.title_2.Split('+')[1];
        }
        else
        {
            effectTx.text = "";
        }
        //titleTx.text = _data.config.title_1;
        //countdownMethod();
        //setTimer();
        gameObject.SetActiveTrue();
    }

    void setTimer()
    {
        if (timer != 0)
        {
            GameTimer.inst.RemoveTimer(timer);
            timer = 0;
        }

        timer = GameTimer.inst.AddTimer(1, _data.remainTime, countdownMethod);
    }

    void countdownMethod()
    {
        if (_data.remainTime > 0)
        {
            remainTimeTx.text = TimeUtils.timeSpanStrip(_data.remainTime);
        }
        else
        {
            //数据统一刷新 回调中干掉你！
            remainTimeTx.text = TimeUtils.timeSpanStrip(1);
        }
    }

    public void Clear()
    {
        if (timer != 0)
        {
            GameTimer.inst.RemoveTimer(timer);
            timer = 0;
        }
        gameObject.SetActiveFalse();
    }


}
