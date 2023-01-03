using System;
using System.Collections.Generic;
using UnityEngine;


//商铺内角色寻路(基于A*)
[DisallowMultipleComponent]
public class RoleMoveByAStar : MonoBehaviour, IRoleMove
{
    private Action onMoveStartHandler;
    private Action<Vector3, Vector3> onMoveStepCompleteHandler;
    private Action onMoveCompleteHandler;
    private Action<Vector3> onPosChangeHandler;

    private Stack<PathNode> pathNodeList;
    private Vector3 curPos;//当前节点
    private Vector3 nextPos;//下一个节点
    private Vector3 movepath;
    private float movetime;

    private bool isMoveing;//是否在移动中
    public bool isMoving()
    {
        return isMoveing;
    }

    private float moveSpeed = StaticConstants.moveSpeed;

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        if (moveSpeed <= 0)
        {
            Logger.error("将要设置寻路速度小于0？");
            return;
        }

        this.moveSpeed = moveSpeed;
    }

    public void moveTo(Stack<PathNode> pathlist)
    {
        if (pathNodeList != null)
            pathNodeList.Clear();
        pathNodeList = pathlist;

        if (pathNodeList != null && pathNodeList.Count > 0)
        {
            onMoveStart();
            NextGrid();
        }
    }

    public void stopMove()
    {
        if (pathNodeList != null)
            pathNodeList.Clear();

        isMoveing = false;
    }

    float moveOnCellTime = 0;
    private void NextGrid()
    {
        if (this.enabled == false || this == null)
        {
            return;
        }

        if (pathNodeList == null) //容错
        {
            isMoveing = false;
            onMoveEndComplete();
            return;
        }

        if (pathNodeList.Count > 0)
        {
            isMoveing = true;
            movetime = 0;
            curPos = transform.position;
            nextPos = MapUtils.CellPosToCenterPos(pathNodeList.Pop().WorldPos);
            moveOnCellTime = Vector3.Distance(curPos, nextPos) / moveSpeed;
            movepath = nextPos - transform.position;
            onMoveStepComplete();
        }
        else
        {
            isMoveing = false;
            onMoveEndComplete();
        }
    }

    private void Update()
    {
        if (isMoveing)
        {
            movetime += Time.deltaTime;
            Vector3 position = transform.position;
            var dis = Vector3.Distance(nextPos, position);
            if (dis < 0.01f || movetime >= moveOnCellTime)
            {
                onPosChg(nextPos);
                NextGrid();
            }
            else
            {
                onPosChg(curPos + movepath * (movetime / moveOnCellTime));
            }
        }
    }

    public void setOnStartHandler(Action startHandler)
    {
        onMoveStartHandler = startHandler;
    }

    public void setOnPosChangedHandler(Action<Vector3> posChgHandler)
    {
        onPosChangeHandler = posChgHandler;
    }

    public void setOnStepCompleteHandler(Action<Vector3, Vector3> stepHandler)
    {
        onMoveStepCompleteHandler = stepHandler;
    }

    public void setOnMoveEndCompleteHandler(Action endHandler)
    {
        onMoveCompleteHandler = endHandler;
    }


    void onMoveStart()
    {
        onMoveStartHandler?.Invoke();
    }

    void onPosChg(Vector3 pos)
    {
        transform.position = pos;
        onPosChangeHandler?.Invoke(pos);
    }

    void onMoveStepComplete()
    {
        onMoveStepCompleteHandler?.Invoke(curPos, nextPos);
    }

    void onMoveEndComplete()
    {
        onMoveCompleteHandler?.Invoke();
    }

}
