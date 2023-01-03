using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerItemComp : MonoBehaviour
{
    public Text professionTxt;
    public Text nameTxt;
    public Text levelTxt;
    public Text expTxt;
    public GUIIcon headIcon;
    public GUIIcon typeIcon;
    public Slider expSlider;
    public GUIIcon sliderFillIcon;
    public Text addSpeedText;
    public GameObject levelLimitTipObj;
    public Text levelLimitTipTx;
    public GameObject unLockObj;
    public GameObject lockBgObj;
    public GameObject lockObj;
    public GUIIcon lockTypeIcon;
    public Text lockTips_up;
    public Text lockTips_down;
    public GameObject redPoint;

    //礼包
    //public Image lockImg;
    public GUIIcon giftBgIcon;
    public GUIIcon giftIcon;


    public void InitWorkerData(WorkerData data)
    {
        gameObject.SetActive(true);


        headIcon.SetSprite(StaticConstants.roleHeadIconAtlasName, data.config.icon);
        typeIcon.SetSprite("worker_atlas", data.config.profession_icon);
        nameTxt.text = LanguageManager.inst.GetValueByKey(data.config.name);
        professionTxt.text = LanguageManager.inst.GetValueByKey(data.config.profession);

        addSpeedText.text = "+" + data.addSpeed + "%";
        LayoutRebuilder.ForceRebuildLayoutImmediate(nameTxt.transform.parent as RectTransform);

        unLockObj.SetActive(data.state == EWorkerState.Unlock);
        lockObj.SetActive(data.state != EWorkerState.Unlock);
        lockBgObj.SetActive(data.state == EWorkerState.Locked);
        redPoint.SetActive(false);


        GUIHelper.SetUIGray(transform, data.state == EWorkerState.Locked);
        GUIHelper.SetSingleUIGray(headIcon.transform, data.state != EWorkerState.Unlock);

        if (data.state == EWorkerState.Unlock)//已解锁的
        {
            unLockObj.SetActive(true);
            lockObj.SetActive(false);

            levelTxt.text = data.level.ToString();
            expTxt.text = LanguageManager.inst.GetValueByKey("经验") + data.exp + "/<color=#72e75b>" + data.maxExp + "</color>";
            expSlider.value = Mathf.Max(0.05f, (float)data.exp / data.maxExp);


            CityBuildingData buildingData = UserDataProxy.inst.GetBuildingData(data.config.connect_build_id);

            if (buildingData == null)
            {
                Logger.error("未找到对应建筑物的信息  建筑物ID ： " + data.config.connect_build_id);
                levelLimitTipTx.gameObject.SetActiveFalse();
            }
            else
            {
                bool isActive = data.level >= buildingData.effectVal;
                levelLimitTipObj.SetActive(isActive);

                if (isActive)
                {
                    expSlider.value = 1;
                    expTxt.text = string.Empty;
                    sliderFillIcon.SetSprite("hero_atlas_2", "gongjiang_jinduman");
                }
                else
                {
                    sliderFillIcon.SetSprite("__common_1", "zhizuo_jindutiao1");
                }

                levelLimitTipTx.text = LanguageManager.inst.GetValueByKey("需要{0}级{1}", (buildingData.level + 1).ToString(), LanguageManager.inst.GetValueByKey(buildingData.config.name));
            }
        }
        else if (data.state == EWorkerState.CanUnlock)//可解锁的
        {
            unLockObj.SetActive(false);
            lockObj.SetActive(true);

            lockTips_up.gameObject.SetActive(true);
            lockTypeIcon.gameObject.SetActive(false);

            lockTips_up.text = LanguageManager.inst.GetValueByKey(data.config.lock_des);
            lockTips_down.text = LanguageManager.inst.GetValueByKey("招募{0}", LanguageManager.inst.GetValueByKey(data.config.name));
            lockTips_down.color = GUIHelper.GetColorByColorHex("FFCF0D");


            if (data.isNew)
            {
                redPoint.SetActive(true);
                data.isNew = false;
            }

            //lockImg.enabled = data.config.locked != 1;
            giftBgIcon.gameObject.SetActive(data.config.locked == 1);
            if (data.config.locked == 1)
            {
                if (HotfixBridge.inst.GetDirectPurchaseDataById(data.config.sale_id, out DirectPurchaseData directPurchaseData))
                {
                    giftBgIcon.SetSprite(directPurchaseData.bgIconAtlas, directPurchaseData.bgIcon);
                    giftIcon.SetSprite(directPurchaseData.iconAtlas, directPurchaseData.icon);
                }
                else
                {
                    giftBgIcon.gameObject.SetActive(false);
                    Logger.error("没有该礼包id对应的礼包，但是与之绑定的工匠是可解锁状态  工匠id: " + data.config.id + "   礼包id : " + data.config.sale_id);
                }
            }

        }
        else if (data.state == EWorkerState.Locked)//未解锁的   这里展示的都是 未解锁但可预览的（达到前置店主等级的）
        {

            unLockObj.SetActive(false);
            lockObj.SetActive(true);
            //lockImg.enabled = true;
            giftBgIcon.gameObject.SetActive(false);

            GUIHelper.SetUIGray(lockObj.transform, false);


            lockTips_up.gameObject.SetActive(false);
            lockTypeIcon.gameObject.SetActive(true);
            lockTypeIcon.SetSprite("worker_atlas", data.config.profession_icon);



            if (UserDataProxy.inst.playerData.level < data.config.level)
            {
                lockTips_down.text = LanguageManager.inst.GetValueByKey("需要店主达到{0}级", data.config.level.ToString());
            }
            else
            {
                if (data.config.get_type == (int)kRoleWorkerGetType.buildingLink)
                {
                    var buildingCfg = BuildingConfigManager.inst.GetConfig(data.config.build_id);
                    lockTips_down.text = LanguageManager.inst.GetValueByKey("需要{0}达到{1}级", LanguageManager.inst.GetValueByKey(buildingCfg.name), data.config.build_level_id.ToString());
                }
                else if (data.config.get_type == (int)kRoleWorkerGetType.giftLink)
                {
                    lockTips_down.text = LanguageManager.inst.GetValueByKey("需要等待礼包活动开启");
                }
            }


            lockTips_down.color = GUIHelper.GetColorByColorHex("FF586C");
        }

    }

}
