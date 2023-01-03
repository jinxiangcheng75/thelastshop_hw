using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailDetailsUI : ViewBase<EmailDetailsUIComp>
{

    public override string viewID => ViewPrefabName.EmailDetailsUI;
    public override string sortingLayerName => "popup";

    private readonly float hasAccessorysScrollViewHeight = 420f;
    private readonly float notHasAccessorysScrollViewHeight = 645f;

    EmailData _data;

    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.readBtn.ButtonClickTween(onClaimedBtnClick);
        contentPane.delBtn.ButtonClickTween(onDelBtnClick);
    }

    public void SetData(EmailData data)
    {
        _data = data;

        contentPane.recipientTx.text = data.receiver;
        contentPane.contentTx.text = data.content;
        contentPane.addresserTx.text = data.from;

        contentPane.contentTx.transform.setLocalY(0);
        contentPane.contentTx.rectTransform.sizeDelta = new Vector2(contentPane.contentTx.rectTransform.sizeDelta.x, contentPane.contentTx.preferredHeight);

        float rectX = (contentPane.scrollViewRect.transform as RectTransform).sizeDelta.x;

        float tempOffset = FGUI.inst.isLandscape ? -120 : 0;
        if (data.hasAccessories)//有附件
        {

            contentPane.AccessoryObj.SetActiveTrue();

            (contentPane.scrollViewRect.transform as RectTransform).sizeDelta = new Vector2(rectX, hasAccessorysScrollViewHeight + tempOffset);
            contentPane.scrollViewRect.enabled = contentPane.contentTx.preferredHeight > hasAccessorysScrollViewHeight + tempOffset;

            for (int i = 0; i < contentPane.fourAccessorieItems.Count; i++)
            {
                if (i < data.accessories.Count)
                {
                    contentPane.fourAccessorieItems[i].SetData(data.accessories[i],(EMailStatus)data.state);
                }
                else
                {
                    contentPane.fourAccessorieItems[i].Clear();
                }
            }

            ////////TODO 暂不考虑超过4个奖励的情况
        }
        else
        {
            contentPane.AccessoryObj.SetActiveFalse();

            
            (contentPane.scrollViewRect.transform as RectTransform).sizeDelta = new Vector2(rectX, notHasAccessorysScrollViewHeight + tempOffset);
            contentPane.scrollViewRect.enabled = contentPane.contentTx.preferredHeight > notHasAccessorysScrollViewHeight + tempOffset;
        }

        checkState();

    }

    void checkState()
    {
        if (_data.state == (int)EMailStatus.Read || !_data.hasAccessories)
        {
            if (_data.state == (int)EMailStatus.Unread && !_data.hasAccessories)
            {
                EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_SINGLEREAD, _data.id);
            }

            //GUIHelper.SetUIGray(contentPane.readBtn.transform, true);
            //contentPane.readBtn.interactable = false;
            //var ex = contentPane.readBtn.GetComponent<ButtonEx>();
            //if (ex != null) ex.enabled = false;
            contentPane.readBtn.gameObject.SetActive(false);
            contentPane.delBtn.gameObject.SetActive(true);
        }
        else
        {
            //GUIHelper.SetUIGray(contentPane.readBtn.transform, false);
            //contentPane.readBtn.interactable = true;
            //var ex = contentPane.readBtn.GetComponent<ButtonEx>();
            //if (ex != null) ex.enabled = true;
            contentPane.readBtn.gameObject.SetActive(true);
            contentPane.delBtn.gameObject.SetActive(false);
        }
    }

    public void NeedRefresh(int mailId)
    {
        if (_data.id == mailId)
        {
            SetData(EmailDataProxy.inst.GetEmailByID(mailId));
        }
    }

    public void NeedHide(int mailId)
    {
        if (_data.id == mailId)
        {
            _data = null;
            hide();
        }
    }

    void onClaimedBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_CLAIMED, _data.id);
    }

    void onDelBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_SINGLEDEL, _data.id);
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
    }
}
