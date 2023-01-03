using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景内的人物父类
[RequireComponent(typeof(RoleMoveByAStar), typeof(CharacterAttacher))]
public abstract class IndoorRole : RoleBase
{

    protected DressUpSystem _character; //可直接播放动画 改变方向
    protected RoleMoveByAStar _aStar;
    protected CharacterAttacher _attacher;

    public DressUpSystem Character { get { return _character; } }
    public RoleMoveByAStar AStar { get { return _aStar; } }
    public CharacterAttacher Attacher { get { return _attacher; } }

    protected override void Init()
    {
        //移动
        _aStar = GetComponent<RoleMoveByAStar>();
        _aStar.setOnStartHandler(onMoveStart);
        _aStar.setOnPosChangedHandler(onPosChgHandler);
        _aStar.setOnStepCompleteHandler(onMoveStepComplete);
        _aStar.setOnMoveEndCompleteHandler(onMoveEndComplete);

        //谈话
        _attacher = GetComponent<CharacterAttacher>();
    }

    #region 位置
    [HideInInspector]
    public Vector3Int currCellPos;

    public void SetCellPos(Vector3Int cellpos)
    {
        Vector3 pos = MapUtils.CellPosToCenterPos(cellpos);
        setworldPos(pos);
        transform.position = pos;
    }

    protected virtual void setworldPos(Vector3 pos)
    {
        currCellPos = MapUtils.WorldPosToCellPos(pos);
    }

    #endregion

    #region 层级 方向
    int lastOrder = -1;

    protected void setOrder(int order,string sortingLayerName = "map_Actor")
    {
        if (lastOrder == order) return;

        if (_character != null)
        {
            lastOrder = order;
            _character.SetSortingAndOrderLayer(sortingLayerName, order);
        }

        var rendererslist = transform.GetComponentsInChildren<Renderer>(true);
        foreach (var rander in rendererslist)
        {
            var setorderspript = rander.gameObject.GetComponent<SetRendererOrder>();
            if (setorderspript != null)
            {
                setorderspript.setOrder(order, false);
                continue;
            }
            rander.sortingOrder = order;
        }
    }

    public void UpdateSortingOrder()
    {
        setOrder(MapUtils.GetTileMapOrder(transform.position.y - 0.5f, transform.position.x, 1, 1));
    }
    #endregion

    #region 移动
    public virtual void move(Stack<PathNode> pathList)
    {
        if (pathList == null) return;
        _aStar.moveTo(pathList);
    }

    public bool isMoving { get { return _aStar.isMoving(); } }

    public virtual void stopMove()
    {
        _aStar.stopMove();
        if (_character != null) 
        {
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
            _character.Play(idleAnimationName, true);
        }
    }

    protected virtual void onMoveStart()
    {
        if (_character != null)
        {
            string moveAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_walking);
            _character.Play(moveAnimationName, true, AStar.MoveSpeed / 0.8f * 1.1f);
        }
    }

    protected virtual void onPosChgHandler(Vector3 pos)
    {
        setworldPos(pos);
    }

    protected virtual void onMoveStepComplete(Vector3 curPos, Vector3 nextPos)
    {
        setOrder(MapUtils.GetTileMapOrder(transform.position.y - 0.2f, transform.position.x, 1, 1));
        if (_character != null) _character.SetDirection(getDir(MapUtils.WorldPosToCellPos(curPos), MapUtils.WorldPosToCellPos(nextPos)));
    }

    protected virtual void onMoveEndComplete()
    {
        setOrder(MapUtils.GetTileMapOrder(transform.position.y - 0.2f, transform.position.x, 1, 1));
        if (_character != null) 
        {
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
            _character.Play(idleAnimationName, true);
        }
    }

    protected RoleDirectionType getDir(Vector3 from, Vector3 to)
    {
        Vector3 span = to - from;
        if (span.x > 0 || span.y < 0)
        {
            return RoleDirectionType.Right;
        }
        else
        {
            return RoleDirectionType.Left;
        }
    }
    #endregion

    #region 气泡（点击，谈话）

    public virtual void SetBubbleClickHandler(Action onClick)
    {
        _attacher.onClickHandler = onClick;
    }

    public void SetSpBgIcon(Sprite sprite)
    {
        _attacher.SetSpBgIcon(sprite);
    }

    public void ShowSpPop(Sprite sp, int count, bool showCountTx, bool outline, in Color outlineColor, bool needTile = true, float spScale = 1.6f)
    {
        _attacher.ShowSpIcon(sp, count, showCountTx, outline, outlineColor, needTile, spScale);
    }

    public virtual void Talk(string msg, Action talkComplete = null)
    {
        _attacher.Talk(msg, talkComplete);
    }

    public void Talk(string[] msgs, Action stepComplete = null, Action endComplete = null)
    {
        _attacher.Talk(msgs, stepComplete, endComplete);
    }

    public void HidePopup(Action callback = null)
    {
        _attacher.HidePopup(callback);
    }

    public void SetTalkSpacing(float talkSpacing)
    {
        if (talkSpacing < 0) return;

        _attacher.talkSpacing = talkSpacing;
    }

    public bool isTalking { get { return _attacher.isTalking; } }
    #endregion

    #region 显隐
    [HideInInspector]
    public bool isVisible = true;
    public void SetVisible(bool visible)
    {
        isVisible = visible;

        _attacher.SetVisible(visible);

        int endVal = visible ? 1 : 0;
        if (_character != null && _character.skeletonAlpha != endVal) _character.Fade(endVal, 0.35f);

    }
    #endregion
}
