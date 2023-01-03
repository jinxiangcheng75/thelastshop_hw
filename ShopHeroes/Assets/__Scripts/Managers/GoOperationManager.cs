using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoOperationManager : TSingletonHotfix<GoOperationManager>
{
    public int operationIsStart = 0;
    //bool operationIsStart = false;
    OperationData curData = null;
    List<int> operationIds = new List<int>();
    int index = 0;

    public bool isInitOperation;
    public bool isInitShopperData = false;
    public bool isDoing = false;

    private K_Operation_Type curOperationType;

    public OperationData CurData
    {
        get { return curData; }
        private set { }
    }
    protected override void init()
    {

    }

    public void StartHyperLink(int helplinkid)
    {
        OperationData data = new OperationData();
        data.setData(K_Operation_DataType.HyperLink, helplinkid);
        StartJumpOperation(data);
    }
    public void StartMainline(int mainlineId)
    {
        OperationData data = new OperationData();
        data.setData(K_Operation_DataType.MainLine, mainlineId);
        StartJumpOperation(data);
    }
    public void StartSevenDayTask(int sevenDayTaskId)
    {
        OperationData operationData = new OperationData();
        operationData.setData(K_Operation_DataType.SevenDay, sevenDayTaskId);
        StartJumpOperation(operationData);
    }
    public void StartNoviceTask(int taskId)
    {

    }

    public void StartDailyTask(int taskId)
    {

    }
    public void StartJumpOperation(OperationData data)
    {
        GUIManager.BackMainView();
        isDoing = true;
        operationIsStart = 1;
        isInitOperation = false;
        curData = data;
        operationIds = new List<int>(curData.operations);
        index = 0;
        if (GuideManager.inst.isInTriggerGuide) return;
        if (operationIds.Count <= 0) return;
        InitOperationData();
        runOperation(operationIds[index]);
    }

    private void InitOperationData()
    {
        if (curData == null) return;

        if (curData.task_scenes == "IndoorScene")
        {
            if (ManagerBinder.inst.mGameState == kGameState.Town)
            {
                operationIds.Insert(0, 2);
            }
        }

        if (curData.task_scenes == "CityScene")
        {
            if (ManagerBinder.inst.mGameState == kGameState.Shop)
            {
                operationIds.Insert(0, 1);
            }
        }
    }

    private void runOperation(int id)
    {
        if (curData == null) return;
        // 操作表cfg
        var cfg = OperationConfigManager.inst.GetConfig(id);
        //Logger.error("当前的操作是" + cfg.id + "    type" + (K_Operation_Type)cfg.type);
        if (cfg == null) return;
        MainLineData data;
        curOperationType = (K_Operation_Type)cfg.type;
        curData.offset = cfg.guide_btn_dev;
        switch ((K_Operation_Type)cfg.type)
        {
            case K_Operation_Type.Normal:
                data = new MainLineData(cfg.main_interface, cfg.btn);
                if (cfg.btn == null)
                {
                    data.btnName = curData.condition_id.ToString();
                }
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.FINDTARGETTRANSFORM, data);
                break;
            case K_Operation_Type.SelectSceneFuniture:
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTSCENEFUR, curData.condition_id);
                break;
            case K_Operation_Type.BuyOrSetFurniture:
                data = new MainLineData(cfg.main_interface, curData.condition_id.ToString());
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTUIFURN, data);
                break;
            case K_Operation_Type.RecruitHero:
                data = new MainLineData(cfg.main_interface, "recruitHero");
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.RECRUITHERO, data);
                break;
            case K_Operation_Type.AddHeroSlot:
                data = new MainLineData(cfg.main_interface, "addSlot");
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.ADDHEROSLOT, data);
                break;
            case K_Operation_Type.SelectHeroInfo:
                data = new MainLineData(cfg.main_interface, "heroData");
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.CLICKHEROINFO, data);
                break;
            case K_Operation_Type.SelectShopper:
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.CLICKSHOPPERPOP, curData.condition_id);
                break;
            case K_Operation_Type.OpenTargetTbox:
                data = new MainLineData(cfg.main_interface, cfg.btn);
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.OPENTARGETTBOX, curData.condition_id, data);
                break;
            case K_Operation_Type.BuildInvest:
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.BUILDINVEST, curData.condition_id);
                break;
            case K_Operation_Type.ClickScience:
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SCIENCEBUILDINVEST);
                break;
            case K_Operation_Type.SelectCanRecruitHero:
                data = new MainLineData(cfg.main_interface, cfg.btn);
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTCANRECRUITHERO, data);
                break;
            case K_Operation_Type.SelectTargetEquip:
                data = new MainLineData(cfg.main_interface, curData.condition_id.ToString());
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTTARGETEQUIP, data);
                break;
            case K_Operation_Type.SelectTargetWorker:
                data = new MainLineData(cfg.main_interface, curData.condition_id.ToString());
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTTARGETWORKER, data);
                break;
            case K_Operation_Type.SelectTrasnferTargetHero:
                data = new MainLineData(cfg.main_interface, curData.condition_id.ToString());
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTTARGETTRANSFERHERO, data);
                break;
            case K_Operation_Type.SelectTargetExplore:
                data = new MainLineData(cfg.main_interface, curData.condition_id.ToString());
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTTARGETEXPLORE, data);
                break;
            case K_Operation_Type.SelectTransferTargetToggle:
                data = new MainLineData(cfg.main_interface, cfg.btn);
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTTARGETTRANSFERTOGGLE, curData.condition_id, data);
                break;
            case K_Operation_Type.SelectTargetEquipPage:
                EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SELECTTARGETEQUIPPAGE, curData.condition_id);
                break;
        }

    }

    public void DissatisfactionDialog()
    {
        if (curData == null) return;

        if (!string.IsNullOrEmpty(curData.dialog))
        {
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SHOWMAINLINEDIALOG, curData.dialog);
        }

        curData = null;
        operationIsStart = 0;
    }

    public void nextOperation(bool isAdd = true)
    {
        if (curData == null) return;

        if (isAdd)
            operationIsStart++;

        index++;
        if (operationIds.Count - 1 < index)
        {
            isDoing = false;
            curData = null;
            operationIsStart = 0;
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTIMERRESET, true);
            return;
        }

        runOperation(operationIds[index]);
    }

    public void setFingerTimeReset(bool state)
    {
        if (MainLineDataProxy.inst.MainTaskIsAllOver) return;
        if (GuideManager.inst.isInTriggerGuide) return;
        if (operationIsStart <= 0)
        {
            if (state && curData != null && curData.type != K_Operation_DataType.MainLine)
            {
                // if (curOperationType != K_Operation_Type.BuildInvest && curOperationType != K_Operation_Type.ClickScience)
                isDoing = false;
                curData = null;
            }
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTIMERRESET, state);
        }
        else
        {
            operationIsStart--;
            //if (operationIsStart == 0 && (GUIManager.GetCurrWindowViewID() == ViewPrefabName.MainUI || GUIManager.GetCurrWindowViewID() == ViewPrefabName.CityUI))
            //{
            //    setFingerTimeReset(true);
            //}
        }
    }
}
