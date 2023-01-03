using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryRewardItem : MonoBehaviour
{
    public GUIIcon bgIcon;
    public GUIIcon icon;
    public Text nameText;
    public Text numText;
    public GUIIcon qualityIcon;
    public Text qualityText;
    public GameObject rareEffect;
    public GameObject srareEffect;
    public Button selfBtn;

    CommonRewardData data;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, data, selfBtn.transform);
        });
    }

    public void setData(CommonRewardData _data)
    {
        Logger.log("转盘获得的奖励id是" + _data.rewardId);
        if (_data.rewardId == -1)
        {
            gameObject.SetActive(false);
            return;
        }
        data = _data;
        var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
        if (tempItemCfg == null)
        {
            var equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(data.rewardId);
            icon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1]);
            nameText.text = equipConfig.quality_name;
        }
        else
        {
            nameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
            if (tempItemCfg.type == 16)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
                icon.SetSprite(equipCfg.atlas, equipCfg.icon);
            }
            else
            {
                icon.SetSprite(tempItemCfg.atlas, tempItemCfg.icon);
            }
        }

        rareEffect.SetActive(false);
        srareEffect.SetActive(false);
        if (data.rarity == 2)
        {
            rareEffect.SetActive(true);
            qualityIcon.gameObject.SetActive(true);
            qualityIcon.SetSprite("lottery_atlas", StaticConstants.GetLotteryQualityBgIcon[0]);
            qualityText.text = LanguageManager.inst.GetValueByKey("稀有");
            nameText.color = GUIHelper.GetColorByColorHex("#f10dff");
        }
        else if (data.rarity == 3)
        {
            AudioManager.inst.PlaySound(106);
            srareEffect.SetActive(true);
            qualityIcon.gameObject.SetActive(true);
            qualityIcon.SetSprite("lottery_atlas", StaticConstants.GetLotteryQualityBgIcon[1]);
            qualityText.text = LanguageManager.inst.GetValueByKey("超稀有");
            nameText.color = GUIHelper.GetColorByColorHex("#ffd40d");
        }
        else
        {
            qualityIcon.gameObject.SetActive(false);
            nameText.color = Color.white;
        }

        if (data.rarity == 0)
        {
            bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetLotteryBgIcon[0]);
        }
        else
        {
            bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetLotteryBgIcon[data.rarity - 1]);
        }

        numText.text = data.count.ToString();
        gameObject.SetActive(true);
    }

    public void clearData()
    {
        gameObject.SetActive(false);
    }
}
