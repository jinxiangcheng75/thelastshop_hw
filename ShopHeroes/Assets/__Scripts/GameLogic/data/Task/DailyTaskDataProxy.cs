using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 任务系统  #陆泓屹
/// </summary>

//public class DailyTaskDataProxy : TSingletonHotfix<DailyTaskDataProxy>, IDataModelProx
//{
//    private Dictionary<int, TaskData> _taskDic;
//    public List<TaskData> taskList
//    {
//        get { return _taskDic.Values.ToList(); }
//    }

//    public int refreshNumber { get; private set; }

//    public bool needShowRedPoint
//    {
//        get 
//        {
//            foreach (var item in taskList)
//            {
//                if (item.taskState == (int)EDailyTaskState.Reached)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }
//    }

//    public void Init()
//    {
//        _taskDic = new Dictionary<int, TaskData>();
//    }

//    public void Clear()
//    {

//    }

//    public void OnDailyTaskChanged(Response_DailyTask_Change data)
//    {
//        if (_taskDic.ContainsKey(data.task.taskId))
//        {
//            _taskDic[data.task.taskId] = new TaskData(data.task);
//        }
//        else
//        {
//            Logger.error("未找到该id的任务  id = " + data.task.taskId);
//        }

//        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint);

//    }

//    public void RefreshTaskData(List<OneDailyTask> dailyTasks, int refreshNumber)
//    {
//        _taskDic.Clear();
//        foreach (var item in dailyTasks)
//        {
//            _taskDic.Add(item.taskId, new TaskData(item));
//        }
//        this.refreshNumber = refreshNumber;

//        EventController.inst.TriggerEvent(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint);

//    }

//    public TaskData GetTaskDataByTaskId(int taskId)
//    {
//        if (_taskDic.ContainsKey(taskId))
//        {
//            return _taskDic[taskId];
//        }

//        Logger.error("没有id是" + taskId + "的任务");
//        return null;
//    }

//    public string GetTaskName(TaskData data, bool isTopShow = false)
//    {
//        string name = LanguageManager.inst.GetValueByKey(data.name);

//        switch (data.taskType)
//        {
//            case 1:
//            case 2:
//                {
//                    EquipDrawingsConfig equipConfig = EquipConfigManager.inst.GetEquipDrawingsCfg(data.taskTargetId);

//                    name = name.Replace("{0}", LanguageManager.inst.GetValueByKey(equipConfig.name));
//                    if (isTopShow)
//                        name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
//                    else
//                        name = name.Replace("{1}", data.parameter_2.ToString());
//                    break;
//                }
//            case 3:
//            case 4:
//            case 5:
//            case 6:
//                {
//                    if (isTopShow)
//                        name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
//                    else
//                        name = name.Replace("{1}", data.parameter_2.ToString());
//                    break;
//                }
//            case 7:
//                {
//                    EquipClassification equipTypeCfg = EquipConfigManager.inst.GetEquipTypeByID(data.taskTargetId);

//                    name = name.Replace("{0}", LanguageManager.inst.GetValueByKey(equipTypeCfg.name));
//                    if (isTopShow)
//                        name = name.Replace("{1}", "<Color=#F95E00>" + data.parameter_2.ToString()) + "</Color>";
//                    else
//                        name = name.Replace("{1}", data.parameter_2.ToString());
//                    break;
//                }
//        }

//        return name;
//    }
//}
