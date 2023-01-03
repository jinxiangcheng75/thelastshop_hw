using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoobTaskItemComp:MonoBehaviour
{
    //当前Item的数据
    public TaskData data;

    public Image progressCircle;
    public Image characterImg;

    public Text taskName;

    //给Item的组件赋值
    public void ShowItemContent(TaskData data)
    {
        var l = LanguageManager.inst;

        this.data = data;

        switch (data.taskState)
        {
            case (int)EDailyTaskState.Doing:
                {
                    progressCircle.fillAmount = data.parameter_1 / data.parameter_2;
                    //characterImg.sprite=

                    //taskName.text = LanguageManager.inst.GetValueByKey(data.name, data.parameter_1.ToString(), data.parameter_2.ToString());
                    break;
                }

            case (int)EDailyTaskState.Reached:
                {
                    
                    break;
                }
        }
    }
}