public class ReadyToLeaveStateInfo : StateInfo
{
    public int equipId = -1;
}

//顾客准备离开的状态
public class ShopperReadyToLeaveState : StateBase
{

    Shopper _shopper;

    public ShopperReadyToLeaveState(Shopper shopper)
    {
        _shopper = shopper;
    }

    ReadyToLeaveStateInfo stateInfo;


    public override int getState()
    {
        return (int)MachineShopperState.readyToLeave;
    }


    public override void onEnter(StateInfo info)
    {
        //Logger.error("新顾客进入了 准备离开 的状态");

        //保险 容错 再关一次
        if (!_shopper.Attacher.isShowTalkPop) _shopper.HidePopup(() => _shopper.Attacher.SetVisible(false));

        switch (_shopper.readyLeaveType)
        {
            case EAIReadyToLeave.none:

                //leave();
                _shopper.SetState(MachineShopperState.leave, new ShopperLeaveReason(2));

                break;

            case EAIReadyToLeave.success:

                stateInfo = info as ReadyToLeaveStateInfo;

                if (stateInfo != null && stateInfo.equipId != -1)
                {
                    //stateInfo.equipId = 25011; //测试盾牌表现
                    buyEquipBehavior();
                }
                else
                {
                    sellItemBehavior();
                }

                break;

            case EAIReadyToLeave.fail:

                byRefuseBehavior();

                break;
        }

    }

    public override void onUpdate()
    {

    }

    public override void onExit()
    {
    }


    //买到装备的行为
    private void buyEquipBehavior()
    {
        var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(stateInfo.equipId);

        if (equipCfg != null)
        {
            _shopper.Character.SwitchClothingByEquipId(stateInfo.equipId);

            if (equipCfg.equipDrawingsConfig.type == (int)EquipType.Weapon)//是武器
            {
                EGender gender = _shopper.shopperData.getGender();

                string weaponSlotName = gender == EGender.Male ? StaticConstants.man_weapon_slotName : StaticConstants.woman_weapon_slotName;
                string placeWeaponSlotName = SpineUtils.GetPlaceWeaponSlotName(gender, (EquipSubType)equipCfg.equipDrawingsConfig.sub_type, out string placeWeaponAniName);

                _shopper.Character.TakeOutSlotAttachment(placeWeaponSlotName); //先摘除 后放动画 防止穿帮

                string sellAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.successful_transaction);

                _shopper.Character.Play(sellAniName, completeDele: t =>
                {
                    _shopper.Character.Play(placeWeaponAniName, completeDele: c =>
                    {

                        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equipCfg.equipDrawingsConfig.sub_type);
                        if (classcfg != null)
                        {
                            float slotRot = gender == EGender.Male ? classcfg.m_rotation_angle : classcfg.g_rotation_angle;
                            _shopper.Character.AttToAnotherSlot(weaponSlotName, placeWeaponSlotName, slotRot, classcfg.GetSlotPos(gender), classcfg.GetSlotScale(gender));
                        }

                        leave();
                    });
                });
            }
            else if (equipCfg.equipDrawingsConfig.sub_type == (int)EquipSubType.shield)//盾牌 
            {
                EGender gender = _shopper.shopperData.getGender();

                string shieldSlotName = gender == EGender.Male ? StaticConstants.man_shield_slotName : StaticConstants.woman_shield_slotName;
                string placeShieldSlotName = SpineUtils.GetPlaceWeaponSlotName(gender, (EquipSubType)equipCfg.equipDrawingsConfig.sub_type, out string placeShieldAniName);

                _shopper.Character.TakeOutSlotAttachment(placeShieldSlotName); //先摘除 后放动画 防止穿帮

                string sellAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.successful_transaction);

                _shopper.Character.Play(sellAniName, completeDele: t =>
                {
                    _shopper.Character.Play(placeShieldAniName, completeDele: c =>
                    {

                        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equipCfg.equipDrawingsConfig.sub_type);
                        if (classcfg != null)
                        {
                            float slotRot = gender == EGender.Male ? classcfg.m_rotation_angle : classcfg.g_rotation_angle;
                            _shopper.Character.AttToAnotherSlot(shieldSlotName, placeShieldSlotName, slotRot, classcfg.GetSlotPos(gender), classcfg.GetSlotScale(gender));
                        }

                        leave();
                    });
                });
            }
            else // 非武器盾牌
            {
                string sellAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.successful_transaction);

                _shopper.Character.Play(sellAniName, completeDele: t =>
                {
                    leave();
                });
            }

        }
    }

    //卖出物品
    private void sellItemBehavior()
    {
        string sellAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.successful_transaction);

        _shopper.Character.Play(sellAniName, completeDele: t =>
        {
            leave();
        });
    }

    //被拒绝
    private void byRefuseBehavior()
    {
        string failAniName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_shopper.Character.gender, (int)kIndoorRoleActionType.rejected);

        _shopper.Character.Play(failAniName, completeDele: t =>
        {
            leave();
        });
    }


    private void leave()
    {
        if (_shopper == null) return;

        if (_shopper.readyLeaveType != EAIReadyToLeave.none)
        {
            //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.SUBTRACTROUNDCOUNTERNUM);
            HotfixBridge.inst.TriggerLuaEvent("SUBTRACTROUNDCOUNTERNUM");
            _shopper.readyLeaveType = EAIReadyToLeave.none;
        }

        //if (_shopper.shopperData.data.shopperState == (int)EShopperState.Leaving)
        //{
        _shopper.SetState(MachineShopperState.leave, null);
        //}
        //else
        //{
        //    _shopper.SetState(MachineShopperState.queuing, null);
        //}
    }

}
