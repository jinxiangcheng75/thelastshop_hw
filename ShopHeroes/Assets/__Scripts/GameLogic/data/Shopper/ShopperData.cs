
using System.Collections.Generic;

public class ShopperData
{
    public OneShopper data;
    int timerId;
    public bool isCacheRamble;
    public bool isAutoLeave;
    public int leaveFlag = 0;

    public ShopperData(OneShopper shopper)
    {
        data = shopper;

        ////// 测试
        //data.shopperComeType = (int)EShopperComeType.GuideTask;
        //data.shopperId = 10;
        //data.shopperType = (int)EShopperType.SellCopyItem;
        //data.targetEquipId = 0;
        //data.targetItemId = 30005;
        //data.targetCount = 7;
        //data.price = 6000000;
        //////

        isAutoLeave = shopper.leaveTime > 0;
        SetTimer();
    }

    public EGender getGender()
    {
        return (EGender)data.gender;
    }

    public List<int> GetEquips()
    {
        List<int> equips = new List<int>();

        foreach (HeroEquip item in data.equips)
        {
            if (item.equipId != 0)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(item.equipId);

                if (equipCfg != null && equipCfg.dressId != 213010) //临时屏蔽木桶头盔
                {
                    equips.Add(item.equipId);
                }
            }
        }

        return equips;
    }

    public int GetInitWeaponEquipType_2()
    {
        int weaponType_2 = -1;
        foreach (HeroEquip item in data.equips)
        {
            var cfg = EquipConfigManager.inst.GetEquipInfoConfig(item.equipId);
            if (cfg == null) continue;

            if (cfg.equipDrawingsConfig.type == (int)EquipType.Weapon)
            {
                weaponType_2 = cfg.equipDrawingsConfig.sub_type;
            }
        }

        return weaponType_2;
    }

    public EquipConfig GetInitShieldEquipCfg()
    {
        EquipConfig equipCfg = null;
        foreach (HeroEquip item in data.equips)
        {
            var cfg = EquipConfigManager.inst.GetEquipInfoConfig(item.equipId);
            if (cfg == null) continue;

            if (cfg.equipDrawingsConfig.type == (int)EquipSubType.shield)
            {
                equipCfg = cfg;
            }
        }

        return equipCfg;
    }

    public void setPriceDouble()
    {
        data.hasDouble = 1;
    }

    public void SetTimer()
    {

        if (timerId != 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (data.shopperType == (int)EShopperType.Warrior && data.shopperState == (int)EShopperState.Queuing && isAutoLeave)
        {
            int ticks = data.leaveTime;

            timerId = GameTimer.inst.AddTimer(1, ticks, () =>
            {
                data.leaveTime -= 1;

                if (data.leaveTime <= 0)
                {
                    data.leaveTime = 1;
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, data.shopperUid, true);
                }
            });

        }
    }

}
