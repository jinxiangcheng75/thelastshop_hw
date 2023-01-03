using UnityEngine;

//店主在柜台附近的状态
public class ShopkeeperOnCounterRoundState : StateBase
{
    Shopkeeper _shopkeeper;

    bool isBargaining;//是否在讨价还价
    bool isIdle;//是否在待机

    float otherIdleAnimTime = 5f;//5秒
    float timer;


    float logicTimer;
    float logicRunTime = 1f;

    public ShopkeeperOnCounterRoundState(Shopkeeper shopkeeper)
    {
        _shopkeeper = shopkeeper;
    }

    public override int getState()
    {
        return (int)MachineShopkeeperState.onCounterRound;
    }

    public override void onEnter(StateInfo info)
    {
        if (_shopkeeper.CheckIsRoundCounter())
        {
            _shopkeeper.FaceToCounter();
        }
        else
        {
            //不在柜台附近
            _shopkeeper.SetState((int)MachineShopkeeperState.toCounter, null);
        }
        timer = 0;

        //Logger.error("新店主进入了 在柜台 的状态");

    }

    public override void onUpdate()
    {
        logicTimer += Time.deltaTime;

        if (logicTimer >= logicRunTime)
        {



            if (shopperIsBuy())
            {

                if (!isBargaining)
                {
                    bargaining();
                    isBargaining = true;
                }

                isIdle = false;
            }
            else
            {

                if (!isIdle)
                {
                    _shopkeeper.FaceToCounter();
                    string idleAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopkeeper.Character.gender, (int)kIndoorRoleActionType.normal_standby);
                    _shopkeeper.Character.Play(idleAniName, true);
                    isIdle = true;
                }


                isBargaining = false;
            }


            logicTimer = 0;
        }


        if (isIdle)
        {
            timer += Time.deltaTime;

            if (timer >= otherIdleAnimTime)
            {
                otherIdleAnim();

                if (_shopkeeper.isShowingAcheivement && Helper.randomResult(50)) //气泡抖动
                {
                    if (/*!_shopper.isBubbleAlpha &&*/ _shopkeeper.Attacher.headBubbleIsShow && !_shopkeeper.Attacher.isShowTalkPop)
                    {
                        _shopkeeper.Attacher.spAnim.CrossFade("idle_2", 0f);
                        _shopkeeper.Attacher.spAnim.Update(0f);
                        _shopkeeper.Attacher.spAnim.Play("idle_2");
                    }
                }

                timer = 0;
            }
        }

    }

    public override void onExit()
    {

    }


    //判断是否需要讨价还价
    private bool shopperIsBuy()
    {
        var shopperUI = GUIManager.GetWindow<ShopperUIView>();
        bool result = shopperUI != null ? shopperUI.isShowing : false;

        return result;
    }

    //讨价还价
    private void bargaining()
    {

        string bargainingAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopkeeper.Character.gender, (int)kIndoorRoleActionType.haggle);
        _shopkeeper.Character.Play(bargainingAnimationName, true);
    }


    //换个姿势
    private void otherIdleAnim()
    {
        if (Random.Range(0, 8) == 0) //8分之一的概率
        {
            string specialIdleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopkeeper.Character.gender, (int)kIndoorRoleActionType.special_standby);
            _shopkeeper.Character.Play(specialIdleAnimationName, completeDele: t =>
            {
                if (_shopkeeper == null) return;
                string idleAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopkeeper.Character.gender, (int)kIndoorRoleActionType.normal_standby);
                _shopkeeper.Character.Play(idleAniName, true);
            });
        }
    }

}
