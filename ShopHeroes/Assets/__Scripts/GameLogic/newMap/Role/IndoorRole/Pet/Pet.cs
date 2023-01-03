using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachinePetState
{
    none = -1,
    ramble,//闲逛
    stayBySee,//停留被观赏
    stayDoorway, //去门口傻愣着
}


//宠物
public class Pet : IndoorRole
{

    [HideInInspector]
    public int petUid;
    [HideInInspector]
    public PetData petData;
    [HideInInspector]
    public Action moveEndCompleteHandler;

    int stayBySeeCount; //等待被观看的人数

    public int StayBySeeCount
    {
        get
        {
            return stayBySeeCount;
        }
        set
        {
            stayBySeeCount = Math.Max(0, value);
            //Logger.error("宠物等待被观看的人数:  " + stayBySeeCount);

            if (GetCurState() == MachinePetState.stayDoorway)
            {
                return;
            }

            if (stayBySeeCount == 1)
            {
                SetState(MachinePetState.stayBySee);
            }

            if (stayBySeeCount == 0)
            {
                SetState(MachinePetState.ramble);
            }

        }
    }

    StateMachine _machine;

    protected override void Init()
    {
        base.Init();
        gameObject.name = "Pet";

        _machine = new StateMachine();

        _machine.Init(new List<IState> { new PetRambleState(this),
                                         new PetStayBySeeState(this),
                                         new PetStayDoorWay(this)});

    }


    public MachinePetState GetCurState()
    {
        if (_machine != null) return (MachinePetState)_machine.GetCurState();

        return MachinePetState.none;
    }


    public void SetState(MachinePetState state, StateInfo Info = null)
    {
        if (_machine != null) _machine.SetState((int)state, Info);
    }

    private void Update()
    {

        if (_machine != null)
        {
            _machine.OnUpdate();
        }

    }

    public void SetData(PetData data)
    {
        petUid = data.petUid;
        petData = data;

        if (AStar != null)
        {
            AStar.SetMoveSpeed(petData.petCfg.moveSpeed * .1f);
        }

        if (_character == null)
        {
            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(petData.petCfg.model, callback: (dressUpSystem) =>
            {
                _character = dressUpSystem;
                onCharacterCreated();
            });
        }
        else
        {
            //仅刷新外观
            CharacterManager.inst.ReSetCharacterByModel(_character, petData.petCfg.model, repackedCallback: (system) =>
            {
                if (IndoorMapEditSys.inst != null && (IndoorMapEditSys.inst.shopDesignMode != DesignMode.normal))
                {
                    _character.skeletonAlpha = 0;
                }
            });
        }
    }

    void onCharacterCreated()
    {
        var go = _character.gameObject;
        go.transform.SetParent(_attacher.actorParent);
        go.transform.localPosition = Vector3.zero;
        go.name = "您的宠物" + petData.petInfo.petName;

        SetCellPos(MapUtils.IndoorCellposToMapCellPos(IndoorMap.inst.GetFreeGridPos()));
        UpdateSortingOrder();

        SetState(MachinePetState.ramble);

        if (IndoorMapEditSys.inst != null && (IndoorMapEditSys.inst.shopDesignMode != DesignMode.normal))
        {
            _character.skeletonAlpha = 0;
        }
    }

    //随机移动一个位置
    public void MoveToRandomPos()
    {
        var pos = IndoorMap.inst.GetFreeGridPos();
        Vector3Int endPos = MapUtils.IndoorCellposToMapCellPos(pos);

        if (endPos != Vector3Int.zero)
        {
            var movePath = IndoorMap.inst.FindPath(currCellPos, endPos);
            if (movePath.Count > 1)
                move(movePath);
            else
            {
                moveEndCompleteHandler?.Invoke();
            }
        }
        else
        {
            moveEndCompleteHandler?.Invoke();
        }
    }

    protected override void onMoveStart()
    {
        _character.Play("walk", true /*,AStar.MoveSpeed / 0.8f*/);
    }

    public override void stopMove()
    {
        _aStar.stopMove();
        _character.Play("idle", true);
    }

    protected override void onMoveEndComplete()
    {
        setOrder(MapUtils.GetTileMapOrder(transform.position.y - 0.2f, transform.position.x, 1, 1));
        moveEndCompleteHandler?.Invoke();
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }


    int[] directionArr = { -1, 0, 1, 0, -1 }; // -1,0下 0,1左 1,0上 0,-1 右

    public Vector3Int GetFreePos()
    {

        List<Vector3Int> poslist = new List<Vector3Int>();


        for (int i = 0; i < directionArr.Length - 1; i++)
        {
            int left = directionArr[i];
            int right = directionArr[i + 1];

            var pos = currCellPos + new Vector3Int(left, right, 0);
            pos.x -= StaticConstants.IndoorOffsetX;
            pos.y -= StaticConstants.IndoorOffsetY;

            if (!IndoorMap.inst.IsObstacleGrid(pos.x, pos.y))
            {
                poslist.Add(MapUtils.IndoorCellposToMapCellPos(pos));
            }

        }

        if (poslist.Count > 0)
        {
            return poslist[UnityEngine.Random.Range(0, poslist.Count)];
        }


        return Vector3Int.zero;
    }


}
