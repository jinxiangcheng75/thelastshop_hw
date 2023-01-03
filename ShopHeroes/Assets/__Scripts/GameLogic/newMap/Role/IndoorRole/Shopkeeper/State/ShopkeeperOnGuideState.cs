using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperOnGuideState : StateBase
{
    Shopkeeper _shopkeeper;

    public ShopkeeperOnGuideState(Shopkeeper shopkeeper)
    {
        _shopkeeper = shopkeeper;
    }

    public override int getState()
    {
        return (int)MachineShopkeeperState.inGuide;
    }

    public override void onEnter(StateInfo info)
    {
    }

    public override void onUpdate()
    {

    }
}
