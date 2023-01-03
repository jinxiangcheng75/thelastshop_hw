using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//顾客排队的状态
public class ShopperQueuingState : StateBase
{
    Shopper _shopper;

    float checkTopPopOutTime = 2f;//2秒
    int checkTopPopOutTimer;

    float otherIdleAnimTime = 5f;//5秒
    float otherIdleAnimTimer;

    float bubbleShakeAnimTime_bottom = 4f;
    float bubbleShakeAnimTime_top = 6f;
    float bubbleShakeAnimTime_time;
    float bubbleShakeAnimTime_timer;


    public ShopperQueuingState(Shopper shopper)
    {
        _shopper = shopper;
    }

    public override int getState()
    {
        return (int)MachineShopperState.queuing;
    }

    public override void onEnter(StateInfo info)
    {
        otherIdleAnimTimer = 0;
        _shopper.isNew = false;
        _shopper.ShowPopupCheckOut();

        string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.normal_standby);
        _shopper.Character.Play(idleAnimationName, true);
        //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.ADDROUNDCOUNTERNUM);
        HotfixBridge.inst.TriggerLuaEvent("ADDROUNDCOUNTERNUM");
        //Logger.error("新顾客进入了 排队 的状态");

        checkTopPopOutTimer = GameTimer.inst.AddTimer(checkTopPopOutTime, () =>
         {

             if (null == this || _shopper == null)
             {
                 if (checkTopPopOutTimer != 0)
                 {
                     GameTimer.inst.RemoveTimer(checkTopPopOutTimer);
                     checkTopPopOutTimer = 0;
                 }
                 return;
             }

             if (!shopperUIViewShow() && !_shopper.Attacher.headBubbleIsShow)
             {
                 _shopper.ShowPopupCheckOut();
             }


         });
        bubbleShakeAnimTime_time = Random.Range(bubbleShakeAnimTime_bottom, bubbleShakeAnimTime_top);

        if (_shopper.shopperData.data.shopperType == (int)EShopperType.HighPriceBuy)
        {
            EventController.inst.TriggerEvent(GameEventType.PetCompEvent.PET_LEAVEDOORWAY);
        }

        _shopper.RefreshCurCellPos(true);
    }


    public override void onUpdate()
    {
        if (!_shopper.isBargaining)
        {
            otherIdleAnimTimer += Time.deltaTime;

            if (otherIdleAnimTimer >= otherIdleAnimTime)
            {
                otherIdleAnimTimer = 0;
                otherIdleAnim();
            }
        }


        bubbleShakeAnimTime_timer += Time.deltaTime;

        if (bubbleShakeAnimTime_timer >= bubbleShakeAnimTime_time)
        {
            bubbleShakeAnimTime_timer = 0;
            bubbleShakeAnimTime_time = Random.Range(bubbleShakeAnimTime_bottom, bubbleShakeAnimTime_top);

            if (Helper.randomResult(50)) //气泡抖动
            {
                if (/*!_shopper.isBubbleAlpha &&*/ !shopperUIViewShow() && _shopper.Attacher.headBubbleIsShow && !_shopper.Attacher.isShowTalkPop)
                {
                    _shopper.Attacher.spAnim.CrossFade("null", 0f);
                    _shopper.Attacher.spAnim.Update(0f);
                    _shopper.Attacher.spAnim.Play("idle_2");
                }
            }
        }

    }

    public override void onExit()
    {
        if (checkTopPopOutTimer != 0)
        {
            GameTimer.inst.RemoveTimer(checkTopPopOutTimer);
            checkTopPopOutTimer = 0;
        }

    }

    //换个姿势
    private void otherIdleAnim()
    {
        if (Helper.randomResult(50)) // 1/2 的概率
        {
            string specialIdleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.special_standby);
            _shopper.Character.Play(specialIdleAnimationName, completeDele: t =>
            {
                if (_shopper == null) return;
                string idleAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.normal_standby);
                _shopper.Character.Play(idleAniName, true);
            });
        }
    }

    //判断shopperUI在打开的情况下气泡是否打开
    private bool shopperUIViewShow()
    {
        var shopperUI = GUIManager.GetWindow<ShopperUIView>();
        bool result = shopperUI != null ? shopperUI.isShowing : false;

        return result;
    }

}
