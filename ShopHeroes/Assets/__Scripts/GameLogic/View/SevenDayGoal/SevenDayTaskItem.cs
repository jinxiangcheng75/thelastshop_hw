using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class SevenDayTaskItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GameObject obj_name;
    public Text descText;
    public Text scheduleText;
    public Button goBtn;
    public Button getBtn;
    public Button vipBtn;
    public Text tx_finish;
    public Transform parenTrans;
    public SevenDayTaskAwardItem obj1;
    public SevenDayTaskAwardItem obj2;
    public SevenDayTaskAwardItem obj3;

    public Image grayImg;
    public Image vipLockImg;

    private int index = 0;

    private SevenDayGoalSingle data;

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    private void Awake()
    {
        goBtn.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("HideUI_WelfareUI");
            //EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.HIDEUI_SEVENDAYTUI);
            //EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.JUMPTOOTHERPANEL, data);
            GoOperationManager.inst.StartSevenDayTask(data.cfg.id);
        });
        getBtn.ButtonClickTween(() =>
        {
            if (data.state == ESevenDayTaskState.Rewarded && !SevenDayGoalDataProxy.inst.SevenDayFlag)
            {
                //HotfixBridge.inst.TriggerLuaEvent("ShowUI_MallBuyVipUI");
                return;
            }
            EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYAWARD, data.id);
        });
        vipBtn.ButtonClickTween(() =>
        {
            if (data.state == ESevenDayTaskState.Rewarded && !SevenDayGoalDataProxy.inst.SevenDayFlag)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_SevenDayBuyPassUI");
            }
        });
    }

    public void setData(SevenDayGoalSingle data)
    {
        this.data = data;
        parenTrans.gameObject.SetActive(true);
        tx_finish.text = descText.text = SevenDayGoalDataProxy.inst.setTaskDescByType(data);
        if (data.process >= data.limit)
            scheduleText.text = AbbreviationUtility.AbbreviateNumber(data.limit, 2) + "/" + AbbreviationUtility.AbbreviateNumber(data.limit, 2);
        else
            scheduleText.text = AbbreviationUtility.AbbreviateNumber(data.process, 2) + "/" + AbbreviationUtility.AbbreviateNumber(data.limit, 2);
        if (data.state == ESevenDayTaskState.VIPRewarded)
        {
            grayImg.enabled = true;
        }
        else
        {
            grayImg.enabled = false;
        }

        setAwardData();
        setButtonData();
    }

    private void setAwardData()
    {
        if (obj1.gameObject.activeSelf)
            obj1.gameObject.SetActive(false);
        if (obj2.gameObject.activeSelf)
            obj2.gameObject.SetActive(false);
        if (obj3.gameObject.activeSelf)
            obj3.gameObject.SetActive(false);

        if (data.cfg.type_reward1 != -1 && data.cfg.reward1 != -1 && data.cfg.reward1_number != -1)
        {
            obj1.gameObject.SetActive(true);
            //GUIHelper.SetUIGray(obj1.transform, (data.state == ESevenDayTaskState.Rewarded || data.state == ESevenDayTaskState.VIPRewarded));
            obj1.setData(data.cfg.type_reward1, data.cfg.reward1, data.cfg.reward1_number, (data.state == ESevenDayTaskState.Rewarded || data.state == ESevenDayTaskState.VIPRewarded), false);
        }
        if (data.cfg.type_reward2 != -1 && data.cfg.reward2 != -1 && data.cfg.reward2_number != -1)
        {
            obj2.gameObject.SetActive(true);
            //GUIHelper.SetUIGray(obj2.transform, (data.state == ESevenDayTaskState.Rewarded || data.state == ESevenDayTaskState.VIPRewarded));
            obj2.setData(data.cfg.type_reward2, data.cfg.reward2, data.cfg.reward2_number, (data.state == ESevenDayTaskState.Rewarded || data.state == ESevenDayTaskState.VIPRewarded), false);
        }
        if (data.cfg.type_reward3 != -1 && data.cfg.reward3 != -1 && data.cfg.reward3_number != -1)
        {
            obj3.gameObject.SetActive(true);
            //GUIHelper.SetUIGray(obj3.transform, data.state == ESevenDayTaskState.VIPRewarded);
            vipLockImg.enabled = !SevenDayGoalDataProxy.inst.SevenDayFlag;
            obj3.setData(data.cfg.type_reward3, data.cfg.reward3, data.cfg.reward3_number, data.state == ESevenDayTaskState.VIPRewarded, !SevenDayGoalDataProxy.inst.SevenDayFlag);
        }
    }

    private void setButtonData()
    {
        goBtn.interactable = data.cfg.day <= SevenDayGoalDataProxy.inst.curDay;
        getBtn.interactable = data.cfg.day <= SevenDayGoalDataProxy.inst.curDay;
        vipBtn.interactable = data.cfg.day <= SevenDayGoalDataProxy.inst.curDay;
        GUIHelper.SetUIGray(goBtn.transform, data.cfg.day > SevenDayGoalDataProxy.inst.curDay);
        GUIHelper.SetUIGray(getBtn.transform, data.cfg.day > SevenDayGoalDataProxy.inst.curDay);
        GUIHelper.SetUIGray(vipBtn.transform, data.cfg.day > SevenDayGoalDataProxy.inst.curDay);
        goBtn.gameObject.SetActive(data.state == ESevenDayTaskState.Doing || data.state == ESevenDayTaskState.NotUnlock);
        getBtn.gameObject.SetActive(data.state == ESevenDayTaskState.CanReward || (SevenDayGoalDataProxy.inst.SevenDayFlag && data.state == ESevenDayTaskState.Rewarded));
        vipBtn.gameObject.SetActive(data.state == ESevenDayTaskState.Rewarded && !SevenDayGoalDataProxy.inst.SevenDayFlag);
        //getBtn.interactable = (data.state == ESevenDayTaskState.CanReward || data.state == ESevenDayTaskState.Rewarded);

        tx_finish.enabled = data.state == ESevenDayTaskState.VIPRewarded;
        obj_name.SetActive(data.state != ESevenDayTaskState.VIPRewarded);

    }
}
