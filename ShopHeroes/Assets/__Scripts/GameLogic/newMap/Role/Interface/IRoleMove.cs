using System;
using UnityEngine;

//角色移动
public interface IRoleMove
{
    void setOnStartHandler(Action startHandler);//开始移动
    void setOnPosChangedHandler(Action<Vector3> posChgHandler);//位置发生变化
    void setOnStepCompleteHandler(Action<Vector3, Vector3> stepHandler);//移动到下一个节点时
    void setOnMoveEndCompleteHandler(Action endHandler);//结束移动

    bool isMoving();//是否处于移动中
}
