using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoxRewardItem : MonoBehaviour
{
    public Transform bgTrans;
    public GUIIcon icon;
    public Text numText;
    public Button selfBtn;
    public GUIIcon bgIcon;
    public Text rarityText;
    public Image tuzhidiImg;
    public Image huangguanImg;
    OneRewardItem data;
    CommonRewardData commonData;

    Vector2 curPos;
    RectTransform rect;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, selfBtn.transform);
        });

        rect = transform.GetComponent<RectTransform>();
    }

    public void setData(OneRewardItem item, int index, bool isVip = false)
    {
        data = item;
        commonData = new CommonRewardData(data.itemId, data.count, data.quality, data.itemType);
        tuzhidiImg.enabled = false;
        var cfg = ItemconfigManager.inst.GetConfig(item.itemId);
        if (item.itemType == 16)
        {
            tuzhidiImg.enabled = true;
            if (cfg != null)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(cfg.effect);
                icon.SetSprite(equipCfg.atlas, equipCfg.icon);
            }
            else
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(item.itemId);
                if (equipCfg != null)
                    icon.SetSprite(equipCfg.atlas, equipCfg.icon);
            }

        }
        else if (item.itemType == 26)
        {
            var qualityCfg = EquipConfigManager.inst.GetEquipDrawingsCfgByEquipId(item.itemId);
            EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(item.itemId);
            icon.SetSprite(qualityCfg.atlas, qualityCfg.icon, StaticConstants.qualityTxtColor[eqcfg.quality - 1]);
        }
        else
        {
            if (cfg != null)
                icon.SetSprite(cfg.atlas, cfg.icon);
        }

        if (item.quality <= 1)
        {
            bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetLotteryBgIcon[0]);
        }
        else if (item.quality == 2)
        {
            if ((ItemType)item.itemType == ItemType.Equip)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(data.itemId);
                bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetItemBgIcon[equipCfg.equipQualityConfig.quality - 1]);
            }
            else
            {
                bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetItemBgIcon[cfg.property > 0 ? cfg.property - 1 : 0]);
            }
        }
        else if (item.quality == 3)
        {
            bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetLotteryBgIcon[item.quality - 1]);
        }

        numText.text = "x" + item.count;
        rarityText.gameObject.SetActive(item.quality != 1);
        if (item.quality == 2)
        {
            rarityText.text = LanguageManager.inst.GetValueByKey("稀有");
            rarityText.color = GUIHelper.GetColorByColorHex("#f77dff");
        }
        else if (item.quality == 3)
        {
            rarityText.text = LanguageManager.inst.GetValueByKey("专属图纸");
            rarityText.color = GUIHelper.GetColorByColorHex("#ffc12c");
        }

        huangguanImg.enabled = isVip;

        curPos = rect.anchoredPosition;
    }

    public void setAnimation(int index)
    {
        GameTimer.inst.AddTimer(0.1f + 0.1f * index, 1, () =>
        {
            if (this == null) return;
            bgTrans.DOScale(new Vector3(1, 1, 1), 0.25f).From(new Vector3(1.5f, 1.5f, 1.5f)).OnStart(() =>
            {
                if (this == null) return;
                //gameObject.SetActive(true);
            });
            //rect.DOAnchorPos(curPos, 0.25f).From(new Vector2(472, 0));
        });
    }

    public void clearData()
    {
        bgTrans.localScale = new Vector3(0, 0, 0);
        DOTween.Kill(bgTrans);
    }
}
