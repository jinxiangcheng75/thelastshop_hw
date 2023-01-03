using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopperToInDoorState : StateBase
{
    Shopper _shopper;

    public ShopperToInDoorState(Shopper shopper)
    {
        _shopper = shopper;
    }

    public override int getState()
    {
        return (int)MachineShopperState.toInDoor;
    }


    bool canMoveToInDoor, moveEndHandlerTrigger, notTryTalk;
    Vector3Int talkPos;

    public override void onEnter(StateInfo info)
    {
        canMoveToInDoor = false;
        notTryTalk = false;
        moveEndHandlerTrigger = false;
        //talkPos = randomTalkPos();

        //Logger.error("新顾客进入了 进门前 的状态");

        if (!_shopper.isInDoor)
        {
            canMoveToInDoor = enterIndoor();

            //进不了店 拒绝他 离开
            if (!canMoveToInDoor)
            {
                _shopper.isNew = false;
                _shopper.SetState(MachineShopperState.leave, new ShopperLeaveReason(1));
                EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, _shopper.shopperData.data.shopperUid, true);
            }
            else if (!_shopper.CheckHasTypeHandler())
            {
                _shopper.SetState(MachineShopperState.leave, new ShopperLeaveReason(99));
                EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, _shopper.shopperData.data.shopperUid, true);
            }
        }
        else
        {
            toIndoorMoveEndHandler();
        }
    }


    public override void onUpdate()
    {
        if (moveEndHandlerTrigger) return;

        //在固定范围讲一句话
        if (canMoveToInDoor)
        {
            if (_shopper.isMoving)
            {
                //if (_shopper.currCellPos == talkPos && !notTryTalk)
                //{
                //    notTryTalk = true;
                //    if (Random.Range(0, 3) == 0)  // 1/3 概率说话
                //    {
                //        talk();
                //    }
                //}
            }
            else
            {
                toIndoorMoveEndHandler();
            }
        }

    }

    public override void onExit()
    {

    }

    //进屋
    private bool enterIndoor()
    {
        var endPos = IndoorMap.inst.GetIndoorPoint();


        if (endPos == Vector3Int.zero)
        {
            return false;
        }
        else
        {
            Stack<PathNode> movepath = IndoorMap.inst.FindPath(_shopper.currCellPos, endPos);
            if ((movepath.Count > 0 && endPos != Vector3Int.zero) || endPos == _shopper.currCellPos)
            {
                _shopper.move(movepath);
                _shopper.moveTargetPos = endPos;
                return true;
            }
            else
            {
                return false;

            }
        }
    }

    //随机一个讲话地点
    private Vector3Int randomTalkPos()
    {
        var list = ShopperDataProxy.inst.shopperToIndoorCanTalkPosList;

        if (list.Count > 0) return list[Random.Range(0, list.Count)];

        return Vector3Int.zero;
    }

    //讲话
    private void talk()
    {
        if (_shopper.isTalking) return;

        if (_shopper.shopperData.data.shopperType == (int)EShopperType.Worker) return; //工匠不会说话

        _shopper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(_shopper.shopperData.data.shopperId, _shopper.shopperData.data.shopperLevel, (int)TalkConditionType.beforeIndoor)));
    }


    //开始逛商店 最终后端决定离开还是排队 （中间过程前端控制）
    private void rambleShop()
    {
        if (_shopper.shopperData.data.shopperState == 99) //直接去柜台
        {
            _shopper.SetState(MachineShopperState.toCounter, null);
            return;
        }

        int checkLevel = -1;
        WorldParConfig worldParConfig = WorldParConfigManager.inst.GetConfig(8304);
        if (worldParConfig != null) checkLevel = (int)worldParConfig.parameters;

        bool needRandEquipIdToCounter = false;

        if (UserDataProxy.inst.playerData.level < checkLevel && _shopper.shopperData.data.shopperComeType == (int)EShopperComeType.Normal) //1.来买的 2.等级判定内的 3.货架上没有的 4.非剧情顾客 do.为他主动筛选一个符合他职业且当前店主可以做的装备id 让他直接排队(发送Request_Shopper_Queue变为排队)
        {

            //if (_shopper.shopperData.data.targetEquipId == 0) //装备id为0的
            //{
            //    needRandEquipIdToCounter = true;
            //}
            //else
            //{
            var shelfEquips = ItemBagProxy.inst.GetOnShelfEquipItems();
            var equip = shelfEquips.Find(t => t.equipid == _shopper.shopperData.data.targetEquipId); //shelfEquips.Find(t => t.itemUid == _shopper.shopperData.data.targetEquipUid); //Uid改为id

            bool shopHaveThisEquip = equip != null; //商店是否有这件装备（id）

            if (!shopHaveThisEquip)//货架上没有的或者仓库空了
            {
                needRandEquipIdToCounter = true;
            }
            //}
        }


        if (needRandEquipIdToCounter)
        {
            List<int> subTypes = HeroProfessionConfigManager.inst.GetAllFieldEquipId(_shopper.shopperData.data.shopperId); //可穿戴的装备小类型
            List<EquipData> canMakeAndCanWearEquips = EquipDataProxy.inst.GetEquipDatasBySubTypes(subTypes, MarketEquipLvManager.inst.GetCurMarketLevel());

            if (canMakeAndCanWearEquips.Count > 0)
            {
                EquipData equip = canMakeAndCanWearEquips.GetRandomElement();
                EquipQualityConfig equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equip.equipDrawingId, 1);
                EventController.inst.TriggerEvent<int, string, int>(GameEventType.ShopperEvent.SHOPPER_BUY_CONFIRM, _shopper.shopperUid, /*equipUid*/string.Empty, equipCfg.id);
            }
            else
            {
                _shopper.SetState(MachineShopperState.ramble, null);
            }
        }
        else
        {
            _shopper.SetState(MachineShopperState.ramble, null);
        }

    }

    private void toIndoorMoveEndHandler()
    {
        if (moveEndHandlerTrigger) return;

        if (_shopper.isBuy())
        {
            rambleShop();
        }
        else
        {
            _shopper.SetState(MachineShopperState.toCounter, null);
        }

        moveEndHandlerTrigger = true;
    }

}
