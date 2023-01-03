using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExploreIntensifyItem : MonoBehaviour
{
    public Button selfBtn;
    public Button infoBtn;
    public GUIIcon icon;
    public Text nameText;
    public Text numText;
    public Text desText;

    private Item data;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            if (data == null) return;
            if (data.count > 0)
                onclickCallBack?.Invoke(data.itemConfig.id);
            else
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 30);
            }
        });

        infoBtn.onClick.AddListener(() =>
        {
            if (data == null) return;
            EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_SHOW_ITEMINFO, data.itemConfig.id);
        });
    }
    private System.Action<int> onclickCallBack;
    public void setData(Item itemData, System.Action<int> onclick)
    {
        if (itemData == null) return;
        onclickCallBack = onclick;
        data = itemData;
        icon.SetSprite(data.itemConfig.atlas, data.itemConfig.icon);
        nameText.text = LanguageManager.inst.GetValueByKey(data.itemConfig.name);
        numText.text = data.count.ToString();
        numText.color = data.count > 0 ? Color.white : Color.red;
        desText.text = LanguageManager.inst.GetValueByKey(data.itemConfig.desc);
    }
}
