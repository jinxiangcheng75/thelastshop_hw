
using UnityEngine;
/// <summary>
/// 任务系统  #陆泓屹
/// </summary>
public class DailyTaskSystem : BaseSystem
{
    private TaskPanelView taskPanelView;
    private TaskMsgBoxView taskMsgBox;
    private DailyTaskBoxAwardDesPanel boxAwardDesPanel;

    private int nextTaskCoolTimer;
    private int refreshCoolTimer;

    private int curRefreshTaskId;

    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_TASKPANEL, ShowTaskPanel);
        EventController.inst.AddListener(GameEventType.HIDEUI_TASKPANEL, HideTaskPanel);
        EventController.inst.AddListener<RectTransform, int>(GameEventType.TaskEvent.TASK_SHOW_LIVENESSBOXDESPANEL, showLivenessBoxDesPanel);
        EventController.inst.AddListener(GameEventType.TaskEvent.TASK_GET_DATALIST, GetAllDailyTaskDatas);
        EventController.inst.AddListener<int>(GameEventType.TaskEvent.TASK_REPLACE_DATALIST, ReplaceOneDailyTask);
        EventController.inst.AddListener<int>(GameEventType.TaskEvent.TASK_REWARD_DATALIST, GetDailyTaskListAfterReward);
        EventController.inst.AddListener<int>(GameEventType.SHOWUI_MSGBOXCOM_TASK, ShowMsgBox);
        EventController.inst.AddListener(GameEventType.TaskEvent.TASK_RESHOWTASKPANEL, ReShowTaskPanel);
        EventController.inst.AddListener<int>(GameEventType.TaskEvent.TASK_RESHOWTASKPANELANIM, AnimShowTaskPanel);
        EventController.inst.AddListener(GameEventType.TaskEvent.TASK_REFRESHMSGBOX, RefreshMsgBox);
        EventController.inst.AddListener(GameEventType.TaskEvent.TASK_COLLTIMEDOWN, CoolTimeDown);
        EventController.inst.AddListener(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS, RefreshUnionTaskMess);

        EventController.inst.AddListener<int>(GameEventType.TaskEvent.TASK_GETLIVENESSBOXAWARD, GetDailyActiveBoxReward);

    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_TASKPANEL, ShowTaskPanel);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_TASKPANEL, HideTaskPanel);
        EventController.inst.RemoveListener<RectTransform, int>(GameEventType.TaskEvent.TASK_SHOW_LIVENESSBOXDESPANEL, showLivenessBoxDesPanel);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.TASK_GET_DATALIST, GetAllDailyTaskDatas);
        EventController.inst.RemoveListener<int>(GameEventType.TaskEvent.TASK_REPLACE_DATALIST, ReplaceOneDailyTask);
        EventController.inst.RemoveListener<int>(GameEventType.TaskEvent.TASK_REWARD_DATALIST, GetDailyTaskListAfterReward);
        EventController.inst.RemoveListener<int>(GameEventType.SHOWUI_MSGBOXCOM_TASK, ShowMsgBox);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.TASK_RESHOWTASKPANEL, ReShowTaskPanel);
        EventController.inst.RemoveListener<int>(GameEventType.TaskEvent.TASK_RESHOWTASKPANELANIM, AnimShowTaskPanel);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.TASK_REFRESHMSGBOX, RefreshMsgBox);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.TASK_COLLTIMEDOWN, CoolTimeDown);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.TASK_REFRESHUNIONTASKMESS, RefreshUnionTaskMess);


        EventController.inst.RemoveListener<int>(GameEventType.TaskEvent.TASK_GETLIVENESSBOXAWARD, GetDailyActiveBoxReward);


    }

    private void ShowMsgBox(int task_id)
    {
        taskMsgBox = GUIManager.OpenView<TaskMsgBoxView>((view) =>
        {
            view.setComMsgText(task_id);
        });

        curRefreshTaskId = task_id;
    }

    private void RefreshMsgBox()
    {
        if (taskMsgBox != null && taskMsgBox.isShowing)
        {
            taskMsgBox.setComMsgText(curRefreshTaskId);
        }
    }

    //请求此账号的任务数据列表
    private void GetAllDailyTaskDatas()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            //发送获取任务的请求
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_DailyTask_Data()
            });
        }
    }

    //刷新某一任务
    private void ReplaceOneDailyTask(int task_ID)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            //发送获取任务的请求
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_DailyTask_Refresh()
                {
                    taskId = task_ID
                }
            });
        }
    }

    //完成某一任务
    private void GetDailyTaskListAfterReward(int task_ID)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            //发送获取任务的请求
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_DailyTask_Reward()
                {
                    taskId = task_ID
                }
            });
        }
    }

    //领取某一活跃宝箱
    private void GetDailyActiveBoxReward(int activeTaskId)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Active_Reward()
                {
                    activeRewardId = activeTaskId
                }
            });
        }
    }

    private void ShowTaskPanel()
    {
        taskPanelView = GUIManager.OpenView<TaskPanelView>((view) =>
        {
            view.ShowAllTaskItems();
        });
    }

    private void ReShowTaskPanel()
    {
        if (taskPanelView != null && taskPanelView.isShowing)
        {
            taskPanelView.ShowAllTaskItems();
        }
    }

    private void AnimShowTaskPanel(int taskId)
    {
        if (taskPanelView != null && taskPanelView.isShowing)
        {
            taskPanelView.ShowAllTaskItemsByAnim(taskId);
        }
    }

    private void RefreshUnionTaskMess()
    {
        var taskPanel = GUIManager.GetWindow<TaskPanelView>();
        if (taskPanel != null && taskPanel.isShowing)
        {
            taskPanel.RefreshUnionTaskMess();
        }

        var taskReSetPanel = GUIManager.GetWindow<UnionTaskResetUI>();
        if (taskReSetPanel != null && taskReSetPanel.isShowing)
        {
            taskReSetPanel.SetData();
        }

    }


    private void HideTaskPanel()
    {
        GUIManager.HideView<TaskPanelView>();
    }


    private void showLivenessBoxDesPanel(RectTransform boxTf, int activeTaskId)
    {
        GUIManager.OpenView<DailyTaskBoxAwardDesPanel>(view =>
        {
            view.SetData(boxTf, activeTaskId);
        });

    }

    //倒计时
    private void CoolTimeDown()
    {
        if (nextTaskCoolTimer > 0)
        {
            GameTimer.inst.RemoveTimer(nextTaskCoolTimer);
            nextTaskCoolTimer = 0;
        }

        if (refreshCoolTimer > 0)
        {
            GameTimer.inst.RemoveTimer(refreshCoolTimer);
            refreshCoolTimer = 0;
            RefreshMsgBox();
        }

        if (UserDataProxy.inst.task_nextTime > 0)
        {
            nextTaskCoolTimer = GameTimer.inst.AddTimer(UserDataProxy.inst.task_nextTime, 1, () =>
            {
                GetAllDailyTaskDatas();
                return;
            },GameTimerType.byServerTime);
        }

        if (UserDataProxy.inst.task_refreshTime > 0 && UserDataProxy.inst.task_refreshNumber == 0)
        {
            refreshCoolTimer = GameTimer.inst.AddTimer(UserDataProxy.inst.task_refreshTime, 1, () =>
            {
                GetAllDailyTaskDatas();
                return;
            }, GameTimerType.byServerTime);
        }

    }

}
