using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfInventoryItem : MonoBehaviour
{
    public GUIIcon itemBgIcon;
    public GUIIcon qualityIcon;
    public GUIIcon itemIcon;
    public Text itemNameTx;
    public Text levelTx;
    public Text storeTx;            //库存
    public Toggle lockToggle;
    public GameObject sotreObj;
    public Button infoBtn;
    public GameObject lvObj;
    public Text clickToLookTx;

    public GameObject obj_superEquipSign;

    EquipItem equipItem;
    private bool isExchange;
    private int[] _equipTypes;
    private int _shelfUid;

    public Animator btnAnimator;

    private void Awake()
    {
        var self = gameObject.GetComponent<Button>();
        self.onClick.AddListener(() =>
        {
            if (isExchange)
            {
                if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(101).parameters)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(101).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                }
                else
                {
                    //打开交易所
                    EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.SHOWUI_MARKETINVENTORYUI, _equipTypes, MarketEquipLvManager.inst.GetCurMarketLevel(), true, (int)MarketInventoryFromType.byShelf);
                }
            }
            else
            {
                //货架上摆放装备
                EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.SHELFUPGRADE_PUTONEQUIP, equipItem.itemUid, _shelfUid);
            }
        });


        infoBtn.onClick.AddListener(() =>
            {
                if (equipItem != null)
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPITEMUI, equipItem.itemUid, 0, new List<EquipItem>());
            }
        );

        lockToggle.onValueChanged.AddListener((ison) =>
        {
            //锁定
            if (ison != equipItem.isLock)
            {
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_LOCK, equipItem.itemUid, ison);
            }
        });


    }

    public void setData(EquipItem item, int index, int shelfUid, bool needShowAni)
    {
        if (item.count - item.onShelfCount <= 0)
        {
            gameObject.SetActive(false);
            return;
        }

        isShowAnimed = false;

        _shelfUid = shelfUid;
        equipItem = item;
        isExchange = false;

        lockToggle.gameObject.SetActive(true);
        qualityIcon.gameObject.SetActive(true);
        clickToLookTx.gameObject.SetActive(false);
        infoBtn.gameObject.SetActive(true);
        lvObj.gameObject.SetActive(true);
        sotreObj.gameObject.SetActive(true);

        lockToggle.isOn = equipItem.isLock;
        itemNameTx.text = item.equipConfig.name;
        itemNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality - 1]);
        levelTx.text = item.equipConfig.equipDrawingsConfig.level.ToString();
        storeTx.text = "x" + (item.count - item.onShelfCount).ToString();
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[item.quality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), item.quality);
        itemIcon.SetSprite(item.equipConfig.equipDrawingsConfig.atlas, item.equipConfig.equipDrawingsConfig.icon);

        itemBgIcon.SetSprite("__common_1", item.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");
        obj_superEquipSign.SetActive(item.quality > StaticConstants.SuperEquipBaseQuality);

        if (index < 9 && needShowAni) showAnim(index); //前9个
    }

    public void SetExchangeBtn(int[] equipTypes, bool needShowAni)
    {
        isShowAnimed = false;


        isExchange = true;
        _equipTypes = equipTypes;
        qualityIcon.gameObject.SetActive(false);
        lockToggle.gameObject.SetActive(false);
        infoBtn.gameObject.SetActive(false);
        lvObj.gameObject.SetActive(false);
        sotreObj.gameObject.SetActive(false);
        itemIcon.SetSprite("__common_1", "shichang_jiaoyitubiao");
        itemNameTx.text = LanguageManager.inst.GetValueByKey("交易所");
        itemNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[0]);
        clickToLookTx.gameObject.SetActive(true);
        itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");
        obj_superEquipSign.SetActive(false);

        if (needShowAni) showAnim(0); //前9个
    }

    bool isShowAnimed;

    void showAnim(int index)
    {
        if (isShowAnimed) return;
        isShowAnimed = true;

        gameObject.SetActive(false);

        GameTimer.inst.AddTimer(0.28f + 0.02f * index, 1, () =>
        {
            gameObject.SetActive(true);
            btnAnimator.CrossFade("show", 0f);
            btnAnimator.Update(0f);
            btnAnimator.Play("show");
        });

    }
}
