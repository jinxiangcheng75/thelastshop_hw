using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UnityEngine;
using DG.Tweening;

public class ChatMsgView : ViewBase<ChatMsgUICom>
{
    public override string viewID => ViewPrefabName.ChatMsgUI;
    public override string sortingLayerName => "window";

    private const string NonBreakingSpace = "\u00A0";//不换行空格的Unicode编码

    List<ChatData> currChannelList;
    bool isInitToggle = false;
    protected override void onInit()
    {
        base.onInit();
        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.sendBtn.onClick.AddListener(SendMsg);

        contentPane.scrollView.itemRenderer = this.listitemRenderer;
        //contentPane.scrollView.totalItemCount = 0;
        contentPane.worldToggle.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (isInitToggle)
                {
                    contentPane.worldRedPoint.SetActive(false);
                }

                MesgeTableChange(0);
            }
        });
        contentPane.UnionTogle.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (isInitToggle)
                {
                    contentPane.unionRedPoint.SetActive(false);
                }

                MesgeTableChange(2);
            }
        });
        contentPane.SysTogle.onValueChanged.AddListener((ison) =>
        {
            if (ison)
            {
                if (isInitToggle)
                {
                    contentPane.sysRedPoint.SetActive(false);
                }

                MesgeTableChange(3);
            }
        });

        currTable = SaveManager.inst.GetInt("chatCurrTable");
    }

    private int currTable = 0;
    public void MesgeTableChange(int type)
    {
        if (currTable == type) return;

        contentPane.worldToggle.isOn = type == 0;
        contentPane.UnionTogle.isOn = type == 2;
        contentPane.SysTogle.isOn = type == 3;

        AudioManager.inst.PlaySound(11);
        currTable = type;
        SaveManager.inst.SaveInt("chatCurrTable", currTable);
        EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_CHANNEL_CHANGE, currTable);
    }

    void SendMsg()
    {
        string str = contentPane.inputField.text;
        str = str.Replace(" ", NonBreakingSpace);

        if (str.CompareTo("#GM_") == 0)
        {
            GMCheatCode.inst.isActivate = true;
        }
        EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_SENDMSG, str, currTable);

        contentPane.inputField.text = "";
    }
    protected override void DoShowAnimation()
    {
        //base.DoShowAnimation();
        contentPane.showAnimation.onComplete.AddListener(onShown);
        contentPane.showAnimation.DOPlayForward();
    }
    protected override void DoHideAnimation()
    {
        //  base.DoHideAnimation();
        contentPane.showAnimation.onRewind.AddListener(HideView);
        contentPane.showAnimation.DOPlayBackwards();
    }

    protected override void onShown()
    {
        contentPane.showAnimation.transform.localPosition = Vector3.zero;
        contentPane.showAnimation.onComplete.RemoveAllListeners();

        isInitToggle = true;
        switch (currTable)
        {
            case 0:
                contentPane.worldToggle.isOn = true;
                break;
            case 2:
                contentPane.UnionTogle.isOn = true;
                break;
            case 3:
                contentPane.SysTogle.isOn = true;
                break;
        }


        EventController.inst.TriggerEvent(GameEventType.ChatSysEvent.CHAT_CHANNEL_CHANGE, currTable);

        showRaceLampText();
    }

    public void showRaceLampText()
    {
        float width = contentPane.raceLampText.preferredWidth;
        float endX = -(380 + width);

        contentPane.raceLampObj.SetActive(true);
        contentPane.raceLampTextRect.DOLocalMoveX(endX, width / 60).From(400 + 100).SetEase(Ease.Linear).OnComplete(() =>
           {
               if (contentObject == null) return;
               contentPane.raceLampObj.SetActive(false);
           });
    }

    protected override void onHide()
    {
        ChatDataProxy.inst.clearChatHeadPool();
        contentPane.showAnimation.transform.localPosition = new Vector3(-1000, 0, 0);
        contentPane.showAnimation.onRewind.RemoveAllListeners();
    }

    public void UpdateView(List<ChatData> list)
    {
        if (list == null || list.Count <= 0)
        {
            contentPane.noneMsgText.gameObject.SetActive(true);
            contentPane.scrollView.totalItemCount = 0;
        }
        else
        {
            contentPane.noneMsgText.gameObject.SetActive(false);
            currChannelList = list;
            contentPane.scrollView.totalItemCount = currChannelList.Count;
        }

        if (currTable == 3)
        {
            contentPane.inputField.transform.parent.gameObject.SetActive(false);
            var rectTransform = contentPane.scrollView.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, -216);
            var raceRect = contentPane.raceLampObj.GetComponent<RectTransform>();
            if (raceRect != null)
            {
                if (FGUI.inst.isLandscape)
                {
                    raceRect.anchoredPosition = new Vector2(raceRect.anchoredPosition.x, -349);
                }
                else
                {
                    raceRect.anchoredPosition = new Vector2(raceRect.anchoredPosition.x, -290);
                }
            }
        }
        else
        {
            contentPane.inputField.transform.parent.gameObject.SetActive(true);
            var rectTransform = contentPane.scrollView.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, -330);
            var raceRect = contentPane.raceLampObj.GetComponent<RectTransform>();
            if (raceRect != null)
            {
                if (FGUI.inst.isLandscape)
                {
                    raceRect.anchoredPosition = new Vector2(raceRect.anchoredPosition.x, -453);
                }
                else
                {
                    raceRect.anchoredPosition = new Vector2(raceRect.anchoredPosition.x, -394);
                }
            }
        }

        if (ChatDataProxy.inst.newChatType != -1)
        {
            if (ChatDataProxy.inst.newChatType == 0 && currTable != 0)
                contentPane.worldRedPoint.SetActive(true);
            if (ChatDataProxy.inst.newChatType == 1 && currTable != 2)
                contentPane.unionRedPoint.SetActive(true);
            if (ChatDataProxy.inst.newChatType == 2 && currTable != 3)
                contentPane.sysRedPoint.SetActive(true);

            if (isInitToggle)
                ChatDataProxy.inst.newChatType = -1;
        }
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        ChatItem item = (ChatItem)obj;
        if (index < currChannelList.Count)
        {
            item.gameObject.SetActive(true);
            item.SetData(currTable, currChannelList[index]);
        }
    }
}
