using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerInvestItem : MonoBehaviour
{
    private CityBuildingData _data;

    public GUIIcon BackGround;

    [Header("Top")]
    public GUIIcon icon;
    public Text lvTx;
    public Text professionTx;

    [Header("已解锁")]
    public GameObject unLockObj;
    public GUIIcon professionBg;
    public Text curLvTx;
    public Text nextLvTx;
    public Slider expSlider;
    public Text sliderTx;
    public GameObject redPointObj;

    public GameObject unLock_maxObj;//满级
    public Text unLock_maxLvTx;
    public Text unLock_maxProfessionLvTips;


    public GameObject unLock_notMaxObj;//未满级
    public Text unLock_notMaxProfessionLvTips;


    [Header("未解锁")]
    public GameObject lockObj;
    public Text lockTips;
    public GUIIcon lockIcon;
    public Slider lockSlider;
    public Text lockSliderTx;


    private void Start()
    {
        var button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(onButtonClick);
        }
    }

    private void onButtonClick()
    {
        if (_data.state == 0 && _data.config.unlock_type == (int)kCityBuildingUnlockType.NeedOneWorker)
        {
            EventController.inst.TriggerEvent<int, bool, System.Action>(GameEventType.WorkerCompEvent.Worker_ClickToRecruit, _data.config.unlock_id, false, () =>
            {
                EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, _data.buildingId);
            });
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, _data.buildingId);
        }
    }

    private string getLockTips()
    {

        //lockSliderTx.text = "";
        string result = "";
        lockIcon.gameObject.SetActiveFalse();

        switch ((kCityBuildingUnlockType)_data.config.unlock_type)
        {
            case kCityBuildingUnlockType.Unlock:
                result = LanguageManager.inst.GetValueByKey("已解锁");
                break;
            case kCityBuildingUnlockType.BuildingLv:

                var data = UserDataProxy.inst.GetBuildingData(_data.config.unlock_id);
                var cfg = BuildingConfigManager.inst.GetConfig(_data.config.unlock_id);
                lockSlider.maxValue = _data.config.unlock_val;
                lockSlider.value = data == null ? 0 : data.level;

                result = LanguageManager.inst.GetValueByKey("需{0}等级达到{1}", cfg.name, _data.config.unlock_val.ToString());
                break;
            case kCityBuildingUnlockType.ShopLv:
                uint shopLv = UserDataProxy.inst.playerData.level;
                lockSlider.maxValue = _data.config.unlock_val;
                lockSlider.value = shopLv;

                result = LanguageManager.inst.GetValueByKey("需店主等级达到{0}", _data.config.unlock_val.ToString());
                break;
            case kCityBuildingUnlockType.NeedOneWorker:
                var config = WorkerConfigManager.inst.GetConfig(_data.config.unlock_id);
                lockSlider.maxValue = 1;
                lockSlider.value = 0;
                result = LanguageManager.inst.GetValueByKey("需解锁工匠{0}", LanguageManager.inst.GetValueByKey(config.name));
                lockIcon.gameObject.SetActiveTrue();
                lockIcon.SetSprite(StaticConstants.roleHeadIconAtlasName, config.icon);

                break;
            default:
                break;
        }

        lockSliderTx.text = lockSlider.value + "/" + lockSlider.maxValue;

        return result;
    }


    public void SetData(CityBuildingData data)
    {
        gameObject.name = data.buildingId.ToString();
        _data = data;

        lockObj.SetActive(_data.state == 0 && _data.config.unlock_type != (int)kCityBuildingUnlockType.NeedOneWorker);
        unLockObj.SetActive(_data.state != 0 || _data.config.unlock_type == (int)kCityBuildingUnlockType.NeedOneWorker);

        if (_data.state == 0 && _data.config.unlock_type != (int)kCityBuildingUnlockType.NeedOneWorker) //未解锁并且不是工匠类型解锁
        {
            BackGround.SetSprite("__common_1", "zhizuo_wupindi1");
            lockTips.text = getLockTips();
        }
        else
        {
            redPointObj.SetActive(_data.isNew);
            if (_data.isNew) _data.isNew = false;

            expSlider.value = Mathf.Max(0.05f, (float)_data.costCount / _data.needClickCount);

            var nextUpCfg = BuildingUpgradeConfigManager.inst.GetConfig(_data.buildingId, _data.level + 1);

            if (nextUpCfg == null)//满级情况
            {
                unLock_maxLvTx.text = LanguageManager.inst.GetValueByKey("{0}级", _data.level.ToString());
                unLock_maxObj.gameObject.SetActive(true);
                unLock_notMaxObj.gameObject.SetActive(false);
                BackGround.SetSprite("__common_1", "zhizuo_wupindi2");
                professionBg.SetSprite("__common_1", "zhizuo_mingzidi1");
            }
            else
            {
                unLock_maxObj.gameObject.SetActive(false);
                unLock_notMaxObj.gameObject.SetActive(true);
                BackGround.SetSprite("__common_1", "zhizuo_wupindi");
                professionBg.SetSprite("__common_1", "zhizuo_mingzidi");
                expSlider.gameObject.SetActiveTrue();
            }

            if (_data.buildingId == 3700) //康复研究
            {
                var cfg = nextUpCfg == null ? _data.upgradeConfig : nextUpCfg;

                curLvTx.text = (UserDataProxy.inst.GetExploreGroupRestTimeSpeedUp(cfg.effect_id) * 100) + "%";
                nextLvTx.text = cfg.GetEffectDec();
            }
            else if (_data.buildingId == 3800) //探险研究 
            {
                var cfg = nextUpCfg == null ? _data.upgradeConfig : nextUpCfg;

                curLvTx.text = (UserDataProxy.inst.GetExploreDropMaterialOutputUp(cfg.effect_id) * 100) + "%";
                nextLvTx.text = cfg.GetEffectDec();
            }
            else
            {
                curLvTx.text = _data.upgradeConfig.GetEffectDec();
                nextLvTx.text = nextUpCfg == null ? LanguageManager.inst.GetValueByKey("已满级") : nextUpCfg.GetEffectDec();
            }

            sliderTx.text = _data.costCount + "/" + _data.needClickCount;
        }


        string iconName = _data.GetIconAndAtlas(out string atlas, _data.level);
        icon.SetSprite(atlas, iconName);

        lvTx.text = _data.level.ToString();
        professionTx.text = LanguageManager.inst.GetValueByKey(_data.config.name);
        unLock_notMaxProfessionLvTips.text = unLock_maxProfessionLvTips.text = LanguageManager.inst.GetValueByKey(_data.upgradeConfig.effect_dec);

    }

}
