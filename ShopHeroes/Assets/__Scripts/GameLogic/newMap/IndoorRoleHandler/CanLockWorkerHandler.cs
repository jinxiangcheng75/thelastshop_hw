using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可解锁工匠小人
public partial class IndoorRoleSystem
{

    //只吃一个
    IndoorCanLockWorker indoorCanLockWorker;

    void AddListeners_CanLockWorker()
    {
        var e = EventController.inst;

        e.AddListener(GameEventType.IndoorRole_CanLockWorkerCompEvent.CHECK_CANLOCKWORKER, refreshIndoorCanLockWorker);
        e.AddListener(GameEventType.WorkerCompEvent.Worker_DataChg, refreshIndoorCanLockWorker);

    }

    void RemoveListeners_CanLockWorker()
    {
        var e = EventController.inst;

        e.RemoveListener(GameEventType.IndoorRole_CanLockWorkerCompEvent.CHECK_CANLOCKWORKER, refreshIndoorCanLockWorker);
        e.RemoveListener(GameEventType.WorkerCompEvent.Worker_DataChg, refreshIndoorCanLockWorker);

    }

    int getCanLockWorkerId()
    {
        var list = RoleDataProxy.inst.WorkerList.FindAll(t => t.state == EWorkerState.CanUnlock && t.config.get_type != 6);

        var lockCanShowList = RoleDataProxy.inst.WorkerList.FindAll(t => t.state == EWorkerState.Locked && t.canShow && t.config.get_type != 6);

        if (list.Count + lockCanShowList.Count > 0)
        {
            if (list.Count > 0)
            {
                list.Sort((a, b) => a.config.cost_money.CompareTo(b.config.cost_money));

                return list[0].id;
            }
            else
            {
                lockCanShowList.Sort((a, b) => a.config.cost_money.CompareTo(b.config.cost_money));

                return lockCanShowList[0].id;
            }

        }

        return -1;
    }

    private void refreshIndoorCanLockWorker()
    {
        if (WorldParConfigManager.inst.GetConfig(176).parameters == 0)
        {
            return;
        }

        if (GuideDataProxy.inst == null || GuideDataProxy.inst.CurInfo == null || !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            return;
        }


        int workerId = getCanLockWorkerId();

        if (workerId == -1) //当前没有可解锁的工匠
        {
            if (indoorCanLockWorker != null)
            {
                indoorCanLockWorker.SetVisible(false);
            }
        }
        else
        {
            if (indoorCanLockWorker == null)
            {
                indoorCanLockWorker = IndoorMap.inst.CreateIndoorCanLockWorker(workerId);
            }
            else
            {
                indoorCanLockWorker.SetData(workerId);
                if (!indoorCanLockWorker.isVisible) indoorCanLockWorker.SetVisible(true);
            }
        }

    }

}