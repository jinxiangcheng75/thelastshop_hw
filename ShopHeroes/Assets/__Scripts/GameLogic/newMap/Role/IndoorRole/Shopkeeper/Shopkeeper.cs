using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineShopkeeperState
{
    none = -1,
    onCounterRound,//在柜台旁
    ramble,//闲逛
    toCounter,//去柜台（可能去不了）
    inGuide,//在引导其间(无任何操作)
}

public enum ShopkeeperRambleType
{
    none,

    shopkeeper_ramble_1 = StaticConstants.shopkeeperWeightBase + 1,
    shopkeeper_ramble_2,
    shopkeeper_ramble_3,
    shopkeeper_ramble_4,
    shopkeeper_ramble_5,

}


//测试店主
[DisallowMultipleComponent]
public class Shopkeeper : IndoorRole
{
    StateMachine _machine;

    int _roundCounterShopperNum;
    public bool isShowingAcheivement;//是否有成就完成气泡

    public Action onMoveEndCompleteHandler;

    [HideInInspector]
    public bool isCanMoveToCounter;//是否可以移动到柜台
    [HideInInspector]
    public ShopkeeperRambleType rambleType = ShopkeeperRambleType.none;

    public int roundCounterShopperNum // 在柜台旁边的顾客数量
    {
        get
        {
            return _roundCounterShopperNum;
        }
        set
        {
            _roundCounterShopperNum = value;
            if (_roundCounterShopperNum < 0) _roundCounterShopperNum = 0;

            ///做具体处理
            if (_machine != null)
            {
                MachineShopkeeperState state = GetCurState();
                if (state == MachineShopkeeperState.inGuide) return;
                if (state != MachineShopkeeperState.toCounter)
                {
                    if (_roundCounterShopperNum > 0)
                    {
                        _machine.SetState((int)MachineShopkeeperState.onCounterRound, null);
                    }
                    else
                    {
                        if (state != MachineShopkeeperState.ramble) _machine.SetState((int)MachineShopkeeperState.ramble, null);
                    }
                }
            }
        }
    }


    protected override void Init()
    {
        base.Init();
        gameObject.name = "Shopkeeper";

        CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)UserDataProxy.inst.playerData.gender), SpineUtils.RoleDressToUintList(UserDataProxy.inst.playerData.userDress), (EGender)UserDataProxy.inst.playerData.gender, callback: (dressUpSystem) =>
        {
            _character = dressUpSystem;
            var go = dressUpSystem.gameObject;
            go.transform.SetParent(_attacher.actorParent);
            go.transform.localPosition = Vector3.zero;
            UpdateSortingOrder();
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
            _character.Play(idleAnimationName, true);
            _character.SetDirection(RoleDirectionType.Left);
        });

        _machine = new StateMachine();

        _machine.Init(new List<IState> { new ShopkeeperOnCounterRoundState(this),
                                         new ShopkeeperRambleState(this),
                                         new ShopkeeperToCounterState(this),
                                         new ShopkeeperOnGuideState(this)});

        isCanMoveToCounter = true;
    }

    private void Update()
    {
        if (_machine != null) _machine.OnUpdate();
    }

    public MachineShopkeeperState GetCurState()
    {
        if (_machine != null) return (MachineShopkeeperState)_machine.GetCurState();

        return MachineShopkeeperState.none;
    }


    public void SetState(int state, StateInfo Info = null)
    {
        if (_machine != null) _machine.SetState(state, Info);
    }

    //更新装扮
    public void ChangeClothing()
    {
        CharacterManager.inst.ReSetCharacter(_character, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)UserDataProxy.inst.playerData.gender), SpineUtils.RoleDressToUintList(UserDataProxy.inst.playerData.userDress), (EGender)UserDataProxy.inst.playerData.gender);
    }

    //移动到对应家具
    public bool MoveToFurniture(int furnitureUid)
    {
        if (furnitureUid == -1) return false;

        var endPos = IndoorMap.inst.GetFurnitureFrontPos(furnitureUid);
        Stack<PathNode> movepath = IndoorMap.inst.FindPath(currCellPos, endPos);

        if (movepath.Count > 0 && endPos != Vector3Int.zero)
        {
            move(movepath);
            return true;
        }

        Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "____店主无法前往该家具 当前位置：" + currCellPos.ToString() + "   家具uid ： " + furnitureUid + "  家具位置：" + endPos.ToString()));

        return false;
    }

    public bool MoveToPet(int petUid)
    {
        if (petUid == -1) return false;

        if (IndoorRoleSystem.inst == null) return false;

        Pet pet = IndoorRoleSystem.inst.GetPetByUid(petUid);

        if (pet == null) return false;

        var endPos = pet.GetFreePos();
        Stack<PathNode> movepath = IndoorMap.inst.FindPath(currCellPos, endPos);

        if (movepath.Count > 0 && endPos != Vector3Int.zero)
        {
            move(movepath);
            return true;
        }

        Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "____店主无法前往该宠物 当前位置：" + currCellPos.ToString() + "   宠物Id ： " + petUid + "  宠物位置：" + endPos.ToString()));

        return false;
    }

    //随机移动一个位置
    public bool MoveToRandomPos()
    {
        var pos = IndoorMap.inst.GetFreeGridPos();
        Vector3Int endPos = MapUtils.IndoorCellposToMapCellPos(pos);

        if (endPos != Vector3Int.zero)
        {
            var movePath = IndoorMap.inst.FindPath(currCellPos, endPos);
            if (movePath.Count > 1)
            {
                move(movePath);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //移动到柜台
    public bool moveToCounter()
    {
        bool canMove = false;

        var endPos = IndoorMap.inst.GetCounterOperationPos();

        Stack<PathNode> movePath = new Stack<PathNode>();

        if (endPos != new Vector3Int(99999, 99999, 99999))
        {
            movePath = IndoorMap.inst.FindPath(currCellPos, endPos);

            if (movePath.Count > 0)
            {
                move(movePath);
                canMove = true;
            }
            else
            {
                canMove = false;
            }
        }
        else
        {
            canMove = false;
        }

        return canMove;
    }


    //面向柜台
    public void FaceToCounter()
    {
        if (UserDataProxy.inst == null || _character == null) return;
        IndoorData.ShopDesignItem counterdata = UserDataProxy.inst.GetCounter();
        if (counterdata != null)
        {
            if (counterdata.dir == 0)
            {
                _character.SetDirection(RoleDirectionType.Right);
            }
            else
            {
                _character.SetDirection(RoleDirectionType.Left);
            }
        }
    }

    //面向对应家具
    public void FaceToFurniture(int fUid)
    {

        if (_character != null)
        {
            if (IndoorMap.inst.GetFurnituresByUid(fUid, out Furniture f))
            {
                _character.SetDirection(getDir(currCellPos, f.cellpos));
            };
        }
    }

    public void FaceToPet(int petUid)
    {

        if (_character != null)
        {
            if (IndoorRoleSystem.inst != null)
            {
                var pet = IndoorRoleSystem.inst.GetPetByUid(petUid);

                if (pet != null)
                {
                    _character.SetDirection(getDir(currCellPos, pet.currCellPos));
                }

            }
        }

    }

    //是否在柜台附近
    public bool CheckIsRoundCounter()
    {
        var endPos = IndoorMap.inst.GetCounterOperationPos();

        if (endPos != new Vector3Int(99999, 99999, 99999))
        {
            if (Vector3.Distance(currCellPos, endPos) < 0.1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }

    //店铺格局发生了变化
    public void OnDesignChanged()
    {
        MachineShopkeeperState curState = GetCurState();

        if (curState == MachineShopkeeperState.ramble || curState == MachineShopkeeperState.inGuide)
        {
            //闲逛或者在引导中 不做处理
            return;
        }
        else
        {
            if (!CheckIsRoundCounter())
            {
                SetState((int)MachineShopkeeperState.toCounter);
            }
            else
            {
                FaceToCounter();
                if (curState != MachineShopkeeperState.onCounterRound)
                    SetState((int)MachineShopkeeperState.onCounterRound);
            }
        }


    }

    //面向顾客
    public void FaceToShopper(Vector3 shopperPosition)
    {
        if (isMoving) return;

        RoleDirectionType direction = getDir(IndoorMap.inst.gameMapGrid.WorldToCell(transform.position), IndoorMap.inst.gameMapGrid.WorldToCell(shopperPosition) + Vector3Int.up); // + 1 偏移
        if (_character != null) _character.SetDirection(direction);
    }


    //展示成就气泡
    public void ShowAcheivementBubble()
    {
        if (this == null) return;
        if (!AcheivementDataProxy.inst.NeedRedPoint) return;

        isShowingAcheivement = true;

        AtlasAssetHandler.inst.GetAtlasSprite("main_atlas", "zhuejiemian_qipao2", (gsprite) =>
        {
            SetSpBgIcon(gsprite.sprite);
            gsprite.release();
        });

        AtlasAssetHandler.inst.GetAtlasSprite("main_atlas", "zhuejiemian_iconchenjiu", (gsprite) =>
        {
            Color qcol = Color.white;
            ShowSpPop(gsprite.sprite, 1, false, false, in qcol, false, 1f);
        });
        SetBubbleClickHandler(() => { EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.SHOWUI_ACHEIVEMENTUI); });
    }

    //隐藏成就气泡
    public void HideAcheivementBubble()
    {
        if (isShowingAcheivement)
        {
            isShowingAcheivement = false;
            HidePopup();
        }
    }


    protected override void onMoveEndComplete()
    {
        base.onMoveEndComplete();
        onMoveEndCompleteHandler?.Invoke();
    }


    public override void Talk(string msg, Action talkComplete = null)
    {
        if (isShowingAcheivement)
        {
            talkComplete = talkComplete == null ? ShowAcheivementBubble : talkComplete + ShowAcheivementBubble;
        }

        base.Talk(msg, talkComplete);
    }

    bool checkCurPosIsObstacle()
    {
        int gridFlag = IndoorMap.inst.GetIndoorGridFlags(currCellPos.x - StaticConstants.IndoorOffsetX, currCellPos.y - StaticConstants.IndoorOffsetY);

        if (gridFlag == 0)
        {
            //设置一下Flag
            IndoorMap.inst.SetGridFlags(true, currCellPos.x - StaticConstants.IndoorOffsetX, currCellPos.y - StaticConstants.IndoorOffsetY, 1415926, 1415926);
            return false;
        }

        return gridFlag != 1415926 && gridFlag != (int)kTileGroupType.Counter;

    }

    public void RefreshCurCellPos()
    {

        if (checkCurPosIsObstacle())
        {
            Vector3Int newPos = IndoorMap.inst._FindFreeCell(MapUtils.MapCellPosToIndoorCellpos(currCellPos), 1, 1, true);
            SetCellPos(MapUtils.IndoorCellposToMapCellPos(newPos));
            UpdateSortingOrder();
            IndoorMap.inst.SetGridFlags(true, newPos.x, newPos.y, 1415926, 1415926);
        }

        if (!CheckIsRoundCounter())
        {
            OnDesignChanged();
        }
        else
        {
            FaceToCounter();
        }
    }

}
