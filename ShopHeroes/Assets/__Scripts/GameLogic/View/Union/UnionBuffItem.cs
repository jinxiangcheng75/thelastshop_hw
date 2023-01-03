using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionBuffItem : MonoBehaviour
{
    public Button button;
    public GUIIcon unionBuffIcon;
    public Image ringImg;

    public int big_type;

    [SerializeField]
    private bool isMemberOfUnionMainUI;

    int timerId;
    UnionBuffData _data;

    private void Start()
    {
        button.ButtonClickTween(onButtonClick);
    }


    public void UpdateData()
    {
        _data = UserDataProxy.inst.GetUnionBuffData((EUnionScienceType)big_type);

        clearTimer();

        if (_data == null)
        {
            gameObject.SetActive(false);
            return;
        }


        if (_data.remainTime <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            unionBuffIcon.SetSprite(_data.config.atlas, _data.config.icon);
            setTimer();
        }

    }

    void clearTimer()
    {
        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    void setTimer()
    {
        ringImg.fillAmount = _data.remainTime / (_data.config.skill_time * 60f);

        timerId = GameTimer.inst.AddTimer(1, () =>
        {
            if (this == null || ringImg == null)
            {
                clearTimer();
                return;
            }

            ringImg.fillAmount = _data.remainTime / (_data.config.skill_time * 60f);

            if (_data == null || _data.remainTime <= 0)
            {
                if (null == this) return;

                clearTimer();
                gameObject.SetActive(false);

            }

        });
    }

    void onButtonClick()
    {

        if (isMemberOfUnionMainUI)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_BUFFDETAIL, transform as RectTransform, _data);
        }
        else
        {           
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionBuffDesPanel",transform, _data);
        }
    }

}
