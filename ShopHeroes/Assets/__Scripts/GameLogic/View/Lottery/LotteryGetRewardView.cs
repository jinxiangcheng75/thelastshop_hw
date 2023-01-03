using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotteryGetRewardView : ViewBase<LotteryGetRewardComp>
{
    public override string viewID => ViewPrefabName.LotteryGetReward;

    public override string sortingLayerName => "popup";
    Tween tween;

    protected override void onInit()
    {
        base.onInit();
        contentPane.confirmBtn.ButtonClickTween(() =>
        {
            hide();
            buttonClickHandler();
        });
        contentPane.singleConfirmBtn.ButtonClickTween(() =>
        {
            hide();
            buttonClickHandler();
        });
    }

    private void buttonClickHandler()
    {
        EventController.inst.TriggerEvent(GameEventType.LotteryEvent.LOTTERY_GETREWARDCOMPLETE, LotteryDataProxy.inst.tempList.Count > 0);
        HotfixBridge.inst.TriggerLuaEvent("WelfareLottery_refreshCumulativeData", LotteryDataProxy.inst.tempList.Count > 0);
    }

    public void setRewardInfo(List<CommonRewardData> rewardsList)
    {
        contentPane.allObj.SetActive(true);
        contentPane.singleObj.SetActive(false);
        contentPane.effectTrans.gameObject.SetActive(false);
        for (int i = 0; i < contentPane.allItems.Count; i++)
        {
            int index = i;
            if (index < rewardsList.Count)
            {
                contentPane.allItems[index].setData(rewardsList[index]);
            }
            else
            {
                contentPane.allItems[index].clearData();
            }
        }
    }

    public void setSingleRewardInfo(CommonRewardData data)
    {
        contentPane.effectTrans.gameObject.SetActive(true);
        tween = contentPane.effectTrans.DORotate(new Vector3(0, 0, -360 * 4), 20).SetEase(Ease.Linear).SetLoops(-1);
        contentPane.allObj.SetActive(false);
        contentPane.singleObj.SetActive(true);

        contentPane.singleIcon.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * 256;

        if ((ItemType)data.itemType == ItemType.Craftsman)
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            var workerCfg = WorkerConfigManager.inst.GetConfig(tempItemCfg.effect);
            contentPane.singleIcon.SetSprite("portrait_atlas", workerCfg.pic);
            contentPane.singleDescText.text = LanguageManager.inst.GetValueByKey(workerCfg.name) + "x" + data.count;
        }
        else if ((ItemType)data.itemType == ItemType.Equip)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(data.rewardId);
            contentPane.singleIcon.SetSpriteURL(equipCfg.equipDrawingsConfig.big_icon, StaticConstants.qualityTxtColor[equipCfg.equipQualityConfig.quality - 1]);
            contentPane.singleIcon.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * 512;
            contentPane.singleDescText.text = equipCfg.quality_name + "x" + data.count;
        }
        else if ((ItemType)data.itemType == ItemType.EquipmentDrawing)
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
            contentPane.singleIcon.SetSpriteURL(equipDrawingCfg.big_icon);
            contentPane.singleDescText.text = LanguageManager.inst.GetValueByKey(equipDrawingCfg.name) + "x" + data.count;
        }
        else if ((ItemType)data.itemType == ItemType.ShopkeeperDress)
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            var dressCfg = dressconfigManager.inst.GetConfig(tempItemCfg.effect);
            contentPane.singleIcon.SetSprite("ClotheIcon_atlas", dressCfg.icon);
            contentPane.singleDescText.text = LanguageManager.inst.GetValueByKey(dressCfg.name) + "x" + data.count;
        }
        else if ((ItemType)data.itemType == ItemType.Furniture)
        {
            var itemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            var furnitureCfg = FurnitureConfigManager.inst.getConfig(itemCfg.effect);
            contentPane.singleIcon.SetSprite(furnitureCfg.atlas,furnitureCfg.icon);
            contentPane.singleDescText.text = LanguageManager.inst.GetValueByKey(furnitureCfg.name) + "x" + data.count;
        }
        else
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            if (tempItemCfg != null)
            {
                contentPane.singleIcon.SetSpriteURL(tempItemCfg.icon);
                contentPane.singleDescText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name) + "x" + data.count;
            }
        }

        if (GameSettingManager.inst.needShowUIAnim)
        {
            contentPane.singleAnim.CrossFade("show", 0f);
            contentPane.singleAnim.Update(0f);
            contentPane.singleAnim.Play("show");
        }
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(25);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
        tween.Kill();
        var topview = GUIManager.GetWindow<TopPlayerInfoView>();
        if (topview != null)
        {
            topview.UpdateShow();
        }
    }
}
