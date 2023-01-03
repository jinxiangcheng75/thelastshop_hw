using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//市场 您的列表 摊位
public class BoothItemComp : MonoBehaviour
{
    private kBoothStateType _state;
    private Button selfButton;
    private BoothItem _data;

    [Header("点击激活")]
    public GameObject addStateObj;
    [Header("已就绪")]
    public GameObject okStateObj;
    [Header("上架")]
    public GameObject boothStateObj;
    [Header("无变化")]
    public GameObject notChangeObj;
    public GUIIcon nc_itemBgIcon;
    public GUIIcon nc_itemIcon;
    public GUIIcon nc_qualityIcon;
    public Text nc_numText;
    public Text nc_typeText;
    public GUIIcon nc_arrowIcon;
    public Text nc_timeText;
    public Text nc_levelText;
    public GameObject nc_superEquipSignObj;
    [Header("有变化")]
    public GameObject hasChangedObj;
    public GameObject hc_leftUpperObj;
    public GUIIcon hc_leftUpperIcon;
    public GUIIcon hc_itemBgIcon;
    public GUIIcon hc_itemOrMoneyIcon;
    public GUIIcon hc_qualityIcon;
    public Text hc_exchangeNumText;
    public Text hc_typeText;
    public GUIIcon hc_arrowIcon;
    public GameObject hc_superEquipSignObj;
    [Header("红点")]
    public GameObject obj_redPoint;
    [Header("锁定状态")]
    public GameObject lockStateObj;
    LoopEventcomp loopEventcomp;
    float clickTimer;


    private void Awake()
    {
        selfButton = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        selfButton.ButtonClickTween(onButtonClick);
    }


    public void SetState(kBoothStateType state)
    {
        obj_redPoint.SetActive(false);

        _state = state;
        lockStateObj.SetActive(false);
        addStateObj.SetActive(false);
        okStateObj.SetActive(false);
        boothStateObj.SetActive(false);

        selfButton.interactable = (_state != kBoothStateType.Lock);

        switch (state)
        {
            case kBoothStateType.Lock:
                lockStateObj.SetActive(true);
                break;
            case kBoothStateType.Extension:
                addStateObj.SetActive(true);
                break;
            case kBoothStateType.OK:
                okStateObj.SetActive(true);
                break;
            case kBoothStateType.HasItem:
                boothStateObj.SetActive(true);
                break;
        }
    }

    private void Update()
    {
        if (_data != null && _data.exchangeNum > 0 && clickTimer < 1)
            clickTimer += Time.deltaTime;
    }

    public void SetData(BoothItem data)
    {
        SetState(kBoothStateType.HasItem);
        _data = data;

        bool isSelfSell = (kMarketItemType)data.marketType == kMarketItemType.selfSell; //报价

        notChangeObj.SetActive(data.exchangeNum <= 0);
        hasChangedObj.SetActive(data.exchangeNum > 0);

        obj_redPoint.SetActive(data.redPoint);

        if (data.exchangeNum > 0)  //有成功买或卖出去的
        {
            hc_typeText.text = LanguageManager.inst.GetValueByKey(isSelfSell ? "报价" : "请求");
            hc_arrowIcon.SetSprite("market_atlas", isSelfSell ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
            hc_arrowIcon.transform.localScale = new Vector3(isSelfSell ? -1 : 1, 1, 1);
            hc_leftUpperIcon.SetSprite("market_atlas", isSelfSell ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
            hc_leftUpperIcon.transform.localScale = new Vector3(isSelfSell ? -1 : 1, 1, 1);

            clickTimer = 1;

            hc_leftUpperObj.SetActive(isSelfSell);
            hc_exchangeNumText.text = "x" + (isSelfSell ? AbbreviationUtility.AbbreviateNumber(data.exchangeNum, 2) : data.exchangeNum.ToString());
            hc_qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[0]);
            GUIHelper.showQualiyIcon(hc_qualityIcon.GetComponent<RectTransform>(), 0, 160);
            if (isSelfSell)
            {
                hc_superEquipSignObj.SetActive(false);

                hc_leftUpperIcon.transform.localScale = Vector3.one;
                hc_itemOrMoneyIcon.SetSprite("__common_1", data.moneyType == 0 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");


                if (data.itemType == 0)
                {
                    EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(data.itemId, data.itemQuality);
                    hc_leftUpperIcon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon);

                }
                else
                {
                    itemConfig itemConfig = ItemconfigManager.inst.GetConfig(data.itemId);
                    hc_leftUpperIcon.SetSprite(itemConfig.atlas, itemConfig.icon);

                }
            }
            else
            {


                if (data.itemType == 0)
                {
                    EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(data.itemId, data.itemQuality);
                    hc_itemOrMoneyIcon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon);
                    hc_qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[data.itemQuality - 1]);
                    GUIHelper.showQualiyIcon(hc_qualityIcon.GetComponent<RectTransform>(), data.itemQuality, 160);

                    bool isSuperEquip = data.itemQuality > StaticConstants.SuperEquipBaseQuality;
                    hc_superEquipSignObj.SetActive(isSuperEquip);
                    //hc_itemBgIcon.SetSprite(isSuperEquip ? "__common_1" : "market_atlas", isSuperEquip ? "cktb_wupinkuang_super" : "shichang_zhongjianyuan1");

                }
                else
                {
                    itemConfig itemConfig = ItemconfigManager.inst.GetConfig(data.itemId);
                    hc_itemOrMoneyIcon.SetSprite(itemConfig.atlas, itemConfig.icon);

                    hc_superEquipSignObj.SetActive(false);
                    //hc_itemBgIcon.SetSprite("market_atlas", "shichang_zhongjianyuan1");

                }

            }

            //当有交易出去的时候  若当前打开的面板是变化的 请求关闭 摊位详情及 摊位取消界面
            EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_MARKETITEMCHANGED, data);
        }
        else
        {

            nc_typeText.text = LanguageManager.inst.GetValueByKey(isSelfSell ? "报价" : "请求");
            nc_arrowIcon.SetSprite("market_atlas", isSelfSell ? "zhuejiemian_chengshilv" : "shichang_lanjiantou");
            nc_arrowIcon.transform.localScale = new Vector3(isSelfSell ? -1 : 1, 1, 1);

            if (data.itemType == 0)
            {
                EquipConfig equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(data.itemId, data.itemQuality);
                nc_itemIcon.SetSprite(equipConfig.equipDrawingsConfig.atlas, equipConfig.equipDrawingsConfig.icon);
                nc_qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[data.itemQuality - 1]);
                GUIHelper.showQualiyIcon(nc_qualityIcon.GetComponent<RectTransform>(), data.itemQuality, 160);
                nc_levelText.text = equipConfig.equipDrawingsConfig.level.ToString();

                bool isSuperEquip = data.itemQuality > StaticConstants.SuperEquipBaseQuality;

                nc_itemBgIcon.SetSprite(isSuperEquip ? "__common_1" : "market_atlas", isSuperEquip ? "cktb_wupinkuang_super" : "shichang_zhongjianyuan");
                nc_superEquipSignObj.SetActive(isSuperEquip);
            }
            else
            {
                itemConfig itemConfig = ItemconfigManager.inst.GetConfig(data.itemId);
                nc_itemIcon.SetSprite(itemConfig.atlas, itemConfig.icon);
                nc_qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[data.itemQuality - 1]);
                GUIHelper.showQualiyIcon(hc_qualityIcon.GetComponent<RectTransform>(), data.itemQuality, 160);
                nc_levelText.text = 1.ToString();

                nc_itemBgIcon.SetSprite("market_atlas", "shichang_zhongjianyuan");
                nc_superEquipSignObj.SetActive(false);
            }

            nc_numText.text = "x" + data.remainNum;

            if ((int)data.remainTime <= 0)
            {
                obj_redPoint.SetActive(true);
                nc_timeText.text = LanguageManager.inst.GetValueByKey("已过期");
                nc_timeText.color = GUIHelper.GetColorByColorHex("FD4F4F");
                clearTimer();

                //当有过期的时候  若当前打开的面板是变化的 请求关闭摊位取消界面及刷新摊位详情面板
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_MARKETITEMPASTTIME, data);
            }
            else
            {
                obj_redPoint.SetActive(false);
                nc_timeText.text = TimeUtils.timeSpanStrip((int)data.remainTime - 1);
                nc_timeText.color = GUIHelper.GetColorByColorHex("F0B331");
                setTimer();
            }


            //nc_typeIcon.SetSprite("", "");
        }

    }

    private void setTimer()
    {
        clearTimer();

        loopEventcomp = GameTimer.inst.AddLoopTimerComp(gameObject, 1, () =>
        {
            if (_data.remainTime <= 0)
            {
                clearTimer();
                return;
            }
            nc_timeText.text = TimeUtils.timeSpanStrip((int)_data.remainTime - 1);
        }, (int)_data.remainTime);

    }

    private void clearTimer()
    {
        if (loopEventcomp != null)
        {
            GameTimer.inst.removeLoopTimer(loopEventcomp);
            loopEventcomp = null;
        }
    }

    private void onButtonClick()
    {


        switch (_state)
        {
            case kBoothStateType.Lock:
                break;
            case kBoothStateType.Extension:
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETBUYBOOTHUI);
                break;
            case kBoothStateType.OK:
                //EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.HIDEUI_MARKETUI);
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_BOOTHCREATELISTUI);
                break;
            case kBoothStateType.HasItem:

                //有请求或售卖掉的情况

                if (_data.exchangeNum > 0)
                {
                    //发送收取的消息请求
                    if (clickTimer >= 1)
                    {
                        EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKET_BOOTHITEMDEALWITH, _data.boothField);
                        clickTimer = 0;
                    }
                }
                else
                {
                    //查看摊位信息
                    EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_BOOTHITEMINFOUI, _data);
                }
                break;
        }

    }

}
