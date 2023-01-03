using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SuggestListItem : MonoBehaviour
{
    private string equipUid;
    private int recommend_energy;

    public GUIIcon itemBgIcon;
    public GUIIcon qualityIcon;
    public GUIIcon itemIcon;
    public Text itemNameTx;
    public GUIIcon subTypeIcon;
    public Text levelTx;
    public Text storeTx;            //库存
    public Text sellPriceText;      //当前价格
    public Text suggestEnergyText; //推荐能量

    public GameObject obj_superEquipSign;

    private void Awake()
    {
        var self = gameObject.GetComponent<Button>();
        self.onClick.AddListener(() =>
        {
            if (recommend_energy < UserDataProxy.inst.playerData.energy)
            {
                //改变需求列表
                if (onclickCallBack != null)
                    onclickCallBack.Invoke(equipUid);
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("能量不足"), GUIHelper.GetColorByColorHex("FF2828"));
            }
        });
    }

    private System.Action<string> onclickCallBack;
    public void setData(EquipItem item, System.Action<string> onclick)
    {
        onclickCallBack = onclick;
        this.equipUid = item.itemUid;
        itemNameTx.text = item.equipConfig.name;
        itemNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality - 1]);

        obj_superEquipSign.SetActive(item.quality > StaticConstants.SuperEquipBaseQuality);
        itemBgIcon.SetSprite("__common_1", item.quality > StaticConstants.SuperEquipBaseQuality ? "cktb_wupinkuang_super" : "cktb_wupinkuang");

        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(item.equipConfig.equipDrawingsConfig.sub_type);
        subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);
        levelTx.text = item.equipConfig.equipDrawingsConfig.level.ToString();
        storeTx.text = "x" + item.count.ToString();

        qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[item.quality - 1]);
        GUIHelper.showQualiyIcon(qualityIcon.GetComponent<RectTransform>(), item.quality);

        string qcolor = item.quality > 1 ? StaticConstants.qualityColor[item.quality - 1] : "";
        itemIcon.SetSprite(item.equipConfig.equipDrawingsConfig.atlas, item.equipConfig.equipDrawingsConfig.icon, qcolor);
        sellPriceText.text = item.sellPrice.ToString();
        recommend_energy = item.equipConfig.equipQualityConfig.recommend_energy;
        suggestEnergyText.text = $"-{recommend_energy}";
    }
}
