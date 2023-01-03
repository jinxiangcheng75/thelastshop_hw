using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveAwardUIComp : MonoBehaviour
{
    public GetQualityItem getQualityItemUI;     //得到品质物品 界面
    public GetItemUI getItemUI; //获得物品(图纸,资源等  要显示需要事件触发)
    public NewBlueprintUI newBlueprint; //新图纸已解锁
    public DrawingUp drawingUp;     //图纸升级
    public ActivateDrawingUI activateDrawingUI; //已激活
    public DrawingStarUp drawingStarUp; //图纸升星
}
