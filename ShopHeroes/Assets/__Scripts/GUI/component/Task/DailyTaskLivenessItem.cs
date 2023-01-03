using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyTaskLivenessItem : MonoBehaviour
{

    public Button boxBtn;
    public GUIIcon bg_icon;
    public GUIIcon icon;
    public Text needPointText;
    public Text tx_num;
    public Image img_Vip;
    public GameObject alreadyGetObj;
    public GameObject canGetVFXObj;

    ActiveRewardBoxData _data;

    void Start()
    {
        boxBtn.ButtonClickTween(onButtonClick);
    }

    public void SetData(ActiveRewardBoxData data, float sliderLength)
    {
        _data = data;

        img_Vip.enabled = data.config.vip_on == 1;


        if (data.config.vip_on == 1) //需要VIP
        {

            if (UserDataProxy.inst.playerData.isVip()) //我有vip
            {
                var cfg = VipLevelConfigManager.inst.GetConfig(UserDataProxy.inst.playerData.vipLevel);

                if (cfg.type_7 != 1 || cfg.type_8 != 1)
                {
                    gameObject.SetActiveFalse();
                    return;
                }
            }
            else
            {
                gameObject.SetActiveFalse();
                return;
            }
        }

        //icon.SetSprite(data.config.atlas_point, data.config.icon_point);
        var itemCfg = ItemconfigManager.inst.GetConfig(data.config.reward1_id);
        if (itemCfg != null)
        {
            icon.SetSprite(itemCfg.atlas, itemCfg.icon);
            tx_num.text = "x" + _data.config.reward1_num.ToString();
        }

        canGetVFXObj.SetActiveFalse();

        switch ((ActiveRewardBoxState)_data.state)
        {
            case ActiveRewardBoxState.dontGet:
                alreadyGetObj.SetActive(false);
                bg_icon.SetSprite("task_atlas", _data.config.vip_on == 1 ? "zhuejiemian_buffzhuti" : "zhuejiemian_gongnengdi2");
                bg_icon.iconImage.rectTransform.sizeDelta = new Vector2(111.3f,110.6f);
                GUIHelper.SetUIGrayColor(bg_icon.transform, 1);
                break;
            case ActiveRewardBoxState.canGet:
                alreadyGetObj.SetActive(false);
                bg_icon.SetSprite("task_atlas", "renwu_lvselingqu");
                bg_icon.iconImage.rectTransform.sizeDelta = new Vector2(136.8f, 133.6f);
                GUIHelper.SetUIGrayColor(bg_icon.transform, 1);
                //光效特效
                canGetVFXObj.SetActiveTrue();
                break;
            case ActiveRewardBoxState.alreadyGet:
                alreadyGetObj.SetActive(true);
                bg_icon.SetSprite("task_atlas", _data.config.vip_on == 1 ? "zhuejiemian_buffzhuti" : "zhuejiemian_gongnengdi2");
                bg_icon.iconImage.rectTransform.sizeDelta = new Vector2(111.3f, 110.6f);
                GUIHelper.SetUIGrayColor(bg_icon.transform, 0.5f);
                break;
        }

        needPointText.text = data.config.need_point.ToString();
        (transform as RectTransform).anchoredPosition = Vector2.right * (sliderLength * ((float)data.config.need_point / UserDataProxy.inst.task_activePointEnd));

        gameObject.SetActiveTrue();

    }

    private void onButtonClick()
    {
        switch ((ActiveRewardBoxState)_data.state)
        {
            case ActiveRewardBoxState.dontGet:
                //EventController.inst.TriggerEvent(GameEventType.TaskEvent.TASK_SHOW_LIVENESSBOXDESPANEL, transform as RectTransform, _data.active_task_id);
                var itemCfg = ItemconfigManager.inst.GetConfig(_data.config.reward1_id);
                if (itemCfg != null)
                {
                    EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, new CommonRewardData(itemCfg.id, _data.config.reward1_num, itemCfg.property, itemCfg.type), icon.transform);
                }
                break;
            case ActiveRewardBoxState.canGet:

                if (_data.config.vip_on == 1) //需要VIP
                {
                    if (UserDataProxy.inst.playerData.isVip()) //我有vip
                    {
                        var cfg = VipLevelConfigManager.inst.GetConfig(UserDataProxy.inst.playerData.vipLevel);

                        if (cfg.type_7 != 1 || cfg.type_8 != 1)
                        {
                            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("你的VIP已过期或等级不足，无法领取"), GUIHelper.GetColorByColorHex("FF2828"));
                            gameObject.SetActiveFalse();
                            return;
                        }
                    }
                    else
                    {
                        EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("你的VIP已过期或等级不足，无法领取"), GUIHelper.GetColorByColorHex("FF2828"));
                        gameObject.SetActiveFalse();
                        return;
                    }
                }

                EventController.inst.TriggerEvent<int>(GameEventType.TaskEvent.TASK_GETLIVENESSBOXAWARD, _data.active_task_id);
                break;
            case ActiveRewardBoxState.alreadyGet:
                break;
        }

    }


    public void Clear()
    {
        _data = null;
        gameObject.SetActiveFalse();
    }

}
