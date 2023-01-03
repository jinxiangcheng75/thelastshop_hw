using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AccessoryData
{
    public ItemType type;
    public int itemId;
    public int quality;
    public long count;
    public int itemType;
    public itemConfig config;

    public AccessoryData(OneRewardItem info)
    {
        SetInfo(info);
    }

    public void SetInfo(OneRewardItem info)
    {
        itemId = info.itemId;
        quality = info.quality;
        count = info.count;
        itemType = info.itemType;

        config = ItemconfigManager.inst.GetConfig(itemId);

        if (config != null)
        {
            type = (ItemType)config.type;
        }
        else if((ItemType)itemType != ItemType.Equip)
        {
            Logger.error("OneRewardItem 未获取到对应itemCfg  itemID : " + info.itemId);
        }

    }

    public string atlas
    {
        get
        {
            if ((ItemType)itemType == ItemType.Equip)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(itemId);
                if (equipCfg != null)
                {
                    return equipCfg.equipDrawingsConfig.atlas;
                }
            }

            if (config == null) return string.Empty;

            if (type == ItemType.Hero)
            {
                var cfg = HeroProfessionConfigManager.inst.GetConfig(config.effect);
                return cfg == null ? "" : cfg.atlas;
            }
            else if (type == ItemType.EquipmentDrawing)
            {
                var drawCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(config.effect);
                return drawCfg == null ? "" : drawCfg.atlas;
            }

            return config.atlas;
        }
    }

    public string icon
    {
        get
        {
            if ((ItemType)itemType == ItemType.Equip)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(itemId);
                if (equipCfg != null)
                {
                    return equipCfg.equipDrawingsConfig.icon;
                }
            }

            if (config == null) return string.Empty;

            if (type == ItemType.Hero)
            {
                var cfg = HeroProfessionConfigManager.inst.GetConfig(config.effect);
                return cfg == null ? "" : cfg.ocp_icon;
            }
            else if (type == ItemType.EquipmentDrawing)
            {
                var drawCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(config.effect);
                return drawCfg == null ? "" : drawCfg.icon;
            }

            return config.icon;
        }
    }

}

public class EmailData
{
    public int id;
    public int state;//状态  EMailStatus    1 已读 2 未读   Unread = 0,Read = 1,Unclaimed = 2,Claimed = 3,Deleted = 99,

    public string title
    {
        get
        {
            return mailDetails.Find(t => t.lang == (int)LanguageManager.inst.curType).mailTitle;
        }
    }

    public string receiver
    {
        get
        {
            return mailDetails.Find(t => t.lang == (int)LanguageManager.inst.curType).mailReceiver;
        }
    }

    public string content
    {
        get
        {
            return mailDetails.Find(t => t.lang == (int)LanguageManager.inst.curType).mailContent;
        }
    }

    public string from
    {
        get
        {
            return mailDetails.Find(t => t.lang == (int)LanguageManager.inst.curType).mailSender;
        }
    }

    public int dateTime;//日期
    public int deadlineTime;//到期日期
    public List<AccessoryData> accessories;//附件信息
    public List<OneMailDetail> mailDetails;//邮件详情

    public bool hasAccessories //是否有附件
    {
        get
        {
            return accessories.Count > 0;
        }
    }

    public EmailData(OneMail info)
    {
        SetInfo(info);
    }

    public void SetInfo(OneMail info)
    {
        id = info.mailId;
        state = info.mailStatus;
        dateTime = info.createTime;
        deadlineTime = info.expireTime;
        mailDetails = info.detailList;

        accessories = new List<AccessoryData>();
        foreach (var item in info.itemList)
        {
            AccessoryData data = new AccessoryData(item);
            accessories.Add(data);
        }

    }

}
