using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class ChangeNameView : ViewBase<ChangeNameComp>
{
    public override string viewID => ViewPrefabName.ChangeNamePanel;
    public override string sortingLayerName => "window";

    string sensitiveWords = "[`~!#$^&*()=|{}':;',\\[\\].<>/?~！#￥……&*（）——|{}【】‘；：”“'。，、？]_-+\"";

    RectTransform textRect;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;

        contentPane.changeBtn.ButtonClickTween(() => ChangeNameClick(UserDataProxy.inst.playerData.freeNameCount > 0));
        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.nameInput.onValueChanged.AddListener((str) =>
        {
            for (int i = 0; i < sensitiveWords.Length; i++)
            {
                if (str.Contains(sensitiveWords[i].ToString()))
                {
                    int index = str.IndexOf(sensitiveWords[i]);
                    str = str.Remove(index);
                    contentPane.nameInput.text = str;
                }
            }
        });
        InitPanelUI();

        if (contentPane.timeText != null)
        {
            textRect = contentPane.timeText.gameObject.GetComponent<RectTransform>();
        }
    }

    private void InitPanelUI()
    {
        JudgeIsFreeToChange();
    }

    private void JudgeIsFreeToChange()
    {
        if (UserDataProxy.inst.playerData.freeNameCount > 0)
        {
            contentPane.gemObj.enabled = false;
            if (textRect != null)
            {
                textRect.anchoredPosition = new Vector2(0, textRect.anchoredPosition.y);
            }
            contentPane.timeText.text = LanguageManager.inst.GetValueByKey("免费剩余") + UserDataProxy.inst.playerData.freeNameCount;
        }
        else
        {
            var wdpCfg = WorldParConfigManager.inst.GetConfig(190);
            if (wdpCfg != null)
            {
                contentPane.timeText.text = wdpCfg.parameters.ToString();
                //if (UserDataProxy.inst.playerData.gem >= wdpCfg.parameters)
                //    contentPane.timeText.color = GUIHelper.GetColorByColorHex("FFFFFF");
                //else
                //    contentPane.timeText.color = GUIHelper.GetColorByColorHex("FF0000");

                contentPane.titleText.text = LanguageManager.inst.GetValueByKey("之后每次更换将花费{0}金块。", wdpCfg.parameters.ToString());
            }
            else
            {
                contentPane.timeText.text = "";
                contentPane.titleText.text = "";
            }

            contentPane.greenImg.enabled = false;
            if (textRect != null)
            {
                textRect.anchoredPosition = new Vector2(24, textRect.anchoredPosition.y);
            }
            contentPane.applyText.text = LanguageManager.inst.GetValueByKey("改变");
            contentPane.gemObj.enabled = true;

        }
    }

    private void ChangeNameClick(bool isFree)
    {
        if (isFree)
        {
            if (contentPane.nameInput.text != "" && contentPane.nameInput.text != null)
            {
                if (UserDataProxy.inst.playerData.playerName == contentPane.nameInput.text)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("不能和当前姓名一致"), Color.white);
                }
                else
                {
                    int strLen = Helper.GetStringRealLen(contentPane.nameInput.text);
                    if (strLen < 2 || strLen > contentPane.nameInput.characterLimit)
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称长度需要处于2与14字符之间"), GUIHelper.GetColorByColorHex("FF2828"));
                        return;
                    }
                    string outStr;
                    if (WordFilter.inst.filter(contentPane.nameInput.text, out outStr, check_only: true))
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称中包含敏感内容"), GUIHelper.GetColorByColorHex("FF2828"));
                        return;
                    }

                    NetworkEvent.SendRequest(new NetworkRequestWrapper()
                    {
                        req = new Request_User_ChangeName()
                        {
                            nickName = contentPane.nameInput.text
                        }
                    });
                }
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称不能为空"), GUIHelper.GetColorByColorHex("FF2828"));
            }
        }
        else
        {
            if (!contentPane.greenImg.isActiveAndEnabled)
            {
                contentPane.greenImg.enabled = true;
                contentPane.applyText.text = LanguageManager.inst.GetValueByKey("确认");
            }
            else
            {
                if (contentPane.nameInput.text == "" || contentPane.nameInput.text == null)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称不能为空"), GUIHelper.GetColorByColorHex("FF2828"));
                    contentPane.greenImg.enabled = false;
                    contentPane.applyText.text = LanguageManager.inst.GetValueByKey("改变");
                }
                else
                {
                    // 判断钻石是否够worldPar表格里面id为190的参数个 够的话前段扣钻石 发送消息给后端 不够直接提醒钻石数量不够
                    var wdpCfg = WorldParConfigManager.inst.GetConfig(190);
                    if (wdpCfg == null)
                    {
                        return;
                    }
                    if (UserDataProxy.inst.playerData.gem >= wdpCfg.parameters)
                    {
                        if (UserDataProxy.inst.playerData.playerName == contentPane.nameInput.text)
                        {
                            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("和当前姓名一致"), Color.white);
                        }
                        else
                        {
                            int strLen = Helper.GetStringRealLen(contentPane.nameInput.text);
                            if (strLen < 2 || strLen > contentPane.nameInput.characterLimit)
                            {
                                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称长度需要处于2与14字符之间"), GUIHelper.GetColorByColorHex("FF2828"));
                                return;
                            }
                            string outStr;
                            if (WordFilter.inst.filter(contentPane.nameInput.text, out outStr, check_only: true))
                            {
                                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称中包含敏感内容"), GUIHelper.GetColorByColorHex("FF2828"));
                                return;
                            }
                            NetworkEvent.SendRequest(new NetworkRequestWrapper()
                            {
                                req = new Request_User_ChangeName()
                                {
                                    nickName = contentPane.nameInput.text
                                }
                            });
                        }
                    }
                    else
                    {
                        //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条数量不够"), GUIHelper.GetColorByColorHex("FF2828"));
                        contentPane.greenImg.enabled = false;
                        contentPane.applyText.text = LanguageManager.inst.GetValueByKey("改变");

                        HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, (int)wdpCfg.parameters - UserDataProxy.inst.playerData.gem);
                    }
                }
            }
        }
    }

    protected override void onHide()
    {
        base.onHide();
        contentPane.nameInput.text = "";
        //AudioManager.inst.PlaySound(11);
    }

    protected override void onShown()
    {
        base.onShown();
        InitPanelUI();
        //AudioManager.inst.PlaySound(10);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        contentPane.greenImg.enabled = false;
        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }
}
