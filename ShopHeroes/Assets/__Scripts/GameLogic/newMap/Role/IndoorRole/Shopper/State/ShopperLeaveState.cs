using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopperLeaveReason : StateInfo
{
    public int reason; // 0 正常离开 1 进不到店内 2 到不了柜台  ... 99 没有对应类型的顾客

    public ShopperLeaveReason(int reason)
    {
        this.reason = reason;
    }
}

//顾客离开的状态
public class ShopperLeaveState : StateBase
{
    Shopper _shopper;

    public ShopperLeaveState(Shopper shopper)
    {
        _shopper = shopper;
    }

    bool canLeave;


    public override int getState()
    {
        return (int)MachineShopperState.leave;
    }


    public override void onEnter(StateInfo info)
    {
        canLeave = false;

        //Logger.error("新顾客进入了 离开 的状态");

        if (info == null)
        {
            //保险 容错 再关一次
            if (!_shopper.Attacher.isShowTalkPop) _shopper.HidePopup(() => _shopper.Attacher.SetVisible(false));
        }
        else
        {
            ShopperLeaveReason reson = info as ShopperLeaveReason;
            if (reson.reason == 1)
            {
#if UNITY_EDITOR
                Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "leave reson: 顾客进不来店内~~~"));
#endif
                if (IndoorMapEditSys.inst != null && (IndoorMapEditSys.inst.shopDesignMode == DesignMode.normal)) _shopper.Attacher.SetVisible(true);
                _shopper.SetTalkSpacing(999);
                _shopper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(_shopper.shopperData.data.shopperId, _shopper.shopperData.data.shopperLevel, (int)TalkConditionType.cantMoveToInDoor)));
            }
            else if (reson.reason == 2)
            {
                bool needTalk = false;

                if (_shopper.isBuy() || _shopper.shopperData.leaveFlag == 1)//是来买东西的 1/1 吐槽 或者因为柜台上限已满被强迫拒绝时 100%说话
                {
                    needTalk = true;
                }
                else //卖东西的 1/3 吐槽
                {
                    needTalk = Random.Range(0, 3) == 0;
                }

                if (needTalk)
                {
                    if (IndoorMapEditSys.inst != null && (IndoorMapEditSys.inst.shopDesignMode == DesignMode.normal)) _shopper.Attacher.SetVisible(true);
                    _shopper.SetTalkSpacing(999);
                    _shopper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(_shopper.shopperData.data.shopperId, _shopper.shopperData.data.shopperLevel, _shopper.shopperData.leaveFlag == 1 ? (int)TalkConditionType.queuingNumIsFull : (int)TalkConditionType.shopper_cantMoveToCounter)));
                }
#if UNITY_EDITOR
                Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "leave reson: 顾客到不了柜台~~~   是否为后端让其离开  : " + (_shopper.shopperData.leaveFlag == 1) + "     shopperUid : " + _shopper.shopperUid));
#endif
            }
            else if (reson.reason == 99)
            {
                Logger.error("没有对应类型顾客的处理逻辑，直接拒绝离开 顾客类型(对应EShopperType) : " + _shopper.shopperData.data.shopperType);
            }

        }

    }

    public override void onUpdate()
    {
        if (!canLeave && _shopper.MoveLeave())
        {
            canLeave = true;
        }

        if (canLeave && !_shopper.isMoving) //到地方了
        {
            ShopperDataProxy.inst.RemoveShopper(_shopper.shopperUid);
            _shopper.ClearPosCache();
            _shopper.DestroySelf();
        }
    }

    public override void onExit()
    {

    }
}
