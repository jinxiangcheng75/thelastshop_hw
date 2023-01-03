using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//顾客去柜台的状态
public class ShopperToCounterState : StateBase
{
    Shopper _shopper;

    public ShopperToCounterState(Shopper shopper)
    {
        _shopper = shopper;
    }

    bool canMoveToCounter;
    float timer;

    public override int getState()
    {
        return (int)MachineShopperState.toCounter;
    }

    public override void onEnter(StateInfo info)
    {
        timer = 0;
        canMoveToCounter = false;
        //canMoveToCounter = _shopper.MoveToCounter();

        //Logger.error("新顾客进入了 去柜台 的状态");
    }

    public override void onUpdate()
    {
        if (!canMoveToCounter && _shopper.MoveToCounter())
        {
            canMoveToCounter = true;

        }

        if (canMoveToCounter)
        {
            if (!_shopper.isMoving)
            {
                _shopper.SetState(MachineShopperState.queuing, null);
            }
        }
        else
        {

            timer += Time.deltaTime;
            if (timer >= 2)
            {
                //去不了柜台
                refuse();
            }
        }


    }

    public override void onExit()
    {

    }

    private void refuse()
    {
        Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "顾客到不了柜台，直接拒绝~~~"));

        //拒绝的回调中切换状态
        //_machine.SetState((int)MachineShopperState.wait, null);

        //拒绝他！
        _shopper.SetState(MachineShopperState.leave, new ShopperLeaveReason(2));
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, _shopper.shopperData.data.shopperUid, true);
    }

}
