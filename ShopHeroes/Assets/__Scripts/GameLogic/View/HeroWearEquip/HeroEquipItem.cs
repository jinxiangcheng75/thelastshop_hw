using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroEquipItem : MonoBehaviour
{
    private string equipUid;
    public GUIIcon qualityIcon;
    public GUIIcon itemIcon;
    public Text itemNameTx;
    public Text levelTx;
    public Text storeTx;            //库存
    public GameObject marketObj;
    public GameObject levelObj;
    public GameObject numObj;
    private bool isMarket;
    public Button infoBtn;
    public List<RoleEquipItem> allProperty;
    public GameObject allPropertyObj;
    public Text abrasionText;
    public GameObject abrasionObj;

    public GameObject superNormalObj;
    public GUIIcon Iconbg;

    private int[] _equipTypes;
    private int maxLevel;
    private int equipFieldId;
    private int heroUid;
    private void Awake()
    {
        var self = gameObject.GetComponent<Button>();
        self.onClick.AddListener(() =>
        {
            //改变需求列表
            if (isMarket)
            {
                //打开交易所
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(101).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁", WorldParConfigManager.inst.GetConfig(101).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
                MarketDataProxy.inst.Payload = "1|" + heroUid + "|" + equipFieldId;
                int max = maxLevel;
                if (max > MarketEquipLvManager.inst.GetCurMarketLevel())
                {
                    max = MarketEquipLvManager.inst.GetCurMarketLevel();
                }
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETINVENTORYUI, _equipTypes, max, false, (int)MarketInventoryFromType.byHeroWearEquip);
            }
            else
            {
                onclickCallBack.Invoke(equipUid);
            }

        });

        infoBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, equipUid, 0, new List<EquipItem>());
        });
    }

    private System.Action<string> onclickCallBack;
    public void setData(EquipItem item, System.Action<string> onclick)
    {
        gameObject.name = item.equipid.ToString();

        onclickCallBack = onclick;
        this.equipUid = item.itemUid;
        isMarket = false;

        qualityIcon.gameObject.SetActiveTrue();
        marketObj.SetActiveFalse();
        infoBtn.gameObject.SetActiveTrue();
        levelObj.SetActiveTrue();
        numObj.SetActiveTrue();
        allPropertyObj.SetActive(true);
        abrasionObj.SetActive(true);

        itemNameTx.text = item.equipConfig.name;
        itemNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality - 1]);
        levelTx.text = item.equipConfig.equipDrawingsConfig.level.ToString();
        storeTx.text = "x" + item.count.ToString();
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[item.quality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), item.quality);
        itemIcon.SetSprite(item.equipConfig.equipDrawingsConfig.atlas, item.equipConfig.equipDrawingsConfig.icon);

        abrasionText.text = RoleDataProxy.inst.GetEquipDamagerVal(item.equipConfig.equipQualityConfig.quality) / 100.0f + "%";

        superNormalObj.SetActive(item.quality > StaticConstants.SuperEquipBaseQuality);
        Iconbg.SetSprite("__common_1", item.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        setEquipAttributeInfo(item);
    }

    private void setEquipAttributeInfo(EquipItem item)
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(item.equipid);
        for (int i = 0; i < allProperty.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                allProperty[index].gameObject.SetActive(true);
                allProperty[index].valText.text = propertyList[index].ToString();
            }
            else
            {
                allProperty[index].gameObject.SetActive(false);
            }
        }
    }

    public void setMarketDara(int[] equipTypes, int maxLevel, int _equipFieldId, int _heroUid)
    {
        gameObject.name = "market";
        this.maxLevel = maxLevel;
        equipFieldId = _equipFieldId;
        heroUid = _heroUid;
        _equipTypes = equipTypes;
        isMarket = true;

        superNormalObj.SetActive(false);
        qualityIcon.gameObject.SetActiveFalse();
        marketObj.SetActiveTrue();
        infoBtn.gameObject.SetActiveFalse();
        levelObj.SetActiveFalse();
        numObj.SetActiveFalse();
        allPropertyObj.SetActive(false);
        abrasionObj.SetActive(false);

        itemNameTx.text = LanguageManager.inst.GetValueByKey("交易所");
        itemNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);
        itemIcon.SetSprite("__common_1", "shichang_jiaoyitubiao"/*, needSetNativeSize: true*/);
    }
}
