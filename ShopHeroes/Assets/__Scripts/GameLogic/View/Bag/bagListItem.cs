using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class bagListItem : MonoBehaviour
{
    private string uid;
    private int id;  //装备id 
    private int type = 0; //0=装备 1=特殊资源 2=其他
    public GUIIcon itemBgIcon;
    public GUIIcon qualityIcon;
    public GUIIcon itemIcon;
    public GUIIcon equipSubTypeIcon;
    public Button infoBtn;
    public Toggle lockToggle;
    public Text itemNameTx;
    public Text typeText;
    public Text levelTx;
    public Text storeTx; //库存
    public Text sellPriceText;
    public Text referralText;
    public Text boxBottomText;
    public GUIIcon boxKeyIcon;
    public Text boxKeyNumText;
    public Image marketImg;

    public Image e_0_1;
    public Image e_0_2;

    public GameObject obj_superEquipSign;

    ItemType curType;

    public Animator btnAnimator;

    public System.Action<string, int> itemOnclick = null;
    private void Awake()
    {
        var self = gameObject.GetComponent<Button>();
        self.ButtonClickTween(() =>
        {
            onselfclick(0);
        });
        infoBtn.onClick.AddListener(() =>
        {
            onselfclick(1);
        });

        lockToggle.onValueChanged.AddListener((ison) =>
        {
            //锁定
            if (ison != currEquip.isLock)
            {
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_LOCK, currEquip.itemUid, ison);
            }
        });

    }

    // fromType 0 - item本身 1 - infoBtn
    public void onselfclick(int fromType)
    {
        if (typeState == 0)
        {
            if (itemOnclick != null)
            {
                itemOnclick.Invoke(uid, fromType);
            }
        }
        else if (typeState == 1 || typeState == 2)
        {
            //
            if (itemOnclick != null)
            {
                itemOnclick.Invoke(id.ToString(), fromType);
            }
        }
    }
    public int typeState
    {
        get { return type; }
        set
        {
            type = value;
            if (type == 0)
            {
                e_0_1.enabled = true;
                e_0_2.enabled = true;
                levelTx.gameObject.SetActive(true);
                sellPriceText.gameObject.SetActive(true);
                lockToggle.gameObject.SetActive(true);
                infoBtn.gameObject.SetActive(true);
                referralText.gameObject.SetActive(false);
                boxBottomText.enabled = false;
                boxKeyNumText.enabled = false;
                boxKeyIcon.iconImage.enabled = false;

                typeText.text = LanguageManager.inst.GetValueByKey("价值");
                typeText.enabled = true;
                equipSubTypeIcon.iconImage.enabled = true;
                obj_superEquipSign.SetActive(currEquip.quality > StaticConstants.SuperEquipBaseQuality);
                itemBgIcon.SetSprite("__common_1", currEquip.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

            }
            else if (type == 1)
            {
                e_0_1.enabled = false;
                e_0_2.enabled = false;
                levelTx.gameObject.SetActive(false);
                sellPriceText.gameObject.SetActive(false);
                lockToggle.gameObject.SetActive(false);
                //infoBtn.gameObject.SetActive(false);
                referralText.gameObject.SetActive(true);
                boxBottomText.enabled = false;
                boxKeyNumText.enabled = false;
                boxKeyIcon.iconImage.enabled = false;

                typeText.enabled = false;
                equipSubTypeIcon.iconImage.enabled = false;
                obj_superEquipSign.SetActive(false);
                itemBgIcon.SetSprite("__common_1", "cktb_wupinkuang");

            }
            else if (type == 2)
            {
                e_0_1.enabled = false;
                e_0_2.enabled = false;
                levelTx.gameObject.SetActive(false);
                sellPriceText.gameObject.SetActive(false);
                lockToggle.gameObject.SetActive(false);
                //infoBtn.gameObject.SetActive(false);
                equipSubTypeIcon.iconImage.enabled = false;
                obj_superEquipSign.SetActive(false);
                itemBgIcon.SetSprite("__common_1","cktb_wupinkuang");

                if (curType == ItemType.Box)
                {
                    referralText.gameObject.SetActive(false);
                    boxBottomText.enabled = true;
                    boxKeyNumText.enabled = true;
                    boxKeyIcon.iconImage.enabled = true;

                    typeText.enabled = true;
                }
                else
                {
                    referralText.gameObject.SetActive(true);
                    boxBottomText.enabled = false;
                    boxKeyNumText.enabled = false;
                    boxKeyIcon.iconImage.enabled = false;

                    typeText.enabled = false;
                }
            }
        }
    }
    EquipItem currEquip;
    //装备
    public void setData(EquipItem item, int index, bool needShowAni)
    {
        isShowAnimed = false;

        currEquip = item;
        uid = item.itemUid;
        id = item.ID;
        typeState = 0;
        itemNameTx.rectTransform.anchoredPosition = new Vector2(-12f + equipSubTypeIcon.iconImage.rectTransform.sizeDelta.x / 2, 24);
        itemNameTx.text = item.equipConfig.name;
        itemNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality - 1]);
        levelTx.text = item.equipConfig.equipDrawingsConfig.level.ToString();
        storeTx.enabled = true;
        marketImg.enabled = false;
        storeTx.text = "x" + item.count.ToString();
        lockToggle.isOn = item.isLock;
        qualityIcon.gameObject.SetActive(true);
        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[item.quality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), item.quality);
        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(item.equipConfig.equipDrawingsConfig.sub_type);
        equipSubTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);
        equipSubTypeIcon.iconImage.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality - 1]);
        string qcolor = item.quality > 1 ? StaticConstants.qualityColor[item.quality - 1] : "";

        itemIcon.iconImage.SetUIGrayColor(1);
        itemIcon.SetSprite(item.equipConfig.equipDrawingsConfig.atlas, item.equipConfig.equipDrawingsConfig.icon, qcolor);
        // GUIHelper.SetUIGray(itemIcon.transform, false);

        sellPriceText.text = item.sellPrice.ToString();
        //Logger.log("设置背包道具信息:" + item.equipConfig.equipDrawingsConfig.name);


        if (index < 9 && needShowAni) showAnim(index); //前9个

    }

    //资源
    public void setData(Item item, int index, bool needShowAni)
    {
        isShowAnimed = false;


        id = item.ID;
        typeState = 1;
        itemNameTx.rectTransform.anchoredPosition = new Vector2(-6, 24);
        itemNameTx.text = LanguageManager.inst.GetValueByKey(item.itemConfig.name);
        itemNameTx.color = Color.white;
        itemIcon.iconImage.SetUIGrayColor(1);
        itemIcon.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);
        referralText.text = LanguageManager.inst.GetValueByKey(item.itemConfig.desc);
        storeTx.enabled = true;
        marketImg.enabled = false;
        storeTx.text = "x" + item.count.ToString();
        qualityIcon.gameObject.SetActive(false);

        if (index < 9 && needShowAni) showAnim(index); //前9个
    }

    //其他
    public void setOtherData(Item item, int index, bool needShowAni)
    {
        isShowAnimed = false;

        curType = (ItemType)item.itemConfig.type;

        id = item.ID;
        typeState = 2;
        itemNameTx.rectTransform.anchoredPosition = new Vector2(-6, 24);
        itemNameTx.text = LanguageManager.inst.GetValueByKey(item.itemConfig.name);
        itemNameTx.color = Color.white;
        itemIcon.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);
        referralText.text = LanguageManager.inst.GetValueByKey(item.itemConfig.desc);
        storeTx.enabled = true;
        marketImg.enabled = false;
        storeTx.text = "x" + item.count.ToString();
        qualityIcon.gameObject.SetActive(false);
        itemIcon.iconImage.SetUIGrayColor(1);

        if (curType == ItemType.Box)
        {
            if (item.count <= 0)
            {
                marketImg.enabled = true;
                storeTx.enabled = false;
                itemIcon.iconImage.SetUIGrayColor(0.5f);
            }
            var keyCfg = ItemBagProxy.inst.GetItem(item.ID + StaticConstants.tboxAndKeyOffset);
            typeText.enabled = true;
            typeText.text = LanguageManager.inst.GetValueByKey(keyCfg.itemConfig.name);
            boxKeyNumText.text = keyCfg.count.ToString();
            boxKeyNumText.color = keyCfg.count > 0 ? Color.white : GUIHelper.GetColorByColorHex("#FF5E5E");
            boxKeyIcon.SetSprite(keyCfg.itemConfig.atlas, keyCfg.itemConfig.icon);
            boxBottomText.text = item.count > 0 ? LanguageManager.inst.GetValueByKey("立即打开") : LanguageManager.inst.GetValueByKey("通过市场购买");
        }

        if (index < 9 && needShowAni) showAnim(index); //前9个
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
