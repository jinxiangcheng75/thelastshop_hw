using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainLineData
{
    public string panelName;
    public string btnName;

    public MainLineData(string _panelName, string _btnName)
    {
        panelName = _panelName;
        btnName = _btnName;
    }
}

public class MainLineTaskView : ViewBase<MainLineTaskComp>
{
    public override string viewID => ViewPrefabName.MailLineTaskUI;
    public override string sortingLayerName => "top";

    MainlineData data;
    protected override void onInit()
    {
        base.onInit();

        contentPane.contentCanvas.sortingLayerName = "window";
        contentPane.taskBtn.ButtonClickTween(() =>
        {
            // 判断是否是可以领奖状态
            Logger.log("当前任务id是" + MainLineDataProxy.inst.Data.cfg.id + "   他当前的状态是" + MainLineDataProxy.inst.Data.state);
            if (MainLineDataProxy.inst.Data.state == EMainTaskState.CanReward)
            {
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REQUESTMAINLINEREWARD);
            }
            else
            {
                //setOperationStart();
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SHOWMAINLINEINFOUI, data);
            }
        });
    }

    protected override void onShown()
    {
        setTaskData(false);
    }

    public void setTaskData(bool needPlayeAnim)
    {
        if (WorldParConfigManager.inst.GetConfig(164).parameters == 0 || MainLineDataProxy.inst.MainTaskIsAllOver)
        {
            hideTaskContent();
            return;
        }
        data = MainLineDataProxy.inst.Data;
        if (data == null) return;
        if (data.cfg == null) return;

        if (IndoorMapEditSys.inst != null)
        {
            if (!IndoorMapEditSys.inst.isDesigning)
                contentPane.contentCanvas.gameObject.SetActive(true);
        }
        else
        {
            contentPane.contentCanvas.gameObject.SetActive(true);
        }

        contentPane.taskText.text = LanguageManager.inst.GetValueByKey(data.cfg.des) /*+ " " + data.param + "/" + data.limit*/;
        float textHeight = contentPane.taskText.preferredHeight;
        contentPane.bgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, textHeight + 30);
        contentPane.bgLgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, textHeight + 30);
        if (data.state != EMainTaskState.Idle)
        {
            contentPane.bgIcon.SetSprite("mainline_atlas", "zhuxian_jinduneirong2");
            contentPane.gouImg.enabled = true;
            contentPane.taskIcon.iconImage.enabled = false;
            contentPane.taskText.color = Color.green;
            if (!needPlayeAnim)
                contentPane.taskSchedule.fillAmount = 1;
            else
            {
                contentPane.taskSchedule.DOFillAmount(1, 0.5f).OnComplete(() =>
                {
                    playeEffect();
                });
            }
            contentPane.bgIcon.iconImage.material = contentPane.lg4;
            contentPane.bgLgIcon.material = contentPane.lg4;
            contentPane.circleLgIcon.material = contentPane.lg5;
            contentPane.bgLgIcon.enabled = true;
            contentPane.bgIcon.iconImage.enabled = true;
        }
        else
        {
            contentPane.taskIcon.SetSprite(data.cfg.atlas, data.cfg.icon);
            contentPane.bgIcon.SetSprite("mainline_atlas", "zhuxian_jinduneirong1");
            contentPane.gouImg.enabled = false;
            contentPane.taskIcon.iconImage.enabled = true;
            contentPane.taskText.color = Color.white;

            long thisParam = 0, thisLimit = 0;
            if (data.cfg.task_type == 11)
            {
                thisLimit = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(data.limit, data.cfg.condition_id);
                var exploreCfg = ExploreDataProxy.inst.GetGroupDataByGroupId(data.cfg.condition_id);
                if (exploreCfg == null)
                {
                    thisParam = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(1, data.cfg.condition_id);
                }
                else
                {
                    thisParam = ExploreInstanceLvConfigManager.inst.GetExpByCurLevel(exploreCfg.groupData.level, data.cfg.condition_id) + exploreCfg.groupData.exp;
                }
            }
            else if (data.cfg.task_type == 15)
            {
                thisLimit = ShopkeeperUpconfigManager.inst.GetExpVal(data.limit);
                thisParam = ShopkeeperUpconfigManager.inst.GetExpVal((int)UserDataProxy.inst.playerData.level) + UserDataProxy.inst.playerData.CurrExp;
            }
            else
            {
                thisParam = data.param;
                thisLimit = data.limit;
            }

            if (!needPlayeAnim)
                contentPane.taskSchedule.fillAmount = (float)thisParam / thisLimit;
            else
            {
                contentPane.taskSchedule.DOFillAmount((float)thisParam / thisLimit, 0.5f).OnComplete(() =>
                {
                    playeEffect();
                });
            }
            contentPane.bgIcon.iconImage.material = null;
            contentPane.bgLgIcon.material = null;
            contentPane.circleLgIcon.material = null;
            contentPane.bgLgIcon.enabled = false;
            contentPane.bgIcon.iconImage.enabled = true;
        }
    }

    private void playeEffect()
    {
        Vector3 v3 = new Vector3();
        float amountCount = contentPane.taskSchedule.fillAmount;
        float r = contentPane.taskSchedule.GetComponent<RectTransform>().sizeDelta.x / 2 - 15;
        Vector3 startPos = new Vector3(contentPane.taskSchedule.transform.position.x, contentPane.taskSchedule.transform.position.y, 0);
        float x, y;
        if (amountCount <= 0.25f)
        {
            x = amountCount / 0.25f;
            y = 1 - amountCount / 0.25f;
        }
        else if (amountCount <= 0.5f)
        {
            x = 1 - (amountCount - 0.25f) / 0.25f;
            y = 1 - amountCount / 0.25f;
        }
        else if (amountCount <= 0.75f)
        {
            x = -(amountCount - 0.5f) / 0.25f;
            y = ((amountCount - 0.5f) / 0.25f) - 1;
        }
        else
        {
            x = 1 - ((amountCount - 0.75f) / 0.25f);
            y = (amountCount - 0.75f) / 0.25f;
        }

        EffectManager.inst.Spawn(3010, Vector3.zero, (gamevfx) =>
        {
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.SetParent(contentPane.taskSchedule.transform.parent);
            gamevfx.transform.localPosition = contentPane.taskSchedule.transform.localPosition + new Vector3(x * r, y * r);
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 101);
        });
    }

    public void newTaskPlayAnim()
    {
        setTaskData(false);

        var trans = contentPane.contentCanvas.GetComponent<RectTransform>();
        trans.DOAnchorPos3DX(284, 0.8f).From(-600);
    }

    public void hideTaskContent()
    {
        contentPane.contentCanvas.gameObject.SetActive(false);
    }

    public void setTimerReset(bool state)
    {
        if (state)
        {
            if (contentPane.taskBtn != null)
                setTargetTransform(contentPane.taskBtn.transform, false, K_Operation_Finger.Normal);
        }
        contentPane.finger.setBoolState(state);
    }

    public void setFingerActive(bool fingerActive)
    {
        contentPane.finger.setFingerState(fingerActive);
    }

    public void setTargetTransform(Transform targetTrans, bool needSetPosImmediately, K_Operation_Finger type)
    {
        contentPane.finger.setTargetTrans(targetTrans, needSetPosImmediately, type);
    }

    public void setTargetTransform(Vector3 targetTrans, bool needSetPosImmediately, K_Operation_Finger type)
    {
        contentPane.finger.setTargetTrans(targetTrans, needSetPosImmediately, type);
    }

    public void setDialogData(string talkStr)
    {
        if (contentPane.dialog == null)
        {
            Logger.error("主线任务的对话组件是空的");
            return;
        }

        contentPane.dialog.setData(talkStr);
    }

    public void setFloatPrefabPlay(int itemId, long count)
    {
        GameObject go = GameObject.Instantiate(contentPane.prefab.gameObject, contentPane.prefabTrans);
        go.GetComponent<MainLinePrefabItem>().setData(itemId, count);
    }

    protected override void onHide()
    {

    }
}
