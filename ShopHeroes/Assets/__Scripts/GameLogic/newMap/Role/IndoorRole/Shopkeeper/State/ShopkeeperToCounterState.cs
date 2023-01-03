using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperToCounterState : StateBase
{
    Shopkeeper _shopkeeper;

    public ShopkeeperToCounterState(Shopkeeper shopkeeper)
    {
        _shopkeeper = shopkeeper;
    }

    bool isCanMoveToCounter;
    int num;
    int maxNum = 3;

    public override int getState()
    {
        return (int)MachineShopkeeperState.toCounter;
    }

    public override void onEnter(StateInfo info)
    {
        isCanMoveToCounter = false;

        num = 0;
        _shopkeeper.isCanMoveToCounter = isCanMoveToCounter = _shopkeeper.moveToCounter();
    }

    public override void onUpdate()
    {

        //柜台在错误位置
        if (IndoorMap.inst.GetFurnituresByUid(UserDataProxy.inst.GetCounter().uid, out Furniture counter))
        {
            if (!counter.Results)
            {
                _shopkeeper.stopMove();
                if (!_shopkeeper.isTalking) _shopkeeper.Talk(LanguageManager.inst.GetValueByKey("我走不到柜台啊"));
                return;
            }
        }

        if (!_shopkeeper.isTalking && !isCanMoveToCounter)
        {
            _shopkeeper.Talk(LanguageManager.inst.GetValueByKey("我走不到柜台啊"));
            return;
        }

        if (isCanMoveToCounter && !_shopkeeper.isMoving)
        {
            if (_shopkeeper.CheckIsRoundCounter())
            {
                if (!_shopkeeper.isTalking)
                {
                    _shopkeeper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetSpkeeperRandomTalk((int)TalkConditionType.shopkeeper_customerCome)));
                }

                _shopkeeper.SetState((int)MachineShopkeeperState.onCounterRound, null);
            }
            else
            {
                if (num < maxNum)
                {
                    num++;
                    _shopkeeper.isCanMoveToCounter = isCanMoveToCounter = _shopkeeper.moveToCounter();
                }
                else
                {
                    _shopkeeper.stopMove();
                    if (!_shopkeeper.isTalking) _shopkeeper.Talk(LanguageManager.inst.GetValueByKey("我走不到柜台啊"));
                }
            }
        }

    }

    public override void onExit()
    {

    }
}
