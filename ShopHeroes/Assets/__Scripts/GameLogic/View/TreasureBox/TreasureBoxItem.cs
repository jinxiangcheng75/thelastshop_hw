using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureBoxItem : MonoBehaviour
{
    public GUIIcon icon;
    public Text numText;
    public Text nameText;
    public Text keyNameText;
    public Text keyNumText;
    public GUIIcon keyIcon;
    public Button selfBtn;
    public Text bottomText;
    public Image marketImg;
    System.Action<int> clickAction;
    TreasureBoxData data;
    private void Awake()
    {
        if (selfBtn == null) return;
        selfBtn.onClick.AddListener(() =>
        {
            if (data.count > 0)
                clickAction?.Invoke(data.boxItemId);
            else
                EventController.inst.TriggerEvent(GameEventType.MarketCompEvent.MARKETUI_REQUIREDITEM, 1, data.boxItemId, 1, true);
        });
    }

    public void setData(TreasureBoxData box, System.Action<int> clickHandler)
    {
        data = box;
        clickAction = clickHandler;

        var boxCfg = ItemconfigManager.inst.GetConfig(box.boxItemId);
        var keyCfg = ItemconfigManager.inst.GetConfig(box.keyId);

        icon.SetSprite(boxCfg.atlas, boxCfg.icon);
        //GUIHelper.SetSingleUIGray(icon.transform, box.count <= 0);
        icon.iconImage.SetUIGrayColor(box.count <= 0 ? 0.5f : 1);
        keyIcon.SetSprite(keyCfg.atlas, keyCfg.icon);
        nameText.text = LanguageManager.inst.GetValueByKey(boxCfg.name);
        keyNameText.text = LanguageManager.inst.GetValueByKey(keyCfg.name);

        if (box.count > 0)
        {
            numText.enabled = true;
            marketImg.enabled = false;
            numText.text = "x" + data.count;
        }
        else
        {
            numText.enabled = false;
            marketImg.enabled = true;
        }

        keyNumText.text = data.keyCount.ToString();
        keyNumText.color = data.keyCount > 0 ? Color.white : GUIHelper.GetColorByColorHex("#FF5E5E");
        bottomText.text = box.count > 0 ? LanguageManager.inst.GetValueByKey("立即打开") : LanguageManager.inst.GetValueByKey("通过市场购买");
    }
}
