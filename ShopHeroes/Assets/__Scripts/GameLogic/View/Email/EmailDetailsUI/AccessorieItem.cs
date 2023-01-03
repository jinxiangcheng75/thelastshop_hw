using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//邮件 附件item
public class AccessorieItem : MonoBehaviour
{
    public GUIIcon bgIcon;
    public GUIIcon icon;
    public GUIIcon bgGrayIcon;
    public Image gouImg;
    public Text numTx;
    public Button selfBtn;

    AccessoryData _data;
    CommonRewardData commonData;

    private void Awake()
    {
        selfBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, selfBtn.transform);
        });
    }

    public void SetData(AccessoryData data, EMailStatus state)
    {
        _data = data;
        commonData = new CommonRewardData(data.itemId, data.count, data.quality, data.itemType);

        gameObject.SetActiveTrue();

        if (data.quality <= 0)
        {
            bgIcon.SetSprite("email_atlas", StaticConstants.GetItemBgIcon[0]);
            bgGrayIcon.SetSprite("email_atlas", StaticConstants.GetItemBgIcon[0]);
        }
        else
        {
            bgIcon.SetSprite("email_atlas", StaticConstants.GetItemBgIcon[data.quality - 1]);
            bgGrayIcon.SetSprite("email_atlas", StaticConstants.GetItemBgIcon[data.quality - 1]);
        }
        icon.SetSprite(data.atlas, data.icon);
        numTx.text = data.count > 0 ? data.count.ToString("N0") : "";

        gouImg.enabled = state == EMailStatus.Read;
        bgGrayIcon.iconImage.enabled = state == EMailStatus.Read;
    }

    public void Clear()
    {
        gameObject.SetActiveFalse();
    }

}
