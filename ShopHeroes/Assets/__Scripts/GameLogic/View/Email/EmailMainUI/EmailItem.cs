using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailItem : MonoBehaviour, IDynamicScrollViewItem
{
    public Button button;
    public GUIIcon icon;
    public Text titleTx;
    public Text fromTx;
    public Text dateTx;
    public Text deadlineTx;
    public GameObject mask;

    EmailData _data;

    int _index;

    private void Start()
    {
        button.onClick.AddListener(onSelfBtnClick);
    }

    public void SetData(EmailData data)
    {
        _data = data;
        gameObject.SetActiveTrue();

        titleTx.text = data.title;
        fromTx.text = data.from;
        dateTx.text = TimeUtils.getDateTimeBySecs(data.dateTime).ToString("yyyy-MM-dd");

        //判断状态
        if (data.state == (int)EMailStatus.Unread || data.state == (int)EMailStatus.Unclaimed)
        {
            icon.SetSprite("email_atlas", data.hasAccessories ? "youjian_weidu1" : "youjian_weidu");
            mask.SetActiveFalse();
        }
        else
        {
            if (data.state == (int)EMailStatus.Read)
            {
                if(!data.hasAccessories)
                    icon.SetSprite("email_atlas", "youjian_icon");
                else
                    icon.SetSprite("email_atlas", "youjian_icon1");
            }

            mask.SetActiveTrue();
        }

        deadlineTx.text = TimeUtils.timeTruncate2Str(data.deadlineTime - TimeUtils.GetNowSeconds()) + LanguageManager.inst.GetValueByKey("后过期");

    }

    void onSelfBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_EmailDetailsUI, _data);
    }

    public void onUpdateItem(int index)
    {
        _index = index;
    }
}
