using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//店主闲逛状态
public class ShopkeeperRambleState : StateBase
{
    Shopkeeper _shopkeeper;

    public ShopkeeperRambleState(Shopkeeper shopkeeper)
    {
        _shopkeeper = shopkeeper;
    }

    public override int getState()
    {
        return (int)MachineShopkeeperState.ramble;
    }

    bool isWaiting;
    float timer;
    float waitTime;

    /*
     * 条件无法满足 过渡到4  若4无法移动 直接站立等待
     * 1.有空货架时，行走至空货架附近空地后，站立并停止。  弹出aitalk，对应9类对话
     * 2.常规状态（无视是否有空货架）闲逛后在空地站立并停止。 弹出aitalk，对应10类对话
     * 3.店主闲逛后站立行走至空货架旁站立并停止。  不说话
     * 4.店主闲逛后站立在空第站立并停止。   不说话
     * 5.店主走到宠物处停留并说话。说完话后原地停留。  弹出aitalk，对应12类对话。
     */
    readonly int[] weights = AITalkProbConfigManager.inst.GetWeights(StaticConstants.shopkeeperWeightBase, StaticConstants.shopkeeperWeightBase + 1000); //{ 20, 20, 30, 30, 10 };

    public override void onEnter(StateInfo info)
    {

        int index = Helper.getRandomValuefromweights(weights);

        timer = 0;
        isWaiting = false;


        _shopkeeper.rambleType = (ShopkeeperRambleType)(index + StaticConstants.shopkeeperWeightBase + 1);

        //Logger.error("店主进入了 闲逛 的状态  随机到的行为是" + (int)_shopkeeper.rambleType);


        switch (index + 1)
        {
            case 1: scroll_one(); break;
            case 2: scroll_two(); break;
            case 3: scroll_three(); break;
            case 4: scroll_four(); break;
            case 5: scroll_five(); break;
        }

    }

    void beginWait()
    {
        float time_min = WorldParConfigManager.inst.GetConfig(1103).parameters;
        float time_max = WorldParConfigManager.inst.GetConfig(1104).parameters;

        float waitTime = Random.Range(time_min, time_max);

        //Logger.error("店主开始等待，距下一次闲逛行为时间(秒):  " + waitTime);
        timer = 0;
        this.waitTime = waitTime;
        isWaiting = true;
    }

    // 1.有空货架时，行走至空货架附近空地后，站立并停止。  弹出aitalk，对应13类对话
    void scroll_one()
    {
        int shelfUid = UserDataProxy.inst.GetOneEmptyShelfUid();

        if (shelfUid != -1 && _shopkeeper.MoveToFurniture(shelfUid))
        {
            _shopkeeper.onMoveEndCompleteHandler = () =>
            {
                int uid = shelfUid;
                _shopkeeper.FaceToFurniture(uid);
                _shopkeeper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetSpkeeperRandomTalk((int)TalkConditionType.shopkeeper_shelfIsNull)));
                _shopkeeper.onMoveEndCompleteHandler = null;
                beginWait();
            };
        }
        else
        {
            scroll_four();
        }


    }

    // 2.常规状态（无视是否有空货架）闲逛后在空地站立并停止。 弹出aitalk，对应14类对话
    void scroll_two()
    {
        bool canRamble = false;
        for (int i = 0; i < 3; i++)
        {
            if (canRamble) break;
            canRamble = _shopkeeper.MoveToRandomPos();
        }

        if (canRamble)
        {
            _shopkeeper.onMoveEndCompleteHandler = () =>
            {
                _shopkeeper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetSpkeeperRandomTalk((int)TalkConditionType.shopkeeper_rambleOver)));
                _shopkeeper.onMoveEndCompleteHandler = null;
                beginWait();
            };
        }
        else
        {
            scroll_four();
        }


    }

    // 3.店主闲逛后站立行走至空货架旁站立并停止。  不说话
    void scroll_three()
    {
        int shelfUid = UserDataProxy.inst.GetOneEmptyShelfUid();

        if (shelfUid != -1 && _shopkeeper.MoveToFurniture(shelfUid))
        {
            _shopkeeper.onMoveEndCompleteHandler = () =>
            {
                int uid = shelfUid;
                _shopkeeper.FaceToFurniture(uid);
                _shopkeeper.onMoveEndCompleteHandler = null;
                beginWait();
            };
        }
        else
        {
            scroll_four();
        }

    }

    // 4.店主闲逛后站立在空第站立并停止。   不说话
    void scroll_four()
    {
        bool canRamble = false;
        for (int i = 0; i < 3; i++)
        {
            if (canRamble) break;
            canRamble = _shopkeeper.MoveToRandomPos();
        }

        if (canRamble)
        {
            _shopkeeper.onMoveEndCompleteHandler = () =>
            {
                _shopkeeper.onMoveEndCompleteHandler = null;
                beginWait();
            };
        }
        else
        {
            beginWait();
        }

    }


    bool needSubPetSeeCount;
    int _petUid;

    // 5.店主走到宠物处停留并说话。说完话后原地停留。  弹出aitalk，对应12类对话。
    void scroll_five()
    {
        int petUid = UserDataProxy.inst.GetOnePetUid();

        if (petUid != -1 && _shopkeeper.MoveToPet(petUid))
        {
            Pet pet = IndoorRoleSystem.inst.GetPetByUid(petUid);
            pet.StayBySeeCount++;
            _petUid = petUid;
            needSubPetSeeCount = true;

            _shopkeeper.onMoveEndCompleteHandler = () =>
            {
                _shopkeeper.FaceToPet(petUid);

                string lookAnimName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopkeeper.Character.gender, (int)kIndoorRoleActionType.ornamental_pets);

                _shopkeeper.Character.Play(lookAnimName, completeDele: t =>
                {
                    if (this == null) return;
                    string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopkeeper.Character.gender, (int)kIndoorRoleActionType.normal_standby);
                    _shopkeeper.Character.Play(idleAnimationName, true);
                    pet.StayBySeeCount--;
                    needSubPetSeeCount = false;
                    _shopkeeper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetSpkeeperRandomTalk((int)TalkConditionType.lookPet)));
                    _shopkeeper.onMoveEndCompleteHandler = null;
                    beginWait();
                });
            };
        }
        else
        {
            scroll_four();
        }

    }

    public override void onUpdate()
    {
        if (isWaiting)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                onEnter(null);
            }
        }
    }

    public override void onExit()
    {
        if (needSubPetSeeCount)
        {
            Pet pet = IndoorRoleSystem.inst.GetPetByUid(_petUid);
            if (pet != null) pet.StayBySeeCount--;
        }
        _shopkeeper.onMoveEndCompleteHandler = null;
    }

}
