using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonRewardItem : MonoBehaviour
{
    public GUIIcon bgIcon;
    public GUIIcon icon;
    public Text numText;
    public Text nameText;
    public Button selfBtn;
    public Image tuzhiImg;

    public Animator btnAnimator;

    private CommonRewardData data;

    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, data, selfBtn.transform);
        });
    }

    public void SetData(CommonRewardData _data)
    {
        //Logger.log("获得的奖励id是" + _data.rewardId);
        if (_data.rewardId == -1)
        {
            gameObject.SetActive(false);
            return;
        }

        data = _data;

        tuzhiImg.enabled = false;
        if ((ItemType)data.itemType == ItemType.Craftsman)
        {
            var workItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            WorkerConfig workerCfg = null;
            if (workItemCfg != null)
            {
                workerCfg = WorkerConfigManager.inst.GetConfig(workItemCfg.effect);
            }
            else
            {
                workerCfg = WorkerConfigManager.inst.GetConfig(data.rewardId);
            }

            if (workerCfg != null)
            {
                icon.SetSprite("portrait_atlas", workerCfg.pic);
                nameText.text = LanguageManager.inst.GetValueByKey(workerCfg.name);
            }
        }
        else if ((ItemType)data.itemType == ItemType.Equip)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(data.rewardId);
            if (equipCfg != null)
            {
                icon.SetSprite(equipCfg.equipDrawingsConfig.atlas, equipCfg.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[equipCfg.equipQualityConfig.quality - 1]);
                nameText.text = equipCfg.quality_name;
            }
        }
        else if ((ItemType)data.itemType == ItemType.EquipmentDrawing)
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            EquipDrawingsConfig equipDrawingCfg = null;
            if (tempItemCfg != null)
                equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
            else
                equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(data.rewardId);
            if (equipDrawingCfg != null)
            {
                icon.SetSprite(equipDrawingCfg.atlas, equipDrawingCfg.icon);
                nameText.text = LanguageManager.inst.GetValueByKey(equipDrawingCfg.name);
            }
            tuzhiImg.enabled = true;
        }
        else if ((ItemType)data.itemType == ItemType.ShopkeeperDress)
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            dressconfig dressCfg = null;
            if (tempItemCfg != null)
            {
                dressCfg = dressconfigManager.inst.GetConfig(tempItemCfg.effect);
            }
            else
            {
                dressCfg = dressconfigManager.inst.GetConfig(data.rewardId);
            }

            if (dressCfg != null)
            {
                icon.SetSprite("ClotheIcon_atlas", dressCfg.icon);
                nameText.text = LanguageManager.inst.GetValueByKey(dressCfg.name);
            }
        }
        else if ((ItemType)data.itemType == ItemType.Furniture)
        {
            var itemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            FurnitureConfig furnitureCfg = null;
            if (itemCfg != null)
                furnitureCfg = FurnitureConfigManager.inst.getConfig(itemCfg.effect);
            else
                furnitureCfg = FurnitureConfigManager.inst.getConfig(data.rewardId);
            if (furnitureCfg != null)
            {
                icon.SetSprite(furnitureCfg.atlas, furnitureCfg.icon);
                nameText.text = LanguageManager.inst.GetValueByKey(furnitureCfg.name);
            }
        }
        else
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            if (tempItemCfg != null)
            {
                icon.SetSprite(tempItemCfg.atlas, tempItemCfg.icon);
                nameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
            }
        }

        if (data.rarity == 0)
        {
            bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetItemBgIcon[0]);
            nameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);
        }
        else
        {
            bgIcon.SetSprite(StaticConstants.commonAtlas, StaticConstants.GetItemBgIcon[data.rarity - 1]);
            nameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[data.rarity - 1]);
        }

        numText.text = data.count.ToString();
        gameObject.SetActive(true);

    }

    public void setData(CommonRewardData _data, int index)
    {
        SetData(_data);

        if (GameSettingManager.inst.needShowUIAnim) doAnim(index);
    }

    public void clearData()
    {
        gameObject.SetActive(false);
    }


    void doAnim(int index)
    {
        bgIcon.gameObject.SetActive(false);

        GameTimer.inst.AddTimer(0.46f + 0.1f * index, 1, () =>
        {
            if (this != null)
            {
                bgIcon.gameObject.SetActive(true);
                btnAnimator.CrossFade("show", 0f);
                btnAnimator.Update(0f);
                btnAnimator.Play("show");
            }
        });
    }

}
