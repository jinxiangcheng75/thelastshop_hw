using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleRestingView : ViewBase<RoleRestingComp>
{
    public override string viewID => ViewPrefabName.RoleRestingUI;
    public override string sortingLayerName => "popup";
    public int restType; // 0 - 单个 1 - 多个
    int isFromHeroInfo;
    RoleHeroData data;
    int timerId = 0;
    int gemNum = 0;
    protected override void onInit()
    {
        base.onInit();

        AddUIEvent();
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(closePanel);
        contentPane.bgBtn.onClick.AddListener(closePanel);
        contentPane.allItemBtn.ButtonClickTween(() => useItemMethod(false, false));
        contentPane.allGemBtn.ButtonClickTween(() =>
        {
            if (contentPane.sureAgainObj.activeSelf)
            {
                useItemMethod(false, true);
                
            }
            else
                contentPane.sureAgainObj.SetActive(true);
        });
        contentPane.singleItemBtn.ButtonClickTween(() => useItemMethod(true, false));
        contentPane.singleGemBtn.ButtonClickTween(() =>
        {
            if (contentPane.singleSureAgainObj.activeSelf)
            {
                useItemMethod(true, true);
            }
            else
                contentPane.singleSureAgainObj.SetActive(true);
        });
        contentPane.singleFinishBtn.ButtonClickTween(closePanel);
    }

    private void useItemMethod(bool isSingle, bool isUseGem)
    {
        if (!isUseGem)
        {
            if (isSingle)
            {
                if (ItemBagProxy.inst.GetItem(150002).count <= 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("道具数量不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HERORECOVER, data.uid, 1);
            }
            else
            {
                if (ItemBagProxy.inst.GetItem(150001).count <= 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("道具数量不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    return;
                }
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HERORECOVER, 0, 1);
            }
        }
        else
        {
            if (UserDataProxy.inst.playerData.gem >= gemNum)
            {
                if (isSingle)
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HERORECOVER, data.uid, 10002);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_HERORECOVER, 0, 10002);
                }
            }
            else
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, gemNum - UserDataProxy.inst.playerData.gem);
                return;
            }
        }
    }

    private void closePanel()
    {
        hide();
        //if (restType == 0 && isFromHeroInfo == 1)
        //{
        //    EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINFO_SHOWUI, data.uid);
        //}
    }

    public void setSingleData(RoleHeroData roleData, int isFromHeroInfo)
    {
        data = roleData;
        restType = 0;
        this.isFromHeroInfo = isFromHeroInfo;
        contentPane.allObj.SetActive(false);
        contentPane.singleObj.SetActive(true);
        contentPane.topText.text = LanguageManager.inst.GetValueByKey("恢复{0}", LanguageManager.inst.GetValueByKey(roleData.nickName));
        //LanguageManager.inst.GetValueByKey("恢复") + roleData.nickName;
        contentPane.singleRestingText.text = LanguageManager.inst.GetValueByKey("{0}将准备好再次去冒险。", LanguageManager.inst.GetValueByKey(roleData.nickName));
        //roleData.nickName + LanguageManager.inst.GetValueByKey("将准备好再次去冒险。");
        contentPane.singleItemNumText.color = ItemBagProxy.inst.GetItem(150002).count >= 1 ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("FF0000");

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (timerId == 0)
        {
            if (data.remainTime > 0)
            {
                contentPane.notFinishObj.SetActiveTrue();
                contentPane.finishObj.SetActiveFalse();
                int result = DiamondCountUtils.GetHeroRestingFastCost(data.remainTime);
                gemNum = result;
                contentPane.singleGemNumText.text = result.ToString("N0");
                //contentPane.singleGemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
                timerId = GameTimer.inst.AddTimer(1, () =>
                {
                    if (data.remainTime <= 0)
                    {
                        GameTimer.inst.RemoveTimer(timerId);
                        timerId = 0;
                    }
                    else
                    {
                        result = DiamondCountUtils.GetHeroRestingFastCost(data.remainTime);
                        gemNum = result;
                        contentPane.singleGemNumText.text = result.ToString("N0");
                        //contentPane.singleGemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
                    }
                });
            }
            else
            {
                if (data.currentState == 0)
                {
                    contentPane.notFinishObj.SetActiveFalse();
                    contentPane.finishObj.SetActiveTrue();
                }
            }
        }
    }

    public void setAllData()
    {
        restType = 1;
        contentPane.allObj.SetActive(true);
        contentPane.singleObj.SetActive(false);
        contentPane.itemNumText.color = ItemBagProxy.inst.GetItem(150001).count >= 1 ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("FF0000");

        List<RoleHeroData> tempList = RoleDataProxy.inst.GetRestingStateHeroCount();
        if (tempList.Count <= 0)
        {
            hide();
            return;
        }

        int childCount = contentPane.restingRoleContent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            int index = i;
            GameObject restingObj = contentPane.restingRoleContent.GetChild(index).gameObject;
            if (index < tempList.Count)
            {
                restingObj.SetActive(true);
                if (index == childCount - 1)
                {
                    contentPane.restingCountText.text = "+" + (tempList.Count - 3);
                }
                else
                {
                    restingObj.GetComponent<RoleRestingItemUI>().setData(tempList[index]);
                }
            }
            else
            {
                restingObj.SetActive(false);
            }
        }

        int result = 0;
        for (int i = 0; i < tempList.Count; i++)
        {
            result += DiamondCountUtils.GetHeroRestingFastCost(tempList[i].remainTime);
        }
        //int result = DiamondCountUtils.GetHeroRestingFastCost(allRemainTime);
        gemNum = result;
        contentPane.gemNumText.text = result.ToString("N0");
        //contentPane.gemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (timerId == 0)
        {
            timerId = GameTimer.inst.AddTimer(1, () =>
             {
                 result = 0;
                 for (int i = 0; i < tempList.Count; i++)
                 {
                     result += DiamondCountUtils.GetHeroRestingFastCost(tempList[i].remainTime);
                 }
                 //result = DiamondCountUtils.GetHeroRestingFastCost(allRemainTime);
                 gemNum = result;
                 contentPane.gemNumText.text = result.ToString("N0");
                 //contentPane.gemNumText.color = UserDataProxy.inst.playerData.gem >= result ? Color.white : Color.red;
             });
        }
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
        foreach (Transform item in contentPane.restingRoleContent)
        {
            if (item.gameObject.GetComponent<RoleRestingItemUI>() != null)
                item.gameObject.GetComponent<RoleRestingItemUI>().clearData();
        }
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        foreach (Transform item in contentPane.restingRoleContent)
        {
            if (item.gameObject.GetComponent<RoleRestingItemUI>() != null)
                item.gameObject.GetComponent<RoleRestingItemUI>().clearData();
        }

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        contentPane.sureAgainObj.SetActive(false);
        contentPane.singleSureAgainObj.SetActive(false);
    }
}
